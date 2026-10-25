USE [SBODemoAU];
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_Geography_GetRecord @position nvarchar(100),@countryCode nvarchar(50)
AS
BEGIN
  SET NOCOUNT ON;
  SELECT g.* FROM dbo.APZ_D_GGP1 g
  WHERE g.CountryCode=@countryCode AND (ISNULL(@position,N'')=N'' OR g.[Type]=@position OR g.Code=@position)
  ORDER BY g.Id,g.Code;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_Geography_CheckExists @code nvarchar(100),@isCountry int
AS
BEGIN
  SET NOCOUNT ON;
  IF @isCountry=1
    SELECT o.CountryCode Code,o.CountryName Name,
      (SELECT COUNT(*) FROM dbo.APZ_D_GGP1 g WHERE g.CountryCode=o.CountryCode) ReferenceCount
    FROM dbo.APZ_D_OGGP o WHERE o.CountryCode=@code;
  ELSE
    SELECT g.Code,g.Name,
      (SELECT COUNT(*) FROM dbo.APZ_D_GGP1 c WHERE c.FatherCode=g.Code) ReferenceCount
    FROM dbo.APZ_D_GGP1 g WHERE g.Code=@code;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_OutletVisitingFrequency_GetData @userName nvarchar(100),@branch int
AS
BEGIN
  SET NOCOUNT ON;
  DECLARE @userId int=(SELECT TOP(1) USERID FROM dbo.OUSR WHERE USER_CODE=@userName);
  SELECT f.* FROM dbo.APZ_D_OOVF f
  WHERE ISNULL(@userName,N'')=N'' OR f.UserSign=@userId OR f.UserSign IS NULL
  ORDER BY f.Name,f.Code;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_SettingReport_GetRowByRowId @docEntry int,@rowId int,@fatherNum int=-1
AS
BEGIN
  SET NOCOUNT ON;
  SELECT * FROM dbo.APZ_D_SRT2
  WHERE DocEntry=@docEntry AND Id=@rowId AND (@fatherNum=-1 OR FatherNum=@fatherNum)
  ORDER BY Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_HistoryVisit @fromDate nvarchar(100)=NULL,@toDate nvarchar(100)=NULL,@userSign int=-1,@pageIndex int=1
AS
BEGIN
  SET NOCOUNT ON;
  DECLARE @from date=TRY_CONVERT(date,@fromDate),@to date=TRY_CONVERT(date,@toDate);
  ;WITH visits AS
  (
    SELECT ROW_NUMBER() OVER(ORDER BY v.FinishedDate DESC,v.DocEntry DESC) RowNum,v.*,
      CONCAT(ISNULL(e.firstName,N''),N' ',ISNULL(e.middleName,N''),N' ',ISNULL(e.lastName,N'')) U_fullName,
      (SELECT COUNT(*) FROM dbo.ORDR o WHERE o.CardCode=v.StoreCode AND o.U_GUID=v.Guid) OrderNumber
    FROM dbo.APZ_D_VISIT v LEFT JOIN dbo.OHEM e ON e.userId=v.UserSign
    WHERE (@from IS NULL OR v.FinishedDate>=@from) AND (@to IS NULL OR v.FinishedDate<DATEADD(day,1,@to))
      AND (@userSign=-1 OR v.UserSign=@userSign)
      AND (NOT EXISTS(SELECT 1 FROM #tbl) OR EXISTS(SELECT 1 FROM #tbl f WHERE f.CardCode COLLATE DATABASE_DEFAULT=v.StoreCode))
  )
  SELECT * FROM visits WHERE @pageIndex=-1 OR (RowNum>20*@pageIndex-20 AND RowNum<=20*@pageIndex) ORDER BY RowNum;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_StoreImageAlbum_GetList
  @fromDate nvarchar(100)=NULL,@toDate nvarchar(100)=NULL,@userSign int=-1,@pageIndex int=1,@typeAttach nvarchar(100)=N'',@empId int=-1
AS
BEGIN
  SET NOCOUNT ON;
  DECLARE @from date=TRY_CONVERT(date,@fromDate),@to date=TRY_CONVERT(date,@toDate);
  ;WITH images AS
  (
    SELECT ROW_NUMBER() OVER(ORDER BY i.DateCreated DESC,i.Code DESC) RowNum,i.*,a.Name AlbumName
    FROM dbo.APZ_D_UPIM i LEFT JOIN dbo.APZ_D_IMAGEALBUM a ON a.Id=i.AlbumId
    WHERE (@from IS NULL OR i.DateCreated>=@from) AND (@to IS NULL OR i.DateCreated<DATEADD(day,1,@to))
      AND (@userSign=-1 OR i.UserSign=@userSign) AND (@empId=-1 OR i.UserSign=@empId)
      AND (NOT EXISTS(SELECT 1 FROM #tbl) OR EXISTS(SELECT 1 FROM #tbl f WHERE f.CardCode COLLATE DATABASE_DEFAULT=i.CardCode))
      AND (NOT EXISTS(SELECT 1 FROM #tblAlbum) OR EXISTS(SELECT 1 FROM #tblAlbum f WHERE TRY_CONVERT(int,f.Id)=i.AlbumId))
  )
  SELECT * FROM images WHERE @pageIndex=-1 OR (RowNum>20*@pageIndex-20 AND RowNum<=20*@pageIndex) ORDER BY RowNum;
END;
GO
