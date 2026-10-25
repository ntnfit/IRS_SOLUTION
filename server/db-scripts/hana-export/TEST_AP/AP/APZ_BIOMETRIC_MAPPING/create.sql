CREATE PROCEDURE APZ_Biometric_Mapping
AS
BEGIN

	Declare MchN table (
		MachineNo Int
	);
	--Declare MachineNo Int = 0;
	Declare RowCount Int = 0;
	
	Create local temporary table #APZ_OTEMDATA as (select top 0 * from APZ_OTEM);
	exec 'drop table #APZ_OTEMDATA';
	exec 'create local temporary table #APZ_OTEMDATA as (select * from "#dtUserData");';
	
	Insert Into :MchN Select Distinct "MachineNo" From #APZ_OTEMDATA;
	--Select IfNull((Select Top 1 "MachineNo" From #APZ_OTEMDATA),0) Into MachineNo From Dummy;
	Select IfNull((Select Count(*) From #APZ_OTEMDATA Where "EnrollNo" is not null),0) Into RowCount From Dummy ;
	
	Create local temporary table #tbl as (select Top 0 * from APZ_OTEM);
	Insert Into #tbl Select * From APZ_OTEM Where "MachineNo" in (Select MachineNo From :MchN);
	
	If :RowCount > 0
	Then
		Delete From #APZ_OTEMDATA Where "EnrollNo" Is Null;
		MERGE INTO #APZ_OTEMDATA a Using #tbl b ON a."MachineNo" = b."MachineNo" And a."EnrollNo" = b."EnrollNo"
		When MATCHED Then UPDATE SET a."empNo" = b."empNo"
		When NOT MATCHED Then INSERT ("MachineNo","EnrollNo","EnrollName","empNo") VALUES (b."MachineNo",b."EnrollNo",b."EnrollName",b."empNo");
		
		Select Distinct * From #APZ_OTEMDATA Order by "EnrollNo";
	Else
		Select Distinct * From #tbl Order by "EnrollNo";
	End If;
	
	Drop Table #tbl;
	Drop Table #APZ_OTEMDATA;
END;
