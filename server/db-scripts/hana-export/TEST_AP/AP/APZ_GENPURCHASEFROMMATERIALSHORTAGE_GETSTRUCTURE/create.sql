
create procedure APZ_GenPurchaseFromMaterialShortage_GetStructure
as
begin
	-- Khai bao bang Structure
		create local temporary table #tblStructure
            (
             "ColumnName" NVARCHAR(50) NOT NULL
            ,"Caption" NVARCHAR(100) NULL
            ,"DataType" VARCHAR(20) NULL DEFAULT 'nvarchar'  --int smallint decimal money float datetime date numeric
            ,"Width" INT NULL DEFAULT 75
            ,"GroupIndex" INT NULL DEFAULT -1	-- dùng cho grid  default -1
            ,"Visible" tinyint NULL DEFAULT 1
            ,"FormatType" NVARCHAR(20) NULL   -- numeric, datetime
            ,"DisplayFormat" NVARCHAR(20) NULL  -- '{0:n2}'
            ,"ReadOnly" tinyint NULL DEFAULT 0
            ,"AllowEdit" tinyint NULL DEFAULT 1
            ,"AllowMove" tinyint NULL DEFAULT 1
            ,"AllowSize" tinyint NULL DEFAULT 1
            ,"AllowSort" tinyint NULL DEFAULT 1
            ,"Fixed" VARCHAR(1) NULL	-- L:Left / R:Right
			-- Summary Item
            ,"SummaryType" VARCHAR(10) NULL	-- sum,average...
			-- Group Summary
            ,"GroupSummaryType" VARCHAR(10) NULL	-- sum,average...
            );
            INSERT INTO #tblStructure 
						("ColumnName",			"Caption",				"DataType",		"Width",	"GroupIndex", "Visible", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed")
			select 		 N'Dscription',		N'Item Name',			'string',		150,		-1,				1,			1,				0,			1,			1,			1,		 'L' from dummy
			union select N'CardCode',		N'Vendor',				'string',		100,		-1,				1,			0,				1,			1,			1,			1,		 'L' from dummy 
			union select N'CardName',		N'Vendor Name',			'string',		150,		-1,				1,			1,				0,			1,			1,			1,		 'L' from dummy
			union select N'ItemCode',		N'Item Code',			'string',		100,		-1,				1,			1,				0,			1,			1,			1,		 'L' from dummy
			union select N'SubCatNum',		N'BP Catalog No.',		'string',		150,		-1,				1,			1,				0,			1,			1,			1,		 '' from dummy
			union select N'SlpCode',		N'Buyer',				'int',			150,		-1,				1,			0,				1,			1,			1,			1,		 '' from dummy
			union select N'ShipDate',		N'ReqDate',				'datetime',		150,		-1,				1,			1,				0,			1,			1,			1,		 '' from dummy
			union select N'MrpOpenQty',		N'Requirement Open Qty','qty',			100,		-1,				1,			1,				0,			1,			1,			1,		 '' from dummy
			union select N'Quantity',		N'Quantity',			'qty',			100,		-1,				1,			0,				1,			1,			1,			1,		 '' from dummy
			union select N'InvQty',			N'Inventory Qty',		'qty',			100,		-1,				1,			1,				0,			1,			1,			1,		 '' from dummy
			union select N'WhsCode',		N'Whs Code',			'string',		150,		-1,				1,			0,				1,			1,			1,			1,		 '' from dummy
			union select N'PriceBefDi',		N'Price Before Discount','price',		100,		-1,				1,			0,				1,			1,			1,			1,		 '' from dummy
			union select N'DiscPrcnt',		N'DiscPrcnt',			'percent',		100,		-1,				1,			0,				1,			1,			1,			1,		 '' from dummy
			union select N'Remark',			N'Remarks',				'string',		150,		-1,				1,			0,				1,			1,			1,			1,		 '' from dummy
			union select N'Series',			N'Series',				'int',			100,		-1,				1,			0,				1,			1,			1,			1,		 '' from dummy
			union select N'BplID',			N'Branch',				'int',			150,		-1,				1,			0,				1,			1,			1,			1,		 '' from dummy
			union select N'UomEntry',		N'Uom Group',			'string',		150,		-1,				1,			1,				0,			1,			1,			1,		 '' from dummy
			union select N'unitMsr',		N'UoM Name',			'string',		100,		-1,				1,			0,				1,			1,			1,			1,		 '' from dummy
			union select N'UseBaseUn',		N'Inventory Uom',		'string',		100,		-1,				1,			0,				1,			1,			1,			1,		 '' from dummy
			union select N'NumPerMsr',		N'Item Per Unit',		'unit',			150,		-1,				1,			1,				0,			1,			1,			1,		 '' from dummy
			union select N'UomCode',		N'UoM Code',			'string',		150,		-1,				1,			0,				1,			1,			1,			1,		 '' from dummy
			union select N'TaxCode',		N'Tax Code',			'string',		150,		-1,				1,			0,				1,			1,			1,			1,		 '' from dummy
			union select N'U_OriginType',	N'Origin Type',			'string',		150,		-1,				1,			1,				0,			1,			1,			1,		 '' from dummy
			union select N'U_OriginCode',	N'Origin Code',			'int',			150,		-1,				1,			1,				0,			1,			1,			1,		 '' from dummy
			union select N'U_OriginLine',	N'Origin Line',			'int',			100,		-1,				1,			1,				0,			1,			1,			1,		 '' from dummy;


		SELECT * FROM #tblStructure;
		drop table #tblStructure;
end;




