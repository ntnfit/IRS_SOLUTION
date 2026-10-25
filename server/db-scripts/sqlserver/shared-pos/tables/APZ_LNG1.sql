-- converted from HANA catalog export (APZ_LNG1); review before production use
IF OBJECT_ID(N'dbo.[APZ_LNG1]') IS NULL
CREATE TABLE dbo.[APZ_LNG1] (
    [LngCode] nvarchar(50) NOT NULL,
    [TrlLng] nvarchar(50) NOT NULL,
    [TrlName] nvarchar(254) NULL,
    [Remarks] nvarchar(254) NULL,
    CONSTRAINT [PK_APZ_LNG1] PRIMARY KEY (LngCode,  TrlLng)
);
