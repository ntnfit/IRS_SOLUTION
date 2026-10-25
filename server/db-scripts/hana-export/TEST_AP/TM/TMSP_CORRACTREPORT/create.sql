-- B1 DEPENDS: AFTER:PT:PROCESS_END

CREATE PROCEDURE TmSp_CorrActReport(IN account NVARCHAR(255),
									IN	dateFrom NVARCHAR(8),
									IN	dateTo NVARCHAR(8))
										
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
BEGIN
	SELECT "Account" AS "Acct",
		SUM("Debit") AS "DebitLC",
		SUM("Credit") AS "CreditLC",
		SUM("SYSDeb") AS "DebitSC",
		SUM("SYSCred") AS "CreditSC",
		0 AS "Sort"
	FROM JDT1 AS Lines, OJDT AS Header
	WHERE Header."TransId" = Lines."TransId"
	AND "Account" = :account
	AND Header."RefDate" < TO_TIMESTAMP(:dateFrom)
	GROUP BY "Account"
	
	UNION ALL

	SELECT :account AS "Acct", 0 AS "DebitLC", 0 AS "CreditLC", 0 AS "DebitSC", 0 AS "CreditSC", 0 AS "Sort" FROM DUMMY
	WHERE NOT EXISTS
	(
		SELECT 1
		FROM JDT1 AS Lines, OJDT AS Header
		WHERE Header."TransId" = Lines."TransId"
		AND "Account" = :account
		AND Header."RefDate" < TO_TIMESTAMP(:dateFrom)
	)
		
	UNION ALL

	SELECT "Acct",
		SUM("CreditLC") AS "DebitLC",
		SUM("DebitLC") AS "CreditLC",
		SUM("CreditSC") AS "DebitSC",
		SUM("DebitSC") AS "CreditSC",
		1 AS "Sort"
	FROM
	(
		SELECT Lines."Account" AS "Acct",
			SUM("Debit") AS "DebitLC",
			SUM("Credit") AS "CreditLC",
			SUM("SYSDeb") AS "DebitSC",
			SUM("SYSCred") AS "CreditSC"
		FROM JDT1 AS Lines, OJDT AS Header
		WHERE (
			"ContraAct" IN
			(
				SELECT "CardCode"
				FROM OCRD
				WHERE "DebPayAcct" = :account

				UNION ALL

				SELECT "CardCode"
				FROM CRD3
				WHERE "AcctCode" = :account
			)
			OR "ContraAct" = :account
		) AND :account IN
		(
			SELECT "Account"
			FROM JDT1
			WHERE JDT1."TransId" = Header."TransId"
		)
		AND "Account" <> :account
		AND Lines."TransId" = Header."TransId"
		GROUP BY "Account", Header."RefDate"
		HAVING Header."RefDate"
		BETWEEN TO_TIMESTAMP(:dateFrom) 
		AND TO_TIMESTAMP(:dateTo)
		
		UNION ALL
	
-- Debit and credit are swapped intentionally
		SELECT Lines1."Account" AS "Acct",
			SUM(Lines2."Credit") AS "DebitLC",
			SUM(Lines2."Debit") AS "CreditLC",
			SUM(Lines2."SYSCred") AS "DebitSC",
			SUM(Lines2."SYSDeb") AS "CreditSC"
		FROM JDT1 AS Lines1, JDT1 AS Lines2, OJDT AS Header
		WHERE Lines1."TransId" = Lines2."TransId"
		AND Lines1."TransId" = Header."TransId"
		AND Lines2."Account" = :account
		AND Lines2."ContraAct" = Lines1."ShortName"
		AND (Lines1."ContraAct" IS NULL or Lines1."ContraAct" = '')
		AND ((Lines2."Credit" < Lines1."Debit" AND Lines2."Credit" > 0) OR (Lines2."Credit" > Lines1."Debit" AND Lines2."Credit" < 0) OR
			(Lines2."Debit" < Lines1."Credit" AND Lines2."Debit" > 0) OR (Lines2."Debit" > Lines1."Credit" AND Lines2."Debit" < 0))
		AND Header."RefDate"
			BETWEEN TO_TIMESTAMP(:dateFrom) 
			AND TO_TIMESTAMP(:dateTo)
		GROUP BY Lines1."Account"
	
		UNION ALL
	
		SELECT Lines1."Account" AS "Acct",
			SUM(Lines1."Debit") AS "DebitLC",
			SUM(Lines1."Credit") AS "CreditLC",
			SUM(Lines1."SYSDeb") AS "DebitSC",
			SUM(Lines1."SYSCred") AS "CreditSC"
		FROM JDT1 AS Lines1, JDT1 AS Lines2, OJDT AS Header
		WHERE Lines1."TransId" = Lines2."TransId"
		AND Lines1."TransId" = Header."TransId"
		AND Lines2."Account" = :account
		AND Lines2."ContraAct" = Lines1."ShortName"
		AND (Lines1."ContraAct" IS NULL or Lines1."ContraAct" = '')
		AND ((Lines2."Credit" > Lines1."Debit" AND Lines2."Credit" > 0) OR (Lines2."Credit" < Lines1."Debit" AND Lines2."Credit" < 0) OR
			(Lines2."Debit" > Lines1."Credit" AND Lines2."Debit" > 0) OR (Lines2."Debit" < Lines1."Credit" AND Lines2."Debit" < 0))
		AND Header."RefDate"
			BETWEEN TO_TIMESTAMP(:dateFrom) 
			AND TO_TIMESTAMP(:dateTo)
		GROUP BY Lines1."Account"
		
		UNION ALL

-- Debit and credit are swapped intentionally
		SELECT Lines1."Account" AS "Acct",
			SUM(-Lines3."Credit") AS "DebitLC",
			SUM(-Lines3."Debit") AS "CreditLC",
			SUM(-Lines3."SYSCred") AS "DebitSC",
			SUM(-Lines3."SYSDeb") AS "CreditSC"
		FROM JDT1 AS Lines1, JDT1 AS Lines2, JDT1 AS Lines3, OJDT AS Header
		WHERE Lines2."TransId" = Lines3."TransId"
		AND Lines1."TransId" = Lines2."TransId"
		AND Lines1."TransId" = Header."TransId"
		AND Lines3."ContraAct" = Lines1."ShortName"
		AND Lines3."Account" <> :account
		AND Lines2."Account" = :account
		AND Lines2."ContraAct" = Lines1."ShortName"
		AND (Lines1."ContraAct" IS NULL or Lines1."ContraAct" = '')
		AND ((Lines2."Credit" > Lines1."Debit" AND Lines2."Credit" > 0) OR (Lines2."Credit" < Lines1."Debit" AND Lines2."Credit" < 0) OR
			(Lines2."Debit" > Lines1."Credit" AND Lines2."Debit" > 0) OR (Lines2."Debit" < Lines1."Credit" AND Lines2."Debit" < 0))
		AND Header."RefDate"
			BETWEEN TO_TIMESTAMP(:dateFrom) 
			AND TO_TIMESTAMP(:dateTo)
		GROUP BY Lines1."Account"
		
		UNION ALL

		SELECT 'Wrong correspondence 1' AS "Acct",
			SUM(Lines."Debit") AS "DebitLC",
			SUM(Lines."Credit") AS "CreditLC",
			SUM(Lines."SYSDeb") AS "DebitSC",
			SUM(Lines."SYSCred") AS "CreditSC"
		FROM JDT1 AS Lines, OJDT AS Header
		WHERE Header."TransId" = Lines."TransId"
		AND Lines."Account" = :account
		AND Lines."ContraAct" IS NULL
		AND NOT EXISTS
		(
			SELECT 1 FROM JDT1
			WHERE Lines."TransId" = JDT1."TransId"
			AND JDT1."ContraAct" = Lines."ShortName"
		)
		AND Header."RefDate"
			BETWEEN TO_TIMESTAMP(:dateFrom) 
			AND TO_TIMESTAMP(:dateTo)
		GROUP BY "Account"
		
		UNION ALL

		SELECT 'Wrong correspondence 2' AS "Acct",
			SUM(Lines."Debit") AS "DebitLC",
			SUM(Lines."Credit") AS "CreditLC",
			SUM(Lines."SYSDeb") AS "DebitSC",
			SUM(Lines."SYSCred") AS "CreditSC"
		FROM JDT1 AS Lines, OJDT AS Header
		WHERE Header."TransId" = Lines."TransId"
		AND Lines."Account" = :account
		AND Lines."ContraAct" IS NOT NULL
		AND Lines."ContraAct" <> ''
		AND NOT EXISTS
		(
			SELECT 1 FROM JDT1
			WHERE Lines."TransId" = JDT1."TransId"
			AND JDT1."ShortName" = Lines."ContraAct"
		)
		AND Header."RefDate"
			BETWEEN TO_TIMESTAMP(:dateFrom) 
			AND TO_TIMESTAMP(:dateTo)
		GROUP BY "Account"
	
	) DUMMY
	GROUP BY "Acct"
	ORDER BY "Sort", "Acct";
END;


