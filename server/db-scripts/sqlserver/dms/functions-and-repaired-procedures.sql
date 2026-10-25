USE [SBODemoAU];
GO

CREATE OR ALTER FUNCTION dbo.CompareSplit
(
    @compareData nvarchar(max),
    @RowData nvarchar(max),
    @SplitOn nvarchar(5)
)
RETURNS smallint
AS
BEGIN
    DECLARE @part nvarchar(max), @position int;
    IF ISNULL(@SplitOn, N'') = N'' RETURN CASE WHEN @RowData=@compareData THEN 1 ELSE 0 END;
    WHILE CHARINDEX(@SplitOn, @RowData) > 0
    BEGIN
        SET @position=CHARINDEX(@SplitOn,@RowData);
        SET @part=LEFT(@RowData,@position-1);
        IF @part=@compareData RETURN 1;
        SET @RowData=SUBSTRING(@RowData,@position+LEN(@SplitOn),5000);
    END;
    RETURN CASE WHEN @RowData=@compareData THEN 1 ELSE 0 END;
END;
GO

CREATE OR ALTER FUNCTION dbo.Split
(
    @RowData nvarchar(max),
    @SplitOn nvarchar(5)
)
RETURNS @result TABLE ([DATA] nvarchar(max))
AS
BEGIN
    DECLARE @position int;
    IF ISNULL(@SplitOn,N'')=N''
    BEGIN
        INSERT @result VALUES(LTRIM(RTRIM(@RowData)));
        RETURN;
    END;
    WHILE CHARINDEX(@SplitOn,@RowData)>0
    BEGIN
        SET @position=CHARINDEX(@SplitOn,@RowData);
        INSERT @result VALUES(LTRIM(RTRIM(LEFT(@RowData,@position-1))));
        SET @RowData=SUBSTRING(@RowData,@position+LEN(@SplitOn),5000);
    END;
    INSERT @result VALUES(LTRIM(RTRIM(@RowData)));
    RETURN;
END;
GO

CREATE OR ALTER FUNCTION dbo.SplitString
(
    @RowData nvarchar(max),
    @SplitOn nvarchar(10)
)
RETURNS @result TABLE (id int, part nvarchar(max))
AS
BEGIN
    DECLARE @position int, @id int=1;
    IF ISNULL(@SplitOn,N'')=N''
    BEGIN
        INSERT @result VALUES(1,@RowData);
        RETURN;
    END;
    WHILE CHARINDEX(@SplitOn,@RowData)>0
    BEGIN
        SET @position=CHARINDEX(@SplitOn,@RowData);
        INSERT @result VALUES(@id,LEFT(@RowData,@position-1));
        SET @RowData=SUBSTRING(@RowData,@position+LEN(@SplitOn),5000);
        SET @id+=1;
    END;
    INSERT @result VALUES(@id,@RowData);
    RETURN;
END;
GO

CREATE OR ALTER FUNCTION dbo.GetTimeInteger(@timeValue time)
RETURNS int
AS
BEGIN
    RETURN CONVERT(int,REPLACE(CONVERT(char(8),@timeValue,108),':',''));
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_Approval_GetTemplateDetails @wddCode int, @wtmCode int
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @statusCnt int=0;
    SELECT @statusCnt=COUNT(1) FROM dbo.WDD1 WHERE WddCode=@wddCode AND Status IN ('W','N');
    SELECT a.WddCode,a.WtmCode,b.Name WtmName,a.DraftEntry DocNo,
      CASE a.ObjType WHEN 23 THEN 'Sales Quotation' WHEN 17 THEN 'Sales Order' WHEN 15 THEN 'Delivery' WHEN 16 THEN 'Return'
       WHEN 203 THEN 'A/R Down Payment' WHEN 13 THEN 'A/R Invoice' WHEN 14 THEN 'A/R Credit Memo'
       WHEN 1470000113 THEN 'Purchase Request' WHEN 540000006 THEN 'Purchase Quotation' WHEN 22 THEN 'Purchase Order'
       WHEN 20 THEN 'Goods Receipt PO' WHEN 21 THEN 'Goods Returns' WHEN 18 THEN 'A/P Invoice' WHEN 19 THEN 'A/P Credit Memo'
       WHEN 59 THEN 'Goods Receipt' WHEN 60 THEN 'Goods Issue' WHEN 1250000001 THEN 'Inventory Transfer Request'
       WHEN 67 THEN 'Inventory Transfer' WHEN 46 THEN 'Outgoing Payment' WHEN 1470000065 THEN 'Inventory Counting'
       WHEN 10000071 THEN 'Inventory Posting' WHEN 112 THEN 'Documents - Drafts' ELSE '' END DocType,
      a.OwnerID UserId,g.U_NAME UserName,CONVERT(varchar(30),a.CreateDate,121) CreateDate,a.CreateTime,b.Remarks,
      c.WstCode,CONVERT(varchar(20),c.SortId)+'.'+d.Name WstName,e.UserID AuthId,h.U_NAME AuthName,
      CASE WHEN f.Status IN('W','Y','N') THEN f.Status WHEN @statusCnt>0 THEN 'S' ELSE 'W' END StatusCode,
      CASE f.Status WHEN 'W' THEN 'Pending' WHEN 'Y' THEN 'Approved' WHEN 'N' THEN 'Rejected'
        ELSE CASE WHEN @statusCnt>0 THEN 'Not started' ELSE 'Pending' END END Status,
      CONVERT(varchar(30),f.UpdateDate,121) UpdateDate,f.UpdateTime,f.Remarks AuthRemarks,
      CASE WHEN ISNULL(i.DocTotalFC,0)>0 THEN ISNULL(i.DocTotalFC,0) ELSE ISNULL(i.DocTotal,0) END Total,
      i.DocCur Currency,c.SortId,a.IsDraft
    FROM dbo.OWDD a JOIN dbo.OWTM b ON a.WtmCode=b.WtmCode JOIN dbo.WTM2 c ON a.WtmCode=c.WtmCode
    JOIN dbo.OWST d ON c.WstCode=d.WstCode JOIN dbo.WST1 e ON d.WstCode=e.WstCode
    LEFT JOIN dbo.WDD1 f ON e.WstCode=f.StepCode AND e.UserID=f.UserID AND f.WddCode=a.WddCode
    JOIN dbo.OUSR g ON a.OwnerID=g.USERID JOIN dbo.OUSR h ON e.UserID=h.USERID JOIN dbo.ODRF i ON a.DraftEntry=i.DocEntry
    WHERE a.WddCode=@wddCode AND a.WtmCode=@wtmCode ORDER BY c.SortId;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_Delivery_GetListDeliveryOrder
    @userCode nvarchar(50)=N'', @date datetime, @status nvarchar(10)=N'A'
AS
BEGIN
    SET NOCOUNT ON;
    SELECT a.*,u.FileName,ISNULL(u.LinkThumbnails,ISNULL(u.LinkDownload,(SELECT TOP(1) DefaultAvatar FROM dbo.APZ_D_OAPC WHERE AppName=N'i-DMS'))) LinkThumbnails,
      (SELECT TOP(1) x.ConfirmTime FROM dbo.APZ_D_ODLO x WHERE x.UserSign=@userCode AND x.CardCode=a.CardCode AND x.Status=N'Done' ORDER BY x.ConfirmTime DESC) LastDelivery
    FROM dbo.APZ_D_ODLO a LEFT JOIN dbo.APZ_D_UPIM u ON a.CardCode=u.CardCode
    WHERE a.UserSign=@userCode AND CAST(a.ScheduledDate AS date)=CAST(@date AS date)
      AND (@status=N'A' OR a.Status=@status);
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_GetLastestEmployeeTrackingLocation @empId int
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @empName nvarchar(254)=N'';
    SELECT @empName=CONCAT(ISNULL(firstName,N''),N' ',ISNULL(middleName,N''),N' ',ISNULL(lastName,N'')) FROM dbo.OHEM WHERE empID=@empId;
    SELECT TOP(1) t.*,@empId empID,@empName empName FROM dbo.APZ_D_TKLC t
    WHERE t.UserSign=(SELECT TOP(1) userId FROM dbo.OHEM WHERE empID=@empId) ORDER BY t.CreateDate DESC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_GetLastestUserTrackingLocation @userId int
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @empId int=-1,@empName nvarchar(254)=N'';
    SELECT TOP(1) @empId=empID,@empName=CONCAT(ISNULL(firstName,N''),N' ',ISNULL(middleName,N''),N' ',ISNULL(lastName,N'')) FROM dbo.OHEM WHERE userId=@userId;
    SELECT TOP(1) t.*,@empId empID,@empName empName FROM dbo.APZ_D_TKLC t WHERE t.UserSign=@userId ORDER BY t.CreateDate DESC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_Delivery_GetDeliveryOrderDetails
    @docEntry int,
    @cardCode nvarchar(50)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT a.*, T1.FileName,
           ISNULL(T1.LinkThumbnails, ISNULL(T1.LinkDownload,
               (SELECT TOP (1) DefaultAvatar FROM dbo.APZ_D_OAPC WHERE AppName = N'i-DMS'))) AS LinkThumbnails
    FROM dbo.APZ_D_ODLO AS a
    LEFT JOIN dbo.APZ_D_UPIM AS T1 ON a.CardCode = T1.CardCode
    WHERE a.DocEntry = @docEntry AND a.CardCode = @cardCode;

    SELECT * FROM dbo.APZ_D_DLO1 WHERE DocEntry = @docEntry AND CardCode = @cardCode;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_SettingApprovalDetail_LoadData
    @id int = 0,
    @fromDate datetime2 = NULL,
    @toDate datetime2 = NULL,
    @objectType nvarchar(250) = N''
AS
BEGIN
    SET NOCOUNT ON;
    IF @id > 0
    BEGIN
        SELECT * FROM dbo.APZ_D_OSAD WHERE DocEntry = @id;
        RETURN;
    END;
    SELECT * FROM dbo.APZ_D_OSAD
    WHERE (ISNULL(@objectType, N'') = N'' OR ApprovalObjectType = @objectType)
      AND (@fromDate IS NULL OR CreateDate >= @fromDate)
      AND (@toDate IS NULL OR CreateDate <= @toDate)
    ORDER BY DocEntry DESC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_Geography_Remove @countryCode nvarchar(50)
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM dbo.APZ_D_OSTR WHERE CountryCode = @countryCode)
    BEGIN
        SELECT 20600004 AS ErrorCode, N'This country has been used' AS ErrorMessage;
        RETURN;
    END;
    DELETE FROM dbo.APZ_D_GGP1 WHERE CountryCode = @countryCode;
    DELETE FROM dbo.APZ_D_OGGP WHERE CountryCode = @countryCode;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_Geography_GetData
    @countryCode varchar(50), @location varchar(50), @status varchar(1)
AS
BEGIN
    SET NOCOUNT ON;
    IF (@countryCode <> '' OR @location <> '' OR @status <> '')
        SELECT * FROM dbo.APZ_D_OGGP
        WHERE (CountryCode = '' OR CountryCode LIKE @countryCode + '%')
          AND (Location = '' OR Location LIKE @location + '%')
          AND (Status = '' OR Status LIKE @status + '%');
    ELSE
        SELECT * FROM dbo.APZ_D_OGGP;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_QuestionMasterData_Remove @id int = 0
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM dbo.APZ_D_OSUV WHERE SurveyQuessionId = @id)
    BEGIN
        SELECT 1 AS ErrorCode, N'Cannot remove Question because it has been used' AS ErrorMessage;
        RETURN;
    END;
    DELETE FROM dbo.APZ_D_OCHM WHERE DocEntry = @id;
    DELETE FROM dbo.APZ_D_CHM1 WHERE DocEntry = @id;
    SELECT 0 AS ErrorCode, N'' AS ErrorMessage;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_QuestionMasterData_LoadData
    @id int = 0, @fromDate datetime2 = NULL, @toDate datetime2 = NULL, @status char(1) = ''
AS
BEGIN
    SET NOCOUNT ON;
    IF @id > 0
    BEGIN
        SELECT * FROM dbo.APZ_D_OCHM WHERE DocEntry = @id;
        RETURN;
    END;
    SELECT * FROM dbo.APZ_D_OCHM
    WHERE (ISNULL(@status, '') = '' OR Status = @status)
      AND (@fromDate IS NULL OR CreateTime >= @fromDate)
      AND (@toDate IS NULL OR CreateTime <= @toDate)
    ORDER BY DocEntry DESC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_SettingReport_LoadData
    @id int = 0, @fromDate datetime2 = NULL, @toDate datetime2 = NULL, @report nvarchar(250) = N''
AS
BEGIN
    SET NOCOUNT ON;
    IF @id > 0
    BEGIN
        SELECT * FROM dbo.APZ_D_OSRT WHERE DocEntry = @id;
        RETURN;
    END;
    SELECT * FROM dbo.APZ_D_OSRT
    WHERE (ISNULL(@report, N'') = N'' OR ReportId = @report)
      AND (@fromDate IS NULL OR CreateDate >= @fromDate)
      AND (@toDate IS NULL OR CreateDate <= @toDate)
    ORDER BY DocEntry DESC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_GetListNotification
    @userName nvarchar(50), @appName nvarchar(50), @dbName nvarchar(50),
    @startIndex int = 0, @pageSize int = 0
AS
BEGIN
    SET NOCOUNT ON;
    IF @startIndex = 0
        SELECT @startIndex = ISNULL(MAX(ID), 0) + 1 FROM dbo.APZ_D_ELN1;
    SELECT *
    FROM (
        SELECT *, ROW_NUMBER() OVER (ORDER BY ID DESC) AS RowNum
        FROM dbo.APZ_D_ELN1
        WHERE Method = @appName AND DatabaseName = @dbName
          AND dbo.CompareSplit(@userName, SendUser, N',') = 1
          AND IsRemove = 'N' AND ID < @startIndex
    ) AS RowConstrainedResult
    WHERE RowNum >= 1 AND (@pageSize <= 0 OR RowNum <= @pageSize)
    ORDER BY RowNum;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_Geography_CheckRemoveDetail @code varchar(50)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @message nvarchar(254) = N'Cannot remove this item because item has been used';
    DECLARE @objType int = 1470000158;
    IF EXISTS (SELECT 1 FROM dbo.APZ_D_OGGP a JOIN dbo.APZ_D_OSTR b ON a.CountryCode=b.CountryCode WHERE a.CountryCode=@code)
        SELECT @objType AS ErrorCode, @message AS ErrorMessage;
    IF EXISTS (SELECT 1 FROM dbo.APZ_D_GGP1 a JOIN dbo.APZ_D_STR1 b ON a.Code=b.GgpCode WHERE a.Code=@code)
        SELECT @objType AS ErrorCode, @message AS ErrorMessage;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_ImageAlbum_Remove @id int
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM dbo.APZ_D_UPIM WHERE AlbumId=@id)
    BEGIN SELECT 1 AS ErrorCode, N'Cannot remove id because it has been used' AS ErrorMessage; RETURN; END;
    IF @id=-1
    BEGIN SELECT 2 AS ErrorCode, N'Cannot remove id because it default' AS ErrorMessage; RETURN; END;
    DELETE FROM dbo.APZ_D_ImageAlbum WHERE Id=@id;
    SELECT 0 AS ErrorCode, N'' AS ErrorMessage;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_SettingUDF_GetData
    @menuID int, @tableName nvarchar(50), @type nvarchar(5)
AS
BEGIN
    SET NOCOUNT ON;
    IF @type=N'APZ'
    BEGIN
        SELECT t0.*,
          CASE WHEN t0.TypeID='N' AND t0.EditType='T' THEN 'D' WHEN t0.TypeID='A' AND t0.EditType='I' THEN 'M' WHEN t0.TypeID='M' AND t0.EditType='' THEN 'A' ELSE t0.TypeID END AS TypeID1,
          CASE WHEN t0.TypeID='M' AND t0.EditType='' THEN 'B' ELSE t0.EditType END AS EditType1,
          ISNULL(t1.IsActive,'N') AS IsActive, ISNULL(t1.IsEditable,'Y') AS IsEditable
        FROM dbo.APZ_CUFD t0 LEFT JOIN dbo.APZ_D_SUDF t1 ON t0.TableID=t1.TableID AND t0.FieldID=t1.FieldID AND t1.MenuID=@menuID
        WHERE t0.TableID=@tableName ORDER BY t0.FieldID;
        SELECT TableID,FieldID,IndexID,FldValue,Descr,FldDate FROM dbo.APZ_UFD1 WHERE TableID=@tableName ORDER BY FieldID,IndexID;
    END
    ELSE
    BEGIN
        SELECT t0.*,
          CASE WHEN t0.TypeID='N' AND t0.EditType='T' THEN 'D' WHEN t0.TypeID='A' AND t0.EditType='I' THEN 'M' WHEN t0.TypeID='M' AND t0.EditType='' THEN 'A' ELSE t0.TypeID END AS TypeID1,
          CASE WHEN t0.TypeID='M' AND t0.EditType='' THEN 'B' ELSE t0.EditType END AS EditType1,
          ISNULL(t1.IsActive,'N') AS IsActive, ISNULL(t1.IsEditable,'Y') AS IsEditable
        FROM dbo.CUFD t0 LEFT JOIN dbo.APZ_D_SUDF t1 ON t0.TableID=t1.TableID AND t0.FieldID=t1.FieldID AND t1.MenuID=@menuID
        WHERE t0.TableID=@tableName ORDER BY t0.FieldID;
        SELECT TableID,FieldID,IndexID,FldValue,Descr,FldDate FROM dbo.UFD1 WHERE TableID=@tableName ORDER BY FieldID,IndexID;
    END
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_UpdateInventory_SearchData
    @cardCode nvarchar(15)=N'', @searchKey nvarchar(100)=N''
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @inventoryType nvarchar(5)=N'O';
    SELECT TOP (1) @inventoryType=WarehouseInventoryType FROM dbo.APZ_D_OAPC ORDER BY WarehouseInventoryType;
    IF @inventoryType=N'O'
        SELECT DISTINCT T0.ItemCode,T2.ItemName FROM dbo.RDR1 T0 JOIN dbo.ORDR T1 ON T0.DocEntry=T1.DocEntry JOIN dbo.OITM T2 ON T0.ItemCode=T2.ItemCode
        WHERE (T1.CardCode=@cardCode OR @cardCode=N'') AND (T0.ItemCode LIKE N'%'+@searchKey+N'%' OR T2.ItemName LIKE N'%'+@searchKey+N'%')
          AND ((T2.validFor='Y' AND ISNULL(T2.frozenFor,'N')='N' AND ISNULL(T2.validFrom,CONVERT(date,GETDATE()))<=CONVERT(date,GETDATE()) AND ISNULL(T2.validTo,CONVERT(date,GETDATE()))>=CONVERT(date,GETDATE()))
            OR (ISNULL(T2.validFor,'N')='N' AND T2.frozenFor='Y' AND (ISNULL(T2.frozenFrom,CONVERT(date,GETDATE()))>CONVERT(date,GETDATE()) OR ISNULL(T2.frozenTo,CONVERT(date,GETDATE()))<CONVERT(date,GETDATE())))
            OR (T2.validFor='Y' AND T2.frozenFor='Y' AND ISNULL(T2.validFrom,CONVERT(date,GETDATE()))<=CONVERT(date,GETDATE()) AND ISNULL(T2.validTo,CONVERT(date,GETDATE()))>=CONVERT(date,GETDATE()) AND (ISNULL(T2.frozenFrom,CONVERT(date,GETDATE()))>CONVERT(date,GETDATE()) OR ISNULL(T2.frozenTo,CONVERT(date,GETDATE()))<CONVERT(date,GETDATE())))
            OR (ISNULL(T2.validFor,'N')='N' AND ISNULL(T2.frozenFor,'N')='N')) ORDER BY T0.ItemCode;
    ELSE IF @inventoryType=N'A'
        SELECT DISTINCT T0.ItemCode,T2.ItemName FROM dbo.RDR1 T0 JOIN dbo.ORDR T1 ON T0.DocEntry=T1.DocEntry JOIN dbo.OITM T2 ON T0.ItemCode=T2.ItemCode
        WHERE (T0.ItemCode LIKE N'%'+@searchKey+N'%' OR T2.ItemName LIKE N'%'+@searchKey+N'%')
          AND ((T2.validFor='Y' AND ISNULL(T2.frozenFor,'N')='N' AND ISNULL(T2.validFrom,CONVERT(date,GETDATE()))<=CONVERT(date,GETDATE()) AND ISNULL(T2.validTo,CONVERT(date,GETDATE()))>=CONVERT(date,GETDATE()))
            OR (ISNULL(T2.validFor,'N')='N' AND T2.frozenFor='Y' AND (ISNULL(T2.frozenFrom,CONVERT(date,GETDATE()))>CONVERT(date,GETDATE()) OR ISNULL(T2.frozenTo,CONVERT(date,GETDATE()))<CONVERT(date,GETDATE())))
            OR (T2.validFor='Y' AND T2.frozenFor='Y' AND ISNULL(T2.validFrom,CONVERT(date,GETDATE()))<=CONVERT(date,GETDATE()) AND ISNULL(T2.validTo,CONVERT(date,GETDATE()))>=CONVERT(date,GETDATE()) AND (ISNULL(T2.frozenFrom,CONVERT(date,GETDATE()))>CONVERT(date,GETDATE()) OR ISNULL(T2.frozenTo,CONVERT(date,GETDATE()))<CONVERT(date,GETDATE())))
            OR (ISNULL(T2.validFor,'N')='N' AND ISNULL(T2.frozenFor,'N')='N')) ORDER BY T0.ItemCode;
    ELSE
        SELECT T0.ItmsGrpCod AS ItemCode,T0.ItmsGrpNam AS ItemName FROM dbo.OITB T0 WHERE CONVERT(nvarchar(50),T0.ItmsGrpCod) LIKE N'%'+@searchKey+N'%' OR T0.ItmsGrpNam LIKE N'%'+@searchKey+N'%' ORDER BY T0.ItmsGrpCod;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_SettingReport_SaveUserReportParams
AS
BEGIN
    SET NOCOUNT ON;
    DELETE t FROM dbo.APZ_D_SRT4 t
    WHERE t.DocEntry=(SELECT TOP (1) DocEntry FROM #APZ_D_SRT4)
      AND t.UserCode COLLATE DATABASE_DEFAULT=(SELECT TOP (1) UserCode COLLATE DATABASE_DEFAULT FROM #APZ_D_SRT4);
    INSERT dbo.APZ_D_SRT4 (DocEntry,UserCode,Name,Value,CreateDate,CreateTime)
    SELECT DocEntry,UserCode,Name,Value,CONVERT(date,GETDATE()),CONVERT(time,GETDATE()) FROM #APZ_D_SRT4;
    SELECT '200' AS MessageCode, N'' AS Message;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_ImageAlbum_Saves
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.APZ_D_ImageAlbum;
    INSERT dbo.APZ_D_ImageAlbum (Id,Name,Remarks) SELECT Id,Name,Remarks FROM #APZ_D_ImageAlbum;
    SELECT 0 AS ErrorCode,N'' AS ErrorMessage;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_Marker_Saves
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.APZ_D_MARKER;
    INSERT dbo.APZ_D_MARKER (Code,Name,SvgData,DfltColorHex,DfltSize)
    SELECT Code,Name,SvgData,DfltColorHex,DfltSize FROM #APZ_D_MARKER;
    SELECT 0 AS ErrorCode,N'' AS ErrorMessage;
END;
GO

CREATE OR ALTER PROCEDURE dbo.Apz_D_Delivery_ConfirmDelivery @type nvarchar(50)=N'A'
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE b SET b.StartTime=a.StartTime,b.ConfirmTime=a.ConfirmTime,b.Feedback=a.Feedback,
      b.ActualAmount=a.ActualAmount,b.DeliveredFull=a.DeliveredFull,b.Latitude=a.Latitude,
      b.Longtitude=a.Longtitude,b.Status=N'Done'
    FROM dbo.APZ_D_ODLO b JOIN #Temp_Confirm a ON a.DocEntry=b.DocEntry AND a.CardCode COLLATE DATABASE_DEFAULT=b.CardCode COLLATE DATABASE_DEFAULT;
    SELECT 0 AS ErrorCode,N'' AS ErrorMessage;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_RemoveDeviceToken
AS
BEGIN
    SET NOCOUNT ON;
    DELETE a FROM dbo.APZ_D_OADT a JOIN #APZ_D_OADT b
      ON a.UserID=b.UserID
     AND a.DeviceID COLLATE DATABASE_DEFAULT=b.DeviceID COLLATE DATABASE_DEFAULT
     AND a.DeviceName COLLATE DATABASE_DEFAULT=b.DeviceName COLLATE DATABASE_DEFAULT
     AND a.DatabaseName COLLATE DATABASE_DEFAULT=b.DatabaseName COLLATE DATABASE_DEFAULT
     AND a.AppCode COLLATE DATABASE_DEFAULT=b.AppCode COLLATE DATABASE_DEFAULT;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_InsertDeviceToken
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE a SET a.UserName=b.UserName,a.AppType=b.AppType,a.DeviceToken=b.DeviceToken,
      a.OSName=b.OSName,a.OSVersion=b.OSVersion,a.MacAddress=b.MacAddress,
      a.AppVersion=b.AppVersion,a.UpdateTime=CONVERT(date,GETDATE())
    FROM dbo.APZ_D_OADT a JOIN #APZ_D_OADT b
      ON a.UserID=b.UserID
     AND a.DeviceID COLLATE DATABASE_DEFAULT=b.DeviceID COLLATE DATABASE_DEFAULT
     AND a.DeviceName COLLATE DATABASE_DEFAULT=b.DeviceName COLLATE DATABASE_DEFAULT
     AND a.DatabaseName COLLATE DATABASE_DEFAULT=b.DatabaseName COLLATE DATABASE_DEFAULT
     AND a.AppCode COLLATE DATABASE_DEFAULT=b.AppCode COLLATE DATABASE_DEFAULT;

    DECLARE @baseId int=(SELECT ISNULL(MAX(ID),0) FROM dbo.APZ_D_OADT);
    INSERT dbo.APZ_D_OADT (ID,UserID,UserName,DeviceID,DeviceName,Domain,DatabaseName,AppType,DeviceToken,OSName,OSVersion,MacAddress,AppCode,AppVersion,CreateTime,UpdateTime)
    SELECT @baseId+ROW_NUMBER() OVER(ORDER BY (SELECT NULL)),b.UserID,b.UserName,b.DeviceID,b.DeviceName,b.Domain,b.DatabaseName,b.AppType,b.DeviceToken,b.OSName,b.OSVersion,b.MacAddress,b.AppCode,b.AppVersion,CONVERT(date,GETDATE()),CONVERT(date,GETDATE())
    FROM #APZ_D_OADT b
    WHERE NOT EXISTS (SELECT 1 FROM dbo.APZ_D_OADT a WHERE a.UserID=b.UserID
      AND a.DeviceID COLLATE DATABASE_DEFAULT=b.DeviceID COLLATE DATABASE_DEFAULT
      AND a.DeviceName COLLATE DATABASE_DEFAULT=b.DeviceName COLLATE DATABASE_DEFAULT
      AND a.DatabaseName COLLATE DATABASE_DEFAULT=b.DatabaseName COLLATE DATABASE_DEFAULT
      AND a.AppCode COLLATE DATABASE_DEFAULT=b.AppCode COLLATE DATABASE_DEFAULT);
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_Sync_GetCommentCustomer
    @userName nvarchar(50),@objectType nvarchar(50),@relatedTyp nvarchar(50)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @userId int=(SELECT TOP (1) USERID FROM dbo.OUSR WHERE USER_CODE=@userName ORDER BY USERID);
    SELECT c.*,c.RelatedEnt AS CardCode,
      (SELECT TOP (1) LinkThumbnails FROM dbo.APZ_D_UUPIM WHERE UserSign=@userId AND ObjectType=@objectType) AS LinkThumbnails
    FROM dbo.APZ_D_OCMC c
    WHERE c.UserSign=@userId
      AND EXISTS (SELECT 1 FROM #temp t WHERE t.CardCode COLLATE DATABASE_DEFAULT=c.RelatedEnt COLLATE DATABASE_DEFAULT)
      AND c.RelatedTyp=@relatedTyp
    ORDER BY c.CreatedTime DESC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_TrackingUserLocation
AS
BEGIN
    SET NOCOUNT ON;
    INSERT dbo.APZ_D_TKLC(Code,LineNum,UserSign,DeviceId,DeviceName,DeviceType,DeviceModel,SystemName,SystemVersion,MacAddress,BatteryPercent,Latitude,Longitude,TrackingDate,TrackingTime,CreateDate,CreateTime,CreateType,Distance,AppVersion)
    SELECT a.Code,(SELECT ISNULL(MAX(x.LineNum),0) FROM dbo.APZ_D_TKLC x WHERE x.UserSign=a.UserSign)+ROW_NUMBER() OVER(PARTITION BY a.UserSign ORDER BY a.TrackingDate),
      a.UserSign,a.DeviceId,a.DeviceName,a.DeviceType,a.DeviceModel,a.SystemName,a.SystemVersion,a.MacAddress,a.BatteryPercent,a.Latitude,a.Longitude,a.TrackingDate,
      CONVERT(time,a.TrackingDate),CONVERT(date,GETDATE()),CONVERT(time,GETDATE()),a.CreateType,a.Distance,a.AppVersion
    FROM #APZ_D_TKLC a;
    IF @@ROWCOUNT<=0 SELECT 400 AS Code,N'Insert table header fail' AS Message;
    ELSE SELECT 200 AS Code,N'' AS Message;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_UploadFile
AS
BEGIN
    SET NOCOUNT ON;
    DELETE t FROM dbo.APZ_D_UUPIM t WHERE EXISTS(SELECT 1 FROM #APZ_D_UUPIM s WHERE s.Code COLLATE DATABASE_DEFAULT=t.Code COLLATE DATABASE_DEFAULT);
    INSERT dbo.APZ_D_UUPIM(Code,CreateDate,CreateTime,UserSign,FileId,FileName,LinkDownload,LinkThumbnails,MimeType,Path,Size,WebUrl,UploadDate,UploadTime,AlbumId,FileNameDrive,ObjectType,ObjectCode,Extension,PathThumbnail,Domain,DateCreated,TimestampCreated)
    SELECT Code,CONVERT(date,GETDATE()),CONVERT(time,GETDATE()),UserSign,FileId,FileName,LinkDownload,LinkThumbnails,MimeType,Path,Size,WebUrl,UploadDate,CONVERT(time,UploadDate),AlbumId,FileNameDrive,ObjectType,ObjectCode,Extension,PathThumbnail,Domain,DateCreated,TimestampCreated FROM #APZ_D_UUPIM;
    IF @@ROWCOUNT<=0 SELECT N'1470000169' AS ObjectType,406 AS Code,N'Insert table APZ_D_UUPIM fail' AS Message;
    ELSE SELECT N'1470000169' AS ObjectType,200 AS Code,N'' AS Message;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_UploadImage
AS
BEGIN
    SET NOCOUNT ON;
    DELETE t FROM dbo.APZ_D_UPIM t WHERE EXISTS(SELECT 1 FROM #APZ_D_UPIM s WHERE s.Code COLLATE DATABASE_DEFAULT=t.Code COLLATE DATABASE_DEFAULT);
    INSERT dbo.APZ_D_UPIM(Code,CardCode,VisitCode,CreateDate,CreateTime,UserSign,FileId,FileName,LinkDownload,LinkThumbnails,MimeType,Path,Size,WebUrl,UploadDate,UploadTime,IsAvatar,AlbumId,FileNameDrive,Extension,PathThumbnail,Domain,DateCreated,TimestampCreated)
    SELECT Code,CardCode,VisitCode,CONVERT(date,GETDATE()),CONVERT(time,GETDATE()),UserSign,FileId,FileName,LinkDownload,LinkThumbnails,MimeType,Path,Size,WebUrl,UploadDate,CONVERT(time,UploadDate),IsAvatar,AlbumId,FileNameDrive,Extension,PathThumbnail,Domain,DateCreated,TimestampCreated FROM #APZ_D_UPIM;
    IF @@ROWCOUNT<=0 SELECT N'1470000169' AS ObjectType,406 AS Code,N'Insert table APZ_D_UPIM fail' AS Message;
    ELSE SELECT N'1470000169' AS ObjectType,200 AS Code,N'' AS Message;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_SettingUDF_SaveData
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @menuID int=(SELECT TOP(1) MenuID FROM #temp);
    DELETE FROM dbo.APZ_D_SUDF WHERE MenuID=@menuID;
    IF EXISTS(SELECT 1 FROM #temp)
    BEGIN
      INSERT dbo.APZ_D_SUDF(TableID,MenuID,FieldID,IsActive,IsEditable,CreateDate,UserSign)
      SELECT TableID,MenuID,FieldID,IsActive,IsEditable,SYSDATETIME(),UserSign FROM #temp;
      IF @@ROWCOUNT<=0 SELECT 406 AS MessageCode,N'Insert data to table APZ_D_SUDF failed' AS Message;
      ELSE SELECT 200 AS MessageCode,N'' AS Message;
    END
    ELSE SELECT 404 AS MessageCode,N'Notfound data to save' AS Message;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_Sync_UpdateInventory_SearchData
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @inventoryType nvarchar(5)=N'O';
    SELECT TOP(1) @inventoryType=WarehouseInventoryType FROM dbo.APZ_D_OAPC ORDER BY WarehouseInventoryType;
    IF @inventoryType=N'O'
      SELECT CONVERT(nvarchar(254),T0.ItemCode) AS ItemCode,T2.ItemName,T1.CardCode
      FROM dbo.RDR1 T0 JOIN dbo.ORDR T1 ON T0.DocEntry=T1.DocEntry JOIN dbo.OITM T2 ON T0.ItemCode=T2.ItemCode
      WHERE (EXISTS(SELECT 1 FROM #temp x WHERE x.CardCode COLLATE DATABASE_DEFAULT=T1.CardCode COLLATE DATABASE_DEFAULT) OR NOT EXISTS(SELECT 1 FROM #temp))
        AND ((T2.validFor='Y' AND ISNULL(T2.frozenFor,'N')='N' AND ISNULL(T2.validFrom,CONVERT(date,GETDATE()))<=CONVERT(date,GETDATE()) AND ISNULL(T2.validTo,CONVERT(date,GETDATE()))>=CONVERT(date,GETDATE())) OR (ISNULL(T2.validFor,'N')='N' AND ISNULL(T2.frozenFor,'N')='N'));
    ELSE IF @inventoryType=N'A'
      SELECT DISTINCT CONVERT(nvarchar(254),T0.ItemCode) AS ItemCode,T2.ItemName
      FROM dbo.RDR1 T0 JOIN dbo.ORDR T1 ON T0.DocEntry=T1.DocEntry JOIN dbo.OITM T2 ON T0.ItemCode=T2.ItemCode
      WHERE (T2.validFor='Y' AND ISNULL(T2.frozenFor,'N')='N') OR (ISNULL(T2.validFor,'N')='N' AND ISNULL(T2.frozenFor,'N')='N');
    ELSE SELECT CONVERT(nvarchar(254),ItmsGrpCod) AS ItemCode,ItmsGrpNam AS ItemName FROM dbo.OITB;
END;
GO
