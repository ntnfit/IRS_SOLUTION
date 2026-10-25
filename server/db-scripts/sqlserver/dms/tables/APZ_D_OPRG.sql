-- converted from HANA catalog export (APZ_D_OPRG); review before production use
IF OBJECT_ID(N'dbo.[APZ_D_OPRG]') IS NULL
CREATE TABLE dbo.[APZ_D_OPRG] (
    [ID] int NOT NULL,
    [Name] nvarchar(250) NOT NULL,
    [Descr] nvarchar(max) NULL,
    CONSTRAINT [PK_APZ_D_OPRG] PRIMARY KEY (ID)
);
