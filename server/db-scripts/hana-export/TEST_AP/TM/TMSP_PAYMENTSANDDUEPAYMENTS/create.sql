-- B1 DEPENDS: AFTER:PT:PROCESS_END

CREATE PROCEDURE TmSp_PaymentsAndDuePayments (
	v_CurrentYear SMALLINT,
	v_FromBPCode NVARCHAR(15),
	v_ToBPCode NVARCHAR(15))
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
	v_PreviousYear SMALLINT;
	v_PreviousYearStart DATETIME;
	v_CurrentYearEnd DATETIME;
	v_vendorCode NVARCHAR(15);
	v_cnt integer;
	-- Create cursor to loop through all vendors
	-- from given range
	CURSOR vendorCursor FOR
	SELECT
		"CardCode" as CardCode
	FROM
		OCRD
	WHERE
		"CardType" = 'S'
		AND "CardCode" >= :v_FromBPCode
		AND "CardCode" <= :v_ToBPCode
	ORDER BY
		"CardCode";
BEGIN

	select count (*) into v_cnt from "PUBLIC"."M_TEMPORARY_TABLES" where TABLE_NAME = '#TBLPDPTEMPDATA';
   	if :v_cnt > 0 then
    	drop table #tblPDPTempData;
    end if;
	
	-- Create temporary table to store data to
	CREATE LOCAL TEMPORARY TABLE #tblPDPTempData
	(
		DocType nvarchar(20),
		DocAbs int,
		DocInstalmntID smallint,
		CardCode nvarchar(15),
		CardName nvarchar(100),
		DocumentDate datetime,
		DueDate datetime,
		PaymentDate datetime,
		DocTotal DECIMAL(21,6),
		OpenAmount DECIMAL(21,6),
		BoEStatus nvarchar(1),
		BoEAbs int,
		DaysOverdue int,
		YearFlag nvarchar(1),
		DueDateOverThreshold nvarchar(1),
		ReconciliationTotal DECIMAL(21,6),
		SubDocType nvarchar(20),
		SubDocAbs int,
		SubDocInstalmntID smallint,
		ReconcAbs int
	);

	delete from #tblPDPTempData where 1=1;

	-- Initialize local variables
	v_PreviousYear := :v_CurrentYear - 1;
	SELECT  TO_DATE(TO_VARCHAR(:v_PreviousYear) || TO_VARCHAR('0101'),'YYYYMMDD') INTO v_PreviousYearStart FROM DUMMY;
	SELECT  TO_DATE(TO_VARCHAR(:v_CurrentYear) || TO_VARCHAR('1231'),'YYYYMMDD') INTO v_CurrentYearEnd FROM DUMMY;

	-- Initialize cursor variable
	
	FOR cur_row as vendorCursor DO 

	-- Select PCH records
		INSERT INTO #tblPDPTempData
		SELECT
			T0."ObjType",
			T1."DocEntry",
			T1."InstlmntID",
			T0."CardCode",
			T0."CardName",
			T0."TaxDate",
			T1."DueDate",
			NULL,
			T1."InsTotal",
			(T1."InsTotal" - T1."PaidToDate"),
			NULL,
			NULL,
			0,
			N'N',
			N'N',
			(
				SELECT
					-ABS (Sum (Ix."ReconSum"))
				FROM
				(
					SELECT
						I2."ReconSum"
					FROM
						OITR I0
						INNER JOIN ITR1 I1 ON I1."ReconNum" = I0."ReconNum"
							AND I1."SrcObjTyp" = N'18'
						INNER JOIN ITR1 I2 ON I2."ReconNum" = I0."ReconNum"
							AND I2."SrcObjTyp" = N'46'
						INNER JOIN OVPM I3 ON I3."DocNum" = I2."SrcObjAbs" 
					WHERE
						I0."CancelAbs" = 0
						AND I0."ReconType" IN (0, 3)
						AND I3."TaxDate" > :v_CurrentYearEnd
						AND I1."SrcObjAbs" = T0."DocEntry"
						
					UNION SELECT
						I2."ReconSum"
					FROM
						OITR I0
						INNER JOIN ITR1 I1 ON I1."ReconNum" = I0."ReconNum"
							AND I1."SrcObjTyp" = N'18'
						INNER JOIN ITR1 I2 ON I2."ReconNum" = I0."ReconNum"
							AND I2."SrcObjTyp" = N'19'
						INNER JOIN ORPC I3 ON I3."DocEntry" = I2."SrcObjAbs" 
					WHERE
						I0."CancelAbs" = 0
						AND I0."ReconType" IN (0, 3)
						AND I3."TaxDate" > :v_CurrentYearEnd
						AND I1."SrcObjAbs" = T0."DocEntry"
						
					UNION SELECT
						I2."ReconSum"
					FROM
						OITR I0
						INNER JOIN ITR1 I1 ON I1."ReconNum" = I0."ReconNum"
							AND I1."SrcObjTyp" = N'18'
						INNER JOIN ITR1 I2 ON I2."ReconNum" = I0."ReconNum"
							AND I2."SrcObjTyp" = N'30'
						INNER JOIN OJDT I3 ON I3."TransId" = I2."SrcObjAbs"
						INNER JOIN JDT1 I4 ON I4."TransId" = I3."TransId" 
					WHERE
						I0."CancelAbs" = 0
						AND I0."ReconType" IN (0, 3)
						AND I3."TaxDate" > :v_CurrentYearEnd
						AND I4."Debit" <> 0
						AND I1."SrcObjAbs" = T0."DocEntry"
				) Ix
			),
			NULL,
			NULL,
			NULL,
			NULL
		FROM
			"OPCH" T0
			INNER JOIN "PCH6" T1 ON T1."DocEntry" = T0."DocEntry"
		WHERE
			(
				T0."TaxDate" >= :v_PreviousYearStart
				AND T0."TaxDate" <= :v_CurrentYearEnd
			)
			AND T0."CardCode" = cur_row.CardCode 
			AND T0."CANCELED" = 'N'
			AND T1."InsTotal" > 0
			
		-- Select payments for PCH records
		UNION SELECT
			T3."ObjType",
			T2."DocEntry",
			T2."InstlmntID",
			T3."CardCode",
			T3."CardName",
			T1."TaxDate",
			T1."DocDueDate",
			T1."TaxDate",
			T0."SumApplied",
			T0."SumApplied",
			T4."BoeStatus",
			T4."BoeKey",
			0,
			N'N',
			N'N',
			NULL,
			T1."ObjType",
			T1."DocEntry",
			-1,
			NULL
		FROM
			"VPM2" T0
			INNER JOIN "OVPM" T1 ON T1."DocNum" = T0."DocNum"
			INNER JOIN "PCH6" T2 ON T2."DocEntry" = T0."DocEntry"
				AND T2."InstlmntID" = T0."InstId"
				AND T0."InvType" = N'18'
			INNER JOIN "OPCH" T3 ON T3."DocEntry" = T2."DocEntry"
			LEFT OUTER JOIN "OBOE" T4 ON T4."BoeKey" = T1."BoeAbs"
		WHERE
			(
				T3."TaxDate" >= :v_PreviousYearStart
				AND T3."TaxDate" <= :v_CurrentYearEnd
				AND T1."TaxDate" <= :v_CurrentYearEnd
			)
			AND T3."CardCode" = cur_row.CardCode
			AND T3."CANCELED" = 'N'
			AND T2."InsTotal" > 0
			AND T1."Canceled" = 'N'
			
		-- Select credit memos for PCH records
		UNION SELECT
			T0."ObjType",
			T1."DocEntry",
			T1."InstlmntID",
			T5."CardCode",
			T5."CardName",
			T5."TaxDate",
			T6."DueDate",
			T5."TaxDate",
			T6."InsTotal",
			(T6."InsTotal" - T6."PaidToDate"),
			NULL,
			NULL,
			-10,
			N'N',
			N'N',
			NULL,
			T6."ObjType",
			T6."DocEntry",
			T6."InstlmntID",
			NULL
		FROM
			"OPCH" T0
			INNER JOIN "PCH6" T1 ON T1."DocEntry" = T0."DocEntry"
			INNER JOIN "OITR" T2 ON T2."ReconType" = N'4'
			INNER JOIN "ITR1" T3 ON T3."ReconNum" = T2."ReconNum"
				AND T3."SrcObjAbs" = T0."DocEntry"
				AND T3."SrcObjTyp" = N'18'
			INNER JOIN "ITR1" T4 ON T4."ReconNum" = T3."ReconNum"
				AND T4."SrcObjTyp" = N'19'
			INNER JOIN "ORPC" T5 ON T5."DocEntry" = T4."SrcObjAbs"
			INNER JOIN "RPC6" T6 ON T6."DocEntry" = T5."DocEntry"
		WHERE
			(
				T0."TaxDate" >= :v_PreviousYearStart
				AND T0."TaxDate" <= :v_CurrentYearEnd
				AND T5."TaxDate" <= :v_CurrentYearEnd
			)
			AND T0."CardCode" = cur_row.CardCode
			AND T0."CANCELED" = 'N'
			AND T1."InsTotal" > 0
			AND T2."CancelAbs" = 0
		
		-- Select reconciled payments for PCH records
		UNION SELECT
			T3."ObjType",
			T3."DocEntry",
			T4."InstlmntID",
			T3."CardCode",
			T3."CardName",
			T5."TaxDate",
			T5."DocDueDate",
			T5."TaxDate",
			T1."ReconSum",
			T1."ReconSum",
			T6."BoeStatus",
			T6."BoeKey",
			0,
			N'N',
			N'N',
			T2."ReconSum",
			T5."ObjType",
			T5."DocEntry",
			-1,
			T0."ReconNum"
		FROM
			"OITR" T0
			INNER JOIN "ITR1" T1 ON T1."ReconNum" = T0."ReconNum"
				AND T1."SrcObjTyp" = N'18'
			INNER JOIN "ITR1" T2 ON T2."ReconNum" = T0."ReconNum"
				AND T2."SrcObjTyp" = N'46'
			INNER JOIN "OPCH" T3 ON T3."DocEntry" = T1."SrcObjAbs"
			INNER JOIN "PCH6" T4 ON T4."DocEntry" = T3."DocEntry"
				AND T1."TransRowId" = T4."InstlmntID" - 1
			INNER JOIN "OVPM" T5 ON T5."DocEntry" = T2."SrcObjAbs"
			LEFT OUTER JOIN "OBOE" T6 ON T6."BoeKey" = T5."BoeAbs"
		WHERE
			(
				T3."TaxDate" >= :v_PreviousYearStart
				AND T3."TaxDate" <= :v_CurrentYearEnd
				AND T5."TaxDate" <= :v_CurrentYearEnd
			)
			AND T3."CardCode" = cur_row.CardCode
			AND T5."Canceled" = 'N'
			AND T0."CancelAbs" = 0
			AND T0."ReconType" = N'0'
		
		-- Select reconciled credit memos for PCH records
		UNION SELECT
			T3."ObjType",
			T3."DocEntry",
			T4."InstlmntID",
			T3."CardCode",
			T3."CardName",
			T5."TaxDate",
			T6."DueDate",
			T5."TaxDate",
			T1."ReconSum",
			T1."ReconSum",
			NULL,
			NULL,
			-10,
			N'N',
			N'N',
			T2."ReconSum",
			T6."ObjType",
			T6."DocEntry",
			T6."InstlmntID",
			T0."ReconNum"
		FROM
			"OITR" T0
			INNER JOIN "ITR1" T1 ON T1."ReconNum" = T0."ReconNum"
				AND T1."SrcObjTyp" = N'18'
			INNER JOIN "ITR1" T2 ON T2."ReconNum" = T0."ReconNum"
				AND T2."SrcObjTyp" = N'19'
			INNER JOIN "OPCH" T3 ON T3."DocEntry" = T1."SrcObjAbs"
			INNER JOIN "PCH6" T4 ON T4."DocEntry" = T3."DocEntry"
			INNER JOIN "ORPC" T5 ON T5."DocEntry" = T2."SrcObjAbs"
			INNER JOIN "RPC6" T6 ON T6."DocEntry" = T5."DocEntry"
		WHERE
			(
				T3."TaxDate" >= :v_PreviousYearStart
				AND T3."TaxDate" <= :v_CurrentYearEnd
				AND T5."TaxDate" <= :v_CurrentYearEnd
			)
			AND T3."CardCode" = cur_row.CardCode
			AND T5."CANCELED" = 'N'
			AND T0."CancelAbs" = 0
			AND T0."ReconType" = N'0'
			
		-- Select reconciled journal entries (CM type) for PCH records
		UNION SELECT
			T0."ObjType",
			T1."DocEntry",
			T1."InstlmntID",
			T7."CardCode",
			T7."CardName",
			T5."TaxDate",
			T6."DueDate",
			T5."TaxDate",
			T3."ReconSum",
			T3."ReconSum",
			NULL,
			NULL,
			-10,
			N'N',
			N'N',
			T4."ReconSum",
			T6."ObjType",
			T6."TransId",
			T6."Line_ID" + 1,
			T2."ReconNum"
		FROM
			"OPCH" T0
			INNER JOIN "PCH6" T1 ON T1."DocEntry" = T0."DocEntry"
			INNER JOIN "OITR" T2 ON T2."ReconType" = N'0'
			INNER JOIN "ITR1" T3 ON T3."ReconNum" = T2."ReconNum"
				AND T3."SrcObjAbs" = T0."DocEntry"
				AND T3."SrcObjTyp" = N'18'
			INNER JOIN "ITR1" T4 ON T4."ReconNum" = T3."ReconNum"
				AND T4."SrcObjTyp" = N'30'
			INNER JOIN "OJDT" T5 ON T5."TransId" = T4."SrcObjAbs"
			INNER JOIN "JDT1" T6 ON T6."TransId" = T5."TransId"
			INNER JOIN "OCRD" T7 ON T7."CardCode" = T6."ShortName"
		WHERE
			(
				T0."TaxDate" >= :v_PreviousYearStart
				AND T0."TaxDate" <= :v_CurrentYearEnd
				AND T5."TaxDate" <= :v_CurrentYearEnd
			)
			AND T0."CardCode" = cur_row.CardCode
			AND T0."CANCELED" = 'N'
			AND T1."InsTotal" > 0
			AND T6."Debit" <> 0
			AND T2."CancelAbs" = 0
		
		-- Select DPO records
		UNION SELECT
			T0."ObjType",
			T1."DocEntry",
			T1."InstlmntID",
			T0."CardCode",
			T0."CardName",
			T0."TaxDate",
			T1."DueDate",
			NULL,
			T1."InsTotal",
			(T1."InsTotal" - T1."PaidToDate"),
			NULL,
			NULL,
			0,
			N'N',
			N'N',
			(
				SELECT
					-ABS (Sum (Ix."ReconSum"))
				FROM
				(
					SELECT
						I2."ReconSum"
					FROM
						OITR I0
						INNER JOIN ITR1 I1 ON I1."ReconNum" = I0."ReconNum"
							AND I1."SrcObjTyp" = N'204'
						INNER JOIN ITR1 I2 ON I2."ReconNum" = I0."ReconNum"
							AND I2."SrcObjTyp" = N'46'
						INNER JOIN OVPM I3 ON I3."DocNum" = I2."SrcObjAbs" 
					WHERE
						I0."CancelAbs" = 0
						AND I0."ReconType" IN (0, 3)
						AND I3."TaxDate" > :v_CurrentYearEnd
						AND I1."SrcObjAbs" = T0."DocEntry"
						
					UNION SELECT
						I2."ReconSum"
					FROM
						OITR I0
						INNER JOIN ITR1 I1 ON I1."ReconNum" = I0."ReconNum"
							AND I1."SrcObjTyp" = N'204'
						INNER JOIN ITR1 I2 ON I2."ReconNum" = I0."ReconNum"
							AND I2."SrcObjTyp" = N'19'
						INNER JOIN ORPC I3 ON I3."DocEntry" = I2."SrcObjAbs" 
					WHERE
						I0."CancelAbs" = 0
						AND I0."ReconType" IN (0, 3)
						AND I3."TaxDate" > :v_CurrentYearEnd
						AND I1."SrcObjAbs" = T0."DocEntry"
						
					UNION SELECT
						I2."ReconSum"
					FROM
						OITR I0
						INNER JOIN ITR1 I1 ON I1."ReconNum" = I0."ReconNum"
							AND I1."SrcObjTyp" = N'204'
						INNER JOIN ITR1 I2 ON I2."ReconNum" = I0."ReconNum"
							AND I2."SrcObjTyp" = N'30'
						INNER JOIN OJDT I3 ON I3."TransId" = I2."SrcObjAbs"
						INNER JOIN JDT1 I4 ON I4."TransId" = I3."TransId" 
					WHERE
						I0."CancelAbs" = 0
						AND I0."ReconType" IN (0, 3)
						AND I3."TaxDate" > :v_CurrentYearEnd
						AND I4."Debit" <> 0
						AND I1."SrcObjAbs" = T0."DocEntry"
				) Ix
			),
			NULL,
			NULL,
			NULL,
			NULL
		FROM
			"ODPO" T0
			INNER JOIN "DPO6" T1 ON T1."DocEntry" = T0."DocEntry"
		WHERE
			(
				T0."TaxDate" >= :v_PreviousYearStart
				AND T0."TaxDate" <= :v_CurrentYearEnd
			)
			AND T0."CardCode" = cur_row.CardCode
			AND T0."CANCELED" = 'N'
			AND T1."InsTotal" > 0
		
		-- Select payments for DPO records
		UNION SELECT
			T3."ObjType",
			T2."DocEntry",
			T2."InstlmntID",
			T3."CardCode",
			T3."CardName",
			T3."TaxDate",
			T2."DueDate",
			T1."TaxDate",
			T0."SumApplied",
			T0."SumApplied",
			T4."BoeStatus",
			T4."BoeKey",
			0,
			N'N',
			N'N',
			NULL,
			T1."ObjType",
			T1."DocEntry",
			-1,
			NULL
		FROM
			"VPM2" T0
			INNER JOIN "OVPM" T1 ON T1."DocNum" = T0."DocNum"
			INNER JOIN "DPO6" T2 ON T2."DocEntry" = T0."DocEntry"
				AND T2."InstlmntID" = T0."InstId"
				AND T0."InvType" = N'204'
			INNER JOIN "ODPO" T3 ON T3."DocEntry" = T2."DocEntry"
			LEFT OUTER JOIN "OBOE" T4 ON T4."BoeKey" = T1."BoeAbs"
		WHERE
			(
				T3."TaxDate" >= :v_PreviousYearStart
				AND T3."TaxDate" <= :v_CurrentYearEnd
				AND T1."TaxDate" <= :v_CurrentYearEnd
			)
			AND T3."CardCode" = cur_row.CardCode
			AND T3."CANCELED" = 'N'
			AND T2."InsTotal" > 0
			AND T1."Canceled" = 'N'
		
		-- Select credit memos for DPO records
		UNION SELECT
			T0."ObjType",
			T1."DocEntry",
			T1."InstlmntID",
			T5."CardCode",
			T5."CardName",
			T5."TaxDate",
			T6."DueDate",
			T5."TaxDate",
			T6."InsTotal",
			(T6."InsTotal" - T6."PaidToDate"),
			NULL,
			NULL,
			-10,
			N'N',
			N'N',
			NULL,
			T6."ObjType",
			T6."DocEntry",
			T6."InstlmntID",
			NULL
		FROM
			"ODPO" T0
			INNER JOIN "DPO6" T1 ON T1."DocEntry" = T0."DocEntry"
			INNER JOIN "OITR" T2 ON T2."ReconType" = N'4'
			INNER JOIN "ITR1" T3 ON T3."ReconNum" = T2."ReconNum"
				AND T3."SrcObjAbs" = T0."DocEntry"
				AND T3."SrcObjTyp" = N'204'
			INNER JOIN "ITR1" T4 ON T4."ReconNum" = T3."ReconNum"
				AND T4."SrcObjTyp" = N'19'
			INNER JOIN "ORPC" T5 ON T5."DocEntry" = T4."SrcObjAbs"
			INNER JOIN "RPC6" T6 ON T6."DocEntry" = T5."DocEntry"
		WHERE
			(
				T0."TaxDate" >= :v_PreviousYearStart
				AND T0."TaxDate" <= :v_CurrentYearEnd
				AND T5."TaxDate" <= :v_CurrentYearEnd
			)
			AND T0."CardCode" = cur_row.CardCode
			AND T0."CANCELED" = 'N'
			AND T1."InsTotal" > 0
			AND T2."CancelAbs" = 0
		
		-- Select reconciled payments for DPO records
		UNION SELECT
			T3."ObjType",
			T3."DocEntry",
			T4."InstlmntID",
			T3."CardCode",
			T3."CardName",
			T5."TaxDate",
			T5."DocDueDate",
			T5."TaxDate",
			T1."ReconSum",
			T1."ReconSum",
			T6."BoeStatus",
			T6."BoeKey",
			0,
			N'N',
			N'N',
			T2."ReconSum",
			T5."ObjType",
			T5."DocEntry",
			-1,
			T0."ReconNum"
		FROM
			"OITR" T0
			INNER JOIN "ITR1" T1 ON T1."ReconNum" = T0."ReconNum"
				AND T1."SrcObjTyp" = N'204'
			INNER JOIN "ITR1" T2 ON T2."ReconNum" = T0."ReconNum"
				AND T2."SrcObjTyp" = N'46'
			INNER JOIN "ODPO" T3 ON T3."DocEntry" = T1."SrcObjAbs"
			INNER JOIN "DPO6" T4 ON T4."DocEntry" = T3."DocEntry"
				AND T1."TransRowId" = T4."InstlmntID" - 1
			INNER JOIN "OVPM" T5 ON T5."DocEntry" = T2."SrcObjAbs"
			LEFT OUTER JOIN "OBOE" T6 ON T6."BoeKey" = T5."BoeAbs"
		WHERE
			(
				T3."TaxDate" >= :v_PreviousYearStart
				AND T3."TaxDate" <= :v_CurrentYearEnd
				AND T5."TaxDate" <= :v_CurrentYearEnd
			)
			AND T3."CardCode" = cur_row.CardCode
			AND T5."Canceled" = 'N'
			AND T0."CancelAbs" = 0
			AND T0."ReconType" = N'0'
		
		-- Select reconciled credit memos for DPO records
		UNION SELECT
			T3."ObjType",
			T3."DocEntry",
			T4."InstlmntID",
			T3."CardCode",
			T3."CardName",
			T5."TaxDate",
			T6."DueDate",
			T5."TaxDate",
			T1."ReconSum",
			T1."ReconSum",
			NULL,
			NULL,
			-10,
			N'N',
			N'N',
			T2."ReconSum",
			T6."ObjType",
			T6."DocEntry",
			T6."InstlmntID",
			T0."ReconNum"
		FROM
			"OITR" T0
			INNER JOIN "ITR1" T1 ON T1."ReconNum" = T0."ReconNum"
				AND T1."SrcObjTyp" = N'204'
			INNER JOIN "ITR1" T2 ON T2."ReconNum" = T0."ReconNum"
				AND T2."SrcObjTyp" = N'19'
			INNER JOIN "ODPO" T3 ON T3."DocEntry" = T1."SrcObjAbs"
			INNER JOIN "DPO6" T4 ON T4."DocEntry" = T3."DocEntry"
			INNER JOIN "ORPC" T5 ON T5."DocEntry" = T2."SrcObjAbs"
			INNER JOIN "RPC6" T6 ON T6."DocEntry" = T5."DocEntry"
		WHERE
			(
				T3."TaxDate" >= :v_PreviousYearStart
				AND T3."TaxDate" <= :v_CurrentYearEnd
				AND T5."TaxDate" <= :v_CurrentYearEnd
			)
			AND T3."CardCode" = cur_row.CardCode
			AND T5."CANCELED" = 'N'
			AND T0."CancelAbs" = 0
			AND T0."ReconType" = N'0'
		
		-- Select reconciled journal entries (CM type) for DPO records
		UNION SELECT
			T0."ObjType",
			T1."DocEntry",
			T1."InstlmntID",
			T7."CardCode",
			T7."CardName",
			T5."TaxDate",
			T6."DueDate",
			T5."TaxDate",
			T3."ReconSum",
			T3."ReconSum",
			NULL,
			NULL,
			-10,
			N'N',
			N'N',
			T4."ReconSum",
			T6."ObjType",
			T6."TransId",
			T6."Line_ID" + 1,
			T2."ReconNum"
		FROM
			"ODPO" T0
			INNER JOIN "DPO6" T1 ON T1."DocEntry" = T0."DocEntry"
			INNER JOIN "OITR" T2 ON T2."ReconType" = N'0'
			INNER JOIN "ITR1" T3 ON T3."ReconNum" = T2."ReconNum"
				AND T3."SrcObjAbs" = T0."DocEntry"
				AND T3."SrcObjTyp" = N'204'
			INNER JOIN "ITR1" T4 ON T4."ReconNum" = T3."ReconNum"
				AND T4."SrcObjTyp" = N'30'
			INNER JOIN "OJDT" T5 ON T5."TransId" = T4."SrcObjAbs"
			INNER JOIN "JDT1" T6 ON T6."TransId" = T5."TransId"
			INNER JOIN "OCRD" T7 ON T7."CardCode" = T6."ShortName"
		WHERE
			(
				T0."TaxDate" >= :v_PreviousYearStart
				AND T0."TaxDate" <= :v_CurrentYearEnd
				AND T5."TaxDate" <= :v_CurrentYearEnd
			)
			AND T0."CardCode" = cur_row.CardCode
			AND T0."CANCELED" = 'N'
			AND T1."InsTotal" > 0
			AND T6."Debit" <> 0
			AND T2."CancelAbs" = 0

		-- Select JE records (invoice type)
		UNION SELECT
			T0."ObjType",
			T1."TransId",
			T1."Line_ID" + 1,
			T2."CardCode",
			T2."CardName",
			T0."TaxDate",
			T1."DueDate",
			NULL,
			T1."Credit",
			T1."BalDueCred",
			NULL,
			NULL,
			0,
			N'N',
			N'N',
			(
				SELECT
					-ABS (Sum (Ix."ReconSum"))
				FROM
				(
					SELECT
						I2."ReconSum"
					FROM
						OITR I0
						INNER JOIN ITR1 I1 ON I1."ReconNum" = I0."ReconNum"
							AND I1."SrcObjTyp" = N'30'
						INNER JOIN ITR1 I2 ON I2."ReconNum" = I0."ReconNum"
							AND I2."SrcObjTyp" = N'46'
						INNER JOIN OVPM I3 ON I3."DocNum" = I2."SrcObjAbs" 
					WHERE
						I0."CancelAbs" = 0
						AND I0."ReconType" IN (0, 3)
						AND I3."TaxDate" > :v_CurrentYearEnd
						AND I1."SrcObjAbs" = T0."TransId"
						
					UNION SELECT
						I2."ReconSum"
					FROM
						OITR I0
						INNER JOIN ITR1 I1 ON I1."ReconNum" = I0."ReconNum"
							AND I1."SrcObjTyp" = N'30'
						INNER JOIN ITR1 I2 ON I2."ReconNum" = I0."ReconNum"
							AND I2."SrcObjTyp" = N'19'
						INNER JOIN ORPC I3 ON I3."DocEntry" = I2."SrcObjAbs" 
					WHERE
						I0."CancelAbs" = 0
						AND I0."ReconType" IN (0, 3)
						AND I3."TaxDate" > :v_CurrentYearEnd
						AND I1."SrcObjAbs" = T0."TransId"
						
					UNION SELECT
						I2."ReconSum"
					FROM
						OITR I0
						INNER JOIN ITR1 I1 ON I1."ReconNum" = I0."ReconNum"
							AND I1."SrcObjTyp" = N'30'
						INNER JOIN ITR1 I2 ON I2."ReconNum" = I0."ReconNum"
							AND I2."SrcObjTyp" = N'30'
						INNER JOIN OJDT I3 ON I3."TransId" = I2."SrcObjAbs"
						INNER JOIN JDT1 I4 ON I4."TransId" = I3."TransId" 
					WHERE
						I0."CancelAbs" = 0
						AND I0."ReconType" IN (0, 3)
						AND I3."TaxDate" > :v_CurrentYearEnd
						AND I4."Debit" <> 0
						AND I1."SrcObjAbs" = T0."TransId"
						AND I2."SrcObjAbs" <> T0."TransId"
				) Ix
			),
			NULL,
			NULL,
			NULL,
			NULL
		FROM
			"OJDT" T0
			INNER JOIN "JDT1" T1 ON T1."TransId" = T0."TransId"
			INNER JOIN "OCRD" T2 ON T2."CardCode" = T1."ShortName"
		WHERE
			(
				T0."TaxDate" >= :v_PreviousYearStart
				AND T0."TaxDate" <= :v_CurrentYearEnd
			)
			AND T1."ShortName" = cur_row.CardCode
			AND T0."TransType" IN (N'30',N'-2',N'-3')
			AND T1."Credit" <> 0
			AND T0."StornoToTr" IS NULL
			AND NOT EXISTS (
				SELECT
					T0."TransId"
				FROM
					"OJDT" U0
				WHERE
					U0."StornoToTr" = T0."TransId"
			)
		
		-- Select payments for JE records
		UNION SELECT
			T3."ObjType",
			T2."TransId",
			T2."Line_ID" + 1,
			T4."CardCode",
			T4."CardName",
			T1."TaxDate",
			T1."DocDueDate",
			T1."TaxDate",
			T0."SumApplied",
			T0."SumApplied",
			T5."BoeStatus",
			T5."BoeKey",
			0,
			N'N',
			N'N',
			NULL,
			T1."ObjType",
			T1."DocEntry",
			-1,
			NULL
		FROM
			"VPM2" T0
			INNER JOIN "OVPM" T1 ON T1."DocNum" = T0."DocNum"
			INNER JOIN "JDT1" T2 ON T2."TransId" = T0."DocEntry"
				AND T0."InvType" = N'30'
			INNER JOIN "OJDT" T3 ON T3."TransId" = T2."TransId"
			INNER JOIN "OCRD" T4 ON T4."CardCode" = T2."ShortName"
			LEFT OUTER JOIN "OBOE" T5 ON T5."BoeKey" = T1."BoeAbs"
		WHERE
			(
				T3."TaxDate" >= :v_PreviousYearStart
				AND T3."TaxDate" <= :v_CurrentYearEnd
				AND T1."TaxDate" <= :v_CurrentYearEnd
			)
			AND T2."ShortName" = cur_row.CardCode
			AND T3."TransType" IN (N'30',N'-2',N'-3')
			AND T2."Credit" <> 0
			AND T3."StornoToTr" IS NULL
			AND NOT EXISTS (
				SELECT
					T0."DocEntry"
				FROM
					"OJDT" U0
				WHERE
					U0."StornoToTr" = T3."TransId"
			)
			AND T1."Canceled" = 'N'
		
		-- Select reconciled payments for JE records
		UNION SELECT
			T3."ObjType",
			T4."TransId",
			T4."Line_ID" + 1,
			T7."CardCode",
			T7."CardName",
			T5."TaxDate",
			T5."DocDueDate",
			T5."TaxDate",
			T1."ReconSum",
			T1."ReconSum",
			T6."BoeStatus",
			T6."BoeKey",
			0,
			N'N',
			N'N',
			T2."ReconSum",
			T5."ObjType",
			T5."DocEntry",
			-1,
			T0."ReconNum"
		FROM
			"OITR" T0
			INNER JOIN "ITR1" T1 ON T1."ReconNum" = T0."ReconNum"
				AND T1."SrcObjTyp" = N'30'
			INNER JOIN "ITR1" T2 ON T2."ReconNum" = T0."ReconNum"
				AND T2."SrcObjTyp" = N'46'
			INNER JOIN "OJDT" T3 ON T3."TransId" = T1."SrcObjAbs"
			INNER JOIN "JDT1" T4 ON T4."TransId" = T3."TransId"
				AND T1."TransRowId" = T4."Line_ID"
			INNER JOIN "OVPM" T5 ON T5."DocEntry" = T2."SrcObjAbs"
			LEFT OUTER JOIN "OBOE" T6 ON T6."BoeKey" = T5."BoeAbs"
			INNER JOIN "OCRD" T7 ON T7."CardCode" = T1."ShortName"
		WHERE
			(
				T3."TaxDate" >= :v_PreviousYearStart
				AND T3."TaxDate" <= :v_CurrentYearEnd
				AND T5."TaxDate" <= :v_CurrentYearEnd
			)
			AND T4."ShortName" = cur_row.CardCode
			AND T4."Credit" <> 0
			AND T3."StornoToTr" IS NULL
			AND NOT EXISTS (
				SELECT
					T0."ReconNum"
				FROM
					"OJDT" U0
				WHERE
					U0."StornoToTr" = T3."TransId"
			)
			AND T5."Canceled" = 'N'
			AND T0."CancelAbs" = 0
			AND T0."ReconType" = N'0'
		
		-- Select reconciled credit memos for JE records
		UNION SELECT
			T3."ObjType",
			T4."TransId",
			T4."Line_ID" + 1,
			T7."CardCode",
			T7."CardName",
			T5."TaxDate",
			T5."DocDueDate",
			T5."TaxDate",
			T1."ReconSum",
			T1."ReconSum",
			NULL,
			NULL,
			-10,
			N'N',
			N'N',
			T2."ReconSum",
			T6."ObjType",
			T6."DocEntry",
			T6."InstlmntID",
			T0."ReconNum"
		FROM
			"OITR" T0
			INNER JOIN "ITR1" T1 ON T1."ReconNum" = T0."ReconNum"
				AND T1."SrcObjTyp" = N'30'
			INNER JOIN "ITR1" T2 ON T2."ReconNum" = T0."ReconNum"
				AND T2."SrcObjTyp" = N'19'
			INNER JOIN "OJDT" T3 ON T3."TransId" = T1."SrcObjAbs"
			INNER JOIN "JDT1" T4 ON T4."TransId" = T3."TransId"
			INNER JOIN "ORPC" T5 ON T5."DocEntry" = T2."SrcObjAbs"
			INNER JOIN "RPC6" T6 ON T6."DocEntry" = T6."DocEntry"
			INNER JOIN "OCRD" T7 ON T7."CardCode" = T1."ShortName"
		WHERE
			(
				T3."TaxDate" >= :v_PreviousYearStart
				AND T3."TaxDate" <= :v_CurrentYearEnd
				AND T5."TaxDate" <= :v_CurrentYearEnd
			)
			AND T4."ShortName" = cur_row.CardCode
			AND T4."Credit" <> 0
			AND T3."StornoToTr" IS NULL
			AND NOT EXISTS (
				SELECT
					T0."ReconNum"
				FROM
					"OJDT" U0
				WHERE
					U0."StornoToTr" = T3."TransId"
			)
			AND T5."CANCELED" = 'N'
			AND T0."CancelAbs" = 0
			AND T0."ReconType" = N'0'
		
		-- Select reconciled journal entries (CM type) for JE (invoice type) records
		UNION SELECT
			T0."ObjType",
			T1."TransId",
			T1."Line_ID" + 1,
			T7."CardCode",
			T7."CardName",
			T5."TaxDate",
			T6."DueDate",
			T5."TaxDate",
			T3."ReconSum",
			T3."ReconSum",
			NULL,
			NULL,
			-10,
			N'N',
			N'N',
			T4."ReconSum",
			T6."ObjType",
			T6."TransId",
			T6."Line_ID" + 1,
			T2."ReconNum"
		FROM
			"OJDT" T0
			INNER JOIN "JDT1" T1 ON T1."TransId" = T0."TransId"
				AND T1."Credit" <> 0
			INNER JOIN "OITR" T2 ON T2."ReconType" = N'0'
			INNER JOIN "ITR1" T3 ON T3."ReconNum" = T2."ReconNum"
				AND T3."SrcObjAbs" = T0."TransId"
				AND T3."SrcObjTyp" = N'30'
			INNER JOIN "ITR1" T4 ON T4."ReconNum" = T3."ReconNum"
				AND T4."SrcObjTyp" = N'30'
			INNER JOIN "OJDT" T5 ON T5."TransId" = T4."SrcObjAbs"
			INNER JOIN "JDT1" T6 ON T6."TransId" = T5."TransId"
			INNER JOIN "OCRD" T7 ON T7."CardCode" = T6."ShortName"
				AND T7."CardCode" = T1."ShortName"
		WHERE
			(
				T0."TaxDate" >= :v_PreviousYearStart
				AND T0."TaxDate" <= :v_CurrentYearEnd
				AND T5."TaxDate" <= :v_CurrentYearEnd
			)
			AND T7."CardCode" = cur_row.CardCode
			AND T6."Debit" <> 0
			AND T2."CancelAbs" = 0
			
		-- Select non-based payments (they may be paid by BoE
		-- and may be partially reconciled)
		UNION SELECT
			T0."ObjType",
			T0."DocEntry",
			-1,
			T0."CardCode",
			T0."CardName",
			T0."TaxDate",
			T0."DocDueDate",
			T0."TaxDate",
			T0."NoDocSum",
			T0."NoDocSum",
			(
				SELECT
					"BoeStatus"
				FROM
					OBOE B0
				WHERE
					B0."BoeKey" = T0."BoeAbs"
			),
			T0."BoeAbs",
			0,
			'N',
			'N',
			(
				SELECT
					Sum (I1."ReconSum")
				FROM
					OITR I0
					INNER JOIN ITR1 I1 ON I1."ReconNum" = I0."ReconNum"
				WHERE
					I0."CancelAbs" = 0
					AND I0."ReconType" IN (0, 3)
					AND I1."SrcObjTyp" = 46
					AND I1."SrcObjAbs" = T0."DocEntry"
			),
			T0."ObjType",
			T0."DocEntry",
			-1,
			NULL
		FROM
			OVPM T0
		WHERE
			(
				T0."TaxDate" >= :v_PreviousYearStart
				AND T0."TaxDate" <= :v_CurrentYearEnd
			)
			AND T0."CardCode" = cur_row.CardCode
			AND T0."NoDocSum" > 0
		
		-- Select non-based credit memos
		UNION SELECT
			T0."ObjType",
			T1."DocEntry",
			-1,
			T0."CardCode",
			T0."CardName",
			T0."TaxDate",
			T1."DueDate",
			NULL,
			T1."InsTotal",
			(T1."InsTotal" - T1."PaidToDate"),
			NULL,
			NULL,
			-10,
			N'N',
			N'N',
			NULL,
			NULL,
			NULL,
			NULL,
			NULL 
		FROM
			"ORPC" T0
			INNER JOIN "RPC6" T1 ON T1."DocEntry" = T0."DocEntry"
			LEFT OUTER JOIN "ITR1" T2 ON T2."SrcObjAbs" = T0."DocEntry"
				AND T2."SrcObjTyp" = N'19'
			LEFT OUTER JOIN "OITR" T3 ON T3."ReconNum" = T2."ReconNum"
		WHERE
			(
				T0."TaxDate" >= :v_PreviousYearStart
				AND T0."TaxDate" <= :v_CurrentYearEnd
			)
			AND T0."CardCode" = cur_row.CardCode
			AND T1."InsTotal" > 0
			AND T1."InsTotal" > T1."PaidToDate"
			AND
			(
				T3."ReconType" IS NULL
				OR T3."ReconType" <> N'4'
			)

		-- Select JE records (credit memo type)
		UNION SELECT
			T0."ObjType",
			T1."TransId",
			T1."Line_ID" + 1,
			T2."CardCode",
			T2."CardName",
			T0."TaxDate",
			T1."DueDate",
			NULL,
			T1."Debit",
			T1."BalDueDeb",
			NULL,
			NULL,
			-10,
			N'N',
			N'N',
			NULL,
			NULL,
			NULL,
			NULL,
			NULL
		FROM
			"OJDT" T0
			INNER JOIN "JDT1" T1 ON T1."TransId" = T0."TransId"
			INNER JOIN "OCRD" T2 ON T2."CardCode" = T1."ShortName"
		WHERE
			(
				T0."TaxDate" >= :v_PreviousYearStart
				AND T0."TaxDate" <= :v_CurrentYearEnd
			)
			AND T1."ShortName" = cur_row.CardCode
			AND T0."TransType" IN (N'30',N'-2',N'-3')
			AND T1."Debit" <> 0
			AND T1."BalDueDeb" > 0
			AND T0."StornoToTr" IS NULL
			AND NOT EXISTS (
				SELECT
					T0."TransId"
				FROM
					"OJDT" U0
				WHERE
					U0."StornoToTr" = T0."TransId"
			);
	
	END FOR; -- Close cursor


	-- Present collected data
	SELECT
		DocType AS "Doc. No. (Type)", -- 1
		DocAbs AS "Doc. No. (Doc. number)", -- 2
		DocInstalmntID AS "Doc. No. (Installment ID)", -- 3
		CardCode AS "Card Code", -- 4
		CardName AS "Card Name", -- 5
		DocumentDate AS "Document Date", -- 6
		DueDate AS "Due Date", -- 7
		PaymentDate AS "Payment Date", -- 8
		DocTotal AS "Document Total", -- 9
		OpenAmount AS "Open Amount", -- 10
		BoEStatus AS "BoE Status", -- 11
		BoEAbs AS "BoE Key", -- 12
		DaysOverdue AS "Days Overdue", -- 13
		YearFlag AS "Year Flag", -- 14
		DueDateOverThreshold AS "Due Date is Over Threshold", -- 15
		ReconciliationTotal AS "Reconciliation Total", -- 16
		SubDocType AS "Sub Document Type", -- 17
		SubDocAbs AS "Sub Document Number", -- 18
		SubDocInstalmntID AS "Sub Document Installment", --19
		ReconcAbs AS "Reconciliation Number" --20
	FROM
		#tblPDPTempData
	ORDER BY
		1, 2, 3, 17, 18, 19, 20;

	-- Close table
	--DROP TABLE #tblPDPTempData;
END;


