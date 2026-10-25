
create procedure APZ_ServiceLevelAgreement_GetById
(
in id int
)
as
begin
select * from APZ_SLA1 where "SLAID"=:id;
select * from APZ_SLA2 where "SLAID"=:id;
end;




