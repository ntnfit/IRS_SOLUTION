
create procedure APZ_MRP_GETWAREHOUSE(in branch int default -1)
as
begin
	select "WhsCode", "WhsName" from OWHS where Ifnull("Locked",'N')='N' and (:branch = 0 or ifnull("BPLid",-1) = :branch);
end;




