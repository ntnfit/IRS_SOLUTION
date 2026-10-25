CREATE procedure ILD_R_TheoDoiKetQuaQAQC (
	in itemcode			nvarchar(5000), --ILD_LayItemQA
	in groupID			varchar(500), --USP_CFL_ITEMGROUP
	in warehouse		nvarchar(20), --USP_CFL_WAREHOUSE
	in doctype			varchar(3),
	in FromDate			timestamp,
	in ToDate			timestamp,
	in UDCODE			varchar(500), --ILD_Lay_UDCODEQA
	in QAResult			varchar(3)
)
as
begin
/* Tham số đầu vào
declare FromDate date = '2020-08-01';
declare ToDate date = '2024-08-31';
declare Type varchar(3) = 'TP'; -- NVL/ TP
declare groupID int;
declare itemcode nvarchar(5000);
declare warehouse nvarchar(20);*/

declare Type varchar(2);
select  case when :doctype = 'NVL' then '20' else '59' end
into Type
from dummy;

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
						--and "BaseType"  =:Type
		) as Nguon1
	) as Nguon2
	   
	group by "BatchNum", Nguon2."ItemCode","ItemName","BaseEntry","BaseNum","WhsCode",
	"CardCode","CardName","DocDate","CreateDate","BsDocType","BsDocEntry","BaseType";
--
--- Update số tời khai
IF(:Type = '20') then

 delete from #tbl where BaseType <> '20';
	-----
	update a
	set SoToKhai = b."U_SoToKhai",
		Soxe  =  b."U_ContNo"
	
	from #tbl a
	inner join   (select distinct "U_SoToKhai", "U_ContNo","DocEntry" from OPDN) b on  b."DocEntry"  = a.DocEntry;
	-- Update UD Code từ kết quả QAQC Result
	update a
	set  UDCODE =  b."U_UDCode",
		 QAQCEntry = b."DocEntry"
	from #tbl a
	left join "@QC_RESULT_H" b on b."U_BaseDocType" = '20' and b."U_BaseDocEntry" = a.DocEntry
	
	where  b."DocEntry"  = (select max("DocEntry") from "@QC_RESULT_H" c 
								where  c."U_BaseDocType" = '20' and c."U_BaseDocEntry" = a.DocEntry );
End IF;

IF(:Type = '59') then
	 delete from #tbl where BaseType not in ('59','60');
	---
	update a
	set SoToKhai = (select distinct "U_SoToKhai" from OIGN b where b."DocEntry"  = a.DocEntry)
	from #tbl a;
	-- Update UD Code từ kết quả QAQC Result
	update a
	set  UDCODE =  b."U_UDCode",
		QAQCEntry = b."DocEntry"
	from #tbl a
	left join "@QC_RESULT_H" b on  b."U_BaseDocEntry" = a.BsDocEntry
								   and b."U_BaseDocType" = '202'  
	where  b."DocEntry"  = (select max("DocEntry") from "@QC_RESULT_H" c 
								where  c."U_BaseDocType" = '202' and c."U_BaseDocEntry" = a.BsDocEntry );
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
	delete from #tbl where BaseType = '60';
-- Xóa đi dữ liệu phiếu với lệnh sản xuất rã
	delete from #tbl where DocEntry in (select DocEntry from #LenhRa);
End IF;
--- Update thông tin Batch
 
update a
set Batch = b."MnfSerial",
	Status =  b."LotNumber",
	ExpDate = b."ExpDate",
	MfrDate  = b."MnfDate",
	AdmissionDate = b."InDate",
	SSCC_Remark =  b."U_Remark",
	SSCC_Entry  =  b."AbsEntry"
from #tbl a
left join OBTN b on a.SSCC  = b."DistNumber";
-- update Plan No 

update a
set PlanNo =  (select "U_QCTemplate" from OITM where "ItemCode" = a.ItemCode)
from #tbl a;

---
Create local temporary table #temp2
(
	QADocEntry int,
	BaseType varchar(20),
	BaseEntry  varchar(20),
	BsDocType varchar(20),
	BsDocEntry varchar(50),
	SSCC varchar(50),
	InsLotStock_SSCC varchar(5000), -- List SSCC line tab InsLotStock
	InsLotStock_Type varchar(15)
);
---



	Temp1 = select distinct BaseType, DocEntry, BsDocType, BsDocEntry,SSCC
	from #tbl a
	where BsDocType  = (case when :Type = '59' then '202' else '22' end); -- 22: Purchase Order
	
   --- Insert distinct docentry lệnh sản xuất
	insert into #temp2 (BaseType,BaseEntry,BsDocType, BsDocEntry)
	select distinct  BaseType, DocEntry,BsDocType, BsDocEntry
	from  :Temp1;
	 
   --- Update thông tin QC Result
	update a
	set QADocEntry  = (select Max( "DocEntry" )
						from "@QC_RESULT_H" b
						 where  ifnull("U_BaseDocType",'') = (case when :Type = '59' then '202' else '20' end)
						   and  b."U_BaseDocEntry"  =   (case when :Type = '59' then a.BsDocEntry else a.BaseEntry end) )
	from #temp2 a;
	---
   ----


	--- xóa những record chưa có thông tin QAQC (do chưa làm phiếu QAQC)
	delete from  #temp2 a where ifnull(QADocEntry,0)  = 0;
	--- Update thông tin InsLotStock_SSCC
	update a
	set  InsLotStock_SSCC  =  b."U_SSCC",
		 InsLotStock_Type  = b."U_Type"
 
	from #temp2 a
	inner join  "@QC_RESULT_INS_L" b on a.QADocEntry  =  b."DocEntry"
	where ifnull(cast ("U_SSCC" as  varchar(5000)),'') <> '';
	------ Insert dữ liệu bảng tạm đã đánh số dòng (row number ) --> QCRESULT_SSCC
	create local temporary table #QCRESULT_SSCC
	(
		Row_num int,
		QADocEntry int,
		BsDocType varchar(20),
		BsDocEntry varchar(50),
		InsLotStock_SSCC varchar(5000), -- List SSCC line tab InsLotStock
		InsLotStock_Type varchar(15)
	);
	insert into #QCRESULT_SSCC (Row_num, QADocEntry,BsDocType,BsDocEntry,InsLotStock_SSCC,InsLotStock_Type)
	select ROW_NUMBER() OVER (),
			QADocEntry,BsDocType,BsDocEntry,InsLotStock_SSCC,InsLotStock_Type
	from #temp2;

--
	Create local temporary table #SSCCDetail_QAResult
	(
		QADocEntry int,
		--QAResult_SSCC nvarchar(5000),
		Row_num int,
		SSCC_Part nvarchar(50),
		InsLotStock_Type varchar(15)
		
	);
----
Begin
	Declare loopCount int = 0;
	Declare i int = 1;
	Declare QAResult_SSCC nvarchar(5000);
	Declare QADocEntry int;
	Declare InsLotStock_Type varchar(15);
	
	Select Count(*) Into loopCount From #QCRESULT_SSCC;
	       While :i <= :loopCount
				DO
				Select a.InsLotStock_SSCC Into QAResult_SSCC From #QCRESULT_SSCC a Where Row_num = :i;
				Select a.QADocEntry Into QADocEntry From #QCRESULT_SSCC a Where Row_num = :i;
				Select a.InsLotStock_Type Into InsLotStock_Type From #QCRESULT_SSCC a Where Row_num = :i;
				
			 	insert into #SSCCDetail_QAResult ( SSCC_Part, Row_Num)
			 	select  "part",i
			 	from  "SPLITSTRING" (:QAResult_SSCC,',');
				
				--
				Update  a
				set QADocEntry = :QADocEntry,  InsLotStock_Type  =:InsLotStock_Type
				from #SSCCDetail_QAResult a
				where i = row_num;
				
					i := i + 1;
			END WHILE;


End;
---- update InsLotStock_Type cho table #tbl

Update a
set  InsLotStockType  = (select  distinct InsLotStock_Type from  #SSCCDetail_QAResult b where b.SSCC_Part = a.SSCC)
from #tbl a;
-- Update kho thực tế
update a
set WhseCurrent  = b."WhsCode"
from #tbl a
inner join OBBQ b on a.SSCC_Entry = b."SnBMDAbs" and a.ItemCode = b."ItemCode"
and "OnHandQty"  >0;

-- XÓA CÁC DÒNG CÓ BsDocEntry = 0 (theo yêu cầu)
DELETE FROM #tbl x where BsDocEntry = 0;
-- lọc theo tham số QA reult
IF :QAResult = '000' then
		DELETE FROM #tbl x where QAQCEntry is null;
	elseIF :QAResult = '999' then
		DELETE FROM #tbl x where QAQCEntry is not null;
End IF;

-- cập nhật các số docnum của phiếu gốc

update x set DocNumBsEntry	= (select "DocNum" from OWOR where x.BsDocEntry = "DocEntry") from #tbl x where BaseType = 59;
-- Type : 20: GRPO
--update x set BSDOCENTRY	= DOCENTRY from #tbl x where BaseType = 20;
update x set DocNumBsEntry	= (select "DocNum" from OPDN where x.BsDocEntry = "DocEntry") from #tbl x where BaseType = 20;
-- Update UD name
update x set UDName	= (select "Name" from  "@QC_UDCODE"  where "U_Active"  = 'Y' and x.UDCode = "Code") from #tbl x ;


--
select DocEntry,DocNum, SSCC,Qty, ItemCode,ItemName, WhseCode,WhseCurrent,
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
		 IFNULL(:warehouse, '') = ''  OR cast (WhseCurrent as nvarchar) IN (SELECT "DATA" FROM "SPLIT"(:warehouse, ','))
		 
order by ItemCode;

--- Drop table
drop table #tbl;
drop table #tblWH;
drop table #tblITEM;
drop table #LenhRa;
drop table #temp2;
drop table #QCRESULT_SSCC;
drop table #SSCCDetail_QAResult;
end;

