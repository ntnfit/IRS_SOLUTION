

CREATE PROCEDURE APZ_D_Geography_GetData(
	in countryCode varchar(50), 
	in location varchar(50), 
	in status varchar(1)
)
AS
BEGIN

	if (:countryCode <> '' or :location <> '' or :status <> '')
	then
		--Search data
		select * from APZ_D_OGGP 
		where ("CountryCode" = '' or "CountryCode" like :countryCode + '%') and ("Location" = '' or "Location" like :location + '%') and ("Status" = '' or "Status" like :status + '%');
	else
		--Get Geography data		
		select * from APZ_D_OGGP;
	end if;
END




