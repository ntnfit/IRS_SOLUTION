

CREATE PROCEDURE APZ_CFL_GetMarketingCampaign(campaignId int default 0)
AS
BEGIN
	create local temporary table #tblStructure
	(
		"ColumnName" NVARCHAR(50) NOT NULL,
       	"Caption" NVARCHAR(100) NULL,
        "DataType" VARCHAR(20) NULL DEFAULT 'nvarchar', --int decimal money datetime date/ numeric alpha Memo(general) Numeric Date float(unit)  ValidValue(combobox)
		"SubType" VARCHAR(20) NULL,			--None Phone Percentage Address Image Measurement Price Quantity Rate Sum Time
        "Width" INT NULL DEFAULT 75,
        "GroupIndex" INT NULL DEFAULT -1,	-- dùng cho grid  default -1
        "Visible" tinyint NULL DEFAULT 1,
        "FormatType" NVARCHAR(20) NULL,   -- numeric, datetime/ 
        "DisplayFormat" NVARCHAR(20) NULL,  -- '{0:n2}'
        "ReadOnly" tinyint NULL DEFAULT 0,
        "AllowEdit" tinyint NULL DEFAULT 1,
        "AllowMove" tinyint NULL DEFAULT 1,
        "AllowSize" tinyint NULL DEFAULT 1,
        "AllowSort" tinyint NULL DEFAULT 1,
        "Fixed" VARCHAR(1) NULL,	-- L:Left / R:Right
		-- Summary Item
        "SummaryType" VARCHAR(10) NULL,	-- sum,average...
		-- Group Summary
        "GroupSummaryType" VARCHAR(10) NULL,	-- sum,average...
		"Parent" tinyint NULL,	-- dùng cho treelist
		"Key" tinyint NULL		-- dùng cho treelist (identity)
	);
	
	 INSERT INTO #tblStructure("ColumnName", "Caption", "DataType",	"Width",	"GroupIndex", "Visible",	"FormatType", "DisplayFormat", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "SummaryType", "GroupSummaryType","Parent", "Key")
     select 		  N'DocEntry',				N'Id',				'int',			75,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL,0,1 from DUMMY
	 union all select N'CampaignNo',			N'Code',			'nvarchar',		75,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL,0,0 from DUMMY
	 union all select N'CampaignName',			N'Name',			'nvarchar',		75,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL,0,0 from DUMMY
	 union all select N'Status',				N'Status',			'char',			75,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL,0,0 from DUMMY
	 union all select N'StartDate',				N'Start Date',		'datetime',		75,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL,0,0 from DUMMY
	 union all select N'Stage',					N'Stage',			'char',			75,		-1,			0,			null,		null,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL,0,0 from DUMMY;
	 
	 select * from #tblStructure;
	 select * from APZ_OSMC  WHERE (:campaignId=0 OR "DocEntry" <> :campaignId) and "Status" = 'O';
END;



