CREATE procedure USP_RPT_PROD_04_29_06_2025 (
	in fdate			timestamp,
	in tdate			timestamp,
	in itemcode			varchar(1000),--ILD_LayItemThanhPham
	in warehouse		nvarchar(20),
	in batch			varchar(50), --ILD_LayBatch_BCQAQC
	in DocNumPO			nvarchar(1000), -- Get_DocNumPO
	in OutSoucing		varchar(20),
	out tab_result table (
		ID				int,
		GrpCode			int,
		GrpName			nvarchar(100),
		ItemCode		nvarchar(50),
		ItemName		nvarchar(100),
		ItemUoM			nvarchar(50),
		WOID			int,
		WONo			nvarchar(50),
		WOStatus		nvarchar(20),
		WOType			nvarchar(20),
		DocType			nvarchar(20),
		DocNote			nvarchar(50),
		DocID			int,
		DocNum			int,
		DocDate			timestamp,
		DocTime 		nvarchar(15),
		TotalQty		numeric(21,6),
		Qty				numeric(21,6),
		UnitPrice		numeric(21,6),
		Amount			numeric(21,6),
		DisNo			nvarchar(50),
		DisID			int,
		Note			nvarchar(100),
		SSCC			varchar(50),
		Batch			varchar(50),
		UDCode			varchar(50),
		SSCC_Status		varchar(50),
		IsSSCC_Father 	char(1),
		ORIBatch		varchar(50), -- SSCC cha
		OutSourcingID   varchar(50),
		MnfDate			date,
		ExpDate			date,
		AdmissionDate	date,
		Labour			numeric(21,10),
		Machine			numeric(21,6),
		Materials		numeric(21,6),
		LineTotal		numeric(21,6),
		isGoodsReceipt	char(1),
		isGoodsIssued	char(1),
		OutSourcing_Type nvarchar(20), -- Tach:1, Dow_Up:2
		Father1		varchar(100),
		AdmissionDate_Father1	date,
		Dis_Status		varchar(10),
		TotalAmount		numeric(21,6),
		
		StartDate		date,
		DueDate			date,
		U_Group nvarchar (20),
		Doc_Exception char(1)
	)
)
as
begin
	declare  tblITEM table (
		ItemCode		nvarchar(50),
		ItemName		nvarchar(100),
		UoM				nvarchar(50),
		InventoryAcct	nvarchar(20),
		GLMethod		char(1),
		GrpID			int,
		GrpName			nvarchar(100),
		U_Group nvarchar(20)
	);
	declare WOID_SSCC table
	(
		WOID 	int,
		WONo			nvarchar(50),
		WOStatus		nvarchar(20),
		WOType			nvarchar(20),
		SSCC			varchar(50),
		Batch			varchar(50)
	);
	declare tblWH table (
		WHCode			nvarchar(20),
		WHName			nvarchar(100),
		WHAccount		nvarchar(20)
	);
	declare OutSourcing table
	(
		ReceiptID int,
		IssuedID int,
		OutSourcingID varchar(50),
		OutSourcing_Type char(1) -- Tach:1, Dow_Up:2
								 -- 1 Downgrade,2 Upgrade,3 Merge (gộp),4 Split (tách),5 Convert
	);
	declare Diff_tbl_JE_OINM table  
	(
		DocNum varchar(20),
		ItemCode varchar(50),
		tbl_LineTotal decimal(19,6),
		
		DiffValue decimal(19,6)
	);
	declare total6229,total6279 decimal(28,10);
	declare pro11 table
	(
		STDAMOUNT  DECIMAL(21,6),
		WONO VARCHAR(50), 
		DOCENTRY int ,
		FGCODE  VARCHAR(50),
		ITMSGRPCOD int,
		DISID int,
		U_Group nvarchar(20),
		DOCNUM NVARCHAR(20)
		
	);
	declare tbl table
	(
		ID				int,
		GrpCode			int,
		GrpName			nvarchar(100),
		ItemCode		nvarchar(50),
		ItemName		nvarchar(100),
		ItemUoM			nvarchar(50),
		WOID			int,
		WONo			nvarchar(50),
		WOStatus		nvarchar(20),
		WOType			nvarchar(20),
		DocType			nvarchar(20),
		DocNote			nvarchar(50),
		DocID			int,
		DocNum			int,
		DocDate			timestamp,
		DocTime 		nvarchar(15),
		TotalQty		numeric(21,6),
		Qty				numeric(21,6),
		UnitPrice		numeric(21,6),
		Amount			numeric(21,6),
		DisNo			nvarchar(50),
		DisID			int,
		Note			nvarchar(100),
		SSCC			varchar(50),
		Batch			varchar(50),
		UDCode			varchar(50),
		SSCC_Status		varchar(50),
		IsSSCC_Father 	char(1),
		ORIBatch		varchar(50), -- SSCC cha
		OutSourcingID   varchar(50),
		MnfDate			date,
		ExpDate			date,
		AdmissionDate	date,
		Labour			numeric(21,10),
		Machine			numeric(21,6),
		Materials		numeric(21,6),
		LineTotal		numeric(21,6),
		isGoodsReceipt	char(1),
		isGoodsIssued	char(1),
		OutSourcing_Type nvarchar(20), -- Tach:1, Dow_Up:2
		Father1		varchar(100),
		AdmissionDate_Father1	date,
		Dis_Status		varchar(10),
		TotalAmount		numeric(21,6),
		
		StartDate		date,
		DueDate			date,
		U_Group nvarchar (20),
		Doc_Exception char(1)
	);
		declare TruyXuatSSCC table
	(
		SSCC_OngNoi varchar(150),
		Admission_Date_OngNoi date,
		
		SSCC_LV0 varchar(150),
		Admission_Date_LV0 date,
		
		SSCC_LV1 varchar(150),
		Admission_Date_LV1 date,
		
		SSCC_LV2 varchar(150),
		Admission_Date_LV2 date,
		
		SSCC_LV3 varchar(150),
		Admission_Date_LV3 date,
		
		SSCC_LV4 varchar(150),
		Admission_Date_LV4 date,
		
		SSCC_LV5 varchar(150),
		Admission_Date_LV5 date,
		
		SSCC_LV6 varchar(150),
		Admission_Date_LV6 date
);
declare JE_OINM table
(
	"DocNum"  int,
	"ItemCode" nvarchar(50),
	"LineTotal" numeric(21,6),
	"ApplObj" int
);
-- Luu thong tin cac phieu GR,GI khong lien quan den lenh san xuat
declare tblExceptionDocNum table
(
	"DocNum" nvarchar(20),
	"DocType" nvarchar(2),
	"Include_In_Report" nvarchar(20)
	
);
	--Truy xuat SSCC
	call ILD_TruyXuatSSCCNhieuCap (tab_result_truyxuat); 
	---create local temporary table :TruyXuatSSCC as (select * from :tab_result_truyxuat);
	--pro11
/* tam khoa
	CALL ILD_R_IRS_StandardCost(fdate,tdate, datapro11);
	create local temporary table:pro11 as (select sum(STDAMOUNT) as STDAMOUNT,WONO,DOCENTRY,FGCODE,ITMSGRPCOD ,DISID 
											from :datapro11 
											group by WONO,DISID,DOCENTRY,FGCODE, ITMSGRPCOD  );
*/ -- ket thuc tam khoa
	--:tbl



	insert into :TruyXuatSSCC
	select * from :tab_result_truyxuat;
	-- CHI TIẾT NHẬP KHO KẾT QUẢ SẢN XUẤT----------------------------------------------------------------------------------------------
	insert into :tblWH (WHCode,WHName,WHAccount) select "WhsCode","WhsName","BalInvntAc"
	 from "OWHS" where  (warehouse='' or "WhsCode"=warehouse);
	------------------------------------------------------------------------------------------------------------------------

	insert into :tblITEM (ItemCode,ItemName,UoM,GrpID,GrpName,GLMethod,InventoryAcct, U_Group  ) 
	select a."ItemCode",a."ItemName",a."InvntryUom",b."ItmsGrpCod",b."ItmsGrpNam",a."GLMethod",
			case when a."GLMethod"='C' then b."BalInvntAc" end,
			b."U_Group"
	from "OITM" a, "OITB" b 
	where a."ItmsGrpCod"=b."ItmsGrpCod" and a."ItemType"='I'
		and (b."ItmsGrpCod" in (120,121,127, 253) or b."U_Group" = 'TP');-- 253 la gia cong
	-- REPORT ----------------------------------------------------------------------------------------------------------------
	insert into :tbl (ID, GrpCode,GrpName,ItemCode,ItemName,ItemUoM,WOID,WONo,
						WOStatus,WOType,DocType,DocID,DocDate,Qty,LineTotal,Note,DocTime
							,SSCC, Batch, SSCC_Status, U_Group )
						
	select   ROW_NUMBER() OVER(PARTITION BY a."CreatedBy",receipt."BatchNum" ) ,c.GrpID,c.GrpName,c.ItemCode,c.ItemName,c.UoM,d."DocEntry" ,d."DocNum",
						d."Status",d."Type",a."TransType",a."CreatedBy",a."DocDate",
						--a."InQty"-a."OutQty",
						receipt."Quantity",
						 a."TransValue",
 
						a."Comments",
						
						case when LENGTH(a."DocTime")  = 1 then  '00:0'|| a."DocTime"
							 when LENGTH(a."DocTime")  = 2 then  '00:'|| a."DocTime"				
							 when LENGTH(a."DocTime")  = 3 then  '0'||left(a."DocTime",1) || ':' || right(a."DocTime",2) 
		 					 when LENGTH(a."DocTime")  = 4 then   left(a."DocTime",2) || ':' || right(a."DocTime",2)  
	 			else  '' end  as "DocTime",
	 			-- SSCC
				receipt."BatchNum" as "SSCC",
				-- Batch
				f."MnfSerial" as "Batch",
				f."LotNumber" as "SSCC_Status",
				--
				c.U_Group 
						
	from "OINM" a  
	inner join  :tblWH b on a."Warehouse"=b.WHCode
	inner join :tblITEM c on a."ItemCode"=c.ItemCode
	inner join "OWOR" d on a."AppObjAbs"=d."DocEntry"
	left join (select   "BatchNum","BaseType", "BaseEntry", "ItemCode","Quantity" from IBT1 where "BaseType" = 59) receipt 
				on receipt."BaseEntry" =  a."CreatedBy" and receipt."BaseType" = 59 and receipt."ItemCode" = c.ItemCode
	left join OBTN f on receipt."BatchNum" = f."DistNumber"
	
	where a."TransType"  =  59 
			--and a."AppObjType"='P' 
			and a."ApplObj"='202' and a."DocDate" between fdate and tdate
			
	and (ifnull(:DocNumPO, '') = '' or cast(d."DocNum" as nvarchar) in (select "DATA" from split(:DocNumPO,',')));
---================================Lấy những phiếu ngoại lệ (goog receipt, good issued)
insert into  :tblExceptionDocNum ("DocNum", "DocType","Include_In_Report")
select  "DocNum", '59', "U_Include_In_Report"
from OIGN
where "DocDate" between fdate and tdate and "CANCELED"  = 'N' and ifnull("U_Include_In_Report",'')  like  '%04%';
---
insert into  :tblExceptionDocNum ("DocNum", "DocType","Include_In_Report")
select  "DocNum", '60', "U_Include_In_Report"
from OIGE
where "DocDate" between fdate and tdate and "CANCELED"  = 'N' and ifnull("U_Include_In_Report",'')  like  '%04%';

if not exists (select 1 from  :tblExceptionDocNum) then
	insert into :tblExceptionDocNum  ("DocNum")
	select 1 from dummy;
end if;

---------------------------------------------------------------------------------------------------------------------
-- insert PHIEU RECEIPT thanh pham (goods receipt doc lap)
insert into :tbl (ID, GrpCode,GrpName,ItemCode,ItemName,ItemUoM,
						DocType,DocID,DocDate,Qty,LineTotal,Note,DocTime
							,SSCC, Batch, SSCC_Status, isGoodsReceipt, TotalQty, U_Group)
							
							
select   ROW_NUMBER() OVER(PARTITION BY a."CreatedBy",receipt."BatchNum" ) ,c.GrpID,c.GrpName,
							c.ItemCode,c.ItemName,c.UoM, 

						  a."TransType",a."CreatedBy",a."DocDate",
						receipt."Quantity",
						 a."TransValue",
 
						a."Comments",
						
						case when LENGTH(a."DocTime")  = 1 then  '00:0'|| a."DocTime"
							 when LENGTH(a."DocTime")  = 2 then  '00:'|| a."DocTime"				
							 when LENGTH(a."DocTime")  = 3 then  '0'||left(a."DocTime",1) || ':' || right(a."DocTime",2) 
		 					 when LENGTH(a."DocTime")  = 4 then   left(a."DocTime",2) || ':' || right(a."DocTime",2)  
	 			else  '' end  as "DocTime",
	 			-- SSCC
				receipt."BatchNum" as "SSCC",
				-- Batch
				f."MnfSerial" as "Batch",
				f."LotNumber" as "SSCC_Status",
				'Y' as "isGoodsReceipt",
				a."InQty" as  "TotalQty",
				--
				c.U_Group
						
						
	from "OINM" a  
	inner join  :tblWH b on a."Warehouse"=b.WHCode
	inner join :tblITEM c on a."ItemCode"=c.ItemCode
	left join (select   "BatchNum","BaseType", "BaseEntry", "ItemCode","Quantity",  "BaseLinNum" from IBT1 where "BaseType" = 59) receipt 
				on receipt."BaseEntry" =  a."CreatedBy" and receipt."BaseType" = 59 and receipt."ItemCode" = c.ItemCode
					and    a."DocLineNum"  = receipt."BaseLinNum"
	left join OBTN f on receipt."BatchNum" = f."DistNumber"
	
	where a."TransType"  =  59 and a."DocDate" between fdate and tdate and a."ApplObj" <>'202' 
		  and   (a."BASE_REF"   in ('1704230005' ,'332416371','332416372')  
		  				 or  a."BASE_REF"   in (select "DocNum" from :tblExceptionDocNum  
		  				 										where "DocType" = '59' )
		  				 						)
			and a."CreatedBy" not in (select   "U_receiptID"   
										from  "@TBL_OUTS_DOCNUM"
										where "U_docDate" between fdate and tdate ) ;
										
										
										
-- insert PHIEU ISSUED thanh pham (goods Issued doc lap)
insert into :tbl (ID, GrpCode,GrpName,ItemCode,ItemName,ItemUoM,
						DocType,DocID,DocDate,Qty,LineTotal,Note,DocTime
							,SSCC, Batch, SSCC_Status, isGoodsIssued, TotalQty, U_Group)
							
							
select   ROW_NUMBER() OVER(PARTITION BY a."CreatedBy",issued."BatchNum" ) ,c.GrpID,c.GrpName,
		 c.ItemCode,c.ItemName,c.UoM, 

		 a."TransType",a."CreatedBy",a."DocDate",
		  -issued."Quantity",a."TransValue",
 
		  a."Comments",
						
						case when LENGTH(a."DocTime")  = 1 then  '00:0'|| a."DocTime"
							 when LENGTH(a."DocTime")  = 2 then  '00:'|| a."DocTime"				
							 when LENGTH(a."DocTime")  = 3 then  '0'||left(a."DocTime",1) || ':' || right(a."DocTime",2) 
		 					 when LENGTH(a."DocTime")  = 4 then   left(a."DocTime",2) || ':' || right(a."DocTime",2)  
	 			else  '' end  as "DocTime",
	 			-- SSCC
				issued."BatchNum" as "SSCC",
				-- Batch
				f."MnfSerial" as "Batch",
				f."LotNumber" as "SSCC_Status",
				'Y' as "isGoodsIssued",
				a."OutQty" as "TotalQty",
				--
				c.U_Group
						
						
	from "OINM" a  
	inner join  :tblWH b on a."Warehouse"=b.WHCode
	inner join :tblITEM c on a."ItemCode"=c.ItemCode
	left join (select   "BatchNum","BaseType", "BaseEntry", "ItemCode","Quantity",  "BaseLinNum" from IBT1 where "BaseType" = 60) issued 
				on issued."BaseEntry" =  a."CreatedBy" and issued."BaseType" = 60 and issued."ItemCode" = c.ItemCode 
											and  a."DocLineNum"  = issued."BaseLinNum"
	left join OBTN f on issued."BatchNum" = f."DistNumber"
	
	where a."TransType"  =  60 and a."DocDate" between fdate and tdate and a."ApplObj" <>'202' 
		    and (a."BASE_REF"   in( '1603230016','322418143','322418144') 
		    		or  a."BASE_REF"   in (select "DocNum" from :tblExceptionDocNum  
							where "DocType" = '60'))
							
			and a."CreatedBy" not in (select   "U_issueID"   
										from  "@TBL_OUTS_DOCNUM"
										where "U_docDate" between fdate and tdate ) ;

 
 
--
update a
set a.WOID = b.WOID,
	a.WONo = b.WONo,
	a.WOStatus =b.WOStatus ,
	a.WOType =  b.WOType
from :tbl a
inner join  :tbl b on a.SSCC = b.SSCC
where ifnull(a.WONo,'')  = '' and a.DocType =  59;
			
--
delete from :tbl where ifnull(ID,0) <> 1;-- Xoa di nhung SSCC receipt trung;

--
	
	-- Insert Lệnh sản xuất ID và SSCC từ table temp :tbl
	insert into :WOID_SSCC (WOID, WONo, WOStatus, WOType, Batch,SSCC)
	select    max(WOID),max(WONo), max(WOStatus), max(WOType),max(Batch),SSCC
	from :tbl
	group by SSCC;
	----------------
	
	--- Insert thông tin lệnh rã vào lệnh gốc

	insert into :tbl(GrpCode,GrpName,DisID, DisNo,ItemCode,ItemName,ItemUoM,
						DocType,DocID,DocDate,Qty,LineTotal, DocTime, SSCC,U_Group)
						
	select    distinct g."ItmsGrpCod",g."ItmsGrpNam",  c."DocEntry", c."DocNum", e."ItemCode",f."ItemName",f."InvntryUom",
						e."TransType",e."CreatedBy",e."DocDate",
						 ---e."OutQty",
						-issued."Quantity",
						e."TransValue", 
						
						case when LENGTH(e."DocTime")  = 1 then  '00:0'|| e."DocTime"
							 when LENGTH(e."DocTime")  = 2 then  '00:'|| e."DocTime"
							 when LENGTH(e."DocTime")  = 3 then  '0'||left(e."DocTime",1) || ':' || right(e."DocTime",2) 
		 					 when LENGTH(e."DocTime")  = 4 then   left(e."DocTime",2) || ':' || right(e."DocTime",2)  
	 			else  '' end  as "DocTime",
	 			--
			issued."BatchNum",
			--
			g."U_Group"
	 		
	 from OWOR c
	 left join  OINM e on  e."AppObjAbs" = c."DocEntry"		
	 inner join OITM f on f."ItemCode"  = e."ItemCode"	
	 inner join OITB g on f."ItmsGrpCod"= g."ItmsGrpCod"	      
					     		      
	left join (select "BatchNum","BaseType", "BaseEntry", "ItemCode", "Quantity" from IBT1 where "BaseType" =  60) issued 
	on issued."BaseEntry" =  e."CreatedBy" and issued."ItemCode" = e."ItemCode"
	where  e."DocDate" between fdate and tdate 
			and   e."ApplObj"  = '202'   
			and c."Type"  = 'D' 
			and (f."ItmsGrpCod" in (120,121,127, 253) or g."U_Group" = 'TP');		      
					      
					      
-- update thông tin lệnh gốc  --> lệnh rã dựa vào sscc
update a
set WOID  = (select   WOID  from :WOID_SSCC b 	where ifnull(a.SSCC,'') = ifnull(b.SSCC,'')),
	WOStatus  = (select   WOStatus from :WOID_SSCC b where ifnull(a.SSCC,'') = ifnull(b.SSCC,'')),
	WOType  = (select   WOType from :WOID_SSCC b where ifnull(a.SSCC,'') = ifnull(b.SSCC,'')),
	Batch  = (select   Batch 	from :WOID_SSCC b where ifnull(a.SSCC,'') = ifnull(b.SSCC,'')),
	WONo  = (select   WONo 	from :WOID_SSCC b where ifnull(a.SSCC,'') = ifnull(b.SSCC,''))
from :tbl a
where ifnull(WONO,'')  = '';
--

insert into :OutSourcing (ReceiptID,IssuedID,OutSourcingID, OutSourcing_Type)
select "U_receiptID", "U_issueID", "Code", "U_Type"
from  "@TBL_OUTS_DOCNUM"
where "U_docDate" between fdate and tdate;
-----
/* old
update a
set a.OutSourcing_Type = case when b."ItemCode"  =  c."ItemCode" then 1 else 2 end 
from :OutSourcing a
inner join IGE1 b on a.IssuedID  = b."DocEntry"
inner join IGN1  c on a.ReceiptID  = c."DocEntry";
*/
 
 IF exists (select ReceiptID from :OutSourcing) then
 
	 --- Insert phieu Receipt - OutSourcing
	INSERT into :tbl(GrpCode,GrpName,ItemCode,ItemName,ItemUoM,
							DocType,DocID,DocDate,Qty,LineTotal, DocTime, SSCC, IsSSCC_Father,
							  Batch, SSCC_Status, 
							  ORIBatch)
							
	SELECT    distinct g."ItmsGrpCod",g."ItmsGrpNam",  e."ItemCode",f."ItemName",f."InvntryUom",
							e."TransType",e."CreatedBy",e."DocDate",h."Quantity",e."TransValue", 
							
					 case 
							when LENGTH(e."DocTime")  = 1 then  '00:0'|| e."DocTime"
							when LENGTH(e."DocTime")  = 2 then  '00:'|| e."DocTime"
							when LENGTH(e."DocTime")  = 3 then  '0'||left(e."DocTime",1) || ':' || right(e."DocTime",2) 
			 				when LENGTH(e."DocTime")  = 4 then   left(e."DocTime",2) || ':' || right(e."DocTime",2)  
		 			else  '' end  as "DocTime",
		 			--
				h."BatchNum",
				'N' as "IsSSCC_Father",
				"MnfSerial" as "Batch",
				"LotNumber" as "SSCC_Status",
				"U_ORIBatch" -- sscc cha
				
		 		
	FROM  OINM e 	
		 inner join OITM f on f."ItemCode"  = e."ItemCode"	
		 inner join OITB g on f."ItmsGrpCod"= g."ItmsGrpCod"	      		     		      
		 left join IBT1 h on h."ItemCode" = e."ItemCode" and h."BaseEntry" =  e."CreatedBy" and "BsDocType"   =  -1
		 left join OBTN i on h."BatchNum" = i."DistNumber"
		 		
	WHERE  (f."ItmsGrpCod" in (120,121,127, 253) or g."U_Group" = 'TP') and e."TransType" = 59  and "CreatedBy" IN (select ReceiptID from :OutSourcing)  and "BaseType" =59;	
End IF; 
-----
IF exists (select IssuedID from :OutSourcing) then

	 --- Insert phieu issue - OutSourcing: Cha
	INSERT into :tbl(GrpCode,GrpName,ItemCode,ItemName,ItemUoM,
							DocType,DocID,DocDate,Qty,LineTotal, DocTime, SSCC, IsSSCC_Father,
							Batch, SSCC_Status,
							ORIBatch)
							
	SELECT    distinct g."ItmsGrpCod",g."ItmsGrpNam",  e."ItemCode",f."ItemName",f."InvntryUom",
							e."TransType",e."CreatedBy",e."DocDate",
							-h."Quantity",
							e."TransValue", 
							
					case when LENGTH(e."DocTime")  = 3 then  '0'||left(e."DocTime",1) || ':' || right(e."DocTime",2) 
			 		when LENGTH(e."DocTime")  = 4 then   left(e."DocTime",2) || ':' || right(e."DocTime",2)  
		 			else  '' end  as "DocTime",
		 			--
				h."BatchNum",
				'Y' as "SSCC_Cha",
				i."MnfSerial" as "Batch",
				i."LotNumber" as "SSCC_Status",
				h."BatchNum"
		 		
	FROM  OINM e 	
		 inner join OITM f on f."ItemCode"  = e."ItemCode"	
		 inner join OITB g on f."ItmsGrpCod"= g."ItmsGrpCod"	      		     		      
		 left join IBT1 h on h."ItemCode" = e."ItemCode" and h."BaseEntry" =  e."CreatedBy" and "BsDocType"   =  -1
		 left join OBTN i on h."BatchNum" = i."DistNumber"			
	WHERE (f."ItmsGrpCod" in (120,121,127, 253) or g."U_Group" = 'TP' ) and  e."TransType" = 60  and "CreatedBy" IN (select IssuedID from :OutSourcing)  and "BaseType" =60;	
End IF;
--- Lâý thông tin Return component
Return_Component = select a."DocNum", a."DocEntry", b."BaseEntry" as "WOID", b."BaseRef" as "WONO", b."ItemCode"
					from OIGN a 
					inner join IGN1 b on a."DocEntry"  =  b."DocEntry"
					inner join OWOR e on e."DocEntry" = b."BaseEntry"
					inner join WOR1 f on e."DocEntry"  = f."DocEntry" and b."ItemCode" =  f."ItemCode" and b."BaseLine" = f."LineNum"
					where b."BaseType" = 202 
					AND  f."PlannedQty"  > 0 --PlannedQty = - (âm) là rework 
					and a."DocDate" between fdate and tdate;
--- Xoa du lieu phieu Return component			
delete 
from :tbl  
where DocType =  59 
		and  WOID||DocID||ItemCode in  (select "WOID"||"DocEntry"||"ItemCode" from :Return_Component);

 
--
Update a
set OutSourcingID = b.OutSourcingID 
from :tbl a
--old left  join  :OutSourcing b on a.DocID =  b.IssuedID or a.DocID =  b.ReceiptID 
left  join  :OutSourcing b on a.DocID =  b.IssuedID
where ifnull(IsSSCC_Father,'') = 'Y';
---  update  OutSourcingID cho cac SSCC Con
Update a
set OutSourcingID = b.OutSourcingID 
from :tbl a
inner join :tbl b on a.ORIBATCH = b.SSCC and ifnull(b.IsSSCC_Father,'') = 'Y'
where ifnull(a.IsSSCC_Father,'') = 'N';


-- Update thông tin lệnh sản xuất cho các phiếu Issued (Cha) outsourcing
update a
set WOID  =  b.WOID, 
	WOStatus  = b.WOStatus, 
	WOType  = b.WOType, 
	WONo  =  b.WONo
from :tbl a
inner join (select   distinct WOID,WOStatus,WOType,WONo, SSCC from :tbl b where DocType =  59 and  WONo <> ''  ) b 
				on ifnull(a.SSCC,'') = ifnull(b.SSCC,'') 
where ifnull(a.WONo,'')  = '' ;
---
Update a
set WOID = d."BsDocEntry",
	WOStatus  = e."LotNumber",
	WOType  = f."Type", 
	WONo  =  f."DocNum"
from :tbl a
inner join (select distinct b.* , c.Batch
					from IBT1 b
					inner join :tbl c on b."BatchNum" = c.SSCC
					where "BsDocType" = 202  and  ifnull(c.IsSSCC_Father,'') = 'Y' and ifnull(c.WONo,'') = '') d on d.Batch= a.Batch
inner join OBTN e on e."MnfSerial" =  a.Batch
inner join OWOR f on f."DocEntry" = d."BsDocEntry"

where ifnull(a.IsSSCC_Father,'') = 'Y' and ifnull(a.WONo,'') = '' ;

-- Update thông tin lệnh sản xuất cho các phiếu Receipt (con) outsourcing
update a
set WOID  =  b.WOID, 
	WOStatus  = b.WOStatus, 
	WOType  = b.WOType, 
	WONo  =  b.WONo
from :tbl a
inner join (select   distinct WOID,WOStatus,WOType,WONo, SSCC from :tbl b  where  ifnull(WONo,'') <> '' ) b on ifnull(a.ORIBatch,'') = ifnull(b.SSCC,'')
where ifnull(a.ORIBatch,'') <> '' and   ifnull(a.WONo,'')  = ''  ; 
---
update a
set WOID  =  b.WOID, 
	WOStatus  = b.WOStatus, 
	WOType  = b.WOType, 
	WONo  =  b.WONo
from :tbl a
inner join (select   distinct WOID,WOStatus,WOType,WONo, SSCC from :tbl b where DocType =  59 and  WONo <> ''  ) b 
				on ifnull(a.SSCC,'') = ifnull(b.SSCC,'') 
where ifnull(a.WONo,'')  = ''    and  ifnull(OutSourcingID,'') <> '' ;
---

-- Update thông tin lệnh sản xuất cho các phiếu Receipt (con) outsourcing
update a
set WOID  =  b.WOID, 
	WOStatus  = b.WOStatus, 
	WOType  = b.WOType, 
	WONo  =  b.WONo
from :tbl a
inner join (select   distinct WOID,WOStatus,WOType,WONo, SSCC from :tbl b  where  ifnull(WONo,'') <> '' ) b on ifnull(a.ORIBatch,'') = ifnull(b.SSCC,'')
where ifnull(a.ORIBatch,'') <> '' and   ifnull(a.WONo,'')  = ''  ; 

--
--- Update thong tin lenh san xuat cho sscc Outsourcing
--1. SSCC Cha Outsourcing 
update a
set a.WONO =  b.WONO,
	a.WOID =  b.WOID
from  :tbl a
inner join (select distinct WONO, WOID, SSCC from :tbl b where  ifnull(b.WONO,'') <> '') b on a.SSCC = b.SSCC
where   ifnull(a.ISSSCC_FATHER,'')  = 'Y' and  ifnull(a.WONO,'') = '';

---
--2. SSCC Con Outsourcing 
update a
set a.WONO =  b.WONO,
	a.WOID =  b.WOID
from  :tbl a
inner join (select distinct WONO, WOID, SSCC from :tbl b where  ifnull(b.WONO,'') <> '' and  ifnull(b.ISSSCC_FATHER,'')  = 'Y') b on a.ORIBATCH = b.SSCC
where   ifnull(a.ISSSCC_FATHER,'')  = 'N' and  ifnull(a.WONO,'') = '';
--
update a
set a.OutSourcing_Type = b.OutSourcing_Type
from :tbl a
inner join :OutSourcing b on a.OutSourcingID = b.OutSourcingID
where  ifnull(a.ISSSCC_FATHER,'') <> '';
---
update a
set a.WOID = b.WOID,
	a.WONo = b.WONo,
	a.WOStatus =b.WOStatus ,
	a.WOType =  b.WOType
from :tbl a
inner join  :tbl b on a.SSCC = b.SSCC and  ifnull(b.WONo,'') <> ''
where ifnull(a.WONo,'')  = '' and (ifnull(a.isGoodsReceipt,'')  ='Y' or  ifnull(a.isGoodsIssued,'')  ='Y');

--

-- update DocNum
-- Old:28:08 update x set DocNum = (select "DocNum" from APZ_ODOC where "DocEntry" = x.DocID and "ObjType" = x.DocType)  from :tbl x;  
UPDATE x
SET x.DocNum = y."DocNum"
FROM :tbl x
INNER JOIN  APZ_ODOC y ON  y."DocEntry" = x.DocID and  y."ObjType" = x.DocType;

--- TRUONG HOP WOID  = NULL MA DOCNUM VA DOCID CO GIA TRI

/* tam khoa
update a
set a.WOID  = (select max(b.WOID) from :tbl b where a.DocNum = b.DocNum and a.DocType = b.DocType),
 	a.WONO  = (select max(b.WONO) from :tbl b where a.DocNum = b.DocNum and a.DocType = b.DocType)
from :tbl a
where ifnull(a.WONO,'') = '' or ifnull(a.WOID,0) = 0;
---
*/


update a
set a.WONO  = left(SSCC,9)
from :tbl a;
---
update a
set a.WOID =b."DocEntry"
from :tbl a 
inner join OWOR b on a.WONO  = cast(b."DocNum" as nvarchar);


--
delete from :tbl where  ifnull(WOID,0)  = 0;
--
update a
set TotalQty  = (select sum(Qty) from :tbl b  where  a.DocID = b.DocID  and a.WOID = b.WOID  group by DocID )
from :tbl a 
where  a.DocNum not in ( '1603230016','322418143','322418144','1704230005' ,'332416371','332416372')
		and   a.DocNum not in (select "DocNum" from :tblExceptionDocNum  );
---
update a 
set TotalQty  =  Qty
from :tbl a
where ifnull(TotalQty,0) = 0;
--------------


--
/*Xử lý chênh lệch từng phiếu thành phẩm: 
	Nguyên nhân: do cùng 1 phiếu receipt from production , dòng itemcode tách ra nhiều dòng 
--> Transvalue của phiếu ở OINM mà join với dữ liệu SSCC sẽ không đúng tổng phiếu */
insert into :JE_OINM("DocNum" ,"ItemCode","LineTotal","ApplObj")
select "BASE_REF" as "DocNum", "ItemCode",  sum("TransValue") as "LineTotal", "ApplObj"
			from OINM  
			where "TransType"  =  59 
			--and "ApplObj"='202' and "DocDate" between fdate and tdate
			and "DocDate" between fdate and tdate
			group by "BASE_REF", "ItemCode", "ApplObj";

update a
set TotalAmount = b."LineTotal"
from :tbl a
inner join :JE_OINM b on a.DocNum =  b."DocNum" and a.ItemCode  =  b."ItemCode"
where "ApplObj" = 202;

---  Good Receipt
update a
set TotalAmount =  LineTotal 
from :tbl a
where DocNum  in ( '332416371', '332416372')  and  DocNum in (select "DocNum" from :tblExceptionDocNum where "DocType" = '59')

or  ifnull(TotalAmount,0)  =  0;
---
delete from  :tbl where   DocNum  in ( '332416371', '332416372','322418144','322418143')  and ifnull(:OutSoucing, '') = 'N' ;


delete from :tbl where ifnull(:OutSoucing, '') = 'N' and   DocNum in (select "DocNum" from :tblExceptionDocNum );

---

-- Truong hop Outsourcing : "ApplObj" = -1 
update a
set TotalAmount = b."LineTotal"
from :tbl a
inner join :JE_OINM b on a.DocNum =  b."DocNum" and a.ItemCode  =  b."ItemCode"
where "ApplObj" = -1 and ifnull(totalamount,0) = 0;
			
---Diff_tbl_JE_OINM
insert into :Diff_tbl_JE_OINM (DocNum, ItemCode, tbl_LineTotal)
select DocNum, ItemCode, sum(LineTotal) 
from :tbl 
where DocType  ='59'
group by  DocNum, ItemCode;

update  a
set  DiffValue  =  ifnull(b."LineTotal",0) - ifnull( a.tbl_LineTotal,0)
from :Diff_tbl_JE_OINM a
inner join :JE_OINM b on a.DocNum  = b."DocNum" and a.ItemCode =  b."ItemCode";

 

update a
set a.LineTotal  =   a.LineTotal + (select  sum(b.DiffValue) 
									from  :Diff_tbl_JE_OINM b where a.DocNum  =  b.DocNum  and a.ItemCode  =  b.ItemCode )
--b.DiffValue
from :tbl a
where  a.SSCC||a.DocNum  =  (select max(c.SSCC)||max(c.DocNum) from  :tbl c where  c.DocType  =  '59' 
								and a.DocNum = c.DocNum and a.ItemCode = c.ItemCode )
								
and a.DocType = '59';
---
update a
set Amount = (ifnull(a.TotalAmount,0)/a.TOTALQTY) * a.Qty
--(ifnull(LineTotal,0)* a.Qty ) / TOTALQTY
from  :tbl a
where ifnull(TOTALQTY,0) <> 0 and a.Qty <>0  and a.DocType = '59';
--
update a
set Amount =  (ifnull(LineTotal,0)* a.Qty ) / TOTALQTY
from  :tbl a
where ifnull(TOTALQTY,0) <> 0 and a.DocType <> '59';

--
update :tbl
set UnitPrice = ifnull(Amount,0)/ Qty
where ifnull(Qty,0) <>0;
---


update :tbl
set ORIBatch = ''
where ORIBatch  = SSCC; 
	update :tbl set DocNote = 'Receipt from Production' where DocType='59'  and ifnull (isGoodsReceipt,'') ='' and ifnull(ISSSCC_FATHER,'') = '' ;
	update :tbl set DocNote = 'Receipt from Outsourcing' where DocType='59'  and ifnull (isGoodsReceipt,'') ='Y' ;
	update :tbl set DocNote = 'Receipt from Outsourcing' where DocType='59' and ifnull(ISSSCC_FATHER,'') <> '' ;
	
	
	update :tbl set DocNote = 'Issue for Production' where DocType='60' and ifnull(ISSSCC_FATHER,'') = '' and ifnull (isGoodsIssued,'') ='';
	update :tbl set DocNote = 'Issue for Outsourcing' where DocType='60'  and ifnull (isGoodsIssued,'') ='Y' ;
	update :tbl set DocNote = 'Issue for Outsourcing' where DocType='60' and ifnull(ISSSCC_FATHER,'') <> '';
	
	
	update :tbl set WOType='Standard' where WOType='S';
	update :tbl set WOType='Disassembly' where WOType='D';
	update :tbl set WOType='Special' where WOType='P';
	update :tbl set WOStatus='Released' where WOStatus='R';
	update :tbl set WOStatus='Closed' where WOStatus='L';	
	
	update :tbl 
	set DocNote =   'Issue for Disassembly' 
	where DocType='60' and  ifnull(DisNo,'') <> '' ;
	
---
Update a
set UDCode  =  (select "U_UDCode" from  "@QC_RESULT_H" b   where  a.Batch  = b."U_Batch"
				and b."DocEntry" = (select max("DocEntry") from "@QC_RESULT_H" c where  a.Batch  = c."U_Batch" ))
from :tbl a	;

--- update thong tin 
update  a
set MnfDate=  b."MnfDate",
	ExpDate = b."ExpDate"
from :tbl A
inner join  OBTN b on a.SSCC  =  b."DistNumber";

-- update DocNum
--- old:28:08:2024  update x set DocNum = (select "DocNum" from APZ_ODOC where "DocEntry" = x.DocID and "ObjType" = x.DocType)  from :tbl x;  
update x set DocNum = y."DocNum"
from :tbl x
inner join  APZ_ODOC  y on "DocEntry" = x.DocID and y."ObjType" = x.DocType;

------
Count_SSCC_LenhSX = select count(SSCC) as "So_SSCC",DOCID,DOCTYPE from :tbl group by DOCID,DOCTYPE;
------=========================== Machine
insert into :pro11 (STDAMOUNT,WONO, DOCENTRY, FGCODE, ITMSGRPCOD,DISID, DOCNUM )
select STDAMOUNT,WONO, DOCENTRY, FGCODE, ITMSGRPCOD,DISID, DOCNUM
from  PRO11_DATA_TEMP
where POSTINGDATE between fdate and tdate;

--- insert thêm dư liệu từ bảng vât lý COSTING_STANDARDCOST
insert into :pro11 (STDAMOUNT,WONO, DOCENTRY, FGCODE, ITMSGRPCOD,DISID, U_Group, DOCNUM )
select STDAMOUNT,WONO, DOCENTRY, FGCODE, ITMSGRPCOD,DISID , U_Group, DOCNUM
from  COSTING_STANDARDCOST 
where POSTINGDATE between fdate and tdate;

---
 
update  a
set Machine = (select  sum (round(STDAMOUNT,2))  from :pro11 b
			where  a.WONO = b.WONO and a.DOCID = b.DOCENTRY and a.DOCTYPE =  59 
				and a.ItemCode  = b.FGCODE and (b.ITMSGRPCOD in (125,233) or ifnull(b.U_Group,'')  ='MC_TTMC' ))
				 /(select  case when ifnull("So_SSCC",0)>0 then "So_SSCC" else 1 end 
				 from :Count_SSCC_LenhSX c where  c."DOCID" = a.DOCID  and c.DOCTYPE =  59 )
from :tbl a 
where  ifnull(DisNo,'')  ='';
 

 
-- lenh ra
update  a
set Machine = (select sum (round(STDAMOUNT,2)) from :pro11 b
			where  a.DISID = b.DISID and a.DOCID = b.DOCENTRY and a.DOCTYPE =  60 
				and a.ItemCode  = b.FGCODE and (b.ITMSGRPCOD in (125,233) or ifnull(b.U_Group,'')  ='MC_TTMC'))
				/(select  case when ifnull("So_SSCC",0)>0 then "So_SSCC" else 1 end 
						from :Count_SSCC_LenhSX c where c."DOCID" = a.DOCID and c.DOCTYPE =  60)
from :tbl a 
where ifnull(DisNo,'')  <>'';

------=========================== Labour

update  a
set Labour = (select  sum (round(STDAMOUNT,2))  from :pro11 b
			--(select round(sum (STDAMOUNT),2) from :pro11 b
			where  a.WONO = b.WONO and a.DOCID = b.DOCENTRY and a.DOCTYPE =  59 
				and a.ItemCode  = b.FGCODE and (b.ITMSGRPCOD = 126 or ifnull(b.U_Group,'')  ='LB_TTLB'))
				/(select  case when ifnull("So_SSCC",0)>0 then "So_SSCC" else 1 end 
				from :Count_SSCC_LenhSX c where c."DOCID" = a.DOCID and c.DOCTYPE =  59)
from :tbl a
where  ifnull(DisNo,'')  ='';
--

update  a
set Labour = (select sum (round(STDAMOUNT,2)) from :pro11 b
			--Old: where  a.WONO = b.WONO and a.DOCID = b.DOCENTRY and a.DOCTYPE =  60 
			where  a.DISID = b.DISID and a.DOCID = b.DOCENTRY and a.DOCTYPE =  60 
				and a.ItemCode  = b.FGCODE and  (b.ITMSGRPCOD = 126 or ifnull(b.U_Group,'')  ='LB_TTLB'))
			/(select  case when ifnull("So_SSCC",0)>0 then "So_SSCC" else 1 end 
			from :Count_SSCC_LenhSX c where c."DOCID" = a.DOCID and c.DOCTYPE =  60)
				
from :tbl a
where  ifnull(DisNo,'')  <>'';


------=========================== Materials
update  a
set Materials = (select sum (round(STDAMOUNT,2)) from :pro11 b
			where  a.WONO = b.WONO and a.DOCID = b.DOCENTRY and a.DOCTYPE =  59 
				and a.ItemCode  = b.FGCODE 
				and b.ITMSGRPCOD not in (125,126,233)  and ifnull(b.U_Group,'') not in ('MC_TTMC','LB_TTLB'))
				/(select  case when ifnull("So_SSCC",0)>0 then "So_SSCC" else 1 end 
					from :Count_SSCC_LenhSX c where c."DOCID" = a.DOCID and c.DOCTYPE =  59)
from :tbl a
where  ifnull(DisNo,'')  ='';
----
update  a
set Materials = (select sum (round(STDAMOUNT,2)) from :pro11 b
			where  a.DISID = b.DISID and a.DOCID = b.DOCENTRY and a.DOCTYPE =  60 
			--a.WONO = b.WONO and a.DOCID = b.DOCENTRY and a.DOCTYPE =  60 
				and a.ItemCode  = b.FGCODE 
				and b.ITMSGRPCOD not in (125,126,233) 
				and ifnull(b.U_Group,'') not in ('MC_TTMC','LB_TTLB'))
				/(select  case when ifnull("So_SSCC",0)>0 then "So_SSCC" else 1 end 
				from :Count_SSCC_LenhSX c where c."DOCID" = a.DOCID and c.DOCTYPE =  60)
from :tbl a
where  ifnull(DisNo,'')  <>'';


 


---------------- Update Labour, MACHINE, Materials cho cac SSCC Con
-- MATERIALS Con
update a
set a.MATERIALS  = b.MATERIALS/b.QTY * a.QTY
from :tbl a 
inner join :tbl b on a.ORIBATCH = b.SSCC  and b.DocType = 59
where ifnull(a.ISSSCC_FATHER,'') = 'N' and a.OutSourcing_Type = 4; -- OutSourcing_Type = 4 (tach)
-- Labour Con
update a  
set a.Labour  = b.Labour/b.QTY * a.QTY
from :tbl a 
inner join :tbl b on a.ORIBATCH = b.SSCC  and b.DocType = 59
where ifnull(a.ISSSCC_FATHER,'') = 'N' and a.OutSourcing_Type = 4; -- OutSourcing_Type = 4 (tach)
-- Machine Con
update a
set a.Machine  = b.Machine/b.QTY * a.QTY
from :tbl a 
inner join :tbl b on a.ORIBATCH = b.SSCC  and b.DocType = 59
where ifnull(a.ISSSCC_FATHER,'') = 'N' and a.OutSourcing_Type = 4; -- OutSourcing_Type = 4 (tach)
------------------- Update SSCC Cha ( bi tach 1 lan nua): 
update a
set a.MATERIALS  =b.MATERIALS,
	a.Labour  = b.Labour,
	a.Machine  = b.Machine
from :tbl a 
inner join :tbl b on a.SSCC = b.SSCC and ifnull(b.ISSSCC_FATHER,'') = 'N' and b.OutSourcing_Type   = 4
where ifnull(a.ISSSCC_FATHER,'') = 'Y' and a.OutSourcing_Type   = 4
and ifnull(a.MATERIALS,0) = 0; 



update a
set a.MATERIALS  = b.MATERIALS/b.QTY * a.QTY,
	a.Labour  = 	b.Labour/b.QTY * a.QTY,
	a.Machine  = b.Machine/b.QTY * a.QTY
from :tbl a 
inner join :tbl b on a.ORIBATCH = b.SSCC
where ifnull(a.ISSSCC_FATHER,'') = 'N' and a.OutSourcing_Type = 4
			 and (ifnull(b.MATERIALS,0)>0 or ifnull(b.Labour,0)>0 or ifnull(b.Machine,0)>0)
and  (ifnull(a.MATERIALS,0) = 0 or ifnull(a.Labour,0) = 0 or ifnull(a.Machine,0) = 0) ; -- OutSourcing_Type = 4 (tach)




---------------- Update Labour, MACHINE, Materials cho cac SSCC Cha (phieu Issued) --> Lay tu phieu Receip from production
update a
set a.MATERIALS  = b.MATERIALS,
	a.Labour  = b.Labour,
	a.Machine  =b.Machine
from :tbl a 
inner join :tbl b on a.SSCC = b.SSCC and  (ifnull(b.MATERIALS,0) >0 or ifnull(b.Labour,0) >0 or ifnull(b.Machine,0) >0 )
where ifnull(a.ISSSCC_FATHER,'') = 'Y' and  a.OutSourcing_Type = 4;

--- Update Labour, MACHINE, Materials: Truong hop downgrade/updategrade:1 Downgrade,2 Upgrade,
 -- MATERIALS Con
update a
set a.MATERIALS  = b.MATERIALS
from :tbl a 
inner join :tbl b on a.ORIBATCH = b.SSCC 
where ifnull(a.ISSSCC_FATHER,'') = 'N' and a.OutSourcing_Type  in (1, 2);  
-- Labour Con
update a  
set a.Labour  = b.Labour
from :tbl a 
inner join :tbl b on a.ORIBATCH = b.SSCC 
where ifnull(a.ISSSCC_FATHER,'') = 'N' and a.OutSourcing_Type  in (1, 2); 
-- Machine Con
update a
set a.Machine  =b.Machine 
from :tbl a 
inner join :tbl b on a.ORIBATCH = b.SSCC 
where ifnull(a.ISSSCC_FATHER,'') = 'N' and a.OutSourcing_Type  in (1, 2); 

--- Update Labour, MACHINE, Materials: Truong hop downgrade/updategrade
 -- MATERIALS Con
update a
set a.MATERIALS  = b.MATERIALS
from :tbl a 
inner join :tbl b on a.ORIBATCH = b.SSCC and ifnull(b.ISSSCC_FATHER,'') = 'N' and a.OutSourcing_Type  in (1, 2)
where ifnull(a.ISSSCC_FATHER,'') = 'N' and a.OutSourcing_Type  in (1, 2); 
-- Labour Con
update a  
set a.Labour  = b.Labour 
from :tbl a 
inner join :tbl b on a.ORIBATCH = b.SSCC and ifnull(b.ISSSCC_FATHER,'') = 'N' and a.OutSourcing_Type  in (1, 2)
where ifnull(a.ISSSCC_FATHER,'') = 'N' and a.OutSourcing_Type  in (1, 2); 
-- Machine Con
update a
set a.Machine  =b.Machine
from :tbl a 
inner join :tbl b on a.ORIBATCH = b.SSCC  and ifnull(b.ISSSCC_FATHER,'') = 'N' and a.OutSourcing_Type  in (1, 2)
where ifnull(a.ISSSCC_FATHER,'') = 'N' and a.OutSourcing_Type  in (1, 2); 

--
update a
set a.MATERIALS  = (select sum(b.MATERIALS)
					from :tbl b 
					where a.OUTSOURCINGID = b.OUTSOURCINGID 
					and   ifnull(b.ISSSCC_FATHER,'') = 'Y' and b.DOCTYPE = 60),
					
	a.Labour  = (select sum(b.Labour)
					from :tbl b 
					where a.OUTSOURCINGID = b.OUTSOURCINGID 
					and   ifnull(b.ISSSCC_FATHER,'') = 'Y' and b.DOCTYPE = 60),
	a.Machine  = (select sum(b.Machine)
					from :tbl b 
					where a.OUTSOURCINGID = b.OUTSOURCINGID 
					and   ifnull(b.ISSSCC_FATHER,'') = 'Y' and b.DOCTYPE = 60)
from :tbl a 
where ifnull(a.ISSSCC_FATHER,'') = 'N' and a.OutSourcing_Type=3;  
---------------- Update Labour, MACHINE, Materials cho cac SSCC Cha (phieu Issued) --> Lay tu phieu Receip from production
/*
update a
set a.MATERIALS  =  case when  ifnull(a.ISSSCC_FATHER,'') =  'Y' then  round(-b.MATERIALS,2) else round(b.MATERIALS,2) end ,
	a.Labour  =  case when  ifnull(a.ISSSCC_FATHER,'') =  'Y' then  round(-b.Labour,2) else round(b.Labour,2) end , 
	a.Machine  = case when  ifnull(a.ISSSCC_FATHER,'') =  'Y' then  round(-b.Machine,2) else round(b.Machine,2) end  
from :tbl a 
inner join :tbl b on a.SSCC = b.SSCC 
where ifnull(a.ISSSCC_FATHER,'') <> '';
*/
update a
set a.MATERIALS  = -b.MATERIALS,
	a.Labour  =    -b.Labour,
	a.Machine  =   -b.Machine
from :tbl a 
inner join :tbl b on a.SSCC = b.SSCC 
where ifnull(a.ISSSCC_FATHER,'') = 'Y'; 
---
update a
set a.MATERIALS  = -b.MATERIALS,
	a.Labour  =    -b.Labour,
	a.Machine  =   -b.Machine
from :tbl a 
inner join :tbl b on a.SSCC = b.SSCC and (ifnull(b.MATERIALS,0) >0 or ifnull(b.Labour,0) >0 or ifnull(b.Machine,0) >0 )
where (ifnull(a.isGoodsReceipt,'') = 'Y' or ifnull(a.isGoodsIssued,'') = 'Y'); 
--
update a
set  a.MATERIALS = -a.MATERIALS
from :tbl a 
where a.DocType = 59 and ifnull(a.MATERIALS,0)<0;


update a
set  a.Labour = -a.Labour 
from :tbl a
where DocType = 59 and ifnull(a.Labour,0)<0;


update a
set  a.Machine = -a.Machine  
from :tbl a
where DocType = 59 and ifnull(a.Machine,0)<0;

---

update a
set UNITPRICE = UNITPRICE *  -1,
	AMOUNT = AMOUNT *  -1
from :tbl a
where DOCTYPE= 59 and ifnull(UNITPRICE,0) <0
		or (DocNum in (select  "DocNum" from :tblExceptionDocNum where "DocType" = 60)); --31.03.2025

---- Cập nhật thông tin Batch - OBTN
update a
set AdmissionDate  = b."InDate",
	Dis_Status  = b."U_Dis_Status"
from :tbl a
inner join OBTN b on a.SSCC = b."DistNumber";
---
	-- Update SSCC Ông Nội, và ngày tạo 
if exists (select 1 from :tbl ) then
	begin

		 -- LV0
		 update a
		 set a.Father1  = b.SSCC_OngNoi,
		 	 a.AdmissionDate_Father1 = b.Admission_Date_OngNoi
		 from :tbl a
		 inner join :TruyXuatSSCC b on   a.SSCC = b.SSCC_LV0
		 where ifnull(b.SSCC_LV6,'') = '' and ifnull(b.SSCC_LV5,'') = '' 
		 and ifnull(b.SSCC_LV4,'') = '' and ifnull(b.SSCC_LV3,'') = '' 
		 and  ifnull(b.SSCC_LV2,'') =''  and ifnull(b.SSCC_LV1,'') = ''and  ifnull(b.SSCC_LV0,'') <> '';
		 		 -- LV1
		 update a
		 set a.Father1  = b.SSCC_OngNoi,
		 	 a.AdmissionDate_Father1 = b.Admission_Date_OngNoi
		 from :tbl a
		 inner join :TruyXuatSSCC b on   a.SSCC = b.SSCC_LV1
		 where ifnull(b.SSCC_LV6,'') = '' and ifnull(b.SSCC_LV5,'') = ''
		  and ifnull(b.SSCC_LV4,'') = '' and ifnull(b.SSCC_LV3,'') = '' and  ifnull(b.SSCC_LV2,'') =''  and  ifnull(b.SSCC_LV1,'') <> '';
		  		 -- LV2
		 update a
		 set a.Father1  = b.SSCC_OngNoi,
		 	 a.AdmissionDate_Father1 = b.Admission_Date_OngNoi
		 from :tbl a
		 inner join :TruyXuatSSCC b on   a.SSCC = b.SSCC_LV2
		 where ifnull(b.SSCC_LV6,'') = '' and ifnull(b.SSCC_LV5,'') = '' 
		 and ifnull(b.SSCC_LV4,'') = '' and ifnull(b.SSCC_LV3,'') = ''   and  ifnull(b.SSCC_LV2,'') <> '';
		 		 -- LV3
		 update a
		 set a.Father1  = b.SSCC_OngNoi,
		 	 a.AdmissionDate_Father1 = b.Admission_Date_OngNoi
		 from :tbl a
		 inner join :TruyXuatSSCC b on   a.SSCC = b.SSCC_LV3
		 where ifnull(b.SSCC_LV6,'') = '' and ifnull(b.SSCC_LV5,'') = '' and ifnull(b.SSCC_LV4,'') = '' and  ifnull(b.SSCC_LV3,'') <> '';
		 		 -- LV4
		 update a
		 set a.Father1  = b.SSCC_OngNoi,
		 	 a.AdmissionDate_Father1 = b.Admission_Date_OngNoi
		 from :tbl a
		 inner join :TruyXuatSSCC b on   a.SSCC = b.SSCC_LV4
		 where ifnull(b.SSCC_LV6,'') = '' and ifnull(b.SSCC_LV5,'') = '' and  ifnull(b.SSCC_LV4,'') <> '';
		 		 -- LV5
		 update a
		 set a.Father1  = b.SSCC_OngNoi,
		 	 a.AdmissionDate_Father1 = b.Admission_Date_OngNoi
		 from :tbl a
		 inner join :TruyXuatSSCC b on   a.SSCC = b.SSCC_LV5
		 where ifnull(b.SSCC_LV6,'') = '' and  ifnull(b.SSCC_LV5,'') <> '';
		 		 -- LV6
		 		 update a
		 set a.Father1  = b.SSCC_OngNoi,
		 	 a.AdmissionDate_Father1 = b.Admission_Date_OngNoi
		 from :tbl a
		 inner join :TruyXuatSSCC b on   a.SSCC = b.SSCC_LV6
		 where ifnull(b.SSCC_LV6,'') <> '';


	
	end;
end if;
--- Update  SSCC Ông Nội, và ngày tạo  = chính nó nếu nó là kẻ FA

update a 
set a.Father1  = SSCC,
	a.AdmissionDate_Father1 = AdmissionDate
from  :tbl a
where ifnull(a.Father1,'') = '';
-- update  OutSourcing_Type

update a
set OutSourcing_Type  = Case when OutSourcing_Type  = '1' then 'Upgrade' 
							 when OutSourcing_Type  = '2' then 'Downgrade' 
							 when OutSourcing_Type  = '3' then 'Merge' 
							 when OutSourcing_Type  = '4' then 'Split' 
							 when OutSourcing_Type  = '5' then 'Convert' end
from :tbl a;

--- UPDATE STARTDATE, DUE DATE PO

update a
set StartDate  = b."StartDate",
	DueDate   = b."DueDate"
from :tbl a
inner join OWOR b on a.WOID  = b."DocEntry";
--- UPDATE 
Update a
set  Materials  = 0,
	 Machine = 0,
	 Labour = 0
from  :tbl a
where DocNote in ('Receipt from Outsourcing','Issue for Outsourcing');
		--a.DocNum in (select "DocNum" from  :tblExceptionDocNum)
		--and ifnull(OutSourcing_Type,'')  = ''
		--and a.DOCNUM not in (select distinct b.DOCNUM  from :pro11 b);

/*
--- Xử lý chênh lệch so với số liệu hạch toán
JE = select "Account",Sum("Debit"), Sum("Credit"), sum("Credit"-"Debit") as Amount ,  sum("Debit"-"Credit") as Amount_Debit_Credit 
		from "JDT1"
		where ("Account" like '621%' or "Account" like '622%' or "Account" like '627%') 
				and "RefDate" between fdate and tdate group by "Account";
---======================== LBOR
-- :total6229
  select ifnull(sum(Amount_Debit_Credit),0) into total6229   from :JE  where "Account" like '6229%' ;		
-- Lbor
		update a
		set a.LABOUR  =  a.LABOUR +  ((case when :total6229 <0 then :total6229 * -1 else :total6229 end)  
									- (select case when  sum(b.LABOUR) <0  then  sum(b.LABOUR)  * -1 else sum(b.LABOUR)  end from  :tbl b) ) 
		from :tbl a
		where a.SSCC  = (select max(c.SSCC) from :tbl c);
---======================== Machine 
-- :total6279
  select ifnull(sum(Amount_Debit_Credit),0) into total6279   from :JE  where "Account" like '6279%' ;
		update a
		set a.MACHINE  =  a.MACHINE +  ((case when :total6279 <0 then :total6279 * -1 else :total6279 end)  
									- (select case when  sum(b.MACHINE) <0  then  sum(b.MACHINE)  * -1 else sum(b.MACHINE)  end from  :tbl b) ) 
		from :tbl a
		where a.SSCC  = (select max(c.SSCC) from :tbl c);

*/
IF (OutSoucing = 'Y') THEN
 update a
 set a.Doc_Exception = 'Y'
 from :tbl a
 inner join  OIGN b on a.DOCID = b."DocEntry" and ifnull(b."U_Include_In_Report",'')  <>'N'
 where a.DOCTYPE = '59';
 --
 update a
 set a.Doc_Exception = 'Y'
 from :tbl a
 inner join  OIGE b on a.DOCID = b."DocEntry" and ifnull(b."U_Include_In_Report",'')  <>'N'
 where a.DOCTYPE = '60';

end if;

tab_result  = select    * 
				from :tbl a
				where   
				(
					ifnull(:Batch, '') = '' 
					or cast (a.Batch as nvarchar) in (select "DATA" from split(:Batch ,','))
				)
				and
				(
					ifnull(:itemcode, '') = '' 
					or cast (a.ItemCode as nvarchar) in (select "DATA" from split(:itemcode ,','))
				)
				and
				(
					ifnull(:DocNumPO, '') = '' 
					or cast (a.WONO as nvarchar) in (select "DATA" from split(:DocNumPO ,','))
				)
				and
				(
					ifnull(:OutSoucing, 'Y') = 'Y' 
					or ifnull(a.OutSourcingID, '') = '' 
				)
				order by DocDate||DocTime;


-- drop table 
--drop table :TruyXuatSSCC;
 
end
