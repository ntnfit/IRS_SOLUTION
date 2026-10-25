
Create PROCEDURE APZ_Vehicle_Saves
AS 
begin

	declare isExist int=0;
	
	create local temporary table #APZ_VHCELocal as (select top 0 * from APZ_VHCE);
	exec 'drop table #APZ_VHCELocal;';
	exec 'create local temporary table #APZ_VHCELocal as (select * from "#APZ_VHCE");';

	delete from #APZ_VHCELocal where IFNULL("Code", '') = '' and IFNULL("Name", '') = '';

	if (select 1 from #APZ_VHCELocal where IFNULL("Code", '') = '') = 1
	then
		select 700026 ,'Vehicle Code cannot be empty' from dummy;
		return;
	end if;
	
 	select (select count("Code") from #APZ_VHCELocal group by("Code") having count("Code") > 1) into isExist from dummy;
	if (ifnull(isExist,0) > 0)
	then
			select 700026 ,'Vehicle is overlapping with an existing Vehicle' from dummy;
			return;
	end if;

	delete from APZ_VHCE;
	insert into APZ_VHCE("Code", "Name", "VehicleNo", "Type", "Load", "Driver", "Phone", "Remarks", "BPLId") select * from #APZ_VHCELocal;
	
	select 0, '' from dummy;
end;




