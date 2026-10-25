create procedure ILD_Lay_InspectionTypeQA
as
begin
-- Loai phiếu
select * from "@QC_INS_TYPE" order by "Code";

end;

