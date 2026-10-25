-- converted from HANA catalog export (APZ_D_OSAD); review before production use
IF OBJECT_ID(N'dbo.[APZ_D_OSAD]') IS NULL
CREATE TABLE dbo.[APZ_D_OSAD] (
    [DocEntry] int NOT NULL,
    [ApprovalObjectType] nvarchar(250) NULL,
    [UserSign] int NULL,
    [CreateDate] datetime NULL,
    [Active] char(1) NULL,
    CONSTRAINT [PK_APZ_D_OSAD] PRIMARY KEY (DocEntry)
);
