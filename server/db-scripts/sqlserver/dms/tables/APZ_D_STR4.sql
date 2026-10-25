-- converted from HANA catalog export (APZ_D_STR4); review before production use
IF OBJECT_ID(N'dbo.[APZ_D_STR4]') IS NULL
CREATE TABLE dbo.[APZ_D_STR4] (
    [RouteCode] nvarchar(50) NOT NULL,
    [Id] int NOT NULL,
    [EmpId] int NOT NULL,
    [LogInstance] int NULL,
    CONSTRAINT [PK_APZ_D_STR4] PRIMARY KEY (RouteCode,  Id,  EmpId)
);
