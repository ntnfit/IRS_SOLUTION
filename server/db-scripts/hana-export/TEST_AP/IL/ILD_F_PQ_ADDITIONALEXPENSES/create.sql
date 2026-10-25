CREATE procedure ILD_F_PQ_AdditionalExpenses (
	in DocID	int
)
as
begin
 
 
Select a."Dscription" , "VatPrcnt","LineTotal", ifnull(a."PriceAfVAT",0) * ifnull(a."Quantity",0) AS  "TotalAmount", "InvntItem"

from  PQT1  a
inner join OPQT b  on a."DocEntry"  = b."DocEntry"
inner join  OITM c on a."ItemCode"  = c."ItemCode"
Where b."DocEntry"=  DocID
 and c."InvntItem"   =  'N';


end;


