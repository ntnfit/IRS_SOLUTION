
CALL APZ_EXISTSPROC('APZ_TimeSheeting_Load');

GO

CREATE PROCEDURE APZ_TimeSheeting_Load(
	in Branch INT default -1,
	in Dept VARCHAR(254) default '',
	in Emp VARCHAR(254) default '',
	in EmpName NVARCHAR(254) default '',
	in SStatus varchar(254) default '',
	in Period VARCHAR(20) default '',
	in BaseEntry INT default 0)
AS
BEGIN

	DECLARE err_message nvarchar(254);
	DECLARE StartDate timestamp = null;
	DECLARE EndDate timestamp = null;
	DECLARE WorkQuota NUMERIC(19,6) = 0;
	DECLARE daterage VARCHAR(400000)='';
	DECLARE sqlString nvarchar(4000000) = '';
	DECLARE Udfcols NVARCHAR(40000) = '';
	declare i int = 0;
	DECLARE chk_emp int = 0;

  	CREATE local temporary table #temp_NhanVien AS (
  		SELECT top 0 a."empID", a."ExtEmpNo", a."firstName", a."lastName", a."dept", b."Name" AS "Department", a."startDate", a."termDate"
  		FROM OHEM a LEFT OUTER JOIN OUDP b ON a."dept" = b."Code" Where a."Active" = 'Y');
  	
  	--select * from APZ_OHWP
	select ifNULL((SELECT a."FromDate" FROM APZ_OHWP a WHERE a."PeriodCode"=:Period AND (a."Branch"=:Branch)),'1900-01-01') into StartDate FROM DUMMY;
	select IFNULL((SELECT a."ToDate" FROM APZ_OHWP a WHERE a."PeriodCode"=:Period  AND (a."Branch"=:Branch)),'1900-01-01') into EndDate FROM DUMMY;
	
	select ifNULL((SELECT a."WorkQuota" from APZ_OHWP a WHERE a."PeriodCode"=:Period AND a."Branch"=:Branch),0) into WorkQuota FROM DUMMY;
  		
  	insert into #temp_NhanVien
	  	SELECT a."empID", a."ExtEmpNo", a."firstName", a."lastName", a."dept", b."Name" AS "Department", a."startDate", a."termDate"
	  	FROM OHEM a LEFT OUTER JOIN OUDP b ON a."dept" = b."Code"
			WHERE 
			( a."branch"=:Branch)
			AND (cast(a."dept" as nvarchar) IN (SELECT Data From Split(:Dept,',')) OR :Dept='')
			AND (a."empID" =:Emp OR :Emp='' OR :Emp=0)
			AND (a."firstName" LIKE '%' || EmpName || '%' OR a."lastName" LIKE '%' || :EmpName || '%' OR :EmpName='')
			AND (cast(a."status" as nvarchar) IN (SELECT Data From Split(:SStatus,',')) OR :SStatus='')
			AND a."Active"='Y';
	
	/*Lấy ra bảng chứa thông tin các ngày trong khoảng kì chấm công*/
	Create local temporary table #Number (GENERATED_PERIOD_START int);
	Insert Into #Number
		SELECT GENERATED_PERIOD_START FROM SERIES_GENERATE_DECIMAL(1, 0, DAYS_BETWEEN(:StartDate,:EndDate) + 1);
		
	create local temporary table #DateData ("ddate" timestamp);
	Insert Into #DateData
		SELECT DISTINCT CAST(ADD_DAYS(:StartDate, a.GENERATED_PERIOD_START) AS timestamp) FROM #Number A;
			
	select string_agg('(select MAX("WorkCode") FROM #tbl where "EmpId" = a."EmpId" and "WorkDay" = ''' || TO_VARCHAR("ddate", 'yyyy-MM-dd') || ''') "' || TO_VARCHAR("ddate", 'yyyy.MM.dd') || '"', ',') into daterage from #DateData;
   	
	IF IFNULL(:daterage,'')<>''
	THEN
		select IFNULL((SELECT string_agg('b."' || COLUMN_NAME || '"',',') from TABLE_COLUMNS where SCHEMA_NAME = CURRENT_SCHEMA and TABLE_NAME = N'APZ_OTSL' AND COLUMN_NAME like 'U__%' ESCAPE '_'), '') into Udfcols FROM DUMMY;

		IF :BaseEntry >0 
		THEN /*Tông hợp dữ liệu từ bảng VÀO - RA*/
		
			q = SELECT  a.*, ROW_NUMBER() OVER (PARTITION BY "empID","WorkDay" ORDER BY "empID") "rn",SUM(ifnull("WorkTotal",0)) OVER (PARTITION BY "empID","WorkDay" ORDER BY "empID") "SumInDay",b."CalcType"
					FROM APZ_IOD1 a join APZ_OWSC b on a."SchID" = b."SchID" where "Period" = :Period and "AbsEntry" = :BaseEntry;
					
			create local temporary table #tblInOut as (SELECT *	FROM :q WHERE   ("rn" = 1 and "CalcType" = '2') or "CalcType" = '1');
	
			create local temporary table #tblWorkCode as (select top 0 * from APZ_OHWC);
			insert into #tblWorkCode
				select a.* from APZ_OHWC a join APZ_OHWT b on a."WorkType" = b."Id" and b."Typ" = 'W' and a."Branch" = :Branch;
			--Trường hợp không tính công theo ca
			update #tblInOut  set "WorkCode" = ifnull((Select "Code" from (select *, row_number() over(order by "Rate" desc) rn FROM #tblWorkCode where "Rate" =ROUND(#tblInOut."SumInDay",4) or "Rate" =ROUND(#tblInOut."SumInDay",3) 
												or "Rate" =ROUND(#tblInOut."SumInDay",2) or "Rate" =ROUND(#tblInOut."SumInDay",1) or "Rate" < #tblInOut."SumInDay") x where rn = 1),'') where "CalcType" = '2';
	
	
			--Lấy thông tin tổng hợp,tổng đi muộn, tổng về sớm	
			p = SELECT  a.*, ROW_NUMBER() OVER (PARTITION BY "empID" ORDER BY "empID") "rn2"
						,SUM(ifnull("Late",0)) OVER (PARTITION BY "empID" ORDER BY "empID") "SumLate",SUM(ifnull("Early",0)) OVER (PARTITION BY "empID" ORDER BY "empID") "SumEarly"
					FROM #tblInOut a LEFT JOIN APZ_OHWC c on c."Branch" = :Branch and a."WorkCode" = c."Code" where "Period" = :Period and "AbsEntry" = :BaseEntry;
					
			create local temporary table #tblLateEarly as (SELECT  * FROM :p WHERE   "rn2" = 1);
	
			--Tổng hợp mã đi làm
			X = SELECT  a."empID",ROW_NUMBER() OVER (PARTITION BY "empID" ORDER BY "empID") "rn2",SUM(ifnull(c."Rate",0)) OVER (PARTITION BY "empID" ORDER BY "empID") "SumTotal" FROM #tblInOut a 
					LEFT JOIN APZ_OHWC c on c."Branch" = :Branch and a."WorkCode" = c."Code" LEFT JOIN APZ_OHWT d on c."WorkType" = d."Id" where "Period" = :Period and "AbsEntry" = :BaseEntry  and d."Typ" = 'W';
					
			create local temporary table #tblWorkSum as (SELECT  * FROM :x WHERE   "rn2" = 1);
	
			--Tổng hợp mã nghỉ
			Y = SELECT  a."empID",ROW_NUMBER() OVER (PARTITION BY "empID" ORDER BY "empID") "rn2",SUM(ifnull(c."Rate",0)) OVER (PARTITION BY "empID" ORDER BY "empID") "SumTotal" FROM #tblInOut a 
						LEFT JOIN APZ_OHWC c on c."Branch" = :Branch and a."WorkCode"=c."Code" LEFT JOIN APZ_OHWT d on c."WorkType" = d."Id"  where "Period" = :Period and "AbsEntry" = :BaseEntry and d."Typ" = 'A';
					
			create local temporary table #tblAbsentSum as (SELECT  * FROM :Y WHERE "rn2" = 1);
	
			--Tổng hợp mã phép
			XY = SELECT  a."empID",ROW_NUMBER() OVER (PARTITION BY "empID" ORDER BY "empID") "rn2",SUM(ifnull(c."Rate",0)) OVER (PARTITION BY "empID" ORDER BY "empID") "SumTotal" FROM #tblInOut a 
					LEFT JOIN APZ_OHWC c on c."Branch" = :Branch and a."WorkCode"=c."Code" LEFT JOIN APZ_OHWT d on c."WorkType" = d."Id" where "Period" = :Period and "AbsEntry" = :BaseEntry and d."Typ" = 'L';
					
			create local temporary table #tblLeaveSum as (SELECT  * FROM :XY WHERE   "rn2" = 1);
	
			--Tổng hợp tất cả các phần ở trên để cho vào bảng #tblSUM
			create local temporary table #tblSUM as (select a.*,b."SumTotal" as "WorkSum",c."SumTotal" as "AbsentSum",d."SumTotal" as "LeaveSum" 
								from #tblLateEarly a left join #tblWorkSum b on a."empID" = b."empID" left join #tblAbsentSum c on a."empID" = c."empID" left join #tblLeaveSum d on a."empID" = d."empID");
								
			sqlString = 'create local temporary table #tbl as (SELECT  a."empID" "EmpId",a."ExtEmpNo", a."lastName" || '', '' || a."firstName" as "EmpName",a."Department",  a."startDate", a."termDate", ''' || ifnull(:Period,'') || ''' "Period",' || 
					ifnull(cast(:Branch as nvarchar),'') || ' "Branch",b."TotalBefore",b."WorkSum",b."AbsentSum",b."LeaveSum",' || ifnull(cast(:WorkQuota as nvarchar),'') || ' "WorkQuota",''O'' as "DocStatus"
					,b."Extra",b."WorkTotal",b."LateSum",b."EarlySum",b."UserSign2",b."UpdateDate",d."LeaveTotal", d."Remain", e."CompLeaveTotal" "TotalOT", e."Used" "UsedOT", e."Remain" "RemainOT",  b."Comments"
					,t0."ddate" "WorkDay",' || ifnull(Cast(:BaseEntry as nvarchar),'') || ' as "BaseEntry"
					,ifnull((select string_agg(ifnull(aa."WorkCode",''''),'','') from #tblInOut aa where DAYS_BETWEEN(aa."WorkDay",t0."ddate")=0 AND  aa."empID"=a."empID" and ifnull(aa."WorkCode",'''') != '''' 
					and aa."Period" = ''' || ifnull(:Period,'') || ''' AND aa."AbsEntry" = ' || cast(:BaseEntry as nvarchar) || '),'''') AS "WorkCode" ' || case when ifnull(:Udfcols,'') = '' then '' else ',' || :Udfcols end || ' 
			from #temp_NhanVien a LEFT JOIN  #DateData T0 ON T0."ddate" IS NOT NULL 
			LEFT JOIN APZ_OTSL b ON 1=0
			LEFT JOIN APZ_TSL2 d ON	a."empID"=d."EmpId"
			AND d."Period"=''' || ifnull(:Period,'') || '''	
			LEFT JOIN APZ_TSL3 e ON a."empID"=e."EmpId"
			AND e."Period"=''' || ifnull(:Period,'') || ''');';
			exec :sqlstring;
			
			--Sau khi đổ dữ liệu từ bảng IN/OUT vào bảng công thì cập nhật các dữ liệu tổng (các trường bảng APZ_OTSL) 
			sqlString = 'Update a set a."TotalBefore" = b."WorkSum",a."WorkSum" = b."WorkSum",a."WorkTotal" = b."WorkSum",a."AbsentSum" = b."AbsentSum",a."LeaveSum" = b."LeaveSum",a."LateSum" = b."SumLate",a."EarlySum" = b."Early"
				  from #tbl a left join #tblSUM b on a."EmpId" = b."empID";';
		ELSE
			/*Trường hợp không dùng bảng dữ liệu IN/OUT*/
			/*Tính toán lại thông tin */
			q = SELECT  a.*, ROW_NUMBER() OVER (PARTITION BY "empID","WorkDay" ORDER BY "empID") "rn",SUM(ifnull("WorkTotal",0)) OVER (PARTITION BY "empID","WorkDay" ORDER BY "empID") "SumInDay",b."CalcType"
					FROM APZ_IOD1 a join APZ_OWSC b on a."SchID" = b."SchID" 
						Join APZ_OIOD c on a."AbsEntry" = c."AbsEntry"
					WHERE a."Period" = :Period And Ifnull(c."status",'') = 'C';
					
			create local temporary table #tblInOut as (SELECT *	FROM :q WHERE   ("rn" = 1 and "CalcType" = '2') or "CalcType" = '1');
	
			create local temporary table #tblWorkCode as (select top 0 * from APZ_OHWC);
			insert into #tblWorkCode
				select a.* from APZ_OHWC a join APZ_OHWT b on a."WorkType" = b."Id" and b."Typ" = 'W' and a."Branch" = :Branch;
			--Trường hợp không tính công theo ca
			update #tblInOut  set "WorkCode" = ifnull((Select "Code" from (select *, row_number() over(order by "Rate" desc) rn FROM #tblWorkCode where "Rate" =ROUND(#tblInOut."SumInDay",4) or "Rate" =ROUND(#tblInOut."SumInDay",3) 
												or "Rate" =ROUND(#tblInOut."SumInDay",2) or "Rate" =ROUND(#tblInOut."SumInDay",1) or "Rate" < #tblInOut."SumInDay") x where rn = 1),'') where "CalcType" = '2';
	
	
			--Lấy thông tin tổng hợp,tổng đi muộn, tổng về sớm	
			p = SELECT  a.*, ROW_NUMBER() OVER (PARTITION BY "empID" ORDER BY "empID") "rn2"
						,SUM(ifnull("Late",0)) OVER (PARTITION BY "empID" ORDER BY "empID") "SumLate",SUM(ifnull("Early",0)) OVER (PARTITION BY "empID" ORDER BY "empID") "SumEarly"
					FROM #tblInOut a LEFT JOIN APZ_OHWC c on c."Branch" = :Branch and a."WorkCode" = c."Code" where "Period" = :Period;
					
			create local temporary table #tblLateEarly as (SELECT  * FROM :p WHERE   "rn2" = 1);
	
			--Tổng hợp mã đi làm
			X = SELECT  a."empID",ROW_NUMBER() OVER (PARTITION BY "empID" ORDER BY "empID") "rn2",SUM(ifnull(c."Rate",0)) OVER (PARTITION BY "empID" ORDER BY "empID") "SumTotal" FROM #tblInOut a 
					LEFT JOIN APZ_OHWC c on c."Branch" = :Branch and a."WorkCode" = c."Code" LEFT JOIN APZ_OHWT d on c."WorkType" = d."Id" where "Period" = :Period and d."Typ" = 'W';
					
			create local temporary table #tblWorkSum as (SELECT  * FROM :x WHERE   "rn2" = 1);
	
			--Tổng hợp mã nghỉ
			Y = SELECT  a."empID",ROW_NUMBER() OVER (PARTITION BY "empID" ORDER BY "empID") "rn2",SUM(ifnull(c."Rate",0)) OVER (PARTITION BY "empID" ORDER BY "empID") "SumTotal" FROM #tblInOut a 
						LEFT JOIN APZ_OHWC c on c."Branch" = :Branch and a."WorkCode"=c."Code" LEFT JOIN APZ_OHWT d on c."WorkType" = d."Id"  where "Period" = :Period and d."Typ" = 'A';
					
			create local temporary table #tblAbsentSum as (SELECT  * FROM :Y WHERE "rn2" = 1);
	
			--Tổng hợp mã phép
			XY = SELECT  a."empID",ROW_NUMBER() OVER (PARTITION BY "empID" ORDER BY "empID") "rn2",SUM(ifnull(c."Rate",0)) OVER (PARTITION BY "empID" ORDER BY "empID") "SumTotal" FROM #tblInOut a 
					LEFT JOIN APZ_OHWC c on c."Branch" = :Branch and a."WorkCode"=c."Code" LEFT JOIN APZ_OHWT d on c."WorkType" = d."Id" where "Period" = :Period and d."Typ" = 'L';
					
			create local temporary table #tblLeaveSum as (SELECT  * FROM :XY WHERE   "rn2" = 1);
			
			--Tổng hợp tất cả các phần ở trên để cho vào bảng #tblSUM
			create local temporary table #tblSUM as (select a.*,b."SumTotal" as "WorkSum",c."SumTotal" as "AbsentSum",d."SumTotal" as "LeaveSum" 
								from #tblLateEarly a left join #tblWorkSum b on a."empID" = b."empID" left join #tblAbsentSum c on a."empID" = c."empID" left join #tblLeaveSum d on a."empID" = d."empID");
			
			sqlString = 'create local temporary table #tbl as (SELECT a."empID" "EmpId",a."ExtEmpNo", a."lastName"  || '', '' ||a."firstName" as "EmpName",a."Department", a."startDate", a."termDate", 
					''' || ifnull(:Period,'') || ''' "Period",' || ifnull(cast(:Branch as nvarchar),'') || ' "Branch",b."TotalBefore",b."WorkSum",b."AbsentSum"
					,b."LeaveSum", ' || ifnull(cast(:WorkQuota as nvarchar),'') || ' "WorkQuota"
					,b."DocStatus",b."Extra",b."WorkTotal",b."LateSum",b."EarlySum",b."UserSign2",b."UpdateDate",d."LeaveTotal", d."Remain", e."CompLeaveTotal" "TotalOT"
					, e."Used" "UsedOT", e."Remain" "RemainOT",  b."Comments",t0."ddate" "WorkDay",b."BaseEntry"
					,ifnull((select string_agg(ifnull(aa."WorkCode",''''), '','') from APZ_TSL1 aa where DAYS_BETWEEN(aa."WorkDay",t0."ddate")=0 AND  aa."EmpId"=a."empID" and aa."WorkCode" is not null and aa."Period" = '''
					|| ifnull(:Period,'') || '''),'''') AS "WorkCode" ' || case when ifnull(:Udfcols,'') = '' then '' else ',' || :Udfcols end || '
			from #temp_NhanVien a LEFT JOIN  #DateData T0 ON T0."ddate" IS NOT NULL 
			LEFT JOIN APZ_OTSL b ON b."EmpId"=a."empID" AND b."Period"=''' || ifnull(:Period,'') || '''
			LEFT JOIN APZ_TSL2 d ON a."empID"=d."EmpId" AND d."Period"=''' || ifnull(:Period,'') || '''	
			LEFT JOIN APZ_TSL3 e ON	a."empID"=e."EmpId"	AND e."Period"='''|| ifnull(:Period,'') || ''');';
			exec :sqlstring;
			--Cập nhật lại thông tin cho TimeSheeting
			sqlString = 'Update a set a."TotalBefore" = b."WorkSum",a."WorkSum" = b."WorkSum",a."WorkTotal" = b."WorkSum",
								a."AbsentSum" = b."AbsentSum",a."LeaveSum" = b."LeaveSum",a."LateSum" = b."SumLate",a."EarlySum" = b."Early"
				  from #tbl a left join #tblSUM b on a."EmpId" = b."empID";';
		End IF;
		
		exec :sqlString;
		
		Udfcols = replace(:Udfcols, 'b."', '"');
		
		sqlString = N'
		SELECT distinct "EmpId","ExtEmpNo", "EmpName", "Department", "Period", "Branch", "TotalBefore","WorkSum", "AbsentSum", "LeaveSum", "WorkQuota", "DocStatus", "Extra","WorkTotal","LateSum","EarlySum","UserSign2"
						,"UpdateDate","LeaveTotal", "Remain", "TotalOT","UsedOT", "RemainOT",  "Comments", "BaseEntry"' || case when ifnull(:Udfcols,'') = '' then '' else ',' || :Udfcols end ||
						case when ifnull(:daterage,'') = '' then '' else ',' || :daterage end || '  FROM #tbl a;';

		EXECUTE immediate :sqlstring;

		DROP TABLE #DateData;
	
	END IF;
	
	DROP TABLE #temp_NhanVien;
END