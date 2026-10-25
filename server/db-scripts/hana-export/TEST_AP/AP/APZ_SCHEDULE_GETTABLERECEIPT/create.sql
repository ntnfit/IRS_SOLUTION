

CREATE PROCEDURE APZ_Schedule_GetTableReceipt
AS
BEGIN
	CREATE LOCAL TEMPORARY TABLE #tempdoc AS (SELECT TOP 0 * FROM OIGN);
	CREATE LOCAL TEMPORARY TABLE #templine AS (SELECT TOP 0 * FROM IGN1);
	INSERT INTO #tempdoc ("DocEntry", "DocNum", "Instance", "Segment", "PIndicator", "DocSubType") VALUES (0, 0, 0, 0, '', '');
	INSERT INTO #templine ("DocEntry", "LineNum") VALUES (0, 0);
	SELECT * FROM #tempdoc;
	SELECT * FROM #templine;
	
	DROP TABLE #tempdoc;
	DROP TABLE #templine;
END



