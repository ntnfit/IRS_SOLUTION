CREATE procedure ILD_R_QAChiTiet
(
	FromDate date,  --QC date
	ToDate date,  -- QC date
	ItemCode nvarchar(250), --ILD_LayItemQA
	InspectionStage nvarchar(50), --ILD_Lay_InspectionStageQA
	InspectionType nvarchar(50), --ILD_Lay_InspectionTypeQA
	QCType nvarchar(20), -- ILD_LAY_QCTYPEQA  
	UDCode nvarchar(50), -- ILD_Lay_UDCODEQA
	Batch nvarchar(50), --ILD_Lay_BatchQA
	VendorCode nvarchar(50) --ILD_Lay_VendorQA
)
as
Begin
create local temporary table #tbl
(
	QC_DocEntry smallint,
	ItemCode nvarchar(50),
	ItemName nvarchar(150),
	FromWareHouse nvarchar(50),
	InspectionStage nvarchar(50),
	InspectionType nvarchar(50),
	QCType nvarchar(50),
	UDCode nvarchar(50),
	QCDate date,
	QCHour nvarchar(10),
	Batch nvarchar(50),
	-- Line
	Ins_Lot_Stock_Type nvarchar(15),
	TotalQty decimal(19,6),
	SSCC ntext,
	-----	
	Status nvarchar(50),
	
	BaseType nvarchar(15),  -- 20: Goods Receipt PO
	GRPOID	 int,
	ReceiptFPID int,  -- Receipt From production  :  Goods Receipt	DocEntry	59
	
		
	Inspection_Base int
);
	insert into #tbl (	QC_DocEntry,
						ItemCode,
						ItemName,
						FromWareHouse,
						InspectionStage,
						InspectionType,
						QCType,
						UDCode,
						QCDate,
						QCHour,
						Batch,
						
					 	Ins_Lot_Stock_Type,
						SSCC,
						TotalQty,
						Status,
						Inspection_Base,
						
						BaseType,
						GRPOID,
						ReceiptFPID
						)
	
		select a."DocEntry","U_ItemCode" as "ItemCode",  "U_ItemName" as "ItemName",
		'' as "FromWareHouse",
		"U_Ins_StageCode" as "Inspection stage",
		"U_Ins_Type" as  "Inspection Type",
		"U_QCType" as "QC Type",
		"U_UDCode" as "UD Code",
		"U_QCDate" as "QC Date",		
		case when LENGTH("U_QCHour")  = 3 then  '0'||left("U_QCHour",1) || ':' || right("U_QCHour",2) 
		 when LENGTH("U_QCHour")  = 4 then   left("U_QCHour",2) || ':' || right("U_QCHour",2)  
	 	else  '' end  as "QC Hour",

		"U_Batch" as "Batch",
		
		b."U_Type" as "Ins_Type",
		b."U_SSCC" as "SSCC",
		b."U_Quantity" as "Quantity",
		b."U_Status" as  "Status",
		
		a."U_Ins_Base",
		a."U_BaseDocType",
		case when ifnull(a."U_BaseDocEntry",0) = 20 then a."U_BaseDocEntry" end as "GRPOID",
		
		case when ifnull(a."U_BaseDocEntry",0) = 59 then a."U_BaseDocEntry" end as "ReceiptFPID"
		from  "@QC_RESULT_H" a
		left join "@QC_RESULT_INS_L" b on   a."DocEntry" = b."DocEntry"

where a."U_QCDate" between  Fromdate and Todate
and
(
					ifnull(:ItemCode, '') = '' 
					or cast (a."U_ItemCode" as nvarchar) in (select "DATA" from split(:ItemCode,','))
)

and
(
					ifnull(:InspectionStage, '') = '' 
					or cast (a."U_Ins_StageCode" as nvarchar) in (select "DATA" from split(:InspectionStage ,','))
)
and
(
					ifnull(:InspectionType, '') = '' 
					or cast (a."U_Ins_Type" as nvarchar) in (select "DATA" from split(:InspectionType,','))
)

and
(
					ifnull(:QCType, '') = '' 
					or cast (a."U_QCType" as nvarchar) in (select "DATA" from split(:QCType,','))
)

and
(
					ifnull(:UDCode, '') = '' 
					or cast (a."U_UDCode" as nvarchar) in (select "DATA" from split(:UDCode,','))
)
and
(
					ifnull(:VendorCode , '') = '' 
					or cast (a."U_BPCode" as nvarchar) in (select "DATA" from split(:VendorCode,','))
);				
			

--delete from #tbl where QC_DocEntry  in (select  Inspection_Base from #tbl);

/*
-- Update thông tin SSCC và Số lượng ĐÃ tranfer 
update  a
set a.QtyTransferred  = b."Quantity_Tranfer", a.SSCCTransferred  =  b."SSCC_Transferred"
from #tbl a
inner join  (select "ItemCode" ,sum("Quantity") as "Quantity_Tranfer", STRING_AGG("BatchNum", ',' Order by  "BatchNum")  as  "SSCC_Transferred"
			  from  IBT1
			  where  "BaseType"  = 67 group by  "ItemCode" ) b on 1=1
where a.ItemCode = b."ItemCode" and b."SSCC_Transferred"  in ( replace(a.SSCC, ',' ,''',''')) ;
-- Qty Not Transferred
Update  a
set  QtyNotTransferred =  a.TotalQty -  ifnull(a.QtyTransferred,0)
from  #tbl a;
--- Lấy dữ liệu chưa Tranfer

*/
---
select QC_DocEntry "QC_DocEntry",
		ItemCode "ItemCode",
		ItemName "ItemName",
		FromWareHouse  "FromWareHouse",
		InspectionStage "Ins_Stage",
		InspectionType "Ins_Type",
		QCType "QCType",
		UDCode "UDCode",
		QCDate "QCDate",
		QCHour "QCHour",
		Batch "Batch",
-- Line
		Ins_Lot_Stock_Type "Ins_Lot_Stock_Type",
		TotalQty "TotalQty",
		SSCC "SSCC",
		Status "Status",
		Inspection_Base "Inspection_Base",
		
		
		BaseType "BaseType",
		GRPOID "GRPOID",
		ReceiptFPID "ReceiptFPID"
from #tbl order by ItemCode,QC_DocEntry, Batch,QCDate;
drop table #tbl;
End;

