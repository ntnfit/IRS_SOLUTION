
CALL APZ_EXISTSPROC('APZ_Capacity_Save');

GO

CREATE PROCEDURE APZ_Capacity_Save
AS
BEGIN
	Declare code bigint;
	Declare scheCode bigint;
	
	Create Local Temporary Table #dtDataCp as (Select * From APZ_CGRC Where 1 = 0);
	exec 'drop table #dtDataCp;';
	exec 'create local temporary table #dtDataCp as (select * from "#tempDataCp");';
	
	IF Not Exists(select 1 from #dtDataCp)
	Then
	
		Select 80000501, 'Data can not be null' From Dummy;
		Return;
		
	End If;

	Select Ifnull((Select top 1 "GrcId" from #dtDataCp),0) Into code From Dummy;
  	Select Ifnull((Select top 1 "ScheCode" from #dtDataCp),0) Into ScheCode From Dummy;

	Delete From APZ_CGRC where "ScheCode" = :scheCode;
	
	If (:code > 0)
	Then
	
		Delete From APZ_CGRC Where "GrcId" = :code;
		
	else
	
		Declare newCode bigint;
		Select IFNULL((Select Max("GrcId") From APZ_CGRC),0) + 1 Into newCode From Dummy;
		Update #dtDataCp set "GrcId" = :newCode;
		
	End If;

	Insert Into APZ_CGRC Select * From #dtDataCp;
	Select 0, '' From Dummy;
 
END