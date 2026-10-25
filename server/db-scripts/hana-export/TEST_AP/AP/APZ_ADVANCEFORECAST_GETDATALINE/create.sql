
CREATE PROCEDURE APZ_AdvanceForecast_GetDataLine(in AbsID int default -1)
AS
BEGIN
	Declare priceList int = 0;
	
	select ifnull((select "PriceList" from APZ_OFCT where "Code" = :AbsID),-1) into priceList from DUMMY;
	
	select *, b."ItemName", b."InvntryUom", cast(null as numeric(19, 6)) "Price" from APZ_FCT1 a inner join oitm b ON a."ItemCode" = b."ItemCode" where "Code" = :AbsID Order By a."LineID";
	Select "ItemCode",Ifnull("Price",0) "Price" from ITM1 where "PriceList" = :priceList;

END





