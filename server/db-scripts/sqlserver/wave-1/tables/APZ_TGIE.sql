-- converted from HANA catalog export (APZ_TGIE); review before production use
IF OBJECT_ID(N'dbo.[APZ_TGIE]') IS NULL
CREATE TABLE dbo.[APZ_TGIE] (
    [Id] int NOT NULL,
    [IssueDate] datetime NULL,
    [IssueType] int NULL,
    [Priority] int NULL,
    [Status] int NULL,
    [Reporter] int NULL,
    [HandleBy] int NULL,
    [SolutionId] int NULL,
    [ResponseDate] datetime NULL,
    [BpCode] nvarchar(50) NULL,
    [ProjectCode] int NULL,
    [SolutionBy] int NULL,
    [WbsCode] nvarchar(50) NULL,
    [AttachedFiled] int NULL,
    [Note] nvarchar(max) NULL,
    [ProjectOwner] int NULL,
    [UserSign] int NULL,
    [UserSign2] int NULL,
    [ServiceCall] int NULL,
    [Summary] nvarchar(max) NULL,
    [IssueDeleted] char(1) NULL,
    CONSTRAINT [PK_APZ_TGIE] PRIMARY KEY (Id)
);
