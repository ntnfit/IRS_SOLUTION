create procedure APZ_EI_GetListWhsCode
as
begin
	select "WhsCode", "WhsName", "Street"
	from OWHS
	where "WhsCode" <> '01';
end;
