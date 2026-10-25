
create procedure APZ_DiscountGroup_Save(in type char(1))
As
begin
	Declare docId bigint = 0;
	create local temporary table #OEDGLocal as (select top 0 * from OEDG);
	exec 'insert into #OEDGLocal select * from "#OEDG"';
	
	create local temporary table #EDG1Local as (select top 0 * from EDG1);
	exec 'insert into #EDG1Local select * from "#EDG1"';
	
	
	if :type = 'U'
	then
	select ifnull( (select top 1 "AbsEntry" from #OEDGLocal),0) into docId from dummy;
	delete from EDG1 where "AbsEntry" = :docId;
	delete from OEDG where "AbsEntry" = :docId;
	else 
	
		select  ifnull((select MAX("AbsEntry") from OEDG), 0) + 1 into docId from dummy;

		update #OEDGLocal set "AbsEntry" = :docId;
		
	end if;
	

		UPDATE #EDG1Local set "AbsEntry" = :docId;
		
		
	insert into EDG1 select*from #EDG1Local;

	insert into OEDG select*from #OEDGLocal;

	  UPDATE ONNM SET "AutoKey" = docId WHERE "ObjectCode" = '1470000077' ;

	select :docId, '' from dummy;
	 drop table #OEDGLocal;
	 drop table #EDG1Local;
end;




