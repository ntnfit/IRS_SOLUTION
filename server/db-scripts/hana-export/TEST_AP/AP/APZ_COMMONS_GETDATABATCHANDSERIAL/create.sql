
CREATE PROCEDURE Apz_Commons_GetDataBatchAndSerial (
	IN absEntry INT DEFAULT -1,
	distNumber NVARCHAR(36) DEFAULT '',
	itemCode NVARCHAR(50) DEFAULT '',
	whsCode NVARCHAR(8) DEFAULT ''
)
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER

AS
BEGIN
	DECLARE ManBtchNum char(1) := '';
	DECLARE ManSerNum char(1) := '';
	
	SELECT "ManBtchNum", "ManSerNum" into ManBtchNum, ManSerNum  FROM OITM WHERE "ItemCode" = :itemCode;
	
	IF :ManBtchNum = 'Y' THEN
		if :absEntry > 0 then
			SELECT b.*, A."Quantity" "InStock", A."WhsCode", A."Quantity" "LogQty", A."CommitQty", 'B' "ManBatchSerial"
			from OBTQ a join OBTN b on a."MdAbsEntry" = b."AbsEntry" where b."AbsEntry" = :absEntry and a."ItemCode" = :itemCode and a."WhsCode" = :whsCode;
		else
			SELECT b.*, A."Quantity" "InStock", A."WhsCode", A."Quantity" "LogQty", A."CommitQty", 'B' "ManBatchSerial" 
				FROM (SELECT "Quantity", "WhsCode", "CommitQty", "SysNumber", "MdAbsEntry" FROM OBTQ WHERE "ItemCode" = :itemCode AND "WhsCode" = :whsCode) A 
				INNER JOIN 
					(SELECT * FROM OBTN WHERE (IFNULL(:distNumber, '') = '' OR "DistNumber" = :distNumber)
											AND "ItemCode" = :itemCode) b ON A."SysNumber" = b."SysNumber" AND A."MdAbsEntry" = b."AbsEntry" WHERE A."Quantity" > 0;
		end if;
		
	ELSEIF :ManSerNum = 'Y' THEN
		if :absEntry > 0 then
			SELECT b.*, A."Quantity" "InStock", A."WhsCode", A."Quantity" "LogQty", A."CommitQty", 'S' "ManBatchSerial"
			from OSRQ a join OSRN b on a."MdAbsEntry" = b."AbsEntry" where b."AbsEntry" = :absEntry and a."ItemCode" = :itemCode and a."WhsCode" = :whsCode;
		else
			SELECT b.*, A."Quantity" "InStock", A."WhsCode", A."Quantity" "LogQty", A."CommitQty", 'S' "ManBatchSerial" 
				FROM (SELECT "Quantity", "WhsCode", "CommitQty", "SysNumber", "MdAbsEntry" FROM OSRQ WHERE "ItemCode" = :itemCode AND "WhsCode" = :whsCode) AS A 
				INNER JOIN 
					(SELECT * FROM OSRN WHERE (IFNULL(:distNumber, '') = '' OR "DistNumber" = :distNumber)
											AND "ItemCode" = :itemCode) AS b ON A."SysNumber" = b."SysNumber" AND A."MdAbsEntry" = b."AbsEntry" WHERE A."Quantity" > 0;
		end if;	
	END IF;

END;



