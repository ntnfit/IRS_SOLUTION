
CREATE  procedure APZ_Freight_GetVATGI
as
begin
	select "Code","Name" from OVTG WHERE "Category" = 'I' and "Inactive" = 'N';
end;



