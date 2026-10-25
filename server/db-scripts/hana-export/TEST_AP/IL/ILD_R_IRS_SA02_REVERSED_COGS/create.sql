CREATE procedure ILD_R_IRS_SA02_Reversed_COGS
(
	in Year_Period int,
	in Period nvarchar(20),
	out tbl_rs table 
	(
		ItemCode nvarchar(20),
		ItemName nvarchar(100),
		
		System_date date,
		Posting_date date,
		
		DO_ID int, -- Docnum
		DO_NO nvarchar(50),
		DO_SSCC nvarchar(50),
		DO_SSCC_Qty decimal(19,6),
		DO_Qty_Total decimal(19,6),
		
		DO_Amount_Total decimal(19,6),-- lay gia tri cua phieu
		DO_SSCC_Cost decimal(19,6),
		DO_SSCC_Amount decimal(19,6),
		DO_LineNum int,
		WhsCode nvarchar(8),
	
		-- Return 
		Return_ID int,
		Return_NO varchar(50),
		Return_Date date,
		ORIBatch_Del varchar(50),
		--
		Inv_ID int,
		Inv_No	nvarchar(50),
		Inv_TaxNo	decimal(19,6),
		Inv_Date	date,
		--
		
		SSCC_Father1 nvarchar(50),
		Father1_Date date, 
		--
		GLAccount nvarchar(15),
		Currency nvarchar(10),
		UoM nvarchar(15),
		--
		Total_Delivery_Qty decimal(19,6),
		Total_Alloc_COGS decimal(19,6),
		Adj_Reversed_COGS decimal(19,6),
		
		Unit_Var_Cost decimal(19,6),
		U_Term nvarchar(3)
	 )

)
as
begin

--declare period_code varchar(20)  = '2024-02';

declare thang char(2)  = right (:Period,2);
declare nam char(4)  = left (:Period,4);

declare fromdate date = :nam||'/'||:thang || '/01';
declare todate  date  = last_day (:fromdate);

declare tbl table
(
	ItemCode nvarchar(20),
	ItemName nvarchar(100),
	
	System_date date,
	Posting_date date,
	
	DO_ID int, -- Docnum
	DO_NO nvarchar(50),
	DO_SSCC nvarchar(50),
	DO_SSCC_Qty decimal(19,6),
	DO_Qty_Total decimal(19,6),
	
	DO_Amount_Total decimal(19,6),-- lay gia tri cua phieu
	DO_SSCC_Cost decimal(19,6),
	DO_SSCC_Amount decimal(19,6),
	DO_LineNum int,
	WhsCode nvarchar(8),

	-- Return 
	Return_ID int,
	Return_NO varchar(50),
	Return_Date date,
	ORIBatch_Del varchar(50),
	--
	Inv_ID int,
	Inv_No	nvarchar(50),
	Inv_TaxNo	decimal(19,6),
	Inv_Date	date,
	--
	
	SSCC_Father1 nvarchar(50),
	Father1_Date date, 
	--
	GLAccount nvarchar(15),
	Currency nvarchar(10),
	UoM nvarchar(15),
	--
	Total_Delivery_Qty decimal(19,6),
	Total_Alloc_COGS decimal(19,6),
	Adj_Reversed_COGS decimal(19,6),
	
	Unit_Var_Cost decimal(19,6)	,
	U_Term nvarchar(3)
	
);
declare Do_List table
(
	DO_ID int,
	DO_NO nvarchar(50),
	DO_SSCC nvarchar(50),
	DO_SSCC_Qty decimal(19,6),
	ItemCode nvarchar(20),
	ItemName nvarchar(100),
	DO_LineNum int,
	
	Father1		varchar(100),
	AdmissionDate_Father1	date,
	
	DO_632 decimal(19,6),
	WhsCode nvarchar(8),
	System_date date,
	Posting_date date
	
);
declare Costing table
(
	ItemCode nvarchar(50),
	SalesQty decimal (19,6), -- Delivery Qty
	AllocToCOGS decimal (19,6)
);
call ILD_TruyXuatSSCCNhieuCap (tab_result); 
create local temporary table #TruyXuatSSCC_2 as (select * from :tab_result);
--
/*

--ILD_R_IRS_SALEREPORT
"Danh sách phiếu Delivery Order điều kiện sau:
1. DN đã xuất kho tháng này, chưa xuất hóa đơn hoặc xuất hóa đơn tháng khác 
->Bố sung 3 cột: Inv no., Tax inv, Inv Date
2. SSCC có Father1 được sản xuất trong kỳ 
->Bố sung 3 cột: SSCC, SSCC_Father1, Father1_Date"			

*/
--Truy xuat SSCC Cha




-- B1: Lấy những DO (Chi tiết đến SSCC), trong tháng
insert into :Do_List (DO_ID,DO_NO,DO_SSCC,DO_SSCC_Qty, ItemCode,ItemName, DO_LineNum, WhsCode,
					System_date, Posting_date)
					
select a."BaseEntry" as "DO_ID",
		a."BaseNum" as "DO_NO", 
		a."BatchNum" as "SSCC",
		a."Quantity" as "SSCC_Qty",
		
		a."ItemCode",
		a."ItemName",
		a."BaseLinNum" as "DO_LineNum",
		
		
		a."WhsCode",
		a."CreateDate",
		a."DocDate" as "Post_Date"
		
from IBT1 a
left join ODLN b on b."DocEntry" = a."BaseEntry"
where a."BaseType"  =  15 and b.CANCELED = 'N' and month(a."DocDate") = :thang 
						   and year(a."DocDate") = :nam;
--    
---============== Thông tin A/R Invoice ===============
AR_Invoice =  Select a."DocEntry" ,a."DocNum",a."U_InvNo" as "Inv_TaxNo", a."DocDate", a."DocDueDate" as "DeliveryDate",
					b."ItemCode",b."Dscription" as "ItemName",b."LineNum",
					b."Quantity",b."Price",b."LineTotal",
					--
					b."BaseRef",
					b."BaseType",
					b."BaseEntry"  as "DO_ID",
					b."BaseLine" as "DO_LineNum"
					From OINV a
					inner join INV1 b on a."DocEntry" =  b."DocEntry"
					where b."BaseEntry"||b."BaseLine" in (select distinct DO_ID||DO_LineNum from :Do_List)
					and a."CANCELED"  =  'N' and b."BaseType" = 15;

 
 
 
delete from :Do_List  
where  DO_ID|| DO_LineNum in 
						(select b."DO_ID"||b."DO_LineNum" from :AR_Invoice b
							where b."DocDate" <= last_day(nam||'-'||thang||'-01'));
							
							
-- Tim SSCC Ong Noi
if exists (select 1 from :Do_List ) then
	begin

		 -- LV0
		 update a
		 set a.Father1  = b.SSCC_OngNoi,
		 	 a.AdmissionDate_Father1 = b.Admission_Date_OngNoi
		 from :Do_List a
		 inner join #TruyXuatSSCC_2 b on   a.DO_SSCC = b.SSCC_LV0
		 where ifnull(b.SSCC_LV6,'') = '' and ifnull(b.SSCC_LV5,'') = '' 
		 and ifnull(b.SSCC_LV4,'') = '' and ifnull(b.SSCC_LV3,'') = '' 
		 and  ifnull(b.SSCC_LV2,'') =''  and ifnull(b.SSCC_LV1,'') = ''and  ifnull(b.SSCC_LV0,'') <> '';
		 		 -- LV1
		 update a
		 set a.Father1  = b.SSCC_OngNoi,
		 	 a.AdmissionDate_Father1 = b.Admission_Date_OngNoi
		 from :Do_List a
		 inner join #TruyXuatSSCC_2 b on   a.DO_SSCC = b.SSCC_LV1
		 where ifnull(b.SSCC_LV6,'') = '' and ifnull(b.SSCC_LV5,'') = ''
		  and ifnull(b.SSCC_LV4,'') = '' and ifnull(b.SSCC_LV3,'') = '' and  ifnull(b.SSCC_LV2,'') =''  and  ifnull(b.SSCC_LV1,'') <> '';
		  		 -- LV2
		 update a
		 set a.Father1  = b.SSCC_OngNoi,
		 	 a.AdmissionDate_Father1 = b.Admission_Date_OngNoi
		 from :Do_List a
		 inner join #TruyXuatSSCC_2 b on   a.DO_SSCC = b.SSCC_LV2
		 where ifnull(b.SSCC_LV6,'') = '' and ifnull(b.SSCC_LV5,'') = '' 
		 and ifnull(b.SSCC_LV4,'') = '' and ifnull(b.SSCC_LV3,'') = ''   and  ifnull(b.SSCC_LV2,'') <> '';
		 		 -- LV3
		 update a
		 set a.Father1  = b.SSCC_OngNoi,
		 	 a.AdmissionDate_Father1 = b.Admission_Date_OngNoi
		 from :Do_List a
		 inner join #TruyXuatSSCC_2 b on   a.DO_SSCC = b.SSCC_LV3
		 where ifnull(b.SSCC_LV6,'') = '' and ifnull(b.SSCC_LV5,'') = '' and ifnull(b.SSCC_LV4,'') = '' and  ifnull(b.SSCC_LV3,'') <> '';
		 		 -- LV4
		 update a
		 set a.Father1  = b.SSCC_OngNoi,
		 	 a.AdmissionDate_Father1 = b.Admission_Date_OngNoi
		 from :Do_List a
		 inner join #TruyXuatSSCC_2 b on   a.DO_SSCC = b.SSCC_LV4
		 where ifnull(b.SSCC_LV6,'') = '' and ifnull(b.SSCC_LV5,'') = '' and  ifnull(b.SSCC_LV4,'') <> '';
		 		 -- LV5
		 update a
		 set a.Father1  = b.SSCC_OngNoi,
		 	 a.AdmissionDate_Father1 = b.Admission_Date_OngNoi
		 from :Do_List a
		 inner join #TruyXuatSSCC_2 b on   a.DO_SSCC = b.SSCC_LV5
		 where ifnull(b.SSCC_LV6,'') = '' and  ifnull(b.SSCC_LV5,'') <> '';
		 		 -- LV6
		 		 update a
		 set a.Father1  = b.SSCC_OngNoi,
		 	 a.AdmissionDate_Father1 = b.Admission_Date_OngNoi
		 from :Do_List a
		 inner join #TruyXuatSSCC_2 b on   a.DO_SSCC = b.SSCC_LV6
		 where ifnull(b.SSCC_LV6,'') <> '';


	
	end;
End If;
--- Update  SSCC Ông Nội, và ngày tạo  = chính nó nếu nó là kẻ FA 
update a 
set a.Father1  = DO_SSCC,
	a.AdmissionDate_Father1 = b."InDate"
from  :Do_List a
inner join OBTN b on a.DO_SSCC = b."DistNumber"
where ifnull(a.Father1,'') = '';
--Lay SSCC thoa Dieu kien: SSCC có Father1 được sản xuất trong kỳ 
---delete from  :Do_List where month(AdmissionDate_Father1) <> :thang  and   year(AdmissionDate_Father1)  = :nam;

Do_List_FN =  select * from :Do_List where   month(AdmissionDate_Father1)  = :thang  and   year(AdmissionDate_Father1)  = :nam;

---
DO_Roots = select 	a."DocEntry" as "DO_ID", 
					b."BaseRef",
					b."BaseType",
					b."BaseEntry",
					b."BaseLine",
					b."Quantity" as "DO_Qty",
					b."LineTotal" as "DO_LineTotal",
					
					--delivery line
					b."ItemCode",
					b."Dscription" as "ItemName",
					b."LineNum" as "DO_LineNum",
					
					---
					b."Currency" as "Currency",
					b."unitMsr" as "UoM",
					a."U_Term"
		 
		from ODLN a
		inner join DLN1 b on   a."DocEntry" =  b."DocEntry"
		where "CANCELED"  = 'N' and a."DocEntry" in (select distinct DO_ID from :Do_List_FN);
--=============== Returns
Return_List = select 	b."DocEntry" as "Return_ID", 
					d."BaseRef",
					d."BaseType",
					d."BaseEntry",
					d."BaseLine",
					d."Quantity" as "Return_Qty",
					b."DocDate" as "Return_Date",
					--Return line
					d."ItemCode",
					d."Dscription" as "ItemName",
					d."LineNum" as "Return_LineNum"
		 
			from ORDN b
			inner join RDN1 d on   b."DocEntry" =  d."DocEntry"
			where month(b."DocDate") =:thang   and   year(b."DocDate") = :nam
					and "CANCELED"  = 'N';
------ Insert du lieu vao tbl
insert into :tbl (ItemCode,ItemName, System_date, Posting_date,
				  DO_ID, DO_NO, DO_SSCC, DO_SSCC_Qty, WhsCode, DO_LineNum,
				   SSCC_Father1,Father1_Date )
				  
select ItemCode,ItemName, System_date, Posting_date,
	DO_ID, DO_NO, DO_SSCC, DO_SSCC_Qty, WhsCode, DO_LineNum,
	--
	Father1,AdmissionDate_Father1
from :Do_List_FN;
---- insert du lieu return

		
Return_List_SSCC = select 
					a."BatchNum" as "SSCC"
					
					from IBT1 a
					where a."BaseType"  =  16 and a."DocDate" between  FromDate and ToDate
							and a."BaseEntry" in (select "Return_ID" from :Return_List); 
/*		
 
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
	a.Posting_date  = b.Posting_date,
	a.ItemName  = b.ItemName

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
*/
		
--================== UPDATE DU LIEU
DO_JE = select  "ItemCode","BASE_REF" as "DO_No",-sum("TransValue") as "TransValue"  from OINM
		where  substring("CostAct",0,3) in ('632','157') and "TransType" = 15
				and "DocDate" between Fromdate and Todate
			and "BASE_REF"  in (select distinct DO_NO From :tbl)
			group by   "ItemCode","BASE_REF";
			
----


--- update  DO_Qty_Total 
/* Khoa 28-02-2025 
update a
set a.DO_Qty_Total = b."DO_Qty",
	a.Currency   = b."Currency",
	a.UoM =  b."UoM"
from :tbl a
inner join :DO_Roots  b on a.DO_ID||a.DO_LineNum||a.ItemCode  = b."DO_ID"||b."DO_LineNum"||b."ItemCode";
*/
update a
set a.U_Term  = (select "U_Term" from  :DO_Roots  b where  a.DO_ID  = b."DO_ID" group by "U_Term")
from  :tbl a;

update a
set a.DO_Qty_Total  = (select sum(b."DO_Qty") from  :DO_Roots  b where  a.DO_ID ||a.ItemCode  = b."DO_ID"||b."ItemCode")
from  :tbl a;
--
update a
set a.GLAccount = (select max(b."InvntAct") from OINM b 
								where  b."TransType"  =15 
									   and b."CreatedBy"  = a.Do_ID)
from :tbl a;

--- update  DO_Amount_Total
update a
set DO_Amount_Total  =  b."TransValue"
from :tbl a
inner join :DO_JE b on a.DO_NO =  b."DO_No" and a.ItemCode = b."ItemCode";


/*
 
update a
set a.DO_Amount_Total = (select Max(b."CogsVal") 
						from OINM b
						where a.DO_ID ||a.ItemCode  = b."CreatedBy" ||b."ItemCode")
from  :tbl a;
 */

/*
update a
set a.DO_Amount_Total = (select Max(b."CogsVal") 
						from OINM b
						where a.DO_ID ||a.ItemCode  = b."CreatedBy" ||b."ItemCode")
from  :tbl a;
*/
---
update a
set  DO_SSCC_Amount  =  ifnull(DO_Amount_Total,0)/ DO_QTY_TOTAL *  DO_SSCC_QTY 
from :tbl a
where ifnull(DO_QTY_TOTAL,0) <> 0 *  ifnull(DO_SSCC_QTY,0);



---update DO_SSCC_Amount
update a
set DO_SSCC_Cost = IFNULL(DO_SSCC_Amount,1) /  ifnull(DO_SSCC_QTY,1)  
from :tbl a
WHERE ifnull(DO_SSCC_QTY,0) <> 0;



/* tam khoa
----Cập nhật thông tin A/R Invoice
update a
set Inv_ID = b."DocEntry",
	Inv_NO = b."DocNum",
	Inv_Date = b."DocDate", 
	Inv_TaxNo= b."Inv_TaxNo" 
from :tbl a
inner join :AR_Invoice b on a.DO_ID||a.DO_LineNum =  b."DO_ID"||b."DO_LineNum";
-----
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
	Inv_TaxNo= b."Inv_TaxNo" 
from :tbl a
inner join :AR_Crd_Invoice b on a.Return_ID||a.DO_LineNum =  b."BaseEntry"||b."BaseLine";

*/ --=====tam khoa 


---- Lay du lieu tu bang vat ly Costing
insert into :Costing (ItemCode,SalesQty,AllocToCOGS)
select distinct FGCode,SalesQty,AllocToCOGS
from TBL_COSTING_TOTAL
where  PERCODE  = :period;
---
update a
set a.Total_Delivery_Qty  = b.SalesQty,
	a.Total_Alloc_COGS  = b.AllocToCOGS
from :tbl a
inner join :Costing b on a.ItemCode  = b.ItemCode;
--- Adj_Reversed_COGS
update a
set Adj_Reversed_COGS  = DO_SSCC_Qty/Total_Delivery_Qty *Total_Alloc_COGS
from :tbl a
where ifnull(Total_Delivery_Qty,0)<> 0 and ifnull(Total_Alloc_COGS,0) <>0;
--- Bo sscc return

delete from :tbl where DO_SSCC IN (select "SSCC" from  :Return_List_SSCC);

-- update Unit_Var_Price
update  a
set Unit_Var_Cost = Total_Alloc_COGS/Total_Delivery_Qty
from :tbl a
where ifnull(Total_Delivery_Qty,0) <> 0;

update a
set Adj_Reversed_COGS = round(Adj_Reversed_COGS,2)
from   :tbl a;

tbl_rs =  select * from :tbl;

-- drop table
drop table #TruyXuatSSCC_2;

end
