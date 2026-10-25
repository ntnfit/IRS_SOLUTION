-- B1 DEPENDS: AFTER:PT:PROCESS_END

create procedure TmSp_ZZZ_VIEW_SBO_PUR_IV_CM
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER
AS
cnt int;
sqlStr nvarchar(20000);
BEGIN
	select count (*) into cnt from "PUBLIC"."VIEWS" where "VIEW_NAME" = 'SBO_PUR_IV_CM' AND "SCHEMA_NAME" = CURRENT_SCHEMA;
	if :cnt > 0 then
		sqlStr := ('drop view SBO_PUR_IV_CM');
		exec (:sqlStr);
	end if;
	
	sqlStr := 'CREATE VIEW SBO_PUR_IV_CM AS
			SELECT TO_CHAR(PCH1."DocEntry") as "DOCENTRY", PCH1."LineNum" as "LINENUM",
      '' '' as "RETURNID", PCH1."ItemCode" as "ITEMCODE", TO_CHAR(OPCH."CardCode") as "CARDCODE",
      ''X'' as "GRID",
      TO_CHAR(YEAR(OPOR."DocDate"))
       || case LENGTH(MONTH(OPOR."DocDate"))
          when 1 then ''0'' || TO_CHAR(MONTH(OPOR."DocDate"))
          else TO_CHAR(MONTH(OPOR."DocDate"))
         end
      || case LENGTH(EXTRACT(DAY FROM OPOR."DocDate"))
          when 1 then ''0'' || TO_CHAR(EXTRACT(DAY FROM OPOR."DocDate"))
          else TO_CHAR(EXTRACT(DAY FROM OPOR."DocDate"))
        end  AS "PODOCDATE",
      TO_CHAR(YEAR(OPCH."DocDate"))
       || case LENGTH(MONTH(OPCH."DocDate"))
          when 1 then ''0'' || TO_CHAR(MONTH(OPCH."DocDate"))
          else TO_CHAR(MONTH(OPCH."DocDate"))
         end
      || case LENGTH(EXTRACT(DAY FROM OPCH."DocDate"))
          when 1 then ''0'' || TO_CHAR(EXTRACT(DAY FROM OPCH."DocDate"))
          else TO_CHAR(EXTRACT(DAY FROM OPCH."DocDate"))
        end  AS "DOCDATE",
      TO_CHAR(YEAR(OPCH."CreateDate"))
       || case LENGTH(MONTH(OPCH."CreateDate"))
          when 1 then ''0'' || TO_CHAR(MONTH(OPCH."CreateDate"))
          else TO_CHAR(MONTH(OPCH."CreateDate"))
         end
      || case LENGTH(EXTRACT(DAY FROM OPCH."CreateDate"))
          when 1 then ''0'' || TO_CHAR(EXTRACT(DAY FROM OPCH."CreateDate"))
          else TO_CHAR(EXTRACT(DAY FROM OPCH."CreateDate"))
        end  AS "CREATEDATE",
      TO_CHAR(YEAR(OPCH."UpdateDate"))
       || case LENGTH(MONTH(OPCH."UpdateDate"))
          when 1 then ''0'' || TO_CHAR(MONTH(OPCH."UpdateDate"))
          else TO_CHAR(MONTH(OPCH."UpdateDate"))
         end
      || case LENGTH(EXTRACT(DAY FROM OPCH."UpdateDate"))
          when 1 then ''0'' || TO_CHAR(EXTRACT(DAY FROM OPCH."UpdateDate"))
          else TO_CHAR(EXTRACT(DAY FROM OPCH."UpdateDate"))
        end  AS "UPDATEDATE",
      ROUND(PCH1."LineTotal" * (1 - OPCH."DiscPrcnt" / 100) 
            + PCH1."VatSum", 2) AS "IVVALUE", 
      CASE PCH1."UseBaseUn" 
          WHEN ''Y'' THEN PCH1."Quantity" 
          WHEN ''N'' THEN PCH1."Quantity" * OITM."NumInBuy" 
      END  AS "IVQUANTITY", 
      CASE (SELECT Top 1 "MainCurncy" FROM  "' || CURRENT_SCHEMA || '"."OADM")
          WHEN ''Eur'' THEN ''EUR''
          WHEN ''$'' THEN ''USD''    
          WHEN ''€'' THEN ''EUR''
          ELSE (SELECT Top 1 "MainCurncy" FROM  "' || CURRENT_SCHEMA || '"."OADM") 
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
FROM  "' || CURRENT_SCHEMA || '"."PCH1" INNER JOIN
       "' || CURRENT_SCHEMA || '"."OPCH" ON PCH1."DocEntry" = OPCH."DocEntry" INNER JOIN
       "' || CURRENT_SCHEMA || '"."OITM" ON PCH1."ItemCode" = OITM."ItemCode" INNER JOIN
       "' || CURRENT_SCHEMA || '"."OPOR" ON PCH1."BaseEntry" = OPOR."DocEntry" 
WHERE OPCH."DocType" = ''I'' and PCH1."BaseType" = ''22''
UNION
SELECT TO_CHAR(PCH1."DocEntry") as "DOCENTRY", PCH1."LineNum" as "LINENUM",
      '' '' as "RETURNID", PCH1."ItemCode" as "ITEMCODE", TO_CHAR(OPCH."CardCode") as "CARDCODE",
      '' '' as "GRID",
      TO_CHAR(YEAR(OPOR."DocDate"))
       || case LENGTH(MONTH(OPOR."DocDate"))
          when 1 then ''0'' || TO_CHAR(MONTH(OPOR."DocDate"))
          else TO_CHAR(MONTH(OPOR."DocDate"))
         end
      || case LENGTH(EXTRACT(DAY FROM OPOR."DocDate"))
          when 1 then ''0'' || TO_CHAR(EXTRACT(DAY FROM OPOR."DocDate"))
          else TO_CHAR(EXTRACT(DAY FROM OPOR."DocDate"))
        end  AS "PODOCDATE",
      TO_CHAR(YEAR(OPCH."DocDate"))
       || case LENGTH(MONTH(OPCH."DocDate"))
          when 1 then ''0'' || TO_CHAR(MONTH(OPCH."DocDate"))
          else TO_CHAR(MONTH(OPCH."DocDate"))
         end
      || case LENGTH(EXTRACT(DAY FROM OPCH."DocDate"))
          when 1 then ''0'' || TO_CHAR(EXTRACT(DAY FROM OPCH."DocDate"))
          else TO_CHAR(EXTRACT(DAY FROM OPCH."DocDate"))
        end  AS "DOCDATE",
      TO_CHAR(YEAR(OPCH."CreateDate"))
       || case LENGTH(MONTH(OPCH."CreateDate"))
          when 1 then ''0'' || TO_CHAR(MONTH(OPCH."CreateDate"))
          else TO_CHAR(MONTH(OPCH."CreateDate"))
         end
      || case LENGTH(EXTRACT(DAY FROM OPCH."CreateDate"))
          when 1 then ''0'' || TO_CHAR(EXTRACT(DAY FROM OPCH."CreateDate"))
          else TO_CHAR(EXTRACT(DAY FROM OPCH."CreateDate"))
        end  AS "CREATEDATE",
      TO_CHAR(YEAR(OPCH."UpdateDate"))
       || case LENGTH(MONTH(OPCH."UpdateDate"))
          when 1 then ''0'' || TO_CHAR(MONTH(OPCH."UpdateDate"))
          else TO_CHAR(MONTH(OPCH."UpdateDate"))
         end
      || case LENGTH(EXTRACT(DAY FROM OPCH."UpdateDate"))
          when 1 then ''0'' || TO_CHAR(EXTRACT(DAY FROM OPCH."UpdateDate"))
          else TO_CHAR(EXTRACT(DAY FROM OPCH."UpdateDate"))
        end  AS "UPDATEDATE",
      ROUND(PCH1."LineTotal" * (1 - OPCH."DiscPrcnt" / 100) 
            + PCH1."VatSum", 2) AS "IVVALUE", 
      CASE PCH1."UseBaseUn" 
          WHEN ''Y'' THEN PCH1."Quantity" 
          WHEN ''N'' THEN PCH1."Quantity" * OITM."NumInBuy" 
      END  AS "IVQUANTITY", 
      CASE (SELECT Top 1 "MainCurncy" FROM  "' || CURRENT_SCHEMA || '"."OADM")
          WHEN ''Eur'' THEN ''EUR''
          WHEN ''$'' THEN ''USD''    
          WHEN ''€'' THEN ''EUR''
          ELSE (SELECT Top 1 "MainCurncy" FROM  "' || CURRENT_SCHEMA || '"."OADM") 
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
FROM  "' || CURRENT_SCHEMA || '"."PCH1" INNER JOIN
       "' || CURRENT_SCHEMA || '"."OPCH" ON PCH1."DocEntry" = OPCH."DocEntry" INNER JOIN
       "' || CURRENT_SCHEMA || '"."OITM" ON PCH1."ItemCode" = OITM."ItemCode" INNER JOIN
       "' || CURRENT_SCHEMA || '"."PDN1" ON PCH1."BaseEntry" = PDN1."DocEntry" AND PCH1."BaseLine" = PDN1."LineNum" INNER JOIN
       "' || CURRENT_SCHEMA || '"."OPOR" ON PDN1."BaseEntry" = OPOR."DocEntry"
WHERE OPCH."DocType" = ''I'' and PCH1."BaseType" = ''20'' AND PDN1."BaseType" = ''22''
UNION
SELECT TO_CHAR(RPC1."DocEntry") as "DOCENTRY", RPC1."LineNum" as "LINENUM",
      ''X'' as "RETURNID", RPC1."ItemCode" as "ITEMCODE", TO_CHAR(ORPC."CardCode") as "CARDCODE", 
      ''X'' as "GRID",
      TO_CHAR(YEAR(OPOR."DocDate"))
       || case LENGTH(MONTH(OPOR."DocDate"))
          when 1 then ''0'' || TO_CHAR(MONTH(OPOR."DocDate"))
          else TO_CHAR(MONTH(OPOR."DocDate"))
         end
      || case LENGTH(EXTRACT(DAY FROM OPOR."DocDate"))
          when 1 then ''0'' || TO_CHAR(EXTRACT(DAY FROM OPOR."DocDate"))
          else TO_CHAR(EXTRACT(DAY FROM OPOR."DocDate"))
        end  AS "PODOCDATE",
      TO_CHAR(YEAR(ORPC."DocDate"))
       || case LENGTH(MONTH(ORPC."DocDate"))
          when 1 then ''0'' || TO_CHAR(MONTH(ORPC."DocDate"))
          else TO_CHAR(MONTH(ORPC."DocDate"))
         end
      || case LENGTH(EXTRACT(DAY FROM ORPC."DocDate"))
          when 1 then ''0'' || TO_CHAR(EXTRACT(DAY FROM ORPC."DocDate"))
          else TO_CHAR(EXTRACT(DAY FROM ORPC."DocDate"))
        end  AS "DOCDATE",
      TO_CHAR(YEAR(ORPC."CreateDate"))
       || case LENGTH(MONTH(ORPC."CreateDate"))
          when 1 then ''0'' || TO_CHAR(MONTH(ORPC."CreateDate"))
          else TO_CHAR(MONTH(ORPC."CreateDate"))
         end
      || case LENGTH(EXTRACT(DAY FROM ORPC."CreateDate"))
          when 1 then ''0'' || TO_CHAR(EXTRACT(DAY FROM ORPC."CreateDate"))
          else TO_CHAR(EXTRACT(DAY FROM ORPC."CreateDate"))
        end  AS "CREATEDATE",
      TO_CHAR(YEAR(ORPC."UpdateDate"))
       || case LENGTH(MONTH(ORPC."UpdateDate"))
          when 1 then ''0'' || TO_CHAR(MONTH(ORPC."UpdateDate"))
          else TO_CHAR(MONTH(ORPC."UpdateDate"))
         end
      || case LENGTH(EXTRACT(DAY FROM ORPC."UpdateDate"))
          when 1 then ''0'' || TO_CHAR(EXTRACT(DAY FROM ORPC."UpdateDate"))
          else TO_CHAR(EXTRACT(DAY FROM ORPC."UpdateDate"))
        end  AS "UPDATEDATE",
      ROUND(RPC1."LineTotal" * (1 - ORPC."DiscPrcnt" / 100) 
            + RPC1."VatSum", 2) AS "IVVALUE", 
      CASE RPC1."UseBaseUn" 
          WHEN ''Y'' THEN RPC1."Quantity" 
          WHEN ''N'' THEN RPC1."Quantity" * OITM."NumInBuy" 
      END  AS IVQUANTITY, 
      CASE (SELECT Top 1 "MainCurncy" FROM  "' || CURRENT_SCHEMA || '"."OADM")
          WHEN ''Eur'' THEN ''EUR''
          WHEN ''$'' THEN ''USD''    
          WHEN ''€'' THEN ''EUR''
          ELSE (SELECT Top 1 "MainCurncy" FROM  "' || CURRENT_SCHEMA || '"."OADM") 
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
      End AS BASEUNIT
FROM  "' || CURRENT_SCHEMA || '"."RPC1" INNER JOIN
       "' || CURRENT_SCHEMA || '"."ORPC" ON RPC1."DocEntry" = ORPC."DocEntry" INNER JOIN
       "' || CURRENT_SCHEMA || '"."OITM" ON RPC1."ItemCode" = OITM."ItemCode" INNER JOIN
       "' || CURRENT_SCHEMA || '"."PCH1" ON RPC1."BaseEntry" = PCH1."DocEntry" AND RPC1."BaseLine" = PCH1."LineNum" INNER JOIN
       "' || CURRENT_SCHEMA || '"."OPOR" ON PCH1."BaseEntry" = OPOR."DocEntry"
WHERE ORPC."DocType" = ''I'' and RPC1."BaseType" = ''18'' and PCH1."BaseType" = ''22''
UNION
SELECT TO_CHAR(RPC1."DocEntry") as "DOCENTRY", RPC1."LineNum" as "LINENUM",
      ''X'' as "RETURNID", RPC1."ItemCode" as "ITEMCODE", TO_CHAR(ORPC."CardCode") as "CARDCODE", 
      ''X'' as "GRID",
      TO_CHAR(YEAR(OPOR."DocDate"))
       || case LENGTH(MONTH(OPOR."DocDate"))
          when 1 then ''0'' || TO_CHAR(MONTH(OPOR."DocDate"))
          else TO_CHAR(MONTH(OPOR."DocDate"))
         end
      || case LENGTH(EXTRACT(DAY FROM OPOR."DocDate"))
          when 1 then ''0'' || TO_CHAR(EXTRACT(DAY FROM OPOR."DocDate"))
          else TO_CHAR(EXTRACT(DAY FROM OPOR."DocDate"))
        end  AS "PODOCDATE",
      TO_CHAR(YEAR(ORPC."DocDate"))
       || case LENGTH(MONTH(ORPC."DocDate"))
          when 1 then ''0'' || TO_CHAR(MONTH(ORPC."DocDate"))
          else TO_CHAR(MONTH(ORPC."DocDate"))
         end
      || case LENGTH(EXTRACT(DAY FROM ORPC."DocDate"))
          when 1 then ''0'' || TO_CHAR(EXTRACT(DAY FROM ORPC."DocDate"))
          else TO_CHAR(EXTRACT(DAY FROM ORPC."DocDate"))
        end  AS "DOCDATE",
      TO_CHAR(YEAR(ORPC."CreateDate"))
       || case LENGTH(MONTH(ORPC."CreateDate"))
          when 1 then ''0'' || TO_CHAR(MONTH(ORPC."CreateDate"))
          else TO_CHAR(MONTH(ORPC."CreateDate"))
         end
      || case LENGTH(EXTRACT(DAY FROM ORPC."CreateDate"))
          when 1 then ''0'' || TO_CHAR(EXTRACT(DAY FROM ORPC."CreateDate"))
          else TO_CHAR(EXTRACT(DAY FROM ORPC."CreateDate"))
        end  AS "CREATEDATE",
      TO_CHAR(YEAR(ORPC."UpdateDate"))
       || case LENGTH(MONTH(ORPC."UpdateDate"))
          when 1 then ''0'' || TO_CHAR(MONTH(ORPC."UpdateDate"))
          else TO_CHAR(MONTH(ORPC."UpdateDate"))
         end
      || case LENGTH(EXTRACT(DAY FROM ORPC."UpdateDate"))
          when 1 then ''0'' || TO_CHAR(EXTRACT(DAY FROM ORPC."UpdateDate"))
          else TO_CHAR(EXTRACT(DAY FROM ORPC."UpdateDate"))
        end  AS "UPDATEDATE",
      ROUND(RPC1."LineTotal" * (1 - ORPC."DiscPrcnt" / 100) 
            + RPC1."VatSum", 2) AS "IVVALUE", 
      CASE RPC1."UseBaseUn" 
          WHEN ''Y'' THEN RPC1."Quantity" 
          WHEN ''N'' THEN RPC1."Quantity" * OITM."NumInBuy" 
      END  AS "IVQUANTITY", 
      CASE (SELECT Top 1 "MainCurncy" FROM  "' || CURRENT_SCHEMA || '"."OADM")
          WHEN ''Eur'' THEN ''EUR''
          WHEN ''$'' THEN ''USD''    
          WHEN ''€'' THEN ''EUR''
          ELSE (SELECT Top 1 "MainCurncy" FROM  "' || CURRENT_SCHEMA || '"."OADM") 
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
FROM  "' || CURRENT_SCHEMA || '"."RPC1" INNER JOIN
       "' || CURRENT_SCHEMA || '"."ORPC" ON RPC1."DocEntry" = ORPC."DocEntry" INNER JOIN
       "' || CURRENT_SCHEMA || '"."OITM" ON RPC1."ItemCode" = OITM."ItemCode" INNER JOIN
       "' || CURRENT_SCHEMA || '"."PCH1" ON RPC1."BaseEntry" = PCH1."DocEntry" AND RPC1."BaseLine" = PCH1."LineNum" INNER JOIN
       "' || CURRENT_SCHEMA || '"."PDN1" ON PCH1."BaseEntry" = PDN1."DocEntry" AND PCH1."BaseLine" = PDN1."LineNum" INNER JOIN      
       "' || CURRENT_SCHEMA || '"."OPOR" ON PDN1."BaseEntry" = OPOR."DocEntry"
WHERE ORPC."DocType" = ''I'' and RPC1."BaseType" = ''18'' and PCH1."BaseType" = ''20'' AND PDN1."BaseType" = ''22''';

   exec (:sqlStr);
	--ins_msg_proc(:sqlStr);
END;


