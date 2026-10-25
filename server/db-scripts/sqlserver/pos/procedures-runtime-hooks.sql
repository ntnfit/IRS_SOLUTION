USE [SBODemoAU];
GO

CREATE OR ALTER PROCEDURE dbo.SP_APZ_STORE_SAVE
AS
BEGIN
  SET NOCOUNT ON; SET XACT_ABORT ON;
  IF NOT EXISTS(SELECT 1 FROM #APZ_STORE WHERE NULLIF(Code,N'') IS NOT NULL)
  BEGIN SELECT N'Store code is required' ErrorMessage; RETURN; END;

  MERGE dbo.APZ_STORE AS target
  USING (SELECT * FROM #APZ_STORE) AS source ON target.Code=source.Code COLLATE DATABASE_DEFAULT
  WHEN MATCHED THEN UPDATE SET Dscription=source.Dscription,BPLId=source.BPLId,WhsCode=source.WhsCode,
    Address=source.Address,Email=source.Email,CardCode=source.CardCode,SlpCode=source.SlpCode,ListNum=source.ListNum,
    UserSign=source.UserSign,UpdateDate=source.UpdateDate,UserSign2=source.UserSign2,Locked=source.Locked,
    DataSource=source.DataSource,LogInstanc=source.LogInstanc,OSKeyBoardEnable=source.OSKeyBoardEnable
  WHEN NOT MATCHED THEN INSERT(StoreID,Code,Dscription,BPLId,WhsCode,Address,Email,CardCode,SlpCode,ListNum,UserSign,UpdateDate,CreateDate,UserSign2,Locked,DataSource,LogInstanc,OSKeyBoardEnable)
    VALUES(CASE WHEN ISNULL(source.StoreID,0)=0 THEN (SELECT ISNULL(MAX(s.StoreID),0)+1 FROM dbo.APZ_STORE s) ELSE source.StoreID END,
      source.Code,source.Dscription,source.BPLId,source.WhsCode,source.Address,source.Email,source.CardCode,source.SlpCode,source.ListNum,source.UserSign,source.UpdateDate,source.CreateDate,source.UserSign2,source.Locked,source.DataSource,source.LogInstanc,source.OSKeyBoardEnable);
  SELECT TOP(1) Code FROM #APZ_STORE;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_APZ_STORE_REMOVE @Code nvarchar(50)
AS
BEGIN
  SET NOCOUNT ON;
  IF EXISTS(SELECT 1 FROM dbo.ORDR WHERE U_RetailStore=@Code)
  BEGIN SELECT N'You cannot remove this Store; linked transactions exist' ErrorMessage; RETURN; END;
  DELETE FROM dbo.APZ_RPOS WHERE Store=@Code;
  DELETE FROM dbo.APZ_STORE WHERE Code=@Code;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_Retail_PosMaster_REMOVE @PosID nvarchar(50)
AS
BEGIN
  SET NOCOUNT ON;
  IF EXISTS(SELECT 1 FROM dbo.ORDR WHERE U_Posid=TRY_CONVERT(int,@PosID))
  BEGIN SELECT N'You cannot remove this POS; linked transactions exist' ErrorMessage; RETURN; END;
  DELETE FROM dbo.APZ_RPOS WHERE PosID=TRY_CONVERT(int,@PosID);
END;
GO

CREATE OR ALTER PROCEDURE dbo.apz_retail_beforeaction @obj_id nvarchar(50),@transtype char(1)
AS
BEGIN
  SET NOCOUNT ON;
  IF TRY_CONVERT(int,@obj_id) IS NULL SELECT 1 ErrorCode,N'Invalid retail document id' ErrorMessage;
  ELSE IF @transtype NOT IN('A','U','D','C') SELECT 2 ErrorCode,N'Invalid transaction type' ErrorMessage;
  ELSE SELECT 0 ErrorCode,N'' ErrorMessage;
END;
GO

CREATE OR ALTER PROCEDURE dbo.apz_retail_afteraction @obj_id nvarchar(50),@transtype char(1)
AS
BEGIN
  SET NOCOUNT ON;
  IF TRY_CONVERT(int,@obj_id) IS NULL SELECT 1 ErrorCode,N'Invalid retail document id' ErrorMessage;
  ELSE IF @transtype NOT IN('A','U','D','C') SELECT 2 ErrorCode,N'Invalid transaction type' ErrorMessage;
  ELSE SELECT 0 ErrorCode,N'' ErrorMessage;
END;
GO
