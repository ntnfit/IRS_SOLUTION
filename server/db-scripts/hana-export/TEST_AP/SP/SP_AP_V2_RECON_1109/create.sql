CREATE PROCEDURE "SP_AP_V2_RECON_1109"
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

"PAY_RECON" AS
(
    SELECT
        AP."DocEntry",
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
        ON V2."InvType" = 18
       AND V2."DocEntry" = AP."DocEntry"
    INNER JOIN OVPM OV
        ON OV."DocEntry" = V2."DocNum"
    WHERE OV."Canceled" = 'N'
      AND OV."DocDate" BETWEEN :P_PAYMENT_DATE_FROM AND :P_PAYMENT_DATE_TO

    UNION ALL

    SELECT
        AP."DocEntry",
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
        ON L."APDocEntry" = AP."DocEntry"
    INNER JOIN ODPO DP
        ON DP."DocEntry" = L."DPDocEntry"
    INNER JOIN VPM2 V2
        ON V2."InvType" = 204
       AND V2."DocEntry" = DP."DocEntry"
    INNER JOIN OVPM OV
        ON OV."DocEntry" = V2."DocNum"
    WHERE DP."CANCELED" = 'N'
      AND OV."Canceled" = 'N'
      AND OV."DocDate" BETWEEN :P_PAYMENT_DATE_FROM AND :P_PAYMENT_DATE_TO
),

"PAY_PACK" AS
(
    SELECT
        X."DocEntry",

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
    GROUP BY X."DocEntry"
)

SELECT
    ROW_NUMBER() OVER (ORDER BY AP."DocNum" ASC) AS "#",
    'PU ' || TO_NVARCHAR(AP."DocNum") AS "Document No.",
    AP."DocEntry" AS "AP DocEntry",
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
    IFNULL(AP_LINE."Department", '') AS "Department",
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

    IFNULL(AP."DocTotal", 0)
        - IFNULL(CM."Total A/P Credit Memo", 0)
        - IFNULL(PAY."Total Downpayment Amount", 0)
        - IFNULL(PAY."Total Outgoing Amount", 0) AS "Balance due",

    IFNULL(AP."DocTotalFC", 0)
        - IFNULL(CM."Total A/P Credit Memo FC", 0)
        - IFNULL(PAY."Total Downpayment Amount FC", 0)
        - IFNULL(PAY."Total Outgoing Amount FC", 0) AS "Balance due FC"

FROM AP
LEFT JOIN AP_LINE ON AP_LINE."DocEntry" = AP."DocEntry"
LEFT JOIN PO_PACK PO ON PO."DocEntry" = AP."DocEntry"
LEFT JOIN PR_PACK PR ON PR."DocEntry" = AP."DocEntry"
LEFT JOIN CM_PACK CM ON CM."DocEntry" = AP."DocEntry"
LEFT JOIN PAY_PACK PAY ON PAY."DocEntry" = AP."DocEntry"
LEFT JOIN OCTG PT ON PT."GroupNum" = AP."GroupNum"

ORDER BY AP."DocNum";

END;