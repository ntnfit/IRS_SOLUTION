-- B1 DEPENDS: AFTER:SP:TmSp_OpportAnalysis_CRD AFTER:SP:TmSp_OpportAnalysis_ITM AFTER:SP:TmSp_OpportAnalysis_SLP AFTER:PT:PROCESS_END

CREATE PROCEDURE Tmsp_OpportAnalysis(in CondText nvarchar(2048),in ObjType int,in CutByObject int,in GetAllOpr nchar) 
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
as 
begin
if (:ObjType = 4) then 
call TmSp_OpportAnalysis_ITM (:CondText, :CutByObject, 0, :GetAllOpr);
elseif (:ObjType = 52) then 
call TmSp_OpportAnalysis_ITM (:CondText, :CutByObject, 1, :GetAllOpr);
elseif (:ObjType = 2) then 
call TmSp_OpportAnalysis_CRD (:CondText, 0, :GetAllOpr);
elseif (:ObjType = 10) then 
call TmSp_OpportAnalysis_CRD (:CondText, 1, :GetAllOpr);
elseif (:ObjType = 53) then 
call TmSp_OpportAnalysis_SLP (:CondText, :GetAllOpr);
end if;
end;


