
CREATE PROCEDURE apz_shipping_getdoc(
	in FromDate timestamp default NULL,
	in ToDate timestamp default NULL,
	in CardCode NVARCHAR(50) default '',
	in DocEntry INT default 0,
	in DocStatus CHAR(1) default '')
AS
BEGIN
	create local temporary table #tbl as (SELECT TOP 0 * FROM APZ_SHIP);
	
	IF :DocEntry<>0
	THEN
		insert into #tbl
		SELECT T0.* FROM APZ_SHIP T0   WHERE T0."DocEntry"=:DocEntry ORDER BY "DocEntry" desc;
		
		UPDATE #tbl SET "DocStatus"=  'L' WHERE "DocStatus"<>'C' AND "DocEntry" IN (SELECT DISTINCT "DocEntry" FROM APZ_VIEW_SHIP_DELIVERIED);

	   SELECT * FROM #tbl;
	ELSE 
		insert into #tbl
			SELECT T0.* FROM APZ_SHIP T0 WHERE (T0."CardCode" = :CardCode OR :CardCode = '') AND T0."DocDate" BETWEEN IFNULL(:FromDate, '1900-01-01') AND IFNULL(:ToDate, CURRENT_DATE) ORDER BY T0."DocEntry" DESC;

    	UPDATE #tbl SET "DocStatus" = 'L' WHERE "DocStatus"<>'C' AND "DocEntry" IN (SELECT DISTINCT "DocEntry" FROM APZ_VIEW_SHIP_DELIVERIED) ;
		 
		SELECT * FROM #tbl T0 WHERE (T0."DocStatus"=:DocStatus OR ifnull(:DocStatus,'')='');
	END IF;	
END;



