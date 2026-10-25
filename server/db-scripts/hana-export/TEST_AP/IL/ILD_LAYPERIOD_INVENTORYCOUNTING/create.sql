CREATE procedure ILD_LayPeriod_InventoryCounting
as
begin
select "Code","Name" 
from  OFPR a inner join OACP b on a."Category" = b."PeriodCat" 
where a."Category" = year(CURRENT_TIMESTAMP) or  a."Category" = year(CURRENT_TIMESTAMP) - 1 
order by "Code";
end;

