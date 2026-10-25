
CREATE PROCEDURE APZ_Commons_GetDataBPByCode (
	IN 	cardCode NVARCHAR(15) DEFAULT ''
)
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER
READS SQL DATA
AS
BEGIN
	DECLARE MainCurr VARCHAR(10);
	SELECT IFNULL((SELECT "MainCurncy" FROM OADM), '') INTO MainCurr FROM DUMMY;
	
	SELECT a.*, b."CntctCode", b."Name", CASE WHEN IFNULL(a."Currency", '##') = '##' THEN :MainCurr ELSE a."Currency" END AS "BaseCurrency", IFNULL(c."Rate", 1) AS "Rate" 
		FROM OCRD a 
		LEFT OUTER JOIN OCPR b ON a."CntctPrsn" = b."Name" AND a."CardCode" = b."CardCode" 
		LEFT OUTER JOIN (SELECT "Rate", "Currency" FROM ORTT WHERE CAST("RateDate" AS date) = CAST(NOW() AS date)) AS c ON CASE WHEN IFNULL(a."Currency", '##') = '##' THEN :MainCurr ELSE a."Currency" END = c."Currency" 
		WHERE a."CardCode" = :cardCode;
END;




