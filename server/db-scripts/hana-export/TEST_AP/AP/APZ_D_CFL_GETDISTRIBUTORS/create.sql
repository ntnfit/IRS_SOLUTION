

CREATE PROCEDURE APZ_D_CFL_GetDistributors
AS
BEGIN

	--Khai bao bang Structure
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
    
    insert into #tblStructure(	"ColumnName",		"Caption", 			"DataType", 	"Width",	"GroupIndex", "Visible",	"FormatType", "DisplayFormat", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "SummaryType", "GroupSummaryType")
    				select 	 	 'CardCode',		'Code',	 			'varchar',		  75,			-1,			1,			NULL,			NULL,				1,		 0,				1,			1,			1,		   	'L',		NULL,		 NULL from dummy
   					union all
    				select  	 'CardName',		'Name',				'nvarchar',	   	  150,			-1,			1,			NULL,			NULL,				1,		 0,				1,			1,			1,		 	'L',		NULL,		 NULL from dummy
    				union all
    				select  	 'CardType',		'Card Type',		'varchar',	   	  75,			-1,			1,			NULL,			NULL,				1,		 0,				1,			1,			1,		 	'L',		NULL,		 NULL from dummy
    				union all
    				select  	 'Address',			'Address',			'nvarchar',	   	  150,			-1,			1,			NULL,			NULL,				1,		 0,				1,			1,			1,		 	'L',		NULL,		 NULL from dummy;

	select * from #tblStructure;
	
	select "CardCode", "CardName", "CardType", "Address" from OCRD
	where "U_SubType" = 'D'; 
	
END




