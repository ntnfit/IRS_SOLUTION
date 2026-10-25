-- B1 DEPENDS: BEFORE:PT:CREATE_TABLES_TO_VIEWS

CREATE PROCEDURE _TmSp_CreateViewIBT1
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER
AS
sql1 varchar(4999);
rownum int;
DBName varchar(255);
BEGIN


	SELECT COUNT(*) into rownum  FROM "PUBLIC"."VIEWS" 
	where "VIEW_NAME" = 'IBT1_LINK' AND "SCHEMA_NAME" = CURRENT_SCHEMA;
	
	IF :rownum > 0 	then
		sql1 := 'DROP VIEW ' || '"'|| CURRENT_SCHEMA || '"' ||'.IBT1_LINK';
		exec (:sql1);
     end if;

	sql1 := 'create view ' || '"'|| CURRENT_SCHEMA || '"' || '.IBT1_LINK '||
		' as  ' ||

		' SELECT	MIN(b."LogEntry")		as "LogEntry", '||
					'MIN(b."ItemCode")		as "ItemCode", '||
					'MIN(c."DistNumber")	as "BatchNum", '||
					'MIN(b."LocCode")		as "WhsCode", '||
					'MIN(b."ItemName")		as "ItemName", '||
					'MIN(b."ApplyType")	as "BaseType",    ' ||  
					'MIN(b."ApplyEntry")	as "BaseEntry", '||
					'MIN(b."AppDocNum")	as "BaseNum", '||
					'MIN(b."ApplyLine")	as "BaseLinNum", '||
					'MIN(b."DocDate")		as "DocDate", '||
					'(	  '						 ||
					'	CASE WHEN ABS(SUM(a."Quantity")) = 0 THEN SUM(a."AllocQty") '||
					'	ELSE ABS(SUM(a."Quantity")) END '||
					')					as "Quantity", '||
					'MIN(b."CardCode")		as "CardCode", '||
					'MIN(b."CardName")		as "CardName", '||
					'( '||
					'	CASE WHEN  SUM(a."Quantity") > 0 THEN 0 '||
					'	WHEN SUM(a."Quantity") < 0 THEN 1 '||
					'	ELSE 2 END '||
					')					as "Direction", '||
					'MIN(b."CreateDate")	as "CreateDate", '||
					'MIN(b."BaseType")		as "BsDocType", '||
					'MIN(b."BaseEntry")	as "BsDocEntry", '||
					'MIN(b."BaseLine")		as "BsDocLine", '||
					' ''N''				as "DataSource", '||
					'NULL				as "UserSign" '||
		'FROM		ITL1 a '||
		'INNER JOIN	OITL b ON a."LogEntry" = b."LogEntry" '||
		'INNER JOIN	OBTN c ON a."ItemCode" = c."ItemCode" and a."SysNumber" = c."SysNumber" '||
		'GROUP BY	b."ItemCode", a."SysNumber", b."ApplyType", b."ApplyEntry", b."ApplyLine", b."LocCode", b."StockEff" '||
		'having		(SUM(b."DocQty") <> 0) '		||
		'OR			(SUM(b."DefinedQty") <> 0)  '	||
		'OR			(SUM(b."DocQty") = 0 and b."StockEff" = 2  '||
					 'and min(b."BaseType") <> 17 and min(b."BaseType") <> 13 ) ';
    
	exec (:sql1);

	SELECT COUNT(*) into rownum  FROM "PUBLIC"."VIEWS" where "VIEW_NAME" = 'IBT1' AND "SCHEMA_NAME" = CURRENT_SCHEMA;
	IF :rownum > 0 	then
		sql1 := 'DROP VIEW '|| '"'|| CURRENT_SCHEMA || '"' ||'.IBT1';
		exec (:sql1);
     end if;
     
     sql1 := 'create view ' || '"'|| CURRENT_SCHEMA || '"' ||'.IBT1 '||
		'as '||

		'SELECT  "ItemCode" '||
			 ' ,"BatchNum" '||
			 ' ,"WhsCode" '||
			 ' ,(	'||
			'		select count(1) from ' || '"'|| CURRENT_SCHEMA || '"' || '.IBT1_LINK b '||
			'		where b."ItemCode" = a."ItemCode" and b."BatchNum" = a."BatchNum" and b."WhsCode" = a."WhsCode" and b."LogEntry" < a."LogEntry" '||
			 ' ) as "LineNum" '||
			 ' ,"ItemName" '||
			 ' ,"BaseType" '||
			  ',"BaseEntry" '||
			  ',"BaseNum" '||
			 ' ,"BaseLinNum" '||
			 ' ,"DocDate" '||
			 ' ,"Quantity" '||
			  ',"CardCode" '||
			 ' ,"CardName" '||
			 ' ,"Direction" '||
			 ' ,"CreateDate" '||
			 ' ,"BsDocType" '||
			 ' ,"BsDocEntry" '||
			 ' ,"BsDocLine" '||
			 ' ,"DataSource" '||
			 ' ,"UserSign" ' ||     
		'from '|| '"'|| CURRENT_SCHEMA || '"' ||'.IBT1_LINK a ';
		exec (:sql1);
     
END;


