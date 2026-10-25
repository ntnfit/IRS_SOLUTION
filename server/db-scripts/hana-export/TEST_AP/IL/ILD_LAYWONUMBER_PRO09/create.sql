CREATE procedure ILD_LayWONumber_Pro09
(
)
as
begin
	select "DocNum"  
	from  OWOR 
	order by "DocNum";
end
