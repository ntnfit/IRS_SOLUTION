CREATE procedure ILD_LAY_WAREHOUSE (
)
as
begin
	select "WhsCode","WhsName" from "OWHS" ORDER BY "WhsCode";

end

