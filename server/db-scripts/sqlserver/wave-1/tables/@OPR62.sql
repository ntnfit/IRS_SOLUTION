-- converted from HANA catalog export (@OPR62); review before production use
IF OBJECT_ID(N'dbo.[@OPR62]') IS NULL
CREATE TABLE dbo.[@OPR62] (
    [DocEntry] int NOT NULL,
    [LineId] int NOT NULL,
    [VisOrder] int NULL,
    [Object] nvarchar(20) NULL,
    [LogInst] int NULL,
    [U_ItemCode] nvarchar(50) NULL,
    [U_ItemName] nvarchar(254) NULL,
    [U_FrgnName] nvarchar(254) NULL,
    [U_IPercent] decimal(21,6) NULL,
    [U_Price] decimal(21,6) NULL,
    [U_PotentAmount] decimal(21,6) NULL,
    [U_DeliveryDate] datetime NULL,
    [U_OpportId] int NULL,
    [U_Quantity] decimal(21,6) NULL,
    [U_AttFile] nvarchar(254) NULL,
    CONSTRAINT [PK_@OPR62] PRIMARY KEY (DocEntry,  LineId)
);
