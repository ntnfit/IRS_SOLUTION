-- B1 DEPENDS: AFTER:PT:CREATE_VIEWS

CREATE PROCEDURE TmSp_HU_PrepareTaxReportData (IN PeriodStartStr NVARCHAR(50), IN PeriodEndStr NVARCHAR(50))
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER
AS
  PeriodStart      TIMESTAMP;
  PeriodEnd        TIMESTAMP;
  temp_var_0       INTEGER;
  VatLimit         INTEGER;
BEGIN
  -- 1. setup all parameters
  PeriodStart := CAST(:PeriodStartStr AS TIMESTAMP);
  PeriodEnd   := CAST(:PeriodEndStr AS TIMESTAMP);

  IF (:PeriodStart < '2018-07-01') THEN
      VatLimit := 1000000;
  ELSE
      VatLimit :=  100000;
  END IF;

  --------------------------------------------------------------------------------
  SELECT COUNT(*) INTO temp_var_0 FROM objects
  WHERE OBJECT_TYPE = 'TABLE' AND OBJECT_NAME LIKE 'TMP_HU_DOC_TYPE_NAME' and schema_name = CURRENT_SCHEMA;
  IF :temp_var_0 > 0 THEN
      DROP TABLE "TMP_HU_DOC_TYPE_NAME";
  END IF;

  CREATE COLUMN TABLE "TMP_HU_DOC_TYPE_NAME" ("ObjType" INTEGER NOT NULL, "ObjTypeName" NVARCHAR(50) NULL);

  INSERT INTO "TMP_HU_DOC_TYPE_NAME" VALUES ( 13, N'Kimen' || NCHAR(0x0151) || N' sz' || NCHAR(0x00E1) || N'mla');                        -- 'AR Invoice'
  INSERT INTO "TMP_HU_DOC_TYPE_NAME" VALUES ( 14, N'Kimen' || NCHAR(0x0151) || N' j' || NCHAR(0x00F3) || N'v' || NCHAR(0x00E1) || NCHAR(0x00ED) || N'r' || NCHAR(0x00E1) || N's');                      -- 'AR Credit Memo'
  INSERT INTO "TMP_HU_DOC_TYPE_NAME" VALUES ( 15, N'Sz' || NCHAR(0x00E1) || N'll' || NCHAR(0x00ED) || N't' || NCHAR(0x00E1) || N's');                            -- 'AR Delivery'
  INSERT INTO "TMP_HU_DOC_TYPE_NAME" VALUES ( 16, N'Vev' || NCHAR(0x0151) || N'i vissz' || NCHAR(0x00E1) || N'ru');                       -- 'AR Return'
  INSERT INTO "TMP_HU_DOC_TYPE_NAME" VALUES (165, N'Kimen' || NCHAR(0x0151) || N' helyesb' || NCHAR(0x00ED) || N't' || NCHAR(0x0151) || N' sz' || NCHAR(0x00E1) || N'mla');             -- 'AR Corr. Invoice'
  INSERT INTO "TMP_HU_DOC_TYPE_NAME" VALUES (166, N'Kimen' || NCHAR(0x0151) || N' helyesb' || NCHAR(0x00ED) || N't' || NCHAR(0x0151) || N' sz' || NCHAR(0x00E1) || N'mla storn' || NCHAR(0x00F3) || N'ja');    -- 'AR Corr. Invoice Rev.'
  INSERT INTO "TMP_HU_DOC_TYPE_NAME" VALUES (203, N'Kimen' || NCHAR(0x0151) || N' el' || NCHAR(0x0151) || N'legsz' || NCHAR(0x00E1) || N'mla');                  -- 'Downpayment (incoming)'

  INSERT INTO "TMP_HU_DOC_TYPE_NAME" VALUES ( 18, N'Bej' || NCHAR(0x00F6) || N'v' || NCHAR(0x0151) || N' sz' || NCHAR(0x00E1) || N'mla');                        -- 'AP Invoice'
  INSERT INTO "TMP_HU_DOC_TYPE_NAME" VALUES ( 19, N'Bej' || NCHAR(0x00F6) || N'v' || NCHAR(0x0151) || N' j' || NCHAR(0x00F3) || N'v' || NCHAR(0x00E1) || NCHAR(0x00ED) || N'r' || NCHAR(0x00E1) || N's');                      -- 'AP Credit Memo'
  INSERT INTO "TMP_HU_DOC_TYPE_NAME" VALUES ( 20, N'' || NCHAR(0x00C1) || N'rube' || NCHAR(0x00E9) || N'rkez' || NCHAR(0x00E9) || N's');                         -- 'AP Goods Receipt'
  INSERT INTO "TMP_HU_DOC_TYPE_NAME" VALUES ( 21, N'Sz' || NCHAR(0x00E1) || N'll' || NCHAR(0x00ED) || N't' || NCHAR(0x00F3) || N'i vissz' || NCHAR(0x00E1) || N'ru');                   -- 'AP Return'
  INSERT INTO "TMP_HU_DOC_TYPE_NAME" VALUES (163, N'Bej' || NCHAR(0x00F6) || N'v' || NCHAR(0x0151) || N' helyesb' || NCHAR(0x00ED) || N't' || NCHAR(0x0151) || N' sz' || NCHAR(0x00E1) || N'mla');             -- 'AP Corr. Invoice'
  INSERT INTO "TMP_HU_DOC_TYPE_NAME" VALUES (164, N'Bej' || NCHAR(0x00F6) || N'v' || NCHAR(0x0151) || N' helyesb' || NCHAR(0x00ED) || N't' || NCHAR(0x0151) || N' sz' || NCHAR(0x00E1) || N'mla storn' || NCHAR(0x00F3) || N'ja');    -- 'AP Corr. Invoice Rev.'
  INSERT INTO "TMP_HU_DOC_TYPE_NAME" VALUES (204, N'Bej' || NCHAR(0x00F6) || N'v' || NCHAR(0x0151) || N' el' || NCHAR(0x0151) || N'legsz' || NCHAR(0x00E1) || N'mla');                  -- 'Downpayment (outgoing)'

  INSERT INTO "TMP_HU_DOC_TYPE_NAME" VALUES ( 30, N'Napl' || NCHAR(0x00F3) || N'k' || NCHAR(0x00F6) || N'nyvel' || NCHAR(0x00E9) || N's');                       -- 'Journal Entry'
  INSERT INTO "TMP_HU_DOC_TYPE_NAME" VALUES ( 24, N'Bej' || NCHAR(0x00F6) || N'v' || NCHAR(0x0151) || N' fizet' || NCHAR(0x00E9) || N'sek');                     -- 'Payment (incoming)'
  INSERT INTO "TMP_HU_DOC_TYPE_NAME" VALUES ( 46, N'Kimen' || NCHAR(0x0151) || N' fizet' || NCHAR(0x00E9) || N'sek');                     -- 'Payment (outgoing)'


  --------------------------------------------------------------------------------
  -- 2. create temporary table where all calculated data are stored
  SELECT COUNT(*) INTO temp_var_0 FROM objects
  WHERE OBJECT_TYPE = 'TABLE' AND OBJECT_NAME LIKE 'ReportedDocLines' and schema_name = CURRENT_SCHEMA;
  IF :temp_var_0 > 0 THEN
      DROP TABLE "ReportedDocLines";
  END IF;

  CREATE COLUMN TABLE "ReportedDocLines" (
      "DocType"                  NVARCHAR(20) NOT NULL,
      "CommonTaxEntry"           INTEGER NULL,
      "TaxEntry"                 INTEGER NOT NULL,
      "LineSeq"                  INTEGER NOT NULL,
      "FlagOriginal"             NCHAR(2) NOT NULL,
      "BP"                       NVARCHAR(20) NOT NULL,
      "BPName"                   NVARCHAR(100) NULL,
      "BPLicTradNum"             NVARCHAR(32) NULL,
      "BPAdditionalID"           NVARCHAR(64) NULL,
      "BPUnifiedTaxID"           NVARCHAR(32) NULL,
      "ConnectedBP"              NVARCHAR(20) NULL,
      "ConnectedBPName"          NVARCHAR(100) NULL,
      "ConnectedBPLicTradNum"    NVARCHAR(32) NULL,
      "ConnectedBPAdditionalID"  NVARCHAR(64) NULL,
      "ConnectedBPUnifiedTaxID"  NVARCHAR(32) NULL,
      "NumAtCard"                NVARCHAR(100) NULL,
      "ObjType"                  NVARCHAR(20) NOT NULL,
      "AbsEntry"                 INTEGER NOT NULL,
      "DocNum"                   INTEGER NOT NULL,
      "OrdinalNum"               INTEGER NOT NULL,
      "LineNum"                  INTEGER NOT NULL,
      "ArrType"                  INTEGER NOT NULL,
      "GrpNum"                   INTEGER NOT NULL,
      "BaseObjType"              NVARCHAR(20) NOT NULL,
      "BaseAbsEntry"             INTEGER NOT NULL,
      "BaseDocNum"               NVARCHAR(100) NULL,
      "BaseOrdinalNum"           INTEGER NOT NULL,
      "BaseLineNum"              INTEGER NOT NULL,
      "BaseArrType"              INTEGER NOT NULL,
      "BaseGrpNum"               INTEGER NOT NULL,
      "DocsInChain"              INTEGER NOT NULL,  -- DEFAULT (1),
      "TaxCode"                  NVARCHAR(8) NOT NULL,
      "TaxType"                  NCHAR(1) NOT NULL, -- Y = regular, A = acquisition, R = reverse
      "BaseSum"                  DECIMAL(21,6) NOT NULL,
      "VatSum"                   DECIMAL(21,6) NOT NULL,
      "DocBaseSum"               DECIMAL(21,6) NOT NULL,
      "DocVatSum"                DECIMAL(21,6) NOT NULL,
      "DocDate"                  TIMESTAMP NOT NULL,
      "VatDate"                  TIMESTAMP NOT NULL,
      "TaxDate"                  TIMESTAMP NOT NULL,
      "Cancelled"                NCHAR(1) NOT NULL  -- Y = Yes, N = No, C = Cancellation
     );
  --------------------------------------------------------------------------------
  -- 3. complete doc chains (same CommonTaxEntry) with all needed data
  SELECT COUNT(*) INTO temp_var_0 FROM objects
  WHERE OBJECT_TYPE = 'TABLE' AND OBJECT_NAME LIKE 'DocLinesInChain' and schema_name = CURRENT_SCHEMA;
  IF :temp_var_0 > 0 THEN
      DROP TABLE "DocLinesInChain";
  END IF;

  CREATE COLUMN TABLE "DocLinesInChain" (
      "CommonTaxEntry"   INTEGER NULL,
      "TaxEntry"         INTEGER NOT NULL,
      "LineSeq"          INTEGER NOT NULL,
      "ObjType"          INTEGER NOT NULL,
      "AbsEntry"         INTEGER NOT NULL,
      "OrdinalNum"       INTEGER NOT NULL,
      "LineNum"          INTEGER NOT NULL,
      "ArrType"          INTEGER NOT NULL,
      "GrpNum"           INTEGER NOT NULL,
      "BaseObjType"      INTEGER NOT NULL,
      "BaseAbsEntry"     INTEGER NOT NULL,
      "BaseOrdinalNum"   INTEGER NOT NULL,
      "BaseLineNum"      INTEGER NOT NULL,
      "BaseArrType"      INTEGER NOT NULL,
      "BaseGrpNum"       INTEGER NOT NULL,
      "IsBased"          INTEGER NULL,
      "IsInPeriod"       INTEGER NULL, -- DEFAULT (-1),
      "IsAboveVatLimit"  INTEGER NULL, -- DEFAULT (-1)
      "Status"           INTEGER NULL, -- DEFAULT (0)
      "Cancelled"        NCHAR(1) NULL -- Y = Yes, N = No, C = Cancellation
  );

  --------------------------------------------------------------------------------
  -- fix TAX1 issue with cancelling invoices including linked downpayments (SMS incident 47230)
  SELECT COUNT(*) INTO temp_var_0 FROM objects
  WHERE OBJECT_TYPE = 'TABLE' AND OBJECT_NAME LIKE 'copyTAX1' and schema_name = CURRENT_SCHEMA;
  IF :temp_var_0 > 0 THEN
      DROP TABLE "copyTAX1";
  END IF;

  CREATE TABLE "copyTAX1" LIKE TAX1 WITH data;

  UPDATE "copyTAX1" tblLines
  SET "BaseObjTyp" = tblBaseDocLine."BaseObjTyp",
      "BaseAbs"    = tblBaseDocLine."BaseAbs",
      "BaseArrTyp" = 22,
      "BaseLinNum" = tblLines."SrcLineNum"
  FROM
    "copyTAX1" tblLines
    INNER JOIN
    OTAX tblMain
      ON tblMain."AbsEntry" = tblLines."AbsEntry"
    INNER JOIN  -- select only cancelling docs
    "B1_MarketingDocumentsView" tblDocs
      ON tblDocs."ObjType"  = tblMain."SrcObjType" AND
         tblDocs."DocEntry" = tblMain."SrcObjAbs" AND
         tblDocs."CANCELED" = 'C'
    INNER JOIN  -- cancelling docs are based only on 1 base doc, therefore following aggregation is OK
    (SELECT "AbsEntry", MAX("BaseObjTyp") "BaseObjTyp", MAX("BaseAbs") "BaseAbs"
     FROM "copyTAX1"
     WHERE "SrcArrType" = 12
     GROUP BY "AbsEntry"
    ) tblBaseDocLine
      ON tblBaseDocLine."AbsEntry" = tblMain."AbsEntry"
  WHERE tblLines."SrcArrType" = 22;

   --------------------------------------------------------------------------------
  -- 4. prepare standalone docs, which are not part of any doc chain
  INSERT INTO "ReportedDocLines"
  (SELECT
      (CASE WHEN tblMain."SrcObjType" IN (13,15,203,18,20,204) THEN 'standalone'
            WHEN tblMain."SrcObjType" IN (14,16,165,166,19,21,163,164) THEN 'chain'
            WHEN tblMain."SrcObjType" IN (30,24,46) THEN 'tax booking'
            ELSE '' END) AS "DocType",
      NULL, tblMain."AbsEntry", tblLine."LineSeq",
      (CASE WHEN tblMain."SrcObjType" IN (13,15,203,18,20,204) THEN 'E'
            WHEN tblMain."SrcObjType" IN (14,16,165,166,19,21,163,164) THEN 'KT'
            ELSE '' END) AS "FlagOriginal",
      tblDocs."CardCode", tblDocs."CardName", tblDocs."LicTradNum", tblSameBP."CardCode", tblTheBP."AddID", tblTheBP."VatIdUnCmp",
      tblSameBP."CardName", tblSameBP."LicTradNum", tblSameBP."AddID", tblSameBP."VatIdUnCmp", tblDocs."NumAtCard",
      tblMain."SrcObjType", tblMain."SrcObjAbs", tblDocs."DocNum", tblMain."OrdinLNum", tblLine."SrcLineNum", tblLine."SrcArrType", tblLine."SrcGrpNum", -1,
      -1, NULL, -1, -1, -1, -1,
      1,
      tblLine."TaxCode", (CASE WHEN tblLine."IsAcq" = 'Y' THEN 'A' ELSE tblLine."TaxType" END) AS "TaxType",
      (CASE WHEN tblLine."CrditDebit" = 'C' THEN tblLine."BaseSum" ELSE -tblLine."BaseSum" END) AS "BaseSum",
      (CASE WHEN tblLine."CrditDebit" = 'C' THEN tblLine."VatSum" ELSE -tblLine."VatSum" END) AS "VatSum",
      tblBigVat."DocBaseSum", tblBigVat."DocVatSum",
      tblDocs."DocDate", tblDocs."VatDate", tblDocs."TaxDate",
      tblDocs."CANCELED"
   FROM
     OTAX tblMain

     INNER JOIN -- document lines, freights, ...
     "copyTAX1" tblLine
       ON tblLine."AbsEntry" = tblMain."AbsEntry" AND
          tblLine."IsAcq"   <> 'Y' AND                              -- exclude lines with acquisition tax
          tblLine."TaxType" <> 'R' AND                              -- exclude lines with reverse tax
          tblLine."TaxType" <> 'D'                                  -- exclude lines with deferred tax (temporary fix, util 2017-07-01 legal change)

     LEFT JOIN
     OTAX tblCheckBase
       ON tblCheckBase."SrcObjType" = tblLine."BaseObjTyp" AND
          tblCheckBase."SrcObjAbs"  = tblLine."BaseAbs"

     INNER JOIN -- get doc. dates and only docs within reported period
     "B1_MarketingDocumentsView" tblDocs
       ON tblDocs."ObjType"  = tblMain."SrcObjType" AND
          tblDocs."DocEntry" = tblMain."SrcObjAbs" AND
          tblDocs."VatDate" >= :PeriodStart AND
          tblDocs."VatDate" <= :PeriodEnd

     INNER JOIN -- only docs where doc total VAT >= 1000000 (@VatLimit)
     (SELECT "AbsEntry",
        SUM(CASE WHEN "CrditDebit" = 'C' THEN IFNULL("BaseSum", 0) ELSE -IFNULL("BaseSum", 0) END) AS "DocBaseSum",
        SUM(CASE WHEN "CrditDebit" = 'C' THEN IFNULL("VatSum",  0) ELSE -IFNULL("VatSum", 0)  END) AS "DocVatSum"
      FROM
        "copyTAX1"
      WHERE "IsAcq" <> 'Y'       -- exclude lines with acquisition tax
        AND "TaxType" <> 'R'     -- exclude lines with reverse tax
        AND "TaxType" <> 'D'     -- exclude lines with deferred tax (temporary fix, util 2017-07-01 legal change)
      GROUP BY "AbsEntry"
      HAVING ABS(SUM(CASE WHEN "CrditDebit" = 'C' THEN "VatSum" ELSE -"VatSum" END)) >= :VatLimit
     ) AS tblBigVat
       ON tblBigVat."AbsEntry" = tblMain."AbsEntry"

     LEFT JOIN -- only doc lines which are not a base for another doc line (see WHERE condition)
     "copyTAX1" tblChildLine
       ON tblChildLine."BaseObjTyp" = tblMain."SrcObjType" AND
          tblChildLine."BaseAbs"    = tblMain."SrcObjAbs" AND
          tblChildLine."BaseLinNum" = tblLine."SrcLineNum" AND
          tblChildLine."BaseArrTyp" = tblLine."SrcArrType" AND
          tblChildLine."BaseGrpNum" = tblLine."SrcGrpNum"

     LEFT OUTER JOIN -- main BP, to access additional data
     OCRD tblTheBP
       ON tblTheBP."CardCode" = tblDocs."CardCode"

     LEFT OUTER JOIN -- connected BP, so it is possible to group data per the same partner (who is customer and vendor at the same time)
     OCRD tblSameBP
       ON tblSameBP."ConnBP" = tblDocs."CardCode"

  WHERE tblChildLine."AbsEntry" IS NULL
    AND (tblLine."BaseObjTyp" <= 0 OR tblLine."BaseAbs" <= 0 OR tblLine."BaseLinNum" <  0 OR   -- there is no base doc or
         tblCheckBase."AbsEntry" IS NULL)                                                      -- it is not VAT relevant (not in OTAX, TAX1 tables)
  );

  --PRINT '  4. Standalone docs prepared';


  --------------------------------------------------------------------------------
  -- 4.5 prepare standalone docs, which are part a doc chain
  INSERT INTO "ReportedDocLines"
  (SELECT
     'standalone' AS "DocType", NULL, tblMain."AbsEntry", tblLine."LineSeq", '',
     tblDocs."CardCode", tblDocs."CardName", tblDocs."LicTradNum", tblSameBP."CardCode", tblSameBP."CardName", tblTheBP."AddID", tblTheBP."VatIdUnCmp",
     tblSameBP."LicTradNum", tblSameBP."AddID", tblSameBP."VatIdUnCmp", tblDocs."NumAtCard",
     tblMain."SrcObjType", tblMain."SrcObjAbs", tblDocs."DocNum", tblMain."OrdinLNum", tblLine."SrcLineNum", tblLine."SrcArrType", tblLine."SrcGrpNum",
     -1, -1, NULL, -1, -1, -1, -1,
     1,
     tblLine."TaxCode", (CASE WHEN tblLine."IsAcq" = 'Y' THEN 'A' ELSE tblLine."TaxType" END) AS "TaxType",
     (CASE WHEN tblLine."CrditDebit" = 'C' THEN tblLine."BaseSum" ELSE -tblLine."BaseSum" END) AS "BaseSum",
     (CASE WHEN tblLine."CrditDebit" = 'C' THEN tblLine."VatSum" ELSE -tblLine."VatSum" END) AS "VatSum",
     tblBigVat."DocBaseSum", tblBigVat."DocVatSum",
     tblDocs."DocDate", tblDocs."VatDate", tblDocs."TaxDate",
     tblDocs."CANCELED"
   FROM
     OTAX tblMain

     INNER JOIN -- document lines, freights, ...
     "copyTAX1" tblLine
       ON tblLine."AbsEntry" = tblMain."AbsEntry" AND
          tblLine."IsAcq"   <> 'Y' AND    -- exclude lines with acquisition tax
          tblLine."TaxType" <> 'R' AND    -- exclude lines with reverse tax
          tblLine."TaxType" <> 'D'        -- exclude lines with deferred tax (temporary fix, util 2017-07-01 legal change)

     LEFT JOIN
     "ReportedDocLines" tblStandaloneDocs
       ON tblStandaloneDocs."TaxEntry" = tblLine."AbsEntry" AND
          tblStandaloneDocs."LineSeq"  = tblLine."LineSeq"

     INNER JOIN -- get doc. dates and only docs within reported period
     "B1_MarketingDocumentsView" tblDocs 
       ON tblDocs."ObjType"  = tblMain."SrcObjType" AND
          tblDocs."DocEntry" = tblMain."SrcObjAbs" AND
          tblDocs."VatDate" >= :PeriodStart AND
          tblDocs."VatDate" <= :PeriodEnd

     INNER JOIN -- only docs where doc total VAT >= 1000000 (@VatLimit)
     (SELECT "AbsEntry",
        SUM(CASE WHEN "CrditDebit" = 'C' THEN IFNULL("BaseSum", 0) ELSE -IFNULL("BaseSum", 0) END) AS "DocBaseSum",
        SUM(CASE WHEN "CrditDebit" = 'C' THEN IFNULL("VatSum",  0) ELSE -IFNULL("VatSum",  0) END) AS "DocVatSum"
      FROM
        "copyTAX1"
      WHERE "IsAcq"   <> 'Y'           -- exclude lines with acquisition tax
        AND "TaxType" <> 'R'           -- exclude lines with reverse tax
        AND "TaxType" <> 'D'           -- exclude lines with deferred tax (temporary fix, util 2017-07-01 legal change)
      GROUP BY "AbsEntry"
      HAVING ABS(SUM(CASE WHEN "CrditDebit" = 'C' THEN "VatSum" ELSE -"VatSum" END)) >= :VatLimit
     ) AS tblBigVat
       ON tblBigVat."AbsEntry" = tblMain."AbsEntry"

      LEFT OUTER JOIN -- main BP, to access additional data
      OCRD tblTheBP
        ON tblTheBP."CardCode" = tblDocs."CardCode"

      LEFT OUTER JOIN -- connected BP, so it is possible to group data per the same partner (who is customer and vendor at the same time)
      OCRD tblSameBP
        ON tblSameBP."ConnBP" = tblDocs."CardCode"

  WHERE tblStandaloneDocs."AbsEntry" IS NULL                   -- exclude pure standalone docs
    AND tblMain."SrcObjType" IN (13, 15, 203, 18, 20, 204)     -- only main docs (not reversals) should be included:
                                                               --   <-- "in spite of a document which belongs to the report period and  corrected or canceled with CM or CRI should show in this area"
    AND tblDocs."CANCELED" <> 'C');                            -- exclude CANCELLATION documents - fix for CRM Incident 24635 / 2018

  --PRINT '  4.5 Standalone docs from chains prepared';


  --------------------------------------------------------------------------------
  -- 5. prepare docs in chain
  INSERT INTO "DocLinesInChain"
  (SELECT tblMain."AbsEntry", tblMain."AbsEntry", tblDocLines."LineSeq",
     tblMain."SrcObjType", tblMain."SrcObjAbs", tblMain."OrdinLNum", tblDocLines."SrcLineNum", tblDocLines."SrcArrType", tblDocLines."SrcGrpNum",
     tblDocLines."BaseObjTyp", tblDocLines."BaseAbs", -1, tblDocLines."BaseLinNum", tblDocLines."BaseArrTyp", tblDocLines."BaseGrpNum", 0, -1, -1, 0,
     tblDocs."CANCELED"
   FROM
     OTAX tblMain
     INNER JOIN -- doc lines which are not based on anything
     "copyTAX1" tblDocLines
       ON tblDocLines."AbsEntry" = tblMain."AbsEntry" AND
          tblDocLines."IsAcq"   <> 'Y' AND                 -- exclude lines with acquisition tax
          tblDocLines."TaxType" <> 'R' AND                 -- exclude lines with reverse tax
          tblDocLines."TaxType" <> 'D'                     -- exclude lines with deferred tax (temporary fix, util 2017-07-01 legal change)

     LEFT JOIN -- for downpayments, BaseAbs is set to an existing number, but that doc is not in OTAX/TAX1 (it is not tax relevant)
     OTAX tblCheckBase
       ON tblCheckBase."SrcObjType" = tblDocLines."BaseObjTyp" AND
          tblCheckBase."SrcObjAbs"  = tblDocLines."BaseAbs"

     INNER JOIN 
     "B1_MarketingDocumentsView" tblDocs
       ON tblDocs."ObjType"  = tblMain."SrcObjType" AND
          tblDocs."DocEntry" = tblMain."SrcObjAbs"

   WHERE (tblDocLines."BaseObjTyp" <= 0 OR --tblDocLines.BaseAbs <= 0 OR tblDocLines.BaseLinNum < 0 OR -- there is no base doc or
          tblCheckBase."AbsEntry" IS NULL)
  );

  SELECT COUNT(*) INTO temp_var_0
  FROM "DocLinesInChain"
  WHERE "Status" = 0;

  WHILE temp_var_0 > 0 DO
    UPDATE "DocLinesInChain"
    SET "Status" = 1
    WHERE "Status" = 0;

    INSERT INTO "DocLinesInChain"
    (SELECT tblBaseDoc."CommonTaxEntry", tblDocLines."AbsEntry", tblDocLines."LineSeq",
       tblMain."SrcObjType", tblMain."SrcObjAbs", tblMain."OrdinLNum", tblDocLines."SrcLineNum", tblDocLines."SrcArrType", tblDocLines."SrcGrpNum",
       tblBaseDoc."ObjType", tblBaseDoc."AbsEntry", tblBaseDoc."OrdinalNum", tblBaseDoc."LineNum", tblBaseDoc."ArrType", tblBaseDoc."GrpNum", 1, -1, -1, 0,
       tblDocs."CANCELED"
     FROM
       "DocLinesInChain" tblBaseDoc
       INNER JOIN
       "copyTAX1" tblDocLines
         ON tblDocLines."BaseObjTyp" = tblBaseDoc."ObjType" AND
            tblDocLines."BaseAbs"    = tblBaseDoc."AbsEntry" AND
            tblDocLines."BaseLinNum" = tblBaseDoc."LineNum" AND
            tblDocLines."BaseArrTyp" = tblBaseDoc."ArrType" AND
            tblDocLines."BaseGrpNum" = tblBaseDoc."GrpNum"
       INNER JOIN
       OTAX tblMain
         ON tblMain."AbsEntry" = tblDocLines."AbsEntry"
       INNER JOIN
       "B1_MarketingDocumentsView" tblDocs
         ON tblDocs."ObjType"  = tblMain."SrcObjType" AND
            tblDocs."DocEntry" = tblMain."SrcObjAbs"
     WHERE tblBaseDoc."Status" = 1
    );

    UPDATE "DocLinesInChain"
    SET "Status" = 2
    WHERE "Status" = 1;

    SELECT COUNT(*) INTO temp_var_0
    FROM "DocLinesInChain"
    WHERE "Status" = 0;
  END WHILE;

  --PRINT '  5. Docs in chains prepared';


  --------------------------------------------------------------------------------
  -- 6. update "IsInPeriod", "IsAboveVatLimit" flag for docs in chain
  UPDATE tblMain
  SET "IsInPeriod"      = (CASE WHEN tblDocs."VatDate" >= :PeriodStart AND tblDocs."VatDate" <= :PeriodEnd THEN 1 ELSE 0 END),
      "IsAboveVatLimit" = (CASE WHEN ABS("VatSum") >= :VatLimit THEN 1 ELSE 0 END)
  FROM
    "DocLinesInChain" tblMain

    INNER JOIN
    "B1_MarketingDocumentsView" tblDocs
      ON tblDocs."ObjType"  = tblMain."ObjType" AND
         tblDocs."DocEntry" = tblMain."AbsEntry"

    INNER JOIN
    (SELECT "AbsEntry", SUM(CASE WHEN "CrditDebit" = 'C' THEN IFNULL("VatSum", 0) ELSE -IFNULL("VatSum", 0) END) AS "VatSum"
     FROM "copyTAX1"
     WHERE "IsAcq"   <> 'Y'          -- exclude lines with acquisition tax
       AND "TaxType" <> 'R'          -- exclude lines with reverse tax
       AND "TaxType" <> 'D'          -- exclude lines with deferred tax (temporary fix, util 2017-07-01 legal change)
     GROUP BY "AbsEntry"
    ) AS tblDocVatSums
      ON tblDocVatSums."AbsEntry" = tblMain."TaxEntry";

  --PRINT '  6. Docs in chains updated';


  --------------------------------------------------------------------------------
  -- 6.5 update "IsAboveVatLimit" flag for docs in chain, when chain total VAT >= 1000000, but each particular doc VAT <= 1000000
  UPDATE tblMain
  SET "IsAboveVatLimit" = 1
  FROM
    "DocLinesInChain" tblMain
    INNER JOIN
    (SELECT tblMainChains."CommonTaxEntry"
     FROM 
       (SELECT DISTINCT "CommonTaxEntry", "TaxEntry"
        FROM 
          "DocLinesInChain"
        WHERE "ObjType" IN (13, 165, 166, 203, 19, 163, 164, 204)
       ) AS tblMainChains

       INNER JOIN
       (SELECT "AbsEntry", SUM(CASE WHEN "CrditDebit" = 'C' THEN IFNULL("VatSum", 0) ELSE -IFNULL("VatSum", 0) END) AS "VatSum"
        FROM "copyTAX1"
        WHERE "IsAcq"   <> 'Y'          -- exclude lines with acquisition tax
          AND "TaxType" <> 'R'          -- exclude lines with reverse tax
          AND "TaxType" <> 'D'          -- exclude lines with deferred tax (temporary fix, util 2017-07-01 legal change)
        GROUP BY "AbsEntry"
       ) AS tblDocVatSums
         ON tblDocVatSums."AbsEntry" = tblMainChains."TaxEntry"

     GROUP BY tblMainChains."CommonTaxEntry"
     HAVING ABS(SUM(tblDocVatSums."VatSum")) >= :VatLimit
    ) AS tblChainDocs
      ON tblChainDocs."CommonTaxEntry" = tblMain."CommonTaxEntry";

  --PRINT '  6.5 Docs in chains updated (chain total)';


  --------------------------------------------------------------------------------
  -- 6.8 update "Cancelled"
  UPDATE tblMain
  SET "Cancelled" = tblDocs."CANCELED"
  FROM
    "DocLinesInChain" tblMain
    INNER JOIN
    "B1_MarketingDocumentsView" tblDocs
      ON tblDocs."ObjType"  = tblMain."ObjType" AND
         tblDocs."DocEntry" = tblMain."AbsEntry";

  --PRINT '  6.8 Cancelled docs inb chain recognized;


  --------------------------------------------------------------------------------
  -- 7. calc docs in chain relevant for reported period
  INSERT INTO "ReportedDocLines"
  (SELECT 'chain' AS "DocType", tblMain."CommonTaxEntry", tblMain."TaxEntry", tblMain."LineSeq",
     (CASE WHEN tblMain."IsBased" = 0 THEN 'E' WHEN tblMain."IsInPeriod" = 1 THEN 'KT' ELSE 'K' END) AS "FlagOriginal",
     (CASE WHEN tblDocs."CardCode" IS NULL THEN '' ELSE tblDocs."CardCode" END) AS "CardCode",
      tblDocs."CardName", tblDocs."LicTradNum", tblTheBP."AddID", tblTheBP."VatIdUnCmp",
      tblSameBP."CardCode", tblSameBP."CardName", tblSameBP."LicTradNum", tblSameBP."AddID", tblSameBP."VatIdUnCmp", tblDocs."NumAtCard",
      tblMain."ObjType", tblMain."AbsEntry",
      (CASE WHEN tblDocs."DocNum" IS NULL THEN -1 ELSE tblDocs."DocNum" END) AS "DocNum",
      tblMain."OrdinalNum", tblMain."LineNum", tblMain."ArrType", tblMain."GrpNum",
      tblMain."BaseObjType", tblMain."BaseAbsEntry", tblBaseDocs."DocNum", tblMain."BaseOrdinalNum",
      tblMain."BaseLineNum", tblMain."BaseArrType", tblMain."BaseGrpNum",
      tblChains."CountDocsInChain",
      tblDocLine."TaxCode", (CASE WHEN tblDocLine."IsAcq" = 'Y' THEN 'A' ELSE tblDocLine."TaxType" END) AS "TaxType",
      (CASE WHEN tblDocLine."CrditDebit" = 'C' THEN tblDocLine."BaseSum" ELSE -tblDocLine."BaseSum" END) AS "BaseSum",
      (CASE WHEN tblDocLine."CrditDebit" = 'C' THEN tblDocLine."VatSum" ELSE -tblDocLine."VatSum" END) AS "VatSum",
      tblDocSum."DocBaseSum", tblDocSum."DocVatSum",
      (CASE WHEN tblDocs."DocDate" IS NULL THEN '1900-01-01' ELSE tblDocs."DocDate" END) AS "DocDate",
      (CASE WHEN tblDocs."VatDate" IS NULL THEN '1900-01-01' ELSE tblDocs."VatDate" END) AS "VatDate",
      (CASE WHEN tblDocs."TaxDate" IS NULL THEN '1900-01-01' ELSE tblDocs."TaxDate" END) AS "TaxDate",
      tblDocs."CANCELED"
  FROM
    "DocLinesInChain" tblMain
    INNER JOIN -- at least 1 doc in chain must be within reported perdiod, and at least 1 doc in chain must have VAT >= 1000000 (@VatLimit)
    (SELECT "CommonTaxEntry", COUNT(DISTINCT "TaxEntry") AS "CountDocsInChain"
     FROM
       "DocLinesInChain"
     GROUP BY "CommonTaxEntry"
     HAVING MAX("IsBased")         > 0
        AND MAX("IsInPeriod")      > 0
        AND MAX("IsAboveVatLimit") > 0
    ) AS tblChains
        ON tblChains."CommonTaxEntry" = tblMain."CommonTaxEntry"

    INNER JOIN
    "copyTAX1" tblDocLine
      ON tblDocLine."AbsEntry" = tblMain."TaxEntry" AND
         tblDocLine."LineSeq"  = tblMain."LineSeq" AND
         tblDocLine."IsAcq"   <> 'Y' AND      -- exclude lines with acquisition tax
         tblDocLine."TaxType" <> 'R' AND      -- exclude lines with reverse tax
         tblDocLine."TaxType" <> 'D'          -- exclude lines with deferred tax (temporary fix, util 2017-07-01 legal change)

    INNER JOIN -- calculate total doc VAT
    (SELECT "AbsEntry",
       SUM(CASE WHEN "CrditDebit" = 'C' THEN IFNULL("BaseSum", 0) ELSE -IFNULL("BaseSum", 0) END) AS "DocBaseSum",
       SUM(CASE WHEN "CrditDebit" = 'C' THEN IFNULL("VatSum",  0) ELSE -IFNULL("VatSum",  0) END) AS "DocVatSum"
     FROM
       "copyTAX1"
     WHERE "IsAcq"   <> 'Y'          -- exclude lines with acquisition tax
       AND "TaxType" <> 'R'          -- exclude lines with reverse tax
       AND "TaxType" <> 'D'          -- exclude lines with deferred tax (temporary fix, util 2017-07-01 legal change)
     GROUP BY "AbsEntry"
    ) AS tblDocSum
      ON tblDocSum."AbsEntry" = tblMain."TaxEntry"

    LEFT JOIN -- get doc. data
    "B1_MarketingDocumentsView" tblDocs
      ON tblDocs."ObjType"  = tblMain."ObjType" AND
         tblDocs."DocEntry" = tblMain."AbsEntry"

    LEFT JOIN -- get base doc. data
    "B1_MarketingDocumentsView" tblBaseDocs
      ON tblBaseDocs."ObjType"  = tblMain."BaseObjType" AND
         tblBaseDocs."DocEntry" = tblMain."BaseAbsEntry"

    LEFT OUTER JOIN -- main BP, to access additional data
    OCRD tblTheBP
      ON tblTheBP."CardCode" = tblDocs."CardCode"

    LEFT JOIN -- connected BP, so it is possible to group data per the same partner (who is customer and vendor at the same time)
    OCRD tblSameBP
      ON tblSameBP."ConnBP" = tblDocs."CardCode");

  --PRINT '  7. Docs in chains transferred to result';


  --------------------------------------------------------------------------------
  -- 8. standalone AP docs which were skipped because of low doc VAT sum (but total BP VAT sum >= 1000000)
  INSERT INTO "ReportedDocLines"
  (SELECT
     'rest AP' AS "DocType", NULL, tblMain."AbsEntry", tblLine."LineSeq",
     (CASE WHEN tblMain."SrcObjType" IN (13,15,203,18,20,204) THEN 'E'
           WHEN tblMain."SrcObjType" IN (14,16,165,166,19,21,163,164) THEN 'KT'
           ELSE '' END) AS "FlagOriginal",
     (CASE WHEN tblDocs."CardCode" IS NULL THEN '' ELSE tblDocs."CardCode" END) AS "CardCode",
     tblDocs."CardName", tblDocs."LicTradNum", tblTheBP."AddID", tblTheBP."VatIdUnCmp",
     tblSameBP."CardCode", tblSameBP."CardName", tblSameBP."LicTradNum", tblSameBP."AddID", tblSameBP."VatIdUnCmp", tblDocs."NumAtCard",
     tblMain."SrcObjType", tblMain."SrcObjAbs",
     (CASE WHEN tblDocs."DocNum" IS NULL THEN -1 ELSE tblDocs."DocNum" END) AS "DocNum",
     tblMain."OrdinLNum", tblLine."SrcLineNum", tblLine."SrcArrType", tblLine."SrcGrpNum",
     -1, -1, NULL, -1, -1, -1, -1,
     1,
     tblLine."TaxCode", (CASE WHEN tblLine."IsAcq" = 'Y' THEN 'A' ELSE tblLine."TaxType" END) AS "TaxType",
     (CASE WHEN tblLine."CrditDebit" = 'C' THEN tblLine."BaseSum" ELSE -tblLine."BaseSum" END) AS "BaseSum",
     (CASE WHEN tblLine."CrditDebit" = 'C' THEN tblLine."VatSum" ELSE -tblLine."VatSum" END) AS "VatSum",
     tblBigVat."DocBaseSum", tblBigVat."DocVatSum",
     (CASE WHEN tblDocs."DocDate" IS NULL THEN '1900-01-01' ELSE tblDocs."DocDate" END) AS "DocDate",
     (CASE WHEN tblDocs."VatDate" IS NULL THEN '1900-01-01' ELSE tblDocs."VatDate" END) AS "VatDate",
     (CASE WHEN tblDocs."TaxDate" IS NULL THEN '1900-01-01' ELSE tblDocs."TaxDate" END) AS "TaxDate",
     tblDocs."CANCELED"
   FROM
     OTAX tblMain

     INNER JOIN -- document lines, freights, ...
     "copyTAX1" tblLine
       ON tblLine."AbsEntry" = tblMain."AbsEntry" AND
          tblLine."IsAcq"   <> 'Y' AND      -- exclude lines with acquisition tax
          tblLine."TaxType" <> 'R' AND      -- exclude lines with reverse tax
          tblLine."TaxType" <> 'D'          -- exclude lines with deferred tax (temporary fix, util 2017-07-01 legal change)

     LEFT JOIN -- exclude already reported docs/chains from this calculation (see WHERE condition)
     "ReportedDocLines" tblAlreadyReported
       ON tblAlreadyReported."TaxEntry" = tblLine."AbsEntry" AND
          tblAlreadyReported."LineSeq"  = tblLine."LineSeq"

     INNER JOIN -- get doc. dates and only docs within reported period
     "B1_MarketingDocumentsView" tblDocs
       ON tblDocs."ObjType"  = tblMain."SrcObjType" AND
          tblDocs."DocEntry" = tblMain."SrcObjAbs" AND
          tblDocs."VatDate" >= :PeriodStart AND
          tblDocs."VatDate" <= :PeriodEnd

     INNER JOIN -- BP total VAT per reported period >= 1000000 (@VatLimit)
     (SELECT tblDocsBigBP."LicTradNum" AS "LicTradNum",
        SUM(CASE WHEN tblLineBigBP."CrditDebit" = 'C' THEN tblLineBigBP."VatSum" ELSE -tblLineBigBP."VatSum" END) AS "LicTradNumVat"
      FROM
        "copyTAX1" tblLineBigBP
        INNER JOIN
        OTAX tblMainBigBP
          ON tblMainBigBP."AbsEntry" = tblLineBigBP."AbsEntry"
        INNER JOIN
        "B1_MarketingDocumentsView" tblDocsBigBP
          ON tblDocsBigBP."ObjType"  = tblMainBigBP."SrcObjType" AND
             tblDocsBigBP."DocEntry" = tblMainBigBP."SrcObjAbs" AND
             tblDocsBigBP."VatDate" >= :PeriodStart AND
             tblDocsBigBP."VatDate" <= :PeriodEnd
      WHERE tblLineBigBP."IsAcq"   <> 'Y'          -- exclude lines with acquisition tax
        AND tblLineBigBP."TaxType" <> 'R'          -- exclude lines with reverse tax
        AND tblMainBigBP."SrcObjType" IN (18, 19, 20, 21, 163, 164, 204)        -- only AP side
      GROUP BY tblDocsBigBP."LicTradNum"
      HAVING ABS(SUM(CASE WHEN tblLineBigBP."CrditDebit" = 'C' THEN tblLineBigBP."VatSum" ELSE -tblLineBigBP."VatSum" END)) >= :VatLimit
     ) AS tblBigBP
       ON tblBigBP."LicTradNum" = tblDocs."LicTradNum"

     INNER JOIN -- total doc VAT
     (SELECT "AbsEntry",
        SUM(CASE WHEN "CrditDebit" = 'C' THEN IFNULL("BaseSum", 0) ELSE -IFNULL("BaseSum", 0) END) AS "DocBaseSum",
        SUM(CASE WHEN "CrditDebit" = 'C' THEN IFNULL("VatSum",  0) ELSE -IFNULL("VatSum",  0) END) AS "DocVatSum"
      FROM
        "copyTAX1"
      WHERE "IsAcq"   <> 'Y'          -- exclude lines with acquisition tax
        AND "TaxType" <> 'R'          -- exclude lines with reverse tax
        AND "TaxType" <> 'D'          -- exclude lines with deferred tax (temporary fix, util 2017-07-01 legal change)
      GROUP BY "AbsEntry"
     ) AS tblBigVat
       ON tblBigVat."AbsEntry" = tblMain."AbsEntry"

     LEFT OUTER JOIN -- main BP, to access additional data
     OCRD tblTheBP
       ON tblTheBP."CardCode" = tblDocs."CardCode"

     LEFT OUTER JOIN -- connected BP, so it is possible to group data per the same partner (who is customer and vendor at the same time)
     OCRD tblSameBP
       ON tblSameBP."ConnBP" = tblDocs."CardCode"
   WHERE tblAlreadyReported."TaxEntry" IS NULL
     AND tblMain."SrcObjType" IN (18, 19, 20, 21, 163, 164, 204)        -- 204 = Downpayment (outgoing)
  );

  --PRINT '  8. Remaining standalone AP docs prepared';


  --------------------------------------------------------------------------------
  -- 10. prepare Journal Entries
  INSERT INTO "ReportedDocLines"
  (SELECT 'tax booking' AS "DocType", NULL, tblMain."AbsEntry", tblLine."LineSeq", '',
      IFNULL(tblDocs."CardCode", ''), tblDocs."CardName", tblDocs."LicTradNum", tblTheBP."AddID", tblTheBP."VatIdUnCmp",
      tblSameBP."CardCode", tblSameBP."CardName", tblSameBP."LicTradNum", tblSameBP."AddID", tblSameBP."VatIdUnCmp", tblDocs."NumAtCard",
      tblMain."SrcObjType", tblMain."SrcObjAbs", tblDocs."DocNum", tblMain."OrdinLNum", tblLine."SrcLineNum", tblLine."SrcArrType", tblLine."SrcGrpNum",
      -1, -1, NULL, -1, -1, -1, -1,
      1,
      tblLine."TaxCode", (CASE WHEN tblLine."IsAcq" = 'Y' THEN 'A' ELSE tblLine."TaxType" END) AS "TaxType",
      (CASE WHEN tblLine."CrditDebit" = 'C' THEN IFNULL(tblLine."BaseSum", 0) ELSE -IFNULL(tblLine."BaseSum", 0) END) AS "BaseSum",
      (CASE WHEN tblLine."CrditDebit" = 'C' THEN IFNULL(tblLine."VatSum",  0) ELSE -IFNULL(tblLine."VatSum",  0) END) AS "VatSum",
      tblBigVat."DocBaseSum",
      tblBigVat."DocVatSum",
      tblDocs."DocDate", tblDocs."VatDate", tblDocs."TaxDate",
      'N'
   FROM
     "copyTAX1" tblLine
     INNER JOIN
     OTAX tblMain
       ON tblMain."AbsEntry"   = tblLine."AbsEntry" AND
          tblMain."SrcObjType" = 30           -- only Journal Ent. (30)

     INNER JOIN -- get doc. dates and only docs within reported period
     "B1_VatView" tblDocs
       ON tblDocs."SrcObjType" = tblMain."SrcObjType" AND
          tblDocs."SrcObjAbs"  = tblMain."SrcObjAbs" AND
          tblDocs."VatDate"   >= :PeriodStart AND
          tblDocs."VatDate"   <= :PeriodEnd

     INNER JOIN -- only docs where doc total VAT >= 1000000 (@VatLimit)
     (SELECT tblLine."AbsEntry",
        SUM(CASE WHEN "CrditDebit" = 'C' THEN IFNULL("BaseSum", 0) ELSE -IFNULL("BaseSum", 0) END) AS "DocBaseSum",
        SUM(CASE WHEN "CrditDebit" = 'C' THEN IFNULL("VatSum",  0) ELSE -IFNULL("VatSum",  0) END) AS "DocVatSum"
      FROM
        "copyTAX1" tblLine
      WHERE tblLine."IsAcq"   <> 'Y'          -- exclude lines with acquisition tax
        AND tblLine."TaxType" <> 'R'          -- exclude lines with reverse tax
        AND tblLine."TaxType" <> 'D'          -- exclude lines with deferred tax (temporary fix, util 2017-07-01 legal change)
      GROUP BY tblLine."AbsEntry"
     ) AS tblBigVat
       ON tblBigVat."AbsEntry" = tblMain."AbsEntry"

     LEFT OUTER JOIN -- main BP, to access additional data
     OCRD tblTheBP
       ON tblTheBP."CardCode" = tblDocs."CardCode"

     LEFT OUTER JOIN -- connected BP, so it is possible to group data per the same partner (who is customer and vendor at the same time)
     OCRD tblSameBP
         ON tblSameBP."ConnBP" = tblDocs."CardCode"

   WHERE tblLine."IsAcq"   <> 'Y'             -- exclude lines with acquisition tax
     AND tblLine."TaxType" <> 'R'             -- exclude lines with reverse tax
  );

  --PRINT ' 10. Journal Entries prepared';


  --------------------------------------------------------------------------------
  -- 11. prepare Payments
  INSERT INTO "ReportedDocLines"
  (SELECT 'tax booking' AS "DocType", NULL, tblMain."AbsEntry", tblLine."LineSeq", '',
      '', NULL, NULL, NULL,
      tblDocs."LicTradNum",
      NULL, NULL, NULL, NULL, NULL, NULL,
      tblMain."SrcObjType", tblMain."SrcObjAbs", tblDocs."DocEntry", tblMain."OrdinLNum", tblLine."SrcLineNum", tblLine."SrcArrType", tblLine."SrcGrpNum",
      -1, -1, NULL, -1, -1, -1, -1,
      1,
      tblLine."TaxCode", (CASE WHEN tblLine."IsAcq" = 'Y' THEN 'A' ELSE tblLine."TaxType" END) AS "TaxType",
      (CASE WHEN tblLine."CrditDebit" = 'C' THEN IFNULL(tblLine."BaseSum", 0) ELSE -IFNULL(tblLine."BaseSum", 0) END) AS "BaseSum",
      (CASE WHEN tblLine."CrditDebit" = 'C' THEN IFNULL(tblLine."VatSum",  0) ELSE -IFNULL(tblLine."VatSum",  0) END) AS "VatSum",
      tblBigVat."DocBaseSum",
      tblBigVat."DocVatSum",
      tblDocs."DocDate", tblDocs."VatDate", tblDocs."TaxDate",
      tblDocs."Canceled"
   FROM
     "copyTAX1" tblLine

     INNER JOIN
     OTAX tblMain
       ON tblMain."AbsEntry" = tblLine."AbsEntry" AND
          tblMain."SrcObjType" IN (24, 46)    -- only Payments (24 = incoming, 46 = outgoing)

     INNER JOIN -- payments data and selection
     (SELECT "ObjType", "DocEntry", "DocDate", "VatDate", "TaxDate",
        NULL AS "LicTradNum",
        "Canceled"
      FROM OVPM
      WHERE "DocType"  = 'A'
        AND "ApplyVAT" = 'Y'
        AND "VatDate" >= :PeriodStart
        AND "VatDate" <= :PeriodEnd
      UNION ALL
      SELECT "ObjType", "DocEntry", "DocDate", "VatDate", "TaxDate",
        NULL AS "LicTradNum",
        "Canceled"
      FROM ORCT
      WHERE "DocType"  = 'A'
        AND "ApplyVAT" = 'Y'
        AND "VatDate" >= :PeriodStart
        AND "VatDate" <= :PeriodEnd
     ) AS tblDocs
       ON tblDocs."ObjType"  = tblMain."SrcObjType" AND
          tblDocs."DocEntry" = tblMain."SrcObjAbs"

     INNER JOIN -- only docs where doc total VAT >= 1000000 (@VatLimit)
     (SELECT tblLine."AbsEntry",
        SUM(CASE WHEN "CrditDebit" = 'C' THEN IFNULL("BaseSum", 0) ELSE -IFNULL("BaseSum", 0) END) AS "DocBaseSum",
        SUM(CASE WHEN "CrditDebit" = 'C' THEN IFNULL("VatSum",  0) ELSE -IFNULL("VatSum",  0) END) AS "DocVatSum"
      FROM
        "copyTAX1" tblLine
      WHERE tblLine."IsAcq"   <> 'Y'          -- exclude lines with acquisition tax
        AND tblLine."TaxType" <> 'R'          -- exclude lines with reverse tax
        AND tblLine."TaxType" <> 'D'          -- exclude lines with deferred tax (temporary fix, util 2017-07-01 legal change)
      GROUP BY tblLine."AbsEntry"
     ) AS tblBigVat
       ON tblBigVat."AbsEntry" = tblMain."AbsEntry"

   WHERE tblLine."IsAcq"   <> 'Y'             -- exclude lines with acquisition tax
     AND tblLine."TaxType" <> 'R'             -- exclude lines with reverse tax
  );

  --PRINT ' 11. Payments prepared';


  --------------------------------------------------------------------------------
  -- 11.5 update "Cancelled" docs sums
  UPDATE "ReportedDocLines"
  SET "BaseSum"    = -"BaseSum",
      "VatSum"     = -"VatSum",
      "DocBaseSum" = -"DocBaseSum",
      "DocVatSum"  = -"DocVatSum"
  WHERE "Cancelled" = 'C';
  --PRINT ' 11.5 Cancelled docs inb chain recognized;

  --------------------------------------------------------------------------------
  -- final cleaning and resource release
  DROP TABLE "copyTAX1";

  --------------------------------------------------------------------------------
  -- 12. print result data (EN and HU versions of headers are included)
  SELECT
    --ConnectedBPLicTradNum AS "Vendor Tax ID", ConnectedBP AS "Vendor Code", ConnectedBPName AS "Vendor Name",
    --ConnectedBPAdditionalID AS "Vendor Add. ID", ConnectedBPUnifiedTaxID AS "Vendor Unified Tax ID",
    --BPLicTradNum AS "Customer Tax ID", BP AS "Customer Code", BPName AS "Customer Name",
    --BPAdditionalID AS "Customer Add. ID", BPUnifiedTaxID AS "Customer Unified Tax ID",
    --RptType AS "Report Type", CAST(PageType AS INTEGER) AS "Page Type", DocChain, DocType AS "Doc. Type",
    --VendorRefNum AS "Vendor Ref. Num.", DocNum AS "Doc Number", CorrType AS "Correction Type",
    --BaseDocNum AS "Base Doc Number",
    --CONVERT(NVARCHAR(10), DocDate, 111) AS PostingDate, CONVERT(NVARCHAR(10), VatDate, 111) AS VatDate, CONVERT(NVARCHAR(10), TaxDate, 111) AS DocDate,
    --IsNULL(dbo.GetReportedDocSum_HU(ObjType, BaseSum), 0) AS DocBaseSum,
    --IsNULL(dbo.GetReportedDocSum_HU(ObjType, VatSum),  0) AS DocVatSum,
    --Cancelled

    "ConnectedBPLicTradNum" AS "Szallitoi Adoszam", "ConnectedBP" AS "Szallito Kodja", "ConnectedBPName" AS "Szallito Neve",
    "ConnectedBPAdditionalID" AS "Szallito 2. ID-szam", "ConnectedBPUnifiedTaxID" AS "Szallito Egyseges Adoszam",
    "BPLicTradNum" AS "Vevoi Adoszam", "BP" AS "Vevo Kodja", "BPName" AS "Vevo Neve",
    "BPAdditionalID" AS "Vevo 2. ID-szam", "BPUnifiedTaxID" AS "Vevo Egyseges Adoszam",
    "RptType" AS "Riport Tipus", CAST("PageType" AS INTEGER) AS "Oldaltipus", "DocChain" AS "Dokument Lanc", "DocType" AS "Dokumentum Tipus",
    "VendorRefNum" AS "Szallitoi/Vevoi referencia szam", "DocNum" AS "Bizonylatszam", "CorrType" AS "Szamla Tipus", 
    "BaseDocNum" AS "Elozmeny szamla sorszama",
    CONCAT(CONCAT(CONCAT(CONCAT(SUBSTRING(CAST ("DocDate" AS VARCHAR), 0, 4), '/'), SUBSTRING(CAST ("DocDate" AS VARCHAR), 6, 2)), '/'), SUBSTRING(CAST ("DocDate" AS VARCHAR), 9, 2)) AS "Konyvelesi Datum",
    CONCAT(CONCAT(CONCAT(CONCAT(SUBSTRING(CAST ("VatDate" AS VARCHAR), 0, 4), '/'), SUBSTRING(CAST ("VatDate" AS VARCHAR), 6, 2)), '/'), SUBSTRING(CAST ("VatDate" AS VARCHAR), 9, 2)) AS "AFA Datum",
    CONCAT(CONCAT(CONCAT(CONCAT(SUBSTRING(CAST ("TaxDate" AS VARCHAR), 0, 4), '/'), SUBSTRING(CAST ("TaxDate" AS VARCHAR), 6, 2)), '/'), SUBSTRING(CAST ("TaxDate" AS VARCHAR), 9, 2)) AS "Bizonylat Datuma",
    --"ObjType",
    (CASE WHEN "ObjType" IN (18, 19, 20, 21, 163, 164, 204, 30) THEN -IFNULL("BaseSum", 0) ELSE IFNULL("BaseSum", 0) END) AS "Bizonylat erteke",
    (CASE WHEN "ObjType" IN (18, 19, 20, 21, 163, 164, 204, 30) THEN -IFNULL("VatSum", 0)  ELSE IFNULL("VatSum", 0)  END) AS "Adoertek"
    --"Cancelled"
  FROM
   (SELECT -- standalone AR docs
      "BPLicTradNum", "BP", MIN("BPName") AS "BPName", MIN("BPAdditionalID") AS "BPAdditionalID", MIN("BPUnifiedTaxID") AS "BPUnifiedTaxID",
      MIN("ConnectedBPLicTradNum") AS "ConnectedBPLicTradNum", MIN("ConnectedBP") AS "ConnectedBP", MIN("ConnectedBPName") AS "ConnectedBPName",
      MIN("ConnectedBPAdditionalID") AS "ConnectedBPAdditionalID", MIN("ConnectedBPUnifiedTaxID") AS "ConnectedBPUnifiedTaxID",
      --'AR standalone - 01.lap' AS RptType,
      NCHAR(0x00C9) || N'rt' || NCHAR(0x00E9) || N'kes' || NCHAR(0x00ED) || N't' || NCHAR(0x00E9) || N's - 01.lap' AS "RptType",
      1 AS "PageType",
      (CASE WHEN "CommonTaxEntry" <= 0 THEN NULL ELSE "CommonTaxEntry" END) AS "DocChain", MIN("TaxEntry") AS "TaxEntry",
      "ObjType", (SELECT "ObjTypeName" FROM "TMP_HU_DOC_TYPE_NAME" T WHERE T."ObjType" = rdl."ObjType") AS "DocType",
      MIN("NumAtCard") AS "VendorRefNum", MIN("DocNum") AS "DocNum", (CASE WHEN "ObjType" IS NULL THEN NULL ELSE MAX("FlagOriginal") END) AS "CorrType",
      (CASE WHEN "ObjType" IN (14, 16, 165, 166, 19, 21, 163, 164) AND (MIN("BaseAbsEntry") IS NULL OR MIN("BaseAbsEntry") <= 0) THEN NULL --n'!'
            ELSE CAST(MIN("BaseDocNum") AS NVARCHAR(100)) END) AS "BaseDocNum",
      MIN("DocDate") AS "DocDate", MIN("VatDate") AS "VatDate", MIN("TaxDate") AS "TaxDate",
      SUM("BaseSum") AS "BaseSum",
      SUM("VatSum") AS "VatSum",
      "Cancelled"
    FROM "ReportedDocLines" rdl
    WHERE "DocType" = 'standalone'
      AND "ObjType" IN (13, 14, 15, 16, 165, 166, 203)
    GROUP BY "BPLicTradNum", "BP", "CommonTaxEntry", "ObjType", "AbsEntry", "Cancelled"

    UNION ALL

    SELECT -- AR docs in chain
      "BPLicTradNum", "BP", MIN("BPName") AS "BPName", MIN("BPAdditionalID") AS "BPAdditionalID", MIN("BPUnifiedTaxID") AS "BPUnifiedTaxID",
      MIN("ConnectedBPLicTradNum") AS "ConnectedBPLicTradNum", MIN("ConnectedBP") AS "ConnectedBP", MIN("ConnectedBPName") AS "ConnectedBPName",
      MIN("ConnectedBPAdditionalID") AS "ConnectedBPAdditionalID", MIN("ConnectedBPUnifiedTaxID") AS "ConnectedBPUnifiedTaxID",
      --'AR chain - 01-K lap' AS RptType,
      NCHAR(0x00C9) || N'rt' || NCHAR(0x00E9) || N'kes' || NCHAR(0x00ED) || N't' || NCHAR(0x00E9) || N's korrekci' || NCHAR(0x00F3) || N'ja - 01-K lap' AS "RptType",
      2 AS "PageType",
      (CASE WHEN "CommonTaxEntry" <= 0 THEN NULL ELSE "CommonTaxEntry" END) AS "DocChain", MIN("TaxEntry") AS "TaxEntry",
      "ObjType", (SELECT "ObjTypeName" FROM "TMP_HU_DOC_TYPE_NAME" T WHERE T."ObjType" = rdl."ObjType") AS "DocType",
      MIN("NumAtCard") AS "VendorRefNum", MIN("DocNum") AS "DocNum", (CASE WHEN "ObjType" IS NULL THEN NULL ELSE MAX("FlagOriginal") END) AS "CorrType",
      (CASE WHEN "ObjType" IN (14, 16, 165, 166, 19, 21, 163, 164) AND (MIN("BaseAbsEntry") IS NULL OR MIN("BaseAbsEntry") <= 0) THEN NULL --n'!'
            ELSE CAST(MIN("BaseDocNum") AS NVARCHAR(100)) END) AS "BaseDocNum",
      MIN("DocDate") AS "DocDate", MIN("VatDate") AS "VatDate", MIN("TaxDate") AS "TaxDate", 
      SUM("BaseSum") AS "BaseSum",
      SUM("VatSum") AS "VatSum",
      "Cancelled"
    FROM "ReportedDocLines" rdl
    WHERE "DocType" = 'chain'
      AND "ObjType" IN (13, 14, 15, 16, 165, 166, 203)
    GROUP BY "BPLicTradNum", "BP", "CommonTaxEntry", "ObjType", "AbsEntry", "Cancelled"
    
    UNION ALL
    
    SELECT MIN("ConnectedBPLicTradNum"), MIN("ConnectedBP"), MIN("ConnectedBPName") AS "BPName",
      MIN("ConnectedBPAdditionalID") AS "ConnectedBPAdditionalID", MIN("ConnectedBPUnifiedTaxID") AS "ConnectedBPUnifiedTaxID",
      "BPLicTradNum" AS "ConnectedBPLicTradNum", "BP" AS "ConnectedBP", MIN("BPName") AS "ConnectedBPName",
      MIN("BPAdditionalID") AS "BPAdditionalID", MIN("BPUnifiedTaxID") AS "BPUnifiedTaxID",
      --'AP standalone - 02 lap' AS RptType,
      N'Beszerz' || NCHAR(0x00E9) || N's - 02.lap' AS "RptType",
      3 AS "PageType",
      (CASE WHEN "CommonTaxEntry" <= 0 THEN NULL ELSE "CommonTaxEntry" END) AS "DocChain", MIN("TaxEntry") AS "TaxEntry",
      "ObjType", (SELECT "ObjTypeName" FROM "TMP_HU_DOC_TYPE_NAME" T WHERE T."ObjType" = rdl."ObjType") AS "DocType",
      MIN("NumAtCard") AS "VendorRefNum", MIN("DocNum") AS "DocNum", (CASE WHEN "ObjType" IS NULL THEN NULL ELSE MAX("FlagOriginal") END) AS "CorrType",
      (CASE WHEN "ObjType" IN (14, 16, 165, 166, 19, 21,163, 164) AND (MIN("BaseAbsEntry") IS NULL OR MIN("BaseAbsEntry") <= 0) THEN NULL --n'!'
            ELSE CAST(MIN("BaseDocNum") AS NVARCHAR(100)) END) AS "BaseDocNum",
      MIN("DocDate") AS "DocDate", MIN("VatDate") AS "VatDate", MIN("TaxDate") AS "TaxDate",
      SUM("BaseSum") AS "BaseSum",
      SUM("VatSum") AS "VatSum",
      "Cancelled"
    FROM "ReportedDocLines" rdl
    WHERE "DocType" = 'standalone'
      AND "ObjType" IN (18, 19, 20, 21, 163, 164, 204)
    GROUP BY "BPLicTradNum", "BP", "CommonTaxEntry", "ObjType", "AbsEntry", "Cancelled"

    UNION ALL

    SELECT -- AP docs in chain
      MIN("ConnectedBPLicTradNum"), MIN("ConnectedBP"), MIN("ConnectedBPName") AS "BPName",
      MIN("ConnectedBPAdditionalID") AS "ConnectedBPAdditionalID", MIN("ConnectedBPUnifiedTaxID") AS "ConnectedBPUnifiedTaxID",
      "BPLicTradNum" AS "ConnectedBPLicTradNum", "BP" AS "ConnectedBP", MIN("BPName") AS "ConnectedBPName",
      MIN("BPAdditionalID") AS "BPAdditionalID", MIN("BPUnifiedTaxID") AS "BPUnifiedTaxID",
      --'AP chain - 02-K lap' AS RptType,
      N'Beszerz' || NCHAR(0x00E9) || N's korrekci' || NCHAR(0x00F3) || N'ja - 02-K lap' AS "RptType",
      4 AS "PageType",
      (CASE WHEN "CommonTaxEntry" <= 0 THEN NULL ELSE "CommonTaxEntry" END) AS "DocChain", MIN("TaxEntry") AS "TaxEntry",
      "ObjType", (SELECT "ObjTypeName" FROM "TMP_HU_DOC_TYPE_NAME" T WHERE T."ObjType" = rdl."ObjType") AS "DocType",
      MIN("NumAtCard") AS "VendorRefNum", MIN("DocNum") AS "DocNum", (CASE WHEN "ObjType" IS NULL THEN NULL ELSE MAX("FlagOriginal") END) AS "CorrType",
      (CASE WHEN "ObjType" IN (14, 16, 165, 166, 19, 21, 163, 164) AND (MIN("BaseAbsEntry") IS NULL OR MIN("BaseAbsEntry") <= 0) THEN NULL --n'!'
            ELSE CAST(MIN("BaseDocNum") AS NVARCHAR(100)) END) AS "BaseDocNum",
      MIN("DocDate") AS "DocDate", MIN("VatDate") AS "VatDate", MIN("TaxDate") AS "TaxDate",
      SUM("BaseSum") AS "BaseSum", 
      SUM("VatSum") AS "VatSum",
      "Cancelled"
    FROM "ReportedDocLines" rdl
    WHERE "DocType" = 'chain'
      AND "ObjType" IN (18, 19, 20, 21, 163, 164, 204)
    GROUP BY "BPLicTradNum", "BP", "CommonTaxEntry", "ObjType", "AbsEntry", "Cancelled"

    UNION ALL

    -- OPTION 1 for section 6/D  - use/uncomment either Option 1 (separated docs in 6/D) or Option 2 (sum of all docs in 6/D)
    --
    --SELECT -- remaining AP docs, where sum per business partner within the selected period > 1000000, but each of these docs is < 1000000
    --  MIN("ConnectedBPLicTradNum"), MIN("ConnectedBP"), MIN("ConnectedBPName") AS "BPName",
    --  MIN("ConnectedBPAdditionalID") AS "ConnectedBPAdditionalID", MIN("ConnectedBPUnifiedTaxID") AS "ConnectedBPUnifiedTaxID",
    --  "BPLicTradNum" AS "ConnectedBPLicTradNum", "BP" AS "ConnectedBP", MIN("BPName") AS "ConnectedBPName",
    --  MIN("BPAdditionalID") AS "BPAdditionalID", MIN("BPUnifiedTaxID") AS "BPUnifiedTaxID",
    --  --'AP rest - 6/D' AS RptType,
    --  N'Egy' || NCHAR(0x00E9) || N'b beszerz' || NCHAR(0x00E9) || N's f' || NCHAR(0x0151) || N'lap - 6/D mez' || NCHAR(0x0151) AS "RptType",
    --  (CASE WHEN SUM("BaseSum") <  0 THEN 3
    --        WHEN SUM("BaseSum") >= 0 THEN 4
    --        --ELSE 5
    --  END) as "PageType",
    --  (CASE WHEN "CommonTaxEntry" <= 0 THEN NULL ELSE "CommonTaxEntry" END) AS "DocChain", MIN("TaxEntry") AS "TaxEntry",
    --  "ObjType", (SELECT "ObjTypeName" FROM "TMP_HU_DOC_TYPE_NAME" T WHERE T."ObjType" = rdl."ObjType") AS "DocType",
    --  MIN("NumAtCard") AS "VendorRefNum", MIN("DocNum") AS "DocNum", (CASE WHEN "ObjType" IS NULL THEN NULL ELSE MAX("FlagOriginal") END) AS "CorrType",
    --  (CASE WHEN "ObjType" IN (14, 16, 165, 166, 19, 21, 163, 164) AND (MIN("BaseAbsEntry") IS NULL OR MIN("BaseAbsEntry") <= 0) THEN NULL --n'!'
    --        ELSE CAST(MIN("BaseDocNum") AS NVARCHAR(100)) END) AS "BaseDocNum",
    --  MIN("DocDate") AS "DocDate", MIN("VatDate") AS "VatDate", MIN("TaxDate") AS "TaxDate",
    --  SUM("BaseSum") AS "BaseSum",
    --  SUM("VatSum") AS "VatSum",
    --  "Cancelled"
    --FROM "ReportedDocLines" rdl
    --WHERE "DocType" = 'rest AP'
    --GROUP BY "BPLicTradNum", "BP", "CommonTaxEntry", "ObjType", "AbsEntry", "Cancelled"
    ----------------------------------------

    -- OPTION 2 for section 6/D  - use/uncomment either Option 1 (separated docs in 6/D) or Option 2 (sum of all docs in 6/D)
    --
    SELECT -- remaining AP docs, where sum per business partner within the selected period > 1000000, but each of these docs is < 1000000
      MIN("ConnectedBPLicTradNum"), MIN("ConnectedBP"), MIN("ConnectedBPName") AS "BPName",
      MIN("ConnectedBPAdditionalID") AS "ConnectedBPAdditionalID", MIN("ConnectedBPUnifiedTaxID") AS "ConnectedBPUnifiedTaxID",
      "BPLicTradNum" AS "ConnectedBPLicTradNum", "BP" AS "ConnectedBP", MIN("BPName") AS "ConnectedBPName",
      MIN("BPAdditionalID") AS "BPAdditionalID", MIN("BPUnifiedTaxID") AS "BPUnifiedTaxID",
      --'AP rest - 6/D' AS RptType,
      N'Egy' || NCHAR(0x00E9) || N'b beszerz' || NCHAR(0x00E9) || N's f' || NCHAR(0x0151) || N'lap - 6/D mez' || NCHAR(0x0151) AS "RptType",
      (CASE WHEN SUM("BaseSum") <  0 THEN 3
            WHEN SUM("BaseSum") >= 0 THEN 4
            --ELSE 5
       END) AS "PageType",
      (CASE WHEN "CommonTaxEntry" <= 0 THEN NULL ELSE "CommonTaxEntry" END) AS "DocChain", MIN("TaxEntry") AS "TaxEntry",
      MIN("ObjType") AS "ObjType", NULL AS "DocType",
      MIN("NumAtCard") AS "VendorRefNum", MIN("DocNum") AS "DocNum", NULL AS "CorrType",
      '' AS "BaseDocNum",
      MIN("DocDate") AS "DocDate", MIN("VatDate") AS "VatDate", MIN("TaxDate") AS "TaxDate",
      SUM("BaseSum") AS "BaseSum",
      SUM("VatSum") AS "VatSum",
      NULL AS "Cancelled"
    FROM "ReportedDocLines" rdl
    WHERE "DocType" = 'rest AP'
    GROUP BY "BPLicTradNum", "BP", "CommonTaxEntry"
    ----------------------------------------

    UNION ALL

    SELECT -- journal entries and payments (on account)
      (CASE WHEN tblTaxCodes."Category" = 'O' THEN MIN("BPLicTradNum")            ELSE MIN("ConnectedBPLicTradNum") END)   AS "BPLicTradNum",
      (CASE WHEN tblTaxCodes."Category" = 'O' THEN MIN("BP")                      ELSE MIN("ConnectedBP") END)             AS "BP",
      (CASE WHEN tblTaxCodes."Category" = 'O' THEN MIN("BPName")                  ELSE MIN("ConnectedBPName") END)         AS "BPName",
      (CASE WHEN tblTaxCodes."Category" = 'O' THEN MIN("BPAdditionalID")          ELSE MIN("ConnectedBPAdditionalID") END) AS "BPAdditionalID",
      (CASE WHEN tblTaxCodes."Category" = 'O' THEN MIN("BPUnifiedTaxID")          ELSE MIN("ConnectedBPUnifiedTaxID") END) AS "BPUnifiedTaxID",
      (CASE WHEN tblTaxCodes."Category" = 'O' THEN MIN("ConnectedBPLicTradNum")   ELSE MIN("BPLicTradNum") END)            AS "ConnectedBPLicTradNum",
      (CASE WHEN tblTaxCodes."Category" = 'O' THEN MIN("ConnectedBP")             ELSE MIN("BP") END)                      AS "ConnectedBP",
      (CASE WHEN tblTaxCodes."Category" = 'O' THEN MIN("ConnectedBPName")         ELSE MIN("BPName") END)                  AS "ConnectedBPName",
      (CASE WHEN tblTaxCodes."Category" = 'O' THEN MIN("ConnectedBPAdditionalID") ELSE MIN("BPAdditionalID") END)          AS "ConnectedBPAdditionalID",
      (CASE WHEN tblTaxCodes."Category" = 'O' THEN MIN("ConnectedBPUnifiedTaxID") ELSE MIN("BPUnifiedTaxID") END)          AS "ConnectedBPUnifiedTaxID",
      --'JE and Payments' AS RptType,
      N'Egy' || NCHAR(0x00E9) || N'b t' || NCHAR(0x00E9) || N'telek' AS "RptType",
      (CASE WHEN tblTaxCodes."Category" = 'O' AND SUM("BaseSum") >= 0 THEN 1
            WHEN tblTaxCodes."Category" = 'O' AND SUM("BaseSum") <  0 THEN 2
            WHEN tblTaxCodes."Category" = 'I' AND SUM("BaseSum") <  0 THEN 3
            WHEN tblTaxCodes."Category" = 'I' AND SUM("BaseSum") >= 0 THEN 4
            ELSE 6
       END) as "PageType",
      NULL AS "DocChain", MIN("TaxEntry") AS "TaxEntry", "ObjType",
      (SELECT "ObjTypeName" FROM "TMP_HU_DOC_TYPE_NAME" T WHERE T."ObjType" = rdl."ObjType") AS "DocType",
      MIN("NumAtCard") AS "VendorRefNum", MIN("DocNum") AS "DocNum", (CASE WHEN "ObjType" IS NULL THEN NULL ELSE MAX("FlagOriginal") END) AS "CorrType",
      NULL AS "BaseDocNum",
      MIN("DocDate") AS "DocDate", MIN("VatDate") AS "VatDate", MIN("TaxDate") AS "TaxDate",
      SUM("BaseSum") AS "BaseSum",
      SUM("VatSum") AS "VatSum",
      "Cancelled"
    FROM
      "ReportedDocLines" rdl
      INNER JOIN
      OVTG tblTaxCodes
        ON tblTaxCodes."Code" = rdl."TaxCode"
    WHERE "DocType" = 'tax booking'
      AND "ObjType" IN (30, 24, 46)
    GROUP BY "ObjType", "AbsEntry", "LineNum", "TaxCode", "Cancelled", tblTaxCodes."Category"
   ) AS tblData

  ORDER BY  tblData."BPLicTradNum", tblData."BP", tblData."ConnectedBPLicTradNum", tblData."ConnectedBP",
            tblData."PageType", tblData."RptType", tblData."DocChain", tblData."DocDate", tblData."TaxEntry";
END;


