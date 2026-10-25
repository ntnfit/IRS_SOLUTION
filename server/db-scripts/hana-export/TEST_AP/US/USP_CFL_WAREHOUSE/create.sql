CREATE procedure USP_CFL_WAREHOUSE (
)
as
begin
	select "WhsCode","WhsName" from "OWHS" ORDER BY "WhsCode";

end

