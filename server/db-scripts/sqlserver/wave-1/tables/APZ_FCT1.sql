-- converted from HANA catalog export (APZ_FCT1); review before production use
IF OBJECT_ID(N'dbo.[APZ_FCT1]') IS NULL
CREATE TABLE dbo.[APZ_FCT1] (
    [Code] int NOT NULL,
    [LineID] int NULL,
    [ItemCode] nvarchar(50) NOT NULL,
    [Date] datetime NOT NULL,
    [Quantity] decimal(19,6) NULL,
    [WhsCode] nvarchar(8) NOT NULL,
    [BPCode] nvarchar(15) NULL,
    [SplCode] int NULL,
    [SubCatNum] nvarchar(50) NULL,
    CONSTRAINT [PK_APZ_FCT1] PRIMARY KEY (Code,  ItemCode,  Date,  WhsCode)
);
