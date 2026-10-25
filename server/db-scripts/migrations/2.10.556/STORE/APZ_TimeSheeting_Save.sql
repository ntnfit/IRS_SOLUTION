
CALL APZ_EXISTSPROC('APZ_TimeSheeting_Save');

GO

CREATE PROCEDURE APZ_TimeSheeting_Save
AS
BEGIN
	
	DECLARE Period VARCHAR(4000)= '';
	DECLARE Branch INT = 0;
	DECLARE BaseEntry INT = 0;
	DECLARE OldEntry INT = 0;
	DECLARE IsImport CHAR(1) = '';
	DECLARE Usersign2 int = 0;
	DECLARE err_message nvarchar(40000)='';
	declare checkStatus nvarchar(10) = '';
	DECLARE maxLineNum int = 0;
	DECLARE EmpList int = 0;
	DECLARE RowDeletedOTSL int = 0;
	declare sqlString nvarchar(400000) = '';
	DECLARE Udfcols NVARCHAR(40000) = '';
	DECLARE RowInsertedOTSL int = 0;
	DECLARE RowDeletedTSL1 int = 0;
	DECLARE RowInsertedTSL1 int = 0;
	
	create local temporary table #dtAPZ_OTSLData as (select top 0 *, cast(null as int) "Branch", cast(null as nvarchar) "WorkCode", cast(null as timestamp) "WorkDay", cast(null as int) "Remain",
												cast(null as int) "TotalOT", cast(null as int) "UsedOT", cast(null as int) "RemainOT" from APZ_OTSL);
	exec 'drop table #dtAPZ_OTSLData;';
	exec 'create local temporary table #dtAPZ_OTSLData as (select * from "#dtAPZ_OTSL");';
	
	select IFNULL((SELECT TOP 1 "Period" FROM #dtAPZ_OTSLData),'') INTO Period FROM DUMMY;

	select ifNULL((SELECT TOP 1 "Branch" FROM #dtAPZ_OTSLData),0) into Branch from dummy;
	select ifNULL((SELECT TOP 1 "BaseEntry" FROM #dtAPZ_OTSLData where ifnull("BaseEntry",0) != 0),0) into BaseEntry from dummy;
	select ifnull((select top 1 "BaseEntry" from APZ_OTSL where "BaseEntry" = :BaseEntry and :BaseEntry>0),0) into OldEntry FROM dummy;
	select CASE WHEN :OldEntry = 0 and :BaseEntry !=0 then 'Y' else 'N' end into IsImport from dummy;
	select ifnull((select Top 1 "UserSign2" from #dtAPZ_OTSLData),0) into Usersign2 from dummy;

	--Update BaseEntry all lines
	IF :BaseEntry != 0
	THEN
		UPDATE #dtAPZ_OTSLData set "BaseEntry" = :BaseEntry;
	END IF;
	
	select ifnull((SELECT "PeriodStatus" FROM APZ_OHWP Where "PeriodCode" =:Period and "Branch" = :Branch),'') into checkStatus from DUMMY;
	IF :checkStatus !='U'
	THEN
		err_message='The selected Period has been closed.';
		select 1600008 "error_code", :err_message "error_message" from DUMMY;
		return;
	END IF;
	--If import is override then alert error
	IF :OldEntry != 0 and :OldEntry != :BaseEntry
	THEN
		err_message='Current Timesheet is based on other In/Out AbsEntry. Please cancel this TimeSheet and re-import!';
		select 1600008 "error_code", :err_message "error_message" from DUMMY;
		return;
	END IF;
	select ifNULL((SELECT MAX("LineNum") FROM APZ_TSL1 WHERE "Period"=:Period),0) into maxLineNum from DUMMY;
	
	--create a temp table for list of empid 
	create local temporary table #tbl("EmpId" int, "WorkDay" timestamp);
	
	INSERT INTO #tbl
		SELECT DISTINCT a."EmpId", a."WorkDay" FROM #dtAPZ_OTSLData a;
	
	Select Count(1) Into EmpList From (Select DISTINCT "EmpId" From #tbl);
	
	--EmpList = ::ROWCOUNT;

	DELETE FROM APZ_OTSL WHERE "Period"=:Period AND "EmpId" IN (SELECT DISTINCT "EmpId" FROM #tbl);
	
	RowDeletedOTSL = ::ROWCOUNT;

	select IFNULL((SELECT string_agg('"' || COLUMN_NAME || '"',',') from TABLE_COLUMNS where SCHEMA_NAME = CURRENT_SCHEMA AND TABLE_NAME = 'APZ_OTSL' AND COLUMN_NAME like 'U[_]%'),'') into Udfcols from DUMMY;
	
	if ifnull(:Udfcols, '') <> '' then
		Udfcols = ',' || :Udfcols;
	END IF;
	
	sqlString = 'INSERT INTO APZ_OTSL
		        ("Period" ,"EmpId" ,"EmpName","TotalBefore" ,"WorkSum","AbsentSum","LeaveSum","WorkQuota", "DocStatus","Extra","WorkTotal" ,"LeaveTotal", "Comments", "UserSign2","UpdateDate", "BaseEntry", "LateSum","EarlySum"
				  ' || ifnull(:Udfcols,'') || '
		        )
		SELECT DISTINCT ''' || ifnull(:Period,'') || ''',"EmpId","EmpName","TotalBefore","WorkSum","AbsentSum","LeaveSum","WorkQuota","DocStatus","Extra","WorkTotal","LeaveTotal","Comments","UserSign2"
			,CURRENT_DATE,"BaseEntry","LateSum","EarlySum"' || ifnull(:Udfcols,'') || '
		 FROM #dtAPZ_OTSLData  WHERE "WorkCode"<>'''' OR "Extra"<>0 or "Comments"<>'''';';
		--PRINT @sql
	EXEC :sqlString;
		
	RowInsertedOTSL = ::ROWCOUNT;
	
	DELETE FROM APZ_TSL1 WHERE "Period"=:Period AND "EmpId" IN (SELECT "EmpId" FROM #tbl) AND "WorkDay" In (SELECT "WorkDay" FROM #tbl);
	
	RowDeletedTSL1 = ::ROWCOUNT;
	
	BEGIN
		declare indexSplit int = 1;
		declare lineNumTSL1 int = IFNULL((Select Max("LineNum") From APZ_TSL1 Where "Period" = :Period),0);
		while exists(select 1 from #dtAPZ_OTSLData where GetStringSplitIndex("WorkCode", ',', :indexSplit) <> '')
		DO
				INSERT INTO APZ_TSL1 ("Period", "EmpId", "LineNum", "WorkDay", "WorkCode", "WorkTotal")
				SELECT :Period,a."EmpId",ROW_NUMBER() OVER(PARTITION BY a."EmpId" ORDER BY a."EmpId" ASC) + :lineNumTSL1,a."WorkDay",c."Code",c."Rate"
					FROM #dtAPZ_OTSLData a inner JOIN APZ_OHWC c on c."Branch" = :Branch and GetStringSplitIndex(a."WorkCode", ',', :indexSplit) = c."Code";
				indexSplit = :indexSplit + 1;
				select max("LineNum") into lineNumTSL1 from APZ_TSL1 where "Period" = :Period;
		END WHILE;
	END;
	
	/*Update lại thông tin tổng hợp giờ làm*/
	q = SELECT  a.*, ROW_NUMBER() OVER (PARTITION BY "empID","WorkDay" ORDER BY "empID") "rn",SUM(ifnull("WorkTotal",0)) OVER (PARTITION BY "empID","WorkDay" ORDER BY "empID") "SumInDay",b."CalcType"
		FROM APZ_IOD1 a join APZ_OWSC b on a."SchID" = b."SchID" where "Period" = :Period;
					
	create local temporary table #tblInOut as (SELECT *	FROM :q WHERE   ("rn" = 1 and "CalcType" = '2') or "CalcType" = '1');
	
	create local temporary table #tblWorkCode as (select top 0 * from APZ_OHWC);
	insert into #tblWorkCode select a.* from APZ_OHWC a join APZ_OHWT b on a."WorkType" = b."Id" and b."Typ" = 'W' and a."Branch" = :Branch;
	--Trường hợp không tính công theo ca
	update #tblInOut  set "WorkCode" = ifnull((Select "Code" from (select *, row_number() over(order by "Rate" desc) rn 
											FROM #tblWorkCode where "Rate" =ROUND(#tblInOut."SumInDay",4) or "Rate" =ROUND(#tblInOut."SumInDay",3) 
												or "Rate" =ROUND(#tblInOut."SumInDay",2) or "Rate" =ROUND(#tblInOut."SumInDay",1) or "Rate" < #tblInOut."SumInDay") x where rn = 1),'') where "CalcType" = '2';
	
	--Lấy thông tin tổng hợp,tổng đi muộn, tổng về sớm	
	p = SELECT  a.*, ROW_NUMBER() OVER (PARTITION BY "empID" ORDER BY "empID") "rn2"
				,SUM(ifnull("Late",0)) OVER (PARTITION BY "empID" ORDER BY "empID") "SumLate",SUM(ifnull("Early",0)) OVER (PARTITION BY "empID" ORDER BY "empID") "SumEarly"
		FROM #tblInOut a LEFT JOIN APZ_OHWC c on c."Branch" = :Branch and a."WorkCode" = c."Code" where "Period" = :Period and "AbsEntry" = :BaseEntry;		
	
	create local temporary table #tblLateEarly as (SELECT  * FROM :p WHERE   "rn2" = 1);
	
	--Tổng hợp mã đi làm
	X = SELECT  a."empID",ROW_NUMBER() OVER (PARTITION BY "empID" ORDER BY "empID") "rn2",SUM(ifnull(c."Rate",0)) OVER (PARTITION BY "empID" ORDER BY "empID") "SumTotal" 
		FROM #tblInOut a LEFT JOIN APZ_OHWC c on c."Branch" = :Branch and a."WorkCode" = c."Code" 
						LEFT JOIN APZ_OHWT d on c."WorkType" = d."Id" where "Period" = :Period and "AbsEntry" = :BaseEntry  and d."Typ" = 'W';
	create local temporary table #tblWorkSum as (SELECT  * FROM :x WHERE   "rn2" = 1);
	
	--Tổng hợp mã nghỉ
	Y = SELECT a."empID",ROW_NUMBER() OVER (PARTITION BY "empID" ORDER BY "empID") "rn2",SUM(ifnull(c."Rate",0)) OVER (PARTITION BY "empID" ORDER BY "empID") "SumTotal" 
		FROM #tblInOut a LEFT JOIN APZ_OHWC c on c."Branch" = :Branch and a."WorkCode"=c."Code" 
			LEFT JOIN APZ_OHWT d on c."WorkType" = d."Id"  where "Period" = :Period and "AbsEntry" = :BaseEntry and d."Typ" = 'A';
	create local temporary table #tblAbsentSum as (SELECT  * FROM :Y WHERE "rn2" = 1);
	
	--Tổng hợp mã phép
	XY = SELECT  a."empID",ROW_NUMBER() OVER (PARTITION BY "empID" ORDER BY "empID") "rn2",SUM(ifnull(c."Rate",0)) OVER (PARTITION BY "empID" ORDER BY "empID") "SumTotal" 
		FROM #tblInOut a LEFT JOIN APZ_OHWC c on c."Branch" = :Branch and a."WorkCode"=c."Code" LEFT JOIN APZ_OHWT d on c."WorkType" = d."Id" where "Period" = :Period and "AbsEntry" = :BaseEntry and d."Typ" = 'L';
	create local temporary table #tblLeaveSum as (SELECT  * FROM :XY WHERE   "rn2" = 1);
	
	--Tổng hợp tất cả các phần ở trên để cho vào bảng #tblSUM
	create local temporary table #tblSUM as (Select a.*,b."SumTotal" as "WorkSum",c."SumTotal" as "AbsentSum",d."SumTotal" as "LeaveSum" 
						From #tblLateEarly a LEFT JOIN #tblWorkSum b on a."empID" = b."empID" LEFT JOIN #tblAbsentSum c on a."empID" = c."empID" left join #tblLeaveSum d on a."empID" = d."empID");
	
	Update a set a."TotalBefore" = b."WorkSum",a."WorkSum" = b."WorkSum",a."WorkTotal" = b."WorkSum",a."AbsentSum" = b."AbsentSum",a."LeaveSum" = b."LeaveSum",a."LateSum" = b."SumLate",a."EarlySum" = b."Early"
				  from APZ_OTSL a left join #tblSUM b on a."EmpId" = b."empID" And a."Period" = :Period;
	/*End Update*/
	RowInsertedTSL1 = ::ROWCOUNT;

	DELETE FROM APZ_TSL2 WHERE "Period"=:Period AND "EmpId" IN (SELECT "EmpId" FROM #tbl);

    INSERT INTO APZ_TSL2 ("Period", "EmpId", "EmpName", "LeaveTotal", "Used", "Remain", "UserSign2", "UpdateDate") 
    	SELECT DISTINCT "Period", "EmpId", "EmpName", ifnull("LeaveTotal", 0), ifnull("LeaveSum", 0), ifnull("Remain", 0), "UserSign2", "UpdateDate" 
    	FROM #dtAPZ_OTSLData WHERE "WorkCode" <> '' Or "LeaveTotal" Is Not Null;

	DELETE FROM APZ_TSL3 WHERE "Period"=:Period AND "EmpId" IN (SELECT "EmpId" FROM #tbl);

    INSERT INTO APZ_TSL3 ("Period", "EmpId", "EmpName", "CompLeaveTotal", "Used", "Remain") 
    	SELECT DISTINCT "Period", "EmpId", "EmpName", ifnull("TotalOT", 0), ifnull("UsedOT", 0), ifnull("RemainOT", 0) FROM #dtAPZ_OTSLData WHERE "WorkCode" <> '';
	
	IF :IsImport = 'Y'
	THEN
		--DELETE FROM APZ_TSH1 where "Period" = :Period AND "EmpId" IN (SELECT "EmpId" FROM #tbl);
		--DELETE FROM APZ_OTSH where "Period" = :Period AND "EmpId" IN (SELECT "EmpId" FROM #tbl);
		
		--INSERT INTO APZ_TSH1 ("Period", "EmpId", "LineNum", "WorkDay", "OvertimeType", "OvertimeHour", "FreeTxt") 
		--	SELECT "Period", "empID", "LineNum", "WorkDay", "OvertimeType", "OvertimeHour", "FreeTxt" FROM APZ_IOD2 WHERE "AbsEntry" = :BaseEntry AND "empID" IN (SELECT "EmpId" FROM #tbl);
		
		--create local temporary table #dtOTSUM as (select top 0 * from APZ_OTSH);
		
		--INSERT INTO #dtOTSUM ("Period", "EmpId", "OvertimeType", "TotalBefore", "WorkTotal", "UserSign2", "UpdateDate") 
		--	SELECT a."Period", a."empID", a."OvertimeType", SUM("OvertimeHour") AS "TotalBefore", SUM("OvertimeHour") AS "WorkTotal", :UserSign2, CURRENT_DATE FROM APZ_IOD2 a WHERE "AbsEntry" = :BaseEntry 
		--					GROUP BY a."Period", a."empID", a."OvertimeType";
		
		--UPDATE a SET a."EmpName" = ifnull(b."lastName", '') || ', ' || ifnull(b."firstName", '') FROM #dtOTSUM a LEFT OUTER JOIN OHEM b ON a."EmpId" = b."empID";
		
		--INSERT into APZ_OTSH  select * from #dtOTSUM;
		
		--Close Doc TimeSheeting In/Out
		IF NOT Exists(Select 1 From APZ_IOD2 Where "AbsEntry" = :BaseEntry) THEN
			--Case Data OT is empty
			UPDATE APZ_OIOD set "status" = 'C' where "AbsEntry" = :BaseEntry;
		ELSE
			IF Exists(Select 1 From APZ_OTSH Where "BaseEntry" = :BaseEntry) THEN
				--Check Data had been saved in data OT (Multiples OT Type)
				IF NOT Exists(Select Distinct(x."OvertimeType")
						From APZ_IOD2 x LEFT JOIN APZ_OTSH y on x."Period" = y."Period" And x."OvertimeType" = y."OvertimeType" And x."AbsEntry" = y."BaseEntry"
						Where x."AbsEntry" = :BaseEntry AND y."OvertimeType" IS NULL) 
				THEN
					UPDATE APZ_OIOD set "status" = 'C' where "AbsEntry" = :BaseEntry;
				END IF;
			END IF;
		END IF;
	END IF;
	SELECT ifnull(:Period,'') AS "Period", ifnull(:maxLineNum,0) AS "LineNum", ifnull(:EmpList,0) AS "EmpList", ifnull(:RowDeletedOTSL,0) AS "RowDeletedOTSL", ifnull(:RowDeletedTSL1,0) AS "RowDeletedTSL1"
				, ifnull(:RowInsertedOTSL,0) AS "RowInsertedOTSL", ifnull(:RowInsertedTSL1,0) AS "RowInsertedTSL1" FROM DUMMY;

end