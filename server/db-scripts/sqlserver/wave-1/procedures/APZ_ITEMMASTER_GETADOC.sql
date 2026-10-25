-- HANA: APZ_ItemMaster_GetADoc
CREATE OR ALTER PROCEDURE dbo.APZ_ItemMaster_GetADoc
    @ItemCode nvarchar(50) = N''
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @cloudImage nvarchar(500) = ISNULL((
        SELECT TOP 1 t.LinkThumbnails FROM dbo.APZ_D_UUPIM t
        WHERE t.ObjectCode = @ItemCode AND t.ObjectType = '810039'
        ORDER BY t.UploadDate, t.UploadTime DESC), N'');
    SELECT TOP 1 *, @cloudImage AS CloudImage FROM OITM WHERE ItemType <> 'F' AND ItemCode = @ItemCode;
END;
