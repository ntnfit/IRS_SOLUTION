-- converted from HANA catalog export (APZ_D_OOVF); review before production use
IF OBJECT_ID(N'dbo.[APZ_D_OOVF]') IS NULL
CREATE TABLE dbo.[APZ_D_OOVF] (
    [Code] nvarchar(50) NOT NULL,
    [Name] nvarchar(254) NOT NULL,
    [Address] nvarchar(max) NULL,
    [Description] nvarchar(254) NULL,
    [StartDate] datetime NULL,
    [Finished] char(1) NULL DEFAULT 'N',
    [Recurring] char(1) NULL DEFAULT 'W',
    [RecurNum] int NULL DEFAULT 1,
    [RecurNote] nvarchar(254) NULL,
    [EveryDay] char(1) NULL DEFAULT 'N',
    [DayOfWeek] char(1) NULL DEFAULT 'N',
    [Mon] char(1) NULL DEFAULT 'N',
    [Tue] char(1) NULL DEFAULT 'N',
    [Wed] char(1) NULL DEFAULT 'N',
    [Thu] char(1) NULL DEFAULT 'N',
    [Fri] char(1) NULL DEFAULT 'N',
    [Sat] char(1) NULL DEFAULT 'N',
    [Sun] char(1) NULL DEFAULT 'N',
    [DayOfMonth] char(1) NULL DEFAULT 'N',
    [ListOfDate] nvarchar(1000) NULL,
    [UserSign] int NULL,
    [UpdateDate] datetime NULL,
    [LogInstanc] int NULL,
    [NumberOfVisits] int NULL
);
