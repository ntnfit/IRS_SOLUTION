-- B1 DEPENDS: AFTER:PT:PROCESS_END

create procedure TmSp_ZZZ_VIEW_SBO_CUSTOMER_TEXT
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER
AS
cnt int;
sqlStr nvarchar(4000);
BEGIN
	select count (*) into cnt from "PUBLIC"."VIEWS" where "VIEW_NAME" = 'SBO_CUSTOMER_TEXT' AND "SCHEMA_NAME" = CURRENT_SCHEMA;
	if :cnt > 0 then
		sqlStr := ('drop view SBO_CUSTOMER_TEXT');
		exec (:sqlStr);
	end if;
	
	sqlStr := 'CREATE VIEW SBO_CUSTOMER_TEXT
				AS
				SELECT     OCRD."CardCode" as "CARDCODE", OCRD."CardName" as "CARDNAME"
				FROM         "OCRD"
				WHERE "CardType" = ''C''';
	exec (:sqlStr);
END;


