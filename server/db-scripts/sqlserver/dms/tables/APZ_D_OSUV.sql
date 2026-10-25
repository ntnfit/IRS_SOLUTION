-- converted from HANA catalog export (APZ_D_OSUV); review before production use
IF OBJECT_ID(N'dbo.[APZ_D_OSUV]') IS NULL
CREATE TABLE dbo.[APZ_D_OSUV] (
    [DocEntry] int NOT NULL,
    [PersonName] nvarchar(250) NOT NULL,
    [Address] nvarchar(4000) NULL,
    [Phone] nvarchar(250) NULL,
    [Email] nvarchar(250) NULL,
    [Notes] nvarchar(max) NULL,
    [Rate] int NULL,
    [SurveyQuessionId] int NULL,
    [ClgCode] int NOT NULL,
    [CreateTime] datetime NULL,
    [UpdateTime] datetime NULL,
    CONSTRAINT [PK_APZ_D_OSUV] PRIMARY KEY (DocEntry)
);
