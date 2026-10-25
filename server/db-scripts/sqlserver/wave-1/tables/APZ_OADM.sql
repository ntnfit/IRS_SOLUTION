-- converted from HANA catalog export (APZ_OADM); review before production use
IF OBJECT_ID(N'dbo.[APZ_OADM]') IS NULL
CREATE TABLE dbo.[APZ_OADM] (
    [DisableSplitFolder] nvarchar(2) NULL DEFAULT 'N',
    [AutoSaveDraftApproval] varchar(1) NULL DEFAULT 'N',
    [TaskAsgn] varchar(1) NULL DEFAULT 'N',
    [DflLng] nvarchar(50) NULL,
    [CheckPromotion] char(1) NULL,
    [SearchDate] int NULL DEFAULT 15,
    [CheckOpenFormPR] char(1) NULL DEFAULT 'N',
    [IngoreActiveEmp] char(1) NULL DEFAULT 'N'
);
