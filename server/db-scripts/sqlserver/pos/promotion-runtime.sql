USE [SBODemoAU];
GO

IF OBJECT_ID(N'dbo.APZ_D_PMO1',N'U') IS NULL
BEGIN
  CREATE TABLE dbo.APZ_D_PMO1
  (
    Code nvarchar(50) NOT NULL,Id int NOT NULL,FatherId int NULL,[Type] nvarchar(10) NULL,
    ConditionType char(10) NULL,ConditionCode nvarchar(50) NULL,ConditionName nvarchar(254) NULL,
    Remarks nvarchar(500) NULL,GroupCode nvarchar(50) NULL,PackId int NULL,
    CONSTRAINT PK_APZ_D_PMO1 PRIMARY KEY(Code,Id)
  );
END;
GO

IF OBJECT_ID(N'dbo.APZ_D_PMO2',N'U') IS NULL
BEGIN
  CREATE TABLE dbo.APZ_D_PMO2
  (
    Code nvarchar(50) NOT NULL,GroupId int NOT NULL,Id int NOT NULL,FatherId int NULL,[Type] nvarchar(10) NULL,
    BenefitType char(10) NULL,BenefitCode nvarchar(50) NULL,BenefitName nvarchar(254) NULL,
    Remarks nvarchar(500) NULL,PackId int NULL,GroupCode nvarchar(254) NULL,
    CONSTRAINT PK_APZ_D_PMO2 PRIMARY KEY(Code,Id)
  );
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_Retail_GetPromotions
AS
BEGIN
  SET NOCOUNT ON;
  DECLARE @cardCode nvarchar(50)=(SELECT TOP(1) CardCode COLLATE DATABASE_DEFAULT FROM #ORDR);

  /*
    POS consumes one flat list. Only active, in-date programs are returned. A registered
    customer restriction is honoured when registrations exist; programs without a
    registration remain generally available. The recovered promotion editor stores the
    actual benefit rows in APZ_D_PMO2.
  */
  SELECT
    p.Name TenCT,CONVERT(nvarchar(50),b.GroupId) Grp,b.GroupCode GroupCT,
    b.BenefitCode MaHang,
    COALESCE(NULLIF(b.BenefitName,N''),i.ItemName,b.Remarks,p.BenefitDesc) TenHang,
    CASE WHEN TRY_CONVERT(int,b.BenefitType)=3 THEN CONVERT(decimal(19,6),1) ELSE CONVERT(decimal(19,6),0) END SL,
    i.InvntryUom DVT,i.CodeBars,
    CONVERT(decimal(19,6),COALESCE(linePrice.Price,0)) Price,
    CONVERT(decimal(19,6),CASE WHEN TRY_CONVERT(int,b.BenefitType) IN(1,2) THEN COALESCE(TRY_CONVERT(decimal(19,6),b.Remarks),0) ELSE 0 END) ChietKhau,
    TRY_CONVERT(int,b.BenefitType) BenefitType,p.Code PromotionCode,b.Id BenefitId
  FROM dbo.APZ_D_OPMO p
  JOIN dbo.APZ_D_PMO2 b ON b.Code=p.Code
  LEFT JOIN dbo.OITM i ON i.ItemCode=b.BenefitCode
  OUTER APPLY(SELECT TOP(1) r.Price FROM #RDR1 r WHERE r.ItemCode COLLATE DATABASE_DEFAULT=b.BenefitCode) linePrice
  WHERE ISNULL(p.Status,N'') NOT IN(N'N',N'C')
    AND (p.FromDate IS NULL OR p.FromDate<=GETDATE())
    AND (p.ToDate IS NULL OR p.ToDate>=CONVERT(date,GETDATE()))
    AND (NOT EXISTS(SELECT 1 FROM dbo.APZ_D_PMO3 reg WHERE reg.Code=p.Code)
      OR EXISTS(SELECT 1 FROM dbo.APZ_D_PMO3 reg WHERE reg.Code=p.Code AND reg.CardCode=@cardCode AND ISNULL(reg.StatusCode,'A')<>'N'))
  ORDER BY p.Code,b.GroupId,b.Id;
END;
GO
