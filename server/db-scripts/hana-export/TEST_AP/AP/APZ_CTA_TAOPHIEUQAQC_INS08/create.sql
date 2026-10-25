CREATE procedure APZ_CTA_TaoPhieuQAQC_Ins08
 (
 	in DocEntry nvarchar(10)
 )
as
begin
 
Select 
a."DocEntry",
a."U_ItemCode",
a."U_ItemName",
'08' as  "U_Ins_Type",
a."U_Ins_StageCode",
a."U_QCType",
a."U_TemplateCode",
a."U_UDCode",
a."U_ApprovedNote",
a."U_QCDate",
a."U_QCHour",
a."U_BPCode",
a."U_BPName",
a."U_Batch",
a."U_SSCC",
 
a."U_BaseDocType",
a."U_BaseDocEntry",
a."U_Warehouse",
a."U_MethodGetSamp",

0 as "U_QuantityTotal",

a."U_QuantityOfSample",
a."U_TotalOK",
a."U_TotalNG",
a."U_DocStatus",
a."U_empIDCreated",
a."U_empNameCreated",
a."U_Ins_Base",
a."Remark",
---Line
b."U_Code",
b."U_Name",
b."U_GroupCode",
b."U_Method",
b."U_Operator",
b."U_ValueType",
--b."U_Target_Boolean",
b."U_Target_Number_1",
b."U_Target_Number_2",
b."U_Actual_Boolean",
b."U_Actual_Number",
b."U_QAResult",
b."U_UoM",
b."U_Weighing",
b."U_ActionUponDeviation",
-- QAQC Results Ins Lot Stock


c."U_Type",
case when c."U_Type"  = 'RE-EXAMINATION'  then c."U_Status"  end as "U_Status",
case when c."U_Type"  = 'RE-EXAMINATION'  then c."U_Quantity"  end as "U_Quantity",
case when c."U_Type"  = 'RE-EXAMINATION'  then c."U_SSCC"  end as "U_SSCC"

from "@QC_RESULT_H" a
left join "@QC_RESULT_CHAR_L" b on a."DocEntry"  =  b."DocEntry"
left join "@QC_RESULT_INS_L" c on a."DocEntry"  =  c."DocEntry"
--left join (select "U_Quantity","DocEntry" from  "@QC_RESULT_INS_L" e where "U_Type"  = 'RE-EXAMINATION' and e."DocEntry"  =  :DocEntry)e on  1 =1
where cast(a."DocEntry" as nvarchar(10))  =  DocEntry
		and c."U_Type"  = 'RE-EXAMINATION';
		
--and c."U_Type"  = 'RE-EXAMINATION';


end;

