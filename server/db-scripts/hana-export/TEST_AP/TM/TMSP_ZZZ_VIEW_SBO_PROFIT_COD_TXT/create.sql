-- B1 DEPENDS: AFTER:PT:PROCESS_END

create procedure TmSp_ZZZ_VIEW_SBO_PROFIT_COD_TXT
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER
AS
cnt int;
sqlStr nvarchar(4000);
BEGIN
	select count (*) into cnt from "PUBLIC"."VIEWS" where "VIEW_NAME" = 'SBO_PROFIT_COD_TXT' AND "SCHEMA_NAME" = CURRENT_SCHEMA;
	if :cnt > 0 then
		sqlStr := ('drop view SBO_PROFIT_COD_TXT');
		exec (:sqlStr);
	end if;
	
	sqlStr := 'CREATE VIEW SBO_PROFIT_COD_TXT
				AS
				SELECT TO_CHAR(P."OcrCode") AS "SBOPFT_COD", 
				left(P."OcrName",20) AS "TXTSH"
				FROM OOCR AS P';
	exec (:sqlStr);
END;


