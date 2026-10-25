-- HANA: apz_filtersetting_insertquery — refreshes saved SAP queries ('Q-' rows) in APZ_FilterObject
CREATE OR ALTER PROCEDURE dbo.apz_filtersetting_insertquery
    @UserName nvarchar(20) = N''
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.APZ_FilterObject WHERE LEFT(ObjType, 2) = 'Q-';

    INSERT INTO dbo.APZ_FilterObject (ObjType, ObjDesc, DataFilter)
    SELECT N'Q-' + CAST(a.IntrnalKey AS nvarchar(20)), a.QName, a.QString
    FROM OUQR a
    RIGHT JOIN OQCN b ON a.QCategory = b.CategoryId
    WHERE a.IntrnalKey IS NOT NULL;
END;
