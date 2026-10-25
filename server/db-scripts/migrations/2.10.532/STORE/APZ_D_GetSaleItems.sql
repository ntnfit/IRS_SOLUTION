
CALL APZ_EXISTSPROC('APZ_D_GetSaleItems');

GO


CREATE PROCEDURE APZ_D_GetSaleItems
(
	IN UserName NVARCHAR(50) DEFAULT '',
	IN CardCode NVARCHAR(50) DEFAULT '',
	IN DocDate DATETIME DEFAULT null
)
AS
BEGIN

		DECLARE WhsDefault VARCHAR(20);
		DECLARE PriceList INT = 1;
		declare fromDate datetime; 
		declare toDate datetime; 
		select add_days(current_date, -1) into fromDate from dummy;
		select add_days(current_date, 1) into toDate from dummy;
		
		WhsDefault = IFNULL((SELECT "Warehouse" FROM OUDG WHERE "Code"= (SELECT TOP 1 "DfltsGroup" FROM OUSR WHERE USER_CODE=:userName)),'');
		
		create local temporary table #tblPrice ("ItemCode" nvarchar(50), "Price" decimal, "Currency" nvarchar(3), "Discount" decimal, "CardCode" nvarchar(15), "IsGrossPrc" NVARCHAR(1));
		
		insert into #tblPrice select a."ItemCode",ifnull(b."Price",a."Price"), a."Currency", a."Discount", a."CardCode", c."IsGrossPrc" 
		from OSPP a
		left join SPP1 b on b."ItemCode" = a."ItemCode" and b."CardCode" = a."CardCode" and :docdate between b."FromDate" and ifnull(b."ToDate",'2099-12-31')
		LEFT JOIN OPLN c ON a."ListNum" = c."ListNum"
		where a."CardCode" = :CardCode;
		
		IF(NOT EXISTS (SELECT 1 FROM #tblPrice)) THEN
		
			INSERT INTO #tblPrice ("ItemCode", "Price", "Currency", "Discount", "CardCode", "IsGrossPrc") 
			SELECT a."ItemCode", a."Price", a."Currency", 0 AS "Discount", b."CardCode", c."IsGrossPrc" 
			FROM ITM1 a INNER JOIN OCRD b ON a."PriceList" = b."ListNum" AND b."CardCode" = :CardCode INNER JOIN OPLN c ON a."PriceList" = c."ListNum";
		
		END IF;


        SELECT A."ItemCode", 
		A."ItemName" AS "ItemDescription", 
		A."CodeBars",
		A."ItemName" AS "ItemName",
		M."Substitute",
		ifnull(A."FrgnName", '') AS "FrgnName", 
		ifnull(A."ItmsGrpCod", 0) AS "ItmsGrpCod",
        CASE WHEN ifnull(OCRD."ECVatGroup",'') ='' then A."VatGourpSa" ELSE OCRD."ECVatGroup" end  AS "SalesVATGroup",
		CASE WHEN ifnull(OCRD."ECVatGroup",'') ='' THEN A."VatGroupPu" ELSE OCRD."ECVatGroup" end  AS "VatGroupPu", 
		CASE WHEN ifnull(OCRD."ECVatGroup",'') ='' THEN A."VatGourpSa" ELSE OCRD."ECVatGroup" end  AS "VatGourpSa",
		CASE WHEN :WhsDefault <>'' THEN :WhsDefault else  a."DfltWH" end AS "WarehouseCode",
		CASE WHEN :WhsDefault <>'' THEN :WhsDefault else  a."DfltWH" end AS "DefaultWarehouse",
		CASE WHEN :WhsDefault <>'' THEN :WhsDefault else  a."DfltWH" end AS "dfltWH",
        --(SELECT SUM(ifnull(OnHand, 0) - ifnull(IsCommited, 0) + ifnull(OnOrder, 0))FROM OITW WHERE ItemCode = A.ItemCode  AND Locked = 'N') AS QuantityOnStock,  
        (SELECT SUM(ifnull("OnHand", 0) - ifnull("IsCommited", 0) + ifnull("OnOrder", 0))FROM OITW WHERE "ItemCode" = A."ItemCode"  AND "Locked" = 'N' AND (:WhsDefault = '' OR(:WhsDefault <>'' AND "WhsCode" = :WhsDefault ))) AS "QuantityOnStock",  
		OCRD."ListNum",
		CASE WHEN ifnull(OCRD."ECVatGroup",'') ='' THEN ifnull(k."Rate", 0) ELSE ovtg."Rate" end  AS "RatePu", 
		CASE WHEN ifnull(OCRD."ECVatGroup",'') ='' THEN ifnull(g."Rate", 0) ELSE ovtg."Rate" end  AS "RateSa",
		ifnull(l."TreeType",'N') as "TreeType", 
		ifnull(a."TreeQty",1) as "TreeQty", 
		ifnull(k."Inactive", '') AS "VatPuInActive",
        ifnull(g."Inactive", '') AS "VatSaInActive",
		A."BuyUnitMsr", 
		A."SalUnitMsr",
		A."NumInBuy",
		A."NumInSale",
		A."InvntryUom",
		a."UgpEntry" as "UgpCode",
		ifnull(a1."AltQty", 1) / ifnull(a1."BaseQty", 1) as "NumInInven",
		ifnull(p."Price",0) "Price",
		ifnull(p."IsGrossPrc",'N') "IsGrossPrc",
		ifnull(p."Currency",'') "Currency",
		ifnull(p."Discount",0) "Discount",
		(select ifnull("LinkThumbnails", '') from 
							(select "LinkThumbnails", row_number() over(order by "UploadDate", "UploadTime" desc) as row_id from APZ_D_UUPIM where "ObjectCode" = A."ItemCode" and "ObjectType" = '810039') 
				where row_id = 1) as "AvatarImage"
            FROM OITM A
			--INNER JOIN OBCD T1 on A.ItemCode = T1.ItemCode and T1.BcdCode = A.CodeBars
            LEFT JOIN OSHP D ON A."ShipType" = D."TrnspCode"
            LEFT JOIN OMRC E ON A."FirmCode" = E."FirmCode"
            LEFT JOIN OITB f ON A."ItmsGrpCod" = f."ItmsGrpCod"
            LEFT JOIN OVTG g ON A."VatGourpSa" = g."Code"
            LEFT JOIN OVTG k ON A."VatGroupPu" = k."Code"
            LEFT JOIN OITT l ON l."Code" = A."ItemCode"
			LEFT JOIN OSCN M on a."ItemCode" = M."ItemCode" and M."CardCode" = :CardCode
			left join UGP1 a1 on a."UgpEntry" = a1."UgpEntry" and a1."UomEntry" in (select "UomEntry" from OUOM where "UomName" = a."InvntryUom")
			LEFT JOIN OCRD 
			LEFT JOIN OVTG ON OCRD."ECVatGroup"=OVTG."Code" ON OCRD."CardCode"=:CardCode
			LEFT JOIN #tblPrice p ON a."ItemCode"=p."ItemCode" AND p."CardCode"=:CardCode
			
			WHERE ((A."validFor" = 'Y' and ifnull(A."frozenFor",'N') = 'N' and (ifnull(a."validFrom", :fromDate) <= current_date and ifnull(a."validTo", :toDate) >= current_date)) 
		OR (ifnull(A."validFor",'N') = 'N' and A."frozenFor" = 'Y' and (ifnull(a."frozenFrom", :fromDate) > current_date or ifnull(a."frozenTo", :toDate) < current_date))
		or (A."validFor" = 'Y' and A."frozenFor" = 'Y' and (ifnull(a."validFrom", :fromDate) <= current_date and ifnull(a."validTo", :toDate) >= current_date) and
					(ifnull(a."frozenFrom", :fromDate) > current_date or ifnull(a."frozenTo", :toDate) < current_date))
		OR (A."validFor" = 'N' AND A."frozenFor" = 'N')) and A."SellItem" = 'Y';

END