-- HANA: APZ_SalesOpportunity_GetItemDetail — item lines (@OPR62) of a sales opportunity plus its extra user fields;
-- OpprId = -1 returns one blank row with the same columns.
CREATE OR ALTER PROCEDURE dbo.APZ_SalesOpportunity_GetItemDetail
    @OpprId int
AS
BEGIN
    SET NOCOUNT ON;
    IF OBJECT_ID(N'dbo.[@OPR62]', N'U') IS NULL
        THROW 50002, N'APZ_SalesOpportunity_GetItemDetail: table @OPR62 does not exist', 1;

    -- user fields other than the fixed ones the form shows itself
    DECLARE @fixed TABLE (name sysname);
    INSERT INTO @fixed VALUES (N'U_ItemCode'), (N'U_ItemName'), (N'U_FrgnName'), (N'U_Quantity'), (N'U_IPercent'), (N'U_Price'),
                              (N'U_PotentAmount'), (N'U_DeliveryDate'), (N'U_OpportId'), (N'U_line'), (N'U_AttFile');
    DECLARE @udfPlain nvarchar(max), @udfDefaulted nvarchar(max);
    SELECT @udfPlain = STRING_AGG(CAST(QUOTENAME(c.name) AS nvarchar(max)), N', ') WITHIN GROUP (ORDER BY c.column_id),
           @udfDefaulted = STRING_AGG(CAST(CASE WHEN t.name IN ('bit', 'money', 'int', 'decimal', 'smallint', 'numeric', 'tinyint', 'float', 'bigint')
                                                THEN N'ISNULL(A.' + QUOTENAME(c.name) + N', 0) AS ' + QUOTENAME(c.name)
                                                ELSE N'A.' + QUOTENAME(c.name) + N' AS ' + QUOTENAME(c.name) END AS nvarchar(max)), N', ')
                           WITHIN GROUP (ORDER BY c.column_id)
    FROM sys.columns c JOIN sys.types t ON c.user_type_id = t.user_type_id
    WHERE c.object_id = OBJECT_ID(N'dbo.[@OPR62]') AND LEFT(c.name, 2) = N'U_' AND c.name NOT IN (SELECT name FROM @fixed);

    DECLARE @sql nvarchar(max);
    IF @OpprId = -1
        SET @sql = N'
SELECT TOP 0 U_ItemCode AS ItemCode, U_ItemName AS ItemName, U_FrgnName AS FrgnName, U_Quantity AS Quantity, U_IPercent AS IPercent,
       U_Price AS Price, U_PotentAmount AS PotentAmount, U_DeliveryDate AS DeliveryDate, U_OpportId AS OpportId, LineId AS Line,
       U_AttFile AS AttFile, CAST(N'''' AS nvarchar(254)) AS AttFullPath, DocEntry'
            + CASE WHEN ISNULL(@udfPlain, N'') <> N'' THEN N', ' + @udfPlain ELSE N'' END + N'
INTO #tbl FROM [@OPR62];
INSERT INTO #tbl (ItemCode, ItemName, FrgnName, Quantity, IPercent, Price, PotentAmount, DeliveryDate, OpportId, Line, AttFile, AttFullPath, DocEntry)
VALUES (N'''', N'''', N'''', 0, 0, 0, 0, NULL, -1, -1, N'''', N'''', 0);
SELECT * FROM #tbl;';
    ELSE
        SET @sql = N'
SELECT ISNULL(A.DocEntry, 0) AS DocEntry, ISNULL(A.U_ItemCode, N'''') AS ItemCode, ISNULL(A.U_ItemName, N'''') AS ItemName,
       ISNULL(A.U_FrgnName, N'''') AS FrgnName, ISNULL(A.U_Quantity, 0) AS Quantity, ISNULL(A.U_IPercent, 0) AS IPercent,
       ISNULL(A.U_Price, 0) AS Price, ISNULL(A.U_PotentAmount, 0) AS PotentAmount, A.U_DeliveryDate AS DeliveryDate,
       @OpprId AS OpportId, ISNULL(A.LineId, -1) AS Line, ISNULL(A.U_AttFile, N'''') AS AttFile, CAST(N'''' AS nvarchar(254)) AS AttFullPath'
            + CASE WHEN ISNULL(@udfDefaulted, N'') <> N'' THEN N', ' + @udfDefaulted ELSE N'' END + N'
FROM [@OPR62] A
WHERE A.DocEntry = ' + CASE WHEN COL_LENGTH(N'dbo.OOPR', N'U_Itemdetail') IS NULL
                            THEN N'0'  -- the OOPR user field linking to @OPR62 is not defined in this company DB
                            ELSE N'ISNULL((SELECT TOP 1 U_Itemdetail FROM OOPR WHERE OpprId = @OpprId), 0)' END + N';';

    EXEC sys.sp_executesql @sql, N'@OpprId int', @OpprId;
END;
