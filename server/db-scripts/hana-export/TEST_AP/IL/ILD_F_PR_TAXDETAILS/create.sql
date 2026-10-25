CREATE procedure ILD_F_PR_TaxDetails (
	in DocID	int
)
as
begin

select "VatGroup", "VatPrcnt",
 --max("DocTotal") as "DocTotal",   
sum(case when a."Currency" = 'VND' then a."PriceAfVAT"  * "Quantity"   else  a."GTotal" end )as "DocTotal", 
--sum((ifnull(a."PriceAfVAT",0) * ifnull(a."Quantity",0) ) -  ifnull(a."LineTotal",0)) as "VatAmount"
sum(	case when  a."VatPrcnt"  = 0 then 0 
			 when   a."VatPrcnt"  >  0 and a."Currency" = 'VND' then (a."PriceAfVAT"  *  a."Quantity") * ( (a."VatPrcnt")/100)
		     else a."VatSum" end) as  "VatAmount"
from  PRQ1  a
inner join OPRQ b  on a."DocEntry"  = b."DocEntry"
 where b."DocEntry"=  :DocID and  "VatPrcnt">0
 
 group by   "VatGroup", "VatPrcnt";


end;

