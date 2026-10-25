-- B1 DEPENDS: AFTER:PT:PROCESS_END

create procedure TmSp_ZZZ_VIEW_SBO_ACCOUNT_ATTR
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
cnt int;
sqlStr nvarchar(4000);
BEGIN
	select count (*) into cnt from "PUBLIC"."VIEWS" where "VIEW_NAME" = 'SBO_ACCOUNT_ATTR' AND "SCHEMA_NAME" =  CURRENT_SCHEMA;
	if :cnt > 0 then
		sqlStr := ('drop view SBO_ACCOUNT_ATTR');
		exec (:sqlStr);
	end if;
	
	sqlStr := 'CREATE VIEW "SBO_ACCOUNT_ATTR"
			AS
			SELECT left("OACT"."AcctCode",10) AS "GL_ACCOUNT", 
			CASE "OACT"."ActType"  
			WHEN ''E''  THEN ''E''
			WHEN ''I''  THEN ''I''
			END AS "INCST_FLAG",
			CASE "OACT"."ActType" 
				WHEN ''N''  THEN ''X''
			END AS "BAL_FLAG"
			FROM "' || CURRENT_SCHEMA || '"."OACT" WHERE ("Postable" = ''Y'')';
	
	--ins_msg_proc (:sqlStr);
	exec (:sqlStr);
END;


