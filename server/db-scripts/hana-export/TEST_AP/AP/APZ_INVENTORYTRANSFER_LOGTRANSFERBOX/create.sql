
CREATE PROCEDURE Apz_InventoryTransfer_LogTransferBox(in lstBox NVARCHAR(40000), in docTransfer INT)
AS
BEGIN

		-- demo data
		INSERT INTO APZ_TransferLog ("DocEntry", "BoxCode", "DocTransfer") 
			SELECT Data, Data, :docTransfer FROM Split(:lstBox,',');
			
		SELECT 0 "Code", '' "Message" from DUMMY;

END




