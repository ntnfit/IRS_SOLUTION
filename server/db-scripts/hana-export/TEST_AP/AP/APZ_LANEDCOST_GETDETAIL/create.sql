
create PROCEDURE APZ_LanedCost_GetDetail(in docId int)
AS 
BEGIN
SELECT a.*, CASE WHEN a."BaseType" = 20 THEN b."UomCode" WHEN a."BaseType" = 18 THEN c."UomCode" END AS "UomCode", '' AS "DocCur" FROM IPF1 a LEFT OUTER JOIN PDN1 b ON a."ItemCode" = b."ItemCode" AND a."BaseEntry" = b."DocEntry" LEFT OUTER JOIN PCH1 c ON a."ItemCode" = c."ItemCode" AND a."BaseEntry" = c."DocEntry" AND a."DocEntry" = b."DocEntry" WHERE a."DocEntry" = :docId;
SELECT a.*, b."AlcName" AS "AlcName", '' AS "DocCur", 0 AS "DocRate" FROM IPF2 a LEFT OUTER JOIN OALC b ON a."AlcCode" = b."AlcCode" WHERE "DocEntry" = :docId;
SELECT * FROM ATC1 WHERE "AbsEntry" = (SELECT "AtcEntry" FROM OIPF WHERE "DocEntry" = :docId);

END;




