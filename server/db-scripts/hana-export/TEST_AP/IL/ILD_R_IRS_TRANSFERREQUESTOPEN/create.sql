CREATE procedure ILD_R_IRS_TransferRequestOpen (
	in Fromdate date,
	in Todate date
)
as
begin
create local  temporary  table #tbl
(
	RowNum int,
	DocEntry int,
	DocNum int,
	DocDate date,
	DocStatus varchar(1),
	ItemCode varchar(50),
	ItemName nvarchar(100),
	LineNum int,
	Quantity numeric(19,6),
	--OpenQty_Temp numeric(19,6),	
	OpenQty  numeric(19,6),
	-- Transfer
	TR_DocNum int,
	TR_DocEntry int,
	TR_Qty numeric(19,6),
	TR_WhsCode varchar(50)
);
--
insert into #tbl(RowNum, DocEntry, DocNum,DocDate,DocStatus,  ItemCode, ItemName, LineNum, Quantity,
					--
					TR_DocNum,TR_DocEntry,TR_Qty,TR_WhsCode)
select ROW_NUMBER() OVER( PARTITION BY  Transfer_Request."DocEntry",Transfer_Request."ItemCode" 
						 ORDER BY Transfer_Request."DocEntry"  asc ) as "RowID",
						 
		Transfer_Request."DocEntry", Transfer_Request."DocNum",Transfer_Request."RequestDate",Transfer_Request."DocStatus",
		Transfer_Request."ItemCode", Transfer_Request."ItemName",Transfer_Request."LineNum",
		Transfer_Request."Quantity",
		
							---
							TR."DocNum",
							TR1."DocEntry" as "TR_DocEntry",
							
							TR1."Quantity" as "TR_Qty",
							TR1."WhsCode" as "TR_WhsCode"
							
							
							
from 
					(
					
							select  a."DocEntry" , a."DocNum",a."DocDate" as "RequestDate",a."DocStatus",
									-- line
									b."ItemCode", b."Dscription" as "ItemName",b."LineNum",b."Quantity", b."OpenQty"
							from  OWTQ a
							inner join WTQ1  b on a."DocEntry" = b."DocEntry"
							
							where    "CANCELED" ='N' 
														and  a."DocDate" between Fromdate and Todate) as Transfer_Request
					
left join WTR1 as TR1 on TR1."BaseEntry"||TR1."BaseLine"||TR1."ItemCode"  = Transfer_Request."DocEntry"||Transfer_Request."LineNum"||Transfer_Request."ItemCode"
left join OWTR as  TR on TR."DocEntry" = TR1."DocEntry";
---

Create local temporary table #OpenQuantity
(
	RowNum int,
	DocEntry int, -- DocEntry cua transfer request
	DocNum int,  -- DocNum cua transfer request
	
	Request_Qty numeric(19,6),
	Open_Qty numeric(19,6),
	TR_QTy numeric(19,6) -- Tong so luong da Transfer  doi voi DocNum transfer request 
	
);
insert into #OpenQuantity ( DocEntry,DocNum)
select    distinct DocEntry, DocNum
from #tbl a;


update a
set  Request_Qty  =  (select sum("Quantity") from  WTQ1 b where  a.DocEntry =  b."DocEntry" ) 
from #OpenQuantity  a;


update a
set  TR_QTy  =  (select sum("Quantity") from  WTR1 b where  a.DocEntry =  b."BaseEntry" ) 
from #OpenQuantity  a;

 
update a
set Open_Qty  = Request_Qty - TR_QTy
from  #OpenQuantity a
where  Request_Qty - TR_QTy >0;


insert into #tbl (RowNum,DocNum,DocEntry, ItemName, Quantity,OpenQty,TR_Qty)
select 99,DocNum,DocEntry,'Open Quantity',Request_Qty,Open_Qty,TR_QTy
from #OpenQuantity ;

---
/*
Update a
set  OpenQty_Temp = b."OpenQty"
	 
	  
from #tbl a
inner join (select DocNum,ItemCode, Quantity -  TR_Qty as "OpenQty" from #tbl b  where RowNum  =1 ) b 
												on b.DocNum =  a.DocNum 
												and  b.ItemCode =  a.ItemCode ;
---
update a
set OpenQty = OpenQty_Temp
from  #tbl a
where RowNum  = 1;
---

update a
set  OpenQty  = 
			ifnull((select OpenQty from #tbl b where b.RowNum = a.RowNum-1 and a.DocNum  = b.DocNum and a.ItemCode = b.ItemCode),0)	 
from #tbl a
where RowNum  > 1;
*/
---
 
select * from #tbl ;

drop table #tbl;
drop table #OpenQuantity;

end;

