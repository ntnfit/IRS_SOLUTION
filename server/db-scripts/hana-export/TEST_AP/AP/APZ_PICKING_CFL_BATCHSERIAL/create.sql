
CREATE PROCEDURE apz_picking_cfl_batchserial(
	in ItemCode NVARCHAR(50),
	in WhsCode NVARCHAR(50),
	in LineNum INT,
	in DocEntry INT,
	in OrderNum INT default 0,
	in OrderLine INT default -1)
AS
BEGIN

-- Khai báo bảng Structure
		create local temporary table #tblStructure
            (
	             "ColumnName" NVARCHAR(50) NOT NULL
	            ,"Caption" NVARCHAR(100) NULL
	            ,"DataType" VARCHAR(20) NULL DEFAULT 'nvarchar' --int decimal money datetime date/ numeric alpha Memo(general) Numeric Date float(unit)  ValidValue(combobox)
				,"SubType" VARCHAR(20) NULL			--none phone percentage address image measurement price quantity rate sum time
	            ,"Width" INT NULL DEFAULT 75
	            ,"GroupIndex" INT NULL DEFAULT -1	-- dùng cho grid  default -1
	            ,"Visible" smallint NULL DEFAULT 1
	            ,"FormatType" NVARCHAR(20) NULL   -- numeric, datetime/ 
	            ,"DisplayFormat" NVARCHAR(20) NULL  -- '{0:n2}'
	            ,"ReadOnly" smallint NULL DEFAULT 0
	            ,"AllowEdit" smallint NULL DEFAULT 1
	            ,"AllowMove" smallint NULL DEFAULT 1
	            ,"AllowSize" smallint NULL DEFAULT 1
	            ,"AllowSort" smallint NULL DEFAULT 1
	            ,"Fixed" VARCHAR(1) NULL	-- L:Left / R:Right
				-- Summary Item
	            ,"SummaryType" VARCHAR(10) NULL	-- sum,average...
				-- Group Summary
	            ,"GroupSummaryType" VARCHAR(10) NULL	-- sum,average...
				--,Parent BIT NULL	-- dùng cho treelist
				--,[Key] BIT NULL		-- dùng cho treelist (identity)
            );
            INSERT INTO #tblStructure("ColumnName", "Caption", "DataType", "Width", "GroupIndex", "Visible", "FormatType", "DisplayFormat", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "SummaryType", "GroupSummaryType")
                select N'DistNumber',		N'Batch/Serial No.',			'nvarchar',		75,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from DUMMY
                	UNION ALL
				select N'Quantity',N'Quantity',			'nvarchar',		150,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from DUMMY
					UNION ALL
				select N'LotNumber',N'Lot No.',			'nvarchar',		150,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL FROM DUMMY
					UNION ALL
				select N'BinCode',N'Bin. Code',			'nvarchar',		150,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL FROM DUMMY
					UNION ALL
				select N'BinDescription',N'Bin. Description',			'nvarchar',		150,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from DUMMY;
			 
		SELECT *  FROM #tblStructure;
		create local temporary table #tempAuto ("DocEntry" int);
		call apz_fun_picking_AutoSelect(:ItemCode,:WhsCode,0,:LineNum,:DocEntry,:OrderNum,:OrderLine, '#tempAuto');
		select * from #tempAuto;
		drop table #tempAuto;
		
END



