-- B1 DEPENDS: AFTER:PT:PROCESS_END

create procedure TmSp_ZZZ_VIEW_SBO_PROFIT_CTR_ATTR
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER
AS
cnt int;
sqlStr nvarchar(4000);
BEGIN
	select count (*) into cnt from "PUBLIC"."VIEWS" where "VIEW_NAME" = 'SBO_PROFIT_CTR_ATTR' AND "SCHEMA_NAME" = CURRENT_SCHEMA;
	if :cnt > 0 then
		sqlStr := ('drop view SBO_PROFIT_CTR_ATTR');
		exec (:sqlStr);
	end if;
	
	sqlStr := 'CREATE VIEW SBO_PROFIT_CTR_ATTR
				AS
				SELECT TO_CHAR(P."PrcCode") AS "PROFIT_CTR", 
				left(P."GrpCode",20) AS "SORT_COD",
				CASE (SELECT Top 1 C."MainCurncy" from "' || CURRENT_SCHEMA || '"."OADM" as C) 	
          			WHEN ''€'' THEN ''EUR''	
					WHEN ''Eur'' THEN ''EUR''
          			ELSE  (SELECT Top 1 C."MainCurncy" from "' || CURRENT_SCHEMA || '"."OADM" as C) 	
      			END		
				AS "OBJ_CURR"
				FROM "' || CURRENT_SCHEMA || '"."OPRC" AS P';
	exec (:sqlStr);
END;


