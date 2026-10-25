-- converted from HANA catalog export (APZ_OMDC); review before production use
IF OBJECT_ID(N'dbo.[APZ_OMDC]') IS NULL
CREATE TABLE dbo.[APZ_OMDC] (
    [MsgCode] int NOT NULL,
    [MsgName] nvarchar(max) NULL,
    [MsgLng] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_APZ_OMDC] PRIMARY KEY (MsgCode,  MsgLng)
);
