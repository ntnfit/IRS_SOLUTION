-- B1 DEPENDS: AFTER:PT:PROCESS_END AFTER:SP:TmSp_B1CFLDeployFunctions AFTER:SP:_TmSp_BootCreateGlobalTempTables2 AFTER:SP:_TmSp_ValidateSpParam

CREATE PROCEDURE TmSp_B1CFLGetDocuments(
	v_nvFrom NVARCHAR(8),
	v_nvTo NVARCHAR(8),
	v_docViewName NVARCHAR(32),
	v_predictDueDate SMALLINT,
	v_useDelay SMALLINT,
	v_inclRecurrTrans SMALLINT,
	v_tempORCLName NVARCHAR(32))
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
	v_sql nclob;
	v_nvDelayTable NVARCHAR(2);
	v_nvCardCode NVARCHAR(15);
	v_nvCardCodeBeforeNext NVARCHAR(15);
	v_siPmntDate SMALLINT; -- Although CRD5."PmntDate" is NVARCHAR(2) !!!
	v_siPmntDateDayFirst SMALLINT; -- Although CRD5."PmntDate" is NVARCHAR(2) !!!
	v_siDueDateDay SMALLINT; -- Although CRD5."PmntDate" is NVARCHAR(2) !!!.
	v_nvDueDateField NVARCHAR(15);
	CURSOR crPaymentDates FOR SELECT T0."CardCode", T0."PmntDate" 
	FROM (SELECT "CardCode", TO_SMALLINT("PmntDate") AS "PmntDate" FROM CRD5) T0 ORDER BY T0."CardCode" ASC, T0."PmntDate" ASC;
	CURSOR cB1CFLGetDocuments FOR SELECT * FROM "B1CFLGetDocuments";
	v_cnt integer;
	v_HolidayDueDate date;
BEGIN
	--For HANA security issue call procedure "_TmSp_ValidateSpParam"
    call _TmSp_ValidateSpParam(:v_nvFrom);    
    call _TmSp_ValidateSpParam(:v_nvTo);    
	call _TmSp_ValidateSpParam(:v_docViewName);   
	call _TmSp_ValidateSpParam(:v_tempORCLName);   
	
	IF :v_predictDueDate = 0 THEN
		-- Query for Documents with Due Date
		v_sql := 'SELECT T0."CtlAccount", T0."CardCode", T0."DueDate", T0."Debit", T0."Credit", T0."DebitFC", T0."CreditFC", T0."DebitSC", T0."CreditSC", T0."DocCur", T0."DocDate", T0."TransId", T0."DocNum", T0."DocNum", T0."JrnlMemo", T0."ObjType", T0."Project", T0."AgrNo", T0."DocEntry", NULL AS NA, T0."AgrNum", T0."AvrageLate", T0."InstID", 
		 T0."InstNum", T0."InstPrcnt", T0."GroupNum"
			 FROM "' || :v_docViewName || '" T0 ';
		v_nvDelayTable := 'T0';
	ELSE
		select count (*) into v_cnt from "PUBLIC"."M_TEMPORARY_TABLES" where TABLE_NAME = '#B1CFLPaymentDates' and connection_id = current_connection;	
		if :v_cnt > 0 then
			exec ('drop table "#B1CFLPaymentDates"');
		END if;
		
		--SELECT "CardCode" INTO "#B1CFLPaymentDates" FROM CRD5 WHERE "CardCode" IS NULL
		CREATE LOCAL TEMPORARY TABLE "#B1CFLPaymentDates" (
			"CardCode" NVARCHAR(15) NOT NULL ,
			"DueDateDay" SMALLINT NOT NULL, -- Although CRD5."PmntDate" is NVARCHAR(2) !!!
			"PmntDate" SMALLINT NOT NULL, -- Although CRD5."PmntDate" is NVARCHAR(2) !!!
			"NextMonth" SMALLINT NOT NULL);
		delete from "#B1CFLPaymentDates" where 1=1;
		--CREATE NONCLUSTERED INDEX B1CFLPaymentDates_PRIMARY ON "#B1CFLPaymentDates" (
		--CardCode ASC,
		--DueDateDay ASC,
		--	PmntDate ASC
		--) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

		open crPaymentDates;
		
		FETCH crPaymentDates INTO v_nvCardCode, v_siPmntDate;
		v_siPmntDateDayFirst := v_siPmntDate;
		v_siDueDateDay := 1;
		while (NOT crPaymentDates::NOTFOUND) DO
			WHILE :v_siDueDateDay <= :v_siPmntDate do
				INSERT INTO "#B1CFLPaymentDates" VALUES (:v_nvCardCode, :v_siDueDateDay, :v_siPmntDate, 0);
				v_siDueDateDay := :v_siDueDateDay + 1;
			END while;
			v_nvCardCodeBeforeNext := :v_nvCardCode;
			
			FETCH crPaymentDates INTO v_nvCardCode, v_siPmntDate;
			
			IF :v_nvCardCodeBeforeNext <> :v_nvCardCode THEN
				WHILE :v_siDueDateDay <= 31 DO
					INSERT INTO "#B1CFLPaymentDates" VALUES (:v_nvCardCodeBeforeNext, :v_siDueDateDay, :v_siPmntDateDayFirst, 1);
					v_siDueDateDay := :v_siDueDateDay + 1;
				END WHILE;
				v_siDueDateDay := 1;
			END if;			
		END WHILE;
		WHILE (:v_siDueDateDay <= 31 AND :v_nvCardCodeBeforeNext IS NOT NULL) DO
			INSERT INTO "#B1CFLPaymentDates" VALUES (:v_nvCardCodeBeforeNext, :v_siDueDateDay, v_siPmntDateDayFirst, 1);
			v_siDueDateDay := :v_siDueDateDay + 1;
		END WHILE;
		close crPaymentDates;
		
		-- Query for Documents with Prediction of Due Date
		
		IF :v_inclRecurrTrans = 1 then
			v_nvDueDateField := 'T2."PlanDate"';
		ELSE 
			v_nvDueDateField := 'T0."DueDate"';
		end if;
		
		truncate table "B1CFLGetDocuments";
		truncate table "B1CFLGetDocumentsHoliday";
		--sub table
		v_sql := 'insert into "B1CFLGetDocuments" SELECT T0."DocEntry", T0."InstID", T0."DueDateByPaymentDates" AS "DueDate", T0."HldCode", T0."Instance", T0."Code"
			FROM (
				SELECT T0."DocEntry", T0."InstID", T0."HldCode", T0."Instance", T0."Code",
					CASE
						WHEN T1."CardCode" IS NOT NULL THEN
							CASE WHEN T1."PmntDate" > dayofmonth(last_day(add_months(T0."DueDateOnDoc",T1."NextMonth"))) 
								THEN to_date(YEAR(add_months(T0."DueDateOnDoc",T1."NextMonth"+1)) || ''-'' || MONTH(add_months(T0."DueDateOnDoc",T1."NextMonth"+1)) || ''-1'')
								ELSE to_date(YEAR(add_months(T0."DueDateOnDoc",T1."NextMonth")) || ''-'' || MONTH(add_months(T0."DueDateOnDoc",T1."NextMonth")) || ''-''	|| T1."PmntDate")
							END
						ELSE T0."DueDateOnDoc"
					END AS "DueDateByPaymentDates"
				FROM (
					SELECT T0."DocEntry", T0."InstID", T0."CardCode", T0."HldCode",
						CASE
							WHEN T0."PayDuMonth" = ''N'' THEN to_date(add_days(add_months(' || :v_nvDueDateField || ', T0."ExtraMonth" + T0."InstMonth"), T0."ExtraDays" + T0."InstDays" - COALESCE(T1."TolDays", 0)))
							WHEN T0."PayDuMonth" = ''Y'' THEN to_date(add_days(add_months(next_day(last_day(' || :v_nvDueDateField || ')), T0."ExtraMonth" + T0."InstMonth"), T0."ExtraDays" + T0."InstDays" - COALESCE(T1."TolDays", 0)))
							WHEN T0."PayDuMonth" = ''H'' THEN to_date(add_days(add_months(YEAR(' || :v_nvDueDateField || ') || ''-'' || Month(' || :v_nvDueDateField || ') || ''-'' || floor(DAYOFMONTH(last_day(' || :v_nvDueDateField || '))/2)+1, T0."ExtraMonth" + T0."InstMonth"), T0."ExtraDays" + T0."InstDays" - COALESCE(T1."TolDays", 0)))
							WHEN T0."PayDuMonth" = ''E'' THEN to_date(add_days(add_months(last_day(' || :v_nvDueDateField || '), T0."ExtraMonth" + T0."InstMonth"), T0."ExtraDays" + T0."InstDays" - COALESCE(T1."TolDays", 0)))
							ELSE NULL
						END AS "DueDateOnDoc", ';
		IF :v_inclRecurrTrans = 1 then
			v_sql := :v_sql || 'T2."Instance", T3."Code" ';
		
		ELSE 
			v_sql := :v_sql || 'NULL AS "Instance", NULL AS "Code" ';
		END IF;
		v_sql := :v_sql || '
					FROM "' || :v_docViewName || '" T0
					INNER JOIN OCTG T1 ON T1."GroupNum" = T0."GroupNum"';
		IF :v_inclRecurrTrans = 1 then
			v_sql := v_sql || ' 
				INNER JOIN (SELECT * FROM ORCL  
								UNION ALL 
								SELECT * FROM "' || :v_tempORCLName || '") T2 
						ON T2."DocEntry" = T0."DocEntry" AND ABS(T2."DocObjType") = T0."ObjType" AND T2."Status" = ''N'' 
					INNER JOIN ORCP T3 ON T3."AbsEntry" = T2."RcpEntry"';
		end if;
		v_sql := :v_sql || ') T0
				LEFT JOIN "#B1CFLPaymentDates" T1 ON T1."CardCode" = T0."CardCode" AND T1."DueDateDay" = EXTRACT (DAY FROM T0."DueDateOnDoc")
			) T0';
		--SELECT :V_SQL from dummy;
		EXEC (:v_sql);
		
		FOR cur_row as cB1CFLGetDocuments DO 
			IF cur_row."HldCode" IS NULL THEN
				call B1CFLConsiderHoliday (cur_row."DueDate", '', :v_HolidayDueDate);
			ELSE
				call B1CFLConsiderHoliday (cur_row."DueDate", cur_row."HldCode", :v_HolidayDueDate);
			END IF;
			insert into "B1CFLGetDocumentsHoliday" values(cur_row."DocEntry",cur_row."InstID",:v_HolidayDueDate,cur_row."Instance",cur_row."Code");

		END FOR;
				
		--main table
		v_sql := 'SELECT T1."CtlAccount", T1."CardCode", T0."DueDate", T1."Debit", T1."Credit", T1."DebitFC", T1."CreditFC", T1."DebitSC", T1."CreditSC", T1."DocCur", T1."DocDate", T1."TransId", T1."DocNum", T1."DocNum", T1."JrnlMemo", T1."ObjType", T1."Project", T1."AgrNo", T1."DocEntry", NULL AS NA, T1."AgrNum", T1."AvrageLate", T1."InstID", T1."InstNum", T1."InstPrcnt", T1."GroupNum"';
		IF :v_inclRecurrTrans = 1  then
			v_sql := :v_sql || ', T0."Code", T0."Instance"';
		else 
			v_sql := :v_sql || ', null,null';
		end if;
		v_sql := v_sql || ' FROM "B1CFLGetDocumentsHoliday" T0
		INNER JOIN "' || :v_docViewName || '" T1 ON T1."DocEntry" = T0."DocEntry" AND T1."InstID" = T0."InstID" ';
		v_nvDelayTable := 'T1';
	END IF;

	v_sql := :v_sql || ' WHERE T0."DueDate" >= add_days(''' || :v_nvFrom || ''', ';
	IF :v_useDelay = 1 THEN
		v_sql := v_sql || '-' || :v_nvDelayTable || '."AvrageLate") ';
	ELSE 
		v_sql := v_sql || '0)';
	END IF; 
	v_sql := v_sql || 'AND T0."DueDate" <= add_days(''' || :v_nvTo || ''', ';
	IF v_useDelay = 1 THEN
		v_sql := :v_sql || '-' || :v_nvDelayTable || '."AvrageLate") ';
	ELSE 
		v_sql := v_sql || '0)';
	END IF;
	
	IF :v_predictDueDate = 0 THEN
		v_sql := :v_sql || 'ORDER BY T0."DocEntry" ASC ';
	ELSEIF :v_inclRecurrTrans = 0  THEN
		v_sql := :v_sql || 'ORDER BY T1."DocEntry" ASC, T1."InstID" ASC ';
	ELSE 
		v_sql := :v_sql || 'ORDER BY T1."DocEntry" ASC, T0."Code" ASC, T0."Instance" ASC, T1."InstID" ASC ';
	END IF;
	
	execute immediate (:v_sql);
	--SELECT :V_SQL from dummy;
	--call TmSp_B1CFLGetDocuments (N'20020701',N'20120731',N'docview1',1,0,0,N''); 
	
END;


