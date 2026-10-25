CREATE procedure USP_CFL_DOCUMENTTYPE (
)
as
begin
	if (select count(1) from OBJECTS WHERE OBJECT_TYPE = 'TABLE' AND SCHEMA_NAME = CURRENT_SCHEMA AND OBJECT_NAME = 'APZ_OBJECT_TYPE')>0 then
		drop table APZ_OBJECT_TYPE;
	end if;
		
	Create Table APZ_OBJECT_TYPE (
	 	ID			int,
		ProcName	nvarchar(100),
		DocType		nvarchar(50),
		AppObj		int default -1,
		Transfer	nvarchar(1) default ''
	);
		
	insert into APZ_OBJECT_TYPE (ID,ProcName,DocType,AppObj,Transfer)
	select 1,'Goods Receipt PO','20',-1,'' from dummy union
	select 2,'Goods Return','21',-1,'' from dummy union
	select 3,'A/P Invoice','18',-1,'' from dummy union
	select 4,'A/P Credit Memo','19',-1,'' from dummy union
	
	select 5,'Delivery','15',-1,'' from dummy union
	select 6,'Return','16',-1,'' from dummy union
	select 7,'A/R Invoice','13',-1,'' from dummy union
	select 8,'A/R Credit Memo','14',-1,'' from dummy union
	
	select 9,'Goods Receipt','59',-1,'' from dummy union
	select 10,'Goods Issue','60',-1,'' from dummy union
	
	select 11,'Receipt from Production','59',202,'' from dummy union
	select 12,'Issue for Production','60',202,'' from dummy union
	
	select 13,'Transfer In','67',-1,'I' from dummy union
	select 14,'Transfer Out','67',-1,'O' from dummy union
	
	select 15,'Landed Cost','69',-1,'' from dummy union
	
	select 16,'Revaluation','162',-1,'' from dummy;

	select * from APZ_OBJECT_TYPE;	
end

