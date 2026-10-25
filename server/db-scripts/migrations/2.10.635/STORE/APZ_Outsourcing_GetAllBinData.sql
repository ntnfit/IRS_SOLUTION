
CALL APZ_EXISTSPROC('APZ_Outsourcing_GetAllBinData');

GO

CREATE PROCEDURE APZ_Outsourcing_GetAllBinData(in itemCode nvarchar(50) default '', in whsCode nvarchar(50) default '')
AS
BEGIN
	-- Get columns issue
	IF :ItemCode='-1'
	THEN
		SELECT '' AS "DistNumber", '' AS "FromBinCode", 0.0 AS "AvailQty", 0.0 AS "Quantity", 0 AS "LineNum", 0 AS "BtSAbsEntry", 0 AS "FromBinAbs", '' AS "MnfSerial", '' AS "LotNumber", 
			(SELECT "BinActivat" FROM OWHS WHERE "WhsCode" = :WhsCode) AS "BinActivat" 
		FROM DUMMY;
	ELSEIF :ItemCode='-2'
	THEN
	-- get columns receipt
		SELECT '' AS "DistNumber", '' AS "ToBinCode", 0.0 AS "Quantity", 0 AS "LineNum", 0 AS "BtSAbsEntry", 0 AS "ToBinAbs", '' AS "MnfSerial", '' AS "LotNumber",
			(SELECT "BinActivat" FROM OWHS WHERE "WhsCode" = :WhsCode) AS "BinActivat" 
		FROM DUMMY;
	ELSE
	-- get issue bins data
		SELECT "DistNumber", "BinCode" AS "FromBinCode", "Instock" AS "AvailQty", 0.0 AS "Quantity", 0 AS "LineNum", "AbsEntryBatch" AS "BtSAbsEntry", 
			a."BinAbs" AS "FromBinAbs",a."MnfSerial", a."LotNumber", b."BinActivat" AS "BinActivat" 
		FROM APZ_OITW_LOCATION a INNER JOIN OWHS b ON a."WhsCode" = b."WhsCode"
		WHERE a."WhsCode" = :WhsCode AND "ItemCode" = :ItemCode;
	END IF;
END;