USE [SBODemoAU];
GO

-- IRS retail refund linkage. In production these fields should also be registered
-- through SAP Business One metadata; the dev database needs the physical columns
-- so the recovered SQL contract matches the HANA implementation.
IF COL_LENGTH(N'dbo.RDR1', N'U_OriginType') IS NULL
    ALTER TABLE dbo.RDR1 ADD U_OriginType nvarchar(20) NULL;
GO
IF COL_LENGTH(N'dbo.RDR1', N'U_OriginCode') IS NULL
    ALTER TABLE dbo.RDR1 ADD U_OriginCode int NULL;
GO
IF COL_LENGTH(N'dbo.RDR1', N'U_OriginLine') IS NULL
    ALTER TABLE dbo.RDR1 ADD U_OriginLine int NULL;
GO

IF COL_LENGTH(N'dbo.ORDR', N'U_RetailStore') IS NULL ALTER TABLE dbo.ORDR ADD U_RetailStore nvarchar(50) NULL;
IF COL_LENGTH(N'dbo.ORDR', N'U_Posid') IS NULL ALTER TABLE dbo.ORDR ADD U_Posid int NULL;
IF COL_LENGTH(N'dbo.ORDR', N'U_RetailPaidEnt') IS NULL ALTER TABLE dbo.ORDR ADD U_RetailPaidEnt int NULL;
IF COL_LENGTH(N'dbo.ORDR', N'U_RelatedTyp') IS NULL ALTER TABLE dbo.ORDR ADD U_RelatedTyp nvarchar(20) NULL;
IF COL_LENGTH(N'dbo.ORDR', N'U_RelatedEnt') IS NULL ALTER TABLE dbo.ORDR ADD U_RelatedEnt int NULL;
GO

IF COL_LENGTH(N'dbo.ODRF', N'U_RetailStore') IS NULL ALTER TABLE dbo.ODRF ADD U_RetailStore nvarchar(50) NULL;
IF COL_LENGTH(N'dbo.ODRF', N'U_Posid') IS NULL ALTER TABLE dbo.ODRF ADD U_Posid int NULL;
IF COL_LENGTH(N'dbo.ODRF', N'U_RetailPaidEnt') IS NULL ALTER TABLE dbo.ODRF ADD U_RetailPaidEnt int NULL;
IF COL_LENGTH(N'dbo.ODRF', N'U_RelatedTyp') IS NULL ALTER TABLE dbo.ODRF ADD U_RelatedTyp nvarchar(20) NULL;
IF COL_LENGTH(N'dbo.ODRF', N'U_RelatedEnt') IS NULL ALTER TABLE dbo.ODRF ADD U_RelatedEnt int NULL;
GO
