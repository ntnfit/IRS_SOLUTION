USE [SBODemoAU];
GO

CREATE OR ALTER PROCEDURE dbo.APZ_Function_GetApprovalTemplateData
    @userId int,
    @status varchar(10),
    @requestType char(1) = ''
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @mainCurrency nvarchar(3)=(SELECT TOP(1) MainCurncy FROM dbo.OADM);

    SELECT DISTINCT
        a.WddCode,a.WtmCode,CONVERT(nvarchar(254),t.Name) WtmName,
        CONVERT(nvarchar(15),ISNULL(d.CardCode,N'')) CardCode,
        CONVERT(nvarchar(254),ISNULL(d.CardName,N'')) CardName,
        a.OwnerID OwnerId,CONVERT(varchar(50),a.DraftEntry) DocNo,a.ObjType,
        CONVERT(varchar(100),CASE a.ObjType
          WHEN 23 THEN 'Sales Quotation' WHEN 17 THEN 'Sales Order' WHEN 15 THEN 'Delivery'
          WHEN 16 THEN 'Return' WHEN 203 THEN 'A/R Down Payment' WHEN 13 THEN 'A/R Invoice'
          WHEN 14 THEN 'A/R Credit Memo' WHEN 22 THEN 'Purchase Order' WHEN 20 THEN 'Goods Receipt PO'
          WHEN 21 THEN 'Goods Returns' WHEN 18 THEN 'A/P Invoice' WHEN 19 THEN 'A/P Credit Memo'
          WHEN 59 THEN 'Goods Receipt' WHEN 60 THEN 'Goods Issue' WHEN 67 THEN 'Inventory Transfer'
          WHEN 46 THEN 'Outgoing Payment' WHEN 112 THEN 'Documents - Drafts'
          ELSE CONCAT('Object ',a.ObjType) END) DocType,
        CONVERT(datetime,a.DocDate) DocDate,CONVERT(datetime,d.DocDueDate) DocDueDate,
        CONVERT(decimal(21,6),CASE WHEN d.DocCur=@mainCurrency THEN d.DocTotal ELSE d.DocTotalFC END) DocTotal,
        CONVERT(varchar(10),d.DocCur) DocCur,a.IsDraft,a.OwnerID UserId,
        CONVERT(varchar(10),u.USER_CODE) UserCode,CONVERT(nvarchar(50),u.U_NAME) UserName,
        CONVERT(datetime,a.CreateDate) UpdateDate,a.CreateTime UpdateTime,a.Status,
        CONVERT(nvarchar(max),ISNULL(a.Remarks,N'')) Remarks,@requestType RequestType,
        CONVERT(char(5),'False') ItemCheck,CONVERT(char(5),a.ProcesStat) ProcesStat
    FROM dbo.OWDD a
    JOIN dbo.OUSR u ON u.USERID=a.OwnerID
    JOIN dbo.OWTM t ON t.WtmCode=a.WtmCode
    JOIN dbo.ODRF d ON d.DocEntry=a.DraftEntry
    WHERE d.DocStatus='O'
      AND ((@requestType='M' AND a.OwnerID=@userId)
        OR (@requestType='P' AND EXISTS(SELECT 1 FROM dbo.WDD1 w WHERE w.WddCode=a.WddCode AND w.UserID=@userId)))
      AND (@status='All'
        OR (@status='C' AND a.ProcesStat IN ('A','P'))
        OR (@status<>'C' AND (@status='A' OR a.Status=@status) AND a.ProcesStat IN ('W','Y','N')))
    ORDER BY DocDate;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_Approved_List
    @userName nvarchar(100)='-1'
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @userId int=ISNULL((SELECT TOP(1) USERID FROM dbo.OUSR WHERE USER_CODE=@userName),-1);
    CREATE TABLE #approval(WddCode int,WtmCode int,WtmName nvarchar(254),CardCode nvarchar(15),CardName nvarchar(254),OwnerId int,DocNo varchar(50),ObjType int,DocType varchar(100),DocDate datetime,DocDueDate datetime,DocTotal decimal(21,6),DocCur varchar(10),IsDraft char(1),UserId int,UserCode varchar(10),UserName nvarchar(50),UpdateDate datetime,UpdateTime int,Status char(1),Remarks nvarchar(max),RequestType char(1),ItemCheck char(5),ProcesStat char(5));
    INSERT #approval EXEC dbo.APZ_Function_GetApprovalTemplateData @userId,'All','P';
    ;WITH x AS
    (
      SELECT a.*,
        CASE WHEN EXISTS(SELECT 1 FROM dbo.WDD1 w WHERE w.WddCode=a.WddCode AND w.UserID=@userId AND w.Status='W') THEN 1 ELSE 0 END IsPending,
        CASE WHEN EXISTS(SELECT 1 FROM dbo.WDD1 w WHERE w.WddCode=a.WddCode AND w.UserID=@userId)
          AND NOT EXISTS(SELECT 1 FROM dbo.WDD1 w WHERE w.WddCode=a.WddCode AND w.UserID=@userId AND w.Status='W') THEN 1 ELSE 0 END IsApproved
      FROM #approval a
    )
    SELECT DocType Name,SUM(IsPending) PendingQuantity,SUM(IsApproved) ApprovedQuantity,
      COUNT(*) AllQuantity,CONVERT(nvarchar(20),ObjType) ObjType
    FROM x GROUP BY DocType,ObjType ORDER BY DocType;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_Approved_ListV2
    @userName nvarchar(100)='-1', @fromDate nvarchar(100)=NULL, @toDate nvarchar(100)=NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @userId int=ISNULL((SELECT TOP(1) USERID FROM dbo.OUSR WHERE USER_CODE=@userName),-1),
            @from date=TRY_CONVERT(date,@fromDate),@to date=TRY_CONVERT(date,@toDate);
    CREATE TABLE #approval(WddCode int,WtmCode int,WtmName nvarchar(254),CardCode nvarchar(15),CardName nvarchar(254),OwnerId int,DocNo varchar(50),ObjType int,DocType varchar(100),DocDate datetime,DocDueDate datetime,DocTotal decimal(21,6),DocCur varchar(10),IsDraft char(1),UserId int,UserCode varchar(10),UserName nvarchar(50),UpdateDate datetime,UpdateTime int,Status char(1),Remarks nvarchar(max),RequestType char(1),ItemCheck char(5),ProcesStat char(5));
    INSERT #approval EXEC dbo.APZ_Function_GetApprovalTemplateData @userId,'All','P';
    ;WITH x AS (SELECT a.*,COALESCE((SELECT TOP(1) w.Status COLLATE DATABASE_DEFAULT FROM dbo.WDD1 w WHERE w.WddCode=a.WddCode AND w.UserID=@userId ORDER BY w.SortId DESC),a.ProcesStat COLLATE DATABASE_DEFAULT) CurrentStatus FROM #approval a WHERE (@from IS NULL OR a.DocDate>=@from) AND (@to IS NULL OR a.DocDate<DATEADD(day,1,@to)))
    SELECT DocType Name,COUNT(*) AllQuantity,SUM(CASE WHEN CurrentStatus IN('Y','A','P') THEN 1 ELSE 0 END) ApprovedQuantity,SUM(CASE WHEN CurrentStatus='W' THEN 1 ELSE 0 END) PendingQuantity,SUM(CASE WHEN CurrentStatus IN('-','C','N') THEN 1 ELSE 0 END) RejectedQuantity,CONVERT(nvarchar(20),ObjType) ObjType
    FROM x GROUP BY DocType,ObjType ORDER BY DocType;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_Approved_ListItem
    @objType nvarchar(20),@userCode nvarchar(50)='',@fromDate nvarchar(100)=NULL,@toDate nvarchar(100)=NULL,
    @pageIndex int=1,@typeFilter nvarchar(10)='A',@searchText nvarchar(100)='',@wddCode int=-1
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @userId int=ISNULL((SELECT TOP(1) USERID FROM dbo.OUSR WHERE USER_CODE=@userCode),-1),@from date=TRY_CONVERT(date,@fromDate),@to date=TRY_CONVERT(date,@toDate);
    CREATE TABLE #a(WddCode int,WtmCode int,WtmName nvarchar(254),CardCode nvarchar(15),CardName nvarchar(254),OwnerId int,DocNo varchar(50),ObjType int,DocType varchar(100),DocDate datetime,DocDueDate datetime,DocTotal decimal(21,6),DocCur varchar(10),IsDraft char(1),UserId int,UserCode varchar(10),UserName nvarchar(50),UpdateDate datetime,UpdateTime int,Status char(1),Remarks nvarchar(max),RequestType char(1),ItemCheck char(5),ProcesStat char(5));
    INSERT #a EXEC dbo.APZ_Function_GetApprovalTemplateData @userId,'All','P';
    ;WITH x AS (SELECT a.*,
      CASE a.Status COLLATE DATABASE_DEFAULT WHEN '-' THEN N'Without' WHEN 'W' THEN N'Pending' WHEN 'Y' THEN N'Approved' WHEN 'N' THEN N'Rejected' WHEN 'P' THEN N'Generated' WHEN 'A' THEN N'Generated by Authorizer' WHEN 'C' THEN N'Canceled' END StatusName,
      p.UserID ProcessUserId,pu.U_NAME ProcessUserName,ou.U_NAME OwnerUserName,p.Status UserStatus,
      s.Remarks CurrentStepRemarks,p.StepCode CurrentStep,
      CASE WHEN a.ProcesStat COLLATE DATABASE_DEFAULT<>'W' THEN a.ProcesStat COLLATE DATABASE_DEFAULT ELSE ISNULL(p.Status COLLATE DATABASE_DEFAULT,'W') END CurrentStatus
      FROM #a a OUTER APPLY(SELECT TOP(1) w.UserID,w.Status,w.StepCode FROM dbo.WDD1 w WHERE w.WddCode=a.WddCode ORDER BY CASE WHEN w.UserID=@userId THEN 0 ELSE 1 END,w.SortId DESC)p
      LEFT JOIN dbo.OUSR pu ON pu.USERID=p.UserID LEFT JOIN dbo.OUSR ou ON ou.USERID=a.OwnerId LEFT JOIN dbo.WTM2 s ON s.WtmCode=a.WtmCode AND s.WstCode=p.StepCode
      WHERE CONVERT(nvarchar(20),a.ObjType)=@objType AND (@from IS NULL OR a.DocDate>=@from) AND (@to IS NULL OR a.DocDate<DATEADD(day,1,@to))
      AND (@wddCode IS NULL OR @wddCode IN(-1,0) OR a.WddCode=@wddCode)
      AND (ISNULL(@searchText,N'')=N'' OR CONVERT(nvarchar(20),a.WddCode)=@searchText OR a.DocNo LIKE N'%'+@searchText+N'%' OR a.UserName LIKE N'%'+@searchText+N'%' OR a.Remarks LIKE N'%'+@searchText+N'%')),
    f AS (SELECT *,ProcesStat DocumentStatus,ROW_NUMBER() OVER(ORDER BY WddCode DESC) RowNum FROM x WHERE @typeFilter='A' OR (@typeFilter='Y' AND CurrentStatus IN('Y','A','P')) OR (@typeFilter='N' AND CurrentStatus='W') OR (@typeFilter='C' AND CurrentStatus IN('-','C','N')))
    SELECT RowNum,WddCode,WtmCode,WtmName,CardCode,CardName,OwnerId,DocNo,ObjType,DocType,DocDate,DocDueDate,DocTotal,DocCur,IsDraft,UserId,UserCode,UserName,UpdateDate,UpdateTime,Status,Remarks,RequestType,ItemCheck,ProcesStat,StatusName,ProcessUserId,ProcessUserName,OwnerUserName,UserStatus,CurrentStepRemarks,CurrentStep,UserStatus StepStatus,DocumentStatus,CurrentStatus
    FROM f WHERE @pageIndex=-1 OR (RowNum>20*@pageIndex-20 AND RowNum<=20*@pageIndex) ORDER BY RowNum;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_Approval_GetDetailItem
    @userCode nvarchar(100),@status varchar(10),@requestType char(1)='',@wddEntry int=-1
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @userId int=ISNULL((SELECT TOP(1) USERID FROM dbo.OUSR WHERE USER_CODE=@userCode),-1);
    CREATE TABLE #a(WddCode int,WtmCode int,WtmName nvarchar(254),CardCode nvarchar(15),CardName nvarchar(254),OwnerId int,DocNo varchar(50),ObjType int,DocType varchar(100),DocDate datetime,DocDueDate datetime,DocTotal decimal(21,6),DocCur varchar(10),IsDraft char(1),UserId int,UserCode varchar(10),UserName nvarchar(50),UpdateDate datetime,UpdateTime int,Status char(1),Remarks nvarchar(max),RequestType char(1),ItemCheck char(5),ProcesStat char(5));
    INSERT #a EXEC dbo.APZ_Function_GetApprovalTemplateData @userId,@status,@requestType;
    SELECT a.WddCode,a.WtmCode,a.WtmName,a.CardCode,a.CardName,a.OwnerId,a.DocNo,a.ObjType,a.DocType,CONVERT(varchar(50),a.DocDate,121) DocDate,CONVERT(varchar(50),a.DocDueDate,121) DocDueDate,a.DocTotal,a.DocCur,a.IsDraft,
      ISNULL(p.UserID,a.UserId) UserId,CONVERT(varchar(10),ISNULL(pu.USER_CODE,a.UserCode)) UserCode,CONVERT(nvarchar(50),ISNULL(pu.U_NAME,a.UserName)) UserName,
      CONVERT(varchar(80),ISNULL(p.UpdateDate,a.UpdateDate),121) UpdateDate,ISNULL(p.UpdateTime,a.UpdateTime) UpdateTime,ISNULL(p.Status,a.Status) Status,CONVERT(nvarchar(max),ISNULL(p.Remarks,a.Remarks)) Remarks,a.RequestType,a.ItemCheck,ou.U_NAME OwnerName,a.ProcesStat CurrentStatus,s.Remarks CurrentStepRemarks,p.StepCode CurrentStep
    FROM #a a LEFT JOIN dbo.OUSR ou ON ou.USERID=a.OwnerId
    OUTER APPLY(SELECT TOP(1) w.UserID,w.UpdateDate,w.UpdateTime,w.Status,w.Remarks,w.StepCode FROM dbo.WDD1 w WHERE w.WddCode=a.WddCode ORDER BY w.UpdateDate DESC,w.UpdateTime DESC,w.SortId DESC)p
    LEFT JOIN dbo.OUSR pu ON pu.USERID=p.UserID LEFT JOIN dbo.WTM2 s ON s.WtmCode=a.WtmCode AND s.WstCode=p.StepCode
    WHERE (@wddEntry IS NULL OR @wddEntry=-1 OR a.WddCode=@wddEntry) AND a.IsDraft='Y' ORDER BY a.UpdateDate DESC,a.UpdateTime DESC;
END;
GO
