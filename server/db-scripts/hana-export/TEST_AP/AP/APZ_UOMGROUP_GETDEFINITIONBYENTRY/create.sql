
CREATE  PROCEDURE APZ_UoMGroup_GetDefinitionByEntry
(in UgpEntry int)
AS
BEGIN
	select * from UGP1 where "UgpEntry" = :UgpEntry;
END




