-- converted from HANA catalog export (APZ_CLG3); review before production use
IF OBJECT_ID(N'dbo.[APZ_CLG3]') IS NULL
CREATE TABLE dbo.[APZ_CLG3] (
    [CmtCode] int NOT NULL,
    [UserSign] nvarchar(50) NOT NULL,
    [Unread] varchar(1) NULL,
    CONSTRAINT [PK_APZ_CLG3] PRIMARY KEY (CmtCode,  UserSign)
);
