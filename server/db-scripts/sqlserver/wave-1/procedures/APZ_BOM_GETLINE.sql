-- HANA: apz_BOM_GetLine — BOM component lines plus every ITT1 user field (U_*), numeric UDFs defaulted to 0.
-- @father goes through sp_executesql (HANA concatenated it into the SQL text).
CREATE OR ALTER PROCEDURE dbo.apz_BOM_GetLine
    @father nvarchar(50) = N''
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @udf nvarchar(max) = (
        SELECT STRING_AGG(CAST(
            CASE WHEN t.name IN ('bit', 'money', 'int', 'decimal', 'smallint', 'numeric', 'tinyint', 'float', 'bigint')
                 THEN N'ISNULL(A.' + QUOTENAME(c.name) + N', 0) AS ' + QUOTENAME(c.name)
                 ELSE N'A.' + QUOTENAME(c.name) + N' AS ' + QUOTENAME(c.name) END AS nvarchar(max)), N', ')
            WITHIN GROUP (ORDER BY c.column_id)
        FROM sys.columns c JOIN sys.types t ON c.user_type_id = t.user_type_id
        WHERE c.object_id = OBJECT_ID(N'dbo.ITT1') AND LEFT(c.name, 2) = N'U_');

    DECLARE @sql nvarchar(max) = N'
SELECT ISNULL(A.Father, N'''') AS Father, A.VisOrder, ISNULL(A.ChildNum, 0) AS ChildNum, ISNULL(A.Code, N'''') AS Code,
       ISNULL(A.Quantity, 0) AS Quantity, ISNULL(A.Warehouse, N'''') AS Warehouse, ISNULL(A.Price, 0) AS Price,
       ISNULL(A.PriceList, 0) AS PriceList, ISNULL(A.IssueMthd, N'''') AS IssueMthd, ISNULL(A.Comment, N'''') AS Comment,
       ISNULL(A.Type, 4) AS Type, ISNULL(A.WipActCode, N'''') AS WipActCode, ISNULL(A.AddQuantit, 0) AS AddQuantit,
       ISNULL(CASE A.Type WHEN 4 THEN b.ItemName WHEN 290 THEN e.ResName END, N'''') AS ItemName,
       ISNULL(A.Price, 0) * ISNULL(A.Quantity, 0) AS Total, ISNULL(b.InvntryUom, N'''') AS Uom,
       ISNULL(A.Project, N'''') AS Project, ISNULL(A.Currency, N''VND'') AS Currency,
       ISNULL(A.OcrCode, N'''') AS OcrCode, ISNULL(A.OcrCode2, N'''') AS OcrCode2, ISNULL(A.OcrCode3, N'''') AS OcrCode3,
       ISNULL(A.OcrCode4, N'''') AS OcrCode4, ISNULL(A.OcrCode5, N'''') AS OcrCode5, A.LineText'
        + CASE WHEN ISNULL(@udf, N'') <> N'' THEN N', ' + @udf ELSE N'' END + N'
FROM ITT1 A
LEFT JOIN OITM b ON A.Code = b.ItemCode
LEFT JOIN ORSC e ON e.ResCode = A.Code
WHERE A.Father = ISNULL(@father, N'''')
ORDER BY A.VisOrder;';
    EXEC sys.sp_executesql @sql, N'@father nvarchar(50)', @father;
END;
