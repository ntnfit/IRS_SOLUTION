
CREATE PROCEDURE APZ_SaveJobTypes
AS
BEGIN
	declare Id int=0;
	
	create local temporary table #APZ_JobTypeLocal as(select top 0 * from APZ_OHJT);
	exec 'drop table #APZ_JobTypeLocal;';
	exec 'create local temporary table #APZ_JobTypeLocal as (select * from "#APZ_JobType");';
	
	delete from APZ_OHJT;
	insert into APZ_OHJT("Id","Name","Remark") select "Id","Name","Remark" from #APZ_JobTypeLocal where IFNULL("Id",0)<>0;
	
	--Update Id for new rows
	Update #APZ_JobTypeLocal Set "Id" = 0 Where IFNULL("Id",0)=0;
	
	select (ifnull((select max("Id") from APZ_OHJT),0)) into Id from DUMMY;
	insert into APZ_OHJT("Id","Name","Remark") select :Id + row_number() over(partition by "Id" order by "Id" asc),"Name","Remark" from #APZ_JobTypeLocal where "Id"=0;
	
END;

