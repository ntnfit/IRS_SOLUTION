USE [SBODemoAU];
GO

-- DMS upload-server configuration queried by UploadImageRepository.
IF OBJECT_ID(N'dbo.APZ_D_UPSV', N'U') IS NULL
CREATE TABLE dbo.APZ_D_UPSV
(
    Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_APZ_D_UPSV PRIMARY KEY,
    ServerAddress nvarchar(500) NOT NULL,
    [Domain] nvarchar(254) NOT NULL,
    DBName nvarchar(254) NOT NULL,
    UploadAddress nvarchar(1000) NULL,
    UserName nvarchar(254) NULL,
    [Password] nvarchar(1000) NULL,
    RootFolder nvarchar(1000) NULL,
    Status char(1) NULL
);
GO

-- Legacy retail persistence. The recovered desktop POS currently posts sales
-- through SAP Documents, but the API still exposes the older Retail.Save path.
-- Preserve its original ORDR/RDR1-shaped contract in independent archive tables.
IF OBJECT_ID(N'dbo.APZ_RSRC', N'U') IS NULL
    SELECT TOP (0) * INTO dbo.APZ_RSRC FROM dbo.ORDR;
GO
IF OBJECT_ID(N'dbo.APZ_SRC1', N'U') IS NULL
    SELECT TOP (0) * INTO dbo.APZ_SRC1 FROM dbo.RDR1;
GO
IF OBJECT_ID(N'dbo.APZ_SRC2', N'U') IS NULL
CREATE TABLE dbo.APZ_SRC2
(
    DocEntry int NULL, PayType nvarchar(50) NULL,
    Amount decimal(19,6) NULL, Note nvarchar(500) NULL
);
GO
IF OBJECT_ID(N'dbo.APZ_SRC3', N'U') IS NULL
CREATE TABLE dbo.APZ_SRC3
(
    DocEntry int NULL, CardCode nvarchar(50) NULL,
    Amount decimal(19,6) NULL, Remarks nvarchar(500) NULL
);
GO
