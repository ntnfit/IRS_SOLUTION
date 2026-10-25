
CREATE PROCEDURE APZ_InventoryTransaction_GoodIssueIT_GetDocColumns (
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
				AND TABLE_NAME = N'IGE1'
				AND IS_NULLABLE='FALSE';
		listColumns := :listColumns || 'QuantityOnStock,QuantityOnWareHouse';
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
		WHERE TABLE_NAME = 'IGE1' 
			AND IS_NULLABLE='FALSE';
		val := :val || '0,0';
		sqlStr := N'CREATE LOCAL TEMPORARY TABLE #Tmp_IGE1 AS (SELECT TOP 0 *,0 AS QuantityOnStock,0 as QuantityOnWareHouse FROM IGE1);'
			|| 	  N'INSERT INTO #Tmp_IGE1 (' || :listColumns || ') VALUES (' || :val || ')'
			||	  N'SELECT *, '''' AS "ManBtchSerial", '''' AS "SalUnitMsr", 0 AS "NumInSale", '''' AS "BuyUnitMsr", 0 AS "NumInBuy" FROM #Tmp_IGE1;'
			||	  N'DROP TABLE #Tmp_IGE1' ;
		EXECUTE IMMEDIATE :sqlStr;	
	END;
	ELSE
			
	END IF;
END;




