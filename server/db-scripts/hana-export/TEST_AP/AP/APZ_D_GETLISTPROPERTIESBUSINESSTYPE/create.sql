

CREATE PROCEDURE APZ_D_GetListPropertiesBusinessType
AS
BEGIN

	select * from OCQG where "GroupName" not like '%Business Partners Property%';

END



