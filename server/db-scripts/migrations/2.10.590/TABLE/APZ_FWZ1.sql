
CREATE TABLE APZ_FWZ1 
(
	"AbsEntry" int NOT NULL,
	"ContainerId" int NOT NULL,
	"Caption" nvarchar(254) NULL,
	"ContainerType" char(1) NULL,
	"ShowCaption" char(1) NULL,
	"ItemId" nvarchar(50) NULL,
	"StructureType" int NULL,
	"DataFrom" char(1) NULL,
	"DataString" ntext NULL,
	"StackGroup" tinyint NULL,
	"VisIndex" int NULL,
	"AddCtxMenu" char(1) default 'Y',
	"ShowCheckCol" char(1) default 'N',
	"Type" Int default 1, --1: Grid, 2: Tree
	PRIMARY KEY("AbsEntry", "ContainerId")
)
