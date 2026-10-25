

CREATE PROCEDURE APZ_Schedule_GetDataMrp(IN mrpCode int)
AS
BEGIN
	select * from APZ_OMRP where "MrpCode" = :mrpCode;
END



