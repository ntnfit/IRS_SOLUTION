CREATE PROCEDURE APZ_TimeSheetingHour_Save
AS
BEGIN
	
	DECLARE Period VARCHAR(20);
	DECLARE Branch INT = 0;
	declare checkPeriodStatus nchar(1) = '';
	DECLARE err_message nvarchar(4000)= '';
	DECLARE OvertimeType VARCHAR(20) = '';
	DECLARE maxLineNumOT int = 0;
	DECLARE BaseEntry INT = 0;
	
	create local temporary table #dtAPZ_OTSHData as (select top 0 *, cast(null as int) "Branch", cast(null as int) "WorkCode", cast(null as timestamp) "WorkDay" from APZ_OTSH);
	exec 'DROP TABLE #dtAPZ_OTSHData;';
	exec 'create local temporary table #dtAPZ_OTSHData as (select * from "#dtAPZ_OTSH");';
	
	SELECT IFNULL((SELECT TOP 1 "Period" FROM #dtAPZ_OTSHData),'') into Period FROM DUMMY;

	SELECT IFNULL((SELECT TOP 1 "Branch" FROM #dtAPZ_OTSHData),0) into Branch FROM DUMMY;
	
	SELECT IFNULL((SELECT TOP 1 "BaseEntry" FROM #dtAPZ_OTSHData),0) into BaseEntry FROM DUMMY;

	select IFNULL((SELECT "PeriodStatus" FROM APZ_OHWP Where "PeriodCode" =:Period and "Branch" = :Branch),'') into checkPeriodStatus from dummy;
	
	IF :checkPeriodStatus !='U'
	THEN
		err_message ='The selected Period has been closed.';
		--RAISERROR (@err_message,10,1)
		--RETURN;
		select 1600008 "error_code", :err_message "error_message" FROM DUMMY;
		return;
	END IF;
	
	SELECT IFNULL((SELECT TOP 1 "OvertimeType" FROM #dtAPZ_OTSHData),'') INTO OvertimeType FROM DUMMY;
	create local temporary table #tbl("EmpId" int);

  	SELECT IFNULL((SELECT MAX("LineNum") FROM APZ_TSH1),0) into maxLineNumOT FROM DUMMY;

	INSERT INTO #tbl
		SELECT a."EmpId" FROM #dtAPZ_OTSHData a;
	
	DELETE FROM APZ_OTSH WHERE "Period"=:Period AND "OvertimeType"=:OvertimeType AND "EmpId" IN (SELECT b."EmpId" FROM #tbl b);
	
	DELETE FROM APZ_TSH1 WHERE "Period"=:Period AND "OvertimeType"=:OvertimeType AND "EmpId" IN (SELECT b."EmpId" FROM #tbl b);
	
	INSERT INTO APZ_OTSH ("Period", "EmpId", "EmpName", "TotalBefore", "DocStatus", "Extra", "WorkTotal", "Comments", "OvertimeType", "UserSign2", "UpdateDate","BaseEntry")
		SELECT DISTINCT :Period, a."EmpId", a."EmpName", a."TotalBefore", a."DocStatus", a."Extra", a."WorkTotal", a."Comments", a."OvertimeType", a."UserSign2", a."UpdateDate", :BaseEntry 
			FROM #dtAPZ_OTSHData a WHERE ifnull(a."WorkCode",0) <> 0 OR a."Extra" <> 0 OR a."Comments" <> '';
    
	INSERT INTO APZ_TSH1 ("Period", "EmpId", "LineNum", "WorkDay", "OvertimeHour", "OvertimeType")
		SELECT :Period, a."EmpId", ROW_NUMBER() OVER (PARTITION BY a."EmpId" ORDER BY a."EmpId" ASC), a."WorkDay", a."WorkCode", a."OvertimeType" FROM #dtAPZ_OTSHData a WHERE ifnull(a."WorkCode",0) <> 0;
	
	--Close TimeSheeting In/Out
	IF (IFNULL(:BaseEntry,0) != 0) 
	THEN
		IF Exists(Select 1 From APZ_OIOD Where "AbsEntry" = :BaseEntry) THEN
			--Case BaseEntry have many OT Type
			IF NOT Exists(Select Distinct(x."OvertimeType")
						From APZ_IOD2 x LEFT JOIN APZ_OTSH y on x."Period" = y."Period" And x."OvertimeType" = y."OvertimeType" And x."AbsEntry" = y."BaseEntry"
						Where x."AbsEntry" = :BaseEntry AND y."OvertimeType" IS NULL) 
			THEN
				--Case When Data In TimeSheeting had been saved
				IF Exists(Select 1 From APZ_OTSL Where "BaseEntry" = :BaseEntry) THEN
					UPDATE APZ_OIOD set "status" = 'C' where "AbsEntry" = :BaseEntry;
				END IF;
			END IF;
		END IF;
	END IF;
	
	select 0, '' FROM DUMMY;
end
