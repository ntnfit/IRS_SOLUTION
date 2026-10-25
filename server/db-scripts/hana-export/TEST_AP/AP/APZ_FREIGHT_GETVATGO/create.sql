
CREATE  procedure APZ_Freight_GetVATGO
as
begin
	select "Code","Name" from OVTG WHERE "Category" = 'O' and "Inactive" = 'N';
end;



