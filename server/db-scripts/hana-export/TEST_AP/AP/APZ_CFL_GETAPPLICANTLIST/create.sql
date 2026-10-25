
CREATE PROCEDURE APZ_Cfl_GetApplicantList()
AS 
BEGIN
create local temporary  table #tblGender (
    "Code" int,
    "Name" varchar(255)
);
INSERT INTO #tblGender  VALUES	(1,'Male');
 INSERT INTO #tblGender  VALUES	(2,'Female');
create local temporary table #tblStructure
    (
     "ColumnName" NVARCHAR(50) NOT NULL
    ,"Caption" NVARCHAR(100) NULL
    ,"DataType" VARCHAR(20) NULL DEFAULT 'nvarchar' --int decimal money datetime date/ numeric alpha Memo(general) Numeric Date float(unit)  ValidValue(combobox)
	,"SubType" VARCHAR(20) NULL			--none phone percentage address image measurement price quantity rate sum time
    ,"Width" INT NULL DEFAULT 75
    ,"GroupIndex" INT NULL DEFAULT -1	-- dùng cho grid  default -1
    ,"Visible" tinyint NULL DEFAULT 1
    ,"FormatType" NVARCHAR(20) NULL   -- numeric, datetime/ 
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
	--,Parent BIT NULL	-- dùng cho treelist
	--,[Key] BIT NULL		-- dùng cho treelist (identity)
    );
    
    INSERT INTO #tblStructure
				("ColumnName",		"Caption",				"DataType",		"Width",	"GroupIndex", "Visible",	"FormatType", "DisplayFormat", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "SummaryType", "GroupSummaryType")
        SELECT	 'ApplicantNo',		'Applicant No',			'nvarchar',			150,		-1,				1,			NULL,			NULL,			1,		 0,			1,			1,			1,		 'L',		NULL,		 NULL 		from dummy
	union select 'ApplicantName',	'Applicant Name',		'nvarchar',			150,		-1,				1,			NULL,			NULL,			1,		 0,			1,			1,			1,		 'L',		NULL,		 NULL 		from dummy
	union select 'DoB',				'DoB',					'timestamp',		150,		-1,				1,			NULL,			NULL,			1,		 0,			1,			1,			1,		 'L',		NULL,		 NULL 		from dummy
	union select 'GenderName',		'Gender',				'nvarchar',			150,		-1,				1,			NULL,			NULL,			1,		 0,			1,			1,			1,		 'L',		NULL,		 NULL 		from dummy
	union select 'Gender',			'Gender Code',			'int',				150,		-1,				1,			NULL,			NULL,			1,		 0,			1,			1,			1,		 'L',		NULL,		 NULL 		from dummy
	union select 'ApplicantID',		'ApplicantID',			'int',				150,		-1,				1,			NULL,			NULL,			1,		 0,			1,			1,			1,		 'L',		NULL,		 NULL 		from dummy
	union select 'Qualifications',	'Qualifications',		'nvarchar',			150,		-1,				1,			NULL,			NULL,			1,		 0,			1,			1,			1,		 'L',		NULL,		 NULL 		from dummy
	union select 'Address',			'Address',				'nvarchar',			150,		-1,				1,			NULL,			NULL,			1,		 0,			1,			1,			1,		 'L',		NULL,		 NULL 		from dummy
	union select 'Phone',			'Phone',				'nvarchar',			150,		-1,				1,			NULL,			NULL,			1,		 0,			1,			1,			1,		 'L',		NULL,		 NULL 		from dummy
	union select 'Email',			'Email',				'nvarchar',			150,		-1,				1,			NULL,			NULL,			1,		 0,			1,			1,			1,		 'L',		NULL,		 NULL 		from dummy;
		
SELECT *FROM #tblStructure;
SELECT a.*,b."Name" as "GenderName",b."Code" as "Gender" 
FROM APZ_OHAP a left join #tblGender b on a."Gender"=b."Code" where a."ApplicantNo" not in(SELECT DISTINCT "ApplicantNo" from APZ_HJV6 where "Status"='H');
 end;




