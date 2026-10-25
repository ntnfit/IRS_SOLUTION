

CREATE PROCEDURE APZ_CFL_GetListAtp(in bplId int default -1, typeAtp char(1) default '', status nvarchar(10) default '', outputType char(1) default '')
AS
BEGIN

	--Declare structure table
	create local temporary table #tblStructure
    (
		"ColumnName" NVARCHAR(50) NOT NULL,
		"Caption" NVARCHAR(100) NULL,
		"DataType" VARCHAR(20) NULL DEFAULT 'nvarchar',  --int smallint decimal money float datetime date numeric
		"Width" INT NULL DEFAULT 75,
		"GroupIndex" INT NULL DEFAULT -1,	-- dùng cho grid  default -1
		"Visible" TINYINT NULL DEFAULT 1,
		"FormatType" NVARCHAR(20) NULL,   -- numeric, datetime
		"DisplayFormat" NVARCHAR(20) NULL,  -- '{0:n2}'
		"ReadOnly" TINYINT NULL DEFAULT 0,
		"AllowEdit" TINYINT NULL DEFAULT 1,
		"AllowMove" TINYINT NULL DEFAULT 1,
		"AllowSize" TINYINT NULL DEFAULT 1,
		"AllowSort" TINYINT NULL DEFAULT 1,
		"Fixed" VARCHAR(1) NULL,	-- L:Left / R:Right
		--Summary Item
		"SummaryType" VARCHAR(10) NULL,	-- sum,average...
		--Group Summary
		"GroupSummaryType" VARCHAR(10) NULL	-- sum,average...
    );

	--Insert column to tblStructure
	insert into #tblStructure("ColumnName",		"Caption", 		"DataType",   "Width",	"GroupIndex", "Visible","FormatType", "DisplayFormat", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "SummaryType", "GroupSummaryType")
	                  select 	N'Code',		N'Code',		 'int',		 	75,			-1,			1,			NULL,		NULL,				1,		 0,				1,			1,			1,		 	'L',		NULL,		 NULL from dummy
					  union all
					  select 	N'Name',		N'Name',		'nvarchar',	 	150,		-1,			1,			NULL,		NULL,				1,		 0,				1,			1,			1,		 	'L',		NULL,		 NULL from dummy
					  union all
					  select 	N'TypeName',	N'Atp Type',	'nvarchar',		150,		-1,			1,			NULL,		NULL,				1,		 0,				1,			1,			1,		 	NULL,		NULL,		 NULL from dummy
					  union all	
					  select 	N'FromDate',	N'From date',	'datetime',		150,		-1,			1,			NULL,		NULL,				1,		 0,				1,			1,			1,		 	NULL,		NULL,		 NULL from dummy
					  union all
					  select 	N'ToDate',		N'To date',		'datetime',		150,		-1,			1,			NULL,		NULL,				1,		 0,				1,			1,			1,		 	NULL,		NULL,		 NULL from dummy;
	
	--Select columns of tblStructure 				  
	select * from #tblStructure;					  

	--Get data
	select a.*, b."TypeName" 
	from APZ_OATP a 
	inner join (select "TypeCode", "TypeName" from APZ_OTCF where "ObjectCode" = '700013') b on a."Type" = b."TypeCode"
	where (:typeAtp = '' or "Type" = :typeAtp) and (:bplId = -1 or "BplId" = :bplId) and (:status = '' or a."Status" in (select data from Split(:status, ',')))
	and (:outputType = '' or (:outputType = 'S' and "Code" not in (
				select ifnull("U_OriginCode", -1) from PRQ1 where ifnull("U_OriginType", '') = '700013' group by "U_OriginCode" 
				union all
				select ifnull("U_OriginCode", -1) from POR1 where ifnull("U_OriginType", '') = '700013' group by "U_OriginCode" 
				union all
				select ifnull("U_OriginCode", -1) from DRF1 where ifnull("U_OriginType",'') = '700013' group by "U_OriginCode"))
	or (:outputType = 'G' and "Code" not in (select "AtpCode" from APZ_OSAT group by "AtpCode")))
	order by "Code" desc;

END




