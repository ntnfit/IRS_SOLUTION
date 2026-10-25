-- converted from HANA catalog export (APZ_ORTC); review before production use
IF OBJECT_ID(N'dbo.[APZ_ORTC]') IS NULL
CREATE TABLE dbo.[APZ_ORTC] (
    [DocEntry] int NOT NULL,
    [ProjectId] int NULL,
    [PostingDate] datetime NULL,
    [Total] decimal(19,6) NULL,
    [Comment] nvarchar(max) NULL,
    [Status] char(1) NULL,
    [DueDate] datetime NULL,
    [TotalActual] decimal(19,6) NULL,
    [DocumentDate] datetime NULL,
    [DocCur] nvarchar(3) NULL,
    [UserSign] int NULL,
    [CreateDate] datetime NULL,
    [CreateTime] smallint NULL,
    [UserSign2] int NULL,
    [UpdateDate] datetime NULL,
    [UpdateTime] smallint NULL,
    [LogInstanc] int NULL,
    CONSTRAINT [PK_APZ_ORTC] PRIMARY KEY (DocEntry)
);
