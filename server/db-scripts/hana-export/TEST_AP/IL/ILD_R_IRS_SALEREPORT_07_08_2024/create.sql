CREATE PROCEDURE ILD_R_IRS_SaleReport_07_08_2024
(
	in FromDate date,
	in ToDate date,
	in Customer varchar(5000),--ILD_LAY_Customer
	in DONo varchar(5000), --ILD_LAY_DeliveryNo
	in SONo varchar(5000), --ILD_LAY_SalesOrderNo
	in DocType char(1) -- 1: Delivery Order, 2: Invoiced
)
as
begin
declare tbl table
(
	-- Thong tin tu phieu Delivery
	DO_ID int,
	DO_No varchar(50),
	DO_SSCC nVarChar(50),
	DO_SSCC_Qty decimal(21,6),
	--- Delivery line
	ItemCode nvarchar(50),
	ItemName nvarchar(250),
	DO_LineNum	int,

	DO_BaseRef varchar(20),
	DO_BaseType int,
	DO_BaseEntry int,
	DO_BaseLine int,
	--
	DO_Qty_Total decimal(21,6),
	DO_632 decimal(21,6),
	COGS decimal(21,6),
	--
	WOID	int,
	WONo	varchar(50),	
	
	Batch varchar(50),
	SSCC_Status varchar(50),
	MnfDate date,
	ExpDate date,
	AdmissiongDate date,
	
	
	Customer_Code varchar(15),
	Customer_Name nvarchar(100),
	Contact_Person nvarchar(150),
	
	DO_Date	date,
	DO_DocDate date,
	ContNo varchar(50),
	ShipTo nvarchar(250), -- Dia chi
	ShipToAddress nvarchar(250),  -- Ten Congty

	
	--
	SO_ID int,
	SO_NO varchar(50),
	SO_Date date,
	
	-- A/R Invoice
	Inv_ID int,
	Inv_NO varchar(50),
	Inv_TaxNo varchar(50),
	Inv_Date date,
	
	Inv_Price decimal(21,6),
	Inv_Quantity decimal(21,6),
	Inv_SalesAmount  decimal(21,6),
	--
	PO_CompltQty	decimal(21,6),
	StdAmount_MC_LB_09 decimal(21,6),
	Standard_Cost	decimal(21,6),
	COGS_Material	decimal(21,6),
	GMVC	decimal(21,6),
	
	-- Return 
	Return_ID int,
	Return_NO varchar(50),
	Return_Date date,
	ORIBatch_Del varchar(50),
	
	Father1		varchar(100),
	AdmissionDate_Father1	date
);
 
--Truy xuat SSCC Cha
call ILD_TruyXuatSSCCNhieuCap (tab_result); 
create local temporary table #TruyXuatSSCC_2 as (select * from :tab_result);
--FromDate,ToDate,Resource,OutSoucing
/* tam khoa: 25- 06- 2024
CALL ILD_R_IRS_LenhSanXuatDaNhapThanhPham('2020-01-01',ToDate,'Y','N', datapro09);
create local temporary table #pro09 as (select sum("Amount") as "Amount",sum("PlanCompltAmount") as "StdAmount","DocNum" as "WONO","DocEntry" as "WOID", "ItmsGrpCod"  
											from :datapro09
											where "ItmsGrpCod" in (113,114,125,126)
											group by "DocNum","DocEntry", "ItmsGrpCod"  );
*/										
-- pro04:fdate,tdate,itemcode,warehouse,batch,DocNumPO,OutSoucing
/* tam khoa: 25- 06- 2024
Call USP_RPT_PROD_04 ('2020-01-01',ToDate,'','','','','Y',datapro04);
create local temporary table #pro04 aS (select sum(LABOUR) as "LABOUR", 
												sum(MACHINE) as "MACHINE",
												sum(MATERIALS) as "MATERIALS", SSCC
										from :datapro04
										group by SSCC);
*/
--

/* Luong du lieu: Delivery  --> list down SSCC
*/
DO_List = select 	b."DocEntry" as "DO_ID", 
					d."BaseRef",
					d."BaseType",
					d."BaseEntry",
					d."BaseLine",
					d."Quantity" as "DO_Qty",
					--
					b."CardCode" as "Customer_Code",
					b."CardName" as "Customer_Name",
					c."Name" as "Contact_Person",
					b."DocDueDate" as "Delivery_Date",
					b."DocDate" as "Delivery_DocDate",
					b."U_ContNo" as "ContNo",
					case when  SUBSTR_AFTER (upper(b."PickRmrk"),'-') = '' then  upper(b."PickRmrk") 
									else LTRIM(SUBSTR_AFTER (upper(b."PickRmrk"),'-')) end as "ShipTo",
					b."Address2" as "ShipToAddress",
					--delivery line
					d."ItemCode",
					d."Dscription" as "ItemName",
					d."LineNum" as "DO_LineNum"
		 
		from ODLN b
		left join OCPR c on b."CntctCode" = c."CntctCode"
		inner join DLN1 d on   b."DocEntry" =  d."DocEntry"
		where 
		--b."DocDate" between  FromDate and ToDate and 
		"CANCELED"  = 'N'
		and
			(
					ifnull(:Customer, '') = '' 
					or cast (b."CardCode" as nvarchar) in (select "DATA" from split(:Customer ,','))
			)
			and
			(
					ifnull(:DONo, '') = '' 
					or cast (b."DocNum" as nvarchar) in (select "DATA" from split(:DONo ,','))
			);
----
insert into :tbl (DO_ID,DO_NO,DO_SSCC,DO_SSCC_Qty,
					ItemCode, DO_LineNum)
select a."BaseEntry" as "DO_ID",
		a."BaseNum" as "DO_NO", 
		a."BatchNum" as "SSCC",
		a."Quantity" as "SSCC_Qty",
		
		a."ItemCode",
		a."BaseLinNum" as "DO_LineNum"
		
from IBT1 a
where a."BaseType"  =  15 
--and a."DocDate" between  FromDate and ToDate
		and a."BaseEntry" in (select "DO_ID" from :DO_List);
		--and a."BaseEntry"  =53;
		
--- update thong tin Delivery
 
update a
set a.ItemName  = b."ItemName",
	a.Customer_Code = b."Customer_Code",
	a.Customer_Name = b."Customer_Name",
	a.Contact_Person = b."Contact_Person",
	a.DO_Date = b."Delivery_Date",
	a.DO_DocDate  =b."Delivery_DocDate",
	a.ContNo = b."ContNo",
	a.ShipTo = b."ShipTo",
	a.ShipToAddress = b."ShipToAddress",
	
	a.DO_BaseRef =  b."BaseRef",
	a.DO_BaseType =  b."BaseType",
	a.DO_BaseEntry =  b."BaseEntry",
	a.DO_BaseLine =  b."BaseLine",
	a.DO_Qty_Total =  b."DO_Qty"
from :tbl a
inner join :DO_List  b on a.DO_ID||a.DO_LineNum||a.ItemCode  = b."DO_ID"||b."DO_LineNum"||b."ItemCode";

-- Returns
Return_List = select 	b."DocEntry" as "Return_ID", 
					d."BaseRef",
					d."BaseType",
					d."BaseEntry",
					d."BaseLine",
					d."Quantity" as "Return_Qty",
					--
					b."CardCode" as "Customer_Code",
					b."CardName" as "Customer_Name",
					c."Name" as "Contact_Person",
					b."DocDueDate" as "Return_Date",
					b."U_ContNo" as "ContNo",
					case when  SUBSTR_AFTER (upper(b."PickRmrk"),'-') = '' then  upper(b."PickRmrk") 
									else LTRIM(SUBSTR_AFTER (upper(b."PickRmrk"),'-')) end as "ShipTo",
					b."Address2" as "ShipToAddress",
					--Return line
					d."ItemCode",
					d."Dscription" as "ItemName",
					d."LineNum" as "Return_LineNum"
		 
		from ORDN b
		left join OCPR c on b."CntctCode" = c."CntctCode"
		inner join RDN1 d on   b."DocEntry" =  d."DocEntry"
		where b."DocDate" between  FromDate and ToDate and "CANCELED"  = 'N'
		and
			(
					ifnull(:Customer, '') = '' 
					or cast (b."CardCode" as nvarchar) in (select "DATA" from split(:Customer ,','))
			);

---
insert into :tbl (Return_ID,Return_NO,DO_SSCC,DO_SSCC_Qty,
					ItemCode, DO_LineNum)
select a."BaseEntry" as "Return_ID",
		a."BaseNum" as "Return_NO", 
		a."BatchNum" as "SSCC",
		-a."Quantity" as "SSCC_Qty",
		
		a."ItemCode",
		a."BaseLinNum" as "Return_LineNum"
		
from IBT1 a
where a."BaseType"  =  16 and a."DocDate" between  FromDate and ToDate
		and a."BaseEntry" in (select "Return_ID" from :Return_List);
-- Cap nhap so U_ORIBatch_Del cho phieu return
update a
set a.ORIBatch_Del = b."U_ORIBatch_Del"

from :tbl a
inner join OBTN b on a.DO_SSCC =  b."DistNumber"
where ifnull(a.DO_NO,'') ='';	
		
-- Cap nhap so DO cho phieu return
update a
set a.DO_ID = b.DO_ID,
	a.DO_NO = b.DO_NO,
	a.DO_Date  = b.DO_Date,
	a.ItemName  = b.ItemName,
	a.Customer_Code = b.Customer_Code,
	a.Customer_Name = b.Customer_Name,
	a.Contact_Person = b.Contact_Person,
	a.ContNo = b.ContNo,
	a.ShipTo = b.ShipTo,
	a.ShipToAddress = b.ShipToAddress

from :tbl a
inner join :tbl b on a.ORIBatch_Del  = b.DO_SSCC
where ifnull(a.DO_NO,'') ='';
-- update thong tin return tu Return List
update a
set a.Return_Date = b."Return_Date",
	a.DO_Qty_Total  = -b."Return_Qty"
from :tbl  a
inner join :Return_List b on a.Return_ID = b."Return_ID"
where ifnull(a.Return_NO,'')<> '';




---============== Thông tin A/R Invoice ===============
Sale_Orders = Select a."DocEntry",a."DocNum",a."DocDate", a."DocDueDate" as "DeliveryDate",
					b."ItemCode",b."Dscription" as "ItemName",b."LineNum" 
				From ORDR a
				inner join RDR1 b on a."DocEntry" =  b."DocEntry"
				where a."DocEntry"||b."LineNum" in (select distinct DO_BaseEntry||DO_BaseLine from :tbl where DO_BaseType  = 17)
				and a."CANCELED"  =  'N'
				and
				(
					ifnull(:SONo, '') = '' 
					or cast (a."DocNum" as nvarchar) in (select "DATA" from split(:SONo ,','))
				);


----Cập nhật thông tin Sale Order
update a
set SO_ID = b."DocEntry",
	SO_NO = b."DocNum",
	SO_Date = b."DocDate" 
	
from :tbl a
inner join :Sale_Orders b on a.DO_BaseEntry||a.DO_BaseLine =  b."DocEntry"||b."LineNum"
where ifnull(a.SO_NO,'') ='';

---============== Thông tin A/R Invoice ===============
AR_Invoice =  Select a."DocEntry",a."DocNum",a."U_InvNo" as "Inv_TaxNo", a."DocDate", a."DocDueDate" as "DeliveryDate",
					b."ItemCode",b."Dscription" as "ItemName",b."LineNum",
					b."Quantity",b."Price",b."LineTotal",
					--
					b."BaseRef",
					b."BaseType",
					b."BaseEntry",
					b."BaseLine"
					From OINV a
					inner join INV1 b on a."DocEntry" =  b."DocEntry"
					where b."BaseEntry"||b."BaseLine" in (select distinct DO_ID||DO_LineNum from :tbl)
					and a."CANCELED"  =  'N' and b."BaseType" = 15;

----Cập nhật thông tin A/R Invoice
update a
set Inv_ID = b."DocEntry",
	Inv_NO = b."DocNum",
	Inv_Date = b."DocDate", 
	Inv_TaxNo= b."Inv_TaxNo",
	
	Inv_Price = b."Price", 
	Inv_Quantity = b."Quantity",
	Inv_SalesAmount = ifnull( b."Price",0)  * ifnull(a.DO_SSCC_Qty,0)
from :tbl a
inner join :AR_Invoice b on a.DO_ID||a.DO_LineNum =  b."BaseEntry"||b."BaseLine";

---============== Thông tin A/R Credit Memo Invoice ===============
AR_Crd_Invoice =  Select a."DocEntry",a."DocNum",a."U_InvNo" as "Inv_TaxNo", a."DocDate", a."DocDueDate" as "Return_Date",
					b."ItemCode",b."Dscription" as "ItemName",b."LineNum",
					b."Quantity",b."Price",b."LineTotal",
					--
					b."BaseRef",
					b."BaseType",
					b."BaseEntry",
					b."BaseLine"
					From ORIN a
					inner join RIN1 b on a."DocEntry" =  b."DocEntry"
					where b."BaseEntry"||b."BaseLine" in (select distinct Return_ID||DO_LineNum from :tbl)
					and a."CANCELED"  =  'N' and b."BaseType" = 16;

----Cập nhật thông tin A/R Credit Memo Invoice
update a
set Inv_ID = b."DocEntry",
	Inv_NO = b."DocNum",
	Inv_Date = b."DocDate", 
	Inv_TaxNo= b."Inv_TaxNo",
	
	Inv_Price = b."Price", 
	Inv_Quantity = b."Quantity",
	Inv_SalesAmount = ifnull( b."Price",0)  * ifnull(a.DO_SSCC_Qty,0)
from :tbl a
inner join :AR_Crd_Invoice b on a.Return_ID||a.DO_LineNum =  b."BaseEntry"||b."BaseLine";
					
---============== Thông tin Lệnh sản xuất theo SSCC ===============
PO_SSCC_Lve1  = Select  a."BsDocEntry" as "WOID", b."DocNum" as "WONO",a."BatchNum" as "SSCC", a."ItemCode"
		   From  IBT1 a 
		   inner join OWOR b on a."BsDocEntry" = b."DocEntry"
		    WHERE "BaseType" = 59  and "BsDocType"  = 202 and "BatchNum"  in (select distinct DO_SSCC from :tbl);

 
--
update a
set a.WOID = b."WOID",
	a.WONo = b."WONO"
from :tbl a
inner join  :PO_SSCC_Lve1 b on a.DO_SSCC = b."SSCC";
----

create local temporary table #PO_SSCC_Lve2_3
(
	WOID		int,
	SSCC 		nvarchar(50),
	SSCC_Cha 	nvarchar(50),
	SSCC_OngNoi nvarchar(50)
);
insert into  #PO_SSCC_Lve2_3 (SSCC)
select distinct DO_SSCC from :tbl where ifnull(WOID,0) = 0;
-- update thong tin SSCC Cha
update a
set a.SSCC_Cha = b."U_ORIBatch"
from #PO_SSCC_Lve2_3 a
inner join OBTN b on a.SSCC = b."DistNumber";

-- update thong tin SSCC Ong Noi
update a
set a.SSCC_OngNoi = b."U_ORIBatch"
from #PO_SSCC_Lve2_3 a
inner join OBTN b on a.SSCC_Cha = b."DistNumber";
--- update thong tin lenh san xuat (Không có SSCC Cấp 3 (ông nội))
update a
set WOID = b."WOID"
from #PO_SSCC_Lve2_3 a
inner join  (select  "BsDocEntry" as "WOID","BatchNum","ItemCode"
				from  IBT1 b
				WHERE "BaseType"  = 59 and "BsDocType" =202) b on a.SSCC_Cha =  b."BatchNum"
where ifnull(SSCC_OngNoi,'') ='';
--- update thong tin lenh san xuat = SSCC Cấp 3 (ông nội)
update a
set WOID = b."WOID"
from #PO_SSCC_Lve2_3 a
inner join  (select  "BsDocEntry" as "WOID","BatchNum","ItemCode"
				from  IBT1 b
				WHERE "BaseType"  = 59 and "BsDocType" =202) b on a.SSCC_OngNoi =  b."BatchNum"
where ifnull(SSCC_OngNoi,'') <>'';
--

Update a
set WOID =b.WOID
from :tbl a
inner join #PO_SSCC_Lve2_3 b on a.DO_SSCC  = b.SSCC;
---
update a
set a.WONO  = b."DocNum",
	a.PO_CompltQty  = b."CmpltQty"
from :tbl a
inner join  OWOR b on a.WOID = b."DocEntry";
-- Cập nhật thông tin Sale order  Cho phiếu return
update a
set a.SO_ID = b.SO_ID,
	a.SO_NO = b.SO_NO,
	a.SO_Date = b.SO_Date,
	a.PO_CompltQty = b.PO_CompltQty, 
	a.WOID = b.WOID, 
	a.WONo = b.WONo 
	
from :tbl a
inner join :tbl b on a.DO_ID||a.ORIBatch_Del =  b.DO_ID||b.DO_SSCC
where ifnull(a.ORIBatch_Del,'') <>'';


---============== Thông tin Lệnh sản xuất theo SSCC: Delivery ===============
DO_JE = select  "ItemCode","BASE_REF" as "DO_No",-sum("TransValue") as "TransValue"  from OINM
		where    "CostAct" like '632%' and "TransType" = 15
				and "DocDate" between Fromdate and Todate
			and "BASE_REF"  in (select distinct DO_NO From :tbl)
			group by   "ItemCode","BASE_REF";
			
----
update a
set DO_632  =  b."TransValue"
from :tbl a
inner join :DO_JE b on a.DO_NO =  b."DO_No" and a.ItemCode = b."ItemCode";

---============== Thông tin Lệnh sản xuất theo SSCC - Return ===============
Return_JE = select   "ItemCode","BASE_REF" as "Return_No",sum("TransValue") as "TransValue"  from OINM
		where    "CostAct" like '632%' and "TransType" = 16
				and "DocDate" between Fromdate and Todate
			and "BASE_REF"  in (select distinct Return_No From :tbl)
			group by   "ItemCode","BASE_REF";
			
----
update a
set DO_632  =  b."TransValue"
from :tbl a
inner join :Return_JE b on a.Return_No =  b."Return_No" and a.ItemCode =b."ItemCode";
-- 
update a
set COGS = ifnull(DO_632,0)/DO_Qty_Total * DO_SSCC_Qty
from  :tbl a
where ifnull(DO_Qty_Total * DO_SSCC_Qty,0) >0;

--
/* tam khoa ngay 25-06-2024
update a
set  StdAmount_MC_LB_09 =  (select sum(b."Amount") from  #pro09 b where a.WONO = b."WONO" and  b."ItmsGrpCod" in (125,126))
from :tbl a;
*/
---

/* tam khoa ngay 25-06-2024
--- COGS_Material
update a
set  COGS_Material = (select sum(MATERIALS) from #pro04 b  where a.DO_SSCC  = b.SSCC)
from :tbl a;
--- COGS_Material
update a
set  Standard_Cost = (select sum(LABOUR)+  sum(MACHINE) from #pro04 b  where a.DO_SSCC  = b.SSCC)
from :tbl a;



-- GMVC
	update :tbl
	set GMVC = ifnull(Inv_SalesAmount,0) - ifnull(COGS_Material,0) - ifnull(Standard_Cost,0) ;
	 
*/

---- Cập nhật thông tin Batch - OBTN
update a
set Batch = b."MnfSerial",
	SSCC_Status = b."LotNumber" ,
	MnfDate = b."MnfDate",
	ExpDate = b."ExpDate",
	AdmissiongDate  = b."InDate" 
from :tbl a
inner join OBTN b on a.DO_SSCC = b."DistNumber";
---
if exists (select 1 from :tbl ) then
	begin

		 -- LV0
		 update a
		 set a.Father1  = b.SSCC_OngNoi,
		 	 a.AdmissionDate_Father1 = b.Admission_Date_OngNoi
		 from :tbl a
		 inner join #TruyXuatSSCC_2 b on   a.DO_SSCC = b.SSCC_LV0
		 where ifnull(b.SSCC_LV6,'') = '' and ifnull(b.SSCC_LV5,'') = '' 
		 and ifnull(b.SSCC_LV4,'') = '' and ifnull(b.SSCC_LV3,'') = '' 
		 and  ifnull(b.SSCC_LV2,'') =''  and ifnull(b.SSCC_LV1,'') = ''and  ifnull(b.SSCC_LV0,'') <> '';
		 		 -- LV1
		 update a
		 set a.Father1  = b.SSCC_OngNoi,
		 	 a.AdmissionDate_Father1 = b.Admission_Date_OngNoi
		 from :tbl a
		 inner join #TruyXuatSSCC_2 b on   a.DO_SSCC = b.SSCC_LV1
		 where ifnull(b.SSCC_LV6,'') = '' and ifnull(b.SSCC_LV5,'') = ''
		  and ifnull(b.SSCC_LV4,'') = '' and ifnull(b.SSCC_LV3,'') = '' and  ifnull(b.SSCC_LV2,'') =''  and  ifnull(b.SSCC_LV1,'') <> '';
		  		 -- LV2
		 update a
		 set a.Father1  = b.SSCC_OngNoi,
		 	 a.AdmissionDate_Father1 = b.Admission_Date_OngNoi
		 from :tbl a
		 inner join #TruyXuatSSCC_2 b on   a.DO_SSCC = b.SSCC_LV2
		 where ifnull(b.SSCC_LV6,'') = '' and ifnull(b.SSCC_LV5,'') = '' 
		 and ifnull(b.SSCC_LV4,'') = '' and ifnull(b.SSCC_LV3,'') = ''   and  ifnull(b.SSCC_LV2,'') <> '';
		 		 -- LV3
		 update a
		 set a.Father1  = b.SSCC_OngNoi,
		 	 a.AdmissionDate_Father1 = b.Admission_Date_OngNoi
		 from :tbl a
		 inner join #TruyXuatSSCC_2 b on   a.DO_SSCC = b.SSCC_LV3
		 where ifnull(b.SSCC_LV6,'') = '' and ifnull(b.SSCC_LV5,'') = '' and ifnull(b.SSCC_LV4,'') = '' and  ifnull(b.SSCC_LV3,'') <> '';
		 		 -- LV4
		 update a
		 set a.Father1  = b.SSCC_OngNoi,
		 	 a.AdmissionDate_Father1 = b.Admission_Date_OngNoi
		 from :tbl a
		 inner join #TruyXuatSSCC_2 b on   a.DO_SSCC = b.SSCC_LV4
		 where ifnull(b.SSCC_LV6,'') = '' and ifnull(b.SSCC_LV5,'') = '' and  ifnull(b.SSCC_LV4,'') <> '';
		 		 -- LV5
		 update a
		 set a.Father1  = b.SSCC_OngNoi,
		 	 a.AdmissionDate_Father1 = b.Admission_Date_OngNoi
		 from :tbl a
		 inner join #TruyXuatSSCC_2 b on   a.DO_SSCC = b.SSCC_LV5
		 where ifnull(b.SSCC_LV6,'') = '' and  ifnull(b.SSCC_LV5,'') <> '';
		 		 -- LV6
		 		 update a
		 set a.Father1  = b.SSCC_OngNoi,
		 	 a.AdmissionDate_Father1 = b.Admission_Date_OngNoi
		 from :tbl a
		 inner join #TruyXuatSSCC_2 b on   a.DO_SSCC = b.SSCC_LV6
		 where ifnull(b.SSCC_LV6,'') <> '';


	
	end;
End If;
--- Update  SSCC Ông Nội, và ngày tạo  = thong tin sscc ORIBatch Del
update a 
set a.Father1  = a.ORIBatch_Del,
	a.AdmissionDate_Father1 = b."InDate"
from  :tbl a
inner join OBTN b on a.ORIBatch_Del  = b."DistNumber"
where ifnull(a.Father1,'') = '' and  ifnull(a.ORIBatch_Del,'') <> '';
--- Update  SSCC Ông Nội, và ngày tạo  = chính nó nếu nó là kẻ FA 
update a 
set a.Father1  = DO_SSCC,
	a.AdmissionDate_Father1 = AdmissiongDate
from  :tbl a
where ifnull(a.Father1,'') = '';

If (DocType = 1) Then -- delivery
	update a
	set a.DO_No =  b.DO_No,
		a.DO_Date = b.DO_Date,
		a.DO_DocDate = b.DO_DocDate,
		a.ItemName = b.ItemName
	from  :tbl a
	inner join (select distinct DO_No,DO_Date,DO_DocDate,DO_SSCC ,ItemName
					from :tbl b 
					where ifnull(b.Return_NO,'') ='') b on   a.DO_SSCC = b.DO_SSCC  
	where ifnull(a.Return_NO,'') <> '';
end if;
 
--
If (DocType = 1) Then
			select 	
			DO_No   "Delivery note",
			DO_Date	"Delivery Date",
			DO_DocDate	"Delivery Posting Date",
			Customer_Code    "Customer Code",
			Customer_Name "Customer Name",
			Contact_Person "Contact person",
			
			ContNo "Container No",
			ShipTo || '. '|| ShipToAddress as "Ship-to" , -- Dia chi
		
			--
			SO_NO "Sale Order No",
			SO_Date "Order date",
			
			WONo "Production Order",
			
			ItemCode "Item code",
			ItemName "Item name",
			
			DO_SSCC   "SSCC",
			SSCC_Status "SSCC Status",
			DO_SSCC_Qty "Quantity",
		
			Batch "Production batch",
			 
			MnfDate "Manufacturing Date",
			ExpDate "Expiration Date",
			AdmissiongDate	"Admissiong Date",
			-- A/R Invoice
			Inv_ID,
			Inv_NO "Invoice No",
			Inv_TaxNo "Tax Invoice",
			Inv_Date "Invoice Date",
			
			Inv_Price "Unit Price",
			Inv_SalesAmount  "Sales Amount",
			
			COGS,
			COGS_Material "COGS  (material)" ,
			Standard_Cost "Standard cost",
			GMVC,
		 
			--
			DO_Qty_Total,
			DO_632,
			WOID,
			SO_ID,
			DO_ID,
			PO_CompltQty,
			StdAmount_MC_LB_09,
			-- Return
			Return_ID,
			Return_NO "Return No",
			Return_Date "Return Date",
			ORIBatch_Del "ORIBatch Del",
			Father1,
			AdmissionDate_Father1
			
		from :tbl
		where  DO_DocDate between FromDate and Todate  and 
			  
		
			
			(
					ifnull(:SONo, '') = '' 
					or cast (SO_NO as nvarchar) in (select "DATA" from split(:SONo ,','))
				
			)
			and
			(
					ifnull(:Customer, '') = '' 
					or cast (Customer_Code as nvarchar) in (select "DATA" from split(:Customer ,','))
			)
			and
			(
					ifnull(:DONo, '') = '' 
					or cast (DO_No as nvarchar) in (select "DATA" from split(:DONo ,','))
			)
		order by  DO_No, DO_DocDate;
Else
			select 	
			DO_No   "Delivery note",
			DO_Date	"Delivery Date",
			DO_DocDate	"Delivery Posting Date",
			Customer_Code    "Customer Code",
			Customer_Name "Customer Name",
			Contact_Person "Contact person",
			
			ContNo "Container No",
			ShipTo || '. '|| ShipToAddress as "Ship-to" , -- Dia chi
		
			--
			SO_NO "Sale Order No",
			SO_Date "Order date",
			
			WONo "Production Order",
			
			ItemCode "Item code",
			ItemName "Item name",
			
			DO_SSCC   "SSCC",
			SSCC_Status "SSCC Status",
			DO_SSCC_Qty "Quantity",
		
			Batch "Production batch",
			 
			MnfDate "Manufacturing Date",
			ExpDate "Expiration Date",
			AdmissiongDate	"Admissiong Date",
			-- A/R Invoice
			Inv_ID,
			Inv_NO "Invoice No",
			Inv_TaxNo "Tax Invoice",
			Inv_Date "Invoice Date",
			
			Inv_Price "Unit Price",
			Inv_SalesAmount  "Sales Amount",
			
			COGS,
			COGS_Material "COGS  (material)" ,
			Standard_Cost "Standard cost",
			GMVC,
		 
			--
			DO_Qty_Total,
			DO_632,
			WOID,
			SO_ID,
			DO_ID,
			PO_CompltQty,
			StdAmount_MC_LB_09,
			-- Return
			Return_ID,
			Return_NO "Return No",
			Return_Date "Return Date",
			ORIBatch_Del "ORIBatch Del",
			
			Father1,
			AdmissionDate_Father1
			
		 
		
		from :tbl
		where  Inv_Date between FromDate and Todate
			and
			
			(
					ifnull(:SONo, '') = '' 
					or cast (SO_NO as nvarchar) in (select "DATA" from split(:SONo ,','))
				
			)
			and
			(
					ifnull(:Customer, '') = '' 
					or cast (Customer_Code as nvarchar) in (select "DATA" from split(:Customer ,','))
			)
			and
			(
					ifnull(:DONo, '') = '' 
					or cast (DO_No as nvarchar) in (select "DATA" from split(:DONo ,','))
			)
		order by  DO_ID;
End IF;



-- drop table
drop table #PO_SSCC_Lve2_3;
--drop table #pro09;
--drop table #pro04;
drop table #TruyXuatSSCC_2;
end;

