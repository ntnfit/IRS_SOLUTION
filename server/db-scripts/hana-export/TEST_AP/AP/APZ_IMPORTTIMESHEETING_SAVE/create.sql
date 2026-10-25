
CREATE PROCEDURE APZ_ImportTimeSheeting_Save
AS
BEGIN
	DECLARE Period nvarchar(400000);
	DECLARE Branch int;
	DECLARE HourPerDay int;
	DECLARE maxLineNum int;
	declare afterLoop int = 1;
	declare beforeLoop int = 0;
	declare dataIndex int = 1;
	DECLARE maxLineNumOT int;
	
	create local temporary table #tempTSL1 as (select top 0 * from APZ_TSL1);
	exec 'drop table #tempTSL1';
	exec 'create local temporary table #tempTSL1 as (select * from "#dtAPZ_TSL1")';
	
	create local temporary table #tempTSH1 as (select top 0 * from APZ_TSH1);
	exec 'drop table #tempTSH1';
	exec 'create local temporary table #tempTSH1 as (select * from "#dtAPZ_TSH1")';
	
	select IFNULL((SELECT TOP 1 "Period" from #tempTSL1),'') into Period from DUMMy;
	IF :Period=''
	THEN
		select ifNULL((SELECT TOP 1 "Period" from #tempTSH1),'') into Period from dummy;
	END IF;
	
	-- Lấy chi nhánh tương ứng. Do chấm công là theo chi nhánh nên sẽ lấy chi nhánh theo 1 nhân viên bất kì
	select ifNULL((SELECT TOP 1 "branch" from OHEM where "empID" = IFNULL((select top 1 "EmpId" from #tempTSL1),0)),0) into Branch from dummy;
	-- Lấy ra só giờ trong 1 ngày đã khai báo trong kì. Do các kì có thể trùng Code và khác chi nhánh, nên là phải lấy thông tin chi nhánh từ nhân viên import
	select IFNULL((select TOP 1 "HourPerDay" from APZ_OHWP where "PeriodCode" = :Period and "Branch" = :Branch),8) into HourPerDay from DUMMY;
	
	--Time Sheeting
	 DELETE FROM  APZ_TSL1 a where exists(select 1 from #tempTSL1 b where a."EmpId"=b."EmpId" AND a."Period"=b."Period" AND a."WorkDay"=b."WorkDay");
	 SELECT IFNULL(MAX("LineNum"),0) into maxLineNum FROM APZ_TSL1;
	
	 --create temp table for TSL1
	create local temporary table #InsertTableTSL1 ("Period" nvarchar(50), "EmpId" int, "LineNum" int, "WorkDay" timestamp, "WorkCode" nvarchar(50), "InTime" numeric(19,6), "OutTime" numeric(19,6), "WorkTotal" numeric(19,6), "FreeTxt" nvarchar(254));
	
	insert into #InsertTableTSL1
	  Select :Period "Period" ,a."EmpId" ,ROW_NUMBER() OVER (PARTITION BY a."EmpId" ORDER BY a."EmpId" ASC) + :maxLineNum as "LineNum",a."WorkDay"
	  						, GetStringSplitIndex(a."WorkCode",',', :dataIndex) "WorkCode",a."InTime",a."OutTime",null, a."FreeTxt" from #tempTSL1 a where GetStringSplitIndex(a."WorkCode",',', :dataIndex) <> '';

	while :afterLoop > :beforeLoop
	  DO
	  	dataIndex = :dataIndex + 1;
	  	select ifnull(count(1),0) into beforeLoop from #InsertTableTSL1;
	  	SELECT IFNULL(MAX("LineNum"),0) into maxLineNum FROM #InsertTableTSL1;
	  	insert into #InsertTableTSL1 
	  		Select :Period "Period" ,a."EmpId" ,ROW_NUMBER() OVER (PARTITION BY a."EmpId" ORDER BY a."EmpId" ASC) + :maxLineNum as "LineNum",a."WorkDay"
	  					, GetStringSplitIndex(a."WorkCode",',', :dataIndex) "WorkCode",a."InTime",a."OutTime",cast(null as numeric(19,6)) "WorkTotal", a."FreeTxt" from #tempTSL1 a where GetStringSplitIndex(a."WorkCode",',', :dataIndex) <> '';
	  	select ifnull(count(1),0) into afterLoop from #InsertTableTSL1;
	END WHILE;
	  
	delete from #InsertTableTSL1 a where not exists(select 1 from APZ_OHWC b where a."WorkCode"=b."Code");
	update a set "WorkTotal" = b."Rate" from #InsertTableTSL1 a inner join APZ_OHWC b on a."WorkCode"=b."Code";
	
	--save to TSL1
	Insert into  APZ_TSL1("Period","EmpId" ,"LineNum","WorkDay","WorkCode","InTime","OutTime","WorkTotal","FreeTxt") 
	Select * from #InsertTableTSL1;
	
	  --create temp table for OTSL
	create local temporary table #dtInsert ("period" nvarchar(50),"empid" int, "Typ" nvarchar(50), "worktype" numeric(19,6));
	insert into #dtInsert
	  select "Period","EmpId",c."Typ",sum(IFNULL("WorkTotal",0))
			from APZ_TSL1 a INNER JOIN APZ_OHWC b on a."WorkCode"=b."Code" LEFT JOIN APZ_OHWT c ON b."WorkType"=c."Id"
			WHERE "Period"=:Period group by "Period","EmpId",c."Typ";
	
	-- Lấy ra các thông số tổng hợp các loại công
	create local temporary table #InsertTableOTSL as(SELECT "period","empid"
			, (select Max("worktype") from #dtInsert where "Typ" = 'W' and "period" = a."period" and "empid" = a."empid") "WorkSum"
			, (select Max("worktype") from #dtInsert where "Typ" = 'A' and "period" = a."period" and "empid" = a."empid") "AbsentSum"
			, (select Max("worktype") from #dtInsert where "Typ" = 'L' and "period" = a."period" and "empid" = a."empid") "LeaveSum"
			, cast(null as numeric(19,6)) as "WorkTotal"
			, (select Max("worktype") from #dtInsert where "Typ" = 'O' and "period" = a."period" and "empid" = a."empid") "UsedOT"
			from #dtInsert a group by "period","empid");
			
	update #InsertTableOTSL set "WorkTotal" = IFNULL("WorkSum",0)+IFNULL("AbsentSum",0)+IFNULL("LeaveSum",0);
	
	
	
	  --save to OTSL
	update a set "TotalBefore"=IFNULL(b."WorkTotal",0),"WorkSum"=b."WorkSum","AbsentSum"=b."AbsentSum","LeaveSum"=b."LeaveSum", "WorkTotal"=IFNULL(b."WorkTotal",0)+IFNULL("Extra",0)
		 from APZ_OTSL a inner join (select "period",x."empid",y."lastName" || ', ' || y."firstName" as "EmpName",x."WorkSum",x."AbsentSum",x."LeaveSum","WorkTotal" 
										from #InsertTableOTSL x INNER JOIN OHEM y on x."empid"=y."empID") b ON a."Period"=b."period" and a."EmpId"=b."empid";

	insert into APZ_OTSL("Period","EmpId","EmpName","TotalBefore","WorkSum","AbsentSum","LeaveSum","WorkTotal")
		select x."period",x."empid",y."lastName" || ', ' || y."firstName",IFNULL(x."WorkTotal",0),x."WorkSum",x."AbsentSum",x."LeaveSum",IFNULL(x."WorkTotal",0) 
		from #InsertTableOTSL x INNER JOIN OHEM y on x."empid"=y."empID"
			where not exists(select 1 from APZ_OTSL a where a."Period"=x."period" and a."EmpId"=y."empID");
	
	--Lấy ra bảng tổng hợp
	create local temporary table #tbl ("EmpId" int, "EmpName" nvarchar(254),"Period" nvarchar(50),"TotalBefore" numeric(19,6), "WorkSum" numeric(19,6),"AbsentSum" numeric(19,6), "LeaveSum" numeric(19,6)
				,"WorkQuota" numeric(19,6), "DocStatus" nvarchar(50),"Extra" numeric(19,6), "WorkTotal" numeric(19,6), "LeaveTotal" numeric(19,6), "Remain" numeric(19,6), "TotalOT" numeric(19,6)
				, "UsedOT" numeric(19,6), "RemainOT" numeric(19,6));
	
	insert into #tbl
		select b."EmpId",b."EmpName",b."Period",b."TotalBefore",b."WorkSum",b."AbsentSum",b."LeaveSum",b."WorkQuota",b."DocStatus",b."Extra",b."WorkTotal",d."LeaveTotal", d."Remain", e."CompLeaveTotal", e."Used", e."Remain" 
		 from APZ_OTSL b 
			LEFT JOIN APZ_TSL2 d ON b."EmpId"=d."EmpId"	AND d."Period"=:Period
			LEFT JOIN APZ_TSL3 e ON b."EmpId"=e."EmpId" AND e."Period"=:Period where b."Period" = :Period and b."EmpId" in (select DISTINCT "EmpId" from #tempTSL1);
	
	update a set "UsedOT" = b."UsedOT"*:HourPerDay from #tbl a inner join (select "period",x."empid",y."lastName" ||', '||y."firstName" as "EmpName",x."WorkSum",x."AbsentSum",x."LeaveSum","WorkTotal" ,x."UsedOT"
												from #InsertTableOTSL x INNER JOIN OHEM y on x."empid"=y."empID" ) b ON a."Period"=b."period" and a."EmpId"=b."empid";
	
	--Luu bang thong tin phep 
	update a set "LeaveTotal" = b."LeaveTotal","Used" = b."LeaveSum","Remain" = b."LeaveTotal" - b."LeaveSum" from APZ_TSL2 a inner join #tbl b on a."EmpId" = b."EmpId" and a."Period" =b."Period";
	insert into APZ_TSL2("Period","EmpId","EmpName","LeaveTotal","Used","Remain")
		select b."Period",b."EmpId",b."EmpName",IFNULL(b."LeaveTotal",0),IFNULL(b."LeaveSum",0),IFNULL(b."LeaveTotal",0) - IFNULL(b."LeaveSum",0) from #tbl b
							where not exists(select 1 from APZ_TSL2 a where a."EmpId" = b."EmpId" and a."Period" =b."Period");
	
	--Luu bang  thong tin nghi bu
	update a set "CompLeaveTotal" = b."TotalOT","Used" = b."UsedOT","Remain" = b."TotalOT" - b."UsedOT" from APZ_TSL3 a inner join #tbl b on a."EmpId" = b."EmpId" and a."Period" =b."Period";
	insert into APZ_TSL3("Period","EmpId","EmpName","CompLeaveTotal","Used","Remain")
		select b."Period",b."EmpId",b."EmpName",IFNULL(b."TotalOT",0),IFNULL(b."UsedOT",0),IFNULL(b."TotalOT",0) - IFNULL(b."UsedOT",0) from #tbl b 
						where not exists(select 1 from APZ_TSL3 a where a."EmpId" = b."EmpId" and a."Period" =b."Period");

	--Time Sheeting Overtime
	DELETE FROM  APZ_TSH1 a where exists(select 1 from #tempTSH1 b where a."EmpId"=b."EmpId" AND a."Period"=b."Period" AND a."WorkDay"=b."WorkDay" and a."OvertimeType"=b."OvertimeType");
	
	SELECT ifnull(MAX("LineNum"),0) into maxLineNumOT FROM APZ_TSH1;
	
	--save to TSH1
	Insert into  APZ_TSH1("Period","EmpId" ,"LineNum","WorkDay","OvertimeType","OvertimeHour","FreeTxt") 
		Select :Period "Period",a."EmpId", ROW_NUMBER() OVER (PARTITION BY a."EmpId" ORDER BY a."EmpId" ASC)+ :maxLineNumOT as "LineNum",a."WorkDay",
				  a."OvertimeType",a."OvertimeHour",a."FreeTxt" from #tempTSH1 a WHERE a."OvertimeType"<>'';
	
	  --save to OTSH
	update a set "OvertimeType"=b."OvertimeType","TotalBefore"=IFNULL(b."OvertimeHour",0), "WorkTotal"=IFNULL(b."OvertimeHour",0)+IFNULL("Extra",0)
		 from APZ_OTSH a inner join (select "Period",x."EmpId",y."lastName" ||', '|| y."firstName" as "EmpName","OvertimeType",sum(IFNULL("OvertimeHour",0)) "OvertimeHour" 
												from APZ_TSH1  x INNER JOIN OHEM y on x."EmpId"=y."empID"
									WHERE "Period"=:Period group by "Period",x."EmpId",y."lastName",y."firstName","OvertimeType") b ON a."Period"=b."Period" and a."EmpId"=b."EmpId" and a."OvertimeType"=b."OvertimeType";
	
	insert into APZ_OTSH("Period","EmpId","EmpName","OvertimeType","TotalBefore","WorkTotal")
		select "Period",x."EmpId",y."lastName" ||', '|| y."firstName","OvertimeType",sum(IFNULL("OvertimeHour",0)), sum(IFNULL("OvertimeHour",0))
												from APZ_TSH1  x INNER JOIN OHEM y on x."EmpId"=y."empID" 
									WHERE "Period"=:Period and not exists(select 1 from APZ_OTSH a where a."Period"=x."Period" and a."EmpId"=x."EmpId" and a."OvertimeType"=x."OvertimeType") 
								group by "Period",x."EmpId",y."lastName",y."firstName","OvertimeType";
END
 




