
CREATE PROCEDURE APZ_UoM_CheckInGroup
(in UomEntry int)
AS
BEGIN
	select * from OUOM a join UGP1 b on a."UomEntry" = b."UomEntry" where b."UomEntry" = :UomEntry;
END




