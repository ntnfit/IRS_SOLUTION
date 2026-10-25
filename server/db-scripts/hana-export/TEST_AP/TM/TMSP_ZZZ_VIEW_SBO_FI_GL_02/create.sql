-- B1 DEPENDS: AFTER:PT:PROCESS_END

create procedure TmSp_ZZZ_VIEW_SBO_FI_GL_02
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER
AS
cnt int;
sqlStr nvarchar(5000);
BEGIN
	select count (*) into cnt from "PUBLIC"."VIEWS" where "VIEW_NAME" = 'SBO_FI_GL_02' AND "SCHEMA_NAME" =  CURRENT_SCHEMA;
	if :cnt > 0 then
		sqlStr := ('drop view SBO_FI_GL_02');
		exec (:sqlStr);
	end if;

	
	sqlStr := 'CREATE VIEW SBO_FI_GL_02			
		AS			
		SELECT
			J."TransId" as "AC_DOC_NO",
			J."Line_ID" as "ITEM_NUM",
			left(J."Account",10) as "GL_ACCOUNT", 
			left(IFNULL(A."AccntntCod",''DEFT_ACCT''),10)  as "SBO_ALTACT", 
			J."Debit" as "DEBIT_LC", 		
			J."Credit" as "CREDIT_LC", 		
			CASE (SELECT Top 1 C."MainCurncy" from "' || CURRENT_SCHEMA || '"."OADM" as C) 		
	          		WHEN ''€'' THEN ''EUR''	
					WHEN ''Eur'' THEN ''EUR''
					WHEN ''$'' THEN ''USD''
	          		ELSE (SELECT Top 1 C."MainCurncy" from "' || CURRENT_SCHEMA || '"."OADM" as C)
			      	END  AS "LOC_CURRCY",	
			J."SYSCred" as "SBOCRD_SC",
			J."SYSDeb" as "SBODBT_SC",
			CASE (SELECT Top 1 C."SysCurrncy" from "' || CURRENT_SCHEMA || '"."OADM" as C) 		
 	      		WHEN ''€'' THEN ''EUR''	
				WHEN ''Eur'' THEN ''EUR''
				WHEN ''$'' THEN ''USD''
          		ELSE (SELECT Top 1 C."MainCurncy" from "' || CURRENT_SCHEMA || '"."OADM" as C)
		      	END  AS "SBOSYS_CUR",	
			J."FCDebit" as "DEBIT_DC",
			J."FCCredit" as "CREDIT_DC",
			CASE 	J."FCCurrency"
          		WHEN ''€'' THEN ''EUR''	
				WHEN ''Eur'' THEN ''EUR''
				WHEN ''$'' THEN ''USD''
          		ELSE J."FCCurrency"
      		END  AS "DOC_CURRCY",	
			(SELECT SUM(J2."Debit"-J2."Credit") from "' || CURRENT_SCHEMA || '"."JDT1" as J2 where (TO_INT(J2."TransId")+TO_INT(J2."Line_ID"))<=(TO_INT(J."TransId")+TO_INT(J."Line_ID")) and J2."Account"=J."Account") as "BALANCE",	
			TO_CHAR(YEAR(J."DueDate"))		
	       		|| case LENGTH(MONTH(J."DueDate"))	
          			when 1 then ''0'' || TO_CHAR(MONTH(J."DueDate"))
          			else TO_CHAR(MONTH(J."DueDate"))
        		 end	
    	  		|| case LENGTH(TO_CHAR(EXTRACT(DAY FROM J."DueDate")))	
          			when 1 then ''0'' || TO_CHAR(EXTRACT(DAY FROM J."DueDate"))
          			else TO_CHAR(EXTRACT(DAY FROM J."DueDate"))
        		end  	
			as "NETDUEDATE",	
			J."ContraAct" AS 	"REF_DOC_AA",
			TO_INT(J."TransType") as "SBOTRN_TYP",
			TO_CHAR(YEAR(J."RefDate"))		
       		|| case LENGTH(MONTH(J."RefDate"))
          			when 1 then ''0'' || TO_CHAR(MONTH(J."RefDate"))
          			else TO_CHAR(MONTH(J."RefDate"))
        		 end	
      		|| case LENGTH(TO_CHAR(EXTRACT(DAY FROM J."RefDate")))	
          			when 1 then ''0'' || TO_CHAR(EXTRACT(DAY FROM J."RefDate"))
          			else TO_CHAR(EXTRACT(DAY FROM J."RefDate"))
       		end  	
			as "PSTNG_DATE",	
			J."Ref1" AS "REF_KEY1",
			J."Ref2" AS "REF_KEY2",
			J."CreatedBy" AS "REF_KEY3",
			J."BaseRef" AS "REF_DOC_NO",
			J."Project" AS "OPP_REF",
			J."TransCode" AS "SBOTRN_COD",
			J."ProfitCode" AS "SBOPFT_COD",
			TO_CHAR(YEAR(J."TaxDate"))		
       		|| case LENGTH(MONTH(J."TaxDate"))
          			when 1 then ''0'' || TO_CHAR(MONTH(J."TaxDate"))
          			else TO_CHAR(MONTH(J."TaxDate"))
        		 end	
      		|| case LENGTH(TO_CHAR(EXTRACT(DAY FROM J."TaxDate")))
          			when 1 then ''0'' || TO_CHAR(EXTRACT(DAY FROM J."TaxDate"))
          			else TO_CHAR(EXTRACT(DAY FROM J."TaxDate"))
        		end  	
			as "SBOTX_DATE",
			J."BatchNum" AS "BATCH",
			J."VatGroup" AS "SBOVAT_GRP",
			J."BaseSum" AS "ID_BASE",
			J."VatRate" AS "ID_TXRATE",
			J."Indicator" AS "BBP_FLAG",
			J."AdjTran" AS "SBOADJ_IND",		/* Adj Transaction  Indicators */
			J."RevSource" AS "SBOREV_IND", 	 /* Reverse Sources Indicators */
			CASE (SELECT Top 1 C."MainCurncy" from "' || CURRENT_SCHEMA || '"."OADM" as C) 		
        		WHEN ''€'' THEN ''EUR''	
				WHEN ''Eur'' THEN ''EUR''
				WHEN ''$'' THEN ''USD''
       		ELSE (SELECT Top 1 C."MainCurncy" from "' || CURRENT_SCHEMA || '"."OADM" as C)
      		END		
	 		AS "CURRENCY"
			FROM "' || CURRENT_SCHEMA || '"."JDT1" AS J 
			INNER JOIN	"' || CURRENT_SCHEMA || '"."OACT" AS A ON J."Account"=A."AcctCode"';	
	
--	ins_msg_proc (:sqlStr);
	exec (:sqlStr);	
END;


