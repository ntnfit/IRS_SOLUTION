
CREATE PROCEDURE APZ_TimeSheeting_SaveOvertimeInformation
AS
BEGIN
	
	DECLARE Period VARCHAR(20);
	
	create local temporary table #dtAPZ_TSL3Data as (select top 0 * from APZ_TSL3);
	exec 'insert into #dtAPZ_TSL3Data select * from "#dtAPZ_TSL3"';
	
	SELECT IFNULL((SELECT TOP 1 "Period" FROM #dtAPZ_TSL3Data),'') into Period from DUMMY;

	DELETE FROM APZ_TSL3 WHERE "Period"=:Period AND "EmpId" IN (SELECT "EmpId" FROM #dtAPZ_TSL3Data);

	INSERT INTO APZ_TSL3 SELECT * FROM #dtAPZ_TSL3data;


end



