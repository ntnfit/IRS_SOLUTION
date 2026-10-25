
CREATE PROCEDURE Util_AdvanceReportTemplete_GetDataCurrency
AS
BEGIN
	
	select null "Code", null "Name" FROM DUMMY
	union all
	select "CurrCode" "Code", "CurrName" "Name" from OCRN;

END



