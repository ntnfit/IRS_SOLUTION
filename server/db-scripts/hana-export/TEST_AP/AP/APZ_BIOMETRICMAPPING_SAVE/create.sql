
CREATE PROCEDURE APZ_BiometricMapping_Save
AS
BEGIN

	declare empError int;

	Create local temporary table #APZ_OTEMDATA as (select top 0 * from APZ_OTEM);
	exec 'drop table #APZ_OTEMDATA';
	exec 'create local temporary table #APZ_OTEMDATA as (select * from "#APZ_OTEM")';
	
	select ifnull((select top 1 "empNo" from #APZ_OTEMDATA where ifnull("empNo", 0) <> 0 group by "empNo" having count(1) > 1), -1) into empError from DUMMY;
	if :empError <> -1
	THEN
	
		select 30003, 'Cannot duplicate mapping Employee ' || cast(:empError as nvarchar) from dummy;
		return;
	
	END IF;
	
	Delete from APZ_OTEM where "MachineNo" = (select top 1 "MachineNo" from #APZ_OTEMDATA);
	Insert into APZ_OTEM Select * from #APZ_OTEMDATA a where a."MachineNo" is not null and "empNo" is not null;
	
	select 200, '' from dummy;
END;




