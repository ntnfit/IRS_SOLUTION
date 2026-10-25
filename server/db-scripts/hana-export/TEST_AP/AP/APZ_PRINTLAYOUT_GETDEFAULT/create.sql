
CREATE PROCEDURE Apz_Printlayout_GetDefault (
	IN 	userName NVARCHAR(50)
	,in TypeCode NVARCHAR(20)
	,in CardCode NVARCHAR(50) DEFAULT ''
)
AS
BEGIN
	DECLARE userId INT := 0;
	
	SELECT TOP 1 USERID INTO userId FROM OUSR WHERE USER_CODE = :userName;
	SELECT IFNULL(a."TypeCode", c."TypeCode") AS "TypeCode", IFNULL(a."TypeName", c."TypeName") AS "TypeName", IFNULL(a."DocCode", c."DocCode") AS "DocCode", IFNULL(IFNULL(r."DocName", a."DocName"), c."DocName") AS "DocName", IFNULL(a."FileName", '') AS "FileName", c."Text" AS "XtraText" 
		FROM APZ_PRINTLAYOUT a 
		LEFT JOIN RDOC r ON a."DocCode"=r."DocCode"
		LEFT JOIN RDFL b ON a."DocCode" = b."DfltReport" 
		FULL OUTER JOIN APZ_RDOC c ON c."DocCode" = a."DocCode" 
		WHERE IFNULL(a."TypeCode", c."TypeCode") = :TypeCode 
			AND (b."CardCode" = :CardCode 
				OR IFNULL(b."CardCode", '') = '' 
				OR :CardCode = '') 
		ORDER BY b."DfltReport" DESC;
	
END;

