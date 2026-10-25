
CREATE procedure APZ_CFL_GetListSchedule
(
	IN code int default 0,
	MRPCode int default 0,
	status char(1) default '',
	mrpType nvarchar(10) default '',
	scheduleType nvarchar(10) default '',
	isGenCapacity char(1) default '',
	fDate date default '',
	tDate date default '',
	fixDate date default ''
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
				      select N'PSCode',			N'Code',				'int',			75,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from dummy
				union select N'PSName',			N'PS Name',				'nvarchar',		150,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from dummy
				union select N'ScheName',		N'Schedule Type',		'nvarchar',		150,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from dummy
				union select N'MrpCode',		N'Mrp cocde',			'int',			75,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from dummy
				union select N'MrpName',		N'Mrp Name',			'nvarchar',		150,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from dummy
				union select N'MrpTypeName',	N'Mrp Type',			'nvarchar',		150,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from dummy;
				
	SELECT * FROM #tblStructure;

	create local temporary table #tblScheduleType
	(
		"ScheType" nvarchar(1),
		"ScheName" nvarchar(20)
	);

	INSERT INTO #tblScheduleType
		  select 'O','Production' from dummy
	union select 'P','Purchase' from dummy
	union select 'T', 'Transfer' from dummy;
	
	select a.*,b."MrpCode", b."MrpName", b."BeginDate", b."FCstDate", b."FixDate", b."Type" "MrpType", c."ScheName", d."TypeName" "MrpTypeName" from apz_OPRS a inner join APZ_OMRP b on a."MrpId" = b."MrpCode"
																						  INNER JOIN #tblScheduleType c ON c."ScheType" = a."Type"
																						  INNER JOIN (Select "TypeCode", "TypeName" from APZ_OTCF where "ObjectCode" = '700003' and "Locked" = 'N') d
																						  ON d."TypeCode" = b."Type"
								 where (ifnull(:code,0) = 0 or a."PSCode" = :code) and
								 (ifnull(:MRPCode,0) = 0 or a."MrpId" = :MRPCode) and
								 (ifnull(:status,'') = '' or a."Status" = :status) and
								 (ifnull(:fDate,'') = '' or b."BeginDate" = :fDate) and
								 (ifnull(:tDate,'') = '' or b."FCstDate" = :tDate) and
								 (ifnull(:fixDate,'') = '' or b."FixDate" = :fixDate) and
								 (ifnull(:mrpType,'') = '' or exists(select data from split(:mrpType,';') where data = b."Type")) AND 
								 (ifnull(:scheduleType,'') = '' or exists(select data from split(:scheduleType,';') where data = a."Type"))
								 and (ifnull(:isGenCapacity, '') = '' or :isGenCapacity = 'N' and not exists(select 1 from APZ_CGRC where "ScheCode" = a."PSCode") or :isGenCapacity = 'Y' and exists(select 1 from APZ_CGRC where "ScheCode" = a."PSCode"))
								 ORDER BY "PSCode" DESC;

								 
								 
END;			  
   
   
   




