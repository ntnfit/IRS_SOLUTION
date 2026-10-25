-- HANA: APZ_AdvanceForecast_GetColumnsDoc — returns one blank header row with APZ_OFCT's columns
CREATE OR ALTER PROCEDURE dbo.APZ_AdvanceForecast_GetColumnsDoc
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 0 * INTO #tempOFCT FROM dbo.APZ_OFCT;
    INSERT INTO #tempOFCT (Code, Name, FormView, StartDate, EndDate)
    VALUES (0, N'', 'D', CAST(GETDATE() AS date), CAST(GETDATE() AS date));
    SELECT * FROM #tempOFCT;
    DROP TABLE #tempOFCT;
END;
