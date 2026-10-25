-- converted from HANA catalog export (APZ_D_STC1); review before production use
IF OBJECT_ID(N'dbo.[APZ_D_STC1]') IS NULL
CREATE TABLE dbo.[APZ_D_STC1] (
    [Code] nvarchar(100) NOT NULL,
    [LineNum] int NOT NULL,
    [ItemCode] nvarchar(50) NULL,
    [Dscription] nvarchar(254) NULL,
    [Quantity] decimal(19,6) NULL,
    CONSTRAINT [PK_APZ_D_STC1] PRIMARY KEY (Code,  LineNum)
);
