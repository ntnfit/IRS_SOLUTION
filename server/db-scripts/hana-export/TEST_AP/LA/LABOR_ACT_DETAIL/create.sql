CREATE procedure Labor_Act_Detail(
	in StartDatee timestamp,
	in EndDatee	timestamp,
	out TB table (
		PItemCode		nvarchar(30),
		PItemName		nvarchar(100),	
		PItmsGrpCod		nvarchar(10),
		PItmsGrpNam		nvarchar(100),
		ItemCode		nvarchar(30),
		ItemName 		nvarchar(150),
		ItmsGrpCod		nvarchar(10),
		ItmsGrpNam		nvarchar(100),
		
		--So luong
		"Labor Hour (Std)"	numeric(21,6),
	
		
		--Chi phi
		"Labor (Std)"	numeric(21,6),






		-- cột phụ
		TotalQty			 numeric(21,6),
		TotalChiPhi			 numeric(21,6)
	)
)
as
begin 
	declare StartDate timestamp = :StartDatee;
	declare EndDate timestamp = :EndDatee;
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
	
	--delete from #JE where DocNumIssue is null;
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
		"Labor Hour (Std)"	numeric(21,6),
	
		
		--Chi phi
		"Labor (Std)"	numeric(21,6),






		-- cột phụ
		TotalQty			 numeric(21,6),
		TotalChiPhi			 numeric(21,6)
		
		
	);
	insert into #tblDetail (PItemCode,PItmsGrpCod,ItemCode,ItmsGrpCod)
	    select a."ItemCode",a."ItmsGrpCod",b."ItemCode",b."ItmsGrpCod"
		from OITM a,OITM b
		where a."ItmsGrpCod" in (121,120) and b."ItmsGrpCod" in (126)
		and a."ItemCode" in (select distinct PItemCode from #Detail where PItemCode is not null);
	--select * from OITB
	
	
	
-------------------------------------------
	update x set TotalQty = (select sum(Qty) from #Detail where ItemCode in (select ItemCode from #tblDetail) and ItmsGrpCod = '126' and Type not in ('D')) from #tblDetail x;
	
	--update x set TotalChiPhi = (select sum(Credit) from #JE where ItemCode in (select ItemCode from #tblDetail) and Account like '6229' and ContraAct like '154%' and ItmsGrpCod = '126') from #tblDetail x;
	update x set TotalChiPhi = ((select sum(Debit) from #JE where  Account like '6229') - (select sum(Credit) from #JE where  Account like '6229')) from #tblDetail x;
-----------------UPdate So Luong bang #tblDetail
	------------"Labor Hour (Std)" 
	update x set "Labor Hour (Std)" = (select sum(Qty) from #Detail where ItemCode = x.ItemCode and Type not in ('D') and PItemCode = x.PItemCode  and ItmsGrpCod = 126) from #tblDetail x ;
	
----------------------------------------UPdate Chi phi bang #tblDetail 
	---------update "Labor (Std)"
	update x set "Labor (Std)" = (select sum(LineTotal) from #Detail where ItemCode = x.ItemCode and Type not in ('D') and PItemCode = x.PItemCode  and ItmsGrpCod = 126) from #tblDetail x ;


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
		"Labor Hour (Std)"	numeric(21,6),
		
		
		"Labor (Std)"	numeric(21,6),
		
		-- cột phụ
		TotalQty		 numeric(21,6),
		"Total Labor Expense"		 numeric(21,6), --Total chi phi
		"Labor (Act)"	numeric(21,6) -- phan bổ
		
		
	);
	insert into #tbl (PItemCode,PItemName,PItmsGrpCod,PItmsGrpNam,"Labor Hour (Std)","Labor (Std)",TotalQty,"Total Labor Expense"	)
	select PItemCode,PItemName,PItmsGrpCod,PItmsGrpNam,sum("Labor Hour (Std)"),sum("Labor (Std)"),TotalQty,TotalChiPhi
	from #tblDetail
	group by PItemCode,PItemName,PItmsGrpCod,PItmsGrpNam,TotalQty,TotalChiPhi
	;
	update x set "Labor (Act)" = ( case when "Labor Hour (Std)" <> 0 then	"Labor Hour (Std)" / TotalQty * "Total Labor Expense" else 0 end ) from #tbl x;
	
	
	
	update x set "Labor (Act)" = (
	case when  "Total Labor Expense" - (select sum("Labor (Act)") from #tbl) > 0 then "Labor (Act)" + ("Total Labor Expense" - (select sum("Labor (Act)") from #tbl)) end
	) from #tbl x where "Labor (Act)" = (select max("Labor (Act)") from #tbl);
	
	
	--them dong tong
	insert into #tbl (PItmsGrpNam,"Labor Hour (Std)","Labor (Std)","Labor (Act)"	)
	select 'Total',sum("Labor Hour (Std)"),sum("Labor (Std)"),sum("Labor (Act)")
	from #tbl
	group by PItmsGrpNam ;
	
	
	TB = 
	select 
		*
	from #tblDetail;
	-------------Show
	--select * from #Detail; 
	--select * from #JE ;
	--select * from #tblDetail;
	--select * from #tbl;
	
	
	
	drop table #Detail;
	drop table #JE;
	drop table #tblDetail;
	drop table #tbl;
/*
	Luu ý:
	1. Bảng #Detail nếu có phiếu journal xuất hiện 2 mã dưới line trùng nhau thì số liệu sẽ sum  sai
*/	
end;

