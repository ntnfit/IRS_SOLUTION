CREATE procedure ILD_LAY_Customer
(
)
as
begin
	select "CardCode","CardName" from OCRD where "CardType" = 'C'  order by "CardCode";
end;

