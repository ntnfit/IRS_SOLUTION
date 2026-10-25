CREATE PROCEDURE Machine_Act(
	in StartDatee timestamp,
	in EndDatee	timestamp,
	out TB table (
		"Mã TP"		nvarchar(30),
		"Tên TP"		nvarchar(100),	
		"Mã nhóm"		nvarchar(10),
		"Tên nhóm"		nvarchar(100),
		"Machine Hour (Issue)"	numeric(21,6),
		"Freeze dried Hour (Issue)"	numeric(21,6),
		"Freeze dried Hour (Pro Journal)"		numeric(21,6),
		
		"Machine Hour (Std)"	numeric(21,6),
		
		"Machine (Std)"	numeric(21,6),
		
		-- cột phụ
		"Total Machine Expense"		 numeric(21,6), --Total chi phi
		"Machine (Act)"	numeric(21,6) -- phan bổ
	)
)
as
begin 
	declare StartDate timestamp = :StartDatee;
	declare EndDate timestamp = :EndDatee;
	declare total_Machine_Hour numeric(21,6);
----------------------------------------------------#Detail
	create local temporary table #Detail (
		TransType				nvarchar(20),
		DocNumPO				nvarchar(20),
		DocNum		nvarchar(20),
		DocEntry	nvarchar(20),
		ItemCode	nvarchar(30),
		ItmsGrpCod	nvarchar(30),
		ItemName	nvarchar(100),
		Qty			numeric(21,6),
		LineTotal	numeric(21,6),
		
		PItemCode	nvarchar(30),
		WipAct		nvarchar(30),
		Warehouse	nvarchar(10),
		Type		nvarchar(1),
		ID			int,
		Pro_JournalID	int
	);
	insert into #Detail (DocNumPO,DocNum,DocEntry,ItemCode,ItemName,Qty,LineTotal,PItemCode,Type,ID,Pro_JournalID)
	select a."BaseRef",b."DocNum",b."DocEntry",a."ItemCode",a."Dscription",a."Quantity",a."LineTotal",c."ItemCode",c."Type",
	ROW_NUMBER() OVER (PARTITION BY a."BaseRef",b."DocNum",b."DocEntry")--,c."U_Pro_Journal"--,e.""Freeze dried Hour (Pro Journal)""
	, case when e."U_ItemCode" = a."ItemCode" then c."U_Pro_Journal" end

	from IGE1 a
	left join OIGE b on b."DocEntry" = a."DocEntry"
	left join OWOR c on c."DocEntry" = a."BaseEntry"
	left join "@PRO_JOURNAL_H" d on d."DocEntry" = c."U_Pro_Journal"
	left join "@PRO_JOURNAL_L" e on e."DocEntry" = d."DocEntry"
	where a."DocDate"  between StartDate and EndDate 
	and c."Type" is not null
	--and b."DocEntry" = 313
	order by a."DocDate"
	;
	update x set ItmsGrpCod = (select "ItmsGrpCod" from OITM where "ItemCode" = x.ItemCode) from #Detail x;
	--------------------------------#JE
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
	and a."ContraAct" like '154%'
	and not exists (select t."TransId" from OJDT t where t."StornoToTr" = a."TransId" or (t."StornoToTr" is not null and a."TransId" = t."TransId")) 
	--and a."ContraAct" like '154%'
	order by a."RefDate"
	;
	
	
	update x set  DocNumIssue =  (select distinct DocNum from #Detail where DocEntry = x.CreatedBy) from #JE x;
	
	
	update x set  ItemCode =  (select distinct ItemCode from #Detail where DocEntry = x.CreatedBy and ID =x.ID) from #JE x;
	update x set  ItmsGrpCod =  (select distinct "ItmsGrpCod" from OITM where "ItemCode" = x.ItemCode) from #JE x;
	
	--------------------------------------------------------#tblDetail
	create local temporary table #tblDetail (
		PItemCode		nvarchar(30),
		PItemName		nvarchar(100),	
		PItmsGrpCod		nvarchar(10),
		PItmsGrpNam		nvarchar(100),
		ItemCode		nvarchar(30),
		ItemName 		nvarchar(150),
		ItmsGrpCod		nvarchar(10),
		ItmsGrpNam		nvarchar(100),
		
		--So luong
		"Machine Hour (Issue)"	numeric(21,6),
		"Freeze dried Hour (Issue)"	numeric(21,6),
		
		"Freeze dried Hour (Pro Journal)"		numeric(21,6),
		
		--Chi phi
		"Machine (Std)"	numeric(21,6),

		-- cột phụ
		TotalQty			 numeric(21,6),
		TotalChiPhi			 numeric(21,6)
		
		
	);
	insert into #tblDetail (PItemCode,PItmsGrpCod,ItemCode,ItmsGrpCod)
	    select a."ItemCode",a."ItmsGrpCod",b."ItemCode",b."ItmsGrpCod"
		from OITM a,OITM b
		where a."ItmsGrpCod" in (121,120) and b."ItmsGrpCod" in (124,123)
		and a."ItemCode" in (select distinct PItemCode from #Detail where PItemCode is not null);
	--select * from OITB
	
	
	
-------------------------------------------
	update x set TotalQty = (select sum(Qty) from #Detail where ItemCode in (select ItemCode from #tblDetail) and ItmsGrpCod = '124' and Type not in ('D')) from #tblDetail x;
	--update x set TotalChiPhi = ((select sum(Debit) from #JE where  Account like '6279') - (select sum(Credit) from #JE where  Account like '6279')) from #tblDetail x;
	update x set TotalChiPhi = ((select sum(Debit - Credit) from #JE where  Account = '627402' ) 
								 + (select sum(Debit - Credit) from #JE where  Account = '6279')) from #tblDetail x; 							 
										 
-----------------UPdate So Luong bang #tblDetail
	------------"Machine Hour (Issue)" 9
	update x set "Machine Hour (Issue)" = (select sum(Qty) from #Detail where ItemCode = x.ItemCode and Type not in ('D') and PItemCode = x.PItemCode  and ItmsGrpCod = 124) from #tblDetail x ;
	--------------"Freeze dried Hour (Issue)"
	update x set "Freeze dried Hour (Issue)" =   
	(	with tbl as (
			select a."BaseRef",b."DocNum",b."DocEntry",a."ItemCode" as "ItemCode",f."ItmsGrpCod",a."Dscription",a."Quantity",a."LineTotal",c."ItemCode" as "PItemCode",c."Type",
			ROW_NUMBER() OVER (PARTITION BY a."BaseRef",b."DocNum",b."DocEntry") as "ID"--,c."U_Pro_Journal"--,e.""Machine Running Hour (Pro Journal)""
			, case when e."U_ItemCode" = a."ItemCode" then c."U_Pro_Journal" end as "U_Pro_Journal"
			
			from IGE1 a
			left join OIGE b on b."DocEntry" = a."DocEntry"
			left join OWOR c on c."DocEntry" = a."BaseEntry"
			left join "@PRO_JOURNAL_H" d on d."DocEntry" = c."U_Pro_Journal"
			left join "@PRO_JOURNAL_L" e on e."DocEntry" = d."DocEntry"
			left join OITM f on f."ItemCode" = a."ItemCode"
			where a."DocDate"  between StartDate and EndDate
			and c."Type" is not null
		)
		select sum("Quantity") from tbl where   "U_Pro_Journal" is not  null and "PItemCode" = x.PItemCode and "ItemCode" = x.ItemCode and "ItmsGrpCod" = 124
		
     
     )from #tblDetail x ;

	------------"Freeze dried Hour (Pro Journal)" 	
	update x set "Freeze dried Hour (Pro Journal)" =   
	(			
	
		 --select b."DocEntry",a."U_ItemCode",a.""Freeze dried Hour (Pro Journal)"",c."DocNum"
	     select sum(a."U_Machine") 
	     from "@PRO_JOURNAL_L" a
	     left join "@PRO_JOURNAL_H" b on b."DocEntry" = a."DocEntry"
	     left join OWOR c on c."U_Pro_Journal" = b."DocEntry"
	     left join OITM d on d."ItemCode" = a."U_ItemCode"
	     where c."DocNum" in (select distinct DocNumPO from #Detail where DocNumPO is not null) and a."U_ItemCode" = x.ItemCode and c."ItemCode"  = x.PItemCode  and d."ItmsGrpCod" = 124
     
     )from #tblDetail x ;
----------------------------------------UPdate Chi phi bang #tblDetail 
	---------update "Machine (Std)"
	update x set "Machine (Std)" = (select sum(LineTotal) from #Detail where ItemCode = x.ItemCode and Type not in ('D') and PItemCode = x.PItemCode  and ItmsGrpCod = 124) from #tblDetail x ;


----------------------------------------UPdate Chi phi bang #tblDetail End

	---------update thong tin Name
	update x set ItmsGrpCod = (select "ItmsGrpCod" from OITM where "ItemCode" = x.ItemCode) from #tblDetail x;
	update x set ItmsGrpNam = (select "ItmsGrpNam" from OITB where "ItmsGrpCod" = x.ItmsGrpCod) from #tblDetail x;
	update x set PItemName = (select "ItemName" from OITM where "ItemCode" = x.PItemCode) from #tblDetail x;
	update x set ItemName = (select "ItemName" from OITM where "ItemCode" = x.ItemCode) from #tblDetail x;
	
	update x set PItmsGrpCod = (select "ItmsGrpCod" from OITM where "ItemCode" = x.PItemCode) from #tblDetail x;
	update x set PItmsGrpNam = (select "ItmsGrpNam" from OITB where "ItmsGrpCod" = x.PItmsGrpCod) from #tblDetail x;
	
	----------------------------#tblTotal
	create local temporary table #tbl(
		PItemCode		nvarchar(30),
		PItemName		nvarchar(100),	
		PItmsGrpCod		nvarchar(10),
		PItmsGrpNam		nvarchar(100),
		"Machine Hour (Issue)"	numeric(21,6),
		"Freeze dried Hour (Issue)"	numeric(21,6),
		"Freeze dried Hour (Pro Journal)"		numeric(21,6),
		
		"Machine Hour (Std)"	numeric(21,6),
		
		"Machine (Std)"	numeric(21,6),
		
		-- cột phụ
		TotalQty		 numeric(21,6),
		"Total Machine Expense"		 numeric(21,6), --Total chi phi
		"Machine (Act)"	numeric(21,6) -- phan bổ
		
		
	);
	insert into #tbl (PItemCode,PItemName,PItmsGrpCod,PItmsGrpNam,"Machine Hour (Issue)","Freeze dried Hour (Issue)","Freeze dried Hour (Pro Journal)","Machine (Std)",TotalQty,"Total Machine Expense"	)
	select PItemCode,PItemName,PItmsGrpCod,PItmsGrpNam,sum("Machine Hour (Issue)"),sum("Freeze dried Hour (Issue)"),sum("Freeze dried Hour (Pro Journal)"),sum("Machine (Std)"),TotalQty,TotalChiPhi
	from #tblDetail
	group by PItemCode,PItemName,PItmsGrpCod,PItmsGrpNam,TotalQty,TotalChiPhi
	;
	
	--update "Machine Hour (Std)"
	update x set "Machine Hour (Std)" = ( ifnull("Machine Hour (Issue)",0) - ifnull("Freeze dried Hour (Issue)",0) + ifnull("Freeze dried Hour (Pro Journal)",0)) from #tbl x;
	update x set "Machine (Act)" = ( "Machine Hour (Std)" / 
		CASE WHEN (select sum("Machine Hour (Std)") from #tbl) * "Total Machine Expense" = 0 THEN 1 END  
	) from #tbl x;
	
	update x set "Machine (Act)" = (
	case when  "Total Machine Expense" - (select sum("Machine (Act)") from #tbl) > 0 then "Machine (Act)" + ("Total Machine Expense" - (select sum("Machine (Act)") from #tbl)) end
	) from #tbl x where "Machine (Act)" = (select max("Machine (Act)") from #tbl);

	--them dong tong
	insert into #tbl (PItmsGrpNam,"Machine Hour (Issue)","Freeze dried Hour (Issue)","Freeze dried Hour (Pro Journal)","Machine (Std)","Machine (Act)"	)
	select 'Total',sum("Machine Hour (Issue)"),sum("Freeze dried Hour (Issue)"),sum("Freeze dried Hour (Pro Journal)"),sum("Machine (Std)"),sum("Machine (Act)")
	from #tbl
	group by PItmsGrpNam ;
	
	----
	select sum("Machine Hour (Std)")
	into total_Machine_Hour
	from #tbl
	where PItmsGrpNam  <>  'Total';

	--
	update a 
	set "Machine (Act)" = round("Total Machine Expense"/:total_Machine_Hour * "Machine Hour (Std)",2)
	from #tbl a
	where :total_Machine_Hour * "Machine Hour (Std)" > 0;
	

	-------------Show
	select * from #Detail; 
	select * from #JE ;
	select * from #tblDetail;
	
	TB = select 
	PItemCode as "Mã TP",
	PItemName as "Tên TP",
	PItmsGrpCod as "Mã nhóm" ,
	PItmsGrpNam as "Tên nhóm",
	"Machine Hour (Issue)",
	"Freeze dried Hour (Issue)",
	"Freeze dried Hour (Pro Journal)",
	"Machine Hour (Std)",
	"Machine (Std)",
	"Total Machine Expense",
	"Machine (Act)"
	from #tbl;	
	
	drop table #Detail;
	drop table #JE;
	drop table #tblDetail;
	drop table #tbl;
/*
	Luu ý:
	1. Bảng #Detail nếu có phiếu journal xuất hiện 2 mã dưới line trùng nhau thì số liệu sẽ sum  sai
*/	
end;

