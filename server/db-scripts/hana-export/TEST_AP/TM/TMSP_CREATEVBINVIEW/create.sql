-- B1 DEPENDS: AFTER:PT:PROCESS_END

CREATE PROCEDURE TmSp_CreateVBINView
language sqlscript
SQL SECURITY INVOKER
AS
  sql1    nvarchar(4999);
  rowNum  int;
BEGIN
	  SELECT COUNT(*) INTO rowNum FROM "PUBLIC"."VIEWS" WHERE "VIEW_NAME" = 'VBIN' AND "SCHEMA_NAME" = current_schema;
	  IF :rowNum > 0
	  THEN
	    sql1 := 'DROP VIEW  "'||current_schema||'"."VBIN"';
	    EXEC(:sql1);
	  END IF;
	  
	  SELECT COUNT(*) INTO rowNum FROM "PUBLIC"."VIEWS" WHERE "VIEW_NAME" = 'VBIN_Qty' AND "SCHEMA_NAME" = current_schema;
	  IF :rowNum > 0
	  THEN
	    sql1 := 'DROP VIEW  "'||current_schema||'"."VBIN_Qty"';
	    EXEC(:sql1);
	  END IF;
	
	sql1 := 'create view  "'||current_schema||'"."VBIN_Qty" '||
		'as  '||
		'SELECT T0."BinAbs", SUM(T0."OnHandQty") AS "ItemQty", COUNT(DISTINCT T0."ItemCode") AS "ItemsNum", 
		COUNT(DISTINCT T0."ItemCode") AS "SnBNum", COUNT(DISTINCT T0."ItemCode") AS "ItemWeight" 
		FROM "OIBQ" T0 
		WHERE T0."OnHandQty" <> (0 ) 
		GROUP BY T0."BinAbs" 
		';
	EXEC (:sql1);
	sql1 := '
		create view  "'||current_schema||'"."VBIN" '||
		'as  '||
		'SELECT T0.*,T1.*
		FROM "OBIN" T0 
		LEFT OUTER JOIN "VBIN_Qty" T1 ON T1."BinAbs" = T0."AbsEntry"
	'
	;
	EXEC (:sql1);
END


