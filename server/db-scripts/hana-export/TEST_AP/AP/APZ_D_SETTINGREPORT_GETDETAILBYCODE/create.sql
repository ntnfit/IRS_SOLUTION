

CREATE PROCEDURE APZ_D_SettingReport_GetDetailByCode(in code nvarchar(250) default '')
AS
BEGIN

	select * from APZ_UDR1 where "Code" = :code;
	select * from APZ_UDR2 where "Code" = :code;

END




