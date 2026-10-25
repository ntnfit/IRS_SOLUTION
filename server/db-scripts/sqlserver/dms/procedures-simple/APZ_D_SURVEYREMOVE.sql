USE [SBODemoAU];
GO
CREATE OR ALTER PROCEDURE dbo.[APZ_D_SurveyRemove]
    @docEntry int = 0
AS
BEGIN
    SET NOCOUNT ON;
DELETE FROM APZ_D_OSUV where [DocEntry] = @docEntry;
	DELETE FROM APZ_D_SUV1 where [DocEntry] = @docEntry;
	SELECT 0, '';
END;
GO
