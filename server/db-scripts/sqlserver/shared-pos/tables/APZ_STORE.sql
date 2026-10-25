-- converted from HANA catalog export (APZ_STORE); review before production use
IF OBJECT_ID(N'dbo.[APZ_STORE]') IS NULL
CREATE TABLE dbo.[APZ_STORE] (
    [StoreID] int NOT NULL,
    [Code] nvarchar(50) NOT NULL,
    [Dscription] nvarchar(100) NULL,
    [BPLId] int NULL,
    [WhsCode] nvarchar(50) NULL,
    [Address] nvarchar(100) NULL,
    [Email] nvarchar(100) NULL,
    [CardCode] nvarchar(15) NULL,
    [SlpCode] int NULL,
    [ListNum] int NULL,
    [UserSign] smallint NULL,
    [UpdateDate] datetime NULL,
    [CreateDate] datetime NULL,
    [UserSign2] smallint NULL,
    [Locked] char(1) NULL,
    [DataSource] char(1) NULL,
    [LogInstanc] smallint NULL,
    [OSKeyBoardEnable] char(1) NULL,
    CONSTRAINT [PK_APZ_STORE] PRIMARY KEY (StoreID)
);
