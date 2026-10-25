CREATE procedure ILD_PRODUCTION_COSTING_Disassembly_Thang7 (
	in DocEntry int
)
as
begin
--declare DocEntry_Issued for production: khi lam lệnh rã; 
declare Todate date = CURRENT_TIMESTAMP ;--- lay thoi gian hien tai dang luu phieu  

declare tbl  table
(

	WOID int,
	WONo varchar(50),
	DocEntry int,
	DocNum int,
	SystemDate date,
	PostingDate date,
	DocType int,
	-- 
	FGCode varchar(50),
	FGName nvarchar(150),
	CmpltQty numeric(21,6),
	PlanFGQty numeric(21,6),
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
	-- Thong tin lenh san xuat ra,
	DisID  int,
	DisNo  varchar(50),
	---
	Plan_Materials_Qty numeric(19,6),
	IssuedQty numeric(19,6),
	IssueType varchar(1),
	
	U_Group nvarchar(20)

);

declare LSX table
(
	WOID int,
	WONo int,
	FGCode varchar(50),
	CmpltQty numeric(19,6),
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
	WareHouse	nvarchar(50),
	
	U_Group nvarchar(20)
	
);
declare Issued_LenhRa table
(
	WONo varchar(50),
	WOID int,
	DisID int,
	DisNo varchar(50),
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
select "DocDate" into Todate from OINM where "TransType"  =  60 and "CreatedBy" =:DocEntry;


insert into  :LSX (WOID,WONo,FGCode, CmpltQty,PlannedQty_Header, 
					ItemCode,ItemName,LineNum, 
					BomRate, 
					
					PlannedQty_Line,
					IssuedQty,
					IssueType,
					ItmsGrpCod,ItmsGrpName,WareHouse, U_Group)

select a."DocEntry", a."DocNum",a."ItemCode", a."CmpltQty",a."PlannedQty",
		b."ItemCode",c."ItemName",b."LineNum", 
		b."PlannedQty"/a."PlannedQty" as  "BomRate",
		
		b."PlannedQty",
		b."IssuedQty",
		b."IssueType",
		
		d."ItmsGrpCod",
		d."ItmsGrpNam",
		b."wareHouse" ,
		--
		d."U_Group"
		
from OWOR a
inner join WOR1 b on a."DocEntry" = b."DocEntry"
inner join OITM c on c."ItemCode" = b."ItemCode"
inner join OITB d on d."ItmsGrpCod"  = c."ItmsGrpCod"
where a."DocEntry" in  (select   max("BaseEntry") as "WOID" 
						 from   IGE1 
						 where "BaseType"  = 202  and "DocEntry"  = :DocEntry)
and ifnull(b."PlannedQty",0) >0;
--- 

insert into :Issued_LenhRa (DocEntry, DocNum,
						Qty,Price, Amount, DisID,DisNo,ItemCode,ItemName, WhsCode,
						SystemDate ,PostingDate,
						--
						SystemDate_Time,
						PostingDate_Time,
						--
						SystemDate_UseCompare,
						PostingDate_UseCompare) 
select   "DocEntry",  "DocNum",
		 "Quantity", "Price",  "LineTotal", "DisID",  "DisNo",  "ItemCode",  "Dscription", "WhsCode",
		 TO_TIMESTAMP(TO_VARCHAR(Nguon1."CreateDate",'YYYY-MM-DD')||' '||Nguon1."DocTime_2") as "Create_Date_Time",
		 TO_TIMESTAMP(TO_VARCHAR(Nguon1."DocDate",'YYYY-MM-DD')||' '||Nguon1."DocTime_2") as "DocDate_Date_Time",
		 --
		TO_VARCHAR(Nguon1."CreateDate",'YYYY-MM-DD')||' '||Nguon1."DocTime_2" as "SystemDate_Time",
		TO_VARCHAR(Nguon1."DocDate",'YYYY-MM-DD')||' '||Nguon1."DocTime_2" as "PostingDate_Time",
		--
		TO_decimal(TO_VARCHAR(TO_TIMESTAMP(TO_VARCHAR(Nguon1."CreateDate",'YYYY-MM-DD')||' '||Nguon1."DocTime_2"),'YYYYMMDDHH24MISS')),
		--PostingDate_UseCompare
		TO_decimal(TO_VARCHAR(TO_TIMESTAMP(TO_VARCHAR(Nguon1."DocDate",'YYYY-MM-DD')||' '||Nguon1."DocTime_2"),'YYYYMMDDHH24MISS'))
from
(
		select a."DocEntry", a."DocNum", 
		
				b."Quantity", b."Price", b."LineTotal",b."BaseEntry" as  "DisID", b."BaseRef" as "DisNo", b."ItemCode", b."Dscription",
				b."WhsCode", a."DocTime", 
				
				
				TO_VARCHAR(a."CreateDate",'YYYY-MM-DD') as "CreateDate",
				TO_VARCHAR(a."DocDate",'YYYY-MM-DD') as "DocDate",
				 
		LEFT(CASE 
		WHEN LENGTH(a."DocTime") = 3 THEN CAST('0' || a."DocTime" AS TIME) 
		WHEN LENGTH(a."DocTime") = 2 THEN CAST('00' || a."DocTime" AS TIME) 
		WHEN LENGTH(a."DocTime") = 1 THEN CAST('000' || a."DocTime" AS TIME) 
		ELSE CAST(a."DocTime" AS TIME) END,5)  as "DocTime_2"
				
		from OIGE a
		inner join IGE1 b  on a."DocEntry" = b."DocEntry"
		inner join OITM c  on b."ItemCode"  = c."ItemCode"
		inner join OWOR d on d."DocNum"  = b."BaseRef"
		left  join  OITB e on c."ItmsGrpCod"  = e."ItmsGrpCod" 
		where  (c."ItmsGrpCod"  in (120,121,127, 253) or e."U_Group" = 'TP')  
				and d."Type" = 'D' --Disassembly
				and b."BaseType"  =  202  and a."DocEntry"  = :DocEntry 
	
	 ) as Nguon1;
	 
 
	  
---- Thông tin SSCC với lệnh rã
SSCC_LenhRa =  	select  distinct  c."DocEntry" as "DisID", c."DocNum" as "DisNo", e."ItemCode",e."Dscription" as  "ItemName",
						 e."CreatedBy",	issued."BatchNum" as  "SSCC" ,  left(issued."BatchNum",9) as "WONum_Root"
	 		
				 from OWOR c
				 left join  OINM e on  e."AppObjAbs" = c."DocEntry"		
				 inner join OITM f on f."ItemCode"  = e."ItemCode"  
				 left  join  OITB g on f."ItmsGrpCod"  = g."ItmsGrpCod"				     		      
				  left join (select "BatchNum","BaseType", "BaseEntry", "ItemCode", "Quantity" 
											from IBT1 
											where "BaseType" =  60) issued 
				on issued."BaseEntry" =  e."CreatedBy" and issued."ItemCode" = e."ItemCode"
				where  issued."BaseEntry" = :DocEntry
						and   e."ApplObj"  = '202'   
						and c."Type"  = 'D' 
						and ( (f."ItmsGrpCod"  in (120,121,127, 253) or g."U_Group" = 'TP')  );
	 
--- Thông tin lệnh sản xuất gốc
SSCC_LSXGoc  = select  a."ItemCode", a."BsDocEntry" as "WoID" 
				from IBT1 a
				inner join (select top 1 * from  :SSCC_LenhRa) b 
														on a."ItemCode" = b."ItemCode" and a."BatchNum" = b."SSCC"
														AND a."BaseType" = 59 AND  a."BsDocType" =  202;											
  
				
---
insert into :tbl (WONo,WOID,DisID,DisNo, DocEntry,  DocNum, SystemDate, PostingDate,FGCode,FGName,CmpltQty,
				PlanFGQty,UnitPriceFG,AmountFG,
				--
				ItemCode,ItemName,BomRate,StdQty,ItmsGrpCod,ItmsGrpName, WareHouse, LineNum,
				--
				SystemDate_UseCompare,
				PostingDate_UseCompare,
				--
				SystemDate_Time,
				PostingDate_Time,
				DocType,
				
				--
				Plan_Materials_Qty,
				IssuedQty,
				IssueType,
				--
				U_Group
				)
				
select a.WONo, a.WOID ,a.DisId,a.DisNo, a.DocEntry,  a.DocNum, a.SystemDate, a.PostingDate,a.ItemCode,a.ItemName,
			--b.CmpltQty,
			a.Qty,
							b.PlannedQty_Header,a.Price,a.Amount,
		--
		b.ItemCode, b.ItemName,b.BomRate,b.Qty, b.ItmsGrpCod,b.ItmsGrpName,
		b.WareHouse,
		b.LineNum,
		a.SystemDate_UseCompare,
		a.PostingDate_UseCompare,
		--
		a.SystemDate_Time,
		a.PostingDate_Time,
		60 as "DocType_Issued",
		--
		b.PlannedQty_Line,
		b.IssuedQty,
		b.IssueType,
		--
		b.U_Group
		 

from :LSX b
left join  :Issued_LenhRa a on  a.ItemCode = b.FGCode and a.DisNo  = b.WONo; 
---

update a 
set a.WOID  =  b."WoID"
from :tbl a, :SSCC_LSXGoc b;
--
update a
set  a.WONO  = (select "DocNum" from OWOR b where a.WoID  = b."DocEntry")
from :tbl a;

if exists (select 1 from :tbl where ifnull(WOID,0)  = 0) then 
	update a
	set  WONO = d."WONum_Root",
		 WOID = d."WOID"
	
	from :tbl a, (select top 1 b.*, c."DocEntry" as "WOID"  from  :SSCC_LenhRa b 
					    inner join OWOR c on b."WONum_Root" = c."DocNum" where ifnull("WONum_Root",'')  <> '') d;
end if;
----
update a
set StdQty  =  round(CmpltQty * BomRate,3)
from :tbl a;
---	
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

/*
	do thời điểm xây dựng giải pháp lưu phiếu và table :COSTING_STANDARDCOST (ở mỗi phiếu
	nhập TP và xuất nvl cho lsx. chưa hoàn thiện. Nên sẽ dùng table PRO11_DATA_TEMP (lưu dữ liệu pro11 đến thời điểm  giải pháp được hoàn chỉnh)
	PRO11_DATA_TEMP: dùng lệnh thủ công insert từ báo cáo pro11 vào (theo từng tháng)

*/
---
/* giải pháp cũ: 
pro11_temp =  select "ITEMCODE",   Max("UNITPRICE") as UnitPrice
			 from 
			 (
			 	 select  "ITEMCODE",   Max("UNITPRICE") as UnitPrice
						  from PRO11_DATA_TEMP 
						  where  ITMSGRPCOD IN (125,126) and WOID  in (select distinct WOID from :tbl)-- woid: la lsx: gốc
						  group by "ITEMCODE"
						  
						  union
						  
						  select  "ITEMCODE",   Max("UNITPRICE") as UnitPrice 
						  from  COSTING_STANDARDCOST
						  where  ITMSGRPCOD IN (125,126) and WOID  in (select distinct WOID from :tbl)-- woid: la lsx: gốc
						  group by "ITEMCODE"
			 ) as Final
			 group by "ITEMCODE";

			  
			
update a
set UnitPrice  = ifnull(b.UnitPrice,0),
	StdAmount = ifnull(b.UnitPrice,0)  *  ifnull(StdQty,0)
from :tbl a 
inner join :pro11_temp b on a.ItemCode = b.ITEMCODE
where ItmsGrpCod in (125,126);
*/ -- kêt thúc giải pháp cũ

update a
set UnitPrice  = ifnull(b."AvgPrice",0),
	StdAmount = ifnull(b."AvgPrice",0)  *  ifnull(StdQty,0)
from :tbl a 
inner join OITM b on a.ItemCode  = b."ItemCode"
where (a.ItmsGrpCod in (125,126,233,254)  or  a.U_Group in ('MC_TTMC','LB_TTLB'));

----  doi vơi item không phải nhân công, máy móc
update   :tbl
set  UnitPrice  = ifnull(Cumulative_Value,0)/Cumulative_Qty
where ifnull(Cumulative_Qty,0) >0 and ItmsGrpCod not in (125,126,233,254)   and  ifnull(U_Group,'') not in ('','MC_TTMC','LB_TTLB');
 
---
update  :tbl
set  StdAmount = ifnull(UnitPrice,0)   * ifnull(StdQty,0)
where  ItmsGrpCod not in (125,126,233,254) and  ifnull(U_Group,'') not in ('','MC_TTMC','LB_TTLB');
---
/* tam khoa
update  a
		set CMPLTQTY  = NULL,  UNITPRICEFG  = NULL,  AMOUNTFG  =NULL 
		from :tbl a
		where   LineNum  <>0;
*/
-- Update Amount nguyen vat lieu chinh: green coffee
/* old
update a
set StdAmount =  ifnull(a.AMOUNTFG,0) -  (select   sum(STDAMOUNT) 
					from   :tbl b 
					where a.WOID =  b.WOID and  a.DOCNUM =  b.DOCNUM
							and ITMSGRPCOD  <> 113
						 ) 
from :tbl a
where ITMSGRPCOD  = 113;
*/
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

--========Truong hop Bom cuar co nvl gia cong KHONG CO NVL CHINH 113
IF (select count (*)  from :tbl where ITMSGRPCOD  = 113 ) <=0 then 
	update a
	set StdAmount =  ifnull(a.AMOUNTFG,0) -  (select   sum(STDAMOUNT) 
					from   :tbl b 
					where a.WOID =  b.WOID and  a.DOCNUM =  b.DOCNUM
							and ITMSGRPCOD  <> 253 or linenum not in (select c.linenum from :tbl  c
																			where c.ITMSGRPCOD  = 253
																			and c.StdQty  = (select max(d.StdQty) from :tbl d where ITMSGRPCOD  = 253))
						 ) 
	from :tbl a
	where ITMSGRPCOD  = 253 and  StdQty =    (select max(b.StdQty) from :tbl b where ITMSGRPCOD  = 253);
END IF;

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
/*tam khoa
update a
set STDAMOUNT  = ifnull(a.AMOUNTFG,0)
from :tbl a
where ifnull(STDAMOUNT,0)  = 0;
*/
--- Truong hop co  2 nvl Chinh 113 trong cung 1 bom
if    (select count(*) from :tbl where ITMSGRPCOD  = 113 ) > 1  then
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
																			and c.linenum  = (select max(d.linenum) from :tbl d where ITMSGRPCOD  = 113))
						 ) 
	from :tbl a
	where ITMSGRPCOD  = 113 and  linenum =    (select max(b.linenum) from :tbl b where ITMSGRPCOD  = 113);
	---
	update a
	set UNITPRICE =   ifnull(STDAMOUNT,0)/STDQTY
	from :tbl a
	where ITMSGRPCOD  = 113 and ifnull(STDQTY,0) >0
		and linenum =    (select max(b.linenum) from :tbl b where ITMSGRPCOD  = 113);
end if;

---
update a
set UNITPRICE  = ifnull(STDAMOUNT,0)/STDQTY
from :tbl a
where ifnull(UNITPRICE,0)  = 0 and  ifnull(STDQTY,0) >0;

----
update a
set CmpltQty = -CmpltQty,
		AmountFG = -AmountFG,
		StdQty = -StdQty,
		StdAmount = -StdAmount
from :tbl a
where ifnull(a.DisNo,'') <> '';

-- Truong hop co  2 nvl Chinh 113 trong cung 1 bom
/*
	Trường hợp thứ tự ở lệnh sản xuất có nguyên vật liệu (NVL) Chính  = 113 với linenum  = 0,
	update lênh sau để đảm bảo 1 gia tri AMOUNTFG/ docentry  : AMOUNTFG = sum (StdAmount)
*/
if    (select count(*) from :tbl where ITMSGRPCOD  = 113 ) > 1  then
			update  a
			set CMPLTQTY  = NULL,  UNITPRICEFG  = NULL,  AMOUNTFG  =NULL 
			from :tbl a
			where    LineNum  <>0     ;
end if;
/*
	Trường hợp thứ tự ở lệnh sản xuất có nguyên vật liệu (NVL) Chính  = 113 với linenum  <> 0,
	update lênh sau để đảm bảo 1 gia tri AMOUNTFG/ docentry  : AMOUNTFG = sum (StdAmount)
*/
if    (select count(*) from :tbl where ITMSGRPCOD  = 113  and  LineNum  <> 0) = 1  then
			update  a
			set CMPLTQTY  = NULL,  UNITPRICEFG  = NULL,  AMOUNTFG  =NULL 
			from :tbl a
			where    ITMSGRPCOD   <> 113     ;
end if;

--========XU LY TRUONG HOP: BOM khong co nvl chinh   = 113
-- Truong hop nay co  1 item nvl gia cong lam nvl chinh
if (select count (*)  from :tbl where ITMSGRPCOD  = 113 ) <=0 then 
			update  a
			set CMPLTQTY  = NULL,  UNITPRICEFG  = NULL,  AMOUNTFG  =NULL 
			from :tbl a
			where    ITMSGRPCOD   <> 253     ;
end if;




---
/* khoa ngay 25-02-2025
update a
set UNITPRICE = 0
from  :tbl a
where ifnull(UNITPRICEFG,0)=0;
*/
--- insert dữ liệu vào table vật lý standard cost
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
									IssueType,
									
									DISID,
									DISNO,
									--
									U_Group
									)
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
		IssueType,
		
		DISID,
		DISNO,
		--
		U_Group
from :tbl;
---


--drop table :Issued_LenhRa;
--drop table #tbl;
--drop table #LSX;

end;
