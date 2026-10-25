
CREATE PROCEDURE APZ_InventoryTranferRequest_GetDataDoc(
	in cardCode nvarchar(50) default '',
	in docNum int default 0,
	in docEntry int default 0,
	in docStatus char(1) default '',
	in priceList int default 0,
	in fromDate timestamp default '',
	in toDate timestamp default '',
	in typeGet int default 0)
AS
BEGIN

	declare tempDocNum int = 0;
	
	if :typeGet = 4
	THEN
		SELECT a.*, b."Name" AS "CntctName", a."DocCur" AS "Currency",cast(0 as tinyint) "IsDraft" FROM OWTQ a LEFT OUTER JOIN OCPR b ON a."CntctCode" = b."CntctCode" 
					WHERE (IFNULL(:docEntry, 0) <> 0 AND a."DocEntry" = :docEntry) OR (IFNULL(:docEntry, 0) = 0 AND (IFNULL(:cardCode, '') = '' OR a."CardCode" LIKE '%' || :cardCode || '%') 
					AND (IFNULL(:docStatus, '') = '' OR :docStatus = 'A' OR a."DocStatus" = :docStatus) AND (IFNULL(:docNum, 0) = 0 OR a."DocNum" = :docNum) AND (IFNULL(:priceList, 0) = 0 
					OR a."GroupNum" = :priceList) AND (IFNULL(:fromDate, '') = '' OR a."DocDate" >= :fromDate) AND (IFNULL(:toDate, '') = '' OR a."DocDate" <= :toDate)) ORDER BY "DocNum" DESC;
	elseif :typeGet = 1
	THEN
			
			select IFNULL((select min("DocNum") from OWTQ),0) into tempDocNum from DUMMY;
			if :DocNum <= tempDocNum
			THEN
				select IFNULL((select max("DocNum") from OWTQ), 0) + 1 into DocNum from DUMMY;
			END IF;

			SELECT TOP 1 a.*, b."Name" AS "CntctName", a."DocCur" AS "Currency",cast(0 as tinyint) "IsDraft" FROM OWTQ a LEFT OUTER JOIN OCPR b ON a."CntctCode" = b."CntctCode" WHERE "DocNum" < :DocNum ORDER BY "DocNum" DESC;

	ELSEIF :typeGet = 2
	THEN
	
			select ifnull((select max("DocNum") from OWTQ),0) into tempDocNum from DUMMY;
			if :DocNum = tempDocNum
			THEN
				select IFNULL((select min(DocNum) from OWTQ), 0) - 1 into DocNum from DUMMY;
			END IF;

			SELECT a.*, b."Name" AS "CntctName", a."DocCur" AS "Currency",cast(0 as tinyint) "IsDraft" FROM OWTQ a LEFT OUTER JOIN OCPR b ON a."CntctCode" = b."CntctCode" WHERE "DocNum" > :DocNum ORDER BY "DocNum" ASC;

	ELSEIF :typeGet = 3
	THEN

			SELECT a.*, b."Name" AS "CntctName", a."DocCur" AS "Currency",cast(0 as tinyint) "IsDraft" FROM OWTQ a LEFT OUTER JOIN OCPR b ON a."CntctCode" = b."CntctCode" WHERE "DocNum" = IFNULL((SELECT MAX("DocNum") FROM OWTQ), 0);

	ELSE

			SELECT a.*, b."Name" AS "CntctName", a."DocCur" AS "Currency",cast(0 as tinyint) "IsDraft" FROM OWTQ a LEFT OUTER JOIN OCPR b ON a."CntctCode" = b."CntctCode" WHERE "DocNum" = IFNULL((SELECT MIN("DocNum") FROM OWTQ), 0);

	end IF;

END




