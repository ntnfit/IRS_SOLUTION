-- B1 DEPENDS: AFTER:PT:PROCESS_END

create procedure TmSp_ZZZ_VIEW_SBO_SALES_IN
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER
AS
cnt int;
sqlStr nvarchar(20000);
BEGIN
	select count (*) into cnt from "PUBLIC"."VIEWS" where "VIEW_NAME" = 'SBO_SALES_IN' AND "SCHEMA_NAME" = CURRENT_SCHEMA;
	if :cnt > 0 then
		sqlStr := ('drop view SBO_SALES_IN');
		exec (:sqlStr);
	end if;
	
	sqlStr := 'CREATE VIEW SBO_SALES_IN
AS
SELECT     OINV."DocNum" AS "DOCNUM", INV1."LineNum" AS "LINENUM", OINV."CANCELED" as "CANCELLED",  INV1."ItemCode" as "ITEMCODE", 
             OINV."CardCode" as "CARDCODE",
              TO_CHAR(OINV."SlpCode") AS "SALESPERSON",

              TO_CHAR(year(OINV."DocDate")) || CASE LENGTH(MONTH(OINV."DocDate"))
              WHEN 1 THEN ''0'' || TO_CHAR(MONTH(OINV."DocDate")) ELSE TO_CHAR(MONTH(OINV."DocDate")) 
              END 
              || CASE LENGTH(EXTRACT(DAY FROM OINV."DocDate")) WHEN 1 THEN ''0'' 
              || TO_CHAR(EXTRACT(DAY FROM OINV."DocDate")) 
              ELSE TO_CHAR(EXTRACT(DAY FROM OINV."DocDate")) END AS "DOCDATE",  

            TO_CHAR(year(OINV."UpdateDate"))
       || case LENGTH(MONTH(OINV."UpdateDate"))
          when 1 then ''0'' || TO_CHAR(MONTH(OINV."UpdateDate"))
          else TO_CHAR(MONTH(OINV."UpdateDate"))
         end
      || case LENGTH(EXTRACT(DAY FROM OINV."UpdateDate"))
          when 1 then ''0'' || TO_CHAR(EXTRACT(DAY FROM OINV."UpdateDate"))
          else TO_CHAR(EXTRACT(DAY FROM OINV."UpdateDate"))
        end  AS "UPDATEDATE", 
      
      ROUND ( INV1."LineTotal" * (1- OINV."DiscPrcnt"/ 100), 2) AS "ORDERVALUE", 
       
         
          CASE INV1."UseBaseUn" 
          WHEN ''Y'' THEN INV1."Quantity" 
          WHEN ''N'' THEN INV1."Quantity" * OITM."NumInSale"
          END  AS "ORDERQUANTITY", 
         

      INV1."GrossBuyPr" AS "GROSSBYEPR", 
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


FROM         "' || CURRENT_SCHEMA || '"."INV1" INNER JOIN
                      "' || CURRENT_SCHEMA || '"."OINV" ON INV1."DocEntry" = OINV."DocEntry" INNER JOIN
                      "' || CURRENT_SCHEMA || '"."OITM" ON INV1."ItemCode" = OITM."ItemCode"';
exec (:sqlStr);
	--ins_msg_proc(:sqlStr);
END;


