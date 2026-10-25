-- HANA: APZ_Dashboard_GetListDashboardByUser
-- OPRM is a SAP B1 table that newer B1 versions have; on an older company DB this proc compiles but fails at run time.
CREATE OR ALTER PROCEDURE dbo.APZ_Dashboard_GetListDashboardByUser
    @userName nvarchar(50)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT a.Id
    FROM dbo.APZ_DAB1 a
    JOIN OPRM b ON N'barSub_Dashboard' + CAST(a.Id AS nvarchar(20)) = b.menucode AND b.USER_CODE = @userName
    WHERE a.CatId > 0 AND b.authoriza = 'F';
END;
