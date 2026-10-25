
CREATE PROCEDURE APZ_CapacitySetting_Save
AS
BEGIN
	
	Create Local Temporary Table #dtGeneral as (Select * From APZ_CPCS Where 1 = 0);
	exec 'drop table #dtGeneral;';
	exec 'create local temporary table #dtGeneral as (select * from "#tempGeneral");';
	
	Create Local Temporary Table #dtTooltip as (Select * From APZ_CPCS1 Where 1 = 0);
	exec 'drop table #dtTooltip;';
	exec 'create local temporary table #dtTooltip as (select * from "#tempTooltip");';
	
	Delete From APZ_CPCS;
	Delete From APZ_CPCS1;
	
	Insert Into APZ_CPCS select * from #dtGeneral;
	Insert Into APZ_CPCS1 select * from #dtTooltip;

END

