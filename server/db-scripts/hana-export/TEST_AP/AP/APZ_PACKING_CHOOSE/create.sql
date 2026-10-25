
CREATE PROCEDURE apz_packing_choose(
	in ObjType NVARCHAR(20),
	in Id INT default 0,
	in DistNumber NVARCHAR(50) default '',
	in CardCode NVARCHAR(50) default '')
AS
BEGIN
	create local temporary table #tbl as (SELECT TOP 0 *, 'N' AS "ManBatchSerial" FROM APZ_PAK1);
	
	IF :ObjType='156'
	THEN
	
		create local temporary table #tbl_batch
		(
			"ItemCode" NVARCHAR(50),
			"DistNumber" NVARCHAR(50),
			"AbsEntry" INT,
			"ManBatchSerial" CHAR(1),
			"SysNumber" int
		);
		
		INSERT INTO #tbl_batch ("ItemCode", "DistNumber", "AbsEntry", "ManBatchSerial", "SysNumber") 
			SELECT T0."ItemCode", T0."DistNumber", T0."AbsEntry", 'B', T1."SysNumber" FROM APZ_PKL2 T0 INNER JOIN OBTN T1 ON T0."MdAbsEntry" = T1."AbsEntry" 
					INNER JOIN oitm T2 ON T1."ItemCode" = T2."ItemCode" AND T2."ManBtchNum" = 'Y' 
		UNION ALL 
			SELECT T0."ItemCode", T0."DistNumber", T0."AbsEntry", 'S', T1."SysNumber" FROM APZ_PKL2 T0 INNER JOIN OSRN T1 ON T0."MdAbsEntry" = T1."AbsEntry" 
					INNER JOIN oitm T2 ON T1."ItemCode" = T2."ItemCode" AND T2."ManSerNum" = 'Y';
	
		INSERT INTO #tbl("DocEntry", "PackNo", "PickNo", "ItemCode", "Dscription", "Quantity", "DistNumber", "SysNumber", "UoMCode", "UoMName", "NumPerMsr", "UseBaseUn", "WhsCode", "TransType", "TransNo", "TransRow"
							, "BatchSerialEntry", "ManBatchSerial", "LineNum", "LineStatus") 
			SELECT 0, NULL AS "PackNo", T0."AbsEntry" AS "PickNo", t1."ItemCode", t1."Dscription", Tx."Quantity" AS "Quantity", t3."DistNumber", T3."SysNumber", T1."UomCode", T1."unitMsr", T1."NumPerMsr", T1."UseBaseUn"
					, T1."WhsCode", t1."ObjType" AS "TransType", T1."DocEntry" AS "TransNo", T1."LineNum" AS "TransRow", T3."AbsEntry", T3."ManBatchSerial" AS "ManBatchSerial", T1."LineNum", T1."LineStatus" 
				FROM APZ_PKL2 Tx INNER JOIN APZ_PKL1 T0 ON tx."AbsEntry" = T0."AbsEntry" AND Tx."PickEntry" = T0."PickEntry" 
								INNER JOIN APZ_OPKL T4 ON T0."AbsEntry" = T4."AbsEntry" 
								INNER JOIN RDR1 T1 ON T0."OrderEntry" = T1."DocEntry" AND T1."LineNum" = T0."OrderLine" 
								INNER JOIN OITM T2 ON T1."ItemCode" = T2."ItemCode" 
								INNER JOIN #tbl_batch T3 ON T3."ItemCode" = T2."ItemCode" AND T3."DistNumber" = Tx."DistNumber" 
								INNER JOIN ordr T5 ON t1."DocEntry" = T5."DocEntry" 
						WHERE (T0."AbsEntry" = :Id OR :Id = 0) AND IFNULL(T0."PickStatus", 'O') <> 'C' AND tx."Confirmed" = 'Y' AND (:CardCode = '' OR (:CardCode <> '' AND T5."CardCode" = :CardCode));
	
	ELSEIF :ObjType='700019' -- Packing
	THEN
		INSERT INTO #tbl("DocEntry", "PackNo", "PickNo", "ItemCode", "Dscription", "Quantity", "DistNumber", "SysNumber", "UoMCode", "UoMName", "NumPerMsr", "UseBaseUn", "WhsCode", "TransType", "TransNo", "TransRow"
							, "BatchSerialEntry", "ManBatchSerial", "LineNum", "LineStatus") 
			SELECT 0, T0."DocEntry" AS "PackNo", T0."PickNo", T0."ItemCode", T0."Dscription", T0."Quantity", T0."DistNumber", t0."SysNumber", T0."UoMCode", T0."UoMName", T0."NumPerMsr", T0."UseBaseUn", T0."WhsCode"
					, T0."TransType", T0."TransNo", T0."TransRow", T0."BatchSerialEntry", CASE WHEN T1."ManBtchNum" = 'Y' THEN 'B' WHEN T1."ManSerNum" = 'Y' THEN 'S' ELSE 'N' END AS "ManBatchSerial"
					, ROW_NUMBER() OVER ( ORDER BY T0."DocEntry"), 'O' 
				FROM APZ_PAK1 T0 INNER JOIN OITM T1 ON T0."ItemCode" = T1."ItemCode" WHERE (T0."DocEntry" = :Id OR :id = 0) AND T0."LineStatus" = 'O' ORDER BY T0."DocEntry" DESC;
	END IF;
	
	SELECT * FROM #tbl ORDER BY "PickNo";
END;



