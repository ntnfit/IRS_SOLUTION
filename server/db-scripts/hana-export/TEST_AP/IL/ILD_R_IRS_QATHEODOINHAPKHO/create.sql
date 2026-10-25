CREATE procedure ILD_R_IRS_QATheoDoiNhapKho
(
	in Batch nvarchar(500) --ILD_Lay_BatchQA
)
As
Begin

--- Table chứa dữ liệu từ phiếu QAQC Result
Create local temporary table #tbl_QAQC
(
	QADocEntry int,
	LineID int,
	ItemCode nvarchar(50),
	ItemName nvarchar(150),
	Ins_Lot_Stock_Type nvarchar(50), -- Release, rework,..
	QAResult_SSCC nvarchar(5000),
	Inspection_Base int
	
);
--b1: Đổ dữ liệu phiếu QAQC
insert into #tbl_QAQC (QADocEntry,
					LineID,
					ItemCode,
					ItemName,
					Ins_Lot_Stock_Type,					
					QAResult_SSCC,
					Inspection_Base
					)
select a."DocEntry",
		b."LineId",
		"U_ItemCode" as "ItemCode",
		"U_ItemName" as "ItemName",
		 b."U_Type" as "Ins_Type",
		b."U_SSCC" as "QAResult_SSCC",
		a."U_Ins_Base"
from  "@QC_RESULT_H" a
left join "@QC_RESULT_INS_L" b on   a."DocEntry" = b."DocEntry"
order by  "DocEntry";

-- b2: Xóa bỏ những phiếu bị kiểm tra lại: chỉ lấy những phiếu cuối cùng
delete from #tbl_QAQC where QADocEntry  in (select  Inspection_Base from #tbl_QAQC);

-- b3: Lấy dữ liệu SSCC theo từng type và chuẩn bị cắt: Release, rework,... SSCC có dạng: sscc01,sscc02 (cách nhau dấu phẩy)
create local temporary table #temp1
(
	Row_num int,
	QADocEntry int,
	LineID int,
	ItemCode nvarchar(50),
	ItemName nvarchar(150),
	Ins_Lot_Stock_Type nvarchar(50), -- Release, rework,..
	QAResult_SSCC nvarchar(5000)
);

insert into #temp1 (Row_num, QADocEntry, LineID,Ins_Lot_Stock_Type,  QAResult_SSCC, ItemCode,ItemName)
select distinct ROW_NUMBER() OVER () AS row_num, a.QADocEntry, a.LineID,
					a.Ins_Lot_Stock_Type,
				   cast (a.QAResult_SSCC as nvarchar(5000)),
				   a.ItemCode,
				   a.ItemName
								
from #tbl_QAQC a
where a.Ins_Lot_Stock_Type <> 'RE-EXAMINATION' and ifnull(cast (a.QAResult_SSCC as nvarchar(5000)),'') <> '';



------b4 :For để cắt chuỗi SSCC QA Result ở bảng temp1 thành table SPLITSTRING
Create local temporary table #SSCC_SSCCDetail_QAResult
(
	QADocEntry int,
	LineID int,
	QAResult_SSCC nvarchar(5000),
	Row_num int,
	SSCC_Part nvarchar(50)
	
);

Begin
	Declare loopCount int = 0;
	Declare i int = 1;
	Declare QAResult_SSCC nvarchar(5000);
	Declare QADocEntry int;
	Declare LineID int;
	
	Select Count(*) Into loopCount From #temp1;
	       While :i <= :loopCount
				DO
				Select a.QAResult_SSCC Into QAResult_SSCC From #temp1 a Where Row_num = :i;
				Select a.QADocEntry Into QADocEntry From #temp1 a Where Row_num = :i;
				Select a.LineID Into LineID From #temp1 a Where Row_num = :i;
				
			 	insert into #SSCC_SSCCDetail_QAResult ( SSCC_Part, Row_Num)
			 	select  "part",i
			 	from  "SPLITSTRING" (:QAResult_SSCC,',');
				
				--
				Update  a
				set QADocEntry = :QADocEntry, LineID = :LineID , QAResult_SSCC = :QAResult_SSCC
				from #SSCC_SSCCDetail_QAResult a
				where i = row_num;
				
					i := i + 1;
			END WHILE;


End;


--- b5: Lấy dữ liệu SSCC ĐÃ inventory transfer
create local temporary table #SSCC_SSCCDetail_Transfered
(	
	QADocEntry int,

	SSCCTransfered nvarchar(5000),
	QtyTransfered decimal(19,6)
);
-- Lấy dữ liệu: SSCC đã tranfer
insert into #SSCC_SSCCDetail_Transfered (SSCCTransfered, QtyTransfered, QADocEntry)
select distinct "BatchNum", "Quantity", b.QADocEntry
from IBT1 a
inner join  #SSCC_SSCCDetail_QAResult b on a."BatchNum"  = b.SSCC_Part
where "BaseType"  = 67 and "Direction"  =  1;


-- Direction: 1 (in la warehouse From); 0 (out la warehouse To)

Create local temporary table #tbl
(
	QADocEntry int,
	Ins_Lot_Stock_Type nvarchar(50),
	ItemCode nvarchar(50),
	ItemName nvarchar(150),
	Batch nvarchar(50),
	QAResult_SSCC nvarchar(5000),
	Qty_QAResult decimal(19,6),
	-- Đã transfer
	Qty_TransF decimal(19,6),
	-- Chưa transfer
	Qty_Not_TransF decimal(19,6),
	FromWhs nvarchar(50),
	ToWhs nvarchar(50)
	
);
insert into #tbl (QADocEntry,
					Ins_Lot_Stock_Type,
					ItemCode,
					ItemName,
					
					QAResult_SSCC,
					Qty_QAResult)
							
select  a.QADocEntry "QADocEntry",a.Ins_Lot_Stock_Type "Ins_Lot_Stock_Type",
		a.ItemCode "ItemCode",
		a.ItemName "ItemCode",
		QA.SSCC_Part "QAResult_SSCC",
		0 "Qty_QAResult"

from #temp1 a
left join  #SSCC_SSCCDetail_QAResult QA on  a.QADocEntry = QA.QADocEntry and  a.LineID = QA.LineID;
  --Yes: Đã transfer

--- b6: Update dữ liệu transfer

update a
set a.Qty_TransF  =  b.QtyTransfered
from #tbl a
inner join #SSCC_SSCCDetail_Transfered b on a.QADocEntry  = b.QADocEntry
where a.QAResult_SSCC  = b.SSCCTransfered;

-- Update thông tin Batch
update a
set Batch  =  b."MnfSerial" 
from #tbl a 
inner join OBTN b on a.QAResult_SSCC = b."DistNumber";

-- Update thông tin Số lượng Qty_QAResult: GRPO nhập nguyên vật liệu
Update a
set Qty_QAResult  = c."Quantity"
from  #tbl a 
inner join  "@QC_RESULT_H" b on a.QADocEntry  = b."DocEntry"
inner join  IBT1 c on  a.QAResult_SSCC = c."BatchNum" and c."BaseType" = 20
where ifnull(b."U_BaseDocType",'')  =  '20';
 
-- Update thông tin Số lượng Qty_QAResult: Receipt from productio 
Update a
set Qty_QAResult  = c."Quantity"
from  #tbl a 
inner join  "@QC_RESULT_H" b on a.QADocEntry  = b."DocEntry"
inner join  OIBT c on  a.QAResult_SSCC = c."BatchNum" and c."BaseType" = 59
where ifnull(b."U_BaseDocType",'')  =  '59';

-- Update Qty_Not_TransF
update a
set a.Qty_Not_TransF   =  Qty_QAResult - Qty_TransF
from #tbl a;
----
select  QADocEntry,
		Ins_Lot_Stock_Type,
		ItemCode,
		ItemName,
		Batch,			
		QAResult_SSCC,
		 
		Qty_QAResult,
		Qty_TransF,
		Qty_Not_TransF,
		
		FromWhs,
		ToWhs
from #tbl a
where 
 
(
					ifnull(:Batch, '') = '' 
					or cast (Batch as nvarchar) in (select "DATA" from split(:Batch,','))
)

;

drop table #tbl_QAQC;
drop table #temp1;
drop table #SSCC_SSCCDetail_QAResult;
drop table #SSCC_SSCCDetail_Transfered;

drop table #tbl;
end;

