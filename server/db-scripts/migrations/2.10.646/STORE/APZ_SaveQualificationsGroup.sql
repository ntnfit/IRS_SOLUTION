
CALL APZ_EXISTSPROC('APZ_SaveQualificationsGroup');

GO

CREATE PROCEDURE APZ_SaveQualificationsGroup
AS
BEGIN
 	Declare Id int=0;

	/* Create new temporary table */
 	Create local temporary table #APZ_QuaGroupLocal as(select top 0 * from APZ_OHQG);
 	exec 'drop table #APZ_QuaGroupLocal;';
	exec 'create local temporary table #APZ_QuaGroupLocal as (select * from "#APZ_QuaGroup");';

	Delete From APZ_OHQG;
	Insert into APZ_OHQG("Id","Name","Remark") select "Id","Name","Remark" from #APZ_QuaGroupLocal where IFNULL("Id",0)<>0;
	
	--Update Id for new rows
	Update #APZ_QuaGroupLocal Set "Id" = 0 Where IFNULL("Id",0)=0;
	select (ifnull((select max("Id") from APZ_OHQG),0)) into Id from DUMMY;
	insert into APZ_OHQG("Id","Name","Remark") select :Id + row_number() over(Partition by "Id" Order By "Id" asc),"Name","Remark" from #APZ_QuaGroupLocal where "Id"=0;
	
 	select 0,'' from DUMMY;
END;