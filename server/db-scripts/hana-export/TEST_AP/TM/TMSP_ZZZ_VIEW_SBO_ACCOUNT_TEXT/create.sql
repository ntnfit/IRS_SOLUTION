-- B1 DEPENDS: AFTER:PT:PROCESS_END

create procedure TmSp_ZZZ_VIEW_SBO_ACCOUNT_TEXT
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
cnt int;
sqlStr nvarchar(4000);
BEGIN
	select count (*) into cnt from "PUBLIC"."VIEWS" where "VIEW_NAME" = 'SBO_ACCOUNT_TEXT' AND "SCHEMA_NAME" =  CURRENT_SCHEMA;
	if :cnt > 0 then
		sqlStr := ('drop view SBO_ACCOUNT_TEXT');
		exec (:sqlStr);
	end if;
	
	sqlStr := 'CREATE VIEW "SBO_ACCOUNT_TEXT"
				AS
				SELECT "OACT"."AcctCode" AS "GL_ACCOUNT",
				left("OACT". "AcctName",20) AS "TXTSH", 
				left("OACT"."AcctName",60) AS "TXTLG"
				FROM "' || CURRENT_SCHEMA || '"."OACT"  WHERE ("OACT"."Postable" = ''Y'')';
	
	--ins_msg_proc (:sqlStr);
	exec (:sqlStr);
END;


