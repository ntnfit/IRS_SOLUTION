-- HANA: APZ_Integrated_CheckIRSAuthorizations — makes sure the integration menus exist for 'manager'
CREATE OR ALTER PROCEDURE dbo.APZ_Integrated_CheckIRSAuthorizations
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM dbo.OPRM WHERE menucode = N'frm_Integrate_Document')
        INSERT INTO dbo.OPRM (USER_CODE, menucode, authoriza) VALUES (N'manager', N'frm_Integrate_Document', N'F');
    IF NOT EXISTS (SELECT 1 FROM dbo.OPRM WHERE menucode = N'frm_Integrate_BusinessPartner')
        INSERT INTO dbo.OPRM (USER_CODE, menucode, authoriza) VALUES (N'manager', N'frm_Integrate_BusinessPartner', N'F');
    SELECT CAST(N'' AS nvarchar(254));
END;
