CREATE VIEW "TEST_AP"."APZ_OITW_LOCATION" ( "ItemCode", "BinAbs", "BinCode", "Instock", "CommitQty", "WhsCode", "BinDescription", "DistNumber", "AbsEntryBatch", "SysNumber", "MnfSerial", "LotNumber", "SL1Code", "SL2Code", "SL3Code", "SL4Code", "DaysNum", "ManBtchNum", "ManSerNum" ) AS (((SELECT T0."ItemCode", T0."BinAbs", T1."BinCode", CASE T6."ManBtchNum" WHEN 'Y' THEN T2."OnHandQty" ELSE CASE T6."ManSerNum" WHEN 'Y' THEN T3."OnHandQty" ELSE T0."OnHandQty" END END AS "Instock"
			, CAST(0 AS decimal(19, 6)) AS "CommitQty", T1."WhsCode", IFNULL(T1."Descr", T1."BinCode") AS "BinDescription"
			, CASE T6."ManBtchNum" WHEN 'Y' THEN T4."DistNumber" ELSE CASE T6."ManSerNum" WHEN 'Y' THEN T5."DistNumber" ELSE '' END END AS "DistNumber"
			, CASE T6."ManBtchNum" WHEN 'Y' THEN T4."AbsEntry" ELSE CASE T6."ManSerNum" WHEN 'Y' THEN T5."AbsEntry" ELSE null END END AS "AbsEntryBatch"
			, CASE T6."ManBtchNum" WHEN 'Y' THEN T4."SysNumber" ELSE CASE T6."ManSerNum" WHEN 'Y' THEN T5."SysNumber" ELSE null END END AS "SysNumber"
			, CASE T6."ManBtchNum" WHEN 'Y' THEN T4."MnfSerial" ELSE CASE T6."ManSerNum" WHEN 'Y' THEN T5."MnfSerial" ELSE '' END END AS "MnfSerial"
			, CASE T6."ManBtchNum" WHEN 'Y' THEN T4."LotNumber" ELSE CASE T6."ManSerNum" WHEN 'Y' THEN T5."LotNumber" ELSE '' END END AS "LotNumber"
			, T1."SL1Code", T1."SL2Code", T1."SL3Code", T1."SL4Code", DAYS_BETWEEN(CASE WHEN T4."InDate" IS NULL THEN T5."InDate" ELSE T4."InDate" END, NOW()) AS "DaysNum", T6."ManBtchNum", T6."ManSerNum" 
		FROM OIBQ T0 INNER JOIN OBIN T1 ON T0."BinAbs" = T1."AbsEntry" AND T0."OnHandQty" <> 0 
					 LEFT JOIN OBBQ T2 ON T0."BinAbs" = T2."BinAbs" AND T0."ItemCode" = T2."ItemCode" AND T2."OnHandQty" <> 0 
					 LEFT JOIN OSBQ T3 ON T0."BinAbs" = T3."BinAbs" AND T0."ItemCode" = T3."ItemCode" AND T3."OnHandQty" <> 0 
					 LEFT JOIN OBTN T4 ON T2."SnBMDAbs" = T4."AbsEntry" AND T2."ItemCode" = T4."ItemCode" 
					 LEFT JOIN OSRN T5 ON T3."SnBMDAbs" = T5."AbsEntry" AND T3."ItemCode" = T5."ItemCode" 
					 INNER JOIN OITM T6 ON T0."ItemCode" = T6."ItemCode" AND T6."validFor" = 'Y') UNION ALL (SELECT A."ItemCode", 0, null, A."Quantity", a."CommitQty", A."WhsCode", '', b."DistNumber", b."AbsEntry", b."SysNumber", b."MnfSerial", b."LotNumber", null, null, null, null, DAYS_BETWEEN(b."InDate", NOW()) AS "DaysNum"
			, c."ManBtchNum", c."ManSerNum" 
		FROM OBTQ A INNER JOIN OBTN b ON A."ItemCode" = b."ItemCode" AND A."SysNumber" = b."SysNumber" 
					INNER JOIN OITM c ON a."ItemCode" = c."ItemCode" AND c."validFor" = 'Y' WHERE A."Quantity" > 0 AND A."WhsCode" NOT IN (SELECT "WhsCode" FROM OBIN))) UNION ALL (SELECT A."ItemCode", 0, null, A."Quantity", a."CommitQty", A."WhsCode", '', b."DistNumber", b."AbsEntry", b."SysNumber", b."MnfSerial", b."LotNumber", null, null, null, null, DAYS_BETWEEN(b."InDate", NOW()) AS "DaysNum"
			, c."ManBtchNum", c."ManSerNum" 
		FROM OSRQ A INNER JOIN OSRN b ON A."ItemCode" = b."ItemCode" AND A."SysNumber" = b."SysNumber" 
					INNER JOIN OITM c ON a."ItemCode" = c."ItemCode" AND c."validFor" = 'Y' WHERE A."Quantity" > 0 AND A."WhsCode" NOT IN (SELECT "WhsCode" FROM OBIN))) WITH READ ONLY