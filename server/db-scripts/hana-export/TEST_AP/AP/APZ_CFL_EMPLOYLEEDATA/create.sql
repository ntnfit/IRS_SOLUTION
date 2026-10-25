
CREATE PROCEDURE Apz_Cfl_EmployleeData 
(
in	val NVARCHAR(5000) DEFAULT '' 
,in   userName NVARCHAR(20) DEFAULT ''
,in   branch NVARCHAR(20) DEFAULT ''
,in   empBranch int DEFAULT 0
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
                SELECT	N'empID',				'empID',				'int',			75,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from Dummy
				union select N'ExtEmpNo',			N'Ext. Employee No',			'nvarchar',		75,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from Dummy
				union select N'firstName',			N'First Name',			'nvarchar',		75,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from Dummy
				union select N'middleName',			N'Middle Name',			'nvarchar',		75,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL from Dummy
				union select N'lastName',			N'Last Name',			'nvarchar',		75,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL from Dummy;
		SELECT * FROM #tblStructure; 
		if ifnull(val,'') =''
		then 
			SELECT a.*, IFNULL(a."firstName", '') || ' ' || IFNULL(a."middleName", '') || ' ' || IFNULL(a."lastName", '') AS "fullName", b."Name" AS "BranchName"
						, c."Name" AS "Department", e."name" AS "PositionName", CASE WHEN a."manager" IS NOT NULL THEN IFNULL(d."firstName", '') || ', ' || IFNULL(d."lastName", '') ELSE '' END AS "ManagerName" 
				FROM ohem a LEFT OUTER JOIN oubr b ON a."branch" = b."Code" 
							LEFT OUTER JOIN oudp c ON a."dept" = c."Code" 
							LEFT OUTER JOIN OHEM d ON a."manager" = d."empID" 
							LEFT OUTER JOIN OHPS e ON a."position" = e."posID" 
				WHERE (IFNULL(:empBranch, 0) = 0 OR (:empBranch <> 0 AND a."branch" = :empBranch)) AND a."Active" = 'Y';
		elseif (:val ='technician')
		then 
			SELECT a.*, IFNULL(a."firstName", '') || ' ' || IFNULL(a."middleName", '') || ' ' || IFNULL(a."lastName", '') AS "fullName", b."Name" AS "BranchName", c."Name" AS "Department"
					, CASE WHEN a."manager" IS NOT NULL THEN IFNULL(d."firstName", '') || ', ' || IFNULL(d."lastName", '') ELSE '' END AS "ManagerName" 
				FROM ohem a INNER JOIN hem6 f ON a."empID" = f."empID" 
								INNER JOIN OHTY g ON f."roleID" = g."typeID" 
								LEFT OUTER JOIN oubr b ON a."branch" = b."Code" 
								LEFT OUTER JOIN oudp c ON a."dept" = c."Code" 
								LEFT OUTER JOIN OHEM d ON a."manager" = d."empID" 
								LEFT OUTER JOIN OHPS e ON a."position" = e."posID" 
					WHERE e."name" = 'Technician' AND (IFNULL(:empBranch, 0) = 0 OR (:empBranch <> 0 AND a."branch" = :empBranch)) AND a."Active" = 'Y';
		else
			SELECT a.*, IFNULL(a."firstName", '') || ' ' || IFNULL(a."middleName", '') || ' ' || IFNULL(a."lastName", '') AS "fullName", b."Name" AS "BranchName", c."Name" AS "Department"
					, CASE WHEN a."manager" IS NOT NULL THEN IFNULL(d."firstName", '') || ', ' || IFNULL(d."lastName", '') ELSE '' END AS "ManagerName" 
				FROM ohem a LEFT OUTER JOIN oubr b ON a."branch" = b."Code" 
							LEFT OUTER JOIN oudp c ON a."dept" = c."Code" 
							LEFT OUTER JOIN OHEM d ON a."manager" = d."empID" 
							LEFT OUTER JOIN OHPS e ON a."position" = e."posID"
					where cast(a."empID" as nvarchar) in (select data from split(:val,',')) and (ifnull(:empBranch,0) = 0 or (:empBranch <> 0 and a."branch" = :empBranch)) and a."Active" = 'Y';
			
		end if;
	end
			




