
CREATE PROCEDURE APZ_OWGT_GetLineByUnitDisply
(UnitDisply nvarchar(2))
AS
BEGIN
	select * from OWGT where "UnitDisply" = :UnitDisply;
END
--APZ_OWGT_GetLineByUnitDisply 'kg'




