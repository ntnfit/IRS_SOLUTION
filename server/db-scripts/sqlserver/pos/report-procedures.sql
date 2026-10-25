USE [SBODemoAU];
GO

CREATE OR ALTER PROCEDURE dbo.USP_POS_INVOICE
    @DocID int,
    @type int
AS
BEGIN
    SET NOCOUNT ON;

    IF @type=2
    BEGIN
      SELECT CONVERT(nvarchar(254),adm.CompnyName) COMPNAME,CONVERT(nvarchar(254),adm.CompnyAddr) COMPADD,
        h.DocEntry DOCENTRY,ISNULL(l.BaseEntry,-1) BASEENTRY,ISNULL(l.BaseLine,-1) BASELINE,ISNULL(l.BaseType,-1) BASETYPE,
        CONVERT(nvarchar(50),h.DocNum) INVOICENO,CONVERT(datetime,h.DocDate) DOCDATE,
        CONVERT(nvarchar(50),h.CardCode) BPCODE,CONVERT(nvarchar(254),h.CardName) BPNAME,
        CONVERT(nvarchar(500),h.Address) BPADDR,CONVERT(nvarchar(500),h.Address2) SHIPTO,
        CONVERT(nvarchar(100),N'') VESSEL,CONVERT(datetime,h.DocDueDate) ETD,
        CONVERT(nvarchar(100),N'') FROMPORT,CONVERT(nvarchar(100),N'') TOPORT,
        CONVERT(nvarchar(100),ISNULL(h.NumAtCard,N'')) SONO,CONVERT(nvarchar(50),l.ItemCode) ITEMCODE,
        CONVERT(nvarchar(254),l.Dscription) ITEMNAME,CONVERT(decimal(19,6),l.Quantity) QTY,
        CONVERT(nvarchar(100),l.unitMsr) UOM,CONVERT(decimal(19,6),l.Price) UNITPRICE,
        CONVERT(decimal(19,6),l.LineTotal+l.VatSum) AMOUNT,CONVERT(nvarchar(100),N'') ORIGIN,
        CONVERT(nvarchar(100),N'') INCOTERM,h.GroupNum GROUPNUM,CONVERT(nvarchar(254),g.PymntGroup) PAYMENTTERM,
        CONVERT(nvarchar(10),h.DocCur) CURR
      FROM dbo.OINV h JOIN dbo.INV1 l ON l.DocEntry=h.DocEntry
      CROSS JOIN dbo.OADM adm LEFT JOIN dbo.OCTG g ON g.GroupNum=h.GroupNum
      WHERE h.DocEntry=@DocID ORDER BY l.LineNum;
      RETURN;
    END;

    SELECT CONVERT(nvarchar(254),adm.CompnyName) COMPNAME,CONVERT(nvarchar(254),adm.CompnyAddr) COMPADD,
      h.DocEntry DOCENTRY,ISNULL(l.BaseEntry,-1) BASEENTRY,ISNULL(l.BaseLine,-1) BASELINE,ISNULL(l.BaseType,-1) BASETYPE,
      CONVERT(nvarchar(50),h.DocNum) INVOICENO,CONVERT(datetime,h.DocDate) DOCDATE,
      CONVERT(nvarchar(50),h.CardCode) BPCODE,CONVERT(nvarchar(254),h.CardName) BPNAME,
      CONVERT(nvarchar(500),h.Address) BPADDR,CONVERT(nvarchar(500),h.Address2) SHIPTO,
      CONVERT(nvarchar(100),N'') VESSEL,CONVERT(datetime,h.DocDueDate) ETD,
      CONVERT(nvarchar(100),N'') FROMPORT,CONVERT(nvarchar(100),N'') TOPORT,
      CONVERT(nvarchar(100),ISNULL(h.NumAtCard,N'')) SONO,CONVERT(nvarchar(50),l.ItemCode) ITEMCODE,
      CONVERT(nvarchar(254),l.Dscription) ITEMNAME,CONVERT(decimal(19,6),l.Quantity) QTY,
      CONVERT(nvarchar(100),l.unitMsr) UOM,CONVERT(decimal(19,6),l.Price) UNITPRICE,
      CONVERT(decimal(19,6),l.LineTotal+l.VatSum) AMOUNT,CONVERT(nvarchar(100),N'') ORIGIN,
      CONVERT(nvarchar(100),N'') INCOTERM,h.GroupNum GROUPNUM,CONVERT(nvarchar(254),g.PymntGroup) PAYMENTTERM,
      CONVERT(nvarchar(10),h.DocCur) CURR
    FROM dbo.ORDR h JOIN dbo.RDR1 l ON l.DocEntry=h.DocEntry
    CROSS JOIN dbo.OADM adm LEFT JOIN dbo.OCTG g ON g.GroupNum=h.GroupNum
    WHERE h.DocEntry=@DocID ORDER BY l.LineNum;
END;
GO

CREATE OR ALTER PROCEDURE dbo.USP_SAL_SalesAndTaxSum
    @StoreCode nvarchar(100),
    @PosID int
AS
BEGIN
    SET NOCOUNT ON;
    SELECT CONVERT(nvarchar(254),adm.CompnyName) COMPNAME,CONVERT(nvarchar(254),adm.CompnyAddr) COMPADD,
      MIN(h.DocEntry) DOCENTRY,-1 BASEENTRY,-1 BASELINE,-1 BASETYPE,
      CONCAT(N'POS ',@PosID,N' / ',@StoreCode) INVOICENO,CONVERT(datetime,CONVERT(date,GETDATE())) DOCDATE,
      CONVERT(nvarchar(50),@StoreCode) BPCODE,CONVERT(nvarchar(254),N'Sales and Taxes Summary') BPNAME,
      CONVERT(nvarchar(500),N'') BPADDR,CONVERT(nvarchar(500),N'') SHIPTO,CONVERT(nvarchar(100),N'') VESSEL,
      CONVERT(datetime,CONVERT(date,GETDATE())) ETD,CONVERT(nvarchar(100),N'') FROMPORT,CONVERT(nvarchar(100),N'') TOPORT,
      CONVERT(nvarchar(100),N'') SONO,CONVERT(nvarchar(50),ISNULL(l.TaxCode,N'NO TAX')) ITEMCODE,
      CONVERT(nvarchar(254),CONCAT(N'Tax ',ISNULL(l.TaxCode,N'NO TAX'),N' (',CONVERT(decimal(9,2),ISNULL(l.VatPrcnt,0)),N'%)')) ITEMNAME,
      CONVERT(decimal(19,6),COUNT_BIG(*)) QTY,CONVERT(nvarchar(100),N'lines') UOM,
      CONVERT(decimal(19,6),SUM(l.LineTotal)) UNITPRICE,CONVERT(decimal(19,6),SUM(l.LineTotal+l.VatSum)) AMOUNT,
      CONVERT(nvarchar(100),N'') ORIGIN,CONVERT(nvarchar(100),N'') INCOTERM,0 GROUPNUM,
      CONVERT(nvarchar(254),N'') PAYMENTTERM,CONVERT(nvarchar(10),MAX(h.DocCur)) CURR
    FROM dbo.ORDR h JOIN dbo.RDR1 l ON l.DocEntry=h.DocEntry CROSS JOIN dbo.OADM adm
    WHERE ISNULL(h.U_RetailStore,N'')=@StoreCode AND ISNULL(h.U_Posid,0)=@PosID
    GROUP BY adm.CompnyName,adm.CompnyAddr,l.TaxCode,l.VatPrcnt
    ORDER BY l.TaxCode;
END;
GO
