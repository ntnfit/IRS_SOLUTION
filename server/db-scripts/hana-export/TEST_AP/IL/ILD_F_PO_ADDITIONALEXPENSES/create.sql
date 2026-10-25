CREATE procedure ILD_F_PO_AdditionalExpenses (
	in DocID	int
)
as
begin
 
 
Select a."Dscription" , "VatPrcnt","LineTotal", ifnull(a."PriceAfVAT",0) * ifnull(a."Quantity",0) AS  "TotalAmount", "InvntItem"

from  POR1  a
inner join OPOR b  on a."DocEntry"  = b."DocEntry"
inner join  OITM c on a."ItemCode"  = c."ItemCode"
Where b."DocEntry"=  DocID
	  and ifnull(c."U_IsAdditionFee",0) =1;
 --and c."InvntItem"   =  'N';


end;

