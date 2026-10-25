USE [SBODemoAU];
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_Delivery_GetLines @docEntry int
AS
BEGIN
  SET NOCOUNT ON;
  SELECT * FROM dbo.APZ_D_DLO1 WHERE DocEntry=@docEntry ORDER BY ItemCode;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_Delivery_GetListHistory
  @fromDate nvarchar(100)=NULL,@toDate nvarchar(100)=NULL,@userCode nvarchar(100)=N'',@pageIndex int=1
AS
BEGIN
  SET NOCOUNT ON;
  DECLARE @from date=TRY_CONVERT(date,@fromDate),@to date=TRY_CONVERT(date,@toDate),@userId nvarchar(100);
  SELECT TOP(1) @userId=CONVERT(nvarchar(100),USERID) FROM dbo.OUSR WHERE USER_CODE=@userCode;
  ;WITH filtered AS
  (
    SELECT ROW_NUMBER() OVER(ORDER BY d.ScheduledDate DESC,d.DocEntry DESC) RowNum,d.*
    FROM dbo.APZ_D_ODLO d
    WHERE (@from IS NULL OR d.ScheduledDate>=@from)
      AND (@to IS NULL OR d.ScheduledDate<DATEADD(day,1,@to))
      AND (ISNULL(@userCode,N'')=N'' OR d.UserSign COLLATE DATABASE_DEFAULT IN(@userCode,@userId))
      AND (NOT EXISTS(SELECT 1 FROM #tbl) OR EXISTS(SELECT 1 FROM #tbl f WHERE f.CardCode COLLATE DATABASE_DEFAULT=d.CardCode))
      AND (NOT EXISTS(SELECT 1 FROM #tblStatus) OR EXISTS(SELECT 1 FROM #tblStatus f WHERE f.Status COLLATE DATABASE_DEFAULT=d.Status))
  )
  SELECT * FROM filtered WHERE @pageIndex=-1 OR (RowNum>20*@pageIndex-20 AND RowNum<=20*@pageIndex) ORDER BY RowNum;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_Delivery_SaveProcess @type char(1)
AS
BEGIN
  SET NOCOUNT ON; SET XACT_ABORT ON;
  DECLARE @docEntry int=(SELECT TOP(1) DocEntry FROM #APZ_D_ODLO);
  IF @type NOT IN('A','U','D') BEGIN SELECT -1 Result,N'Invalid transaction type' Message; RETURN; END;
  IF @type<>'D' AND NOT EXISTS(SELECT 1 FROM #APZ_D_ODLO) BEGIN SELECT -1 Result,N'Delivery header is required' Message; RETURN; END;

  IF @type='A'
  BEGIN
    SELECT @docEntry=ISNULL(MAX(DocEntry),0)+1 FROM dbo.APZ_D_ODLO WITH(UPDLOCK,HOLDLOCK);
    UPDATE #APZ_D_ODLO SET DocEntry=@docEntry;
    UPDATE #APZ_D_DLO1 SET DocEntry=@docEntry;
  END;

  IF @type IN('U','D')
  BEGIN
    DELETE FROM dbo.APZ_D_DLO1 WHERE DocEntry=@docEntry;
    DELETE FROM dbo.APZ_D_ODLO WHERE DocEntry=@docEntry;
  END;

  IF @type<>'D'
  BEGIN
    INSERT dbo.APZ_D_ODLO SELECT * FROM #APZ_D_ODLO;
    INSERT dbo.APZ_D_DLO1 SELECT * FROM #APZ_D_DLO1;
  END;
  SELECT @docEntry Result,N'' Message;
END;
GO
