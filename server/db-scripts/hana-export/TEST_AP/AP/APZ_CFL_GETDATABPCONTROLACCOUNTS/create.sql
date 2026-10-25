
Create procedure APZ_Cfl_GetDataBPControlAccounts(
cardType nvarchar(1) default '',
acctType nvarchar(1) default '')
as
begin
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
                	Select	  N'AcctCode',		N'Account Number',		'int',			100,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL FROM DUMMY
				union select N'AcctName',		N'Account Name',		'nvarchar',		200,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL FROM DUMMY
				union select N'CurrTotal',		N'Account Balance',		'nvarchar',		120,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL FROM DUMMY;
				
		
SELECT *
FROM #tblStructure;


if :acctType='D' -- down payment
then
	SELECT
	*
	from 
		OACT
	where
		"LocManTran"='Y';
else if :acctType='O' -- open debts
then
begin
	if :cardType='C' -- customer
then
		SELECT
			*
		from 
			OACT
		where
			"LocManTran"='Y' AND "GroupMask" = 1;
	else
		SELECT
			*
		from 
			OACT
		where
			"LocManTran"='Y' AND "GroupMask" = 2;
	end if;
end;
end if;
end if;
END




