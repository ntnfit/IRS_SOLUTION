CREATE procedure ILD_LAY_ITEMGROUP (
)
as
begin
	select 0 as Code,'All' as Name from dummy union
	select "ItmsGrpCod","ItmsGrpNam" from "OITB";
	
end

