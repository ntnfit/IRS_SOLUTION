-- B1 DEPENDS: AFTER:PT:PROCESS_END

create procedure TmSp_ZZZ_VIEW_B1_SnBOpenQtyViewForWTR
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
cnt int;
sqlStr nvarchar(20000);
BEGIN
	select count (*) into cnt from "PUBLIC"."VIEWS" where "VIEW_NAME" = 'B1_SnBOpenQtyViewForWTR' AND "SCHEMA_NAME" = CURRENT_SCHEMA;
	if :cnt > 0 then
		sqlStr := ('drop view "B1_SnBOpenQtyViewForWTR"');
		exec (:sqlStr);
	end if;
	
	sqlStr := 'CREATE VIEW "B1_SnBOpenQtyViewForWTR"
AS
SELECT
	 T0."ApplyType" AS "DocType",
	 T0."ApplyEntry" AS "DocEntry",
	 T0."ApplyLine" AS "DocLine",
	 T0."AppSubLine" AS "DocSubLine",
	 T0."ManagedBy" AS "ManagedBy",
	 T0."ItemCode" AS "ItemCode",
	 T0."SysNumber" AS "SysNumber",
	 T0."LocCode" AS "LocCode",
	 SUM(T0."Quantity") AS "Quantity" 
FROM "' || CURRENT_SCHEMA || '"."B1_SnBOpenQtyInnerView" T0 
GROUP BY T0."ApplyType",
	 T0."ApplyEntry",
	 T0."ApplyLine",
	 T0."AppSubLine",
	 T0."ManagedBy",
	 T0."ItemCode",
	 T0."SysNumber",
	 T0."LocCode",
	 T0."TransId" HAVING SUM(T0."Quantity") <> (0 ) WITH READ ONLY';

  exec (:sqlStr);
	--ins_msg_proc(:sqlStr);
END;


