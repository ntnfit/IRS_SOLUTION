
CREATE PROCEDURE APZ_SalesOpportunity_GetItemDetail (IN OpprId integer) 
AS 
BEGIN 

	---------------------------------------------- THANHNV convert mặc dù chả hiểu gì hihi ----------------------------------------------------
	DECLARE sqlString NVARCHAR(40000)='';
	DECLARE UDF NVARCHAR(40000);
	IF :OpprId=-1
	then 	
		SELECT string_agg('"' || COLUMN_NAME || '"',',') into UDF FROM TABLE_COLUMNS WHERE SCHEMA_NAME = CURRENT_SCHEMA and TABLE_NAME = N'@OPR62' AND LEFT(COLUMN_NAME,2) ='U_'	
			AND COLUMN_NAME NOT IN ('U_ItemCode','U_ItemName','U_FrgnName','U_ItemCode','U_Quantity','U_IPercent','U_Price','U_PotentAmount','U_DeliveryDate','U_OpportId','U_line','U_AttFile');
			
		---------------------------- CREATE TEMP TABLE -------------------------------------------------
		sqlString='create local temporary table #tbl as (SELECT TOP 0 "U_ItemCode" AS "ItemCode", "U_ItemName" AS "ItemName", "U_FrgnName" AS "FrgnName", "U_Quantity" AS "Quantity", "U_IPercent" AS "IPercent", "U_Price" AS "Price", 
						"U_PotentAmount" AS "PotentAmount", "U_DeliveryDate" AS "DeliveryDate", "U_OpportId" AS "OpportId", "LineId" AS "Line", "U_AttFile" AS "AttFile", '''' AS "AttFullPath", "DocEntry" ';
			
		if(IFNULL(:UDF, '') <> '')
		THEN
			sqlString=:sqlString || ',' ||  IFNULL(:UDF,'');
		END IF;
		sqlString = :sqlString || ' from "@OPR62");';
		
		EXECUTE immediate :sqlString;
		------------------------------------ END CREATE TEMP TABLE --------------------------------------
		
		------------------------------------ INSERT DEFAULT VALUE ---------------------------------------
		sqlstring = 'INSERT INTO #tbl ("ItemCode", "ItemName", "FrgnName", "Quantity", "IPercent", "Price", "PotentAmount", "DeliveryDate", "OpportId", "Line", "AttFile", "AttFullPath", "DocEntry")
			 values ('''','''','''',0,0,0,0,'''',-1,-1,'''','''',0);';
		EXECUTE immediate :sqlString;
		------------------------------------ END INSERT DEFAULT -----------------------------------------

		EXECUTE immediate 'select * from #tbl;';
		EXEC 'drop table #tbl;';
	else 
		SELECT  string_agg(CASE LOWER(DATA_TYPE_NAME)
						WHEN 'bit' THEN 'IFNULL(A."'|| COLUMN_NAME || '",0) "' ||COLUMN_NAME || '"'
						WHEN 'money'  THEN 'IFNULL(A."' || COLUMN_NAME || '",0) "' || COLUMN_NAME || '"'
						WHEN 'int'  THEN 'IFNULL(A."' || COLUMN_NAME || '",0) "' || COLUMN_NAME || '"'
						WHEN 'decimal'  THEN 'IFNULL(A."' || COLUMN_NAME || '",0) "' || COLUMN_NAME || '"'
						WHEN 'smallint'  THEN 'IFNULL(A."' || COLUMN_NAME || '",0) "' || COLUMN_NAME || '"'
						WHEN 'numeric'  THEN 'IFNULL(A."' || COLUMN_NAME || '",0) "' || COLUMN_NAME || '"'
						WHEN 'tinyint'  THEN 'IFNULL(A."' || COLUMN_NAME || '",0) "' || COLUMN_NAME || '"'
						WHEN 'float'  THEN 'IFNULL(A."' || COLUMN_NAME || '",0) "' || COLUMN_NAME || '"'
						WHEN 'bigint'  THEN 'IFNULL(A."' || COLUMN_NAME || '",0) "' || COLUMN_NAME || '"'
						else 'A."' || COLUMN_NAME || '" "' || COLUMN_NAME || '"' end,',') into UDF
				FROM TABLE_COLUMNS WHERE SCHEMA_NAME = CURRENT_SCHEMA
						AND TABLE_NAME = N'@OPR62' AND LEFT(COLUMN_NAME,2) ='U_'
						AND COLUMN_NAME NOT IN ('U_ItemCode','U_ItemName','U_FrgnName','U_ItemCode','U_Quantity','U_IPercent','U_Price','U_PotentAmount','U_DeliveryDate','U_OpportId','U_line','U_AttFile');
		
		
		sqlString='SELECT IFNULL("DocEntry",0) as "DocEntry",
		IFNULL("U_ItemCode",'''') as "ItemCode",
		IFNULL("U_ItemName",'''') as "ItemName",
		IFNULL("U_FrgnName",'''') as "FrgnName",
		IFNULL("U_Quantity",0) as "Quantity",
		IFNULL("U_IPercent",0) as "IPercent",
		IFNULL("U_Price",0) as "Price",
		IFNULL("U_PotentAmount",0) as "PotentAmount",
		IFNULL("U_DeliveryDate",'''') as "DeliveryDate",
		' || cast(:OpprId as NVARCHAR) || ' as "OpportId",
		IFNULL("LineId",-1) as "Line",
		IFNULL("U_AttFile",'''') as "AttFile",
		'''' as "AttFullPath" ';
		
		if(IFNULL(:UDF, '') <> '')
		THEN
			sqlString=:sqlString || ',' ||  IFNULL(:UDF,'');
		END IF;
		
		 sqlString=:sqlString || ' from "@OPR62" A where "DocEntry"=IFNULL((select top 1 "U_Itemdetail" from oopr where "OpprId"=''' || cast(:OpprId as NVARCHAR) || '''),0)';
		
		 EXECUTE immediate :sqlString;
	 END IF;
end








