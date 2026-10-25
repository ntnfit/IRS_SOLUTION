-- B1 DEPENDS: BEFORE:PT:CREATE_TABLES_TO_VIEWS

CREATE PROCEDURE _TmSp_CreateViewOSRD
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER

AS

sqlString1 nvarchar(4000);
sqlString2 nvarchar(4000);
sqlString3 nvarchar(4000);
sqlStrDrop nvarchar(1000);
exists1 int;
DBName nvarchar(255);      
      
BEGIN
	exists1 := 0;
	sqlStrDrop := 'drop view ' || '"'|| CURRENT_SCHEMA || '"' ||'.OSRD';
	-- create view OSRD
	select count(*) into exists1
	from "PUBLIC".objects
	where object_oid = (select VIEW_OID from "PUBLIC"."VIEWS" where VIEW_NAME = 'OSRD' AND "SCHEMA_NAME" = CURRENT_SCHEMA)
		and object_type = 'VIEW';
	
	if (:exists1 <> 0)then
		exec(:sqlStrDrop);
	end if;
	
	sqlString1 := ' CREATE VIEW ' || '"'|| CURRENT_SCHEMA || '"' ||'. OSRD AS ' 
				||  'SELECT '   
				||  'MIN(a."ItemCode")	as "ItemCode",'
				||	'MIN(a."ApplyType")	as "DocType",'
				||	'MIN(a."ApplyEntry")	as "DocEntry",'
				||	'MIN(a."AppDocNum")	as "DocNum",'
				||	'MIN(a."ApplyLine")	as "DocLineNum",'
				||	'ABS(SUM(a."DocQty") - SUM(IFNULL (d."DocQty", 0)))	as "DocQuty",'
				||	'ABS(SUM(a."DocQty") - SUM(IFNULL (d."DocQty", 0))) - MIN(b."DefinedQty") as "DocOpenQty",'
				||	'MIN(a."DocDate")		as "DocDate",'
				||	'(CASE WHEN ABS(SUM(a."DocQty") - SUM(IFNULL (d."DocQty", 0))) > MIN(b."DefinedQty") THEN ''0'''
				||	' ELSE ''1'' END)		as "Status",'
				||	'(CASE WHEN MIN(a."ManagedBy") = 10000044 THEN 106'
				||	' ELSE 94 END)			as "SrdType",'
				||	'MIN(a."ItemName")	as "ItemName",'
				||	'MIN(a."LocCode")		as "WhsCode",'
				||	'MIN(a."CardCode")	as "CardCode",'
				||	'MIN(a."CardName")		as "CardName",'
				||	'MIN(c."ManOutOnly")	as "ManOutOnly",'
				||	'(CASE WHEN (SUM(a."DocQty") > 0 AND MIN(a."StockEff") = 1)'
				||	' OR (SUM(a."DocQty") < 0 AND MIN(a."StockEff") = 2)  THEN ''0'''
				||	' ELSE ''1'' END)		as "Direction",'
				||	'NULL				as "CreateNew",'
				||	'''N''				as "DataSource",'
				||	'NULL				as "UserSign"'
				||  ' FROM  ' || '"'|| CURRENT_SCHEMA || '"' ||'.OITL  a';
				
	sqlString2 := '	LEFT  JOIN	( '
				||  'select '	
				||  '(CASE when IFNULL(ABS(SUM(bb."Quantity")), 0) > 0 THEN'
				||  '	IFNULL(ABS(SUM(bb."Quantity")), 0)'
				||  '	else IFNULL(ABS(SUM(bb."AllocQty")), 0) END'
				||  '	) 					as "DefinedQty",'
				||  'MIN (aa."LogEntry") as "LogEntry"'
				||  ' from  ' || '"'|| CURRENT_SCHEMA || '"' ||'.OITL aa'
				||  ' left join	' || '"'|| CURRENT_SCHEMA || '"' ||'.ITL1 bb on aa."LogEntry" = bb."LogEntry"'
				||  ' group by	aa."ItemCode", aa."ApplyType", aa."ApplyEntry", aa."ApplyLine", aa."LocCode"'
				||  ' ) b on b."LogEntry" = a."LogEntry" ';
				
	sqlString3 := ' INNER JOIN	' || '"'|| CURRENT_SCHEMA || '"' ||'.OITM c on c."ItemCode" = a."ItemCode"'
				||	' LEFT OUTER JOIN ' || '"'|| CURRENT_SCHEMA || '"'
				||	'.OITL d on (d."LogEntry" = a."LogEntry" and d."StockQty" = 0 and d."DefinedQty" > 0 and d."StockEff" = 1) '
				||	' or (d."LogEntry" = a."LogEntry" and d."StockEff" = 2 and d."ApplyType" in (13,14,15,60,59,67))'
				||	'WHERE d."ApplyType" <> 13 or d."LogEntry" is null '
				||	'or  d."ApplyType" = 13 and exists (select 1 from "'|| CURRENT_SCHEMA || '".OITL U0 WHERE U0."TransId" = a."TransId" AND U0."StockEff" <> 2) '
				||  ' GROUP BY	a."ItemCode", a."ApplyType", a."ApplyEntry", a."ApplyLine", a."LocCode"'
				||  ' HAVING (MIN(a."ApplyType") <> 67 and SUM(a."DocQty") <> 0)'
				||  '	OR	 (MIN(a."ApplyType") = 67 and SUM(a."DocQty") < 0)';
				
	exec (:sqlString1 || :sqlString2 || :sqlString3);
END;


