
CREATE PROCEDURE APZ_Commons_GetUoMCodeByCode(
	in uomGroup int default -1,
	in uomCode nvarchar(100))
AS
BEGIN
	SELECT * FROM OUOM a INNER JOIN UGP1 b ON a."UomEntry" = b."UomEntry" INNER JOIN OUGP c ON b."UgpEntry" = c."UgpEntry" WHERE a."UomCode" = :uomCode AND c."UgpEntry" = :uomGroup;
END;




