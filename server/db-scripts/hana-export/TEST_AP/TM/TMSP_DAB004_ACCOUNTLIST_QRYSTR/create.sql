-- B1 DEPENDS: AFTER:PT:PROCESS_END
--DROP PROCEDURE TmSp_DAB004_AccountList_QRYSTR;
CREATE PROCEDURE TmSp_DAB004_AccountList_QRYSTR(in Indexer int, OUT outQryString nvarchar(8000))
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
BoeEnable nchar;
SgmActEnable nchar;
LocalSet nvarchar(3); 
ApaCheck nchar;
--QryString nvarchar(max);
QryString nvarchar(8000);
PeriodCategory nvarchar(10);
PerPageVal int := 0;
CommonStr nvarchar(8000);
ActSep nchar;

BEGIN

	SELECT "EnableBOE", "EnbSgmnAct", "LawsSet" INTO BoeEnable, SgmActEnable, LocalSet FROM CINF;

	IF :LocalSet = N'CN' OR :LocalSet = N'JP' THEN
		ApaCheck := N'Y';
	ELSE
		ApaCheck := N'N';
	END IF;
	
	SELECT "ActSep" INTO ActSep FROM OADM;

	--- Initialization
	PerPageVal := 5;

	--- Period Category
	SELECT "Category" INTO PeriodCategory FROM OFPR 
	GROUP BY "Category" HAVING MIN(TO_DATE("F_RefDate")) <= TO_DATE(CURRENT_TIMESTAMP) AND MAX(TO_DATE("T_RefDate")) >= TO_DATE(CURRENT_TIMESTAMP);
	
	---Remove duplicated record
	CommonStr := ' SELECT T6."AcctCode", T6."AcctName", T6."Balance", MIN(T6."AcctType") "AcctType" FROM (';

	-----'A' represent Cash Account, 'C' represent Check Account, 'D' represent Credit Card, 'E' represent Boe, 'V' represent Voucher, 'U' represent Vendor deduct, 'M' represent Customer deduct
	---Cash account
	CommonStr := :CommonStr || ' SELECT T0."AcctCode" "AcctCode", T0."AcctName" "AcctName", T0."CurrTotal" "Balance", ''A'' "AcctType" FROM OACT T0 WHERE T0."Finanse" = ''Y''' ||
	' UNION';
	 
	IF :ApaCheck <> N'Y' THEN
		----Check account not in CN,JP
		--- Check receivable
		CommonStr := :CommonStr || ' SELECT T1."AcctCode" "AcctCode", T1."AcctName" "AcctName", T1."CurrTotal" "Balance", ''C'' "AcctType" FROM OACP T0 INNER JOIN OACT T1 ON T0."LinkAct_2" = T1."AcctCode"' || 
		' WHERE T0."PeriodCat" = ''' || :PeriodCategory || '''' || 
		' UNION';
		--- Check in transactions
		CommonStr := :CommonStr || ' SELECT T0."CashCheck" "AcctCode", MIN(T1."AcctName") "AcctName", MIN(T1."CurrTotal") "Balance", ''C'' "AcctType" FROM OCHH T0 INNER JOIN OACT T1 ON T0."CashCheck" = T1."AcctCode"' || 
		' GROUP BY T0."CashCheck"' ||
		' UNION';
	END IF;

	IF :BoeEnable = N'Y' THEN
		--- Boe account enable
		--- Boe receivable, payable...
		CommonStr := :CommonStr || ' SELECT T1."AcctCode" "AcctCode", T1."AcctName" "AcctName", T1."CurrTotal" "Balance", ''E'' "AcctType" FROM OACP T0 , OACT T1' ||
		' WHERE (T0."CBoERcvble" = T1."AcctCode" OR' ||
				' T0."CBoEOnClct" = T1."AcctCode" OR' || 
				' T0."CBoEDiscnt" = T1."AcctCode" OR' || 
				' T0."CBoEPresnt" = T1."AcctCode" OR' || 
				' T0."CUnpaidBoE" = T1."AcctCode" OR' || 
				' T0."VBoEPayble" = T1."AcctCode" OR' || 
				' T0."VAsstBoEPy" = T1."AcctCode") AND T0."PeriodCat" = ''' || :PeriodCategory || ''''  
		' UNION';
		
		---Boe in transactions
		CommonStr := :CommonStr || ' SELECT T0."AcctCode" "AcctCode", MIN(T1."AcctName") "AcctName", MIN(T1."CurrTotal") "Balance", ''E'' "AcctType" FROM CRD3 T0 INNER JOIN  OACT T1 ON T0."AcctCode" = T1."AcctCode"' || 
		' WHERE T0."AcctType" IN (''R'', ''P'', ''C'', ''S'', ''Y'', ''I'', ''U'')'  ||
		' GROUP BY T0."AcctCode"' ||
		' UNION';
	END IF;

	--- Credit card
	CommonStr := :CommonStr || ' SELECT T0."CreditAcct" "AcctCode", MIN(T1."AcctName") "AcctName", MIN(T1."CurrTotal") "Balance", ''D'' "AcctType" FROM OCRH T0 INNER JOIN OACT T1 ON T0."CreditAcct" = T1."AcctCode"' ||
	' GROUP BY T0."CreditAcct"' || 
	' ) T6 GROUP BY T6."AcctCode", T6."AcctName", T6."Balance"';
	
	--- Voucher
	CommonStr := :CommonStr || ' UNION' ||
	' SELECT T1."AcctCode" "AcctCode", T1."AcctName" "AcctName", T1."CurrTotal" "Balance", ''V'' "AcctType" FROM OACP T0, OACT T1' ||
	' WHERE ( T0."LinkAct_5" = T1."AcctCode" OR' || 
			' T0."LinkAct_14" = T1."AcctCode" OR' ||
			' T0."LinkAct_15" = T1."AcctCode" ) AND T0."PeriodCat" = ''' || :PeriodCategory || '''';
	
	--- Customer Deduct account
	CommonStr := :CommonStr || ' UNION' || 
	' SELECT T1."AcctCode" "AcctCode", T1."AcctName" "AcctName", T1."CurrTotal" "Balance", ''M'' "AcctType" FROM OACP T0, OACT T1' ||
	' WHERE  T0."LinkAct_6" = T1."AcctCode" AND T0."PeriodCat" = ''' || :PeriodCategory || ''''; 
	
	--- Vendor Deduct account
	CommonStr := :CommonStr || ' UNION ' ||
	' SELECT T1."AcctCode" "AcctCode", T1."AcctName" "AcctName", T1."CurrTotal" "Balance", ''U'' "AcctType" FROM OACP T0, OACT T1' ||
	' WHERE  T0."LinkAct_16" = T1."AcctCode" AND T0."PeriodCat" = ''' || :PeriodCategory || ''''; 
	
	--- Final Query  
	--- Return different result collection according to @Indexer
	IF :Indexer <> 0 THEN
		----First Line is special line including total records& result type
		QryString := ' SELECT ''A'' as "AcctCode", NULL as "AcctName", COUNT(0) as "Balance" FROM (' || 
					:CommonStr || ' ) AS T7 WHERE T7."AcctType" NOT IN (''M'',''U'',''V'')' ||
					' UNION ALL (';
		IF :SgmActEnable = N'Y' THEN
			QryString := :QryString || ' SELECT REPLACE(RTRIM(IFNULL(T9."Segment_0",'''')||'' ''||IFNULL(T9."Segment_1",'''')||'' ''||IFNULL(T9."Segment_2",'''')||'' ''||IFNULL(T9."Segment_3",'''')||'' ''||IFNULL(T9."Segment_4",'''')||'' ''||IFNULL(T9."Segment_5",'''')||'' ''||IFNULL(T9."Segment_6",'''')||'' ''||IFNULL(T9."Segment_7",'''')||'' ''||IFNULL(T9."Segment_8",'''')||'' ''||IFNULL(T9."Segment_9",'''')),'' '','''||:ActSep||'''), T8."AcctName", T8."Balance"';
		ELSE
			QryString := :QryString || ' SELECT T8.*';					   
		END IF;
		
		QryString := :QryString || ' FROM ((SELECT TOP ' || :Indexer*:PerPageVal || ' T7."AcctCode", T7."AcctName", T7."Balance" FROM (' ||
					:CommonStr || ' ) AS T7 WHERE T7."AcctType" NOT IN (''M'', ''U'', ''V'')  ORDER BY T7."AcctCode")';
		
		IF :Indexer > 1 THEN
			QryString := :QryString || ' EXCEPT' ||
						' (SELECT TOP ' || (:Indexer-1)*:PerPageVal || ' T7."AcctCode", T7."AcctName", T7."Balance" FROM (' ||
						:CommonStr || ' ) AS T7 WHERE T7."AcctType" NOT IN (''M'', ''U'', ''V'') ORDER BY T7."AcctCode")';						  
		END IF;

		QryString := :QryString || ' ) AS T8';
		
		IF :SgmActEnable = N'Y' THEN
			QryString := :QryString || ' LEFT OUTER JOIN OACT T9 ON T8."AcctCode" = T9."AcctCode"';
		END IF;
		
		QryString := :QryString || ' ORDER BY T8."AcctCode")';
	ELSE
		QryString := ' SELECT T7."AcctCode", T7."AcctName", T7."AcctType" FROM (' || :CommonStr || ' ) AS T7 ORDER BY T7."AcctCode"';
	END IF;
	
	--SELECT QryString FROM DUMMY;
	--EXEC :QryString;
	outQryString := :QryString;
END;


