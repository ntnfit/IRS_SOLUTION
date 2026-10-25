-- B1 DEPENDS: AFTER:PT:PROCESS_END

CREATE PROCEDURE TmSp_CreateVTSPView
language sqlscript
SQL SECURITY INVOKER
AS
  sqlStr    nvarchar(4999);
  rowNum  int;
BEGIN
	  SELECT COUNT(*) INTO rowNum FROM "PUBLIC"."VIEWS" WHERE "VIEW_NAME" = 'VTSP' AND "SCHEMA_NAME" = current_schema;
	  IF :rowNum > 0
	  THEN
	    sqlStr := 'DROP VIEW  "'||current_schema||'"."VTSP"';
	    EXEC(:sqlStr);
	  END IF;
	
	sqlStr := '
		create view  "'||current_schema||'"."VTSP" '||
		'as  '||
		'SELECT T0."AbsEntry", T0."TransCode", T0."TransName", T0."TransID", IFNULL(T1."LineNum", 0) as "LineNum", T2."ModeName" as "TransMode", T3."TypeName" as "VehicleTyp", T1."VehicleNo"
		FROM "OTSP" T0 
		LEFT OUTER JOIN "TSP1" T1 ON T1."AbsEntry" = T0."AbsEntry"
		LEFT OUTER JOIN "OETM" t2 ON T2."ModeCode" = t1."TransMode"
		LEFT OUTER JOIN "OEVT" t3 ON T3."TypeCode" = t1."VehicleTyp"
	'
	;
	EXEC (:sqlStr);
END


