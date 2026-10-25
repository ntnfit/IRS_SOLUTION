
create procedure APZ_ProjectConfirm_GetLineById
(
in Id int
)
as
begin
select * from APZ_PJC1 where "DocEntry"=:Id;
end;




