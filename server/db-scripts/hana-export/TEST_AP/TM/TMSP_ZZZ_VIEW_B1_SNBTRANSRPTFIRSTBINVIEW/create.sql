-- B1 DEPENDS: AFTER:PT:PROCESS_END

create procedure TmSp_ZZZ_VIEW_B1_SnBTransRptFirstBinView
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
cnt int;
sqlStr nvarchar(20000);
BEGIN
	select count (*) into cnt from "PUBLIC"."VIEWS" where "VIEW_NAME" = 'B1_SnBTransRptFirstBinView' AND "SCHEMA_NAME" = CURRENT_SCHEMA;
	if :cnt > 0 then
		sqlStr := ('drop view "B1_SnBTransRptFirstBinView"');
		exec (:sqlStr);
	end if;
	
	sqlStr := 'CREATE VIEW "B1_SnBTransRptFirstBinView"
AS
SELECT
	 T0."ITLEntry",
	 T0."SnBMDAbs",
	 MIN(T1."BinCode") AS "BinCode",
	 COUNT(DISTINCT T1."AbsEntry") AS "SnBTransRptFirstBinViewBinCount" 
FROM "' || CURRENT_SCHEMA || '"."OBTL" T0 
INNER JOIN "' || CURRENT_SCHEMA || '"."OBIN" T1 ON T0."BinAbs" = T1."AbsEntry" 
WHERE T0."ITLEntry" <> (-1 ) 
GROUP BY T0."ITLEntry",
	 T0."SnBMDAbs" WITH READ ONLY';

  exec (:sqlStr);
	--ins_msg_proc(:sqlStr);
END;


