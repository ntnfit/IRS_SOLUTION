-- converted from HANA catalog export (APZ_D_STR3); review before production use
IF OBJECT_ID(N'dbo.[APZ_D_STR3]') IS NULL
CREATE TABLE dbo.[APZ_D_STR3] (
    [Id] int NOT NULL,
    [RouteCode] varchar(50) NOT NULL,
    [EmpId] int NOT NULL,
    [CardCode] nvarchar(50) NOT NULL,
    [Remarks] nvarchar(254) NULL,
    [LogInstance] int NULL,
    CONSTRAINT [PK_APZ_D_STR3] PRIMARY KEY (Id,  RouteCode,  CardCode)
);
