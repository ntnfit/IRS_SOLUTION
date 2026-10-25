
CREATE TABLE APZ_CPCS
(
	"DocEntry" int NOT NULL,
	"DragType" char(1) NULL,
	"Ex1Qry" nclob NULL,
	"Ex2Qry" nclob NULL,
	"Ex3Qry" nclob NULL,
	"Ex4Qry" nclob NULL,
	"Ex5Qry" nclob NULL,
	"NotCompColor" int NULL,
	"CompColor" int NULL,
	"GenPoColor" int NULL,
	"CloseColor" int NULL,
	"ApWdth" int NULL,
	"TlvSpaWdth" int NULL,
	"RsPerPag" int NULL,
	"ApVisSta" char(1) NULL,
	"ApVisEnd" char(1) NULL,
	"CancelColor" int null,
	"ApHeight" int NULL,
	PRIMARY KEY ("DocEntry")
);

GO


