-- B1 DEPENDS: AFTER:SP:TmSp_DAB004_AccountList_QRYSTR AFTER:PT:PROCESS_END AFTER:SP:_TmSp_ValidateSpParam
--DROP PROCEDURE TmSp_DAB004_CashFlowNamedDetails;
CREATE PROCEDURE TmSp_DAB004_CashFlowNamedDetails 
	(IN IntervalSeq INTEGER, 
	IN Direction nvarChar(1), 
	IN Page INTEGER, 
	IN FilterStr nvarChar(200), 
	IN PerPageVal INTEGER, 
	IN CardSlpFlag nvarChar(1), 
	IN IntervalVal nvarChar(1), 
	IN VoucherFlag nvarChar(1), 
	IN RecurringFlag nvarChar(1)) 
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS 

	/********* Begin Procedure Script ************/
	v_Today DateTime;
	v_DateFrom DateTime;
	v_DateTo	  DateTime;
	v_BoeEnable    nvarChar(1);
	v_SgmActEnable nvarChar(1);
	v_LocalSet     nvarChar(3);
	v_ApaCheck     nvarChar(1);
	v_QryString    nvarChar(8000);
	v_FmtString    nvarChar(100);
	v_Frequency nvarChar(1);
	v_SubFrequncy INTEGER;
	v_LimitFlag nvarChar(1);
	v_LimitDate DateTime;
	v_Currency nVarchar(3);
	v_AcctCode nVarchar(15);
	v_NextDue DateTime;
	v_Debit DECIMAL(21,6);
	v_Credit DECIMAL(21,6);
	v_MainCurrency nVarchar(3);
	v_DirectRateFlag nvarChar(1);
	v_Rate DECIMAL(21,6);
	v_DebitCurrency nVarchar(3);
	v_CreditCurrency nVarchar(3);
	v_RcurCode nvarChar(8);
	v_Ref nvarChar(11);
	v_TotalCount INTEGER;
	v_TempInt INTEGER;
	v_NoDefinedSlpName nvarChar(100);
	v_Multiplier INTEGER;
	v_NoDefinedSLPFlag nvarChar(1);
	v_FilterInBPFlag nvarChar(1);
	
	v_FrequencyC nvarChar(1);
	v_SubFrequncyC INTEGER;
	v_LimitFlagC nvarChar(1);
	v_LimitDateC DateTime;
	v_CurrencyC nVarchar(3);
	v_RcurCodeC nvarChar(8);
	v_RefC nvarChar(11);
	v_NextDueC DateTime;
	v_DebitC DECIMAL(21,6);
	v_CreditC DECIMAL(21,6);
	
	in_IntervalVal  nvarChar(1);
	in_VoucherFlag nvarChar(1);
	in_RecurringFlag nvarChar(1);
	--- CURSOR DEFINE
	CURSOR c_RecurTransQry (DateTo DATE, Multiplier INTEGER) FOR
	SELECT T0."Frequency", T0."Remind", T0."LimitRtrns", T0."LimitDate", T1."Currency", T0."RcurCode"
		, T0."Ref1", T0."NextDeu", T1."Debit", T1."Credit"  
	FROM ORCR T0 
	INNER JOIN RCR1 T1 ON T1."RcurCode" = T0."RcurCode" AND T0."Instance" = T1."Instance"
	WHERE T0."Frequency" <> 'T'  AND T0."NextDeu" <= :DateTo AND 
		(T0."LimitRtrns" <> 'Y' OR T0."NextDeu" <= T0."LimitDate") AND
		(T0."Frequency" <> 'O' OR T1."Instance" <> 0) AND T1."CtrlAcct" <> T1."AcctCode" AND
		((T1."Debit")*:Multiplier < 0 OR (T1."Credit")*:Multiplier > 0);
		
BEGIN
	--For HANA security issue call procedure "_TmSp_ValidateSpParam"
    call _TmSp_ValidateSpParam(:Direction);    
    call _TmSp_ValidateSpParam(:FilterStr);    
	call _TmSp_ValidateSpParam(:CardSlpFlag);   
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
	select count (*) into v_TempInt from "PUBLIC"."M_TEMPORARY_TABLES" where TABLE_NAME = '#CASHFLOW_TMPNAMEDDETAIL' AND SCHEMA_NAME = CURRENT_SCHEMA and connection_id = current_connection;
	IF :v_TempInt > 0 THEN
		DROP TABLE #CASHFLOW_TmpNamedDetail;
	END IF;
	select count (*) into v_TempInt from "PUBLIC"."M_TEMPORARY_TABLES" where TABLE_NAME = '#CASHFLOW_TMPVIEWTOSHOW' AND SCHEMA_NAME = CURRENT_SCHEMA and connection_id = current_connection;
	IF :v_TempInt > 0 THEN
		DROP TABLE #CASHFLOW_TmpViewToShow;
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
	CREATE LOCAL TEMPORARY COLUMN TABLE #CASHFLOW_TmpOCRD("CardCode" nvarChar(15), "CardName" nvarChar(100), "CardType" nvarChar(1));
	
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
	
	--- Initial Multiplier
	v_Multiplier := 1;
	IF (:Direction = 'I') THEN
		v_Multiplier := -1;
	END IF;
	
	--- No defined Sales Employee 
	v_NoDefinedSlpName := '';
	v_NoDefinedSLPFlag := 'N';
	SELECT COUNT(*) INTO v_TempInt FROM OSLP WHERE "SlpCode" = '-1';
	IF (:v_TempInt > 0) THEN
		SELECT "SlpName" INTO v_NoDefinedSlpName FROM OSLP WHERE "SlpCode" = '-1';	
	END IF;
	IF (LEFT(:FilterStr,2) = N'IN' AND LOCATE(:FilterStr, '-1') > 0) THEN
			v_NoDefinedSLPFlag := 'Y';
	END IF;
	
	--- Main Currency (think about different currency)
	SELECT "MainCurncy", "DirectRate" INTO v_MainCurrency, v_DirectRateFlag FROM OADM;
	
	----Get/Set Account List
	CREATE LOCAL TEMPORARY COLUMN TABLE #CASHFLOW_TmpAcct ("CardCode" nvarChar(15), "CardName" nvarChar(100), "CardType" nvarChar(1));

	CALL TmSp_DAB004_AccountList_QRYSTR (0, v_QryString);
	v_QryString := 'Insert INTO #CASHFLOW_TmpAcct (' || :v_QryString || ')';
	-- SELECT :v_QryString FROM DUMMY;
	EXEC (:v_QryString);
	
	----Create temp table
	CREATE LOCAL TEMPORARY COLUMN TABLE #CASHFLOW_TmpNamedDetail ("CardCode" nvarChar(15), "CardName" nvarChar(155), "BaseRef" nvarChar(11), 
		"BaseType" nvarChar(20), "DueDate" DateTime, "Amount" DECIMAL(21,6), "CreatedBy" nvarChar(11));
	
	----Customer,Vendor 
	IF (:CardSlpFlag = 'C') THEN
		v_QryString := 'SELECT T1."CardCode", T1."CardName", T0."BaseRef", T0."TransType", T0."DueDate", ABS(T0."BalDueDeb" - T0."BalDueCred"), T0."CreatedBy" FROM JDT1 T0 			INNER JOIN #CASHFLOW_TmpOCRD T1 ON  T1."CardCode" = T0."ShortName"  			WHERE (T0."DueDate" BETWEEN ''' || :v_DateFrom || ''' AND ''' || :v_DateTo || ''') AND 			((T0."BalDueDeb")*' || :v_Multiplier || ' < 0  			OR (T0."BalDueCred")*' || :v_Multiplier || ' > 0) AND T0."Account" <> T0."ShortName" AND  			T1."CardType" IN (''C'', ''S'')  AND  T0."Account" NOT IN (SELECT "CardCode" FROM #CASHFLOW_TmpAcct WHERE "CardType" = ''E'') AND			T0."TransType" IN (''30'',''-2'',''69'',''25'',''47'',''-4'',''182'',''203'',''204'',''14'',''13'',''165'',''19'',''18'',''163'',''57'',''76'',''24'',''46'') AND T1."CardCode" ' || :FilterStr || ' ';
	ELSE
		v_QryString := 'SELECT CAST(T2."SlpCode" AS nvarChar(15)), T3."SlpName", T0."BaseRef", T0."TransType", T0."DueDate", ABS(T0."BalDueDeb" - T0."BalDueCred"), T0."CreatedBy" FROM JDT1 T0 			INNER JOIN #CASHFLOW_TmpOCRD T1 ON T1."CardCode" = T0."ShortName"			LEFT OUTER JOIN "B1_JournalTransSourceView" T2 ON T2."ObjType" = T0."TransType" AND T2."DocEntry" = T0."CreatedBy" AND (T2."TransType" = ''I'' AND T2."InstlmntID" = T0."SourceLine")  			LEFT OUTER JOIN OSLP T3 ON T2."SlpCode" = T3."SlpCode" 			WHERE (T0."DueDate" BETWEEN ''' || :v_DateFrom || ''' AND ''' || :v_DateTo || ''') AND 			 ((T0."BalDueDeb")*' || :v_Multiplier || ' < 0 OR (T0."BalDueCred")*' || :v_Multiplier || ' > 0) AND  T0."Account" <> T0."ShortName" AND  			 T1."CardType" IN (''C'', ''S'')  AND  T0."Account" NOT IN (SELECT "CardCode" FROM #CASHFLOW_TmpAcct WHERE "CardType" = ''E'') AND			 T0."TransType" IN (''30'',''-2'',''69'',''25'',''47'',''-4'',''182'',''203'',''204'',''14'',''13'',''165'',''19'',''18'',''163'',''57'',''76'',''24'',''46'') AND T2."SlpCode" ' || :FilterStr || ' ';
	END IF;
	
	----Boe
	IF (:v_BoeEnable = 'Y') THEN
		IF (:CardSlpFlag = 'C') THEN
			v_QryString := :v_QryString || ' UNION ALL 				SELECT T1."CardCode", T1."CardName", T0."BaseRef", T0."TransType", T0."DueDate", ABS(T0."BalDueDeb" - T0."BalDueCred"), T0."CreatedBy" FROM JDT1 T0 				INNER JOIN #CASHFLOW_TmpOCRD T1  ON  T1."CardCode" = T0."ShortName"   				WHERE (T0."DueDate" BETWEEN ''' || :v_DateFrom || ''' AND ''' || :v_DateTo || ''')  AND  				  ((T0."BalDueDeb")*' || :v_Multiplier || ' < 0 OR (T0."BalDueCred")*' || :v_Multiplier || ' > 0) AND  T0."Account" <> T0."ShortName" AND				  T0."Account" IN (SELECT "CardCode" FROM #CASHFLOW_TmpAcct WHERE "CardType" = ''E'') AND T1."CardCode" ' || :FilterStr || ' ';
		ELSE
			v_QryString := :v_QryString || ' UNION ALL 				SELECT CAST(T2."SlpCode" AS nvarChar(15)), T3."SlpName", T0."BaseRef", T0."TransType", T0."DueDate", ABS(T0."BalDueDeb" - T0."BalDueCred") , T0."CreatedBy" FROM JDT1 T0 				INNER JOIN #CASHFLOW_TmpOCRD T1  ON  T1."CardCode" = T0."ShortName"   				LEFT OUTER JOIN "B1_JournalTransSourceView" T2 ON T2."ObjType" = T0."TransType" AND T2."DocEntry" = T0."CreatedBy" AND (T2."TransType" = ''I'' AND T2."InstlmntID" = T0."SourceLine")  				LEFT OUTER JOIN OSLP T3 ON T2."SlpCode" = T3."SlpCode" 				WHERE (T0."DueDate" BETWEEN ''' || :v_DateFrom || ''' AND ''' || :v_DateTo || ''')  AND  				  ((T0."BalDueDeb")*' || :v_Multiplier || ' < 0 OR (T0."BalDueCred")*' || :v_Multiplier || ' > 0) AND  T0."Account" <> T0."ShortName" AND				  T0."Account" IN (SELECT "CardCode" FROM #CASHFLOW_TmpAcct WHERE "CardType" = ''E'') AND T2."SlpCode" ' || :FilterStr || ' ';
		END IF;
	END IF;
	
	----Voucher customer,vendor
	IF (:in_VoucherFlag = 'Y') THEN
		IF (:CardSlpFlag = 'C') THEN
			v_QryString := :v_QryString || ' UNION ALL 				SELECT T2."CardCode", T2."CardName", T0."BatchNum", ''28'', T0."DueDate", ABS(T0."Debit" - T0."Credit"), IFNULL(T0."CreatedBy",T0."BatchNum")  FROM  BTF1 T0  				INNER  JOIN OBTF T1  ON  T0."BatchNum" = T1."BatchNum"  AND  T0."TransId" = T1."TransId"   				INNER  JOIN #CASHFLOW_TmpOCRD T2  ON  T2."CardCode" = T0."ShortName"   				WHERE T1."BtfStatus" <> ''C''  AND (T0."DueDate" BETWEEN ''' || :v_DateFrom || ''' AND ''' || :v_DateTo || ''')  AND 				  ((T0."Debit")*' || :v_Multiplier || ' < 0 OR (T0."Credit")*' || :v_Multiplier || ' > 0) AND				  T0."Account" <> T0."ShortName"  AND  T2."CardType" IN (''C'', ''S'') AND T2."CardCode" ' || :FilterStr || ' ';
		ELSE
			IF (:v_NoDefinedSLPFlag = 'Y') THEN
				v_QryString := :v_QryString || ' UNION ALL					SELECT ''-1'', ''' || :v_NoDefinedSlpName || ''', T0."BatchNum", ''28'', T0."DueDate", ABS(T0."Debit" - T0."Credit"),  IFNULL(T0."CreatedBy",T0."BatchNum")   FROM  BTF1 T0  					 INNER  JOIN OBTF T1  ON  T0."BatchNum" = T1."BatchNum"  AND  T0."TransId" = T1."TransId"   					 INNER  JOIN #CASHFLOW_TmpOCRD T2  ON  T2."CardCode" = T0."ShortName"   					 WHERE T1."BtfStatus" <> ''C''  AND (T0."DueDate" BETWEEN ''' || :v_DateFrom || ''' AND ''' || :v_DateTo || ''')  AND					   ((T0."Debit")*' || :v_Multiplier || ' < 0 OR (T0."Credit")*' || :v_Multiplier || ' > 0) AND					   T0."Account" <> T0."ShortName"  AND  T2."CardType" IN (''C '', ''S'') ';
			END IF;
		END IF;
	END IF;
	 
	--SELECT :v_QryString FROM DUMMY;
	EXEC ('INSERT INTO #CASHFLOW_TmpNamedDetail (' || :v_QryString || ')');
	
	----Recurring transaction
	IF (:in_RecurringFlag = 'Y') THEN
		--- Recurring transaction loop 1  
		OPEN c_RecurTransQry (:v_DateTo, :v_Multiplier);
		FETCH c_RecurTransQry INTO v_FrequencyC,v_SubFrequncyC,v_LimitFlagC,v_LimitDateC,v_CurrencyC,v_RcurCodeC,v_RefC,v_NextDueC,v_DebitC,v_CreditC;
		WHILE NOT c_RecurTransQry::NOTFOUND DO
			v_Frequency := :v_FrequencyC;
			v_SubFrequncy := :v_SubFrequncyC;
			v_LimitFlag := :v_LimitFlagC;
			v_LimitDate := :v_LimitDateC;
			v_Currency := :v_CurrencyC;
			v_RcurCode := :v_RcurCodeC;
			v_Ref := :v_RefC;
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
						SELECT COUNT(0) INTO v_TempInt FROM ORTT WHERE "Currency" = :v_Currency AND "RateDate" = :v_Today;
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
				IF (:v_NextDue >= :v_DateFrom) THEN					
					IF (:CardSlpFlag = 'C') THEN 
						v_QryString := 'INSERT INTO #CASHFLOW_TmpNamedDetail SELECT T0."CardCode", T0."CardName", (CASE WHEN '
							|| '''' || :v_Ref || '''' || ' IS NULL THEN ' || '''' || :v_RcurCode || '''' || ' ELSE NULL END), ''34'', '
							|| '''' || :v_NextDue || '''' || ', ABS((' || :v_Debit || '- ' || :v_Credit || ')*' || :v_Rate
							|| '), T0."CardCode" FROM #CASHFLOW_TmpOCRD T0 WHERE T0."CardCode" = ''' || :v_AcctCode || ''' AND T0."CardCode" ' || :FilterStr || ' ';
						IF (:v_QryString IS NOT NULL) THEN
							EXEC (:v_QryString);
						END IF;
					ELSE			
						IF (:v_NoDefinedSLPFlag = 'Y') THEN						
		     					INSERT INTO #CASHFLOW_TmpNamedDetail 
		     					SELECT '-1', :v_NoDefinedSlpName,(CASE WHEN :v_Ref IS NULL THEN :v_RcurCode ELSE NULL END), '34', :v_NextDue, ABS((:v_Debit-:v_Credit)*:v_Rate) , '-1'
		     					FROM #CASHFLOW_TmpOCRD T0 
		     					WHERE T0."CardCode" = :v_AcctCode;
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
			FETCH c_RecurTransQry INTO v_FrequencyC,v_SubFrequncyC,v_LimitFlagC,v_LimitDateC,v_CurrencyC,v_RcurCodeC,v_RefC,v_NextDueC,v_DebitC,v_CreditC;
		END WHILE;	
		CLOSE c_RecurTransQry;
	END IF;
	
	----Final Results
	v_TotalCount := 0;
	
	SELECT COUNT(1) INTO v_TotalCount FROM #CASHFLOW_TmpNamedDetail;
	----First Line is special line including total records& result type
	v_QryString := 'SELECT ''D'', NULL, CAST(' || :v_TotalCount || ' AS nvarChar(15)), NULL, NULL, NULL, NULL FROM DUMMY';
	IF (:v_TotalCount > 0) AND (:v_TotalCount > (:Page-1)*:PerPageVal) AND (:Page*:PerPageVal) >= 0 THEN
		v_QryString := :v_QryString || ' UNION ALL 			SELECT * FROM ( (SELECT TOP ' || (:Page*:PerPageVal)  || '* FROM #CASHFLOW_TmpNamedDetail 			ORDER BY "CardCode", "CardName", "BaseRef", "BaseType", "DueDate", "Amount") ';
		IF (:Page > 1 AND (:Page-1)*:PerPageVal > 0) THEN
			 v_QryString := :v_QryString || ' EXCEPT 				(SELECT TOP ' || ((:Page-1)*:PerPageVal)  || '* FROM #CASHFLOW_TmpNamedDetail 				ORDER BY "CardCode", "CardName", "BaseRef", "BaseType", "DueDate", "Amount") ';
		END IF;
		v_QryString := :v_QryString || ') AS T1 ';
	END IF;
	
	---- SELECT :v_QryString FROM DUMMY;
	CREATE LOCAL TEMPORARY COLUMN TABLE #CASHFLOW_TmpViewToShow ("CardCode" nvarChar(15), "CardName" nvarChar(155), "BaseRef" nvarChar(11), 
		"BaseType" nvarChar(20), "DueDate" DateTime, "Amount" DECIMAL(21,6), "CreatedBy" nvarChar(11));
	v_QryString := 'INSERT INTO #CASHFLOW_TmpViewToShow (' || :v_QryString || ')';
	EXEC (:v_QryString);
	SELECT * FROM #CASHFLOW_TmpViewToShow;
	
	-- Drop Temp tables
	--DROP TABLE #CASHFLOW_TmpOCRD;
	--DROP TABLE #CASHFLOW_TmpAcct;
	--DROP TABLE #CASHFLOW_TmpNamedDetail;
	--DROP TABLE #CASHFLOW_TmpViewToShow;

END


