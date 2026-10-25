CREATE procedure General_production_costs(
in StartDatee timestamp,
	in EndDatee	timestamp,
	out TB table (
		ItemCode nvarchar(30),
		ItemName 	nvarchar(100),
		ItmsGrpCod	nvarchar(30),
		ItmsGrpNam	nvarchar(100),
		"FG Cmplt Qty" numeric(21,6),
	    "FG Cmplt Amount"	numeric(21,6),
	    TotalQty	numeric(21,6),
	    
	    
	    "Debit (627)" numeric(21,6),
	    "Debit (6279)"		numeric(21,6),
	    "Credit (627)"		numeric(21,6),
	    "Credit (6279)"		numeric(21,6),
	    
	    "Total General Cost"	numeric(21,6),
	    
	    "General production costs" numeric(21,6)
	)
)
as
begin 
	declare StartDate timestamp = :StartDatee;
	declare EndDate timestamp = :EndDatee;
	
	call Finish_Goods ('2023-06-01','2023-09-01',TB=>RS);
	
	create local temporary table #Data (
		ItemCode nvarchar(30),
		ItemName 	nvarchar(100),
		ItmsGrpCod	nvarchar(30),
		ItmsGrpNam	nvarchar(100),
		"FG Cmplt Qty" numeric(21,6),
	    "FG Cmplt Amount"	numeric(21,6),
	    TotalQty	numeric(21,6),
	    
	    
	    "Debit (627)" numeric(21,6),
	    "Debit (6279)"		numeric(21,6),
	    "Credit (627)"		numeric(21,6),
	    "Credit (6279)"		numeric(21,6),
	    
	    "Total General Cost"	numeric(21,6),
	    
	    "General production costs" numeric(21,6)
	    
	    
	);
	insert into #Data (ItemCode,ItemName,ItmsGrpCod,ItmsGrpNam,"FG Cmplt Qty","FG Cmplt Amount")
	select ItemCode,ItemName,ItmsGrpCod,ItmsGrpNam,"FG Cmplt Qty" ,"FG Cmplt Amount" from :RS;
	
	--xóa dòng tổng
	delete from #Data where ItemCode is null;
	
	-- update gias trij coojt Total Qty
	update x set TotalQty =  (select sum("FG Cmplt Qty") from #Data) from #Data x;

	-----------#JE
	create local temporary table #JE (
		TransId		int,
		TransType	int,
		Account		nvarchar(25),
		ShortName	nvarchar(25),
		ContraAct	nvarchar(25),
		LineID		int,
		ID			int,
		
		Debit		numeric(21,6),
		Credit		numeric(21,6),
		RefDate		timestamp,
		
		ItemCode	nvarchar(30),
		ItmsGrpCod	nvarchar(30),
		CreatedBy	int,
		
		
		
		DocEntryIssue	int,
		DocNumIssue		int,
		DocNumPo		int
		
	);
	insert into #JE (TransId,TransType,Account,ShortName,ContraAct,LineID,ID,Debit,Credit,RefDate,CreatedBy)
	select	a."TransId", a."TransType", a."Account", a."ShortName", a."ContraAct", a."Line_ID",
	ROW_NUMBER() OVER (PARTITION BY a."TransId",a."TransType")
	,a."Debit", a."Credit", a."RefDate",a."CreatedBy"--,b.DocEntry,b.DocNum,b.DocNumPo
	from JDT1 a
	--inner join #Detail b on b.DocEntry = a."CreatedBy"
	where a."RefDate" between StartDate and EndDate 
	--and a."TransId" = 808 
	--and a."Account" like '6229' 
	--and a."ContraAct" like '154%'
	and not exists (select t."TransId" from OJDT t where t."StornoToTr" = a."TransId" or (t."StornoToTr" is not null and a."TransId" = t."TransId")) 
	--and a."ContraAct" like '154%'
	order by a."RefDate"
	;
	
	
	-- update Debit 627
	update x set "Debit (627)" = (select sum(Debit) from #JE where Account like '627%') from #Data x;
	update x set "Debit (6279)" = (select sum(Debit) from #JE where Account like '6279') from #Data x;
	
	
	update x set "Credit (627)" = (select sum(Credit) from #JE where Account like '627%') from #Data x;
	update x set "Credit (6279)" = (select sum(Credit) from #JE where Account like '6279') from #Data x;
	
	
	update x set "Total General Cost" = ( (ifnull("Debit (627)",0) - ifnull("Debit (6279)",0)) - (ifnull("Credit (627)",0) - ifnull("Credit (6279)",0))) from #Data x;
	
	
	update x set "General production costs" = (
	Case when TotalQty > 0 then "FG Cmplt Qty" / TotalQty * "Total General Cost" else 0  end
	) from #Data x;
	
	
	--update làm tròn
	update x set "General production costs" = (
	case when  "Total General Cost" - (select sum("General production costs") from #Data) > 0 then "General production costs" + ("Total General Cost" - (select sum("General production costs") from #Data)) end
	) from #Data x where "General production costs" = (select max("General production costs") from #Data);
	
	/*
	--thêm dòng tổng
	insert into #Data (ItmsGrpNam,"Debit (627)","Debit (6279)","Credit (627)","Credit (6279)","General production costs")
	select 'Total',sum("Debit (627)"),sum("Debit (6279)"),sum("Credit (627)"),sum("Credit (6279)"),sum("General production costs")
	from #Data;
	--group by ItmsGrpNam
	*/
	
	
	
	
	
	
	TB = select * from #Data;
	
	
	drop table #Data;
	drop table #JE;
end;

