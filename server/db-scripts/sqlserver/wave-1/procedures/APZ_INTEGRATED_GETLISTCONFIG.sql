-- HANA: APZ_Integrated_GetListConfig — returns active integration configs only when the ASYNC module is installed
CREATE OR ALTER PROCEDURE dbo.APZ_Integrated_GetListConfig
AS
BEGIN
    SET NOCOUNT ON;
    IF SCHEMA_ID(N'ASYNC') IS NOT NULL AND OBJECT_ID(N'ASYNC.CONF') IS NOT NULL
        EXEC (N'SELECT * FROM ASYNC.CONF WHERE Valid = ''Y''');
END;
