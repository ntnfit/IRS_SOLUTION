-- B1 DEPENDS: AFTER:PT:PROCESS_END

create procedure TmSp_ZZZ_VIEW_SBO_PUR_GR_GRET
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER
AS
cnt int;
sqlStr nvarchar(10000);
sqlStr2 nvarchar(10000);
BEGIN
	select count (*) into cnt from "PUBLIC"."VIEWS" where "VIEW_NAME" = 'SBO_PUR_GR_GRET' AND "SCHEMA_NAME" = CURRENT_SCHEMA;
	if :cnt > 0 then
		sqlStr := ('drop view SBO_PUR_GR_GRET');
		exec (:sqlStr);
	end if;
	
	sqlStr := 'CREATE VIEW SBO_PUR_GR_GRET AS
				SELECT TO_CHAR(PDN1."DocEntry") as "DOCENTRY", 
				PDN1."LineNum" as "LINENUM",
      			'' '' as "RETURNID", 
      			PDN1."ItemCode" as "ITEMCODE", 
      			TO_CHAR(OPDN."CardCode") as "CARDCODE", 
      			TO_CHAR(YEAR(OPOR."DocDate")) 
       			|| case LENGTH(MONTH(OPOR."DocDate"))
          			when 1 then ''0'' || TO_CHAR(MONTH(OPOR."DocDate"))
          			else TO_CHAR(MONTH(OPOR."DocDate"))
         			end
      			|| case LENGTH(EXTRACT(DAY FROM OPOR."DocDate"))
          			when 1 then ''0'' || TO_CHAR(EXTRACT(DAY FROM OPOR."DocDate"))
          			else TO_CHAR(EXTRACT(DAY FROM OPOR."DocDate"))
        			end  AS PODOCDATE,
      			TO_CHAR(YEAR(OPDN."DocDate"))
       			|| case LENGTH(MONTH(OPDN."DocDate"))
         			 when 1 then ''0'' || TO_CHAR(MONTH(OPDN."DocDate"))
          			else TO_CHAR(MONTH(OPDN."DocDate"))
         			end
      			|| case LENGTH(EXTRACT(DAY FROM OPDN."DocDate"))
          			when 1 then ''0'' || TO_CHAR(EXTRACT(DAY FROM OPDN."DocDate"))
          			else TO_CHAR(EXTRACT(DAY FROM OPDN."DocDate"))
        			end  AS "DOCDATE",
       			TO_CHAR(YEAR(OPDN."CreateDate"))
       			|| case LENGTH(MONTH(OPDN."CreateDate"))
          			when 1 then ''0'' || TO_CHAR(MONTH(OPDN."CreateDate"))
          			else TO_CHAR(MONTH(OPDN."CreateDate"))
         		end
      			|| case LENGTH(EXTRACT(DAY FROM OPDN."CreateDate"))
          			when 1 then ''0'' || TO_CHAR(EXTRACT(DAY FROM OPDN."CreateDate"))
         			 else TO_CHAR(EXTRACT(DAY FROM OPDN."CreateDate"))
        			end  AS "CREATEDATE",
       			TO_CHAR(YEAR(OPDN."UpdateDate"))
       			|| case LENGTH(MONTH(OPDN."UpdateDate"))
         			when 1 then ''0'' || TO_CHAR(MONTH(OPDN."UpdateDate"))
          			else TO_CHAR(MONTH(OPDN."UpdateDate"))
         			end
      			|| case LENGTH(EXTRACT(DAY FROM OPDN."UpdateDate"))
          			when 1 then ''0'' || TO_CHAR(EXTRACT(DAY FROM OPDN."UpdateDate"))
         			else TO_CHAR(EXTRACT(DAY FROM OPDN."UpdateDate"))
        			end  AS "UPDATEDATE",
      				ROUND(PDN1."LineTotal" * (1 - OPDN."DiscPrcnt" / 100) + PDN1."VatSum", 2) AS "GRVALUE", 
     			 CASE PDN1."UseBaseUn" 
          			WHEN ''Y'' THEN PDN1."Quantity" 
          			WHEN ''N'' THEN PDN1."Quantity" * OITM."NumInBuy" 
      			END  AS "GRQUANTITY", 
      			CASE (SELECT Top 1 "MainCurncy" FROM "' || CURRENT_SCHEMA || '"."OADM")
          			WHEN ''Eur'' THEN ''EUR''
          			WHEN ''$'' THEN ''USD''    
          			WHEN ''€'' THEN ''EUR''
          			ELSE (SELECT Top 1 "MainCurncy" FROM "' || CURRENT_SCHEMA || '"."OADM") 
      			END AS "CURRENCY", 
      			CASE OITM."BuyUnitMsr"
          			WHEN ''Centimeter'' THEN ''CM''
          			WHEN ''CM'' THEN ''CM''
          			WHEN ''Gram'' THEN ''G''
          			WHEN ''G'' THEN ''G''
          			WHEN ''Kilogram'' THEN ''KG''
          			WHEN ''KG'' THEN ''KG''
          			WHEN ''Liter'' THEN ''L''
          			WHEN ''L'' THEN ''L''
          			WHEN ''Meter'' THEN ''M''
          			WHEN ''M'' THEN ''M''
          			WHEN ''Tonnes'' THEN ''TO''
          			WHEN ''Ton'' THEN ''TO''
          			WHEN ''TO'' THEN ''TO''
          			ELSE ''PC''
      			End AS "BASEUNIT"
			FROM "' || CURRENT_SCHEMA || '"."PDN1" 
			INNER JOIN "' || CURRENT_SCHEMA || '"."OPDN" ON PDN1."DocEntry" = OPDN."DocEntry" 
			INNER JOIN "' || CURRENT_SCHEMA || '"."OITM" ON PDN1."ItemCode" = OITM."ItemCode" 
			INNER JOIN "' || CURRENT_SCHEMA || '"."OPOR" ON PDN1."BaseEntry" = OPOR."DocEntry"
			WHERE OPDN."DocType" = ''I'' and PDN1."BaseType" = ''22''';
		
		sqlStr := :sqlStr || 
			'UNION SELECT TO_CHAR(RPD1."DocEntry") as "DOCENTRY", RPD1."LineNum" as "LINENUM",
      		''X'' as "RETURNID", 
      		RPD1."ItemCode" as "ITEMCODE", TO_CHAR(ORPD."CardCode") as "CARDCODE", 
       		TO_CHAR(YEAR(OPOR."DocDate"))
       		|| case LENGTH(MONTH(OPOR."DocDate"))
          		when 1 then ''0'' || TO_CHAR(MONTH(OPOR."DocDate"))
          		else TO_CHAR(MONTH(OPOR."DocDate"))
         		end
      		|| case LENGTH(EXTRACT(DAY FROM OPOR."DocDate"))
          		when 1 then ''0'' || TO_CHAR(EXTRACT(DAY FROM OPOR."DocDate"))
          		else TO_CHAR(EXTRACT(DAY FROM OPOR."DocDate"))
        		end  AS "PODOCDATE",
     		 TO_CHAR(YEAR(ORPD."DocDate"))
       		|| case LENGTH(MONTH(ORPD."DocDate"))
          		when 1 then ''0'' || TO_CHAR(MONTH(ORPD."DocDate"))
          		else TO_CHAR(MONTH(ORPD."DocDate"))
         		end
      		|| case LENGTH(EXTRACT(DAY FROM ORPD."DocDate"))
          		when 1 then ''0'' || TO_CHAR(EXTRACT(DAY FROM ORPD."DocDate"))
          		else TO_CHAR(EXTRACT(DAY FROM ORPD."DocDate"))
        		end  AS "DOCDATE",
       		TO_CHAR(YEAR(ORPD."CreateDate"))
       		|| case LENGTH(MONTH(ORPD."CreateDate"))
         		when 1 then ''0'' || TO_CHAR(MONTH(ORPD."CreateDate"))
          		else TO_CHAR(MONTH(ORPD."CreateDate"))
         		end
      		|| case LENGTH(EXTRACT(DAY FROM ORPD."CreateDate"))
          		when 1 then ''0'' || TO_CHAR(EXTRACT(DAY FROM ORPD."CreateDate"))
          		else TO_CHAR(EXTRACT(DAY FROM ORPD."CreateDate"))
        		end  AS "CREATEDATE",
       		TO_CHAR(YEAR(ORPD."UpdateDate"))
       		|| case LENGTH(MONTH(ORPD."UpdateDate"))
          		when 1 then ''0'' || TO_CHAR(MONTH(ORPD."UpdateDate"))
          		else TO_CHAR(MONTH(ORPD."UpdateDate"))
         		end
      		|| case LENGTH(EXTRACT(DAY FROM ORPD."UpdateDate"))
          		when 1 then ''0'' || TO_CHAR(EXTRACT(DAY FROM ORPD."UpdateDate"))
          		else TO_CHAR(EXTRACT(DAY FROM ORPD."UpdateDate"))
        		end  AS "UPDATEDATE",
      		ROUND(RPD1."LineTotal" * (1 - ORPD."DiscPrcnt" / 100) + RPD1."VatSum", 2) AS "GRVALUE", 
      		CASE RPD1."UseBaseUn" 
          		WHEN ''Y'' THEN RPD1."Quantity" 
          		WHEN ''N'' THEN RPD1."Quantity" * OITM."NumInBuy" 
     	 	END  AS "GRQUANTITY", 
      		CASE (SELECT Top 1 "MainCurncy" FROM "' || CURRENT_SCHEMA || '"."OADM")
          		WHEN ''Eur'' THEN ''EUR''
          		WHEN ''$'' THEN ''USD''    
          		WHEN ''€'' THEN ''EUR''
          		ELSE (SELECT Top 1 "MainCurncy" FROM "' || CURRENT_SCHEMA || '"."OADM") 
      		END AS "CURRENCY", 
      		CASE OITM."BuyUnitMsr"
          		WHEN ''Centimeter'' THEN ''CM''
          			WHEN ''CM'' THEN ''CM''
          		WHEN ''Gram'' THEN ''G''
          		WHEN ''G'' THEN ''G''
          		WHEN ''Kilogram'' THEN ''KG''
          		WHEN ''KG'' THEN ''KG''
          		WHEN ''Liter'' THEN ''L''
          		WHEN ''L'' THEN ''L''
          		WHEN ''Meter'' THEN ''M''
          		WHEN ''M'' THEN ''M''
         		WHEN ''Tonnes'' THEN ''TO''
          		WHEN ''Ton'' THEN ''TO''
          		WHEN ''TO'' THEN ''TO''
          		ELSE ''PC''
      		End AS "BASEUNIT"
		FROM "' || CURRENT_SCHEMA || '"."RPD1" 
		INNER JOIN "' || CURRENT_SCHEMA || '"."ORPD" ON RPD1."DocEntry" = ORPD."DocEntry" 
		INNER JOIN "' || CURRENT_SCHEMA || '"."OITM" ON RPD1."ItemCode" = OITM."ItemCode" 
		INNER JOIN "' || CURRENT_SCHEMA || '"."PDN1" ON RPD1."BaseEntry" = PDN1."DocEntry" AND RPD1."BaseLine" = PDN1."LineNum" 
		INNER JOIN "' || CURRENT_SCHEMA || '"."OPOR" ON PDN1."BaseEntry" = OPOR."DocEntry"
	WHERE ORPD."DocType" = ''I'' and RPD1."BaseType" = ''20''';
	
    exec (:sqlStr);
	--ins_msg_proc(:sqlStr);
END;


