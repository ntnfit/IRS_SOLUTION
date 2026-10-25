
CREATE procedure APZ_CFL_GetListMRP
(
	in code int default 0,
	status char(1) default '',
	forecast nvarchar(50) default '',
	type nvarchar(10) default '',
	demType char(1) default ''
)
as
BEGIN
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
                	   SELECT N'MrpCode',			N'Code',				'int',		75,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL FROM DUMMY
			     UNION SELECT N'MrpName',			N'Name',				'nvarchar',	150,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL FROM DUMMY
				 UNION SELECT N'TypeName',			N'Mrp Type',			'nvarchar',	150,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL FROM DUMMY
				 UNION SELECT N'FCCode',				N'Forecast Code',		'nvarchar',	150,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL FROM DUMMY;
                	  	
				
	
	SELECT * FROM #tblStructure;

	select a.*, b."TypeName" from APZ_OMRP a INNER JOIN (Select "TypeCode", "TypeName" from APZ_OTCF where "ObjectCode" = '700003' and "Locked" = 'N') b ON b."TypeCode" = a."Type"
																				where (IFNULL(:type, '') = '' or a."Type" in (SELECT Data FROM Split(:type, ','))) and
									(ifnull(:status, '') = '' or a."Status" = :status) and
									(ifnull(:forecast, '') = '' or a."FCCode" = :forecast) order by "MrpCode" DESC;
END




