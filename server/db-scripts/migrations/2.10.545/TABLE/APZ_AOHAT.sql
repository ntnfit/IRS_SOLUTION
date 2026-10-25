
CREATE TABLE APZ_AOHAT 
(
	"Code" integer NOT NULL, 
	"Name" nvarchar(254) NOT NULL, 
	"Remarks" nclob NULL, 
	"LogInstanc" integer NULL, 
	"UserSign2" integer NULL, 
	"UpdateDate" timestamp NULL
);

GO

ALTER TABLE APZ_AOHAT ADD CONSTRAINT "PK_APZ_AOHAT" PRIMARY KEY ("Code", "LogInstanc");