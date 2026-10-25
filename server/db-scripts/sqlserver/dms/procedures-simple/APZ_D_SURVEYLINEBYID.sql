USE [SBODemoAU];
GO
CREATE OR ALTER PROCEDURE dbo.[APZ_D_SurveyLineById]
    @docEntry int = 0
AS
BEGIN
    SET NOCOUNT ON;
select * from APZ_D_OSUV where [DocEntry] = @docEntry;
	select * from APZ_D_SUV1 where [DocEntry] = @docEntry;
END;
GO
