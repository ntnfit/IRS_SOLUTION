-- converted from HANA catalog export (APZ_D_VSTEP); review before production use
IF OBJECT_ID(N'dbo.[APZ_D_VSTEP]') IS NULL
CREATE TABLE dbo.[APZ_D_VSTEP] (
    [Code] int NOT NULL,
    [ObjectKey] nvarchar(100) NULL,
    [Caption] nvarchar(100) NULL,
    [Active] char(1) NULL,
    [OrderId] int NULL,
    CONSTRAINT [PK_APZ_D_VSTEP] PRIMARY KEY (Code)
);
