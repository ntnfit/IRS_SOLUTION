-- HANA: apz_Document_GetDocLine — lines of a marketing document (e.g. RDR1) with item/UoM/stock info, the
-- freight lines of groups 0..2 from the <prefix>2 table flattened as <col>_F1.._F3, and text/subtotal lines from
-- <prefix>10 appended as ItemType -18 rows.
-- Differences from HANA, same output shape:
--   * the temp table is shaped from @tblLine itself (HANA always used RDR1's shape)
--   * freight columns are taken from the freight table's catalog instead of a hard-coded list of 63 names
--   * table names are validated and quoted; @DocEntry is a parameter, not concatenated text
CREATE OR ALTER PROCEDURE dbo.apz_Document_GetDocLine
    @DocEntry int,
    @tbl varchar(30),
    @tblLine nvarchar(30)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @prefix nvarchar(30) = LEFT(@tblLine, LEN(@tblLine) - 1);   -- RDR1 -> RDR
    DECLARE @tblText nvarchar(30) = @prefix + N'10', @tblExpns nvarchar(30) = @prefix + N'2';
    IF OBJECT_ID(N'dbo.' + QUOTENAME(@tblLine), N'U') IS NULL
        THROW 50003, N'apz_Document_GetDocLine: unknown line table', 1;
    DECLARE @hasExpns bit = CASE WHEN OBJECT_ID(N'dbo.' + QUOTENAME(@tblExpns), N'U') IS NULL THEN 0 ELSE 1 END;
    DECLARE @hasText bit = CASE WHEN OBJECT_ID(N'dbo.' + QUOTENAME(@tblText), N'U') IS NULL THEN 0 ELSE 1 END;

    -- freight columns in HANA's order (only those present in this freight table are used)
    DECLARE @fgCols TABLE (ord int IDENTITY, name sysname);
    INSERT INTO @fgCols (name) VALUES (N'lineVatS'),(N'DrawnSC'),(N'RetReqLC'),(N'WtLiable'),(N'DedVatSumF'),(N'ExpnsCode'),(N'OcrCode'),
        (N'DedVatSumS'),(N'TrgGroup'),(N'VatGroup'),(N'PaidFC'),(N'ObjType'),(N'EquVatSumF'),(N'ExtTaxSum'),(N'VatSumSy'),(N'EquVatSumS'),
        (N'TaxAmtSrc'),(N'TotalSumSy'),(N'ExtTaxSumS'),(N'VatDscntPr'),(N'PaidToDate'),(N'VatSumFrgn'),(N'RetReqSC'),(N'TaxStatus'),
        (N'EquVatPer'),(N'DedVatSum'),(N'Project'),(N'VatSum'),(N'IsAcquistn'),(N'BaseMethod'),(N'VatAppldFC'),(N'TaxType'),(N'ExtTaxSumF'),
        (N'VatGrpSrc'),(N'ExtTaxRate'),(N'DrawnTotal'),(N'VatApplied'),(N'Stock'),(N'LineNum'),(N'VisOrder'),(N'BaseGroup'),(N'AnalysRpt'),
        (N'VatPrcnt'),(N'LineTotal'),(N'FixCurr'),(N'DocEntry'),(N'lineVatlF'),(N'VatAppldSC'),(N'TotalFrgn'),(N'LstPchPrce'),(N'TaxCode'),
        (N'Status'),(N'PaidSys'),(N'LogInstanc'),(N'EquVatSum'),(N'RetReqFC'),(N'DrawnFC'),(N'GroupNum'),(N'OcrCode2'),(N'OcrCode3'),
        (N'OcrCode4'),(N'OcrCode5'),(N'lineVat');

    DECLARE @fgSelect nvarchar(max) = N'', @fgJoin nvarchar(max) = N'';
    IF @hasExpns = 1
    BEGIN
        SELECT @fgSelect = STRING_AGG(CAST(N'FG' + g.n + N'.' + QUOTENAME(f.name) + N' AS ' + QUOTENAME(f.name + N'_F' + g.n) AS nvarchar(max)), N', ')
                           WITHIN GROUP (ORDER BY g.n, f.ord)
        FROM @fgCols f CROSS JOIN (VALUES (N'1'), (N'2'), (N'3')) g (n)
        WHERE COL_LENGTH(N'dbo.' + QUOTENAME(@tblExpns), f.name) IS NOT NULL;
        SET @fgSelect = N', ' + @fgSelect;
        SET @fgJoin = N'
LEFT JOIN ' + QUOTENAME(@tblExpns) + N' FG1 ON FG1.GroupNum = 0 AND A.DocEntry = FG1.DocEntry AND FG1.LineNum = A.LineNum
LEFT JOIN ' + QUOTENAME(@tblExpns) + N' FG2 ON FG2.GroupNum = 1 AND A.DocEntry = FG2.DocEntry AND FG2.LineNum = A.LineNum
LEFT JOIN ' + QUOTENAME(@tblExpns) + N' FG3 ON FG3.GroupNum = 2 AND A.DocEntry = FG3.DocEntry AND FG3.LineNum = A.LineNum';
    END;

    -- extra columns are CAST so the temp table accepts NULLs for the text lines appended later
    DECLARE @sql nvarchar(max) = N'
SELECT A.*,
       CAST(N'''' AS nvarchar(max)) AS LineText,
       CAST((SELECT Rate FROM OVTG WHERE Code = A.VatGroup) AS decimal(21,6)) AS RateVat,
       CAST(N'''' AS char(1)) AS BOMTreeType,
       CAST(b.SalUnitMsr AS nvarchar(100)) AS SalUnitMsr, CAST(ISNULL(b.NumInSale, 0) AS decimal(21,6)) AS NumInSale,
       CAST(b.BuyUnitMsr AS nvarchar(100)) AS BuyUnitMsr, CAST(ISNULL(b.NumInBuy, 0) AS decimal(21,6)) AS NumInBuy,
       CAST(ISNULL(b.UgpEntry, -1) AS int) AS UgpCode, CAST(b.InvntryUom AS nvarchar(100)) AS InvUnitMsr,
       CAST(ISNULL(d.AltQty, 1) / ISNULL(d.BaseQty, 1) AS decimal(21,6)) AS NumInInven,
       CAST(CASE WHEN b.ManBtchNum = ''Y'' THEN ''B'' WHEN b.ManSerNum = ''Y'' THEN ''S'' ELSE ''N'' END AS char(1)) AS ManBatchSerial,
       CAST(0 AS int) AS LineSeq,
       CAST(ISNULL(b.OnHand, 0) AS decimal(21,6)) AS QuantityOnStock, CAST(ISNULL(t1.OnHand, 0) AS decimal(21,6)) AS QuantityOnWareHouse,
       CAST(ISNULL(b.IsCommited, 0) AS decimal(21,6)) AS IsCommited, CAST(ISNULL(t1.IsCommited, 0) AS decimal(21,6)) AS IsCommitedInWareHouse,
       CAST(ISNULL(b.OnOrder, 0) AS decimal(21,6)) AS OnOrder, CAST(ISNULL(t1.OnOrder, 0) AS decimal(21,6)) AS OnOrderInWareHouse,
       CAST(b.PurPackUn AS decimal(21,6)) AS PurPackUn, CAST(b.SalPackUn AS decimal(21,6)) AS SalPackUn,
       CAST(t1.AvgPrice AS decimal(21,6)) AS AvgPrice,
       CAST((SELECT ISNULL(Number, 0) FROM OOAT WHERE AbsID = A.AgrNo) AS int) AS AgrNumber'
        + @fgSelect + N'
INTO #tbl
FROM ' + QUOTENAME(@tblLine) + N' A
INNER JOIN OITM b ON A.ItemCode = b.ItemCode
LEFT JOIN OITW t1 ON A.ItemCode = t1.ItemCode AND A.WhsCode = t1.WhsCode
LEFT JOIN UGP1 d ON b.UgpEntry = d.UgpEntry AND d.UomEntry = b.IUoMEntry' + @fgJoin + N'
WHERE A.DocEntry = @DocEntry;';

    IF @hasText = 1
        SET @sql += N'
INSERT INTO #tbl (BOMTreeType, DocEntry, LineNum, ItemCode, LineText, VisOrder, Dscription, SalUnitMsr, NumInSale, BuyUnitMsr, NumInBuy,
                  UgpCode, InvUnitMsr, NumInInven, ManBatchSerial, LineType, LineSeq, QuantityOnStock, QuantityOnWareHouse, IsCommited,
                  IsCommitedInWareHouse, OnOrder, OnOrderInWareHouse, ItemType, AgrNumber)
SELECT N'''', DocEntry, AftLineNum, N'''', LineText, AftLineNum, CASE WHEN LineType = ''S'' THEN N''SubTotal'' ELSE N'''' END,
       N'''', 0, N'''', 0, -1, N'''', 0, ''N'', LineType, LineSeq, 0, 0, 0, 0, 0, 0, -18, 0
FROM ' + QUOTENAME(@tblText) + N' WHERE DocEntry = @DocEntry;';

    SET @sql += N'
UPDATE #tbl SET Rate = 1 WHERE Rate = 0;
SELECT *, ManBatchSerial AS ManBtchSerial, IsCommited AS IsCommittedItem, OnOrder AS OnOrderedItem, QuantityOnStock AS OnHandItem,
       QuantityOnWareHouse AS OnHandWhs, IsCommitedInWareHouse AS IsCommittedWhs, OnOrderInWareHouse AS OnOrderedWhs
FROM #tbl ORDER BY VisOrder, LineSeq;';

    EXEC sys.sp_executesql @sql, N'@DocEntry int', @DocEntry;
END;
