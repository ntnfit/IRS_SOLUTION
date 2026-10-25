USE [SBODemoAU];
GO

CREATE OR ALTER PROCEDURE dbo.APZ_PosMaster_GetAllDoc
AS
BEGIN
    SET NOCOUNT ON;
    SELECT a.* FROM dbo.APZ_RPOS a
    JOIN dbo.APZ_STORE b ON a.Store=b.Code AND b.Locked='N';
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_Retail_CLOCK_GetDoc @StoreCode nvarchar(50), @PosID int
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @LineId int=(SELECT ISNULL(MAX(LineId),0) FROM dbo.APZ_CLOCK WHERE StoreCode=@StoreCode AND PosID=@PosID AND ClockType='I');
    IF @LineId=0 SELECT 'N' AS ClockIn,'N' AS ClockOut;
    ELSE SELECT 'Y' AS ClockIn,CASE WHEN T1.LineId IS NOT NULL THEN 'Y' ELSE 'N' END AS ClockOut
      FROM dbo.APZ_CLOCK T0 LEFT JOIN dbo.APZ_CLOCK T1
        ON T0.StoreCode=T1.StoreCode AND T0.PosID=T1.PosID AND T0.LineId<T1.LineId
      WHERE T0.StoreCode=@StoreCode AND T0.PosID=@PosID AND T0.LineId=@LineId;
END;
GO

CREATE OR ALTER PROCEDURE dbo.apz_Retail_GetTaxCode
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM dbo.OVTG WHERE Category='O' AND Inactive='N';
END;
GO

CREATE OR ALTER PROCEDURE dbo.apz_retail_SaveClock
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE t SET t.UserSign=s.UserSign,t.ClockDate=s.ClockDate,t.ClockTime=s.ClockTime,t.ClockType=s.ClockType,t.CashID=s.CashID
    FROM dbo.APZ_CLOCK t JOIN #APZ_CLOCK s
      ON t.StoreCode COLLATE DATABASE_DEFAULT=s.StoreCode COLLATE DATABASE_DEFAULT AND t.PosID=s.PosID AND t.LineId=s.LineId
    WHERE ISNULL(s.LineId,0)<>0;

    INSERT dbo.APZ_CLOCK(LineId,StoreCode,PosID,UserSign,ClockDate,ClockTime,ClockType,CashID)
    SELECT CASE WHEN ISNULL(s.LineId,0)=0 THEN ISNULL((SELECT MAX(t.LineId) FROM dbo.APZ_CLOCK t),0)+ROW_NUMBER() OVER(ORDER BY(SELECT NULL)) ELSE s.LineId END,
      s.StoreCode,s.PosID,s.UserSign,s.ClockDate,s.ClockTime,s.ClockType,s.CashID
    FROM #APZ_CLOCK s
    WHERE ISNULL(s.LineId,0)=0 OR NOT EXISTS(SELECT 1 FROM dbo.APZ_CLOCK t WHERE t.StoreCode COLLATE DATABASE_DEFAULT=s.StoreCode COLLATE DATABASE_DEFAULT AND t.PosID=s.PosID AND t.LineId=s.LineId);
END;
GO

CREATE OR ALTER PROCEDURE dbo.apz_Retail_GetRefundLine @DocNum int
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @DocEntry int=(SELECT ISNULL((SELECT TOP(1) DocEntry FROM dbo.ORDR WHERE DocNum=@DocNum),0));
    SELECT * INTO #refundLines FROM dbo.RDR1 WHERE DocEntry=@DocEntry AND (Quantity>0 OR (Quantity=0 AND TrgetEntry IS NOT NULL));
    UPDATE t SET Quantity=t.Quantity+ISNULL((SELECT SUM(b.Quantity) FROM dbo.RDR1 b WHERE b.U_OriginType='17' AND b.U_OriginCode=@DocEntry AND b.U_OriginLine=t.LineNum),0)
    FROM #refundLines t;
    SELECT * FROM dbo.ORDR WHERE DocEntry=@DocEntry;
    SELECT * FROM #refundLines WHERE Quantity>0;
END;
GO

CREATE OR ALTER PROCEDURE dbo.apz_retail_cfl_customer
    @UserName nvarchar(50)=N'',@StoreCode nvarchar(50)=N'',@CardCode nvarchar(50)=N''
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @fromDate date=DATEADD(day,-1,CONVERT(date,GETDATE())),@toDate date=DATEADD(day,1,CONVERT(date,GETDATE()));
    DECLARE @structure TABLE(ColumnName nvarchar(50),Caption nvarchar(100),DataType varchar(20),Width int,GroupIndex int,Visible tinyint,FormatType nvarchar(20),DisplayFormat nvarchar(20),[ReadOnly] tinyint,AllowEdit tinyint,AllowMove tinyint,AllowSize tinyint,AllowSort tinyint,Fixed varchar(1),SummaryType varchar(10),GroupSummaryType varchar(10));
    INSERT @structure VALUES
      (N'CardCode',N'Customer Code','nvarchar',250,-1,1,NULL,NULL,1,0,1,1,1,'L',NULL,NULL),(N'CardName',N'Customer Name','nvarchar',300,-1,1,NULL,NULL,1,0,1,1,1,'L',NULL,NULL),
      (N'Balance',N'Balance','money',250,-1,1,N'numeric',N'{0:n2}',1,0,1,1,1,NULL,NULL,NULL),(N'CntctPrsn',N'Contact Person','nvarchar',250,-1,1,NULL,NULL,1,0,1,1,1,NULL,NULL,NULL),
      (N'SlpCode',N'Sales Employee','nvarchar',250,-1,0,NULL,NULL,1,0,1,1,1,NULL,NULL,NULL),(N'SalesPersonCode',N'SalesPersonCode','nvarchar',250,-1,1,NULL,NULL,1,0,1,1,1,NULL,NULL,NULL),
      (N'BillToDef',N'Bill-to Default','nvarchar',250,-1,1,NULL,NULL,1,0,1,1,1,NULL,NULL,NULL),(N'Currency',N'Currency','nvarchar',250,-1,1,NULL,NULL,1,0,1,1,1,NULL,NULL,NULL),
      (N'ContactPersonCode',N'ContactPersonCode','int',250,-1,1,NULL,NULL,1,0,1,1,1,NULL,NULL,NULL),(N'LicTradNum',N'Federal Tax ID','nvarchar',250,-1,1,NULL,NULL,1,0,1,1,1,NULL,NULL,NULL),
      (N'CntPrsName',N'Contact Person','nvarchar',250,-1,1,NULL,NULL,1,0,1,1,1,NULL,NULL,NULL),(N'Cellular',N'Mobile Phone','nvarchar',250,-1,1,NULL,NULL,1,0,1,1,1,NULL,NULL,NULL),
      (N'AddID',N'ID Number','nvarchar',250,-1,1,NULL,NULL,1,0,1,1,1,NULL,NULL,NULL);
    SELECT * FROM @structure;
    SELECT A.CardCode,A.CardName,A.LicTradNum,A.Balance,A.CntctPrsn,B.CntctCode AS ContactPersonCode,A.SlpCode,D.Street,ISNULL(A.Currency,'##') AS Currency,
      B.Name AS CntPrsName,A.Address,A.Cellular,A.MailAddres,A.AddID
    FROM dbo.OCRD A LEFT JOIN dbo.OCPR B ON A.CardCode=B.CardCode AND A.CntctPrsn=B.Name
      LEFT JOIN dbo.CRD1 D ON A.CardCode=D.CardCode AND A.ShipToDef=D.Address AND D.AddrType='S'
    WHERE ((A.validFor='Y' AND ISNULL(A.frozenFor,'N')='N' AND ISNULL(A.validFrom,@fromDate)<=CONVERT(date,GETDATE()) AND ISNULL(A.validTo,@toDate)>=CONVERT(date,GETDATE()))
       OR (ISNULL(A.validFor,'N')='N' AND ISNULL(A.frozenFor,'N')='N'))
      AND A.CardType='C' AND (ISNULL(@CardCode,N'')=N'' OR A.CardCode LIKE N'%'+@CardCode+N'%');
END;
GO

CREATE OR ALTER PROCEDURE dbo.apz_retail_cfl_item
    @UserName nvarchar(50)=N'',@StoreCode nvarchar(50)=N'',@ItemCode nvarchar(50)=N'',@CardCode nvarchar(50)=N'',@Codebars nvarchar(254)=N''
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @WhsDefault varchar(20)=ISNULL((SELECT TOP(1) Warehouse FROM dbo.OUDG WHERE Code=(SELECT TOP(1) DfltsGroup FROM dbo.OUSR WHERE USER_CODE=@UserName)),''),
            @fromDate date=DATEADD(day,-1,CONVERT(date,GETDATE())),@toDate date=DATEADD(day,1,CONVERT(date,GETDATE()));
    IF ISNULL(@Codebars,N'')<>N'' SELECT TOP(1) @ItemCode=ISNULL(ItemCode,N'') FROM dbo.OBCD WHERE BcdCode=@Codebars;
    DECLARE @structure TABLE(ColumnName nvarchar(50),Caption nvarchar(100),DataType varchar(20),Width int,GroupIndex int,Visible tinyint,FormatType nvarchar(20),DisplayFormat nvarchar(20),[ReadOnly] tinyint,AllowEdit tinyint,AllowMove tinyint,AllowSize tinyint,AllowSort tinyint,Fixed varchar(1),SummaryType varchar(10),GroupSummaryType varchar(10));
    INSERT @structure VALUES
      (N'ItemCode',N'Item Code','nvarchar',250,-1,1,NULL,NULL,1,0,1,1,1,'L',NULL,NULL),(N'ItemDescription',N'Item Name','nvarchar',300,-1,1,NULL,NULL,1,0,1,1,1,'L',NULL,NULL),
      (N'CodeBars',N'Bar Code','nvarchar',75,-1,1,NULL,NULL,1,0,1,1,1,NULL,NULL,NULL),(N'FrgnName',N'FrgnName','nvarchar',75,-1,1,NULL,NULL,1,0,1,1,1,NULL,NULL,NULL),
      (N'ItmsGrpCod',N'ItmsGrpCod','int',75,-1,0,NULL,NULL,1,0,1,1,1,NULL,NULL,NULL),(N'SalesVATGroup',N'SalesVATGroup','nvarchar',75,-1,0,NULL,NULL,1,0,1,1,1,NULL,NULL,NULL),
      (N'QuantityOnStock',N'QuantityOnStock','numeric',75,-1,1,N'numeric',N'{0:n2}',1,0,1,1,1,NULL,NULL,NULL),(N'Currency',N'Currency','nvarchar',75,-1,0,NULL,NULL,1,0,1,1,1,NULL,NULL,NULL),
      (N'Price',N'Price','numeric',75,-1,0,N'numeric',N'{0:n2}',1,0,1,1,1,NULL,NULL,NULL),(N'WarehouseCode',N'WarehouseCode','nvarchar',75,-1,0,NULL,NULL,1,0,1,1,1,NULL,NULL,NULL);
    SELECT * FROM @structure;
    SELECT A.ItemCode,A.ItemName AS ItemDescription,A.CodeBars,A.ItemName,ISNULL(A.FrgnName,'') AS FrgnName,ISNULL(A.ItmsGrpCod,0) AS ItmsGrpCod,
      CASE WHEN ISNULL(C.ECVatGroup,'')='' THEN A.VatGourpSa ELSE C.ECVatGroup END AS SalesVATGroup,
      CASE WHEN @WhsDefault<>'' THEN @WhsDefault ELSE A.DfltWH END AS WarehouseCode,
      (SELECT SUM(ISNULL(OnHand,0)-ISNULL(IsCommited,0)+ISNULL(OnOrder,0)) FROM dbo.OITW WHERE ItemCode=A.ItemCode AND Locked='N') AS QuantityOnStock,
      C.ListNum AS PriceList,P.Price,P.Currency,0 AS Discount
    FROM dbo.OITM A LEFT JOIN dbo.OCRD C ON C.CardCode=@CardCode
      LEFT JOIN dbo.ITM1 P ON A.ItemCode=P.ItemCode
      LEFT JOIN dbo.APZ_STORE S ON S.ListNum=P.PriceList
    WHERE ((A.validFor='Y' AND ISNULL(A.frozenFor,'N')='N' AND ISNULL(A.validFrom,@fromDate)<=CONVERT(date,GETDATE()) AND ISNULL(A.validTo,@toDate)>=CONVERT(date,GETDATE()))
       OR (ISNULL(A.validFor,'N')='N' AND ISNULL(A.frozenFor,'N')='N'))
      AND ((ISNULL(@Codebars,N'')=N'' AND (@ItemCode=N'' OR A.ItemCode=@ItemCode)) OR (ISNULL(@Codebars,N'')<>N'' AND A.ItemCode=@ItemCode))
      AND A.InvntItem='Y' AND (ISNULL(@StoreCode,N'')=N'' OR S.Code=@StoreCode);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_Retail_GetDocEntry @StoreCode nvarchar(50),@POSID int
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @startDate date,@endDate date,@startTime int,@endTime int;
    SELECT TOP(1) @startDate=CONVERT(date,ClockDate),@startTime=ClockTime FROM dbo.APZ_CLOCK
      WHERE StoreCode=@StoreCode AND PosID=@POSID AND ClockType='I' ORDER BY LineId DESC;
    SELECT TOP(1) @endDate=CONVERT(date,ClockDate),@endTime=ClockTime FROM dbo.APZ_CLOCK
      WHERE StoreCode=@StoreCode AND PosID=@POSID AND ClockType='O' ORDER BY LineId DESC;
    ;WITH docs AS (
      SELECT * FROM dbo.ORDR WHERE U_RetailStore=@StoreCode AND U_Posid=@POSID AND ISNULL(U_RetailPaidEnt,0)>0 AND ISNULL(CANCELED,'N')<>'Y'
        AND ((@startDate=@endDate AND CONVERT(date,DocDate)=@startDate AND DocTime BETWEEN @startTime AND @endTime)
          OR (@startDate<@endDate AND (CONVERT(date,DocDate)>@startDate AND CONVERT(date,DocDate)<@endDate OR CONVERT(date,DocDate)=@startDate AND DocTime>=@startTime OR CONVERT(date,DocDate)=@endDate AND DocTime<=@endTime)))
    )
    SELECT STRING_AGG(CONVERT(nvarchar(max),DocEntry),N',') AS ListDocEntry,CardCode AS BpCode
    FROM docs GROUP BY CardCode,CASE WHEN ISNULL(VatSum,0)>0 THEN 1 ELSE 0 END;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_PosMaster_GetDoc @StoreCode nvarchar(50)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM dbo.APZ_RPOS WHERE Store=@StoreCode;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_CustomerSearch_GetDoc
    @cardType char(1)='C',@userName nvarchar(50)=N'',@branch nvarchar(50)=N''
AS
BEGIN
    SET NOCOUNT ON;
    SELECT CardCode,CardName,CardType,GroupCode,Currency,Balance,CntctPrsn,Phone1,Cellular,E_Mail,Address,LicTradNum,SlpCode,ListNum
    FROM dbo.OCRD
    WHERE (@cardType='' OR CardType=@cardType) AND ISNULL(validFor,'N')='Y' AND ISNULL(frozenFor,'N')<>'Y'
    ORDER BY CardCode;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_ItemSearch_GetDoc
    @itemCode nvarchar(50)=N'',@itemType nvarchar(10)=N'',@userName nvarchar(50)=N'',@branch nvarchar(50)=N'',@pr nvarchar(100)=N''
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ItemCode,ItemName,FrgnName,CodeBars,ItmsGrpCod,DfltWH,InvntryUom,SalUnitMsr,NumInSale,VatGourpSa,OnHand,IsCommited,OnOrder,AvgPrice
    FROM dbo.OITM
    WHERE (@itemCode=N'' OR ItemCode LIKE N'%'+@itemCode+N'%' OR ItemName LIKE N'%'+@itemCode+N'%' OR CodeBars=@itemCode)
      AND (@itemType=N'' OR (@itemType=N'I' AND InvntItem='Y') OR (@itemType=N'S' AND SellItem='Y'))
      AND (@pr=N'' OR ItemCode LIKE N'%'+@pr+N'%' OR ItemName LIKE N'%'+@pr+N'%')
      AND ISNULL(validFor,'N')='Y' AND ISNULL(frozenFor,'N')<>'Y'
    ORDER BY ItemCode;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_Retail_GetByCode
    @UserName nvarchar(50)=N'',@Branch nvarchar(50)=N'',@ItemCode nvarchar(254)=N''
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @resolved nvarchar(50)=@ItemCode;
    SELECT TOP(1) @resolved=ItemCode FROM dbo.OBCD WHERE BcdCode=@ItemCode;
    SELECT i.ItemCode,i.ItemName AS Dscription,i.CodeBars,i.DfltWH AS WhsCode,i.VatGourpSa AS VatGroup,
      i.SalUnitMsr,i.NumInSale,i.InvntryUom,i.OnHand,i.IsCommited,i.OnOrder,
      p.Price,p.Currency,p.PriceList,CAST(1 AS decimal(19,6)) AS Quantity,CAST(0 AS decimal(19,6)) AS DiscPrcnt
    FROM dbo.OITM i LEFT JOIN dbo.ITM1 p ON i.ItemCode=p.ItemCode AND p.PriceList=1
    WHERE i.ItemCode=@resolved AND i.SellItem='Y' AND ISNULL(i.validFor,'N')='Y' AND ISNULL(i.frozenFor,'N')<>'Y';
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_Retail_SearchTransactions
    @DocNum int=0,@CardCode nvarchar(50)=N'',@FromDate datetime=NULL,@ToDate datetime=NULL,@StoreCode nvarchar(50)=N'',@ItemCode nvarchar(50)=N''
AS
BEGIN
    SET NOCOUNT ON;
    SELECT DISTINCT h.DocEntry,h.DocNum,h.DocDate,h.DocDueDate,h.CardCode,h.CardName,h.DocStatus,h.CANCELED,h.DocTotal,h.VatSum,h.DiscPrcnt,h.DocCur,
      h.U_RetailStore,h.U_Posid,h.U_RetailPaidEnt
    FROM dbo.ORDR h LEFT JOIN dbo.RDR1 l ON h.DocEntry=l.DocEntry
    WHERE (@DocNum<=0 OR h.DocNum=@DocNum) AND (@CardCode=N'' OR h.CardCode=@CardCode)
      AND (@FromDate IS NULL OR h.DocDate>=CONVERT(date,@FromDate)) AND (@ToDate IS NULL OR h.DocDate<=CONVERT(date,@ToDate))
      AND (@StoreCode=N'' OR h.U_RetailStore=@StoreCode) AND (@ItemCode=N'' OR l.ItemCode=@ItemCode)
    ORDER BY h.DocEntry DESC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_Retail_GetTransDetail @DocEntry int
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM dbo.ORDR WHERE DocEntry=@DocEntry;
    SELECT * FROM dbo.RDR1 WHERE DocEntry=@DocEntry ORDER BY LineNum;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_RetailInventory_Search @ItemCode nvarchar(50)=N''
AS
BEGIN
    SET NOCOUNT ON;
    SELECT w.ItemCode,i.ItemName,w.WhsCode,s.WhsName,w.OnHand,w.IsCommited,w.OnOrder,(w.OnHand-w.IsCommited+w.OnOrder) AS Available
    FROM dbo.OITW w JOIN dbo.OITM i ON w.ItemCode=i.ItemCode LEFT JOIN dbo.OWHS s ON w.WhsCode=s.WhsCode
    WHERE (@ItemCode=N'' OR w.ItemCode LIKE N'%'+@ItemCode+N'%' OR i.ItemName LIKE N'%'+@ItemCode+N'%')
    ORDER BY w.ItemCode,w.WhsCode;
END;
GO
