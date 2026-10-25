
create procedure APZ_WareHouse_DefaultTable()
As
Begin
CREATE local temporary TABLE #tbl AS (SELECT TOP 0 IFNULL("WhsCode", '') AS "WarehouseCode", IFNULL("WhsName", '') AS "WarehouseName", IFNULL("Location", -1) AS "Location", IFNULL("Building", '') AS "BuildingFloorRoom", IFNULL("Street", '') AS "Street", IFNULL("Block", '') AS "Block", IFNULL("ZipCode", '') AS "ZipCode", IFNULL("City", '') AS "City", IFNULL("County", '') AS "County", IFNULL("FedTaxID", '') AS "FederalTaxID", IFNULL("objType", '') AS "ObjTypeHistory", IFNULL("Locked", 'N') AS "Locked", IFNULL("DropShip", 'N') AS "DropShips", IFNULL("Nettable", 'Y') AS "Nettables", IFNULL("BPLid", -1) AS "BPLid", IFNULL("AddrType", '') AS "AddressType", IFNULL("StreetNo", '') AS "StreetNo", IFNULL("Country", '') AS "Country", IFNULL("State", '') AS "State", IFNULL("GlblLocNum", '') AS "GlblLocNum", IFNULL("BinActivat", 'N') AS "BinActivat", IFNULL("BinSeptor", '-') AS "BinSeptor", IFNULL("RecBinEnab", 'N') AS "RecBinEnab", IFNULL("DftBinEnfd", 'N') AS "DftBinEnfd", IFNULL("AutoIssMtd", 0) AS "AutoIssMtd", IFNULL("AutoRecvMd", 0) AS "AutoRecvMd", IFNULL("RecvMaxQty", 'N') AS "RecvMaxQty", IFNULL("RecvMaxWT", 'N') AS "RecvMaxWT" FROM OWHS);
INSERT INTO #tbl ("WarehouseCode", "WarehouseName", "Location", "BuildingFloorRoom", "Street", "Block", "ZipCode", "City", "County", "FederalTaxID", "ObjTypeHistory", "Locked", "DropShips", "Nettables", "BPLid", "AddressType", "StreetNo", "Country", "State", "GlblLocNum", "BinActivat", "BinSeptor", "RecBinEnab", "DftBinEnfd", "AutoIssMtd", "AutoRecvMd", "RecvMaxQty", "RecvMaxWT") VALUES ('', '', -1, '', '', '', '', '', '', '', '', 'N', 'N', 'Y', -1, '', '', '', '', '', 'N', '-', 'N', 'N', 0, 0, 'N', 'N');


select * from #tbl;

drop table #tbl;

End;









