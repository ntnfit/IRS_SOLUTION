CREATE procedure APZ_CTA_TaoPhieuQAQC_From_OriginalError
 (
 	in DocEntry nvarchar(10)
 )
as
begin
 
Select 
a."DocEntry",
a."U_ItemCode",
a."U_ItemName",
a."U_Ins_Type",
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
a."U_SoXe",
a."U_AttachFiles",


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
b."U_Stage",
b."U_MinimumFrequency",
b."U_Remark",
b."U_CharType"

from "@QC_RESULT_H" a
left join "@QC_RESULT_CHAR_L" b on a."DocEntry"  =  b."DocEntry"
where cast(a."DocEntry" as nvarchar(10))  =  DocEntry;
		


end;

