-- converted from HANA catalog export (APZ_OPIM); review before production use
IF OBJECT_ID(N'dbo.[APZ_OPIM]') IS NULL
CREATE TABLE dbo.[APZ_OPIM] (
    [PlugId] int NOT NULL,
    [Name] nvarchar(254) NULL,
    [Version] nvarchar(50) NULL,
    [Platform] nvarchar(50) NULL,
    [Group] char(1) NULL,
    CONSTRAINT [PK_APZ_OPIM] PRIMARY KEY (PlugId)
);
