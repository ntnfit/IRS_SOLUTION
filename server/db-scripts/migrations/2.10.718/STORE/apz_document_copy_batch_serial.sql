
CALL APZ_EXISTSPROC('apz_document_copy_batch_serial');

GO

CREATE  procedure apz_document_copy_batch_serial(
	in DocEntry varchar(5000),
	in ObjType VARCHAR(20)
)
AS
BEGIN

	declare closedQuote nvarchar(1);
	DECLARE tblname NVARCHAR(10)='';
	DECLARE tblline NVARCHAR(10)='';
	DECLARE sqlQuery VARCHAR(50000)='';
	DECLARE n INT;
	DECLARE p INT=1;
	DECLARE shipdoc INT;

IF :ObjType = '112'
then

	create local temporary table #tempDraft as 
	(SELECT TOP 0 *, CAST(null AS integer) AS "BinAbs", CAST(null AS nvarchar(50)) AS "BinCode", CAST(null AS decimal(19, 6)) AS "SelectedQty", CAST(null AS nvarchar(50)) AS "WhsCode"
					, CAST(null AS nvarchar(100)) AS "BinDescription", CAST(null AS integer) AS "BaseLineNumber", CAST(null AS integer) AS "BaseDocEntry", 'B' AS "ManBatchSerial", CAST(null AS decimal(19, 6)) AS "LogQty"
					, 'Y' AS "IsExits", CAST(null AS integer) AS "TranType", 'N' AS "IsBase" FROM OBTN);
	
	--get data bastch/serial line
	insert into #tempDraft
		SELECT d.*, ifnull(c."BinAbs", 0), c."BinCode", CASE WHEN ifnull(c."BinAbs", 0) = 0 THEN a."Quantity" ELSE c."Quantity" END, a."WhsCode", c."BinCode", a."LineNum", a."AbsEntry", 'B'
				, CASE WHEN ifnull(c."BinAbs", 0) = 0 THEN a."Quantity" ELSE c."Quantity" END, 'Y', 1, 'N'
			 FROM (SELECT * FROM DRF16 WHERE "AbsEntry" = :DocEntry AND "ObjId" = '10000044') a INNER JOIN oitm b ON a."ItemCode" = b."ItemCode" 
			 		LEFT JOIN OBTN d ON a."ObjAbs" = d."AbsEntry" 
			 		LEFT JOIN (SELECT t1."LineNum", t1."SnBType", t1."SnBMDAbs", t2."AbsEntry" AS "BinAbs", t2."BinCode", t1."Quantity" FROM DRF19 t1 
			 								INNER JOIN OBIN t2 ON t1."WhsCode" = t2."WhsCode" AND t1."BinAbs" = t2."AbsEntry" 
			 									WHERE "DocEntry" = :DocEntry) AS c ON a."LineNum" = c."LineNum" AND a."ObjId" = c."SnBType" AND a."ObjAbs" = c."SnBMDAbs"
	UNION ALL 
	SELECT d.*, ifnull(c."BinAbs", 0), c."BinCode", CASE WHEN ifnull(c."BinAbs", 0) = 0 THEN a."Quantity" ELSE c."Quantity" END, a."WhsCode", c."BinCode", a."LineNum", a."AbsEntry", 'S'
			, CASE WHEN ifnull(c."BinAbs", 0) = 0 THEN a."Quantity" ELSE c."Quantity" END, 'Y', 1, 'N' FROM (SELECT * FROM DRF16 WHERE "AbsEntry" = :DocEntry AND "ObjId" = '10000045') AS a 
		INNER JOIN oitm b ON a."ItemCode" = b."ItemCode" 
		LEFT JOIN OSRN d ON a."ObjAbs" = d."AbsEntry" 
		LEFT JOIN (SELECT t1."LineNum", t1."SnBType", t1."SnBMDAbs", t2."AbsEntry" AS "BinAbs", t2."BinCode", t1."Quantity" FROM DRF19 t1 
			INNER JOIN OBIN t2 ON t1."WhsCode" = t2."WhsCode" AND t1."BinAbs" = t2."AbsEntry" WHERE "DocEntry" = :DocEntry) AS c ON a."LineNum" = c."LineNum" AND a."ObjId" = c."SnBType" AND a."ObjAbs" = c."SnBMDAbs"
	UNION ALL 
	SELECT d.*, ifnull(c."BinAbs", 0), c."BinCode", CASE WHEN ifnull(c."BinAbs", 0) = 0 THEN a."Quantity" ELSE c."Quantity" END, a."WhsCode", c."BinCode", a."LineNum", a."AbsEntry", 'B'
				, CASE WHEN ifnull(c."BinAbs", 0) = 0 THEN a."Quantity" ELSE c."Quantity" END, 'N', 1, 'N' FROM (SELECT * FROM DRF16 WHERE "AbsEntry" = :DocEntry AND "ObjId" = '10000068') AS a 
		INNER JOIN oitm b ON a."ItemCode" = b."ItemCode" 
		LEFT JOIN ODBN d ON a."ObjAbs" = d."AbsEntry" 
		LEFT JOIN (SELECT t1."LineNum", t1."SnBType", t1."SnBMDAbs", t2."AbsEntry" AS "BinAbs", t2."BinCode", t1."Quantity" FROM DRF19 t1 
			INNER JOIN OBIN t2 ON t1."WhsCode" = t2."WhsCode" AND t1."BinAbs" = t2."AbsEntry" WHERE "DocEntry" = :DocEntry) AS c ON a."LineNum" = c."LineNum" AND a."ObjId" = c."SnBType" AND a."ObjAbs" = c."SnBMDAbs"
	UNION ALL 
	SELECT d.*, ifnull(c."BinAbs", 0), c."BinCode", CASE WHEN ifnull(c."BinAbs", 0) = 0 THEN a."Quantity" ELSE c."Quantity" END, a."WhsCode", c."BinCode", a."LineNum", a."AbsEntry", 'S'
			, CASE WHEN ifnull(c."BinAbs", 0) = 0 THEN a."Quantity" ELSE c."Quantity" END, 'N', 1, 'N' FROM (SELECT * FROM DRF16 WHERE "AbsEntry" = :DocEntry AND "ObjId" = '10000069') AS a 
		INNER JOIN oitm b ON a."ItemCode" = b."ItemCode" 
		LEFT JOIN ODSN d ON a."ObjAbs" = d."AbsEntry" 
		LEFT JOIN (SELECT t1."LineNum", t1."SnBType", t1."SnBMDAbs", t2."AbsEntry" AS "BinAbs", t2."BinCode", t1."Quantity" FROM DRF19 t1 
			INNER JOIN OBIN t2 ON t1."WhsCode" = t2."WhsCode" AND t1."BinAbs" = t2."AbsEntry" WHERE "DocEntry" = :DocEntry) AS c ON a."LineNum" = c."LineNum" AND a."ObjId" = c."SnBType" AND a."ObjAbs" = c."SnBMDAbs";

	UPDATE #tempDraft set "ObjType" = '10000044' where "ObjType" = '10000068';
	UPDATE #tempDraft set "ObjType" = '10000045' where "ObjType" = '10000069';

	UPDATE a SET a."BinCode" = b."BinCode", a."BinAbs" = b."AbsEntry", a."BinDescription" = b."BinCode" FROM #tempDraft a 
				INNER JOIN (SELECT "WhsCode", "AbsEntry", "BinCode" FROM OBIN WHERE "SysBin" = 'Y') AS b ON a."WhsCode" = b."WhsCode" WHERE ifnull(a."BinAbs", 0) = 0;

	select * from #tempDraft;

ELSE
	select ifnull((select top 1 "ClosedQuot" from oadm),'') into closedQuote from DUMMY;
	
	CREATE local temporary TABLE #tblDoc
	(
		"TempBaseLine" INT,
		"TempBaseEntry" INT,
		"TempBaseType" VARCHAR(20),
		"TempLineNum" INT 
	);
	
	select ifnull((SELECT top 1 "TblName" FROM apz_tblobject a WHERE a."ObjType"=:ObjType),'') into tblname from dummy;
	select ifnull((SELECT top 1  "TblLine" FROM apz_tblobject a WHERE a."ObjType"=:ObjType),'') into tblline from dummy;

	IF :ObjType='700021'
	then 

		create local temporary table #temp_ship ("TransType" nvarchar(20) null
												,"TransNo" int null
												,"TransRow" int null
												,"ItemCode" nvarchar(50) null
												,"Quantity" numeric(19,6) null
												,"VisOrder" int null
												,"TreeType" varchar(1));
		
		insert into #temp_ship
			SELECT T0."TransType", T0."TransNo", T0."TransRow", T0."ItemCode", SUM(T0."Quantity" * ifnull(T0."NumPerMsr", 1)), T2."VisOrder", T2."TreeType" FROM APZ_SHIP1 T0 
				INNER JOIN APZ_SHIP T1 ON T0."DocEntry" = T1."DocEntry" 
				INNER JOIN RDR1 T2 ON T2."LineNum" = T0."TransRow" 
				INNER JOIN ORDR T3 ON T0."TransNo" = T3."DocNum" AND T2."DocEntry" = T3."DocEntry"
			 WHERE T0."DocEntry" IN (SELECT Data FROM split(:docentry,',')) AND T1."DocStatus"='O'
	 				GROUP BY T0."TransType",T0."TransNo",T0."TransRow",T0."ItemCode",T2."VisOrder",T2."TreeType";

		create local temporary table #temp_bom (ID int null,VisOrder int null);
		
		insert into #temp_bom
			 SELECT ROW_NUMBER() OVER (ORDER BY "TransNo"), "VisOrder" FROM #temp_ship  WHERE "TreeType"='I';
			 
		select ifnull((SELECT COUNT(1) FROM #temp_ship WHERE "TreeType"='I'),0) into n from DUMMY;
		select ifnull((SELECT TOP 1 "TransNo" FROM #temp_ship),'') into shipdoc from dummy;
		   
		   WHILE :p<=:n
		   DO
			   INSERT INTO #temp_ship ("TransType", "TransNo", "TransRow", "ItemCode", "Quantity", "VisOrder", "TreeType") 
			   		SELECT TOP 1 '17', T1."DocNum", T0."LineNum", T0."ItemCode", T0."Quantity", T0."VisOrder", T0."TreeType" FROM rdr1 T0 
			   					INNER JOIN ORDR T1 ON t0."DocEntry" = T1."DocEntry" WHERE T0."VisOrder" < (SELECT TOP 1 "VisOrder" FROM #temp_bom WHERE ID = :p) AND T0."TreeType" = 'S' 
			   							AND T1."DocNum" = :shipdoc ORDER BY T0."VisOrder" DESC;
			      
				p=:p+1;
				
		   END while;

		  	create local temporary table #ret_ship as (SELECT DISTINCT * FROM #temp_ship);
		  	create local temporary table #tblship as (SELECT T0."TransType", T0."TransNo", T0."TransRow", T0."ItemCode", "Quantity" FROM #ret_ship T0 ORDER BY T0."VisOrder" ASC);

			--SELECT * FROM #tblship

			create local temporary table #tbltempdoc as (
				SELECT A."LineNum" AS "TempBaseLine", A."LineNum", A."BaseLine", A."VisOrder", X."DocNum" AS "TempBaseEntry", '17' AS "TempBaseType"
						, ROW_NUMBER() OVER ( ORDER BY A."DocEntry",A."VisOrder" ASC) AS "TempLineNum" FROM RDR1 A INNER JOIN ORDR X ON A."DocEntry" = X."DocEntry" 
								INNER JOIN #tblship T5 ON T5."TransType" = A."ObjType" AND T5."TransNo" = X."DocNum" AND T5."TransRow" = A."LineNum" 
								INNER JOIN OITW b1 ON a."ItemCode" = b1."ItemCode" AND a."WhsCode" = b1."WhsCode" ORDER BY ROW_NUMBER() OVER ( ORDER BY A."LineNum"));
	
 
			update #tbltempdoc set "BaseLine"="TempBaseLine","LineNum"="TempLineNum"-1,"VisOrder"="TempLineNum"-1;
							 
			INSERT INTO #tblDoc ("TempBaseLine", "TempBaseEntry", "TempBaseType", "TempLineNum") 
				SELECT "TempBaseLine", "TempBaseEntry", "TempBaseType", "LineNum" FROM #tbltempdoc A ORDER BY A."VisOrder";
			--select * from #tblDoc
	elseIF :ObjType='1250000025' or :ObjType='1250000026'
	THEN
		
		insert into #tblDoc
					 SELECT 	 A."AgrLineNum" "TempBaseLine"
							,A."AgrNo" "TempBaseEntry"
							,'1250000025' "TempBaseType"
							,ROW_NUMBER() OVER(ORDER BY A."AgrNo" ASC) - 1 AS "TempLineNum"
				 FROM OAT1 A WHERE A."AgrNo" IN (SELECT Data FROM split(:docentry,','))
					AND (
					(A."LineStatus"='O' AND A."PlanQty">0)
					) ORDER BY  ROW_NUMBER() OVER(ORDER BY A."AgrLineNum");
		
	else
    
		sqlQuery = 'insert into #tblDoc
					 SELECT 	 A."LineNum" "TempBaseLine"
							,A."DocEntry" "TempBaseEntry"
							,A."ObjType" "TempBaseType"
							,ROW_NUMBER() OVER(ORDER BY A."DocEntry", A."LineNum") - 1 AS "TempLineNum"
				 FROM ' || :tblline || ' A WHERE A."DocEntry" IN (SELECT Data FROM split('''|| :docentry || ''','',''))
				AND (
				(A."LineStatus"=''O'' AND A."OpenQty"<>0)
				or (''' || :closedQuote || '''=''Y'' And ''23''=' || :ObjType || ' And ((select "DocStatus" From ' || :tblname || ' where "DocEntry" =A."DocEntry")=''C'')) 
				OR (''' || :ObjType || '''=''112'')
				);';
			--print @sql
			exec :sqlQuery;
			sqlQuery = '';
	END if;

		create local temporary table #tblBatch as (SELECT TOP 0 T0.*, 0 AS "BinAbs", CAST(null AS varchar(50)) AS "BinCode", CAST(null AS decimal(19, 6)) AS "SelectedQty", CAST(null AS varchar(50)) AS "WhsCode"
													, CAST(null AS varchar(254)) AS "BinDescription", CAST(null AS integer) AS "BaseLineNumber", CAST(null AS integer) AS "BaseDocEntry"
													, CAST(null AS varchar(50)) AS "ManBatchSerial", CAST(null AS decimal(19, 6)) AS "LogQty", 'Y' AS "IsExits", 1 AS "TranType", 'Y' AS "IsBase" FROM OBTN T0);
		--select * from #tblBatch
		call apz_fun_getbatchserial_byDocEntry(:docentry,:ObjType, '#tblBatch');
		--SELECT * FROM #tblBatch where ItemCode='FM02111817'

		if :ObjType<>'1250000001'
		then
			UPDATE T0 SET T0."BaseLineNumber"=T1."TempLineNum" FROM #tblBatch T0 INNER JOIN #tblDoc T1 ON T0."BaseDocEntry"=T1."TempBaseEntry" AND T0."BaseLineNumber"=T1."TempBaseLine";  
		end if;
		--SELECT * FROM #tblDoc
		IF :ObjType<>'700021'
		then
		--update a set IsBase = 'Y', a.LogQty = case when a.LogQty < ifnull(b.Quantity,0) - ifnull(b.CommitQty,0) then a.LogQty else ifnull(b.Quantity,0) - ifnull(b.CommitQty,0) end from #tblBatch a INNER JOIN OBTQ b ON a.ItemCode = b.ItemCode and a.AbsEntry = b.MdAbsEntry and a.SysNumber = b.SysNumber where a.ManBatchSerial = 'B'
		--update a set IsBase = 'Y', a.LogQty = case when a.LogQty < ifnull(b.Quantity,0) - ifnull(b.CommitQty,0) then a.LogQty else ifnull(b.Quantity,0) - ifnull(b.CommitQty,0) end from #tblBatch a INNER JOIN OSRQ b ON a.ItemCode = b.ItemCode and a.AbsEntry = b.MdAbsEntry and a.SysNumber = b.SysNumber where a.ManBatchSerial = 'S'
		
			delete from #tblBatch where "LogQty" <= 0;
		end if;
		--SELECT * FROM #tblBatch
		-- apz_document_copy_batch_serial '1,2,3,4,5','700021'
		SELECT * FROM #tblBatch where "LogQty" > 0 ORDER BY "BaseLineNumber";-- and ItemCode='FM02111816'
		--SELECT DISTINCT itemcode,baselinenumber FROM #tblBatch
END if;
END