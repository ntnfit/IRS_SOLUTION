CREATE PROCEDURE APZ_Outsourcing_GetAllBinData(in itemCode nvarchar(50) default '', in whsCode nvarchar(50) default '')
AS
BEGIN
	-- Get columns issue
	IF :ItemCode='-1'
	THEN
		SELECT '' AS "DistNumber", '' AS "FromBinCode", 0.0 AS "AvailQty", 0.0 AS "Quantity", 0 AS "LineNum", 0 AS "BtSAbsEntry", 0 AS "FromBinAbs", '' AS "MnfSerial", '' AS "LotNumber", 
			(SELECT "BinActivat" FROM OWHS WHERE "WhsCode" = :WhsCode) AS "BinActivat",'' AS "U_ORIBatch",'' AS "U_ORIBatch_List", Cast(null as nvarchar(50)) as "U_Father1", Cast(null as timeStamp) as "U_InDate_Father1"
			,Cast(null AS TimeStamp) AS "InDate", Cast(null AS TimeStamp) As "MnfDate", Cast(null AS TimeStamp) AS "ExpDate", Cast(null AS Nvarchar(200)) As "Notes"
			, Cast(null AS Nvarchar(50)) AS "SysNumber", Cast(null AS Int) AS "U_CreatedTime",  Cast(null AS Nvarchar(10)) AS "U_SSCC_Status"
		FROM DUMMY;
	ELSEIF :ItemCode='-2'
	THEN
	-- get columns receipt
		SELECT '' AS "DistNumber", '' AS "ToBinCode", 0.0 AS "Quantity", 0 AS "LineNum", 0 AS "BtSAbsEntry", 0 AS "ToBinAbs", '' AS "MnfSerial", '' AS "LotNumber",
			(SELECT "BinActivat" FROM OWHS WHERE "WhsCode" = :WhsCode) AS "BinActivat",'' AS "U_ORIBatch",'' AS "U_ORIBatch_List", Cast(null as nvarchar(50)) as "U_Father1", Cast(null as timeStamp) as "U_InDate_Father1"
			,Cast(null AS TimeStamp) AS "InDate", Cast(null AS TimeStamp) As "MnfDate", Cast(null AS TimeStamp) AS "ExpDate", Cast(null AS Nvarchar(200)) As "Notes"
			, Cast(null AS Nvarchar(50)) AS "SysNumber", Cast(null AS Int) AS "U_CreatedTime",  Cast(null AS Nvarchar(10)) AS "U_SSCC_Status"
		FROM DUMMY;
	ELSE
	-- get issue bins data
		SELECT a."DistNumber", "BinCode" AS "FromBinCode", "Instock" AS "AvailQty", 0.0 AS "Quantity", 0 AS "LineNum", "AbsEntryBatch" AS "BtSAbsEntry", 
			a."BinAbs" AS "FromBinAbs",a."MnfSerial", a."LotNumber", b."BinActivat" AS "BinActivat",c."U_ORIBatch",c."U_ORIBatch_List",c."U_Father1",c."U_InDate_Father1"
			, c."InDate",  c."MnfDate",  c."ExpDate", c."Notes", c."SysNumber", c."U_CreatedTime", c."U_SSCC_Status"
		FROM APZ_OITW_LOCATION a 
			INNER JOIN OWHS b ON a."WhsCode" = b."WhsCode"
			LEFT JOIN OBTN c On a."DistNumber" = c."DistNumber" And a."ItemCode" = c."ItemCode"
		WHERE a."WhsCode" = :WhsCode AND a."ItemCode" = :ItemCode
		Order By a."DistNumber";
	END IF;
END;
