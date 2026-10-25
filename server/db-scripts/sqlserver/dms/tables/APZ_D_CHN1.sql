-- converted from HANA catalog export (APZ_D_CHN1); review before production use
IF OBJECT_ID(N'dbo.[APZ_D_CHN1]') IS NULL
CREATE TABLE dbo.[APZ_D_CHN1] (
    [ItemCode] varchar(50) NOT NULL,
    [ChannelCode] varchar(50) NULL,
    [Status] char(1) NULL,
    [Remarks] nvarchar(254) NULL,
    [Active] char(1) NULL,
    [CreateDate] datetime NULL,
    [CreateTime] int NULL,
    [UpdateDate] datetime NULL,
    [UpdateTime] int NULL,
    CONSTRAINT [PK_APZ_D_CHN1] PRIMARY KEY (ItemCode)
);
