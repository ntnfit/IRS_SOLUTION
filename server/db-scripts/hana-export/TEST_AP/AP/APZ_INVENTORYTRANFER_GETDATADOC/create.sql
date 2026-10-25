
CREATE PROCEDURE APZ_InventoryTranfer_GetDataDoc(
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
	
	declare checkDocNum int = 0;
	
	---------------------------------------------------- CURRENT --------------------------------------------------------
	if :typeGet = 4
	THEN
		SELECT a.*, b."Name" AS "CntctName", a."DocCur" AS "Currency",cast(0 as tinyint) "IsDraft" FROM OWTR a LEFT OUTER JOIN OCPR b ON a."CntctCode" = b."CntctCode" 
				WHERE (:docEntry <> 0 AND a."DocEntry" = :docEntry)
				OR (IFNULL(:docEntry, 0) = 0 AND :docNum <> 0 AND a."DocNum"=:docNum)
				OR (IFNULL(:docEntry, 0) = 0 AND IFNULL(:docNum, 0) = 0
					AND (IFNULL(:cardCode, '') = '' OR a."CardCode" LIKE '%' || :cardCode || '%') 
					AND (IFNULL(:docStatus, '') = '' OR :docStatus = 'A' OR a."DocStatus" = :docStatus)
					AND (IFNULL(:priceList, 0) = 0 OR a."GroupNum" = :priceList)
					AND (IFNULL(:fromDate, '') = '' OR a."DocDate" >= :fromDate)
					AND (IFNULL(:toDate, '') = '' OR a."DocDate" <= :toDate))
		ORDER BY "DocNum" DESC;
	---------------------------------------------------- Previous -----------------------------------------------------------
	elseif :typeGet = 1
	THEN
		------- Nếu Record hiện tại là giá trị nhỏ nất thì lấy thành bản ghi cuối cùng
		select ifnull((select min("DocNum") from OWTR),0) into checkDocNum from DUMMY;
		if :DocNum <= :checkDocNum
		THEN
			select IFNULL((select max("DocNum") from OWTR), 0) + 1 into DocNum from DUMMY;
		END IF;

		SELECT TOP 1 a.*, b."Name" AS "CntctName", a."DocCur" AS "Currency",cast(0 as tinyint) "IsDraft" FROM OWTR a LEFT OUTER JOIN OCPR b ON a."CntctCode" = b."CntctCode" WHERE "DocNum" < :DocNum ORDER BY "DocNum" DESC;
	--------------------------------------------------- Next -----------------------------------------------------------
	ELSEIF :typeGet = 2
	THEN
		------------------- Nếu record hiện tại là giá trị lớn nhất thì lấy thành bản ghi nhỏ nhất
		select ifnull((select max("DocNum") from OWTR),0) into checkDocNum from DUMMY;
		if :DocNum = :checkDocNum
		then
			SELECT IFNULL((select min(DocNum) from OWTR), 0) - 1 into DocNum from DUMMY;
		end IF;

		SELECT TOP 1 a.*, b."Name" AS "CntctName", a."DocCur" AS "Currency",cast(0 as tinyint) "IsDraft" FROM OWTR a LEFT OUTER JOIN OCPR b ON a."CntctCode" = b."CntctCode" WHERE "DocNum" > :DocNum ORDER BY "DocNum" ASC;
	-------------------------------------------------- Last ----------------------------------------------------------------
	elseif :typeGet = 3
	THEN

		SELECT a.*, b."Name" AS "CntctName", a."DocCur" AS "Currency",cast(0 as tinyint) "IsDraft" FROM OWTR a LEFT OUTER JOIN OCPR b ON a."CntctCode" = b."CntctCode" WHERE "DocNum" = IFNULL((SELECT MAX("DocNum") FROM OWTR), 0);

	ELSEIF :typeGet = 0
	THEN

		SELECT a.*, b."Name" AS "CntctName", a."DocCur" AS "Currency",cast(0 as tinyint) "IsDraft" FROM OWTR a LEFT OUTER JOIN OCPR b ON a."CntctCode" = b."CntctCode" WHERE "DocNum" = IFNULL((SELECT MIN("DocNum") FROM OWTR), 0);

	end if;

END;




