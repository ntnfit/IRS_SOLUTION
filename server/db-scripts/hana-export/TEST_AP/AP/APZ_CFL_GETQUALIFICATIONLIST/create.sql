
CREATE PROCEDURE APZ_Cfl_GetQualificationList
as
BEGIN
	CREATE LOCAL TEMPORARY TABLE #tblStructure
	(
	     "ColumnName" NVARCHAR(50) NOT NULL
	    ,"Caption" NVARCHAR(100) NULL
	    ,"DataType" VARCHAR(20) NULL DEFAULT 'nvarchar' --int decimal money datetime date/ numeric alpha Memo(general) Numeric Date float(unit)  ValidValue(combobox)
		,"SubType" VARCHAR(20) NULL			--none phone percentage address image measurement price quantity rate sum time
	    ,"Width" INT NULL DEFAULT 75
	    ,"GroupIndex" INT NULL DEFAULT -1	-- d�ng cho grid  default -1
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
		--,Parent BIT NULL	-- d�ng cho treelist
		--,[Key] BIT NULL		-- d�ng cho treelist (identity)
	);
	INSERT INTO #tblStructure ("ColumnName", "Caption", "DataType", "Width", "GroupIndex", "Visible", "FormatType", "DisplayFormat", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "SummaryType", "GroupSummaryType")
		select N'QuaCode',		N'Qualification Code',	'int',		150,	-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL FROM DUMMY
			UNION ALL
		SELECT N'Name',			N'Name',				'nvarchar',		150,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL FROM DUMMY
			UNION ALL
		SELECT N'QuaGroup',		N'Qualification Group',	'int',		150,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL FROM DUMMY
			UNION ALL
		SELECT N'Description',	N'Description',			'nvarchar',		150,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL FROM DUMMY;
			
	SELECT * FROM #tblStructure;
		
	SELECT "QuaCode", a."Name", b."Name" AS "QuaGroup", "Description" FROM APZ_OHQU a LEFT OUTER JOIN APZ_OHQG b ON b."Id" = a."QuaGroup";

END



