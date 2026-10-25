CREATE PROCEDURE "SP_AP_V3_RECON"
(
    IN P_PAYMENT_DATE_FROM  DATE,
    IN P_PAYMENT_DATE_TO    DATE,
    IN P_AP_POSTING_FROM    DATE,
    IN P_AP_POSTING_TO      DATE
)
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER
READS SQL DATA
AS
BEGIN

DECLARE V_ASOF_DATE DATE;
V_ASOF_DATE := IFNULL(:P_PAYMENT_DATE_TO, CURRENT_DATE);

WITH "AP" AS
(
    SELECT
        T0."DocEntry",
        T0."DocNum",
        T0."TransId",
        IFNULL(T0."CtlAccount", '') AS "CtlAccount",
        CAST(T0."DocDate" AS DATE) AS "DocDate",
        CAST(T0."DocDueDate" AS DATE) AS "DocDueDate",
        CAST(T0."TaxDate" AS DATE) AS "TaxDate",
        IFNULL(T0."CardCode", '') AS "CardCode",
        IFNULL(T0."CardName", '') AS "CardName",
        IFNULL(T0."DocCur", '') AS "DocCur",
        IFNULL(T0."U_ContractNo", '') AS "U_ContractNo",

        IFNULL(T0."DocTotal", 0)
            + IFNULL(T0."DpmAmnt", 0)
            + IFNULL(T0."DpmVat", 0) AS "DocTotal",

        IFNULL(T0."DocTotalFC", 0)
            + IFNULL(T0."DpmAmntFC", 0)
            + IFNULL(T0."DpmVatFc", 0) AS "DocTotalFC",

        IFNULL(T0."JrnlMemo", '') AS "JrnlMemo",
        IFNULL(T0."U_APInv", '') AS "U_APInv",
        CAST(T0."U_InvDate" AS DATE) AS "U_InvDate",
        T0."GroupNum",
        IFNULL(T0."U_SoToKhai", '') AS "U_SoToKhai"
    FROM OPCH T0
    WHERE T0."CANCELED" = 'N'
      AND T0."DocDate" <= :V_ASOF_DATE
      AND (:P_AP_POSTING_FROM IS NULL OR T0."DocDate" >= :P_AP_POSTING_FROM)
      AND (:P_AP_POSTING_TO IS NULL OR T0."DocDate" <= :P_AP_POSTING_TO)
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

"DP_LINE" AS
(
    SELECT
        X."DocEntry",
        CAST(STRING_AGG(IFNULL(TO_NVARCHAR(X."OcrCode"), ''), ', ') AS NVARCHAR(5000)) AS "Department"
    FROM (
        SELECT DISTINCT "DocEntry", "OcrCode"
        FROM DPO1
        WHERE "OcrCode" IS NOT NULL
    ) X
    GROUP BY X."DocEntry"
),

"DP_PO_LINK" AS
(
    SELECT DISTINCT
        D1."DocEntry" AS "DPDocEntry",
        PO0."DocEntry" AS "PODocEntry",
        PO0."DocNum" AS "PODocNum",
        IFNULL(PO0."U_ContractNo", '') AS "ContractNo"
    FROM DPO1 D1
    INNER JOIN OPOR PO0
        ON PO0."DocEntry" = D1."BaseEntry"
       AND D1."BaseType" = 22
    WHERE PO0."CANCELED" = 'N'
),

"DP_PO_PACK" AS
(
    SELECT
        X."DPDocEntry",
        CAST(STRING_AGG(IFNULL(TO_NVARCHAR(X."PODocNum"), ''), ', ' ORDER BY X."PODocNum") AS NVARCHAR(5000)) AS "PO No.",
        CAST(STRING_AGG(IFNULL(TO_NVARCHAR(X."ContractNo"), ''), ', ' ORDER BY X."PODocNum") AS NVARCHAR(5000)) AS "Contract No."
    FROM
    (
        SELECT DISTINCT
            "DPDocEntry",
            "PODocNum",
            "ContractNo"
        FROM DP_PO_LINK
    ) X
    GROUP BY X."DPDocEntry"
),

"PO_LINK" AS
(
    SELECT DISTINCT
        P1."DocEntry",
        PO0."DocEntry" AS "PODocEntry",
        PO0."DocNum" AS "PODocNum",
        IFNULL(PO0."U_ContractNo", '') AS "ContractNo"
    FROM PCH1 P1
    INNER JOIN OPOR PO0
        ON PO0."DocEntry" = P1."BaseEntry"
       AND P1."BaseType" = 22

    UNION

    SELECT DISTINCT
        P1."DocEntry",
        PO0."DocEntry" AS "PODocEntry",
        PO0."DocNum" AS "PODocNum",
        IFNULL(PO0."U_ContractNo", '') AS "ContractNo"
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
        CAST(STRING_AGG(IFNULL(TO_NVARCHAR(X."ContractNo"), ''), ', ') AS NVARCHAR(5000)) AS "Contract No."
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
        CAST(STRING_AGG(IFNULL(TO_NVARCHAR(X."Department_PR"), ''), ', ') AS NVARCHAR(5000)) AS "Department_PR"
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
          AND R0."DocDate" <= :V_ASOF_DATE
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

"DP_APPLIED_ASOF" AS
(
    SELECT
        P9."BaseAbs" AS "DPDocEntry",
        SUM(IFNULL(P9."DrawnSum", 0)) AS "Applied Amount AsOf",
        SUM(IFNULL(P9."DrawnSumFc", 0)) AS "Applied Amount FC AsOf"
    FROM PCH9 P9
    INNER JOIN OPCH AP0
        ON AP0."DocEntry" = P9."DocEntry"
    WHERE AP0."CANCELED" = 'N'
      AND AP0."DocDate" <= :V_ASOF_DATE
    GROUP BY P9."BaseAbs"
),

"RECON_ASOF" AS
(
    SELECT
        R1."TransId",
        R1."TransRowId",
        R1."ReconNum",
        R1."IsCredit",
        IFNULL(R1."ReconSum", 0) AS "ReconSum",
        IFNULL(R1."ReconSumFC", 0) AS "ReconSumFC",
        CAST(R0."ReconDate" AS DATE) AS "ReconDate"
    FROM ITR1 R1
    INNER JOIN OITR R0
        ON R0."ReconNum" = R1."ReconNum"
       AND IFNULL(R0."Canceled", 'N') = 'N'
    WHERE R0."ReconDate" <= :V_ASOF_DATE
),

"DOC_PAYMENT_RECON_ASOF" AS
(
    SELECT DISTINCT
        DOC."TransId" AS "DocTransId",
        PAY."TransId" AS "PaymentTransId"
    FROM RECON_ASOF DOC
    INNER JOIN RECON_ASOF PAY
        ON PAY."ReconNum" = DOC."ReconNum"
    WHERE DOC."TransId" <> PAY."TransId"
      AND PAY."IsCredit" = 'D'
),

"PAYMENT_RECON_NUM" AS
(
    SELECT
        X."PaymentTransId",
        CAST(STRING_AGG(IFNULL(TO_NVARCHAR(X."ReconNum"), ''), '; ' ORDER BY X."ReconNum") AS NVARCHAR(5000)) AS "ReconNum"
    FROM
    (
        SELECT DISTINCT
            OV."TransId" AS "PaymentTransId",
            R1."ReconNum"
        FROM OVPM OV
        INNER JOIN RECON_ASOF R1
            ON R1."TransId" = OV."TransId"
        WHERE OV."Canceled" = 'N'
    ) X
    GROUP BY X."PaymentTransId"
),

"PAYMENT_RECON_AMOUNT_ASOF" AS
(
    SELECT
        R1."TransId" AS "PaymentTransId",
        SUM(IFNULL(R1."ReconSum", 0)) AS "ReconAmount",
        SUM(IFNULL(R1."ReconSumFC", 0)) AS "ReconAmountFC"
    FROM RECON_ASOF R1
    WHERE R1."IsCredit" = 'D'
    GROUP BY R1."TransId"
),

"PAY_RECON" AS
(
    SELECT
        AP."DocEntry" AS "APDocEntry",
        OV."TransId" AS "PaymentTransId",
        IFNULL(RC."ReconNum", '') AS "ReconNum",
        CAST(OV."DocDate" AS DATE) AS "Payment date",
        'PS ' || TO_NVARCHAR(OV."DocNum") AS "Payment No.",
        CAST(NULL AS DATE) AS "Downpayment Date",
        CASE
            WHEN IFNULL(OV."TrsfrSum", 0) <> 0 THEN OV."TrsfrAcct"
            WHEN IFNULL(OV."CashSum", 0) <> 0 THEN OV."CashAcct"
            WHEN IFNULL(OV."CheckSum", 0) <> 0 THEN OV."CheckAcct"
            ELSE ''
        END AS "Payment_GL",
        IFNULL(V2."SumApplied", 0) AS "Payment Amount",
        IFNULL(V2."AppliedFC", 0) AS "Payment Amount FC",
        CASE
            WHEN DPR."PaymentTransId" IS NULL THEN 0
            ELSE IFNULL(V2."SumApplied", 0)
        END AS "Applied Amount AsOf",
        CASE
            WHEN DPR."PaymentTransId" IS NULL THEN 0
            ELSE IFNULL(V2."AppliedFC", 0)
        END AS "Applied Amount FC AsOf",
        'Outgoing' AS "Payment type"
    FROM AP
    INNER JOIN VPM2 V2
        ON V2."InvType" = 18
       AND V2."DocEntry" = AP."DocEntry"
    INNER JOIN OVPM OV
        ON OV."DocEntry" = V2."DocNum"
    LEFT JOIN PAYMENT_RECON_NUM RC
        ON RC."PaymentTransId" = OV."TransId"
    LEFT JOIN DOC_PAYMENT_RECON_ASOF DPR
        ON DPR."DocTransId" = AP."TransId"
       AND DPR."PaymentTransId" = OV."TransId"
    WHERE OV."Canceled" = 'N'
      AND OV."DocDate" <= :V_ASOF_DATE
      AND (:P_PAYMENT_DATE_FROM IS NULL OR OV."DocDate" >= :P_PAYMENT_DATE_FROM)

    UNION ALL

    SELECT
        AP."DocEntry" AS "APDocEntry",
        OV."TransId" AS "PaymentTransId",
        IFNULL(RC."ReconNum", '') AS "ReconNum",
        CAST(OV."DocDate" AS DATE) AS "Payment date",
        'PS ' || TO_NVARCHAR(OV."DocNum") AS "Payment No.",
        CAST(DP."DocDate" AS DATE) AS "Downpayment Date",
        CASE
            WHEN IFNULL(OV."TrsfrSum", 0) <> 0 THEN OV."TrsfrAcct"
            WHEN IFNULL(OV."CashSum", 0) <> 0 THEN OV."CashAcct"
            WHEN IFNULL(OV."CheckSum", 0) <> 0 THEN OV."CheckAcct"
            ELSE ''
        END AS "Payment_GL",
        IFNULL(L."DP Applied Amount", 0) AS "Payment Amount",
        IFNULL(L."DP Applied Amount FC", 0) AS "Payment Amount FC",
        CASE
            WHEN DPR."PaymentTransId" IS NULL THEN 0
            ELSE IFNULL(L."DP Applied Amount", 0)
        END AS "Applied Amount AsOf",
        CASE
            WHEN DPR."PaymentTransId" IS NULL THEN 0
            ELSE IFNULL(L."DP Applied Amount FC", 0)
        END AS "Applied Amount FC AsOf",
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
    LEFT JOIN PAYMENT_RECON_NUM RC
        ON RC."PaymentTransId" = OV."TransId"
    LEFT JOIN DOC_PAYMENT_RECON_ASOF DPR
        ON DPR."DocTransId" = DP."TransId"
       AND DPR."PaymentTransId" = OV."TransId"
    WHERE DP."CANCELED" = 'N'
      AND OV."Canceled" = 'N'
      AND OV."DocDate" <= :V_ASOF_DATE
      AND (:P_PAYMENT_DATE_FROM IS NULL OR OV."DocDate" >= :P_PAYMENT_DATE_FROM)
),

"PAY_SORT" AS
(
    SELECT
        X.*,
        ROW_NUMBER() OVER
        (
            PARTITION BY X."APDocEntry"
            ORDER BY
                X."Payment date",
                X."Payment No.",
                CASE WHEN X."Payment type" = 'Downpayment' THEN 1 ELSE 2 END
        ) AS "RN"
    FROM PAY_RECON X
),

"DP_UNMAPPED" AS
(
    SELECT
        DP."DocEntry" AS "DPDocEntry",
        DP."DocNum" AS "DP No.",
        CAST(DP."DocDate" AS DATE) AS "Downpayment Date",
        IFNULL(DP."CardCode", '') AS "CardCode",
        IFNULL(DP."CardName", '') AS "CardName",
        IFNULL(DP."DocCur", '') AS "DocCur",
        DP."GroupNum",
        IFNULL(DPPO."PO No.", '') AS "PO No.",
        IFNULL(DPPO."Contract No.", '') AS "Contract No.",
        OV."TransId" AS "PaymentTransId",
        IFNULL(RC."ReconNum", '') AS "ReconNum",
        CAST(OV."DocDate" AS DATE) AS "Payment date",
        'PS ' || TO_NVARCHAR(OV."DocNum") AS "Payment No.",
        CASE
            WHEN IFNULL(OV."TrsfrSum", 0) <> 0 THEN OV."TrsfrAcct"
            WHEN IFNULL(OV."CashSum", 0) <> 0 THEN OV."CashAcct"
            WHEN IFNULL(OV."CheckSum", 0) <> 0 THEN OV."CheckAcct"
            ELSE ''
        END AS "Payment_GL",

        IFNULL(V2."SumApplied", 0) AS "Unmapped Amount",
        IFNULL(V2."AppliedFC", 0) AS "Unmapped Amount FC",

        IFNULL(DPL."Department", '') AS "Department"
    FROM ODPO DP
    INNER JOIN VPM2 V2
        ON V2."InvType" = 204
       AND V2."DocEntry" = DP."DocEntry"
    INNER JOIN OVPM OV
        ON OV."DocEntry" = V2."DocNum"
    LEFT JOIN DP_LINE DPL
        ON DPL."DocEntry" = DP."DocEntry"
    LEFT JOIN DP_PO_PACK DPPO
        ON DPPO."DPDocEntry" = DP."DocEntry"
    LEFT JOIN PAYMENT_RECON_NUM RC
        ON RC."PaymentTransId" = OV."TransId"
    LEFT JOIN DP_APPLIED_ASOF DPA
        ON DPA."DPDocEntry" = DP."DocEntry"
    WHERE DP."CANCELED" = 'N'
      AND DP."DocDate" <= :V_ASOF_DATE
      AND OV."Canceled" = 'N'
      AND OV."DocDate" <= :V_ASOF_DATE
      AND IFNULL(DPA."Applied Amount AsOf", 0) = 0
      AND (:P_PAYMENT_DATE_FROM IS NULL OR OV."DocDate" >= :P_PAYMENT_DATE_FROM)
),

"DP_OPEN" AS
(
    SELECT
        DP."DocEntry" AS "DPDocEntry",
        DP."DocNum" AS "DP No.",
        CAST(DP."DocDate" AS DATE) AS "Downpayment Date",
        IFNULL(DP."CardCode", '') AS "CardCode",
        IFNULL(DP."CardName", '') AS "CardName",
        IFNULL(DP."DocCur", '') AS "DocCur",
        DP."GroupNum",
        IFNULL(DPPO."PO No.", '') AS "PO No.",
        IFNULL(DPPO."Contract No.", '') AS "Contract No.",
        OV."TransId" AS "PaymentTransId",
        IFNULL(RC."ReconNum", '') AS "ReconNum",
        CAST(OV."DocDate" AS DATE) AS "Payment date",
        'PS ' || TO_NVARCHAR(OV."DocNum") AS "Payment No.",
        CASE
            WHEN IFNULL(OV."TrsfrSum", 0) <> 0 THEN OV."TrsfrAcct"
            WHEN IFNULL(OV."CashSum", 0) <> 0 THEN OV."CashAcct"
            WHEN IFNULL(OV."CheckSum", 0) <> 0 THEN OV."CheckAcct"
            ELSE ''
        END AS "Payment_GL",

        IFNULL(DP."DocTotal", 0)
            - IFNULL(DPA."Applied Amount AsOf", 0) AS "Open Amount",

        IFNULL(DP."DocTotalFC", 0)
            - IFNULL(DPA."Applied Amount FC AsOf", 0) AS "Open Amount FC",

        IFNULL(DPL."Department", '') AS "Department"
    FROM ODPO DP
    INNER JOIN VPM2 V2
        ON V2."InvType" = 204
       AND V2."DocEntry" = DP."DocEntry"
    INNER JOIN OVPM OV
        ON OV."DocEntry" = V2."DocNum"
    LEFT JOIN DP_LINE DPL
        ON DPL."DocEntry" = DP."DocEntry"
    LEFT JOIN DP_PO_PACK DPPO
        ON DPPO."DPDocEntry" = DP."DocEntry"
    LEFT JOIN PAYMENT_RECON_NUM RC
        ON RC."PaymentTransId" = OV."TransId"
    LEFT JOIN DP_APPLIED_ASOF DPA
        ON DPA."DPDocEntry" = DP."DocEntry"
    WHERE DP."CANCELED" = 'N'
      AND DP."DocDate" <= :V_ASOF_DATE
      AND OV."Canceled" = 'N'
      AND OV."DocDate" <= :V_ASOF_DATE
      AND IFNULL(DPA."Applied Amount AsOf", 0) > 0
      AND IFNULL(DP."DocTotal", 0) - IFNULL(DPA."Applied Amount AsOf", 0) > 0
      AND (:P_PAYMENT_DATE_FROM IS NULL OR OV."DocDate" >= :P_PAYMENT_DATE_FROM)
),

"OUTGOING_STANDALONE" AS
(
    SELECT
        OV."DocEntry" AS "PaymentDocEntry",
        OV."TransId" AS "PaymentTransId",
        IFNULL(RC."ReconNum", '') AS "ReconNum",
        CAST(OV."DocDate" AS DATE) AS "Payment date",
        'PS ' || TO_NVARCHAR(OV."DocNum") AS "Payment No.",
        IFNULL(OV."CardCode", '') AS "CardCode",
        IFNULL(OV."CardName", '') AS "CardName",
        IFNULL(OV."DocCurr", '') AS "DocCur",
        CAST(NULL AS INTEGER) AS "GroupNum",

        CASE
            WHEN IFNULL(OV."TrsfrSum", 0) <> 0 THEN IFNULL(OV."TrsfrAcct", '')
            WHEN IFNULL(OV."CashSum", 0) <> 0 THEN IFNULL(OV."CashAcct", '')
            WHEN IFNULL(OV."CheckSum", 0) <> 0 THEN IFNULL(OV."CheckAcct", '')
            ELSE ''
        END AS "Payment_GL",

        IFNULL(OV."DocTotal", 0) AS "Payment Amount",
        IFNULL(OV."DocTotalFC", 0) AS "Payment Amount FC",
        IFNULL(OV."JrnlMemo", '') AS "Description",

        IFNULL(OV."DocTotal", 0)
            - IFNULL(PRA."ReconAmount", 0) AS "OpenBalAmount",

        IFNULL(OV."DocTotalFC", 0)
            - IFNULL(PRA."ReconAmountFC", 0) AS "OpenBalAmountFC",

        CASE
            WHEN IFNULL(PRA."ReconAmount", 0) = 0
                THEN 'Outgoing Independent - Unreconciled'
            WHEN ABS(IFNULL(OV."DocTotal", 0) - IFNULL(PRA."ReconAmount", 0)) > 0.000001
                THEN 'Outgoing Independent - Partially Reconciled'
            ELSE 'Outgoing Independent - Fully Reconciled'
        END AS "Payment type"
    FROM OVPM OV
    LEFT JOIN PAYMENT_RECON_NUM RC
        ON RC."PaymentTransId" = OV."TransId"
    LEFT JOIN PAYMENT_RECON_AMOUNT_ASOF PRA
        ON PRA."PaymentTransId" = OV."TransId"
    WHERE OV."PayNoDoc" = 'Y'
      AND OV."Canceled" = 'N'
      AND OV."DocType" = 'S'
      AND NOT EXISTS
      (
          SELECT 1
          FROM VPM2 V2
          WHERE V2."DocNum" = OV."DocEntry"
      )
      AND OV."DocDate" <= :V_ASOF_DATE
      AND (:P_PAYMENT_DATE_FROM IS NULL OR OV."DocDate" >= :P_PAYMENT_DATE_FROM)
),

"JE_RECON_PACK" AS
(
    SELECT
        X."TransId",
        X."TransRowId",
        CAST(STRING_AGG(IFNULL(TO_NVARCHAR(X."ReconNum"), ''), '; ' ORDER BY X."ReconNum") AS NVARCHAR(5000)) AS "ReconNum"
    FROM
    (
        SELECT DISTINCT
            R1."TransId",
            R1."TransRowId",
            R1."ReconNum"
        FROM RECON_ASOF R1
        WHERE R1."IsCredit" = 'D'
    ) X
    GROUP BY
        X."TransId",
        X."TransRowId"
),

"JE_RECON_AMOUNT_ASOF" AS
(
    SELECT
        R1."TransId",
        R1."TransRowId",
        SUM(IFNULL(R1."ReconSum", 0)) AS "ReconAmount",
        SUM(IFNULL(R1."ReconSumFC", 0)) AS "ReconAmountFC"
    FROM RECON_ASOF R1
    WHERE R1."IsCredit" = 'D'
    GROUP BY
        R1."TransId",
        R1."TransRowId"
),

"JE_OUTGOING_STANDALONE" AS
(
    SELECT
        J0."TransId" AS "PaymentTransId",
        J1."Line_ID" AS "PaymentLine",
        IFNULL(RP."ReconNum", '') AS "ReconNum",

        CAST(J0."RefDate" AS DATE) AS "Payment date",
        'JE ' || TO_NVARCHAR(J0."TransId") AS "Payment No.",

        IFNULL(J1."ShortName", '') AS "CardCode",
        IFNULL(BP."CardName", '') AS "CardName",
        BP."GroupNum" AS "GroupNum",

        IFNULL(J1."Account", '') AS "Control Account",
        IFNULL(J1."ContraAct", '') AS "Payment_GL",

        CASE
            WHEN J1."FCCurrency" IS NULL
              OR TRIM(J1."FCCurrency") = ''
                THEN IFNULL(ADM."MainCurncy", '')
            ELSE J1."FCCurrency"
        END AS "DocCur",

        IFNULL(J0."Memo", '') AS "Description",
        IFNULL(J1."ProfitCode", '') AS "Department",

        IFNULL(J1."Debit", 0) AS "Payment Amount",
        IFNULL(J1."FCDebit", 0) AS "Payment Amount FC",

        IFNULL(J1."Debit", 0)
            - IFNULL(JRA."ReconAmount", 0) AS "OpenBalAmount",

        IFNULL(J1."FCDebit", 0)
            - IFNULL(JRA."ReconAmountFC", 0) AS "OpenBalAmountFC",

        CASE
            WHEN IFNULL(JRA."ReconAmount", 0) = 0
                THEN 'JE Outgoing Independent - Unreconciled'

            WHEN ABS(IFNULL(J1."Debit", 0) - IFNULL(JRA."ReconAmount", 0)) > 0.000001
                THEN 'JE Outgoing Independent - Partially Reconciled'

            ELSE 'JE Outgoing Independent - Fully Reconciled'
        END AS "Payment type"

    FROM OJDT J0
    INNER JOIN JDT1 J1
        ON J1."TransId" = J0."TransId"
    INNER JOIN OCRD BP
        ON BP."CardCode" = J1."ShortName"
       AND BP."CardType" = 'S'
    CROSS JOIN OADM ADM
    LEFT JOIN JE_RECON_PACK RP
        ON RP."TransId" = J1."TransId"
       AND RP."TransRowId" = J1."Line_ID"
    LEFT JOIN JE_RECON_AMOUNT_ASOF JRA
        ON JRA."TransId" = J1."TransId"
       AND JRA."TransRowId" = J1."Line_ID"

    WHERE J0."TransType" = 30
      AND IFNULL(J1."Debit", 0) > 0
      AND IFNULL(J0."StornoToTr", 0) = 0
      AND NOT EXISTS
      (
          SELECT 1
          FROM OJDT REV
          WHERE IFNULL(REV."StornoToTr", 0) = J0."TransId"
            AND REV."RefDate" <= :V_ASOF_DATE
      )
      AND J0."RefDate" <= :V_ASOF_DATE
      AND (:P_PAYMENT_DATE_FROM IS NULL OR J0."RefDate" >= :P_PAYMENT_DATE_FROM)
),

"FINAL_AP" AS
(
    SELECT
        AP."DocNum" AS "SortDocNum",
        PR."Payment date" AS "SortPaymentDate",
        PR."Payment No." AS "SortPaymentNo",
        PR."RN" AS "SortRN",

        TO_NVARCHAR(AP."TransId") AS "Transaction No.",
        AP."CtlAccount" AS "Control Account",
        'PU ' || TO_NVARCHAR(AP."DocNum") AS "Document No.",
        AP."DocDate" AS "Posting Date",
        AP."DocDueDate" AS "Due Date",
        AP."TaxDate" AS "Document Date",
        AP."CardCode" AS "BP Code",
        AP."CardName" AS "BP Name",
        AP."JrnlMemo" AS "Description",
        AP."U_APInv" AS "Số hóa đơn",
        AP."U_InvDate" AS "Ngày hóa đơn",
        AP."U_SoToKhai" AS "Số tờ khai",
        IFNULL(PO."PO No.", '') AS "PO No.",
        IFNULL(AP."U_ContractNo", '') AS "Contract No.",
        IFNULL(PT."PymntGroup", '') AS "Payment Term",
        IFNULL(PRP."Department_PR", '') AS "Department_PR",
        IFNULL(AP_LINE."Department", '') AS "Department",

        AP."DocCur" AS "Currency",
        CASE WHEN PR."RN" = 1 THEN IFNULL(AP."DocTotal", 0) ELSE 0 END AS "Total Payment Due",
        CASE WHEN PR."RN" = 1 THEN IFNULL(AP."DocTotalFC", 0) ELSE 0 END AS "Total Payment Due FC",
        CASE WHEN PR."RN" = 1 THEN IFNULL(CM."A/P Credit Memo No.", '') ELSE '' END AS "A/P Credit Memo No.",
        CASE WHEN PR."RN" = 1 THEN IFNULL(CM."Total A/P Credit Memo", 0) ELSE 0 END AS "Total A/P Credit Memo",
        CASE WHEN PR."RN" = 1 THEN IFNULL(CM."Total A/P Credit Memo FC", 0) ELSE 0 END AS "Total A/P Credit Memo FC",

        CASE
            WHEN PR."RN" = 1
                THEN IFNULL(AP."DocTotal", 0) - IFNULL(CM."Total A/P Credit Memo", 0)
            ELSE 0
        END AS "Net Payment Due",

        CASE
            WHEN PR."RN" = 1
                THEN IFNULL(AP."DocTotalFC", 0) - IFNULL(CM."Total A/P Credit Memo FC", 0)
            ELSE 0
        END AS "Net Payment Due FC",

        PR."PaymentTransId" AS "Payment TransId",
        PR."ReconNum" AS "ReconNum",
        PR."Payment date",
        PR."Payment No.",
        PR."Downpayment Date",
        PR."Payment_GL",
        IFNULL(PR."Payment Amount", 0) AS "Payment Amount",
        IFNULL(PR."Payment Amount FC", 0) AS "Payment Amount FC",
        PR."Payment type",

        CASE
            WHEN PR."RN" = 1
                THEN IFNULL(AP."DocTotal", 0)
                     - IFNULL(CM."Total A/P Credit Memo", 0)
                     - IFNULL(PR."Applied Amount AsOf", 0)
            ELSE 0 - IFNULL(PR."Applied Amount AsOf", 0)
        END AS "Balance due",

        CASE
            WHEN PR."RN" = 1
                THEN IFNULL(AP."DocTotalFC", 0)
                     - IFNULL(CM."Total A/P Credit Memo FC", 0)
                     - IFNULL(PR."Applied Amount FC AsOf", 0)
            ELSE 0 - IFNULL(PR."Applied Amount FC AsOf", 0)
        END AS "Balance due FC"

    FROM PAY_SORT PR
    INNER JOIN AP
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
),

"FINAL_DP_UNMAPPED" AS
(
    SELECT
        0 AS "SortDocNum",
        DPU."Payment date" AS "SortPaymentDate",
        DPU."Payment No." AS "SortPaymentNo",
        1 AS "SortRN",

        '' AS "Transaction No.",
        '' AS "Control Account",
        '' AS "Document No.",
        CAST(NULL AS DATE) AS "Posting Date",
        CAST(NULL AS DATE) AS "Due Date",
        CAST(NULL AS DATE) AS "Document Date",
        DPU."CardCode" AS "BP Code",
        DPU."CardName" AS "BP Name",
        '' AS "Description",
        '' AS "Số hóa đơn",
        CAST(NULL AS DATE) AS "Ngày hóa đơn",
        '' AS "Số tờ khai",
        DPU."PO No." AS "PO No.",
        '' AS "Contract No.",
        IFNULL(PT."PymntGroup", '') AS "Payment Term",
        '' AS "Department_PR",
        DPU."Department" AS "Department",

        DPU."DocCur" AS "Currency",
        0 AS "Total Payment Due",
        0 AS "Total Payment Due FC",
        '' AS "A/P Credit Memo No.",
        0 AS "Total A/P Credit Memo",
        0 AS "Total A/P Credit Memo FC",
        0 AS "Net Payment Due",
        0 AS "Net Payment Due FC",

        DPU."PaymentTransId" AS "Payment TransId",
        DPU."ReconNum" AS "ReconNum",
        DPU."Payment date",
        DPU."Payment No.",
        DPU."Downpayment Date",
        DPU."Payment_GL",
        DPU."Unmapped Amount" AS "Payment Amount",
        DPU."Unmapped Amount FC" AS "Payment Amount FC",
        'Downpayment' AS "Payment type",

        0 - DPU."Unmapped Amount" AS "Balance due",
        0 - DPU."Unmapped Amount FC" AS "Balance due FC"

    FROM DP_UNMAPPED DPU
    LEFT JOIN OCTG PT
        ON PT."GroupNum" = DPU."GroupNum"
),

"FINAL_DP_OPEN" AS
(
    SELECT
        0 AS "SortDocNum",
        DPO."Payment date" AS "SortPaymentDate",
        DPO."Payment No." AS "SortPaymentNo",
        1 AS "SortRN",

        '' AS "Transaction No.",
        '' AS "Control Account",
        '' AS "Document No.",
        CAST(NULL AS DATE) AS "Posting Date",
        CAST(NULL AS DATE) AS "Due Date",
        CAST(NULL AS DATE) AS "Document Date",
        DPO."CardCode" AS "BP Code",
        DPO."CardName" AS "BP Name",
        '' AS "Description",
        '' AS "Số hóa đơn",
        CAST(NULL AS DATE) AS "Ngày hóa đơn",
        '' AS "Số tờ khai",
        DPO."PO No." AS "PO No.",
        '' AS "Contract No.",
        IFNULL(PT."PymntGroup", '') AS "Payment Term",
        '' AS "Department_PR",
        DPO."Department" AS "Department",

        DPO."DocCur" AS "Currency",
        0 AS "Total Payment Due",
        0 AS "Total Payment Due FC",
        '' AS "A/P Credit Memo No.",
        0 AS "Total A/P Credit Memo",
        0 AS "Total A/P Credit Memo FC",
        0 AS "Net Payment Due",
        0 AS "Net Payment Due FC",

        DPO."PaymentTransId" AS "Payment TransId",
        DPO."ReconNum" AS "ReconNum",
        DPO."Payment date",
        DPO."Payment No.",
        DPO."Downpayment Date",
        DPO."Payment_GL",
        DPO."Open Amount" AS "Payment Amount",
        DPO."Open Amount FC" AS "Payment Amount FC",
        'Downpayment' AS "Payment type",

        0 - DPO."Open Amount" AS "Balance due",
        0 - DPO."Open Amount FC" AS "Balance due FC"

    FROM DP_OPEN DPO
    LEFT JOIN OCTG PT
        ON PT."GroupNum" = DPO."GroupNum"
),

"FINAL_OUTGOING_STANDALONE" AS
(
    SELECT
        0 AS "SortDocNum",
        OPS."Payment date" AS "SortPaymentDate",
        OPS."Payment No." AS "SortPaymentNo",
        1 AS "SortRN",

        '' AS "Transaction No.",
        '' AS "Control Account",
        '' AS "Document No.",
        CAST(NULL AS DATE) AS "Posting Date",
        CAST(NULL AS DATE) AS "Due Date",
        CAST(NULL AS DATE) AS "Document Date",
        OPS."CardCode" AS "BP Code",
        OPS."CardName" AS "BP Name",
        OPS."Description" AS "Description",
        '' AS "Số hóa đơn",
        CAST(NULL AS DATE) AS "Ngày hóa đơn",
        '' AS "Số tờ khai",
        '' AS "PO No.",
        '' AS "Contract No.",
        IFNULL(PT."PymntGroup", '') AS "Payment Term",
        '' AS "Department_PR",
        '' AS "Department",

        OPS."DocCur" AS "Currency",
        0 AS "Total Payment Due",
        0 AS "Total Payment Due FC",
        '' AS "A/P Credit Memo No.",
        0 AS "Total A/P Credit Memo",
        0 AS "Total A/P Credit Memo FC",
        0 AS "Net Payment Due",
        0 AS "Net Payment Due FC",

        OPS."PaymentTransId" AS "Payment TransId",
        OPS."ReconNum" AS "ReconNum",
        OPS."Payment date",
        OPS."Payment No.",
        CAST(NULL AS DATE) AS "Downpayment Date",
        OPS."Payment_GL",
        OPS."Payment Amount",
        OPS."Payment Amount FC",
        OPS."Payment type",
        0 - OPS."OpenBalAmount" AS "Balance due",
        0 - OPS."OpenBalAmountFC" AS "Balance due FC"

    FROM OUTGOING_STANDALONE OPS
    LEFT JOIN OCTG PT
        ON PT."GroupNum" = OPS."GroupNum"
),

"FINAL_JE_OUTGOING_STANDALONE" AS
(
    SELECT
        0 AS "SortDocNum",
        JE."Payment date" AS "SortPaymentDate",
        JE."Payment No." AS "SortPaymentNo",
        JE."PaymentLine" + 1 AS "SortRN",

        TO_NVARCHAR(JE."PaymentTransId") AS "Transaction No.",
        JE."Control Account" AS "Control Account",
        '' AS "Document No.",
        CAST(NULL AS DATE) AS "Posting Date",
        CAST(NULL AS DATE) AS "Due Date",
        CAST(NULL AS DATE) AS "Document Date",
        JE."CardCode" AS "BP Code",
        JE."CardName" AS "BP Name",
        JE."Description" AS "Description",
        '' AS "Số hóa đơn",
        CAST(NULL AS DATE) AS "Ngày hóa đơn",
        '' AS "Số tờ khai",
        '' AS "PO No.",
        '' AS "Contract No.",
        IFNULL(PT."PymntGroup", '') AS "Payment Term",
        '' AS "Department_PR",
        JE."Department" AS "Department",

        JE."DocCur" AS "Currency",
        0 AS "Total Payment Due",
        0 AS "Total Payment Due FC",
        '' AS "A/P Credit Memo No.",
        0 AS "Total A/P Credit Memo",
        0 AS "Total A/P Credit Memo FC",
        0 AS "Net Payment Due",
        0 AS "Net Payment Due FC",

        JE."PaymentTransId" AS "Payment TransId",
        JE."ReconNum" AS "ReconNum",
        JE."Payment date",
        JE."Payment No.",
        CAST(NULL AS DATE) AS "Downpayment Date",
        JE."Payment_GL",
        JE."Payment Amount",
        JE."Payment Amount FC",
        JE."Payment type",

        0 - JE."OpenBalAmount" AS "Balance due",
        0 - JE."OpenBalAmountFC" AS "Balance due FC"

    FROM JE_OUTGOING_STANDALONE JE
    LEFT JOIN OCTG PT
        ON PT."GroupNum" = JE."GroupNum"
),

"FINAL_ALL" AS
(
    SELECT * FROM FINAL_DP_UNMAPPED
    UNION ALL
    SELECT * FROM FINAL_DP_OPEN
    UNION ALL
    SELECT * FROM FINAL_OUTGOING_STANDALONE
    UNION ALL
    SELECT * FROM FINAL_JE_OUTGOING_STANDALONE
    UNION ALL
    SELECT * FROM FINAL_AP
)

SELECT
    ROW_NUMBER() OVER
    (
        ORDER BY
            "SortPaymentDate",
            "SortDocNum",
            "SortPaymentNo",
            "SortRN"
    ) AS "#",

    "Transaction No.",
    "Control Account",
    "Document No.",
    "Posting Date",
    "Due Date",
    "Document Date",
    "BP Code",
    "BP Name",
    "Description",
    "Số hóa đơn",
    "Ngày hóa đơn",
    "Số tờ khai",
    CAST(IFNULL("PO No.", '') AS NVARCHAR(5000)) AS "PO No.",
    "Contract No.",
    "Payment Term",
    "Department_PR",
    "Department",
    "Currency",
    "Total Payment Due",
    "Total Payment Due FC",
    "A/P Credit Memo No.",
    "Total A/P Credit Memo",
    "Total A/P Credit Memo FC",
    "Net Payment Due",
    "Net Payment Due FC",
    "Payment TransId",
    "ReconNum",
    "Payment date",
    "Payment No.",
    "Downpayment Date",
    "Payment_GL",
    "Payment Amount",
    "Payment Amount FC",
    "Payment type",
    "Balance due",
    "Balance due FC"

FROM FINAL_ALL

ORDER BY
    "SortPaymentDate",
    "SortDocNum",
    "SortPaymentNo",
    "SortRN";

END;