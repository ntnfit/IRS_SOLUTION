-- converted from HANA catalog export (APZ_PRG2); review before production use
IF OBJECT_ID(N'dbo.[APZ_PRG2]') IS NULL
CREATE TABLE dbo.[APZ_PRG2] (
    [AbsEntry] int NOT NULL,
    [LineNum] int NOT NULL,
    [Dependent] int NOT NULL,
    [VisOrder] int NULL,
    [LogInstanc] int NULL,
    CONSTRAINT [PK_APZ_PRG2] PRIMARY KEY (AbsEntry,  LineNum,  Dependent)
);
