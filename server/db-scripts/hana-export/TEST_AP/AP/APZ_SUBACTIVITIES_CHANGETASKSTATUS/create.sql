
CREATE PROCEDURE APZ_SubActivities_ChangeTaskStatus

AS
BEGIN

		--DECLARE @ClgCode int = ISNULL((SELECT TOP 1 ClgCode from #dtAPZ_OCLG ORDER BY ClgCode DESC),0);

       -- DECLARE @Closed nvarchar(max) = ISNULL((SELECT TOP 1 Closed from #dtAPZ_OCLG ORDER BY ClgCode DESC),0);

       -- DECLARE @status nvarchar(max) = ISNULL((SELECT TOP 1 [status] from #dtAPZ_OCLG ORDER BY ClgCode DESC),0);

		create local temporary table #dtAPZ_OCLGData ("ClgCode" int, "Closed" nvarchar(50), "status" nvarchar(50), "CloseDate" timestamp, "AttendUser" nvarchar(50));
		exec 'DROP TABLE #dtAPZ_OCLGData;';
		exec 'create local temporary table #dtAPZ_OCLGData as (select * from "#dtAPZ_OCLG");';
		
		update a set "Closed"=b."Closed", "status"=b."status", "CloseDate"=b."CloseDate", "AttendUser"=b."AttendUser" from APZ_OCLG a inner join #dtAPZ_OCLGData b ON a."ClgCode"=b."ClgCode";
		select top 1 "ClgCode" from #dtAPZ_OCLGData;
END





