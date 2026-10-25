
create procedure APZ_WarrantyCard_GetById
(
in id int
)
as
begin
select a.*, b."ContractCycle",b."CycleUnit",b."TolerantTime",b."TolerantUnit" 
from APZ_INS1 a left join APZ_OCTT b on a."ContractTemplateNum"=b."ContractTemplateNum" where "WrntyID"=:Id;
end;




