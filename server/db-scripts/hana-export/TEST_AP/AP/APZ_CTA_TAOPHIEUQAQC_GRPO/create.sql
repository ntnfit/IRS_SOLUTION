CREATE procedure APZ_CTA_TaoPhieuQAQC_GRPO
 (
 	in DocEntry nvarchar(20) -- DocEntry GRPO đang chọn để làm phiếu QAQC Result
 )
as
begin
select Nguon1.*,b."U_Ins_Type" as  "InspectionType"
From 
(
 select 
 		
		c."U_QCTemplate" as "PlanCode", --Template Code
		'I' as "QCType",
		a."CardCode" as "BPCode",
		b."ItemCode" as "ItemCode",
		b."Dscription" as "ItemName",
		'20' as "BaseDocType",
		a."DocEntry" as "BaseDocEntry",
		b."Quantity" as "QuantityTotal",
		a."U_ContNo" as "Soxe"
 from OPDN a
 left join PDN1 b on a."DocEntry"  = b."DocEntry"
 inner join OITM c on c."ItemCode"  =  b."ItemCode"
 where cast(a."DocEntry" as nvarchar(20))= DocEntry) as Nguon1
 left join (select "U_Code","U_Ins_Type" from "@QC_TEMPLATE_H")b 
 			on Nguon1."PlanCode"= b."U_Code";


end;

