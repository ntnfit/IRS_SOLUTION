-- HANA: APZ_AdditionalGeneralSettings_GetData
CREATE OR ALTER PROCEDURE dbo.APZ_AdditionalGeneralSettings_GetData
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM dbo.APZ_OADM;
    SELECT * FROM dbo.APZ_ATC2;
    SELECT * FROM dbo.APZ_D_OAPC;
END;
