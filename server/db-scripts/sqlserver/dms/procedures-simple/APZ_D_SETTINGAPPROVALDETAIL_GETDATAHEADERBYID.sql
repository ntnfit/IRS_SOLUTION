USE [SBODemoAU];
GO
CREATE OR ALTER PROCEDURE dbo.[APZ_D_SettingApprovalDetail_GetDataHeaderById]
    @Id int = 0
AS
BEGIN
    SET NOCOUNT ON;
select * from APZ_D_SAD1 where [DocEntry]=@Id order by [OrderId];
END;
GO
