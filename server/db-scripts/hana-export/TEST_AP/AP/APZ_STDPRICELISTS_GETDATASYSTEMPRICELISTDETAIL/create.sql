
--	APZ_STDPriceLists_GetDataSystemPriceListDetail -1
Create PROCEDURE APZ_STDPriceLists_GetDataSystemPriceListDetail(in  priceList int)
as
begin

	declare mainCurr nvarchar(50);
	select ifnull((select "MainCurncy" from OADM),'') into mainCurr from DUMMY;
	
	select "ItemCode", "ItemName", case when :priceList = -1 then "LastPurCur" else :mainCurr end "Currency", case when :priceList = -1 then "LastPurPrc" else "AvgPrice" end "Price"
		, ifnull(b."UomCode", 'manual') "PriceUnit", ifnull(c."UomCode", 'manual') "InvUom"
				from OITM a left join OUOM b ON a."PriceUnit" = b."UomEntry"
				left join OUOM c ON a."IUoMEntry" = c."UomEntry" order by "ItemCode";

end



