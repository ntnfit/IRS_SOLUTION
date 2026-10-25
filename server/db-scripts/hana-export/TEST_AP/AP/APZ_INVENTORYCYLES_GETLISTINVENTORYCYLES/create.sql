
CREATE PROCEDURE APZ_InventoryCyles_GetListInventoryCyles 
(
in	name nvarchar(20) DEFAULT '' 
,in  frequency VarChar(1) DEFAULT ''
,in  subOption VarChar(1) DEFAULT ''
,in   interval int DEFAULT 0,
in nextDate timestamp default '',
in enType VarChar(1) default '',
in maxOccur int default 0,
in seEndDate timestamp default 0,
in dayInMonth int default 0,
in week int default 0,
in dayOfWeek int default 0,
in month int default 0
)
AS
    BEGIN
        -- Khai bao bang Structure
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
                SELECT	N'Name',				'empID',				'int',			75,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from Dummy
				union select N'Frequency',			N'Ext. Employee No',			'nvarchar',		75,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from Dummy
				;
		SELECT * FROM #tblStructure; 
	select * from OCYC a where (IFNULL(:name, '') = '' or "Name"  like  :name+'%') and
									(ifnull(:frequency, '') = '' or "Frequency" = :frequency) and
									(ifnull(:subOption, '') = '' or "SubOption" = :subOption) and
									(ifnull(:interval,0) = 0 or "Interval" = :interval) and
									(ifnull(:nextDate, '') = '' or "NextDate" = :nextDate) and
									(IFNULL(:enType, '') = '' or "EndType" =:enType) and
									(ifnull(:maxOccur,0) = 0 or "MaxOccur" = :maxOccur) and
									(ifnull(:seEndDate, '') = '' or "SeEndDat" = :seEndDate) and
									(ifnull(:dayInMonth, 0) = 0 or "DayInMonth" = :dayInMonth) and
									(ifnull(:week, 0) = 0 or "Week" =:week) and
									(ifnull(:dayOfWeek,0) = 0 or "DayOfWeek" =:dayOfWeek) and
									(ifnull(:month,0) = 0 or "Month" =:month);
			drop table #tblStructure;
	end
			




