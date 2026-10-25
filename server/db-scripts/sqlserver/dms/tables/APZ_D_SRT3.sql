-- converted from HANA catalog export (APZ_D_SRT3); review before production use
IF OBJECT_ID(N'dbo.[APZ_D_SRT3]') IS NULL
CREATE TABLE dbo.[APZ_D_SRT3] (
    [DocEntry] int NOT NULL,
    [DashboardId] int NOT NULL,
    [DashboardName] nvarchar(100) NULL,
    [Layout] nvarchar(max) NULL
);
