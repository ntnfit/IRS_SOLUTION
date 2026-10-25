CREATE procedure ILD_R_TheoDoiKetQuaQAQC_ThanhPham (
	in itemcode			nvarchar(5000), --ILD_LayItemQA
	in groupID			varchar(500), --USP_CFL_ITEMGROUP
	in warehouse		nvarchar(20), --USP_CFL_WAREHOUSE
	in FromDate			timestamp,
	in ToDate			timestamp,
	in UDCODE			varchar(500), --ILD_Lay_UDCODEQA
	in QAResult			varchar(500)
)
as
begin
/* Tham số đầu vào
declare FromDate date = '2020-08-01';
declare ToDate date = '2024-08-31';
declare groupID int;
declare itemcode nvarchar(5000);
declare warehouse nvarchar(20);*/

create local temporary table #tblWH (
		WHCode			nvarchar(20),
		WHName			nvarchar(100),
		WHAccount		nvarchar(20)
	);
	insert into #tblWH (WHCode,WHName,WHAccount) 
	select "WhsCode","WhsName","BalInvntAc" 
	from "OWHS"
	where (warehouse='' or "WhsCode"=warehouse);
--- Item
create local temporary table #tblITEM (
		ItemCode		nvarchar(50),
		ItemName		nvarchar(100),
		UoM				nvarchar(50),
		InventoryAcct	nvarchar(20),
		GLMethod		char(1),
		GrpID			int,
		GrpName			nvarchar(100)
	);
	insert into #tblITEM (ItemCode,ItemName,UoM,GrpID,GrpName,GLMethod,InventoryAcct) 
	select a."ItemCode",a."ItemName",a."InvntryUom",b."ItmsGrpCod",b."ItmsGrpNam",a."GLMethod",case when a."GLMethod"='C' then b."BalInvntAc" end
	from "OITM" a, "OITB" b 
	where a."ItmsGrpCod"=b."ItmsGrpCod" and a."InvntItem"='Y'
	and
		(
			IFNULL(:groupID, '') = '' OR cast (a."ItmsGrpCod" as nvarchar) IN (SELECT "DATA" FROM "SPLIT"(:groupID, ','))
		)
	and
		(
			IFNULL(:itemcode, '') = '' OR cast (a."ItemCode" as nvarchar) IN (SELECT "DATA" FROM "SPLIT"(:itemcode, ','))
		) 
	;
--- 
Create local temporary table #LenhRa
(
	DocEntry int,
	DocType  varchar(20)
);
---
create local temporary table #tbl
(
	DocEntry int,
	DocNum varchar(50),
	Vendor varchar(50),
	VendorName nvarchar(150),
	PostingDate date,
	DocumentDate	date,
	SoToKhai	varchar(50),
	ItemCode	varchar(50),
	ItemName	nvarchar(150),
	ItemGroup	nvarchar(100),
	WhseCode	varchar(50),
	Qty	decimal(19,6),
	SSCC varchar(50),
	SSCC_Entry int, -- Là AbsEntry (Key) OBTN Batch (SSCC)
	Batch varchar(50),
	Status varchar(20),
	ExpDate date, --Expiration Date
	MfrDate date,
	AdmissionDate date,
	UDCode varchar(10),
	PlanNo	nvarchar(20),
	InsLotStockType varchar(20),
	BsDocType varchar(20),
	BsDocEntry varchar(50),
	BaseType varchar(20),
	SSCC_Remark nvarchar(100),
	Soxe varchar(20),
	WhseCurrent varchar(20),
	QAQCEntry varchar(20),
	DocNumBsEntry	varchar(50),
	
	UDName		nvarchar(100)
);
insert into #tbl (DocEntry,DocNum, SSCC,ItemCode,ItemName, WhseCode,
					Vendor,VendorName,PostingDate,DocumentDate, Qty , BsDocType,BsDocEntry, BaseType )

select "BaseEntry","BaseNum", "BatchNum",Nguon2."ItemCode","ItemName","WhsCode",
		"CardCode","CardName","DocDate","CreateDate",
		 sum("Quantity_D0")   - sum("Quantity_D1")  as "Quantity", "BsDocType","BsDocEntry","BaseType"
From 
	(
		select "BatchNum", "ItemCode","ItemName","BaseEntry","BaseNum","WhsCode",
		"CardCode","CardName","DocDate","CreateDate","BsDocType","BsDocEntry","BaseType",
		
				case when  "Direction"  = 0 then "Quantity" else 0 end as "Quantity_D0",
				case when  "Direction"  = 1 then "Quantity" else 0 end as "Quantity_D1"
				
				
		from 
		(

				select "Quantity", "BatchNum", "Direction" ,"ItemCode","ItemName","WhsCode",
					"BaseEntry","BaseNum","CardCode","CardName","DocDate","CreateDate","BsDocType","BsDocEntry","BaseType"
				from IBT1 
				where   
						     "ItemCode" in (select ItemCode from #tblITEM  )
						   and "WhsCode"  in (select WHCode from #tblWH)
						and "DocDate" between FromDate and ToDate
						 and "BaseType"  in (59,60)
		) as Nguon1
	) as Nguon2
	   
	group by "BatchNum", Nguon2."ItemCode","ItemName","BaseEntry","BaseNum","WhsCode",
	"CardCode","CardName","DocDate","CreateDate","BsDocType","BsDocEntry","BaseType";
--
 
 
	---
	update a
	set SoToKhai = (select distinct "U_SoToKhai" from OIGN b where b."DocEntry"  = a.DocEntry)
	from #tbl a;
	-- Update UD Code từ kết quả QAQC Result
 
	--- Insert giá tri va bảng lệnh rã: Vì thành phẩm sẽ bỏ ra sscc rã
	insert into #LenhRa (DocEntry, DocType)

	 select "BaseEntry", "BaseType"
	 From IBT1
	 where ifnull("BsDocType",'') = '202' 
	 and "BsDocEntry" 
	 				 in (
	 
							 select "DocEntry" 
							 from OWOR 
							 where "DocEntry" in (select  "BsDocEntry" 
													from IBT1
													where "DocDate" between FromDate and ToDate
															and ifnull("BsDocType",'') = '202' )
									and "Type"  = 'D'
						);
	
		
----
-------- Insert thông tin outsourcing

Create local temporary table #OutSourcing
(
	ReceiptID int,
	IssuedID int,
	OutSourcingID varchar(50)
);
insert into #OutSourcing
select "U_receiptID", "U_issueID", "Code"
from  "@TBL_OUTS_DOCNUM"
where "U_docDate" between FromDate and ToDate;
-- Xóa dữ liêu với SSCC (cha): phiếu issued trong outsourcing
/*
delete
from #tbl 
where DocEntry in (select IssuedID  from #OutSourcing)  and BaseType  =  60;
*/

---
/*
delete from #tbl where BaseType = '60';
-- Xóa đi dữ liệu phiếu với lệnh sản xuất rã
	delete from #tbl where DocEntry in (select DocEntry from #LenhRa);
 
*/
-------
create local temporary table #QCRESULT_SSCC
(
	Row_num int,
	QADocEntry int,
	ItemCode varchar(50),
	InsLotStock_SSCC varchar(5000),
	InsLotStock_Type varchar(15),
	InsLotStock_Status varchar(15),
	UDCode varchar(10)
	--BaseReceiptEntry_List  varchar(5000)
);
insert into #QCRESULT_SSCC (Row_num,QADocEntry,
							ItemCode,  
							InsLotStock_SSCC,
							InsLotStock_Type,
							InsLotStock_Status,
							UDCode)
							--BaseReceiptEntry_List)
							
							
select ROW_NUMBER() OVER (),a."DocEntry" as "QADocEntry", 
			  a."U_ItemCode",
			  b."U_SSCC" ,
			  b."U_Type", 
			  b."U_Status",
		 	  a."U_UDCode"
		      --a."U_BaseReceiptEntry"
from "@QC_RESULT_H" a
inner join "@QC_RESULT_INS_L" b on a."DocEntry"  = b."DocEntry"
where ifnull(cast(b."U_SSCC" as varchar(5000)),'') <> ''
		--and ifnull(cast(a."U_BaseReceiptEntry" as varchar(5000)),'')   <> ''
		and "U_BaseDocType"  in (59,202)
		and "U_BaseDocEntry" in (select distinct BsDocEntry from  #tbl where BsDocType  = 202);
		
---
Create local temporary table #SSCCDetail_QAResult
(
		QADocEntry int,
		Row_num int,
		SSCC_Part nvarchar(50),
		InsLotStock_Type varchar(15),
		UDCode varchar(50),
		--ReceiptEntry int,
		ItemCode varchar(50)
		
);
----
Begin
	Declare loopCount int = 0;
	Declare i int = 1;
	Declare QAResult_SSCC nvarchar(5000);
	Declare QADocEntry int;
	Declare InsLotStock_Type varchar(15);
	Declare UDCode varchar(50);
	Declare ItemCode varchar(50);
	
	Select Count(*) Into loopCount From #QCRESULT_SSCC;
	       While :i <= :loopCount
				DO
				Select a.InsLotStock_SSCC Into QAResult_SSCC From #QCRESULT_SSCC a Where Row_num = :i;
				Select a.QADocEntry Into QADocEntry From #QCRESULT_SSCC a Where Row_num = :i;
				Select a.InsLotStock_Type Into InsLotStock_Type From #QCRESULT_SSCC a Where Row_num = :i;
				Select a.UDCode Into UDCode From #QCRESULT_SSCC a Where Row_num = :i;
				Select a.ItemCode Into ItemCode From #QCRESULT_SSCC a Where Row_num = :i;
				
			 	insert into #SSCCDetail_QAResult ( SSCC_Part, Row_Num)
			 	select  "part",i
			 	from  "SPLITSTRING" (:QAResult_SSCC,',');
				
				--
				Update  a
				set QADocEntry = :QADocEntry,  InsLotStock_Type  =:InsLotStock_Type,
					UDCode  =:UDCode,
					ItemCode = :ItemCode
				from #SSCCDetail_QAResult a
				where i = row_num;
				
					i := i + 1;
			END WHILE;


End;
-----
/*
Create local temporary table #ReceiptEntry_Detail_QAResult
(
		QADocEntry int,
		Row_num int,
		BaseReceiptEntry varchar(15)
		
);	
Begin
	Declare loopCount int = 0;
	Declare i int = 1;
	Declare BaseReceiptEntry_List nvarchar(5000);
	Declare QADocEntry int;
	
	Select Count(*) Into loopCount From #QCRESULT_SSCC;
	       While :i <= :loopCount
				DO
				Select a.BaseReceiptEntry_List Into BaseReceiptEntry_List From #QCRESULT_SSCC a Where Row_num = :i;
				Select a.QADocEntry Into QADocEntry From #QCRESULT_SSCC a Where Row_num = :i;
	
				
			 	insert into #ReceiptEntry_Detail_QAResult ( BaseReceiptEntry, Row_Num)
			 	select  "part",i
			 	from  "SPLITSTRING" (:BaseReceiptEntry_List,',');
			 	---
				Update  a
				set QADocEntry = :QADocEntry
				from #ReceiptEntry_Detail_QAResult a
				where i = row_num;
				--
					i := i + 1;
			END WHILE;


End;
*/

---
/*
update a
set ReceiptEntry =  b."BaseEntry"
from #SSCCDetail_QAResult  a
inner join IBT1 b ON   "ItemCode"  = a.ItemCode and a.SSCC_Part  =  b."BatchNum"
where b."BaseType" =  59;
*/
---

-- Update UD Code từ kết quả QAQC Result
	update a
	set   
		 QAQCEntry = b.QADocEntry,
		 InsLotStockType  = b.InsLotStock_Type,
		 UDCode  = b.UDCode
	from #tbl a
	left join #SSCCDetail_QAResult b on  a.SSCC = b. SSCC_Part;
	--and a.DocEntry  =  b.ReceiptEntry

-- XÓA CÁC DÒNG CÓ BsDocEntry = 0 
DELETE FROM #tbl x where BsDocEntry = 0;
-- lọc theo tham số QA reult
IF :QAResult = '0' then
		DELETE FROM #tbl x where cast(QAQCEntry as varchar) is not null; -- Co lam phieu QC roi, chon 0:Not Yet QA Result: Lay du lieu chua co phieu QC
End IF;

--- Update thông tin Batch
update a
set Batch = b."MnfSerial",
	Status =  b."LotNumber",
	ExpDate = b."ExpDate",
	MfrDate  = b."MnfDate",
	AdmissionDate = b."InDate",
	SSCC_Remark =  b."U_Remark",
	SSCC_Entry  =  b."AbsEntry",
	ItemGroup  = d."ItmsGrpNam"
from #tbl a
left join OBTN b on a.SSCC  = b."DistNumber"
left join OITM c on a.ItemCode = c."ItemCode"
left join OITB d on d."ItmsGrpCod"  = c."ItmsGrpCod";
-- update Plan No 

update a
set PlanNo =  (select "U_QCTemplate" from OITM where "ItemCode" = a.ItemCode)
from #tbl a;

-- Update kho thực tế
update a
set WhseCurrent  = b."WhsCode"
from #tbl a
inner join OBBQ b on a.SSCC_Entry = b."SnBMDAbs" and a.ItemCode = b."ItemCode"
and "OnHandQty"  >0;

-- cập nhật các số docnum của phiếu gốc
update x set DocNumBsEntry	= (select "DocNum" from OWOR where x.BsDocEntry = "DocEntry")
from #tbl x;
-- where BaseType = 59;

--update x set DocNumBsEntry	= (select "DocNum" from OIGE where x.DocEntry = "DocEntry") 
--from #tbl x where BaseType = 60;
 
-- Update UD name
update x set UDName	= (select "Name" from  "@QC_UDCODE"  where "U_Active"  = 'Y' and x.UDCode = "Code") from #tbl x ;

--
delete 
from #tbl a
WHERE DocEntry  in (select DocEntry from  APZ_ODOC WHERE  "ObjType"  = 59 and "CANCELED"  <> 'N' 
					and "DocEntry"  = a.DocEntry  );

--
select DocEntry,DocNum, SSCC,Qty, ItemCode,ItemName,ItemGroup, WhseCode,WhseCurrent,
					Vendor,VendorName,PostingDate,DocumentDate,    
				Batch,
				Status,
				ExpDate,
				MfrDate,
				AdmissionDate,
				UDCode,
				BaseType,
				SSCC_Remark,
				PlanNo,
				InsLotStockType,
				BsDocEntry,
				Soxe,
				QAQCEntry,DocNumBsEntry,UDName
from #tbl
where   (
			 IFNULL(:UDCODE, '') = ''  OR cast (UDCode as nvarchar) IN (SELECT "DATA" FROM "SPLIT"(:UDCODE, ','))
		    or      (cast (ifnull(UDCode,'') as nvarchar(50)) = case when IFNULL(:UDCODE, '') = '999' then '' end )
		    
		 )
		 and 
		 ( IFNULL(:warehouse, '') = ''  OR cast (WhseCurrent as nvarchar) IN (SELECT "DATA" FROM "SPLIT"(:warehouse, ',')) )
		 
		 
		 And 
	 	 
		 (
		 	 
		 	  Ifnull(:QAResult, '') = ''  OR cast (QAQCEntry as nvarchar) IN (SELECT "DATA" FROM "SPLIT"(:QAResult, ','))
		      or      (cast (ifnull(QAQCEntry,'') as nvarchar(50)) = case when IFNULL(:QAResult, '') = '0' then '' end )
		 	 
		 	 
		 )
order by ItemCode;

--- Drop table
drop table #tbl;
drop table #tblWH;
drop table #tblITEM;
drop table #LenhRa;

drop table #QCRESULT_SSCC;
drop table #SSCCDetail_QAResult;
--drop table #ReceiptEntry_Detail_QAResult;
--drop table #OutSourcing;
end;

