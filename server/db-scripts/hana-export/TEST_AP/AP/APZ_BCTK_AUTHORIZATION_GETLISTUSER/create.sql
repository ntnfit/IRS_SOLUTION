
CREATE PROCEDURE APZ_BCTK_Authorization_GetListUser

AS
BEGIN

	Select "USER_CODE", "U_NAME" from OUSR where "E_Mail" like '%Enable Integrate CMMS%' order by "USER_CODE";

END



