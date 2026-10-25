-- B1 DEPENDS: AFTER:PT:PROCESS_END

create procedure TmSp_ZZZ_VIEW_SBO_FI_GL_01
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER
AS
cnt int;
sqlStr nvarchar(4000);
BEGIN
	select count (*) into cnt from "PUBLIC"."VIEWS" where "VIEW_NAME" = 'SBO_FI_GL_01' AND "SCHEMA_NAME" =  CURRENT_SCHEMA;
	if :cnt > 0 then
		sqlStr := ('drop view SBO_FI_GL_01');
		exec (:sqlStr);
	end if;
	
	sqlStr := 'CREATE VIEW SBO_FI_GL_01		
				AS			
				SELECT 
				DISTINCT
				left(J."Account",10) as "GL_ACCOUNT",
				left(IFNULL(A."AccntntCod",''DEFT_ACCT''),10)  as "SBO_ALTACT",
				(SELECT SUM(J2."Debit") as "DEBIT" from "' || CURRENT_SCHEMA || '"."JDT1" as J2 where (J2."FinncPriod"=J."FinncPriod") and J2."Account"=J."Account")  as "DEBIT", 
				(SELECT SUM(J2."Credit") from "' || CURRENT_SCHEMA || '"."JDT1" as J2 where (J2."FinncPriod"=J."FinncPriod") and J2."Account"=J."Account")  as "CREDIT", 		
				(SELECT SUM(J2."Debit"-J2."Credit") from "' || CURRENT_SCHEMA || '"."JDT1" as J2 where (J2."FinncPriod"<=J."FinncPriod") and J2."Account"=J."Account") as "BALANCE",		
				''0'' || TO_CHAR(MONTH(J."RefDate")) || TO_CHAR(YEAR(J."RefDate")) as "FISCPER",
				CASE (SELECT Top 1 C."MainCurncy" from "' || CURRENT_SCHEMA || '"."OADM" as C) 		
          			WHEN ''€'' THEN ''EUR''	
					WHEN ''Eur'' THEN ''EUR''
					WHEN ''$'' THEN ''USD''
          			ELSE (SELECT Top 1 C."MainCurncy" from "' || CURRENT_SCHEMA || '"."OADM" as C)
      			END		
	 			AS "CURRENCY",
				''10'' AS "CURTYPE"		
				FROM "' || CURRENT_SCHEMA || '"."JDT1" as J 
				INNER JOIN "' || CURRENT_SCHEMA || '"."OACT" as A ON J."Account"=A."AcctCode"';
 	
 	exec (:sqlStr);	
END;


