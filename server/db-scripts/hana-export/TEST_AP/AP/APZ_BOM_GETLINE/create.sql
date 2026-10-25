CREATE PROCEDURE apz_BOM_GetLine(in father nvarchar(50) default '')
as
BEGIN

	DECLARE UDF NVARCHAR(5000);
	DECLARE sqlQuery NVARCHAR(5000)='';
	SELECT (SELECT  string_agg(CASE DATA_TYPE_NAME 
		WHEN 'bit' THEN 'ifnull(A."' || COLUMN_NAME ||'",0) "'||COLUMN_NAME || '",'
		WHEN 'money'  THEN 'ifnull(A."' || COLUMN_NAME || '",0) "' || COLUMN_NAME || '",'
		WHEN 'int'  THEN 'ifnull(A.' || COLUMN_NAME || ',0) ' || COLUMN_NAME || ','
		WHEN 'decimal'  THEN 'ifnull(A."'|| COLUMN_NAME || '",0) "' || COLUMN_NAME || '",'
		WHEN 'smallint'  THEN 'ifnull(A."' || COLUMN_NAME || '",0)"' || COLUMN_NAME || '",'
		WHEN 'numeric'  THEN 'ifnull(A."'|| COLUMN_NAME || '",0)"' || COLUMN_NAME || '",'
		WHEN 'tinyint'  THEN 'ifnull(A."' || COLUMN_NAME || ',0) "' || COLUMN_NAME || '",'
		WHEN 'float'  THEN 'ifnull(A."' || COLUMN_NAME || '",0) "' || COLUMN_NAME || '",'
		WHEN 'bigint'  THEN 'ifnull(A."' || COLUMN_NAME || '",0) "' || COLUMN_NAME || '",'
		else
		'A."' || COLUMN_NAME || '" "' || COLUMN_NAME || '",' end)
		FROM TABLE_COLUMNS
		WHERE SCHEMA_NAME = current_schema and TABLE_NAME = N'ITT1' AND LEFT(COLUMN_NAME,2) ='U_') INTO UDF FROM dummy;
	
	sqlQuery=' select 
				ifnull("Father",'''') as "Father"
				,"VisOrder"
				,ifnull("ChildNum",0) as "ChildNum"
				,ifnull(a."Code",'''') as "Code" 
	            ,ifnull("Quantity",0) as "Quantity" 
				,ifnull("Warehouse",'''') as "Warehouse" 
				,ifnull("Price",0) as "Price" 
				,ifnull(a."PriceList",0) as "PriceList"
				,ifnull(a."IssueMthd",'''') "IssueMthd"
	            ,ifnull("Comment",'''') as "Comment",
	            ifnull("Type",4) "Type"
	            ,ifnull("WipActCode",'''') as "WipActCode"
				 ,ifnull("AddQuantit",0) as "AddQuantit",
				ifnull(case a."Type"
			            when 4 then b."ItemName"
			            when 290 then e."ResName"
			                end,'''') as "ItemName"
				,ifnull("Price",0)*ifnull("Quantity",0) as "Total"
	            ,ifnull(b."InvntryUom",'''') as "Uom"
				,ifnull("Project",'''') "Project"
				,ifnull("Currency",''VND'') "Currency"
				,ifnull(a."OcrCode",'''') as "OcrCode"
				,ifnull(a."OcrCode2",'''') as "OcrCode2"
				,ifnull(a."OcrCode3",'''') as "OcrCode3"
				,ifnull(a."OcrCode4",'''') as "OcrCode4"
				,ifnull(a."OcrCode5",'''') as "OcrCode5"
				,"LineText" ';
	
	             IF ifnull(:UDF,'')<>''
				then 
					sqlQuery=:sqlQuery ||  ',' ||  SUBSTRING(:UDF,0,LENGTH(:UDF) - 1);
				END if;
				sqlQuery = :sqlQuery ||   ' FROM itt1 a left join oitm b on a."Code"=b."ItemCode" left join orsc e on e."ResCode"=a."Code"
				WHERE  a."Father" = ''' || ifnull(:father,'') || ''' order by a."VisOrder";';

	--select sqlQuery from dummy;
	execute immediate :sqlQuery;
END
--call apz_BOM_GetLine ('LM4029PS')
