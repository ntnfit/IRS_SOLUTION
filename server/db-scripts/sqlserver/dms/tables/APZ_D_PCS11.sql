-- converted from HANA catalog export (APZ_D_PCS11); review before production use
IF OBJECT_ID(N'dbo.[APZ_D_PCS11]') IS NULL
CREATE TABLE dbo.[APZ_D_PCS11] (
    [Code] nvarchar(50) NOT NULL,
    [LineNum] int NOT NULL,
    [FieldID] int NULL,
    [Expression] char(1) NULL,
    [Value] nvarchar(max) NULL,
    [ObjectType] nvarchar(50) NULL,
    CONSTRAINT [PK_APZ_D_PCS11] PRIMARY KEY (Code,  LineNum)
);
