CREATE PROCEDURE APZ_Biometric_Save
AS
BEGIN
	
	declare AbsEntry Int = 0;
	
	--Temp_APZ_OTBD
	create local temporary table #APZ_OTBDDATA as (select top 0 * from APZ_OTBD);
	exec 'drop table #APZ_OTBDDATA';
	exec 'create local temporary table #APZ_OTBDDATA as (select * from #APZ_OTBD)';
	--Temp_APZ_TBD1
	create local temporary table #APZ_TBD1DATA as (select top 0 * from APZ_TBD1);
	exec 'drop table #APZ_TBD1DATA';
	exec 'create local temporary table #APZ_TBD1DATA as (select DISTINCT * from #APZ_TBD1)';
	
	Update #APZ_OTBDDATA Set "AbsEntry" = Ifnull((select Max("AbsEntry") from APZ_OTBD),0) + 1 Where Ifnull("AbsEntry",0) <= 0;
	
	Update a Set "FromDate" = b."FromDate", "ToDate" = b."ToDate"
	From #APZ_OTBDDATA a JOIN APZ_OHWP b On a."Period" = b."PeriodCode";
	
	Select IfNull((select top 1 "AbsEntry" from #APZ_OTBDDATA),0) Into AbsEntry From Dummy;

	Update #APZ_TBD1DATA Set "AbsEntry" = :AbsEntry;
	
	If Exists(Select 1 From #APZ_TBD1DATA Where "LineNum" is null)
	Then
		Insert Into #APZ_TBD1DATA("AbsEntry","LineNum","EnrollNo","DateTimeRecord","ClockDate","OriginType","Source","MachineNo","WorkCode","Remarks","FixedNote","LogInstanc") 
		Select "AbsEntry","LineNum","EnrollNo","DateTimeRecord","ClockDate","OriginType","Source","MachineNo","WorkCode","Remarks","FixedNote","LogInstanc" 
		From APZ_TBD1
		Where "AbsEntry" = :AbsEntry;
	End If;
	
	Delete From #APZ_TBD1DATA where "EnrollNo" is null;
	
	create local temporary table #Draft as (select "EnrollNo","DateTimeRecord",ROW_NUMBER() OVER (PARTITION BY "EnrollNo" ORDER BY "DateTimeRecord") as "LineDraft" from #APZ_TBD1DATA);
	
	create local temporary table #MaxTable as (Select "EnrollNo", -1 as "MaxLine" From #APZ_TBD1DATA);
	Update #MaxTable Set "MaxLine" = Ifnull((select Max("LineNum") from #APZ_TBD1DATA),0);
	
	update a set a."LineNum" = Ifnull(b."LineDraft",0) + c."MaxLine" 
	from #APZ_TBD1DATA a join #Draft b on a."EnrollNo" = b."EnrollNo" and a."DateTimeRecord" = b."DateTimeRecord" join #MaxTable c on a."EnrollNo" = c."EnrollNo" where "LineNum" is null;
	 
	DELETE From APZ_OTBD where "AbsEntry" = :AbsEntry;
	DELETE From APZ_TBD1 where "AbsEntry" = :AbsEntry And (APZ_TBD1."EnrollNo", APZ_TBD1."ClockDate") in (select "EnrollNo", "ClockDate" from #APZ_TBD1DATA);
	
	INSERT INTO APZ_OTBD Select * from #APZ_OTBDDATA;
	INSERT INTO APZ_TBD1 Select * from #APZ_TBD1DATA;
	
	Select AbsEntry From Dummy;
	Drop Table #APZ_OTBDDATA;
	Drop Table #APZ_TBD1DATA;
	Drop Table #Draft;
	Drop Table #MaxTable;
	
END
