
create procedure APZ_ProjectConfirm_GetNo()
as
begin
declare IdMax int=0;
select MAX("DocEntry")+1 into IdMax from APZ_OPJC ;
select IdMax from dummy;
end;




