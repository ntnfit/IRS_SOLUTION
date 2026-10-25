
CREATE PROCEDURE APZ_Commons_GetUoMCodeByName(
	in uomGroup int default -1,
	in uomName nvarchar(100) default '')
AS
BEGIN
	SELECT * FROM OUOM a INNER JOIN UGP1 b ON a."UomEntry" = b."UomEntry" INNER JOIN OUGP c ON b."UgpEntry" = c."UgpEntry" WHERE a."UomName" = :uomName AND c."UgpEntry" = :uomGroup;
END;




