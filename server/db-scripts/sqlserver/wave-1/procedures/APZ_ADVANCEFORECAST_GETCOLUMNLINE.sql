-- HANA: APZ_AdvanceForecast_GetColumnLine — returns one blank line row with APZ_FCT1's columns + ItemName, InvntryUom
CREATE OR ALTER PROCEDURE dbo.APZ_AdvanceForecast_GetColumnLine
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 0 *, CAST(N'' AS nvarchar(254)) AS ItemName, CAST(NULL AS nvarchar(50)) AS InvntryUom
    INTO #tblLine FROM dbo.APZ_FCT1;
    -- HANA inserted '' into the date column (an "empty" date); the blank row needs Date to accept NULL here
    ALTER TABLE #tblLine ALTER COLUMN [Date] datetime NULL;
    INSERT INTO #tblLine (Code, LineID, ItemCode, [Date], Quantity, WhsCode, ItemName)
    VALUES (0, 0, N'', NULL, 0, N'', N'');
    SELECT * FROM #tblLine;
    DROP TABLE #tblLine;
END;
