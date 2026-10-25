-- converted from HANA catalog export (APZ_CLOCK); review before production use
IF OBJECT_ID(N'dbo.[APZ_CLOCK]') IS NULL
CREATE TABLE dbo.[APZ_CLOCK] (
    [LineId] int NOT NULL,
    [StoreCode] nvarchar(50) NOT NULL,
    [PosID] int NOT NULL,
    [UserSign] smallint NULL,
    [ClockDate] datetime NULL,
    [ClockTime] int NULL,
    [ClockType] char(1) NULL,
    [CashID] int NULL,
    CONSTRAINT [PK_APZ_CLOCK] PRIMARY KEY (StoreCode,  PosID,  LineId)
);
