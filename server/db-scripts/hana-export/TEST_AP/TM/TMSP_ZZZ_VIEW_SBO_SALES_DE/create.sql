-- B1 DEPENDS: AFTER:PT:PROCESS_END

create procedure TmSp_ZZZ_VIEW_SBO_SALES_DE
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER
AS
cnt int;
sqlStr nvarchar(20000);
BEGIN
	select count (*) into cnt from "PUBLIC"."VIEWS" where "VIEW_NAME" = 'SBO_SALES_DE' AND "SCHEMA_NAME" = CURRENT_SCHEMA;
	if :cnt > 0 then
		sqlStr := ('drop view SBO_SALES_DE');
		exec (:sqlStr);
	end if;
	
	sqlStr := 'CREATE VIEW SBO_SALES_DE
AS
SELECT     ODLN."DocNum" AS "DOCNUM", DLN1."LineNum" AS "LINENUM", ODLN."CANCELED" as "CANCELLED", DLN1."ItemCode" as "ITEMCODE", 
             ODLN."CardCode" as "CARDCODE",
              TO_CHAR(ODLN."SlpCode") AS "SALESPERSON",
             
              TO_CHAR(YEAR(ODLN."DocDate")) || CASE LENGTH(MONTH(ODLN."DocDate")) 
              WHEN 1 THEN ''0'' || TO_CHAR(MONTH(ODLN."DocDate")) ELSE TO_CHAR(MONTH(ODLN."DocDate")) 
              END 
              || CASE LENGTH(EXTRACT(DAY FROM ODLN."DocDate")) WHEN 1 THEN ''0''
               || TO_CHAR(EXTRACT(DAY FROM ODLN."DocDate")) 
              ELSE TO_CHAR(EXTRACT(DAY FROM ODLN."DocDate")) END AS "DOCDATE",

           TO_CHAR(YEAR(ODLN."UpdateDate"))
       || case LENGTH(MONTH(ODLN."UpdateDate"))
          when 1 then ''0'' || TO_CHAR(MONTH(ODLN."UpdateDate"))
          else TO_CHAR(MONTH(ODLN."UpdateDate"))
         end
      || case LENGTH(EXTRACT(DAY FROM ODLN."UpdateDate"))
          when 1 then ''0'' || TO_CHAR(EXTRACT(DAY FROM ODLN."UpdateDate"))
          else TO_CHAR(EXTRACT(DAY FROM ODLN."UpdateDate"))
        end  AS "UPDATEDATE",

          CASE DLN1."UseBaseUn" 
          WHEN ''Y'' THEN DLN1."Quantity" 
          WHEN ''N'' THEN DLN1."Quantity" * OITM."NumInSale"
          END  AS "DELIVERYQUANTITY",


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

FROM         "' || CURRENT_SCHEMA || '"."DLN1" INNER JOIN
                      "' || CURRENT_SCHEMA || '"."ODLN" ON DLN1."DocEntry" = ODLN."DocEntry" INNER JOIN
                      "' || CURRENT_SCHEMA || '"."OITM" ON DLN1."ItemCode" = OITM."ItemCode"';
  exec (:sqlStr);
	--ins_msg_proc(:sqlStr);
END;


