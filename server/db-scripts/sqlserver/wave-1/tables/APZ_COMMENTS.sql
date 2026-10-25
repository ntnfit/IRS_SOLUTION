-- converted from HANA catalog export (APZ_COMMENTS); review before production use
IF OBJECT_ID(N'dbo.[APZ_COMMENTS]') IS NULL
CREATE TABLE dbo.[APZ_COMMENTS] (
    [ID] varchar(50) NOT NULL,
    [OBJTYPE] varchar(20) NULL,
    [OBJECT_CODE] varchar(50) NULL,
    [DETAIL_CODE] varchar(50) NULL,
    [CREATE_DATE] datetime NULL,
    [CREATE_TIME] datetime NULL,
    [CREATED_BY] varchar(50) NULL,
    [CREATOR_NAME] varchar(100) NULL,
    [COMMENTS] varchar(max) NULL,
    CONSTRAINT [PK_APZ_COMMENTS] PRIMARY KEY (ID)
);
