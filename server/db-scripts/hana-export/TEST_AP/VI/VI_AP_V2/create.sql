CREATE VIEW "TEST_AP"."VI_AP_V2" ( "#", "Document No.", "Control Account", "Posting Date", "Due Date", "Document Date", "BP Code", "BP Name", "Description", "Số hóa đơn", "Ngày hóa đơn", "PO No.", "Contract No.", "PO Count", "Payment Term", "Department_PR", "PR Count", "Department", "Currency", "Total Payment Due", "A/P Credit Memo No.", "Total A/P Credit Memo", "Net Payment Due", "Downpayment No.", "Downpayment_GL", "Total Downpayment Amount", "Outgoing No.", "Outgoing_GL", "Total Outgoing Amount", "Balance due", "QC Status" ) AS WITH "AP" AS (SELECT
        T0."DocEntry",
        T0."DocNum",
        T0."CtlAccount",
        T0."DocDate",
        T0."DocDueDate",
        T0."TaxDate",
        T0."CardCode",
        T0."CardName",
        T0."Comments",
        T0."NumAtCard",
        T0."DocCur",
        T0."DocTotal",
        T0."GroupNum"
    FROM OPCH T0
    WHERE T0."CANCELED" = 'N'
      AND T0."DocDate" BETWEEN '2025-05-01' AND '2025-05-31'), "AP_LINE" AS (SELECT
        P1."DocEntry",
        STRING_AGG(TO_NVARCHAR(P1."OcrCode"), ', ') AS "Department"
    FROM (
        SELECT DISTINCT
            "DocEntry",
            "OcrCode"
        FROM PCH1
        WHERE "OcrCode" IS NOT NULL
    ) P1
    GROUP BY P1."DocEntry"), "PO_LINK" AS (((SELECT DISTINCT
        P1."DocEntry",
        PO0."DocEntry" AS "PODocEntry",
        PO0."DocNum" AS "PODocNum",
        PO0."U_ContractNo" AS "ContractNo"
    FROM PCH1 P1
    INNER JOIN OPOR PO0
        ON PO0."DocEntry" = P1."BaseEntry"
       AND P1."BaseType" = 22) UNION (SELECT DISTINCT
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
       AND GR1."BaseType" = 22))), "PO_PACK" AS (SELECT
        X."DocEntry",
        STRING_AGG(TO_NVARCHAR(X."PODocNum"), ', ') AS "PO No.",
        STRING_AGG(TO_NVARCHAR(X."ContractNo"), ', ') AS "Contract No.",
        COUNT(*) AS "PO Count"
    FROM (
        SELECT DISTINCT
            "DocEntry",
            "PODocNum",
            "ContractNo"
        FROM PO_LINK
    ) X
    GROUP BY X."DocEntry"), "PR_LINK" AS (SELECT DISTINCT
        PL."DocEntry",
        R1."DocEntry" AS "PRDocEntry",
        R1."OcrCode" AS "Department_PR"
    FROM PO_LINK PL
    INNER JOIN POR1 PO1
        ON PO1."DocEntry" = PL."PODocEntry"
       AND PO1."BaseType" = 1470000113
    INNER JOIN PRQ1 R1
        ON R1."DocEntry" = PO1."BaseEntry"
       AND R1."LineNum" = PO1."BaseLine"
    WHERE R1."OcrCode" IS NOT NULL), "PR_PACK" AS (SELECT
        X."DocEntry",
        STRING_AGG(TO_NVARCHAR(X."Department_PR"), ', ') AS "Department_PR",
        COUNT(*) AS "PR Count"
    FROM (
        SELECT DISTINCT
            "DocEntry",
            "Department_PR"
        FROM PR_LINK
    ) X
    GROUP BY X."DocEntry"), "CM_PACK" AS (SELECT
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
        GROUP BY
            R1."BaseEntry",
            R0."DocNum"
    ) X
    GROUP BY X."DocEntry"), "DP_AMT" AS (SELECT
        X."DocEntry",
        STRING_AGG(TO_NVARCHAR(X."DPDocNum"), ', ') AS "Downpayment No.",
        SUM(X."DrawnSum") AS "Total Downpayment Amount"
    FROM (
        SELECT
            P9."DocEntry",
            P9."BaseAbs",
            DP0."DocNum" AS "DPDocNum",
            SUM(P9."DrawnSum") AS "DrawnSum"
        FROM PCH9 P9
        LEFT JOIN ODPO DP0
            ON DP0."DocEntry" = P9."BaseAbs"
        GROUP BY
            P9."DocEntry",
            P9."BaseAbs",
            DP0."DocNum"
    ) X
    GROUP BY X."DocEntry"), "DP_GL" AS (SELECT
        X."DocEntry",
        STRING_AGG(TO_NVARCHAR(X."AcctCode"), ', ') AS "Downpayment_GL"
    FROM (
        SELECT DISTINCT
            P9."DocEntry",
            D1."AcctCode"
        FROM PCH9 P9
        INNER JOIN DPO1 D1
            ON D1."DocEntry" = P9."BaseAbs"
        WHERE D1."AcctCode" IS NOT NULL
    ) X
    GROUP BY X."DocEntry"), "OG_PACK" AS (SELECT
        X."DocEntry",
        STRING_AGG(TO_NVARCHAR(X."OutgoingNo"), ', ') AS "Outgoing No.",
        STRING_AGG(TO_NVARCHAR(X."OutgoingGL"), ', ') AS "Outgoing_GL",
        SUM(X."OutgoingAmount") AS "Total Outgoing Amount"
    FROM (
        SELECT
            V2."DocEntry",
            V0."DocNum" AS "OutgoingNo",
            V0."TrsfrAcct" AS "OutgoingGL",
            SUM(V2."SumApplied") AS "OutgoingAmount"
        FROM VPM2 V2
        INNER JOIN OVPM V0
            ON V0."DocEntry" = V2."DocNum"
        WHERE V2."InvType" = 18
          AND V0."Canceled" = 'N'
        GROUP BY
            V2."DocEntry",
            V0."DocNum",
            V0."TrsfrAcct"
    ) X
    GROUP BY X."DocEntry") SELECT
    ROW_NUMBER() OVER (ORDER BY AP."DocDate", AP."DocNum") AS "#",

    'PU ' || TO_NVARCHAR(AP."DocNum") AS "Document No.",
    AP."CtlAccount" AS "Control Account",
    AP."DocDate" AS "Posting Date",
    AP."DocDueDate" AS "Due Date",
    AP."TaxDate" AS "Document Date",
    AP."CardCode" AS "BP Code",
    AP."CardName" AS "BP Name",
    AP."Comments" AS "Description",
    AP."NumAtCard" AS "Số hóa đơn",
    AP."TaxDate" AS "Ngày hóa đơn",

    PO."PO No.",
    PO."Contract No.",
    IFNULL(PO."PO Count", 0) AS "PO Count",
    PT."PymntGroup" AS "Payment Term",

    PR."Department_PR",
    IFNULL(PR."PR Count", 0) AS "PR Count",
    AP_LINE."Department",

    AP."DocCur" AS "Currency",
    AP."DocTotal" AS "Total Payment Due",

    CM."A/P Credit Memo No.",
    IFNULL(CM."Total A/P Credit Memo", 0) AS "Total A/P Credit Memo",

    AP."DocTotal" - IFNULL(CM."Total A/P Credit Memo", 0) AS "Net Payment Due",

    DP_AMT."Downpayment No.",
    DP_GL."Downpayment_GL",
    IFNULL(DP_AMT."Total Downpayment Amount", 0) AS "Total Downpayment Amount",

    OG."Outgoing No.",
    OG."Outgoing_GL",
    IFNULL(OG."Total Outgoing Amount", 0) AS "Total Outgoing Amount",

    AP."DocTotal"
      - IFNULL(CM."Total A/P Credit Memo", 0)
      - IFNULL(OG."Total Outgoing Amount", 0) AS "Balance due",

    CASE
        WHEN AP."DocTotal" = 0
             AND IFNULL(DP_AMT."Total Downpayment Amount", 0) > 0
            THEN 'QC01 - AP zero but has downpayment'

        WHEN IFNULL(DP_AMT."Total Downpayment Amount", 0) > AP."DocTotal"
             AND AP."DocTotal" > 0
            THEN 'QC02 - Downpayment greater than AP'

        WHEN IFNULL(OG."Total Outgoing Amount", 0) > AP."DocTotal"
             AND AP."DocTotal" > 0
            THEN 'QC03 - Outgoing greater than AP'

        WHEN AP."DocTotal"
             - IFNULL(CM."Total A/P Credit Memo", 0)
             - IFNULL(OG."Total Outgoing Amount", 0) < 0
            THEN 'QC04 - Negative balance'

        WHEN IFNULL(DP_AMT."Total Downpayment Amount", 0) > 0
             AND OG."Outgoing No." IS NULL
             AND AP."DocTotal" > 0
            THEN 'QC05 - Has downpayment but no outgoing'

        WHEN IFNULL(PO."PO Count", 0) > 1
            THEN 'QC06 - Multiple PO in one AP'

        WHEN IFNULL(PR."PR Count", 0) > 1
            THEN 'QC07 - Multiple PR department in one AP'

        ELSE 'OK'
    END AS "QC Status"

FROM AP

LEFT JOIN AP_LINE
    ON AP_LINE."DocEntry" = AP."DocEntry"

LEFT JOIN PO_PACK PO
    ON PO."DocEntry" = AP."DocEntry"

LEFT JOIN PR_PACK PR
    ON PR."DocEntry" = AP."DocEntry"

LEFT JOIN CM_PACK CM
    ON CM."DocEntry" = AP."DocEntry"

LEFT JOIN DP_AMT
    ON DP_AMT."DocEntry" = AP."DocEntry"

LEFT JOIN DP_GL
    ON DP_GL."DocEntry" = AP."DocEntry"

LEFT JOIN OG_PACK OG
    ON OG."DocEntry" = AP."DocEntry"

LEFT JOIN OCTG PT
    ON PT."GroupNum" = AP."GroupNum" ORDER BY
    AP."DocDate",
    AP."DocNum" WITH READ ONLY