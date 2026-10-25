-- B1 DEPENDS: AFTER:SP:TmSp_DAB004_AccountList_QRYSTR AFTER:PT:PROCESS_END AFTER:SP:_TmSp_ValidateSpParam

Create procedure TmSp_DAB004_CashFlowOverView (IN IntervalCnt INTEGER, IN IntervalVal nvarChar(1)) 
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
 /********* Begin Procedure Script ************/ 
 v_BoeEnable    nvarChar(1);
 v_SgmActEnable nvarChar(1);
 v_LocalSet     nvarChar(3);
 v_ApaCheck     nvarChar(1);
 v_QryString    nvarChar(8000);
 v_FmtString    nvarChar(100);
 v_DateFrom     Date;
 v_DateTo       Date;
 v_DateIter     Date;
 v_Iter         INTEGER;
 v_Frequency 	nvarChar(1);
 v_SubFrequncy 	INTEGER;
 v_LimitFlag 	nvarChar(1);
 v_LimitDate	Date;
 v_Currency 	nVarchar(3);
 v_CtrAcct 		nVarchar(15);
 v_AcctCode 	nVarchar(15);
 v_NextDue 		Date;
 v_Debit 		DECIMAL(21,6);
 v_Credit 		DECIMAL(21,6);
 v_MainCurrency nVarchar(3);
 v_DirectRateFlag 	nvarChar(1);
 v_Rate 		DECIMAL(21,6);
 v_DebitCurrency 	nVarchar(3);
 v_CreditCurrency 	nVarchar(3);
 v_MainInstance 	INTEGER;
 v_SubInstance 	INTEGER;
 v_FirstEndDate Date;
 v_CustomerDeduct 	nVarchar(15);
 v_VendorDeduct 	nVarchar(15);
 v_ErrInt       INTEGER;
 v_TempInt      INTEGER;
 v_FilterInBPFlag 	nvarChar(1);
 
 v_FrequencyC 	nvarChar(1);
 v_SubFrequncyC INTEGER;
 v_LimitFlagC 	nvarChar(1);
 v_LimitDateC	Date;
 v_CurrencyC 	nVarchar(3);
 v_CtrAcctC 	nVarchar(15);
 v_AcctCodeC 	nVarchar(15);
 v_MainInstanceC 	INTEGER;
 v_SubInstanceC 	INTEGER;
 v_NextDueC 		Date;
 v_DebitC 		DECIMAL(21,6);
 v_CreditC 		DECIMAL(21,6);
 v_DebitCurrencyC 	nVarchar(3);
 v_CreditCurrencyC 	nVarchar(3);
 
 v_cDateTo		Date;
 v_IsBreakCur	INTEGER;
 v_curId		INTEGER;
 ----CURSOR DEFINE
 CURSOR c_RecurTransQry (v_cDateTo DATE) FOR
 SELECT T0."Frequency", T0."Remind", T0."LimitRtrns", T0."LimitDate", T1."Currency", T1."CtrlAcct", T1."AcctCode", T0."Instance" AS "InstanceT0", T1."Instance" AS "InstanceT1", T0."NextDeu", T1."Debit", T1."Credit"  FROM "ORCR" T0 
  INNER JOIN "RCR1" T1 ON T1."RcurCode" = T0."RcurCode" --AND T0."Instance" = T1."Instance"
  WHERE T0."Frequency" <> 'T'  AND T0."NextDeu" <= :v_cDateTo AND 
   (T0."LimitRtrns" <> 'Y' OR T0."NextDeu" <= T0."LimitDate");
   
 CURSOR c_ProjectQry1 (v_cDateTo DATE) FOR
 SELECT "DateID", "Debit", "DebCur", "Credit", "CredCur" FROM "OCFL" 
    WHERE ("SecLevel" >= 1 AND "SecLevel" <= 6) AND "DateID" <= :v_cDateTo;
	
 CURSOR c_ProjectQry2 (v_cDateTo DATE) FOR
 SELECT "DateID", "Debit", "DebCur", "Credit", "CredCur" FROM "OCFL" 
    WHERE ("SecLevel" >= 1 AND "SecLevel" < 6) AND "DateID" <= :v_cDateTo;
	
 BEGIN 
--For HANA security issue call procedure "_TmSp_ValidateSpParam"
    call _TmSp_ValidateSpParam(:IntervalVal);    
	
 -- DROP TEMP TABLE
 select count (*) into v_TempInt from "PUBLIC"."M_TEMPORARY_TABLES" where TABLE_NAME = '#CASHFLOW_TMPDATE' AND SCHEMA_NAME = CURRENT_SCHEMA and connection_id = current_connection;
 IF :v_TempInt > 0 THEN
 	DROP TABLE #CASHFLOW_TmpDate;
 END IF;
 select count (*) into v_TempInt from "PUBLIC"."M_TEMPORARY_TABLES" where TABLE_NAME = '#CASHFLOW_TMPOCRD' AND SCHEMA_NAME = CURRENT_SCHEMA and connection_id = current_connection;
 IF :v_TempInt > 0 THEN
 	DROP TABLE #CASHFLOW_TmpOCRD;
 END IF;
 select count (*) into v_TempInt from "PUBLIC"."M_TEMPORARY_TABLES" where TABLE_NAME = '#CASHFLOW_TMPACCT' AND SCHEMA_NAME = CURRENT_SCHEMA and connection_id = current_connection;
 IF :v_TempInt > 0 THEN
 	DROP TABLE #CASHFLOW_TmpAcct;
 END IF;
 select count (*) into v_TempInt from "PUBLIC"."M_TEMPORARY_TABLES" where TABLE_NAME = '#CASHFLOW_TMPOVERVIEW' AND SCHEMA_NAME = CURRENT_SCHEMA and connection_id = current_connection;
 IF :v_TempInt > 0 THEN
 	DROP TABLE #CASHFLOW_TmpOverView;
 END IF;
 
 ----Get all dates
 v_DateFrom := CURRENT_DATE;
 v_DateIter := :v_DateFrom;
 v_Iter := 1;
 create local temporary COLUMN table #CASHFLOW_TmpDate("StartDate" Date, "EndDate" Date);
 WHILE :v_Iter <= :IntervalCnt DO
	IF (:IntervalVal = 'D') THEN
		exec ('INSERT INTO #CASHFLOW_TmpDate VALUES(''' || :v_DateIter || ''',''' || :v_DateIter || ''')');
		v_DateIter := ADD_DAYS (v_DateIter, 1);
	ELSEIF (:IntervalVal = 'W') THEN
		v_DateTo := ADD_DAYS(v_DateIter, 6);
		exec ('INSERT INTO #CASHFLOW_TmpDate VALUES(''' || :v_DateIter || ''',''' || :v_DateTo || ''')');
		v_DateIter := ADD_DAYS (v_DateIter, 7);
	ELSE
		v_DateTo := ADD_MONTHS(v_DateIter, 1);
		v_DateTo := ADD_DAYS (v_DateTo, -1);
		exec ('INSERT INTO #CASHFLOW_TmpDate VALUES(''' || :v_DateIter || ''',''' || :v_DateTo || ''')');
		v_DateIter := ADD_MONTHS(v_DateIter, 1);
	END IF;
	IF (:v_Iter = 1) THEN
		v_FirstEndDate := ADD_DAYS (v_DateIter, -1);
	END IF;
	v_Iter := :v_Iter + 1;
 END WHILE;
 
 v_DateTo := ADD_DAYS (v_DateIter, -1);
 
 SELECT "EnableBOE", "EnbSgmnAct", "LawsSet" INTO v_BoeEnable,v_SgmActEnable,v_LocalSet FROM "CINF";
 
 SELECT "DspFrznBP" INTO v_FilterInBPFlag FROM OADM;
 
 --- Filter the inactive BP
 create local temporary COLUMN table #CASHFLOW_TmpOCRD("CardCode" nvarChar(15), "CardType" nvarChar(1));

 IF (:v_FilterInBPFlag = 'Y') THEN
	INSERT INTO #CASHFLOW_TmpOCRD SELECT "CardCode", "CardType" FROM OCRD;
 ELSE
	INSERT INTO #CASHFLOW_TmpOCRD 
	SELECT "CardCode", "CardType" FROM OCRD 
	WHERE ("CardCode" IS NULL OR ("validFor" = 'Y' OR ("frozenFor" = 'Y' AND ("frozenFrom" IS NOT NULL OR "frozenTo" IS NOT NULL)) OR ("validFor" = 'N' AND "frozenFor" = 'N' )));
 END IF;
 
 IF (:v_LocalSet = 'CN' OR :v_LocalSet = 'JP') THEN
	v_ApaCheck := 'Y';
 ELSE
	v_ApaCheck := 'N';
 END IF;
 --- Main Currency (think about different currency)
 v_ErrInt := 0;
 SELECT "MainCurncy", "DirectRate" INTO v_MainCurrency, v_DirectRateFlag FROM OADM;
 
 ----Get/Set Account List
 create local temporary COLUMN table #CASHFLOW_TmpAcct ("CardCode" nvarChar(15), "CardName" nvarChar(100), "CardType" nvarChar(1));

 CALL TmSp_DAB004_AccountList_QRYSTR (0, v_QryString);
 v_QryString := 'Insert INTO #CASHFLOW_TmpAcct (' || :v_QryString || ')';
 -- SELECT :v_QryString FROM DUMMY;
 exec (:v_QryString);

 
 ----Get Customer/Vendor deduct account
 v_CustomerDeduct := '';
 v_VendorDeduct := '';
 SELECT COUNT(*) INTO v_ErrInt FROM #CASHFLOW_TmpAcct WHERE "CardType" = 'M' AND "CardCode" IS NOT NULL;
 IF (:v_ErrInt > 0) THEN
	SELECT "CardCode" INTO v_CustomerDeduct FROM #CASHFLOW_TmpAcct WHERE "CardType" = 'M' AND "CardCode" IS NOT NULL;
 END IF;
 SELECT COUNT(*) INTO v_ErrInt FROM #CASHFLOW_TmpAcct WHERE "CardType" = 'U' AND "CardCode" IS NOT NULL;
 IF (:v_ErrInt > 0) THEN
	SELECT "CardCode" INTO v_VendorDeduct FROM #CASHFLOW_TmpAcct WHERE "CardType" = 'U' AND "CardCode" IS NOT NULL;
 END IF;
 
 
 ----Mention transtype value(the N represent normal, R represent recurring, P represent voucher)
 create local temporary COLUMN table #CASHFLOW_TmpOverView ("Indicator" Date, "Debit" DECIMAL(21,6), "Credit" DECIMAL(21,6), "OpenBalance" DECIMAL(21,6), "TransType" nvarChar(1));
 
 ----No open balance
 v_QryString := 'SELECT T5."Ind", SUM(T5."Debit"), SUM(T5."Credit"), 0, T5."Typ" FROM ( ';
 ----Cash,Check,Credit Card
 v_QryString := :v_QryString || ' SELECT T1."EndDate" AS "Ind", T0."BalDueDeb" AS "Debit", T0."BalDueCred" AS "Credit", ''N'' AS "Typ" FROM "JDT1" T0, #CASHFLOW_TmpDate T1    WHERE (T0."DueDate" BETWEEN T1."StartDate" AND T1."EndDate")  AND        (T0."BalDueDeb" <> 0  OR  T0."BalDueCred" <> 0 ) AND  T0."Account" = T0."ShortName" AND      T0."ShortName" IN (SELECT "CardCode" FROM #CASHFLOW_TmpAcct WHERE "CardType" NOT IN (''E'', ''V'',''M'', ''U'')) AND	  (T0."TransType" IN (''30'',''-2'',''69'',''25'',''47'',''-4'',''182'',''203'',''204'',''14'',''13'',''165'',''19'',''18'',''163'',''57'',''76'') OR 	   (T0."TransType" = ''24'' AND T0."ShortName" <> ''' || :v_CustomerDeduct || ''') OR	   (T0."TransType" = ''46'' AND T0."ShortName" <> ''' || :v_VendorDeduct || ''') 	   )   UNION ALL ';
 ----Customer,Vendor 
 v_QryString := :v_QryString || ' SELECT T2."EndDate" AS "Ind", T0."BalDueDeb" AS "Debit", T0."BalDueCred" AS "Credit", ''N'' AS "Typ" FROM "JDT1" T0, #CASHFLOW_TmpOCRD T1, #CASHFLOW_TmpDate T2  WHERE (T0."DueDate" BETWEEN T2."StartDate"  AND T2."EndDate") AND T1."CardCode" = T0."ShortName" AND      (T0."BalDueDeb" <> 0  OR  T0."BalDueCred" <> 0) AND  T0."Account" <> T0."ShortName" AND       T1."CardType" IN (''C'', ''S'')  AND  T0."Account" NOT IN (SELECT "CardCode" FROM #CASHFLOW_TmpAcct WHERE "CardType" = ''E'') AND	 T0."TransType" IN (''30'',''-2'',''69'',''25'',''47'',''-4'',''182'',''203'',''204'',''14'',''13'',''165'',''19'',''18'',''163'',''57'',''76'',''24'',''46'') UNION ALL ';
 ----Boe
 IF (:v_BoeEnable = 'Y') THEN
 v_QryString := :v_QryString || '  SELECT T2."EndDate" AS "Ind", T0."BalDueDeb" AS "Debit", T0."BalDueCred" AS "Credit", ''N'' AS "Typ" FROM  "JDT1" T0, #CASHFLOW_TmpOCRD T1, #CASHFLOW_TmpDate T2   WHERE (T0."DueDate" BETWEEN T2."StartDate" AND T2."EndDate") AND  T1."CardCode" = T0."ShortName" AND      (T0."BalDueDeb" <> 0  OR  T0."BalDueCred" <> 0) AND  T0."Account" <> T0."ShortName" AND      T0."Account" IN (SELECT "CardCode" FROM #CASHFLOW_TmpAcct WHERE "CardType" = ''E'')   UNION ALL ';
 END IF;
 ----Voucher cash,check,credit card
 v_QryString := :v_QryString || ' SELECT T2."EndDate" AS "Ind", T0."Debit" AS "Debit", T0."Credit" AS "Credit", ''P'' AS "Typ" FROM  "BTF1" T0, "OBTF" T1, #CASHFLOW_TmpDate T2    WHERE T0."BatchNum" = T1."BatchNum"  AND T0."TransId" = T1."TransId"  AND  T0."Account" = T0."ShortName" AND T1."BtfStatus" <> ''C'' AND       (T0."DueDate" BETWEEN T2."StartDate" AND T2."EndDate")  AND       T0."ShortName" IN (SELECT "CardCode" FROM #CASHFLOW_TmpAcct WHERE "CardType" IN (''A'', ''C'', ''V'',''D'')) UNION ALL ';
 ----Voucher customer,vendor
 v_QryString := :v_QryString || ' SELECT T3."EndDate" AS "Ind", T0."Debit" AS "Debit", T0."Credit" AS "Credit", ''P'' AS "Typ" FROM  "BTF1" T0, "OBTF" T1, #CASHFLOW_TmpOCRD T2, #CASHFLOW_TmpDate T3       WHERE T0."BatchNum" = T1."BatchNum"  AND T0."TransId" = T1."TransId" AND T1."BtfStatus" <> ''C''  AND      (T0."DueDate" BETWEEN  T3."StartDate"  AND T3."EndDate") AND T2."CardCode" = T0."ShortName" AND    T0."Account" <> T0."ShortName"  AND  T2."CardType" IN (''C'', ''S'')';
 v_QryString := :v_QryString || '  ) AS T5 GROUP BY "Ind", "Typ" ';
 
 ---- SELECT :v_QryString FROM DUMMY;
 exec ('INSERT INTO #CASHFLOW_TmpOverView (' || :v_QryString || ')');
 
 ----Open balance
 v_QryString := ' SELECT ''' || :v_FirstEndDate || ''',';
 v_QryString := :v_QryString || '  0, 0, SUM(T5."OpenBalance"), T5."Typ" FROM ( ';
 ----Cash,Check,Credit Card
 v_QryString := :v_QryString || '  SELECT SUM(T0."BalDueDeb" - T0."BalDueCred") AS "OpenBalance", ''N'' AS "Typ" FROM "JDT1" T0   WHERE T0."DueDate" < ''';
 v_QryString := :v_QryString || :v_DateFrom || '''';
 v_QryString := :v_QryString || ' AND T0."TransType" <> ''-3'' AND  T0."Account" = T0."ShortName" AND     T0."ShortName" IN (SELECT "CardCode" FROM #CASHFLOW_TmpAcct WHERE "CardType" NOT IN (''E'', ''V'', ''M'', ''U''))  UNION ALL ';
 ----Customer,Vendor 
 v_QryString := v_QryString || ' SELECT SUM(T0."BalDueDeb" - T0."BalDueCred") AS "OpenBalance", ''N'' AS "Typ" FROM "JDT1" T0  INNER JOIN #CASHFLOW_TmpOCRD T1 ON  T1."CardCode" = T0."ShortName"    WHERE T0."DueDate" < ''';
  v_QryString := :v_QryString || :v_DateFrom || '''';
  v_QryString := :v_QryString || '    AND T0."TransType" <> ''-3'' AND T0."Account" <> T0."ShortName" AND       T1."CardType" IN (''C'', ''S'')  AND  T0."Account" NOT IN (SELECT "CardCode" FROM #CASHFLOW_TmpAcct WHERE "CardType" = ''E'')  UNION ALL ';
 ----Boe
 IF (:v_BoeEnable = 'Y') THEN
  v_QryString := :v_QryString || '  SELECT SUM(T0."BalDueDeb" - T0."BalDueCred") AS "OpenBalance", ''N'' AS "Typ" FROM "JDT1" T0   INNER JOIN #CASHFLOW_TmpOCRD T1  ON  T1."CardCode" = T0."ShortName"     WHERE T0."DueDate" < ''';
   v_QryString := :v_QryString || :v_DateFrom || '''';
   v_QryString := :v_QryString || '     AND T0."TransType" <> ''-3''  AND  T0."Account" <> T0."ShortName" AND      T0."Account" IN (SELECT "CardCode" FROM #CASHFLOW_TmpAcct WHERE "CardType" = ''E'')   UNION ALL ';
 END IF;
 ----Voucher cash,check,credit card
 v_QryString := :v_QryString || ' SELECT SUM(T0."Debit" - T0."Credit") AS "OpenBalance", ''P'' AS "Typ" FROM  "BTF1" T0, "OBTF" T1    WHERE T0."BatchNum" = T1."BatchNum"  AND T0."TransId" = T1."TransId" AND       T0."Account" = T0."ShortName" AND T1."BtfStatus" <> ''C'' AND T0."DueDate" < ''';
 v_QryString := :v_QryString || :v_DateFrom || '''';
 v_QryString := :v_QryString || '	  AND       T0."ShortName" IN (SELECT "CardCode" FROM #CASHFLOW_TmpAcct WHERE "CardType" IN (''A'', ''C'',''D'')) UNION ALL ';
 ----Voucher customer,vendor
 v_QryString := :v_QryString || ' SELECT SUM(T0."Debit" - T0."Credit") AS "OpenBalance", ''P'' AS "Typ" FROM  "BTF1" T0    INNER  JOIN "OBTF" T1  ON  T0."BatchNum" = T1."BatchNum"  AND  T0."TransId" = T1."TransId"     INNER  JOIN #CASHFLOW_TmpOCRD T2  ON  T2."CardCode" = T0."ShortName"    WHERE T1."BtfStatus" <> ''C''  AND  T0."DueDate" < ''';
  v_QryString := :v_QryString || :v_DateFrom || '''';
  v_QryString := :v_QryString || '    AND     T0."Account" <> T0."ShortName"  AND  T2."CardType" IN (''C'', ''S'')';
 v_QryString := :v_QryString || ' ) AS T5 GROUP BY "Typ" ';
 
 ---- SELECT :v_QryString FROM DUMMY;
 exec ('INSERT INTO #CASHFLOW_TmpOverView (' || :v_QryString || ')');
 
 ----Recurring transaction
 
 --- Recurring transaction loop 1
 v_IsBreakCur := 0;
 OPEN c_RecurTransQry (v_DateTo);
 FETCH c_RecurTransQry INTO v_FrequencyC,v_SubFrequncyC,v_LimitFlagC,v_LimitDateC,v_CurrencyC,v_CtrAcctC,v_AcctCodeC,v_MainInstanceC,v_SubInstanceC,v_NextDueC,v_DebitC,v_CreditC;
 WHILE NOT c_RecurTransQry::NOTFOUND DO
	v_Frequency := :v_FrequencyC;
	v_SubFrequncy := :v_SubFrequncyC;
	v_LimitFlag := :v_LimitFlagC;
	v_LimitDate := :v_LimitDateC;
	v_Currency := :v_CurrencyC;
	v_CtrAcct := :v_CtrAcctC;
	v_AcctCode := :v_AcctCodeC;
	v_MainInstance := :v_MainInstanceC;
	v_SubInstance := :v_SubInstanceC;
	v_NextDue := :v_NextDueC;
	v_Debit := :v_DebitC;
	v_Credit := :v_CreditC;
	--- Recurring transaction  loop 2
	WHILE (:v_NextDue <= :v_DateTo AND (:v_LimitFlag <> 'Y' OR :v_NextDue <= :v_LimitDate)) DO
		v_Rate := 1;
		IF (:v_Currency <> :v_MainCurrency ) THEN 
		--- Logic for dashboard: no exchange rate that day then use exchange rate today, return error even no exchange rate today
			SELECT COUNT(*) INTO v_TempInt FROM "ORTT" WHERE "Currency" = :v_Currency AND "RateDate" = :v_NextDue;
			IF (:v_TempInt = 0) THEN
				SELECT COUNT(*) INTO v_TempInt FROM "ORTT" WHERE "Currency" = :v_Currency AND "RateDate" = :v_DateFrom;
				IF (:v_TempInt = 0) THEN
					v_ErrInt := 2;
					v_IsBreakCur := 1;
					BREAK;
				ELSE
					v_ErrInt := 1;
					SELECT "Rate" INTO v_Rate FROM "ORTT" WHERE "Currency" = :v_Currency AND "RateDate" = :v_DateFrom;
					IF (:v_DirectRateFlag <> 'Y') THEN
						v_Rate := 1.0/:v_Rate;
					END IF;
				END IF;
			ELSE
				SELECT "Rate" INTO v_Rate FROM "ORTT" WHERE "Currency" = :v_Currency AND "RateDate" = :v_NextDue;
				IF (:v_DirectRateFlag <> 'Y') THEN
						v_Rate := 1.0/:v_Rate;
				END IF;
			END IF;
		END IF;
		----Not open balance
		IF (:v_NextDue >= :v_DateFrom AND :v_MainInstance = :v_SubInstance AND (:v_Frequency <>'O' OR :v_MainInstance <> 0)) THEN
			IF (:v_CtrAcct = :v_AcctCode) THEN
				IF (:v_BoeEnable = 'Y') THEN
					INSERT INTO #CASHFLOW_TmpOverView SELECT T0."EndDate", :v_Debit*:v_Rate, v_Credit*v_Rate, 0, 'R' FROM #CASHFLOW_TmpDate T0, #CASHFLOW_TmpAcct T1 WHERE (T0."StartDate" <= :v_NextDue AND T0."EndDate" >= :v_NextDue) AND T1."CardCode" = :v_CtrAcct AND T1."CardType" <> 'V';
				ELSE
					INSERT INTO #CASHFLOW_TmpOverView SELECT T0."EndDate", :v_Debit*:v_Rate, v_Credit*v_Rate, 0, 'R' FROM #CASHFLOW_TmpDate T0, #CASHFLOW_TmpAcct T1 WHERE (T0."StartDate" <= :v_NextDue AND T0."EndDate" >= :v_NextDue) AND T1."CardCode" = :v_CtrAcct AND T1."CardType" <> 'V' AND T1."CardType" <> 'E';
				END IF;
			ELSE
				INSERT INTO #CASHFLOW_TmpOverView SELECT T0."EndDate", :v_Debit*:v_Rate, v_Credit*v_Rate, 0, 'R' FROM #CASHFLOW_TmpDate T0, #CASHFLOW_TmpOCRD T1 WHERE (T0."StartDate" <= :v_NextDue AND T0."EndDate" >= :v_NextDue) AND T1."CardCode" = :v_AcctCode;
			END IF;
		END IF;
		----Open balance
		IF (:v_NextDue < :v_DateFrom AND :v_SubInstance = 0 AND :v_Debit <> :v_Credit) THEN
			IF (:v_CtrAcct = :v_AcctCode) THEN
				IF (:v_BoeEnable = 'Y') THEN
					INSERT INTO #CASHFLOW_TmpOverView SELECT :v_FirstEndDate, 0, 0,(:v_Debit - :v_Credit) * :v_Rate, 'R' FROM #CASHFLOW_TmpAcct T0 WHERE T0."CardCode" =:v_CtrAcct AND T0."CardType" <> 'V';
				ELSE
					INSERT INTO #CASHFLOW_TmpOverView SELECT :v_FirstEndDate, 0, 0,(:v_Debit - :v_Credit) * :v_Rate, 'R' FROM #CASHFLOW_TmpAcct T0 WHERE T0."CardCode" =:v_CtrAcct AND T0."CardType" <> 'V' AND T0."CardType" <> 'D';
				END IF;
			ELSE
				INSERT INTO #CASHFLOW_TmpOverView SELECT :v_FirstEndDate, 0, 0,(:v_Debit - :v_Credit) * :v_Rate, 'R' FROM #CASHFLOW_TmpOCRD T0 WHERE T0."CardCode" =:v_AcctCode;
			END IF;
		END IF;
		
		IF (:v_Frequency = 'D') THEN
			v_NextDue := ADD_DAYS (v_NextDue, v_SubFrequncy);
		ELSEIF (:v_Frequency = 'W') THEN
			v_NextDue := ADD_DAYS (v_NextDue, 7);
		ELSEIF (:v_Frequency = 'A') THEN
			v_NextDue := ADD_YEARS (v_NextDue, 1);
		ELSEIF (:v_Frequency = 'M') THEN
			v_NextDue := ADD_MONTHS (v_NextDue, 1);
		ELSEIF (:v_Frequency = 'S') THEN
			v_NextDue := ADD_MONTHS (v_NextDue, 6);
		ELSEIF (:v_Frequency = 'Q') THEN
			v_NextDue := ADD_MONTHS (v_NextDue, 3);
		ELSE
			BREAK;
		END IF;
	END WHILE;
	
	FETCH c_RecurTransQry INTO v_FrequencyC,v_SubFrequncyC,v_LimitFlagC,v_LimitDateC,v_CurrencyC,v_CtrAcctC,v_AcctCodeC,v_MainInstanceC,v_SubInstanceC,v_NextDueC,v_DebitC,v_CreditC;
	IF (:v_IsBreakCur = 1) THEN
		BREAK;
	END IF;
 END WHILE;
 CLOSE c_RecurTransQry;
 
 --- Return if Error
 --IF v_ErrInt = 2 THEN
   --GOTO FINAL_RET
 --END
 
 --- Addtional project transactions
 IF (v_ErrInt <> 2) THEN
	v_IsBreakCur := 0;
	IF (v_BoeEnable = 'Y') THEN
		OPEN c_ProjectQry1 (v_DateTo);
		FETCH c_ProjectQry1 INTO v_NextDueC, v_DebitC, v_DebitCurrencyC, v_CreditC, v_CreditCurrencyC;
		WHILE (NOT c_ProjectQry1::NOTFOUND) DO
			v_NextDue := :v_NextDueC;
			v_Debit := :v_DebitC;
			v_DebitCurrency := :v_DebitCurrencyC;
			v_Credit := :v_CreditC;
			v_CreditCurrency := :v_CreditCurrencyC;
			v_Rate := 1.0;
			IF (:v_DebitCurrency <> :v_MainCurrency  OR :v_CreditCurrency <> :v_MainCurrency) THEN
				--- Logic for dashboard: no exchange rate that day then use exchange rate today, return error even no exchange rate today
				SELECT COUNT(*) INTO v_TempInt FROM "ORTT" WHERE ("Currency" = :v_DebitCurrency OR "Currency" = :v_CreditCurrency) AND "RateDate" = :v_NextDue;
				IF (:v_TempInt = 0) THEN
					SELECT  COUNT(*) INTO v_TempInt FROM "ORTT" WHERE ("Currency" = :v_DebitCurrency OR "Currency" = :v_CreditCurrency) AND "RateDate" = :v_DateFrom;
					IF (:v_TempInt = 0) THEN
						v_ErrInt := 2;
						v_IsBreakCur := 1;
						BREAK;
					ELSE
						v_ErrInt := 1;
						SELECT "Rate" INTO v_Rate FROM "ORTT" WHERE ("Currency" = :v_DebitCurrency OR "Currency" = :v_CreditCurrency) AND "RateDate" = :v_DateFrom;
						IF (:v_DirectRateFlag <> 'Y') THEN
							v_Rate := 1.0 / :v_Rate;
						END IF;
					END IF;
				ELSE
					SELECT "Rate" INTO v_Rate FROM "ORTT" WHERE ("Currency" = :v_DebitCurrency OR "Currency" = :v_CreditCurrency) AND "RateDate" = :v_NextDue;
					IF (:v_DirectRateFlag <> 'Y') THEN
						v_Rate := 1.0 / :v_Rate;
					END IF;	
				END IF;
			END IF;
			----Not open balance
			IF (:v_NextDue >= :v_DateFrom) THEN
				INSERT INTO #CASHFLOW_TmpOverView SELECT T0."EndDate", :v_Credit * :v_Rate, :v_Debit * :v_Rate, 0, 'N' FROM #CASHFLOW_TmpDate T0 WHERE  T0."StartDate" <= :v_NextDue AND T0."EndDate" >= :v_NextDue;
			END IF;
			----Open balance
			IF :v_NextDue < :v_DateFrom THEN
				INSERT INTO #CASHFLOW_TmpOverView VALUES(:v_FirstEndDate, 0, 0, (:v_Credit - :v_Debit) * :v_Rate, 'N');
			END IF;
			FETCH c_ProjectQry1 INTO v_NextDueC, v_DebitC, v_DebitCurrencyC, v_CreditC, v_CreditCurrencyC;
		END WHILE;
		CLOSE c_ProjectQry1;
	ELSE
		OPEN c_ProjectQry2 (v_DateTo);
		FETCH c_ProjectQry2 INTO v_NextDueC, v_DebitC, v_DebitCurrencyC, v_CreditC, v_CreditCurrencyC;
		WHILE (NOT c_ProjectQry2::NOTFOUND) DO
			v_NextDue := :v_NextDueC;
			v_Debit := :v_DebitC;
			v_DebitCurrency := :v_DebitCurrencyC;
			v_Credit := :v_CreditC;
			v_CreditCurrency := :v_CreditCurrencyC;
			v_Rate := 1.0;
			IF (:v_DebitCurrency <> :v_MainCurrency  OR :v_CreditCurrency <> :v_MainCurrency) THEN
				--- Logic for dashboard: no exchange rate that day then use exchange rate today, return error even no exchange rate today
				SELECT COUNT(*) INTO v_TempInt FROM "ORTT" WHERE ("Currency" = :v_DebitCurrency OR "Currency" = :v_CreditCurrency) AND "RateDate" = :v_NextDue;
				IF (:v_TempInt = 0) THEN
					SELECT  COUNT(*) INTO v_TempInt FROM "ORTT" WHERE ("Currency" = :v_DebitCurrency OR "Currency" = :v_CreditCurrency) AND "RateDate" = :v_DateFrom;
					IF (:v_TempInt = 0) THEN
						v_ErrInt := 2;
						v_IsBreakCur := 1;
						BREAK;
					ELSE
						v_ErrInt := 1;
						SELECT "Rate" INTO v_Rate FROM "ORTT" WHERE ("Currency" = :v_DebitCurrency OR "Currency" = :v_CreditCurrency) AND "RateDate" = :v_DateFrom;
						IF (:v_DirectRateFlag <> 'Y') THEN
							v_Rate := 1.0 / :v_Rate;
						END IF;
					END IF;
				ELSE
					SELECT "Rate" INTO v_Rate FROM "ORTT" WHERE ("Currency" = :v_DebitCurrency OR "Currency" = :v_CreditCurrency) AND "RateDate" = :v_NextDue;
					IF (:v_DirectRateFlag <> 'Y') THEN
						v_Rate := 1.0 / :v_Rate;
					END IF;	
				END IF;
			END IF;
			----Not open balance
			IF (:v_NextDue >= :v_DateFrom) THEN
				INSERT INTO #CASHFLOW_TmpOverView SELECT T0."EndDate", :v_Credit * :v_Rate, :v_Debit * :v_Rate, 0, 'N' FROM #CASHFLOW_TmpDate T0 WHERE  T0."StartDate" <= :v_NextDue AND T0."EndDate" >= :v_NextDue;
			END IF;
			----Open balance
			IF :v_NextDue < :v_DateFrom THEN
				INSERT INTO #CASHFLOW_TmpOverView VALUES(:v_FirstEndDate, 0, 0, (:v_Credit - :v_Debit) * :v_Rate, 'N');
			END IF;
			FETCH c_ProjectQry2 INTO v_NextDueC, v_DebitC, v_DebitCurrencyC, v_CreditC, v_CreditCurrencyC;
		END WHILE;
		CLOSE c_ProjectQry2;
	END IF;
 END IF;
 
 ----Final Results
 --- First line show the message info,'E' represent Error, 'W' represent Warning, 'N' represent Normal
 --FINAL_RET:
 IF (:v_ErrInt = 2) THEN
	SELECT NULL As "EndDate", NULL As "Debit", NULL As "Credit", NULL As "OpenBalance", 'E' As "TransType" FROM DUMMY;
 ELSE
	v_QryString := 'SELECT T5.* FROM (';
	IF (v_ErrInt = 1) THEN
		v_QryString := :v_QryString || 'SELECT NULL AS "EndDate", NULL AS "Debit", NULL  AS "Credit", NULL AS "OpenBalance", ''W'' AS "TransType" FROM DUMMY 		UNION ALL ';
	ELSE
		v_QryString := :v_QryString || 'SELECT NULL AS "EndDate", NULL AS "Debit", NULL  AS "Credit", NULL AS "OpenBalance", ''N'' AS "TransType" FROM DUMMY         UNION ALL ';
	END IF;
	v_QryString := :v_QryString || 'SELECT T4."EndDate" AS "EndDate",  T4."Debit" AS "Debit", T4."Credit" AS "Credit", T4."OpenBalance" AS "OpenBalance", T4."TransType" AS "TransType" FROM (          SELECT T2."EndDate" AS "EndDate", SUM(T3."Debit") AS "Debit", SUM(T3."Credit") AS "Credit", SUM(T3."OpenBalance") AS "OpenBalance", T2."TransType" AS "TransType"          FROM (             SELECT T0."TransType", T1."EndDate" FROM (              SELECT ''N'' AS "TransType" FROM DUMMY              UNION              SELECT ''P'' AS "TransType" FROM DUMMY              UNION              SELECT ''R'' AS "TransType" FROM DUMMY) AS T0, #CASHFLOW_TmpDate T1          ) AS T2           LEFT JOIN #CASHFLOW_TmpOverView T3 ON T3."TransType" = T2."TransType" AND T2."EndDate" = T3."Indicator"          GROUP BY T2."EndDate", T2."TransType") AS T4    ) AS T5    ORDER BY T5."EndDate", T5."TransType" ';
	EXECUTE IMMEDIATE (:v_QryString);
 END IF;

 -- Drop Temp tables
 --DROP TABLE #CASHFLOW_TmpDate;
 --DROP TABLE #CASHFLOW_TmpOCRD;
 --DROP TABLE #CASHFLOW_TmpAcct;
 --DROP TABLE #CASHFLOW_TmpOverView;
 END;


