-- converted from HANA catalog export (APZ_OMNI); review before production use
IF OBJECT_ID(N'dbo.[APZ_OMNI]') IS NULL
CREATE TABLE dbo.[APZ_OMNI] (
    [Id] int NOT NULL,
    [FatherId] int NULL,
    [Code] nvarchar(254) NULL,
    [Name] nvarchar(254) NULL,
    [LanguageKey] varchar(254) NULL,
    [MenuType] char(1) NULL,
    CONSTRAINT [PK_APZ_OMNI] PRIMARY KEY (Id)
);
