-- Undo setup-irs-common-dev.sql: removes APZ_CINF and the dev user from the company DB,
-- drops IRS-COMMON and the dev login. SAP B1 data is not touched.
-- sqlcmd -S localhost -E -C -v CompanyDb=SBODemoAU DevLogin=irs_dev -i teardown-irs-common-dev.sql
SET NOCOUNT ON;

USE [$(CompanyDb)];
IF OBJECT_ID(N'dbo.APZ_CINF') IS NOT NULL DROP TABLE dbo.APZ_CINF;
IF USER_ID(N'$(DevLogin)') IS NOT NULL DROP USER [$(DevLogin)];
GO

USE [master];
IF DB_ID(N'IRS-COMMON') IS NOT NULL
BEGIN
    ALTER DATABASE [IRS-COMMON] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [IRS-COMMON];
END;
IF SUSER_ID(N'$(DevLogin)') IS NOT NULL DROP LOGIN [$(DevLogin)];
GO

PRINT 'IRS-COMMON dev setup removed';
