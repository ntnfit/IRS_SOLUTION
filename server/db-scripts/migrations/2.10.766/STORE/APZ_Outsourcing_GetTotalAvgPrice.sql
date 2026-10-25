
CALL APZ_EXISTSPROC('APZ_Outsourcing_GetTotalAvgPrice');

GO

CREATE PROCEDURE APZ_Outsourcing_GetTotalAvgPrice
(	
	in whs nvarchar(5000) default '',
	in item nvarchar(5000) default '',
	in qty nvarchar(5000) default ''	
)
AS
BEGIN

	declare decRound int;
	declare tmp_Outsourcing_AvgPrice table("WhsCode" Nvarchar(8),"ItemCode" Nvarchar(50),"Quantity" decimal(19,6));
	
	tWhs = SELECT * FROM SplitString(:whs,',');
	
	tItem = SELECT * FROM SplitString(:item,',');
	
	tQty = SELECT * FROM SplitString(:qty,',');
	
	INSERT INTO :tmp_Outsourcing_AvgPrice 
	SELECT a."part" "WhsCode",
		b."part" "ItemCode",
		sum(cast(c."part" as decimal(19,6))) "Quantity"
	FROM :tWhs a 
		INNER JOIN :tItem b on a."id" = b."id"
		INNER JOIN :tQty c on a."id" = c."id"
	group by a."part", b."part";
	
	-- Tính chính xác giá trị hạch toán của phiếu Issue
	select a."Decimals" into decRound from OCRN a join OADM b on a."CurrCode"=b."MainCurncy";
	if :decRound = -1 then
		select "SumDec" into decRound from OADM;
	end if;
	
	select sum("Total") "Total" from
	(select round(b."Quantity" * a."StockValue" / a."OnHand", :decRound) "Total"
		from OITW a join :tmp_Outsourcing_AvgPrice b on a."ItemCode"=b."ItemCode" and a."WhsCode"=b."WhsCode"
		where a."OnHand">0);
END