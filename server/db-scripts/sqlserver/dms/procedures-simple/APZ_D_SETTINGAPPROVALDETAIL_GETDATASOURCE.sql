USE [SBODemoAU];
GO
CREATE OR ALTER PROCEDURE dbo.[APZ_D_SettingApprovalDetail_GetDataSource]
    @docNo NVARCHAR(100) = ''
AS
BEGIN
    SET NOCOUNT ON;
select top 1 * from ODRF where [DocEntry] = @docNo;
    select * from DRF1 where [DocEntry] = @docNo;
END;
GO
