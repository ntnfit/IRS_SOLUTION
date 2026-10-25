
CREATE PROCEDURE apz_packing_getdoc(
	in FromDate timestamp default NULL,
	in ToDate timestamp default NULL,
	in CardCode NVARCHAR(50) default '',
	in DocEntry INT default 0,
	in DocStatus CHAR(1) default '')
AS
BEGIN
	IF :DocEntry<>0
	THEN
		SELECT * FROM apz_OPAK WHERE "DocEntry"=:DocEntry ORDER BY "DocEntry" desc;	
	ELSE 
		SELECT * FROM APZ_OPAK WHERE ("DocStatus" = :DocStatus OR ifnull(:DocStatus,'') = '') AND ("CardCode" = :CardCode OR :CardCode = '') 
									AND "DocDate" BETWEEN IFNULL(:FromDate, '1900-01-01') AND IFNULL(:ToDate, CURRENT_DATE) ORDER BY "DocEntry" DESC;
	END IF;
END




