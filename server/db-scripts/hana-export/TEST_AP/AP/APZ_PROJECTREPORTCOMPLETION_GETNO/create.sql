
create procedure APZ_ProjectReportCompletion_GetNo()
as
begin
declare IdMax int=0;
select MAX("DocEntry")+1 into IdMax from APZ_ORTC ;
select IdMax from dummy;
end;




