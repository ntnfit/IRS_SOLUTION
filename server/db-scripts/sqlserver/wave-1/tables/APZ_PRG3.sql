-- converted from HANA catalog export (APZ_PRG3); review before production use
IF OBJECT_ID(N'dbo.[APZ_PRG3]') IS NULL
CREATE TABLE dbo.[APZ_PRG3] (
    [AbsEntry] int NOT NULL,
    [LineNum] int NOT NULL,
    [Resource] int NOT NULL,
    [Role] int NULL,
    [LogInstanc] int NULL,
    CONSTRAINT [PK_APZ_PRG3] PRIMARY KEY (AbsEntry,  LineNum,  Resource)
);
