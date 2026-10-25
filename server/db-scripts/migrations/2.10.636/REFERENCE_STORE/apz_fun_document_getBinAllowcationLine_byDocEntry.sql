
CALL APZ_EXISTSPROC('apz_fun_document_getBinAllowcationLine_byDocEntry');

GO

-- apz_document_getBinAllowcationLine_byDocEntry 682,67
CREATE PROCEDURE apz_fun_document_getBinAllowcationLine_byDocEntry(
	in docEntry nvarchar(5000) default null,
	in ObjType VARCHAR(20),
	in tempTableName nvarchar(254))
AS
begin

	declare obj nvarchar(20);
	create local temporary table #tbl as (SELECT TOP 0 T0.*, CAST(NULL AS integer) AS "BinAbs", CAST(NULL AS nvarchar(50)) AS "BinCode", CAST(NULL AS decimal(19, 6)) AS "SelectedQty"
						, CAST(NULL AS nvarchar(50)) AS "WhsCode", CAST(NULL AS nvarchar(254)) AS "BinDescription", CAST(NULL AS integer) AS "BaseLineNumber", CAST(NULL AS integer) AS "BaseDocEntry"
						, CAST(NULL AS char(1)) AS "ManBatchSerial", CAST(NULL AS decimal(19, 6)) AS "LogQty", CAST(NULL AS char(1)) AS "IsExits", CAST(NULL AS integer) AS "TranType"
						, CAST(NULL AS char(1)) AS "IsBase" FROM OBTN T0);

	if :ObjType = 112
	then

		select (select top 1 "ObjType" from ODRF where "DocEntry" in (select cast(Data as int) from Split(:docEntry,','))) into obj from dummy;

		INSERT INTO #tbl ("ItemCode", "AbsEntry", "SysNumber", "BinAbs", "BinCode", "BinDescription", "CreateDate", "SelectedQty", "BaseLineNumber", "TranType", "LogQty", "WhsCode") 
		SELECT a."ItemCode", 0, 0, c."AbsEntry", c."BinCode", c."Descr", c."CreateDate", d."Qty" AS "AvailQty", a."LineNum" AS "BaseLineNumber", 
			case when :obj = 67 then (case when a."BinActTyp" in (2, 20) then 1 else 2 end) else 1 end "TranType", a."Quantity" AS "LogQty", a."WhsCode" 
				FROM DRF19 a INNER JOIN OBIN c ON a."BinAbs" = c."AbsEntry" LEFT OUTER JOIN (SELECT c."AbsEntry", c."WhsCode", a."ItemCode"
				, SUM(CASE WHEN a."ActionType" in (1,19) THEN b."Quantity" WHEN a."ActionType" in (2,20) THEN -1 * b."Quantity" ELSE 0 END) AS "Qty" 
				FROM OILM a INNER JOIN OBTL b ON a."MessageID" = b."MessageID" INNER JOIN OBIN c ON b."BinAbs" = c."AbsEntry" GROUP BY c."AbsEntry", c."WhsCode", a."ItemCode") AS d 
				ON a."ItemCode" = d."ItemCode" AND c."AbsEntry" = d."AbsEntry" AND c."WhsCode" = d."WhsCode" WHERE a."DocEntry" = :docEntry AND a."SnBType" = -1;
		
		
		--if :obj = '67'
		--then
		--	update a set "TranType" = 2 from #tbl a INNER JOIN (Select "LineNum", "WhsCode" from DRF1 where "DocEntry" in (select cast(Data as int) from Split(:docEntry,','))) b ON a."BaseLineNumber" = b."LineNum" and a."WhsCode" = b."WhsCode";
		--end if;
		
	else

		INSERT INTO #tbl ("ItemCode", "AbsEntry", "SysNumber", "BinAbs", "BinCode", "BinDescription", "CreateDate", "SelectedQty", "BaseLineNumber", "BaseDocEntry", "TranType", "LogQty", "WhsCode")
		SELECT a."ItemCode", 0, 0, c."AbsEntry", c."BinCode", c."Descr", c."CreateDate", d."Qty" AS "AvailQty", a."DocLineNum" AS "BaseLineNumber", a."DocEntry", 
				case when :ObjType = 67 then (case when a."ActionType" in (2, 20) then 1 else 2 end) else 1 end "TranType", b."Quantity" AS "LogQty", c."WhsCode" 
		FROM OILM a INNER JOIN OBTL b ON a."MessageID" = b."MessageID"
						INNER JOIN OBIN c ON b."BinAbs" = c."AbsEntry" 
						left join (select c."AbsEntry", c."WhsCode", a."ItemCode", sum(case when a."ActionType" in (1,19) then b."Quantity" when a."ActionType" in (2,20) then -1 * b."Quantity" else 0 end) "Qty"
										from OILM a INNER JOIN OBTL b ON a."MessageID" = b."MessageID"
										INNER JOIN OBIN c ON b."BinAbs" = c."AbsEntry" group by c."AbsEntry", c."WhsCode", a."ItemCode") d On a."ItemCode" = d."ItemCode" and c."AbsEntry" = d."AbsEntry" and c."WhsCode" = d."WhsCode" 
										where a."DocEntry" in (select cast(Data as int) from Split(:docEntry,',')) and a."TransType" = :ObjType and a."SnBType" = 4;

		--if :ObjType = '67'
		--then
		--	update a set "TranType" = 2 from #tbl a INNER JOIN (Select "LineNum", "WhsCode", "DocEntry" from WTR1 where "DocEntry" in (select cast(Data as int) from Split(:docEntry,','))) b ON a."BaseLineNumber" = b."LineNum" and a."WhsCode" = b."WhsCode" and a."BaseDocEntry" = b."DocEntry";
		--end if;
	END if;
	exec 'drop table ' || :tempTableName || ';';
	exec 'create local temporary table ' || :tempTableName || ' as (select * from #tbl);';
	DROP TABLE #tbl;
end