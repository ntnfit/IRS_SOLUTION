
CREATE PROCEDURE APZ_OLGT_GetLineByUnitDisply
(UnitDisply nvarchar(2))
AS
BEGIN
	select * from OLGT where "UnitDisply" = :UnitDisply;
END




