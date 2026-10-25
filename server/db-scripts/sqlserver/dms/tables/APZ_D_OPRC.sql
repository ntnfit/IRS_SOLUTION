-- converted from HANA catalog export (APZ_D_OPRC); review before production use
IF OBJECT_ID(N'dbo.[APZ_D_OPRC]') IS NULL
CREATE TABLE dbo.[APZ_D_OPRC] (
    [ID] int NOT NULL,
    [Name] nvarchar(250) NOT NULL,
    [Descr] nvarchar(max) NULL,
    CONSTRAINT [PK_APZ_D_OPRC] PRIMARY KEY (ID)
);
