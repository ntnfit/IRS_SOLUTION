
CREATE PROCEDURE Apz_Cfl_EmployleeData2
(
in	val NCLOB 
,in   userName NVARCHAR(20) DEFAULT ''
,in   branch NVARCHAR(20) DEFAULT ''
,in   empBranch int DEFAULT 0
)
AS
    BEGIN
        -- Khai bao bang Structure
		declare tblStructure table
            (
             "ColumnName" NVARCHAR(50) NOT NULL
            ,"Caption" NVARCHAR(100) NULL
            ,"DataType" VARCHAR(20) NULL  --int smallint decimal money float datetime date numeric
            ,"Width" INT NULL
            ,"GroupIndex" INT NULL	-- dùng cho grid  default -1
            ,"Visible" tinyint NULL
            ,"FormatType" NVARCHAR(20) NULL   -- numeric, datetime
            ,"DisplayFormat" NVARCHAR(20) NULL  -- '{0:n2}'
            ,"ReadOnly" tinyint NULL
            ,"AllowEdit" tinyint NULL
            ,"AllowMove" tinyint NULL
            ,"AllowSize" tinyint NULL
            ,"AllowSort" tinyint NULL
            ,"Fixed" VARCHAR(1) NULL	-- L:Left / R:Right
			-- Summary Item
            ,"SummaryType" VARCHAR(10) NULL	-- sum,average...
			-- Group Summary
            ,"GroupSummaryType" VARCHAR(10) NULL	-- sum,average...
            );
            INSERT INTO :tblStructure 
						("ColumnName",			"Caption",				"DataType",		"Width",	"GroupIndex", "Visible",	"FormatType", "DisplayFormat", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "SummaryType", "GroupSummaryType")
                SELECT	N'empID',				'empID',				'int',			75,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from Dummy
				union select N'ExtEmpNo',			N'Ext. Employee No',			'nvarchar',		75,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from Dummy
				union select N'firstName',			N'First Name',			'nvarchar',		75,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from Dummy
				union select N'middleName',			N'Middle Name',			'nvarchar',		75,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL from Dummy
				union select N'lastName',			N'Last Name',			'nvarchar',		75,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL from Dummy
				union select N'BranchName',		N'Branch',			'nvarchar',		150,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from dummy
				union select N'Department',		N'Department',			'nvarchar',		150,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from dummy;
		SELECT * FROM :tblStructure; 
		if (ifnull(val,'') ='')
		then 
			select a.*,b."Name" as "BranchName",c."Name" as "Department" from ohem a left join oubr b on a."branch" = b."Code"
								left join oudp c on a."dept" = c."Code" where (IFNULL(:empBranch,0) = 0 or (:empBranch <> 0 and "branch" = :empBranch));
		else if (:val ='technician')
		then 
			select a.*,b."Name" as "BranchName",c."Name" as "Department" from ohem a join hem6 d on a."empID" = d."empID" join OHTY e on d."roleID" = e."typeID" 
				left join oubr b on a."branch" = b."Code"
				left join oudp c on a."dept" = c."Code"
					where e."name" = 'Technician'  and (ifnull(:empBranch,0) = 0 or (:empBranch <> 0 and "branch" = :empBranch));
		else
			select a.*,b."Name" as "BranchName",c."Name" as "Department" from ohem a 
					left join oubr b on a."branch" = b."Code"
					left join oudp c on a."dept" = c."Code" where Cast("empID" as nvarchar) in (select Data from SplitNclob(:val,',')) and (IFNULL(:empBranch,0) = 0 or (:empBranch <> 0 and "branch" = :empBranch));
			
			end if;
		end if;
	end

