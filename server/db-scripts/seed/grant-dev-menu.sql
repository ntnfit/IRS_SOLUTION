-- Dev only: give the given IRS users full ('F') rights on every menu in APZ_OMNI (OPRM rows).
-- sqlcmd -S localhost -E -C -I -f 65001 -d SBODemoAU -i grant-dev-menu.sql  (set env var Users=apzadmin,manager first)
SET NOCOUNT ON;
DECLARE @u TABLE (USER_CODE nvarchar(20));
INSERT INTO @u SELECT LTRIM(RTRIM(value)) FROM STRING_SPLIT(N'$(Users)', N',');

MERGE dbo.OPRM AS t
USING (SELECT u.USER_CODE, m.Code AS menucode FROM @u u CROSS JOIN dbo.APZ_OMNI m) AS s
   ON t.USER_CODE = s.USER_CODE AND t.menucode = s.menucode
WHEN MATCHED THEN UPDATE SET authoriza = N'F'
WHEN NOT MATCHED THEN INSERT (USER_CODE, menucode, authoriza) VALUES (s.USER_CODE, s.menucode, N'F');

SELECT USER_CODE, COUNT(*) AS menus FROM dbo.OPRM GROUP BY USER_CODE;
