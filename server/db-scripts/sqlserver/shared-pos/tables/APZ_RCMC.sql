-- converted from HANA catalog export (APZ_RCMC); review before production use
IF OBJECT_ID(N'dbo.[APZ_RCMC]') IS NULL
CREATE TABLE dbo.[APZ_RCMC] (
    [CmdID] int NOT NULL,
    [CmdName] nvarchar(100) NULL,
    [Text] nvarchar(254) NULL,
    [TextColor] int NULL,
    [BackColor] int NULL,
    [Hotkey] nvarchar(254) NULL,
    [MnGroup] int NULL,
    [VisOrder] int NULL,
    [Visible] char(1) NULL DEFAULT 'Y',
    CONSTRAINT [PK_APZ_RCMC] PRIMARY KEY (CmdID)
);
