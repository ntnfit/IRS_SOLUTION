

CREATE PROCEDURE APZ_Schedule_GetTablePurchaseRequest
AS
BEGIN
	CREATE LOCAL TEMPORARY TABLE #docRequest AS (SELECT TOP 0 * FROM OPRQ);
	CREATE LOCAL TEMPORARY TABLE #lineRequest AS (SELECT TOP 0 * FROM PRQ1);
	INSERT INTO #docRequest ("DocEntry", "DocNum", "Instance", "Segment", "PIndicator", "DocSubType") VALUES (0, 0, 0, 0, '', '');
	INSERT INTO #lineRequest ("DocEntry", "LineNum") VALUES (0, 0);
	SELECT * FROM #docRequest;
	SELECT * FROM #lineRequest;
	
	DROP TABLE #docRequest;
	DROP TABLE #lineRequest;
END



