
CREATE PROCEDURE apz_packing_getlines(in DocEntry INT default 0)
AS
BEGIN
	SELECT T0.*, CASE WHEN t1."ManBtchNum" = 'Y' THEN 'B' WHEN T1."ManSerNum" = 'Y' THEN 'S' ELSE 'N' END AS "ManBatchSerial" FROM APZ_PAK1 T0 INNER JOIN OITM T1 ON T0."ItemCode" = T1."ItemCode" WHERE T0."DocEntry" = :DocEntry;
END;



