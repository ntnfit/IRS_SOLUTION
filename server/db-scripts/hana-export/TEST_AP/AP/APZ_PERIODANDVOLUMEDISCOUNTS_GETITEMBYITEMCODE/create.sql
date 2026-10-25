
Create procedure APZ_PeriodAndVolumeDiscounts_GetItemByItemCode(in ItemCode nvarchar(50) default '',
	in ListNum int default 0) As 
Begin 
SELECT
	 DISTINCT 0 AS "SrcPrice",
	 a."ItemCode" AS "ItemCode",
	 a."ItemName" AS "ItemName",
	 b."AddPrice1" AS "AddPrice1",
	 b."AddPrice2" AS "AddPrice2",
	 c."UomName" AS "UomName" 
FROM oitm a 
LEFT OUTER JOIN itm1 b ON a."ItemCode" = b."ItemCode" 
LEFT OUTER JOIN OUOM c ON c."UomEntry" = a."INUoMEntry" 
WHERE a."ItemCode" = :ItemCode 
AND b."PriceList" = :ListNum
;
 
End 
;




