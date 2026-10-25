CREATE PROCEDURE TmSp_FinTemplates(
	-- Add the parameters for the stored procedure here
	IN PerID INTEGER,
	IN TemplateId INTEGER,
	IN ReportMode INTEGER
)
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
	v_PostingDateFrom TIMESTAMP;
	v_PostingDateTo TIMESTAMP;
	v_FY_Start TIMESTAMP;
	v_FinancYear TIMESTAMP;
	v_MaxLevel INTEGER;
	v_CurLevel INTEGER;
BEGIN
	SELECT "F_RefDate", "T_RefDate" INTO v_PostingDateFrom, v_PostingDateTo FROM OFPR WHERE "AbsEntry" = :PerID;
	
	SELECT MIN(T0."F_RefDate") INTO v_FY_Start
	FROM OFPR T0
	WHERE T0."Category" = (
		SELECT S0."Category" FROM OFPR S0 WHERE S0."AbsEntry" = :PerID
	);
	
	SELECT T1."FinancYear" INTO v_FinancYear
	FROM OFPR T0
	INNER JOIN OACP T1 ON T0."Category" = T1."PeriodCat"
	WHERE T0."AbsEntry" = :PerID;

	CREATE LOCAL TEMPORARY TABLE #TMP_TABLE_FINTEMPLATES
	(
		"TemplateID" INTEGER,
		"PeriodID" INTEGER,
		"Acct_Level" INTEGER,
		"FatherNum" INTEGER,
		"AccountCode" NVARCHAR(15),
		"AccountName" NVARCHAR(254),
		"Segment0" NVARCHAR(20),
		"Segment1" NVARCHAR(20),
		"Segment2" NVARCHAR(20),
		"Segment3" NVARCHAR(20),
		"AccountFrgnName" NVARCHAR(254),
		"AccountCategory" NVARCHAR(100),
		"ActType" NVARCHAR(1),
		"VisOrder" INTEGER,
		"AcctVisOrder" INTEGER,
		"Dummy" CHAR(1),
		"SubSum" CHAR(1),
		"Formula" CHAR(1),
		"SubName" NVARCHAR(200),
		"Value_Month_cur_Year" DECIMAL(21,6),
		"Budget_Month_cur_Year" DECIMAL(21,6),
		"Value_Month_prev_Year" DECIMAL(21,6),
		"Budget_Month_prev_Year" DECIMAL(21,6),
		"Value_Year_cur_Year" DECIMAL(21,6),
		"Budget_Year_cur_Year" DECIMAL(21,6),
		"Value_Year_prev_Year" DECIMAL(21,6),
		"Budget_Year_prev_Year" DECIMAL(21,6),
		"Value_Month_01" DECIMAL(21,6),
		"Budget_Month_01" DECIMAL(21,6),
		"Value_Month_02" DECIMAL(21,6),
		"Budget_Month_02" DECIMAL(21,6),
		"Value_Month_03" DECIMAL(21,6),
		"Budget_Month_03" DECIMAL(21,6),
		"Value_Month_04" DECIMAL(21,6),
		"Budget_Month_04" DECIMAL(21,6),
		"Value_Month_05" DECIMAL(21,6),
		"Budget_Month_05" DECIMAL(21,6),
		"Value_Month_06" DECIMAL(21,6),
		"Budget_Month_06" DECIMAL(21,6),
		"Value_Month_07" DECIMAL(21,6),
		"Budget_Month_07" DECIMAL(21,6),
		"Value_Month_08" DECIMAL(21,6),
		"Budget_Month_08" DECIMAL(21,6),
		"Value_Month_09" DECIMAL(21,6),
		"Budget_Month_09" DECIMAL(21,6),
		"Value_Month_10" DECIMAL(21,6),
		"Budget_Month_10" DECIMAL(21,6),
		"Value_Month_11" DECIMAL(21,6),
		"Budget_Month_11" DECIMAL(21,6),
		"Value_Month_12" DECIMAL(21,6),
		"Budget_Month_12" DECIMAL(21,6),
		"Value_Month_13" DECIMAL(21,6),
		"Budget_Month_13" DECIMAL(21,6)
	);

	IF :ReportMode = 2 THEN
		INSERT INTO #TMP_TABLE_FINTEMPLATES ("TemplateID", "PeriodID", "Acct_Level", "FatherNum", "AccountCode", "Segment0", "Segment1", "Segment2", "Segment3", "AccountName", "AccountFrgnName", "AccountCategory", "ActType", "AcctVisOrder", "Dummy", "VisOrder", "Value_Month_cur_Year", "Budget_Month_cur_Year", "Value_Month_prev_Year", "Budget_Month_prev_Year", "Value_Year_cur_Year", "Budget_Year_cur_Year", "Value_Year_prev_Year", "Budget_Year_prev_Year", "Value_Month_01", "Budget_Month_01","Value_Month_02", "Budget_Month_02", "Value_Month_03", "Budget_Month_03", "Value_Month_04", "Budget_Month_04", "Value_Month_05", "Budget_Month_05", "Value_Month_06", "Budget_Month_06", "Value_Month_07", "Budget_Month_07", "Value_Month_08", "Budget_Month_08", "Value_Month_09", "Budget_Month_09", "Value_Month_10", "Budget_Month_10", "Value_Month_11", "Budget_Month_11", "Value_Month_12", "Budget_Month_12")
		SELECT :TemplateId, :PerID, 99, FRC1."CatId", FRC1."AcctCode", OACT."Segment_0", OACT."Segment_1", OACT."Segment_2", OACT."Segment_3", OACT."AcctName", OACT."FrgnName", OACG."Name", OACT."ActType", FRC1."VisOrder", 'N',
			(SELECT OFRC."VisOrder" FROM OFRC WHERE OFRC."TemplateId" = :TemplateId AND OFRC."CatId" = FRC1."CatId"),
			(SELECT IFNULL(SUM(JDT1."Debit" - JDT1."Credit"), 0) FROM JDT1 WHERE JDT1."Account" = OACT."AcctCode" AND JDT1."FinncPriod" = :PerID) AS "Value_Month_cur_Year",
			(SELECT IFNULL(SUM(BGT1."DebLTotal" - BGT1."CredLTotal"), 0) FROM BGT1 INNER JOIN OBGT ON BGT1."BudgId" = OBGT."AbsId" WHERE OBGT."FinancYear" = :v_FinancYear AND BGT1."Line_ID" BETWEEN MONTHS_BETWEEN(:v_FinancYear, :v_PostingDateFrom) AND MONTHS_BETWEEN(:v_FinancYear, :v_PostingDateTo) AND OBGT."AcctCode" = OACT."AcctCode") AS "Budget_Month_cur_Year",
			(SELECT IFNULL(SUM(JDT1."Debit" - JDT1."Credit"), 0) FROM JDT1 WHERE JDT1."Account" = OACT."AcctCode" AND JDT1."RefDate" >= ADD_YEARS(:v_PostingDateFrom, -1) AND JDT1."RefDate" < ADD_DAYS(ADD_YEARS(:v_PostingDateTo, -1), 1)) AS "Value_Month_prev_Year",
			(SELECT IFNULL(SUM(BGT1."DebLTotal" - BGT1."CredLTotal"), 0) FROM BGT1 INNER JOIN OBGT ON BGT1."BudgId" = OBGT."AbsId" WHERE OBGT."FinancYear" = ADD_YEARS(:v_FinancYear, -1) AND BGT1."Line_ID" BETWEEN MONTHS_BETWEEN(:v_FinancYear, :v_PostingDateFrom) AND MONTHS_BETWEEN(:v_FinancYear, :v_PostingDateTo) AND OBGT."AcctCode" = OACT."AcctCode") AS "Budget_Month_prev_Year",
			(SELECT IFNULL(SUM(JDT1."Debit" - JDT1."Credit"), 0) FROM JDT1 WHERE JDT1."Account" = OACT."AcctCode" AND JDT1."RefDate" >= :v_FinancYear AND JDT1."RefDate" < ADD_DAYS(:v_PostingDateTo, 1)) AS "Value_Year_cur_Year",
			(SELECT IFNULL(SUM(BGT1."DebLTotal" - BGT1."CredLTotal"), 0) FROM BGT1 INNER JOIN OBGT ON BGT1."BudgId" = OBGT."AbsId" WHERE OBGT."FinancYear" = :v_FinancYear AND BGT1."Line_ID" <= MONTHS_BETWEEN(:v_FinancYear, :v_PostingDateTo) AND OBGT."AcctCode" = OACT."AcctCode") AS "Budget_Year_cur_Year",
			(SELECT IFNULL(SUM(JDT1."Debit" - JDT1."Credit"), 0) FROM JDT1 WHERE JDT1."Account" = OACT."AcctCode" AND JDT1."RefDate" >= ADD_YEARS(:v_FinancYear, -1) AND JDT1."RefDate" < ADD_DAYS(ADD_YEARS(:v_PostingDateTo, -1), 1)) AS "Value_Year_prev_Year", 
			(SELECT IFNULL(SUM(BGT1."DebLTotal" - BGT1."CredLTotal"), 0) FROM BGT1 INNER JOIN OBGT ON BGT1."BudgId" = OBGT."AbsId" WHERE OBGT."FinancYear" = ADD_YEARS(:v_FinancYear, -1) AND BGT1."Line_ID" <= MONTHS_BETWEEN(:v_FinancYear, :v_PostingDateTo) AND OBGT."AcctCode" = OACT."AcctCode") AS "Budget_Year_prev_Year",

			(SELECT IFNULL(SUM(JDT1."Debit" - JDT1."Credit"), 0) FROM JDT1 WHERE JDT1."Account" = OACT."AcctCode" AND YEAR(JDT1."RefDate") = YEAR(:v_FinancYear) AND MONTH(JDT1."RefDate") = MONTH(:v_FinancYear)),
			(SELECT IFNULL(SUM(BGT1."DebLTotal" - BGT1."CredLTotal"), 0) FROM BGT1 INNER JOIN OBGT ON BGT1."BudgId" = OBGT."AbsId" WHERE OBGT."FinancYear" = :v_FinancYear AND BGT1."Line_ID" = 0 AND OBGT."AcctCode" = OACT."AcctCode"),
			(SELECT IFNULL(SUM(JDT1."Debit" - JDT1."Credit"), 0) FROM JDT1 WHERE JDT1."Account" = OACT."AcctCode" AND YEAR(JDT1."RefDate") = YEAR(ADD_MONTHS(:v_FinancYear, 1)) AND MONTH(JDT1."RefDate") = MONTH(ADD_MONTHS(:v_FinancYear, 1))),
			(SELECT IFNULL(SUM(BGT1."DebLTotal" - BGT1."CredLTotal"), 0) FROM BGT1 INNER JOIN OBGT ON BGT1."BudgId" = OBGT."AbsId" WHERE OBGT."FinancYear" = :v_FinancYear AND BGT1."Line_ID" = 1 AND OBGT."AcctCode" = OACT."AcctCode"),
			(SELECT IFNULL(SUM(JDT1."Debit" - JDT1."Credit"), 0) FROM JDT1 WHERE JDT1."Account" = OACT."AcctCode" AND YEAR(JDT1."RefDate") = YEAR(ADD_MONTHS(:v_FinancYear, 2)) AND MONTH(JDT1."RefDate") = MONTH(ADD_MONTHS(:v_FinancYear, 2))),
			(SELECT IFNULL(SUM(BGT1."DebLTotal" - BGT1."CredLTotal"), 0) FROM BGT1 INNER JOIN OBGT ON BGT1."BudgId" = OBGT."AbsId" WHERE OBGT."FinancYear" = :v_FinancYear AND BGT1."Line_ID" = 2 AND OBGT."AcctCode" = OACT."AcctCode"),
			(SELECT IFNULL(SUM(JDT1."Debit" - JDT1."Credit"), 0) FROM JDT1 WHERE JDT1."Account" = OACT."AcctCode" AND YEAR(JDT1."RefDate") = YEAR(ADD_MONTHS(:v_FinancYear, 3)) AND MONTH(JDT1."RefDate") = MONTH(ADD_MONTHS(:v_FinancYear, 3))),
			(SELECT IFNULL(SUM(BGT1."DebLTotal" - BGT1."CredLTotal"), 0) FROM BGT1 INNER JOIN OBGT ON BGT1."BudgId" = OBGT."AbsId" WHERE OBGT."FinancYear" = :v_FinancYear AND BGT1."Line_ID" = 3 AND OBGT."AcctCode" = OACT."AcctCode"),
			(SELECT IFNULL(SUM(JDT1."Debit" - JDT1."Credit"), 0) FROM JDT1 WHERE JDT1."Account" = OACT."AcctCode" AND YEAR(JDT1."RefDate") = YEAR(ADD_MONTHS(:v_FinancYear, 4)) AND MONTH(JDT1."RefDate") = MONTH(ADD_MONTHS(:v_FinancYear, 4))),
			(SELECT IFNULL(SUM(BGT1."DebLTotal" - BGT1."CredLTotal"), 0) FROM BGT1 INNER JOIN OBGT ON BGT1."BudgId" = OBGT."AbsId" WHERE OBGT."FinancYear" = :v_FinancYear AND BGT1."Line_ID" = 4 AND OBGT."AcctCode" = OACT."AcctCode"),
			(SELECT IFNULL(SUM(JDT1."Debit" - JDT1."Credit"), 0) FROM JDT1 WHERE JDT1."Account" = OACT."AcctCode" AND YEAR(JDT1."RefDate") = YEAR(ADD_MONTHS(:v_FinancYear, 5)) AND MONTH(JDT1."RefDate") = MONTH(ADD_MONTHS(:v_FinancYear, 5))),
			(SELECT IFNULL(SUM(BGT1."DebLTotal" - BGT1."CredLTotal"), 0) FROM BGT1 INNER JOIN OBGT ON BGT1."BudgId" = OBGT."AbsId" WHERE OBGT."FinancYear" = :v_FinancYear AND BGT1."Line_ID" = 5 AND OBGT."AcctCode" = OACT."AcctCode"),
			(SELECT IFNULL(SUM(JDT1."Debit" - JDT1."Credit"), 0) FROM JDT1 WHERE JDT1."Account" = OACT."AcctCode" AND YEAR(JDT1."RefDate") = YEAR(ADD_MONTHS(:v_FinancYear, 6)) AND MONTH(JDT1."RefDate") = MONTH(ADD_MONTHS(:v_FinancYear, 6))),
			(SELECT IFNULL(SUM(BGT1."DebLTotal" - BGT1."CredLTotal"), 0) FROM BGT1 INNER JOIN OBGT ON BGT1."BudgId" = OBGT."AbsId" WHERE OBGT."FinancYear" = :v_FinancYear AND BGT1."Line_ID" = 6 AND OBGT."AcctCode" = OACT."AcctCode"),
			(SELECT IFNULL(SUM(JDT1."Debit" - JDT1."Credit"), 0) FROM JDT1 WHERE JDT1."Account" = OACT."AcctCode" AND YEAR(JDT1."RefDate") = YEAR(ADD_MONTHS(:v_FinancYear, 7)) AND MONTH(JDT1."RefDate") = MONTH(ADD_MONTHS(:v_FinancYear, 7))),
			(SELECT IFNULL(SUM(BGT1."DebLTotal" - BGT1."CredLTotal"), 0) FROM BGT1 INNER JOIN OBGT ON BGT1."BudgId" = OBGT."AbsId" WHERE OBGT."FinancYear" = :v_FinancYear AND BGT1."Line_ID" = 7 AND OBGT."AcctCode" = OACT."AcctCode"),
			(SELECT IFNULL(SUM(JDT1."Debit" - JDT1."Credit"), 0) FROM JDT1 WHERE JDT1."Account" = OACT."AcctCode" AND YEAR(JDT1."RefDate") = YEAR(ADD_MONTHS(:v_FinancYear, 8)) AND MONTH(JDT1."RefDate") = MONTH(ADD_MONTHS(:v_FinancYear, 8))),
			(SELECT IFNULL(SUM(BGT1."DebLTotal" - BGT1."CredLTotal"), 0) FROM BGT1 INNER JOIN OBGT ON BGT1."BudgId" = OBGT."AbsId" WHERE OBGT."FinancYear" = :v_FinancYear AND BGT1."Line_ID" = 8 AND OBGT."AcctCode" = OACT."AcctCode"),
			(SELECT IFNULL(SUM(JDT1."Debit" - JDT1."Credit"), 0) FROM JDT1 WHERE JDT1."Account" = OACT."AcctCode" AND YEAR(JDT1."RefDate") = YEAR(ADD_MONTHS(:v_FinancYear, 9)) AND MONTH(JDT1."RefDate") = MONTH(ADD_MONTHS(:v_FinancYear, 9))),
			(SELECT IFNULL(SUM(BGT1."DebLTotal" - BGT1."CredLTotal"), 0) FROM BGT1 INNER JOIN OBGT ON BGT1."BudgId" = OBGT."AbsId" WHERE OBGT."FinancYear" = :v_FinancYear AND BGT1."Line_ID" = 9 AND OBGT."AcctCode" = OACT."AcctCode"),
			(SELECT IFNULL(SUM(JDT1."Debit" - JDT1."Credit"), 0) FROM JDT1 WHERE JDT1."Account" = OACT."AcctCode" AND YEAR(JDT1."RefDate") = YEAR(ADD_MONTHS(:v_FinancYear, 10)) AND MONTH(JDT1."RefDate") = MONTH(ADD_MONTHS(:v_FinancYear, 10))),
			(SELECT IFNULL(SUM(BGT1."DebLTotal" - BGT1."CredLTotal"), 0) FROM BGT1 INNER JOIN OBGT ON BGT1."BudgId" = OBGT."AbsId" WHERE OBGT."FinancYear" = :v_FinancYear AND BGT1."Line_ID" = 10 AND OBGT."AcctCode" = OACT."AcctCode"),
			(SELECT IFNULL(SUM(JDT1."Debit" - JDT1."Credit"), 0) FROM JDT1 WHERE JDT1."Account" = OACT."AcctCode" AND YEAR(JDT1."RefDate") = YEAR(ADD_MONTHS(:v_FinancYear, 11)) AND MONTH(JDT1."RefDate") = MONTH(ADD_MONTHS(:v_FinancYear, 11))),
			(SELECT IFNULL(SUM(BGT1."DebLTotal" - BGT1."CredLTotal"), 0) FROM BGT1 INNER JOIN OBGT ON BGT1."BudgId" = OBGT."AbsId" WHERE OBGT."FinancYear" = :v_FinancYear AND BGT1."Line_ID" = 11 AND OBGT."AcctCode" = OACT."AcctCode")
		FROM OACT INNER JOIN FRC1 ON OACT."AcctCode" = FRC1."AcctCode" LEFT JOIN OACG ON OACT."Category" = OACG."AbsId" WHERE FRC1."TemplateId" = :TemplateId;

		-- Reverse the sign for Sales and Expenditure accounts
		UPDATE #TMP_TABLE_FINTEMPLATES
		SET "Value_Month_cur_Year" = -"Value_Month_cur_Year",
			"Budget_Month_cur_Year" = -"Budget_Month_cur_Year",
			"Value_Month_prev_Year" = -"Value_Month_prev_Year",
			"Budget_Month_prev_Year" = -"Budget_Month_prev_Year",
			"Value_Year_cur_Year" = -"Value_Year_cur_Year",
			"Budget_Year_cur_Year" = -"Budget_Year_cur_Year",
			"Value_Year_prev_Year" = -"Value_Year_prev_Year",
			"Budget_Year_prev_Year" = -"Budget_Year_prev_Year",
			"Value_Month_01" = -"Value_Month_01",
			"Budget_Month_01" = -"Budget_Month_01",
			"Value_Month_02" = -"Value_Month_02",
			"Budget_Month_02" = -"Budget_Month_02",
			"Value_Month_03" = -"Value_Month_03",
			"Budget_Month_03" = -"Budget_Month_03",
			"Value_Month_04" = -"Value_Month_04",
			"Budget_Month_04" = -"Budget_Month_04",
			"Value_Month_05" = -"Value_Month_05",
			"Budget_Month_05" = -"Budget_Month_05",
			"Value_Month_06" = -"Value_Month_06",
			"Budget_Month_06" = -"Budget_Month_06",
			"Value_Month_07" = -"Value_Month_07",
			"Budget_Month_07" = -"Budget_Month_07",
			"Value_Month_08" = -"Value_Month_08",
			"Budget_Month_08" = -"Budget_Month_08",
			"Value_Month_09" = -"Value_Month_09",
			"Budget_Month_09" = -"Budget_Month_09",
			"Value_Month_10" = -"Value_Month_10",
			"Budget_Month_10" = -"Budget_Month_10",
			"Value_Month_11" = -"Value_Month_11",
			"Budget_Month_11" = -"Budget_Month_11",
			"Value_Month_12" = -"Value_Month_12",
			"Budget_Month_12" = -"Budget_Month_12"
		WHERE "ActType" <> 'N';
	ELSE
		INSERT INTO #TMP_TABLE_FINTEMPLATES ("TemplateID", "PeriodID", "Acct_Level", "FatherNum", "AccountCode", "Segment0", "Segment1", "Segment2", "Segment3", "AccountName", "AccountFrgnName", "AccountCategory", "ActType", "AcctVisOrder", "Dummy", "VisOrder", "Value_Month_cur_Year", "Budget_Month_cur_Year", "Value_Month_prev_Year", "Budget_Month_prev_Year", "Value_Year_cur_Year", "Budget_Year_cur_Year", "Value_Year_prev_Year", "Budget_Year_prev_Year")
		SELECT :TemplateId, :PerID, 99, FRC1."CatId", FRC1."AcctCode", OACT."Segment_0", OACT."Segment_1", OACT."Segment_2", OACT."Segment_3", OACT."AcctName", OACT."FrgnName", OACG."Name", OACT."ActType", FRC1."VisOrder", 'N',
			(SELECT OFRC."VisOrder" FROM OFRC WHERE OFRC."TemplateId" = :TemplateId AND OFRC."CatId" = FRC1."CatId") AS "VisOrder",
			
			(SELECT IFNULL(SUM(JDT1."Debit" - JDT1."Credit"), 0) FROM JDT1 WHERE JDT1."Account" = OACT."AcctCode" AND JDT1."FinncPriod" = :PerID) AS "Value_Month_cur_Year",
			(SELECT IFNULL(SUM(BGT1."DebLTotal" - BGT1."CredLTotal"), 0) FROM BGT1 INNER JOIN OBGT ON BGT1."BudgId" = OBGT."AbsId" WHERE OBGT."FinancYear" = :v_FinancYear AND BGT1."Line_ID" BETWEEN MONTHS_BETWEEN(:v_FinancYear, :v_PostingDateFrom) AND MONTHS_BETWEEN(:v_FinancYear, :v_PostingDateTo) AND OBGT."AcctCode" = OACT."AcctCode") AS "Budget_Month_cur_Year",
			(SELECT IFNULL(SUM(JDT1."Debit" - JDT1."Credit"), 0) FROM JDT1 WHERE JDT1."Account" = OACT."AcctCode" AND JDT1."RefDate" >= ADD_YEARS(:v_PostingDateFrom, -1) AND JDT1."RefDate" < ADD_DAYS(ADD_YEARS(:v_PostingDateTo, -1), 1)) AS "Value_Month_prev_Year",
			(SELECT IFNULL(SUM(BGT1."DebLTotal" - BGT1."CredLTotal"), 0) FROM BGT1 INNER JOIN OBGT ON BGT1."BudgId" = OBGT."AbsId" WHERE OBGT."FinancYear" = ADD_YEARS(:v_FinancYear, -1) AND BGT1."Line_ID" BETWEEN MONTHS_BETWEEN(:v_FinancYear, :v_PostingDateFrom) AND MONTHS_BETWEEN(:v_FinancYear, :v_PostingDateTo) AND OBGT."AcctCode" = OACT."AcctCode") AS "Budget_Month_prev_Year",
			(SELECT IFNULL(SUM(JDT1."Debit" - JDT1."Credit"), 0) FROM JDT1 WHERE JDT1."Account" = OACT."AcctCode" AND JDT1."RefDate" >= :v_FinancYear AND JDT1."RefDate" < ADD_DAYS(:v_PostingDateTo, 1)) AS "Value_Year_cur_Year",
			(SELECT IFNULL(SUM(BGT1."DebLTotal" - BGT1."CredLTotal"), 0) FROM BGT1 INNER JOIN OBGT ON BGT1."BudgId" = OBGT."AbsId" WHERE OBGT."FinancYear" = :v_FinancYear AND BGT1."Line_ID" <= MONTHS_BETWEEN(:v_FinancYear, :v_PostingDateTo) AND OBGT."AcctCode" = OACT."AcctCode") AS "Budget_Year_cur_Year",
			(SELECT IFNULL(SUM(JDT1."Debit" - JDT1."Credit"), 0) FROM JDT1 WHERE JDT1."Account" = OACT."AcctCode" AND JDT1."RefDate" >= ADD_YEARS(:v_FinancYear, -1) AND JDT1."RefDate" < ADD_DAYS(ADD_YEARS(:v_PostingDateTo, -1), 1)) AS "Value_Year_prev_Year", 
			(SELECT IFNULL(SUM(BGT1."DebLTotal" - BGT1."CredLTotal"), 0) FROM BGT1 INNER JOIN OBGT ON BGT1."BudgId" = OBGT."AbsId" WHERE OBGT."FinancYear" = ADD_YEARS(:v_FinancYear, -1) AND BGT1."Line_ID" <= MONTHS_BETWEEN(:v_FinancYear, :v_PostingDateTo) AND OBGT."AcctCode" = OACT."AcctCode") AS "Budget_Year_prev_Year"
		FROM OACT INNER JOIN FRC1 ON OACT."AcctCode" = FRC1."AcctCode" LEFT JOIN OACG ON OACT."Category" = OACG."AbsId" WHERE FRC1."TemplateId" = :TemplateId;

		-- Reverse the sign for Sales and Expenditure accounts
		UPDATE #TMP_TABLE_FINTEMPLATES
		SET "Value_Month_cur_Year" = -"Value_Month_cur_Year",
			"Budget_Month_cur_Year" = -"Budget_Month_cur_Year",
			"Value_Month_prev_Year" = -"Value_Month_prev_Year",
			"Budget_Month_prev_Year" = -"Budget_Month_prev_Year",
			"Value_Year_cur_Year" = -"Value_Year_cur_Year",
			"Budget_Year_cur_Year" = -"Budget_Year_cur_Year",
			"Value_Year_prev_Year" = -"Value_Year_prev_Year",
			"Budget_Year_prev_Year" = -"Budget_Year_prev_Year"
		WHERE "ActType" <> 'N';
	END IF;

	INSERT INTO #TMP_TABLE_FINTEMPLATES ("TemplateID", "PeriodID", "Acct_Level", "FatherNum", "AccountCode", "Segment0", "Segment1", "Segment2", "Segment3", "AccountName", "AccountFrgnName", "VisOrder", "AcctVisOrder", "Dummy", "SubSum", "Formula", "SubName")
	SELECT :TemplateId, :PerID, "Levels", "FatherNum", "CatId", null, null, null, null, "Name", "FrgnName", "VisOrder", 0, "Dummy", "SubSum", "Furmula", "SubName" FROM OFRC WHERE "TemplateId" = :TemplateId;

	SELECT MAX("Acct_Level") INTO v_MaxLevel FROM #TMP_TABLE_FINTEMPLATES WHERE "Acct_Level" <> 99;
	v_CurLevel := :v_MaxLevel;

	WHILE :v_CurLevel > 0 DO
		UPDATE T0
		SET T0."Value_Month_cur_Year" = (SELECT SUM(T1."Value_Month_cur_Year") FROM #TMP_TABLE_FINTEMPLATES T1 WHERE (T1."Acct_Level" = 99 OR T1."Acct_Level" = :v_CurLevel + 1) AND T1."FatherNum" = T0."AccountCode" group by T1."FatherNum"),
			T0."Budget_Month_cur_Year" = (SELECT SUM(T1."Budget_Month_cur_Year") FROM #TMP_TABLE_FINTEMPLATES T1 WHERE (T1."Acct_Level" = 99 OR T1."Acct_Level" = :v_CurLevel + 1) AND T1."FatherNum" = T0."AccountCode" group by T1."FatherNum"),
			T0."Value_Month_prev_Year" = (SELECT SUM(T1."Value_Month_prev_Year") FROM #TMP_TABLE_FINTEMPLATES T1 WHERE (T1."Acct_Level" = 99 OR T1."Acct_Level" = :v_CurLevel + 1) AND T1."FatherNum" = T0."AccountCode" group by T1."FatherNum"),
			T0."Budget_Month_prev_Year" = (SELECT SUM(T1."Budget_Month_prev_Year") FROM #TMP_TABLE_FINTEMPLATES T1 WHERE (T1."Acct_Level" = 99 OR T1."Acct_Level" = :v_CurLevel + 1) AND T1."FatherNum" = T0."AccountCode" group by T1."FatherNum"),
			T0."Value_Year_cur_Year" = (SELECT SUM(T1."Value_Year_cur_Year") FROM #TMP_TABLE_FINTEMPLATES T1 WHERE (T1."Acct_Level" = 99 OR T1."Acct_Level" = :v_CurLevel + 1) AND T1."FatherNum" = T0."AccountCode" group by T1."FatherNum"),
			T0."Budget_Year_cur_Year" = (SELECT SUM(T1."Budget_Year_cur_Year") FROM #TMP_TABLE_FINTEMPLATES T1 WHERE (T1."Acct_Level" = 99 OR T1."Acct_Level" = :v_CurLevel + 1) AND T1."FatherNum" = T0."AccountCode" group by T1."FatherNum"),
			T0."Value_Year_prev_Year" = (SELECT SUM(T1."Value_Year_prev_Year") FROM #TMP_TABLE_FINTEMPLATES T1 WHERE (T1."Acct_Level" = 99 OR T1."Acct_Level" = :v_CurLevel + 1) AND T1."FatherNum" = T0."AccountCode" group by T1."FatherNum"),
			T0."Budget_Year_prev_Year" = (SELECT SUM(T1."Budget_Year_prev_Year") FROM #TMP_TABLE_FINTEMPLATES T1 WHERE (T1."Acct_Level" = 99 OR T1."Acct_Level" = :v_CurLevel + 1) AND T1."FatherNum" = T0."AccountCode" group by T1."FatherNum")
		FROM #TMP_TABLE_FINTEMPLATES T0
		WHERE T0."Acct_Level" = :v_CurLevel;

		v_CurLevel := :v_CurLevel - 1;
	END WHILE;

	IF :ReportMode = 2 THEN
		v_CurLevel := :v_MaxLevel;
		
		WHILE :v_CurLevel > 0 DO
			UPDATE T0
			SET T0."Value_Month_01" = (SELECT SUM(T1."Value_Month_01") FROM #TMP_TABLE_FINTEMPLATES T1 WHERE (T1."Acct_Level" = 99 OR T1."Acct_Level" = :v_CurLevel + 1) AND T1."FatherNum" = T0."AccountCode" group by T1."FatherNum"),
				T0."Budget_Month_01" = (SELECT SUM(T1."Budget_Month_01") FROM #TMP_TABLE_FINTEMPLATES T1 WHERE (T1."Acct_Level" = 99 OR T1."Acct_Level" = :v_CurLevel + 1) AND T1."FatherNum" = T0."AccountCode" group by T1."FatherNum"),
				T0."Value_Month_02" = (SELECT SUM(T1."Value_Month_02") FROM #TMP_TABLE_FINTEMPLATES T1 WHERE (T1."Acct_Level" = 99 OR T1."Acct_Level" = :v_CurLevel + 1) AND T1."FatherNum" = T0."AccountCode" group by T1."FatherNum"),
				T0."Budget_Month_02" = (SELECT SUM(T1."Budget_Month_02") FROM #TMP_TABLE_FINTEMPLATES T1 WHERE (T1."Acct_Level" = 99 OR T1."Acct_Level" = :v_CurLevel + 1) AND T1."FatherNum" = T0."AccountCode" group by T1."FatherNum"),
				T0."Value_Month_03" = (SELECT SUM(T1."Value_Month_03") FROM #TMP_TABLE_FINTEMPLATES T1 WHERE (T1."Acct_Level" = 99 OR T1."Acct_Level" = :v_CurLevel + 1) AND T1."FatherNum" = T0."AccountCode" group by T1."FatherNum"),
				T0."Budget_Month_03" = (SELECT SUM(T1."Budget_Month_03") FROM #TMP_TABLE_FINTEMPLATES T1 WHERE (T1."Acct_Level" = 99 OR T1."Acct_Level" = :v_CurLevel + 1) AND T1."FatherNum" = T0."AccountCode" group by T1."FatherNum"),
				T0."Value_Month_04" = (SELECT SUM(T1."Value_Month_04") FROM #TMP_TABLE_FINTEMPLATES T1 WHERE (T1."Acct_Level" = 99 OR T1."Acct_Level" = :v_CurLevel + 1) AND T1."FatherNum" = T0."AccountCode" group by T1."FatherNum"),
				T0."Budget_Month_04" = (SELECT SUM(T1."Budget_Month_04") FROM #TMP_TABLE_FINTEMPLATES T1 WHERE (T1."Acct_Level" = 99 OR T1."Acct_Level" = :v_CurLevel + 1) AND T1."FatherNum" = T0."AccountCode" group by T1."FatherNum"),
				T0."Value_Month_05" = (SELECT SUM(T1."Value_Month_05") FROM #TMP_TABLE_FINTEMPLATES T1 WHERE (T1."Acct_Level" = 99 OR T1."Acct_Level" = :v_CurLevel + 1) AND T1."FatherNum" = T0."AccountCode" group by T1."FatherNum"),
				T0."Budget_Month_05" = (SELECT SUM(T1."Budget_Month_05") FROM #TMP_TABLE_FINTEMPLATES T1 WHERE (T1."Acct_Level" = 99 OR T1."Acct_Level" = :v_CurLevel + 1) AND T1."FatherNum" = T0."AccountCode" group by T1."FatherNum"),
				T0."Value_Month_06" = (SELECT SUM(T1."Value_Month_06") FROM #TMP_TABLE_FINTEMPLATES T1 WHERE (T1."Acct_Level" = 99 OR T1."Acct_Level" = :v_CurLevel + 1) AND T1."FatherNum" = T0."AccountCode" group by T1."FatherNum"),
				T0."Budget_Month_06" = (SELECT SUM(T1."Budget_Month_06") FROM #TMP_TABLE_FINTEMPLATES T1 WHERE (T1."Acct_Level" = 99 OR T1."Acct_Level" = :v_CurLevel + 1) AND T1."FatherNum" = T0."AccountCode" group by T1."FatherNum"),
				T0."Value_Month_07" = (SELECT SUM(T1."Value_Month_07") FROM #TMP_TABLE_FINTEMPLATES T1 WHERE (T1."Acct_Level" = 99 OR T1."Acct_Level" = :v_CurLevel + 1) AND T1."FatherNum" = T0."AccountCode" group by T1."FatherNum"),
				T0."Budget_Month_07" = (SELECT SUM(T1."Budget_Month_07") FROM #TMP_TABLE_FINTEMPLATES T1 WHERE (T1."Acct_Level" = 99 OR T1."Acct_Level" = :v_CurLevel + 1) AND T1."FatherNum" = T0."AccountCode" group by T1."FatherNum"),
				T0."Value_Month_08" = (SELECT SUM(T1."Value_Month_08") FROM #TMP_TABLE_FINTEMPLATES T1 WHERE (T1."Acct_Level" = 99 OR T1."Acct_Level" = :v_CurLevel + 1) AND T1."FatherNum" = T0."AccountCode" group by T1."FatherNum"),
				T0."Budget_Month_08" = (SELECT SUM(T1."Budget_Month_08") FROM #TMP_TABLE_FINTEMPLATES T1 WHERE (T1."Acct_Level" = 99 OR T1."Acct_Level" = :v_CurLevel + 1) AND T1."FatherNum" = T0."AccountCode" group by T1."FatherNum"),
				T0."Value_Month_09" = (SELECT SUM(T1."Value_Month_09") FROM #TMP_TABLE_FINTEMPLATES T1 WHERE (T1."Acct_Level" = 99 OR T1."Acct_Level" = :v_CurLevel + 1) AND T1."FatherNum" = T0."AccountCode" group by T1."FatherNum"),
				T0."Budget_Month_09" = (SELECT SUM(T1."Budget_Month_09") FROM #TMP_TABLE_FINTEMPLATES T1 WHERE (T1."Acct_Level" = 99 OR T1."Acct_Level" = :v_CurLevel + 1) AND T1."FatherNum" = T0."AccountCode" group by T1."FatherNum"),
				T0."Value_Month_10" = (SELECT SUM(T1."Value_Month_10") FROM #TMP_TABLE_FINTEMPLATES T1 WHERE (T1."Acct_Level" = 99 OR T1."Acct_Level" = :v_CurLevel + 1) AND T1."FatherNum" = T0."AccountCode" group by T1."FatherNum"),
				T0."Budget_Month_10" = (SELECT SUM(T1."Budget_Month_10") FROM #TMP_TABLE_FINTEMPLATES T1 WHERE (T1."Acct_Level" = 99 OR T1."Acct_Level" = :v_CurLevel + 1) AND T1."FatherNum" = T0."AccountCode" group by T1."FatherNum"),
				T0."Value_Month_11" = (SELECT SUM(T1."Value_Month_11") FROM #TMP_TABLE_FINTEMPLATES T1 WHERE (T1."Acct_Level" = 99 OR T1."Acct_Level" = :v_CurLevel + 1) AND T1."FatherNum" = T0."AccountCode" group by T1."FatherNum"),
				T0."Budget_Month_11" = (SELECT SUM(T1."Budget_Month_11") FROM #TMP_TABLE_FINTEMPLATES T1 WHERE (T1."Acct_Level" = 99 OR T1."Acct_Level" = :v_CurLevel + 1) AND T1."FatherNum" = T0."AccountCode" group by T1."FatherNum"),
				T0."Value_Month_12" = (SELECT SUM(T1."Value_Month_12") FROM #TMP_TABLE_FINTEMPLATES T1 WHERE (T1."Acct_Level" = 99 OR T1."Acct_Level" = :v_CurLevel + 1) AND T1."FatherNum" = T0."AccountCode" group by T1."FatherNum"),
				T0."Budget_Month_12" = (SELECT SUM(T1."Budget_Month_12") FROM #TMP_TABLE_FINTEMPLATES T1 WHERE (T1."Acct_Level" = 99 OR T1."Acct_Level" = :v_CurLevel + 1) AND T1."FatherNum" = T0."AccountCode" group by T1."FatherNum")
			FROM #TMP_TABLE_FINTEMPLATES T0
			WHERE T0."Acct_Level" = :v_CurLevel;

			v_CurLevel := :v_CurLevel - 1;
		END WHILE;
	END IF;

	CREATE LOCAL TEMPORARY TABLE #TMP_TABLE_FORMULA_PARAMS
	(
		"Param" SMALLINT,
		"CatId" SMALLINT,
		"Sign" SMALLINT
	);

	INSERT INTO #TMP_TABLE_FORMULA_PARAMS ("Param", "CatId", "Sign")
	SELECT "Param_1" as "Param", "CatId", 1 as "Sign" FROM OFRC WHERE "TemplateId" = :TemplateId AND "SubSum" = 'Y' AND "Furmula" = 'Y' AND "Param_1" > 0
	UNION ALL
	SELECT "Param_2" as "Param", "CatId", (CASE WHEN "OP_1" = '+' THEN 1 ELSE -1 END) as "Sign" FROM OFRC WHERE "TemplateId" = :TemplateId AND "SubSum" = 'Y' AND "Furmula" = 'Y' AND "Param_2" > 0
	UNION ALL
	SELECT "Param_3" as "Param", "CatId", (CASE WHEN "OP_2" = '+' THEN 1 ELSE -1 END) as "Sign" FROM OFRC WHERE "TemplateId" = :TemplateId AND "SubSum" = 'Y' AND "Furmula" = 'Y' AND "Param_3" > 0
	UNION ALL
	SELECT "Param_4" as "Param", "CatId", (CASE WHEN "OP_3" = '+' THEN 1 ELSE -1 END) as "Sign" FROM OFRC WHERE "TemplateId" = :TemplateId AND "SubSum" = 'Y' AND "Furmula" = 'Y' AND "Param_4" > 0
	UNION ALL
	SELECT "Param_5" as "Param", "CatId", (CASE WHEN "OP_4" = '+' THEN 1 ELSE -1 END) as "Sign" FROM OFRC WHERE "TemplateId" = :TemplateId AND "SubSum" = 'Y' AND "Furmula" = 'Y' AND "Param_5" > 0
	UNION ALL
	SELECT "Param_6" as "Param", "CatId", (CASE WHEN "OP_5" = '+' THEN 1 ELSE -1 END) as "Sign" FROM OFRC WHERE "TemplateId" = :TemplateId AND "SubSum" = 'Y' AND "Furmula" = 'Y' AND "Param_6" > 0
	UNION ALL
	SELECT "Param_7" as "Param", "CatId", (CASE WHEN "OP_6" = '+' THEN 1 ELSE -1 END) as "Sign" FROM OFRC WHERE "TemplateId" = :TemplateId AND "SubSum" = 'Y' AND "Furmula" = 'Y' AND "Param_7" > 0
	UNION ALL
	SELECT "Param_8" as "Param", "CatId", (CASE WHEN "OP_7" = '+' THEN 1 ELSE -1 END) as "Sign" FROM OFRC WHERE "TemplateId" = :TemplateId AND "SubSum" = 'Y' AND "Furmula" = 'Y' AND "Param_8" > 0
	UNION ALL
	SELECT "Param_9" as "Param", "CatId", (CASE WHEN "OP_8" = '+' THEN 1 ELSE -1 END) as "Sign" FROM OFRC WHERE "TemplateId" = :TemplateId AND "SubSum" = 'Y' AND "Furmula" = 'Y' AND "Param_9" > 0
	UNION ALL
	SELECT "Param_10" as "Param", "CatId", (CASE WHEN "OP_9" = '+' THEN 1 ELSE -1 END) as "Sign" FROM OFRC WHERE "TemplateId" = :TemplateId AND "SubSum" = 'Y' AND "Furmula" = 'Y' AND "Param_10" > 0
	UNION ALL
	SELECT "Param_11" as "Param", "CatId", (CASE WHEN "OP_10" = '+' THEN 1 ELSE -1 END) as "Sign" FROM OFRC WHERE "TemplateId" = :TemplateId AND "SubSum" = 'Y' AND "Furmula" = 'Y' AND "Param_11" > 0
	UNION ALL
	SELECT "Param_12" as "Param", "CatId", (CASE WHEN "OP_11" = '+' THEN 1 ELSE -1 END) as "Sign" FROM OFRC WHERE "TemplateId" = :TemplateId AND "SubSum" = 'Y' AND "Furmula" = 'Y' AND "Param_12" > 0
	UNION ALL
	SELECT "Param_13" as "Param", "CatId", (CASE WHEN "OP_12" = '+' THEN 1 ELSE -1 END) as "Sign" FROM OFRC WHERE "TemplateId" = :TemplateId AND "SubSum" = 'Y' AND "Furmula" = 'Y' AND "Param_13" > 0
	UNION ALL
	SELECT "Param_14" as "Param", "CatId", (CASE WHEN "OP_13" = '+' THEN 1 ELSE -1 END) as "Sign" FROM OFRC WHERE "TemplateId" = :TemplateId AND "SubSum" = 'Y' AND "Furmula" = 'Y' AND "Param_14" > 0
	UNION ALL
	SELECT "Param_15" as "Param", "CatId", (CASE WHEN "OP_14" = '+' THEN 1 ELSE -1 END) as "Sign" FROM OFRC WHERE "TemplateId" = :TemplateId AND "SubSum" = 'Y' AND "Furmula" = 'Y' AND "Param_15" > 0
	UNION ALL
	SELECT "Param_16" as "Param", "CatId", (CASE WHEN "OP_15" = '+' THEN 1 ELSE -1 END) as "Sign" FROM OFRC WHERE "TemplateId" = :TemplateId AND "SubSum" = 'Y' AND "Furmula" = 'Y' AND "Param_16" > 0
	UNION ALL
	SELECT "Param_17" as "Param", "CatId", (CASE WHEN "OP_16" = '+' THEN 1 ELSE -1 END) as "Sign" FROM OFRC WHERE "TemplateId" = :TemplateId AND "SubSum" = 'Y' AND "Furmula" = 'Y' AND "Param_17" > 0
	UNION ALL
	SELECT "Param_18" as "Param", "CatId", (CASE WHEN "OP_17" = '+' THEN 1 ELSE -1 END) as "Sign" FROM OFRC WHERE "TemplateId" = :TemplateId AND "SubSum" = 'Y' AND "Furmula" = 'Y' AND "Param_18" > 0
	UNION ALL
	SELECT "Param_19" as "Param", "CatId", (CASE WHEN "OP_18" = '+' THEN 1 ELSE -1 END) as "Sign" FROM OFRC WHERE "TemplateId" = :TemplateId AND "SubSum" = 'Y' AND "Furmula" = 'Y' AND "Param_19" > 0
	UNION ALL
	SELECT "Param_20" as "Param", "CatId", (CASE WHEN "OP_19" = '+' THEN 1 ELSE -1 END) as "Sign" FROM OFRC WHERE "TemplateId" = :TemplateId AND "SubSum" = 'Y' AND "Furmula" = 'Y' AND "Param_20" > 0
	UNION ALL
	SELECT "Param_21" as "Param", "CatId", (CASE WHEN "OP_20" = '+' THEN 1 ELSE -1 END) as "Sign" FROM OFRC WHERE "TemplateId" = :TemplateId AND "SubSum" = 'Y' AND "Furmula" = 'Y' AND "Param_21" > 0
	UNION ALL
	SELECT "Param_22" as "Param", "CatId", (CASE WHEN "OP_21" = '+' THEN 1 ELSE -1 END) as "Sign" FROM OFRC WHERE "TemplateId" = :TemplateId AND "SubSum" = 'Y' AND "Furmula" = 'Y' AND "Param_22" > 0
	UNION ALL
	SELECT "Param_23" as "Param", "CatId", (CASE WHEN "OP_22" = '+' THEN 1 ELSE -1 END) as "Sign" FROM OFRC WHERE "TemplateId" = :TemplateId AND "SubSum" = 'Y' AND "Furmula" = 'Y' AND "Param_23" > 0
	UNION ALL
	SELECT "Param_24" as "Param", "CatId", (CASE WHEN "OP_23" = '+' THEN 1 ELSE -1 END) as "Sign" FROM OFRC WHERE "TemplateId" = :TemplateId AND "SubSum" = 'Y' AND "Furmula" = 'Y' AND "Param_24" > 0
	UNION ALL
	SELECT "Param_25" as "Param", "CatId", (CASE WHEN "OP_24" = '+' THEN 1 ELSE -1 END) as "Sign" FROM OFRC WHERE "TemplateId" = :TemplateId AND "SubSum" = 'Y' AND "Furmula" = 'Y' AND "Param_25" > 0;

	--evaluate formulas
	UPDATE T0
	SET T0."Value_Month_cur_Year" =
	(
		SELECT SUM(S0."Value_Month_cur_Year" * S1."Sign") FROM #TMP_TABLE_FINTEMPLATES S0
		INNER JOIN #TMP_TABLE_FORMULA_PARAMS S1 ON S0."Acct_Level" <> 99 AND S0."AccountCode" = S1."Param" AND S1."CatId" = T0."AccountCode"
	)
	FROM #TMP_TABLE_FINTEMPLATES T0
	WHERE T0."SubSum" = 'Y' AND T0."Formula" = 'Y';

	SELECT *, :v_FY_Start AS "FY_Start" FROM #TMP_TABLE_FINTEMPLATES order by "VisOrder", "AcctVisOrder";

	DROP TABLE #TMP_TABLE_FORMULA_PARAMS;
	DROP TABLE #TMP_TABLE_FINTEMPLATES;
END;


