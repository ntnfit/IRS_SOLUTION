-- converted from HANA catalog export (@RA_CONN); review before production use
IF OBJECT_ID(N'dbo.[@RA_CONN]') IS NULL
CREATE TABLE dbo.[@RA_CONN] (
    [Code] nvarchar(50) NOT NULL,
    [Name] nvarchar(100) NOT NULL,
    [U_PWD] nvarchar(max) NULL,
    [U_TOKEN] nvarchar(max) NULL,
    [U_USER] nvarchar(254) NULL,
    [U_ServerName] nvarchar(254) NULL,
    CONSTRAINT [PK_@RA_CONN] PRIMARY KEY (Code)
);
