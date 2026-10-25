-- converted from HANA catalog export (APZ_D_DLO5); review before production use
IF OBJECT_ID(N'dbo.[APZ_D_DLO5]') IS NULL
CREATE TABLE dbo.[APZ_D_DLO5] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [CardCode] nvarchar(254) NULL,
    [Note] nvarchar(254) NULL,
    CONSTRAINT [PK_APZ_D_DLO5] PRIMARY KEY (ID)
);
