-- HANA: Apz_Auth_GetAllAuthForm (HANA parameter userName; the SQL Server caller sends @user) — authorization tree for the setup form; the super user gets a fixed admin menu
CREATE OR ALTER PROCEDURE dbo.Apz_Auth_GetAllAuthForm
    @user nvarchar(50),
    @lngCode nvarchar(50),
    @superUser nvarchar(50)
AS
BEGIN
    SET NOCOUNT ON;
    IF @user <> @superUser
    BEGIN
        CREATE TABLE #tempAuth (Id int, Code nvarchar(100) COLLATE DATABASE_DEFAULT, Name nvarchar(254) COLLATE DATABASE_DEFAULT, FatherId int, menucode nvarchar(100) COLLATE DATABASE_DEFAULT,
                                authoriza nvarchar(2) COLLATE DATABASE_DEFAULT, MenuName nvarchar(254) COLLATE DATABASE_DEFAULT, MenuType char(1) COLLATE DATABASE_DEFAULT, LanguageKey nvarchar(100) COLLATE DATABASE_DEFAULT, TempId int);
        INSERT INTO #tempAuth
        SELECT DISTINCT a.Id, a.Code, a.Name, a.FatherId, ISNULL(b.menucode, a.Code), ISNULL(b.authoriza, N'N'), a.Name, a.MenuType, a.LanguageKey,
               CASE WHEN a.MenuType IN ('N', 'F', 'A') THEN a.Id ELSE ISNULL(NULLIF(a.FatherId, -1), a.Id) END
        FROM dbo.APZ_OMNI a
        LEFT JOIN dbo.OPRM b ON a.Code = b.menucode AND b.USER_CODE = @user;

        IF @user NOT IN (N'HR0001', N'HR0002', N'HR0003', N'HR0004')
            DELETE FROM #tempAuth WHERE dbo.HR_GetRootMenuCode(menucode) = N'barSub_HRM';

        UPDATE a SET MenuName = b.ColumnName, Name = b.ColumnName
        FROM #tempAuth a
        JOIN dbo.APZ_FORMSETTING b ON b.FormId = N'oMain' AND b.ItemId = N'oMain' AND b.ColumnId = a.LanguageKey
                                  AND b.UserSign = N'-' AND a.MenuType IN ('F', 'N', 'L') AND b.LngCode = @lngCode;

        SELECT * FROM #tempAuth ORDER BY TempId, FatherId, Name;
    END
    ELSE
        SELECT 1 AS Id, N'barSub_Administration' AS Code, N'Administration' AS Name, -1 AS FatherId, N'barSub_Administration' AS menucode,
               N'F' AS authoriza, N'Administration' AS MenuName, 'N' AS MenuType, N'Administration' AS LanguageKey
        UNION SELECT 2, N'frm_Authorization', N'General Authorization', 1, N'frm_Authorization', N'F', N'General Authorization', 'F', N'Authorization'
        UNION SELECT 3, N'frm_LicenseManager', N'License Manager', 1, N'frm_LicenseManager', N'F', N'License Manager', 'F', N'LicenseManager'
        UNION SELECT 4, N'frm_FormSetting', N'Form Setting', NULL, N'frm_FormSetting', N'F', N'Form Setting', 'F', N'FormSetting';
END;
