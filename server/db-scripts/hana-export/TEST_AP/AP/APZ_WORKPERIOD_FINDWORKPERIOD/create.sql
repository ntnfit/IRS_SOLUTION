
Create PROCEDURE APZ_WorkPeriod_FindWorkPeriod(in status char(1) default '', in code nvarchar(20) default '', in name nvarchar(20) default '')		
AS
BEGIN
-- Khai báo bảng dữ liệu combobox (tùy chọn)
		DECLARE tblValidValue TABLE
		(
			"ColumnName" NVARCHAR(50) NOT NULL
			,"Value" NVARCHAR(254) NOT NULL
			,"Display" NVARCHAR(254) NOT NULL
		);

		-- Khai báo bảng Structure
		create local temporary table #tblStructure
        (
             "ColumnName" NVARCHAR(50) NOT NULL
            ,"Caption" NVARCHAR(100) NULL
            ,"DataType" VARCHAR(20) NULL DEFAULT 'nvarchar' --int decimal money datetime date/ numeric alpha Memo(general) Numeric Date float(unit)  ValidValue(combobox)
			,"SubType" VARCHAR(20) NULL			--none phone percentage address image measurement price quantity rate sum time
            ,"Width" INT NULL DEFAULT 75
            ,"GroupIndex" INT NULL DEFAULT -1	-- dùng cho grid  default -1
            ,"Visible" smallint NULL DEFAULT 1
            ,"FormatType" NVARCHAR(20) NULL   -- numeric, datetime/ 
            ,"DisplayFormat" NVARCHAR(20) NULL  -- '{0:n2}'
            ,"ReadOnly" smallint NULL DEFAULT 0
            ,"AllowEdit" smallint NULL DEFAULT 1
            ,"AllowMove" smallint NULL DEFAULT 1
            ,"AllowSize" smallint NULL DEFAULT 1
            ,"AllowSort" smallint NULL DEFAULT 1
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
                select N'PeriodCode',			N'Code',				'nvarchar(20)',		175,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from DUMMY
                	UNION ALL
				SELECT N'PeriodName',			N'Name',				'nvarchar(20)',		175,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from DUMMY
                	UNION ALL
				SELECT 'SubPeriod',			N'SubPeriod',				'varchar(1)',		75,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from DUMMY
                	UNION ALL
				SELECT 'PeriodStatus',			N'PeriodStatus',				'varchar(1)',		75,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from DUMMY
                	UNION ALL
				SELECT 'Branch',			N'Branch',				'nvarchar(50)',		75,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from DUMMY
                	UNION ALL
				SELECT 'BranchCode',			N'BranchCode',				'int',		75,	-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from DUMMY
                	UNION ALL
				SELECT N'FromDate',			N'FromDate',				'datetime',		75,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from DUMMY
                	UNION ALL
				SELECT N'ToDate',			N'ToDate',				'datetime',		75,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from DUMMY;
					  

		SELECT * FROM #tblStructure;
		select a."PeriodCode",a."PeriodName",a."SubPeriod",a."PeriodStatus",a."FromDate",a."ToDate",a."Branch" as "BranchCode",b."Name" as "Branch" from APZ_OHWP a join OUBR b on a."Branch" = b."Code" where
									(ifnull(:status, '') = '' or "PeriodStatus" = :status) and
									 (ifnull(:code, '') = '' or "PeriodCode"  like  :code || '%') and
									 (ifnull(:name, '') = '' or "PeriodName"  like  :name || '%');
									
								
END



