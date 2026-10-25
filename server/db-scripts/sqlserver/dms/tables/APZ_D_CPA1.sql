-- converted from HANA catalog export (APZ_D_CPA1); review before production use
IF OBJECT_ID(N'dbo.[APZ_D_CPA1]') IS NULL
CREATE TABLE dbo.[APZ_D_CPA1] (
    [Code] nvarchar(100) NOT NULL,
    [VisitCode] nvarchar(100) NOT NULL,
    [LineNum] int NOT NULL,
    [ItemName] nvarchar(254) NULL,
    [IsAvailable] char(1) NULL,
    CONSTRAINT [PK_APZ_D_CPA1] PRIMARY KEY (Code,  VisitCode,  LineNum)
);
