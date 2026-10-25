-- B1 DEPENDS: AFTER:PT:PROCESS_END

create procedure TmSp_ZZZ_VIEW_B1_InvPostListILWithBinView
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
cnt int;
sqlStr nvarchar(20000);
BEGIN
	select count (*) into cnt from "PUBLIC"."VIEWS" where "VIEW_NAME" = 'B1_InvPostListILWithBinView' AND "SCHEMA_NAME" = CURRENT_SCHEMA;
	if :cnt > 0 then
		sqlStr := ('drop view "B1_InvPostListILWithBinView"');
		exec (:sqlStr);
	end if;
	
	sqlStr := 'CREATE VIEW "B1_InvPostListILWithBinView"
AS

(SELECT
	 MIN(T0."LogEntry") AS "LogEntry",
	 T0."ApplyType",
	 T0."ApplyEntry",
	 T0."ApplyLine",
	 T0."AppSubLine",
	 MIN(T0."LocCode") AS "LocCode",
	 MIN(T0."ItemCode") AS "ItemCode",
	 MIN(T0."ManagedBy") AS "ManagedBy",
	 MAX(T0."StockQty") AS "StockQty",
	 -1 AS "MdAbsEntry",
	 MAX(T2."Quantity") AS "Quantity",
	 MAX(T2."Quantity") - SUM(T2."Quantity") AS "TmpQty",
	 MIN(T2."AbsEntry") AS "AbsEntry",
	 MIN(T2."MessageID") AS "MessageID",
	 T2."BinAbs" 
	FROM "' || CURRENT_SCHEMA || '"."OITL" T0 
	LEFT OUTER JOIN "' || CURRENT_SCHEMA || '"."ITL1" T1 ON T0."LogEntry" = T1."LogEntry" 
	LEFT OUTER JOIN "' || CURRENT_SCHEMA || '"."OBTL" T2 ON (T0."LogEntry" = T2."ITLEntry" 
		AND T1."LogEntry" IS NULL ) 
	OR (T1."LogEntry" IS NOT NULL 
		AND T0."LogEntry" = T2."ITLEntry" 
		AND T1."MdAbsEntry" = T2."SnBMDAbs" ) 
	WHERE T2."AbsEntry" IS NOT NULL 
	AND T0."StockEff" = (1 ) 
	AND T0."StockQty" <> T0."DefinedQty" 
	AND T0."DocQty" > (0 ) 
	AND T0."ApplyType" <> (310000001 ) 
	AND T0."ApplyType" <> (10000071 ) 
	GROUP BY T0."ApplyType",
	 T0."ApplyEntry",
	 T0."ApplyLine",
	 T0."AppSubLine",
	 T2."BinAbs") 
UNION ALL (SELECT
	 T0."LogEntry",
	 T0."ApplyType",
	 T0."ApplyEntry",
	 T0."ApplyLine",
	 T0."AppSubLine",
	 T0."LocCode",
	 T0."ItemCode",
	 T0."ManagedBy",
	 T0."StockQty",
	 T1."MdAbsEntry",
	 T1."Quantity",
	 0 AS "TmpQty",
	 T2."AbsEntry",
	 T2."MessageID",
	 T2."BinAbs" 
	FROM "' || CURRENT_SCHEMA || '"."OITL" T0 
	LEFT OUTER JOIN "' || CURRENT_SCHEMA || '"."ITL1" T1 ON T0."LogEntry" = T1."LogEntry" 
	LEFT OUTER JOIN "' || CURRENT_SCHEMA || '"."OBTL" T2 ON T2."ITLEntry" = T1."LogEntry" 
	AND T2."SnBMDAbs" = T1."MdAbsEntry" 
	WHERE (T0."DefinedQty" <> (0) or (T0."ApplyType" = (10000071) and T1."LogEntry" is not null)) 
	AND T0."StockEff" = (1 ) 
	AND T2."AbsEntry" IS NULL) 
UNION ALL (SELECT
	 T0."LogEntry",
	 T0."ApplyType",
	 T0."ApplyEntry",
	 T0."ApplyLine",
	 T0."AppSubLine",
	 T0."LocCode",
	 T0."ItemCode",
	 T0."ManagedBy",
	 T0."StockQty",
	 T1."MdAbsEntry",
	 T2."Quantity",
	 0 AS "TmpQty",
	 T2."AbsEntry",
	 T2."MessageID",
	 T2."BinAbs" 
	FROM "' || CURRENT_SCHEMA || '"."OITL" T0 
	LEFT OUTER JOIN "' || CURRENT_SCHEMA || '"."ITL1" T1 ON T0."LogEntry" = T1."LogEntry" 
	LEFT OUTER JOIN "' || CURRENT_SCHEMA || '"."OBTL" T2 ON T2."ITLEntry" = T1."LogEntry" 
	AND T2."SnBMDAbs" = T1."MdAbsEntry" 
	WHERE (T0."DefinedQty" <> (0) or (T0."ApplyType" = (10000071) and T1."LogEntry" is not null)) 
	AND T0."StockEff" = (1 ) 
	AND T2."AbsEntry" IS NOT NULL) 
UNION ALL (SELECT
	 MIN(T0."LogEntry") AS "LogEntry",
	 T0."ApplyType",
	 T0."ApplyEntry",
	 T0."ApplyLine",
	 T0."AppSubLine",
	 T0."LocCode",
	 MIN(T0."ItemCode") AS "ItemCode",
	 MIN(T0."ManagedBy") AS "ManagedBy",
	 MAX(T0."StockQty") AS "StockQty",
	 -1 AS "MdAbsEntry",
	 MAX(T0."DocQty") - SUM(T1."Quantity") AS "Quantity",
	 0 AS "TmpQty",
	 MIN(T2."AbsEntry") AS "AbsEntry",
	 MIN(T2."MessageID") AS "MessageID",
	 MIN(T2."BinAbs") AS "BinAbs" 
	FROM "' || CURRENT_SCHEMA || '"."OITL" T0 
	LEFT OUTER JOIN "' || CURRENT_SCHEMA || '"."ITL1" T1 ON T0."LogEntry" = T1."LogEntry" 
	LEFT OUTER JOIN "' || CURRENT_SCHEMA || '"."OBTL" T2 ON T2."ITLEntry" = T1."LogEntry" 
	AND T2."SnBMDAbs" = T1."MdAbsEntry" 
	WHERE T0."StockQty" <> T0."DefinedQty" 
	AND T0."StockEff" = (1 ) 
	AND T0."DocQty" > (0 ) 
	AND T2."AbsEntry" IS NULL 
	GROUP BY T0."ApplyType",
	 T0."ApplyEntry",
	 T0."ApplyLine",
	 T0."AppSubLine",
	 T0."LocCode" HAVING MAX(T0."DocQty") <> SUM(T1."Quantity")) WITH READ ONLY';

  exec (:sqlStr);
	--ins_msg_proc(:sqlStr);
END;


