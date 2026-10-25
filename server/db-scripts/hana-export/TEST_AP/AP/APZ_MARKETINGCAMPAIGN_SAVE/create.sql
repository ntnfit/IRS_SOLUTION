
create procedure APZ_MarketingCampaign_Save
(
in type char(1)
)
as
begin
Declare Id int =0;
declare attID int=0;
declare rowNumber int = 1;
declare rowCount int = 0;

create local temporary table #APZ_OSMCLocal as (select top 0 * from APZ_OSMC);
exec 'drop table #APZ_OSMCLocal';
exec 'create local temporary table #APZ_OSMCLocal as (select * from #APZ_OSMC)';

create local temporary table #APZ_SMC1Local as (select top 0 * from APZ_SMC1);
exec 'drop table #APZ_SMC1Local';
exec 'create local temporary table #APZ_SMC1Local as (select * from #APZ_SMC1)';

create local temporary table #APZ_SMC2Local as (select top 0 * from APZ_SMC2);
exec 'drop table #APZ_SMC2Local';
exec 'create local temporary table #APZ_SMC2Local as (select * from #APZ_SMC2)';

create local temporary table #APZ_SMC3Local as (select top 0 * from APZ_SMC3);
exec 'drop table #APZ_SMC3Local';
exec 'create local temporary table #APZ_SMC3Local as (select * from #APZ_SMC3)';

create local temporary table #APZ_SMC4Local as (select top 0 * from APZ_SMC4);
exec 'drop table #APZ_SMC4Local';
exec 'create local temporary table #APZ_SMC4Local as (select * from #APZ_SMC4)';

create local temporary table #APZ_SMC5Local as (select top 0 * from APZ_SMC5);
exec 'drop table #APZ_SMC5Local';
exec 'create local temporary table #APZ_SMC5Local as (select * from #APZ_SMC5)';

create local temporary table #ATC1Local as (select top 0 * from ATC1);
exec 'drop table #ATC1Local';
exec 'create local temporary table #ATC1Local as (select * from #ATC1)';

if :type='U' then -- update

	select min("DocEntry") into Id from #APZ_OSMCLocal;
	delete from APZ_SMC1 where "DocEntry" =:Id;
	delete from APZ_SMC2 where "DocEntry" =:Id;
	delete from APZ_SMC3 where "DocEntry" =:Id;
	delete from APZ_SMC4 where "DocEntry" =:Id;
	delete from APZ_SMC5 where "DocEntry" =:Id;
	delete from ATC1 where "AbsEntry" = (select top 1 "AtcEntry" from APZ_OSMC where "DocEntry" = :Id);
	select top 1 "AtcEntry" into attID from APZ_OSMC where "DocEntry" = :Id;
	
	if (select count(1) from #ATC1Local) = 0 then
		delete from OATC where "AbsEntry" = (select top 1 "AtcEntry" from APZ_OSMC where "DocEntry" = :Id);
		update #APZ_OSMCLocal set "AtcEntry" = NULL;
	else
		if ifnull(:attID, 0) = 0 then
			select ifnull(MAX("AbsEntry"), 0) + 1 into attID from OATC;
			update #APZ_OSMCLocal set "AtcEntry" = :attID; 
			insert into OATC("AbsEntry") values (:attID);
		end if;
	end if;
	
	delete from APZ_OSMC where "DocEntry"=:Id;
	
else --add new 

	select ifnull(max("DocEntry"), 0) + 1 into Id from APZ_OSMC;
	update #APZ_OSMCLocal set "DocEntry" = :Id;

	select ifnull(MAX("AbsEntry"), 0) + 1 into attID from OATC;
	if (select count(1) from #ATC1Local) > 0 then
		Insert into OATC("AbsEntry") values(:attID);
		update #APZ_OSMCLocal set "AtcEntry" = :attID;
	end if;
	
end if;

if (select count(1) from APZ_OSMC where "CampaignNo" = (select top 1 "CampaignNo" from #APZ_OSMCLocal)) > 0 then
	select 122 ,'Campaign code  is overlapping with an existing Marketing Campaign' from dummy;
	return;
end if;		


UPDATE #APZ_SMC1Local set "DocEntry" = :Id, "LineNum" = 0; 
UPDATE #APZ_SMC2Local set "DocEntry" = :Id, "LineNum" = 0;
UPDATE #APZ_SMC3Local set "DocEntry" = :Id, "LineNum" = 0; 
UPDATE #APZ_SMC4Local set "DocEntry" =:Id, "LineNum" = 0;
UPDATE #APZ_SMC5Local set "DocEntry" = :Id, "LineNum" = 0;
UPDATE #ATC1Local set "AbsEntry" = :attID, "Line" = 0;

--Update LineNum value
select count(1) into rowCount from #APZ_SMC1Local;
while :rowNumber <= :rowCount do
	update top 1 #APZ_SMC1Local set "LineNum" = :rowNumber where "LineNum" = 0;
	rowNumber = rowNumber + 1;
end while;

rowNumber = 1;
select count(1) into rowCount from #APZ_SMC2Local;
while :rowNumber <= :rowCount do
	update top 1 #APZ_SMC2Local set "LineNum" = :rowNumber where "LineNum" = 0;
	rowNumber = rowNumber + 1;
end while;

rowNumber = 1;
select count(1) into rowCount from #APZ_SMC3Local;
while :rowNumber <= :rowCount do
	update top 1 #APZ_SMC3Local set "LineNum" = :rowNumber where "LineNum" = 0;
	rowNumber = rowNumber + 1;
end while;

rowNumber = 1;
select count(1) into rowCount from #APZ_SMC4Local;
while :rowNumber <= :rowCount do
	update top 1 #APZ_SMC4Local set "LineNum" = :rowNumber where "LineNum" = 0;
	rowNumber = rowNumber + 1;
end while;

rowNumber = 1;
select count(1) into rowCount from #APZ_SMC5Local;
while :rowNumber <= :rowCount do
	update top 1 #APZ_SMC5Local set "LineNum" = :rowNumber where "LineNum" = 0;
	rowNumber = rowNumber + 1;
end while;

rowNumber = 1;
select count(1) into rowCount from #ATC1Local;
while :rowNumber <= :rowCount do
	update top 1 #ATC1Local set "Line" = :rowNumber where "Line" = 0;
	rowNumber = rowNumber + 1;
end while;

insert into APZ_OSMC select * from #APZ_OSMCLocal;
insert into APZ_SMC1 select * from #APZ_SMC1Local;
insert into APZ_SMC2 select * from #APZ_SMC2Local;
insert into APZ_SMC3 select * from #APZ_SMC3Local;
insert into APZ_SMC4 select * from #APZ_SMC4Local;
insert into APZ_SMC5 select * from #APZ_SMC5Local;
insert into ATC1 select * from #ATC1Local;

select :Id, '' from dummy;
	
end;



