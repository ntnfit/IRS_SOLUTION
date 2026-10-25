-- converted from HANA catalog export (APZ_PRG1); review before production use
IF OBJECT_ID(N'dbo.[APZ_PRG1]') IS NULL
CREATE TABLE dbo.[APZ_PRG1] (
    [AbsEntry] int NOT NULL,
    [LineNum] int NOT NULL,
    [Wbs] nvarchar(50) NULL,
    [TaskCode] nvarchar(50) NULL,
    [TaskName] nvarchar(254) NULL,
    [FromDate] datetime NULL,
    [ToDate] datetime NULL,
    [OcrCode] nvarchar(8) NULL,
    [OcrCode2] nvarchar(8) NULL,
    [OcrCode3] nvarchar(8) NULL,
    [OcrCode4] nvarchar(8) NULL,
    [OcrCode5] nvarchar(8) NULL,
    [Priority] int NULL,
    [Milestone] char(1) NULL,
    [Notes] nvarchar(max) NULL,
    [Status] char(1) NULL,
    [FatherNum] int NULL,
    [VisOrder] int NULL,
    [BaseLine] int NULL,
    [WbsTemplate] nvarchar(50) NULL,
    [LogInstanc] int NULL,
    [WBSMode] char(1) NULL,
    [Duration] decimal(18,9) NULL,
    CONSTRAINT [PK_APZ_PRG1] PRIMARY KEY (AbsEntry,  LineNum)
);
