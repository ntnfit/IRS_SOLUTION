

CREATE PROCEDURE APZ_D_Geography_CheckRemoveDetail(in code varchar(50))
AS
BEGIN

	declare message nvarchar(254) = 'Cannot remove this item because item has been used';
	declare objType int = 1470000158;

	if exists(select 1 from APZ_D_OGGP a inner join APZ_D_OSTR b on a."CountryCode" = b."CountryCode" where a."CountryCode" = :code)
	then
		select :objType as "ErrorCode", :message as "ErrorMessage" from dummy;
	end if;

	if exists(select 1 from APZ_D_GGP1 a inner join APZ_D_STR1 b on a."Code" = b."GgpCode" where a."Code" = :code)
	then
		select :objType as "ErrorCode", :message as "ErrorMessage" from dummy;
	end if;

END




