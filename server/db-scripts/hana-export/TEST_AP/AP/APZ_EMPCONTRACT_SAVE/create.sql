
CREATE PROCEDURE APZ_EmpContract_Save()
LANGUAGE SQLSCRIPT
AS
Begin
	Declare EmpId Int;
 	
 	create local temporary table #dtAPZ_OHECData as (select * From APZ_OHEC Where 1=0);
	exec 'drop table #dtAPZ_OHECData;';
	exec 'create local temporary table #dtAPZ_OHECData as (select * from "#dtAPZ_OHEC");';
 	
 	Select Top 1 "empID" Into EmpId From #dtAPZ_OHECData;
  	
  	Delete From APZ_OHEC Where "empID" = :EmpId;
  	Insert Into APZ_OHEC Select * From #dtAPZ_OHECData;
  	
  	Select "empID" From APZ_OHEC Where "empID" = :EmpId;
  	Drop Table #dtAPZ_OHECData;
END

