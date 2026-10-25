

CREATE PROCEDURE APZ_CFL_ApprovalUsers(in departmntCode int)
AS
BEGIN

	--Khai bao bang Structure
	create local temporary table #tblStructure
    (
		"ColumnName" NVARCHAR(50) NOT NULL,
		"Caption" NVARCHAR(100) NULL,
		"DataType" VARCHAR(20) NULL DEFAULT 'nvarchar',  --int smallint decimal money float datetime date numeric
		"Width" INT NULL DEFAULT 75,
		"GroupIndex" INT NULL DEFAULT -1,	-- dùng cho grid  default -1
		"Visible" TINYINT NULL DEFAULT 1,
		"FormatType" NVARCHAR(20) NULL,   -- numeric, datetime
		"DisplayFormat" NVARCHAR(20) NULL,  -- '{0:n2}'
		"ReadOnly" TINYINT NULL DEFAULT 0,
		"AllowEdit" TINYINT NULL DEFAULT 1,
		"AllowMove" TINYINT NULL DEFAULT 1,
		"AllowSize" TINYINT NULL DEFAULT 1,
		"AllowSort" TINYINT NULL DEFAULT 1,
		"Fixed" VARCHAR(1) NULL,	-- L:Left / R:Right
		--Summary Item
		"SummaryType" VARCHAR(10) NULL,	-- sum,average...
		--Group Summary
		"GroupSummaryType" VARCHAR(10) NULL	-- sum,average...
    );
    
    insert into #tblStructure(	"ColumnName",		"Caption", 					"DataType", 		"Width",	"GroupIndex", "Visible",	"FormatType", "DisplayFormat", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "SummaryType", "GroupSummaryType")
    				select 	 	N'UserCode',		N'User Code',	  			'int',		   		75,				-1,			1,			NULL,			NULL,				1,		 0,				1,			1,			1,		   	'L',		NULL,		 NULL from dummy
   		 union all 	select  	N'UserId',			N'User ID',	  	  			'nvarchar',	   		150,			-1,			0,			NULL,			NULL,				1,		 0,				1,			1,			1,		 	'L',		NULL,		 NULL from dummy
    	 union all	select 		N'UserName',		N'User Name',	  			'nvarchar',	   		150,			-1,			1,			NULL,			NULL,				1,		 0,				1,			1,			1,		 	'L',		NULL,		 NULL from dummy
    	 union all	select 		N'DepartmntCode',	N'Department Code',			'nvarchar',	   		150,			-1,			0,			NULL,			NULL,				1,		 0,				1,			1,			1,		 	'L',		NULL,		 NULL from dummy
    	 union all	select 		N'Departmnt',		N'Department',				'nvarchar',	   	 	150,			-1,			1,			NULL,			NULL,				1,		 0,				1,			1,			1,		 	'L',		NULL,		 NULL from dummy
    	 union all	select 		N'DepartmntDesc',	N'Department Descriptions',	'nvarchar',	   		150,			-1,			1,			NULL,			NULL,				1,		 0,				1,			1,			1,		 	'L',		NULL,		 NULL from dummy
    	 union all 	select 		N'BranCode',		N'Branch Code',		  		'nvarchar',	    	150,			-1,			1,			NULL,			NULL,				1,		 0,				1,			1,			1,		 	'L',		NULL,		 NULL from dummy
    	 union all 	select 		N'Branch',			N'Branch',		  			'nvarchar',	    	150,			-1,			1,			NULL,			NULL,				1,		 0,				1,			1,			1,		 	'L',		NULL,		 NULL from dummy
		 union all 	select 		N'BranDesc',		N'Branch Descriptions',		'nvarchar',	    	150,			-1,			1,			NULL,			NULL,				1,		 0,				1,			1,			1,		 	'L',		NULL,		 NULL from dummy
		 union all  select		COLUMN_NAME,		COLUMN_NAME,				DATA_TYPE_NAME,		75,				-1,			1,			NULL,			NULL,				1,		 0,				1,			1,			1,		 	'L',		NULL,		 NULL from SYS.TABLE_COLUMNS where TABLE_NAME = 'OUSR' and COLUMN_NAME not in('USER_ID', 'USER_CODE', 'U_NAME', 'PASSWORD', 'PASSWORD1', 'PASSWORD2', 'PASSWORD3', 'PASSWORD4')
		 union all  select		COLUMN_NAME,		COLUMN_NAME,				DATA_TYPE_NAME,		75,				-1,			1,			NULL,			NULL,				1,		 0,				1,			1,			1,		 	'L',		NULL,		 NULL from SYS.TABLE_COLUMNS where TABLE_NAME = 'OUDP' and COLUMN_NAME not in('Code', 'Name', 'Remarks')
		 union all  select		COLUMN_NAME,		COLUMN_NAME,				DATA_TYPE_NAME,		75,				-1,			1,			NULL,			NULL,				1,		 0,				1,			1,			1,		 	'L',		NULL,		 NULL from SYS.TABLE_COLUMNS where TABLE_NAME = 'OUBR' and COLUMN_NAME not in('Code', 'Name', 'Remarks');
	
	select * from #tblStructure;
	
	if :departmntCode <> -1
	then

		select a.USERID, a.USER_CODE as "UserCode", a.U_NAME as "UserName", b."Code" as "DepartmntCode", b."Name" as "Departmnt", b."Remarks" as "DepartmntDesc", c."Code" as "BranCode", c."Name" as "Branch", c."Remarks" as "N'BranDesc" 
		from OUSR a 
		left join OUDP b on a."Department" = b."Code"
		left join OUBR c on a."Branch" = c."Code"
		where a."Branch" <> 99 and a."Department" = :departmntCode
		order by "UserName" asc;
		return;
	end if;

	select a.USERID, a.USER_CODE as "UserCode", a.U_NAME as "UserName", b."Code" as "DepartmntCode", b."Name" as "Departmnt", b."Remarks" as "DepartmntDesc", c."Code" as "BranCode", c."Name" as "Branch", c."Remarks" as "N'BranDesc" 
	from OUSR a
	left join OUDP b on a."Department" = b."Code"
	left join OUBR c on a."Branch" = c."Code"
	where a."Branch" <> 99
	order by "UserName" asc;
	
END




