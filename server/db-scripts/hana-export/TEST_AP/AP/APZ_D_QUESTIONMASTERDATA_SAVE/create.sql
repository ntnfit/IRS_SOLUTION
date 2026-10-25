

CREATE PROCEDURE APZ_D_QuestionMasterData_Save(in actionType char(1) default '')
AS
BEGIN

	declare id int = -1;
	
	--Create temporary table
	create local temporary table #APZ_D_OCHMLocal as (select top 0 * from APZ_D_OCHM);
	exec 'drop table #APZ_D_OCHMLocal';
	exec 'create local temporary table #APZ_D_OCHMLocal as (select * from #APZ_D_OCHM)';
	
	create local temporary table #APZ_D_CHM1Local as (select top 0 * from APZ_D_CHM1);
	exec 'drop table #APZ_D_CHM1Local';
	exec 'create local temporary table #APZ_D_CHM1Local as (select * from #APZ_D_CHM1)';

	--When action is Update
	if :actionType = 'U'
	then
		select (select top 1 "DocEntry" from #APZ_D_OCHMLocal) into id from dummy;
		delete from APZ_D_CHM1 where "DocEntry" = :id;
		delete from APZ_D_OCHM where "DocEntry" = :id;
	--When action is Add New
	else 
		select (select IFNULL(MAX("DocEntry"), 0) from APZ_D_OCHM) + 1 into id from dummy;
	end if; 
	
	--Update DocEntry of all child
	update #APZ_D_CHM1Local set "DocEntry" = :id;
	
	--Insert new data
	insert into APZ_D_OCHM("DocEntry", "UserSign", "Status", "CreateTime", "UpdateTime", "Remark") select * from #APZ_D_OCHMLocal;
	insert into APZ_D_CHM1("DocEntry", "LineNum", "VisOrder", "Question") select * from #APZ_D_CHM1Local;
	
	select :id, '' from dummy;

END




