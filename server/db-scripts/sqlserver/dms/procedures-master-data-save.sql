USE [SBODemoAU];
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_Process_AlertTemplate @type char(1)
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @now int = dbo.GetTimeInteger(CONVERT(time, GETDATE()));
    IF @type = 'A'
        UPDATE #APZ_D_OALT SET LogInstanc=NULL, CreateDate=CONVERT(date,GETDATE()), CreateTime=@now, UpdateDate=NULL, UpdateTime=NULL;
    ELSE
        UPDATE #APZ_D_OALT SET LogInstanc=NULL, UpdateDate=CONVERT(date,GETDATE()), UpdateTime=@now;

    IF EXISTS(SELECT 1 FROM #APZ_D_OALT WHERE ISNULL(Code,N'')=N'')
    BEGIN SELECT '01' ErrorCode,N'template code is empty value' ErrorMessage; RETURN; END;
    IF @type='A' AND EXISTS(SELECT 1 FROM #APZ_D_OALT s JOIN dbo.APZ_D_OALT t ON t.Code=s.Code)
    BEGIN SELECT '05' ErrorCode,N'This template already exists' ErrorMessage; RETURN; END;
    IF @type='U' DELETE t FROM dbo.APZ_D_OALT t JOIN #APZ_D_OALT s ON s.Code=t.Code;
    INSERT INTO dbo.APZ_D_OALT SELECT * FROM #APZ_D_OALT;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_Process_Channel @type char(1)
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @allItem char(1)='N', @now int=dbo.GetTimeInteger(CONVERT(time,GETDATE()));
    IF @type='A'
    BEGIN
        UPDATE #APZ_D_OCHN SET UserSign=1,Loginstanc=NULL,CreateDate=CONVERT(date,GETDATE()),CreateTime=@now,UpdateDate=NULL,UpdateTime=NULL;
        UPDATE #APZ_D_CHN1 SET CreateDate=CONVERT(date,GETDATE()),CreateTime=@now,UpdateDate=NULL,UpdateTime=NULL;
    END
    ELSE
    BEGIN
        UPDATE #APZ_D_OCHN SET UserSign=1,Loginstanc=NULL,UpdateDate=CONVERT(date,GETDATE()),UpdateTime=@now;
        UPDATE #APZ_D_CHN1 SET UpdateDate=CONVERT(date,GETDATE()),UpdateTime=@now;
    END;
    UPDATE #APZ_D_CHN1 SET ChannelCode=(SELECT TOP(1) ChannelCode FROM #APZ_D_OCHN);
    IF EXISTS(SELECT 1 FROM #APZ_D_OCHN WHERE ISNULL(ChannelCode,N'')=N'') BEGIN SELECT '01' ErrorCode,N'Channel code is empty value' ErrorMessage; RETURN; END;
    IF EXISTS(SELECT 1 FROM #APZ_D_OCHN WHERE ISNULL(ChannelName,N'')=N'') BEGIN SELECT '02' ErrorCode,N'Channel Name is empty value' ErrorMessage; RETURN; END;
    IF EXISTS(SELECT 1 FROM #APZ_D_OCHN WHERE AllItem='Y') OR EXISTS(SELECT 1 FROM dbo.APZ_D_OCHN WHERE AllItem='Y')
    BEGIN
        SET @allItem='Y';
        UPDATE dbo.APZ_D_OCHN SET AllItem='N' WHERE ChannelCode<>(SELECT TOP(1) ChannelCode FROM #APZ_D_OCHN);
        DELETE FROM dbo.APZ_D_CHN1 WHERE ISNULL(ItemCode,N'')<>N'';
    END
    ELSE
    BEGIN
        IF EXISTS(SELECT 1 FROM #APZ_D_OCHN WHERE AllItem='N') AND NOT EXISTS(SELECT 1 FROM #APZ_D_CHN1) BEGIN SELECT '03' ErrorCode,N'Item is empty' ErrorMessage; RETURN; END;
        IF EXISTS(SELECT 1 FROM #APZ_D_CHN1 WHERE ISNULL(ItemCode,N'')=N'') BEGIN SELECT '04' ErrorCode,N'Item Code is empty value' ErrorMessage; RETURN; END;
        IF EXISTS(SELECT ItemCode FROM #APZ_D_CHN1 GROUP BY ItemCode HAVING COUNT(*)>1) BEGIN SELECT '06' ErrorCode,N'This item already exists' ErrorMessage; RETURN; END;
    END;
    IF @type='A' AND EXISTS(SELECT 1 FROM #APZ_D_OCHN s JOIN dbo.APZ_D_OCHN t ON t.ChannelCode=s.ChannelCode) BEGIN SELECT '05' ErrorCode,N'This channel already exists' ErrorMessage; RETURN; END;
    IF EXISTS(SELECT 1 FROM #APZ_D_CHN1 s JOIN dbo.APZ_D_CHN1 t ON t.ChannelCode<>s.ChannelCode AND t.ItemCode=s.ItemCode) BEGIN SELECT '07' ErrorCode,N'This item already exist in another channel' ErrorMessage; RETURN; END;
    IF @type='U'
    BEGIN
        DELETE t FROM dbo.APZ_D_CHN1 t WHERE t.ChannelCode=(SELECT TOP(1) ChannelCode FROM #APZ_D_OCHN);
        DELETE t FROM dbo.APZ_D_OCHN t JOIN #APZ_D_OCHN s ON s.ChannelCode=t.ChannelCode;
    END;
    INSERT dbo.APZ_D_OCHN(ChannelCode,ChannelName,Status,AllItem,Remarks,UserSign,Loginstanc,CreateDate,CreateTime,UpdateDate,UpdateTime)
    SELECT ChannelCode,ChannelName,Status,AllItem,Remarks,UserSign,Loginstanc,CreateDate,CreateTime,UpdateDate,UpdateTime FROM #APZ_D_OCHN;
    IF @allItem='N' INSERT dbo.APZ_D_CHN1(ItemCode,ChannelCode,Status,Active,Remarks,CreateDate,CreateTime,UpdateDate,UpdateTime)
    SELECT ItemCode,ChannelCode,Status,Active,Remarks,CreateDate,CreateTime,UpdateDate,UpdateTime FROM #APZ_D_CHN1;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_Process_Geography @type char(1)
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @now int=dbo.GetTimeInteger(CONVERT(time,GETDATE()));
    IF @type='A'
    BEGIN
        UPDATE #APZ_D_OGGP SET UserSign=1,CreateDate=CONVERT(date,GETDATE()),CreateTime=@now,UpdateDate=NULL,UpdateTime=NULL;
        UPDATE #APZ_D_GGP1 SET CreateDate=CONVERT(date,GETDATE()),CreateTime=@now,UpdateDate=NULL,UpdateTime=NULL;
    END
    ELSE
    BEGIN
        UPDATE #APZ_D_OGGP SET UserSign=1,UpdateDate=CONVERT(date,GETDATE()),UpdateTime=@now;
        UPDATE #APZ_D_GGP1 SET UpdateDate=CONVERT(date,GETDATE()),UpdateTime=@now;
    END;
    UPDATE #APZ_D_OGGP SET Status='Y' WHERE ISNULL(Status,'')=''; UPDATE #APZ_D_GGP1 SET Status='Y' WHERE ISNULL(Status,'')='';
    IF EXISTS(SELECT 1 FROM #APZ_D_OGGP WHERE ISNULL(CountryCode,N'')=N'') BEGIN SELECT '02' ErrorCode,N'Country Code is empty' ErrorMessage; RETURN; END;
    IF EXISTS(SELECT 1 FROM #APZ_D_OGGP WHERE ISNULL(CountryName,N'')=N'') BEGIN SELECT '03' ErrorCode,N'Country Name is empty' ErrorMessage; RETURN; END;
    IF EXISTS(SELECT 1 FROM #APZ_D_OGGP WHERE ISNULL(Location,N'')=N'') BEGIN SELECT '04' ErrorCode,N'Region is empty' ErrorMessage; RETURN; END;
    IF EXISTS(SELECT 1 FROM #APZ_D_GGP1 WHERE ISNULL(Code,N'')=N'') BEGIN SELECT '06' ErrorCode,N'Code is empty' ErrorMessage; RETURN; END;
    IF EXISTS(SELECT 1 FROM #APZ_D_GGP1 WHERE ISNULL(Name,N'')=N'') BEGIN SELECT '07' ErrorCode,N'Name is empty' ErrorMessage; RETURN; END;
    IF @type='A' AND EXISTS(SELECT 1 FROM #APZ_D_OGGP s JOIN dbo.APZ_D_OGGP t ON t.CountryCode=s.CountryCode) BEGIN SELECT '09' ErrorCode,N'The country already exists' ErrorMessage; RETURN; END;
    IF EXISTS(SELECT Code FROM #APZ_D_GGP1 GROUP BY Code HAVING COUNT(*)>1) BEGIN SELECT '10' ErrorCode,N'Duplicated Geography details code' ErrorMessage; RETURN; END;
    IF @type='U'
    BEGIN
        DELETE t FROM dbo.APZ_D_GGP1 t JOIN #APZ_D_GGP1 s ON s.CountryCode=t.CountryCode;
        DELETE t FROM dbo.APZ_D_OGGP t JOIN #APZ_D_OGGP s ON s.CountryCode=t.CountryCode;
    END;
    INSERT dbo.APZ_D_OGGP(CountryCode,CountryName,Status,Remarks,Location,UserSign,CreateDate,CreateTime,UpdateDate,UpdateTime)
    SELECT CountryCode,CountryName,Status,Remarks,Location,UserSign,CreateDate,CreateTime,UpdateDate,UpdateTime FROM #APZ_D_OGGP;
    INSERT dbo.APZ_D_GGP1 SELECT * FROM #APZ_D_GGP1;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_Process_SalesRoute @type char(1)
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @routeCode nvarchar(100)=ISNULL((SELECT TOP(1) RouteCode FROM #APZ_D_OSTR),N''), @now int=dbo.GetTimeInteger(CONVERT(time,GETDATE()));
    IF @routeCode=N'' BEGIN SELECT 1 ErrorCode,N'Vui lòng nhập mã tuyến' ErrorMessage; RETURN; END;
    IF @type='A' UPDATE #APZ_D_OSTR SET CreateDate=CONVERT(date,GETDATE()),CreateTime=@now,UpdateDate=NULL,UpdateTime=NULL;
    ELSE UPDATE #APZ_D_OSTR SET UpdateDate=CONVERT(date,GETDATE()),UpdateTime=@now;
    UPDATE #APZ_D_STR1 SET RouteCode=@routeCode; UPDATE #APZ_D_STR2 SET RouteCode=@routeCode;
    UPDATE #APZ_D_STR3 SET RouteCode=@routeCode; UPDATE #APZ_D_STR4 SET RouteCode=@routeCode;
    DELETE FROM dbo.APZ_D_STR4 WHERE RouteCode=@routeCode; DELETE FROM dbo.APZ_D_STR3 WHERE RouteCode=@routeCode;
    DELETE FROM dbo.APZ_D_STR2 WHERE RouteCode=@routeCode; DELETE FROM dbo.APZ_D_STR1 WHERE RouteCode=@routeCode;
    DELETE FROM dbo.APZ_D_OSTR WHERE RouteCode=@routeCode;
    INSERT dbo.APZ_D_OSTR SELECT * FROM #APZ_D_OSTR; INSERT dbo.APZ_D_STR1 SELECT * FROM #APZ_D_STR1;
    INSERT dbo.APZ_D_STR2 SELECT * FROM #APZ_D_STR2 WHERE ISNULL(CardCode,N'')<>N'';
    INSERT dbo.APZ_D_STR3 SELECT * FROM #APZ_D_STR3 WHERE ISNULL(CardCode,N'')<>N'';
    INSERT dbo.APZ_D_STR4 SELECT * FROM #APZ_D_STR4 WHERE ISNULL(EmpId,0)<>0;
    SELECT 0 ErrorCode,N'' ErrorMessage;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_NotificationEvent_Update @type nvarchar(100)=N'update'
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    IF @type=N'add'
    BEGIN
        IF EXISTS(SELECT 1 FROM dbo.APZ_D_EVENTCONFIG t JOIN #tempTable s ON s.EventCode=t.EventCode) BEGIN SELECT N'Duplicate EventCode' Message; RETURN; END;
        INSERT dbo.APZ_D_EVENTCONFIG(EventCode,EventName,AppUse) SELECT TOP(1) EventCode,EventName,AppUse FROM #tempTable;
        INSERT dbo.APZ_D_EVENTAPPCONFIG(EventCode,AppName,Message,Title,Active) SELECT TOP(1) EventCode,AppName,Message,Title,Active FROM #tempTable;
        SELECT N'' Message; RETURN;
    END;
    DECLARE @id int=(SELECT TOP(1) ID FROM #tempTable), @oldCode nvarchar(100)=(SELECT TOP(1) EventCode FROM dbo.APZ_D_EVENTCONFIG WHERE ID=(SELECT TOP(1) ID FROM #tempTable));
    UPDATE target SET EventCode=source.EventCode,EventName=source.EventName,
        AppUse=CASE WHEN EXISTS(SELECT 1 FROM dbo.Split(target.AppUse,N',') p WHERE LOWER(LTRIM(RTRIM(p.DATA)))=LOWER(source.AppName)) THEN target.AppUse ELSE CONCAT(target.AppUse,N',',source.AppName) END
    FROM dbo.APZ_D_EVENTCONFIG target JOIN #tempTable source ON source.ID=target.ID;
    IF ISNULL(@oldCode,N'')=N''
        INSERT dbo.APZ_D_EVENTAPPCONFIG(EventCode,AppName,Message,Title,Active) SELECT TOP(1) EventCode,AppName,Message,Title,Active FROM #tempTable;
    ELSE
        UPDATE target SET EventCode=source.EventCode,Active=source.Active,AppName=source.AppName,Message=source.Message,Title=source.Title
        FROM dbo.APZ_D_EVENTAPPCONFIG target CROSS JOIN (SELECT TOP(1)* FROM #tempTable) source
        WHERE target.EventCode=@oldCode AND LOWER(target.AppName)=LOWER(source.AppName);
    SELECT N'' Message;
END;
GO

CREATE OR ALTER PROCEDURE dbo.Apz_D_SaveEventListenerApp
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    UPDATE target SET Pending=COALESCE(NULLIF(source.Pending,''),target.Pending),Success=COALESCE(NULLIF(source.Success,''),target.Success),
        Retry=COALESCE(source.Retry,target.Retry),UpdateDate=COALESCE(source.UpdateDate,CONVERT(date,GETDATE())),
        [Read]=COALESCE(NULLIF(source.[Read],''),target.[Read]),IsRemove=COALESCE(NULLIF(source.IsRemove,''),target.IsRemove)
    FROM dbo.APZ_D_ELN1 target JOIN #APZ_D_ELN1 source ON source.ID=target.ID;
    INSERT dbo.APZ_D_ELN1(EventCode,Title,Message,Image,Data,DataExtra,SendUser,SendAll,Pending,Success,Retry,Method,CreateDate,UpdateDate,[Read],Note,DatabaseName,IsRemove)
    SELECT source.EventCode,source.Title,source.Message,source.Image,source.Data,source.DataExtra,source.SendUser,source.SendAll,source.Pending,source.Success,source.Retry,source.Method,source.CreateDate,source.UpdateDate,source.[Read],source.Note,source.DatabaseName,source.IsRemove
    FROM #APZ_D_ELN1 source WHERE NOT EXISTS(SELECT 1 FROM dbo.APZ_D_ELN1 target WHERE target.ID=source.ID);
    SELECT MAX(ID) FROM dbo.APZ_D_ELN1;
END;
GO

CREATE OR ALTER PROCEDURE dbo.Apz_D_OutletVisitingFrequency_SaveProcess
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    IF NOT EXISTS(SELECT 1 FROM #APZ_D_OOVF) BEGIN SELECT 404 ErrorCode,N'Temp data is not exist' ErrorMessage; RETURN; END;
    UPDATE target SET Name=source.Name,Address=source.Address,Description=source.Description,StartDate=source.StartDate,Finished=source.Finished,
      Recurring=source.Recurring,RecurNum=source.RecurNum,RecurNote=source.RecurNote,EveryDay=source.EveryDay,DayOfWeek=source.DayOfWeek,
      Mon=source.Mon,Tue=source.Tue,Wed=source.Wed,Thu=source.Thu,Fri=source.Fri,Sat=source.Sat,Sun=source.Sun,DayOfMonth=source.DayOfMonth,
      ListOfDate=source.ListOfDate,UserSign=source.UserSign,UpdateDate=source.UpdateDate,LogInstanc=source.LogInstanc,NumberOfVisits=source.NumberOfVisits
    FROM dbo.APZ_D_OOVF target JOIN #APZ_D_OOVF source ON source.Code=target.Code;
    INSERT dbo.APZ_D_OOVF SELECT source.* FROM #APZ_D_OOVF source WHERE NOT EXISTS(SELECT 1 FROM dbo.APZ_D_OOVF target WHERE target.Code=source.Code);
    SELECT TOP(1) Code FROM #APZ_D_OOVF;
END;
GO
