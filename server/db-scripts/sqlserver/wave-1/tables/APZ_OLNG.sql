-- converted from HANA catalog export (APZ_OLNG); review before production use
IF OBJECT_ID(N'dbo.[APZ_OLNG]') IS NULL
CREATE TABLE dbo.[APZ_OLNG] (
    [LngCode] nvarchar(50) NOT NULL,
    [LngName] nvarchar(254) NULL,
    [Status] char(1) NULL,
    [Remarks] nvarchar(254) NULL,
    [SysLng] char(1) NULL DEFAULT 'N',
    CONSTRAINT [PK_APZ_OLNG] PRIMARY KEY (LngCode)
);
