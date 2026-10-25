
CREATE PROCEDURE Apz_Cfl_GetListWarehouse(
 WarehouseCode NVARCHAR(5000) default ''
,    WarehouseName VARCHAR(5000) default ''
--,   @userName NVARCHAR(20) default ''
,   branch NVARCHAR(20) default '')
AS
BEGIN
declare query nvarchar(5000);  
		-- Khai báo bảng Structure
		create local temporary table #tblStructure
	    (
	     "ColumnName" NVARCHAR(50) NOT NULL
	    ,"Caption" NVARCHAR(100) NULL
	    ,"DataType" VARCHAR(20) NULL DEFAULT 'nvarchar' --int decimal money datetime date/ numeric alpha Memo(general) Numeric Date float(unit)  ValidValue(combobox)
		,"SubType" VARCHAR(20) NULL			--none phone percentage address image measurement price quantity rate sum time
	    ,"Width" INT NULL DEFAULT 75
	    ,"GroupIndex" INT NULL DEFAULT -1	-- dng cho grid  default -1
	    ,"Visible" TINYINT NULL DEFAULT '1'
	    ,"FormatType" NVARCHAR(20) NULL   -- numeric, datetime/ 
	    ,"DisplayFormat" NVARCHAR(20) NULL  -- '{0:n2}'
	    ,"ReadOnly" TINYINT NULL DEFAULT '0'
	    ,"AllowEdit" TINYINT NULL DEFAULT '1'
	    ,"AllowMove" TINYINT NULL DEFAULT '1'
	    ,"AllowSize" TINYINT NULL DEFAULT '1'
	    ,"AllowSort" TINYINT NULL DEFAULT '1'
	    ,"Fixed" VARCHAR(1) NULL	-- L:Left / R:Right
		-- Summary Item
	    ,"SummaryType" VARCHAR(10) NULL	-- sum,average...
		-- Group Summary
	    ,"GroupSummaryType" VARCHAR(10) NULL	-- sum,average...
		--,Parent BIT NULL	-- dng cho treelist
		--,[Key] BIT NULL		-- dng cho treelist (identity)
	    );
           INSERT INTO #tblStructure ("ColumnName", "Caption", "DataType", "Width", "GroupIndex", "Visible", "FormatType", "DisplayFormat", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "SummaryType", "GroupSummaryType")
                select N'WhsCode',		N'WarehouseCode',	    'nvarchar',		75,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL FROM DUMMY
					   UNION SELECT N'WhsName',		N'WarehouseName',	    'nvarchar',		150,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL FROM DUMMY
					   UNION SELECT N'Location',		N'Location',			'int',			75,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL FROM DUMMY
					   UNION SELECT N'Building',		N'BuildingFloorRoom',   'nvarchar',	    75,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL FROM DUMMY
					   UNION SELECT N'Street',			N'Street',			    'nvarchar',		75,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL FROM DUMMY
					   UNION SELECT N'Block',			N'Block',				'nvarchar',		75,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL FROM DUMMY
					   UNION SELECT N'ZipCode',		N'ZipCode',				'nvarchar',		75,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL FROM DUMMY
					   UNION SELECT N'City',			N'City',				'nvarchar',		75,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL FROM DUMMY
					   UNION SELECT N'County',			N'County',				'nvarchar',		75,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL FROM DUMMY
					   UNION SELECT N'FedTaxID',		N'FederalTaxID',		'nvarchar',		75,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL FROM DUMMY
					   UNION SELECT N'objType',		N'ObjTypeHistory',		'nvarchar',		75,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL FROM DUMMY
					   UNION SELECT N'Locked',			N'Locked',				'nvarchar',		75,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL FROM DUMMY
					   UNION SELECT N'BPLid',			N'BPLid',				'int',			75,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL FROM DUMMY
					   UNION SELECT N'AddrType',		N'AddressType',			'nvarchar',		75,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL FROM DUMMY
					   UNION SELECT N'StreetNo',		N'StreetNo',			'nvarchar',		75,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL FROM DUMMY
					   UNION SELECT N'Country',		N'Country',				'nvarchar',		75,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL FROM DUMMY
					   UNION SELECT N'State',			N'State',				'nvarchar',		75,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL FROM DUMMY
					   UNION SELECT N'GlblLocNum',		N'GlblLocNum',			'nvarchar',		75,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL FROM DUMMY
					   UNION SELECT N'BinActivat',	    N'BinActivat',			'nvarchar',		75,		0,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL FROM DUMMY
					   UNION SELECT N'DropShip',	    N'DropShips',			'nvarchar',		75,		0,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL FROM DUMMY
					    UNION SELECT N'Nettable',	    N'Nettables',			'nvarchar',		75,		0,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL FROM DUMMY
						UNION SELECT N'Inactive',	    N'Inactive',			'nvarchar',		75,		0,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL FROM DUMMY;


		SELECT *
            FROM #tblStructure;
			 
	
	
	query := 'SELECT IFNULL("WhsCode", '''') AS "WhsCode", IFNULL("WhsName", '''') AS "WhsName", IFNULL("Location", -1) AS "Location", IFNULL("Building", '''') AS "BuildingFloorRoom", IFNULL("Street", '''') AS "Street", IFNULL("Block", '''') AS "Block", IFNULL("ZipCode", '''') AS "ZipCode", IFNULL("City", '''') AS "City", IFNULL("County", '''') AS "County", IFNULL("FedTaxID", '''') AS "FederalTaxID", IFNULL("objType", '''') AS "ObjTypeHistory", IFNULL("Locked", '''') AS "Locked", IFNULL("DropShip", '''') AS "DropShips", IFNULL("Nettable", '''') AS "Nettables", IFNULL("BPLid", 0) AS "BPLid", IFNULL("AddrType", '''') AS "AddressType", IFNULL("StreetNo", '''') AS "StreetNo", IFNULL("Country", '''') AS "Country", IFNULL("State", '''') AS "State", IFNULL("GlblLocNum", '''') AS "GlblLocNum", IFNULL("BinActivat", '''') AS "BinActivat", "Inactive" FROM OWHS WHERE IFNULL("Inactive", '''') <> ''Y'' ';
IF :WareHouseCode <> '' AND :WareHouseCode IS NOT NULL THEN query := :query || 'AND "WhsCode" = N''' || :WareHouseCode || '''';
END IF;
IF :WareHouseName <> '' AND :WareHouseName IS NOT NULL THEN query := :query || 'AND "WhsName" like N''%' || :WareHouseName || '%''';
--if @userName <> '' or @userName <> null
--	set @query=@query + 'AND UserSign = ' + @userName + ''
END IF;
IF :branch <> '' AND :branch IS NOT NULL AND :branch <> '-1' THEN query := :query || ' AND "BPLid" = ' || :branch || '';
END IF;
execute immediate(:query || ';');
 Drop table #tblStructure;
END;





