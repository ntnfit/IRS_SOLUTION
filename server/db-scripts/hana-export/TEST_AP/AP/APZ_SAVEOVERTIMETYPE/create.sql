
CREATE  PROCEDURE APZ_SaveOvertimeType
AS
BEGIN
	declare temp_var_0 integer;
	create local temporary table #temp_OHOT as (select top 0 * from APZ_OHOT);
	execute immediate 'insert into #temp_OHOT select * from #APZ_OHOT;';
	SELECT (SELECT COUNT(*) FROM #temp_OHOT GROUP BY "Code", "Branch" HAVING COUNT(*) > 1) INTO temp_var_0 FROM DUMMY;
	IF :temp_var_0 > 0 
	THEN 
		SELECT 1, 'This entry already exists in the table!' FROM DUMMY;
		RETURN;
	END IF;

	DELETE FROM APZ_OHOT;
	INSERT INTO APZ_OHOT (SELECT  * FROM #temp_OHOT);

END




