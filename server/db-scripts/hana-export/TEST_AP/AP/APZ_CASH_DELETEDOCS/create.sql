

CREATE PROCEDURE APZ_Cash_DeleteDocs(in cashCode nvarchar(50) default '')
AS
BEGIN
	delete from APZ_CASH1 where "CashCode" in (:cashCode);
	delete from APZ_CASH where "CashCode" in (:cashCode);
END;





