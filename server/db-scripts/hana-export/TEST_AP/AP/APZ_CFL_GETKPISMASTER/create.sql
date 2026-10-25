

CREATE PROCEDURE APZ_CFL_GetKPIsMaster
AS
BEGIN
	
	create local temporary table #tblStructure
    (
		"ColumnName" NVARCHAR(50) NOT NULL
		,"Caption" NVARCHAR(100) NULL
		,"DataType" VARCHAR(20) NULL DEFAULT 'nvarchar'  --int smallint decimal money float datetime date numeric
		,"Width" INT NULL DEFAULT 75
		,"GroupIndex" INT NULL DEFAULT -1	-- dng cho grid  default -1
		,"Visible" tinyint NULL DEFAULT 1
		,"FormatType" NVARCHAR(20) NULL   -- numeric, datetime
		,"DisplayFormat" NVARCHAR(20) NULL  -- '{0:n2}'
		,"ReadOnly" tinyint NULL DEFAULT 0
		,"AllowEdit" tinyint NULL DEFAULT 1
		,"AllowMove" tinyint NULL DEFAULT 1
		,"AllowSize" tinyint NULL DEFAULT 1
		,"AllowSort" tinyint NULL DEFAULT 1
		,"Fixed" VARCHAR(1) NULL	-- L:Left / R:Right
		--Summary Item
		,"SummaryType" VARCHAR(10) NULL	-- sum,average...
		--Group Summary
		,"GroupSummaryType" VARCHAR(10) NULL	-- sum,average...
    );

	INSERT INTO #tblStructure
				("ColumnName",		"Caption",			"DataType",		"Width",	"GroupIndex", "Visible",	"FormatType", "DisplayFormat", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "SummaryType", "GroupSummaryType")
			Select N'Code',		N'Mã bộ KPI',			'varchar',		75,			-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,	  'L',		NULL,		 NULL FROM DUMMY
				UNION ALL
			Select N'Name',		N'Tên bộ KPI',			'nvarchar',		150,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,	  'L',		NULL,		 NULL FROM DUMMY
				UNION ALL
			Select N'Remarks',		N'Ghi chú',		'varchar',		75,			-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,	  'L',		NULL,		 NULL FROM DUMMY
				UNION ALL
			Select N'CreateDate',		N'Ngày tạo',			'nvarchar',		150,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,	  'L',		NULL,		 NULL FROM DUMMY;

	select * from #tblStructure;

	select * from APZ_D_KPIS;

END




