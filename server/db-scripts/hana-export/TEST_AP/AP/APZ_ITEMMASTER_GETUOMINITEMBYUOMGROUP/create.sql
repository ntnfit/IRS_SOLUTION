
CREATE PROCEDURE APZ_ItemMaster_GetUomInItemByUomGroup(
	in ugpEntry int default -1,
	in itemCode nvarchar(50) default '')
AS
BEGIN
		select $itemCode "ItemCode", d.Data "UomType", a."UomEntry", a."VolUnit", a."UomCode", a."UomName" 
		from OUOM a inner join UGP1 b On a."UomEntry" = b."UomEntry"
					inner Join OUGP c On b."UgpEntry" = c."UgpEntry" inner Join (select data from split('S,P',',')) d ON 1=1
		where $ugpEntry > 0 and b."UgpEntry" = $ugpEntry;
END

