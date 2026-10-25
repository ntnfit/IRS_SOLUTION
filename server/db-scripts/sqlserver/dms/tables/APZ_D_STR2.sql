-- converted from HANA catalog export (APZ_D_STR2); review before production use
IF OBJECT_ID(N'dbo.[APZ_D_STR2]') IS NULL
CREATE TABLE dbo.[APZ_D_STR2] (
    [Id] int NOT NULL,
    [RouteCode] varchar(50) NOT NULL,
    [EmpId] int NOT NULL,
    [CardCode] nvarchar(50) NOT NULL,
    [Remarks] nvarchar(254) NULL,
    [LogInstance] int NULL,
    CONSTRAINT [PK_APZ_D_STR2] PRIMARY KEY (Id,  RouteCode,  CardCode,  EmpId)
);
