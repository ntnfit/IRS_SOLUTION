
CREATE PROCEDURE APZ_CFL_ListOutgoingPayment
AS
BEGIN
	declare mainCur nvarchar(3);
	select "MainCurncy" into mainCur from OADM;
	select *, case when "DocCurr" = :mainCur then "DocTotal" else "DocTotalFC" end "Total" from OVPM where "Canceled" = 'N';
END



