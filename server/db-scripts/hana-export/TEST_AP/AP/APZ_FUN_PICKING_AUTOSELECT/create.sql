
CREATE PROCEDURE apz_fun_picking_AutoSelect(in ItemCode NVARCHAR(50),
	in WhsCode NVARCHAR(50),
	in Quantity numeric(19,6),
	in LineNum INT,
	in DocEntry INT,
	in OrderNum INT default 0,
	in OrderLine INT default -1,
	in tempTableName nvarchar(100))
AS
BEGIN
	
	create local temporary table #oitw_location as (select top 0 * from APZ_OITW_LOCATION);
	
	insert into #oitw_location
	select * from APZ_OITW_LOCATION where "ItemCode"=:ItemCode and "WhsCode"=:WhsCode;
	
	create local temporary table #ret as (SELECT TOP 0 * FROM APZ_PKL3);
	
	INSERT INTO #ret SELECT T0.* FROM apz_PKL3 T0
		INNER JOIN #oitw_location T1 ON T0."MdAbsEntry"=T1."AbsEntryBatch"
			WHERE "AbsEntry"=:DocEntry AND "PickEntry"=:LineNum;
		
		INSERT INTO #ret("AbsEntry", "PickEntry", "MdAbsEntry", "DistNumber", "ItemCode", "OrderEntry", "OrderLine", "Quantity", "AllocQty", "ReleaseQty", "PickedQty", "Confirmed", "SysNumber", "MnfSerial", "LotNumber", "PickStatus", "BinAbs", "BinCode", "BinDescription")
			SELECT :DocEntry,:LineNum,"AbsEntryBatch","DistNumber","ItemCode",NULL,NULL, "Instock",NULL,NULL,NULL,'N',"SysNumber", "MnfSerial", "LotNumber",'O',"BinAbs","BinCode","BinDescription"
				FROM #oitw_location T0
				WHERE "ItemCode"=:ItemCode AND "WhsCode"=:WhsCode AND "Instock"-IFNULL("CommitQty",0) >0 and "AbsEntryBatch" not in (select "MdAbsEntry" from #ret);
		
		
		create local temporary table #tempAllowcate ("ItemCode" nvarchar(50), "MdAbsEntry" int, "AllocQty" numeric(19,6));
		
		insert into #tempAllowcate
		SELECT T0."ItemCode", T0."MdAbsEntry", SUM(T0."AllocQty") AS "AllocQty" FROM ITL1 T0 INNER JOIN #ret T1 ON t0."MdAbsEntry" = t1."MdAbsEntry" AND T0."ItemCode" = T1."ItemCode" 
			INNER JOIN OITL T2 ON T0."LogEntry" = T2."LogEntry" WHERE (t2."DocNum" <> :OrderNum AND T2."DocType" = 17 AND T2."DocLine" <> :OrderLine) OR T2."DocType" <> 17 GROUP BY T0."ItemCode", T0."MdAbsEntry";
		
		--SELECT * FROM #allocate
		create local temporary table #picked("ItemCode" nvarchar(50), "MdAbsEntry" int, "Qty" numeric(19,6), "AbsEntry" int);
		
		insert into #picked
		SELECT T0."ItemCode", t0."MdAbsEntry", SUM(T0."Quantity") AS "Qty", T0."AbsEntry" FROM APZ_PKL2 T0 INNER JOIN APZ_PKL1 T1 ON T0."AbsEntry" = T1."AbsEntry" INNER JOIN APZ_OPKL T2 ON T0."AbsEntry" = T2."AbsEntry" 
				WHERE T0."AbsEntry" <> :DocEntry AND T2."Status" NOT IN ('C') AND T1."ItemCode" = :ItemCode AND T1."WhsCode" = :WhsCode GROUP BY t0."ItemCode", T0."MdAbsEntry", T0."AbsEntry";
		
		-- shipp-ed
		create local temporary table #shipped ("BatchSerialEntry" int, "Quantity" numeric(19,6), "ItemCode" nvarchar(50), "DocEntry" int, "BaseEntry" int);
		
		insert into #shipped
		SELECT T0."BatchSerialEntry", T0."Quantity", T0."ItemCode", T0."DocEntry", T0."BaseEntry" FROM APZ_SHIP1 T0 WHERE t0."ItemCode" = :ItemCode AND T0."WhsCode" = :WhsCode AND T0."BaseType" = '156';
		
		INSERT INTO #picked ("ItemCode", "MdAbsEntry", "Qty", "AbsEntry") 
			SELECT T0."ItemCode", t0."MdAbsEntry", SUM(T0."Quantity") AS "Qty", T0."AbsEntry" FROM APZ_PKL3 T0 INNER JOIN APZ_PKL1 T1 ON T0."AbsEntry" = T1."AbsEntry" INNER JOIN APZ_OPKL T2 ON T0."AbsEntry" = T2."AbsEntry" 
			WHERE T0."AbsEntry" NOT IN (SELECT "BaseEntry" FROM #shipped) AND T0."AbsEntry" <> :DocEntry AND T2."Status" NOT IN ('C') AND T1."ItemCode" = :ItemCode AND T1."WhsCode" = :WhsCode 
			GROUP BY t0."ItemCode", T0."MdAbsEntry", T0."AbsEntry";
	
	
		--SELECT * FROM #picked
		update T0 set T0."Quantity" = T0."Quantity" - IFNULL(T2."Qty", 0) FROM #ret T0 LEFT OUTER JOIN #picked T2 ON T0."ItemCode" = T2."ItemCode" AND T0."MdAbsEntry" = T2."MdAbsEntry";
		
		exec 'drop table ' || :tempTableName || ';';
		exec 'create local temporary table ' || :tempTableName || ' as (SELECT T0.* FROM #ret T0 LEFT OUTER JOIN #picked T2 ON T0."ItemCode" = T2."ItemCode" AND T0."MdAbsEntry" = T2."MdAbsEntry" 
				LEFT OUTER JOIN #tempAllowcate T1 ON T0."ItemCode" = T1."ItemCode" AND T0."MdAbsEntry" = t1."MdAbsEntry" 
				LEFT OUTER JOIN #shipped T3 ON T0."ItemCode" = T3."ItemCode" AND T0."MdAbsEntry" = T3."BatchSerialEntry" 
				WHERE T0."Quantity" + IFNULL(T2."Qty", 0) - IFNULL(T1."AllocQty", 0) > 0 AND T0."Quantity" > 0 AND T0."Quantity" + IFNULL(T2."Qty", 0) - IFNULL(T3."Quantity", 0) > 0 ORDER BY T0."SysNumber" ASC);';
				
END;




