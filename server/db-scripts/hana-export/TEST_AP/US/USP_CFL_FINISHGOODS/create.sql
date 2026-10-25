CREATE procedure USP_CFL_FINISHGOODS (
)
as
begin
	
	select "ItemCode","ItemName",b."ItmsGrpNam" 
	from "OITM" a, "OITB" b 
	where a."ItemType"='I' and a."ItmsGrpCod"=b."ItmsGrpCod"
	and a."ItemCode" in (select x."Code" from "OITT" x)
	and a."ItemCode" not in (select y."Code" from "ITT1" y)
	;
end

