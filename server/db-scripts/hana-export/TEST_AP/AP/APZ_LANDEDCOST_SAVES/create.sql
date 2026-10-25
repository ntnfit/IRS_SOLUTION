
Create procedure APZ_LandedCost_Saves(in type char(1))
AS 
BEGIN
	Declare docId bigint = 0;
	declare attID int=0;
	declare docNum int =0;
	declare sumCost float = 0;
	declare isdocDate int=0;
	declare isCardCode int =0;
	Declare isItemCode int=0;
	Declare tbIPF1 int =0;
	create local temporary table #OIPFLocal as (select top 0 * from OIPF);
	exec 'insert into #OIPFLocal select * from "#OIPF"';
	
	create local temporary table #IPF1Local as (select top 0 * from IPF1);
	exec 'insert into #IPF1Local select * from "#IPF1"';
	
	create local temporary table #IPF2Local as (select top 0 * from IPF2);
	exec 'insert into #IPF2Local select * from "#IPF2"';
	
	--create local temporary table #ATC1Local as (select top 0 * from ATC1);
	--exec 'insert into #ATC1Local select * from "#ATC1"';
	
	create local temporary table #IPF3Local as (select top 0 * from IPF1);
	exec 'insert into #IPF3Local select * from "#IPF3"';
	
	if :type = 'U'
	then
	select ifnull( (select top 1 "DocEntry" from #OIPFLocal),0) into docId from dummy;
	select ifnull((Select "AtcEntry" from #OIPFLocal),0) into attID from dummy;
	delete from IPF1 where "DocEntry" = :docId;
	delete from IPF2 where "DocEntry" = :docId;
	--delete from ATC1 where "AbsEntry" = :attID;
	delete from OATC where "AbsEntry"=:attID;
	delete from OIPF where "DocEntry" = :docId;
	else 
	
		select  ifnull((select MAX("DocEntry") from OIPF), 0) + 1 into docId from dummy;

		update #OIPFLocal set "DocEntry" = :docId;
		select  ifnull((select MAX("AbsEntry") from OATC), 0) + 1 into attID from dummy;
		Insert into OATC values( :attID);
		update #OIPFLocal set "AtcEntry" = :attID;
		
		Select ifnull((select MAX("DocNum") from OIPF),0)+1 into docNum from dummy;
		update #OIPFLocal set "DocNum" = :docNum;

	
	end if;
	
	select ifnull((select Sum("CostSum") from #IPF2Local),0) into sumCost from dummy ;
	if(:sumCost=0)
	then

			select 120 ,'Both expenditure and custom expenses are missing' from dummy;
			return;

	end if;
  select count(1) into isdocDate from #OIPFLocal a where a."DocDate" > a."DocDueDate";
	if (ifnull(isdocDate,0)=1) 
	then

			select 121 ,'Doc Date cannot be later than Doc Due Date'from  dummy;
			return;

	END if;
	select count(1) into isCardCode from #OIPFLocal where  IFNULL("CardCode",'') = '' ;
	if (ifnull(isCardCode,0)=1)
	then

			select 123 ,'Vendor code is missing' from dummy;
			return;

	end if;
	select count(1) into isItemCode from #IPF1Local where  IFNULL("ItemCode",'') = '' ;
	if (ifnull(isItemCode,0)=1)
	then
	select 124,'Item Code code is missing' from dummy;
	end if;
	select count(1) into tbIPF1 from #IPF1Local ;
	if(ifnull(tbIPF1,0)=0 )
	then
	select 125,'Not enough data' from dummy;
	end if;

		insert into OIPF select * from #OIPFLocal ;

		UPDATE #IPF1Local set "DocEntry" = :docId;
		
		UPDATE  #IPF2Local set "DocEntry" = :docId;
	
		--update #ATC1Local set "AbsEntry" = :attID,"Line"= NumId  FROM (SELECT *,  ROW_NUMBER() OVER( ORDER BY "AbsEntry") NumId FROM #ATC1Local ) ;
	

	insert into IPF1 select*from #IPF1Local;
	update a set "PriceList"=d."PriceList", "CostOH"=d."CostOH" from  IPF1 a inner JOIN (SELECT "PriceList", "CostOH", "ItemCode" FROM #IPF3Local ) d on d."ItemCode"=a."ItemCode";
	insert into IPF2 select*from #IPF2Local;
	--insert into ATC1 select*from #ATC1Local;

	update NNM1 set "NextNumber"=(select MAX("DocNum") from OIPF)+1 where "ObjectCode"='69';

	select :docId, '' from dummy;
	 drop table #OIPFLocal;
	 drop table #IPF1Local;
	 drop table #IPF2Local;
	 --drop table #ATC1Local;
	
END;




