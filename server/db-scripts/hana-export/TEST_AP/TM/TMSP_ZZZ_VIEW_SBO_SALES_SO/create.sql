-- B1 DEPENDS: AFTER:PT:PROCESS_END

create procedure TmSp_ZZZ_VIEW_SBO_SALES_SO
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER
AS
cnt int;
sqlStr nvarchar(20000);
BEGIN
	select count (*) into cnt from "PUBLIC"."VIEWS" where "VIEW_NAME" = 'SBO_SALES_SO' AND "SCHEMA_NAME" = CURRENT_SCHEMA;
	if :cnt > 0 then
		sqlStr := ('drop view SBO_SALES_SO');
		exec (:sqlStr);
	end if;
	
	sqlStr := 'CREATE VIEW SBO_SALES_SO
AS
SELECT     ORDR."DocNum" as "DOCNUM", RDR1."LineNum" as "LINENUM", ORDR."CANCELED" AS "CANCELLED", RDR1."ItemCode" as "ITEMCODE", 
             ORDR."CardCode" as "CARDCODE",
             TO_int(ORDR."SlpCode") AS "SALESPERSON",

	TO_CHAR(YEAR(ORDR."DocDate")) || CASE LENGTH(MONTH(ORDR."DocDate")) 
              WHEN 1 THEN ''0'' || TO_CHAR(MONTH(ORDR."DocDate")) ELSE TO_CHAR(MONTH(ORDR."DocDate")) 
              END 
              || CASE LENGTH(EXTRACT(DAY FROM ORDR."DocDate")) WHEN 1 THEN ''0'' || TO_CHAR(EXTRACT(DAY FROM ORDR."DocDate"))
              ELSE TO_CHAR(EXTRACT(DAY FROM ORDR."DocDate")) END AS "DOCDATE",  

           TO_CHAR(YEAR(ORDR."UpdateDate"))
       || case LENGTH(MONTH(ORDR."UpdateDate"))
          when 1 then ''0'' || TO_CHAR(MONTH(ORDR."UpdateDate"))
          else TO_CHAR(MONTH(ORDR."UpdateDate"))
         end
      || case LENGTH(EXTRACT(DAY FROM ORDR."UpdateDate"))
          when 1 then ''0'' || TO_CHAR(EXTRACT(DAY FROM ORDR."UpdateDate"))
          else TO_CHAR(EXTRACT(DAY FROM ORDR."UpdateDate"))
        end  AS "UPDATEDATE",
        
           ROUND ( RDR1."LineTotal" * (1- ORDR."DiscPrcnt"/ 100), 2) AS "ORDERVALUE", 
          
           CASE RDR1."UseBaseUn" 
          WHEN ''Y'' THEN RDR1."Quantity" 
          WHEN ''N'' THEN RDR1."Quantity" * OITM."NumInSale"
          END  AS "ORDERQUANTITY", 
         
          CASE RDR1."UseBaseUn" 
          WHEN ''Y'' THEN RDR1."OpenQty"
          WHEN ''N'' THEN RDR1."OpenQty" * OITM."NumInSale"
          END  AS "OPENORDQUANT", 

      RDR1."GrossBuyPr" AS "GROSSBYEPR", 
      CASE (SELECT Top 1 "MainCurncy" FROM "' || CURRENT_SCHEMA || '"."OADM")
          WHEN ''Eur'' THEN ''EUR''
          WHEN ''$'' THEN ''USD''    
          WHEN ''€'' THEN ''EUR''
          ELSE (SELECT Top 1 "MainCurncy" FROM "' || CURRENT_SCHEMA || '"."OADM") 
      END AS "CURRENCY", 
 
     CASE OITM."SalUnitMsr"
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

FROM         "' || CURRENT_SCHEMA || '"."RDR1" INNER JOIN
                      "' || CURRENT_SCHEMA || '"."ORDR" ON RDR1."DocEntry" = ORDR."DocEntry" INNER JOIN
                      "' || CURRENT_SCHEMA || '"."OITM" ON RDR1."ItemCode" = OITM."ItemCode"';
                      
exec (:sqlStr);
	--ins_msg_proc(:sqlStr);
END;


