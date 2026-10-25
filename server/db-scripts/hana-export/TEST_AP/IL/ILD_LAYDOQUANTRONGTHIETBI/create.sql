create procedure ILD_LayDoQuanTrongThietBi
as
begin
select distinct "U_CalibrationEvaluation" as "Classification" from   "@CALIBRATIONS_L";

end;

