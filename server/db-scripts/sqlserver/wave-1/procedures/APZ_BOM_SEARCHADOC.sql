-- HANA: apz_BOM_SearchADoc
CREATE OR ALTER PROCEDURE dbo.apz_BOM_SearchADoc
    @Code nvarchar(50)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT a.*, b.ItemName AS ItemName
    FROM OITT a
    LEFT JOIN OITM b ON a.Code = b.ItemCode
    WHERE a.Code = @Code;
END;
