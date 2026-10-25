create procedure ILD_Lay_InspectionStageQA
as
begin
-- Công đoạn
select "Code","Name" from "@QC_INS_STAGE" order by "U_Priority" ;

end;

