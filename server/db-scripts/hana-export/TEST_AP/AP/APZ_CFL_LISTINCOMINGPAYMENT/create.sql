
CREATE PROCEDURE APZ_CFL_ListIncomingPayment
AS
BEGIN
	declare mainCur nvarchar(3);
	select "MainCurncy" into mainCur from OADM;
	select *, case when "DocCurr" = :mainCur then "DocTotal" else "DocTotalFC" end "Total" from ORCT where "Canceled" = 'N';
END




