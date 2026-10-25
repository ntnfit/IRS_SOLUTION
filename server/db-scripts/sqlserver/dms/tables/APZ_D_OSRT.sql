-- converted from HANA catalog export (APZ_D_OSRT); review before production use
IF OBJECT_ID(N'dbo.[APZ_D_OSRT]') IS NULL
CREATE TABLE dbo.[APZ_D_OSRT] (
    [DocEntry] int NOT NULL,
    [ReportId] nvarchar(250) NULL,
    [UserSign] int NULL,
    [CreateDate] datetime NULL,
    [Active] char(1) NULL,
    [ReportName] nvarchar(250) NULL,
    CONSTRAINT [PK_APZ_D_OSRT] PRIMARY KEY (DocEntry)
);
