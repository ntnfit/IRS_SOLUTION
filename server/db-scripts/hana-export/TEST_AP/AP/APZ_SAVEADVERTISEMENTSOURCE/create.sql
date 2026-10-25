
CREATE PROCEDURE APZ_SaveAdvertisementSource
AS
BEGIN
	declare Id int=0;
	create local temporary table #APZ_AdvSourceLocal as(select top 0 * from APZ_OHAS);
 	exec 'drop table #APZ_AdvSourceLocal;';
	exec 'create local temporary table #APZ_AdvSourceLocal as (select * from "#APZ_AdvSource");';
 	
 	delete from APZ_OHAS;
  	insert into APZ_OHAS("Id","Name","Remark") select "Id","Name","Remark" from #APZ_AdvSourceLocal where "Id"<>0;
  
	select ifnull((select max("Id") from APZ_OHAS),0) into Id from DUMMY;
	
  	insert into APZ_OHAS("Id", "Name","Remark")
  	select Id + ROW_NUMBER() OVER(Partition by "Id" ORDER BY "Name" ASC),"Name","Remark" 
  	from #APZ_AdvSourceLocal where "Id"=0;
END;



