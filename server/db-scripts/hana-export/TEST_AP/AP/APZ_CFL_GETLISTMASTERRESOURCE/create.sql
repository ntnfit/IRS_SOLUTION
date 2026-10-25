
create procedure Apz_Cfl_GetListMasterResource(in ResType char(1) default '')
as
begin
	create local temporary table #tblStructure
            (
             "ColumnName" NVARCHAR(50) NOT NULL
            ,"Caption" NVARCHAR(100) NULL
            ,"DataType" VARCHAR(20) NULL DEFAULT 'nvarchar'  --int smallint decimal money float datetime date numeric
            ,"Width" INT NULL DEFAULT 75
            ,"GroupIndex" INT NULL DEFAULT -1	-- dùng cho grid  default -1
            ,"Visible" tinyint NULL DEFAULT 1
            ,"FormatType" NVARCHAR(20) NULL   -- numeric, datetime
            ,"DisplayFormat" NVARCHAR(20) NULL  -- '{0:n2}'
            ,"ReadOnly" tinyint NULL DEFAULT 0
            ,"AllowEdit" tinyint NULL DEFAULT 1
            ,"AllowMove" tinyint NULL DEFAULT 1
            ,"AllowSize" tinyint NULL DEFAULT 1
            ,"AllowSort" tinyint NULL DEFAULT 1
            ,"Fixed" VARCHAR(1) NULL	-- L:Left / R:Right
			-- Summary Item
            ,"SummaryType" VARCHAR(10) NULL	-- sum,average...
			-- Group Summary
            ,"GroupSummaryType" VARCHAR(10) NULL	-- sum,average...
            );
            INSERT INTO #tblStructure 
						("ColumnName",			"Caption",				"DataType",		"Width",	"GroupIndex", "Visible",	"FormatType", "DisplayFormat", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "SummaryType", "GroupSummaryType")
			select N'ResCode',		N'ResCode',			'nvarchar',			75,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from dummy
			union select N'ResName',		N'Resource Name',	'nvarchar',			150,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from dummy
			union select N'ResType',		N'ResType',			'nvarchar',			75,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL from dummy
			union select N'ResTypeName',	N'Resource Type',	'nvarchar',			75,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL from dummy;
					
						
	select * from #tblStructure;
	
	select ifnull("ResCode",'') "ResCode"
			,ifnull("ResName",'') "ResName"
			,ifnull("ResType",'') "ResType"
			,case ifnull("ResType",'')
				when 'M' then 'Machine'
				when 'L' then 'Labor'
				when 'O' then 'Other'
				else 'Other'
			end "ResTypeName"
	from orsc
	where ("ResType" = :ResType and "ResType" <> '') or (COALESCE(TRIM(:ResType),'') = '');
end;




