
CREATE PROCEDURE APZ_Commons_GetDataItemByCode(in itemCode nvarchar(50) default '', in priceList int default -1, in userName nvarchar(50) default '', in branch int default -1)
AS
BEGIN
	DECLARE WhsDefault VARCHAR(20);
	declare priceUnit  int;
	declare echangeInvToPriceUnit numeric(19,6);
	declare exchangeRate numeric(19,6);
	
	select IFNULL((SELECT "Warehouse" FROM OUDG WHERE "Code"=(SELECT TOP 1 "DfltsGroup" FROM OUSR WHERE USER_CODE=:userName)),'') into WhsDefault from DUMMY;
	IF :WhsDefault=''
	THEN
		select ifNULL((SELECT "DflWhs" FROM OBPL WHERE "BPLId"= :branch),'') into WhsDefault FROM DUMMY;
    END IF;
    
	IF :WhsDefault=''
	THEN
		SELECT IFNULL((SELECT "DfltWhs" FROM OADM),'') into WhsDefault FROM DUMMY;
    END	IF;


	-------------------------- dữ liệu Price unit -------------------------------------------------
	select ifnull((select "PriceUnit" from OITM where "ItemCode" = :ItemCode),-1) into priceUnit FROM DUMMY;

	------------------ lấy đơn vị quy đổi từ inventory unit sang Price unit
	select ifnull((select a."AltQty" / ifnull(a."BaseQty", 0) from UGP1 a INNER JOIN (select "ItemCode", "UgpEntry", "PriceUnit" from OITM where "ItemCode" = :ItemCode) b on a."UgpEntry" = b."UgpEntry" and a."UomEntry" = b."PriceUnit"),1) into echangeInvToPriceUnit from dummy;

	----------------------- biến lưu giá trị chuyển đổi từ đơn vị hiện tại sang đơn vị PriceUnit ---------------
	select ifnull((select a."BaseQty" * :echangeInvToPriceUnit / ifnull(a."AltQty", 0) from UGP1 a INNER JOIN (select "ItemCode", "UgpEntry", "InvntryUom" from OITM where "ItemCode" = :ItemCode) b on a."UgpEntry" = b."UgpEntry"
																			INNER JOIN OUOM c ON a."UomEntry" = c."UomEntry" and b."InvntryUom" = c."UomCode"),1) into exchangeRate from DUMMY;

	Select a."ItemCode", a."ItemName", a."ItemType", CASE WHEN  a."DfltWH" <>'' THEN a."DfltWH" else :WhsDefault end AS "DfltWH", case when b."UgpEntry" is null then 1 else b."AltQty" / ifnull(b."BaseQty",1) end "NumInInven", case 

				when a."ManBtchNum" = 'Y' then 'B'
				when a."ManSerNum" = 'Y' then 'S'
				else 'N'

			 End "ManBtchSerial", t1."AvgPrice", a."OnHand",A."BuyUnitMsr", 
		A."SalUnitMsr",
		A."NumInBuy",
		A."NumInSale",
		A."InvntryUom",
		a."UgpEntry", 
		a."ItmsGrpCod",
		a."VatGroupPu",
		a."VatGourpSa",
			 IFNULL(T1."OnHand",0) "InWhs", Ifnull(c."Price", 0) * :exchangeRate "BasePrice", c."Currency" "BaseCurrency", ifnull(c."Rate", 0) "Rate" 
			 from OITM a 
				LEFT JOIN (select x1."ItemCode", x1."Price", x1."Currency", x2."Rate" from (select "ItemCode", "Price", "Currency" from ITM1 where :priceList >= 0 and "PriceList" = :priceList
																				union all 
																					select "ItemCode", "LastPurPrc", "LastPurCur" from OITM where :priceList = -1
																				union all
																					select "ItemCode", "LstEvlPric", '' from OITM where :priceList = -2) x1 
										LEFT JOIN (select "Rate", "Currency" from ORTT where cast("RateDate" as timestamp) = CURRENT_DATE) x2 ON x1."Currency" = x2."Currency") c ON c."ItemCode" = a."ItemCode"
				Left Join OITW t1 on a."ItemCode" = t1."ItemCode" and CASE WHEN  a."DfltWH" <>'' THEN a."DfltWH" else :WhsDefault end = t1."WhsCode"
				left join UGP1 b on a."UgpEntry" = b."UgpEntry" and b."UomEntry" in (select "UomEntry" from OUOM where "UomName" = a."InvntryUom")
                                    where a."ItemCode" = :itemCode;
END




