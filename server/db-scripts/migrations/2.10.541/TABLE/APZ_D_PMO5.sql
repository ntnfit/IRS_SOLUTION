
CREATE TABLE APZ_D_PMO5
(
	"Code" nvarchar(50) NOT NULL,
	"CardCode" nvarchar(50),
	"GroupCode" nvarchar(100) default '-1',
	"CreateDate" timestamp,
    "CreateTime" timestamp,
	"UserSign" int,
    "AttachmentId" nvarchar(100)
)