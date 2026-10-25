
CREATE PROCEDURE APZ_GoodsIssueIt_GetDataDoc (
	IN 	basetype INT DEFAULT -1
,	 	docNum INT DEFAULT 0
,		docEntry INT DEFAULT 0
,		priceList INT DEFAULT 0
,		fromDate TIMESTAMP DEFAULT ''
,		toDate TIMESTAMP DEFAULT ''
,		type INT DEFAULT 0
,		userCode nvarchar(25)
)
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER
AS
BEGIN
	DECLARE minDoc INT := 0;
	DECLARE maxDoc INT := 0;
	
	call apz_func_OwnerOverShip(:userCode, '60', OwnerView);
	
	IF :type = 4 THEN 
		SELECT *, (SELECT SUM(IFNULL(B."LineTotal", 0)) FROM IGE1 B WHERE B."DocEntry" = a."DocEntry") AS "TotalBeforeDis", a."DocCur" AS "Currency" 
			FROM OIGE a 
			WHERE EXISTS (SELECT 1 FROM IGE1 WHERE "DocEntry" = a."DocEntry" AND "BaseType" = :basetype) 
				AND ((IFNULL(:docEntry, 0) <> 0 AND a."DocEntry" = :docEntry)
					OR (IFNULL(:docEntry, 0) = 0 AND :docNum <> 0 AND a."DocNum"=:docNum)
					OR (IFNULL(:docEntry, 0) = 0 AND (IFNULL(:docNum, 0) = 0)
						AND (IFNULL(:fromDate, '') = '' OR a."DocDate" >= :fromDate) 
						AND (IFNULL(:toDate, '') = '' OR a."DocDate" <= :toDate))
						AND (IFNULL(:priceList, 0) = 0 OR a."GroupNum" = :priceList))
				AND (IFNULL(a."OwnerCode", -1) = -1 or a."OwnerCode" in (select distinct "OwnerId" from :OwnerView))
				AND (IFNULL(a."BPLId", -1) = -1 or a."BPLId" in (select x."BPLId" from OBPL x inner join USR6 y on x."BPLId" = y."BPLId" where x."Disabled" = 'N' and y."UserCode" = :userCode))
			ORDER BY "DocNum" DESC;
		
		RETURN;
	END IF;
	
	lstDoc = SELECT "DocEntry", "DocNum" FROM OIGE a WHERE 
			EXISTS (SELECT 1 FROM IGE1 WHERE "DocEntry" = a."DocEntry" AND "BaseType" = :baseType)
			AND (IFNULL(a."OwnerCode", -1) = -1 or a."OwnerCode" in (select distinct "OwnerId" from :OwnerView))
			AND (IFNULL(a."BPLId", -1) = -1 or a."BPLId" in (select x."BPLId" from OBPL x inner join USR6 y on x."BPLId" = y."BPLId" where x."Disabled" = 'N' and y."UserCode" = :userCode));
 
	IF :type = 1 THEN
	
		IF :docNum <= IFNULL((SELECT MIN("DocNum") from :lstDoc), 0) THEN
			SELECT MAX("DocNum") + 1 into docNum default 1 from :lstDoc;
		END IF;
	
		SELECT TOP 1 a.*, (SELECT SUM(IFNULL(B."LineTotal", 0)) FROM IGE1 B WHERE B."DocEntry" = a."DocEntry") AS "TotalBeforeDis", a."DocCur" AS "Currency" 
		FROM OIGE a 
		WHERE a."DocEntry" IN (SELECT "DocEntry" from :lstDoc) AND a."DocNum" < :docNum
		ORDER BY a."DocEntry" DESC;

	ELSEIF :type = 2 THEN
		
		IF :docNum = IFNULL((SELECT MAX("DocNum") from :lstDoc), 0) THEN 
			SELECT MIN("DocNum") - 1 into docNum default 0 from :lstDoc;
		END IF;
		
		SELECT TOP 1 a.*, (SELECT SUM(IFNULL(B."LineTotal", 0)) FROM IGE1 B WHERE B."DocEntry" = a."DocEntry") AS "TotalBeforeDis", a."DocCur" AS "Currency" 
		FROM OIGE a 
		WHERE a."DocEntry" IN (SELECT "DocEntry" from :lstDoc) AND a."DocNum" > :docNum
		ORDER BY a."DocEntry" ASC;
		
	ELSEIF :type = 3 THEN
	
		SELECT TOP 1 a.*, (SELECT SUM(IFNULL(B."LineTotal", 0)) FROM IGE1 B WHERE B."DocEntry" = a."DocEntry") AS "TotalBeforeDis", a."DocCur" AS "Currency" 
		FROM OIGE a 
		WHERE a."DocNum" = IFNULL((SELECT MAX("DocNum") FROM :lstDoc), 0)
		ORDER BY a."DocEntry" DESC;
			
	ELSEIF :type = 0 THEN
	
		SELECT TOP 1 a.*, (SELECT SUM(IFNULL(B."LineTotal", 0)) FROM IGE1 B WHERE B."DocEntry" = a."DocEntry") AS "TotalBeforeDis", a."DocCur" AS "Currency" 
		FROM OIGE a 
		WHERE a."DocNum" = IFNULL((SELECT MIN("DocNum") FROM :lstDoc), 0)
		ORDER BY a."DocEntry" ASC;
	END IF;
END;

