
CALL APZ_EXISTSPROC('apz_fun_document_getBinAllowcationLine_byDocEntry');

GO

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

		INSERT INTO #tbl ("ItemCode", "AbsEntry", "SysNumber", "BinAbs", "BinCode", "BinDescription", "CreateDate", "BaseLineNumber", "TranType", "LogQty", "WhsCode") 
		SELECT a."ItemCode", 0, 0, c."AbsEntry", c."BinCode", c."Descr", c."CreateDate", a."LineNum", 
			case when :obj = 67 then (case when a."BinActTyp" in (2, 20) then 1 else 2 end) else 1 end "TranType", a."Quantity", a."WhsCode" 
				FROM DRF19 a INNER JOIN OBIN c ON a."BinAbs" = c."AbsEntry"
		WHERE a."DocEntry" = :docEntry AND a."SnBType" = -1;
		
		update #tbl a set "SelectedQty"=(select sum("OnHandQty") from OIBQ where "BinAbs"=a."BinAbs" and "ItemCode"=a."ItemCode");
		
	else

		INSERT INTO #tbl ("ItemCode", "AbsEntry", "SysNumber", "BinAbs", "BinCode", "BinDescription", "CreateDate", "BaseLineNumber", "BaseDocEntry", "TranType", "LogQty", "WhsCode")
		SELECT a."ItemCode", 0, 0, c."AbsEntry", c."BinCode", c."Descr", c."CreateDate", a."DocLineNum", a."DocEntry", 
				case when :ObjType = 67 then (case when a."ActionType" in (2, 20) then 1 else 2 end) else 1 end "TranType", b."Quantity", c."WhsCode" 
		FROM OILM a JOIN OBTL b ON a."MessageID" = b."MessageID"
					JOIN OBIN c ON b."BinAbs" = c."AbsEntry"
		where a."DocEntry" in (select cast(Data as int) from Split(:docEntry,',')) and a."TransType" = :ObjType and a."SnBType" = 4;
		
		update #tbl a set "SelectedQty"=(select sum("OnHandQty") from OIBQ where "BinAbs"=a."BinAbs" and "ItemCode"=a."ItemCode");
	END if;
	
	exec 'drop table ' || :tempTableName || ';';
	exec 'create local temporary table ' || :tempTableName || ' as (select * from #tbl);';
	DROP TABLE #tbl;
end