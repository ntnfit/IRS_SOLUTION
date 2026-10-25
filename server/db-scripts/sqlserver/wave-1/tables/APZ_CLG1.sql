-- converted from HANA catalog export (APZ_CLG1); review before production use
IF OBJECT_ID(N'dbo.[APZ_CLG1]') IS NULL
CREATE TABLE dbo.[APZ_CLG1] (
    [ID] int NOT NULL,
    [ObjType] nvarchar(100) NOT NULL,
    [CmtCode] int NOT NULL,
    [UserSign] nvarchar(50) NULL,
    [UserName] nvarchar(100) NULL,
    [CreatedTime] datetime NULL,
    [Content] nvarchar(max) NULL,
    CONSTRAINT [PK_APZ_CLG1] PRIMARY KEY (ID,  ObjType,  CmtCode)
);
