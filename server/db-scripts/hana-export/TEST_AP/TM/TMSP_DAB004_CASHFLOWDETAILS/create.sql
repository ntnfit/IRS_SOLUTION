-- B1 DEPENDS: AFTER:SP:TmSp_DAB004_AccountList_QRYSTR AFTER:PT:PROCESS_END AFTER:SP:_TmSp_ValidateSpParam

CREATE PROCEDURE TmSp_DAB004_CashFlowDetails 
	(IN IntervalSeq INTEGER, 
	IN IncomingPage INTEGER, 
	IN OutgoingPage INTEGER, 
	IN PerPageVal INTEGER, 
	IN Threshold DECIMAL(21,6),
	IN IntervalVal nvarchar(1), 
	IN VoucherFlag nvarchar(1), 
	IN RecurringFlag nvarchar(1)) 
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS 

	/********* Begin Procedure Script ************/
	v_Today DateTime;
	v_DateFrom	  DateTime;
	v_DateTo	  DateTime;
	v_BoeEnable    nvarchar(1);
	v_SgmActEnable nvarchar(1);
	v_LocalSet     nvarChar(3);
	v_ApaCheck     nvarchar(1);
	v_QryString    nvarChar(8000);
	v_FmtString    nvarChar(100);
	v_Frequency nvarchar(1);
	v_SubFrequncy INTEGER;
	v_LimitFlag nvarchar(1);
	v_LimitDate DateTime;
	v_Currency nVarchar(3);
	v_CtrAcct nVarchar(15);
	v_AcctCode nVarchar(15);
	v_NextDue DateTime;
	v_Debit DECIMAL(21,6);
	v_Credit DECIMAL(21,6);
	v_MainCurrency nVarchar(3);
	v_DirectRateFlag nvarchar(1);
	v_Rate DECIMAL(21,6);
	v_DebitCurrency nVarchar(3);
	v_CreditCurrency nVarchar(3);
	v_Instance INTEGER;
	v_RcurCode nvarChar(8);
	v_Ref nvarChar(11);
	v_NegativeThreshold DECIMAL(21,6);
	v_CustomerDeduct nVarchar(15);
	v_VendorDeduct nVarchar(15);
	v_IncomingCount INTEGER;
	v_OutgoingCount INTEGER;
	v_TempInt INTEGER;
	v_FilterInBPFlag nvarchar(1);
	
	v_FrequencyC nvarchar(1);
	v_SubFrequncyC INTEGER;
	v_LimitFlagC nvarchar(1);
	v_LimitDateC DateTime;
	v_CurrencyC nVarchar(3);
	v_RcurCodeC nvarChar(8);
	v_RefC nvarChar(11);
	v_CtrAcctC nVarchar(15);
	v_AcctCodeC nVarchar(15);
	v_InstanceC INTEGER;
	v_NextDueC DateTime;
	v_DebitC DECIMAL(21,6);
	v_CreditC DECIMAL(21,6);
	v_DebitCurrencyC nVarchar(3);
	v_CreditCurrencyC nVarchar(3);
	
	in_IntervalVal  nvarchar(1);
	in_VoucherFlag nvarchar(1);
	in_RecurringFlag nvarchar(1);
	--- CURSOR DEFINE
	CURSOR c_RecurTransQry (DateTo DATE, Threshold DECIMAL(21,6), NegativeThreshold DECIMAL(21,6)) FOR
		SELECT T0."Frequency", T0."Remind", T0."LimitRtrns", T0."LimitDate", T1."Currency", T0."RcurCode"
			, T0."Ref1", T1."CtrlAcct", T1."AcctCode", T1."Instance", T0."NextDeu", T1."Debit", T1."Credit"  
		FROM ORCR T0 
		INNER JOIN RCR1 T1 ON T1."RcurCode" = T0."RcurCode" AND T0."Instance" = T1."Instance"
		WHERE T0."Frequency" <> 'T'  AND T0."NextDeu" <= :DateTo AND 
			(T0."LimitRtrns" <> 'Y' OR T0."NextDeu" <= T0."LimitDate") AND
	    		(T1."Debit" > :Threshold OR T1."Credit" > :Threshold OR T1."Debit" < :NegativeThreshold OR T1."Credit" < :NegativeThreshold);
	
	CURSOR c_ProjectQry1 (DateFrom DATE, DateTo DATE, Threshold DECIMAL(21,6), NegativeThreshold DECIMAL(21,6)) FOR 
		SELECT "DateID", "Debit", "DebCur", "Credit", "CredCur" 
		FROM OCFL 
		WHERE ("SecLevel" >= 1 AND "SecLevel" <= 6) AND ("DateID" BETWEEN :DateFrom AND :DateTo) AND
			("Debit" > :Threshold OR "Credit" > :Threshold OR "Debit" < :NegativeThreshold OR "Credit" < :NegativeThreshold);
			
	CURSOR c_ProjectQry2 (DateFrom DATE, DateTo DATE, Threshold DECIMAL(21,6), NegativeThreshold DECIMAL(21,6)) FOR
		  SELECT "DateID", "Debit", "DebCur", "Credit", "CredCur" 
		  FROM OCFL 
		  WHERE ("SecLevel" >= 1 AND "SecLevel" < 6) AND ("DateID" BETWEEN :DateFrom AND :DateTo) AND
		  	("Debit" > :Threshold OR "Credit" > :Threshold OR "Debit" < :NegativeThreshold OR "Credit" < :NegativeThreshold);
		
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
	select count (*) into v_TempInt from "PUBLIC"."M_TEMPORARY_TABLES" where TABLE_NAME = '#CASHFLOW_TMPDETAIL' AND SCHEMA_NAME = CURRENT_SCHEMA and connection_id = current_connection;
	IF :v_TempInt > 0 THEN
		DROP TABLE #CASHFLOW_TmpDetail;
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
	
	SELECT "EnableBOE", "EnbSgmnAct", "LawsSet" INTO v_BoeEnable,v_SgmActEnable,v_LocalSet FROM CINF;

	IF (:v_LocalSet = 'CN' OR :v_LocalSet = 'JP') THEN
		v_ApaCheck := 'Y';
	ELSE
		v_ApaCheck := 'N';
	END IF;
	
	--- Filter the inactive BP
	CREATE LOCAL TEMPORARY COLUMN TABLE #CASHFLOW_TmpOCRD("CardCode" nvarChar(15), "CardName" nvarChar(100), "CardType" nvarchar(1));
	IF (:v_FilterInBPFlag = 'Y') THEN
		INSERT INTO #CASHFLOW_TmpOCRD SELECT "CardCode", "CardName", "CardType" FROM OCRD;
	ELSE
		INSERT INTO #CASHFLOW_TmpOCRD 
		SELECT "CardCode", "CardName", "CardType" FROM OCRD 
		WHERE ("CardCode" IS NULL OR ("validFor" = 'Y' OR ("frozenFor" = 'Y' AND ("frozenFrom" IS NOT NULL OR "frozenTo" IS NOT NULL)) 
			OR ("validFor" = 'N' AND "frozenFor" = 'N' )));
	END IF;
	
	--- Period
	v_Today := CURRENT_DATE;
	IF (:in_IntervalVal = 'D') THEN
		v_DateFrom := ADD_DAYS(:v_Today, :IntervalSeq - 1);
	 	v_DateTo := :v_DateFrom;
	ELSEIF (:in_IntervalVal = 'W') THEN
		v_DateFrom := ADD_DAYS (:v_Today, 7 * (:IntervalSeq - 1));
		v_DateTo := ADD_DAYS (:v_Today, 7 * :IntervalSeq - 1);
	ELSE
		v_DateFrom := ADD_MONTHS (:v_Today, :IntervalSeq - 1);
		v_DateTo := ADD_DAYS (ADD_MONTHS (:v_Today, :IntervalSeq), -1);
	END IF;
	
	--- Main Currency (think about different currency)
	SELECT "MainCurncy", "DirectRate" INTO v_MainCurrency, v_DirectRateFlag FROM OADM;
	
	----Get/Set Account List
	CREATE LOCAL TEMPORARY COLUMN TABLE #CASHFLOW_TmpAcct ("CardCode" nvarChar(15), "CardName" nvarChar(100), "CardType" nvarchar(1));
	CALL TmSp_DAB004_AccountList_QRYSTR (0, v_QryString);
	v_QryString := 'Insert INTO #CASHFLOW_TmpAcct (' || :v_QryString || ')';
	-- SELECT :v_QryString FROM DUMMY;
	EXEC (:v_QryString);
	
	 ----Get Customer/Vendor deduct account
	v_NegativeThreshold := (-1) * :Threshold;
	v_CustomerDeduct := '';
	v_VendorDeduct := '';
	SELECT COUNT(*) INTO v_TempInt FROM #CASHFLOW_TmpAcct WHERE "CardType" = 'M' AND "CardCode" IS NOT NULL;
	IF (:v_TempInt > 0) THEN
		SELECT "CardCode" INTO v_CustomerDeduct FROM #CASHFLOW_TmpAcct WHERE "CardType" = 'M' AND "CardCode" IS NOT NULL;
	END IF;
	SELECT COUNT(*) INTO v_TempInt FROM #CASHFLOW_TmpAcct WHERE "CardType" = 'U' AND "CardCode" IS NOT NULL;
	IF (:v_TempInt > 0) THEN
		SELECT "CardCode" INTO v_VendorDeduct FROM #CASHFLOW_TmpAcct WHERE "CardType" = 'U' AND "CardCode" IS NOT NULL;
	END IF;

	----Create temp table
	CREATE LOCAL TEMPORARY COLUMN TABLE #CASHFLOW_TmpDetail ("CardCode" nvarChar(15), "CardName" nvarChar(100), "BaseRef" nvarChar(11), 
		"BaseType" nvarChar(20), "DueDate" DateTime, "Direction" nvarchar(1), "Amount" DECIMAL(21,6), "PeyMethod" nvarChar(15), "CreatedBy" nvarChar(20));
	
	 ----I represent incoming, O represent outgoing
	 ----Cash,Check,Credit Card
	 v_QryString := ' SELECT NULL, '''', IFNULL(T0."BaseRef", ''''), T0."TransType", T0."DueDate", (CASE WHEN T0."BalDueDeb" < 0 OR T0."BalDueCred" > 0 THEN ''O'' ELSE ''I'' END), ABS(T0."BalDueDeb" - T0."BalDueCred"), IFNULL(T1."PeyMethod", ''''), T0."CreatedBy" FROM JDT1 T0 		LEFT OUTER JOIN "B1_JournalTransSourceView" T1 ON T1."ObjType" = T0."TransType" AND T1."DocEntry" = T0."CreatedBy" AND (T1."TransType" = ''I'' AND T1."InstlmntID" = T0."SourceLine")  		WHERE (T0."DueDate" BETWEEN ''' || :v_DateFrom || ''' AND ''' || :v_DateTo || ''')  AND 		(T0."BalDueDeb" > ' || :Threshold || ' OR T0."BalDueCred" > ' || :Threshold || ' OR T0."BalDueDeb" < ' || :v_NegativeThreshold || ' OR T0."BalDueCred" < ' || :v_NegativeThreshold || ') AND 		(T0."BalDueDeb" <> 0  OR  T0."BalDueCred" <> 0 ) AND  T0."Account" = T0."ShortName" AND		T0."ShortName" IN (SELECT "CardCode" FROM #CASHFLOW_TmpAcct WHERE "CardType" NOT IN (''E'', ''V'',''M'', ''U'')) AND 		(T0."TransType" IN (''30'',''-2'',''69'',''25'',''47'',''-4'',''182'',''203'',''204'',''14'',''13'',''165'',''19'',''18'',''163'',''57'',''76'') OR 		(T0."TransType" = ''24'' AND T0."ShortName" <> ''' || :v_CustomerDeduct || ''') OR		(T0."TransType" = ''46'' AND T0."ShortName" <> ''' || :v_VendorDeduct || ''') 		)		UNION ALL ';
 	
	----Customer,Vendor 
	v_QryString := :v_QryString || '		SELECT T1."CardCode", T1."CardName", IFNULL(T0."BaseRef", ''''), T0."TransType", T0."DueDate"			, (CASE WHEN T0."BalDueDeb" < 0 OR T0."BalDueCred" > 0 THEN ''O'' ELSE ''I'' END)			, ABS(T0."BalDueDeb" - T0."BalDueCred"), IFNULL(T2."PeyMethod", ''''), T0."CreatedBy"  FROM JDT1 T0 		INNER JOIN #CASHFLOW_TmpOCRD T1 ON  T1."CardCode" = T0."ShortName"  		LEFT OUTER JOIN "B1_JournalTransSourceView" T2 ON T2."ObjType" = T0."TransType" 			AND T2."DocEntry" = T0."CreatedBy" AND (T2."TransType" = ''I'' AND T2."InstlmntID" = T0."SourceLine")  		WHERE (T0."DueDate" BETWEEN ''' || :v_DateFrom || ''' AND ''' || :v_DateTo || ''') AND 			(T0."BalDueDeb" > ' || :Threshold || ' OR T0."BalDueCred" > ' || :Threshold || ' 			OR T0."BalDueDeb" < ' || :v_NegativeThreshold || ' OR T0."BalDueCred" < ' || :v_NegativeThreshold || ') AND 			(T0."BalDueDeb" <> 0  OR  T0."BalDueCred" <> 0) AND  T0."Account" <> T0."ShortName" AND  			T1."CardType" IN (''C'', ''S'')  AND  T0."Account" NOT IN (SELECT "CardCode" FROM #CASHFLOW_TmpAcct WHERE "CardType" = ''E'') AND			T0."TransType" IN (''30'',''-2'',''69'',''25'',''47'',''-4'',''182'',''203'',''204'',''14'',''13'',''165'',''19'',''18'',''163'',''57'',''76'',''24'',''46'')			';
	
	----Boe
	IF (:v_BoeEnable = 'Y') THEN
		v_QryString := :v_QryString || '			UNION ALL 			SELECT T1."CardCode", T1."CardName", IFNULL(T0."BaseRef", ''''), T0."TransType", T0."DueDate"			, (CASE WHEN T0."BalDueDeb" < 0 OR T0."BalDueCred" > 0 THEN ''O'' ELSE ''I'' END)			, ABS(T0."BalDueDeb" - T0."BalDueCred"), IFNULL(T2."PeyMethod", ''''), T0."CreatedBy" 			FROM JDT1 T0 			INNER JOIN #CASHFLOW_TmpOCRD T1  ON  T1."CardCode" = T0."ShortName"    			LEFT OUTER JOIN "B1_JournalTransSourceView" T2 ON T2."ObjType" = T0."TransType" AND T2."DocEntry" = T0."CreatedBy" AND (T2."TransType" = ''I'' AND T2."InstlmntID" = T0."SourceLine")  			WHERE (T0."DueDate" BETWEEN ''' || :v_DateFrom || ''' AND ''' || :v_DateTo || ''') AND 			(T0."BalDueDeb" > ' || :Threshold || ' OR T0."BalDueCred" > ' || :Threshold || ' 			OR T0."BalDueDeb" < ' || :v_NegativeThreshold || ' OR T0."BalDueCred" < ' || :v_NegativeThreshold || ') AND 			(T0."BalDueDeb" <> 0  OR  T0."BalDueCred" <> 0) AND  T0."Account" <> T0."ShortName" AND  			T0."Account" IN (SELECT "CardCode" FROM #CASHFLOW_TmpAcct WHERE "CardType" = ''E'') ';
	END IF;
	
 	----Voucher cash,check,credit card
	IF (:in_VoucherFlag = 'Y') THEN
		v_QryString := :v_QryString || '			UNION ALL			SELECT NULL, '''', TO_NVARCHAR(T0."BatchNum"), ''28'', T0."DueDate", (CASE WHEN T0."Debit" < 0 OR T0."Credit" > 0 THEN ''O'' ELSE ''I'' END), ABS(T0."Debit" - T0."Credit"), '''', T0."CreatedBy" FROM  BTF1 T0, OBTF T1  			WHERE T0."BatchNum" = T1."BatchNum"  AND T0."TransId" = T1."TransId"  AND T0."Account" = T0."ShortName" AND			T1."BtfStatus" <> ''C''  AND (T0."DueDate" BETWEEN ''' || :v_DateFrom || ''' AND ''' || :v_DateTo || ''')  AND 			(T0."Debit" > ' || :Threshold || ' OR T0."Credit" > ' || :Threshold || ' OR T0."Debit" < ' || :v_NegativeThreshold || ' OR T0."Credit" < ' || :v_NegativeThreshold || ') AND 			T0."ShortName" IN (SELECT "CardCode" FROM #CASHFLOW_TmpAcct WHERE "CardType" IN (''A'', ''C'', ''V'',''D''))			UNION ALL ';
				
 		----Voucher customer,vendor		
		v_QryString := :v_QryString || '			SELECT T2."CardCode", T2."CardName", TO_NVARCHAR(T0."BatchNum"), ''28'', T0."DueDate"			, (CASE WHEN T0."Debit" < 0 OR T0."Credit" > 0 THEN ''O'' ELSE ''I'' END)			, ABS(T0."Debit" - T0."Credit"), '''', T0."CreatedBy" FROM  BTF1 T0  			INNER  JOIN OBTF T1  ON  T0."BatchNum" = T1."BatchNum"  AND  T0."TransId" = T1."TransId"   			INNER  JOIN #CASHFLOW_TmpOCRD T2  ON  T2."CardCode" = T0."ShortName"   			WHERE T1."BtfStatus" <> ''C''  AND (T0."DueDate" BETWEEN ''' || :v_DateFrom || ''' AND ''' || :v_DateTo || ''')  AND			(T0."Debit" > ' || :Threshold || ' OR T0."Credit" > ' || :Threshold || ' OR T0."Debit" < ' || :v_NegativeThreshold || ' 			OR T0."Credit" < ' || :v_NegativeThreshold || ') AND 			T0."Account" <> T0."ShortName"  AND  T2."CardType" IN (''C '', ''S'') ';
	END IF;
	 
	--SELECT :v_QryString FROM DUMMY;
	EXEC ('INSERT INTO #CASHFLOW_TmpDetail (' || :v_QryString || ')');
	
	----Recurring transaction
	IF (:in_RecurringFlag = 'Y') THEN
		--- Recurring transaction loop 1  
		OPEN c_RecurTransQry (:v_DateTo, :Threshold, :v_NegativeThreshold);
		FETCH c_RecurTransQry INTO v_FrequencyC,v_SubFrequncyC,v_LimitFlagC,v_LimitDateC,v_CurrencyC,v_RcurCodeC,v_RefC,v_CtrAcctC,v_AcctCodeC,v_InstanceC,v_NextDueC,v_DebitC,v_CreditC;
		WHILE NOT c_RecurTransQry::NOTFOUND DO
			v_Frequency := :v_FrequencyC;
			v_SubFrequncy := :v_SubFrequncyC;
			v_LimitFlag := :v_LimitFlagC;
			v_LimitDate := :v_LimitDateC;
			v_Currency := :v_CurrencyC;
			v_RcurCode := :v_RcurCodeC;
			v_Ref := :v_RefC;
			v_CtrAcct := :v_CtrAcctC;
			v_AcctCode := :v_AcctCodeC;
			v_Instance := :v_InstanceC;
			v_NextDue := :v_NextDueC;
			v_Debit := :v_DebitC;
			v_Credit := :v_CreditC;
			--- Recurring transaction  loop 2
			WHILE (:v_NextDue <= :v_DateTo AND (:v_LimitFlag <> 'Y' OR :v_NextDue <= :v_LimitDate)) DO
				v_Rate := 1;
				IF (:v_Currency <> :v_MainCurrency ) THEN 
					--- Logic for dashboard: no exchange rate that day then use exchange rate today
					SELECT COUNT(0) INTO v_TempInt FROM ORTT WHERE "Currency" = :v_Currency AND "RateDate" = :v_NextDue;
					IF (:v_TempInt = 0) THEN
						SELECT COUNT(*) INTO v_TempInt FROM ORTT WHERE "Currency" = :v_Currency AND "RateDate" = :v_Today;
						IF (:v_TempInt > 0) THEN
							SELECT "Rate" INTO v_Rate FROM ORTT WHERE "Currency" = :v_Currency AND "RateDate" = :v_Today;
							IF (:v_DirectRateFlag <> 'Y') THEN
								v_Rate := 1.0/:v_Rate;
							END IF;
						END IF;
					ELSE
						SELECT "Rate" INTO v_Rate FROM ORTT WHERE "Currency" = :v_Currency AND "RateDate" = :v_NextDue;
						IF (:v_DirectRateFlag <> 'Y') THEN
							v_Rate := 1.0/:v_Rate;
						END IF;
					END IF;
				END IF;
				IF (:v_NextDue >= :v_DateFrom AND (:v_Frequency <>'O' OR :v_Instance <> 0)) THEN					
					IF (:v_AcctCode <> :v_CtrAcct) THEN 
						INSERT INTO #CASHFLOW_TmpDetail 
						SELECT T0."CardCode", T0."CardName"
							, (CASE WHEN :v_Ref IS NULL THEN :v_RcurCode ELSE :v_Ref END), '34'
							, :v_NextDue, (CASE WHEN (:v_Debit < 0 OR :v_Credit > 0) THEN 'O' ELSE 'I' END)
							, ABS((:v_Debit - :v_Credit)*:v_Rate), '', :v_RcurCode 
						FROM #CASHFLOW_TmpOCRD T0
						WHERE T0."CardCode" = :v_AcctCode;
					ELSE			
						IF (:v_BoeEnable = 'Y') THEN						
							INSERT INTO #CASHFLOW_TmpDetail 
							SELECT NULL, '',(CASE WHEN :v_Ref IS NULL THEN :v_RcurCode ELSE :v_Ref END), '34', :v_NextDue
								, (CASE WHEN :v_Debit < 0 OR :v_Credit > 0 THEN 'O' ELSE 'I' END)
								, ABS((:v_Debit-:v_Credit)*:v_Rate), '', :v_RcurCode
							FROM #CASHFLOW_TmpAcct T0 
							WHERE :v_CtrAcct = T0."CardCode" AND T0."CardType" <> 'V';
						ELSE
							INSERT INTO #CASHFLOW_TmpDetail 
							SELECT NULL, '',(CASE WHEN :v_Ref IS NULL THEN :v_RcurCode ELSE :v_Ref END), '34', :v_NextDue
								, (CASE WHEN :v_Debit < 0 OR :v_Credit > 0 THEN 'O' ELSE 'I' END)
								, ABS((:v_Debit-:v_Credit)*:v_Rate), '', :v_RcurCode
							FROM #CASHFLOW_TmpAcct T0 
							WHERE :v_CtrAcct = T0."CardCode" AND T0."CardType" <> 'V' AND T0."CardType" <> 'E';
						END IF;
					END IF;
				END IF;
				IF (:v_Frequency = 'D') THEN
					v_NextDue := ADD_DAYS(:v_NextDue, :v_SubFrequncy);
				ELSEIF (:v_Frequency = 'W') THEN
					v_NextDue := ADD_DAYS(:v_NextDue,7);
				ELSEIF (:v_Frequency = 'A') THEN
					v_NextDue := ADD_YEARS(:v_NextDue,1);
				ELSEIF (:v_Frequency = 'M') THEN
					v_NextDue := ADD_MONTHS(:v_NextDue,1);
				ELSEIF (:v_Frequency = 'S') THEN
					v_NextDue := ADD_MONTHS(:v_NextDue,6);
				ELSEIF (:v_Frequency = 'Q') THEN
					v_NextDue := ADD_MONTHS(:v_NextDue,3);
				ELSE
					BREAK;
				END IF;
			END WHILE;
			FETCH c_RecurTransQry INTO v_FrequencyC,v_SubFrequncyC,v_LimitFlagC,v_LimitDateC,v_CurrencyC,v_RcurCodeC,v_RefC,v_CtrAcctC,v_AcctCodeC,v_InstanceC,v_NextDueC,v_DebitC,v_CreditC;
		END WHILE;	
		CLOSE c_RecurTransQry;
	END IF;

	 --- Addtional project transactions
	 IF (:v_BoeEnable = 'Y') THEN
	 	OPEN c_ProjectQry1 (:v_DateFrom, :v_DateTo, :Threshold, :v_NegativeThreshold);
		FETCH c_ProjectQry1 INTO v_NextDueC,v_DebitC,v_DebitCurrencyC,v_CreditC,v_CreditCurrencyC;
		WHILE (NOT c_ProjectQry1::NOTFOUND) DO
			v_NextDue := v_NextDueC;
			v_Debit := v_DebitC;
			v_DebitCurrency := v_DebitCurrencyC;
			v_Credit := v_CreditC;
			v_CreditCurrency := v_CreditCurrencyC;
			v_Rate := 1.0;
			IF (:v_DebitCurrency <> :v_MainCurrency  OR :v_CreditCurrency <> :v_MainCurrency) THEN
				--- Logic for dashboard: no exchange rate that day then use exchange rate today
				SELECT COUNT(0) INTO v_TempInt FROM ORTT WHERE ("Currency" = :v_DebitCurrency OR "Currency" = :v_CreditCurrency) AND "RateDate" = :v_NextDue;
				IF (:v_TempInt = 0) THEN
					SELECT COUNT(0) INTO v_TempInt FROM ORTT WHERE ("Currency" = :v_DebitCurrency OR "Currency" = :v_CreditCurrency) AND "RateDate" = :v_Today;
					IF (:v_TempInt > 0) THEN
						SELECT "Rate" INTO v_Rate FROM ORTT WHERE ("Currency" = :v_DebitCurrency OR "Currency" = :v_CreditCurrency) AND "RateDate" = :v_Today;
						IF (:v_DirectRateFlag <> 'Y') THEN
							v_Rate := 1.0 / :v_Rate;
						END IF;
					END IF;
				ELSE
					SELECT "Rate" INTO v_Rate FROM ORTT WHERE ("Currency" = :v_DebitCurrency OR "Currency" = :v_CreditCurrency) AND "RateDate" = :v_NextDue;
					IF (:v_DirectRateFlag <> 'Y') THEN
						v_Rate := 1.0 / :v_Rate;
					END IF;
				END IF;
			END IF;
			INSERT INTO #CASHFLOW_TmpDetail VALUES(NULL, '', NULL, '-8', :v_NextDue, (CASE WHEN :v_Debit < 0 OR :v_Debit > 0 THEN 'I' ELSE 'O' END), ABS((:v_Debit - :v_Credit)*:v_Rate), '', NULL);
			FETCH c_ProjectQry1 INTO v_NextDueC,v_DebitC,v_DebitCurrencyC,v_CreditC,v_CreditCurrencyC;
		END WHILE;
	 	CLOSE c_ProjectQry1;
	 ELSE
		OPEN c_ProjectQry2 (:v_DateFrom, :v_DateTo, :Threshold, :v_NegativeThreshold);
		FETCH c_ProjectQry2 INTO v_NextDueC,v_DebitC,v_DebitCurrencyC,v_CreditC,v_CreditCurrencyC;
		WHILE (NOT c_ProjectQry2::NOTFOUND) DO
			v_NextDue := v_NextDueC;
			v_Debit := v_DebitC;
			v_DebitCurrency := v_DebitCurrencyC;
			v_Credit := v_CreditC;
			v_CreditCurrency := v_CreditCurrencyC;
			v_Rate := 1.0;
			IF (:v_DebitCurrency <> :v_MainCurrency  OR :v_CreditCurrency <> :v_MainCurrency) THEN
				--- Logic for dashboard: no exchange rate that day then use exchange rate today
				SELECT COUNT(0) INTO v_TempInt FROM ORTT WHERE ("Currency" = :v_DebitCurrency OR "Currency" = :v_CreditCurrency) AND "RateDate" = :v_NextDue;
				IF (:v_TempInt = 0) THEN
					SELECT COUNT(0) INTO v_TempInt FROM ORTT WHERE ("Currency" = :v_DebitCurrency OR "Currency" = :v_CreditCurrency) AND "RateDate" = :v_Today;
					IF (:v_TempInt > 0) THEN
						SELECT "Rate" INTO v_Rate FROM ORTT WHERE ("Currency" = :v_DebitCurrency OR "Currency" = :v_CreditCurrency) AND "RateDate" = :v_Today;
						IF (:v_DirectRateFlag <> 'Y') THEN
							v_Rate := 1.0 / :v_Rate;
						END IF;
					END IF;
				ELSE
					SELECT "Rate" INTO v_Rate FROM ORTT WHERE ("Currency" = :v_DebitCurrency OR "Currency" = :v_CreditCurrency) AND "RateDate" = :v_NextDue;
					IF (:v_DirectRateFlag <> 'Y') THEN
						v_Rate := 1.0 / :v_Rate;
					END IF;
				END IF;
			END IF;
			INSERT INTO #CASHFLOW_TmpDetail VALUES(NULL, '', NULL, '-8', :v_NextDue, (CASE WHEN :v_Debit < 0 OR :v_Credit > 0 THEN 'I' ELSE 'O' END), ABS((:v_Debit - :v_Credit)*:v_Rate), '', NULL);
			FETCH c_ProjectQry2 INTO v_NextDueC,v_DebitC,v_DebitCurrencyC,v_CreditC,v_CreditCurrencyC;
		END WHILE;
	 	CLOSE c_ProjectQry2;
	 END IF;
	
	----Final Results
	v_IncomingCount := 0;
	v_OutgoingCount := 0;

	SELECT COUNT(*) INTO v_IncomingCount FROM #CASHFLOW_TmpDetail WHERE "Direction" = 'I'; --HAVING COUNT(*) > 0;
	SELECT COUNT(*) INTO v_OutgoingCount FROM #CASHFLOW_TmpDetail WHERE "Direction" = 'O'; --HAVING COUNT(*) > 0;

	v_QryString := 'SELECT CAST(' || :v_IncomingCount || ' AS nvarChar(15)) AS "CardCode", CAST(' || :v_OutgoingCount || ' AS nvarChar(15)) AS "CardName", NULL As "BaseRef", NULL As "BaseType", NULL As "DueDate", NULL As "Direction", NULL AS "Amount", NULL As "PeyMethod", NULL AS "CreatedBy" FROM DUMMY';
	IF (:v_IncomingCount > 0 AND :v_IncomingCount > (:IncomingPage-1)*:PerPageVal) THEN
		v_QryString := :v_QryString || ' UNION ALL 			SELECT * FROM ( (SELECT TOP ' || (:IncomingPage*:PerPageVal) || ' * FROM #CASHFLOW_TmpDetail WHERE "Direction" = ''I'' 			ORDER BY "CardCode", "CardName", "BaseRef", "BaseType", "DueDate", "Direction", "Amount", "PeyMethod", "CreatedBy") ';
		IF (:IncomingPage > 1 AND (:IncomingPage-1)*:PerPageVal > 0) THEN
			v_QryString := :v_QryString || 'EXCEPT 				(SELECT TOP ' || ((:IncomingPage-1)*:PerPageVal) || ' * FROM #CASHFLOW_TmpDetail WHERE "Direction" = ''I'' 				ORDER BY "CardCode", "CardName", "BaseRef", "BaseType", "DueDate", "Direction", "Amount", "PeyMethod", "CreatedBy") ';
		END IF;
		v_QryString := :v_QryString || ') AS T1 ';
	END IF;
	IF (:v_OutgoingCount > 0 AND :v_OutgoingCount > (:OutgoingPage-1)*:PerPageVal) THEN
		v_QryString := :v_QryString || ' UNION ALL			SELECT * FROM ( (SELECT TOP ' || (:OutgoingPage*:PerPageVal) || ' * FROM #CASHFLOW_TmpDetail WHERE "Direction" = ''O'' 			ORDER BY "CardCode", "CardName", "BaseRef", "BaseType", "DueDate", "Direction", "Amount", "PeyMethod", "CreatedBy") ';
		IF (:OutgoingPage > 1 AND (:OutgoingPage-1)*:PerPageVal > 0) THEN
			v_QryString := :v_QryString || 'EXCEPT 				(SELECT TOP ' || ((:OutgoingPage-1)*:PerPageVal) || ' * FROM #CASHFLOW_TmpDetail WHERE "Direction" = ''O'' 				ORDER BY "CardCode", "CardName", "BaseRef", "BaseType", "DueDate", "Direction", "Amount", "PeyMethod", "CreatedBy") ';
		END IF;
		v_QryString := :v_QryString || ') AS T2 ';
	END IF;	

	EXECUTE IMMEDIATE (:v_QryString);
	
	-- Drop Temp tables
	--DROP TABLE #CASHFLOW_TmpOCRD;
	--DROP TABLE #CASHFLOW_TmpAcct;
	--DROP TABLE #CASHFLOW_TmpDetail;
	--DROP TABLE #CASHFLOW_TmpViewToShow;

END


