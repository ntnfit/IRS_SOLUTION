
Create PROCEDURE APZ_IVCounting_Save(in type char(1))
AS 
BEGIN
	Declare Id int =0;
	Declare LineNumber int = 1;
	create local temporary table #APZ_OINCLocal as (select top 0 * from APZ_OINC);
	exec 'drop table #APZ_OINCLocal;';
	exec 'create local temporary table #APZ_OINCLocal as(select * from #APZ_OINC)';
	
	create local temporary table #APZ_INC1Local as (select top 0 * from APZ_INC1);
	exec 'drop table #APZ_INC1Local;';
	exec 'create local temporary table #APZ_INC1Local as(select * from #APZ_INC1)';
	
	create local temporary table #APZ_INC2Local as (select top 0 * from APZ_INC2);
	exec 'drop table #APZ_INC2Local;';
	exec 'create local temporary table #APZ_INC2Local as(select * from #APZ_INC2)';
	
	if :type='U'
	then
		select ifnull((select max("DocEntry") from #APZ_OINCLocal),0) into Id from dummy;
		delete from APZ_INC1 where "DocEntry" =:Id;
		delete from APZ_INC2 where "DocEntry"=:Id;
		delete from  APZ_OINC where "DocEntry"=:Id;
	else
		select ifnull((select max("DocEntry") from APZ_OINC), 0)+1 into Id from dummy;
		update #APZ_OINCLocal set "DocEntry" = :Id;
	end if;

	
	
	Update #APZ_INC1Local set "DocEntry" = :Id,
							  "InvQty" = OITM."OnHand",
							  "Variance" = #APZ_INC1Local."Qty" - #APZ_INC1Local."InvQty",
							  "Variance%" = Case When "InvQty" = 0 Then 100 Else (Abs(#APZ_INC1Local."Variance")/#APZ_INC1Local."InvQty")*100 End
	FROM #APZ_INC1Local, OITM WHERE #APZ_INC1Local."ItemCode" = OITM."ItemCode";
	
	update #APZ_INC2Local
	set "LineId"=null,"DocEntry"=:Id;
		
	while exists (select 1 from #APZ_INC2Local where "LineId" is null)  do
		update top 1 #APZ_INC2Local set "LineId"=:LineNumber where "LineId" is null;
		LineNumber =:LineNumber+1;			
	end while;

	insert into APZ_INC1 select * from #APZ_INC1Local;
	insert into APZ_INC2 select * from #APZ_INC2Local;
	insert into APZ_OINC select * from #APZ_OINCLocal;

	select :Id,'' from dummy;
	
END;




