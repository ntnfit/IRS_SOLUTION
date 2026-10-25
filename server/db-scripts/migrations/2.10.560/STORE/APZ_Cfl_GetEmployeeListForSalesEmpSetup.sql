
-- APZ_Cfl_GetEmployeeListForSalesEmpSetup
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE type='p' and id = OBJECT_ID('APZ_Cfl_GetEmployeeListForSalesEmpSetup'))
BEGIN
    DROP PROCEDURE APZ_Cfl_GetEmployeeListForSalesEmpSetup
END

GO

--	APZ_Cfl_GetEmployeeListForSalesEmpSetup ''
CREATE proc [dbo].[APZ_Cfl_GetEmployeeListForSalesEmpSetup]
@BPLId NVARCHAR(10) = ''
as

DECLARE @tblStructure AS TABLE
    (
     ColumnName NVARCHAR(50) NOT NULL
    ,Caption NVARCHAR(100) NULL
    ,DataType VARCHAR(20) NULL DEFAULT 'nvarchar' --int decimal money datetime date/ numeric alpha Memo(general) Numeric Date float(unit)  ValidValue(combobox)
	,SubType VARCHAR(20) NULL			--none phone percentage address image measurement price quantity rate sum time
    ,Width INT NULL DEFAULT 75
    ,GroupIndex INT NULL DEFAULT -1	-- d�ng cho grid  default -1
    ,Visible BIT NULL DEFAULT 1
    ,FormatType NVARCHAR(20) NULL   -- numeric, datetime/ 
    ,DisplayFormat NVARCHAR(20) NULL  -- '{0:n2}'
    ,[ReadOnly] BIT NULL DEFAULT 0
    ,AllowEdit BIT NULL DEFAULT 1
    ,AllowMove BIT NULL DEFAULT 1
    ,AllowSize BIT NULL DEFAULT 1
    ,AllowSort BIT NULL DEFAULT 1
    ,Fixed VARCHAR(1) NULL	-- L:Left / R:Right
	-- Summary Item
    ,SummaryType VARCHAR(10) NULL	-- sum,average...
	-- Group Summary
    ,GroupSummaryType VARCHAR(10) NULL	-- sum,average...
	--,Parent BIT NULL	-- d�ng cho treelist
	--,[Key] BIT NULL		-- d�ng cho treelist (identity)
    )
    INSERT INTO @tblStructure 
				(ColumnName,		Caption,				DataType,		Width,	GroupIndex, Visible,	FormatType, DisplayFormat, ReadOnly, AllowEdit, AllowMove, AllowSize, AllowSort, Fixed, SummaryType, GroupSummaryType)
		 VALUES 
				(N'firstName',		N'First Name',			'nvarchar',		150,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 '',	NULL,		 NULL)
				,(N'lastName',		N'Last Name',			'nvarchar',		150,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 '',	NULL,		 NULL)
				,(N'empID',			N'Employee No.',		'int',			150,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 '',	NULL,		 NULL)
				,(N'ExtEmpNo',		N'Ext. Employee No',	'nvarchar',		75,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 '',	NULL,		 NULL)
				,(N'BranchName',	N'Branch',				'nvarchar',		150,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 '',	NULL,		 NULL)
				,(N'Department',	N'Department',			'nvarchar',		150,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 '',	NULL,		 NULL)
		
SELECT *
    FROM @tblStructure
	
	SELECT a.*, b.Name AS BranchName, c.Name AS Department
	, (select top 1 LinkThumbnails from APZ_D_UUPIM f where f.ObjectType = '1470000171' and a.userId = f.UserSign order by TimestampCreated desc) as LinkThumbnails
	into #tmp_table
	FROM OHEM          a
		LEFT JOIN OUBR b ON a.branch=b.Code
		LEFT JOIN OUDP c ON a.dept=c.Code
	WHERE a.Active='Y'
	AND (@BPLId = '' OR (@BPLId <>'' AND EXISTS (SELECT 1 FROM HEM10 x WHERE x.empID = a.empID AND x.BPLId IN (SELECT Data FROM dbo.Split(@BPLId,',')))))
	ORDER BY a.lastName

	
	update #tmp_table set middleName='' where  middleName is null
	select * from #tmp_table ORDER BY lastName