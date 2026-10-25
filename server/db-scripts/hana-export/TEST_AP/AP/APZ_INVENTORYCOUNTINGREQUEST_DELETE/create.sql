
CREATE PROCEDURE APZ_InventoryCountingRequest_Delete
(
in ids nvarchar(5000)
)
AS 
BEGIN
delete from APZ_OCRQ where "DocEntry" in(SELECT Data FROM split(:ids,','));
END;




