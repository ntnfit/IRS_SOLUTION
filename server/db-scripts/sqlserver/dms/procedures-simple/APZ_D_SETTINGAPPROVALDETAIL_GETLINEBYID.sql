USE [SBODemoAU];
GO
CREATE OR ALTER PROCEDURE dbo.[APZ_D_SettingApprovalDetail_GetLineById]
    @Id int = 0
AS
BEGIN
    SET NOCOUNT ON;
select * from APZ_D_SAD2 where [DocEntry]=@Id;
END;
GO
