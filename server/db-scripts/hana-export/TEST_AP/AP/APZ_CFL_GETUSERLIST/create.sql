
Create PROCEDURE APZ_Cfl_GetUserList
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
                SELECT	N'USERID',					N'ID',				'int',			50,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL FROM DUMMY
				--,(N'USER_CODE',				N'CODE',			'nvarchar',		100,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL)
				UNION SELECT N'U_NAME',				N'NAME',			'nvarchar',		430,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL FROM DUMMY;
				
				
		
SELECT *
FROM #tblStructure;
SELECT a.*, ifnull(b."firstName", '')  || ' ' || ifnull(b."middleName", '')  || ' ' || ifnull(b."lastName", '') as "EmployeeName",b."empID", c."Name" "DepartmentName"
				 from OUSR a left join OHEM b on a.USERID = b."userId" 
							left join OUDP c ON a."Department" = c."Code"
where "Locked" = 'N' and GROUPS != 99;

END




