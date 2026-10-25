
CREATE PROCEDURE APZ_MenuStructure_GetListMenu(in lngCode nvarchar(50) default '')
AS
BEGIN
	--Create temp table Structure
	declare tblStructure table
	(
		 "ColumnName" nvarchar(50) not null,
		 "Caption" nvarchar(100) null,
		 "DataType" varchar(20) null, --int decimal money datetime date/ numeric alpha Memo(general) Numeric Date float(unit)  ValidValue(combobox)
		 "SubType" varchar(20) null, --none phone percentage address image measurement price quantity rate sum time
		 "Width" int null, --use for grid default -1
		 "GroupIndex" int null,
         "Visible" tinyint null,
         "FormatType" nvarchar(20) null,   --numeric, datetime/	
         "DisplayFormat" nvarchar(20) NULL, -- '{0:n2}'
         "ReadOnly" tinyint null,
         "AllowEdit" tinyint null,
         "AllowMove" tinyint null,
         "AllowSize" tinyint null,
         "AllowSort" tinyint null,
         "Fixed" varchar(1) null,	-- L:Left / R:Right
         "SummaryType" varchar(10) null,	-- sum,average...
		 "GroupSummaryType" varchar(10) null, -- sum,average...
		 "Parent" tinyint null,	--use for Treelist
	 	 "Key" tinyint null --use for Treelist(identity)
	);
	
	declare tbl table
	(
		"Id" int,
		"FatherId" int,
		"Code" nvarchar(100),
		"Name" nvarchar(254),
		"LanguageKey" nvarchar(100),
		"MenuType" char(1)
	);
	
	insert into :tblStructure("ColumnName", "Caption", "DataType", "Width", "GroupIndex", "Visible", "FormatType", "DisplayFormat", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "SummaryType", "GroupSummaryType", "Parent", "Key")
	select  N'Id',			N'ID',			'nvarchar',		75,			-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 '',	NULL,		 NULL,	NULL,		 1 from DUMMY
	union all
	select  N'FatherId',	N'FatherId',	'nvarchar',		100,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 '',	NULL,		 NULL,	1,			NULL from DUMMY
	union all
	select  N'Code',		N'Code',		'nvarchar',		100,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 '',	NULL,		 NULL,	NULL,		 NULL from DUMMY
	union all
	select  N'Name',		N'Name',		'nvarchar',		100,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 '',	NULL,		 NULL,	NULL,		 NULL from DUMMY
	union all
	select  N'LanguageKey',N'LanguageKey',	'nvarchar',		100,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 '',	NULL,		 NULL,	NULL,		 NULL from DUMMY
	union all
	select  N'MenuType',	N'MenuType',	'nvarchar',		100,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 '',	NULL,		 NULL,	NULL,		 NULL from DUMMY;

	select * from :tblStructure;
	
	insert into :tbl select * from APZ_OMNI;
	
	if ifnull(:lngCode,'')<>'' then
		update a set "Name"=b."ColumnName"
		from :tbl a join apz_formsetting b
		on b."FormId"='oMain' and b."ItemId"='oMain' and b."ColumnId"=a."LanguageKey" and b."UserSign"='-' and a."MenuType" in ('F','N','L') and b."LngCode"=:lngCode;
	end if;
	
	select * from :tbl order by "Id", "FatherId";
	
END;

