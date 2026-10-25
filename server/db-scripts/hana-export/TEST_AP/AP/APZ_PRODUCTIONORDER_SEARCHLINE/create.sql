
-- Apz_ProductionOrder_SearchLine 397
CREATE PROCEDURE Apz_ProductionOrder_SearchLine(in DocEntry INT)
AS
BEGIN
    SELECT  a.* ,b."ItemName",b."IsCommited",b."OnOrder",b."OnHand", c."OnHand"+c."OnOrder"-c."IsCommited" AS "Available",b."InvntryUom",c."OnHand" "OnHandInWhse",c."IsCommited" "IsCommitedInWhse"
    	,c."OnOrder" "OnOrderInWhse", case when b."ManBtchNum" = 'Y' then 'B' when b."ManSerNum" = 'Y' then 'S' else 'N' end "ManBtchSerial",
    	a."PlannedQty" "OldLineQty", d."PlannedQty" "OldFatherQty"
    FROM    WOR1 a
            INNER JOIN OITM b ON a."ItemCode" = b."ItemCode" AND a."ItemType"=4
			INNER JOIN OITW c ON a."ItemCode"=c."ItemCode" AND a."wareHouse"=c."WhsCode"
			JOIN OWOR d ON a."DocEntry"=d."DocEntry"
    WHERE   a."DocEntry" = :DocEntry
	UNION ALL
    SELECT  a.* ,b."ResName" AS "ItemName" ,0 AS "IsCommited" ,0 AS "OnOrder",0 "OnHand" ,0 AS "Available",'' AS "InvntryUom",0,0,0, 'N',a."PlannedQty",c."PlannedQty"
    FROM    WOR1 a
            INNER JOIN ORSC b ON a."ItemCode" = b."ResCode" AND a."ItemType"=290
            JOIN OWOR c ON a."DocEntry"=c."DocEntry"
	WHERE a."DocEntry"=:DocEntry
	UNION ALL
	SELECT *,null,null,null,null ,null,null,null,null,null,null,null,null from WOR1 where "DocEntry" = :DocEntry and "ItemType" = -18
	order by "VisOrder" asc;
END

