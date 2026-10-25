-- converted from HANA catalog export (APZ_PRG6); review before production use
IF OBJECT_ID(N'dbo.[APZ_PRG6]') IS NULL
CREATE TABLE dbo.[APZ_PRG6] (
    [AbsEntry] int NOT NULL,
    [LineNum] int NOT NULL,
    [TaskCode] nvarchar(50) NULL,
    [TaskName] nvarchar(254) NULL,
    [InvoicePlan] decimal(19,6) NULL,
    [IncomingPayment] decimal(19,6) NULL,
    [Payments] decimal(19,6) NULL,
    [Duedate] datetime NULL,
    [Remarks] nvarchar(max) NULL,
    [FatherNum] int NULL,
    [IsNew] tinyint NULL,
    [BaseLine] int NULL,
    [VisOrder] int NULL,
    [LogInstanc] int NULL,
    CONSTRAINT [PK_APZ_PRG6] PRIMARY KEY (AbsEntry,  LineNum)
);
