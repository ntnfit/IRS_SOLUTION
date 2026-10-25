USE [SBODemoAU];
GO
CREATE OR ALTER PROCEDURE dbo.[APZ_D_QuestionMasterData_GetLineById]
    @id int = 0
AS
BEGIN
    SET NOCOUNT ON;
select * from APZ_D_CHM1 where [DocEntry] = @id;
END;
GO
