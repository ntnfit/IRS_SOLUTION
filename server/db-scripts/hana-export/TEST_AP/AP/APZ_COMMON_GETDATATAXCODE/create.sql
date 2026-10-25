
CREATE PROCEDURE apz_common_GetDataTaxCode()
AS
BEGIN
	SELECT "Code","Name","Rate" FROM OSTC WHERE "Lock"='N';
END




