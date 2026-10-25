USE [SBODemoAU];
GO
CREATE OR ALTER PROCEDURE dbo.[APZ_D_GetLastestEmployeeTrackingLocation]
    @empId int
AS
BEGIN
    SET NOCOUNT ON;
declare empName nvarchar(254) = '';
	empName := CAST((select ISNULL([firstName], '') + ' ' + ISNULL([middleName], '') + ' ' + ISNULL([lastName], '') from OHEM where [empID] = @empId) AS NVARCHAR);
	
	select top 1 *, @empId as [empID], @empName as [empName] from APZ_D_TKLC 
	where [UserSign] = (select top 1 [userId] from OHEM where [empID] = @empId) order by [CreateDate] desc;
END;
GO
