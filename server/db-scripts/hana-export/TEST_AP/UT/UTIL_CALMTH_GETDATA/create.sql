
CREATE PROCEDURE Util_CalMth_GetData
AS
BEGIN
	select '' "Value",'' "Name" from DUMMY
	UNION all
	Select "CalMthCode" as "Value", "CalMthName" as "Name" from APZ_CalMth;
END



