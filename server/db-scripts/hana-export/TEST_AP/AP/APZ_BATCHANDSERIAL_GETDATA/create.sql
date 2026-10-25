
CREATE PROCEDURE APZ_BatchAndSerial_GetData (
	IN 	itemCode nvarchar(50) DEFAULT ''
,		code nvarchar(50) DEFAULT ''
,		whs nvarchar(8) DEFAULT ''
)
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER
AS
BEGIN
	DECLARE ManBtchNum CHAR(1) := '';
	SELECT IFNULL((SELECT "ManBtchNum" FROM OITM WHERE "ItemCode" = :itemCode), '') INTO ManBtchNum FROM DUMMY;

	IF (:ManBtchNum = 'Y') THEN 
		CREATE LOCAL TEMPORARY TABLE #tblBATCH AS 
			(SELECT TOP 0 T4.*, T0."BinAbs", T1."BinCode", T2."OnHandQty" AS "Instock", T1."WhsCode", IFNULL(T1."Descr", T1."BinCode") AS "BinDescription", CAST(0 AS DECIMAL(19, 6)) AS "LogQty", CAST(0 AS DECIMAL(19, 6)) AS "CommitQty" 
				FROM OIBQ T0 INNER JOIN OBIN T1 ON T0."BinAbs" = T1."AbsEntry" AND T0."OnHandQty" <> 0 
				LEFT OUTER JOIN OBBQ T2 ON T0."BinAbs" = T2."BinAbs" AND T0."ItemCode" = T2."ItemCode" AND T2."OnHandQty" <> 0 
				LEFT OUTER JOIN OBTN T4 ON T2."SnBMDAbs" = T4."AbsEntry" AND T2."ItemCode" = T4."ItemCode" 
				LEFT OUTER JOIN OITM T6 ON T0."ItemCode" = T6."ItemCode" AND T6."validFor" = 'Y');
		
		INSERT INTO #tblBATCH
			SELECT T4.*, T0."BinAbs", T1."BinCode", T2."OnHandQty" AS "Instock", T1."WhsCode", IFNULL(T1."Descr", T1."BinCode") AS "BinDescription", CAST(0 AS DECIMAL(19, 6)) AS "LogQty", CAST(0 AS DECIMAL(19, 6)) AS "CommitQty" 
				FROM OIBQ T0 INNER JOIN OBIN T1 ON T0."BinAbs" = T1."AbsEntry" AND T0."OnHandQty" <> 0 
				LEFT OUTER JOIN OBBQ T2 ON T0."BinAbs" = T2."BinAbs" AND T0."ItemCode" = T2."ItemCode" AND T2."OnHandQty" <> 0 
				LEFT OUTER JOIN OBTN T4 ON T2."SnBMDAbs" = T4."AbsEntry" AND T2."ItemCode" = T4."ItemCode" 
				LEFT OUTER JOIN OITM T6 ON T0."ItemCode" = T6."ItemCode" AND T6."validFor" = 'Y' 
				WHERE T2."OnHandQty" > 0 
					AND T0."ItemCode" = :itemCode 
					AND T1."WhsCode" = :whs 
			UNION ALL SELECT b.*, 0, '', A."Quantity" AS "InStock", A."WhsCode", '', CAST(0 AS DECIMAL(19, 6)) AS "LogQty", A."CommitQty" 
				FROM OBTQ A 
				INNER JOIN OBTN b ON A."ItemCode" = b."ItemCode" AND A."SysNumber" = b."SysNumber" AND b."DistNumber" = :code 
				INNER JOIN OITM c ON A."ItemCode" = c."ItemCode" AND c."validFor" = 'Y' 
				WHERE A."Quantity" > 0 
					AND b."ItemCode" = :itemCode 
					AND A."WhsCode" = :whs 
					AND A."WhsCode" NOT IN (SELECT "WhsCode" FROM OBIN);
				
		UPDATE #tblBATCH SET "LogQty" = "Instock";
		SELECT *, 'B' AS "ManBatchSerial" FROM #tblBATCH;
		DROP TABLE #tblBATCH;
	ELSE
		DECLARE ManSerNum CHAR(1) := '';
		SELECT IFNULL((SELECT "ManSerNum" FROM OITM WHERE "ItemCode" = :itemCode), '') INTO ManSerNum FROM DUMMY;
		
		IF (:ManSerNum = 'Y') THEN 
			CREATE LOCAL TEMPORARY TABLE #tblSERIAL AS 
				(SELECT T5.*, T0."BinAbs", T1."BinCode", T3."OnHandQty" AS "Instock", T1."WhsCode", IFNULL(T1."Descr", T1."BinCode") AS "BinDescription", CAST(0 AS DECIMAL(19, 6)) AS "LogQty", CAST(0 AS DECIMAL(19, 6)) AS "CommitQty" 
					FROM OIBQ T0 
					INNER JOIN OBIN T1 ON T0."BinAbs" = T1."AbsEntry" AND T0."OnHandQty" <> 0 
					LEFT OUTER JOIN OSBQ T3 ON T0."BinAbs" = T3."BinAbs" AND T0."ItemCode" = T3."ItemCode" AND T3."OnHandQty" <> 0 
					LEFT OUTER JOIN OSRN T5 ON T3."SnBMDAbs" = T5."AbsEntry" AND T3."ItemCode" = T5."ItemCode" 
					INNER JOIN OITM T6 ON T0."ItemCode" = T6."ItemCode" AND T6."validFor" = 'Y');
			
			INSERT INTO #tblSERIAL
				SELECT T5.*, T0."BinAbs", T1."BinCode", T3."OnHandQty" AS "Instock", T1."WhsCode", IFNULL(T1."Descr", T1."BinCode") AS "BinDescription", CAST(0 AS DECIMAL(19, 6)) AS "LogQty", CAST(0 AS DECIMAL(19, 6)) AS "CommitQty" 
					FROM OIBQ T0 
					INNER JOIN OBIN T1 ON T0."BinAbs" = T1."AbsEntry" AND T0."OnHandQty" <> 0 
					LEFT OUTER JOIN OSBQ T3 ON T0."BinAbs" = T3."BinAbs" AND T0."ItemCode" = T3."ItemCode" AND T3."OnHandQty" <> 0 
					LEFT OUTER JOIN OSRN T5 ON T3."SnBMDAbs" = T5."AbsEntry" AND T3."ItemCode" = T5."ItemCode" 
					INNER JOIN OITM T6 ON T0."ItemCode" = T6."ItemCode" AND T6."validFor" = 'Y' 
					WHERE T3."OnHandQty" > 0 
						AND T0."ItemCode" = :itemCode 
						AND T1."WhsCode" = :whs 
				UNION ALL SELECT b.*, 0, '', A."Quantity" AS "InStock", A."WhsCode", '', CAST(0 AS DECIMAL(19, 6)) AS "LogQty", A."CommitQty" 
					FROM OSRQ A 
					INNER JOIN OSRN b ON A."ItemCode" = b."ItemCode" AND A."SysNumber" = b."SysNumber" AND b."DistNumber" = :code 
					INNER JOIN OITM c ON A."ItemCode" = c."ItemCode" AND c."validFor" = 'Y' 
					WHERE A."Quantity" > 0 
						AND b."ItemCode" = :itemCode 
						AND A."WhsCode" = :whs 
						AND A."WhsCode" NOT IN (SELECT "WhsCode" FROM OBIN);
					
				UPDATE #tblSERIAL SET "LogQty" = "Instock";
				SELECT *, 'S' AS "ManBatchSerial" FROM #tblSERIAL;
				DROP TABLE #tblSERIAL;
		ELSE
			SELECT TOP 0 '' AS "DistNumber", 0 AS "Instock", '' AS "BinCode" FROM DUMMY;
		END IF;
	END IF;
END;




