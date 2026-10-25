CREATE procedure ILD_PRODUCTION_COSTING_bkup (
	in DocEntry_Receipt int
)
as
begin
--declare DocEntry_Receipt nvarchar(255) = 1010; 
declare Todate date = CURRENT_TIMESTAMP ;--- lay thoi gian hien tai dang luu phieu receipt from production

declare  tbl table 
(

	WOID int,
	WONo varchar(50),
	WOType varchar(1),
	DocEntry int,
	DocNum int,
	SystemDate date,
	PostingDate date,
	-- 
	FGCode varchar(50),
	FGName nvarchar(150),
	PlanFGQty numeric(21,6),
	CmpltQty numeric(21,6),
	UnitPriceFG numeric(21,6),
	AmountFG numeric(21,6),
	--
	ItemCode varchar(50),
	ItemName nvarchar(100),
	BomRate numeric(21,6), 
	StdQty 	numeric(21,6),
	UnitPrice  numeric(21,6),-- Tinh toan sau
	StdAmount numeric(21,6), -- Tinh toan sau
	ItmsGrpCod 	int,
	ItmsGrpName	nvarchar(100),
	--
	Cumulative_Qty numeric(21,6),
	Cumulative_Value numeric(21,6),
	WareHouse	nvarchar(50),
	LineNum 	int,
	SystemDate_UseCompare numeric(21,6),
	PostingDate_UseCompare numeric(21,6),
	SystemDate_Time varchar(20),
	PostingDate_Time varchar(20),
	
	Plan_Materials_Qty numeric(19,6),
	IssuedQty numeric(19,6),
	IssueType varchar(1),
	--
	ReCpt_LineNum int

);
declare Receipt table
(
	WONo varchar(50),
	DocEntry int,
	DocNum int,
	SystemDate date,
	PostingDate date,
	
	Qty numeric(21,6),
	Price  numeric(21,6),
	Amount numeric(21,6),
	ItemCode varchar(50),
	ItemName nvarchar(100),
	WhsCode varchar(50),
	
	SystemDate_Time varchar(20),
	PostingDate_Time varchar(20),
	
	SystemDate_UseCompare numeric(21,6),
	PostingDate_UseCompare numeric(21,6)
);
declare Receipt_Sum table
(
	DocEntry int,
	ItemCode varchar(50),
	Qty numeric(21,6),
	Price  numeric(21,6),
	Amount numeric(21,6)
);
declare LSX table
(
	WOID int,
	WONo int,
	WOType nvarchar(1),
	FGCode varchar(50),
	PlannedQty_Header numeric(19,6), -- 1
	ItemCode varchar(50),
	ItemName nvarchar(150),
	LineNum int,
	BomRate numeric(19,6),
	PlannedQty_Line numeric(19,6), -- 2

	IssuedQty numeric(19,6),
	IssueType varchar(1),
	-- Gia tri can tinh toan
	Qty numeric(19,6),
	ItmsGrpCod 	int,
	ItmsGrpName	nvarchar(100),
	WareHouse	nvarchar(50)
	
);
declare  Issued table 
(
	DocEntry int,
	WONo varchar(50),
	Qty numeric(21,6),
	Price  numeric(21,6),
	Amount_Issued numeric(21,6),
	ItemCode varchar(50),
	WhsCode  varchar(50),
	SystemDate_UseCompare numeric(21,6),
	PostingDate_UseCompare numeric(21,6)
	
);

insert into :Receipt (DocEntry, DocNum,
						Qty,Price, Amount, WONo,ItemCode,ItemName, WhsCode,
						SystemDate ,PostingDate,
						--
						SystemDate_Time,
						PostingDate_Time,
						--
						SystemDate_UseCompare,
						PostingDate_UseCompare) 
select   distinct "DocEntry",  "DocNum",
		 "Quantity", "Price",  "LineTotal",   "WONo",  "ItemCode",  "Dscription", "WhsCode",
		 TO_TIMESTAMP(TO_VARCHAR(Nguon1."CreateDate",'YYYY-MM-DD')||' '||Nguon1."DocTime_2") as "Create_Date_Time",
		 TO_TIMESTAMP(TO_VARCHAR(Nguon1."DocDate",'YYYY-MM-DD')||' '||Nguon1."DocTime_2") as "DocDate_Date_Time",
		 --
		TO_VARCHAR(Nguon1."CreateDate",'YYYY-MM-DD')||' '||Nguon1."DocTime_2" as "SystemDate_Time",
		TO_VARCHAR(Nguon1."DocDate",'YYYY-MM-DD')||' '||Nguon1."DocTime_2" as "PostingDate_Time",
		--,TO_VARCHAR(Nguon1."CreateDate",'YYYY-MM-DD')||' '||Nguon1."DocTime_2" as "Create_Date_Time_String",

--
TO_decimal(TO_VARCHAR(TO_TIMESTAMP(TO_VARCHAR(Nguon1."CreateDate",'YYYY-MM-DD')||' '||Nguon1."DocTime_2"),'YYYYMMDDHH24MISS')),
--PostingDate_UseCompare
TO_decimal(TO_VARCHAR(TO_TIMESTAMP(TO_VARCHAR(Nguon1."DocDate",'YYYY-MM-DD')||' '||Nguon1."DocTime_2"),'YYYYMMDDHH24MISS'))
from
(
		select a."DocEntry", a."DocNum", 
		
				b."Quantity", b."Price", b."LineTotal", b."BaseRef" as "WONo", b."ItemCode", b."Dscription",
				b."WhsCode", a."DocTime", 
				
				
				TO_VARCHAR(a."CreateDate",'YYYY-MM-DD') as "CreateDate",
				TO_VARCHAR(a."DocDate",'YYYY-MM-DD') as "DocDate",
				 
				  LEFT(CASE 
		WHEN LENGTH(a."DocTime") = 3 THEN CAST('0' || a."DocTime" AS TIME) 
		WHEN LENGTH(a."DocTime") = 2 THEN CAST('00' || a."DocTime" AS TIME) 
		WHEN LENGTH(a."DocTime") = 1 THEN CAST('000' || a."DocTime" AS TIME) 
		ELSE CAST(a."DocTime" AS TIME) END,5)  as "DocTime_2",
		
		b."LineNum" as "ReCpt_LineNum"
				
				
		from OIGN a
		inner join IGN1 b  on a."DocEntry" = b."DocEntry"
		inner join OITM c  on b."ItemCode"  = c."ItemCode"
		where c."ItmsGrpCod"  in (120,121,127) 
		--and b."BaseRef"  = '302300005'
			and b."BaseType"  =  202  and a."DocEntry"  = :DocEntry_Receipt 
	
	 ) as Nguon1;
 
	
--
insert into :Receipt_Sum (ItemCode,Qty, Amount )
select  "ItemCode",  sum("Quantity"), sum("LineTotal")
from  IGN1
where "DocEntry"  = :DocEntry_Receipt
group by  "ItemCode";
--
update a
set a.Qty = b.Qty,
	a.Amount  =b.Amount
from :Receipt a
inner join  :Receipt_Sum b on a.ItemCode =  b.ItemCode;
--
insert into  :LSX (WOID,WONo,WOType,FGCode, PlannedQty_Header, 
					ItemCode,ItemName,LineNum, 
					BomRate, 
					
					PlannedQty_Line,
					IssuedQty,
					IssueType,
					
					ItmsGrpCod,ItmsGrpName,WareHouse)

select a."DocEntry", a."DocNum",a."Type",a."ItemCode", a."PlannedQty",
		b."ItemCode",c."ItemName",b."LineNum", 
		b."PlannedQty"/a."PlannedQty" as  "BomRate",
		
		b."PlannedQty",
		b."IssuedQty",
		b."IssueType",
		
		d."ItmsGrpCod",
		d."ItmsGrpNam",
		b."wareHouse"
from OWOR a
inner join WOR1 b on a."DocEntry" = b."DocEntry"
inner join OITM c on c."ItemCode" = b."ItemCode"
inner join OITB d on d."ItmsGrpCod"  = c."ItmsGrpCod"

where a."DocNum" in  (select distinct WONo from :Receipt)
and ifnull(b."PlannedQty",0) >0
order by a."DocNum",b."LineNum";
--- 
update a
set a.Qty  = ifnull(a.PlannedQty_Line,0)/(a.PlannedQty_Header * b.Qty)
from :LSX a
inner join :Receipt b on a.WONo  = b.WONo and a.FGCode =  b.ItemCode;

---
insert into :tbl (WOID,WONo,WOType, DocEntry,  DocNum, SystemDate, PostingDate,FGCode,FGName,CmpltQty,PlanFGQty,UnitPriceFG,AmountFG,
				--
				ItemCode,ItemName,BomRate,StdQty,ItmsGrpCod,ItmsGrpName, WareHouse, LineNum,
				
				Plan_Materials_Qty,
				IssuedQty,
				IssueType,
				--
				SystemDate_UseCompare,
				PostingDate_UseCompare,
				--
				SystemDate_Time,
				PostingDate_Time 
				)
				
select b.WOID,a.WONo, b.WOType,a.DocEntry,  a.DocNum, a.SystemDate, a.PostingDate,a.ItemCode,a.ItemName,a.Qty,b.PlannedQty_Header,a.Price,a.Amount,
		--
		b.ItemCode, b.ItemName,b.BomRate,b.Qty, b.ItmsGrpCod,b.ItmsGrpName,
		b.WareHouse,
		b.LineNum,
		b.PlannedQty_Line,
		b.IssuedQty,
		b.IssueType,
		
		a.SystemDate_UseCompare,
		a.PostingDate_UseCompare,
		--
		a.SystemDate_Time,
		a.PostingDate_Time 

from :Receipt a
inner join  :LSX b on a.WONo  = b.WONo and a.ItemCode = b.FGCode;
--
update a
set StdQty  =  CmpltQty * BomRate
from :tbl a;
----- Tao table OINM temp
OINM_Temp = select "InQty" ,"OutQty" , "ItemCode" , "Warehouse","TransValue",
					 TO_decimal(TO_VARCHAR("Create_Date_Time",'YYYYMMDDHH24MISS')) as "SystemDate_UseCompare",  
					 TO_decimal(TO_VARCHAR("DocDate_Date_Time",'YYYYMMDDHH24MISS')) as "PostingDate_UseCompare"
					
					from (
					select  "InQty" ,"OutQty" , "ItemCode" , "Warehouse", "TransValue",
					TO_TIMESTAMP(TO_VARCHAR(Nguon1."CreateDate",'YYYY-MM-DD')||' '||Nguon1."DocTime") as "Create_Date_Time",
					TO_TIMESTAMP(TO_VARCHAR(Nguon1."DocDate",'YYYY-MM-DD')||' '||Nguon1."DocTime") as "DocDate_Date_Time"
					from 
					(
					select  "InQty"  ,   "OutQty" , "ItemCode" , "Warehouse","TransValue",
					
									TO_VARCHAR("CreateDate",'YYYY-MM-DD') as "CreateDate",
									TO_VARCHAR("DocDate",'YYYY-MM-DD') as "DocDate",
									 LEFT(CASE 
							WHEN LENGTH("DocTime") = 3 THEN CAST('0' || "DocTime" AS TIME) 
							WHEN LENGTH("DocTime") = 2 THEN CAST('00' || "DocTime" AS TIME) 
							WHEN LENGTH("DocTime") = 1 THEN CAST('000' || "DocTime" AS TIME) 
							ELSE CAST("DocTime" AS TIME) END,5)  as "DocTime"
					from OINM b 
					where "CreateDate" < :Todate ) as Nguon1) as Nguon2;

 update a
 set a.Cumulative_Qty  = (select  sum("InQty") - sum("OutQty") from :OINM_Temp b 
 								where a.ItemCode =  b."ItemCode"  and a.WareHouse  = b."Warehouse"
 									and b."SystemDate_UseCompare" <= a.SystemDate_UseCompare)
 from  :tbl a;
 
 ---
 update a
 set a.Cumulative_Value  = (select  sum("TransValue") from :OINM_Temp b 
 								where a.ItemCode =  b."ItemCode"  and a.WareHouse  = b."Warehouse"
 									and b."SystemDate_UseCompare" <= a.SystemDate_UseCompare)
 from  :tbl a;
--
----phieu issue de lay gia tri item khong ton kho: may moc, nhan cong
insert into :Issued (DocEntry,WONo, Qty,Price, Amount_Issued, ItemCode, WhsCode,
						SystemDate_UseCompare,
						PostingDate_UseCompare ) 


select "DocEntry","WONo","Quantity","Price","LineTotal","ItemCode","WhsCode",
 TO_decimal(TO_VARCHAR("Create_Date_Time",'YYYYMMDDHH24MISS')) as "SystemDate_UseCompare",  
 TO_decimal(TO_VARCHAR("DocDate_Date_Time",'YYYYMMDDHH24MISS')) as "PostingDate_UseCompare"
from 
(

		select Nguon1.*,
				TO_TIMESTAMP(TO_VARCHAR(Nguon1."CreateDate",'YYYY-MM-DD')||' '||Nguon1."DocTime") as "Create_Date_Time",
				TO_TIMESTAMP(TO_VARCHAR(Nguon1."DocDate",'YYYY-MM-DD')||' '||Nguon1."DocTime") as "DocDate_Date_Time"
		
		from 
		
			(select  a."DocEntry", a."BaseRef" as "WONo",a."Quantity",a."Price", a."LineTotal", a."ItemCode", 
				--e."CreateDate",
				a."WhsCode",
				TO_VARCHAR(e."CreateDate",'YYYY-MM-DD') as "CreateDate",
				TO_VARCHAR(e."DocDate",'YYYY-MM-DD') as "DocDate",
				LEFT(CASE 
					WHEN LENGTH(e."DocTime") = 3 THEN CAST('0' || e."DocTime" AS TIME) 
					WHEN LENGTH(e."DocTime") = 2 THEN CAST('00' || e."DocTime" AS TIME) 
					WHEN LENGTH(e."DocTime") = 1 THEN CAST('000' || e."DocTime" AS TIME) 
					ELSE CAST(e."DocTime" AS TIME) END,5)  as "DocTime"
			from IGE1 a
			inner join OIGE e on a."DocEntry" = e."DocEntry" 
			inner join OITM c  on a."ItemCode"  = c."ItemCode"
			where c."ItmsGrpCod"  in (125,126,233) 
					and a."BaseType"  =  202  and e."CreateDate" < :Todate
					--CURRENT_TIMESTAMP
			order by  a."DocEntry",a."ItemCode", e."CreateDate" ) as Nguon1) as Nguon2;

 

----- tao ra bang tam:  Issued_Fillter 
Issued_Fillter =  select * 
				 from :Issued
				 where DocEntry   in (select Max(DocEntry) from :Issued group by SystemDate_UseCompare  )
				 order by SystemDate_UseCompare;
---
 update a
 set a.Cumulative_Qty  = b.Qty,
	 a.Cumulative_Value  = b.Amount_Issued,
	 	a.UnitPrice  =  case when a.WOType  = 'D' then c."AvgPrice" else   b.Price end,
	 	a.StdAmount  = (case when a.WOType  = 'D' then c."AvgPrice" else   b.Price end) * a.StdQty
	 	--b.Amount_Issued						 							 							 
 from  :tbl a
 inner join   :Issued_Fillter  b on  a.ItemCode =  b.ItemCode
 inner join OITM c on a.ItemCode =  c."ItemCode"
 where  a.ItmsGrpCod in (125,126,233)
  			and b.SystemDate_UseCompare  <= a.SystemDate_UseCompare
  			and b.DocEntry =  
  			 (select   Max(c.DocEntry) from :Issued_Fillter c where  b.ItemCode = c.ItemCode and c.SystemDate_UseCompare  <= a.SystemDate_UseCompare );
----
update   :tbl
set  UnitPrice  = ifnull(Cumulative_Value,0)/Cumulative_Qty
where ifnull(Cumulative_Qty,0) >0 and ItmsGrpCod not in (125,126,233);
---

update  :tbl
set  StdAmount = ifnull(UnitPrice,0)   * ifnull(StdQty,0)
where  ItmsGrpCod not in (125,126,233);
---
/*
update  a
		set CMPLTQTY  = NULL,  UNITPRICEFG  = NULL,  AMOUNTFG  =NULL 
		from :tbl a
		where   LineNum  <>0;
*/
-- Update Amount nguyen vat lieu chinh: green coffee
update a
set StdAmount =  ifnull(a.AMOUNTFG,0) -  (select   sum(STDAMOUNT) 
					from   :tbl b 
					where a.WOID =  b.WOID and  a.DOCNUM =  b.DOCNUM
							and ITMSGRPCOD  <> 113 or linenum not in (select c.linenum from :tbl  c
																			where c.ITMSGRPCOD  = 113
																			and c.StdQty  = (select max(d.StdQty) from :tbl d where ITMSGRPCOD  = 113))
						 ) 
from :tbl a
where ITMSGRPCOD  = 113 and  StdQty =    (select max(b.StdQty) from :tbl b where ITMSGRPCOD  = 113);
--- Truong hop lenh san xuat chi co 1 don
if  (select count(*) from :tbl ) = 1 then
	update a
	set StdAmount =  AMOUNTFG
	from :tbl a;
end if;
-- Update Unitprice nguyen vat lieu chinh: green coffee
update a
set UNITPRICE =   ifnull(STDAMOUNT,0)/STDQTY
from :tbl a
where ITMSGRPCOD  = 113 and ifnull(STDQTY,0) >0
	and StdQty =    (select max(b.StdQty) from :tbl b where ITMSGRPCOD  = 113);

---
/* tam khoa
update a
set STDAMOUNT  = ifnull(a.AMOUNTFG,0)
from :tbl a
where ifnull(STDAMOUNT,0)  = 0;
*/
---
--- Truong hop co  2 nvl Chinh 113 trong cung 1 bom
if    (select count(*) from :tbl where ITMSGRPCOD  = 113  ) > 1  then
	update a
	set UnitPrice  = ifnull(Cumulative_Value,0)/Cumulative_Qty
	from :tbl a
	where ifnull(Cumulative_Qty,0) >0  and  ITMSGRPCOD  = 113;
	---
	update  :tbl
	set  StdAmount = ifnull(UnitPrice,0)   * ifnull(StdQty,0)
   	where  ITMSGRPCOD  = 113;
	-- Tổng Cột STDAMOUNT phải = AMOUNTFG của phiếu
	update a
	set StdAmount =  ifnull(a.AMOUNTFG,0) -  (select   sum(STDAMOUNT) 
					from   :tbl b 
					where a.WOID =  b.WOID and  a.DOCNUM =  b.DOCNUM
							and ITMSGRPCOD  <> 113 or linenum not in (select c.linenum from :tbl  c
																			where c.ITMSGRPCOD  = 113
																			and c.LineNum  = (select max(d.LineNum) from :tbl d where ITMSGRPCOD  = 113))
						 ) 
	from :tbl a
	where ITMSGRPCOD  = 113 and   LineNum =    (select   max(b.LineNum) from :tbl b where ITMSGRPCOD  = 113);
	---
	update a
	set UNITPRICE =   ifnull(STDAMOUNT,0)/STDQTY
	from :tbl a
	where ITMSGRPCOD  = 113 and ifnull(STDQTY,0) >0
		and LineNum =    (select max(b.LineNum) from :tbl b where ITMSGRPCOD  = 113);
end if;

-----
update a
set UNITPRICE  = ifnull(STDAMOUNT,0)/STDQTY
from :tbl a
where ifnull(UNITPRICE,0)  = 0 and  ifnull(STDQTY,0) >0;

----
update  a
		set CMPLTQTY  = NULL,  UNITPRICEFG  = NULL,  AMOUNTFG  =NULL 
		from :tbl a
		where    LineNum  <>0     ;
---
update a
set UNITPRICE = 0
from  :tbl a
where ifnull(UNITPRICEFG,0)=0;
---
insert into Costing_StandardCost (WOID,
									WONo,
									DocEntry,
									DocNum,
									SystemDate,
									PostingDate,

									FGCode,
									FGName,
									PlanFGQty,
									CmpltQty,
									UnitPriceFG,
									AmountFG,

									ItemCode,
									ItemName,
									BomRate,
									StdQty,
									UnitPrice,
									StdAmount,
									ItmsGrpCod,
									ItmsGrpName,

									Cumulative_Qty,
									Cumulative_Value,
									WareHouse,
									LineNum,
									SystemDate_UseCompare,
									PostingDate_UseCompare,
									SystemDate_Time,
									PostingDate_Time,
									
									Plan_Materials_Qty,
									IssuedQty,
									IssueType)
select WOID,
		WONo,
		DocEntry,
		DocNum,
		SystemDate,
		PostingDate,
-- 
		FGCode,
		FGName,
		PlanFGQty,
		CmpltQty,
		UnitPriceFG,
		AmountFG,
--
		ItemCode,
		ItemName,
		BomRate,
		StdQty,
		UnitPrice,
		StdAmount,
		ItmsGrpCod,
		ItmsGrpName,
--
		Cumulative_Qty,
		Cumulative_Value,
		WareHouse,
		LineNum,
		SystemDate_UseCompare,
		PostingDate_UseCompare,
		SystemDate_Time,
		PostingDate_Time,
		
		Plan_Materials_Qty,
		IssuedQty,
		IssueType
from :tbl;

--select * from :tbl order by  WONo, LineNum;
end;
