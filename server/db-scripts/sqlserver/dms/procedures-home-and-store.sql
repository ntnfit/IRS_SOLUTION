USE [SBODemoAU];
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_Home_Summary
    @userCode nvarchar(50),
    @type char(1)
AS
BEGIN
    SET NOCOUNT ON;
    SET DATEFIRST 1;
    DECLARE @today date=CONVERT(date,GETDATE()), @fromDate date, @toDate date;
    IF @type='W' BEGIN SET @fromDate=DATEADD(day,1-DATEPART(weekday,@today),@today); SET @toDate=DATEADD(day,6,@fromDate); END
    ELSE IF @type='M' BEGIN SET @fromDate=DATEFROMPARTS(YEAR(@today),MONTH(@today),1); SET @toDate=EOMONTH(@today); END
    ELSE BEGIN SET @fromDate=DATEFROMPARTS(YEAR(@today),1,1); SET @toDate=DATEFROMPARTS(YEAR(@today),12,31); END;

    CREATE TABLE #stores(CardCode nvarchar(50) COLLATE SQL_Latin1_General_CP850_CI_AS,CardName nvarchar(254) COLLATE SQL_Latin1_General_CP850_CI_AS);
    EXEC dbo.APZ_D_FilterStores N'',N'','S',@userCode,N'#stores';
    DECLARE @orders TABLE(DocEntry int PRIMARY KEY,DocTotal decimal(19,6),U_GUID nvarchar(254),DocStatus char(1));
    INSERT @orders SELECT DocEntry,DocTotal,U_GUID,DocStatus FROM dbo.ORDR
      WHERE CANCELED='N' AND DocDate BETWEEN @fromDate AND @toDate AND CardCode IN(SELECT CardCode FROM #stores);

    DECLARE @ItemTotal decimal(19,6)=0,@ItemPerOrder decimal(19,6)=0,@RevenuePerOrder decimal(19,6)=0,@TotalRevenue decimal(19,6)=0;
    DECLARE @NumberStore int=(SELECT COUNT(*) FROM #stores),@NumberStoreNew int,@NumberVisit int,@NumberVisitHaveOrder int,@NumberOrder int,@NumberOrderOpen int;
    SELECT @NumberStoreNew=COUNT(*) FROM dbo.OCRD WHERE CardCode IN(SELECT CardCode FROM #stores) AND CreateDate BETWEEN @fromDate AND @toDate;
    SELECT @NumberVisit=COUNT(*) FROM dbo.APZ_D_VISIT WHERE StoreCode IN(SELECT CardCode FROM #stores) AND FinishedDate BETWEEN @fromDate AND DATEADD(day,1,@toDate);
    SELECT @NumberVisitHaveOrder=COUNT(*) FROM @orders WHERE U_GUID IN(SELECT Guid FROM dbo.APZ_D_VISIT WHERE StoreCode IN(SELECT CardCode FROM #stores) AND FinishedDate BETWEEN @fromDate AND DATEADD(day,1,@toDate));
    SELECT @NumberOrder=COUNT(*),@NumberOrderOpen=SUM(CASE WHEN DocStatus='O' THEN 1 ELSE 0 END),@TotalRevenue=ISNULL(SUM(DocTotal),0),@RevenuePerOrder=ISNULL(AVG(DocTotal),0) FROM @orders;
    SELECT @ItemTotal=COUNT(DISTINCT line.ItemCode) FROM dbo.RDR1 line JOIN @orders header ON header.DocEntry=line.DocEntry;
    SELECT @ItemPerOrder=ISNULL(AVG(CONVERT(decimal(19,6),items.Cnt)),0) FROM (SELECT line.DocEntry,COUNT(DISTINCT line.ItemCode) Cnt FROM dbo.RDR1 line JOIN @orders header ON header.DocEntry=line.DocEntry GROUP BY line.DocEntry) items;
    SELECT FORMAT(ISNULL(@ItemTotal,0),'N2') ItemTotal,FORMAT(ISNULL(@ItemPerOrder,0),'N2') ItemPerOrder,
      FORMAT(ISNULL(@RevenuePerOrder,0),'N0') RevenuePerOrder,FORMAT(ISNULL(@TotalRevenue,0),'N0') TotalRevenue,
      FORMAT(ISNULL(@NumberStore,0),'N0') NumberStore,FORMAT(ISNULL(@NumberStoreNew,0),'N0') NumberStoreNew,
      FORMAT(ISNULL(@NumberVisit,0),'N0') NumberVisit,FORMAT(ISNULL(@NumberVisitHaveOrder,0),'N0') NumberVisitHaveOrder,
      FORMAT(ISNULL(@NumberOrder,0),'N0') NumberOrder,FORMAT(ISNULL(@NumberOrderOpen,0),'N0') NumberOrderOpen,@type DateType;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_Home_GetData @userCode nvarchar(100)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @summary TABLE(ItemTotal nvarchar(30),ItemPerOrder nvarchar(30),RevenuePerOrder nvarchar(30),TotalRevenue nvarchar(30),NumberStore nvarchar(30),NumberStoreNew nvarchar(30),NumberVisit nvarchar(30),NumberVisitHaveOrder nvarchar(30),NumberOrder nvarchar(30),NumberOrderOpen nvarchar(30),DateType char(1));
    INSERT @summary EXEC dbo.APZ_D_Home_Summary @userCode,'W';
    INSERT @summary EXEC dbo.APZ_D_Home_Summary @userCode,'M';
    INSERT @summary EXEC dbo.APZ_D_Home_Summary @userCode,'Y';
    DECLARE @userId int=(SELECT TOP(1) USERID FROM dbo.OUSR WHERE USER_CODE=@userCode);
    DECLARE @linkThumb nvarchar(2000)=(SELECT TOP(1) LinkThumbnails FROM dbo.APZ_D_UUPIM WHERE UserSign=@userId AND ObjectType='1470000171' ORDER BY TimestampCreated DESC);
    DECLARE @linkDownload nvarchar(2000)=(SELECT TOP(1) LinkDownload FROM dbo.APZ_D_UUPIM WHERE UserSign=@userId AND ObjectType='1470000171' ORDER BY TimestampCreated DESC);
    SELECT users.USERID,users.U_NAME,users.Branch,employee.salesPrson,employee.empID,
      CONCAT(ISNULL(employee.firstName,N''),N' ',ISNULL(employee.middleName,N''),N' ',ISNULL(employee.lastName,N'')) U_fullName,
      @linkThumb LinkThumbnails,@linkDownload LinkDownload,summary.*
    FROM dbo.OUSR users LEFT JOIN dbo.OHEM employee ON employee.userId=users.USERID CROSS JOIN @summary summary
    WHERE users.USER_CODE=@userCode;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_GetListStore
    @cardType nvarchar(50)=N'',@cardName nvarchar(254)=N'',@subType char(1),@saleUser nvarchar(50),@type char(1)=N''
AS
BEGIN
    SET NOCOUNT ON;
    CREATE TABLE #stores(CardCode nvarchar(50) COLLATE SQL_Latin1_General_CP850_CI_AS,CardName nvarchar(254) COLLATE SQL_Latin1_General_CP850_CI_AS);
    EXEC dbo.APZ_D_FilterStores N'',N'',@subType,@saleUser,N'#stores';
    DECLARE @userId int=ISNULL((SELECT TOP(1) USERID FROM dbo.OUSR WHERE USER_CODE=@saleUser),0);
    IF @type='M'
    BEGIN
      ;WITH latest AS(SELECT promotion.*,ROW_NUMBER() OVER(PARTITION BY CardCode ORDER BY CreateDate DESC) rn FROM dbo.APZ_D_PMO3 promotion)
      SELECT partner.CardCode,partner.CardName,partner.Cellular,partner.CardType,contact.Name CntPrsName,contact.CntctCode,
        CONCAT(ISNULL(ship.Street,N''),N' ',ISNULL(ship.City,N''),N' ',ISNULL(ship.Country,N'')) ShipToAddress,partner.BillToDef,partner.ShipToDef,
        CONCAT(ISNULL(bill.Street,N''),N' ',ISNULL(bill.City,N''),N' ',ISNULL(bill.Country,N'')) BillToAddress,partner.U_Latitude,partner.U_Longitude,
        CASE WHEN promo.CreateDate BETWEEN DATEADD(year,-1,CONVERT(date,GETDATE())) AND CONVERT(date,GETDATE()) THEN 'red' ELSE 'dodgerblue' END MarkerColor,
        N'' MarkerIcon,partner.U_Province TinhTP,COALESCE((SELECT STRING_AGG(CONVERT(nvarchar(max),EmpId),N',') FROM dbo.APZ_D_STR2 WHERE CardCode=partner.CardCode),CONVERT(nvarchar(20),employee.empID)) saleEmployees,partner.SlpCode
      FROM dbo.OCRD partner LEFT JOIN dbo.OCPR contact ON contact.CardCode=partner.CardCode AND contact.Name=partner.CntctPrsn
      LEFT JOIN dbo.CRD1 ship ON ship.CardCode=partner.CardCode AND ship.Address=partner.ShipToDef AND ship.AdresType='S'
      LEFT JOIN dbo.CRD1 bill ON bill.CardCode=partner.CardCode AND bill.Address=partner.BillToDef AND bill.AdresType='B'
      LEFT JOIN dbo.OHEM employee ON employee.userId=partner.UserSign LEFT JOIN latest promo ON promo.CardCode=partner.CardCode AND promo.rn=1
      WHERE partner.CardCode IN(SELECT CardCode FROM #stores); RETURN;
    END;
    ;WITH props AS(
      SELECT partner.CardCode,property.GroupCode,property.GroupName FROM dbo.OCRD partner
      CROSS APPLY(VALUES(1,partner.QryGroup1),(2,partner.QryGroup2),(3,partner.QryGroup3),(4,partner.QryGroup4),(5,partner.QryGroup5),(6,partner.QryGroup6),(7,partner.QryGroup7),(8,partner.QryGroup8)) flags(GroupCode,YesNo)
      JOIN dbo.OCQG property ON property.GroupCode=flags.GroupCode WHERE partner.U_SubType='S' AND flags.YesNo='Y'), business AS(
      SELECT CardCode,STRING_AGG(CONVERT(nvarchar(max),GroupName),N',') BusinessType,STRING_AGG(CONVERT(nvarchar(max),GroupCode),N',') PropertiesCode FROM props GROUP BY CardCode)
    SELECT partner.CardCode,partner.CardName,ISNULL(partner.Balance,0) Balance,partner.Cellular,partner.CardType,contact.Name CntPrsName,partner.SlpCode,
      CONCAT(ISNULL(ship.Street,N''),N' ',ISNULL(ship.City,N''),N' ',ISNULL(ship.Country,N'')) ShipToAddress,
      CONCAT(ISNULL(bill.Street,N''),N' ',ISNULL(bill.City,N''),N' ',ISNULL(bill.Country,N'')) BillToAddress,partner.CreateDate,partner.CreateTS,
      partner.U_Latitude,partner.U_Longitude,partner.ListNum,partner.BillToDef,partner.ShipToDef,partner.U_Province TinhTP,business.PropertiesCode,business.BusinessType,
      image.FileName,COALESCE(image.LinkThumbnails,image.LinkDownload,(SELECT TOP(1) DefaultAvatar FROM dbo.APZ_D_OAPC WHERE AppName='i-DMS')) LinkThumbnails,
      COALESCE(image.LinkDownload,(SELECT TOP(1) DefaultAvatar FROM dbo.APZ_D_OAPC WHERE AppName='i-DMS')) LinkDownload,
      (SELECT COUNT(*) FROM dbo.APZ_D_VISIT visit WHERE visit.StoreCode=partner.CardCode AND YEAR(visit.FinishedDate)=YEAR(GETDATE()) AND MONTH(visit.FinishedDate)=MONTH(GETDATE())) NumOfVisit,
      (SELECT COUNT(*) FROM dbo.ORDR orders WHERE orders.CardCode=partner.CardCode AND YEAR(orders.DocDate)=YEAR(GETDATE()) AND MONTH(orders.DocDate)=MONTH(GETDATE()) AND orders.U_GUID IS NOT NULL AND orders.UserSign=@userId)
       +(SELECT COUNT(*) FROM dbo.OINV invoice WHERE invoice.CardCode=partner.CardCode AND YEAR(invoice.DocDate)=YEAR(GETDATE()) AND MONTH(invoice.DocDate)=MONTH(GETDATE()) AND invoice.U_GUID IS NOT NULL AND invoice.UserSign=@userId) NumOfOrder,
      (SELECT COUNT(*) FROM dbo.ORDR orders WHERE orders.CardCode=partner.CardCode AND YEAR(orders.DocDate)=YEAR(GETDATE()) AND MONTH(orders.DocDate)=MONTH(GETDATE()) AND orders.U_GUID IS NOT NULL AND orders.UserSign=@userId AND orders.DocStatus='O') NotDeliveredQty,
      0 UnPaidQty,COALESCE((SELECT TOP(1) EmpId FROM dbo.APZ_D_STR2 route WHERE route.CardCode=partner.CardCode ORDER BY Id,RouteCode,EmpId),employee.empID) saleEmployee,
      CASE WHEN ((SELECT COUNT(*) FROM dbo.ORDR orders WHERE orders.CardCode=partner.CardCode AND YEAR(orders.DocDate)=YEAR(GETDATE()) AND MONTH(orders.DocDate)=MONTH(GETDATE()) AND orders.U_GUID IS NOT NULL AND orders.UserSign=@userId)+(SELECT COUNT(*) FROM dbo.OINV invoice WHERE invoice.CardCode=partner.CardCode AND YEAR(invoice.DocDate)=YEAR(GETDATE()) AND MONTH(invoice.DocDate)=MONTH(GETDATE()) AND invoice.U_GUID IS NOT NULL AND invoice.UserSign=@userId))>0 THEN 'Y' ELSE 'N' END OrderStatus
    FROM dbo.OCRD partner LEFT JOIN dbo.OCPR contact ON contact.CardCode=partner.CardCode AND contact.Name=partner.CntctPrsn
    LEFT JOIN dbo.CRD1 ship ON ship.CardCode=partner.CardCode AND ship.Address=partner.ShipToDef AND ship.AdresType='S'
    LEFT JOIN dbo.CRD1 bill ON bill.CardCode=partner.CardCode AND bill.Address=partner.BillToDef AND bill.AdresType='B'
    LEFT JOIN business ON business.CardCode=partner.CardCode LEFT JOIN dbo.APZ_D_UPIM image ON image.CardCode=partner.CardCode AND image.IsAvatar='Y'
    LEFT JOIN dbo.OHEM employee ON employee.userId=partner.UserSign WHERE partner.CardCode IN(SELECT CardCode FROM #stores);
END;
GO
