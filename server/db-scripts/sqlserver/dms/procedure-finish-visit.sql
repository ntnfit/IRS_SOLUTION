USE [SBODemoAU];
GO

CREATE OR ALTER PROCEDURE dbo.APZ_VisitStore_FinishVisitStore
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 DECLARE @guid nvarchar(200)=(SELECT TOP(1) Guid FROM #tempDoc),@docEntry int,@userId int,
         @storeCode nvarchar(50)=(SELECT TOP(1) StoreCode FROM #tempDoc),@storeName nvarchar(254),@storeAddress nvarchar(500),
         @geo nvarchar(1000)=COALESCE((SELECT TOP(1) GeoLocationFinish FROM #tempDoc),(SELECT TOP(1) GeoLocation FROM #tempDoc));
 IF EXISTS(SELECT 1 FROM dbo.APZ_D_VISIT WHERE Guid=@guid)
 BEGIN SELECT '1470000161' ObjectType,403 Code,CONCAT(N'Duplicate Guid ',ISNULL(@guid,N'')) Message; RETURN; END;
 SELECT TOP(1) @storeName=CardName,@storeAddress=Address FROM dbo.OCRD WHERE CardCode=@storeCode;
 SELECT TOP(1) @userId=USERID
 FROM dbo.OUSR
 WHERE USER_CODE=(SELECT TOP(1) UserCode COLLATE DATABASE_DEFAULT FROM #tempDoc);
 SELECT @docEntry=ISNULL(MAX(DocEntry),0)+1 FROM dbo.APZ_D_VISIT WITH(UPDLOCK,HOLDLOCK);

 INSERT dbo.APZ_D_VISIT(DocEntry,Guid,UserSign,StoreCode,StoreName,Address,CreateDate,CreateTime,LogInstance,StartDate,StartTime,FinishedDate,FinishedTime,Geolocation)
 SELECT TOP(1) @docEntry,Guid,@userId,StoreCode,@storeName,@storeAddress,CONVERT(date,GETDATE()),CONVERT(time,GETDATE()),NULL,
        StartDate,CONVERT(time,StartDate),FinishedDate,CONVERT(time,FinishedDate),@geo FROM #tempDoc;
 IF @@ROWCOUNT=0 BEGIN SELECT '1470000161' ObjectType,406 Code,N'Insert table header fail' Message; RETURN; END;

 UPDATE #tempLine SET DocEntry=@docEntry,Guid=@guid,CreateDate=CONVERT(date,GETDATE()),CreateTime=CONVERT(time,GETDATE());
 UPDATE #tempLine SET Geolocation=@geo WHERE StepCode<>-1;
 INSERT dbo.APZ_D_VISIT1 SELECT * FROM #tempLine;
 IF @@ROWCOUNT=0 BEGIN SELECT '1470000161' ObjectType,406 Code,N'Insert table line fail' Message; RETURN; END;

 IF EXISTS(SELECT 1 FROM #tempUpdateInventory)
 BEGIN
   INSERT dbo.APZ_D_OSTC(Code,CountDate,CountTime,StatusCode,CreateDate,CreateTime,UserSign,UserSign2,UpdateDate,UpdateTime)
   SELECT TOP(1) Guid,StartDate,CONVERT(time,StartDate),'O',CONVERT(date,GETDATE()),CONVERT(time,GETDATE()),@userId,NULL,NULL,NULL FROM #tempDoc;
   IF @@ROWCOUNT=0 BEGIN SELECT '1470000161' ObjectType,406 Code,N'Insert data to table APZ_D_OSTC fail' Message; RETURN; END;
   INSERT dbo.APZ_D_STC1(Code,LineNum,ItemCode,Dscription,Quantity)
   SELECT @guid,LineNum,ItemCode,ItemName,TRY_CONVERT(int,REPLACE(NULLIF(Quantity,N''),N',',N'')) FROM #tempUpdateInventory;
   IF @@ROWCOUNT=0 BEGIN SELECT '1470000161' ObjectType,406 Code,N'Insert data to table APZ_D_STC1 fail' Message; RETURN; END;
 END;

 IF EXISTS(SELECT 1 FROM #tempDocCompetitor)
 BEGIN
   INSERT dbo.APZ_D_OCPA(Code,VisitCode,StoreCode,CountDate,CountTime,StatusCode,CreateDate,CreateTime,UserSign,UserSign2,UpdateDate,UpdateTime,CompetitorName,Notes)
   SELECT Code,VisitCode,StoreCode,CountDate,CONVERT(time,CountDate),StatusCode,CONVERT(date,GETDATE()),CONVERT(time,GETDATE()),UserSign,UserSign2,UpdateDate,CONVERT(time,UpdateDate),CompetitorName,Notes FROM #tempDocCompetitor;
   IF @@ROWCOUNT=0 BEGIN SELECT '1470000161' ObjectType,406 Code,N'Insert data to table APZ_D_OCPA fail' Message; RETURN; END;
   INSERT dbo.APZ_D_CPA1(Code,VisitCode,LineNum,ItemName,IsAvailable) SELECT Code,VisitCode,LineNum,ItemName,IsAvailable FROM #tempLineCompetitor;
 END;
 SELECT '1470000161' ObjectType,200 Code,N'' Message;
END;
GO
