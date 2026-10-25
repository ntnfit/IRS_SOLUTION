
CREATE PROCEDURE APZ_Inventory_BatchTransaction_GetDoc (
	IN 	itemCode NVARCHAR(30) DEFAULT ''
,		sysNumber NVARCHAR(20) DEFAULT ''
,		locCode NVARCHAR(8) DEFAULT ''
)

AS
BEGIN
	
	SELECT b."TransId", CASE WHEN IFNULL(b."BaseType", -1) = 202 AND b."ApplyType" = 60 THEN -1 
				WHEN IFNULL(b."BaseType", -1) = 202 AND b."ApplyType" = 59 THEN -2 
				ELSE b."ApplyType" END AS "DocType", 
	"AppDocNum" AS "Document", b."ApplyLine" "DocLine", b."DocDate", b."LocCode", b."CardName",
	sum(a."Quantity") AS "Qty", sum(a."AllocQty") "AllocQty", b."ApplyEntry" "DocEntry", 
				CASE WHEN sum(a."Quantity") > 0 then 'IN'
				when sum(a."Quantity") < 0 then 'OUT'
				else 'Allocation' end "Direction", 
				IFNULL(b."BaseType", -1) AS "BaseType" 
			FROM ITL1 a 
			JOIN OITL b ON a."LogEntry" = b."LogEntry"
	where b."ItemCode"=:itemCOde and (b."LocCode"=:locCode OR IFNULL(:locCode, '') = '') and a."SysNumber"=:sysNumber
	group by b."TransId", b."ApplyType", b."ApplyEntry", b."ApplyLine", b."BaseType",
	b."DocDate", b."LocCode", b."CardName", b."DocEntry", b."AppDocNum"
	having sum(a."Quantity") <> 0 or sum(a."AllocQty") <> 0;

END;




