
-- apz_inventorybin_allbin 'HB01', 'S02', 152, 1
create procedure apz_inventorybin_allbin(
	in whsCode nvarchar(8) default '',
	in itemCode nvarchar(50) default '',
	in batchSerialAbs int default 0,
	in tranType int default -1,
	in binCode nvarchar(50) default ''
)
AS 
BEGIN
		IF :tranType = 0 or :tranType = 3 or :tranType = -1 or ifnull(:batchSerialAbs,0) = 0
		then
			SELECT "AbsEntry", "BinCode", "WhsCode" FROM OBIN WHERE IFNULL("Disabled", 'N') = 'N' AND (IFNULL(:whsCode, '') = '' OR "WhsCode" = :whsCode) AND (IFNULL(:binCode, '') = '' OR "BinCode" = :binCode);
		ELSE
		
			SELECT a."AbsEntry", a."BinCode", a."WhsCode", c."DistNumber", b."OnHandQty", c."MnfSerial", c."LotNumber" FROM OBIN a 
										INNER JOIN (SELECT "OnHandQty", "BinAbs", "ItemCode", "SnBMDAbs" FROM OBBQ WHERE (IFNULL(:itemCode, '') = '' OR "ItemCode" = :itemCode) 
													AND (IFNULL(:whsCode, '') = '' OR "WhsCode" = :whsCode)
													AND (IFNULL(:batchSerialAbs, 0) = 0 OR "SnBMDAbs" = :batchSerialAbs) 
												UNION ALL SELECT "OnHandQty", "BinAbs", "ItemCode", "SnBMDAbs" FROM OSBQ WHERE (IFNULL(:itemCode, '') = '' OR "ItemCode" = :itemCode) 
													AND (IFNULL(:whsCode, '') = '' OR "WhsCode" = :whsCode) 
													AND (IFNULL(:batchSerialAbs, 0) = 0 OR "SnBMDAbs" = :batchSerialAbs)) AS b ON a."AbsEntry" = b."BinAbs" 
										INNER JOIN (SELECT "AbsEntry", "SysNumber", "DistNumber", "LotNumber", "MnfSerial" FROM OBTN WHERE IFNULL(:itemCode, '') = '' OR "ItemCode" = :itemCode 
												UNION ALL SELECT "AbsEntry", "SysNumber", "DistNumber", "LotNumber", "MnfSerial" FROM OSRN WHERE IFNULL(:itemCode, '') = '' OR "ItemCode" = :itemCode) AS c 
											ON b."SnBMDAbs" = c."AbsEntry" WHERE IFNULL(:binCode, '') = '' OR "BinCode" = :binCode;

		end if;
END




