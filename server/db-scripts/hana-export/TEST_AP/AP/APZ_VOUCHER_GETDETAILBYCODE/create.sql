

CREATE PROCEDURE APZ_Voucher_GetDetailByCode(in code nvarchar(50) default '')
AS
BEGIN
	
	select * from APZ_VOURCHER1 where "PinCode" = :code;
	
END;



