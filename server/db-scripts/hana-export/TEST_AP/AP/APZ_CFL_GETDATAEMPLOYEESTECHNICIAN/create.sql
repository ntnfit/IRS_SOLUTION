
CREATE PROCEDURE APZ_CFL_GetDataEmployeesTechnician
AS
BEGIN
		-- Khai báo bảng Structure
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
                	Select N'IndName',		N'Description',			'nvarchar',		150,	-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from DUMMY
					   union select N'lastName',		N'Last name',			'nvarchar',		150,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from DUMMY
					   union select N'firstName',		N'First name',			'nvarchar',		150,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from DUMMY
					   union select N'empID',			N'Employee No.',		'int',		150,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from DUMMY
					   union select N'middleName',		N'Middle Name',			'nvarchar',		150,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from DUMMY
					   union select N'userId',			N'User Id',			'nvarchar',		150,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from DUMMY
					   union select N'manager',		N'Manager',			'nvarchar',		150,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from DUMMY
					   union select N'UserSign2',		N'Updating User',			'nvarchar',		150,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from DUMMY
					   union select N'UserSign',		N'User Signature',			'nvarchar',		150,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from DUMMY;
					   

		SELECT * FROM #tblStructure;
		
		SELECT a."lastName" || ', ' || a."firstName" AS "Name", a."lastName", a."firstName", a."empID", a."middleName"
		, (SELECT user_code FROM OUSR WHERE USERID = a."userId") AS "userId"
		, (SELECT user_code FROM OUSR WHERE USERID = a."manager") AS "manager"
		, (SELECT user_code FROM OUSR WHERE USERID = a."UserSign2") AS "UserSign2"
		, (SELECT user_code FROM OUSR WHERE USERID = a."UserSign") AS "UserSign" 
		FROM OHEM a INNER JOIN HEM6 b ON a."empID" = b."empID" 
					INNER JOIN OHTY c ON b."roleID" = c."typeID" WHERE c."typeID" = -2;
END




