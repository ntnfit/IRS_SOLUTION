
CREATE TABLE APZ_OADM
(
	"DisableSplitFolder" nvarchar(2) DEFAULT 'N',
	"AutoSaveDraftApproval" varchar(1) DEFAULT 'N',
	"TaskAsgn" varchar(1) DEFAULT 'N',
	"DflLng" nvarchar(50) null,
	"CheckPromotion" char(1) default 'N',
	"SearchDate" int default 15
);


GO

INSERT INTO APZ_OADM("DisableSplitFolder") 
	select 'N' from DUMMY where not exists(select 1 from APZ_OADM);