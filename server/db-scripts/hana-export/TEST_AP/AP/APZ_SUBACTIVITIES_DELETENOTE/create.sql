
CREATE PROCEDURE APZ_SubActivities_DeleteNote
AS
BEGIN
	declare ClgCode integer ;
	create local temporary table #temp_Note as (select top 0 * from APZ_OCLG);
	exec 'drop table #temp_Note';
	exec 'create local temporary table #temp_Note as (select * from "#APZ_Note")';
	
	select IFNULL((SELECT TOP 1 "ClgCode" FROM #temp_Note), 0) into clgcode from DUMMY;
	DELETE FROM APZ_OCLG WHERE "ClgCode" = :ClgCode;
	
	select :ClgCode from dummy;
END




