

CREATE PROCEDURE APZ_CFL_WorkingProcessTooltips
AS
BEGIN

	CREATE LOCAL TEMPORARY TABLE #tblStructure
	(
	     "ColumnName" NVARCHAR(50) NOT NULL
	    ,"Caption" NVARCHAR(100) NULL
	    ,"DataType" VARCHAR(20) NULL DEFAULT 'nvarchar' --int decimal money datetime date/ numeric alpha Memo(general) Numeric Date float(unit)  ValidValue(combobox)
		,"SubType" VARCHAR(20) NULL			--none phone percentage address image measurement price quantity rate sum time
	    ,"Width" INT NULL DEFAULT 75
	    ,"GroupIndex" INT NULL DEFAULT -1
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
		--,Parent BIT NULL	
		--,[Key] BIT NULL		
	);
	
	INSERT INTO #tblStructure(		"ColumnName", 		"Caption", 			"DataType", "Width", "GroupIndex", "Visible", "FormatType", "DisplayFormat", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "SummaryType", "GroupSummaryType")
				SELECT 				N'ColumnName',		N'Column Name',		'nvarchar',		85,			-1,			1,			NULL,		NULL,			1,		 	0,			1,				1,			1,		 	'L',		NULL,		 NULL FROM DUMMY
				UNION ALL SELECT	N'TableName',		N'Table Name',		'nvarchar',		150,		-1,			0,			NULL,		NULL,			1,			0,			1,				1,			1,		 	'L',		NULL,		 NULL FROM DUMMY
				UNION ALL SELECT 	N'Descriptions',	N'Descriptions',	'nvarchar',		150,		-1,			1,			NULL,		NULL,			1,			0,			1,				1,			1,		 	'L',		NULL,		 NULL FROM DUMMY
				UNION ALL SELECT	N'Query',			N'Query',			'nvarchar',		150,		-1,			0,			NULL,		NULL,			1,		 	0,			1,				1,			1,		 	'L',		NULL,		 NULL FROM DUMMY;
				
	SELECT * FROM #tblStructure; 

	create local temporary table #dtTemp
	(
		"ColumnName" varchar(10),
		"TableName" varchar(10),
		"Descriptions" nvarchar(254),
		"Query" nvarchar(4000)
	);

	insert into #dtTemp("ColumnName", "TableName", "Descriptions", "Query")
	select 'WddCode', 'OWDD', 'Internal Number', 'select "WddCode" from OWDD where "WddCode" = @wddCode and "DraftEntry" = @draftEntry;' from dummy
	union all select 'OwnerID', 'OWDD', 'User request', 'select b.U_NAME from OWDD a inner join OUSR b on a."OwnerID" = b.USERID where a."WddCode" = @wddCode and a."DraftEntry" = @draftEntry;' from dummy
	union all select 'DocEntry', 'OWDD', 'Document Counter', 'select "DocEntry" from OWDD where "DraftEntry" = @draftEntry;' from dummy
	union all select 'DocDate', 'OWDD', 'Posting Date', 'select TO_VARCHAR(TO_DATE("DocDate"), ''dd.MM.yyyy'') from OWDD where "WddCode" = @wddCode and "DraftEntry" = @draftEntry;' from dummy
	union all select 'CurrStep', 'OWDD', 'Current Stage', 'select b."Name" from OWDD a inner join OWST b on a."CurrStep" = b."WstCode" where a."WddCode" = @wddCode and a."DraftEntry" = @draftEntry;' from dummy
	union all select 'CreateDate', 'OWDD', 'Production Date', 'select TO_VARCHAR(TO_DATE("CreateDate"), ''dd.MM.yyyy'') from OWDD where "WddCode" = @wddCode and "DraftEntry" = @draftEntry;' from dummy
	union all select 'IsDraft', 'OWDD', 'Draft', 'select "IsDraft" from OWDD where "DraftEntry" = @draftEntry;' from dummy
	union all select 'MaxReqr', 'OWDD', 'No. of Authorizers', 'select "MaxReqr" from OWDD where "DraftEntry" = @draftEntry and "WtmCode" = @wtmCode;' from dummy
	union all select 'MaxRejReqr', 'OWDD', 'No. of Rejects', 'select "MaxRejReqr" from OWDD where "DraftEntry" = @draftEntry and "WtmCode" = @wtmCode;' from dummy
	union all select 'WtmCode', 'OWDD', 'Template Code', 'select "WtmCode" from OWDD where "DraftEntry" = @draftEntry;' from dummy
	union all select 'WtmName', 'OWTM', 'Template Name', 'select "Name" from OWTM where "WtmCode" = @wtmCode;' from dummy
	union all select 'Remarks', 'OWTM', 'Template Remarks', 'select "Remarks" from OWTM where "WtmCode" = @wtmCode;' from dummy
	union all select 'Conds', 'OWTM', 'Template Conditions', 'select "Conds" from OWTM where "WtmCode" = @wtmCode;' from dummy
	union all select 'CardCode', 'ODRF', 'Card Code', 'select b."CardCode" from OWDD a inner join ODRF b on a."DraftEntry" = b."DocEntry" where a."WddCode" = @wddCode and a."DraftEntry" = @draftEntry;' from dummy
	union all select 'CardName', 'ODRF', 'Card Name', 'select b."CardName" from OWDD a inner join ODRF b on a."DraftEntry" = b."DocEntry" where a."WddCode" = @wddCode and a."DraftEntry" = @draftEntry;' from dummy
	union all select 'UserSign', 'OWDD', 'User Signature', 'select b.USER_CODE from OWDD a inner join OUSR b on a."UserSign" = b.USERID where a."WddCode" = @wddCode and a."DraftEntry" = @draftEntry;' from dummy;
	
	select * from #dtTemp order by "ColumnName";
			
END




