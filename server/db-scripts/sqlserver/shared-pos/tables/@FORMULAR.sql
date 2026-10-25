-- converted from HANA catalog export (@FORMULAR); review before production use
IF OBJECT_ID(N'dbo.[@FORMULAR]') IS NULL
CREATE TABLE dbo.[@FORMULAR] (
    [Code] nvarchar(50) NOT NULL,
    [Name] nvarchar(100) NOT NULL,
    [U_Description] nvarchar(254) NULL,
    [U_ObjType] nvarchar(50) NULL,
    [U_TableName] nvarchar(254) NULL,
    [U_FieldName] nvarchar(100) NULL,
    [U_FieldType] nvarchar(10) NULL DEFAULT 'STR',
    [U_FormularType] nvarchar(10) NULL DEFAULT 'UI',
    [U_DependTable] nvarchar(254) NULL,
    [U_DependField] nvarchar(100) NULL,
    [U_Formular] nvarchar(max) NULL,
    CONSTRAINT [PK_@FORMULAR] PRIMARY KEY (Code)
);
