-- converted from HANA catalog export (APZ_RTC1); review before production use
IF OBJECT_ID(N'dbo.[APZ_RTC1]') IS NULL
CREATE TABLE dbo.[APZ_RTC1] (
    [DocEntry] int NOT NULL,
    [LineNum] int NOT NULL,
    [TaskCode] nvarchar(50) NULL,
    [TaskName] nvarchar(254) NULL,
    [ItemCode] nvarchar(50) NULL,
    [ItemName] nvarchar(254) NULL,
    [Quantity] decimal(19,6) NULL,
    [RefPrice] decimal(19,6) NULL,
    [Total] decimal(19,6) NULL,
    [UoM] nvarchar(254) NULL,
    [Remarks] nvarchar(max) NULL,
    [LineStatus] char(1) NULL,
    [OpenQty] decimal(38,10) NULL,
    [BaseLine] int NULL,
    [BaseEntry] int NULL,
    [ActualQty] decimal(19,6) NULL,
    [ActualAmount] decimal(19,6) NULL,
    [PostingDate] datetime NULL,
    [DueDate] datetime NULL,
    [DocumentDate] datetime NULL,
    [Currency] nvarchar(3) NULL,
    [LogInstanc] int NULL,
    CONSTRAINT [PK_APZ_RTC1] PRIMARY KEY (DocEntry,  LineNum)
);
