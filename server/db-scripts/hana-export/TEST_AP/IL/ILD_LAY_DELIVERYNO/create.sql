CREATE procedure ILD_LAY_DeliveryNo
(
	in FromDate date,
	in ToDate date,
	in Customer varchar(5000)
)
as
begin
	select "DocNum","CardCode","CardName","DocDate" from ODLN 
	where "DocDate" between  FromDate and ToDate and "CANCELED"  = 'N'
		  and
			(
					ifnull(:Customer, '') = '' 
					or cast ("CardCode" as nvarchar) in (select "DATA" from split(:Customer ,','))
			)
	
	
	order by "DocNum";
end;

