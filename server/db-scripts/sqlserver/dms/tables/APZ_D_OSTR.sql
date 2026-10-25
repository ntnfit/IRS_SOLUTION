-- converted from HANA catalog export (APZ_D_OSTR); review before production use
IF OBJECT_ID(N'dbo.[APZ_D_OSTR]') IS NULL
CREATE TABLE dbo.[APZ_D_OSTR] (
    [RouteCode] nvarchar(50) NOT NULL,
    [RouteName] nvarchar(254) NULL,
    [CountryCode] nvarchar(50) NULL,
    [Remarks] nvarchar(254) NULL,
    [Status] varchar(1) NULL,
    [ChannelCode] varchar(50) NULL,
    [UserSign] smallint NULL,
    [CreateDate] datetime NULL,
    [CreateTime] int NULL,
    [UpdateDate] datetime NULL,
    [UpdateTime] int NULL,
    [PriceList] int NULL,
    [PriceListDistributor] int NULL,
    [LogInstance] int NULL,
    [UserSign2] int NULL,
    [Recurring] char(1) NULL,
    [RecurNum] int NULL,
    [NumberOfVisits] int NULL,
    CONSTRAINT [PK_APZ_D_OSTR] PRIMARY KEY (RouteCode)
);
