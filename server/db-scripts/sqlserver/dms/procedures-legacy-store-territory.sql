USE [SBODemoAU];
GO

-- The recovered client contains an older Sale Territory screen whose dataset
-- contract predates the current APZ_D_OSTR/APZ_D_STR* Sales Route schema. Keep
-- that contract in dedicated tables so the legacy screen cannot corrupt routes.
IF OBJECT_ID(N'dbo.APZ_D_STORE', N'U') IS NULL
CREATE TABLE dbo.APZ_D_STORE
(
    Code nvarchar(50) NOT NULL CONSTRAINT PK_APZ_D_STORE PRIMARY KEY,
    Name nvarchar(254) NULL, Region nvarchar(100) NULL, City nvarchar(100) NULL,
    District nvarchar(100) NULL, Ward nvarchar(100) NULL, Town nvarchar(100) NULL,
    CardCode nvarchar(50) NULL, Status char(1) NULL, Address nvarchar(500) NULL,
    GgpCode nvarchar(50) NULL, Remarks nvarchar(500) NULL, UserSign int NULL,
    CreateDate datetime NULL, CreateTime int NULL, UpdateDate datetime NULL, UpdateTime int NULL
);
GO
IF OBJECT_ID(N'dbo.APZ_D_STREV', N'U') IS NULL
CREATE TABLE dbo.APZ_D_STREV
(
    Code nvarchar(50) NOT NULL, ClgCode int NOT NULL, Remarks nvarchar(500) NULL,
    CONSTRAINT PK_APZ_D_STREV PRIMARY KEY (Code, ClgCode)
);
GO
IF OBJECT_ID(N'dbo.APZ_D_LEGACY_OSTR', N'U') IS NULL
CREATE TABLE dbo.APZ_D_LEGACY_OSTR
(
    Code nvarchar(50) NOT NULL CONSTRAINT PK_APZ_D_LEGACY_OSTR PRIMARY KEY,
    Name nvarchar(254) NULL, CountryCode nvarchar(50) NULL, [Type] char(1) NULL,
    Status char(1) NULL, Remarks nvarchar(500) NULL, UserSign int NULL,
    CreateDate datetime NULL, CreateTime int NULL
);
GO
IF OBJECT_ID(N'dbo.APZ_D_LEGACY_STR1', N'U') IS NULL
CREATE TABLE dbo.APZ_D_LEGACY_STR1
(
    Code nvarchar(50) NOT NULL, RegionCode nvarchar(50) NOT NULL,
    Name nvarchar(254) NULL, GgpCode nvarchar(50) NULL, FCode nvarchar(50) NULL,
    Status char(1) NULL, Remarks nvarchar(500) NULL, Id int NOT NULL, FatherId int NULL,
    CONSTRAINT PK_APZ_D_LEGACY_STR1 PRIMARY KEY (Code, RegionCode)
);
GO
IF OBJECT_ID(N'dbo.APZ_D_LEGACY_STR2', N'U') IS NULL
CREATE TABLE dbo.APZ_D_LEGACY_STR2
(
    Code nvarchar(50) NOT NULL, RegionCode nvarchar(50) NOT NULL,
    GgpCode nvarchar(50) NOT NULL, GgpCode1 nvarchar(50) NULL, Remarks nvarchar(500) NULL,
    CONSTRAINT PK_APZ_D_LEGACY_STR2 PRIMARY KEY (Code, RegionCode, GgpCode)
);
GO
IF OBJECT_ID(N'dbo.APZ_D_LEGACY_STR3', N'U') IS NULL
CREATE TABLE dbo.APZ_D_LEGACY_STR3
(
    Code nvarchar(50) NOT NULL, StrCode nvarchar(50) NOT NULL, GgpCode nvarchar(50) NULL,
    Remarks nvarchar(500) NULL,
    CONSTRAINT PK_APZ_D_LEGACY_STR3 PRIMARY KEY (Code, StrCode)
);
GO

CREATE OR ALTER PROCEDURE dbo.APZ_Store_GetData
    @storeCode nvarchar(50)=N'', @cardCode nvarchar(50)=N'', @status nvarchar(10)=N''
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM dbo.APZ_D_STORE
    WHERE (@storeCode=N'' OR Code=@storeCode) AND (@cardCode=N'' OR CardCode=@cardCode)
      AND (@status=N'' OR Status=@status) ORDER BY Name,Code;
END;
GO
CREATE OR ALTER PROCEDURE dbo.APZ_Store_GetRecord @position nvarchar(100), @storeCode nvarchar(50)
AS
BEGIN
    SET NOCOUNT ON;
    ;WITH records AS (SELECT ROW_NUMBER() OVER (ORDER BY Code) RowNum,* FROM dbo.APZ_D_STORE),
    current_record AS (SELECT RowNum FROM records WHERE Code=@storeCode)
    SELECT records.* FROM records CROSS JOIN current_record
    WHERE records.RowNum=CASE @position WHEN N'F' THEN 1 WHEN N'L' THEN (SELECT MAX(RowNum) FROM records)
        WHEN N'P' THEN current_record.RowNum-1 WHEN N'N' THEN current_record.RowNum+1 ELSE current_record.RowNum END;
END;
GO
CREATE OR ALTER PROCEDURE dbo.APZ_Store_CheckRemove @code nvarchar(50)
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS(SELECT 1 FROM dbo.APZ_D_LEGACY_STR3 WHERE StrCode=@code)
        SELECT 1 ErrorCode,N'Store is assigned to a sales territory' ErrorMessage;
END;
GO
CREATE OR ALTER PROCEDURE dbo.APZ_Store_Remove @storeCode nvarchar(50)
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.APZ_D_STREV WHERE Code=@storeCode;
    DELETE FROM dbo.APZ_D_LEGACY_STR3 WHERE StrCode=@storeCode;
    DELETE FROM dbo.APZ_D_STORE WHERE Code=@storeCode;
END;
GO
CREATE OR ALTER PROCEDURE dbo.APZ_Process_Store @type char(1)
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @code nvarchar(50)=(SELECT TOP (1) Code FROM #APZ_D_STORE);
    IF NULLIF(@code,N'') IS NULL BEGIN SELECT 1 ErrorCode,N'Store code is required' ErrorMessage; RETURN; END;
    IF @type='A' AND EXISTS(SELECT 1 FROM dbo.APZ_D_STORE WHERE Code=@code)
        BEGIN SELECT 2 ErrorCode,N'Store code already exists' ErrorMessage; RETURN; END;
    DELETE FROM dbo.APZ_D_STREV WHERE Code=@code; DELETE FROM dbo.APZ_D_STORE WHERE Code=@code;
    INSERT dbo.APZ_D_STORE (Code,Name,Region,City,District,Ward,Town,CardCode,Status,Address,GgpCode,Remarks,UserSign,CreateDate,CreateTime,UpdateDate,UpdateTime)
        SELECT Code,Name,Region,City,District,Ward,Town,CardCode,Status,Address,GgpCode,Remarks,UserSign,CreateDate,CreateTime,UpdateDate,UpdateTime FROM #APZ_D_STORE;
    INSERT dbo.APZ_D_STREV (Code,ClgCode,Remarks) SELECT Code,ClgCode,Remarks FROM #APZ_D_STREV;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_SaleTerritory_GetData
    @territoryCode nvarchar(50)=N'', @location nvarchar(254)=N'', @status nvarchar(10)=N''
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Code,Name,CountryCode,[Type],Status,Remarks,UserSign,CreateDate,CreateTime
    FROM dbo.APZ_D_LEGACY_OSTR
    WHERE (@territoryCode=N'' OR Code=@territoryCode)
      AND (@location=N'' OR Name LIKE N'%'+@location+N'%' OR CountryCode=@location)
      AND (@status=N'' OR Status=@status) ORDER BY Name,Code;
END;
GO
CREATE OR ALTER PROCEDURE dbo.APZ_SaleTerritory_GetRecord @position nvarchar(100), @territoryCode nvarchar(50)
AS
BEGIN
    SET NOCOUNT ON;
    ;WITH records AS (SELECT ROW_NUMBER() OVER (ORDER BY Code) RowNum,* FROM dbo.APZ_D_LEGACY_OSTR),
    current_record AS (SELECT RowNum FROM records WHERE Code=@territoryCode)
    SELECT records.* FROM records CROSS JOIN current_record
    WHERE records.RowNum=CASE @position WHEN N'F' THEN 1 WHEN N'L' THEN (SELECT MAX(RowNum) FROM records)
        WHEN N'P' THEN current_record.RowNum-1 WHEN N'N' THEN current_record.RowNum+1 ELSE current_record.RowNum END;
END;
GO
CREATE OR ALTER PROCEDURE dbo.APZ_SaleTerritory_Remove @territoryCode nvarchar(50)
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.APZ_D_LEGACY_STR3 WHERE Code=@territoryCode;
    DELETE FROM dbo.APZ_D_LEGACY_STR2 WHERE Code=@territoryCode;
    DELETE FROM dbo.APZ_D_LEGACY_STR1 WHERE Code=@territoryCode;
    DELETE FROM dbo.APZ_D_LEGACY_OSTR WHERE Code=@territoryCode;
END;
GO
CREATE OR ALTER PROCEDURE dbo.APZ_Process_SaleTerritory @type char(1)
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @code nvarchar(50)=(SELECT TOP (1) Code FROM #APZ_D_OSTR);
    IF NULLIF(@code,N'') IS NULL BEGIN SELECT 1 ErrorCode,N'Territory code is required' ErrorMessage; RETURN; END;
    IF @type='A' AND EXISTS(SELECT 1 FROM dbo.APZ_D_LEGACY_OSTR WHERE Code=@code)
        BEGIN SELECT 2 ErrorCode,N'Territory code already exists' ErrorMessage; RETURN; END;
    DELETE FROM dbo.APZ_D_LEGACY_STR3 WHERE Code=@code; DELETE FROM dbo.APZ_D_LEGACY_STR2 WHERE Code=@code;
    DELETE FROM dbo.APZ_D_LEGACY_STR1 WHERE Code=@code; DELETE FROM dbo.APZ_D_LEGACY_OSTR WHERE Code=@code;
    INSERT dbo.APZ_D_LEGACY_OSTR (Code,Name,CountryCode,[Type],Status,Remarks,UserSign,CreateDate,CreateTime)
        SELECT Code,Name,CountryCode,[Type],Status,Remarks,UserSign,CreateDate,CreateTime FROM #APZ_D_OSTR;
    INSERT dbo.APZ_D_LEGACY_STR1 (Code,RegionCode,Name,GgpCode,FCode,Status,Remarks,Id,FatherId)
        SELECT Code,RegionCode,Name,GgpCode,FCode,Status,Remarks,Id,FatherId FROM #APZ_D_STR1;
    INSERT dbo.APZ_D_LEGACY_STR2 (Code,RegionCode,GgpCode,GgpCode1,Remarks)
        SELECT Code,RegionCode,GgpCode,GgpCode1,Remarks FROM #APZ_D_STR2 WHERE NULLIF(GgpCode,N'') IS NOT NULL;
    INSERT dbo.APZ_D_LEGACY_STR3 (Code,StrCode,GgpCode,Remarks)
        SELECT Code,StrCode,NULL,Remarks FROM #APZ_D_STR3 WHERE NULLIF(StrCode,N'') IS NOT NULL;
END;
GO
