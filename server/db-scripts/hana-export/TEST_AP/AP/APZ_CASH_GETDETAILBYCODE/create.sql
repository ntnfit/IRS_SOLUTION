

CREATE PROCEDURE APZ_Cash_GetDetailByCode(in cashCode nvarchar(50) default '')
AS
BEGIN
	select a.*, '' as "Currency" from APZ_CASH1 a where "CashCode" = :cashCode;
END;



