
call apz_existsproc('APZ_TimeSheeting_LoadInOutByAbsEntry');

GO

create procedure APZ_TimeSheeting_LoadInOutByAbsEntry(in AbsEntry int)
as
begin
	Declare period nvarchar(20);
	
	Select a.*,b."firstName" || ',' || b."lastName" as "empName",b."dept"
	From APZ_IOD1 a Left Join OHEM b on a."empID" = b."empID" Where a."AbsEntry" = :AbsEntry;
	Select a.*,b."firstName" || ',' || b."lastName" as "empName",b."dept"
	From APZ_IOD2 a Left Join OHEM b on a."empID" = b."empID" Where a."AbsEntry" = :AbsEntry;
	
	Select "Period" Into period From APZ_OIOD Where "AbsEntry" = :AbsEntry;
	--Check Data has copied
	If (Select Count(1) From APZ_OTBD Where "Period" = :period) = (Select Count(1) From (Select Distinct("BaseEntry") From APZ_OTSL Where "Period" = :period AND IFNULL("BaseEntry",0) != 0) x)
	Then
		If Exists(Select 1 From APZ_IOD2 Where "Period" = :period)
		Then
			If (Select Count(1) From APZ_OTBD Where "Period" = :period) = (Select Count(1) From (Select Distinct("BaseEntry") From APZ_OTSH Where "Period" = :period AND IFNULL("BaseEntry",0) != 0) x) Then
				Select 1 AS "CheckTS", 1 As "CheckTSOT" From Dummy;
			Else
				Select 1 AS "CheckTS", 0 As "CheckTSOT" From Dummy;
			End If; 
		Else
			Select 1 AS "CheckTS", 1 As "CheckTSOT" From Dummy; 
		End if;
	Else
		If Exists(Select 1 From APZ_IOD2 Where "Period" = :period)
		Then
			If (Select Count(1) From APZ_OTBD Where "Period" = :period) = (Select Count(1) From (Select Distinct("BaseEntry") From APZ_OTSH Where "Period" = :period AND IFNULL("BaseEntry",0) != 0) x) Then
				Select 0 AS "CheckTS", 1 As "CheckTSOT" From Dummy;
			Else
				Select 0 AS "CheckTS", 0 As "CheckTSOT" From Dummy; 
			End If; 
		Else
			Select 0 AS "CheckTS", 1 As "CheckTSOT" From Dummy;
		End if;
	End if;
end;