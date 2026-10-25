

CREATE PROCEDURE APZ_D_Delivery_ConfirmLateTime
(
	IN docentry INT,
	IN cardcode NVARCHAR(50),
	IN newTime DATETIME,
	IN reason NVARCHAR(254)
)
AS
BEGIN
		  
	UPDATE APZ_D_ODLO SET "LateTime" = :newTime, "LateReason" = :reason WHERE "DocEntry" = :docentry AND "CardCode" = :cardcode;
	
END




