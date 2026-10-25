
Create PROCEDURE APZ_Dashboard_SaveDashboardInForm(in typeSave char(1) default '')
as
begin
	Declare dasId int =0;
	declare userId int=0;
	declare checkCount int = 0;
	
	create local temporary table #APZ_DAB1Data as (select top 0 * from APZ_DAB1);
	EXEC 'insert into #APZ_DAB1Data select * from "#APZ_DAB1";';
	
	create local temporary table #APZ_DAB3Data as (select top 0 * from APZ_DAB3);
	EXEC 'insert into #APZ_DAB3Data select * from "#APZ_DAB3";';
	
	if :typeSave='U'
	THEN
		select IFNULL((select top 1 "Id" from #APZ_DAB1Data),0) into dasId FROM DUMMY;
		SELECT IFNULL((select top 1 "UserSign" from #APZ_DAB1Data),0) into userId FROM DUMMY;
		delete FROM APZ_DAB3 where "DetailId"=:dasId;
		delete FROM APZ_DAB1 where "Id"=:dasId and "UserSign"=:userId;
	else
		
		select ifnull((select max("Id") from APZ_DAB1), 0)+1 into dasId FROM DUMMY;
		update #APZ_DAB1Data set "Id" = :dasId;
		update #APZ_DAB3Data set "DetailId" = :dasId;
	end IF;
	
	select IFNULL((select top 1 1 from APZ_DAB1 a INNER JOIN #APZ_DAB1Data b ON b."Name"=a."Name"),0) into checkCount from DUMMY;
	if checkCount = 1
	THEN

		select 123 ,'Dashboard is overlapping with an existing Dashboard' FROM DUMMY;
		return;

	end IF;
	
	insert into APZ_DAB1 select * from #APZ_DAB1Data;
	insert into APZ_DAB3 select * from #APZ_DAB3Data;
	select 0,'' from DUMMY;
end



