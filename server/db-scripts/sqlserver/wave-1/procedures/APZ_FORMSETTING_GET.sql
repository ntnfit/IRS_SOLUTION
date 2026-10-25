-- HANA: apz_formsetting_get — grid/column layout for a form item.
-- Order of fallbacks: user's own setting -> 'manager' -> default ('-') in the language -> generated from the
-- table's user fields (U_*) when the item is a table and the form is not a UDO. 'oMain'/'oMain' returns menu captions.
CREATE OR ALTER PROCEDURE dbo.apz_formsetting_get
    @FormId nvarchar(50),
    @Itemid nvarchar(50),
    @UserSign nvarchar(50),
    @lngCode nvarchar(50)
AS
BEGIN
    SET NOCOUNT ON;
    SET @UserSign = ISNULL(@UserSign, N'manager');
    SET @lngCode = ISNULL(@lngCode, N'-');
    DECLARE @currentLng nvarchar(50) = CASE WHEN @UserSign <> N'-' THEN N'-' ELSE @lngCode END;

    IF @FormId = N'oMain' AND @Itemid = N'oMain'
    BEGIN
        SELECT ColumnId, ColumnName FROM dbo.APZ_FORMSETTING
        WHERE FormId = N'oMain' AND ItemId = N'oMain' AND UserSign = N'-' AND LngCode = @lngCode;
        RETURN;
    END;

    IF NOT EXISTS (SELECT 1 FROM dbo.APZ_FORMSETTING WHERE FormId = @FormId AND ItemId = @Itemid AND UserSign = @UserSign)
        SET @UserSign = N'manager';

    CREATE TABLE #tbl (FormId nvarchar(50) COLLATE DATABASE_DEFAULT, ItemId nvarchar(50) COLLATE DATABASE_DEFAULT, ColumnId nvarchar(50) COLLATE DATABASE_DEFAULT, UserSign nvarchar(20) COLLATE DATABASE_DEFAULT, Width int, Visible tinyint,
                       VisIndex int, Editable tinyint, IsReadOnly tinyint, DataType nvarchar(100) COLLATE DATABASE_DEFAULT, ColumnName nvarchar(200) COLLATE DATABASE_DEFAULT, Fixed char(1) COLLATE DATABASE_DEFAULT,
                       AllowChSet tinyint, VisSetting tinyint, GroupIndex int, CurrField nvarchar(50) COLLATE DATABASE_DEFAULT);

    INSERT INTO #tbl
    SELECT a.FormId, a.ItemId, a.ColumnId, a.UserSign, a.Width, a.Visible, a.VisIndex, a.Editable, a.IsReadOnly, NULL, a.ColumnName,
           a.Fixed, a.AllowChSet, a.VisSetting, a.GroupIndex, a.CurrField
    FROM dbo.APZ_FORMSETTING a
    WHERE a.FormId = @FormId AND a.ItemId = @Itemid AND a.UserSign = @UserSign AND a.LngCode = @currentLng;

    IF NOT EXISTS (SELECT 1 FROM #tbl)
        INSERT INTO #tbl
        SELECT a.FormId, a.ItemId, a.ColumnId, a.UserSign, a.Width, a.Visible, a.VisIndex, a.Editable, a.IsReadOnly, NULL, a.ColumnName,
               a.Fixed, a.AllowChSet, a.VisSetting, a.GroupIndex, a.CurrField
        FROM dbo.APZ_FORMSETTING a
        WHERE a.FormId = @FormId AND a.ItemId = @Itemid AND a.UserSign = N'-' AND a.LngCode = @lngCode;

    DECLARE @fixedOpr62 TABLE (name sysname);
    INSERT INTO @fixedOpr62 VALUES (N'U_ItemCode'), (N'U_ItemName'), (N'U_FrgnName'), (N'U_Quantity'), (N'U_IPercent'), (N'U_Price'),
                                   (N'U_PotentAmount'), (N'U_DeliveryDate'), (N'U_OpportId'), (N'U_line'), (N'U_AttFile');

    -- no saved layout: build one from the item table's user fields (descriptions from SAP CUFD, else IRS APZ_CUFD)
    IF NOT EXISTS (SELECT 1 FROM #tbl) AND NOT EXISTS (SELECT 1 FROM OUDO WHERE Code = @FormId)
        INSERT INTO #tbl (FormId, ItemId, ColumnId, UserSign, Width, Visible, Editable, IsReadOnly, DataType, ColumnName, AllowChSet, VisSetting, GroupIndex)
        SELECT @FormId, @Itemid, c.name, N'', 100, 0, 1, 0, t.name,
               COALESCE((SELECT TOP 1 u.Descr FROM CUFD u WHERE u.TableID = @Itemid AND c.name = N'U_' + u.AliasID),
                        (SELECT TOP 1 u.Descr FROM dbo.APZ_CUFD u WHERE u.TableID = @Itemid AND c.name = N'U_' + u.AliasID)),
               1, 1, -1
        FROM sys.columns c JOIN sys.types t ON c.user_type_id = t.user_type_id
        WHERE c.object_id = OBJECT_ID(N'dbo.' + QUOTENAME(@Itemid)) AND LEFT(c.name, 2) = N'U_'
          AND (@Itemid <> N'@OPR62' OR c.name NOT IN (SELECT name FROM @fixedOpr62));

    UPDATE x SET ColumnName = COALESCE(
                 (SELECT TOP 1 u.Descr FROM CUFD u WHERE u.TableID = x.ItemId AND x.ColumnId = N'U_' + u.AliasID),
                 (SELECT TOP 1 u.Descr FROM dbo.APZ_CUFD u WHERE u.TableID = x.ItemId AND x.ColumnId = N'U_' + u.AliasID))
    FROM #tbl x
    WHERE x.ColumnName IS NULL AND LEFT(x.ColumnId, 2) = N'U_';

    UPDATE #tbl SET VisIndex = ISNULL((SELECT MAX(VisIndex) FROM #tbl), 0) WHERE VisIndex IS NULL;

    IF @Itemid <> N'@OPR62'
        SELECT * FROM #tbl ORDER BY Visible DESC, VisIndex ASC;
    ELSE
        SELECT * FROM #tbl WHERE ColumnId NOT IN (SELECT name FROM @fixedOpr62) ORDER BY Visible DESC, VisIndex ASC;
END;
