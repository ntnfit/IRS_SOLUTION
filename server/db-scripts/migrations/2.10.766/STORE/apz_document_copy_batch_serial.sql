
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

	call apz_fun_getbatchserial_byDocEntry(:docEntry, '112', batchSerial);
	select b.*,a."BinAbs",a."BinCode",a."SelectedQty",a."WhsCode",a."BinDescription",a."BaseLineNumber",a."BaseDocEntry",a."ManBatchSerial",a."LogQty",a."IsExits",a."TranType",a."IsBase"
	from :batchSerial a join OBTN b on a."ManBatchSerial"='B' and a."AbsEntry"=b."AbsEntry"
	union all
	select b.*,a."BinAbs",a."BinCode",a."SelectedQty",a."WhsCode",a."BinDescription",a."BaseLineNumber",a."BaseDocEntry",a."ManBatchSerial",a."LogQty",a."IsExits",a."TranType",a."IsBase"
	from :batchSerial a join OSRN b on a."ManBatchSerial"='S' and a."AbsEntry"=b."AbsEntry";

ELSE
	
	declare batchSerial table
	(
		"AbsEntry" int,
		"BinAbs" int,
		"BinCode" nvarchar(228),
		"SelectedQty" decimal,
		"WhsCode" nvarchar(50),		
		"BinDescription" nvarchar(254),
		"BaseLineNumber" int,
		"BaseDocEntry" int,
		"ManBatchSerial" char(1),
		"LogQty"	decimal,		
		"IsExits" char(1),
		"TranType" int,
		"IsBase" char(1)
	);
	
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

		call apz_fun_getbatchserial_byDocEntry(:docEntry, :ObjType, temp);
		insert into :batchSerial select * from :temp;

		if :ObjType<>'1250000001'
		then
			UPDATE T0 SET T0."BaseLineNumber"=T1."TempLineNum" FROM :batchSerial T0 INNER JOIN #tblDoc T1 ON T0."BaseDocEntry"=T1."TempBaseEntry" AND T0."BaseLineNumber"=T1."TempBaseLine";  
		end if;
		--SELECT * FROM #tblDoc
		IF :ObjType<>'700021'
		then
		--update a set IsBase = 'Y', a.LogQty = case when a.LogQty < ifnull(b.Quantity,0) - ifnull(b.CommitQty,0) then a.LogQty else ifnull(b.Quantity,0) - ifnull(b.CommitQty,0) end from #tblBatch a INNER JOIN OBTQ b ON a.ItemCode = b.ItemCode and a.AbsEntry = b.MdAbsEntry and a.SysNumber = b.SysNumber where a.ManBatchSerial = 'B'
		--update a set IsBase = 'Y', a.LogQty = case when a.LogQty < ifnull(b.Quantity,0) - ifnull(b.CommitQty,0) then a.LogQty else ifnull(b.Quantity,0) - ifnull(b.CommitQty,0) end from #tblBatch a INNER JOIN OSRQ b ON a.ItemCode = b.ItemCode and a.AbsEntry = b.MdAbsEntry and a.SysNumber = b.SysNumber where a.ManBatchSerial = 'S'
		
			delete from :batchSerial where "LogQty" <= 0;
		end if;
		--SELECT * FROM #tblBatch
		-- apz_document_copy_batch_serial '1,2,3,4,5','700021'
		select b.*,a."BinAbs",a."BinCode",a."SelectedQty",a."WhsCode",a."BinDescription",a."BaseLineNumber",a."BaseDocEntry",a."ManBatchSerial",a."LogQty",a."IsExits",a."TranType",a."IsBase"
		from :batchSerial a join OBTN b on a."ManBatchSerial"='B' and a."AbsEntry"=b."AbsEntry"
		union all
		select b.*,a."BinAbs",a."BinCode",a."SelectedQty",a."WhsCode",a."BinDescription",a."BaseLineNumber",a."BaseDocEntry",a."ManBatchSerial",a."LogQty",a."IsExits",a."TranType",a."IsBase"
		from :batchSerial a join OSRN b on a."ManBatchSerial"='S' and a."AbsEntry"=b."AbsEntry";
		--SELECT DISTINCT itemcode,baselinenumber FROM #tblBatch
		drop table #tblDoc;
END if;
END