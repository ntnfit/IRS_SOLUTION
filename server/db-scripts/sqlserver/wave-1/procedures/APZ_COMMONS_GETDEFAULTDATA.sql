-- HANA: APZ_Commons_GetDefaultData — default warehouse: user defaults group, then branch, then company
CREATE OR ALTER PROCEDURE dbo.APZ_Commons_GetDefaultData
    @userName nvarchar(50) = N'',
    @branch int = -1
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @WhsDefault nvarchar(8) = ISNULL((
        SELECT Warehouse FROM OUDG WHERE Code = (SELECT TOP 1 DfltsGroup FROM OUSR WHERE USER_CODE = @userName)), N'');
    IF @WhsDefault = N''
        SET @WhsDefault = ISNULL((SELECT DflWhs FROM OBPL WHERE BPLId = @branch), N'');
    IF @WhsDefault = N''
        SET @WhsDefault = ISNULL((SELECT TOP 1 DfltWhs FROM OADM), N'');
    SELECT MltpBrnchs AS MltpBrnchs, @WhsDefault AS DftWhs FROM OADM;
END;
