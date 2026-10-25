CREATE procedure APZ_EI_GetListCustomer
as
begin
	select a."CardCode", a."CardName", a."Address", b."GroupCode", b."GroupName"
	from OCRD a left join OCRG b on a."GroupCode" = b."GroupCode"
	where a."CardType" = 'C';
end;
