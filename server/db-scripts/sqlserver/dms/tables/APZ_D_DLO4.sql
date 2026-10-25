-- converted from HANA catalog export (APZ_D_DLO4); review before production use
IF OBJECT_ID(N'dbo.[APZ_D_DLO4]') IS NULL
CREATE TABLE dbo.[APZ_D_DLO4] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [ShipCode] nvarchar(254) NULL,
    [Note] nvarchar(254) NULL,
    CONSTRAINT [PK_APZ_D_DLO4] PRIMARY KEY (ID)
);
