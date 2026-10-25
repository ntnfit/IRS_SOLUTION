
CREATE PROCEDURE APZ_SubActivities_SaveTaskManagementSetting
AS
BEGIN
	DECLARE UserSign int = 0;
	
	create local temporary table #dtAPZ_CLG2Data as (select top 0 * from APZ_CLG2);
	exec 'drop table #dtAPZ_CLG2Data;';
	exec 'create local temporary table #dtAPZ_CLG2Data as (select * from "#dtAPZ_CLG2");';
	
	select IFNULL((SELECT TOP 1 "UserSign" FROM #dtAPZ_CLG2Data),0) into UserSign FROM DUMMY;
    
	DELETE FROM APZ_CLG2 WHERE "UserSign"=:UserSign;
    
	INSERT INTO APZ_CLG2
		SELECT * FROM #dtAPZ_CLG2Data;

END



