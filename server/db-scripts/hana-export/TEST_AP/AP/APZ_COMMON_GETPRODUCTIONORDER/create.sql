
CREATE PROCEDURE APZ_Common_GetProductionOrder()
AS
BEGIN
	SELECT T0.*, t2."SeriesName" AS "Series", t1."UgpEntry", t1."InvntryUom", t1."ItemName"
	FROM OWOR T0 JOIN OITM t1 ON T0."ItemCode" = t1."ItemCode" 
				JOIN NNM1 t2 ON T0."Series" = t2."Series" AND t2."ObjectCode" = '202'
	WHERE T0."Status" in ('R', 'P');
END;



