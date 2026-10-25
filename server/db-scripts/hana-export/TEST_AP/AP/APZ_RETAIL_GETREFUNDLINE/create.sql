
--- [apz_Retail_GetRefundLine] 134

CREATE PROCEDURE apz_Retail_GetRefundLine
(
DocNum INT)
AS
BEGIN
	declare DocEntry integer;
	select IFNULL((SELECT TOP 1 "DocEntry" FROM ORDR WHERE "DocNum" = :DocNum), 0) into DocEntry from DUMMY;
-- Sửa số lượng có thể đổi trả
CREATE LOCAL TEMPORARY TABLE #tbl AS (SELECT top 0 * FROM rdr1);
insert into #tbl select * from RDR1 WHERE "DocEntry" = :DocEntry AND ("Quantity" > 0 OR ("Quantity" = 0 AND "TrgetEntry" IS NOT NULL));
UPDATE #tbl SET "Quantity" = "Quantity" + (SELECT IFNULL(SUM(b."Quantity"), 0) AS "sumRs" FROM RDR1 b WHERE b."U_OriginType" = '17' AND b."U_OriginCode" = :DocEntry AND b."U_OriginLine" = #tbl."LineNum");
SELECT * FROM ORDR WHERE "DocEntry" = :DocEntry;
SELECT * FROM #tbl WHERE "Quantity" > 0;
END	





