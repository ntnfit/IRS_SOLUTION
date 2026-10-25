
CREATE PROCEDURE APZ_Cfl_ProjectReportCompletion(in projectId int)
AS 
BEGIN
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
        SELECT	 'DocEntry',		'#',					'int',			150,		-1,				1,			NULL,			NULL,			1,		 0,			1,			1,			1,		 'L',		NULL,		 NULL 		from dummy
	union select 'ProjectId',		'Project Id',			'int',			150,		-1,				1,			NULL,			NULL,			1,		 0,			1,			1,			1,		 'L',		NULL,		 NULL 		from dummy
	union select 'Code',			'Project Code',			'nvarchar',		150,		-1,				1,			NULL,			NULL,			1,		 0,			1,			1,			1,		 'L',		NULL,		 NULL 		from dummy
	union select 'PostingDate',		'Posting Date',			'timestamp',	150,		-1,				1,			NULL,			NULL,			1,		 0,			1,			1,			1,		 'L',		NULL,		 NULL 		from dummy
	union select 'Total',			'Total',				'Decimal',		150,		-1,				1,			NULL,			NULL,			1,		 0,			1,			1,			1,		 'L',		NULL,		 NULL 		from dummy
	union select 'Status',			'Status',				'nvarchar',		150,		-1,				1,			NULL,			NULL,			1,		 0,			1,			1,			1,		 'L',		NULL,		 NULL 		from dummy;
		
SELECT *
FROM #tblStructure;
	
if :projectId > 0
then
	select a.*,b."Code"  from APZ_ORTC a left join APZ_OPRG b on a."ProjectId"=b."AbsEntry"  Where a."ProjectId"=:projectId and a."Status"='O'
	order by "PostingDate" DESC;
 
 else
 select a.*,b."Code"  from APZ_ORTC a left join APZ_OPRG b on a."ProjectId"=b."AbsEntry"  Where a."Status"='O'
 order by "PostingDate" DESC;
 end if;
 end;




