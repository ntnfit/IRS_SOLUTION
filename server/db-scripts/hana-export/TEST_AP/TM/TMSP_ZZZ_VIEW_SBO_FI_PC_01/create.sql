-- B1 DEPENDS: AFTER:PT:PROCESS_END

create procedure TmSp_ZZZ_VIEW_SBO_FI_PC_01
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER
AS
cnt int;
sqlStr nvarchar(4000);
BEGIN
	select count (*) into cnt from "PUBLIC"."VIEWS" where "VIEW_NAME" = 'SBO_FI_PC_01' AND "SCHEMA_NAME" = CURRENT_SCHEMA;
	if :cnt > 0 then
		sqlStr := ('drop view SBO_FI_PC_01');
		exec (:sqlStr);
	end if;
	
	sqlStr := 'CREATE VIEW SBO_FI_PC_01
				AS
				SELECT TO_CHAR(P."PrcCode") as "PROFIT_CTR",
	 			TO_CHAR(P."OcrCode") as "SBOPFT_COD",
	 			P."Direct"  as "CRM_DIRECT",
	 			J."TransId" as "AC_DOC_NO",
	 			left(J."Account", 10) as "GL_ACCOUNT",
				TO_CHAR(YEAR(J."RefDate"))		
       				|| case LENGTH(MONTH(J."RefDate"))	
          			when 1 then ''0'' || TO_CHAR(MONTH(J."RefDate"))
          			else TO_CHAR(MONTH(J."RefDate"))
        		 end	
      				|| case LENGTH(TO_CHAR(EXTRACT(DAY FROM J."RefDate")))
          			when 1 then ''0'' || TO_CHAR(EXTRACT(DAY FROM J."RefDate"))
          			else TO_CHAR(EXTRACT(DAY FROM J."RefDate"))
        		end  	
				as "CALDAY",
				A."ActType" as "ACCT_TYPE",
				J."Debit" * (P."PrcAmount"/P."OcrTotal") as "DEBIT",
				J."Credit" * (P."PrcAmount"/P."OcrTotal") as "CREDIT",
				CASE A."ActCurr" 		
          			WHEN ''€'' THEN ''EUR''	
					WHEN ''##'' THEN ''EUR''
					WHEN ''$'' THEN ''USD''
          			ELSE A."ActCurr" 	
      			END		
				AS "CURRENCY"	
				FROM (OCR1 as P  INNER JOIN JDT1 as J  ON P."OcrCode"=J."ProfitCode")   
				INNER JOIN OACT as A ON A."AcctCode"=J."Account"';
	exec (:sqlStr);
END;


