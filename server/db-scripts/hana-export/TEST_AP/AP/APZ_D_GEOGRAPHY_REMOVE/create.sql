

CREATE PROCEDURE APZ_D_Geography_Remove(in countryCode nvarchar(50))
AS
BEGIN

	--Check country has been used in another function
	if exists(select 1 from APZ_D_OSTR where "CountryCode" = :countryCode)
	then
		select 20600004 as "ErrorCode", 'This country has been used' as "ErrorMessage" from dummy;
		return;
	end if;

	--Delete Geography Details
	delete from APZ_D_GGP1 where "CountryCode" = :countryCode;

	--Delete Geography Header
	delete from APZ_D_OGGP where "CountryCode" = :countryCode;

END



