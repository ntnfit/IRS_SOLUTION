-- HANA: APZ_Document_Get — document headers of a marketing table (ORDR, OPOR, OINV …) with totals in document
-- currency, filtered by data ownership, branch access and the given criteria.
-- Table names are checked against sys.tables and quoted; filter values go through sp_executesql parameters
-- (HANA concatenated them into the SQL text).
CREATE OR ALTER PROCEDURE dbo.APZ_Document_Get
    @tbl nvarchar(10),
    @tblLine nvarchar(10),
    @DocEntry int = 0,
    @DocDate datetime = NULL,
    @EndDate datetime = NULL,
    @DocStatus nvarchar(1) = N'',
    @Canceled nvarchar(1) = N'',
    @CardCode nvarchar(50) = N'',
    @UserName nvarchar(50) = N'',
    @isIns nvarchar(1) = N'N',
    @Branch int = -1,
    @DocNum int = 0,
    @Requester nvarchar(10) = N'',
    @ReqType int = 0
AS
BEGIN
    SET NOCOUNT ON;
    IF OBJECT_ID(N'dbo.' + QUOTENAME(@tbl), N'U') IS NULL OR OBJECT_ID(N'dbo.' + QUOTENAME(@tblLine), N'U') IS NULL
        THROW 50001, N'APZ_Document_Get: unknown document table', 1;

    DECLARE @defaultPriceList int = ISNULL((SELECT TOP 1 ListNum FROM OPLN ORDER BY ListNum), -1);
    DECLARE @ObjType nvarchar(20) = (SELECT TOP 1 a.ObjType FROM dbo.APZ_TBLOBJECT a WHERE a.TblName = @tbl);
    DECLARE @mainCur nvarchar(3) = (SELECT TOP 1 MainCurncy FROM OADM);
    DECLARE @t nvarchar(130) = QUOTENAME(@tbl), @tl nvarchar(130) = QUOTENAME(@tblLine);

    -- empty copy of the header table plus computed columns
    DECLARE @sql nvarchar(max) = N'
SELECT TOP 0 *, CAST(NULL AS numeric(21,6)) AS Total, CAST(NULL AS numeric(21,6)) AS TotalFreight,
       CAST(NULL AS numeric(21,6)) AS TotalBeforeDis, CAST(NULL AS numeric(21,6)) AS TaxTotal,
       CAST(NULL AS numeric(21,6)) AS DPMTotal, CAST(NULL AS numeric(21,6)) AS Discount,
       CAST(NULL AS nvarchar(20)) AS DocStatusName
INTO #tempDoc FROM ' + @t + N';
INSERT INTO #tempDoc
SELECT A.*,
  CASE WHEN A.DocCur = @mainCur THEN A.DocTotal ELSE A.DocTotalFC END,
  CASE WHEN A.DocCur = @mainCur THEN A.TotalExpns ELSE A.TotalExpFC END,
  CASE WHEN A.DocCur = @mainCur
       THEN (SELECT ISNULL(SUM(B.LineTotal), 0) FROM ' + @tl + N' B WHERE B.DocEntry = A.DocEntry AND B.TaxOnly = ''N'')
       ELSE (SELECT ISNULL(SUM(B.TotalFrgn), 0) FROM ' + @tl + N' B WHERE B.DocEntry = A.DocEntry AND B.TaxOnly = ''N'') END,
  CASE WHEN A.DocCur = @mainCur THEN A.VatSum ELSE A.VatSumFC END,
  CASE WHEN A.DocCur = @mainCur THEN A.DpmAmnt ELSE A.DpmAmntFC END,
  CASE WHEN A.DocCur = @mainCur THEN A.DiscSum ELSE A.DiscSumFC END,
  CASE WHEN A.DocStatus = ''O'' THEN ''Open''
       WHEN A.DocStatus = ''C'' AND (A.CANCELED IN (''N'', ''C'', '''') OR A.CANCELED IS NULL) THEN ''Closed''
       WHEN A.DocStatus = ''C'' AND A.CANCELED = ''Y'' THEN ''CANCELED'' END
FROM ' + @t + N' A
WHERE (ISNULL(A.OwnerCode, -1) = -1
       OR A.OwnerCode IN (SELECT DISTINCT x.OwnerId FROM dbo.apz_func_OwnerOverShip(@UserName, @ObjType) x))';

    IF @DocEntry <> 0
        SET @sql += N' AND A.DocEntry = @DocEntry';
    ELSE IF @DocNum <> 0
        SET @sql += N' AND A.DocNum = @DocNum';
    ELSE
    BEGIN
        IF @DocDate IS NOT NULL SET @sql += N' AND A.DocDate >= CAST(@DocDate AS date)';
        IF @EndDate IS NOT NULL SET @sql += N' AND A.DocDate <= CAST(@EndDate AS date)';
        IF @Canceled = N'Y' AND @DocStatus <> N'A'
            SET @sql += N' AND A.CANCELED = ''Y''';
        ELSE IF @DocStatus = N'D'
            SET @sql += N' AND A.InvntSttus = ''C'' AND A.DocStatus = ''O''';
        ELSE IF @DocStatus <> N'' AND @DocStatus <> N'A'
            SET @sql += N' AND A.DocStatus = @DocStatus AND A.CANCELED IN (''N'', ''C'')';
        IF @CardCode <> N'' SET @sql += N' AND A.CardCode = @CardCode';
        IF @ReqType > 0
        BEGIN
            SET @sql += N' AND A.ReqType = @ReqType';
            IF @Requester <> N'' SET @sql += N' AND A.Requester = @Requester';
        END;
    END;

    -- isIns distinguishes down-payment request vs invoice
    IF @isIns <> N''
    BEGIN
        IF @tbl IN (N'OPCH', N'OINV') SET @sql += N' AND A.isIns = @isIns';
        ELSE IF @tbl IN (N'ODPO', N'ODPI') SET @sql += N' AND A.Posted = @isIns';
    END;

    SET @sql += N'
  AND (ISNULL(A.BPLId, -1) = -1 OR A.BPLId IN (SELECT x.BPLId FROM OBPL x INNER JOIN USR6 y ON x.BPLId = y.BPLId
                                               WHERE x.Disabled = ''N'' AND y.UserCode = ISNULL(@UserName, N'''')));
SELECT A.*, b.Currency, c.Name AS ContactPersonName,
       (SELECT lastName + N'', '' + firstName FROM OHEM WHERE empID = A.OwnerCode) AS OwnerName,
       ISNULL(b.ListNum, @defaultPriceList) AS InfoPriceList
FROM #tempDoc A
LEFT JOIN OCRD b ON A.CardCode = b.CardCode
LEFT JOIN OCPR c ON A.CardCode = c.CardCode AND A.CntctCode = c.CntctCode
ORDER BY A.DocEntry DESC;';

    EXEC sys.sp_executesql @sql,
        N'@mainCur nvarchar(3), @UserName nvarchar(50), @ObjType nvarchar(20), @DocEntry int, @DocNum int, @DocDate datetime,
          @EndDate datetime, @DocStatus nvarchar(1), @CardCode nvarchar(50), @ReqType int, @Requester nvarchar(10), @isIns nvarchar(1),
          @defaultPriceList int',
        @mainCur, @UserName, @ObjType, @DocEntry, @DocNum, @DocDate, @EndDate, @DocStatus, @CardCode, @ReqType, @Requester, @isIns,
        @defaultPriceList;
END;
