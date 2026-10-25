-- converted from HANA catalog export (APZ_OEQR); review before production use
IF OBJECT_ID(N'dbo.[APZ_OEQR]') IS NULL
CREATE TABLE dbo.[APZ_OEQR] (
    [Id] int NOT NULL,
    [Category] nvarchar(100) NULL,
    [QueryId] int NULL,
    [Qname] nvarchar(254) NULL,
    [Qtype] char(1) NULL,
    [Qstring] nvarchar(max) NULL,
    [Name] nvarchar(254) NULL,
    [Dscription] nvarchar(254) NULL,
    CONSTRAINT [PK_APZ_OEQR] PRIMARY KEY (Id)
);
