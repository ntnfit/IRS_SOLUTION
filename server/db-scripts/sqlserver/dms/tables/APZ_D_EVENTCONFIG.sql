-- converted from HANA catalog export (APZ_D_EVENTCONFIG); review before production use
IF OBJECT_ID(N'dbo.[APZ_D_EVENTCONFIG]') IS NULL
CREATE TABLE dbo.[APZ_D_EVENTCONFIG] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [EventCode] nvarchar(50) NULL,
    [EventName] nvarchar(254) NULL,
    [EventMessage] nvarchar(254) NULL,
    [AppUse] nvarchar(100) NULL,
    [EventTitle] nvarchar(254) NULL,
    CONSTRAINT [PK_APZ_D_EVENTCONFIG] PRIMARY KEY (ID)
);
