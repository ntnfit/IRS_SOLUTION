create procedure TRACEABILITY_SOLUTION_NEW_Items()
as
begin
	select "ItemCode", "ItemName" from OITM where "ItmsGrpCod" in (120,121);
end

