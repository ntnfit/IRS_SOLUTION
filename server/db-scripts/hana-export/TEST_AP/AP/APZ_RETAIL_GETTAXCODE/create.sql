

CREATE PROCEDURE apz_Retail_GetTaxCode
AS
BEGIN

	select * from OVTG where "Category" = 'O' and "Inactive" = 'N';

END



