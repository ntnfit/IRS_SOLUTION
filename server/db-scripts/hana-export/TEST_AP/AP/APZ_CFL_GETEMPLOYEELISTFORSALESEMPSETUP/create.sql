
CREATE PROCEDURE APZ_Cfl_GetEmployeeListForSalesEmpSetup(in BPLId NVARCHAR(10) default '')
as
BEGIN
	CREATE LOCAL TEMPORARY TABLE #tblStructure
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
	
	CREATE LOCAL TEMPORARY TABLE #tbl as
	(
		select top 0 a.*, b."Name" as "BranchName", c."Name" as "Department", (select top 0 "LinkThumbnails" from APZ_D_UUPIM) as "LinkThumbnails"
		from OHEM a left join OUBR b on a."branch" = b."Code"
		left join OUDP c on a."dept" = c."Code"
	);
	
	INSERT INTO #tblStructure ("ColumnName", "Caption", "DataType", "Width", "GroupIndex", "Visible", "FormatType", "DisplayFormat", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "SummaryType", "GroupSummaryType")
		select N'firstName',		N'First Name',			'nvarchar',		150,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 '',	NULL,		 NULL FROM DUMMY
			UNION ALL
		select N'lastName',		N'Last Name',			'nvarchar',		150,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 '',	NULL,		 NULL FROM DUMMY
			UNION ALL
		Select N'empID',			N'Employee No.',		'int',			150,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 '',	NULL,		 NULL FROM DUMMY
			UNION ALL
		select N'ExtEmpNo',		N'Ext. Employee No',	'nvarchar',		75,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 '',	NULL,		 NULL FROM DUMMY
			UNION ALL
		select N'BranchName',	N'Branch',				'nvarchar',		150,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 '',	NULL,		 NULL FROM DUMMY
			UNION ALL
		select N'Department',	N'Department',			'nvarchar',		150,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 '',	NULL,		 NULL FROM DUMMY;
						
	SELECT *  FROM #tblStructure;
	
	INSERT INTO	#tbl
	SELECT a.*, b."Name" AS "BranchName", c."Name" AS "Department",
	(select FIRST_VALUE("LinkThumbnails" order by "TimestampCreated" desc) as "LinkThumbnails"
	from APZ_D_UUPIM f where f."ObjectType" = '1470000171' and a."userId" = f."UserSign") as "LinkThumbnails"
	FROM OHEM a LEFT OUTER JOIN OUBR b ON a."branch" = b."Code" LEFT OUTER JOIN OUDP c ON a."dept" = c."Code"
	WHERE a."Active"='Y' AND (:BPLId = '' OR (:BPLId <>'' AND EXISTS (SELECT 1 FROM HEM10 x WHERE x."empID" = a."empID" AND x."BPLId" IN (SELECT Data FROM Split(:BPLId,','))))) ORDER BY a."lastName";
	
	update #tbl set "middleName" = '' where ifnull("middleName", '') = '';
	select * from #tbl ORDER BY "lastName";
			
END

