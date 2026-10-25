CREATE procedure ILD_F_PQ_TaxDetails (
	in DocID	int
)
as
begin

select "VatGroup", "VatPrcnt",

 max("DocTotal") as "DocTotal",   
sum((ifnull(a."PriceAfVAT",0) * ifnull(a."Quantity",0) ) -  ifnull(a."LineTotal",0)) as "VatAmount"
from  PQT1  a
inner join OPQT b  on a."DocEntry"  = b."DocEntry"
 where b."DocEntry"= DocID
 
 group by   "VatGroup", "VatPrcnt";


end;


