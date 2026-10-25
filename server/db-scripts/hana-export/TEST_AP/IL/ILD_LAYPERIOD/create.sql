CREATE procedure ILD_LayPeriod
(
	in Year_Period int
)
as
begin
	select "Code","Name" 
	from  OFPR a inner join OACP b on a."Category" = b."PeriodCat" 
	where a."Category" =   :Year_Period
	order by "Code";
end

