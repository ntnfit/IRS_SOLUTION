
alter table APZ_OUDR add ("AllRowsInGrp" varchar(1) default 'N');

GO

alter table APZ_OUDR add ("ShChkBSelect" char(1) default 'Y');

GO

alter table APZ_OUDR add ("EnbExlHeader" char(1) default 'N');

GO

alter table APZ_OUDR add ("ExcelHeader" nvarchar(4000));

GO

alter table APZ_OUDR add ("ExlHeaderAlign" char(1) default 'L');