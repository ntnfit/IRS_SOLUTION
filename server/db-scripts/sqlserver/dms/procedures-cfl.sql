USE [SBODemoAU];
GO

CREATE OR ALTER PROCEDURE dbo.APZ_CFL_Users
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @structure TABLE
    (
        ColumnName nvarchar(50), Caption nvarchar(100), DataType varchar(20), Width int,
        GroupIndex int, Visible tinyint, FormatType nvarchar(20), DisplayFormat nvarchar(20),
        ReadOnly tinyint, AllowEdit tinyint, AllowMove tinyint, AllowSize tinyint,
        AllowSort tinyint, Fixed varchar(1), SummaryType varchar(10), GroupSummaryType varchar(10)
    );
    INSERT INTO @structure VALUES
        (N'Code', N'Code', 'nvarchar', 75, -1, 1, NULL, NULL, 1, 0, 1, 1, 1, 'L', NULL, NULL),
        (N'Name', N'Name', 'nvarchar', 150, -1, 1, NULL, NULL, 1, 0, 1, 1, 1, 'L', NULL, NULL);

    SELECT * FROM @structure;
    SELECT USER_CODE AS Code, U_NAME AS Name FROM dbo.OUSR;
END;
GO

CREATE OR ALTER PROCEDURE dbo.Apz_Cfl_LoadBusinessPartner
    @cardType nvarchar(10) = N'',
    @userName nvarchar(20) = N'',
    @branch nvarchar(20) = N'',
    @bpType nvarchar(50) = N''
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @structure TABLE
    (
        ColumnName nvarchar(50), Caption nvarchar(100), DataType varchar(20), Width int,
        GroupIndex int, Visible tinyint, FormatType nvarchar(20), DisplayFormat nvarchar(20),
        ReadOnly tinyint, AllowEdit tinyint, AllowMove tinyint, AllowSize tinyint,
        AllowSort tinyint, Fixed varchar(1), SummaryType varchar(10), GroupSummaryType varchar(10)
    );
    INSERT INTO @structure VALUES
        (N'CardCode',N'Code','nvarchar',75,-1,1,NULL,NULL,1,0,1,1,1,'L',NULL,NULL),
        (N'CardName',N'Name','nvarchar',200,-1,1,NULL,NULL,1,0,1,1,1,'L',NULL,NULL),
        (N'CardType',N'Type','varchar',75,-1,0,NULL,NULL,1,0,1,1,1,'L',NULL,NULL),
        (N'Balance',N'Balance','money',75,-1,1,N'numeric',N'{0:n2}',1,0,1,1,1,NULL,NULL,NULL),
        (N'CntctPrsn',N'Contact Person','nvarchar',125,-1,1,NULL,NULL,1,0,1,1,1,NULL,NULL,NULL),
        (N'BillTo',N'Bill To','nvarchar',175,-1,1,NULL,NULL,1,0,1,1,1,NULL,NULL,NULL),
        (N'ShipTo',N'Ship To','nvarchar',175,-1,1,NULL,NULL,1,0,1,1,1,NULL,NULL,NULL),
        (N'SlpCode',N'SlpCode','nvarchar',75,-1,0,NULL,NULL,1,0,1,1,1,NULL,NULL,NULL),
        (N'SalesPersonCode',N'SalesPersonCode','nvarchar',75,-1,0,NULL,NULL,1,0,1,1,1,NULL,NULL,NULL),
        (N'BillToDef',N'BillToDef','nvarchar',75,-1,0,NULL,NULL,1,0,1,1,1,NULL,NULL,NULL),
        (N'Currency',N'Currency','nvarchar',75,-1,0,NULL,NULL,1,0,1,1,1,NULL,NULL,NULL),
        (N'ContactPersonCode',N'ContactPersonCode','int',75,-1,0,NULL,NULL,1,0,1,1,1,NULL,NULL,NULL),
        (N'LicTradNum',N'Lic. Trad. Num.','nvarchar',75,-1,1,NULL,NULL,1,0,1,1,1,NULL,NULL,NULL),
        (N'CntPrsName',N'Contact Person Name','nvarchar',75,-1,0,NULL,NULL,1,0,1,1,1,NULL,NULL,NULL),
        (N'TotalInvc',N'Total Invoiced','money',75,-1,0,NULL,NULL,1,0,1,1,1,NULL,NULL,NULL);
    SELECT * FROM @structure;

    DECLARE @fromDate date = DATEADD(day, -1, CONVERT(date, GETDATE()));
    DECLARE @toDate date = DATEADD(day, 1, CONVERT(date, GETDATE()));
    DECLARE @DspIBPDoc char(1) = ISNULL((SELECT TOP (1) DspIBPDoc FROM dbo.ADM1), 'N');
    DECLARE @doFilter char(1) = ISNULL((SELECT TOP (1) DoFilter FROM dbo.CINF), 'N');
    DECLARE @DoMngMth char(1) = ISNULL((SELECT TOP (1) DoMngMth FROM dbo.OADM), '');

    SELECT partner.CardCode, partner.CardName, partner.CardType, partner.LicTradNum, partner.Balance,
           partner.CntctPrsn, contact.CntctCode AS ContactPersonCode, contact.CntctCode,
           partner.SlpCode, shipping.Street AS MailAddress, ISNULL(partner.Currency, '##') AS Currency,
           contact.Name AS CntPrsName,
           CASE partner.CardType WHEN 'S' THEN 0 ELSE ISNULL((SELECT SUM(invoice.DocTotal) FROM dbo.OINV invoice WHERE invoice.CardCode = partner.CardCode), 0) END AS TotalInvc,
           ISNULL((SELECT TOP (1) address.Street FROM dbo.CRD1 address WHERE address.CardCode = partner.CardCode AND address.AdresType = 'B' AND address.Address = partner.BillToDef), N'') AS BillTo,
           ISNULL((SELECT TOP (1) address.Street FROM dbo.CRD1 address WHERE address.CardCode = partner.CardCode AND address.AdresType = 'S' AND address.Address = partner.ShipToDef), N'') AS ShipTo
    FROM dbo.OCRD partner
    LEFT JOIN dbo.OCPR contact ON contact.CardCode = partner.CardCode AND contact.Name = partner.CntctPrsn
    LEFT JOIN dbo.CRD1 shipping ON shipping.CardCode = partner.CardCode AND shipping.Address = partner.ShipToDef AND shipping.AdresType = 'S'
    WHERE
      (@DspIBPDoc = 'Y' OR
       (@DspIBPDoc = 'N' AND
        ((partner.validFor = 'Y' AND ISNULL(partner.frozenFor, 'N') = 'N' AND ISNULL(partner.validFrom, @fromDate) <= CONVERT(date, GETDATE()) AND ISNULL(partner.validTo, @toDate) >= CONVERT(date, GETDATE()))
         OR (ISNULL(partner.validFor, 'N') = 'N' AND partner.frozenFor = 'Y' AND (ISNULL(partner.frozenFrom, @fromDate) > CONVERT(date, GETDATE()) OR ISNULL(partner.frozenTo, @toDate) < CONVERT(date, GETDATE())))
         OR (partner.validFor = 'Y' AND partner.frozenFor = 'Y' AND ISNULL(partner.validFrom, @fromDate) <= CONVERT(date, GETDATE()) AND ISNULL(partner.validTo, @toDate) >= CONVERT(date, GETDATE()) AND (ISNULL(partner.frozenFrom, @fromDate) > CONVERT(date, GETDATE()) OR ISNULL(partner.frozenTo, @toDate) < CONVERT(date, GETDATE())))
         OR (ISNULL(partner.validFor, 'N') = 'N' AND ISNULL(partner.frozenFor, 'N') = 'N'))))
      AND (ISNULL(@cardType, N'') = N'' OR partner.CardType IN (SELECT DATA FROM dbo.Split(@cardType, N',')))
      AND (@doFilter = 'N' OR (@doFilter = 'Y' AND @DoMngMth NOT IN ('A','B'))
           OR (@doFilter = 'Y' AND @DoMngMth IN ('A','B') AND (ISNULL(partner.OwnerCode, -1) = -1 OR partner.OwnerCode IN (SELECT OwnerId FROM dbo.apz_func_OwnerOverShip(@userName, N'2')))))
      AND (ISNULL(@bpType, N'') = N'' OR partner.CmpPrivate = @bpType)
    ORDER BY partner.CardCode;
END;
GO
