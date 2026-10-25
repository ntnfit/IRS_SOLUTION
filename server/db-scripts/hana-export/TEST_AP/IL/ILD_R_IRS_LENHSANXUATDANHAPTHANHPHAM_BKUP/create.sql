CREATE procedure ILD_R_IRS_LenhSanXuatDaNhapThanhPham_Bkup
(
	in FromDate date,
	in ToDate date,
	in Resource char(3),
	in OutSoucing	varchar(20),
	in WODocNums varchar(5000), --ILD_LayWONumber_Pro09
	in ItemCodes nvarchar(5000), --ILD_LAY_ITEMCODE
	out tab_result table (
		"ObjType"			nvarchar(30) ,
		"DocEntry"			int,
		"LineNum"			int,
		"DocNum"			nvarchar(50),
		"StartDate"			timestamp,
		"FGCode"			nvarchar(50),
		"FGName"			nvarchar(100),
		"PlanQty"			numeric(21,6),
		"CmpltQty"			numeric(21,6),
		"ItemCode"			nvarchar(50),
		"ItemName"			nvarchar(100),
		"IssueQty"			numeric(21,6),
		"IssuedQty"			numeric(21,6),
		"OnHand"			numeric(21,6),
		"OpnQty"			numeric(21,6),
		"NeedQty"			numeric(21,6),
		"WhsCode"			nvarchar(30),
		"WhsName"			nvarchar(100),
		"ItmsGrpCod"		int,
		"ItmsGrpNam"		nvarchar(100),
		"IsOpn"				char(1),
		"IsStock"			char(1),
		"TotQty"			numeric(21,6),
		"Status"			nvarchar(30),
		"AmountFG"			numeric(21,6),
		"UnitPriceFG"		numeric(21,6),
		"Amount"			numeric(21,6),
		"UnitPrice"			numeric(21,6),
		"PlanCompltQty"		numeric(21,6),
		"PlanCompltAmount"	numeric(21,6),
		"BaseQty"			numeric(21,6),
		"BomRate"			numeric(21,6),
		"DueDate"			date,
		"DueDate_Late"		int,
		"PO_Status" 		nvarchar(30),
		"DiffQty"  			numeric(21,6),
		"DiffAmount"  		numeric(21,6),
		"Actual_BOM_Rate1"	numeric(21,6),
		"Actual_BOM_Rate2"	numeric(21,6),
		"Actual_BOM_Rate"   numeric(21,6),
		"U_Group" 			nvarchar(20)
		
  
	)
)
as
begin
	declare Amount_NVL table 
	(
		ItemCode		nvarchar(50),
		WOID			int,
		QtyTotal		numeric(21,6),
		Qty				numeric(21,6),	-- -- số lượng tôchi tiết của từng Batch/SSCC
		Amount			numeric(21,6),
		DocType			nvarchar(20),
		ItmsGrpCod		int
	);
	declare tblResource table (
		FGCode			varchar(50),
		ItemCode		varchar(50),
		ItemName		Nvarchar(150),
		ItmsGrpCod		varchar(50),
		ItmsGrpNam		Nvarchar(150),
		WOID			int,
		WONO			varchar(50),
		IssueQty		numeric(21,6),
		IssuedQty		numeric(21,6),	
		Amount			numeric(21,6),
		StartDate		date,
		LineNum 		int,
		BaseQty			numeric(21,6),
		CmpltQty		numeric(21,6),
		--
		U_Group 		nvarchar(20)
		
		
	);
	declare total6229,total6279 decimal(28,10);
	declare pro11 table
	(
		STDQTY  DECIMAL(21,6),
		STDAMOUNT  DECIMAL(21,6),
		WONO VARCHAR(50), 
		ITEMCODE VARCHAR(50), 
		DISNO  VARCHAR(50),
		U_Group nvarchar(20)
		
	);
	declare tbl table
	 (
	  	"ObjType"			nvarchar(30) ,
		"DocEntry"			int,
		"LineNum"			int,
		"DocNum"			int,
		"StartDate"			timestamp,
		"FGCode"			nvarchar(50),
		"FGName"			nvarchar(100),
		"PlanQty"			numeric(21,6),
		"CmpltQty"			numeric(21,6),
		"ItemCode"			nvarchar(50),
		"ItemName"			nvarchar(100),
		"IssueQty"			numeric(21,6),
		"IssuedQty"			numeric(21,6),
		"OnHand"			numeric(21,6),
		"OpnQty"			numeric(21,6),
		"NeedQty"			numeric(21,6),
		"WhsCode"			nvarchar(30),
		"WhsName"			nvarchar(100),
		"ItmsGrpCod"		int,
		"ItmsGrpNam"		nvarchar(100),
		"IsOpn"				char(1),
		"IsStock"			char(1),
		"TotQty"			numeric(21,6),
		"Status"			nvarchar(30),
		"AmountFG"			numeric(21,6),
		"UnitPriceFG"		numeric(21,6),
		"Amount"			numeric(21,6),
		"UnitPrice"			numeric(21,6),
		"PlanCompltQty"		numeric(21,6),
		"PlanCompltAmount"	numeric(21,6),
		"BaseQty"			numeric(21,6),
		"BomRate"			numeric(21,6),
		"DueDate"			date,
		"DueDate_Late"		int,
		"PO_Status" 		nvarchar(30),
		"DiffQty"  			numeric(21,6),
		"DiffAmount"  		numeric(21,6),
		"Actual_BOM_Rate1"	numeric(21,6),
		"Actual_BOM_Rate2"	numeric(21,6),
		"Actual_BOM_Rate"   numeric(21,6),
		--
		"U_Group" nvarchar(20)
		);
-- JE
declare JE table
(
	
	Account nvarchar(50),
	Amount numeric(21,6),
	Amount_Debit_Credit numeric(21,6)
	
);
declare pro04 table
(
  Qty numeric(21,6),
  Amount numeric(21,6),
  ItemCode nvarchar(50),
  ItemName nvarchar(150),
  GrpCode nvarchar(50),
  GrpName nvarchar(100),
  WOID nvarchar(50),
  WONo nvarchar(50)
);
-- Luu thong tin cac phieu GR,GI khong lien quan den lenh san xuat
declare tblExceptionDocNum table
(
	"DocNum" nvarchar(20),
	"DocType" nvarchar(2),
	"Include_In_Report" nvarchar(20)
	
);
	if ifnull(:fromdate,'') = '' or  ifnull(:Todate,'') = '' then
		select  '2023-01-01', Current_date into fromdate, Todate 
		from dummy;
	end if;
------ lay du lieu tu store pro 04
	CALL USP_RPT_PROD_04(FromDate,ToDate,'','','','',OutSoucing, datapro04);
	insert into :pro04 (Qty,Amount,ItemCode,ItemName,GrpCode,GrpName,WOID,WONo)
	select sum(Qty) as Qty,sum(Amount) as Amount,ItemCode,ItemName,GrpCode,GrpName,WOID,WONo   
											from :datapro04 
											group by ItemCode,ItemName,GrpCode,GrpName,WOID,WONo;
	/*
	create local temporary table #pro04 as (select sum(Qty) as Qty,sum(Amount) as Amount,ItemCode,ItemName,GrpCode,GrpName,WOID,WONo   
											from :datapro04 
											group by ItemCode,ItemName,GrpCode,GrpName,WOID,WONo );*/
											
											
	----
/* tam khoa
	CALL ILD_R_IRS_StandardCost(FromDate,ToDate, datapro11);
	create local temporary table :pro11 as (select sum (STDQTY) as STDQTY, sum(STDAMOUNT)  as STDAMOUNT , 
														WONO,ITEMCODE, DISNO
											from  :datapro11 
											group by WONO,ITEMCODE,DISNO );
*/ -- ket thuc tam khoa
---

---================================Lấy những phiếu ngoại lệ (goog receipt, good issued)
if (:OutSoucing  = 'Y' ) then

	insert into  :tblExceptionDocNum ("DocNum", "DocType","Include_In_Report")
	select  "DocNum", '59', "U_Include_In_Report"
	from OIGN
	where "DocDate" between FromDate and Todate and "CANCELED"  = 'N' 
	and ifnull("U_Include_In_Report",'')  like  '%09%'  and  :OutSoucing  = 'Y';
---
	insert into  :tblExceptionDocNum ("DocNum", "DocType","Include_In_Report")
	select  "DocNum", '60', "U_Include_In_Report"
	from OIGE
	where "DocDate" between FromDate and Todate and "CANCELED"  = 'N' 
	and ifnull("U_Include_In_Report",'')  like  '%09%' and  :OutSoucing  = 'Y';
else 
	insert into  :tblExceptionDocNum ("DocNum", "DocType")
	select 1,1 from dummy;
End if;

---------------------------------------------------------------------------------------------------------------------
	insert into :tbl (
		"ObjType", "DocEntry", "LineNum", "DocNum", "StartDate",
		"FGCode", "FGName", "PlanQty", "CmpltQty",
		
		"ItemCode", "ItemName", "IssueQty", "IssuedQty",  "OnHand",
		 
		"WhsCode", "ItmsGrpCod", "ItmsGrpNam", "Status" ,"BaseQty", "U_Group" )
	select
		a."ObjType",
		a."DocEntry",
		b."LineNum",
		a."DocNum",
		a."StartDate",
		
		c."ItemCode",
		c."ItemName",
		a."PlannedQty",
		a."CmpltQty",
		
		d."ItemCode",
		d."ItemName",
		b."PlannedQty",
		b."IssuedQty",
		IFNULL(f."OnHand",0),
		
		b."wareHouse",
		e."ItmsGrpCod",
		e."ItmsGrpNam",
		case when  a."Status" = 'R' then 'Released' else 'Closed' end as "Status",-- r, l
		--ROW_NUMBER() OVER (PARTITION BY a."DocEntry",  c."ItemCode") as "STT"
		b."BaseQty",
		--
		e."U_Group"

		 
	from
		OWOR a
		inner join WOR1 b on a."DocEntry" = b."DocEntry"
		inner join OITM c on a."ItemCode" = c."ItemCode"
		inner join OITM d on b."ItemCode" = d."ItemCode"
		inner join OITB e on c."ItmsGrpCod" = e."ItmsGrpCod"
		left join OITW f on b."ItemCode" = f."ItemCode" and b."wareHouse" = f."WhsCode"
	where
		a."StartDate" between FromDate and ToDate
		and a."Status" in ('R','L')
		and a."Type" <>  'D'
		and (a."CmpltQty" > 0)
		
		and (b."IssueType" = 'M')
		
		and not (a."Status" = 'L' and b."IssuedQty" = 0);

--
	update a
	set a."CmpltQty" = b.Qty, a."AmountFG"  = b.Amount
	from :tbl a 
	inner join :pro04 b on a."FGCode"  = b.ItemCode and a."DocEntry" = b.WOID
	where a."DocNum" not in ('612400042','612400045','612400046');--6124%: la DocNum Lenh San Xuat
	
	---
 	update a
	set a."CmpltQty" = b.Qty, a."AmountFG"  = b.Amount
	from :tbl a 
	inner join (select ItemCode, Wono ,WoID, sum(Qty) Qty, sum (Amount)  Amount  from  :pro04 group by  ItemCode, Wono, WOID ) b on a."FGCode"  = b.ItemCode and a."DocEntry" = b.WOID
	 where a."DocNum"   in ('612400042','612400045','612400046');
--
--
insert into :tbl ( "DocEntry","DocNum",
					"FGCode","FGName",
					"ItmsGrpCod", "ItmsGrpNam", 
					"CmpltQty","AmountFG", "LineNum")
select WOID,WONo,
	ItemCode,ItemName,
	GrpCode,GrpName,
	
	Qty,Amount, 911
	
from :pro04 a 
where ItemCode not in (select distinct "FGCode" from :tbl b where a.WOID = b."DocEntry" );
--
/*
update a
set a."ItmsGrpCod"  = b."ItmsGrpCod"
from :tbl a 
inner join OITM b on a."FGCode" = b."ItemCode"
where ifnull(a."ItmsGrpCod",0) = 0 and  a."LineNum"  = 911;
*/
--- update line num
update a
set a."StartDate" =  b."StartDate", a."Status" = b."Status"
from  :tbl  a
--inner join (select distinct "StartDate","Status", "DocEntry" from :tbl  where  "LineNum"  <> 911  ) b on   a."DocEntry" = b."DocEntry"
inner join OWOR b on a."DocEntry" = b."DocEntry" 
where "LineNum"  = 911 and ifnull(a."StartDate",'') ='';
---- update line num va planqty
update a
set "LineNum" = b."LineNum",
	"PlanQty" =  b."PlannedQty" * -1,
	"BaseQty"  = b."BaseQty"
from :tbl a
inner join (select  b."LineNum", b."PlannedQty", b."DocEntry", b."BaseQty"
			from OWOR a inner join WOR1 b on a."DocEntry" = b."DocEntry" 
			where a."StartDate" between FromDate and ToDate
				and a."Status" in ('R','L') and a."Type" <>  'D'
				and (b."PlannedQty" < 0)) b on  a."DocEntry" = b."DocEntry"  -- PlannedQty<0: rework
 
where  a."LineNum"  = 911;

	update :tbl set "OpnQty" = CASE when IFNULL("IssuedQty",0) >= "IssueQty" then 0 else "IssueQty"-IFNULL("IssuedQty",0) end where "Status" = 'R';
		
	update :tbl set "NeedQty" = CASE when "OpnQty" > "OnHand" then "OpnQty" - "OnHand" else 0 end where "Status" = 'R';
		
	update a set "IsStock" = 'N' from :tbl a, :tbl b where  b."NeedQty" > 0 and a."Status" = 'R';
	update a set "IsOpn" = 'Y' from :tbl a, :tbl b where   b."OpnQty" > 0 and a."Status" = 'R';
		
	update a set "TotQty" = b."Quantity" from :tbl a, (select x."DocEntry", SUM(x."IssueQty") "Quantity" from :tbl x where x."IssueQty" > 0 group by x."DocEntry") b where a."DocEntry" = b."DocEntry";
		
	update a set "NeedQty"= ifnull("IssueQty",0) - ifnull("IssuedQty",0)  from :tbl a;
	
	delete from :tbl where ifnull("IssueQty",0) < 0;
	
	
	--- Resource: Item Group Machine, Labor (125,126)
	insert into :tblResource ( FGCode,ItemCode,ItemName,ItmsGrpCod,ItmsGrpNam, 
									WOID,WONO,
									IssuedQty, Amount, LineNum, U_Group)
	
	select e."ItemCode" as "FGCode",b."ItemCode",b."Dscription",  d."ItmsGrpCod", d."ItmsGrpNam",
						 b."BaseEntry" as "WOID",b."BaseRef" as "WONO",
						 sum(b."Quantity") as "IssuedQty",
						 
						  sum(b."LineTotal"),
						 b."BaseLine",
						 --
						d."U_Group"
	from OIGE a 
	inner join IGE1 b on a."DocEntry"  = b."DocEntry" 
	left join OITM c on c."ItemCode"  =  b."ItemCode"
	left join OITB d on c."ItmsGrpCod"  = d."ItmsGrpCod"
	inner join OWOR e on e."DocEntry" = b."BaseEntry"
	where a."DocDate" between FromDate and ToDate
		and (c."ItmsGrpCod" in (125,126, 233) or  ifnull(d."U_Group",'')  in  ('MC_TTMC','LB_TTLB') )
		and (b."BaseType" = 202 OR (a."DocNum" in ('322418143', '322418144') 
														or  a."DocNum" in (select "DocNum" from :tblExceptionDocNum  
																							where "DocType" = '60'))
		     )
		
	group by e."ItemCode",b."ItemCode",b."Dscription",  d."ItmsGrpCod", d."ItmsGrpNam",
						 b."BaseEntry",b."BaseRef",
						   b."BaseLine",
						   d."U_Group";
	-- update CmpltQty cho :tblResource
	update a
	set IssueQty  = f."PlannedQty",
		BaseQty = f."BaseQty",
		StartDate =  e."StartDate"
	from :tblResource a
	inner join OWOR e on e."DocEntry" = a.WOID
	inner join WOR1 f on f."DocEntry" = e."DocEntry" and a.LineNum = f."LineNum";	 
						 
	-- update CmpltQty cho :tblResource

	update a
	set a.CmpltQty  = b."CmpltQty"
	from :tblResource a
	left join (select "CmpltQty","FGCode","DocEntry" from :tbl ) b on a.FGCode = b."FGCode"
						 and a.WOID  = b."DocEntry";

	
	--- update gia tri nguyen vat lieu
	insert into :Amount_NVL ( ItemCode,WOID, QtyTotal,Qty, Amount,DocType, ItmsGrpCod )
		
	select   a."ItemCode", d."DocEntry" , 
					a."InQty"-a."OutQty",e."Quantity",
					a."TransValue" ,
					a."TransType",
					g."ItmsGrpCod"
					
			 			
			from "OINM" a
				--left join :tblWH b on a."Warehouse"=b.WHCode
				--left join #tblITEM c on a."ItemCode"=c.ItemCode
				left join "OWOR" d on a."AppObjAbs"=d."DocEntry"
				left join IBT1 e on e."BaseEntry" = a."CreatedBy" and a."ItemCode" = e."ItemCode" and e."BaseType" = a."TransType" 
							and   a."DocLineNum"  =  e."BaseLinNum"
				left join OBTN f on e."BatchNum"=f."DistNumber" and e."ItemCode"=f."ItemCode"
				inner join OITM g on g."ItemCode"  = a."ItemCode"
			where a."TransType"  in  (60,59) --and  a."AppObjType"='C' 
			   and (a."ApplObj"='202'   OR
			   					(a."BASE_REF" in ('322418143', '322418144','332416371','332416372')
			   						or
			   					    	a."BASE_REF"   in (select "DocNum" from :tblExceptionDocNum ) ) 
			   		)
					and a."DocDate" between FromDate and ToDate
					-- Loai phieu return component
					and a."BASE_REF" not in (select  "DocNum"  from OIGN a 
												inner join IGN1 b on a."DocEntry"  =  b."DocEntry"
												where b."BaseType" = 202 and b."IsByPrdct" ='Y' and b."BaseLine" is not null);
	-- 
	update x set Qty = (QtyTotal) from :Amount_NVL x where Qty is null or Qty = 0;
	update x set Qty = (- Qty) from :Amount_NVL x where DocType = 59 and Qty < 0;
	update x set Qty = (- Qty)from :Amount_NVL x where DocType = 60 and Qty > 0;
	
	update a set Amount =   (a.Qty * a.Amount)/ (case when QtyTotal = 0 then 1 else QtyTotal end)  from :Amount_NVL a ;
	----- update amount NVL la rework
	update a
	set "Amount" =   case when  b."Amount" <0 then b."Amount" * -1 else b."Amount" end
	from :tbl a
	inner join (select sum(Amount) as "Amount",WOID,ItemCode from :Amount_NVL b 
	
	where  ItmsGrpCod = 121  and DocType in (59,  60) group by  WOID,  ItemCode)b on b.WOID = a."DocEntry" --  tam bo dkien: and   
						and b.ItemCode = a."ItemCode";
						
	----- update amount NVL <> rework
	update a
	set "Amount" =   case when  b."Amount" <0 then b."Amount" * -1 else b."Amount" end
	from :tbl a
	inner join (select sum(Amount) as "Amount",WOID,ItemCode from :Amount_NVL b 
	where DocType  in ( 59,60) and ItmsGrpCod <> 121  group by  WOID,  ItemCode)b on b.WOID = a."DocEntry"
						and b.ItemCode = a."ItemCode"; 
--
 
	if (:Resource = 'Y') then
	
		insert into :tbl ("ItemCode", "ItemName","ItmsGrpCod","ItmsGrpNam",
		
		"IssueQty", "IssuedQty","DocEntry","DocNum","Amount","StartDate", "LineNum", "BaseQty", "CmpltQty","U_Group")
		
		select ItemCode,ItemName,ItmsGrpCod,ItmsGrpNam, IssueQty , IssuedQty as "IssuedQty", 
		WOID,WONO,Amount, StartDate,LineNum , BaseQty, CmpltQty, U_Group
		from :tblResource a;
		
		
	end if;
--
---- Disassebly

insert into :pro11 (STDQTY,STDAMOUNT, WONO, ITEMCODE, DISNO )
select STDQTY,STDAMOUNT, WONO, ITEMCODE, DISNO
from  PRO11_DATA_TEMP
where POSTINGDATE between Fromdate and todate;
--- insert thêm dư liệu từ bảng vât lý COSTING_STANDARDCOST
insert into :pro11 (STDQTY,STDAMOUNT, WONO, ITEMCODE, DISNO , U_Group)
select STDQTY,STDAMOUNT, WONO, ITEMCODE, DISNO, U_Group
from  COSTING_STANDARDCOST
where POSTINGDATE between Fromdate and todate;

 
Update a
set  a."IssuedQty" = ifnull( a."IssuedQty",0) +  ifnull((select sum(STDQTY) from  :pro11 b
													 where ifnull(DisNo,'') <> '' and a."DocNum" = b.WONO
													 		and a."ItemCode"  = b.ItemCode ),0)
from :tbl a
where a."ItmsGrpCod" in (125,126, 233) or ifnull(a."U_Group",'')   in ('MC_TTMC','LB_TTLB') ;


--
Update a
set "Amount" = ifnull("Amount",0) +    ifnull((select sum(STDAMOUNT) from  :pro11 b
													 where ifnull(DisNo,'') <> '' and a."DocNum" = b.WONO
													 		and a."ItemCode"  = b.ItemCode ),0)
from :tbl a
where  a."ItmsGrpCod" in (125,126, 233) or ifnull(a."U_Group",'')  in  ('MC_TTMC','LB_TTLB');
 

---
	update :tbl
	set "UnitPriceFG"  =   "AmountFG"/(case when "CmpltQty"  =0 then 1 else "CmpltQty" end),
		"UnitPrice"  =   "Amount"/(case when "IssuedQty"  =0 then 1 else "IssuedQty" end),
		"PlanCompltQty" = "CmpltQty" * "BaseQty";
--	
update a
set  "DueDate" = b."DueDate",
	"DueDate_Late" = DAYS_BETWEEN ( b."DueDate",CURRENT_DATE),
	"PO_Status"  =  b."U_POStatus"
from :tbl a
inner join OWOR b on a."DocEntry" =  b."DocEntry";	

update :tbl set "LineNum" = 911 where "ItmsGrpCod"  = 121; 
---
		update  a
		set "PlanQty"  = NULL,  "CmpltQty"  = NULL,  "AmountFG"  =NULL, "UnitPriceFG" = NULL
		from :tbl a
		where  "LineNum" >0  and "ItmsGrpCod"= '120' and "LineNum" <> 911
		or (a."ItemCode" in  (select ItemCode from :tblResource));
		
-----
update a
set  "FGCode"  = b."FGCode", "FGName" = b."FGName", "DueDate"  = b."DueDate",
	 "DueDate_Late"  = b."DueDate_Late",
	 "PO_Status" = b."PO_Status"
from :tbl  a 
inner join  (select * from :tbl ) b on a."DocEntry"  = b."DocEntry"
where a."FGCode" is null;
--
Update a
set "BomRate"  = c."PlannedQty" / b."PlannedQty"
from :tbl a 
inner join OWOR b on a."DocEntry"  =b."DocEntry"
inner join WOR1 c on c."DocEntry"  =b."DocEntry" and a."ItemCode" = c."ItemCode"
where ifnull(c."PlannedQty",0)>0;


---

update a
set "ItmsGrpCod"  =  c."ItmsGrpCod",
	"ItmsGrpNam" =   c."ItmsGrpNam",
	"U_Group"  = 	c."U_Group"
from :tbl a
left join OITM b on b."ItemCode"  =  a."ItemCode"
left join OITB c on b."ItmsGrpCod"  = c."ItmsGrpCod";
----
update a
set  a."FGCode" = b."ItemCode",
     a."FGName" = c."ItemName",
     "U_Group"  = 	d."U_Group"

from :tbl a 
inner join OWOR b on a."DocEntry" = b."DocEntry"
inner join OITM c on b."ItemCode" = c."ItemCode"
left join OITB d on c."ItmsGrpCod" = d."ItmsGrpCod"
where "LineNum" <> 911;
---and ifnull("FGName",'')  = '';


----
---
update a
set "PlanCompltQty" =  (select sum (b.STDQTY) from :pro11 b where  a."DocNum" = b.WONO 
					and  a."ItemCode" =  b.ITEMCODE ) ,
	"PlanCompltAmount" = (select sum (b.STDAMOUNT) from :pro11 b where  a."DocNum" = b.WONO and  a."ItemCode" =  b.ITEMCODE ) 
from :tbl a;
 
--
update a
set  "DiffQty" = ifnull("PlanCompltQty",0) - ifnull("IssuedQty",0),
	 "DiffAmount" =  ifnull("PlanCompltAmount",0) - ifnull("Amount",0)
from :tbl a;
--
DELETE FROM :tbl where "StartDate" is null;
update x set "Actual_BOM_Rate1" = 
(
	-- lấy CmpltQty của dòng đầu tiên Của từng LSX
	ifnull((select max("CmpltQty") from :tbl where "DocNum" = x."DocNum" and  "LineNum" = (select min("LineNum") from :tbl where "DocNum" = x."DocNum")),0)
	-- cộng với tổng CmpltQty của itemgroup 121 (hàng reowork)
	+ ifnull((select sum(a."CmpltQty") from :tbl a left join OITM b on b."ItemCode" = a."FGCode"  where a."DocNum" = x."DocNum" and b."ItmsGrpCod" =121 /*"LineNum" = (select max("LineNum") from :tbl where "DocNum" = x."DocNum"*/ and 1 != (select count(1) from :tbl where "DocNum" = x."DocNum" )),0)
	-- trừ đi tổng IssuedQty của itemgroup 121 (hàng reowork)
	- ifnull((select sum("IssuedQty") from :tbl where "DocNum" = x."DocNum"   and "ItmsGrpCod" =121),0)
    - ifnull((select sum("IssuedQty") from :tbl where "DocNum" = x."DocNum"   and "ItmsGrpCod" =120),0)

)
from :tbl x ;
--

update x set "Actual_BOM_Rate2" = 
(
	-- lấy CmpltQty của dòng đầu tiên Của từng LSX
	ifnull((select max("CmpltQty") from :tbl where "DocNum" = x."DocNum" and  "LineNum" = (select min("LineNum") from :tbl where "DocNum" = x."DocNum")),0)
	-- cộng với tổng CmpltQty của itemgroup 121 (hàng reowork)
	+ ifnull((select sum(a."CmpltQty") from :tbl a left join OITM b on b."ItemCode" = a."FGCode"  where a."DocNum" = x."DocNum" and b."ItmsGrpCod" =121 /*"LineNum" = (select max("LineNum") from :tbl where "DocNum" = x."DocNum"*/ and 1 != (select count(1) from :tbl where "DocNum" = x."DocNum" )),0)
 

)
from :tbl x ;

--- Actual_BOM_Rate
update a
set  "Actual_BOM_Rate" = case 
		when "ItmsGrpCod" = 113 and "Actual_BOM_Rate1" <> 0 then round("IssuedQty" / "Actual_BOM_Rate1",6)
		when "ItmsGrpCod" != 113 and "Actual_BOM_Rate2" <> 0 then round("IssuedQty" / "Actual_BOM_Rate2",6)
		else 0 end
from :tbl a;
---
 
update :tbl   
set "UnitPrice" =  0
where "IssuedQty"  =  0 and  ("ItmsGrpCod" in (125,126, 233)  or   ifnull("U_Group",'')  in ('MC_TTMC','LB_TTLB') );


--- Xử lý chênh lệch so với số liệu hạch toán
insert into :JE  (Account,AMOUNT,Amount_Debit_Credit)
 select "Account", sum("Credit"-"Debit") as Amount ,  
sum("Debit"-"Credit") as Amount_Debit_Credit 
		from "JDT1"
		where ("Account" like '621%' or "Account" like '622%' or "Account" like '627%') 
				and "RefDate" between fromdate and Todate group by "Account";
---======================== LBOR
/* old : 26: 08: 2024
select ifnull(sum(Amount_Debit_Credit),0) into total6229   from :JE  where "Account" like '6229%' ;	

		update a
		set a."PlanCompltAmount"  =  ifnull(a."PlanCompltAmount",0) +  ((case when :total6229 <0 then :total6229 * -1 else :total6229 end)  
									- (select case when  sum(ifnull(b."PlanCompltAmount",0)) <0  then  sum(ifnull(b."PlanCompltAmount",0))  * -1 
												else sum(ifnull(b."PlanCompltAmount",0)) end from  :tbl b where b."ItmsGrpCod" = 126) ) 
		from :tbl a
		where  a."DocNum" || a."LineNum"      = (select max(c."DocNum")||max(c."LineNum")  from :tbl c where  c."ItmsGrpCod" = 126)
				and "ItmsGrpCod" = 126 ;
*/
select ifnull(sum(Amount_Debit_Credit),0) into total6229   from :JE  where Account like '6229%' ;	

		update a
		set a."PlanCompltAmount"  =  ifnull(a."PlanCompltAmount",0) +  ((case when :total6229 <0 then :total6229 * -1 else :total6229 end)  
									- (select  abs(sum(ifnull(b."PlanCompltAmount",0))) from  :tbl b where b."ItmsGrpCod" = 126 or ifnull(b."U_Group",'')  = 'LB_TTLB')  ) 
		from :tbl a
		where  a."DocNum" || a."LineNum"      = (select max(c."DocNum")||max(c."LineNum")  from :tbl c where  c."ItmsGrpCod" = 126 or ifnull(c."U_Group",'')  = 'LB_TTLB')
				and ("ItmsGrpCod" = 126 or ifnull("U_Group",'')  = 'LB_TTLB') ;


---======================== Machine 
/* old : 26: 08: 2024
-- :total6279
  select ifnull(sum(Amount_Debit_Credit),0) into total6279   from :JE  where "Account" like '6279%' ;
		update a
		set a."PlanCompltAmount"  =  ifnull(a."PlanCompltAmount",0) +  ((case when :total6279 <0 then :total6279 * -1 else :total6279 end)  
									- (select case when  sum(ifnull(b."PlanCompltAmount",0)) <0  then  sum(ifnull(b."PlanCompltAmount",0))  * -1 
												else sum(ifnull(b."PlanCompltAmount",0)) end from  :tbl b where b."ItmsGrpCod" = 125) ) 
		from :tbl a
		where  a."DocNum" || a."LineNum"      = (select max(c."DocNum")||max(c."LineNum")  from :tbl c where  c."ItmsGrpCod" = 125)
				and "ItmsGrpCod" = 125 ;
*/
-- :total6279
  select ifnull(sum(Amount_Debit_Credit),0) into total6279   from :JE  where Account like '6279%' ;
		update a
		set a."PlanCompltAmount"  =  ifnull(a."PlanCompltAmount",0) +  ((case when :total6279 <0 then :total6279 * -1 else :total6279 end)  
									- (select   abs(sum(ifnull(b."PlanCompltAmount",0))) 
									from  :tbl b where b."ItmsGrpCod" in (125,233) or  ifnull(b."U_Group",'')  = 'MC_TTMC') ) 
		from :tbl a
		where  a."DocNum" || a."LineNum"      = (select max(c."DocNum")||max(c."LineNum")  from :tbl c where  c."ItmsGrpCod" in (125,233) or ifnull(c."U_Group",'')  = 'MC_TTMC')
				and ("ItmsGrpCod"  in (125,233) or   ifnull("U_Group",'')  = 'MC_TTMC');

---
tab_result = select a."ObjType",
a."DocEntry",
a."LineNum",
cast(a."DocNum" as nvarchar) as "DocNum",
a."StartDate",
a."FGCode",
a."FGName",
a."PlanQty",
a."CmpltQty",
a."ItemCode",
a."ItemName",
a."IssueQty",
a."IssuedQty",
a."OnHand",
a."OpnQty",
a."NeedQty",
a."WhsCode",
a."WhsName",
a."ItmsGrpCod",
a."ItmsGrpNam",
a."IsOpn",
a."IsStock",
a."TotQty",
a."Status",
a."AmountFG",
a."UnitPriceFG",
a."Amount",
a."UnitPrice",
a."PlanCompltQty",
a."PlanCompltAmount",
a."BaseQty",
cast(round(a."BomRate",6) as nvarchar) as "BomRate",
a."DueDate",
a."DueDate_Late",
a."PO_Status",
a."DiffQty",
a."DiffAmount",
a."Actual_BOM_Rate1",
a."Actual_BOM_Rate2",
a."Actual_BOM_Rate",
a."U_Group"
from :tbl  a  
inner join OWOR b on a."DocEntry"  =  b."DocEntry"
where  
(
	ifnull(:WODocNums, '') = '' 
					or cast (a."DocNum" as nvarchar) in (select "DATA" from split(:WODocNums,','))
)
and 
(
	ifnull(:ItemCodes, '') = '' 
					or cast (a."ItemCode" as nvarchar) in (select "DATA" from split(:ItemCodes,','))
)
 order by  "DocEntry", "LineNum";

end
