
CREATE  procedure APZ_Freight_GetMax
as
begin
	select top 1 "ExpnsCode" from OEXD ORDER by "ExpnsCode" DESC;
end



