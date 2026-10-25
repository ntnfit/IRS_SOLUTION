
CREATE PROCEDURE  Apz_Cfl_EmployleeRelatives(
	val NVARCHAR(5000) DEFAULT '' 
,   userName NVARCHAR(20) DEFAULT ''
,   branch NVARCHAR(20) DEFAULT '')
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
                SELECT	N'relaID',				N'relaID',				'int',			75,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from Dummy
					   union select N'firstName',			N'First Name',			'nvarchar',		200,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from Dummy
					   union select N'middleName',			N'Middle Name',			'nvarchar',		75,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL from Dummy
					   union select N'lastName',			N'Last Name',			'nvarchar',		75,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL from Dummy;
		SELECT * FROM #tblStructure;
		if (:val = '')
		then
			select "relaID","firstName","middleName","lastName" from apz_oher;
		else
			select "relaID","firstName","middleName","lastName" from apz_oher where Cast("relaID" as nvarchar) in (select Data from split(:val,',') EXCEPT select top 1 Data from split(:val,',')) and "empID" in (select top 1 Data from split(:val,','));
			--dbo.split(@value,',') will return the empID first for 'select where' command, the next values in dbo.split(@value,',') are relaID values which are used to display in CFL
		end if;
		drop table #tblStructure;
	end




