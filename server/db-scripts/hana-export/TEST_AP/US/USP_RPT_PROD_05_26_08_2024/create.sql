cREATE procedure USP_RPT_PROD_05_26_08_2024 (
	in fdate			timestamp,
	in tdate			timestamp,
	--in groupID			int,
	in warehouse		nvarchar(20),
	--in branch			int,
	in batch			varchar(50), --ILD_LayBatch_BCQAQC
	in DocNumPO			nvarchar(1000),
	in itemcode			varchar(150),--ILD_LayItemThanhPham
	in Resource 		char(1),
	out tab_result table(
		--RowNum 	int GENERATED  by default as identity,
		GrpCode			int,
		GrpName			nvarchar(100),
		ItemCode		nvarchar(50),
		ItemName		nvarchar(100),
		ItemUoM			nvarchar(50),
		WOID			int,
		WONo			nvarchar(50),
		WOStatus		nvarchar(20),
		WOType			nvarchar(20),
		WOItemCode		nvarchar(50),
		WOItemName		nvarchar(100),
		WOItemGroup		nvarchar(100),
		DocType			nvarchar(20),
		DocNote			nvarchar(50),
		DocID			int,
		DocNum			int,
		DocDate			timestamp,
		DocTime 		nvarchar(15),
		QtyTotal		numeric(21,6), -- số lượng tổng của từng Batch/SSCC
		Qty				numeric(21,6),	-- -- số lượng tôchi tiết của từng Batch/SSCC
		Amount			numeric(21,6),
		Note			nvarchar(100),
		SSCC_BatchNum	nvarchar(50),
		Batch_MnfSerial	nvarchar(50),
		MnfDate			timestamp,
		ExpDate			timestamp,
		AdmissionDate	timestamp,
		SSCCStatus_LotNumber	nvarchar(50),
		UnitPrice		numeric(21,6),
		DisID			int,
		DisNo			varchar(50),
		RFPID			int,
		RFPNo			varchar(50),
		
		FATHER1			nvarchar(50),
		FATHER1_DATE	date
	
		
	)
)
as
begin
	declare total6229 decimal(28,10);
	declare tbl table(
		--RowNum 	int GENERATED  by default as identity,
		GrpCode			int,
		GrpName			nvarchar(100),
		ItemCode		nvarchar(50),
		ItemName		nvarchar(100),
		ItemUoM			nvarchar(50),
		WOID			int,
		WONo			nvarchar(50),
		WOStatus		nvarchar(20),
		WOType			nvarchar(20),
		WOItemCode		nvarchar(50),
		WOItemName		nvarchar(100),
		WOItemGroup		nvarchar(100),
		DocType			nvarchar(20),
		DocNote			nvarchar(50),
		DocID			int,
		DocNum			int,
		DocDate			timestamp,
		DocTime 		nvarchar(15),
		QtyTotal		numeric(21,6), -- số lượng tổng của từng Batch/SSCC
		Qty				numeric(21,6),	-- -- số lượng tôchi tiết của từng Batch/SSCC
		Amount			numeric(21,6),
		Note			nvarchar(100),
		SSCC_BatchNum	nvarchar(50),
		Batch_MnfSerial	nvarchar(50),
		MnfDate			timestamp,
		ExpDate			timestamp,
		AdmissionDate	timestamp,
		SSCCStatus_LotNumber	nvarchar(50),
		UnitPrice		numeric(21,6),
		DisID			int,
		DisNo			varchar(50),
		RFPID			int,
		RFPNo			varchar(50),
		
		FATHER1			nvarchar(50),
		FATHER1_DATE	date
	
		
	);
	declare tblWH table (
		WHCode			nvarchar(20),
		WHName			nvarchar(100),
		WHAccount		nvarchar(20)
	);
	declare tblITEM table (
		ItemCode		nvarchar(50),
		ItemName		nvarchar(100),
		UoM				nvarchar(50),
		InventoryAcct	nvarchar(20),
		GLMethod		char(1),
		GrpID			int,
		GrpName			nvarchar(100)
	);
	declare tblResource table (
		GrpCode			int,
		GrpName			nvarchar(100),
		ItemCode		nvarchar(50),
		ItemName		nvarchar(100),
		ItemUoM			nvarchar(50),
		LineNum			int,
		WOID			int,
		WONo			nvarchar(50),
		WOStatus		nvarchar(20),
		WOType			nvarchar(20),
		WOItemCode		nvarchar(50),
		WOItemName		nvarchar(100),
		WOItemGroup		nvarchar(100),
		DocType			nvarchar(20),
		DocID			int,
		DocDate			timestamp,
		DocTime 		nvarchar(15),
		QtyTotal		numeric(21,6),
		Qty				numeric(21,6),	-- -- số lượng tôchi tiết của từng Batch/SSCC
		Amount			numeric(21,6),
		UnitPrice		numeric(21,6)
		
	);
	
	declare pro11 table
	(
		FGCODE VARCHAR(50),
		ITEMCODE VARCHAR(50),
		ITEMNAME NVARCHAR(100),
		ITMSGRPCOD int,
		ITMSGRPNAME NVARCHAR(100),
		WOID  INT,
	 	WONO VARCHAR(50),
	 	STDQTY DECIMAL(21,6),
	 	UNITPRICE DECIMAL(21,6),
		STDAMOUNT  DECIMAL(21,6),
		DISID int,
		DISNO VARCHAR(50),		
		DOCENTRY int ,
		DOCNUM int,
		POSTINGDATE date,
		DocType int
		
	);
		--Truy xuat SSCC
	call ILD_TruyXuatSSCCNhieuCap (tab_result_truyxuat); 
	create local temporary table #TruyXuatSSCC as (select * from :tab_result_truyxuat);
	
	/* TAM KHOA
	CALL ILD_R_IRS_StandardCost(fdate,tdate, datapro11);
	create local temporary table #pro11 as (select   FGCODE,ITEMCODE,ITEMNAME,ITMSGRPCOD,ITMSGRPNAME,
													WOID,WONO,
													STDQTY,
													UNITPRICE,STDAMOUNT,
													DISID,DISNO,
													--- Document
													DOCENTRY,DOCNUM,POSTINGDATE,DocType 
											from :datapro11 
											where  ITMSGRPCOD in (125,126) and ifnull(DISNO,'') <>''
											 );
	*/

	-- CHI TIẾT XUẤT KHO CHO SẢN XUẤT----------------------------------------------------------------------------------------------
	insert into :tblWH (WHCode,WHName,WHAccount) select "WhsCode","WhsName","BalInvntAc" from "OWHS" 
	where --(branch in (-1,0) or "BPLid"=branch) and 
	(warehouse='' or "WhsCode"=warehouse);
	------------------------------------------------------------------------------------------------------------------------

	insert into :tblITEM (ItemCode,ItemName,UoM,GrpID,GrpName,GLMethod,InventoryAcct) 
	select a."ItemCode",a."ItemName",a."InvntryUom",b."ItmsGrpCod",b."ItmsGrpNam",a."GLMethod",case when a."GLMethod"='C' then b."BalInvntAc" end
	from "OITM" a, "OITB" b 
	where a."ItmsGrpCod"=b."ItmsGrpCod" and a."ItemType"='I';
	-- REPORT ----------------------------------------------------------------------------------------------------------------
	insert into :tbl (GrpCode,GrpName,ItemCode,ItemName,ItemUoM,WOID,WONo,WOStatus,
					WOType,WOItemCode,DocType,DocID,DocDate,QtyTotal,Amount,Note, DocTime,SSCC_BatchNum,Batch_MnfSerial,MnfDate,ExpDate,AdmissionDate,SSCCStatus_LotNumber,Qty
					)
		
	select c.GrpID,c.GrpName,c.ItemCode,c.ItemName,c.UoM,d."DocEntry" ,d."DocNum",
			d."Status",d."Type",d."ItemCode",a."TransType",a."CreatedBy",a."DocDate",a."InQty"-a."OutQty",a."TransValue",a."Comments",
									case when LENGTH(a."DocTime")  = 3 then  '0'||left(a."DocTime",1) || ':' || right(a."DocTime",2) 
		 		when LENGTH(a."DocTime")  = 4 then   left(a."DocTime",2) || ':' || right(a."DocTime",2)  
	 			else  '' end  as "DocTime",e."BatchNum",f."MnfSerial",f."MnfDate",f."ExpDate",f."InDate",f."LotNumber",e."Quantity"
	 			
	from "OINM" a
		left join :tblWH b on a."Warehouse"=b.WHCode
		left join :tblITEM c on a."ItemCode"=c.ItemCode
		left join "OWOR" d on a."AppObjAbs"=d."DocEntry"
		left join IBT1 e on e."BaseEntry" = a."CreatedBy" and e."ItemCode" = c.ItemCode and e."BaseType" = a."TransType"
		left join OBTN f on e."BatchNum"=f."DistNumber" and e."ItemCode"=f."ItemCode"
	where a."TransType"  in  (60,59) and  a."AppObjType"  ='C'  and a."ApplObj"='202' and a."DocDate" between fdate and tdate
	and (ifnull(:DocNumPO, '') = '' or cast(d."DocNum" as nvarchar) in (select "DATA" from split(:DocNumPO,',')))
	and (ifnull(:itemcode, '') = '' or cast (c.ItemCode as nvarchar) in (select "DATA" from split(:itemcode ,',')))
	and (ifnull(:Batch, '') = '' or cast (f."MnfSerial" as nvarchar) in (select "DATA" from split(:Batch ,',')))
	;
	-- 
	update x set Qty = (QtyTotal) from :tbl x where Qty is null or Qty = 0;
	update x set Qty = (- Qty) from :tbl x where DocType = 59 and Qty < 0;
	update x set Qty = (- Qty)from :tbl x where DocType = 60 and Qty > 0;
	update a set WOItemName = (select x."ItemName" from "OITM" x where x."ItemCode"=a.WOItemCode) from :tbl a;
	update a set WOItemGroup = (select x."ItmsGrpCod" from "OITM" x where x."ItemCode"=a.WOItemCode) from :tbl a;
	update a set WOItemGroup = (select x."ItmsGrpNam" from "OITB" x where x."ItmsGrpCod"=a.WOItemGroup) from :tbl a;

--
if (:Resource = 'Y') then
insert into :tblResource ( WOItemCode,ItemCode,ItemName,GrpCode,GrpName, 
									WOID,WONO,
									QtyTotal,Qty, Amount,LineNum, 
									DocID, DocDate,DocTime,DocType,
									WOType, WOStatus)
	
	select e."ItemCode" as "FGCode",b."ItemCode",b."Dscription",  d."ItmsGrpCod", d."ItmsGrpNam",
						 b."BaseEntry" as "WOID",b."BaseRef" as "WONO",
						 sum(b."Quantity") as "QtyTotal",sum(b."Quantity") as "Qty", sum(b."LineTotal"), b."BaseLine",
						 
						 a."DocEntry",a."DocDate",
						 --DocTime
						 case when LENGTH(a."DocTime")  = 3 then  '0'||left(a."DocTime",1) || ':' || right(a."DocTime",2) 
		 				when LENGTH(a."DocTime")  = 4 then   left(a."DocTime",2) || ':' || right(a."DocTime",2)  
	 					else  '' end  as "DocTime",
						 
						 60 as "DocType",
						 e."Type",  e."Status"
	from OIGE a 
	inner join IGE1 b on a."DocEntry"  = b."DocEntry" 
	left join OITM c on c."ItemCode"  =  b."ItemCode"
	left join OITB d on c."ItmsGrpCod"  = d."ItmsGrpCod"
	inner join OWOR e on e."DocEntry" = b."BaseEntry"
	where a."DocDate" between fdate and tdate
		and c."ItmsGrpCod" in (125,126)
		and b."BaseType" = 202
		
	and (ifnull(:DocNumPO, '') = '' or cast(e."DocNum" as nvarchar) in (select "DATA" from split(:DocNumPO,',')))
	and (ifnull(:itemcode, '') = '' or cast (b."ItemCode" as nvarchar) in (select "DATA" from split(:itemcode ,',')))
		
	group by e."ItemCode",b."ItemCode",b."Dscription",  d."ItmsGrpCod", d."ItmsGrpNam",
						 b."BaseEntry",b."BaseRef",
						  a."DocEntry",a."DocDate",a."DocTime", b."BaseLine", e."Type",  e."Status";
---
	
	
insert into :tbl (WOItemCode,ItemCode,ItemName,GrpCode,GrpName, 
									WOID,WONO,
									QtyTotal,Qty, Amount, 
									DocID, DocDate,DocTime,DocType, WOType, WOStatus)
		
select WOItemCode,ItemCode,ItemName,GrpCode,GrpName, 
									WOID,WONO,
									-QtyTotal,-Qty, -Amount, 
									DocID, DocDate,DocTime,DocType, WOType, WOStatus
from :tblResource a;
		
--============ Insert du lieu cho bang tam pro11
--1.PRO11_DATA_TEMP
	insert into :pro11 (FGCODE,ITEMCODE,ITEMNAME,ITMSGRPCOD,ITMSGRPNAME,
		WOID,WONO,
		STDQTY,
		UNITPRICE,STDAMOUNT,
		DISID,DISNO,
		--- Document
		DOCENTRY,DOCNUM,POSTINGDATE,DocType )
	select FGCODE,ITEMCODE,ITEMNAME,ITMSGRPCOD,ITMSGRPNAME,
		WOID,WONO,
		 STDQTY,
		UNITPRICE, STDAMOUNT,
		DISID,DISNO,
		--- Document
		DOCENTRY,DOCNUM,POSTINGDATE,DocType
	from  PRO11_DATA_TEMP
	where POSTINGDATE between fdate and tdate 
	  	and ITMSGRPCOD in (125,126) and ifnull(DISNO,'') <>'';
--2.COSTING_STANDARDCOST
	insert into :pro11 (FGCODE,ITEMCODE,ITEMNAME,ITMSGRPCOD,ITMSGRPNAME,
		WOID,WONO,
		STDQTY,
		UNITPRICE,STDAMOUNT,
		DISID,DISNO,
		--- Document
		DOCENTRY,DOCNUM,POSTINGDATE,DocType )
	select FGCODE,ITEMCODE,ITEMNAME,ITMSGRPCOD,ITMSGRPNAME,
		WOID,WONO,
		 STDQTY,
		UNITPRICE, STDAMOUNT,
		DISID,DISNO,
		--- Document
		DOCENTRY,DOCNUM,POSTINGDATE,60
	from  COSTING_STANDARDCOST
	where POSTINGDATE between fdate and tdate 
	  	and ITMSGRPCOD in (125,126) and ifnull(DISNO,'') <>'';

--============
		
		
		
		
----- Insert dữ liệu từ pro11 --> :tbl (máy móc, nhân công của lệnh rã)
insert into :tbl (WOItemCode,ItemCode,ItemName,GrpCode,GrpName, 
									WOID,WONO,
									QtyTotal,Qty, 
									UnitPrice,Amount, 
									DisID, DisNo,
									--- Document
									DocID, DocNum,DocDate,DocType
									
									)
		
select FGCODE,ITEMCODE,ITEMNAME,ITMSGRPCOD,ITMSGRPNAME,
		WOID,WONO,
		-STDQTY,-STDQTY,
		UNITPRICE,-STDAMOUNT,
		DISID,DISNO,
		--- Document
		DOCENTRY,DOCNUM,POSTINGDATE,DocType
		
		
		
from :pro11
where WONO in (select distinct WONO from :tbl);
		
	end if; -- Ket thuc insert  du lieu Resource
-----
update a
set WOItemName = b."ItemName",
	WOItemGroup  =  c."ItmsGrpNam"
from :tbl a
inner join OITM b on a.WOItemCode  = b."ItemCode"
inner join OITB c on c."ItmsGrpCod" = b."ItmsGrpCod"
where WOItemName is null;

---
	update a set Amount =   (a.Qty * a.Amount)/ (case when QtyTotal = 0 then 1 else QtyTotal end)  from :tbl a where ifnull(DisNo,'')  ='';
	
	update :tbl 
	set UnitPrice  = Amount/ (case when Qty = 0 then 1 else Qty end) 
	 where ifnull(DisNo,'')  ='';
	--

	update a
	set WOType = b."Type" ,
		WOStatus = b."Status" 
	from :tbl a
	inner join OWOR b  on a.WOID  = b."DocEntry";

	---
	update :tbl set DocNote = 'Receipt from Production' where DocType='59' ;
	update :tbl 
	set DocNote = case when ifnull(DisNo,'')='' then 'Issue for Production'  else  'Receipt from Disassebly' end
	where DocType='60';
	--- update thông tin  phiếu receipt from production: vì khi RFP được add, đồng thời tạo ra phiếu IFP của labor, machine
	update a
	set a.RFPID  = c."DocEntry",
		a.RFPNo =  c."DocNum"
	from :tbl a
	inner join  OIGE b on a.DocID = b."DocEntry"
	inner join  OIGN c on b."RelatedEnt" = c."DocEntry"
	where a.DocType = '60' and ifnull(b."RelatedTyp",0)   = 59;
	---
	
	
	update :tbl set WOType='Standard' where WOType='S';
	update :tbl set WOType='Disassembly' where WOType='D';
	update :tbl set WOType='Special' where WOType='P';
	update :tbl set WOStatus='Released' where WOStatus='R';
	update :tbl set WOStatus='Closed' where WOStatus='L';
	
	--
	update x set DocNum = (select "DocNum" from APZ_ODOC where "DocEntry" = x.DocID and "ObjType" = x.DocType)  from :tbl x where  ifnull(DisNo,'') = ''; 
/*
--- Xử lý chênh lệch so với số liệu hạch toán
JE = select "Account",Sum("Debit"), Sum("Credit"), sum("Credit"-"Debit") as Amount ,  sum("Debit"-"Credit") as Amount_Debit_Credit 
		from "JDT1"
		where ("Account" like '621%' or "Account" like '622%' or "Account" like '627%') 
				and "RefDate" between fdate and tdate group by "Account";
-- :total6229
  select ifnull(sum(Amount_Debit_Credit),0) into total6229   from :JE  where "Account" like '6229%' ;		
-- Lbor
		update a
		set a.Amount  =  a.Amount +  ((case when :total6229 <0 then :total6229 * -1 else :total6229 end)  
									- (select case when  sum(b.Amount) <0  then  sum(b.Amount)  * -1 else sum(b.Amount)  
													end from  :tbl b where b.GrpCode = '126') ) 
		from :tbl a
		where a.RowNum  = 11210;
		--(select max(c.RowNum ) from :tbl c where c.GrpCode = '126');
*/

----
	-- Update SSCC Ông Nội, và ngày tạo 
if exists (select 1 from :tbl ) then
	begin

		 -- LV0
		 update a
		 set a.Father1  = b.SSCC_OngNoi,
		 	 a.FATHER1_DATE = b.Admission_Date_OngNoi
		 from :tbl a
		 inner join #TruyXuatSSCC b on   a.SSCC_BatchNum = b.SSCC_LV0
		 where ifnull(b.SSCC_LV6,'') = '' and ifnull(b.SSCC_LV5,'') = '' 
		 and ifnull(b.SSCC_LV4,'') = '' and ifnull(b.SSCC_LV3,'') = '' 
		 and  ifnull(b.SSCC_LV2,'') =''  and ifnull(b.SSCC_LV1,'') = ''and  ifnull(b.SSCC_LV0,'') <> '';
		 		 -- LV1
		 update a
		 set a.Father1  = b.SSCC_OngNoi,
		 	 a.FATHER1_DATE = b.Admission_Date_OngNoi
		 from :tbl a
		 inner join #TruyXuatSSCC b on   a.SSCC_BatchNum = b.SSCC_LV1
		 where ifnull(b.SSCC_LV6,'') = '' and ifnull(b.SSCC_LV5,'') = ''
		  and ifnull(b.SSCC_LV4,'') = '' and ifnull(b.SSCC_LV3,'') = '' and  ifnull(b.SSCC_LV2,'') =''  and  ifnull(b.SSCC_LV1,'') <> '';
		  		 -- LV2
		 update a
		 set a.Father1  = b.SSCC_OngNoi,
		 	 a.FATHER1_DATE = b.Admission_Date_OngNoi
		 from :tbl a
		 inner join #TruyXuatSSCC b on   a.SSCC_BatchNum = b.SSCC_LV2
		 where ifnull(b.SSCC_LV6,'') = '' and ifnull(b.SSCC_LV5,'') = '' 
		 and ifnull(b.SSCC_LV4,'') = '' and ifnull(b.SSCC_LV3,'') = ''   and  ifnull(b.SSCC_LV2,'') <> '';
		 		 -- LV3
		 update a
		 set a.Father1  = b.SSCC_OngNoi,
		 	 a.FATHER1_DATE = b.Admission_Date_OngNoi
		 from :tbl a
		 inner join #TruyXuatSSCC b on   a.SSCC_BatchNum = b.SSCC_LV3
		 where ifnull(b.SSCC_LV6,'') = '' and ifnull(b.SSCC_LV5,'') = '' and ifnull(b.SSCC_LV4,'') = '' and  ifnull(b.SSCC_LV3,'') <> '';
		 		 -- LV4
		 update a
		 set a.Father1  = b.SSCC_OngNoi,
		 	 a.FATHER1_DATE = b.Admission_Date_OngNoi
		 from :tbl a
		 inner join #TruyXuatSSCC b on   a.SSCC_BatchNum = b.SSCC_LV4
		 where ifnull(b.SSCC_LV6,'') = '' and ifnull(b.SSCC_LV5,'') = '' and  ifnull(b.SSCC_LV4,'') <> '';
		 		 -- LV5
		 update a
		 set a.Father1  = b.SSCC_OngNoi,
		 	 a.FATHER1_DATE = b.Admission_Date_OngNoi
		 from :tbl a
		 inner join #TruyXuatSSCC b on   a.SSCC_BatchNum = b.SSCC_LV5
		 where ifnull(b.SSCC_LV6,'') = '' and  ifnull(b.SSCC_LV5,'') <> '';
		 		 -- LV6
		 		 update a
		 set a.Father1  = b.SSCC_OngNoi,
		 	 a.FATHER1_DATE = b.Admission_Date_OngNoi
		 from :tbl a
		 inner join #TruyXuatSSCC b on   a.SSCC_BatchNum = b.SSCC_LV6
		 where ifnull(b.SSCC_LV6,'') <> '';


	
	end;
end if;
--- Update  SSCC Ông Nội, và ngày tạo  = chính nó nếu nó là kẻ FA

update a 
set a.Father1  = SSCC_BatchNum,
	a.FATHER1_DATE = AdmissionDate
from  :tbl a
where ifnull(a.Father1,'') = '';
	-----------
tab_result  = select * from :tbl;	
drop table #TruyXuatSSCC;
end

