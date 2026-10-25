CREATE procedure ILD_R_QAKetQuaTheoChiTieu_Old
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
	--Thông tin về phiếu QAQC Result
		DocEntry_QCResult int,
		ItemCode nvarchar(50),
		ItemName nvarchar(150),
		Batch nvarchar(50),
	-- Thông tin chỉ tiêu
		DocEntry_Char int,
		Code_Char nvarchar(50),
		Name_Char nvarchar(150),
		Ins_StageCode nvarchar(50),
		InspectionPoint nvarchar(150),
		GroupCode nvarchar(50),
		Weighing nvarchar(20),
		MinimumFrequency nvarchar(150),
		Method nvarchar(50),
		UoM nvarchar(20),
		Target_Number_1 decimal(19,6),
		Target_Number_2 decimal(19,6),
	-- Thông tin kết quả
		Actual_Boolean nvarchar(5),
		Actual_Number decimal(19,6),
		
		QAResult nvarchar(5), --"Evaluation (OK/ NG)" 
		
		QCDate nvarchar(10), --"Date of creation"
		QCHour nvarchar(5),
		
		----
		GRPO		nvarchar(10),
		PROID		nvarchar(10),
		U_Ins_StageCode	nvarchar(25)
);

insert into #tbl (DocEntry_QCResult,
					ItemCode,
					ItemName,
					Batch,
					-- Thông tin chỉ tiêu
					DocEntry_Char,
					Code_Char,
					Name_Char,
					Ins_StageCode,
					InspectionPoint,
					GroupCode,
					Weighing,
					MinimumFrequency,
					Method,
					UoM,
					Target_Number_1,
					Target_Number_2,
				-- Thông tin kết quả
					Actual_Boolean,
					Actual_Number,
					
					QAResult,
					QCDate,
					QCHour,
					
					----
					GRPO,
					PROID,
					U_Ins_StageCode

				)
select  a."DocEntry" as "DocEntry_QCResult",
		a."U_ItemCode",
		a."U_ItemName",
		a."U_Batch",
		-- Thông tin chỉ tiêu
		c."DocEntry" as "DocEntry_Char",
		b."U_Code" as "Code_Char",
		b."U_Name" as "Name_Char",
		a."U_Ins_StageCode",
		c."U_InspectionPoint",
		d."Name" as "Group_Char",
		b."U_Weighing",
		c."U_MinimumFrequency",
		b."U_Method",
		b."U_UoM",
		b."U_Target_Number_1",
		b."U_Target_Number_2",
		-- Thông tin kết quả
		b."U_Actual_Boolean",
		b."U_Actual_Number",
					
		b."U_QAResult" as QAResult,
		TO_VARCHAR(a."U_QCDate",'DD/MM/YYYY') as "QCDate",
		
		case when LENGTH(a."U_QCHour")  = 3 then  '0'||left(a."U_QCHour",1) || ':' || right(a."U_QCHour",2) 
		 when LENGTH(a."U_QCHour")  = 4 then   left("U_QCHour",2) || ':' || right(a."U_QCHour",2)  
	 	else  '' end  as "QC Hour"
	 	
	 	-----
	 	,case when a."U_BaseDocType" like '20' then a."U_BaseDocType" end
	 	,case when a."U_BaseDocType" like '202' then a."U_BaseDocType" end
		,a."U_Ins_StageCode"
from "@QC_RESULT_H" a
inner join  "@QC_RESULT_CHAR_L" b on a."DocEntry"  = b."DocEntry"
left join  "@QC_CHAR" c on b."U_Code"  =  c."U_Code"
left join "@QC_CHAR_GRP" d on d."Code"  = b."U_GroupCode"
--left join "@QC_RESULT_H" e on e.

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

select 
		--Thông tin về phiếu QAQC Result
		DocEntry_QCResult "Inspection Lot",
		ItemCode "Material",
		ItemName "Material Description",
		Batch  "Batch",
		-- Thông tin chỉ tiêu
		DocEntry_Char  "Master Insp. Char ID",
		Code_Char "Master Insp. Char",
		Ins_StageCode "Inspection stage (type)",
		InspectionPoint "Inspection point",
		GroupCode "Characteristic classification",
		Weighing "Weighing",
		MinimumFrequency "Minimum Frequency",
		Method "Inspection Method",
		Name_Char "Inspection Characteristic",
		
		UoM "Unit of Measure",
		Target_Number_1 "Lower Tol Limit",
		Target_Number_2 "Upper Tol Limit",
	-- Thông tin kết quả
		Actual_Number "QT Result",
		
		QAResult "Evaluation (OK/ NG)", --"Evaluation (OK/ NG)" 
		
		QCDate "Date of creation", --"Date of creation"
		QCHour "Hour of creation",
		
		
		---
		GRPO "GRPO",
		PROID "PROID",
		U_Ins_StageCode "Inspection stage" 


from #tbl;
drop table #tbl;

 


end;

