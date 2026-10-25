create procedure ILD_LayYear_Period
as
begin
	select distinct "Category" from OFPR 
	order by "Category";
end

