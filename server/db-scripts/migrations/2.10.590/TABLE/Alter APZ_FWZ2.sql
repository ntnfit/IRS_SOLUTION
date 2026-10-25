
alter table APZ_FWZ2 add ("SumType" char(1) null default 'N');

GO

alter table APZ_FWZ2 add ("DisplayFormat" nvarchar(254) null);

GO

alter table APZ_FWZ2 add ("MultiSelect" char(1) null default 'N');

GO

alter table APZ_FWZ2 add ("IsParentFieldName" char(1) null default 'N');
