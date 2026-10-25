
CALL APZ_EXISTSPROC('apz_document_copy');

GO

CREATE PROCEDURE apz_document_copy(
-- **** FROM DOCUMENT ****
	in fromtype NVARCHAR(20),
	in docentry NVARCHAR(1024), -- list of docentry
	
-- **** TO DOCUMENT ****
	in totype NVARCHAR(20)
)
AS
BEGIN
	
	DECLARE closedQuote nvarchar(1);
	DECLARE basetype NVARCHAR(50);
	DECLARE tblname NVARCHAR(10)='';
	DECLARE tblline NVARCHAR(10)='';
	DECLARE tbl10 NVARCHAR(10)='';
	DECLARE sqlQuery NVARCHAR(400000)='';
	DECLARE n INT;
	DECLARE p INT=1;
	DECLARE shipdoc INT;
	DECLARE tblTo nvarchar(10);
	DECLARE sql_expns0 NVARCHAR(500000)='';
	DECLARE col_expns0 VARCHAR(500000)='';
	DECLARE WhsDefault VARCHAR(20);
	DECLARE i0 TINYINT=1;
	DECLARE EnblExpns CHAR(1) ='N';
	DECLARE sql_expns NVARCHAR(500000)='';
	DECLARE col_expns VARCHAR(500000)='';
	DECLARE tbl_expns CHAR(4);
	DECLARE i TINYINT=1;
	declare checkCount int = 0;
	
	select ifnull((select top 1 "ClosedQuot" from oadm),'') into closedQuote from DUMMY;
	IF :fromtype='700021'
	then
		tblname='ORDR';
		tblline='RDR1';
		basetype='17';
		tbl10='RDR10';

		create local temporary table #temp_ship ("TransType" nvarchar(20) null,"TransNo" int null,"TransRow" int null,"ItemCode" nvarchar(50) null,"Quantity" numeric(19,6) null,"VisOrder" int null ,"TreeType" varchar(1) null);
		
		INSERT INTO #temp_ship SELECT T0."TransType", T0."TransNo", T0."TransRow", T0."ItemCode", SUM(T0."Quantity" * IFNULL(T0."NumPerMsr", 1)), T2."VisOrder", T2."TreeType" 
			FROM APZ_SHIP1 T0 INNER JOIN APZ_SHIP T1 ON T0."DocEntry" = T1."DocEntry" INNER JOIN RDR1 T2 ON T2."LineNum" = T0."TransRow" 
				INNER JOIN ORDR T3 ON T0."TransNo" = T3."DocNum" AND T2."DocEntry" = T3."DocEntry"
				 WHERE T0."DocEntry" IN (SELECT Data FROM split(:docentry,',')) AND T1."DocStatus"='O'
		 			GROUP BY T0."TransType",T0."TransNo",T0."TransRow",T0."ItemCode",T2."VisOrder",T2."TreeType";

		 create local temporary table #temp_bom as(SELECT ROW_NUMBER() OVER (ORDER BY "TransNo") ID,"VisOrder" FROM #temp_ship  WHERE "TreeType"='I');
		 
		 select ifnull((SELECT COUNT(1) FROM #temp_ship WHERE "TreeType"='I'),0) into n from DUMMY;
		 select ifnull((SELECT TOP 1 "TransNo" FROM #temp_ship),0) into shipdoc from dummy;
		 WHILE p<=n
		 DO
			 INSERT INTO #temp_ship ("TransType", "TransNo", "TransRow", "ItemCode", "Quantity", "VisOrder", "TreeType") 
			 	SELECT TOP 1 '17', T1."DocNum", T0."LineNum", T0."ItemCode", T0."Quantity", T0."VisOrder", T0."TreeType" FROM rdr1 T0 INNER JOIN ORDR T1 ON t0."DocEntry" = T1."DocEntry" 
			 			WHERE T0."VisOrder" < (SELECT TOP 1 "VisOrder" FROM #temp_bom WHERE ID = :p) AND T0."TreeType" = 'S' AND T1."DocNum" = :shipdoc ORDER BY T0."VisOrder" DESC;	      
			 p=:p+1;
		 END while;

		  create local temporary table #ret_ship as (SELECT DISTINCT * FROM #temp_ship);
		  CREATE local temporary TABLE #tblship AS (SELECT T0."TransType", T0."TransNo", T0."TransRow", T0."ItemCode", "Quantity" FROM #ret_ship T0 ORDER BY T0."VisOrder" ASC);

	ELSE
		select ifnull((SELECT TOP 1  "TblName" FROM apz_tblobject a WHERE a."ObjType"=:fromtype),'') into tblname from dummy;
		select ifnull((SELECT TOP 1  "TblLine" FROM apz_tblobject a WHERE a."ObjType"=:fromtype),'') into tblline from dummy;
		tbl10=SUBSTRING(:tblLine,0,LENGTH(:tblLine) - 1) || '10';
		basetype=:fromtype;
	END	if;
	
	select ifnull((SELECT TOP 1 "TblName" FROM apz_tblobject a WHERE a."ObjType"=:totype),'') into tblTo from dummy;
	
	IF :fromtype='1250000025'	-- Blanket Agreement
	then
		
		WHILE i0<=3
		DO
			select ifnull((SELECT string_agg('"' || COLUMN_NAME || '" "' || COLUMN_NAME || '_F' || cast(:i0 as nvarchar) || '"', ',') FROM TABLE_COLUMNS WHERE schema_name = current_schema and TABLE_NAME='POR2'),'') into col_expns0 from dummy;
			sql_expns0 = :sql_expns0 || ' 
			LEFT JOIN                       
					(SELECT ' || :col_expns0 || ' FROM QUT2 WHERE "GroupNum"=' || cast((:i0-1) as nvarchar) || ')
						FG' || cast(:i0 as nvarchar) || ' ON A."AgrNo"=FG'|| cast(:i0 as nvarchar) || '."DocEntry_F' || cast(:i0 as nvarchar) || '" AND FG' || cast(:i0 as nvarchar) || '."LineNum_F' || cast(:i0 as nvarchar) || '" = A."AgrLineNum"';
			i0=:i0+1;
		END WHILE;
		select ifnull((SELECT "DfltWhs" FROM OADM),'') into WhsDefault from dummy;
		sqlQuery = '
			SELECT
					CASE
						WHEN b."ManBtchNum"=''Y'' THEN ''B''
						WHEN b."ManSerNum"=''Y'' THEN ''S''
						ELSE ''N'' END AS "ManBatchSerial"
				  , CASE
						WHEN b."ManBtchNum"=''Y'' THEN ''B''
						WHEN b."ManSerNum"=''Y'' THEN ''S''
						ELSE ''N'' END AS "ManBtchSerial"
				  , ifnull(C."Rate", 1) "TaxPercentagePerRow"
				  , ifnull(C."Rate", 1) "RateVat"
				  , '''' "BOMTreeType"
				  , A."AgrNo" "AgrNo"
				  , D."Number" "AgrNumber"
				  , A."AgrLineNum" "AgrLnNum"
				  , A."PlanQty" AS "Quantity"
				  , A."PlanQty" AS "OpenQty"
				  , CASE WHEN D."Type" <> ''G'' THEN a."UnitPrice" ELSE f."Price" END  AS "PriceBefDi"
				  , CASE WHEN D."Type" <> ''G'' THEN a."UnitPrice" ELSE f."Price" END  AS "Price"
				  , f."Currency" AS "Currency"
				  , 0 AS "DiscPrcnt"
				  , CASE WHEN D."Type" <> ''G'' THEN (A."PlanQty" * A."UnitPrice") ELSE (A."PlanQty" * f."Price") END AS "LineTotal"
				  , A."ItemCode" AS "ItemCode"
				  , A."ItemName" AS "Dscription"
				  , A."UomEntry"
				  , A."UomCode"
				  , A."NumPerMsr"
				  , A."InvntryUom"
				  , b."VatGroupPu"
				  , b."VatGourpSa"
				  , CASE WHEN b."DfltWH" <> '''' THEN b."DfltWH" ELSE ''' || :WhsDefault || ''' END AS "WhsCode"
				  ,''' || :basetype || ''' "TempBaseType"
				  , ROW_NUMBER() OVER ( ORDER BY A."AgrNo" ASC) - 1 AS "LineNum"
				  , ifnull(b."SUoMEntry", -1) AS "SUoMEntry"
				  , (select "UomCode" from OUOM where "UomEntry" = b."SUoMEntry") "SUoMCode"
				  , b."SalUnitMsr" AS "SalUnitMsr"
				  , ifnull(b."NumInSale", 1) AS "NumInSale"
				  , ifnull(b."PUoMEntry", -1) AS "PUoMEntry"
				  , (select "UomCode" from OUOM where "UomEntry" = b."PUoMEntry") "PUoMCode"
				  , b."BuyUnitMsr" AS "BuyUnitMsr"
				  , ifnull(b."NumInBuy", 1) AS "NumInBuy"
				  , ifnull(b."UgpEntry", -1) AS "UgpCode"
				  , b."InvntryUom" AS "InvUnitMsr"
				  , ifnull(T4."AltQty", 1) / ifnull(T4."BaseQty", 1) AS "NumInInven"
				  , cast(null as integer) AS "LineSeq"
				  , ifnull(b."OnHand", null) AS "QuantityOnStock"
				  , ifnull(b."IsCommited", null) AS "IsCommited"
				  , ifnull(b."OnOrder", null) AS "OnOrder"
				  , ifnull(b1."OnHand", null) AS "QuantityOnWareHouse"
				  , ifnull(b."IsCommited", null) AS "IsCommited"
				  , ifnull(b1."IsCommited", null) AS "IsCommitedInWareHouse"
				  , ifnull(b."OnOrder", null) AS "OnOrder"
				  , ifnull(b1."OnOrder", null) AS "OnOrderInWareHouse"
				  , b."PurPackUn", b."SalPackUn", b1."AvgPrice"
				  , C.*, FG1.*, FG2.*, FG3.*
				FROM OAT1 A
					LEFT JOIN QUT1 C ON C."DocEntry"=0
					INNER JOIN OITM b ON A."ItemCode"=b."ItemCode"
					LEFT JOIN UGP1 T4 ON b."UgpEntry"=T4."UgpEntry"
										 AND T4."UomEntry" IN (SELECT "UomEntry" FROM OUOM WHERE "UomName"=b."InvntryUom")
					INNER JOIN OOAT d ON d."AbsID" = a."AgrNo"
					INNER JOIN OCRD e ON e."CardCode" = d."BpCode"
					INNER JOIN ITM1 f ON f."ItemCode" = a."ItemCode" AND f."PriceList" = e."ListNum"
					LEFT JOIN OITW b1 on a."ItemCode" = b1."ItemCode" and (CASE WHEN  b."DfltWH" <>'''' THEN b."DfltWH" else ''' || :WhsDefault || ''' end) = b1."WhsCode" 
					' || :sql_expns0 || '
				WHERE  A."AgrNo" IN (SELECT Data FROM split(''' || :docentry || ''','',''))
					AND (
					(A."LineStatus"=''O'' AND A."PlanQty">0)AND d."Status"=''A'')
				ORDER BY A."VisOrder";';
		--PRINT @SQL
		EXECUTE immediate :sqlQuery;
		RETURN;
	END if;

	EnblExpns='Y';--ifnull((SELECT EnblExpns FROM cinf),'N')
	IF	:EnblExpns='Y'
	then
			
		tbl_expns=SUBSTRING(:tblLine,0,LENGTH(:tblLine) - 1) || '2';

		WHILE i<=3
		DO
			select ifnull((SELECT string_agg('"' || COLUMN_NAME || '" "' || COLUMN_NAME || '_F' || cast(:i as nvarchar) || '",') FROM TABLE_COLUMNS WHERE schema_name = current_schema and TABLE_NAME='POR2'),'') into col_expns from dummy;
			col_expns=SUBSTRING(:col_expns,0,LENGTH(:col_expns) - 1);
			sql_expns= :sql_expns || ' 
			LEFT JOIN                       
					(SELECT ' || :col_expns || ' FROM ' || :tbl_expns || ' WHERE "GroupNum"=' || cast((:i-1) as nvarchar) || ')
					 FG' || cast(:i as nvarchar) || ' ON A."DocEntry"=FG' || cast(:i as nvarchar) || '."DocEntry_F' || cast(:i as nvarchar) || '" AND FG' || cast(:i as nvarchar) || '."LineNum_F' || cast(:i as nvarchar) || '" = A."LineNum"';
			i=:i+1;
		END while;
	END if;

	sqlQuery = 'DO
				BEGIN
					create local temporary table #tbl as (SELECT	 A.*
						,(select cast('''' as ntext) from dummy) "LineText"
						,case when b."ManBtchNum"=''Y'' then ''B'' when b."ManSerNum"=''Y'' then ''S'' else ''N'' END as "ManBatchSerial"
						,case when b."ManBtchNum"=''Y'' then ''B'' when b."ManSerNum"=''Y'' then ''S'' else ''N'' END as "ManBtchSerial"
						,ifnull(C."Rate",null) "TaxPercentagePerRow"
						,ifnull(C."Rate",null) "RateVat"
						,ifnull(A."TreeType", null) AS "BOMTreeType"
						, ifnull(A."LineNum",null) AS "TempBaseLine"
						, ifnull(A."DocEntry",null) AS "TempBaseEntry"
						, ifnull(Header."DocNum", null) AS "TempBaseRef"
						,''' || :basetype || ''' "TempBaseType"
						,ROW_NUMBER() OVER ( ORDER BY A."DocEntry",A."LineNum" ASC) AS "TempLineNum"
						,ROW_NUMBER() OVER ( ORDER BY A."DocEntry",A."VisOrder" ASC) AS "TempVisOrder"
						, b."SalUnitMsr" AS "SalUnitMsr"
						, ifnull(b."NumInSale", null) AS "NumInSale"
						, b."BuyUnitMsr" AS "BuyUnitMsr"
						, ifnull(b."NumInBuy", null) AS "NumInBuy"
						, ifnull(b."UgpEntry", -1) AS "UgpCode"
						, b."InvntryUom" AS "InvUnitMsr"
						, ifnull(T4."AltQty", 1) / ifnull(T4."BaseQty", 1) AS "NumInInven"
						, cast(null as integer) AS "LineSeq"
						, ifnull(b."OnHand", null) AS "QuantityOnStock"
						, ifnull(b1."OnHand", null) AS "QuantityOnWareHouse"
						, ifnull(b."IsCommited", null) AS "IsCommited"
						, ifnull(b1."IsCommited", null) AS "IsCommitedInWareHouse"
						, ifnull(b."OnOrder", null) AS "OnOrder"
						, ifnull(b1."OnOrder", null) AS "OnOrderInWareHouse"
						, b."PurPackUn", b."SalPackUn", b1."AvgPrice"
						, (1 - ((1 - A."DiscPrcnt" / 100) * (1 - Header."DiscPrcnt" / 100))) * 100 AS "TempDisc"';
	IF :fromtype='700021'
	then
		sqlQuery= :sqlQuery || ' ,T5."Quantity" "ShipQuantity" ';
	END if;
	
	sqlQuery= :sqlQuery || ',FG1.*,FG2.*,FG3.* ';
	sqlQuery= :sqlQuery || '
				FROM ' || :tblline || ' A INNER JOIN ' || :tblname || ' Header on A."DocEntry"=Header."DocEntry"';
				
	IF :fromtype='700021'
	then
		sqlQuery= :sqlQuery || ' INNER JOIN ' || :tblname || ' X ON A."DocEntry"=X."DocEntry" INNER JOIN #tblship T5 ON T5."TransType"=A."ObjType" and T5."TransNo"=X."DocNum" and T5."TransRow"=A."LineNum"';
	END	if;

	sqlQuery= :sqlQuery || ' LEFT JOIN OVTG C on C."Code"=A."VatGroup"  
					INNER JOIN OITM b on a."ItemCode" = b."ItemCode"
					LEFT JOIN OITW b1 on a."ItemCode" = b1."ItemCode" and a."WhsCode" = b1."WhsCode"
					LEFT JOIN UGP1 T4 on b."UgpEntry" = T4."UgpEntry" and T4."UomEntry" in (select "UomEntry" from OUOM where "UomName" = b."InvntryUom") ';
	sqlQuery= :sqlQuery || :sql_expns;
	IF :fromtype<>'700021'
	then
		sqlQuery= :sqlQuery || '
			WHERE  A."DocEntry" IN (SELECT Data FROM split(''' || :docentry || ''','',''))
			-- Các Line còn Open
				AND (
				(A."LineStatus"=''O'' AND (A."OpenQty" <> 0 OR A."Quantity"=0))
				or (''' || :closedQuote || '''=''Y'' And ''23''=' || :fromtype || ' ) 
				OR (''' || :fromtype || '''=''112'')
				));
			-- Số lượng open >0';
	ELSE
		sqlQuery= :sqlQuery || ';';
	END	if;
		
	IF :fromtype<>'112'
	then
	
		sqlQuery= :sqlQuery || '
		
		Update #tbl Set 
						"Quantity"="OpenQty"
						Where (select "DocStatus" From ' || :tblname || ' where "DocEntry"=#tbl."DocEntry")<>''C'';';
		sqlQuery= :sqlQuery || ' Update #tbl Set  "LineStatus"=''O'';
						update #tbl set "BaseType"="TempBaseType"
								,"BaseEntry"="TempBaseEntry"
								,"BaseRef" ="TempBaseRef"
								,"BaseLine"="TempBaseLine"
								,"LineNum"="TempLineNum"-1
								,"VisOrder"="TempVisOrder"-1
								,"TargetType"=-1
								,"TrgetEntry"=0; ';
		select ifnull((SELECT COUNT(1) FROM split(:docentry,',')),0) into checkCount from dummy;
		IF :checkCount > 1
		then
	
			sqlQuery= :sqlQuery || ' 
				update #tbl set "DiscPrcnt"="TempDisc","LineTotal"=( "Quantity"*"PriceBefDi" ) * (1-"TempDisc"/100),"Price"="PriceBefDi" * (1-"TempDisc"/100); ';
			
		end	if;    	
	END	 if;
	
	IF :fromtype='700021'
	then
		sqlQuery= :sqlQuery || ' update #tbl set "Quantity"="ShipQuantity","OpenQty"="ShipQuantity"; 
			UPDATE T0 SET T0."InvQty"=T0."Quantity", T0."NumPerMsr"=1 FROM #tbl  T0 WHERE T0."UseBaseUn"=''Y'';
			UPDATE T0 SET T0."InvQty"=T0."Quantity" * T0."NumPerMsr" FROM #tbl  T0 WHERE T0."UseBaseUn="''N'';';
	END if;
	
	sqlQuery= :sqlQuery || ' 
			Update #tbl SET "PQTReqQty"="Quantity","Quantity"=0 WHERE ''' || :fromtype || ''' = ''1470000113'' AND ''' || :totype || '''=''540000006'';
			Update #tbl SET "LineVendor"=null WHERE ''' || :fromtype || ''' = ''1470000113'' AND ''' || :totype || '''<>''1470000113'';
			Update #tbl Set "Rate"=1 where "Rate"=0;
			--alter table #tbl
							--alter column itemcode ntext
			INSERT INTO #tbl ("BOMTreeType", "LineStatus", "DocEntry", "LineNum", "ItemCode", "LineText", "VisOrder", "Dscription", "SalUnitMsr", "NumInSale", "BuyUnitMsr", "NumInBuy", "UgpCode", "InvUnitMsr", "NumInInven", "ManBatchSerial", "ManBtchSerial", "LineType", "TaxPercentagePerRow", "RateVat", "TempBaseLine", "TempBaseEntry", "TempBaseType", "TempLineNum", "LineSeq", "QuantityOnStock", "QuantityOnWareHouse", "IsCommited", "IsCommitedInWareHouse", "OnOrder", "OnOrderInWareHouse", "ItemType")
							SELECT '''', ''O'',"DocEntry","AftLineNum",'''', "LineText","AftLineNum",CASE WHEN "LineType"=''S'' THEN ''SubTotal'' ELSE '''' END,'''',0,'''',0,-1,'''',0,''N'', ''N'',"LineType",0,0,0,0,-1,0,"LineSeq",0,0,0,0,0,0,-18
							FROM ' || :tbl10 || ' WHERE "DocEntry" IN (SELECT Data FROM split(''' || :docentry || ''','',''));
			update #tbl set "ShipToDesc" = NULL WHERE ifnull("ShipToCode",'''')='''';
			update #tbl set "InvQty" = "Quantity" * "NumPerMsr";
		END;';
	--PRINT @sql
	--select :sqlQuery from DUMMy;
	--return;
	EXEC :sqlQuery;
	execute immediate 'SELECT  * FROM #tbl order by "VisOrder","LineSeq";';
	exec 'DROP TABLE #tbl;';
END