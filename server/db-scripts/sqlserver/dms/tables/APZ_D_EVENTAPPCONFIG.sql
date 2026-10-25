-- converted from HANA catalog export (APZ_D_EVENTAPPCONFIG); review before production use
IF OBJECT_ID(N'dbo.[APZ_D_EVENTAPPCONFIG]') IS NULL
CREATE TABLE dbo.[APZ_D_EVENTAPPCONFIG] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [EventCode] nvarchar(50) NULL,
    [AppName] nvarchar(50) NULL,
    [Message] nvarchar(254) NULL,
    [Title] nvarchar(254) NULL,
    [Active] char(1) NULL DEFAULT 'N',
    CONSTRAINT [PK_APZ_D_EVENTAPPCONFIG] PRIMARY KEY (ID)
);
