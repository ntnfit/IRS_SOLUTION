-- converted from HANA catalog export (APZ_UDR1); review before production use
IF OBJECT_ID(N'dbo.[APZ_UDR1]') IS NULL
CREATE TABLE dbo.[APZ_UDR1] (
    [Code] nvarchar(50) NOT NULL,
    [LineNum] int NOT NULL,
    [ParamName] nvarchar(50) NULL,
    [Label] nvarchar(100) NULL,
    [Type] nvarchar(20) NULL,
    [DataType] nvarchar(50) NULL,
    [FormatString] nvarchar(20) NULL,
    [SubType] nvarchar(100) NULL,
    [DefaultValue] nvarchar(254) NULL,
    [ToolTip] nvarchar(254) NULL,
    [AllowMultiple] tinyint NULL,
    [Visible] tinyint NULL,
    [AllowEdit] tinyint NULL,
    CONSTRAINT [PK_APZ_UDR1] PRIMARY KEY (Code,  LineNum)
);
