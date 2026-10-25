-- HANA: apz_ProductionOrder_GetADoc (the HANA body only returns the header; cost columns are placeholders)
CREATE OR ALTER PROCEDURE dbo.apz_ProductionOrder_GetADoc
    @DocEntry int = 0
AS
BEGIN
    SET NOCOUNT ON;
    SELECT t0.*,
           (SELECT ISNULL(BPLid, -1) FROM OWHS WHERE WhsCode = t0.Warehouse) AS BplId,
           CAST(NULL AS decimal(21, 6)) AS ActualCompCost,
           CAST(NULL AS decimal(21, 6)) AS ActualProCost,
           CAST(NULL AS decimal(21, 6)) AS TotalVariable,
           CAST(NULL AS decimal(21, 6)) AS ActualAddCost
    FROM OWOR t0 WHERE t0.DocEntry = @DocEntry;
END;
