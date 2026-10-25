-- converted from HANA catalog export (APZ_OFWZ); review before production use
IF OBJECT_ID(N'dbo.[APZ_OFWZ]') IS NULL
CREATE TABLE dbo.[APZ_OFWZ] (
    [AbsEntry] int NOT NULL,
    [FormId] nvarchar(100) NULL,
    [BaseForm] nvarchar(100) NULL,
    [BaseType] char(1) NULL,
    [Caption] nvarchar(254) NULL,
    [FatherMenu] nvarchar(254) NULL,
    [Valid] char(1) NULL,
    [AcceptButton] char(1) NULL,
    [CancelButton] char(1) NULL,
    [Stack0] char(1) NULL,
    [Stack1] char(1) NULL,
    [Stack2] char(1) NULL,
    [Stack3] char(1) NULL,
    [LayoutCode] nvarchar(10) NULL,
    [FormType] nvarchar(20) NULL,
    CONSTRAINT [PK_APZ_OFWZ] PRIMARY KEY (AbsEntry)
);
