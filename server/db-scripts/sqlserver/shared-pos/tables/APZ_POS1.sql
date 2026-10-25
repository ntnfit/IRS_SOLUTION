-- converted from HANA catalog export (APZ_POS1); review before production use
IF OBJECT_ID(N'dbo.[APZ_POS1]') IS NULL
CREATE TABLE dbo.[APZ_POS1] (
    [UserCode] nvarchar(25) NOT NULL,
    [PosID] int NOT NULL,
    [IsDflt] char(1) NULL DEFAULT 'N',
    CONSTRAINT [PK_APZ_POS1] PRIMARY KEY (UserCode,  PosID)
);
