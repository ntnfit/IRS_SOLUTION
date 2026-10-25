-- B1 DEPENDS: AFTER:PT:PROCESS_END

create procedure TmSp_ZZZ_VIEW_B1_InvPostListILWithoutBinView
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
cnt int;
sqlStr nvarchar(20000);
BEGIN
	select count (*) into cnt from "PUBLIC"."VIEWS" where "VIEW_NAME" = 'B1_InvPostListILWithoutBinView' AND "SCHEMA_NAME" = CURRENT_SCHEMA;
	if :cnt > 0 then
		sqlStr := ('drop view "B1_InvPostListILWithoutBinView"');
		exec (:sqlStr);
	end if;
	
	sqlStr := 'CREATE VIEW "B1_InvPostListILWithoutBinView"
AS
(SELECT
	 T0."LogEntry",
	 T0."ApplyType",
	 T0."ApplyEntry",
	 T0."ApplyLine",
	 T0."AppSubLine",
	 T0."LocCode",
	 T0."ItemCode",
	 T0."ManagedBy",
	 T1."MdAbsEntry",
	 T1."Quantity" 
	FROM "' || CURRENT_SCHEMA || '"."OITL" T0 
	LEFT OUTER JOIN "' || CURRENT_SCHEMA || '"."ITL1" T1 ON T0."LogEntry" = T1."LogEntry" 
	WHERE (T0."DefinedQty" <> (0) or (T0."ApplyType" = (10000071) and T1."LogEntry" is not null)) 
	AND T0."StockEff" = (1 )) 
UNION ALL (SELECT
	 MIN(T0."LogEntry") AS "LogEntry",
	 T0."ApplyType",
	 T0."ApplyEntry",
	 T0."ApplyLine",
	 T0."AppSubLine",
	 T0."LocCode",
	 MIN(T0."ItemCode") AS "ItemCode",
	 MAX(T0."ManagedBy") AS "ManagedBy",
	 -1 AS "MdAbsEntry",
	 MAX(T0."DocQty") - SUM(T1."Quantity") AS "Quantity" 
	FROM "' || CURRENT_SCHEMA || '"."OITL" T0 
	LEFT OUTER JOIN "' || CURRENT_SCHEMA || '"."ITL1" T1 ON T0."LogEntry" = T1."LogEntry" 
	WHERE T0."StockQty" <> T0."DefinedQty" 
	AND T0."StockEff" = (1 ) 
	AND T0."DocQty" > (0 ) 
	GROUP BY T0."ApplyType",
	 T0."ApplyEntry",
	 T0."ApplyLine",
	 T0."AppSubLine",
	 T0."LocCode" HAVING MAX(T0."DocQty") <> SUM(T1."Quantity")) WITH READ ONLY';

  exec (:sqlStr);
	--ins_msg_proc(:sqlStr);
END;


