
CREATE  PROCEDURE APZ_UoMGroup_GetDefinition(in UgpEntry int)
AS
BEGIN
	--select b.*,c."UomCode",
	--(select "UomCode" from 
	--(select "UomCode", row_number () over (partition by e."UomCode" 
    --ORDER BY e."UomCode" asc) as RN from UGP1 d join OUOM e on d."UomEntry" = e."UomEntry" 
    --where d."LineNum"=1 and d."UgpEntry" = b."UgpEntry") where rn = 2) as "BaseUoMCode"
	--from  UGP1 b join OUOM c on b."UomEntry" = c."UomEntry"
	--where b."UgpEntry" = :UgpEntry;

	select a."UgpEntry", a."UomEntry", a."AltQty", a."BaseQty", a."LogInstanc", a."LineNum", a."WghtFactor", a."UdfFactor", b."UomCode" as "BaseUoMCode",b."UomCode"
	from UGP1 a inner join OUOM b on a."UomEntry" = b."UomEntry" 
	where a."UgpEntry" = :UgpEntry;
END;




