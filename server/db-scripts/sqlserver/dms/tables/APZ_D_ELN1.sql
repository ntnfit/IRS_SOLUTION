-- converted from HANA catalog export (APZ_D_ELN1); review before production use
IF OBJECT_ID(N'dbo.[APZ_D_ELN1]') IS NULL
CREATE TABLE dbo.[APZ_D_ELN1] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [EventCode] nvarchar(50) NULL,
    [Title] nvarchar(100) NULL,
    [Message] nvarchar(254) NULL,
    [Image] nvarchar(250) NULL,
    [Data] nvarchar(100) NULL,
    [DataExtra] nvarchar(max) NULL,
    [SendUser] varchar(4000) NULL,
    [SendAll] char(1) NULL DEFAULT 'N',
    [Pending] char(1) NULL DEFAULT 'Y',
    [Success] char(1) NULL DEFAULT 'N',
    [Retry] int NULL DEFAULT 0,
    [Method] nvarchar(50) NULL DEFAULT 'app',
    [CreateDate] datetime NULL DEFAULT CAST(GETDATE() AS date),
    [UpdateDate] datetime NULL DEFAULT CAST(GETDATE() AS date),
    [Read] char(1) NULL DEFAULT 'N',
    [Note] nvarchar(max) NULL,
    [DatabaseName] nvarchar(50) NULL,
    [IsRemove] char(1) NULL,
    CONSTRAINT [PK_APZ_D_ELN1] PRIMARY KEY (ID)
);
