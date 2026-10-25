-- B1 DEPENDS: AFTER:PT:PROCESS_END

create procedure TmSp_ZZZ_VIEW_SBO_ALTACT_ATTR
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER
AS
cnt int;
sqlStr nvarchar(4000);
BEGIN
	select count (*) into cnt from "PUBLIC"."VIEWS" where "VIEW_NAME" = 'SBO_ALTACT_ATTR' AND "SCHEMA_NAME" = CURRENT_SCHEMA;
	if :cnt > 0 then
		sqlStr := ('drop view SBO_ALTACT_ATTR');
		exec (:sqlStr);
	end if;
	
	sqlStr := 'CREATE VIEW SBO_ALTACT_ATTR
				AS
				SELECT 
				left(IFNULL("OACT"."AccntntCod",''DEFT_ACCT''),10)  AS "SBO_ALTACT_ATTR"
				FROM "OACT"     Where "OACT"."Postable" = ''Y''  GROUP BY "OACT"."AccntntCod"';
	
	--ins_msg_proc (:sqlStr);
	exec (:sqlStr);
END;


