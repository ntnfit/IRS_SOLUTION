-- B1 DEPENDS: AFTER:PT:PROCESS_END

create procedure TmSp_ZZZ_VIEW_SBO_PUR_PO
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER
AS
cnt int;
sqlStr nvarchar(20000);
BEGIN
	select count (*) into cnt from "PUBLIC"."VIEWS" where "VIEW_NAME" = 'SBO_PUR_PO' AND "SCHEMA_NAME" = CURRENT_SCHEMA;
	if :cnt > 0 then
		sqlStr := ('drop view SBO_PUR_PO');
		exec (:sqlStr);
	end if;
	
	sqlStr := 'CREATE VIEW SBO_PUR_PO
			AS
		SELECT TO_CHAR(POR1."DocEntry") as "DOCENTRY", POR1."LineNum" as "LINENUM",
      OPOR."CANCELED" as "CANCELLED", POR1."ItemCode" as "ITEMCODE", TO_CHAR(OPOR."CardCode") as "CARDCODE", 
      TO_CHAR(YEAR(OPOR."DocDate"))
       || case LENGTH(MONTH(OPOR."DocDate"))
          when 1 then ''0'' || TO_CHAR(MONTH(OPOR."DocDate"))
          else TO_CHAR(MONTH(OPOR."DocDate"))
         end
      || case LENGTH(EXTRACT(DAY FROM OPOR."DocDate"))
          when 1 then ''0'' || TO_CHAR(EXTRACT(DAY FROM OPOR."DocDate"))
          else TO_CHAR(EXTRACT(DAY FROM OPOR."DocDate"))
        end  AS "DOCDATE",
      TO_CHAR(YEAR(OPOR."UpdateDate"))
       || case LENGTH(MONTH(OPOR."UpdateDate"))
          when 1 then ''0'' || TO_CHAR(MONTH(OPOR."UpdateDate"))
          else TO_CHAR(MONTH(OPOR."UpdateDate"))
         end
      || case LENGTH(EXTRACT(DAY FROM OPOR."UpdateDate"))
          when 1 then ''0'' || TO_CHAR(EXTRACT(DAY FROM OPOR."UpdateDate"))
          else TO_CHAR(EXTRACT(DAY FROM OPOR."UpdateDate"))
        end  AS "UPDATEDATE",
      ROUND(POR1."LineTotal" * (1 - OPOR."DiscPrcnt" / 100) 
            + POR1."VatSum", 2) AS "ORDERVALUE", 
      CASE POR1."UseBaseUn" 
          WHEN ''Y'' THEN POR1."Quantity" 
          WHEN ''N'' THEN POR1."Quantity" * OITM."NumInBuy" 
      END  AS "ORDERQUANTITY", 
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
FROM "' || CURRENT_SCHEMA || '"."POR1" INNER JOIN
      "' || CURRENT_SCHEMA || '"."OPOR" ON POR1."DocEntry" = OPOR."DocEntry" INNER JOIN
      "' || CURRENT_SCHEMA || '"."OITM" ON POR1."ItemCode" = OITM."ItemCode"
WHERE OPOR."DocType" = ''I''';

  exec (:sqlStr);
	--ins_msg_proc(:sqlStr);
END;


