-- HANA: Apz_Auth_GetAllAuth — menu tree with the user's authorization, translated when lngCode is given
CREATE OR ALTER PROCEDURE dbo.Apz_Auth_GetAllAuth
    @userName nvarchar(50),
    @lngCode nvarchar(10) = N''
AS
BEGIN
    SET NOCOUNT ON;
    CREATE TABLE #tbl (Id int, FMenuName int, MenuName nvarchar(254) COLLATE DATABASE_DEFAULT, authoriza char(1) COLLATE DATABASE_DEFAULT, menucode nvarchar(254) COLLATE DATABASE_DEFAULT,
                       USER_CODE nvarchar(25) COLLATE DATABASE_DEFAULT, MenuType char(1) COLLATE DATABASE_DEFAULT, isDefault char(1) COLLATE DATABASE_DEFAULT, TempId int, LngKey nvarchar(50) COLLATE DATABASE_DEFAULT);

    INSERT INTO #tbl
    SELECT a.Id, a.FatherId, a.Name, ISNULL(b.authoriza, 'N'), a.Code, @userName, a.MenuType, b.isDefault,
           CASE WHEN a.MenuType IN ('N', 'F', 'A') THEN a.Id ELSE ISNULL(NULLIF(a.FatherId, -1), a.Id) END,
           a.LanguageKey
    FROM dbo.APZ_OMNI a
    LEFT JOIN dbo.OPRM b ON a.Code = b.menucode AND b.USER_CODE = @userName;

    IF @userName NOT IN (N'HR0001', N'HR0002', N'HR0003', N'HR0004')
        DELETE FROM #tbl WHERE dbo.HR_GetRootMenuCode(menucode) = N'barSub_HRM';

    IF ISNULL(@lngCode, N'') <> N''
        UPDATE a SET MenuName = b.ColumnName
        FROM #tbl a
        JOIN dbo.APZ_FORMSETTING b ON b.FormId = N'oMain' AND b.ItemId = N'oMain' AND b.ColumnId = a.LngKey
                                  AND b.UserSign = N'-' AND a.MenuType IN ('F', 'N', 'L') AND b.LngCode = @lngCode;

    UPDATE #tbl SET MenuName = REPLACE(MenuName, N' && ', N' & ');
    SELECT * FROM #tbl ORDER BY TempId;
END;
