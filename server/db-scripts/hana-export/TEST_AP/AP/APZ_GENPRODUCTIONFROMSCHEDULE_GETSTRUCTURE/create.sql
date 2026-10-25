
CREATE PROCEDURE APZ_GenProductionFromSchedule_GetStructure

AS
BEGIN
	create local temporary table #tblStructurePlan
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
   INSERT INTO #tblStructurePlan ("ColumnName", "Caption", "DataType", "Width", "GroupIndex", "Visible", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed")
	 			 SELECT N'ItemName',		N'Item Name',			'string',		150,		-1,				1,			1,				0,			1,			1,			1,		 'L' FROM DUMMY
		    UNION SELECT N'ItemCode',		N'Item Code',			'string',		100,		-1,				1,			1,				0,			1,			1,			1,		 'L' FROM DUMMY
		    UNION SELECT N'BomVersion',		N'Bom version',			'string',		100,		-1,				1,			1,				0,			1,			1,			1,		 '' FROM DUMMY
			UNION SELECT N'DueDate',		N'DueDate',				'datetime',		150,		-1,				1,			1,				0,			1,			1,			1,		 '' FROM DUMMY
			UNION SELECT N'Quantity',		N'Quantity',			'numeric',		100,		-1,				1,			0,				1,			1,			1,			1,		 '' FROM DUMMY
			UNION SELECT N'PlanQty',		N'Planning Quantity',	'numeric',		100,		-1,				1,			1,				0,			1,			1,			1,		 '' FROM DUMMY
			UNION SELECT N'U_Shift',		N'Shift',				'string',		150,		-1,				1,			1,				0,			1,			1,			1,		 '' FROM DUMMY
			UNION SELECT N'Whse',			N'Whs Code',			'string',		150,		-1,				1,			0,				1,			1,			1,			1,		 '' FROM DUMMY
			UNION SELECT N'PostDate',		N'Posting Date',		'datetime',		150,		-1,				1,			1,				0,			1,			1,			1,		 '' FROM DUMMY
			UNION SELECT N'Remark',			N'Remarks',				'string',		150,		-1,				1,			0,				1,			1,			1,			1,		 '' FROM DUMMY
			UNION SELECT N'CloseDate',		N'Close Date',			'datetime',		150,		-1,				0,			1,				0,			1,			1,			1,		 '' FROM DUMMY
			UNION SELECT N'U_OriginType',	N'Origin Type',			'string',		150,		-1,				1,			1,				0,			1,			1,			1,		 '' FROM DUMMY
			UNION SELECT N'U_OriginCode',	N'Origin Code',			'int',			150,		-1,				1,			1,				0,			1,			1,			1,		 '' FROM DUMMY
			UNION SELECT N'U_OriginLine',	N'Origin Line',			'int',			100,		-1,				1,			1,				0,			1,			1,			1,		 '' FROM DUMMY
			UNION SELECT N'Project',		N'Project',				'string',		150,		-1,				1,			0,				1,			1,			1,			1,		 '' FROM DUMMY
			UNION SELECT N'Series',			N'Series',				'int',			100,		-1,				1,			0,				1,			1,			1,			1,		 '' FROM DUMMY
			UNION SELECT N'BplID',			N'Branch',				'int',			150,		-1,				1,			0,				1,			1,			1,			1,		 '' FROM DUMMY;
			
			
	
	
	create local temporary table #tblStructureRelease
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
   INSERT INTO #tblStructureRelease ("ColumnName", "Caption", "DataType", "Width", "GroupIndex", "Visible", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed")
				  SELECT N'PONo',			N'PO No.',				'int',			100,		-1,				1,			1,				0,			1,			1,			1,		 'L' FROM DUMMY
			UNION SELECT N'PONum',			N'PO Num',				'int',			100,		-1,				1,			1,				0,			1,			1,			1,		 'L' FROM DUMMY
			UNION SELECT N'ItemCode',		N'Item Code',			'string',		100,		-1,				1,			1,				0,			1,			1,			1,		 'L' FROM DUMMY
			UNION SELECT N'ItemName',		N'Item Name',			'string',		150,		-1,				1,			1,				0,			1,			1,			1,		 'L' FROM DUMMY
			UNION SELECT N'DueDate',		N'DueDate',				'datetime',		150,		-1,				1,			1,				0,			1,			1,			1,		 '' FROM DUMMY
			UNION SELECT N'Quantity',		N'Quantity',			'numeric',		100,		-1,				1,			1,				0,			1,			1,			1,		 '' FROM DUMMY
			UNION SELECT N'ReportQty',		N'Report Qty',			'numeric',		100,		-1,				1,			1,				0,			1,			1,			1,		 '' FROM DUMMY
			UNION SELECT N'U_Shift',		N'Shift',				'string',		150,		-1,				1,			1,				0,			1,			1,			1,		 '' FROM DUMMY
			UNION SELECT N'Whse',			N'Whs Code',			'string',		150,		-1,				1,			1,				0,			1,			1,			1,		 '' FROM DUMMY
			UNION SELECT N'Remark',			N'Remarks',				'string',		150,		-1,				1,			0,				1,			1,			1,			1,		 '' FROM DUMMY
			UNION SELECT N'U_OriginType',	N'Origin Type',			'string',		150,		-1,				1,			1,				0,			1,			1,			1,		 '' FROM DUMMY
			UNION SELECT N'U_OriginCode',	N'Origin Code',			'int',			100,		-1,				1,			1,				0,			1,			1,			1,		 '' FROM DUMMY
			UNION SELECT N'U_OriginLine',	N'Origin Line',			'int',			100,		-1,				1,			1,				0,			1,			1,			1,		 '' FROM DUMMY
			UNION SELECT N'Project',		N'Project',				'string',		150,		-1,				1,			1,				0,			1,			1,			1,		 '' FROM DUMMY
			UNION SELECT N'Series',			N'Series',				'int',			100,		-1,				1,			1,				0,			1,			1,			1,		 '' FROM DUMMY
			UNION SELECT N'BplID',			N'Branch',				'int',			150,		-1,				1,			1,				0,			1,			1,			1,		 '' FROM DUMMY;
							  
	 		
	 create local temporary table #tblStructureReport
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
   INSERT INTO #tblStructureReport ("ColumnName", "Caption", "DataType", "Width", "GroupIndex", "Visible", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed")
	 			 SELECT N'PONo',			N'PO No.',				'int',			100,		-1,				1,			1,				0,			1,			1,			1,		 'L' FROM DUMMY
			UNION SELECT N'PONum',			N'PO Num',				'int',			100,		-1,				1,			1,				0,			1,			1,			1,		 'L' FROM DUMMY
			UNION SELECT N'ItemCode',		N'Item Code',			'string',		100,		-1,				1,			1,				0,			1,			1,			1,		 'L' FROM DUMMY
			UNION SELECT N'ItemName',		N'Item Name',			'string',		150,		-1,				1,			1,				0,			1,			1,			1,		 'L' FROM DUMMY
			UNION SELECT N'TranType',		N'Tran Type',			'string',		150,		-1,				1,			0,				1,			1,			1,			1,		 '' FROM DUMMY
			UNION SELECT N'DueDate',		N'DueDate',				'datetime',		150,		-1,				1,			1,				0,			1,			1,			1,		 '' FROM DUMMY
			UNION SELECT N'Quantity',		N'Quantity',			'numeric',		100,		-1,				1,			1,				0,			1,			1,			1,		 '' FROM DUMMY
			UNION SELECT N'ReportQty',		N'Report Qty',			'numeric',		100,		-1,				1,			0,				1,			1,			1,			1,		 '' FROM DUMMY
			UNION SELECT N'Price',			N'Price',				'numeric',		100,		-1,				1,			1,				0,			1,			1,			1,		 '' FROM DUMMY
			UNION SELECT N'U_Shift',		N'Shift',				'string',		150,		-1,				1,			1,				0,			1,			1,			1,		 '' FROM DUMMY
			UNION SELECT N'Whse',			N'Whs Code',			'string',		150,		-1,				1,			1,				0,			1,			1,			1,		 '' FROM DUMMY
			UNION SELECT N'Remark',			N'Remarks',				'string',		150,		-1,				1,			0,				1,			1,			1,			1,		 '' FROM DUMMY
			UNION SELECT N'U_OriginType',	N'Origin Type',			'string',		150,		-1,				1,			1,				0,			1,			1,			1,		 '' FROM DUMMY
			UNION SELECT N'U_OriginCode',	N'Origin Code',			'int',			100,		-1,				1,			1,				0,			1,			1,			1,		 '' FROM DUMMY
			UNION SELECT N'U_OriginLine',	N'Origin Line',			'int',			100,		-1,				1,			1,				0,			1,			1,			1,		 '' FROM DUMMY
			UNION SELECT N'Project',		N'Project',				'string',		150,		-1,				1,			1,				0,			1,			1,			1,		 '' FROM DUMMY
			UNION SELECT N'Series',			N'Series',				'int',			100,		-1,				1,			1,				0,			1,			1,			1,		 '' FROM DUMMY
			UNION SELECT N'BplID',			N'Branch',				'int',			150,		-1,				1,			1,				0,			1,			1,			1,		 '' FROM DUMMY;
			
			
			
	create local temporary table #tblStructureClosed
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
   INSERT INTO #tblStructureClosed ("ColumnName", "Caption", "DataType", "Width", "GroupIndex", "Visible", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed")
						     SELECT N'PONo',			N'PO No.',				'int',			100,		-1,				1,			1,				0,			1,			1,			1,		 'L' FROM DUMMY
					   UNION SELECT N'PONum',			N'PO Num',				'int',			100,		-1,				1,			1,				0,			1,			1,			1,		 'L' FROM DUMMY
					   UNION SELECT N'ItemCode',		N'Item Code',			'string',		100,		-1,				1,			1,				0,			1,			1,			1,		 'L' FROM DUMMY
					   UNION SELECT N'ItemName',		N'Item Name',			'string',		150,		-1,				1,			1,				0,			1,			1,			1,		 'L' FROM DUMMY
					   UNION SELECT N'DueDate',		N'DueDate',				'datetime',		150,		-1,				1,			1,				0,			1,			1,			1,		 '' FROM DUMMY
					   UNION SELECT N'Quantity',		N'Quantity',			'numeric',		100,		-1,				1,			1,				0,			1,			1,			1,		 '' FROM DUMMY
					   UNION SELECT N'ReportQty',		N'Report Qty',			'numeric',		100,		-1,				1,			1,				0,			1,			1,			1,		 '' FROM DUMMY
					   UNION SELECT N'U_Shift',		N'Shift',				'string',		150,		-1,				1,			1,				0,			1,			1,			1,		 '' FROM DUMMY
					   UNION SELECT N'Whse',			N'Whs Code',			'string',		150,		-1,				1,			1,				0,			1,			1,			1,		 '' FROM DUMMY
					   UNION SELECT N'Remark',			N'Remarks',				'string',		150,		-1,				1,			1,				0,			1,			1,			1,		 '' FROM DUMMY
					   UNION SELECT N'U_OriginType',	N'Origin Type',			'string',		150,		-1,				1,			1,				0,			1,			1,			1,		 '' FROM DUMMY
					   UNION SELECT N'U_OriginCode',	N'Origin Code',			'int',			100,		-1,				1,			1,				0,			1,			1,			1,		 '' FROM DUMMY
					   UNION SELECT N'U_OriginLine',	N'Origin Line',			'int',			100,		-1,				1,			1,				0,			1,			1,			1,		 '' FROM DUMMY
					   UNION SELECT N'Project',		N'Project',				'string',		150,		-1,				1,			1,				0,			1,			1,			1,		 '' FROM DUMMY;
					   
					   
					   
	create local temporary table #tblStructureCancel
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
   INSERT INTO #tblStructureCancel ("ColumnName", "Caption", "DataType", "Width", "GroupIndex", "Visible", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed")
							 SELECT N'PONo',			N'PO No.',				'int',			100,		-1,				1,			1,				0,			1,			1,			1,		 'L' FROM DUMMY
					   UNION SELECT N'PONum',			N'PO Num',				'int',			100,		-1,				1,			1,				0,			1,			1,			1,		 'L' FROM DUMMY
					   UNION SELECT N'ItemCode',		N'Item Code',			'string',		100,		-1,				1,			1,				0,			1,			1,			1,		 'L' FROM DUMMY
					   UNION SELECT N'ItemName',		N'Item Name',			'string',		150,		-1,				1,			1,				0,			1,			1,			1,		 'L' FROM DUMMY
					   UNION SELECT N'DueDate',		N'DueDate',				'datetime',		150,		-1,				1,			1,				0,			1,			1,			1,		 '' FROM DUMMY
					   UNION SELECT N'Quantity',		N'Quantity',			'numeric',		100,		-1,				1,			1,				0,			1,			1,			1,		 '' FROM DUMMY
					   UNION SELECT N'ReportQty',		N'Report Qty',			'numeric',		100,		-1,				1,			1,				0,			1,			1,			1,		 '' FROM DUMMY
					   UNION SELECT N'U_Shift',		N'Shift',				'string',		150,		-1,				1,			1,				0,			1,			1,			1,		 '' FROM DUMMY
					   UNION SELECT N'Whse',			N'Whs Code',			'string',		150,		-1,				1,			1,				0,			1,			1,			1,		 '' FROM DUMMY
					   UNION SELECT N'Remark',			N'Remarks',				'string',		150,		-1,				1,			1,				0,			1,			1,			1,		 '' FROM DUMMY
					   UNION SELECT N'U_OriginType',	N'Origin Type',			'string',		150,		-1,				1,			1,				0,			1,			1,			1,		 '' FROM DUMMY
					   UNION SELECT N'U_OriginCode',	N'Origin Code',			'int',			100,		-1,				1,			1,				0,			1,			1,			1,		 '' FROM DUMMY
					   UNION SELECT N'U_OriginLine',	N'Origin Line',			'int',			100,		-1,				1,			1,				0,			1,			1,			1,		 '' FROM DUMMY
					   UNION SELECT N'Project',		N'Project',				'string',		150,		-1,				1,			1,				0,			1,			1,			1,		 '' FROM DUMMY;
					   
					   
					   
	SELECT * FROM #tblStructurePlan;

	SELECT * FROM #tblStructureRelease;

	SELECT * FROM #tblStructureReport;

	SELECT * FROM #tblStructureClosed;

	SELECT * FROM #tblStructureCancel;
									 								
	 	       
END;





