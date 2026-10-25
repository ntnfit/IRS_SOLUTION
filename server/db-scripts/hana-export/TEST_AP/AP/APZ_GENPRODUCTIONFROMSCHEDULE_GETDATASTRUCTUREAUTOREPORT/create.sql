
CREATE PROCEDURE APZ_GenProductionFromSchedule_GetDataStructureAutoreport
AS
BEGIN
		-- Khai báo bảng Structure
		create local temporary table #tblStructure
            (
	             "ColumnName" NVARCHAR(50) NOT NULL
	            ,"Caption" NVARCHAR(100) NULL
	            ,"DataType" VARCHAR(20) NULL DEFAULT 'string' --int decimal money datetime date/ numeric alpha Memo(general) Numeric Date float(unit)  ValidValue(combobox)
	            ,"Width" INT NULL DEFAULT 75
	            ,"GroupIndex" INT NULL DEFAULT -1	-- dùng cho grid  default -1
	            ,"Visible" smallint NULL DEFAULT 1
	            ,"ReadOnly" smallint NULL DEFAULT 0
	            ,"AllowEdit" smallint NULL DEFAULT 1
	            ,"AllowMove" smallint NULL DEFAULT 1
	            ,"AllowSize" smallint NULL DEFAULT 1
	            ,"AllowSort" smallint NULL DEFAULT 1
	            ,"Fixed" VARCHAR(1) NULL	-- L:Left / R:Right
            );
            INSERT INTO #tblStructure ("ColumnName", "Caption", "DataType", "Width", "GroupIndex", "Visible", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed")
                select N'ItemName',		N'Item Name',			'string',		150,		-1,				1,			1,				0,			1,			1,			1,		 'L' from DUMMY
                	UNION ALL
                select N'ItemCode',		N'Item Code',			'string',		100,		-1,				1,			1,				0,			1,			1,			1,		 'L' from DUMMY
                	UNION ALL
				select N'Order',			N'Order',				'int',			100,		-1,				1,			1,				0,			1,			1,			1,		 '' from DUMMY
					UNION ALL
				select N'TotalLine',		N'Line Total',			'numeric',		150,		-1,				1,			1,				0,			1,			1,			1,		 '' from DUMMY
					UNION ALL
				select N'PackQty',		N'PackQty',				'numeric',		100,		-1,				1,			1,				0,			1,			1,			1,		 '' from DUMMY
					UNION ALL
				select N'BarQty',			N'BarQty',				'numeric',		150,		-1,				1,			1,				0,			1,			1,			1,		 '' from DUMMY
					UNION ALL
				select N'CmpQty',			N'CmpQty',				'numeric',		150,		-1,				1,			1,				0,			1,			1,			1,		 '' from DUMMY
					UNION ALL
				select N'DueDate',		N'DueDate',				'datetime',		150,		-1,				1,			1,				0,			1,			1,			1,		 '' from DUMMY
					UNION ALL
				select N'WhsCode',		N'WhsCode',				'string',		150,		-1,				1,			1,				0,			1,			1,			1,		 '' from DUMMY
					UNION ALL
				select N'Quantity',		N'Quantity',			'numeric',		150,		-1,				1,			0,				1,			1,			1,			1,		 '' from DUMMY
					UNION ALL
				select N'DocEntry',		N'Production No.',		'int',			100,		-1,				1,			1,				0,			1,			1,			1,		 '' from DUMMY
					UNION ALL
				select N'Series',			N'Series',				'int',			150,		-1,				1,			1,				0,			1,			1,			1,		 '' from DUMMY
					UNION ALL
				select N'BPLId',			N'Branch',				'int',			100,		-1,				1,			1,				0,			1,			1,			1,		 '' from DUMMY
					UNION ALL
				select N'PONum',			N'Production Num',		'int',			150,		-1,				1,			1,				0,			1,			1,			1,		 '' from DUMMY
					UNION ALL
				select N'Price',			N'Price',				'numeric',		150,		-1,				1,			1,				0,			1,			1,			1,		 '' from DUMMY;
	
		Select * from #tblStructure;

END



