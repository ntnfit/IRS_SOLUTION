USE [SBODemoAU];
GO
CREATE OR ALTER PROCEDURE dbo.[APZ_D_SettingApprovalDetail_Remove]
    @Id int = 0
AS
BEGIN
    SET NOCOUNT ON;
Delete from APZ_D_OSAD where [DocEntry]=@Id;
	Delete from APZ_D_SAD1 where [DocEntry]=@Id;
	Delete from APZ_D_SAD2 where [DocEntry]=@Id;
	select 0, '';
END;
GO
