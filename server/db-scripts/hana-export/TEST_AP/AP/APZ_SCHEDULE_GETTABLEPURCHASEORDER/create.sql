

CREATE PROCEDURE APZ_Schedule_GetTablePurchaseOrder
AS
BEGIN
	CREATE LOCAL TEMPORARY TABLE #docOrder AS (SELECT TOP 0 * FROM OPOR);
	CREATE LOCAL TEMPORARY TABLE #lineOrder AS (SELECT TOP 0 * FROM POR1);
	INSERT INTO #docOrder ("DocEntry", "DocNum", "Instance", "Segment", "PIndicator", "DocSubType") VALUES (0, 0, 0, 0, '', '');
	INSERT INTO #lineOrder ("DocEntry", "LineNum") VALUES (0, 0);
	SELECT * FROM #docOrder;
	SELECT * FROM #lineOrder;
	
	DROP TABLE #docOrder;
	DROP TABLE #lineOrder;
END



