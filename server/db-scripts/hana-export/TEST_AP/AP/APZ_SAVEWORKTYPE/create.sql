
CREATE PROCEDURE APZ_SaveWorkType
AS
BEGIN

	DECLARE sqlString nvarchar(4000000) = '';
	DECLARE Udfcols NVARCHAR(40000) = '';
	DECLARE Id int;
	Create local temporary table #APZ_OHWTDATA as (select top 0 * from APZ_OHWT);
	exec 'drop table #APZ_OHWTDATA';
	exec 'create local temporary table #APZ_OHWTDATA as (select * from #APZ_OHWT)';

	IF Exists(select 1 from APZ_OHWT a inner join #APZ_OHWTDATA b On a."Id" = b."Id"
							where a."Name" <> b."Name" and a."Id" in (select "WorkType" from APZ_OHWC))
	THEN 
		select 1,'You cannot update because being used' from DUMMY;
		return;
	END IF;
	
	delete from APZ_OHWT;
	
	Insert into APZ_OHWT
		Select * from #APZ_OHWTData where ifnull("Id",0) <> 0;
	
	select IFNULL((select Max("Id") from APZ_OHWT),0) into Id from DUMMY;
	if :Id < IFNULL((select Max("Id") from APZ_OHWT),0)
	THEN
		select IFNULL((select Max("Id") from APZ_OHWT),0) into Id from DUMMY;
	END IF;
	
	--Insert into  APZ_OHWT("Id","Name" ,"Typ","Salary")
	--	Select :Id + ROW_NUMBER() OVER(ORDER BY "Name" ASC) ,"Name" ,"Typ","Salary" from #APZ_OHWTDATA where ifnull("Id",0) = 0;
	
	SELECT IFNULL((SELECT string_agg('"' || COLUMN_NAME || '"',',') from TABLE_COLUMNS where SCHEMA_NAME = CURRENT_SCHEMA and TABLE_NAME = N'APZ_OHWT' AND COLUMN_NAME like 'U__%' ESCAPE '_'),'') Into Udfcols From Dummy;
	
	sqlString = N'Insert into  APZ_OHWT("Id","Name" ,"Typ","Salary"'|| case when ifnull(:Udfcols,'') = '' then '' else ',' || :Udfcols end || ')
		Select ' || :Id || ' + ROW_NUMBER() OVER(ORDER BY "Name" ASC) ,"Name" ,"Typ","Salary"'|| case when ifnull(:Udfcols,'') = '' then '' else ',' || :Udfcols end ||' from #APZ_OHWTDATA where ifnull("Id",0) = 0;';
	EXECUTE immediate :sqlstring;

	select 0, '' from DUMMY;
END

