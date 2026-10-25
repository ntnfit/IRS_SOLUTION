-- B1 DEPENDS: AFTER:SP:TmSp_DAB004_AccountList_QRYSTR AFTER:PT:PROCESS_END AFTER:SP:_TmSp_ValidateSpParam
--DROP PROCEDURE TmSp_DAB004_CashFlowRatio;
create procedure TmSp_DAB004_CashFlowRatio (IN IntervalSeq INTEGER, IN LimitedBPsCount INTEGER, IN LimitedSLPsCount INTEGER, IN IntervalVal nvarChar(1), IN VoucherFlag nvarChar(1), IN RecurringFlag nvarChar(1))
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
 /********* Begin Procedure Script ************/ 
 v_Today 		Date;
 v_DateFrom 	Date;
 v_DateTo	  	Date;
 v_BoeEnable    nvarChar(1);
 v_SgmActEnable nvarChar(1);
 v_LocalSet     nvarChar(3);
 v_ApaCheck     nvarChar(1);
 v_QryString    nvarChar(8000);
 v_FmtString    nvarChar(100);
 v_Frequency 	nvarChar(1);
 v_SubFrequncy 	INTEGER;
 v_LimitFlag 	nvarChar(1);
 v_LimitDate 	Date;
 v_Currency 	nVarchar(3);
 v_AcctCode 	nVarchar(15);
 v_NextDue 		Date;
 v_Debit 		DECIMAL(21,6);
 v_Credit 		DECIMAL(21,6);
 v_MainCurrency 	nVarchar(3);
 v_DirectRateFlag 	nvarChar(1);
 v_Rate 		DECIMAL(21,6);
 v_DebitCurrency 	nVarchar(3);
 v_CreditCurrency 	nVarchar(3);
 v_NoDefinedSlpName nvarChar(100);
 v_TempInt     	INTEGER;
 v_FilterInBPFlag 	nvarChar(1);
 v_cDateTo		Date;
 v_tempDate		Date;
 
 v_FrequencyC 	nvarChar(1);
 v_SubFrequncyC 	INTEGER;
 v_LimitFlagC 	nvarChar(1);
 v_LimitDateC 	Date;
 v_CurrencyC 	nVarchar(3);
 v_AcctCodeC 	nVarchar(15);
 v_DebitC 		DECIMAL(21,6);
 v_CreditC 		DECIMAL(21,6);
 
 in_IntervalVal  nvarChar(1);
 in_VoucherFlag nvarChar(1);
 in_RecurringFlag nvarChar(1);
 ----CURSOR DEFINE
 CURSOR c_RecurTransQry (v_cDateTo DATE) FOR
 SELECT T0."Frequency", T0."Remind", T0."LimitRtrns", T0."LimitDate", T1."Currency", T1."AcctCode", T0."NextDeu", T1."Debit", T1."Credit"  FROM "ORCR" T0 
   INNER JOIN "RCR1" T1 ON T1."RcurCode" = T0."RcurCode" AND T0."Instance" = T1."Instance"
   WHERE T0."Frequency" <> 'T'  AND T0."NextDeu" <= :v_cDateTo AND (T0."Frequency" <> 'O' OR T1."Instance" <> 0) AND T1."CtrlAcct" <> T1."AcctCode" AND
    (T0."LimitRtrns" <> 'Y' OR T0."NextDeu" <= T0."LimitDate");
	
 BEGIN
 --For HANA security issue call procedure "_TmSp_ValidateSpParam"
    call _TmSp_ValidateSpParam(:IntervalVal);    
	call _TmSp_ValidateSpParam(:VoucherFlag);    
	call _TmSp_ValidateSpParam(:RecurringFlag);      
	
 -- DROP TEMP TABLE
 select count (*) into v_TempInt from "PUBLIC"."M_TEMPORARY_TABLES" where TABLE_NAME = '#CASHFLOW_TMPOCRD' AND SCHEMA_NAME = CURRENT_SCHEMA and connection_id = current_connection;
 IF :v_TempInt > 0 THEN
 	DROP TABLE #CASHFLOW_TmpOCRD;
 END IF;
 select count (*) into v_TempInt from "PUBLIC"."M_TEMPORARY_TABLES" where TABLE_NAME = '#CASHFLOW_TMPACCT' AND SCHEMA_NAME = CURRENT_SCHEMA and connection_id = current_connection;
 IF :v_TempInt > 0 THEN
 	DROP TABLE #CASHFLOW_TmpAcct;
 END IF;
 select count (*) into v_TempInt from "PUBLIC"."M_TEMPORARY_TABLES" where TABLE_NAME = '#CASHFLOW_TMPRATIO' AND SCHEMA_NAME = CURRENT_SCHEMA and connection_id = current_connection;
 IF :v_TempInt > 0 THEN
 	DROP TABLE #CASHFLOW_TmpRatio;
 END IF;
 select count (*) into v_TempInt from "PUBLIC"."M_TEMPORARY_TABLES" where TABLE_NAME = '#CASHFLOW_TMPRATIOTOSHOW' AND SCHEMA_NAME = CURRENT_SCHEMA and connection_id = current_connection;
 IF :v_TempInt > 0 THEN
 	DROP TABLE #CASHFLOW_TmpRatioToShow;
 END IF;
 
 in_IntervalVal := :IntervalVal;
 in_VoucherFlag := :VoucherFlag;
 in_RecurringFlag := :RecurringFlag;
 
 IF (:IntervalVal = '' or LENGTH(:IntervalVal) = 0) THEN
	in_IntervalVal := 'D';
 END IF;
 
 IF (:VoucherFlag = '' or LENGTH(:VoucherFlag) = 0) THEN
	in_VoucherFlag := 'N';
 END IF;
 
 IF (:RecurringFlag = '' or LENGTH(:RecurringFlag) = 0) THEN
	in_RecurringFlag := 'N';
 END IF;
 
 SELECT "EnableBOE", "EnbSgmnAct", "LawsSet" INTO v_BoeEnable, v_SgmActEnable, v_LocalSet FROM "CINF";
 
 IF (:v_LocalSet = 'CN' OR :v_LocalSet = 'JP') THEN
   v_ApaCheck := 'Y';
 ELSE
   v_ApaCheck := 'N';
 END IF;
 
 --- Filter the inactive BP
 create local temporary COLUMN table #CASHFLOW_TmpOCRD ("CardCode" nvarChar(15), "CardName" nvarChar(100), "CardType" nvarChar(1));
 
 IF (:v_FilterInBPFlag = 'Y') THEN
	INSERT INTO #CASHFLOW_TmpOCRD SELECT "CardCode", "CardName", "CardType" FROM "OCRD";
 ELSE
	INSERT INTO #CASHFLOW_TmpOCRD SELECT "CardCode", "CardName", "CardType" FROM "OCRD" WHERE ("CardCode" IS NULL OR ("validFor" = 'Y' OR ("frozenFor" = 'Y'  AND ("frozenFrom" IS NOT NULL OR "frozenTo" IS NOT NULL  )) OR  ("validFor" = 'N'  AND  "frozenFor" = 'N' )));
 END IF;
 
 --- Period
 v_Today := CURRENT_DATE;
 IF (:in_IntervalVal = 'D') THEN
	v_DateFrom := ADD_DAYS (v_Today, :IntervalSeq - 1);
	v_DateTo := :v_DateFrom;
 ELSEIF (:in_IntervalVal = 'W') THEN
	v_DateFrom := ADD_DAYS (v_Today, (:IntervalSeq - 1) * 7);
	v_DateTo := ADD_DAYS (v_Today, :IntervalSeq * 7);
	v_DateTo := ADD_DAYS (v_DateTo, -1);
 ELSE
	v_DateFrom := ADD_MONTHS (v_Today, :IntervalSeq - 1);
	v_DateTo := ADD_MONTHS (v_Today, :IntervalSeq);
	v_DateTo := ADD_DAYS (v_DateTo, -1);
 END IF;
 
 --- Main Currency (think about different currency)
 SELECT "MainCurncy", "DirectRate" INTO v_MainCurrency, v_DirectRateFlag FROM "OADM";
 
 --- No defined Sales Employee 
 v_NoDefinedSlpName := '';
 select count (*) into v_TempInt FROM "OSLP" WHERE "SlpCode" = '-1';
 IF :v_TempInt > 0 THEN
	SELECT "SlpName" INTO v_NoDefinedSlpName FROM "OSLP" WHERE "SlpCode" = '-1';
 END IF;
 
 ----Get/Set Account List
 CREATE local temporary COLUMN TABLE #CASHFLOW_TmpAcct ("CardCode" nvarChar(15), "CardName" nvarChar(100), "CardType" nvarChar(1));

 CALL TmSp_DAB004_AccountList_QRYSTR (0, v_QryString);
 v_QryString := 'Insert INTO #CASHFLOW_TmpAcct (' || :v_QryString || ')';
 -- SELECT :v_QryString FROM DUMMY;
 exec (:v_QryString);
 
 ----Create temp table
 CREATE local temporary COLUMN TABLE #CASHFLOW_TmpRatio ("CardCode" nvarChar(15), "CardName" nvarChar(100), "SlpCode" nvarChar(15), "SlpName" nvarChar(155), "Direction" nvarChar(1), "Amount" DECIMAL(21,6));
 ----I represent incoming, O represent outgoing
 ----Customer,Vendor 
 v_QryString := 'SELECT T1."CardCode", T1."CardName", (CASE WHEN T2."SlpCode" IS NULL THEN ''-1'' ELSE T2."SlpCode" END) AS "SlpCode", (CASE WHEN T2."SlpCode" IS NULL THEN ''' || :v_NoDefinedSlpName || ''' ELSE T3."SlpName" END) AS "SlpName", (CASE WHEN T0."BalDueDeb" < 0 OR T0."BalDueCred" > 0 THEN ''O'' ELSE ''I'' END) AS "Direction", ABS(T0."BalDueDeb" - T0."BalDueCred") AS "Amount" FROM "JDT1" T0   INNER JOIN #CASHFLOW_TmpOCRD T1 ON  T1."CardCode" = T0."ShortName"  LEFT OUTER JOIN "B1_JournalTransSourceView" T2 ON T2."ObjType" = T0."TransType" AND T2."DocEntry" = T0."CreatedBy" AND (T2."TransType" = ''I'' AND T2."InstlmntID" = T0."SourceLine")    LEFT OUTER JOIN "OSLP" T3 ON T2."SlpCode" = T3."SlpCode"   WHERE (T0."DueDate" BETWEEN ''' || :v_DateFrom || ''' AND ''' || :v_DateTo || ''') AND     (T0."BalDueDeb" <> 0  OR  T0."BalDueCred" <> 0) AND  T0."Account" <> T0."ShortName" AND      T1."CardType" IN (''C'', ''S'')  AND  T0."Account" NOT IN (SELECT "CardCode" FROM #CASHFLOW_TmpAcct WHERE "CardType" = ''E'') AND	 T0."TransType" IN (''30'',''-2'',''69'',''25'',''47'',''-4'',''182'',''203'',''204'',''14'',''13'',''165'',''19'',''18'',''163'',''57'',''76'',''24'',''46'') ';
 
 ----Boe
 IF (:v_BoeEnable = 'Y') THEN
 v_QryString := :v_QryString || '  UNION ALL 
  SELECT T1."CardCode", T1."CardName", (CASE WHEN T2."SlpCode" IS NULL THEN ''-1'' ELSE T2."SlpCode" END) AS "SlpCode", (CASE WHEN T2."SlpCode" IS NULL THEN ''' || :v_NoDefinedSlpName || ''' ELSE T3."SlpName" END) AS "SlpName", (CASE WHEN T0."BalDueDeb" < 0 OR T0."BalDueCred" > 0 THEN ''O'' ELSE ''I'' END) AS "Direction", ABS(T0."BalDueDeb" - T0."BalDueCred") AS "Amount" FROM JDT1 T0    INNER JOIN #CASHFLOW_TmpOCRD T1  ON  T1."CardCode" = T0."ShortName"      LEFT OUTER JOIN "B1_JournalTransSourceView" T2 ON T2."ObjType" = T0."TransType" AND T2."DocEntry" = T0."CreatedBy" AND (T2."TransType" = ''I'' AND T2."InstlmntID" = T0."SourceLine")     LEFT OUTER JOIN "OSLP" T3 ON T2."SlpCode" = T3."SlpCode"    WHERE (T0."DueDate" BETWEEN ''' || :v_DateFrom || ''' AND ''' || :v_DateTo || ''')  AND       (T0."BalDueDeb" <> 0  OR  T0."BalDueCred" <> 0) AND  T0."Account" <> T0."ShortName" AND     T0."Account" IN (SELECT "CardCode" FROM #CASHFLOW_TmpAcct WHERE "CardType" = ''E'') ';
 END IF;
 
 ----Voucher customer,vendor
 IF (:in_VoucherFlag = 'Y') THEN
 v_QryString := :v_QryString || '  UNION ALL   SELECT T2."CardCode", T2."CardName", ''-1'' AS "SlpCode", ''' || :v_NoDefinedSlpName || ''' AS "SlpName", (CASE WHEN T0."Debit" < 0 OR T0."Credit" > 0 THEN ''O'' ELSE ''I'' END) AS "Direction", ABS(T0."Debit" - T0."Credit") AS "Amount"  FROM  "BTF1" T0      INNER  JOIN "OBTF" T1  ON  T0."BatchNum" = T1."BatchNum"  AND  T0."TransId" = T1."TransId"       INNER  JOIN #CASHFLOW_TmpOCRD T2  ON  T2."CardCode" = T0."ShortName"       WHERE T1."BtfStatus" <> ''C''  AND (T0."DueDate" BETWEEN ''' || :v_DateFrom || ''' AND ''' || :v_DateTo || ''') AND       T0."Account" <> T0."ShortName"  AND  T2."CardType" IN (''C '', ''S'') ';
 END IF;
 --SELECT :v_QryString From DUMMY;
 v_QryString := 'INSERT INTO #CASHFLOW_TmpRatio (' || :v_QryString || ')';
 EXEC (:v_QryString);
 
 ----Recurring transaction
 IF (:in_RecurringFlag = 'Y') THEN
 --- Recurring transaction loop 1 
 OPEN c_RecurTransQry (v_DateTo);
 Fetch c_RecurTransQry Into v_FrequencyC, v_SubFrequncyC, v_LimitFlagC, v_LimitDateC, v_CurrencyC, v_AcctCodeC, v_NextDue, v_DebitC, v_CreditC;
 WHILE NOT c_RecurTransQry::NOTFOUND DO
	v_Frequency := :v_FrequencyC;
	v_SubFrequncy := :v_SubFrequncyC;
	v_LimitFlag := :v_LimitFlagC;
	v_LimitDate := :v_LimitDateC;
	v_Currency := :v_CurrencyC;
	v_AcctCode := :v_AcctCodeC;
	v_Debit := :v_DebitC;
	v_Credit := :v_CreditC;
	v_tempDate := :v_NextDue;
	--- Recurring transaction  loop 2
	WHILE (:v_tempDate <= :v_DateTo AND (:v_LimitFlag <> 'Y' OR :v_tempDate <= :v_LimitDate)) DO
		v_Rate := 1;
		IF (:v_Currency <> :v_MainCurrency ) THEN 
			 --- Logic for dashboard: no exchange rate that day then use exchange rate today
			 SELECT count(*) INTO v_TempInt FROM "ORTT" WHERE "Currency" = :v_Currency AND "RateDate" = :v_tempDate;
			 IF (:v_TempInt = 0) THEN
				SELECT count(*) INTO v_TempInt FROM "ORTT" WHERE "Currency" = :v_Currency AND "RateDate" = :v_Today;
				IF (:v_TempInt > 0) THEN
					SELECT  (CASE WHEN :v_DirectRateFlag = 'Y' THEN "Rate" ELSE 1.0/"Rate" END) INTO v_Rate FROM "ORTT" WHERE "Currency" = :v_Currency AND "RateDate" = :v_Today;
				END IF;
			 ELSE
				SELECT (CASE WHEN :v_DirectRateFlag = 'Y' THEN "Rate" ELSE 1.0/"Rate" END) INTO v_Rate FROM "ORTT" WHERE "Currency" = :v_Currency AND "RateDate" = :v_tempDate;
			 END IF;
		END IF;
		IF (:v_tempDate >= :v_DateFrom) THEN
			INSERT INTO #CASHFLOW_TmpRatio SELECT T0."CardCode", T0."CardName", '-1' AS "SlpCode", :v_NoDefinedSlpName AS "SlpName", (CASE WHEN (:v_Debit < 0 OR :v_Credit > 0) THEN 'O' ELSE 'I' END) AS "Direction", ABS((:v_Debit - :v_Credit)*:v_Rate) AS "Amount" FROM #CASHFLOW_TmpOCRD T0 WHERE T0."CardCode" = :v_AcctCode;
		END IF;
		IF (:v_Frequency = 'D') THEN
			v_tempDate := ADD_DAYS (v_tempDate, v_SubFrequncy);
		ELSEIF (:v_Frequency = 'W') THEN
			v_tempDate := ADD_DAYS (v_tempDate, 7);
		ELSEIF (:v_Frequency = 'A') THEN
			v_tempDate := ADD_YEARS (v_tempDate, 1);
		ELSEIF (:v_Frequency = 'M') THEN
			v_tempDate := ADD_MONTHS (v_tempDate, 1);
		ELSEIF (:v_Frequency = 'S') THEN
			v_tempDate := ADD_MONTHS (v_tempDate, 6);
		ELSEIF (:v_Frequency = 'Q') THEN
			v_tempDate := ADD_MONTHS (v_tempDate, 3);
		ELSE
			BREAK;
		END IF;
	END WHILE;
	Fetch c_RecurTransQry Into v_FrequencyC, v_SubFrequncyC, v_LimitFlagC, v_LimitDateC, v_CurrencyC, v_AcctCodeC, v_NextDue, v_DebitC, v_CreditC;
 END WHILE;
 CLOSE c_RecurTransQry;
 END IF;
 
 ----Final Results
 ----Customerr & Sales Employee
 ----Create temp table
 CREATE local temporary COLUMN TABLE #CASHFLOW_TmpRatioToShow ("CardCode" nvarChar(15), "CardName" nvarChar(100), "Direction" nvarChar(1), "Amount" DECIMAL(21,6), "SubType" nvarChar(1));

 v_QryString := ' SELECT T1.* FROM (SELECT TOP ' || :LimitedBPsCount || ' *, ''C'' AS "SubType" FROM ( SELECT "CardCode", "CardName", ''I'' AS "Direction", SUM("Amount") AS "Amount" FROM #CASHFLOW_TmpRatio WHERE "Direction" = ''I'' GROUP BY "CardCode", "CardName") T0                     ORDER BY T0."Amount" DESC) T1                    UNION ALL                    SELECT ''Other'',''Other'', ''I'', SUM("Amount"), ''C'' FROM #CASHFLOW_TmpRatio WHERE "Direction" = ''I'' AND "CardCode" NOT IN (SELECT TOP ' || :LimitedBPsCount || ' T0."CardCode" FROM ( SELECT "CardCode", SUM("Amount") AS "Amount" FROM #CASHFLOW_TmpRatio WHERE "Direction" = ''I'' GROUP BY "CardCode") T0 ORDER BY T0."Amount" DESC)                    UNION ALL                    SELECT T1.* FROM (SELECT TOP ' || :LimitedBPsCount || ' *, ''C'' AS "SubType" FROM ( SELECT "CardCode", "CardName", ''O'' AS "Direction", SUM("Amount") AS "Amount" FROM #CASHFLOW_TmpRatio WHERE "Direction" = ''O'' GROUP BY "CardCode", "CardName") T0                     ORDER BY T0."Amount" DESC) T1                    UNION ALL					SELECT ''Other'',''Other'', ''O'', SUM("Amount"), ''C'' FROM #CASHFLOW_TmpRatio WHERE "Direction" = ''O'' AND "CardCode" NOT IN (SELECT TOP ' || :LimitedBPsCount || ' T0."CardCode" FROM ( SELECT "CardCode", SUM("Amount") AS "Amount" FROM #CASHFLOW_TmpRatio WHERE "Direction" = ''O'' GROUP BY "CardCode") T0 ORDER BY T0."Amount" DESC)                    UNION ALL                    SELECT T1.* FROM (SELECT TOP ' || :LimitedSLPsCount || ' *, ''S'' AS "SubType" FROM ( SELECT "SlpCode", "SlpName", ''I'' AS "Direction", SUM("Amount") AS "Amount" FROM #CASHFLOW_TmpRatio WHERE "Direction" = ''I'' GROUP BY "SlpCode", "SlpName") T0                     ORDER BY T0."Amount" DESC) T1					 UNION ALL                    SELECT ''Other'',''Other'', ''I'', SUM("Amount"), ''S'' FROM #CASHFLOW_TmpRatio WHERE "Direction" = ''I'' AND "SlpCode" NOT IN (SELECT TOP ' || :LimitedSLPsCount || ' T0."SlpCode" FROM ( SELECT "SlpCode", SUM("Amount") AS "Amount" FROM #CASHFLOW_TmpRatio WHERE "Direction" = ''I'' GROUP BY "SlpCode") T0 ORDER BY T0."Amount" DESC)                    UNION ALL                    SELECT T1.* FROM (SELECT TOP ' || :LimitedSLPsCount || ' *, ''S'' AS "SubType" FROM ( SELECT "SlpCode", "SlpName", ''O'' AS "Direction", SUM("Amount") AS "Amount" FROM #CASHFLOW_TmpRatio WHERE "Direction" = ''O'' GROUP BY "SlpCode", "SlpName") T0                     ORDER BY T0."Amount" DESC) T1                     UNION ALL                    SELECT ''Other'',''Other'', ''O'', SUM("Amount"), ''S'' FROM #CASHFLOW_TmpRatio WHERE "Direction" = ''O'' AND "SlpCode" NOT IN (SELECT TOP ' || :LimitedSLPsCount || ' T0."SlpCode" FROM ( SELECT "SlpCode", SUM("Amount") AS "Amount" FROM #CASHFLOW_TmpRatio WHERE "Direction" = ''O'' GROUP BY "SlpCode") T0 ORDER BY t0."Amount" DESC) ';
 v_QryString := 'Insert INTO #CASHFLOW_TmpRatioToShow (' || :v_QryString || ')';
 --SELECT :v_QryString FROM DUMMY;
 EXEC (:v_QryString);
 SELECT * FROM #CASHFLOW_TmpRatioToShow;
 
 -- Drop Temp tables
 --DROP TABLE #CASHFLOW_TmpOCRD;
 --DROP TABLE #CASHFLOW_TmpAcct;
 --DROP TABLE #CASHFLOW_TmpRatio;
 --DROP TABLE #CASHFLOW_TmpRatioToShow;
 END;


