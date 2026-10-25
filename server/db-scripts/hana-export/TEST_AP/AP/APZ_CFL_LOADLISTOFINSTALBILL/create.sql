
CREATE PROCEDURE Apz_Cfl_LoadListOfInstalBill(in customer NVARCHAR(100) default '', in itemCode NVARCHAR(100) default '', in userName NVARCHAR(20) default '', in branch NVARCHAR(20) default '')
AS
BEGIN
        -- Khai bao bang Structure
		create local temporary table #tblStructure
            (
	             "ColumnName" NVARCHAR(50) NOT NULL
	            ,"Caption" NVARCHAR(100) NULL
	            ,"DataType" VARCHAR(20) NULL DEFAULT 'nvarchar'  --int smallint decimal money float datetime date numeric
	            ,"Width" INT NULL DEFAULT 75
	            ,"GroupIndex" INT NULL DEFAULT -1	-- dùng cho grid  default -1
	            ,"Visible" smallint NULL DEFAULT 1
	            ,"FormatType" NVARCHAR(20) NULL   -- numeric, datetime
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
            );
        INSERT INTO #tblStructure ("ColumnName", "Caption", "DataType", "Width", "GroupIndex", "Visible", "FormatType", "DisplayFormat", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "SummaryType", "GroupSummaryType")
            select N'itemCode',			N'Item Code',			'nvarchar',		75,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from DUMMY
            	UNION ALL
			select N'itemName',			N'Item Descripton',		'nvarchar',		200,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from DUMMY
				union all
			select N'customer',			N'Item Code',			'nvarchar',		75,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from DUMMY
				UNION ALL
			select N'manufSN',			N'ManufSN',				'int',			75,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL from DUMMY
				UNION ALL
			select N'internalSN',			N'InternalSN',			'int',			75,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL from DUMMY
				UNION ALL
			select N'custmrName',			N'Customer Name',		'nvarchar',		75,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL FROM DUMMY
				UNION ALL
			select N'status',				N'Status',				'nvarchar',		75,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL from DUMMY
				UNION ALL
			select N'insID',				N'insID',				'int',			75,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL from DUMMY;				   
	
	SELECT * FROM #tblStructure;
	
	IF ifnull(:customer,'') != '' and ifnull(:itemCode,'')  != ''
	THEN
		SELECT "insID", "itemCode", "itemName", "manufSN", "internalSN", "custmrName", "customer", "status" FROM oins WHERE "customer" = :customer AND "itemCode" = :itemCode;

	ELSE
			IF ifnull(:itemCode,'')  != ''
			THEN
				SELECT "insID", "itemCode", "itemName", "manufSN", "internalSN", "custmrName", "customer", "status" FROM oins WHERE "itemCode" = :itemCode;

			ELSEIF ifnull(:customer,'') != ''
			THEN
					SELECT "insID", "itemCode", "itemName", "manufSN", "internalSN", "custmrName", "customer", "status" FROM oins WHERE "customer" = :customer;
				
			ELSE
					SELECT "insID", "itemCode", "itemName", "manufSN", "internalSN", "custmrName", "customer", "status" FROM oins;
			END IF;
	END IF;
end			



