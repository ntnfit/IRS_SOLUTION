-- converted from HANA catalog export (OPRM); IRS menu-authorization table (not a SAP B1 standard table)
IF OBJECT_ID(N'dbo.[OPRM]') IS NULL
CREATE TABLE dbo.[OPRM] (
    [USER_CODE] nvarchar(20) NOT NULL,
    [menucode] nvarchar(50) NOT NULL,
    [authoriza] nvarchar(2) NULL,
    [isDefault] char(1) NULL
);
