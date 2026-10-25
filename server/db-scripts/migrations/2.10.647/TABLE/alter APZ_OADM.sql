
alter table APZ_OADM add ("CheckPromotion" char(1) null);

GO

alter table APZ_OADM add ("SearchDate" int default 15);

GO

Alter Table APZ_OADM add("CheckOpenFormPR" char(1) default 'N');
