-- converted from HANA catalog export (APZ_D_OCHN); review before production use
IF OBJECT_ID(N'dbo.[APZ_D_OCHN]') IS NULL
CREATE TABLE dbo.[APZ_D_OCHN] (
    [ChannelCode] varchar(50) NOT NULL,
    [ChannelName] nvarchar(254) NULL,
    [Status] char(1) NULL,
    [AllItem] char(1) NULL,
    [Remarks] nvarchar(254) NULL,
    [UserSign] smallint NULL,
    [Loginstanc] int NULL,
    [CreateDate] datetime NULL,
    [CreateTime] int NULL,
    [UpdateDate] datetime NULL,
    [UpdateTime] int NULL,
    CONSTRAINT [PK_APZ_D_OCHN] PRIMARY KEY (ChannelCode)
);
