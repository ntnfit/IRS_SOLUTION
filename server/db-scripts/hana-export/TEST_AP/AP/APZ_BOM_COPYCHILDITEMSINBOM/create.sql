
CREATE PROCEDURE APZ_BOM_CopyChildItemsInBom
(
	IN ItemCode nvarchar(50),
	IN BaseType int,
	IN BaseEntry int,
	IN BaseRow int
)
AS
BEGIN
	IF :BaseType=17 THEN
		declare visOrder int;
		declare baseQty numeric(19,6);
		declare maxVisOrder int;
		declare treeType char(1) = '';
		declare tblVisOrder table ("VisOrder" int);
		
		select "VisOrder", "Quantity" into visOrder, baseQty default -1, null from RDR1 where "DocEntry" = :BaseEntry and "LineNum" = :BaseRow and "ItemCode" = :ItemCode;

		maxVisOrder := ifnull((select max("VisOrder") from RDR1 where "DocEntry" = :BaseEntry), -1);
		
		while :visOrder < :maxVisOrder do
			visOrder = visOrder + 1;
			treeType = ifnull((select "TreeType" from RDR1 where "DocEntry" = :BaseEntry and "VisOrder" = :visOrder), '');
			if :treeType='I' then
				insert into :tblVisOrder values (:visOrder);
			end if;
			if :treeType <> 'I' then
				break;
			end if;
		end while;
		
		SELECT 
			t0.*, 
			t0."Dscription" AS "ItemName", 
			t0."WhsCode" AS "DefaultWarehouse", 
			t0."VatPrcnt" AS "RateSa", 
			0 AS "OnHand", 
			ROW_NUMBER() OVER (ORDER BY t0."ItemCode") AS "ChildNum", 
			t0."unitMsr" AS "SalUnitMsr", 
			t0."unitMsr" AS "BuyUitMsr", 
			t0."NumPerMsr" AS "NumInBuy", 
			T0."NumPerMsr" AS "NumInSale", 
			-1 AS "UgpCode", 
			t0."UseBaseUn" AS "InvntryUom", 
			0 AS "NumInInven", 
			t0."WhsCode" AS "Warehouse", 
			t0."FreeTxt" AS "Comment", 
			t0."LineNum" AS "BaseRow", 
			:baseQty AS "BaseQuantity" 
			FROM RDR1 T0 WHERE "DocEntry" = :BaseEntry AND "VisOrder" IN (SELECT "VisOrder" FROM :tblVisOrder) 
			ORDER BY t0."VisOrder";
		
	END IF;

END




