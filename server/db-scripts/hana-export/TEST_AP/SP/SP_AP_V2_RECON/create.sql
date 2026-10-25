CREATE PROCEDURE "SP_AP_V2_RECON"
(
    IN P_AP_POSTING_FROM    DATE,
    IN P_AP_POSTING_TO      DATE,
    IN P_PAYMENT_DATE_FROM  DATE,
    IN P_PAYMENT_DATE_TO    DATE
)
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER
READS SQL DATA
AS
BEGIN

WITH "AP" AS
(
    SELECT
        T0."DocEntry",
        T0."DocNum",
        T0."TransId",
        '18:' || TO_NVARCHAR(T0."DocEntry") AS "SourceKey",
        18 AS "SourceType",
        -1 AS "SourceLine",
        CAST('' AS NVARCHAR(5000)) AS "JEDepartment",
        IFNULL(T0."CtlAccount", '') AS "CtlAccount",
        CAST(T0."DocDate" AS DATE) AS "DocDate",
        CAST(T0."DocDueDate" AS DATE) AS "DocDueDate",
        CAST(T0."TaxDate" AS DATE) AS "TaxDate",
        IFNULL(T0."CardCode", '') AS "CardCode",
        IFNULL(T0."CardName", '') AS "CardName",
        IFNULL(T0."Comments", '') AS "Comments",
        IFNULL(T0."JrnlMemo", '') AS "JrnlMemo",
        IFNULL(T0."U_APInv", '') AS "U_APInv",
        CAST(T0."U_InvDate" AS DATE) AS "U_InvDate",
        IFNULL(T0."U_SoToKhai", '') AS "U_SoToKhai",
        IFNULL(T0."NumAtCard", '') AS "NumAtCard",
        IFNULL(T0."DocCur", '') AS "DocCur",
        IFNULL(T0."U_ContractNo", '') AS "U_ContractNo",
        IFNULL(T0."DocTotal", 0)
            + IFNULL(T0."DpmAmnt", 0)
            + IFNULL(T0."DpmVat", 0) AS "DocTotal",

        IFNULL(T0."DocTotalFC", 0)
            + IFNULL(T0."DpmAmntFC", 0)
            + IFNULL(T0."DpmVatFc", 0) AS "DocTotalFC",

        T0."GroupNum"
    FROM OPCH T0
    WHERE T0."CANCELED" = 'N'
      AND T0."DocDate" BETWEEN :P_AP_POSTING_FROM AND :P_AP_POSTING_TO

    UNION ALL

    -- One payable per supplier credit line; debit rows only settle liabilities.
    SELECT
        J."TransId", J."Number", J."TransId",
        '30:' || TO_NVARCHAR(J."TransId") || ':' || TO_NVARCHAR(L."Line_ID"),
        30, L."Line_ID", CAST(IFNULL(L."ProfitCode", '') AS NVARCHAR(5000)),
        IFNULL(L."Account", ''),
        CAST(L."RefDate" AS DATE), CAST(L."DueDate" AS DATE), CAST(L."TaxDate" AS DATE),
        B."CardCode", B."CardName", IFNULL(J."Memo", ''), IFNULL(L."LineMemo", ''),
        '', CAST(NULL AS DATE), '', '',
        COALESCE(NULLIF(L."FCCurrency", ''), A."MainCurncy"), '',
        IFNULL(L."Credit", 0) - IFNULL(L."Debit", 0),
        IFNULL(L."FCCredit", 0) - IFNULL(L."FCDebit", 0), B."GroupNum"
    FROM OJDT J
    INNER JOIN JDT1 L ON L."TransId" = J."TransId"
    INNER JOIN OCRD B ON B."CardCode" = L."ShortName"
        AND B."CardType" = 'S'
        AND B."DebPayAcct" = L."Account"
    CROSS JOIN OADM A
    WHERE J."TransType" = '30'
      AND IFNULL(L."Credit", 0) - IFNULL(L."Debit", 0) > 0
      AND L."RefDate" BETWEEN :P_AP_POSTING_FROM AND :P_AP_POSTING_TO
),

"AP_LINE" AS
(
    SELECT
        X."DocEntry",
        CAST(STRING_AGG(IFNULL(TO_NVARCHAR(X."OcrCode"), ''), ', ') AS NVARCHAR(5000)) AS "Department"
    FROM (
        SELECT DISTINCT "DocEntry", "OcrCode"
        FROM PCH1
        WHERE "OcrCode" IS NOT NULL
    ) X
    GROUP BY X."DocEntry"
),

"PO_LINK" AS
(
    SELECT DISTINCT
        P1."DocEntry",
        PO0."DocEntry" AS "PODocEntry",
        PO0."DocNum" AS "PODocNum",
        IFNULL(PO0."NumAtCard", '') AS "ContractNo"
    FROM PCH1 P1
    INNER JOIN OPOR PO0
        ON PO0."DocEntry" = P1."BaseEntry"
       AND P1."BaseType" = 22

    UNION

    SELECT DISTINCT
        P1."DocEntry",
        PO0."DocEntry" AS "PODocEntry",
        PO0."DocNum" AS "PODocNum",
        IFNULL(PO0."NumAtCard", '') AS "ContractNo"
    FROM PCH1 P1
    INNER JOIN PDN1 GR1
        ON GR1."DocEntry" = P1."BaseEntry"
       AND P1."BaseType" = 20
    INNER JOIN OPOR PO0
        ON PO0."DocEntry" = GR1."BaseEntry"
       AND GR1."BaseType" = 22
),

"PO_PACK" AS
(
    SELECT
        X."DocEntry",
        CAST(STRING_AGG(IFNULL(TO_NVARCHAR(X."PODocNum"), ''), ', ') AS NVARCHAR(5000)) AS "PO No.",
        CAST(STRING_AGG(IFNULL(TO_NVARCHAR(X."ContractNo"), ''), ', ') AS NVARCHAR(5000)) AS "Contract No.",
        COUNT(*) AS "PO Count"
    FROM (
        SELECT DISTINCT "DocEntry", "PODocNum", "ContractNo"
        FROM PO_LINK
    ) X
    GROUP BY X."DocEntry"
),

"PR_PACK" AS
(
    SELECT
        X."DocEntry",
        CAST(STRING_AGG(IFNULL(TO_NVARCHAR(X."Department_PR"), ''), ', ') AS NVARCHAR(5000)) AS "Department_PR",
        COUNT(*) AS "PR Count"
    FROM (
        SELECT DISTINCT
            PL."DocEntry",
            R1."OcrCode" AS "Department_PR"
        FROM PO_LINK PL
        INNER JOIN POR1 PO1
            ON PO1."DocEntry" = PL."PODocEntry"
           AND PO1."BaseType" = 1470000113
        INNER JOIN PRQ1 R1
            ON R1."DocEntry" = PO1."BaseEntry"
           AND R1."LineNum" = PO1."BaseLine"
        WHERE R1."OcrCode" IS NOT NULL
    ) X
    GROUP BY X."DocEntry"
),

"CM_PACK" AS
(
    SELECT
        X."DocEntry",
        CAST(STRING_AGG(IFNULL(TO_NVARCHAR(X."CMDocNum"), ''), ', ') AS NVARCHAR(5000)) AS "A/P Credit Memo No.",
        SUM(X."CMAmount") AS "Total A/P Credit Memo",
        SUM(X."CMAmountFC") AS "Total A/P Credit Memo FC"
    FROM (
        SELECT
            R1."BaseEntry" AS "DocEntry",
            R0."DocNum" AS "CMDocNum",
            SUM(IFNULL(R1."LineTotal", 0)) AS "CMAmount",
            SUM(IFNULL(R1."TotalFrgn", 0)) AS "CMAmountFC"
        FROM ORPC R0
        INNER JOIN RPC1 R1
            ON R1."DocEntry" = R0."DocEntry"
        WHERE R0."CANCELED" = 'N'
          AND R1."BaseType" = 18
        GROUP BY R1."BaseEntry", R0."DocNum"
    ) X
    GROUP BY X."DocEntry"
),

"AP_DP_LINK" AS
(
    SELECT
        P9."DocEntry" AS "APDocEntry",
        P9."BaseAbs"  AS "DPDocEntry",
        SUM(IFNULL(P9."DrawnSum", 0)) AS "DP Applied Amount",
        SUM(IFNULL(P9."DrawnSumFc", 0)) AS "DP Applied Amount FC"
    FROM PCH9 P9
    GROUP BY
        P9."DocEntry",
        P9."BaseAbs"
),

"RECON_ROWS" AS
(
    SELECT R.*
    FROM ITR1 R
    INNER JOIN OITR H ON H."ReconNum" = R."ReconNum"
    WHERE IFNULL(H."Canceled", 'N') = 'N'
      AND (:P_PAYMENT_DATE_FROM IS NULL OR H."ReconDate" >= :P_PAYMENT_DATE_FROM)
      AND H."ReconDate" <= IFNULL(:P_PAYMENT_DATE_TO, CURRENT_DATE)
),
"JE_SETTLED" AS
(
    -- For JE liabilities the source line records the exact total settlement,
    -- including on-account payments/credit memos without a VPM2 invoice link.
    SELECT AP."SourceKey",
        SUM(ABS(IFNULL(R."ReconSum", 0))) AS "Settled",
        SUM(ABS(IFNULL(R."ReconSumFC", 0))) AS "SettledFC"
    FROM AP
    INNER JOIN RECON_ROWS R ON R."TransId" = AP."TransId"
        AND R."TransRowId" = AP."SourceLine" AND R."ShortName" = AP."CardCode"
        AND R."IsCredit" = 'C'
    WHERE AP."SourceType" = 30
    GROUP BY AP."SourceKey"
),
"RECON_TOTALS" AS
(
    SELECT R."ReconNum", R."ShortName", R."Account",
        SUM(CASE WHEN R."IsCredit" = 'D' THEN ABS(IFNULL(R."ReconSum", 0)) ELSE 0 END) AS "DebitTotal",
        SUM(CASE WHEN R."IsCredit" = 'C' THEN 1 ELSE 0 END) AS "CreditCount"
    FROM RECON_ROWS R
    GROUP BY R."ReconNum", R."ShortName", R."Account"
),
"JE_MATCH" AS
(
    -- Aggregate counterpart rows BEFORE summing the source's reconciled amount.
    SELECT AP."SourceKey", R1."ReconNum", R1."LineSeq",
        MAX(ABS(IFNULL(R1."ReconSum", 0))) AS "SourceAmount",
        MAX(ABS(IFNULL(R1."ReconSumFC", 0))) AS "SourceAmountFC",
        MAX(T."DebitTotal") AS "DebitTotal", MAX(T."CreditCount") AS "CreditCount",
        SUM(ABS(IFNULL(R2."ReconSum", 0))) AS "JEAmount",
        SUM(CASE WHEN IFNULL(R2."FrgnCurr", '') = IFNULL(R1."FrgnCurr", '')
                 THEN ABS(IFNULL(R2."ReconSumFC", 0)) ELSE 0 END) AS "JEAmountFC"
    FROM AP
    INNER JOIN JDT1 L ON L."TransId" = AP."TransId"
        AND L."ShortName" = AP."CardCode"
        AND (AP."SourceType" = 18 OR L."Line_ID" = AP."SourceLine")
    INNER JOIN RECON_ROWS R1 ON R1."TransId" = L."TransId"
        AND R1."TransRowId" = L."Line_ID" AND R1."ShortName" = AP."CardCode"
        AND R1."IsCredit" = 'C'
    INNER JOIN RECON_TOTALS T ON T."ReconNum" = R1."ReconNum"
        AND T."ShortName" = R1."ShortName" AND T."Account" = L."Account"
    INNER JOIN RECON_ROWS R2 ON R2."ReconNum" = R1."ReconNum"
        AND R2."ShortName" = R1."ShortName" AND R2."IsCredit" = 'D'
        AND R2."Account" = L."Account"
        AND R2."SrcObjTyp" = '30' AND R2."TransId" <> R1."TransId"
    GROUP BY AP."SourceKey", R1."ReconNum", R1."LineSeq"
),
"JE_NUMBERS" AS
(
    SELECT X."SourceKey",
        CAST(STRING_AGG(TO_NVARCHAR(X."TransId"), ', ') AS NVARCHAR(5000)) AS "JE Recon No."
    FROM (
        SELECT DISTINCT M."SourceKey", R."TransId"
        FROM JE_MATCH M
        INNER JOIN AP ON AP."SourceKey" = M."SourceKey"
        INNER JOIN RECON_ROWS R ON R."ReconNum" = M."ReconNum"
            AND R."ShortName" = AP."CardCode" AND R."IsCredit" = 'D'
            AND R."SrcObjTyp" = '30' AND R."TransId" <> AP."TransId"
    ) X
    GROUP BY X."SourceKey"
),
"JE_RECON" AS
(
    -- ITR1 contains group membership, not a many-to-many allocation matrix.
    -- All debit rows JE: use the source amount. One credit row: use JE debit amounts.
    -- Mixed many-to-many: NULL JE attribution and AP balance; JE balance uses JE_SETTLED.
    SELECT "SourceKey",
        SUM(CASE WHEN "JEAmount" = "DebitTotal" THEN "SourceAmount"
                 WHEN "CreditCount" = 1 THEN "JEAmount" ELSE 0 END) AS "JE Recon Amount",
        SUM(CASE WHEN "JEAmount" = "DebitTotal" THEN "SourceAmountFC"
                 WHEN "CreditCount" = 1 THEN "JEAmountFC" ELSE 0 END) AS "JE Recon Amount FC",
        MAX(CASE WHEN "JEAmount" <> "DebitTotal" AND "CreditCount" > 1 THEN 1 ELSE 0 END) AS "NeedsReview"
    FROM JE_MATCH
    GROUP BY "SourceKey"
),

"PAY_RECON" AS
(
    SELECT
        AP."SourceKey",
        CAST(OV."DocDate" AS DATE) AS "Payment date",
        CAST(NULL AS DATE) AS "Downpayment Date",
        TO_NVARCHAR(OV."DocNum") AS "Payment_No",

        CASE
            WHEN IFNULL(OV."TrsfrSum", 0) <> 0 THEN OV."TrsfrAcct"
            WHEN IFNULL(OV."CashSum", 0) <> 0 THEN OV."CashAcct"
            WHEN IFNULL(OV."CheckSum", 0) <> 0 THEN OV."CheckAcct"
            ELSE ''
        END AS "Payment_GL",

        IFNULL(V2."SumApplied", 0) AS "Payment Amount",
        IFNULL(V2."AppliedFC", 0) AS "Payment Amount FC",

        'Outgoing' AS "Payment type"
    FROM AP
    INNER JOIN VPM2 V2
        ON V2."InvType" = TO_NVARCHAR(AP."SourceType")
       AND V2."DocEntry" = AP."DocEntry"
       AND (AP."SourceType" = 18 OR V2."DocLine" = AP."SourceLine")
    INNER JOIN OVPM OV
        ON OV."DocEntry" = V2."DocNum"
    WHERE OV."Canceled" = 'N'
      AND (:P_PAYMENT_DATE_FROM IS NULL OR OV."DocDate" >= :P_PAYMENT_DATE_FROM)
      AND OV."DocDate" <= IFNULL(:P_PAYMENT_DATE_TO, CURRENT_DATE)

    UNION ALL

    SELECT
        AP."SourceKey",
        CAST(OV."DocDate" AS DATE) AS "Payment date",
        CAST(DP."DocDate" AS DATE) AS "Downpayment Date",
        TO_NVARCHAR(OV."DocNum") AS "Payment_No",

        CASE
            WHEN IFNULL(OV."TrsfrSum", 0) <> 0 THEN OV."TrsfrAcct"
            WHEN IFNULL(OV."CashSum", 0) <> 0 THEN OV."CashAcct"
            WHEN IFNULL(OV."CheckSum", 0) <> 0 THEN OV."CheckAcct"
            ELSE ''
        END AS "Payment_GL",

        IFNULL(L."DP Applied Amount", 0) AS "Payment Amount",
        IFNULL(L."DP Applied Amount FC", 0) AS "Payment Amount FC",
        'Downpayment' AS "Payment type"
    FROM AP
    INNER JOIN AP_DP_LINK L
        ON L."APDocEntry" = AP."DocEntry" AND AP."SourceType" = 18
    INNER JOIN ODPO DP
        ON DP."DocEntry" = L."DPDocEntry"
    INNER JOIN VPM2 V2
        ON V2."InvType" = 204
       AND V2."DocEntry" = DP."DocEntry"
    INNER JOIN OVPM OV
        ON OV."DocEntry" = V2."DocNum"
    WHERE DP."CANCELED" = 'N'
      AND OV."Canceled" = 'N'
      AND (:P_PAYMENT_DATE_FROM IS NULL OR OV."DocDate" >= :P_PAYMENT_DATE_FROM)
      AND OV."DocDate" <= IFNULL(:P_PAYMENT_DATE_TO, CURRENT_DATE)
),

"PAY_PACK" AS
(
    SELECT
        X."SourceKey",

        CAST(STRING_AGG(
            CASE WHEN X."Payment type" = 'Downpayment'
                 THEN TO_NVARCHAR(X."Payment_No")
            END,
            ', '
        ) AS NVARCHAR(5000)) AS "Downpayment No.",

        CAST(STRING_AGG(
            CASE WHEN X."Payment type" = 'Downpayment'
                 THEN TO_NVARCHAR(X."Downpayment Date", 'YYYY-MM-DD')
            END,
            ', '
        ) AS NVARCHAR(5000)) AS "Downpayment Date",

        CAST(STRING_AGG(
            CASE WHEN X."Payment type" = 'Downpayment'
                 THEN TO_NVARCHAR(X."Payment_GL")
            END,
            ', '
        ) AS NVARCHAR(5000)) AS "Downpayment_GL",

        SUM(
            CASE WHEN X."Payment type" = 'Downpayment'
                 THEN X."Payment Amount"
                 ELSE 0
            END
        ) AS "Total Downpayment Amount",

        SUM(
            CASE WHEN X."Payment type" = 'Downpayment'
                 THEN X."Payment Amount FC"
                 ELSE 0
            END
        ) AS "Total Downpayment Amount FC",

        CAST(STRING_AGG(
            CASE WHEN X."Payment type" = 'Outgoing'
                 THEN TO_NVARCHAR(X."Payment_No")
            END,
            ', '
        ) AS NVARCHAR(5000)) AS "Outgoing No.",

        CAST(STRING_AGG(
            CASE WHEN X."Payment type" = 'Outgoing'
                 THEN TO_NVARCHAR(X."Payment_GL")
            END,
            ', '
        ) AS NVARCHAR(5000)) AS "Outgoing_GL",

        SUM(
            CASE WHEN X."Payment type" = 'Outgoing'
                 THEN X."Payment Amount"
                 ELSE 0
            END
        ) AS "Total Outgoing Amount",

        SUM(
            CASE WHEN X."Payment type" = 'Outgoing'
                 THEN X."Payment Amount FC"
                 ELSE 0
            END
        ) AS "Total Outgoing Amount FC",

        SUM(X."Payment Amount") AS "Total Payment Amount",
        SUM(X."Payment Amount FC") AS "Total Payment Amount FC"
    FROM PAY_RECON X
    GROUP BY X."SourceKey"
)

SELECT
    ROW_NUMBER() OVER (ORDER BY AP."DocNum", AP."SourceType", AP."SourceLine") AS "#",
    CASE WHEN AP."SourceType" = 30 THEN 'JE ' ELSE 'PU ' END || TO_NVARCHAR(AP."DocNum") AS "Document No.",
    AP."DocEntry" AS "AP DocEntry",
    AP."SourceType" AS "Source Object Type",
    AP."SourceLine" AS "JE Line ID",
    AP."CtlAccount" AS "Control Account",
    AP."DocDate" AS "Posting Date",
    AP."DocDueDate" AS "Due Date",
    AP."TaxDate" AS "Document Date",
    AP."CardCode" AS "BP Code",
    AP."CardName" AS "BP Name",
    AP."JrnlMemo" AS "Description",
    IFNULL(AP."U_APInv", '') AS "Số hóa đơn",
    IFNULL(TO_NVARCHAR(AP."U_InvDate"), '') AS "Ngày hóa đơn",
    IFNULL(AP."U_SoToKhai", '') AS "Số tờ khai",
    IFNULL(PO."PO No.", '') AS "PO No.",
    IFNULL(AP."U_ContractNo", '') AS "Contract No",
    IFNULL(PT."PymntGroup", '') AS "Payment Term",
    IFNULL(PR."Department_PR", '') AS "Department_PR",
    CASE WHEN AP."SourceType" = 30 THEN AP."JEDepartment" ELSE IFNULL(AP_LINE."Department", '') END AS "Department",
    AP."DocCur" AS "Currency",

    AP."DocTotal" AS "Total Payment Due",
    AP."DocTotalFC" AS "Total Payment Due FC",

    IFNULL(CM."A/P Credit Memo No.", '') AS "A/P Credit Memo No.",
    IFNULL(CM."Total A/P Credit Memo", 0) AS "Total A/P Credit Memo",
    IFNULL(CM."Total A/P Credit Memo FC", 0) AS "Total A/P Credit Memo FC",

    AP."DocTotal"
        - IFNULL(CM."Total A/P Credit Memo", 0) AS "Net Payment Due",

    AP."DocTotalFC"
        - IFNULL(CM."Total A/P Credit Memo FC", 0) AS "Net Payment Due FC",

    IFNULL(PAY."Downpayment No.", '') AS "Downpayment No.",
    IFNULL(PAY."Downpayment Date", '') AS "Downpayment Date",
    IFNULL(PAY."Downpayment_GL", '') AS "Downpayment_GL",
    IFNULL(PAY."Total Downpayment Amount", 0) AS "Total Downpayment Amount",
    IFNULL(PAY."Total Downpayment Amount FC", 0) AS "Total Downpayment Amount FC",

    IFNULL(PAY."Outgoing No.", '') AS "Outgoing No.",
    IFNULL(PAY."Outgoing_GL", '') AS "Outgoing_GL",
    IFNULL(PAY."Total Outgoing Amount", 0) AS "Total Outgoing Amount",
    IFNULL(PAY."Total Outgoing Amount FC", 0) AS "Total Outgoing Amount FC",

    IFNULL(JN."JE Recon No.", '') AS "JE Recon No.",
    CASE WHEN IFNULL(JE."NeedsReview", 0) = 1 THEN NULL ELSE IFNULL(JE."JE Recon Amount", 0) END AS "JE Recon Amount",
    CASE WHEN IFNULL(JE."NeedsReview", 0) = 1 THEN NULL ELSE IFNULL(JE."JE Recon Amount FC", 0) END AS "JE Recon Amount FC",

    CASE WHEN AP."SourceType" = 30 THEN AP."DocTotal" - IFNULL(JS."Settled", 0)
         WHEN IFNULL(JE."NeedsReview", 0) = 1 THEN NULL ELSE IFNULL(AP."DocTotal", 0)
        - IFNULL(CM."Total A/P Credit Memo", 0)
        - IFNULL(PAY."Total Downpayment Amount", 0)
        - IFNULL(PAY."Total Outgoing Amount", 0)
        - IFNULL(JE."JE Recon Amount", 0) END AS "Balance due",

    CASE WHEN AP."SourceType" = 30 THEN AP."DocTotalFC" - IFNULL(JS."SettledFC", 0)
         WHEN IFNULL(JE."NeedsReview", 0) = 1 THEN NULL ELSE IFNULL(AP."DocTotalFC", 0)
        - IFNULL(CM."Total A/P Credit Memo FC", 0)
        - IFNULL(PAY."Total Downpayment Amount FC", 0)
        - IFNULL(PAY."Total Outgoing Amount FC", 0)
        - IFNULL(JE."JE Recon Amount FC", 0) END AS "Balance due FC",
    CASE WHEN IFNULL(JE."NeedsReview", 0) = 1 THEN 'Review mixed reconciliation'
         ELSE '' END AS "JE Recon Status"

FROM AP
LEFT JOIN AP_LINE ON AP_LINE."DocEntry" = AP."DocEntry" AND AP."SourceType" = 18
LEFT JOIN PO_PACK PO ON PO."DocEntry" = AP."DocEntry" AND AP."SourceType" = 18
LEFT JOIN PR_PACK PR ON PR."DocEntry" = AP."DocEntry" AND AP."SourceType" = 18
LEFT JOIN CM_PACK CM ON CM."DocEntry" = AP."DocEntry" AND AP."SourceType" = 18
LEFT JOIN PAY_PACK PAY ON PAY."SourceKey" = AP."SourceKey"
LEFT JOIN JE_SETTLED JS ON JS."SourceKey" = AP."SourceKey"
LEFT JOIN JE_RECON JE ON JE."SourceKey" = AP."SourceKey"
LEFT JOIN JE_NUMBERS JN ON JN."SourceKey" = AP."SourceKey"
LEFT JOIN OCTG PT ON PT."GroupNum" = AP."GroupNum"

ORDER BY AP."DocNum", AP."SourceType", AP."SourceLine";

END;
