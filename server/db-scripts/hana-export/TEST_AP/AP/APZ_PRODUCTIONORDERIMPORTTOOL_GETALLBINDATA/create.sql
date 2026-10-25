
CREATE PROCEDURE APZ_ProductionOrderImportTool_GetAllBinData(in ItemCode nvarchar(50) default '', in WhsCode nvarchar(50) default '')
AS
BEGIN
	-- Get columns
	if :ItemCode='-1'
	THEN
		SELECT '' AS "DistNumber", '' AS "FromBinCode", 0.0 AS "AvailQty", 0.0 AS "Quantity", 0 AS "ToBin", 0 AS "DocEntry", 0 AS "LineNum", 0 AS "BtSAbsEntry", 0 AS "FromBin"
			, (SELECT "BinActivat" FROM OWHS WHERE "WhsCode" = :WhsCode) AS "FromBinActivat", '' AS "ToBinActivat" FROM DUMMY;
	else
	-- get bins data
		SELECT "DistNumber", "BinCode" AS "FromBinCode", "Instock" AS "AvailQty", 0.0 AS "Quantity", 0 AS "ToBin", 0 AS "DocEntry", 0 AS "LineNum", "AbsEntryBatch" AS "BtSAbsEntry", "BinAbs" AS "FromBin"
		, b."BinActivat" AS "FromBinActivat", '' AS "ToBinActivat" FROM APZ_OITW_LOCATION a INNER JOIN OWHS b ON a."WhsCode" = b."WhsCode" WHERE a."WhsCode" = :WhsCode AND "ItemCode" = :ItemCode;
	END IF;
END



