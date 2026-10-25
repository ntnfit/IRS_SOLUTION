-- Minimal IRS objects so the recovered ApiIRS (SQL Server mode) can run its login flow in dev.
-- Not a real IRS install: only the tables the login path touches (see BrokerService.Login).
-- Run through setup-dev-db.ps1, which supplies the sqlcmd variables:
--   $(CompanyDb)  SAP B1 company DB to register        $(DevLogin) / $(DevPassword)  SQL login for the API
--   $(EncServer) $(EncUser) $(EncPass)  RSA-encrypted APZ_SLSP values   $(EncVersion)  encrypted "2.10.776"
SET NOCOUNT ON;
SET XACT_ABORT ON;

IF DB_ID(N'IRS-COMMON') IS NULL
    CREATE DATABASE [IRS-COMMON];
GO

IF SUSER_ID(N'$(DevLogin)') IS NULL
    CREATE LOGIN [$(DevLogin)] WITH PASSWORD = N'$(DevPassword)', CHECK_POLICY = ON, DEFAULT_DATABASE = [IRS-COMMON];
ELSE
    ALTER LOGIN [$(DevLogin)] WITH PASSWORD = N'$(DevPassword)';
GO

USE [IRS-COMMON];
IF USER_ID(N'$(DevLogin)') IS NULL CREATE USER [$(DevLogin)] FOR LOGIN [$(DevLogin)];
ALTER ROLE db_owner ADD MEMBER [$(DevLogin)];

-- company registry read by GetInfoCompanyDbServer: server/user/pass are RSA-encrypted; Status P = updating, N = inactive
IF OBJECT_ID(N'dbo.APZ_SLSP') IS NULL
    CREATE TABLE dbo.APZ_SLSP (
        CmpDB nvarchar(100) NOT NULL PRIMARY KEY,
        CpnDBServer nvarchar(max) NOT NULL,
        CpnDBUser nvarchar(max) NOT NULL,
        CpnDBPass nvarchar(max) NOT NULL,
        Status nvarchar(1) NOT NULL DEFAULT N'Y');

-- session log written by LicenseManager.UpdateSessionUser
IF OBJECT_ID(N'dbo.APZ_LOGLOGIN') IS NULL
    CREATE TABLE dbo.APZ_LOGLOGIN (
        ID int IDENTITY PRIMARY KEY,
        [Session] nvarchar(400), [User] nvarchar(100), [DataBase] nvarchar(100),
        LoginTime datetime, LogoffTime datetime, UpdateTime datetime, [status] nvarchar(1),
        ClientLocalIpAddress nvarchar(100), ClientMachineName nvarchar(200), ClientWindowsLogonName nvarchar(200));

-- license tables read by LoginIRS.CheckLicenseIrs (left empty: dev logs in as the configured SuperUser)
IF OBJECT_ID(N'dbo.APZ_OALC') IS NULL
    CREATE TABLE dbo.APZ_OALC (InstallNumber nvarchar(50), License nvarchar(max), DateRequest nvarchar(max), HardwareKey nvarchar(400));
IF OBJECT_ID(N'dbo.APZ_LOU') IS NULL
    CREATE TABLE dbo.APZ_LOU (InstallNumber nvarchar(50), LicenseType nvarchar(50), [User] nvarchar(100), [DataBase] nvarchar(100));

-- login screen database list: APZ_SLSP join APZ_OLDD join APZ_OCDB on the certificate domain (SqlCommonsRepository.GetDatabaseListQuery)
IF COL_LENGTH(N'dbo.APZ_SLSP', N'CmpName') IS NULL
    ALTER TABLE dbo.APZ_SLSP ADD CmpName nvarchar(200) NULL;
IF OBJECT_ID(N'dbo.APZ_OCDB') IS NULL
    CREATE TABLE dbo.APZ_OCDB (Domain nvarchar(100) NOT NULL PRIMARY KEY);
IF OBJECT_ID(N'dbo.APZ_OLDD') IS NULL
    CREATE TABLE dbo.APZ_OLDD (Domain nvarchar(100) NOT NULL, [Database] nvarchar(100) NOT NULL, HardwareKey nvarchar(400) NULL,
        CONSTRAINT PK_APZ_OLDD PRIMARY KEY (Domain, [Database]));
GO

MERGE dbo.APZ_SLSP AS t
USING (SELECT N'$(CompanyDb)' AS CmpDB) AS s ON t.CmpDB = s.CmpDB
WHEN MATCHED THEN UPDATE SET CpnDBServer = N'$(EncServer)', CpnDBUser = N'$(EncUser)', CpnDBPass = N'$(EncPass)', Status = N'Y'
WHEN NOT MATCHED THEN INSERT (CmpDB, CpnDBServer, CpnDBUser, CpnDBPass, Status)
    VALUES (s.CmpDB, N'$(EncServer)', N'$(EncUser)', N'$(EncPass)', N'Y');

UPDATE dbo.APZ_SLSP SET CmpName = (SELECT TOP 1 CompnyName FROM [$(CompanyDb)].dbo.OADM) WHERE CmpDB = N'$(CompanyDb)';
IF NOT EXISTS (SELECT 1 FROM dbo.APZ_OCDB WHERE Domain = N'$(Domain)') INSERT INTO dbo.APZ_OCDB (Domain) VALUES (N'$(Domain)');
IF NOT EXISTS (SELECT 1 FROM dbo.APZ_OLDD WHERE Domain = N'$(Domain)' AND [Database] = N'$(CompanyDb)')
    INSERT INTO dbo.APZ_OLDD (Domain, [Database]) VALUES (N'$(Domain)', N'$(CompanyDb)');
GO

USE [$(CompanyDb)];
IF USER_ID(N'$(DevLogin)') IS NULL CREATE USER [$(DevLogin)] FOR LOGIN [$(DevLogin)];
ALTER ROLE db_owner ADD MEMBER [$(DevLogin)];

-- IRS install marker read by Commons.CheckExitsDatabase; Version must equal the server build (encrypted)
IF OBJECT_ID(N'dbo.APZ_CINF') IS NULL
    CREATE TABLE dbo.APZ_CINF ([Version] nvarchar(max) NOT NULL, TextCode nvarchar(max) NULL);
IF EXISTS (SELECT 1 FROM dbo.APZ_CINF)
    UPDATE dbo.APZ_CINF SET [Version] = N'$(EncVersion)';
ELSE
    INSERT INTO dbo.APZ_CINF ([Version]) VALUES (N'$(EncVersion)');
GO

PRINT 'IRS-COMMON dev setup done for $(CompanyDb)';
