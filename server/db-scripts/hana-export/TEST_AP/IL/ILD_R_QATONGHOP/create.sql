CREATE procedure ILD_R_QATongHop
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
	DocEntry smallint,
	ItemCode nvarchar(50),
	ItemName nvarchar(150),
	InspectionStage nvarchar(50),
	InspectionType nvarchar(50),
	QCType nvarchar(20),
	UDCode nvarchar(50),
	QCDate date,
	QCHour nvarchar(10),
	Batch nvarchar(50),
	
	Total_Release decimal(19,6),
	SSCC_Release ntext,
	Status_Release nvarchar (10),
	
	Total_Rework decimal(19,6),
	SSCC_Rework ntext,
	Status_Rework nvarchar (10),
	
	Total_Destruction decimal(19,6),
	SSCC_Destruction nvarchar(50),
	Status_Destruction nvarchar (10),
	
	Total_Re_Exam decimal(19,6),
	SSCC_Re_Exam ntext,
	Status_Re_Exam nvarchar (50),
	
	Inspection_Base smallint
	
	
);
	insert into #tbl (	DocEntry,
						ItemCode,
						ItemName,
						InspectionStage,
						InspectionType,
						QCType,
						UDCode,
						QCDate,
						QCHour,
						Batch,
						
						Total_Release,
						SSCC_Release,
						Status_Release,
						
						Total_Rework,
						SSCC_Rework,
						Status_Rework,
						
						Total_Destruction,
						SSCC_Destruction,
						Status_Destruction,
						
						Total_Re_Exam,
						SSCC_Re_Exam,
						Status_Re_Exam,
						
						Inspection_Base)
	
	select a."DocEntry","U_ItemCode" as "ItemCode",  "U_ItemName" as "ItemName",
		"U_Ins_StageCode" as "Inspection stage",
		"U_Ins_Type" as  "Inspection Type",
		"U_QCType" as "QC Type",
		"U_UDCode" as "UD Code",
		"U_QCDate" as "QC Date",
		
		case when LENGTH("U_QCHour")  = 3 then  '0'||left("U_QCHour",1) || ':' || right("U_QCHour",2) 
		 when LENGTH("U_QCHour")  = 4 then   left("U_QCHour",2) || ':' || right("U_QCHour",2)  
	 	else  '' end  as "QC Hour",
		
		"U_Batch" as "Batch",
		
		release."U_Quantity" as "Total Release",
		release."U_SSCC" as "SSCC Release",
		release."U_Status" as  "Status Release",
		--Rework
		rework."U_Quantity" as "Total Rework",
		rework."U_SSCC" as "SSCC Rework",
		rework."U_Status" as  "Status Rework",
		
		--Destruction
		Destruction."U_Quantity" as "Total Destruction",
		Destruction."U_SSCC" as "SSCC Destruction",
		Destruction."U_Status" as  "Status Destruction",
		--Re_Examination
		Re_Examination."U_Quantity" as "Total Re-Exam",
		Re_Examination."U_SSCC" as "SSCC Re-Exam",
		Re_Examination."U_Status" as  "Status Re-Exam",
		
		"U_Ins_Base"
		from  "@QC_RESULT_H" a
		left join  (select "DocEntry","U_Status","U_Quantity","U_SSCC" from "@QC_RESULT_INS_L" where  "U_Type" = 'RELEASE' ) as release 
			on release."DocEntry" =  a."DocEntry"
			
		left join  (select "DocEntry","U_Status","U_Quantity","U_SSCC" from "@QC_RESULT_INS_L" where  "U_Type" = 'REWORK' ) as rework 
			on rework."DocEntry" =  a."DocEntry"
			
		left join  (select "DocEntry","U_Status","U_Quantity","U_SSCC" from "@QC_RESULT_INS_L" where  "U_Type" = 'DESTRUCTION' ) as Destruction 
			on Destruction."DocEntry" =  a."DocEntry"
			
		left join  (select "DocEntry","U_Status","U_Quantity","U_SSCC" from "@QC_RESULT_INS_L" where  "U_Type" = 'RE-EXAMINATION' ) as Re_Examination 
			on Re_Examination."DocEntry" =  a."DocEntry"

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
)

;
				
			

delete from #tbl where DocEntry  in (select  Inspection_Base from #tbl);

select * from #tbl order by ItemCode, Batch,QCDate;
drop table #tbl;
End;

