
CREATE PROCEDURE APZ_UoMGroup_CheckInItems
(in UgpEntry int)
AS
BEGIN
	select * from OUGP a join OITM b on a."UgpEntry" = b."UgpEntry" where b."UgpEntry" = :UgpEntry;
END
--APZ_UoMGroup_CheckInItems 1




