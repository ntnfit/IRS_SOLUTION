
CREATE PROCEDURE APZ_Inventory_Bathch_BatchDetails_GetQuantities (
	IN 	itemCode NVARCHAR(50) DEFAULT ''
,		sysNumber INT DEFAULT 0
)
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER
READS SQL DATA
AS
BEGIN
	SELECT a."AbsEntry", a."WhsCode", b."Quantity", b."CommitQty", (b."Quantity" - b."CommitQty") AS "AvailQty", a."Location" 
		FROM OBTW a 
		LEFT OUTER JOIN OBTQ b ON a."MdAbsEntry" = b."MdAbsEntry" AND a."WhsCode" = b."WhsCode" 
		WHERE a."ItemCode" = :itemCode 
			AND a."SysNumber" = :sysNumber;
END;




