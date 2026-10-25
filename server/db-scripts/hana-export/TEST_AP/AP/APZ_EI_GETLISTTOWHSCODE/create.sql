create procedure APZ_EI_GetListToWhsCode
as
begin
	select "WhsCode", "WhsName", "Street"
	from OWHS
	where "WhsCode" like 'O%';
end;
