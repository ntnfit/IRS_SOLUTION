
-- quandn
CALL APZ_EXISTSPROC('Apz_Cfl_GetSubType');

GO

CREATE PROCEDURE Apz_Cfl_GetSubType (
	IN 	DataType NVARCHAR(50) DEFAULT ''
)
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER

AS
BEGIN
	CREATE LOCAL TEMPORARY COLUMN TABLE #tblStructure (
		 "ID" INT
		,"ColumnName" NVARCHAR(50) NOT NULL
		,"Caption" NVARCHAR(100)
		,"DataType" NVARCHAR(50) DEFAULT 'nvarchar'	--int decimal money datetime date/ numeric alpha Memo(general) Numeric Date float(unit)  ValidValue(combobox) LinkedTable LinkedUDO* LinkedObject cfl(choosefromlist) LinkedCflObject
	                                        		-- color, validcolor (combobox+color),	linkedStore, LinkedCrystal
		,"SubType" NVARCHAR(500)					-- None Phone Percentage Address Image Measurement Price Quantity Rate Sum Time, UDT dùng cho LinkedTable, (udo code : dùng cho LinkedUDO)*	 , ObjectType(số): dùng cho LinkedObject
	                            					-- type of cfl - số của type hoặc tên store(storename @value, @userName, @branch) lấy dữ liệu cfl-store phải có structure, sub - sub form
		,"Width" INT DEFAULT 75
		,"GroupIndex" INT DEFAULT -1				-- dùng cho grid  default -1
		,"Visible" TINYINT DEFAULT 1
		,"FormatType" NVARCHAR(20)					-- numeric, datetime/ 
		,"DisplayFormat" NVARCHAR(20) NULL			-- '{0:n2}'
		,"FilterString" NVARCHAR(5000)
		,"ReadOnly" TINYINT DEFAULT 0
		,"AllowEdit" TINYINT DEFAULT 1
		,"AllowMove" TINYINT DEFAULT 1
		,"AllowSize" TINYINT DEFAULT 1
		,"AllowSort" TINYINT DEFAULT 1
		,"Fixed" VARCHAR(1)							-- L:Left / R:Right
		,"BackColor" NVARCHAR(20)					-- color column //list color https://msdn.microsoft.com/en-us/library/aa358802(v=vs.85).aspx
		,"ForeColor" NVARCHAR(20)
		,"SummaryType" NVARCHAR(10)					-- sum,average...
		,"GroupSummaryType" NVARCHAR(10)
		,"GroupSummaryShowInFooter" TINYINT
		,"SummaryFormat" NVARCHAR(20)				-- (AVR={0:#.##})/ Count ({0})/ Custom format/ Max (MAX={0})/ Min (MIN={0})/ Sum (SUM={0:#.##})
		,"Expression" NVARCHAR(5000)
		,"Parent" TINYINT							-- dùng cho treelist
		,"Key" TINYINT
		,"PivotArea" NVARCHAR(20)
	);	
	
	IF 	   :DataType = 'cfl'
		OR :DataType = 'validvalue' 
		OR :DataType = 'linkedCflobject' 
		OR :DataType = 'validcolor' 
		OR :DataType = 'sub' THEN
		INSERT INTO #tblStructure 
						("ID", "ColumnName",	"Caption",		"DataType",		"Width","GroupIndex", "Visible", "FormatType", "DisplayFormat", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "SummaryType", "GroupSummaryType")
	  		SELECT   		1, N'SPECIFIC_NAME',N'Name',		'nvarchar',		500,	-1,			  1,		 NULL,		   NULL,			1,		    0,			1,			 1,				1,		    'L',	  NULL,		     NULL FROM DUMMY
			UNION SELECT	2, N'CREATED',		N'Create date',	'datetime',		100,	-1,			  0,		 'datetime',	'dd/MM/yyyy HH:mm',			1,		    0,			 1,			  1,			1,		    'L',	  NULL,		     NULL FROM DUMMY		
			UNION SELECT	3, N'LAST_ALTERED',	N'Last update',	'datetime',		100,	-1,			  0,		 'datetime',	'dd/MM/yyyy HH:mm',			1,		    0,			 1,			  1,			1,		    'L',	  NULL,		     NULL FROM DUMMY;	
		
		SELECT * FROM #tblStructure ORDER BY ID;
		
		SELECT a.PROCEDURE_NAME || IFNULL((SELECT ' ' || STRING_AGG('@'|| b.PARAMETER_NAME || ',')
									FROM SYS.PROCEDURE_PARAMETERS b 
									WHERE b.SCHEMA_NAME = CURRENT_SCHEMA 
										AND b.PROCEDURE_NAME = a.PROCEDURE_NAME
										AND b.PARAMETER_TYPE = 'IN'),'') AS "SPECIFIC_NAME" 				  
			FROM SYS.PROCEDURES a
			WHERE a.SCHEMA_NAME = CURRENT_SCHEMA 
			ORDER BY a.SCHEMA_NAME;
	ELSEIF :DataType = 'linkedstore' THEN
		INSERT INTO #tblStructure 
						("ID", "ColumnName",	"Caption",		"DataType",		"Width","GroupIndex", "Visible", "FormatType", "DisplayFormat", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "SummaryType", "GroupSummaryType")
	  		SELECT   		1, N'Code',			N'Code',		'nvarchar',		75,		-1,			1,			NULL,			NULL,				1,		 0,				1,			1,				1,		 '',	NULL,		 NULL FROM DUMMY
			UNION SELECT	2, N'Name',			N'Name',		'nvarchar',		100,	-1,			1,			NULL,			NULL,				1,		 0,				1,			1,				1,		 '',	NULL,		 NULL FROM DUMMY		
			UNION SELECT	3, N'MenuCapt',		N'MenuCapt',	'nvarchar',		250,	-1,			1,			NULL,			NULL,				1,		 0,				1,			1,				1,		 '',	NULL,		 NULL FROM DUMMY;			
		SELECT * FROM #tblStructure ORDER BY ID;
		
		SELECT "Code" || ' ' || (select string_agg("ParamName",',') from APZ_UDR1 where "Code"=a."Code") "Code", "Name", "MenuCapt" FROM APZ_OUDR a WHERE "MenuItem" = 'Y' ORDER BY "Code";
	ELSEIF :DataType = 'linkedudo' THEN
		INSERT INTO #tblStructure 
						("ID", "ColumnName",	"Caption",		"DataType",		"Width","GroupIndex", "Visible", "FormatType", "DisplayFormat", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "SummaryType", "GroupSummaryType")
	  		SELECT   		1, N'Code',			N'Code',		'nvarchar',		75,		-1,			1,			NULL,			NULL,				1,		 0,				1,			1,				1,		 '',	NULL,		 NULL FROM DUMMY
			UNION SELECT	2, N'Name',			N'Name',		'nvarchar',		100,	-1,			1,			NULL,			NULL,				1,		 0,				1,			1,				1,		 '',	NULL,		 NULL FROM DUMMY		
			UNION SELECT	3, N'MenuCapt',		N'MenuCapt',	'nvarchar',		250,	-1,			1,			NULL,			NULL,				1,		 0,				1,			1,				1,		 '',	NULL,		 NULL FROM DUMMY;			
		SELECT * FROM #tblStructure ORDER BY ID;
		
		SELECT "Code", "Name", "MenuCapt" FROM OUDO WHERE "MenuItem" = 'Y' ORDER BY "Code";
	ELSEIF :DataType = 'linkedtable' THEN
		INSERT INTO #tblStructure 
						("ID", "ColumnName",	"Caption",		"DataType",		"Width","GroupIndex", "Visible", "FormatType", "DisplayFormat", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "SummaryType", "GroupSummaryType")
	  		SELECT   		1, N'TableName',		N'TableName',		'nvarchar',		100,	-1,			1,			NULL,		NULL,					1,		 0,			1,			1,			1,		 '',	NULL,		 NULL FROM DUMMY		
			UNION SELECT	2, N'Descr',			N'Description',		'nvarchar',		250,	-1,			1,			NULL,		NULL,					1,		 0,			1,			1,			1,		 '',	NULL,		 NULL FROM DUMMY;			
		SELECT * FROM #tblStructure ORDER BY ID;
		
		SELECT IFNULL(a."TableName", '''') AS "TableName", IFNULL(a."Descr", '''') AS "Descr" 
			FROM OUTB a 
			WHERE "ObjectType" = 0 
				OR "ObjectType" = 5 
			ORDER BY "TableName";
	ELSEIF :DataType = 'linkedobject' THEN
		INSERT INTO #tblStructure 
						("ID", "ColumnName",	"Caption",		"DataType",		"Width","GroupIndex", "Visible", "FormatType", "DisplayFormat", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "SummaryType", "GroupSummaryType")
	  		SELECT   		1, N'ObjectType',		N'Object Type',		'nvarchar',		100,	-1,			1,			NULL,		NULL,					1,		 0,			1,			1,			1,		 '',	NULL,		 NULL FROM DUMMY		
			UNION SELECT	2, N'Descr',			N'Description',		'nvarchar',		250,	-1,			1,			NULL,		NULL,					1,		 0,			1,			1,			1,		 '',	NULL,		 NULL FROM DUMMY;			
		SELECT * FROM #tblStructure ORDER BY ID;
		
		CREATE LOCAL TEMPORARY COLUMN TABLE #tbl (
			"ObjectType" nvarchar(100) NOT NULL, 
			"Descr" nvarchar(254) NOT NULL
			);
		INSERT INTO #tbl("ObjectType", "Descr")
			SELECT 		 N'2',			N'oBusinessPartners' 			FROM DUMMY
			UNION SELECT N'4',			N'oItems' 						FROM DUMMY
			UNION SELECT N'13',			N'oInvoices' 					FROM DUMMY
			UNION SELECT N'14',			N'oCreditNotes' 				FROM DUMMY
			UNION SELECT N'15',			N'oDeliveryNotes' 				FROM DUMMY
			UNION SELECT N'234000031',  N'oReturnRequest'				FROM DUMMY
			UNION SELECT N'16',			N'oReturns' 					FROM DUMMY
			UNION SELECT N'17',			N'oOrders' 						FROM DUMMY
			UNION SELECT N'23',			N'oQuotations' 					FROM DUMMY
			UNION SELECT N'18',			N'oPurchaseInvoices' 			FROM DUMMY
			UNION SELECT N'19',			N'oPurchaseCreditNotes' 		FROM DUMMY
			UNION SELECT N'25',			N'oAPReserveInvoice' 			FROM DUMMY
			UNION SELECT N'20',			N'oPurchaseDeliveryNotes' 		FROM DUMMY
			UNION SELECT N'15',			N'oDeliveryNotes' 				FROM DUMMY
			UNION SELECT N'21',			N'oPurchaseReturns' 			FROM DUMMY
			UNION SELECT N'22',			N'oPurchaseOrders' 				FROM DUMMY
			UNION SELECT N'40',			N'oPaymentTermsTypes' 			FROM DUMMY
			UNION SELECT N'59',			N'oInventoryGenEntry' 			FROM DUMMY
			UNION SELECT N'60',			N'oInventoryGenExit' 			FROM DUMMY
			UNION SELECT N'64',			N'oWarehouses' 					FROM DUMMY
			UNION SELECT N'66',			N'oProductTrees' 				FROM DUMMY
			UNION SELECT N'67',			N'oStockTransfer' 				FROM DUMMY
			UNION SELECT N'1250000001',	N'oInventoryTransferRequest' 	FROM DUMMY
			UNION SELECT N'97',			N'oSalesOpportunities' 			FROM DUMMY
			UNION SELECT N'112',		N'oDrafts' 						FROM DUMMY
			UNION SELECT N'206000',		N'oUserDefinedObject' 			FROM DUMMY
			UNION SELECT N'170',		N'oContractTemplates' 			FROM DUMMY
			UNION SELECT N'171',		N'oEmployeesInfo' 				FROM DUMMY
			UNION SELECT N'189',		N'oKnowledgeBaseSolutions' 		FROM DUMMY
			UNION SELECT N'202',		N'oProductionOrders' 			FROM DUMMY
			UNION SELECT N'540000006',	N'oPurchaseQuotations' 			FROM DUMMY
			UNION SELECT N'1470000113',	N'oPurchaseRequest' 			FROM DUMMY
			UNION SELECT N'1250000025',	N'oBlanketAgreement' 			FROM DUMMY
			UNION SELECT N'700010',		N'oSerialNumberTransactionReport' FROM DUMMY
			UNION SELECT N'700011',		N'oBatchDetailsTransactionReport' FROM DUMMY
			UNION SELECT N'10000045',	N'oSerialNumberDetails' 		FROM DUMMY
			UNION SELECT N'10000044',	N'oBatchDetails' 				FROM DUMMY
			UNION SELECT N'800000',		N'oShift' 						FROM DUMMY
			UNION SELECT N'800004',		N'oSchedule' 					FROM DUMMY
			UNION SELECT N'6000001',	N'oApplicants' 					FROM DUMMY
			UNION SELECT N'19000096',	N'oQualifications' 				FROM DUMMY
			UNION SELECT N'700013',		N'oAtp' 						FROM DUMMY
			UNION SELECT N'700015',		N'oScheduleForAtp' 				FROM DUMMY
			UNION SELECT N'700019',		N'oPacking' 					FROM DUMMY
			UNION SELECT N'700021',		N'oShipping' 					FROM DUMMY
			UNION SELECT N'700026',		N'oVehicle' 					FROM DUMMY
			UNION SELECT N'700016',		N'oMaterialShortageReport' 		FROM DUMMY
			UNION SELECT N'98',			N'oInterest' 					FROM DUMMY
			UNION SELECT N'99',			N'oInterestLevel' 				FROM DUMMY
			UNION SELECT N'1470000130',	N'oTrackingIssue' 				FROM DUMMY
			UNION SELECT N'1470000132',	N'oProjectManager' 				FROM DUMMY
			UNION SELECT N'1470000134',	N'oReportCompletion' 			FROM DUMMY
			UNION SELECT N'1470000135',	N'oProjectConfirm' 				FROM DUMMY
			UNION SELECT N'1470000137',	N'oInventoryCountingRequest' 	FROM DUMMY
			UNION SELECT N'1250000001',	N'oInventoryTransferRequest' 	FROM DUMMY
			UNION SELECT N'1470000128',	N'oTaskManagement' 				FROM DUMMY
			UNION SELECT N'800001',		N'oWorkSchedule' 				FROM DUMMY
			UNION SELECT N'1470000142',	N'WorkCenter' 					FROM DUMMY
			UNION SELECT N'1470000141',	N'oOperation' 					FROM DUMMY
			UNION SELECT N'1470000144',	N'oServiceLevelAgreement' 		FROM DUMMY
			UNION SELECT N'1470000143',	N'oServiceContractTemplate' 	FROM DUMMY
			UNION SELECT N'1470000145',	N'oMarketingCampaign' 			FROM DUMMY
			UNION SELECT N'10000209',	N'oOutSourcing' 				FROM DUMMY;
			
		SELECT * FROM #tbl ORDER BY "ObjectType";
		DROP TABLE #tbl;
	END IF;
		
	DROP TABLE #tblStructure;
END;