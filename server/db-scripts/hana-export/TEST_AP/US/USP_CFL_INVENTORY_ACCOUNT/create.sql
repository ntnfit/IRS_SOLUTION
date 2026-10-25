CREATE procedure USP_CFL_INVENTORY_ACCOUNT (
)
as
begin
	select "AcctCode","AcctName" from "OACT" 
	where "Postable"='Y'
	and ("AcctCode" in (select "BalInvntAc" from "OITB")
		or 
		"AcctCode" in (select "BalInvntAc" from "OWHS")
	) 
	ORDER BY "AcctCode";
end

