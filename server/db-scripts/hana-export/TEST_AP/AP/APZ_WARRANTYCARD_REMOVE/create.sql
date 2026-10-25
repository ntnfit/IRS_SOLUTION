
create procedure APZ_WarrantyCard_Remove
(
in Id int
)
as
begin
Delete from APZ_OINS where "WrntyID"=:Id ;
Delete from APZ_INS1 where "WrntyID"=:Id;

select 0, '' from dummy;
end;




