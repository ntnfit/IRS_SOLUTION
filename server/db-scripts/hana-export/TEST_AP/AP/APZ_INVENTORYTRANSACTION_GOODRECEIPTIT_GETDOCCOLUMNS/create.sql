
CREATE PROCEDURE APZ_InventoryTransaction_GoodReceiptIT_GetDocColumns (
	IN 	DocEntry INT DEFAULT -1
)
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER
AS
BEGIN
	DECLARE sqlStr NVARCHAR(5000);
	IF :DocEntry = -1 THEN
	BEGIN		
		DECLARE listColumns NVARCHAR(5000);
		DECLARE val NVARCHAR(5000);
		
		SELECT STRING_AGG('"' || COLUMN_NAME || '"',',') INTO listColumns
			FROM TABLE_COLUMNS 
			WHERE SCHEMA_NAME = CURRENT_SCHEMA 
				AND TABLE_NAME = N'OIGN'
				AND IS_NULLABLE='FALSE';
		listColumns := :listColumns || 'TotalBeforeDis';
		SELECT STRING_AGG(
			CASE LOWER(DATA_TYPE_NAME)
			WHEN 'bit' THEN '0' 
			WHEN 'integer' THEN '0' 
			WHEN 'double' THEN '0' 
			WHEN 'decimal' THEN '0' 
			WHEN 'float' THEN '0' 
			WHEN 'numeric' THEN '0' 
			WHEN 'money' THEN '0' 
			WHEN 'smallint' THEN '0' 
			WHEN 'tinyint' THEN '0' 
			WHEN 'bigint' THEN '0' 
			WHEN 'date' THEN '''''' 
			WHEN 'datetime' THEN '''''' 
			WHEN 'char' THEN '''''' 
			WHEN 'nvarchar' THEN '''''' 
			WHEN 'text' THEN '''''' 
			ELSE '''''' 
			END, ',') INTO val
		FROM TABLE_COLUMNS 
		WHERE TABLE_NAME = 'OIGN' 
			AND IS_NULLABLE='FALSE';
		val := :val || '0';
		sqlStr := N'CREATE LOCAL TEMPORARY TABLE #Tmp_OIGN AS (SELECT TOP 0 *,0 AS TotalBeforeDis FROM OIGN);'
			|| 	  N'INSERT INTO #Tmp_OIGN (' || :listColumns || ') VALUES (' || :val || ')'
			||	  N'SELECT * FROM #Tmp_OIGN;'
			||	  N'DROP TABLE #Tmp_OIGN' ;
		EXECUTE IMMEDIATE :sqlStr;	
	END;
	ELSE
		sqlStr := N'SELECT TOP 0 A.*, (SELECT (SUM(IFNULL(B."LineTotal", 0))) FROM IGN1 B WHERE B."DocEntry" = A."DocEntry") As TotalBeforeDis FROM OIGN A;';
		EXECUTE IMMEDIATE :sqlStr;	
	END IF;
END;




