

CREATE procedure APZ_GENPURCHASEFROMSCHEDULE_GETSTRUCTURE 
as
BEGIN
create local temporary table #tblStructure
	    (
	     "ColumnName" NVARCHAR(50) NOT NULL
	    ,"Caption" NVARCHAR(100) NULL
	    ,"DataType" VARCHAR(20) NULL DEFAULT 'nvarchar' --int decimal money datetime date/ numeric alpha Memo(general) Numeric Date float(unit)  ValidValue(combobox)
	    ,"Width" INT NULL DEFAULT 75
	    ,"GroupIndex" INT NULL DEFAULT -1	-- dng cho grid  default -1
	    ,"Visible" TINYINT NULL DEFAULT '1'
	    ,"ReadOnly" TINYINT NULL DEFAULT '0'
	    ,"AllowEdit" TINYINT NULL DEFAULT '1'
	    ,"AllowMove" TINYINT NULL DEFAULT '1'
	    ,"AllowSize" TINYINT NULL DEFAULT '1'
	    ,"AllowSort" TINYINT NULL DEFAULT '1'
	    ,"Fixed" VARCHAR(1) NULL	-- L:Left / R:Right
	    );
   INSERT INTO #tblStructure ("ColumnName", "Caption", "DataType", "Width", "GroupIndex", "Visible", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed")
	      		     select N'Dscription',		N'Item Name',			'string',		150,		-1,				1,			1,				0,			1,			1,			1,		 'L' from dummy
			   union select N'CardCode',		N'Vendor',				'string',		100,		-1,				1,			0,				1,			1,			1,			1,		 'L' from dummy
			   union select N'CardName',		N'Vendor Name',			'string',		150,		-1,				1,			1,				0,			1,			1,			1,		 'L' from dummy
			   union select N'ItemCode',		N'Item Code',			'string',		100,		-1,				1,			1,				0,			1,			1,			1,		 'L' from dummy
			   union select N'SubCatNum',		N'BP Catalog No.',		'string',		150,		-1,				1,			1,				0,			1,			1,			1,		 '' from dummy
			   union select N'SlpCode',		N'Buyer',				'int',			150,		-1,				1,			0,				1,			1,			1,			1,		 '' from dummy
			   union select N'DocDate',		N'Posting Date',		'datetime',		150,		-1,				1,			1,				0,			1,			1,			1,		 '' from dummy
			   union select N'ShipDate',		N'ShipDate',			'datetime',		150,		-1,				1,			1,				0,			1,			1,			1,		 '' from dummy
			   union select N'ReqDate',		N'ReqủieDate',			'datetime',		150,		-1,				1,			1,				0,			1,			1,			1,		 '' from dummy
			   union select N'MrpOpenQty',		N'Requirement Open Qty','qty',			100,		-1,				1,			1,				0,			1,			1,			1,		 '' from dummy
			   union select N'Quantity',		N'Quantity',			'qty',			100,		-1,				1,			0,				1,			1,			1,			1,		 '' from dummy
			   union select N'InvQty',			N'Inventory Qty',		'qty',			100,		-1,				1,			1,				0,			1,			1,			1,		 '' from dummy
			   union select N'WhsCode',		N'Whs Code',			'string',		150,		-1,				1,			0,				1,			1,			1,			1,		 '' from dummy
			   union select N'PriceBefDi',		N'Price Before Discount','price',		100,		-1,				1,			0,				1,			1,			1,			1,		 '' from dummy
			   union select N'DiscPrcnt',		N'DiscPrcnt',			'percent',		100,		-1,				1,			0,				1,			1,			1,			1,		 '' from dummy
			   union select N'Remark',			N'Remarks',				'string',		150,		-1,				1,			0,				1,			1,			1,			1,		 '' from dummy
			   union select N'Series',			N'Series',				'int',			100,		-1,				0,			0,				1,			1,			1,			1,		 '' from dummy
			   union select N'BplID',			N'Branch',				'int',			150,		-1,				1,			0,				1,			1,			1,			1,		 '' from dummy
			   --union select N'UomEntry',		N'Uom entry',			'string',		150,		-1,				1,			1,				0,			1,			1,			1,		 '' from dummy
			   union select N'unitMsr',		N'UoM Name',			'string',		100,		-1,				1,			0,				1,			1,			1,			1,		 '' from dummy
			   union select N'UseBaseUn',		N'Inventory Uom',		'string',		100,		-1,				1,			0,				1,			1,			1,			1,		 '' from dummy
			   union select N'NumPerMsr',		N'Item Per Unit',		'unit',			150,		-1,				1,			1,				0,			1,			1,			1,		 '' from dummy
			   union select N'UomCode',		N'UoM Code',			'string',		150,		-1,				1,			0,				1,			1,			1,			1,		 '' from dummy
			   union select N'UgpEntry',		N'UoM Group',			'int',			150,		-1,				1,			0,				1,			1,			1,			1,		 '' from dummy
			   union select N'VatGroup',		N'Tax Code',			'string',		150,		-1,				1,			0,				1,			1,			1,			1,		 '' from dummy
			   union select N'Project',		N'Project',				'string',		150,		-1,				1,			0,				1,			1,			1,			1,		 '' from dummy
			   union select N'U_OriginType',	N'Origin Type',			'string',		150,		-1,				1,			1,				0,			1,			1,			1,		 '' from dummy
			   union select N'U_OriginCode',	N'Origin Code',			'int',			150,		-1,				1,			1,				0,			1,			1,			1,		 '' from dummy
			   union select N'U_OriginLine',	N'Origin Line',			'int',			100,		-1,				1,			1,				0,			1,			1,			1,		 '' from dummy;
				      		  				
SELECT * FROM #tblStructure;	
							 
END;			  
   
   
   




