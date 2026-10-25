CREATE PROCEDURE "TEST_AP"."SP_AP_V3"
(
    IN P_PAYMENT_POSTING_FROM DATE,
    IN P_PAYMENT_POSTING_TO   DATE,
    IN P_AP_POSTING_TO        DATE
)
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER
READS SQL DATA
AS
BEGIN

WITH "PARAM" AS
(
    SELECT
        :P_PAYMENT_POSTING_FROM AS "PAY_FROM",
        IFNULL(:P_PAYMENT_POSTING_TO, CURRENT_DATE) AS "PAY_TO",
        IFNULL(:P_AP_POSTING_TO, IFNULL(:P_PAYMENT_POSTING_TO, CURRENT_DATE)) AS "AP_TO"
    FROM DUMMY
),

"AP" AS
(
    SELECT
        T0."DocEntry",
        T0."DocNum",
        T0."TransId",
        T0."CtlAccount",
        T0."DocDate",
        T0."DocDueDate",
        T0."TaxDate",
        T0."CardCode",
        T0."CardName",
        T0."Comments",
        T0."NumAtCard",
        NULL AS "SoToKhai",
        T0."DocCur",
        T0."DocTotal",
        T0."GroupNum"
    FROM OPCH T0
    CROSS JOIN PARAM P
    WHERE T0."CANCELED" = 'N'
      AND T0."DocDate" <= P."AP_TO"
),

"AP_LINE" AS
(
    SELECT
        X."DocEntry",
        STRING_AGG(TO_NVARCHAR(X."OcrCode"), ', ') AS "Department"
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
        PO0."U_ContractNo" AS "ContractNo"
    FROM PCH1 P1
    INNER JOIN OPOR PO0
        ON PO0."DocEntry" = P1."BaseEntry"
       AND P1."BaseType" = 22

    UNION

    SELECT DISTINCT
        P1."DocEntry",
        PO0."DocEntry" AS "PODocEntry",
        PO0."DocNum" AS "PODocNum",
        PO0."U_ContractNo" AS "ContractNo"
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
        STRING_AGG(TO_NVARCHAR(X."PODocNum"), ', ') AS "PO No.",
        STRING_AGG(TO_NVARCHAR(X."ContractNo"), ', ') AS "Contract No."
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
        STRING_AGG(TO_NVARCHAR(X."Department_PR"), ', ') AS "Department_PR"
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
        STRING_AGG(TO_NVARCHAR(X."CMDocNum"), ', ') AS "A/P Credit Memo No.",
        SUM(X."CMAmount") AS "Total A/P Credit Memo"
    FROM (
        SELECT
            R1."BaseEntry" AS "DocEntry",
            R0."DocNum" AS "CMDocNum",
            SUM(R1."LineTotal") AS "CMAmount"
        FROM ORPC R0
        INNER JOIN RPC1 R1
            ON R1."DocEntry" = R0."DocEntry"
        WHERE R0."CANCELED" = 'N'
          AND R1."BaseType" = 18
        GROUP BY R1."BaseEntry", R0."DocNum"
    ) X
    GROUP BY X."DocEntry"
),

"DP_DRAW" AS
(
    SELECT
        P9."BaseAbs" AS "DPDocEntry",
        P9."DocEntry" AS "APDocEntry",
        SUM(P9."DrawnSum") AS "DrawnAmount"
    FROM PCH9 P9
    GROUP BY P9."BaseAbs", P9."DocEntry"
),

"DP_DRAW_TOTAL" AS
(
    SELECT
        "DPDocEntry",
        SUM("DrawnAmount") AS "TotalDrawnAmount"
    FROM DP_DRAW
    GROUP BY "DPDocEntry"
),

"DP_PAY_TOTAL" AS
(
    SELECT
        V2."DocEntry" AS "DPDocEntry",
        SUM(V2."SumApplied") AS "TotalPaidAmount"
    FROM VPM2 V2
    INNER JOIN OVPM V0
        ON V0."DocEntry" = V2."DocNum"
    WHERE V2."InvType" = 204
      AND V0."Canceled" = 'N'
    GROUP BY V2."DocEntry"
),

"PAYMENT_ROWS" AS
(
    /* Case Outgoing payment apply directly to AP Invoice */
    SELECT
        AP."DocEntry" AS "APDocEntry",
        AP."TransId" AS "Transaction No.",
        V0."DocDate" AS "Payment date",
        'PS ' || TO_NVARCHAR(V0."DocNum") AS "Payment No.",
        V0."TrsfrAcct" AS "Payment_GL",
        V2."SumApplied" AS "Payment Amount",
        'Outgoing' AS "Payment type",
        1 AS "SortType"
    FROM AP
    INNER JOIN VPM2 V2
        ON V2."DocEntry" = AP."DocEntry"
       AND V2."InvType" = 18
    INNER JOIN OVPM V0
        ON V0."DocEntry" = V2."DocNum"
    CROSS JOIN PARAM P
    WHERE V0."Canceled" = 'N'
      AND V0."DocDate" BETWEEN P."PAY_FROM" AND P."PAY_TO"

    UNION ALL

    /* Case Downpayment đã mapping AP Invoice */
    SELECT
        DD."APDocEntry" AS "APDocEntry",
        AP."TransId" AS "Transaction No.",
        V0."DocDate" AS "Payment date",
        'PS ' || TO_NVARCHAR(V0."DocNum") AS "Payment No.",
        V0."TrsfrAcct" AS "Payment_GL",
        DD."DrawnAmount"
            * V2."SumApplied"
            / NULLIF(DPT."TotalPaidAmount", 0) AS "Payment Amount",
        'Downpayment' AS "Payment type",
        0 AS "SortType"
    FROM VPM2 V2
    INNER JOIN OVPM V0
        ON V0."DocEntry" = V2."DocNum"
    INNER JOIN DP_DRAW DD
        ON DD."DPDocEntry" = V2."DocEntry"
    INNER JOIN DP_PAY_TOTAL DPT
        ON DPT."DPDocEntry" = V2."DocEntry"
    INNER JOIN AP
        ON AP."DocEntry" = DD."APDocEntry"
    CROSS JOIN PARAM P
    WHERE V2."InvType" = 204
      AND V0."Canceled" = 'N'
      AND V0."DocDate" BETWEEN P."PAY_FROM" AND P."PAY_TO"

    UNION ALL

    /* Case AP Down Payment chưa mapping AP Invoice */
    SELECT
        NULL AS "APDocEntry",
        NULL AS "Transaction No.",
        V0."DocDate" AS "Payment date",
        'PS ' || TO_NVARCHAR(V0."DocNum") AS "Payment No.",
        V0."TrsfrAcct" AS "Payment_GL",
        V2."SumApplied"
            - IFNULL(DT."TotalDrawnAmount", 0)
              * V2."SumApplied"
              / NULLIF(DPT."TotalPaidAmount", 0) AS "Payment Amount",
        'Downpayment' AS "Payment type",
        0 AS "SortType"
    FROM VPM2 V2
    INNER JOIN OVPM V0
        ON V0."DocEntry" = V2."DocNum"
    INNER JOIN ODPO DP
        ON DP."DocEntry" = V2."DocEntry"
    INNER JOIN DP_PAY_TOTAL DPT
        ON DPT."DPDocEntry" = V2."DocEntry"
    LEFT JOIN DP_DRAW_TOTAL DT
        ON DT."DPDocEntry" = V2."DocEntry"
    CROSS JOIN PARAM P
    WHERE V2."InvType" = 204
      AND V0."Canceled" = 'N'
      AND V0."DocDate" BETWEEN P."PAY_FROM" AND P."PAY_TO"
      AND V2."SumApplied"
            - IFNULL(DT."TotalDrawnAmount", 0)
              * V2."SumApplied"
              / NULLIF(DPT."TotalPaidAmount", 0) > 0
)

SELECT
    ROW_NUMBER() OVER
    (
        ORDER BY
            PR."Payment date",
            PR."Payment No.",
            PR."APDocEntry",
            PR."SortType"
    ) AS "#",

    PR."Transaction No.",
    AP."CtlAccount" AS "Control Account",
    CASE WHEN AP."DocNum" IS NOT NULL
        THEN 'PU ' || TO_NVARCHAR(AP."DocNum")
    END AS "Document No.",
    AP."DocDate" AS "Posting Date",
    AP."DocDueDate" AS "Due Date",
    AP."TaxDate" AS "Document Date",
    AP."CardCode" AS "BP Code",
    AP."CardName" AS "BP Name",
    AP."Comments" AS "Description",
    AP."NumAtCard" AS "Số hóa đơn",
    AP."TaxDate" AS "Ngày hóa đơn",
    AP."SoToKhai" AS "Số tờ khai",
    PO."PO No.",
    PO."Contract No.",
    PT."PymntGroup" AS "Payment Term",
    PRP."Department_PR",
    AP_LINE."Department",
    AP."DocCur" AS "Currency",

    AP."DocTotal" AS "Total Payment Due",
    CM."A/P Credit Memo No.",
    IFNULL(CM."Total A/P Credit Memo", 0) AS "Total A/P Credit Memo",
    AP."DocTotal" - IFNULL(CM."Total A/P Credit Memo", 0) AS "Net Payment Due",

    PR."Payment date",
    PR."Payment No.",
    PR."Payment_GL",
    PR."Payment Amount",
    PR."Payment type",

    CASE
        WHEN AP."DocEntry" IS NULL
            THEN 0 - PR."Payment Amount"
        ELSE AP."DocTotal"
             - IFNULL(CM."Total A/P Credit Memo", 0)
             - PR."Payment Amount"
    END AS "Balance due"

FROM PAYMENT_ROWS PR
LEFT JOIN AP
    ON AP."DocEntry" = PR."APDocEntry"
LEFT JOIN AP_LINE
    ON AP_LINE."DocEntry" = AP."DocEntry"
LEFT JOIN PO_PACK PO
    ON PO."DocEntry" = AP."DocEntry"
LEFT JOIN PR_PACK PRP
    ON PRP."DocEntry" = AP."DocEntry"
LEFT JOIN CM_PACK CM
    ON CM."DocEntry" = AP."DocEntry"
LEFT JOIN OCTG PT
    ON PT."GroupNum" = AP."GroupNum"
ORDER BY
    PR."Payment date",
    PR."Payment No.",
    PR."APDocEntry",
    PR."SortType";

END;