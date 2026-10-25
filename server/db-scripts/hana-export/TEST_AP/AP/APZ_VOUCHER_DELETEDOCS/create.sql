

CREATE PROCEDURE APZ_Voucher_DeleteDocs(in code nvarchar(50) default '')
AS
BEGIN
	delete from APZ_VOURCHER1 where "PinCode" = :code;
	delete from APZ_VOURCHER where "PinCode" = :code;
END;





