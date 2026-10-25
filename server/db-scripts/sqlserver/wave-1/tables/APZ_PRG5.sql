-- converted from HANA catalog export (APZ_PRG5); review before production use
IF OBJECT_ID(N'dbo.[APZ_PRG5]') IS NULL
CREATE TABLE dbo.[APZ_PRG5] (
    [AbsEntry] int NOT NULL,
    [ObjType] nvarchar(20) NOT NULL,
    [DocEntry] int NOT NULL,
    CONSTRAINT [PK_APZ_PRG5] PRIMARY KEY (AbsEntry,  ObjType,  DocEntry)
);
