-- converted from HANA catalog export (APZ_D_CHM1); review before production use
IF OBJECT_ID(N'dbo.[APZ_D_CHM1]') IS NULL
CREATE TABLE dbo.[APZ_D_CHM1] (
    [DocEntry] int NOT NULL,
    [LineNum] int NOT NULL,
    [VisOrder] int NULL,
    [Question] nvarchar(max) NULL,
    CONSTRAINT [PK_APZ_D_CHM1] PRIMARY KEY (DocEntry,  LineNum)
);
