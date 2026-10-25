CREATE procedure ILD_LAY_ITEMCODE()
as
begin
	select "ItemCode","ItemName" 
	from OITM 
	where    "frozenFor"  = 'N' AND "validFor" = 'Y'
	order by "ItemCode";
	--"frozenFor" --> Inactive....
 	-- validFor --> Active 
end;

