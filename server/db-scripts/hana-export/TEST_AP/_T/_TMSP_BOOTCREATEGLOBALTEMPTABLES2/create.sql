-- B1 DEPENDS: BEFORE:PT:PROCESS_START

CREATE PROCEDURE _TmSp_BootCreateGlobalTempTables2
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS 
sqlStr nvarchar(1000);
exists1 int;
BEGIN
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'Customer_Open_Item_List_TempInstallmentsDocument' and "SCHEMA_NAME" = CURRENT_SCHEMA;
		
	if :exists1 <> 0 then
		drop table "Customer_Open_Item_List_TempInstallmentsDocument" CASCADE;
	end if;

	CREATE GLOBAL TEMPORARY TABLE "Customer_Open_Item_List_TempInstallmentsDocument"
	( "DocEntry" 		integer, "InstlmntID" 	integer, "ObjType" 		nvarchar(20), "CardCode" 		nvarchar(15), "CardName" 		nvarchar(100), "NumAtCard" 	nvarchar(100), "SlpCode" 		smallint, "InsTotal" 		DECIMAL(21,6), 
	  "InsTotalFC" 	DECIMAL(21,6), "InsTotalSy" 	DECIMAL(21,6),  "BlockDunn" 	nvarchar(1), "DunnLevel"		integer, "Installmnt"	smallint, "DunDate"		datetime, "IsSales"		nvarchar(1), "IsDunnable" 	nvarchar(1) );

	  
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'Customer_Open_Item_List_TempJournalSource' and "SCHEMA_NAME" = CURRENT_SCHEMA;
		
	if :exists1 <> 0 then
		drop table "Customer_Open_Item_List_TempJournalSource" CASCADE;
	end if;  
	CREATE GLOBAL TEMPORARY TABLE "Customer_Open_Item_List_TempJournalSource"
	( "DocEntry"		integer, "InstlmntID" 	integer, "Installmnt"	smallint, "DunDate"		datetime, "ObjType"		nvarchar(20), "CardCode"		nvarchar(15), "CardName" 		nvarchar(100), "NumAtCard"		nvarchar(100), 
	  "SlpCode"		smallint, "InsTotal"		DECIMAL(21,6), "InsTotalFC"	DECIMAL(21,6), "InsTotalSy"	DECIMAL(21,6), "BlockDunn"		nvarchar(1), "DunnLevel"		integer, "TransType"		nvarchar(20), "IsSales"		nvarchar(1),
	  "IsDunnable"	nvarchar(1), "BoeNum"		integer, "BoeStatus"		nvarchar(1));

  	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'Customer_Open_Item_List_TempOpenDocumentViewByDocDate' and "SCHEMA_NAME" = CURRENT_SCHEMA;
		
	if :exists1 <> 0 then
		drop table "Customer_Open_Item_List_TempOpenDocumentViewByDocDate" CASCADE;
	end if;  
	CREATE GLOBAL TEMPORARY TABLE "Customer_Open_Item_List_TempOpenDocumentViewByDocDate"
	("DocEntry" 		integer,	"TransId"		integer,"Line_ID"		integer,"InstlmntID"	integer,"Account"		nvarchar(15),"ShortName"		nvarchar(15),"Address"		nvarchar(100),"Phone"			nvarchar(20),"TransType"		nvarchar(20),
	"CreatedBy"		integer, "BaseRef"		nvarchar(11), "SourceLine"	smallint, "Debit"			DECIMAL(21,6), "Credit"		DECIMAL(21,6), "FCDebit"		DECIMAL(21,6), "FCCredit"		DECIMAL(21,6), "DebCred"		nvarchar(1), "RefDate"		datetime,
	"DueDate"		datetime, "TaxDate"		datetime, "BalDueCred"	DECIMAL(21,6), "BalFCCred"		DECIMAL(21,6), "BalScCred"		DECIMAL(21,6), "LineMemo"		nvarchar(50), "Indicator"		nvarchar(2), "CardName"		nvarchar(100), 
	"CardCode"		nvarchar(15), "CardName1"		nvarchar(100), "Balance"		DECIMAL(21,6), "DocTotal"		DECIMAL(21,6), "DocTotalFC"	DECIMAL(21,6), "DocTotalSC"	DECIMAL(21,6), "DunnLevel"		integer, "IsSales"		nvarchar(1), 
	"BPCurrency"	nvarchar(3), "FCCurrency"	nvarchar(3), "Installmnt"	smallint, "DunDate"		datetime, "InsTotal"		DECIMAL(21,6),	"InsTotalFC"	DECIMAL(21,6), "InsTotalSy"	DECIMAL(21,6));

  	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'Customer_Open_Item_List_TempOpenDocumentViewByPosting' and "SCHEMA_NAME" = CURRENT_SCHEMA;
		
	if :exists1 <> 0 then
		drop table "Customer_Open_Item_List_TempOpenDocumentViewByPosting" CASCADE;
	end if;  
	CREATE GLOBAL TEMPORARY TABLE "Customer_Open_Item_List_TempOpenDocumentViewByPosting"
	( "DocEntry" 		integer,	"TransId"		integer,"Line_ID"		integer,"InstlmntID"	integer,"Account"		nvarchar(15),"ShortName"		nvarchar(15),"Address"		nvarchar(100),"Phone"			nvarchar(20),
	"TransType"		nvarchar(20),"CreatedBy"		integer,"BaseRef"		nvarchar(11),"SourceLine"	smallint,"Debit"			DECIMAL(21,6),"Credit"		DECIMAL(21,6),"FCDebit"		DECIMAL(21,6),"FCCredit"		DECIMAL(21,6),
	"DebCred"		nvarchar(1),"RefDate"		datetime,"DueDate"		datetime,"TaxDate"		datetime,"BalDueCred"	DECIMAL(21,6),"BalFCCred"		DECIMAL(21,6),"BalScCred"		DECIMAL(21,6),"LineMemo"		nvarchar(50),
	"Indicator"		nvarchar(2),"CardName"		nvarchar(100),"CardCode"		nvarchar(15),"CardName1"		nvarchar(100),"Balance"		DECIMAL(21,6),"DocTotal"		DECIMAL(21,6),"DocTotalFC"	DECIMAL(21,6),"DocTotalSC"	DECIMAL(21,6),
	"DunnLevel"		integer,"IsSales"		nvarchar(1),"BPCurrency"	nvarchar(3),"FCCurrency"	nvarchar(3),"Installmnt"	smallint,"DunDate"		datetime,"InsTotal"		DECIMAL(21,6),	"InsTotalFC"	DECIMAL(21,6),"InsTotalSy"	DECIMAL(21,6));

	select count(1) into exists1 from "PUBLIC"."M_TABLES"
      where "TABLE_NAME" = 'CRSP_Customer_Open_Item_List_TmpResult' and "SCHEMA_NAME" = CURRENT_SCHEMA;
            
      if :exists1 <> 0 then
            drop table "CRSP_Customer_Open_Item_List_TmpResult" CASCADE;
      end if;  
      
      create global temporary table "CRSP_Customer_Open_Item_List_TmpResult"  (
      "Address"			nvarchar(100),	"BalDueCred"		DECIMAL(21,6),	"BalFCCred"		DECIMAL(21,6),	"BaseRef"		nvarchar(11),	"BPCurrency"		nvarchar(3),
      "CardCode"		nvarchar(15),	"CardName"			nvarchar(100),	"Credit"		DECIMAL(21,6),	"Debit"			DECIMAL(21,6),	"DebCred"			nvarchar(1), 
      "DocEntry"		integer,		"DunDate"			datetime,		"DueDate"		datetime,		"DunnLevel"		integer,		"FCCredit"			DECIMAL(21,6), 
      "FCDebit"			DECIMAL(21,6),	"Installmnt"		smallint,		"InstlmntID"	integer,		"InsTotal"		DECIMAL(21,6),	"InsTotalFC"		DECIMAL(21,6),
      "InsTotalSy"		DECIMAL(21,6),	"IsCredit"			nvarchar(1),		"FCCurrency"      nvarchar(3),  "Line_ID"       int, "LineMemo"        nvarchar(50),
      "Phone"			nvarchar(20),	"ReconBaseRef"		nvarchar(11),	"RefDate"             datetime,   "ReconTransId"    int, "ReconRefDate"    datetime,
      "ReconDocumentDate"	datetime,	"ReconJrnlMemo"		nvarchar(50),	"ReconDebCred"       nvarchar(1),    "ReconDebit"      DECIMAL(21,6), "ReconCredit"  DECIMAL(21,6),
      "ReconDebitFc"	DECIMAL(21,6),	"ReconCreditFC"		DECIMAL(21,6),	"ReconJEFCCurrency" nvarchar(3),    "ReconDate"       datetime,   "ReconCurr"       nvarchar(3),
      "ShortName"		nvarchar(15),	"SrcObjAbs"			int,			"SrcObjTyp" nvarchar(20), "TaxDate"         datetime,   "TransType"       nvarchar(20),
      "ServerDate"		datetime,		"IndicatorName"		nvarchar(50),	"realReconSum"       DECIMAL(21,6),    "realReconSumFC" DECIMAL(21,6),     "IsDPMRequest"    nvarchar(1)
      );

	select count(1) into exists1 from "PUBLIC"."M_TABLES"
      where "TABLE_NAME" = 'TMPSOpportAnal_CRD_Output' and "SCHEMA_NAME" = CURRENT_SCHEMA;  
	  
	if :exists1 <> 0 then
		drop table "TMPSOpportAnal_CRD_Output" CASCADE;
	 end if;
	 
	create global temporary table "TMPSOpportAnal_CRD_Output" ("CardCode" NVARCHAR (15), "CardName" NVARCHAR (100), "Status" NVARCHAR (1), "OpprId" int);
	
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
      where "TABLE_NAME" = 'TMSPFifoGetINMRec_INM' and "SCHEMA_NAME" = CURRENT_SCHEMA;  
	  
	if :exists1 <> 0 then
		drop table "TMSPFifoGetINMRec_INM" CASCADE;
	 end if;
	
	create global temporary table "TMSPFifoGetINMRec_INM"(rowNum int,transNum int, openQty DECIMAL(21,6), CalcPrice DECIMAL(21,6), 
		instance int, OpenValue DECIMAL(21,6), inQty DECIMAL(21,6), outQty DECIMAL(21,6), transValue DECIMAL(21,6), expenses DECIMAL(21,6), 
		openExp DECIMAL(21,6), allocation DECIMAL(21,6), openAlloc DECIMAL(21,6), priceDiff DECIMAL(21,6), openPDiff DECIMAL(21,6), 
		expAlloc DECIMAL(21,6), oExpAlloc DECIMAL(21,6), exchDiff DECIMAL(21,6),openEDiff DECIMAL(21,6), negInvAdjs DECIMAL(21,6), 
		openNegInv DECIMAL(21,6), btransVal int, bexpVal DECIMAL(21,6), bnegAVal DECIMAL(21,6), baseQty int, transSeq int, 
		prevTrans int, instanceCpy int);
	
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
      where "TABLE_NAME" = 'TMSPFifoGetINMRec_Data' and "SCHEMA_NAME" = CURRENT_SCHEMA;  
	  
	if :exists1 <> 0 then
		drop table "TMSPFifoGetINMRec_Data" CASCADE;
	 end if;
	 
	create global temporary table "TMSPFifoGetINMRec_Data"(transNum int, openQty DECIMAL(21,6), CalcPrice DECIMAL(21,6), 
		instance int, OpenValue DECIMAL(21,6), inQty DECIMAL(21,6), outQty DECIMAL(21,6), transValue DECIMAL(21,6), expenses DECIMAL(21,6), 
		openExp DECIMAL(21,6), allocation DECIMAL(21,6), openAlloc DECIMAL(21,6), priceDiff DECIMAL(21,6), openPDiff DECIMAL(21,6), 
		expAlloc DECIMAL(21,6), oExpAlloc DECIMAL(21,6), exchDiff DECIMAL(21,6),openEDiff DECIMAL(21,6), negInvAdjs DECIMAL(21,6), 
		openNegInv DECIMAL(21,6), btransVal int, bexpVal DECIMAL(21,6), bnegAVal DECIMAL(21,6), baseQty int, transSeq int, 
		prevTrans int, Warehouse nvarchar(5000));
		
	select count(1) into exists1 from "PUBLIC"."M_TABLES" where  TABLE_NAME = 'TmSpTransferRecordsToUILM_TMP' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	
	if  :exists1 <> 0 then
	    drop TABLE "TmSpTransferRecordsToUILM_TMP" CASCADE;
	end if;
	
	create global temporary table "TmSpTransferRecordsToUILM_TMP" (NAME	nvarchar(256) not null,	POS	int);
	
	select count(1) into exists1 from "PUBLIC"."M_TABLES" where  TABLE_NAME = 'TMPInTransferRecordsToUILM' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	if  :exists1 <> 0 then
	    drop TABLE "TMPInTransferRecordsToUILM" CASCADE;
	end if;
	
	create global temporary table "TMPInTransferRecordsToUILM"(NAME	nvarchar(256) not null,	POS	int);

	select count(1) into exists1 from "PUBLIC"."M_TABLES" where TABLE_NAME = 'TMP_UpgradeFillOILM_NewQty' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	if  :exists1 <> 0 then
	    drop TABLE "TMP_UpgradeFillOILM_NewQty" CASCADE;
	end if;
	
	create global temporary table "TMP_UpgradeFillOILM_NewQty" (MessageId Integer, NewQty Double);

	select count(1) into exists1 from "PUBLIC"."M_TABLES" where TABLE_NAME = 'TMP_TrailBalance_AcctCode' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	if  :exists1 <> 0 then
	    drop TABLE "TMP_TrailBalance_AcctCode" CASCADE;
	end if;
	
	create global temporary table "TMP_TrailBalance_AcctCode" (acct_code varchar(50), acct_level int, id int, scaned varchar(2));

	select count(1) into exists1 from "PUBLIC"."M_TABLES" where TABLE_NAME = 'TMP_TrailBalance_AcctLimited' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	if  :exists1 <> 0 then
	    drop TABLE "TMP_TrailBalance_AcctLimited" CASCADE;
	end if;
	
	create global temporary table "TMP_TrailBalance_AcctLimited" (AcctCode nvarchar(15), AcctName nvarchar(100), AccntntCod nvarchar(12), FrgnName nvarchar(100), Levels int, FatherNum nvarchar(15), Postable nvarchar(1), GroupMask int, GrpLine int, BalDirect nvarchar(4));

	select count(1) into exists1 from "PUBLIC"."M_TABLES" where TABLE_NAME = 'TMP_TrailBalance_Output' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	if  :exists1 <> 0 then
	    drop TABLE "TMP_TrailBalance_Output" CASCADE;
	end if;
	
	create global temporary table "TMP_TrailBalance_Output" (
		"AcctCode" NVARCHAR (15),
		"AcctName" NVARCHAR (100),
		"AccntntCod" NVARCHAR (12),
		"FrgnName" NVARCHAR (100),
		"Levels" int,
		"FatherNum" NVARCHAR (15),
		"Postable" NVARCHAR (1),
		"GroupMask" int,
		"GrpLine" INTEGER ,
		"BalDirect" NVARCHAR (4),
		"VirtualGroup" int,
		"InitCredit" DECIMAL(21,6),
		"InitDebit" DECIMAL(21,6),
		"CurrentCredit" DECIMAL(21,6),
		"CurrentDebit" DECIMAL(21,6),
		"YearCredit" DECIMAL(21,6),
		"YearDebit" DECIMAL(21,6),
		"LCCredit" DECIMAL(21,6),
		"LCDebit" DECIMAL(21,6),
		"FC1Credit" DECIMAL(21,6),
		"FC1Debit" DECIMAL(21,6),
		"FC2Credit" DECIMAL(21,6),
		"FC2Debit" DECIMAL(21,6),
		"FC3Credit" DECIMAL(21,6),
		"FC3Debit" DECIMAL(21,6),
		"FC4Credit" DECIMAL(21,6),
		"FC4Debit" DECIMAL(21,6),
		"FC5Credit" DECIMAL(21,6),
		"FC5Debit" DECIMAL(21,6));

	select count(1) into exists1 from "PUBLIC"."M_TABLES" where TABLE_NAME = 'TMP_ArcProcess_WithId' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	if  :exists1 <> 0 then
	    drop TABLE "TMP_ArcProcess_WithId" CASCADE;
	end if;
	
	create global temporary table "TMP_ArcProcess_WithId" ("obj1Type" varchar(100),"obj1Key1" varchar(100),"obj1Key2" varchar(100),"obj1Key3" varchar(100),"obj1Key4" varchar(100),"obj1Key5" varchar(100),"obj1Key6" varchar(100),"obj2Type" varchar(100),"obj2Key1" varchar(100),"obj2Key2" varchar(100),"obj2Key3" varchar(100),"obj2Key4" varchar(100),"obj2Key5" varchar(100),"obj2Key6" varchar(100), ID INT);

	select count(1) into exists1 from "PUBLIC"."M_TABLES" where TABLE_NAME = 'TMP_ArcProcess_ClusterChilds' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	if  :exists1 <> 0 then
	    drop TABLE "TMP_ArcProcess_ClusterChilds" CASCADE;
	end if;
	
	create global temporary table "TMP_ArcProcess_ClusterChilds" ("ClusterID" int, "vertexid" int);
	
	select count(1) into exists1 from "PUBLIC"."M_TABLES" where TABLE_NAME = 'TMP_ArcProcess_RowNums' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	if  :exists1 <> 0 then
	    drop TABLE "TMP_ArcProcess_RowNums" CASCADE;
	end if;
	
	create global temporary table "TMP_ArcProcess_RowNums" (ClusterID Int, rownum int);

	select count(1) into exists1 from "PUBLIC"."M_TABLES" where TABLE_NAME = 'TMP_Top5SLP_Results' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	if  :exists1 <> 0 then
	    drop TABLE "TMP_Top5SLP_Results" CASCADE;
	end if;

	create global temporary table "TMP_Top5SLP_Results" (slp int, balDue DECIMAL(21,6));

	select count(1) into exists1 from "PUBLIC"."M_TABLES" where TABLE_NAME = 'TMP_GetMaxRange_Results' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	if  :exists1 <> 0 then
	    drop TABLE "TMP_GetMaxRange_Results" CASCADE;
	end if;

	create global temporary table "TMP_GetMaxRange_Results" (DocDateMax DateTime, DocDateMain DateTime);
	
	select count(1) into exists1 from "PUBLIC"."M_TABLES" where TABLE_NAME = 'TMP_InsertInto_NewItemsInTargetITM' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	if  :exists1 <> 0 then
	    drop TABLE "TMP_InsertInto_NewItemsInTargetITM" CASCADE;
	end if;

	create global temporary table "TMP_InsertInto_NewItemsInTargetITM" ("ItemCode" nvarchar(50) NOT NULL);

	select count(1) into exists1 from "PUBLIC"."M_TABLES" where TABLE_NAME = 'TMP_InsertInto_NewItemsInTargetITW' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	if  :exists1 <> 0 then
	    drop TABLE "TMP_InsertInto_NewItemsInTargetITW" CASCADE;
	end if;

	create global temporary table "TMP_InsertInto_NewItemsInTargetITW" ("ItemCode" nvarchar(50) NOT NULL, "WhsCode" nvarchar(8) NOT NULL);

	select count(1) into exists1 from "PUBLIC"."M_TABLES" where TABLE_NAME = 'TMP_IVSR_SupInterReq_Results' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	if  :exists1 <> 0 then
	    drop TABLE "TMP_IVSR_SupInterReq_Results" CASCADE;
	end if;

	create global temporary table "TMP_IVSR_SupInterReq_Results" (Errors nvarchar(1000));

	select count(1) into exists1 from "PUBLIC"."M_TABLES" where TABLE_NAME = 'TMP_IVSR_SupInterReq_TotalLcZeroPQNotZero' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	if  :exists1 <> 0 then
	    drop TABLE "TMP_IVSR_SupInterReq_TotalLcZeroPQNotZero" CASCADE;
	end if;

	create global temporary table "TMP_IVSR_SupInterReq_TotalLcZeroPQNotZero" (
		"MessageID" INTEGER not null,
		"DocEntry" INTEGER,
		"TransType" INTEGER,
		"DocLineNum" SMALLINT,
		"Quantity" DECIMAL(21,6),
		"EffectQty" DECIMAL(21,6),
		"LocType" SMALLINT,
		"LocCode" NVARCHAR (8),
		"TotalLC" DECIMAL(21,6),
		"TotalFC" DECIMAL(21,6),
		"TotalSC" DECIMAL(21,6),
		"BaseAbsEnt" INTEGER,
		"BaseType" INTEGER,
		"BaseCurr" NVARCHAR (3),
		"Currency" NVARCHAR (3),
		"AccumType" INTEGER,
		"ActionType" INTEGER,
		"ExpensesLC" DECIMAL(21,6),
		"ExpensesFC" DECIMAL(21,6),
		"ExpensesSC" DECIMAL(21,6),
		"DocDueDate" TIMESTAMP,
		"ItemCode" NVARCHAR (50),
		"BPCardCode" NVARCHAR (15),
		"DocDate" TIMESTAMP,
		"DocRate" DECIMAL(21,6),
		"Comment" NVARCHAR (254),
		"JrnlMemo" NVARCHAR (50),
		"Ref1" NVARCHAR (11),
		"Ref2" NVARCHAR (11),
		"BaseLine" SMALLINT,
		"SnBType" INTEGER,
		"CreateTime" SMALLINT,
		"DataSource" NVARCHAR (1),
		"CreateDate" TIMESTAMP,
		"OcrCode" NVARCHAR (8),
		"OcrCode2" NVARCHAR (8),
		"OcrCode3" NVARCHAR (8),
		"OcrCode4" NVARCHAR (8),
		"OcrCode5" NVARCHAR (8),
		"DocPrice" DECIMAL(21,6),
		"CardName" NVARCHAR (100),
		"Dscription" NVARCHAR (100),
		"TreeType" NVARCHAR (1),
		"ApplObj" INTEGER,
		"AppObjAbs" INTEGER,
		"AppObjType" NVARCHAR (1),
		"AppObjLine" SMALLINT,
		"BASE_REF" NVARCHAR (11),
		"TransSeqRf" INTEGER,
		"LayerIDRef" INTEGER,
		"VersionNum" NVARCHAR (11),
		"PriceRate" DECIMAL(21,6),
		"PriceCurr" NVARCHAR (3),
		"DocTotal" DECIMAL(21,6),
		"Price" DECIMAL(21,6),
		"CIShbQty" DECIMAL(21,6),
		"SubLineNum" INTEGER,
		"PrjCode" NVARCHAR (20),
		"SlpCode" SMALLINT,
		"TaxDate" TIMESTAMP,
		"UseDocPric" NVARCHAR (1),
		"VendorNum" NVARCHAR (17),
		"SerialNum" NVARCHAR (17),
		"BlockNum" NVARCHAR (100),
		"ImportLog" NVARCHAR (20),
		"Location" INTEGER,
		"DocPrcRate" DECIMAL(21,6),
		"DocPrcCurr" NVARCHAR (3),
		"CgsOcrCod" NVARCHAR (8),
		"CgsOcrCod2" NVARCHAR (8),
		"CgsOcrCod3" NVARCHAR (8),
		"CgsOcrCod4" NVARCHAR (8),
		"CgsOcrCod5" NVARCHAR (8),
		"BSubLineNo" INTEGER,
		"AppSubLine" INTEGER,
		"UserSign" SMALLINT,
		"SysRate" DECIMAL(21,6),
		"ExFromRpt" NVARCHAR (1),
		"Ref3" NVARCHAR (11),
		"EnSetCost" NVARCHAR (1),
		"RetCost" DECIMAL(21,6),
		"DocAction" INTEGER,
		"UseShpdGd" NVARCHAR (1),
		"AddTotalLC" DECIMAL(21,6),
		"AddExpLC" DECIMAL(21,6),
		"IsNegLnQty" NVARCHAR (1),
		"StgSeqNum" INTEGER,
		"StgEntry" INTEGER,
		"StgDesc" NVARCHAR (100));

	select count(1) into exists1 from "PUBLIC"."M_TABLES" where TABLE_NAME = 'TMP_OpportAnalysisCRD_Output' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	if  :exists1 <> 0 then
	    drop TABLE "TMP_OpportAnalysisCRD_Output" CASCADE;
	end if;

	create global temporary table "TMP_OpportAnalysisCRD_Output" ("OpprId" int, "Line" int, "SlpCode" int, "CntctCode" int,"OpenDate" date,"CloseDate" date, "Step_Id" int, "ClosePrcnt" DECIMAL(21,6),"MaxSumLoc" DECIMAL(21,6),"MaxSumSys" DECIMAL(21,6), "Expr1" NVARCHAR(50), "DocId" INT, "ObjType" NVARCHAR (2), "Status" NVARCHAR (1), "Linked" NVARCHAR (1), "WtSumLoc" DECIMAL(21,6), "WtSumSys" DECIMAL(21,6));

	select count(1) into exists1 from "PUBLIC"."M_TABLES" where TABLE_NAME = 'TmpPsar_Sales_An_Itm_Sng_Y_LC' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	if  :exists1 <> 0 then
	    drop TABLE "TmpPsar_Sales_An_Itm_Sng_Y_LC" CASCADE;
	end if;

	create global temporary table "TmpPsar_Sales_An_Itm_Sng_Y_LC" (DocEntry nvarchar(20), DocNum nvarchar(20), SlpCode nvarchar(20), SlpName nvarchar(155), CardCode nvarchar(20), CardName nvarchar(100), DocDate DateTime, DocDueDate DateTime, ItemCode nvarchar(50), ItemName nvarchar(100), Quantity DECIMAL(21,6), Price DECIMAL(21,6), Sales DECIMAL(21,6), GrossProfit DECIMAL(21,6), GrossPrcnt DECIMAL(21,6), PriceSys DECIMAL(21,6), SalesSys DECIMAL(21,6), GrossProfitSys DECIMAL(21,6), GrossPrcntSys DECIMAL(21,6), LineNum int, SalesForPrcnt DECIMAL(21,6), SalesForPrcntSys DECIMAL(21,6));

	select count(1) into exists1 from "PUBLIC"."M_TABLES" where TABLE_NAME = 'TmpPsar_Sales_An_Slp_Sng_M' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	if  :exists1 <> 0 then
	    drop TABLE "TmpPsar_Sales_An_Slp_Sng_M" CASCADE;
	end if;

	create global temporary table "TmpPsar_Sales_An_Slp_Sng_M" (CardCode nvarchar(15), CardOrSlpName nvarchar(155), Month int, Year Int, Sales DECIMAL(21,6), GrossPrft DECIMAL(21,6), GrossPrcnt DECIMAL(21,6), SalesSys DECIMAL(21,6),GrossPrftSys DECIMAL(21,6), GrossPrcntSys DECIMAL(21,6), SalesForPrcnt DECIMAL(21,6), SalesForPrcntSys DECIMAL(21,6));

	select count(1) into exists1 from "PUBLIC"."M_TABLES" where TABLE_NAME = 'TmpPsar_Sales_An_Slp_Sng_M_LC' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	if  :exists1 <> 0 then
	    drop TABLE "TmpPsar_Sales_An_Slp_Sng_M_LC" CASCADE;
	end if;

	create global temporary table "TmpPsar_Sales_An_Slp_Sng_M_LC" (CardCode nvarchar(15), CardOrSlpName nvarchar(155), Month int, Year Int, Sales DECIMAL(21,6), GrossPrft DECIMAL(21,6), GrossPrcnt DECIMAL(21,6), SalesSys DECIMAL(21,6),GrossPrftSys DECIMAL(21,6), GrossPrcntSys DECIMAL(21,6), SalesForPrcnt DECIMAL(21,6), SalesForPrcntSys DECIMAL(21,6));

	select count(1) into exists1 from "PUBLIC"."M_TABLES" where TABLE_NAME = 'TmpPsar_Sales_An_Slp_Sng_Y' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	if  :exists1 <> 0 then
	    drop TABLE "TmpPsar_Sales_An_Slp_Sng_Y" CASCADE;
	end if;

	create global temporary table "TmpPsar_Sales_An_Slp_Sng_Y" (CardCode nvarchar(15), CardOrSlpName nvarchar(155), DocCount int, Sales DECIMAL(21,6), GrossPrft DECIMAL(21,6), GrossPrcnt DECIMAL(21,6), TotalOpen DECIMAL(21,6), SalesSys DECIMAL(21,6), GrossPrftSys DECIMAL(21,6), GrossPrcntSys DECIMAL(21,6), TotalOpenSys DECIMAL(21,6), SalesForPrcnt DECIMAL(21,6), SalesForPrcntSys DECIMAL(21,6));

	select count(1) into exists1 from "PUBLIC"."M_TABLES" where TABLE_NAME = 'TMP_SetBgtAcc_BgAlrChk' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	if  :exists1 <> 0 then
	    drop TABLE "TMP_SetBgtAcc_BgAlrChk" CASCADE;
	end if;

	create global temporary table "TMP_SetBgtAcc_BgAlrChk" (AcctCode nvarchar(15), DoAlert nchar(1) NULL, AlrType nchar(1) , MOverSum float NULL, YOverSum float);

	select count(1) into exists1 from "PUBLIC"."M_TABLES" where TABLE_NAME = 'TMP_SortIBT1AndSRI1ForUpg_CountUpg' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	if  :exists1 <> 0 then
	    drop TABLE "TMP_SortIBT1AndSRI1ForUpg_CountUpg" CASCADE;
	end if;

	create global temporary table "TMP_SortIBT1AndSRI1ForUpg_CountUpg" (isExist int);

	select count(1) into exists1 from "PUBLIC"."M_TABLES" where TABLE_NAME = 'TMP_SortIBT1AndSRI1ForUpg_CountUpgCurNode' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	if  :exists1 <> 0 then
	    drop TABLE "TMP_SortIBT1AndSRI1ForUpg_CountUpgCurNode" CASCADE;
	end if;

	create global temporary table "TMP_SortIBT1AndSRI1ForUpg_CountUpgCurNode" (cnt int);

	select count(1) into exists1 from "PUBLIC"."M_TABLES" where TABLE_NAME = 'TMP_SpecialPriceCopy_RowCount' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	if  :exists1 <> 0 then
	    drop TABLE "TMP_SpecialPriceCopy_RowCount" CASCADE;
	end if;

	create global temporary table "TMP_SpecialPriceCopy_RowCount" ("rownum" int);
	
	--IAR REPORT
	select count(1) into exists1 from "PUBLIC"."M_TABLES" where TABLE_NAME = 'TMP_PREPAREAUDITREPORTDATA1' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	if  :exists1 <> 0 then
	    drop TABLE "TMP_PREPAREAUDITREPORTDATA1" CASCADE;
	end if;
	select count(1) into exists1 from "PUBLIC"."M_TABLES" where TABLE_NAME = 'TMP_PREPAREAUDITREPORTDATA2' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	if  :exists1 <> 0 then
	    drop TABLE "TMP_PREPAREAUDITREPORTDATA2" CASCADE;
	end if;
	select count(1) into exists1 from "PUBLIC"."M_TABLES" where TABLE_NAME = 'TMP_PREPAREAUDITREPORTDATA3' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	if  :exists1 <> 0 then
	    drop TABLE "TMP_PREPAREAUDITREPORTDATA3" CASCADE;
	end if;
	select count(1) into exists1 from "PUBLIC"."M_TABLES" where TABLE_NAME = 'TMP_PREPAREAUDITREPORTDATA4' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	if  :exists1 <> 0 then
	    drop TABLE "TMP_PREPAREAUDITREPORTDATA4" CASCADE;
	end if;
	select count(1) into exists1 from "PUBLIC"."M_TABLES" where TABLE_NAME = 'TMP_PREPAREAUDITREPORTDATAT1' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	if  :exists1 <> 0 then
	    drop TABLE "TMP_PREPAREAUDITREPORTDATAT1" CASCADE;
	end if;
	select count(1) into exists1 from "PUBLIC"."M_TABLES" where TABLE_NAME = 'TMP_PREPAREAUDITREPORTDATAT2' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	if  :exists1 <> 0 then
	    drop TABLE "TMP_PREPAREAUDITREPORTDATAT2" CASCADE;
	end if;
	select count(1) into exists1 from "PUBLIC"."M_TABLES" where TABLE_NAME = 'TMP_PREPAREAUDITREPORTDATAT3' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	if  :exists1 <> 0 then
	    drop TABLE "TMP_PREPAREAUDITREPORTDATAT3" CASCADE;
	end if;
	select count(1) into exists1 from "PUBLIC"."M_TABLES" where TABLE_NAME = 'TMP_PREPAREAUDITREPORTDATAT4' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	if  :exists1 <> 0 then
	    drop TABLE "TMP_PREPAREAUDITREPORTDATAT4" CASCADE;
	end if;
	select count(1) into exists1 from "PUBLIC"."M_TABLES" where TABLE_NAME = 'TMP_CUMULATIVEVALUE' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	if  :exists1 <> 0 then
	    drop TABLE "TMP_CUMULATIVEVALUE" CASCADE;
	end if;
	
	
	CREATE GLOBAL TEMPORARY ROW TABLE TMP_CumulativeValue("PaginationId" INTEGER,	"CumulativeValue" DECIMAL(21,6), "CumulativeQty" DECIMAL(21,6), "CumulativeCost" DECIMAL(21,6)) no logging;
	CREATE global TEMPORARY TABLE TMP_PREPAREAUDITREPORTDATA1(
		"PaginationId" INTEGER, 
		"ItemCode" NVARCHAR (5000) null,
		"ItemName" NVARCHAR (5000) null,
		"TransSeq" INTEGER null,
		"CreateDate" TIMESTAMP null,
		"DocDate" TIMESTAMP null,
		"Document" VARCHAR (5000) null,
		"DocLineNum" INTEGER null,
		"WhsCode" NVARCHAR (8) null,
		"Qty" DECIMAL(21,6) null,
		"CalcPrice" DECIMAL(21,6) null,
		"TransValue" DECIMAL(21,6) null,
		"CumulativeQty" DECIMAL(21,6) null,
		"CumulativeCost" DECIMAL(21,6) null,
		"CumulativeValue" DECIMAL(21,6) null,
		"GLAccount" NVARCHAR (5000) null,
		"EvalSystem" NVARCHAR (1) null,
		"ValuationMethod" VARCHAR (1000) null,
		"CurrentItemCost" DECIMAL(21,6) null,
		"Currency" NVARCHAR (5000) null,
		"Rate" DECIMAL(21,6) null,
		"UserSign" NVARCHAR(155) null,
		"TransType" INTEGER null,
		"CreatedBy" INTEGER null,
		"BASE_REF" NVARCHAR (5000) null,
		"InvntryUom" NVARCHAR (20) null,
		"CostAct" NVARCHAR (5000) null,
		"LineFlag" INTEGER null,
		"NfmCode" NVARCHAR (10) null,
		"Serial" INTEGER null,
		"SeriesStr" NVARCHAR (5000) null,
		"SubStr" NVARCHAR (5000) null,
		"TaxDate" TIMESTAMP NULL,
		"NcmCode" NVARCHAR (10) NULL,
		"CFOPCode" NVARCHAR (5000) null) no logging;

	CREATE global TEMPORARY  TABLE TMP_PREPAREAUDITREPORTDATAT1(
		"ItemCode" NVARCHAR (5000) null,
		"ItemName" NVARCHAR (5000) null,
		"TransSeq" INTEGER null,
		"CreateDate" TIMESTAMP null,
		"DocDate" TIMESTAMP null,
		"Document" VARCHAR (5000) null,
		"DocLineNum" INTEGER null,
		"WhsCode" NVARCHAR (8) null,
		"Qty" DECIMAL(21,6) null,
		"CalcPrice" DECIMAL(21,6) null,
		"TransValue" DECIMAL(21,6) null,
		"CumulativeQty" DECIMAL(21,6) null,
		"CumulativeCost" DECIMAL(21,6) null,
		"CumulativeValue" DECIMAL(21,6) null,
		"GLAccount" NVARCHAR (5000) null,
		"EvalSystem" NVARCHAR (1) null,
		"ValuationMethod" VARCHAR (1000) null,
		"CurrentItemCost" DECIMAL(21,6) null,
		"Currency" NVARCHAR (5000) null,
		"Rate" DECIMAL(21,6) null,
		"UserSign" NVARCHAR(155) null,
		"TransType" INTEGER null,
		"CreatedBy" INTEGER null,
		"BASE_REF" NVARCHAR (5000) null,
		"InvntryUom" NVARCHAR (20) null,
		"CostAct" NVARCHAR (5000) null,
		"LineFlag" INTEGER null,
		"NfmCode" NVARCHAR (10) NULL,
		"Serial" INTEGER NULL,
		"SeriesStr" NVARCHAR (5000) NULL,
		"SubStr" NVARCHAR (5000) NULL,
		"TaxDate" TIMESTAMP NULL,
		"NcmCode" NVARCHAR (10) NULL,
		"CFOPCode" NVARCHAR (5000) NULL) NO LOGGING;

	CREATE global TEMPORARY  TABLE TMP_PREPAREAUDITREPORTDATA2(
		"PaginationId" INTEGER, 
		"GLAccount" NVARCHAR (30) null,
		"TransSeq" INTEGER null,
		"CreateDate" TIMESTAMP null,
		"DocDate" TIMESTAMP null,
		"DocLineNum" INTEGER null,
		"ItemCode" NVARCHAR (50) null,
		"ItemName" NVARCHAR (100) null,
		"WhsCode" NVARCHAR (8) null,
		"Qty" DECIMAL(21,6) null,
		"CalcPrice" DECIMAL(21,6) null,
		"TransValue" DECIMAL(21,6) null,
		"CumulativeValue" DECIMAL(21,6) null,
		"InvntAct" NVARCHAR (5000) null,
		"EvalSystem" NVARCHAR (1) null,
		"Currency" NVARCHAR (5000) null,
		"Rate" DECIMAL(21,6) null,
		"UserSign" NVARCHAR(155) null,
		"TransType" INTEGER null,
		"CreatedBy" INTEGER null,
		"BASE_REF" NVARCHAR (5000) null,
		"InvntryUom" NVARCHAR (20) null,
		"CostAct" NVARCHAR (5000) null,
		"LineFlag" INTEGER NULL,
		"NfmCode" NVARCHAR (10) NULL,
		"Serial" INTEGER NULL,
		"SeriesStr" NVARCHAR (5000) NULL,
		"SubStr" NVARCHAR (5000) NULL,
		"TaxDate" TIMESTAMP NULL,
		"NcmCode" NVARCHAR (10) NULL,
		"CFOPCode" NVARCHAR (5000) NULL) NO LOGGING;

	CREATE global TEMPORARY  TABLE TMP_PREPAREAUDITREPORTDATAT2(
		"GLAccount" NVARCHAR (30) null,
		"TransSeq" INTEGER null,
		"CreateDate" TIMESTAMP null,
		"DocDate" TIMESTAMP null,
		"DocLineNum" INTEGER null,
		"ItemCode" NVARCHAR (50) null,
		"ItemName" NVARCHAR (100) null,
		"WhsCode" NVARCHAR (8) null,
		"Qty" DECIMAL(21,6) null,
		"CalcPrice" DECIMAL(21,6) null,
		"TransValue" DECIMAL(21,6) null,
		"CumulativeValue" DECIMAL(21,6) null,
		"InvntAct" NVARCHAR (5000) null,
		"EvalSystem" NVARCHAR (1) null,
		"Currency" NVARCHAR (5000) null,
		"Rate" DECIMAL(21,6) null,
		"UserSign" NVARCHAR(155) null,
		"TransType" INTEGER null,
		"CreatedBy" INTEGER null,
		"BASE_REF" NVARCHAR (5000) null,
		"InvntryUom" NVARCHAR (20) NULL,
		"CostAct" NVARCHAR (5000) NULL,
		"LineFlag" INTEGER NULL,
		"NfmCode" NVARCHAR (10) NULL,
		"Serial" INTEGER NULL,
		"SeriesStr" NVARCHAR (5000) NULL,
		"SubStr" NVARCHAR (5000) NULL,
		"TaxDate" TIMESTAMP NULL,
		"NcmCode" NVARCHAR (10) NULL,
		"CFOPCode" NVARCHAR (5000) NULL) NO LOGGING;
		
	CREATE global TEMPORARY  TABLE  TMP_PREPAREAUDITREPORTDATA3(
		"PaginationId" INTEGER, 
		"ItemCode" NVARCHAR (5000) null,
		"ItemName" NVARCHAR (5000) null,
		"WhsCode" NVARCHAR (5000) null,
		"TransSeq" INTEGER null,
		"CreateDate" TIMESTAMP null,
		"DocDate" TIMESTAMP null,
		"Document" VARCHAR (5000) null,
		"DocLineNum" INTEGER null,
		"Qty" DECIMAL(21,6) null,
		"CalcPrice" DECIMAL(21,6) null,
		"TransValue" DECIMAL(21,6) null,
		"CumulativeQty" DECIMAL(21,6) null,
		"CumulativeCost" DECIMAL(21,6) null,
		"CumulativeValue" DECIMAL(21,6) null,
		"GLAccount" NVARCHAR (5000) null,
		"EvalSystem" NVARCHAR (1) null,
		"ValuationMethod" VARCHAR (1000) null,
		"CurrentItemCost" DECIMAL(21,6) null,
		"Currency" NVARCHAR (5000) null,
		"Rate" DECIMAL(21,6) null,
		"UserSign" NVARCHAR(155) null,
		"TransType" INTEGER null,
		"CreatedBy" INTEGER null,
		"BASE_REF" NVARCHAR (5000) null,
		"InvntryUom" NVARCHAR (20) null,
		"CostAct" NVARCHAR (5000) null,
		"LineFlag" INTEGER NULL,
		"NfmCode" NVARCHAR (10) NULL,
		"Serial" INTEGER NULL,
		"SeriesStr" NVARCHAR (5000) NULL,
		"SubStr" NVARCHAR (5000) NULL,
		"TaxDate" TIMESTAMP NULL,
		"NcmCode" NVARCHAR (10) NULL,
		"CFOPCode" NVARCHAR (5000) NULL) NO LOGGING;
		
	CREATE global TEMPORARY  TABLE  TMP_PREPAREAUDITREPORTDATAT3(
		"ItemCode" NVARCHAR (5000) null,
		"ItemName" NVARCHAR (5000) null,
		"WhsCode" NVARCHAR (5000) null,
		"TransSeq" INTEGER null,
		"CreateDate" TIMESTAMP null,
		"DocDate" TIMESTAMP null,
		"Document" VARCHAR (5000) null,
		"DocLineNum" INTEGER null,
		"Qty" DECIMAL(21,6) null,
		"CalcPrice" DECIMAL(21,6) null,
		"TransValue" DECIMAL(21,6) null,
		"CumulativeQty" DECIMAL(21,6) null,
		"CumulativeCost" DECIMAL(21,6) null,
		"CumulativeValue" DECIMAL(21,6) null,
		"GLAccount" NVARCHAR (5000) null,
		"EvalSystem" NVARCHAR (1) null,
		"ValuationMethod" VARCHAR (1000) null,
		"CurrentItemCost" DECIMAL(21,6) null,
		"Currency" NVARCHAR (5000) null,
		"Rate" DECIMAL(21,6) null,
		"UserSign" NVARCHAR(155) null,
		"TransType" INTEGER null,
		"CreatedBy" INTEGER null,
		"BASE_REF" NVARCHAR (5000) null,
		"InvntryUom" NVARCHAR (20) null,
		"CostAct" NVARCHAR (5000) null,
		"LineFlag" INTEGER NULL,
		"NfmCode" NVARCHAR (10) NULL,
		"Serial" INTEGER NULL,
		"SeriesStr" NVARCHAR (5000) NULL,
		"SubStr" NVARCHAR (5000) NULL,
		"TaxDate" TIMESTAMP NULL,
		"NcmCode" NVARCHAR (10) NULL,
		"CFOPCode" NVARCHAR (5000) NULL) NO LOGGING;
		
	CREATE global TEMPORARY TABLE TMP_PREPAREAUDITREPORTDATA4(
		"PaginationId" INTEGER, 
		"WhsCode" NVARCHAR (5000) null,
		"ItemCode" NVARCHAR (5000) null,
		"ItemName" NVARCHAR (5000) null,
		"TransSeq" INTEGER null,
		"CreateDate" TIMESTAMP null,
		"DocDate" TIMESTAMP null,
		"Document" VARCHAR (5000) null,
		"DocLineNum" INTEGER null,
		"Qty" DECIMAL(21,6) null,
		"CalcPrice" DECIMAL(21,6) null,
		"TransValue" DECIMAL(21,6) null,
		"CumulativeQty" DECIMAL(21,6) null,
		"CumulativeCost" DECIMAL(21,6) null,
		"CumulativeValue" DECIMAL(21,6) null,
		"GLAccount" NVARCHAR (5000) null,
		"EvalSystem" NVARCHAR (1) null,
		"ValuationMethod" VARCHAR (1000) null,
		"CurrentItemCost" DECIMAL(21,6) null,
		"Currency" NVARCHAR (5000) null,
		"Rate" DECIMAL(21,6) null,
		"UserSign" NVARCHAR(155) null,
		"TransType" INTEGER null,
		"CreatedBy" INTEGER null,
		"BASE_REF" NVARCHAR (5000) null,
		"InvntryUom" NVARCHAR (20) null,
		"CostAct" NVARCHAR (5000) null,
		"LineFlag" INTEGER NULL,
		"NfmCode" NVARCHAR (10) NULL,
		"Serial" INTEGER NULL,
		"SeriesStr" NVARCHAR (5000) NULL,
		"SubStr" NVARCHAR (5000) NULL,
		"TaxDate" TIMESTAMP NULL,
		"NcmCode" NVARCHAR (10) NULL,
		"CFOPCode" NVARCHAR (5000) NULL) NO LOGGING;
		
	CREATE global TEMPORARY TABLE TMP_PREPAREAUDITREPORTDATAT4(
		"WhsCode" NVARCHAR (5000) null,
		"ItemCode" NVARCHAR (5000) null,
		"ItemName" NVARCHAR (5000) null,
		"TransSeq" INTEGER null,
		"CreateDate" TIMESTAMP null,
		"DocDate" TIMESTAMP null,
		"Document" VARCHAR (5000) null,
		"DocLineNum" INTEGER null,
		"Qty" DECIMAL(21,6) null,
		"CalcPrice" DECIMAL(21,6) null,
		"TransValue" DECIMAL(21,6) null,
		"CumulativeQty" DECIMAL(21,6) null,
		"CumulativeCost" DECIMAL(21,6) null,
		"CumulativeValue" DECIMAL(21,6) null,
		"GLAccount" NVARCHAR (5000) null,
		"EvalSystem" NVARCHAR (1) null,
		"ValuationMethod" VARCHAR (1000) null,
		"CurrentItemCost" DECIMAL(21,6) null,
		"Currency" NVARCHAR (5000) null,
		"Rate" DECIMAL(21,6) null,
		"UserSign" NVARCHAR(155) null,
		"TransType" INTEGER null,
		"CreatedBy" INTEGER null,
		"BASE_REF" NVARCHAR (5000) null,
		"InvntryUom" NVARCHAR (20) null,
		"CostAct" NVARCHAR (5000) null,
		"LineFlag" INTEGER NULL,
		"NfmCode" NVARCHAR (10) NULL,
		"Serial" INTEGER NULL,
		"SeriesStr" NVARCHAR (5000) NULL,
		"SubStr" NVARCHAR (5000) NULL,
		"TaxDate" TIMESTAMP NULL,
		"NcmCode" NVARCHAR (10) NULL,
		"CFOPCode" NVARCHAR (5000) NULL) NO LOGGING;
	--IAR REPORT END
	
	
	
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
      where "TABLE_NAME" = 'CRSP_Monthly_Customer_Status_Report_Customers' and "SCHEMA_NAME" = CURRENT_SCHEMA;  
	  
	if :exists1 <> 0 then
		drop table "CRSP_Monthly_Customer_Status_Report_Customers" CASCADE;
	end if;
	
	Create global temporary Table "CRSP_Monthly_Customer_Status_Report_Customers" (CardCode nvarchar(15), CardName nvarchar(100), GroupCode smallint, Balance DECIMAL(21,6));
	
	
	
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
      where "TABLE_NAME" = 'CRSP_Monthly_Customer_Status_Report_BPMonthly' and "SCHEMA_NAME" = CURRENT_SCHEMA;  
	  
	if :exists1 <> 0 then
		drop table "CRSP_Monthly_Customer_Status_Report_BPMonthly" CASCADE;
	end if;
	
	Create global temporary Table "CRSP_Monthly_Customer_Status_Report_BPMonthly" (YearParam int, MonthParam int, CardCode NVARCHAR(15), CardName NVARCHAR(100), GroupCode smallint, 
		RDRTotal DECIMAL(21,6), GrossBase DECIMAL(21,6), GrossProfit DECIMAL(21,6), 
		DLNTotal DECIMAL(21,6), RDNTotal DECIMAL(21,6), 
		INVTotal DECIMAL(21,6), RINTotal DECIMAL(21,6), RCTTotal DECIMAL(21,6), 
		OpenBalance DECIMAL(21,6), CloseBalance DECIMAL(21,6));
	
	
	
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
      where "TABLE_NAME" = 'CRSP_Monthly_Customer_Status_Report_MonthBegin' and "SCHEMA_NAME" = CURRENT_SCHEMA;  
	  
	if :exists1 <> 0 then
		drop table "CRSP_Monthly_Customer_Status_Report_MonthBegin" CASCADE;
	end if;
	
	Create global temporary Table "CRSP_Monthly_Customer_Status_Report_MonthBegin" (MonthBegin Date);
	
	
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
      where "TABLE_NAME" = 'CRSP_Monthly_Customer_Status_Report_TblResult' and "SCHEMA_NAME" = CURRENT_SCHEMA;  
	  
	if :exists1 <> 0 then
		drop table "CRSP_Monthly_Customer_Status_Report_TblResult" CASCADE;
	end if;
	
	Create global temporary Table "CRSP_Monthly_Customer_Status_Report_TblResult" (YearParam int, MonthParam int, 
		CardCode NVARCHAR(15), CardName NVARCHAR(100), GroupCode smallint, 
		RDRTotal DECIMAL(21,6), GrossBase DECIMAL(21,6), GrossProfit DECIMAL(21,6), 
		DLNTotal DECIMAL(21,6), RDNTotal DECIMAL(21,6), 
		INVTotal DECIMAL(21,6), RINTotal DECIMAL(21,6), RCTTotal DECIMAL(21,6), 
		OpenBalance DECIMAL(21,6), CloseBalance DECIMAL(21,6));
		
		
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
      where "TABLE_NAME" = 'B1CFLGetDocuments' and "SCHEMA_NAME" = CURRENT_SCHEMA;  
	if :exists1 <> 0 then
		  drop table "B1CFLGetDocuments" CASCADE;
	end if;
	
	CREATE GLOBAL TEMPORARY TABLE "B1CFLGetDocuments" (
		"DocEntry" INTEGER NOT NULL ,
		 "InstID" INTEGER,
		 "DueDate" date, 
		 "HldCode" NVARCHAR(20),
		 "Instance" VARCHAR(5000),
		 "Code" VARCHAR(5000));
	
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
      where "TABLE_NAME" = 'B1CFLGetDocumentsHoliday' and "SCHEMA_NAME" = CURRENT_SCHEMA;  
	if :exists1 <> 0 then
		  drop table "B1CFLGetDocumentsHoliday" CASCADE;
	end if;

	CREATE GLOBAL TEMPORARY TABLE "B1CFLGetDocumentsHoliday" (
		 "DocEntry" INTEGER NOT NULL ,
		 "InstID" INTEGER,
		 "DueDate" date, 
		 "Instance" VARCHAR(5000),
		 "Code" VARCHAR(5000));

	select count(1) into exists1 from "PUBLIC"."M_TABLES"
      where "TABLE_NAME" = 'TmpPsar_MinCategory' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	if :exists1 <> 0 then
		drop table "TmpPsar_MinCategory" CASCADE;
	end if;
	
	Create global temporary table "TmpPsar_MinCategory"(
		"AbsEntry" int, 
		"Category" nvarchar(1)
	);

	select count(1) into exists1 from "PUBLIC"."M_TABLES"
      where "TABLE_NAME" = 'CRSP_PaymentsOnInvoice' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	if :exists1 <> 0 then
		drop table "CRSP_PaymentsOnInvoice" CASCADE;
	end if;
	
	Create global temporary table "CRSP_PaymentsOnInvoice"(
		"ObjectID" nvarchar(20),
		"TableName" nvarchar(3),
		"ObjDesc" nvarchar(20),
		"Factor" nvarchar(2)
	);
		 
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
      where "TABLE_NAME" = 'CRSP_TEMP_BPRANGE' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	if :exists1 <> 0 then
		drop table "CRSP_TEMP_BPRANGE" CASCADE;
	end if;
	
	CREATE GLOBAL TEMPORARY TABLE CRSP_TEMP_BPRANGE
	(
		MINCODE NVARCHAR(15),
		MAXCODE NVARCHAR(15)
	);
	
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
      where "TABLE_NAME" = 'CRSP_TEMP_OCRD' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	if :exists1 <> 0 then
		drop table "CRSP_TEMP_OCRD" CASCADE;
	end if;
	
	CREATE GLOBAL TEMPORARY TABLE CRSP_TEMP_OCRD
	(
		"CardCode" NVARCHAR(15),
		"CardName" NVARCHAR(100),
		"GroupCode" SMALLINT,
		"Balance" DECIMAL(21,6)
	);
	
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
      where "TABLE_NAME" = 'CR_TEMP_ITME_GOUPS' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	if :exists1 <> 0 then
		drop table "CR_TEMP_ITME_GOUPS" CASCADE;
	end if;
	Create Global Temporary Table CR_TEMP_ITME_GOUPS
	(
		"ItmsGrpNam" NVarChar(20),
		"ItemCode" NVarChar(50)
	);
	
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
      where "TABLE_NAME" = 'CRSP_INVENTORY_ITEMS' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	if :exists1 <> 0 then
		drop table "CRSP_INVENTORY_ITEMS" CASCADE;
	end if;
	
	Create Global Temporary TABLE CRSP_INVENTORY_ITEMS
	(
		"ItemCode" NVarChar(50)
	);
	
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
      where "TABLE_NAME" = 'CRSPProfitLostByCCBR_JDT1' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	if :exists1 <> 0 then
		drop table "CRSPProfitLostByCCBR_JDT1" CASCADE;
	end if;
	
	CREATE GLOBAL TEMPORARY TABLE "CRSPProfitLostByCCBR_JDT1" 
	( 
		 "TransId" DECIMAL NOT NULL,
		 "Line_ID" DECIMAL NOT NULL,
		 "RefDate" LONGDATE ,
		 "FinancYear" LONGDATE,
		 "Account" NVARCHAR(15),
		 "Typ" VARCHAR(6),
		 "PrcCode" NVARCHAR(8),
		 "OcrCode" NVARCHAR(8),
		 "PrjCode" NVARCHAR(5000),
		 "Credit" DECIMAL(21,6),
		 "CreditPreviousYear" DECIMAL(21,6),
		 "Debit" DECIMAL(21,6),
		 "DebitPreviousYear" DECIMAL(21,6),
		 "CreditBudget" DECIMAL(21,6),
		 "DebitBudget" DECIMAL(21,6),
		 "CreditBudgetPrevYear" DECIMAL(21,6),
		 "DebitBudgetPrevYear" DECIMAL(21,6),
		 "CreditActualMonth" DECIMAL(21,6),
		 "CreditMonthPreviousYear" DECIMAL(21,6),
		 "DebitActualMonth" DECIMAL(21,6),
		 "DebitMonthPreviousYear" DECIMAL(21,6),
		 "CreditActualMonthBudget" DECIMAL(21,6),
		 "DebitActualMonthBudget" DECIMAL(21,6),
		 "CreditFinancYear" DECIMAL(21,6),
		 "DebitFinancYear" DECIMAL(21,6),
		 "CreditFinancYearBudget" DECIMAL(21,6),
		 "DebitFinancYearBudget" DECIMAL(21,6),
		 "CreditPrevFinancYear" DECIMAL(21,6),
		 "DebitPrevFinancYear" DECIMAL(21,6),
		 "BFDebit" DECIMAL(21,6),
		 "BFCredit" DECIMAL(21,6),
		 "OBDebit" DECIMAL(21,6),
		 "OBCredit" DECIMAL(21,6),
		 "BFOBDebit" DECIMAL(21,6),
		 "BFOBCredit" DECIMAL 
	 );
	
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
      where "TABLE_NAME" = 'CRSPProfitLostByCCBR_Res' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	if :exists1 <> 0 then
		drop table "CRSPProfitLostByCCBR_Res" CASCADE;
	end if;
	
	CREATE GLOBAL TEMPORARY TABLE "CRSPProfitLostByCCBR_Res" 
	(
		 "Typ" VARCHAR(6),
		 "TransId" INTEGER NOT NULL ,
		 "Line_ID" INTEGER NOT NULL ,
		 "RefDate" timestamp,
		 "FinancYear" timestamp,
		 "Account" NVARCHAR(15),
		 "PrcCode" NVARCHAR(5000),
		 "OcrCode" NVARCHAR(5000),
		 "PrjCode" NVARCHAR(5000),
		 "Credit" DECIMAL(21,6),
		 "CreditPreviousYear" DECIMAL(21,6),
		 "Debit" DECIMAL(21,6),
		 "DebitPreviousYear" DECIMAL(21,6),
		 "CreditBudget" DECIMAL(21,6),
		 "DebitBudget" DECIMAL(21,6),
		 "CreditBudgetPrevYear" DECIMAL(21,6),
		 "DebitBudgetPrevYear" DECIMAL(21,6),
		 "CreditActualMonth" DECIMAL(21,6),
		 "CreditMonthPreviousYear" DECIMAL(21,6),
		 "DebitActualMonth" DECIMAL(21,6),
		 "DebitMonthPreviousYear" DECIMAL(21,6),
		 "CreditActualMonthBudget" DECIMAL(21,6),
		 "DebitActualMonthBudget" DECIMAL(21,6),
		 "CreditFinancYear" DECIMAL(21,6),
		 "DebitFinancYear" DECIMAL(21,6),
		 "CreditFinancYearBudget" DECIMAL(21,6),
		 "DebitFinancYearBudget" DECIMAL(21,6),
		 "CreditPrevFinancYear" DECIMAL(21,6),
		 "DebitPrevFinancYear" DECIMAL(21,6),
		 "BFDebit" DECIMAL(21,6),
		 "BFCredit" DECIMAL(21,6),
		 "OBDebit" DECIMAL(21,6),
		 "OBCredit" DECIMAL(21,6),
		 "BFOBDebit" DECIMAL(21,6),
		 "BFOBCredit" DECIMAL(21,6)
	 );
	
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
      where "TABLE_NAME" = 'CRSPProfitLostByCCBR_OCR1DistRule' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	if :exists1 <> 0 then
		drop table "CRSPProfitLostByCCBR_OCR1DistRule" CASCADE;
	end if;
	
	CREATE GLOBAL TEMPORARY TABLE "CRSPProfitLostByCCBR_OCR1DistRule" 
	(
	 	"OcrCode" NVARCHAR(8) NOT NULL ,
		 "PrcCode" NVARCHAR(8) NOT NULL ,
		 "OcrTotal" DECIMAL(21,6),
		 "PrcAmount" DECIMAL(21,6),
		 "OcrFactor" DECIMAL(21,6),
		 "Direct" NVARCHAR(1),
		 "ValidFrom" timestamp,
		 "ValidTo" timestamp
	 );
	
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
      where "TABLE_NAME" = 'CRSPProfitLostByCCBR_TmplAcct' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	if :exists1 <> 0 then
		drop table "CRSPProfitLostByCCBR_TmplAcct" CASCADE;
	end if;
	
	CREATE GLOBAL TEMPORARY TABLE "CRSPProfitLostByCCBR_TmplAcct" 
	( 
		 "Code" NVARCHAR(32),
		 "FatherNum" NVARCHAR(32),
		 "Description" NVARCHAR(254),
		 "Lvl" SMALLINT,
		 "Sort" DECIMAL(21,6),
		 "DocType" NVARCHAR(1),
		 "Postable" VARCHAR(1),
		 "FormatCode" NVARCHAR(32),
		 "AcctCodeDisp" NVARCHAR(32),
		 "Segment_0" NVARCHAR(32),
		 "Segment_1" NVARCHAR(32),
		 "Segment_2" NVARCHAR(32),
		 "Segment_3" NVARCHAR(32),
		 "Segment_4" NVARCHAR(32),
		 "Segment_5" NVARCHAR(32),
		 "Segment_6" NVARCHAR(32),
		 "Segment_7" NVARCHAR(32),
		 "Segment_8" NVARCHAR(32),
		 "Segment_9" NVARCHAR(32),
		 "ActCurr" VARCHAR(2),
		 "GroupMask" DECIMAL(21,6),
		 "AccntntCod" NVARCHAR(32),
		 "FrgnName" NVARCHAR(100)
	);
	
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
      where "TABLE_NAME" = 'CRSPProfitLostByCCBR_Plan' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	if :exists1 <> 0 then
		drop table "CRSPProfitLostByCCBR_Plan" CASCADE;
	end if;
	
	CREATE GLOBAL TEMPORARY TABLE "CRSPProfitLostByCCBR_Plan" 
	( 
		 "Lvl" SMALLINT,
		 "AcctCode" NVARCHAR(32),
		 "Description" NVARCHAR(254),
		 "Postable" NVARCHAR(1),
		 "Variator" DECIMAL(21,6),
		 "Liq" VARCHAR,
		 "Level1" NVARCHAR(5000),
		 "Level2" NVARCHAR(5000),
		 "Level3" NVARCHAR(5000),
		 "Level4" NVARCHAR(5000),
		 "Level5" NVARCHAR(5000),
		 "Level6" NVARCHAR(5000),
		 "Level7" NVARCHAR(5000),
		 "Level8" NVARCHAR(5000),
		 "Level1AccntntCod" NVARCHAR(5000),
		 "Level2AccntntCod" NVARCHAR(5000),
		 "Level3AccntntCod" NVARCHAR(5000),
		 "Level4AccntntCod" NVARCHAR(5000),
		 "Level5AccntntCod" NVARCHAR(5000),
		 "Level6AccntntCod" NVARCHAR(5000),
		 "Level7AccntntCod" NVARCHAR(5000),
		 "Level8AccntntCod" NVARCHAR(5000),
		 "Level1FrgnName" NVARCHAR(5000),
		 "Level2FrgnName" NVARCHAR(5000),
		 "Level3FrgnName" NVARCHAR(5000),
		 "Level4FrgnName" NVARCHAR(5000),
		 "Level5FrgnName" NVARCHAR(5000),
		 "Level6FrgnName" NVARCHAR(5000),
		 "Level7FrgnName" NVARCHAR(5000),
		 "Level8FrgnName" NVARCHAR(5000),
		 "Sort" DECIMAL(21,6),
		 "FatherId" NVARCHAR(32),
		 "Id" NVARCHAR(32),
		 "FormatCode" NVARCHAR(5000),
		 "AcctCodeDisp" NVARCHAR(5000),
		 "Segment_0" NVARCHAR(20),
		 "Segment_1" NVARCHAR(20),
		 "Segment_2" NVARCHAR(20),
		 "Segment_3" NVARCHAR(20),
		 "Segment_4" NVARCHAR(20),
		 "Segment_5" NVARCHAR(20),
		 "Segment_6" NVARCHAR(20),
		 "Segment_7" NVARCHAR(20),
		 "Segment_8" NVARCHAR(20),
		 "Segment_9" NVARCHAR(20),
		 "ActCurr" NVARCHAR(3),
		 "GroupMask" DECIMAL(21,6),
		 "DocType" VARCHAR,
		 "AccntntCod" NVARCHAR(32),
		 "FrgnName" NVARCHAR(100),
		 "MaxLvl" SMALLINT,
		 "Desc1" NVARCHAR(254),
		 "Desc2" NVARCHAR(254),
		 "Desc3" NVARCHAR(254),
		 "Desc4" NVARCHAR(254),
		 "Desc5" NVARCHAR(254),
		 "Desc6" NVARCHAR(254),
		 "Desc7" NVARCHAR(254),
		 "Desc8" NVARCHAR(254),
		 "DescAll" NVARCHAR(254),
		 "Code" NVARCHAR(32)
 	);
	
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
      where "TABLE_NAME" = 'CRSPProfitLostByCCBR_Plan0' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	if :exists1 <> 0 then
		drop table "CRSPProfitLostByCCBR_Plan0" CASCADE;
	end if;
	
	CREATE GLOBAL TEMPORARY TABLE "CRSPProfitLostByCCBR_Plan0" 
	( 
		 "Lvl" SMALLINT,
		 "AcctCode" NVARCHAR(32),
		 "Description" NVARCHAR(254),
		 "Postable" NVARCHAR(1),
		 "Variator" DECIMAL(21,6),
		 "Liq" VARCHAR(1),
		 "Level1" NVARCHAR(5000),
		 "Level2" NVARCHAR(5000),
		 "Level3" NVARCHAR(5000),
		 "Level4" NVARCHAR(5000),
		 "Level5" NVARCHAR(5000),
		 "Level6" NVARCHAR(5000),
		 "Level7" NVARCHAR(5000),
		 "Level8" NVARCHAR(5000),
		 "Level1AccntntCod" NVARCHAR(5000),
		 "Level2AccntntCod" NVARCHAR(5000),
		 "Level3AccntntCod" NVARCHAR(5000),
		 "Level4AccntntCod" NVARCHAR(5000),
		 "Level5AccntntCod" NVARCHAR(5000),
		 "Level6AccntntCod" NVARCHAR(5000),
		 "Level7AccntntCod" NVARCHAR(5000),
		 "Level8AccntntCod" NVARCHAR(5000),
		 "Level1FrgnName" NVARCHAR(5000),
		 "Level2FrgnName" NVARCHAR(5000),
		 "Level3FrgnName" NVARCHAR(5000),
		 "Level4FrgnName" NVARCHAR(5000),
		 "Level5FrgnName" NVARCHAR(5000),
		 "Level6FrgnName" NVARCHAR(5000),
		 "Level7FrgnName" NVARCHAR(5000),
		 "Level8FrgnName" NVARCHAR(5000),
		 "Sort" DECIMAL(21,6),
		 "FatherId" NVARCHAR(32),
		 "Id" NVARCHAR(32),
		 "FormatCode" NVARCHAR(5000),
		 "AcctCodeDisp" NVARCHAR(5000),
		 "Segment_0" NVARCHAR(20),
		 "Segment_1" NVARCHAR(20),
		 "Segment_2" NVARCHAR(20),
		 "Segment_3" NVARCHAR(20),
		 "Segment_4" NVARCHAR(20),
		 "Segment_5" NVARCHAR(20),
		 "Segment_6" NVARCHAR(20),
		 "Segment_7" NVARCHAR(20),
		 "Segment_8" NVARCHAR(20),
		 "Segment_9" NVARCHAR(20),
		 "ActCurr" NVARCHAR(3),
		 "GroupMask" DECIMAL(21,6),
		 "DocType" VARCHAR(1),
		 "AccntntCod" NVARCHAR(32),
		 "FrgnName" NVARCHAR(100) 
	 );
	
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
      where "TABLE_NAME" = 'CRSPProfitLostByCCBR_AcctLang' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	if :exists1 <> 0 then
		drop table "CRSPProfitLostByCCBR_AcctLang" CASCADE;
	end if;
	
	create global temporary table "CRSPProfitLostByCCBR_AcctLang" 
	(    
		"Code" NVARCHAR(32),	 
		"Description" NVARCHAR(254)
	);
	
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
      where "TABLE_NAME" = 'CRSP_TEMPJDT1' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	if :exists1 <> 0 then
		drop table "CRSP_TEMPJDT1" CASCADE;
	end if;
	
	CREATE GLOBAL TEMPORARY TABLE CRSP_TEMPJDT1 ( "TransId" INT CS_INT NOT NULL,
	 "Line_ID" INT CS_INT NOT NULL,
	 "RefDate" LONGDATE CS_LONGDATE,
	 "FinancYear" LONGDATE CS_LONGDATE,
	 "Account" NVARCHAR(15) CS_STRING NOT NULL,
	 "Typ" VARCHAR(6) CS_STRING NOT NULL,
	 "PrcCode" NVARCHAR(8) CS_STRING NOT NULL,
	 "OcrCode" NVARCHAR(8) CS_STRING NOT NULL,
	 "PrjCode" NVARCHAR(20) CS_STRING NOT NULL,
	 "Credit" DECIMAL(21,6),
	 "CreditPreviousYear" DECIMAL(21,6),
	 "Debit" DECIMAL(21,6),
	 "DebitPreviousYear" DECIMAL(21,6),
	 "CreditBudget" DECIMAL(21,6),
	 "DebitBudget" DECIMAL(21,6),
	 "CreditBudgetPrevYear" DECIMAL(21,6),
	 "DebitBudgetPrevYear" DECIMAL(21,6),
	 "CreditActualMonth" DECIMAL(21,6),
	 "CreditMonthPreviousYear" DECIMAL(21,6),
	 "DebitActualMonth" DECIMAL(21,6),
	 "DebitMonthPreviousYear" DECIMAL(21,6),
	 "CreditActualMonthBudget" DECIMAL(21,6),
	 "DebitActualMonthBudget" DECIMAL(21,6),
	 "CreditFinancYear" DECIMAL(21,6),
	 "DebitFinancYear" DECIMAL(21,6),
	 "CreditFinancYearBudget" DECIMAL(21,6),
	 "DebitFinancYearBudget" DECIMAL(21,6),
	 "CreditPrevFinancYear" DECIMAL(21,6),
	 "DebitPrevFinancYear" DECIMAL(21,6),
	 "BFDebit" DECIMAL(21,6),
	 "BFCredit" DECIMAL(21,6),
	 "OBDebit" DECIMAL(21,6),
	 "OBCredit" DECIMAL(21,6),
	 "BFOBDebit" DECIMAL(21,6),
	 "BFOBCredit" DECIMAL(21,6) );
	 
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
    where "TABLE_NAME" = 'CRSP_TEMPRES' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	if :exists1 <> 0 then
		drop table "CRSP_TEMPRES" CASCADE;
	end if;
	
	CREATE GLOBAL TEMPORARY TABLE CRSP_TEMPRES("Typ" nvarchar(6), "TransId" int, "Line_ID" int, "RefDate" timestamp,
	"FinancYear" timestamp,"Account" nvarchar(15), "PrcCode" nvarchar(8), "OcrCode" nvarchar(8),
	"PrjCode" nvarchar(20), "Credit" DECIMAL(21,6), "CreditPreviousYear" DECIMAL(21,6), "Debit" DECIMAL(21,6), "DebitPreviousYear" DECIMAL(21,6),
	"CreditBudget" DECIMAL(21,6), "DebitBudget" DECIMAL(21,6), "CreditBudgetPrevYear" DECIMAL(21,6), "DebitBudgetPrevYear" DECIMAL(21,6),
	"CreditActualMonth" DECIMAL(21,6), 
	"CreditMonthPreviousYear" DECIMAL(21,6),
	"DebitActualMonth" DECIMAL(21,6), 
	"DebitMonthPreviousYear" DECIMAL(21,6),
	"CreditActualMonthBudget" DECIMAL(21,6),
	"DebitActualMonthBudget" DECIMAL(21,6), 
	"CreditFinancYear" DECIMAL(21,6), "DebitFinancYear" DECIMAL(21,6), 
	"CreditFinancYearBudget" DECIMAL(21,6),
	"DebitFinancYearBudget" DECIMAL(21,6), 
	"CreditPrevFinancYear" DECIMAL(21,6), 
	"DebitPrevFinancYear" DECIMAL(21,6), 
	"BFDebit" DECIMAL(21,6),
	"BFCredit" DECIMAL(21,6), 
	"OBDebit" DECIMAL(21,6), 
	"OBCredit" DECIMAL(21,6), 
	"BFOBDebit" DECIMAL(21,6), 
	"BFOBCredit" DECIMAL(21,6));
	
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
    where "TABLE_NAME" = 'CRSP_OCR1DistRule' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	if :exists1 <> 0 then
		drop table "CRSP_OCR1DistRule" CASCADE;
	end if;
	
    CREATE GLOBAL TEMPORARY TABLE "CRSP_OCR1DistRule"("OcrCode" nvarchar(8), "PrcCode" nvarchar(8), "OcrTotal" DECIMAL(21,6), "PrcAmount" DECIMAL(21,6), 
    "OcrFactor" DECIMAL(21,6), "Direct" nvarchar(1), "ValidFrom" timestamp, "ValidTo" timestamp);

	select count(1) into exists1 from "PUBLIC"."M_TABLES"
    where "TABLE_NAME" = 'CRSP_Plan' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	if :exists1 <> 0 then
		drop table "CRSP_Plan" CASCADE;
	end if;
	
 	CREATE GLOBAL TEMPORARY TABLE "CRSP_Plan" ("Lvl" int, "AcctCode" nvarchar(15), "Description" nvarchar(254), "Postable" varchar(1), 
 	"Variator" DECIMAL(21,6), "Liq" varchar(1), "Level1" nvarchar(32), "Level2" nvarchar(32), "Level3" nvarchar(32), 
 	"Level4" nvarchar(32), "Level5" nvarchar(32), "Level6" nvarchar(32), "Level7" nvarchar(32), "Level8" nvarchar(32), 
 	"Level1AccntntCod" nvarchar(32), "Level2AccntntCod" nvarchar(32), "Level3AccntntCod" nvarchar(32), "Level4AccntntCod" nvarchar(32), 
 	"Level5AccntntCod" nvarchar(32), "Level6AccntntCod" nvarchar(32), "Level7AccntntCod" nvarchar(32),  "Level8AccntntCod" nvarchar(32), "Level1FrgnName" nvarchar(100), 
 	"Level2FrgnName" nvarchar(100), "Level3FrgnName" nvarchar(100), "Level4FrgnName" nvarchar(100), "Level5FrgnName" nvarchar(100), 
 	"Level6FrgnName" nvarchar(100), "Level7FrgnName" nvarchar(100), "Level8FrgnName" nvarchar(100), "Sort" int, "FatherId" nvarchar(32), 
 	"Id" nvarchar(32), "FormatCode" nvarchar(32), "AcctCodeDisp" nvarchar(32), "Segment_0" nvarchar(32), "Segment_1" nvarchar(32), 
 	"Segment_2" nvarchar(32), "Segment_3" nvarchar(32), "Segment_4" nvarchar(32), "Segment_5" nvarchar(32), "Segment_6" nvarchar(32),  
 	"Segment_7" nvarchar(32), "Segment_8" nvarchar(32), "Segment_9" nvarchar(32), "ActCurr" varchar(2), "GroupMask" int, "DocType" varchar(1), 
 	"AccntntCod" nvarchar(32), "FrgnName" nvarchar(100), "MaxLvl" int, "Desc1" nvarchar(254), "Desc2" nvarchar(254), "Desc3" nvarchar(254), 
 	"Desc4" nvarchar(254), "Desc5" nvarchar(254), "Desc6" nvarchar(254), "Desc7" nvarchar(254), "Desc8" nvarchar(254), "DescAll" nvarchar(254), "Code" nvarchar(32));
	
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
    where "TABLE_NAME" = 'CRSP_TemplateAccount' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	if :exists1 <> 0 then
		drop table "CRSP_TemplateAccount" CASCADE;
	end if;
	
	 CREATE GLOBAL TEMPORARY TABLE "CRSP_TemplateAccount" ("Code" nvarchar(32), "FatherNum" nvarchar(32), "Description" nvarchar(254), "Lvl" int, "Sort" int, "DocType" varchar(1),
    "Postable" varchar(1), "FormatCode" nvarchar(32), "AcctCodeDisp" nvarchar(32),  "Segment_0" nvarchar(32), "Segment_1" nvarchar(32), "Segment_2" nvarchar(32),
    "Segment_3" nvarchar(32), "Segment_4" nvarchar(32), "Segment_5" nvarchar(32), "Segment_6" nvarchar(32), "Segment_7" nvarchar(32), "Segment_8" nvarchar(32), 
    "Segment_9" nvarchar(32), "ActCurr" varchar(2), "GroupMask" int, "AccntntCod" nvarchar(32), "FrgnName" nvarchar(100));

	select count(1) into exists1 from "PUBLIC"."M_TABLES"
    where "TABLE_NAME" = 'CRSP_Plan0' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	if :exists1 <> 0 then
		drop table "CRSP_Plan0" CASCADE;
	end if;
	
	CREATE GLOBAL TEMPORARY TABLE "CRSP_Plan0" ("Lvl" int, "AcctCode" nvarchar(15), "Description" nvarchar(254), 
	"Postable" varchar(1), "Variator" DECIMAL(21,6), "Liq" varchar(1), 
	"Level1" nvarchar(32), "Level2" nvarchar(32), "Level3" nvarchar(32), "Level4" nvarchar(32), "Level5" nvarchar(32), "Level6" nvarchar(32), "Level7" nvarchar(32), "Level8" nvarchar(32), 
	"Level1AccntntCod" nvarchar(32), "Level2AccntntCod" nvarchar(32),"Level3AccntntCod" nvarchar(32), "Level4AccntntCod" nvarchar(32), "Level5AccntntCod" nvarchar(32),	"Level6AccntntCod" nvarchar(32), "Level7AccntntCod" nvarchar(32), "Level8AccntntCod" nvarchar(32), 
	"Level1FrgnName" nvarchar(100),"Level2FrgnName" nvarchar(100), "Level3FrgnName" nvarchar(100), "Level4FrgnName" nvarchar(100), "Level5FrgnName" nvarchar(100), "Level6FrgnName" nvarchar(100), "Level7FrgnName" nvarchar(100),"Level8FrgnName" nvarchar(100), 
	"Sort" int, "FatherId" nvarchar(32), "Id" nvarchar(32), "FormatCode" nvarchar(32), 
	"AcctCodeDisp" nvarchar(32), 
	"Segment_0" nvarchar(32), "Segment_1" nvarchar(32), "Segment_2" nvarchar(32), "Segment_3" nvarchar(32), "Segment_4" nvarchar(32), "Segment_5" nvarchar(32), "Segment_6" nvarchar(32), "Segment_7" nvarchar(32), "Segment_8" nvarchar(32), "Segment_9" nvarchar(32), 
	"ActCurr" varchar(2), "GroupMask" int, "DocType" varchar(1), "AccntntCod" nvarchar(32), "FrgnName" nvarchar(100)); 
	
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
    where "TABLE_NAME" = 'CRSP_AcctLang' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	if :exists1 <> 0 then
		drop table "CRSP_AcctLang" CASCADE;
	end if;
	
	CREATE GLOBAL TEMPORARY TABLE "CRSP_AcctLang" ("Code" nvarchar(32), "Description" nvarchar(254));
	
	
		select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'CR_SalesBOMSummaryByItem' and "SCHEMA_NAME" = CURRENT_SCHEMA;
		
	if :exists1 <> 0 then
		drop table "CR_SalesBOMSummaryByItem" CASCADE;
	end if;

		
	CREATE GLOBAL TEMPORARY TABLE "CR_SalesBOMSummaryByItem"
	(
		"DocEntry" INT NOT NULL,
		"Joined" NVARCHAR(1), 
		"VisOrder" INT, 
		"TreeType" NVARCHAR(1) DEFAULT 'N',             
		"VendorNum" NVARCHAR(17), 
		"ItemCode" NVARCHAR(50), 
		"Dscription" NVARCHAR(100), 
		"Quantity" DECIMAL(21,6), 
		"unitMsr" NVARCHAR(20), 
		"Price" DECIMAL(21,6), 
		"Discprcnt" DECIMAL(21,6), 
		"PriceBefDi" DECIMAL(21,6),             
		"LineTotal" DECIMAL(21,6), 
		"TotalSumSy" DECIMAL(21,6), 
		"TotalFrgn" DECIMAL(21,6), 
		"Currency" NVARCHAR(3), 
		"WhsCode" NVARCHAR(8));	
		

	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'CR_BeforeSummaryINV1' and "SCHEMA_NAME" = CURRENT_SCHEMA;
		
	if :exists1 <> 0 then
		drop table "CR_BeforeSummaryINV1" CASCADE;
	end if;
 
	
	CREATE GLOBAL TEMPORARY TABLE "CR_BeforeSummaryINV1" (
		"VisOrder" INT CS_INT, 
		"Joined" NVARCHAR(1), 
		"ItemCode" NVARCHAR(50), 
		"SubCatNum" NVARCHAR(20), 
		"Dscription" NVARCHAR(100), 
		"TreeType" NVARCHAR(1) DEFAULT 'N', 
		"UseBaseUn"  NVARCHAR(1) DEFAULT 'N', 
		"Price" DECIMAL(21,6), 
		"DiscPrcnt" DECIMAL(21,6), 
		"Factor1" DECIMAL(21,6), 
		"Factor2" DECIMAL(21,6), 
		"Factor3" DECIMAL(21,6), 
		"Factor4" DECIMAL(21,6), 
		"PriceAfVAT" DECIMAL(21,6), 
		"TaxOnly" NVARCHAR(1), 
		"unitMsr" NVARCHAR(20), 
		"NumPerMsr" DECIMAL(21,6), 
		"WhsCode" NVARCHAR(8), 
		"Quantity" DECIMAL(21,6), 
		"PriceBefDi" DECIMAL(21,6), 
		"LineTotal" DECIMAL(21,6), 
		"TotalSumSy" DECIMAL(21,6), 
		"TotalFrgn" DECIMAL(21,6));  	
		
	select count(*) into exists1 from "PUBLIC".m_tables
	where table_name = 'CRSPTrailBalanceCNAcctLimitted' and schema_name = current_schema;
	
	IF :exists1 <> 0 then
	    exec('DROP TABLE "CRSPTrailBalanceCNAcctLimitted" CASCADE');
	end if;
	
	sqlStr := 'create global temporary table "CRSPTrailBalanceCNAcctLimitted"
		(
			"AcctCode" nvarchar(15),
	 		"AcctName" nvarchar(100),
	 		"AccntntCod" nvarchar(15),
	 		"FrgnName" nvarchar(100),
	 		"Levels" smallint,	 		
	 		"FatherNum" nvarchar(15),
	 		"Postable" nvarchar(1),
	 		"GroupMask" smallint,
	 		"GrpLine" int,
	 		"BalDirect" nvarchar(4),
	 		"ProcessBatch" smallint
		)';
	exec (:sqlStr);	
	
	select count(*) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'SPPP' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	
	IF :exists1 <> 0 THEN
		drop table SPPP CASCADE;
	END IF;
	
	CREATE GLOBAL TEMPORARY TABLE SPPP 
	(
		"PagingID" NVARCHAR(20) NOT NULL,
		"Name" NVARCHAR(254) NOT NULL,
		"Value" NCLOB ST_MEMORY_LOB,
		"Type" SMALLINT
	);
	
	select count(*) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'SPPLA' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	
	IF :exists1 <> 0 THEN
		drop table SPPLA CASCADE;
	END IF;
	
	CREATE GLOBAL TEMPORARY TABLE SPPLA 
	(
		"PagingID" NVARCHAR(20) NOT NULL,
		"RowID" INTEGER CS_INT NOT NULL,
		"LineLevel" INTEGER CS_INT,
		"ChildrenNo" INTEGER CS_INT
	);
END;


