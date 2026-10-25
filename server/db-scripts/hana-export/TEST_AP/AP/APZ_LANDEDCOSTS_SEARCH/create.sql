
create PROCEDURE APZ_LandedCosts_Search(
	in fromDate timestamp default '',
	in toDate timestamp default '',
	in vendor nvarchar(50) default '',
	in broker nvarchar(50) default '',
	in docNum int default 0
)
AS 
BEGIN
	SELECT a.*, CAST(0 AS decimal) AS "DocVat1", CAST(0 AS decimal) AS "DocVat2", 
		CAST(0 AS decimal) AS "DocExpCustom", CAST(0 AS decimal) AS "DocActCustom", 
		CAST(0 AS decimal) AS "DocBeforeVat", CAST(0 AS decimal) AS "Total", 
		CAST(0 AS decimal) AS "DocCostSum" 
	FROM OIPF a 
	WHERE (IFNULL(:fromDate, '') = '' OR a."DocDate" >= :fromDate) 
		AND (IFNULL(:toDate, '') = '' OR a."DocDate" <= :toDate) AND (IFNULL(:vendor, '') = '' OR a."CardCode" = :vendor) 
		AND (IFNULL(:broker, '') = '' OR a."AgentCode" = :vendor) 
		AND (IFNULL(:docNum, 0) = 0 OR a."DocNum" = :docNum) 
	ORDER BY "DocNum" DESC;
END;

