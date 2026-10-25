-- B1 DEPENDS: AFTER:SP:_TmSp_BootCreateGlobalTempTables2 AFTER:PT:PROCESS_END AFTER:SP:_TmSp_ValidateSpParam

CREATE  PROCEDURE _TmSp_PrepareAuditReportData(
	in_nvc_pagingID NVARCHAR(100),
	in_i_dataMgrFlag INTEGER -- 0 Drop, 1 Create, 2 Retrieve
    )
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER

AS
	v_tempTableName nvarchar(40);
	v_localSetting nvarchar(2);
	v_groupType nvarchar(2);--(by acct or item)    (1-byitem  2-byacct)
	v_forceOBDisplay nvarchar(2);
	v_dateType nvarchar(2);--(systemdate or posting date)  (0-postingdate, 1-systemdate)
	v_dateFrom nvarchar(20); 
	v_dateTo nvarchar(20);
	v_itemCodeFrom nvarchar(50);
	v_itemCodeTo nvarchar(50);
	v_itemGroupCode nvarchar(20);
	v_itemGroupProperties nvarchar(100); 
	v_itemgrouppropertieStoken nvarchar(2);--  (Y-AND N-OR)
	v_itemgrouppropertieSuseGroups nvarchar(2);--  Y/N
	v_itemgrouppropertieSCLOSEDRange nvarchar(2);--  Y/N
	v_invntActGroup nvarchar(8000);
	v_warehouse nvarchar(8000);
	v_obDescription nvarchar(8000);
	v_hideItemCumulativeQty0 nvarchar(2);
	v_T_SQL nclob;
	v_cumValue DECIMAL(21,6) := 0.0;
	v_i int;
	cnt int;
	v_dateType_Name varchar(100);
	v_dateType_Summary nvarchar(100);

	v_TransValue DECIMAL(21,6);
	v_LineFlag int;
	v_priceSys nvarchar(2);
    CURSOR c_cursor1 FOR SELECT * from tmp_PrepareAuditReportData1 order by "PaginationId";
	CURSOR c_cursor2 FOR SELECT * from tmp_PrepareAuditReportData2 order by "PaginationId";
	CURSOR c_cursor3 FOR SELECT * from tmp_PrepareAuditReportData3 order by "PaginationId";
	CURSOR c_cursor4 FOR SELECT * from tmp_PrepareAuditReportData4 order by "PaginationId";
	
	v_localSetting206  nvarchar(2000);
	v_localSetting219  nvarchar(2000);
	v_localSetting456  nvarchar(2000);
	v_localSetting469  nvarchar(2000);
	v_localSetting657  nvarchar(2000);
	v_localSetting670  nvarchar(2000);
	v_localSetting898  nvarchar(2000);
	v_localSetting1019 nvarchar(2000);
	v_localSetting1261 nvarchar(2000);
	v_localSetting1274 nvarchar(2000);
	v_localSettingMoreColumn nvarchar(100);
	
	v_groupType99     nvarchar(2000);
	v_groupType223     nvarchar(2000);
	v_groupType340     nvarchar(2000);
	v_groupType473     nvarchar(2000);
	v_groupType586     nvarchar(2000);
	v_groupType587     nvarchar(2000);
	v_groupType787     nvarchar(2000);
	v_groupType790     nvarchar(2000);
	v_groupType911     nvarchar(2000);
	v_groupType1148    nvarchar(2000);
	v_groupType1153    nvarchar(2000);
	v_groupType1392    nvarchar(2000);
	v_groupType1396    nvarchar(2000);
	v_groupType1557    nvarchar(2000);
	v_groupType1558    nvarchar(2000);
	v_groupType1652    nvarchar(2000);
	v_groupType406     nvarchar(2000);
	
	l_invntActGroup nvarchar(8000);
	l_warehouse nvarchar(8000);
	l_obDescription nvarchar(8000);
	v_CumulativeValue DECIMAL(21,6) := 0;
	v_CumulativeQty DECIMAL(21,6) := 0;
	v_CumulativeCost DECIMAL(21,6) := 0;
BEGIN
--For HANA security issue call procedure "_TmSp_ValidateSpParam"
    call _TmSp_ValidateSpParam(:in_nvc_pagingID);    
	
	SELECT "Value" INTO v_localSetting FROM "SPPP" WHERE "PagingID" = :in_nvc_pagingID AND "Name" = 'v_localSetting';
	SELECT "Value" INTO v_groupType FROM "SPPP" WHERE "PagingID" = :in_nvc_pagingID AND "Name" = 'v_groupType';  
	SELECT "Value" INTO v_forceOBDisplay FROM "SPPP" WHERE "PagingID" = :in_nvc_pagingID AND "Name" = 'v_forceOBDisplay'; 
	SELECT "Value" INTO v_dateType FROM "SPPP" WHERE "PagingID" = :in_nvc_pagingID AND "Name" = 'v_dateType'; 
	SELECT "Value" INTO v_dateFrom FROM "SPPP" WHERE "PagingID" = :in_nvc_pagingID AND "Name" = 'v_dateFrom'; 
	SELECT "Value" INTO v_dateTo FROM "SPPP" WHERE "PagingID" = :in_nvc_pagingID AND "Name" = 'v_dateTo'; 
	SELECT "Value" INTO v_itemCodeFrom FROM "SPPP" WHERE "PagingID" = :in_nvc_pagingID AND "Name" = 'v_itemCodeFrom'; 
	SELECT "Value" INTO v_itemCodeTo FROM "SPPP" WHERE "PagingID" = :in_nvc_pagingID AND "Name" = 'v_itemCodeTo'; 
	SELECT "Value" INTO v_itemGroupCode FROM "SPPP" WHERE "PagingID" = :in_nvc_pagingID AND "Name" = 'v_itemGroupCode';
	SELECT "Value" INTO v_itemGroupProperties FROM "SPPP" WHERE "PagingID" = :in_nvc_pagingID AND "Name" = 'v_itemGroupProperties';
	SELECT "Value" INTO v_itemgrouppropertieStoken FROM "SPPP" WHERE "PagingID" = :in_nvc_pagingID AND "Name" = 'v_itemgrouppropertieStoken'; 
	SELECT "Value" INTO v_itemgrouppropertieSuseGroups FROM "SPPP" WHERE "PagingID" = :in_nvc_pagingID AND "Name" = 'v_itemgrouppropertieSuseGroups'; 
	SELECT "Value" INTO v_itemgrouppropertieSCLOSEDRange FROM "SPPP" WHERE "PagingID" = :in_nvc_pagingID AND "Name" = 'v_itemgrouppropertieSCLOSEDRange'; 
	SELECT "Value" INTO v_invntActGroup FROM "SPPP" WHERE "PagingID" = :in_nvc_pagingID AND "Name" = 'v_invntActGroup';  
	SELECT "Value" INTO v_warehouse FROM "SPPP" WHERE "PagingID" = :in_nvc_pagingID AND "Name" = 'v_warehouse';
	SELECT "Value" INTO v_obDescription FROM "SPPP" WHERE "PagingID" = :in_nvc_pagingID AND "Name" = 'v_obDescription';
	SELECT "Value" INTO v_hideItemCumulativeQty0 FROM "SPPP" WHERE "PagingID" = :in_nvc_pagingID AND "Name" = 'v_hideItemCumulativeQty0';

	v_tempTableName := UPPER('#TMP_' || :in_nvc_pagingID);

		if ifnull(:v_obDescription,'') <> '' then
			l_obDescription := v_obDescription;
		else 
			l_obDescription := 'Opening Balance';
		end if;
		IF :v_dateType = 0 then
			v_dateType_Name := 'T0."DocDate"';
		else
			v_dateType_Name := 'T0."CreateDate"';
		end if;
		
		if :v_localSetting = 'BR' then
			v_localSettingMoreColumn := '';

			v_localSetting206 := ',T5."NfmCode"
			,T3."Serial"
			,T3."SeriesStr"
			,T3."SubStr"
			,T3."TaxDate"
			,T4."NcmCode"
			,T3."CFOPCode"';
			v_localSetting219 := ' LEFT OUTER  JOIN "DOC1_View" T3 ON T3."DocEntry" = T0."CreatedBy" AND T3."DocType" = T0."TransType" AND T3."LineNum" = T0."DocLineNum"   
			LEFT OUTER  JOIN "ONCM" T4  ON  T4."AbsEntry" = T1."NCMCode"   
			LEFT OUTER  JOIN "ONFM" T5  ON  T5."AbsEntry" = T3."Model" ';
			
			v_localSetting456 := ',NULL AS "NfmCode"
			,NULL AS "Serial"
			,NULL AS "SeriesStr"
			,NULL AS "SubStr"
			,NULL AS "TaxDate"
			,NULL AS "NcmCode"
			,NULL AS "CFOPCode"';

			v_localSetting469 := 'LEFT OUTER  JOIN "DOC1_View" T3 ON T3."DocEntry" = T0."CreatedBy" AND T3."DocType" = T0."TransType" AND T3."LineNum" = T0."DocLineNum"   
			LEFT OUTER  JOIN "ONCM" T4  ON  T4."AbsEntry" = T1."NCMCode"   
			LEFT OUTER  JOIN "ONFM" T5  ON  T5."AbsEntry" = T3."Model" ';
			v_localSetting657 := ',NULL AS "NfmCode"
			,NULL AS "Serial"
			,NULL AS "SeriesStr"
			,NULL AS "SubStr"
			,NULL AS "TaxDate"
			,NULL AS "NcmCode"
			,NULL AS "CFOPCode"';
			v_localSetting670 := 'LEFT OUTER  JOIN "DOC1_View" T3 ON T3."DocEntry" = T0."CreatedBy" AND T3."DocType" = T0."TransType" AND T3."LineNum" = T0."DocLineNum"   
			LEFT OUTER  JOIN "ONCM" T4  ON  T4."AbsEntry" = T1."NCMCode"   
			LEFT OUTER  JOIN "ONFM" T5  ON  T5."AbsEntry" = T3."Model" ';
			v_localSetting898 := ',NULL AS "NfmCode"
			,NULL AS "Serial"
			,NULL AS "SeriesStr"
			,NULL AS "SubStr"
			,NULL AS "TaxDate"
			,NULL AS "NcmCode"
			,NULL AS "CFOPCode"';
			v_localSetting1019 := ',NULL AS "NfmCode"
				,NULL AS "Serial"
				,NULL AS "SeriesStr"
				,NULL AS "SubStr"
				,NULL AS "TaxDate"
				,NULL AS "NcmCode"
				,NULL AS "CFOPCode"';
			v_localSetting1261 := ',NULL AS "NfmCode"
				,NULL AS "Serial"
				,NULL AS "SeriesStr"
				,NULL AS "SubStr"
				,NULL AS "TaxDate"
				,NULL AS "NcmCode"
				,NULL AS "CFOPCode"';
			v_localSetting1274 := 'LEFT OUTER  JOIN "DOC1_View" T3 ON T3."DocEntry" = T0."CreatedBy" AND T3."DocType" = T0."TransType" AND T3."LineNum" = T0."DocLineNum"   
			LEFT OUTER  JOIN "ONCM" T4  ON  T4."AbsEntry" = T1."NCMCode"   
			LEFT OUTER  JOIN "ONFM" T5  ON  T5."AbsEntry" = T3."Model" ';
			--v_localSetting1401 := CN
		else 
			v_localSettingMoreColumn := ',NULL,NULL,NULL,NULL,NULL,NULL,NULL';
			v_localSetting206 := '';
			v_localSetting219 := '';
			v_localSetting456 := '';
			v_localSetting469 := '';
			v_localSetting657 := '';
			v_localSetting670 := '';
			v_localSetting898 := '';
			v_localSetting1019 := '';
			v_localSetting1261 := '';
			v_localSetting1274 :=  '';
			--v_localSetting1401 := '';
		end if; --localSetting
		
		if :v_groupType = 1 then
			if :v_localSetting = 'CN' THEN
			--cn
				v_groupType406 := ' WHEN 0 THEN 0
				 ELSE CASE MAX(T1."EvalSystem") 
				 WHEN ''F'' 
				 THEN 0
				 ELSE CASE SUM(IFNULL(T0."TransValue",0.0))
					  WHEN 0
					  THEN 0
					  ELSE SUM(IFNULL(T0."TransValue",0.0))/(SUM(T0."InQty") - SUM(T0."OutQty")) 
					  END
				 END ';
			ELSE 
				v_groupType406 := ' WHEN 0 THEN 0
				 ELSE CASE MAX(T1."EvalSystem") 
				 WHEN ''F'' 
				 THEN 0
				 ELSE CASE SUM(IFNULL(T0."TransValue",0.0))
					  WHEN 0
					  THEN 0
					  ELSE 0 
					  END
				 END ';
			end if;
			
			v_groupType99 :=  'insert into tmp_PrepareAuditReportDataT1 SELECT *' || :v_localSettingMoreColumn ||' FROM (SELECT T1."ItemCode" AS "ItemCode",
			T1."ItemName" AS "ItemName",	
			T0."TransSeq" AS "TransSeq",	
			T0."CreateDate" AS "CreateDate", 
			T0."DocDate" AS "DocDate",	
			NULL AS "Document",
			T0."DocLineNum" + 1 AS "DocLineNum", 
			T2."WhsCode" AS "WhsCode",
			T0."InQty" - T0."OutQty" AS "Qty",
			T0."CalcPrice" AS "CalcPrice", 
			T0."TransValue" AS  "TransValue",
			0 AS  "CumulativeQty",
			NULL AS "CumulativeCost",
			0 AS "CumulativeValue",
			T0."InvntAct" AS "GLAccount", 
			T1."EvalSystem" AS "EvalSystem",
			NULL AS "ValuationMethod",	
			NULL AS "CurrentItemCost",
			T0."Currency" AS "Currency",
			T0."Rate" AS "Rate", 
			T0."UserSign" AS "UserSign", 
			T0."TransType" AS "TransType",
			T0."CreatedBy" AS "CreatedBy", 
			T0."BASE_REF" AS "BASE_REF", 	
			T1."InvntryUom" AS "InvntryUom", 
			T0."CostAct"  AS "CostAct", 
			300 AS "LineFlag"'	;
			v_groupType223 := ' OR T0."InQty" <> 0 OR T0."OutQty" <> 0 OR T0."TransType" = 162';
			v_groupType340 := ' SELECT DISTINCT T1."ItemCode" AS "ItemCode",
			MAX(T1."ItemName") AS "ItemName",
			NULL AS "TransSeq",	
			NULL AS "CreateDate", 
			NULL AS "DocDate", 
			NULL AS "Document",
			NULL AS "DocLineNum",
			NULL AS "WhsCode",
			NULL AS "Qty",
			NULL AS "CalcPrice", 
			NULL AS  "TransValue",
			SUM(T0."InQty") - SUM(T0."OutQty") AS  "CumulativeQty",
			CASE SUM(T0."InQty") - SUM(T0."OutQty")		
			' || :v_groupType406 || '
			END AS "CumulativeCost",
			SUM(IFNULL(T0."TransValue",0.0)) AS "CumulativeValue",
			NULL AS "GLAccount", 
			NULL AS "EvalSystem",
			MAX(T1."EvalSystem") AS "ValuationMethod",
			0 AS "CurrentItemCost",
			NULL AS "Currency",
			NULL AS "Rate", 
			NULL AS "UserSign", 
			NULL AS "TransType",
			NULL AS "CreatedBy", 
			NULL AS "BASE_REF",
			NULL AS "InvntryUom",
			NULL  AS "CostAct", 
			1 AS "LineFlag"';
			v_groupType473 := ' OR T0."InQty" <> 0 OR T0."OutQty" <> 0 OR T0."TransType" = 162';
			v_groupType586 := ' GROUP BY T1."ItemCode"';
			v_groupType587 := '';
			v_groupType787 := ' GROUP BY T2."WhsCode", T1."ItemCode"';
			v_groupType790 := ' UNION ALL SELECT DISTINCT B."ItemCode" AS "ItemCode",
			NULL AS "ItemName",			
			NULL AS "TransSeq",
			NULL AS "CreateDate",
			NULL AS "DocDate", ''' ||
			:l_obDescription || ''' AS "Document",	
			NULL AS "DocLineNum", 	 
			NULL AS "WhsCode",
			SUM(IFNULL(B."Qty",0.0)) AS "Qty",
			NULL AS "CalcPrice", 
			SUM(IFNULL(B. "TransValue",0.0)) AS  "TransValue",
			SUM(IFNULL(B. "CumulativeQty",0.0)) AS  "CumulativeQty",
			NULL AS "CumulativeCost",
			SUM(IFNULL(B."CumulativeValue",0.0)) AS "CumulativeValue",		
			NULL AS "GLAccount", 
			NULL AS "EvalSystem",
			NULL AS "ValuationMethod",
			NULL AS "CurrentItemCost",
			NULL AS "Currency",
			NULL AS "Rate", 
			NULL AS "UserSign", 
			NULL AS "TransType",
			NULL AS "CreatedBy", 
			NULL AS "BASE_REF", 		
			NULL AS "InvntryUom", 		
			NULL AS "CostAct", 
			100 AS "LineFlag"';
			v_groupType911 := 'SELECT DISTINCT T1."ItemCode" AS "ItemCode",
				NULL AS "ItemName",
				NULL AS "TransSeq",
				NULL AS "CreateDate",
				NULL AS "DocDate", ''' ||
				:l_obDescription || ''' AS "Document",
				NULL AS "DocLineNum", 	 
				NULL AS "WhsCode",
				SUM(T0."InQty") - SUM(T0."OutQty") AS "Qty",
				NULL AS "CalcPrice", 
				SUM(IFNULL(T0."TransValue",0.0)) AS  "TransValue",
				SUM(T0."InQty") - SUM(T0."OutQty") AS  "CumulativeQty",
				NULL AS "CumulativeCost",
				SUM(IFNULL(T0."TransValue",0.0)) AS "CumulativeValue",		
				NULL AS "GLAccount", 
				NULL AS "EvalSystem",
				NULL AS "ValuationMethod",
				NULL AS "CurrentItemCost",
				NULL AS "Currency",
				NULL AS "Rate", 
				NULL AS "UserSign", 
				NULL AS "TransType",
				NULL AS "CreatedBy", 
				NULL AS "BASE_REF", 		
				NULL AS "InvntryUom", 		
				NULL AS "CostAct", 
				100 AS "LineFlag"';
			v_groupType1148 := ' GROUP BY T1."ItemCode"';
			v_groupType1153 := 'SELECT DISTINCT T1."ItemCode" AS "ItemCode",
				NULL AS "ItemName",				
				NULL AS "TransSeq",
				NULL AS "CreateDate",
				NULL AS "DocDate",''' ||
				:l_obDescription || ''' AS "Document",
				NULL AS "DocLineNum", 
				NULL AS "WhsCode",	 
				0 AS "Qty",
				NULL AS "CalcPrice", 
				0 AS  "TransValue",
				0 AS  "CumulativeQty",
				NULL AS "CumulativeCost",
				0 AS "CumulativeValue",		
				NULL AS "InvntAct", 
				NULL AS "EvalSystem",
				NULL AS "ValuationMethod",
				NULL AS "CurrentItemCost",
				NULL AS "Currency",
				NULL AS "Rate", 
				NULL AS "UserSign", 
				NULL AS "TransType",
				NULL AS "CreatedBy", 
				NULL AS "BASE_REF", 		
				NULL AS "InvntryUom", 		
				NULL AS "CostAct", 
				100 AS "LineFlag"';
			v_groupType1392 := 'B."ItemCode"';
			v_groupType1396 := 'A."ItemCode"' ;
			v_groupType1557 := '(1)';
			v_groupType1558 := '"ItemCode"';
			v_groupType1652 := '"LineFlag" = ''1'' ';

		elseif :v_groupType = 2 then
			v_groupType99 := 'insert into tmp_PrepareAuditReportDataT2 SELECT *' || :v_localSettingMoreColumn ||' FROM (SELECT NULL AS "GLAccount",
			T0."TransSeq" AS "TransSeq",
			T0."CreateDate" AS "CreateDate",
			T0."DocDate" AS "DocDate", 
			T0."DocLineNum" + 1 AS "DocLineNum", 
			T1."ItemCode" AS "ItemCode",
			T1."ItemName" AS "ItemName",	 
			T2."WhsCode" AS "WhsCode",
			T0."InQty" - T0."OutQty" AS "Qty",
			T0."CalcPrice" AS "CalcPrice", 
			T0."TransValue" AS  "TransValue",
			NULL AS "CumulativeValue",
			T0."InvntAct" AS "InvntAct", 
			T1."EvalSystem" AS "EvalSystem", 
			T0."Currency" AS "Currency",
			T0."Rate" AS "Rate", 
			T0."UserSign" AS "UserSign", 
			T0."TransType" AS "TransType",
			T0."CreatedBy" AS "CreatedBy", 
			T0."BASE_REF" AS "BASE_REF",	
			T1."InvntryUom" AS "InvntryUom", 	
			T0."CostAct"  AS "CostAct", 
			300 AS "LineFlag"';
			v_groupType223 := '';
			v_groupType340 :=  ' SELECT DISTINCT T0."InvntAct" AS "GLAccount",
			NULL AS "TransSeq",
			NULL AS "CreateDate", 
			NULL AS "DocDate", 
			NULL AS "DocLineNum", 
			NULL AS "ItemCode",
			NULL AS "ItemName",	
			NULL AS "WhsCode",
			NULL AS "Qty",
			NULL AS "CalcPrice", 
			NULL AS  "TransValue",
			SUM(IFNULL(T0."TransValue",0.0)) AS "CumulativeValue",
			T0."InvntAct" AS "InvntAct", 
			NULL AS "EvalSystem",
			NULL AS "Currency",
			NULL AS "Rate", 
			NULL AS "UserSign", 
			NULL AS "TransType",
			NULL AS "CreatedBy", 
			NULL AS "BASE_REF",	
			NULL AS "InvntryUom", 
			NULL  AS "CostAct", 
			1 AS "LineFlag"';
			v_groupType473 := '';
			v_groupType586 := ' GROUP BY T0."InvntAct"';
			v_groupType587 := '';
			v_groupType787 := ' GROUP BY T2."WhsCode", T1."ItemCode"';
			v_groupType790 := ' UNION ALL SELECT DISTINCT ''' || :l_obDescription || ''' AS "GLAccount",
			NULL AS "TransSeq",
			NULL AS "CreateDate", 
			NULL AS "DocDate", 
			NULL AS "DocLineNum",
			NULL AS "ItemCode",
			NULL AS "ItemName",	
			NULL AS "WhsCode",
			NULL AS "Qty",
			NULL AS "CalcPrice", 
			SUM(IFNULL(B."CumulativeValue",0.0)) AS  "TransValue",
			SUM(IFNULL(B."CumulativeValue",0.0)) AS "CumulativeValue",
			B."InvntAct" AS "InvntAct", 
			NULL AS "EvalSystem", 
			NULL AS "Currency",
			NULL AS "Rate", 
			NULL AS "UserSign", 
			NULL AS "TransType",
			NULL AS "CreatedBy", 
			NULL AS "BASE_REF", 	 
			NULL AS "InvntryUom", 	
			NULL  AS "CostAct", 
			100 AS "LineFlag"';
			v_groupType911 := 'SELECT DISTINCT ''' || :l_obDescription || ''' AS "GLAccount",
				NULL AS "TransSeq",
				NULL AS "CreateDate",
				NULL AS "DocDate",  
				NULL AS "DocLineNum",
				NULL AS "ItemCode",
				NULL AS "ItemName",		
				NULL AS "WhsCode",
				NULL AS "Qty",
				NULL AS "CalcPrice", 
				SUM(IFNULL(T0."TransValue",0.0)) AS  "TransValue",
				SUM(IFNULL(T0."TransValue",0.0)) AS "CumulativeValue",
				T0."InvntAct" AS "InvntAct", 
				NULL AS "EvalSystem", 
				NULL AS "Currency",
				NULL AS "Rate", 
				NULL AS "UserSign", 
				NULL AS "TransType",
				NULL AS "CreatedBy", 
				NULL AS "BASE_REF", 		 
				NULL AS "InvntryUom", 		
				NULL  AS "CostAct", 
				100 AS "LineFlag"';
			v_groupType1148 := ' GROUP BY T0."InvntAct"';
			v_groupType1153 := 'SELECT DISTINCT ''' || :l_obDescription || ''' AS "GLAccount",
				NULL AS "TransSeq",
				NULL AS "CreateDate", 
				NULL AS "DocDate",	
				NULL AS "DocLineNum",	
				NULL AS "ItemCode",
				NULL AS "ItemName",		
				NULL AS "WhsCode",
				NULL AS "Qty",
				NULL AS "CalcPrice", 
				0 AS  "TransValue",
				0 AS "CumulativeValue",
				T0."InvntAct" AS "InvntAct", 
				NULL AS "EvalSystem", 
				NULL AS "Currency",
				NULL AS "Rate", 
				NULL AS "UserSign", 
				NULL AS "TransType",
				NULL AS "CreatedBy", 
				NULL AS "BASE_REF",		 
				NULL AS "InvntryUom",		 
				NULL  AS "CostAct", 
				100 AS "LineFlag"';
			v_groupType1392 := 'B."InvntAct"';
			v_groupType1396 := 'A."InvntAct"';
			v_groupType1557 := '(1)';
			v_groupType1558 := '"InvntAct"';
			v_groupType1652 := '';
		elseif :v_groupType = 3 then
			if :v_localSetting = 'CN' THEN
			--cn
			v_groupType406 := ' WHEN 0 THEN 0  
				 ELSE CASE MAX(T1."EvalSystem") 
				 WHEN ''F'' 
				 THEN 0
				 ELSE CASE SUM(IFNULL(T0."TransValue",0.0))
					  WHEN 0
					  THEN 0
					  ELSE SUM(IFNULL(T0."TransValue",0.0))/(SUM(T0."InQty") - SUM(T0."OutQty")) 
					  END
				 END ';
			ELSE 
				v_groupType406 := ' WHEN 0 THEN 0 
				 ELSE CASE MAX(T1."EvalSystem") 
				 WHEN ''F'' 
				 THEN 0
				 ELSE CASE SUM(IFNULL(T0."TransValue",0.0))
					  WHEN 0
					  THEN 0
					  ELSE 0 
					  END
				 END ';
			end if;
			
			v_groupType99 :=  'insert into tmp_PrepareAuditReportDataT3 SELECT *' || :v_localSettingMoreColumn ||' FROM (SELECT T1."ItemCode" AS "ItemCode",
			T1."ItemName" AS "ItemName",
			T2."WhsCode" AS "WhsCode",
			T0."TransSeq" AS "TransSeq",	
			T0."CreateDate" AS "CreateDate", 
			T0."DocDate" AS "DocDate",	
			NULL AS "Document",
			T0."DocLineNum" + 1 AS "DocLineNum", 
			T0."InQty" - T0."OutQty" AS "Qty",
			T0."CalcPrice" AS "CalcPrice", 
			T0."TransValue" AS  "TransValue",
			0 AS  "CumulativeQty",
			NULL AS "CumulativeCost",
			0 AS "CumulativeValue",
			T0."InvntAct" AS "GLAccount", 
			T1."EvalSystem" AS "EvalSystem",
			NULL AS "ValuationMethod",
			NULL AS "CurrentItemCost",	
			T0."Currency" AS "Currency",
			T0."Rate" AS "Rate", 
			T0."UserSign" AS "UserSign", 
			T0."TransType" AS "TransType",
			T0."CreatedBy" AS "CreatedBy", 
			T0."BASE_REF" AS "BASE_REF", 	
			T1."InvntryUom" AS "InvntryUom", 
			T0."CostAct"  AS "CostAct", 
			300 AS "LineFlag"';
			v_groupType223 := ' OR T0."InQty" <> 0 OR T0."OutQty" <> 0 OR T0."TransType" = 162';
			v_groupType340 := ' SELECT DISTINCT T1."ItemCode" AS "ItemCode",
			MAX(T1."ItemName") AS "ItemName",
			NULL AS "WhsCode",
			NULL AS "TransSeq",	
			NULL AS "CreateDate", 
			NULL AS "DocDate", 
			NULL AS "Document",
			NULL AS "DocLineNum",
			NULL AS "Qty",
			NULL AS "CalcPrice", 
			NULL AS  "TransValue",
			SUM(T0."InQty") - SUM(T0."OutQty") AS  "CumulativeQty",
			CASE SUM(T0."InQty") - SUM(T0."OutQty")		
			' || :v_groupType406 || '
			END AS "CumulativeCost",
			SUM(IFNULL(T0."TransValue",0.0)) AS "CumulativeValue",
			NULL AS "GLAccount", 
			NULL AS "EvalSystem",
			MAX(T1."EvalSystem") AS "ValuationMethod",
			NULL AS "CurrentItemCost",
			NULL AS "Currency",
			NULL AS "Rate", 
			NULL AS "UserSign", 
			NULL AS "TransType",
			NULL AS "CreatedBy", 
			NULL AS "BASE_REF",
			NULL AS "InvntryUom",
			NULL  AS "CostAct", 
			1 AS "LineFlag"';
			v_groupType473 := ' OR T0."InQty" <> 0 OR T0."OutQty" <> 0 OR T0."TransType" = 162';
			v_groupType586 := ' GROUP BY T1."ItemCode"';
			v_groupType587 := ' UNION ALL ' || 'SELECT DISTINCT T1."ItemCode" AS "ItemCode",
			NULL AS "ItemName",
			T2."WhsCode" AS "WhsCode",
			NULL AS "TransSeq",	
			NULL AS "CreateDate", 
			NULL AS "DocDate", 
			NULL AS "Document",
			NULL AS "DocLineNum",
			NULL AS "Qty",
			NULL AS "CalcPrice", 
			NULL AS  "TransValue",
			SUM(T0."InQty") - SUM(T0."OutQty") AS  "CumulativeQty",
			CASE SUM(T0."InQty") - SUM(T0."OutQty")		
			' || :v_groupType406 || '
			END AS "CumulativeCost",
			SUM(IFNULL(T0."TransValue",0.0)) AS "CumulativeValue",
			NULL AS "GLAccount", 
			MAX(T1."EvalSystem") AS "EvalSystem",
			NULL AS "ValuationMethod",
			0 AS "CurrentItemCost",
			NULL AS "Currency",
			NULL AS "Rate", 
			NULL AS "UserSign", 
			NULL AS "TransType",
			NULL AS "CreatedBy", 
			NULL AS "BASE_REF",
			NULL AS "InvntryUom",
			NULL  AS "CostAct", 
			2 AS "LineFlag"';
			v_groupType787 := ' GROUP BY T1."ItemCode", T2."WhsCode"';
			v_groupType790 := ' UNION ALL SELECT DISTINCT B."ItemCode" AS "ItemCode",
			NULL AS "ItemName",
			B."WhsCode" AS "WhsCode",		
			NULL AS "TransSeq",
			NULL AS "CreateDate",
			NULL AS "DocDate", '''||
			:l_obDescription || ''' AS "Document",
			NULL AS "DocLineNum", 	 
			SUM(IFNULL(B."Qty",0.0)) AS "Qty",
			NULL AS "CalcPrice", 
			SUM(IFNULL(B. "TransValue",0.0)) AS  "TransValue",
			SUM(IFNULL(B. "CumulativeQty",0.0)) AS  "CumulativeQty",
			NULL AS "CumulativeCost",
			SUM(IFNULL(B."CumulativeValue",0.0)) AS "CumulativeValue",		
			NULL AS "GLAccount", 
			NULL AS "EvalSystem",
			NULL AS "ValuationMethod",
			NULL AS "CurrentItemCost",
			NULL AS "Currency",
			NULL AS "Rate", 
			NULL AS "UserSign", 
			NULL AS "TransType",
			NULL AS "CreatedBy", 
			NULL AS "BASE_REF", 		
			NULL AS "InvntryUom", 		
			NULL AS "CostAct", 
			100 AS "LineFlag"';
			v_groupType911 := 'SELECT DISTINCT T1."ItemCode" AS "ItemCode",
				NULL AS "ItemName",
				T2."WhsCode" AS "WhsCode",		
				NULL AS "TransSeq",
				NULL AS "CreateDate",
				NULL AS "DocDate", '''||
				:l_obDescription || ''' AS "Document",
				NULL AS "DocLineNum", 	 
				SUM(T0."InQty") - SUM(T0."OutQty") AS "Qty",
				NULL AS "CalcPrice", 
				SUM(IFNULL(T0."TransValue",0.0)) AS  "TransValue",
				SUM(T0."InQty") - SUM(T0."OutQty") AS  "CumulativeQty",
				NULL AS "CumulativeCost",
				SUM(IFNULL(T0."TransValue",0.0)) AS "CumulativeValue",		
				NULL AS "GLAccount", 
				NULL AS "EvalSystem",
				NULL AS "ValuationMethod",
				NULL AS "CurrentItemCost",
				NULL AS "Currency",
				NULL AS "Rate", 
				NULL AS "UserSign", 
				NULL AS "TransType",
				NULL AS "CreatedBy", 
				NULL AS "BASE_REF", 		
				NULL AS "InvntryUom", 		
				NULL AS "CostAct", 
				100 AS "LineFlag"';
			v_groupType1148 := ' GROUP BY T1."ItemCode",T2."WhsCode"';
			v_groupType1153 := 'SELECT DISTINCT T1."ItemCode" AS "ItemCode",
				NULL AS "ItemName",
				T2."WhsCode" AS "WhsCode",		
				NULL AS "TransSeq",
				NULL AS "CreateDate",
				NULL AS "DocDate", '''||
				:l_obDescription || ''' AS "Document",
				NULL AS "DocLineNum", 	 
				0 AS "Qty",
				NULL AS "CalcPrice", 
				0 AS  "TransValue",
				0 AS  "CumulativeQty",
				NULL AS "CumulativeCost",
				0 AS "CumulativeValue",		
				NULL AS "InvntAct", 
				NULL AS "EvalSystem",
				NULL AS "ValuationMethod",
				NULL AS "CurrentItemCost",
				NULL AS "Currency",
				NULL AS "Rate", 
				NULL AS "UserSign", 
				NULL AS "TransType",
				NULL AS "CreatedBy", 
				NULL AS "BASE_REF", 		
				NULL AS "InvntryUom", 		
				NULL AS "CostAct", 
				100 AS "LineFlag"';
			v_groupType1392 := 'B."ItemCode", B."WhsCode"';
			v_groupType1396 := 'A."ItemCode", A."WhsCode"';
			v_groupType1557 := '(1,2)';
			v_groupType1558 := '"ItemCode", "WhsCode"' ;
			v_groupType1652 := '"LineFlag" = ''2'' ';
		
		elseif :v_groupType = 4 then
			if :v_localSetting = 'CN' THEN
			--cn
				v_groupType406 := ' WHEN 0 THEN 0
				 ELSE CASE MAX(T1."EvalSystem") 
				 WHEN ''F'' 
				 THEN 0
				 ELSE CASE SUM(IFNULL(T0."TransValue",0.0))
					  WHEN 0
					  THEN 0
					  ELSE SUM(IFNULL(T0."TransValue",0.0))/(SUM(T0."InQty") - SUM(T0."OutQty")) 
					  END
				 END ';
			ELSE 
				v_groupType406 := ' WHEN 0 THEN 0
				 ELSE CASE MAX(T1."EvalSystem") 
				 WHEN ''F'' 
				 THEN 0
				 ELSE CASE SUM(IFNULL(T0."TransValue",0.0))
					  WHEN 0
					  THEN 0
					  ELSE 0 
					  END
				 END ';
			end if;
			
			v_groupType99 := 'insert into tmp_PrepareAuditReportDataT4 SELECT *' || :v_localSettingMoreColumn ||' FROM (SELECT T2."WhsCode" AS "WhsCode",
			T1."ItemCode" AS "ItemCode",
			T1."ItemName" AS "ItemName",	
			T0."TransSeq" AS "TransSeq",	
			T0."CreateDate" AS "CreateDate", 
			T0."DocDate" AS "DocDate",	
			NULL AS "Document",
			T0."DocLineNum" + 1 AS "DocLineNum", 
			T0."InQty" - T0."OutQty" AS "Qty",
			T0."CalcPrice" AS "CalcPrice", 
			T0."TransValue" AS  "TransValue",
			0 AS  "CumulativeQty",
			NULL AS "CumulativeCost",
			0 AS "CumulativeValue",
			T0."InvntAct" AS "GLAccount", 
			T1."EvalSystem" AS "EvalSystem",
			NULL AS "ValuationMethod",
			NULL AS "CurrentItemCost",	
			T0."Currency" AS "Currency",
			T0."Rate" AS "Rate", 
			T0."UserSign" AS "UserSign", 
			T0."TransType" AS "TransType",
			T0."CreatedBy" AS "CreatedBy", 
			T0."BASE_REF" AS "BASE_REF", 	
			T1."InvntryUom" AS "InvntryUom", 
			T0."CostAct"  AS "CostAct", 
			300 AS "LineFlag"';
			v_groupType223 := ' OR T0."InQty" <> 0 OR T0."OutQty" <> 0 OR T0."TransType" = 162';
			v_groupType340 := ' SELECT DISTINCT MAX(T2."WhsCode") AS "WhsCode",
			NULL AS "ItemCode",
			NULL AS "ItemName",    
			NULL AS "TransSeq",	
			NULL AS "CreateDate", 
			NULL AS "DocDate", 
			NULL AS "Document",
			NULL AS "DocLineNum",
			NULL AS "Qty",
			NULL AS "CalcPrice", 
			NULL AS  "TransValue",
			SUM(T0."InQty") - SUM(T0."OutQty") AS  "CumulativeQty",
			NULL AS "CumulativeCost",
			SUM(IFNULL(T0."TransValue",0.0)) AS "CumulativeValue",
			NULL AS "GLAccount", 
			NULL AS "EvalSystem",
			NULL AS "ValuationMethod",
			NULL AS "CurrentItemCost",
			NULL AS "Currency",
			NULL AS "Rate", 
			NULL AS "UserSign", 
			NULL AS "TransType",
			NULL AS "CreatedBy", 
			NULL AS "BASE_REF",
			NULL AS "InvntryUom",
			NULL  AS "CostAct", 
			1 AS "LineFlag"';
			v_groupType473 := ' OR T0."InQty" <> 0 OR T0."OutQty" <> 0 OR T0."TransType" = 162';
			v_groupType586 := ' GROUP BY T2."WhsCode"';
			v_groupType587 := ' UNION ALL ' || 'SELECT DISTINCT T2."WhsCode" AS "WhsCode",
			T1."ItemCode" AS "ItemCode",
			MAX(T1."ItemName") AS "ItemName",    
			NULL AS "TransSeq",	
			NULL AS "CreateDate", 
			NULL AS "DocDate", 
			NULL AS "Document",
			NULL AS "DocLineNum",
			NULL AS "Qty",
			NULL AS "CalcPrice", 
			NULL AS  "TransValue",
			SUM(T0."InQty") - SUM(T0."OutQty") AS  "CumulativeQty",
			CASE SUM(T0."InQty") - SUM(T0."OutQty")	
			' || :v_groupType406 || '
			END AS "CumulativeCost",
			SUM(IFNULL(T0."TransValue",0.0)) AS "CumulativeValue",
			NULL AS "GLAccount", 
			MAX(T1."EvalSystem") AS "EvalSystem",
			MAX(T1."EvalSystem") AS "ValuationMethod",
			0 AS "CurrentItemCost",
			NULL AS "Currency",
			NULL AS "Rate", 
			NULL AS "UserSign", 
			NULL AS "TransType",
			NULL AS "CreatedBy", 
			NULL AS "BASE_REF",
			NULL AS "InvntryUom",
			NULL  AS "CostAct", 
			2 AS "LineFlag"';
			v_groupType787 := ' GROUP BY T2."WhsCode", T1."ItemCode"';
			v_groupType790 := ' UNION ALL SELECT DISTINCT B."WhsCode" AS "WhsCode",
			B."ItemCode" AS "ItemCode",
			NULL AS "ItemName",			
			NULL AS "TransSeq",
			NULL AS "CreateDate",
			NULL AS "DocDate", '''||
			:l_obDescription || ''' AS "Document",
			NULL AS "DocLineNum", 	 
			SUM(IFNULL(B."Qty",0.0)) AS "Qty",
			NULL AS "CalcPrice", 
			SUM(IFNULL(B. "TransValue",0.0)) AS  "TransValue",
			SUM(IFNULL(B. "CumulativeQty",0.0)) AS  "CumulativeQty",
			NULL AS "CumulativeCost",
			SUM(IFNULL(B."CumulativeValue",0.0)) AS "CumulativeValue",		
			NULL AS "GLAccount", 
			NULL AS "EvalSystem",
			NULL AS "ValuationMethod",
			NULL AS "CurrentItemCost",
			NULL AS "Currency",
			NULL AS "Rate", 
			NULL AS "UserSign", 
			NULL AS "TransType",
			NULL AS "CreatedBy", 
			NULL AS "BASE_REF", 		
			NULL AS "InvntryUom", 		
			NULL AS "CostAct", 
			100 AS "LineFlag"';
			v_groupType911 := 'SELECT DISTINCT T2."WhsCode" AS "WhsCode",
				T1."ItemCode" AS "ItemCode",
				NULL AS "ItemName",				
				NULL AS "TransSeq",
				NULL AS "CreateDate",
				NULL AS "DocDate", '''||
				:l_obDescription || ''' AS "Document",
				NULL AS "DocLineNum", 	 
				SUM(T0."InQty") - SUM(T0."OutQty") AS "Qty",
				NULL AS "CalcPrice", 
				SUM(IFNULL(T0."TransValue",0.0)) AS  "TransValue",
				SUM(T0."InQty") - SUM(T0."OutQty") AS  "CumulativeQty",
				NULL AS "CumulativeCost",
				SUM(IFNULL(T0."TransValue",0.0)) AS "CumulativeValue",		
				NULL AS "GLAccount", 
				NULL AS "EvalSystem",
				NULL AS "ValuationMethod",
				NULL AS "CurrentItemCost",
				NULL AS "Currency",
				NULL AS "Rate", 
				NULL AS "UserSign", 
				NULL AS "TransType",
				NULL AS "CreatedBy", 
				NULL AS "BASE_REF", 		
				NULL AS "InvntryUom", 		
				NULL AS "CostAct", 
				100 AS "LineFlag"';
			v_groupType1148 := ' GROUP BY T1."ItemCode",T2."WhsCode"';
			v_groupType1153 := 'SELECT DISTINCT T2."WhsCode" AS "WhsCode",
				T1."ItemCode" AS "ItemCode",
				NULL AS "ItemName",				
				NULL AS "TransSeq",
				NULL AS "CreateDate",
				NULL AS "DocDate", '''||
				:l_obDescription || ''' AS "Document",
				NULL AS "DocLineNum", 	 
				0 AS "Qty",
				NULL AS "CalcPrice", 
				0 AS  "TransValue",
				0 AS  "CumulativeQty",
				NULL AS "CumulativeCost",
				0 AS "CumulativeValue",		
				NULL AS "InvntAct", 
				NULL AS "EvalSystem",
				NULL AS "ValuationMethod",
				NULL AS "CurrentItemCost",
				NULL AS "Currency",
				NULL AS "Rate", 
				NULL AS "UserSign", 
				NULL AS "TransType",
				NULL AS "CreatedBy", 
				NULL AS "BASE_REF", 		
				NULL AS "InvntryUom", 		
				NULL AS "CostAct", 
				100 AS "LineFlag"';
			v_groupType1392 := 'B."ItemCode", B."WhsCode"';
			v_groupType1396 := 'A."WhsCode", A."ItemCode"';
			v_groupType1557 := '(1,2)';
			v_groupType1558 := '"WhsCode", "ItemCode"' ;
			v_groupType1652 := '"LineFlag" = ''2'' ';
			
		end if;
	
	-----------------------------------------------------------------------------
	------------------------------------------------------------------------------
	------------------------------------------------------------------------------
	select count (*) into cnt from "PUBLIC"."M_TEMPORARY_TABLES" where TABLE_NAME = UPPER(:v_tempTableName) AND SCHEMA_NAME = CURRENT_SCHEMA;
	if :cnt > 0 then
		v_T_SQL := 'drop table ' || :v_tempTableName;
		EXEC (:v_T_SQL);
	end if;
	
	
    -- Insert statements for procedure here
   		delete from tmp_PrepareAuditReportData1;
		delete from tmp_PrepareAuditReportData2;
		delete from tmp_PrepareAuditReportData3;
		delete from tmp_PrepareAuditReportData4;
   		delete from tmp_PrepareAuditReportDataT1;
		delete from tmp_PrepareAuditReportDataT2;
		delete from tmp_PrepareAuditReportDataT3;
		delete from tmp_PrepareAuditReportDataT4;
		delete from tmp_cumulativevalue; 
	    	     				   
	v_T_SQL := :v_groupType99 || v_localSetting206;
	
	v_T_SQL := :v_T_SQL || ' FROM OINM T0
	INNER  JOIN OITM T1  ON  T1."ItemCode" = T0."ItemCode"
	INNER  JOIN OWHS T2  ON  T2."WhsCode" = T0."Warehouse"' || :v_localSetting219 || ' WHERE (T0."TransValue" <> 0 ' || :v_groupType223 || ') ';
	
	IF :v_dateFrom <> '' then
		v_T_SQL := :v_T_SQL || ' AND ' || :v_dateType_Name || ' >= TO_DATE(N''' || :v_dateFrom || ''', ''YYYYMMDD'')';
	end if;
	IF :v_dateTo <> '' then 
		v_T_SQL := :v_T_SQL || ' AND ' || :v_dateType_Name || ' <= TO_DATE(N''' || :v_dateTo || ''', ''YYYYMMDD'')';
	end if;
	IF :v_itemCodeFrom <> '' then
		v_T_SQL := :v_T_SQL || ' AND  T0."ItemCode" >= (N''' || :v_itemCodeFrom || ''')';
	end if;
	IF :v_itemCodeTo <> '' then
		v_T_SQL := :v_T_SQL || ' AND  T0."ItemCode" <= (N''' || :v_itemCodeTo || ''')';
	end if;
	IF :v_itemGroupCode <> '0' AND :v_itemGroupCode <> '' then 
		v_T_SQL := :v_T_SQL || ' AND  T1."ItmsGrpCod" = (N''' || :v_itemGroupCode || ''')';
	end if;
	
	IF :v_itemGroupProperties <> '' AND :v_itemgrouppropertieSuseGroups = 'Y'	then
		IF :v_itemgrouppropertieStoken = 'Y' then					
			IF :v_itemgrouppropertieSCLOSEDRange = 'Y' then
				v_i := 1;
				v_T_SQL := :v_T_SQL || ' AND (T1."QryGroup' || :v_i || '" = N''' || SUBSTRING(:v_itemGroupProperties, :v_i, 1) || '''';
				v_i := :v_i + 1;
				while :v_i <= 64 do
					v_T_SQL := :v_T_SQL || ' AND T1."QryGroup' || :v_i || '" = N''' || SUBSTRING(:v_itemGroupProperties, :v_i, 1) || '''';
					v_i := :v_i + 1;
				END WHILE;
				v_T_SQL := :v_T_SQL || ')';
			ELSE					
				v_i := LOCATE(:v_itemGroupProperties,'Y');
				IF :v_i>0 THEN
					v_T_SQL := :v_T_SQL || ' AND (T1."QryGroup' || :v_i || '" = N''Y''';
					v_i := :v_i + 1;
					while :v_i <= 64 do 
						IF SUBSTRING(:v_itemGroupProperties, :v_i, 1) = N'Y' then
							v_T_SQL := :v_T_SQL || ' AND T1."QryGroup' || :v_i || '" = N''Y''';
						END IF;
						 v_i := :v_i + 1;
					END while;
					v_T_SQL := :v_T_SQL || ')';
				END IF;
			END IF;	
			
		ELSE
			IF :v_itemgrouppropertieStoken = 'N' THEN
				IF :v_itemgrouppropertieSCLOSEDRange = 'Y'	then
					--print (LENGTH(v_itemGroupProperties) - LENGTH(REPLACE(v_itemGroupProperties,'Y','')))
					IF (LENGTH(:v_itemGroupProperties) - LENGTH(REPLACE(:v_itemGroupProperties,'Y',''))) <> 64 AND (LENGTH(:v_itemGroupProperties) - LENGTH(REPLACE(:v_itemGroupProperties,'Y',''))) <> 1 THEN
						v_i := LOCATE(:v_itemGroupProperties,'Y');
						IF :v_i>0 THEN
							v_T_SQL := :v_T_SQL || ' AND (T1."QryGroup' || :v_i || '" = N''Y''';
							v_i := :v_i + 1;
							while :v_i <= 64 DO
								IF SUBSTRING(:v_itemGroupProperties, :v_i, 1) = N'Y' THEN
									v_T_SQL := :v_T_SQL || ' OR T1."QryGroup' || :v_i || '" = N''Y''';
								END IF;
								v_i := :v_i + 1;
							END WHILE;
							v_T_SQL := :v_T_SQL || ')';
						END IF;
					ELSE
						v_i := 1;
						v_T_SQL := :v_T_SQL || ' AND (T1."QryGroup' || :v_i || '" = N''' || SUBSTRING(:v_itemGroupProperties, :v_i, 1) || '''';
						v_i := :v_i + 1;
						while :v_i <= 64 do
							v_T_SQL := :v_T_SQL || ' AND T1."QryGroup' || :v_i || '" = N''' || SUBSTRING(:v_itemGroupProperties, :v_i, 1) || '''';
							v_i := :v_i + 1;
						END while;
						v_T_SQL := :v_T_SQL || ')';
					END IF;
				ELSE							 
					v_i := LOCATE(:v_itemGroupProperties,'Y');
					IF :v_i>0 THEN
						v_T_SQL := :v_T_SQL || ' AND (T1."QryGroup' || :v_i || '" = N''Y''';
						v_i := :v_i + 1;
						while :v_i <= 64 do
							IF SUBSTRING(:v_itemGroupProperties, :v_i, 1) = N'Y' THEN
								v_T_SQL := :v_T_SQL || ' OR T1."QryGroup' || :v_i || '" = N''Y''';
							END IF;
							v_i := :v_i + 1;
						END WHILE;
						v_T_SQL := :v_T_SQL || ')';
					END IF;
				END IF;
			END IF;
		END IF;
	END IF;
	
	IF :v_invntActGroup <> '' THEN
		l_invntActGroup := REPLACE(:v_invntActGroup,'|',',');
		v_T_SQL := :v_T_SQL || ' AND T0."InvntAct" IN (' || :l_invntActGroup || ')';
	END IF;
	IF :v_warehouse <> '' then
		l_warehouse := REPLACE(:v_warehouse,'|',',');
		v_T_SQL := :v_T_SQL || ' AND T0."Warehouse" IN (' || :l_warehouse || ')';
	END IF;

--passed	
    v_T_SQL := :v_T_SQL || ' UNION ALL ' || :v_groupType340 || :v_localSetting456;
	v_T_SQL := :v_T_SQL || ' FROM OINM T0
	INNER  JOIN OITM T1  ON  T1."ItemCode" = T0."ItemCode"
	INNER  JOIN OWHS T2  ON  T2."WhsCode" = T0."Warehouse" ' || :v_localSetting469 || ' WHERE (T0."TransValue" <> 0 ' || :v_groupType473 || ') ';

	IF :v_dateFrom <> '' AND :v_forceOBDisplay = 'N' then
		v_T_SQL := :v_T_SQL || ' AND ' || :v_dateType_Name || ' >= TO_DATE(N''' || :v_dateFrom || ''', ''YYYYMMDD'')';
	end if;
	IF :v_dateTo <> '' then 
		v_T_SQL := :v_T_SQL || ' AND ' || :v_dateType_Name || ' <= TO_DATE(N''' || :v_dateTo || ''', ''YYYYMMDD'')';
	end if;
	IF :v_itemCodeFrom <> '' then
		v_T_SQL := :v_T_SQL || ' AND  T0."ItemCode" >= (N''' || :v_itemCodeFrom || ''')';
	end if;
	IF :v_itemCodeTo <> '' then
		v_T_SQL := :v_T_SQL || ' AND  T0."ItemCode" <= (N''' || :v_itemCodeTo || ''')';
	end if;
	IF :v_itemGroupCode <> '0' AND :v_itemGroupCode <> '' then 
		v_T_SQL := :v_T_SQL || ' AND  T1."ItmsGrpCod" = (N''' || :v_itemGroupCode || ''')';
	end if;
	
	IF :v_itemGroupProperties <> '' AND :v_itemgrouppropertieSuseGroups = 'Y'	then
		IF :v_itemgrouppropertieStoken = 'Y' then					
			IF :v_itemgrouppropertieSCLOSEDRange = 'Y' then
				v_i := 1;
				v_T_SQL := :v_T_SQL || ' AND (T1."QryGroup' || :v_i || '" = N''' || SUBSTRING(:v_itemGroupProperties, :v_i, 1) || '''';
				v_i := :v_i + 1;
				while :v_i <= 64 do
					v_T_SQL := :v_T_SQL || ' AND T1."QryGroup' || :v_i || '" = N''' || SUBSTRING(:v_itemGroupProperties, :v_i, 1) || '''';
					v_i := :v_i + 1;
				END WHILE;
				v_T_SQL := :v_T_SQL || ')';
			ELSE					
				v_i := LOCATE(:v_itemGroupProperties,'Y');
				IF :v_i>0 THEN
					v_T_SQL := :v_T_SQL || ' AND (T1."QryGroup' || :v_i || '" = N''Y''';
					v_i := :v_i + 1;
					while :v_i <= 64 do 
						IF SUBSTRING(:v_itemGroupProperties, :v_i, 1) = N'Y' then
							v_T_SQL := :v_T_SQL || ' AND T1."QryGroup' || :v_i || '" = N''Y''';
						END IF;
						 v_i := :v_i + 1;
					END while;
					v_T_SQL := :v_T_SQL || ')';
				END IF;
			END IF;	
			
		ELSE
			IF :v_itemgrouppropertieStoken = 'N' THEN
				IF :v_itemgrouppropertieSCLOSEDRange = 'Y'	then
					--print (LENGTH(v_itemGroupProperties) - LENGTH(REPLACE(v_itemGroupProperties,'Y','')))
					IF (LENGTH(:v_itemGroupProperties) - LENGTH(REPLACE(:v_itemGroupProperties,'Y',''))) <> 64 AND (LENGTH(:v_itemGroupProperties) - LENGTH(REPLACE(:v_itemGroupProperties,'Y',''))) <> 1 THEN
						v_i := LOCATE(:v_itemGroupProperties,'Y');
						IF :v_i>0 THEN
							v_T_SQL := :v_T_SQL || ' AND (T1."QryGroup' || :v_i || '" = N''Y''';
							v_i := :v_i + 1;
							while :v_i <= 64 DO
								IF SUBSTRING(:v_itemGroupProperties, :v_i, 1) = N'Y' THEN
									v_T_SQL := :v_T_SQL || ' OR T1."QryGroup' || :v_i || '" = N''Y''';
								END IF;
								v_i := :v_i + 1;
							END WHILE;
							v_T_SQL := :v_T_SQL || ')';
						END IF;
					ELSE
						v_i := 1;
						v_T_SQL := :v_T_SQL || ' AND (T1."QryGroup' || :v_i || '" = N''' || SUBSTRING(:v_itemGroupProperties, :v_i, 1) || '''';
						v_i := :v_i + 1;
						while :v_i <= 64 do
							v_T_SQL := :v_T_SQL || ' AND T1."QryGroup' || :v_i || '" = N''' || SUBSTRING(:v_itemGroupProperties, :v_i, 1) || '''';
							v_i := :v_i + 1;
						END while;
						v_T_SQL := :v_T_SQL || ')';
					END IF;
				ELSE							 
					v_i := LOCATE(:v_itemGroupProperties,'Y');
					IF :v_i>0 THEN
						v_T_SQL := :v_T_SQL || ' AND (T1."QryGroup' || :v_i || '" = N''Y''';
						v_i := :v_i + 1;
						while :v_i <= 64 do
							IF SUBSTRING(:v_itemGroupProperties, :v_i, 1) = N'Y' THEN
								v_T_SQL := :v_T_SQL || ' OR T1."QryGroup' || :v_i || '" = N''Y''';
							END IF;
							v_i := :v_i + 1;
						END WHILE;
						v_T_SQL := :v_T_SQL || ')';
					END IF;
				END IF;
			END IF;
		END IF;
	END IF;
	
	IF :v_invntActGroup <> '' THEN
		l_invntActGroup := REPLACE(:v_invntActGroup,'|',',');
		v_T_SQL := :v_T_SQL || ' AND T0."InvntAct" IN (' || :l_invntActGroup || ')';
	END IF;
	IF :v_warehouse <> '' then
		l_warehouse := REPLACE(:v_warehouse,'|',',');
		v_T_SQL := :v_T_SQL || ' AND T0."Warehouse" IN (' || :l_warehouse || ')';
	END IF; 
	--passed

	v_T_SQL := :v_T_SQL || :v_groupType586;
	
	IF :v_groupType = '3' OR :v_groupType = '4' then
		v_T_SQL := v_T_SQL || :v_groupType587 || :v_localSetting657;
		v_T_SQL := :v_T_SQL || ' FROM OINM T0 
				INNER  JOIN OITM T1  ON  T1."ItemCode" = T0."ItemCode"   
				INNER  JOIN OWHS T2  ON  T2."WhsCode" = T0."Warehouse" ' || :v_localSetting670 || 'WHERE (T0."TransValue" <> 0 OR T0."InQty" <> 0 OR T0."OutQty" <> 0 OR T0."TransType" = 162) ';	 	
	
		IF :v_dateFrom <> '' AND :v_forceOBDisplay = 'N' then 
			v_T_SQL := :v_T_SQL || ' AND ' || :v_dateType_Name || ' >= TO_DATE(N''' || :v_dateFrom || ''', ''YYYYMMDD'')';
		end if;
		IF :v_dateTo <> '' then 
			v_T_SQL := :v_T_SQL || ' AND ' || :v_dateType_Name || ' <= TO_DATE(N''' || :v_dateTo || ''', ''YYYYMMDD'')';
		end if;
		IF :v_itemCodeFrom <> '' then
			v_T_SQL := :v_T_SQL || ' AND  T0."ItemCode" >= (N''' || :v_itemCodeFrom || ''')';
		end if;
		IF :v_itemCodeTo <> '' then
			v_T_SQL := :v_T_SQL || ' AND  T0."ItemCode" <= (N''' || :v_itemCodeTo || ''')';
		end if;
		IF :v_itemGroupCode <> '0' AND :v_itemGroupCode <> '' then 
			v_T_SQL := :v_T_SQL || ' AND  T1."ItmsGrpCod" = (N''' || :v_itemGroupCode || ''')';
		end if;
		
		IF :v_itemGroupProperties <> '' AND :v_itemgrouppropertieSuseGroups = 'Y'	then
			IF :v_itemgrouppropertieStoken = 'Y' then					
				IF :v_itemgrouppropertieSCLOSEDRange = 'Y' then
					v_i := 1;
					v_T_SQL := :v_T_SQL || ' AND (T1."QryGroup' || :v_i || '" = N''' || SUBSTRING(:v_itemGroupProperties, :v_i, 1) || '''';
					v_i := :v_i + 1;
					while :v_i <= 64 do
						v_T_SQL := :v_T_SQL || ' AND T1."QryGroup' || :v_i || '" = N''' || SUBSTRING(:v_itemGroupProperties, :v_i, 1) || '''';
						v_i := :v_i + 1;
					END WHILE;
					v_T_SQL := :v_T_SQL || ')';
				ELSE					
					v_i := LOCATE(:v_itemGroupProperties,'Y');
					IF :v_i>0 THEN
						v_T_SQL := :v_T_SQL || ' AND (T1."QryGroup' || :v_i || '" = N''Y''';
						v_i := :v_i + 1;
						while :v_i <= 64 do 
							IF SUBSTRING(:v_itemGroupProperties, :v_i, 1) = N'Y' then
								v_T_SQL := :v_T_SQL || ' AND T1."QryGroup' || :v_i || '" = N''Y''';
							END IF;
							 v_i := :v_i + 1;
						END while;
						v_T_SQL := :v_T_SQL || ')';
					END IF;
				END IF;	
				
			ELSE
				IF :v_itemgrouppropertieStoken = 'N' THEN
					IF :v_itemgrouppropertieSCLOSEDRange = 'Y'	then
						--print (LENGTH(v_itemGroupProperties) - LENGTH(REPLACE(v_itemGroupProperties,'Y','')))
						IF (LENGTH(:v_itemGroupProperties) - LENGTH(REPLACE(:v_itemGroupProperties,'Y',''))) <> 64 AND (LENGTH(:v_itemGroupProperties) - LENGTH(REPLACE(:v_itemGroupProperties,'Y',''))) <> 1 THEN
							v_i := LOCATE(:v_itemGroupProperties,'Y');
							IF :v_i>0 THEN
								v_T_SQL := :v_T_SQL || ' AND (T1."QryGroup' || :v_i || '" = N''Y''';
								v_i := :v_i + 1;
								while :v_i <= 64 DO
									IF SUBSTRING(:v_itemGroupProperties, :v_i, 1) = N'Y' THEN
										v_T_SQL := :v_T_SQL || ' OR T1."QryGroup' || :v_i || '" = N''Y''';
									END IF;
									v_i := :v_i + 1;
								END WHILE;
								v_T_SQL := :v_T_SQL || ')';
							END IF;
						ELSE
							v_i := 1;
							v_T_SQL := :v_T_SQL || ' AND (T1."QryGroup' || :v_i || '" = N''' || SUBSTRING(:v_itemGroupProperties, :v_i, 1) || '''';
							v_i := :v_i + 1;
							while :v_i <= 64 do
								v_T_SQL := :v_T_SQL || ' AND T1."QryGroup' || :v_i || '" = N''' || SUBSTRING(:v_itemGroupProperties, :v_i, 1) || '''';
								v_i := :v_i + 1;
							END while;
							v_T_SQL := :v_T_SQL || ')';
						END IF;
					ELSE							 
						v_i := LOCATE(:v_itemGroupProperties,'Y');
						IF :v_i>0 THEN
							v_T_SQL := :v_T_SQL || ' AND (T1."QryGroup' || :v_i || '" = N''Y''';
							v_i := :v_i + 1;
							while :v_i <= 64 do
								IF SUBSTRING(:v_itemGroupProperties, :v_i, 1) = N'Y' THEN
									v_T_SQL := :v_T_SQL || ' OR T1."QryGroup' || :v_i || '" = N''Y''';
								END IF;
								v_i := :v_i + 1;
							END WHILE;
							v_T_SQL := :v_T_SQL || ')';
						END IF;
					END IF;
				END IF;
			END IF;
		END IF;
		
		IF :v_invntActGroup <> '' THEN
			l_invntActGroup := REPLACE(:v_invntActGroup,'|',',');
			v_T_SQL := :v_T_SQL || ' AND T0."InvntAct" IN (' || :l_invntActGroup || ')';
		END IF;
		IF :v_warehouse <> '' then
			l_warehouse := REPLACE(:v_warehouse,'|',',');
			v_T_SQL := :v_T_SQL || ' AND T0."Warehouse" IN (' || :l_warehouse || ')';
		END IF;
		
--passed
		
		v_T_SQL := :v_T_SQL || :v_groupType787;
	end if; --3,4

	v_T_SQL := :v_T_SQL || v_groupType790 || :v_localSetting898;
	
	v_T_SQL := :v_T_SQL || ' FROM (';
	IF :v_dateFrom <> '' then
		
		v_T_SQL := :v_T_SQL || :v_groupType911 || :v_localSetting1019;
		IF :v_groupType = 2 then
			v_T_SQL := :v_T_SQL || ' FROM OINM T0
			INNER  JOIN OITM T1  ON  T1."ItemCode" = T0."ItemCode"   
			INNER  JOIN OWHS T2  ON  T2."WhsCode" = T0."Warehouse"
			WHERE (T0."TransValue" <> 0)';
		else 
			v_T_SQL := :v_T_SQL || ' FROM OINM T0
			INNER  JOIN OITM T1  ON  T1."ItemCode" = T0."ItemCode"   
			INNER  JOIN OWHS T2  ON  T2."WhsCode" = T0."Warehouse"
			WHERE (T0."TransValue" <> 0 OR T0."InQty" <> 0 OR T0."OutQty" <> 0)';
		end if;
		
		v_T_SQL := :v_T_SQL || ' AND ' || :v_dateType_Name || ' < TO_DATE(N''' || :v_dateFrom || ''', ''YYYYMMDD'')';
	
		IF :v_itemCodeFrom <> '' then
			v_T_SQL := :v_T_SQL || ' AND  T0."ItemCode" >= (N''' || :v_itemCodeFrom || ''')';
		end if;
		IF :v_itemCodeTo <> '' then
			v_T_SQL := :v_T_SQL || ' AND  T0."ItemCode" <= (N''' || :v_itemCodeTo || ''')';
		end if;
				
		IF :v_forceOBDisplay = 'Y' OR :v_groupType = 2 then 
		
			IF :v_itemGroupCode <> '0' AND :v_itemGroupCode <> '' then 
				v_T_SQL := :v_T_SQL || ' AND  T1."ItmsGrpCod" = (N''' || :v_itemGroupCode || ''')';
			end if;			
			
			IF :v_itemGroupProperties <> '' AND :v_itemgrouppropertieSuseGroups = 'Y'	then
				IF :v_itemgrouppropertieStoken = 'Y' then					
					IF :v_itemgrouppropertieSCLOSEDRange = 'Y' then
						v_i := 1;
						v_T_SQL := :v_T_SQL || ' AND (T1."QryGroup' || :v_i || '" = N''' || SUBSTRING(:v_itemGroupProperties, :v_i, 1) || '''';
						v_i := :v_i + 1;
						while :v_i <= 64 do
							v_T_SQL := :v_T_SQL || ' AND T1."QryGroup' || :v_i || '" = N''' || SUBSTRING(:v_itemGroupProperties, :v_i, 1) || '''';
							v_i := :v_i + 1;
						END WHILE;
						v_T_SQL := :v_T_SQL || ')';
					ELSE					
						v_i := LOCATE(:v_itemGroupProperties,'Y');
						IF :v_i>0 THEN
							v_T_SQL := :v_T_SQL || ' AND (T1."QryGroup' || :v_i || '" = N''Y''';
							v_i := :v_i + 1;
							while :v_i <= 64 do 
								IF SUBSTRING(:v_itemGroupProperties, :v_i, 1) = N'Y' then
									v_T_SQL := :v_T_SQL || ' AND T1."QryGroup' || :v_i || '" = N''Y''';
								END IF;
								 v_i := :v_i + 1;
							END while;
							v_T_SQL := :v_T_SQL || ')';
						END IF;
					END IF;	
					
				ELSE
					IF :v_itemgrouppropertieStoken = 'N' THEN
						IF :v_itemgrouppropertieSCLOSEDRange = 'Y'	then
							--print (LENGTH(v_itemGroupProperties) - LENGTH(REPLACE(v_itemGroupProperties,'Y','')))
							IF (LENGTH(:v_itemGroupProperties) - LENGTH(REPLACE(:v_itemGroupProperties,'Y',''))) <> 64 AND (LENGTH(:v_itemGroupProperties) - LENGTH(REPLACE(:v_itemGroupProperties,'Y',''))) <> 1 THEN
								v_i := LOCATE(:v_itemGroupProperties,'Y');
								IF :v_i>0 THEN
									v_T_SQL := :v_T_SQL || ' AND (T1."QryGroup' || :v_i || '" = N''Y''';
									v_i := :v_i + 1;
									while :v_i <= 64 DO
										IF SUBSTRING(:v_itemGroupProperties, :v_i, 1) = N'Y' THEN
											v_T_SQL := :v_T_SQL || ' OR T1."QryGroup' || :v_i || '" = N''Y''';
										END IF;
										v_i := :v_i + 1;
									END WHILE;
									v_T_SQL := :v_T_SQL || ')';
								END IF;
							ELSE
								v_i := 1;
								v_T_SQL := :v_T_SQL || ' AND (T1."QryGroup' || :v_i || '" = N''' || SUBSTRING(:v_itemGroupProperties, :v_i, 1) || '''';
								v_i := :v_i + 1;
								while :v_i <= 64 do
									v_T_SQL := :v_T_SQL || ' AND T1."QryGroup' || :v_i || '" = N''' || SUBSTRING(:v_itemGroupProperties, :v_i, 1) || '''';
									v_i := :v_i + 1;
								END while;
								v_T_SQL := :v_T_SQL || ')';
							END IF;
						ELSE							 
							v_i := LOCATE(:v_itemGroupProperties,'Y');
							IF :v_i>0 THEN
								v_T_SQL := :v_T_SQL || ' AND (T1."QryGroup' || :v_i || '" = N''Y''';
								v_i := :v_i + 1;
								while :v_i <= 64 do
									IF SUBSTRING(:v_itemGroupProperties, :v_i, 1) = N'Y' THEN
										v_T_SQL := :v_T_SQL || ' OR T1."QryGroup' || :v_i || '" = N''Y''';
									END IF;
									v_i := :v_i + 1;
								END WHILE;
								v_T_SQL := :v_T_SQL || ')';
							END IF;
						END IF;
					END IF;
				END IF;
			END IF;
		END IF; --:v_forceOBDisplay = 'Y' OR :v_groupType = 2 

		IF :v_invntActGroup <> '' THEN
			l_invntActGroup := REPLACE(:v_invntActGroup,'|',',');
			v_T_SQL := :v_T_SQL || ' AND T0."InvntAct" IN (' || :l_invntActGroup || ')';
		END IF;
		IF :v_warehouse <> '' then
			l_warehouse := REPLACE(:v_warehouse,'|',',');
			v_T_SQL := :v_T_SQL || ' AND T0."Warehouse" IN (' || :l_warehouse || ')';
		END IF;
		
		v_T_SQL := :v_T_SQL || :v_groupType1148 || ' UNION ALL ';
	END if ; --:v_dateFrom <> ''

--passed
	
	v_T_SQL := :v_T_SQL || :v_groupType1153 || :v_localSetting1261;
	IF :v_groupType = 2 then	
		v_T_SQL := :v_T_SQL || ' FROM OINM T0
		INNER  JOIN OITM T1  ON  T1."ItemCode" = T0."ItemCode"
		INNER  JOIN OWHS T2  ON  T2."WhsCode" = T0."Warehouse" ' || :v_localSetting1274 ||
		'WHERE (T0."TransValue" <> 0) ';
	else
		v_T_SQL := :v_T_SQL || ' FROM OINM T0
		INNER  JOIN OITM T1  ON  T1."ItemCode" = T0."ItemCode"
		INNER  JOIN OWHS T2  ON  T2."WhsCode" = T0."Warehouse" ' || :v_localSetting1274 ||
		'WHERE (T0."TransValue" <> 0 OR T0."InQty" <> 0 OR T0."OutQty" <> 0 OR T0."TransType" = 162) ';
	end if;
	
	IF :v_dateFrom <> '' AND :v_forceOBDisplay = 'N' then 
		v_T_SQL := :v_T_SQL || ' AND ' || :v_dateType_Name || ' >= TO_DATE(N''' || :v_dateFrom || ''', ''YYYYMMDD'')';
	end if;
	IF :v_dateTo <> '' then 
		v_T_SQL := :v_T_SQL || ' AND ' || :v_dateType_Name || ' <= TO_DATE(N''' || :v_dateTo || ''', ''YYYYMMDD'')';
	end if;
	IF :v_itemCodeFrom <> '' then
		v_T_SQL := :v_T_SQL || ' AND  T0."ItemCode" >= (N''' || :v_itemCodeFrom || ''')';
	end if;
	IF :v_itemCodeTo <> '' then
		v_T_SQL := :v_T_SQL || ' AND  T0."ItemCode" <= (N''' || :v_itemCodeTo || ''')';
	end if;
	IF :v_itemGroupCode <> '0' AND :v_itemGroupCode <> '' then 
		v_T_SQL := :v_T_SQL || ' AND  T1."ItmsGrpCod" = (N''' || :v_itemGroupCode || ''')';
	end if;
	
	IF :v_itemGroupProperties <> '' AND :v_itemgrouppropertieSuseGroups = 'Y'	then
		IF :v_itemgrouppropertieStoken = 'Y' then					
			IF :v_itemgrouppropertieSCLOSEDRange = 'Y' then
				v_i := 1;
				v_T_SQL := :v_T_SQL || ' AND (T1."QryGroup' || :v_i || '" = N''' || SUBSTRING(:v_itemGroupProperties, :v_i, 1) || '''';
				v_i := :v_i + 1;
				while :v_i <= 64 do
					v_T_SQL := :v_T_SQL || ' AND T1."QryGroup' || :v_i || '" = N''' || SUBSTRING(:v_itemGroupProperties, :v_i, 1) || '''';
					v_i := :v_i + 1;
				END WHILE;
				v_T_SQL := :v_T_SQL || ')';
			ELSE					
				v_i := LOCATE(:v_itemGroupProperties,'Y');
				IF :v_i>0 THEN
					v_T_SQL := :v_T_SQL || ' AND (T1."QryGroup' || :v_i || '" = N''Y''';
					v_i := :v_i + 1;
					while :v_i <= 64 do 
						IF SUBSTRING(:v_itemGroupProperties, :v_i, 1) = N'Y' then
							v_T_SQL := :v_T_SQL || ' AND T1."QryGroup' || :v_i || '" = N''Y''';
						END IF;
						 v_i := :v_i + 1;
					END while;
					v_T_SQL := :v_T_SQL || ')';
				END IF;
			END IF;	
			
		ELSE
			IF :v_itemgrouppropertieStoken = 'N' THEN
				IF :v_itemgrouppropertieSCLOSEDRange = 'Y'	then
					--print (LENGTH(v_itemGroupProperties) - LENGTH(REPLACE(v_itemGroupProperties,'Y','')))
					IF (LENGTH(:v_itemGroupProperties) - LENGTH(REPLACE(:v_itemGroupProperties,'Y',''))) <> 64 AND (LENGTH(:v_itemGroupProperties) - LENGTH(REPLACE(:v_itemGroupProperties,'Y',''))) <> 1 THEN
						v_i := LOCATE(:v_itemGroupProperties,'Y');
						IF :v_i>0 THEN
							v_T_SQL := :v_T_SQL || ' AND (T1."QryGroup' || :v_i || '" = N''Y''';
							v_i := :v_i + 1;
							while :v_i <= 64 DO
								IF SUBSTRING(:v_itemGroupProperties, :v_i, 1) = N'Y' THEN
									v_T_SQL := :v_T_SQL || ' OR T1."QryGroup' || :v_i || '" = N''Y''';
								END IF;
								v_i := :v_i + 1;
							END WHILE;
							v_T_SQL := :v_T_SQL || ')';
						END IF;
					ELSE
						v_i := 1;
						v_T_SQL := :v_T_SQL || ' AND (T1."QryGroup' || :v_i || '" = N''' || SUBSTRING(:v_itemGroupProperties, :v_i, 1) || '''';
						v_i := :v_i + 1;
						while :v_i <= 64 do
							v_T_SQL := :v_T_SQL || ' AND T1."QryGroup' || :v_i || '" = N''' || SUBSTRING(:v_itemGroupProperties, :v_i, 1) || '''';
							v_i := :v_i + 1;
						END while;
						v_T_SQL := :v_T_SQL || ')';
					END IF;
				ELSE							 
					v_i := LOCATE(:v_itemGroupProperties,'Y');
					IF :v_i>0 THEN
						v_T_SQL := :v_T_SQL || ' AND (T1."QryGroup' || :v_i || '" = N''Y''';
						v_i := :v_i + 1;
						while :v_i <= 64 do
							IF SUBSTRING(:v_itemGroupProperties, :v_i, 1) = N'Y' THEN
								v_T_SQL := :v_T_SQL || ' OR T1."QryGroup' || :v_i || '" = N''Y''';
							END IF;
							v_i := :v_i + 1;
						END WHILE;
						v_T_SQL := :v_T_SQL || ')';
					END IF;
				END IF;
			END IF;
		END IF;
	END IF;
	
	IF :v_invntActGroup <> '' THEN
		l_invntActGroup := REPLACE(:v_invntActGroup,'|',',');
		v_T_SQL := :v_T_SQL || ' AND T0."InvntAct" IN (' || :l_invntActGroup || ')';
	END IF;
	IF :v_warehouse <> '' then
		l_warehouse := REPLACE(:v_warehouse,'|',',');
		v_T_SQL := :v_T_SQL || ' AND T0."Warehouse" IN (' || :l_warehouse || ')';
	END IF;
	
	v_dateType_Name := '';
	IF :v_dateType = 0 then
		v_dateType_Name := ', A."DocDate"';
	elseif :v_dateType = 1 then
		if :v_localSetting = 'CN' then
			v_dateType_Name := ', A."CreateDate"';
		else
			v_dateType_Name := '';
		end if;
	else 
		v_dateType_Name := '';
	end if;
	v_T_SQL := v_T_SQL || ') B GROUP BY ' || :v_groupType1392 || ') A ORDER BY ' || :v_groupType1396 || ', A."LineFlag"' || :v_dateType_Name || ',A."TransSeq"';

	
	--test
	--select :v_T_SQL from dummy;
	exec (:v_T_SQL);
	
	--------------------------------------------------------
	--------------------------------------------------------
	IF :v_hideItemCumulativeQty0 = 'Y' then
		IF :v_groupType = 1 then
			delete from tmp_PrepareAuditReportDataT1 where "ItemCode" in (select "ItemCode" from tmp_PrepareAuditReportDataT1 where "CumulativeQty" = 0 and "LineFlag" = 1);
		end if;
		
		IF :v_groupType = 3 then
			delete from tmp_PrepareAuditReportDataT3 as a where exists (select 1 from (select "ItemCode", "WhsCode" from tmp_PrepareAuditReportDataT3
			where "CumulativeQty" = 0 and "LineFlag" = 2) b where a."ItemCode" = b."ItemCode" and a."WhsCode" = b."WhsCode");
		end if;
		
		IF :v_groupType = 4 then
			delete from tmp_PrepareAuditReportDataT4 a where exists (select 1 from (select "ItemCode", "WhsCode" from tmp_PrepareAuditReportDataT4 
			where "CumulativeQty" = 0 and "LineFlag" = 2) b where a."ItemCode" = b."ItemCode" and a."WhsCode" = b."WhsCode");
		end if;
	end if;
	
	IF :v_groupType = 3 then
		delete from tmp_PrepareAuditReportDataT3 WHERE "ItemCode" NOT IN 
		(SELECT DISTINCT "ItemCode" FROM tmp_PrepareAuditReportDataT3 WHERE "LineFlag" = 300)
		AND "CumulativeQty" = 0 AND "CumulativeValue" = 0;
		
		delete from tmp_PrepareAuditReportDataT3 AS A WHERE EXISTS (SELECT 1 FROM (SELECT "ItemCode" as "ItemCode","WhsCode" as "WhsCode" FROM tmp_PrepareAuditReportDataT3 
		where "WhsCode" is not null GROUP BY "ItemCode", "WhsCode" HAVING COUNT(DISTINCT "LineFlag") = 2) B WHERE A."ItemCode" = B."ItemCode" AND A."WhsCode" = B."WhsCode")
		AND "CumulativeQty" = 0 AND "CumulativeValue" = 0;
	end if;
	
	IF v_groupType = 2 then
		DELETE FROM tmp_PrepareAuditReportDataT2 WHERE "InvntAct" NOT IN (SELECT DISTINCT "InvntAct" FROM tmp_PrepareAuditReportDataT2 WHERE "LineFlag" = 300)
		AND "CumulativeValue" = 0;
	end if;
	
	IF v_groupType = 4 then
		DELETE FROM tmp_PrepareAuditReportDataT4 WHERE ("WhsCode" NOT IN 
		(SELECT DISTINCT "WhsCode" FROM tmp_PrepareAuditReportDataT4 WHERE "LineFlag" = 300)
		OR "ItemCode" NOT IN (SELECT DISTINCT "ItemCode" FROM tmp_PrepareAuditReportDataT4 WHERE "LineFlag" = 300))
		AND "CumulativeQty" = 0 AND "CumulativeValue" = 0;
	end if;
	
	IF v_groupType = 1 then 
		DELETE FROM tmp_PrepareAuditReportDataT1 WHERE "ItemCode" NOT IN (SELECT DISTINCT "ItemCode" FROM tmp_PrepareAuditReportDataT1 WHERE "LineFlag" = 300)
		AND "CumulativeQty" = 0 AND "CumulativeValue" = 0;
	END IF;	

	IF :v_forceOBDisplay = 'N' then 
		IF :v_groupType = 3 then 
			delete from tmp_PrepareAuditReportDataT3 WHERE "ItemCode" NOT IN 
			(SELECT DISTINCT "ItemCode" FROM tmp_PrepareAuditReportDataT3 WHERE "LineFlag" = 300);
		end if;
		
		IF v_groupType = 2 then 
			DELETE FROM tmp_PrepareAuditReportDataT2 WHERE "InvntAct" NOT IN (SELECT DISTINCT "InvntAct" FROM tmp_PrepareAuditReportDataT2 WHERE "LineFlag" = 300);
		END IF;
		
		IF v_groupType = 4 then 
			DELETE FROM tmp_PrepareAuditReportDataT4 WHERE "WhsCode" NOT IN 
			(SELECT DISTINCT "WhsCode" FROM tmp_PrepareAuditReportDataT4 WHERE "LineFlag" = 300)
			OR "ItemCode" NOT IN (SELECT DISTINCT "ItemCode" FROM tmp_PrepareAuditReportDataT4 WHERE "LineFlag" = 300);
		END IF;
		
		IF v_groupType = 1 then 
			DELETE FROM tmp_PrepareAuditReportDataT1 WHERE "ItemCode" NOT IN (SELECT DISTINCT "ItemCode" FROM tmp_PrepareAuditReportDataT1 WHERE "LineFlag" = 300);
		END IF;
	END IF;
	
	IF v_groupType = 4 then
			UPDATE tmp_PrepareAuditReportDataT4 T0 SET "CumulativeQty" = (SELECT SUM("CumulativeQty") FROM tmp_PrepareAuditReportDataT4 T1 WHERE T1."LineFlag" = 2 AND T1."WhsCode" = T0."WhsCode" GROUP BY T1."WhsCode" )
			WHERE T0."LineFlag" = 1;
			UPDATE tmp_PrepareAuditReportDataT4 T0 SET "CumulativeValue" = (SELECT SUM("CumulativeValue") FROM tmp_PrepareAuditReportDataT4 T1 WHERE T1."LineFlag" = 2 AND T1."WhsCode" = T0."WhsCode" GROUP BY T1."WhsCode" )
			WHERE T0."LineFlag" = 1;
	END IF;
	
	v_dateType_Name := '';
	IF :v_dateType = 0 then
		v_dateType_Name := ', "DocDate"';
	elseif :v_dateType = 1 then
		if :v_localSetting = 'CN' then
			v_dateType_Name := ', "CreateDate"';
		else
			v_dateType_Name := '';
		end if;
	else 
		v_dateType_Name := '';
	end if;

	v_T_SQL := 'insert into tmp_PrepareAuditReportData' || 
				:v_groupType || ' SELECT  row_number() over(ORDER BY ' || :v_groupType1558 || ', "LineFlag"' || :v_dateType_Name || ',"TransSeq") as "PaginationId", * FROM tmp_PrepareAuditReportDataT' || :v_groupType;
	exec (:v_T_SQL);
	
	v_T_SQL := 'CREATE LOCAL TEMPORARY ROW TABLE ' || :v_tempTableName  || ' as (select * from tmp_PrepareAuditReportData' || :v_groupType || ')';
	exec (:v_T_SQL);

	IF v_groupType = 2 then 
		FOR cur_row2 as c_cursor2 DO 
			IF cur_row2."LineFlag" = 1 then 
				v_CumulativeValue := 0;
			ELSE	
				v_CumulativeValue := :v_CumulativeValue + cur_row2."TransValue";
				IF cur_row2."LineFlag" = 300 THEN				
					insert into tmp_cumulativevalue values (cur_row2."PaginationId",:v_CumulativeValue,0,0);  
				END IF;
				
			END IF;
		END FOR;
		
		v_T_SQL := 'update t1 set t1."CumulativeValue" = t2."CumulativeValue" '
				|| ' from ' || :v_tempTableName || ' t1 ,tmp_cumulativevalue t2 where t1."PaginationId" = t2."PaginationId"';
		exec (:v_T_SQL);
		
		IF :v_forceOBDisplay = 'N' THEN 
			v_T_SQL := 'update t1 set t1."CumulativeValue" = t1."CumulativeValue" + t2."CumulativeValue"'
					|| ' from ' || :v_tempTableName || ' t1 , '  || :v_tempTableName || ' t2'
					|| ' where t1."LineFlag" = 1 and t2."LineFlag" = 100 and t1."PaginationId" = t2."PaginationId" - 1';
			exec (:v_T_SQL);
		END IF;
	end if;
	
	if :v_groupType = 1 then
		FOR cur_row1 as c_cursor1 DO 
			IF cur_row1."LineFlag" = 1 or cur_row1."LineFlag" = 2 then 
				v_CumulativeValue := 0;
				v_CumulativeQty := 0;
			ELSE	
				v_CumulativeValue := :v_CumulativeValue + cur_row1."TransValue";
				v_CumulativeQty := :v_CumulativeQty + cur_row1."Qty";
				IF IFNULL(:v_CumulativeQty,0.0) <> 0 and cur_row1."EvalSystem" <> 'F' and v_CumulativeValue <> 0 and v_localSetting = 'CN' then
					v_CumulativeCost := ROUND(IFNULL(:v_CumulativeValue,0.0)/:v_CumulativeQty,6);
				ELSE 
					v_CumulativeCost := NULL;
				end if;
				
				IF cur_row1."LineFlag" = 300 THEN				
					insert into tmp_cumulativevalue values (cur_row1."PaginationId",:v_CumulativeValue,:v_CumulativeQty,:v_CumulativeCost);
				END IF;
			END IF;
		END FOR;
		
		IF :v_forceOBDisplay = 'N' THEN 
			v_T_SQL := 'update t1 set t1."CumulativeValue" = t2."CumulativeValue", t1."CumulativeQty" = t2."CumulativeQty", t1."CumulativeCost" = t2."CumulativeCost"'
					|| ' from ' || :v_tempTableName || ' t1 ,tmp_cumulativevalue t2 where t1."PaginationId" = t2."PaginationId"';
			exec (:v_T_SQL);
		
			v_T_SQL := 'update t1 set t1."CumulativeValue" = t1."CumulativeValue" + t2."CumulativeValue", t1."CumulativeQty" = t1."CumulativeQty"  + t2."CumulativeQty"'
					|| ' from ' || :v_tempTableName || ' t1 ,'  || :v_tempTableName || ' t2'
					|| ' where t1."LineFlag" = 1 and t2."LineFlag" = 100 and t1."PaginationId" = t2."PaginationId" - 1';
			exec (:v_T_SQL);
		END IF;
	end if;
	
	if :v_groupType = 3 then
		
		FOR cur_row3 as c_cursor3 DO 
			IF cur_row3."LineFlag" = 1 or cur_row3."LineFlag" = 2 then 
				v_CumulativeValue := 0;
				v_CumulativeQty := 0;
			ELSE
				v_CumulativeValue := :v_CumulativeValue + cur_row3."TransValue";
				v_CumulativeQty := :v_CumulativeQty + cur_row3."Qty";
				IF IFNULL(:v_CumulativeQty,0.0) <> 0 and cur_row3."EvalSystem" <> 'F' and v_CumulativeValue <> 0 and v_localSetting = 'CN' then
					v_CumulativeCost := ROUND(IFNULL(:v_CumulativeValue,0.0)/:v_CumulativeQty,6);
				ELSE 
					v_CumulativeCost := NULL;
				end if;
				
				IF cur_row3."LineFlag" = 300 THEN				
					insert into tmp_cumulativevalue values (cur_row3."PaginationId",:v_CumulativeValue,:v_CumulativeQty,:v_CumulativeCost);  
				END IF;
				
			END IF;
		END FOR;
		
		v_T_SQL := 'update t1 set t1."CumulativeValue" = t2."CumulativeValue", t1."CumulativeQty" = t2."CumulativeQty", t1."CumulativeCost" = t2."CumulativeCost"'
				|| ' from ' || :v_tempTableName || ' t1 ,tmp_cumulativevalue t2 where t1."PaginationId" = t2."PaginationId"';
		exec (:v_T_SQL);
		
		IF :v_forceOBDisplay = 'N' THEN 
			v_T_SQL := 'update t1 set t1."CumulativeValue" = t1."CumulativeValue" + t2."CumulativeValue", t1."CumulativeQty" = t1."CumulativeQty"  + t2."CumulativeQty"'
					|| ' from ' || :v_tempTableName || ' t1 ,'  || :v_tempTableName || ' t2'
					|| ' where t1."LineFlag" = 2 and t2."LineFlag" = 100 and t1."PaginationId" = t2."PaginationId" - 1';
			exec (:v_T_SQL);
		
			v_T_SQL := 'update ' || :v_tempTableName || ' t1 '
					|| ' set t1."CumulativeValue" = (Select Sum(t2."CumulativeValue") from ' || :v_tempTableName || ' t2 where t2."LineFlag" = 2 and t1."ItemCode" = t2."ItemCode"), '
					|| ' t1."CumulativeQty" = (Select Sum(t3."CumulativeQty") from ' || :v_tempTableName || ' t3 where t3."LineFlag" = 2 and t1."ItemCode" = t3."ItemCode") '
					|| ' where t1."LineFlag" = 1';
			exec (:v_T_SQL);
		END IF;
	end if;
	
	if :v_groupType = 4 then
		FOR cur_row4 as c_cursor4 DO 
			IF cur_row4."LineFlag" = 1 or cur_row4."LineFlag" = 2 then 
				v_CumulativeValue := 0;
				v_CumulativeQty := 0;
			ELSE
				v_CumulativeValue := :v_CumulativeValue + cur_row4."TransValue";
				v_CumulativeQty := :v_CumulativeQty + cur_row4."Qty";
				IF IFNULL(:v_CumulativeQty,0.0) <> 0 and cur_row4."EvalSystem" <> 'F' and v_CumulativeValue <> 0 and v_localSetting = 'CN' then
					v_CumulativeCost := ROUND(IFNULL(:v_CumulativeValue,0.0)/:v_CumulativeQty,6);
				ELSE 
					v_CumulativeCost := NULL;
				end if;
				
				IF cur_row4."LineFlag" = 300 THEN				
					insert into tmp_cumulativevalue values (cur_row4."PaginationId",:v_CumulativeValue,:v_CumulativeQty,:v_CumulativeCost);  
				END IF;
				
			END IF;
		END FOR;
	
		v_T_SQL := 'update t1 set t1."CumulativeValue" = t2."CumulativeValue", t1."CumulativeQty" = t2."CumulativeQty", t1."CumulativeCost" = t2."CumulativeCost"'
				|| ' from ' || :v_tempTableName || ' t1 ,tmp_cumulativevalue t2 where t1."PaginationId" = t2."PaginationId"';
		exec (:v_T_SQL);
		
		IF :v_forceOBDisplay = 'N' THEN 
			v_T_SQL := 'update t1 set t1."CumulativeValue" = t1."CumulativeValue" + t2."CumulativeValue", t1."CumulativeQty" = t1."CumulativeQty"  + t2."CumulativeQty"'
					|| ' from ' || :v_tempTableName || ' t1 ,'  || :v_tempTableName || ' t2 '
					|| ' where t1."LineFlag" = 2 and t2."LineFlag" = 100 and t1."PaginationId" = t2."PaginationId" - 1';
			exec (:v_T_SQL);
		
			v_T_SQL := 'update ' || :v_tempTableName || ' t1 '
					|| ' set t1."CumulativeValue" = (Select Sum(t2."CumulativeValue") from ' || :v_tempTableName || ' t2 where t2."LineFlag" = 2 and t1."WhsCode" = t2."WhsCode"), '
					|| ' t1."CumulativeQty" = (Select Sum(t3."CumulativeQty") from ' || :v_tempTableName || ' t3 where t3."LineFlag" = 2 and t1."WhsCode" = t3."WhsCode") '
					|| ' where t1."LineFlag" = 1';
			exec (:v_T_SQL);
		END IF;		
	end if;
	
	--CurrentItemCost
	IF :v_groupType = 1 OR :v_groupType = '3' OR :v_groupType = '4' then
		SELECT "PriceSys" into v_priceSys FROM OADM;
		
		IF :v_priceSys = 'Y' then
			
			v_T_SQL := 'UPDATE ' || :v_tempTableName || ' SET "CurrentItemCost" = 
			ROUND((SELECT "AvgPrice" FROM OITW A WHERE A."ItemCode" =  ' || :v_tempTableName || '."ItemCode" 
			AND A."WhsCode" = ' || :v_tempTableName || '."WhsCode"),6) WHERE "LineFlag" = ''2'' 
			AND ("EvalSystem" = ''S'' OR ("EvalSystem" = ''A'' AND (SELECT "OnHand" FROM OITW A WHERE A."ItemCode" = ' || :v_tempTableName || '."ItemCode" 
			AND A."WhsCode" = ' || :v_tempTableName || '."WhsCode") <= 0))';
			--print (v_T_SQL)
			exec (:v_T_SQL);	
			v_T_SQL := 'UPDATE ' || :v_tempTableName || ' SET "CurrentItemCost" = 
			ROUND((SELECT "StockValue" FROM OITW A WHERE A."ItemCode" =' || :v_tempTableName || '."ItemCode" 
			AND A."WhsCode" = ' || :v_tempTableName || '."WhsCode")/(SELECT "OnHand" FROM OITW A WHERE A."ItemCode" =' || :v_tempTableName || '."ItemCode" 
			AND A."WhsCode" = ' || :v_tempTableName || '."WhsCode"),6) WHERE "LineFlag" = ''2'' 
			AND "EvalSystem" = ''A'' AND (SELECT "OnHand" FROM OITW A WHERE A."ItemCode" = ' || :v_tempTableName || '."ItemCode" 
			AND A."WhsCode" = ' || :v_tempTableName || '."WhsCode") > 0';
			--print (v_T_SQL)
			exec (:v_T_SQL);
			
			v_T_SQL := 'UPDATE ' || :v_tempTableName || ' SET "CurrentItemCost" = 0 WHERE ' || :v_groupType1652 || ' AND "EvalSystem" = ''F''';
			exec (:v_T_SQL);
		ELSE
			
			v_T_SQL := 'UPDATE ' || :v_tempTableName || ' SET "CurrentItemCost" = 
			ROUND((SELECT "AvgPrice" FROM OITM A WHERE A."ItemCode" = ' || :v_tempTableName || '."ItemCode"),6) WHERE ' || :v_groupType1652 || '
			AND ("ValuationMethod" = ''S'' OR ("ValuationMethod" = ''A'' AND (SELECT "OnHand" FROM OITM A WHERE A."ItemCode" = ' || :v_tempTableName || '."ItemCode") <= 0))';
			--print (v_T_SQL)
			exec (:v_T_SQL);
			v_T_SQL := 'UPDATE ' || :v_tempTableName || ' SET "CurrentItemCost" = 
			ROUND((SELECT "StockValue" FROM OITM A WHERE A."ItemCode" = ' || :v_tempTableName || '."ItemCode")/
			(SELECT "OnHand" FROM OITM A WHERE A."ItemCode" = ' || :v_tempTableName || '."ItemCode"),6) WHERE ' || :v_groupType1652 || '
			AND "ValuationMethod" = ''A'' AND (SELECT "OnHand" FROM OITM A WHERE A."ItemCode" = ' || :v_tempTableName || '."ItemCode") > 0';
			--print (v_T_SQL)
			exec (:v_T_SQL);
			
			v_T_SQL := 'UPDATE ' || :v_tempTableName || ' SET "CurrentItemCost" = 0 WHERE ' || :v_groupType1652 || ' AND "ValuationMethod" = ''F''';
			exec (:v_T_SQL);
			
		END IF;
		
	END IF; --CurrentItemCost
	
	--delete redundant data
	IF :v_groupType = '1' then
		v_T_SQL := 'UPDATE ' || :v_tempTableName || ' SET "ItemCode" = NULL, "ItemName" = NULL WHERE "LineFlag" <> ''1''';
		exec (:v_T_SQL);
	end if;
	
	IF :v_groupType = '3' then
		v_T_SQL := 'UPDATE ' || :v_tempTableName || ' SET "ItemCode" = NULL, "ItemName" = NULL WHERE "LineFlag" <> ''1''';
		exec (:v_T_SQL);
		v_T_SQL := 'UPDATE ' || :v_tempTableName || ' SET "WhsCode" = NULL WHERE "LineFlag" NOT IN (''1'',''2'')';
		exec (:v_T_SQL);
	END if;
		
	if :v_groupType = '4' then
		v_T_SQL := 'UPDATE ' || :v_tempTableName || ' SET "WhsCode" = NULL WHERE "LineFlag" <> ''1''';
		exec (:v_T_SQL);
		v_T_SQL := 'UPDATE ' || :v_tempTableName || ' SET "ItemCode" = NULL, "ItemName" = NULL WHERE "LineFlag" NOT IN (''1'',''2'')';
		exec (:v_T_SQL);

	END IF;
		
	--delete OB Qty&TransValue
	v_T_SQL := 'UPDATE ' || :v_tempTableName || ' SET "Qty" = NULL, "TransValue" = NULL WHERE "LineFlag" = ''100''';
	--print (v_T_SQL)
	exec (:v_T_SQL);
	
	--update CumulativeCost from 0 to NULL
	IF :v_groupType <> '2' THEN
		v_T_SQL := 'UPDATE ' || :v_tempTableName || ' SET "CumulativeCost" = NULL WHERE "LineFlag" IN (''1'', ''2'') AND "CumulativeCost" = 0';
		exec (:v_T_SQL);
	END IF;
	
	--update UserSign to UserName
	v_T_SQL := 'UPDATE ' || :v_tempTableName || ' SET "UserSign" = 
				 (SELECT "U_NAME" FROM "OUSR" A WHERE A."USERID" = ' || :v_tempTableName || '."UserSign")
				  WHERE "LineFlag" = ''300''';
	exec (:v_T_SQL);
	
	--update rate
	v_T_SQL := 'UPDATE ' || :v_tempTableName || ' SET "Rate" = NULL WHERE "Rate" = 0';
	exec (:v_T_SQL);

	-- Insert tree structure of result into SPPLA 
	DELETE FROM SPPLA WHERE "PagingID" = :in_nvc_pagingID;
	
	IF :v_groupType = '1' OR :v_groupType = '2' THEN
		v_T_SQL := 'INSERT INTO SPPLA ';
		v_T_SQL := v_T_SQL || 'SELECT ' || ''''|| :in_nvc_pagingID || '''' || ', 
				T0."PaginationId", 
				MAP(T0."LineFlag", 1, 1, 100, 2, 300, 2, T0."LineFlag") AS "LineFlag", 
				MAP(T0."LineFlag", 1, LEAD(T0."PaginationId") over (partition by T0."LineFlag" order by T0."PaginationId") - T0."PaginationId") - 1 AS "ChildNo"
				FROM ' || :v_tempTableName || ' T0 WHERE T0."LineFlag" <> 999 ORDER BY T0."PaginationId"';
		exec (:v_T_SQL);
		
		v_T_SQL := 'UPDATE SPPLA SET "ChildrenNo" ';
		v_T_SQL := v_T_SQL || '= (SELECT MAX ("PaginationId") FROM ' || :v_tempTableName ||') - "RowID" WHERE "ChildrenNo" IS NULL AND "LineLevel" <> ''2''';
		exec (:v_T_SQL);
	END IF;
	
	IF :v_groupType = '3' OR :v_groupType = '4' THEN
		v_T_SQL := 'INSERT INTO SPPLA ';
		v_T_SQL := v_T_SQL || 'SELECT ' || ''''|| :in_nvc_pagingID || '''' || ', 
				T0."PaginationId", 
				MAP(T0."LineFlag", 1, 1, 2, 2, 100, 3, 300, 3, T0."LineFlag") AS "LineFlag", 
				MAP(T0."LineFlag", 2, LEAD(T0."PaginationId") over (partition by map(T0."LineFlag", 1, 2, T0."LineFlag") order by T0."PaginationId") - T0."PaginationId" ,
								   1, LEAD(T0."PaginationId") over (partition by T0."LineFlag" order by T0."PaginationId") - T0."PaginationId") - 1 AS "ChildNo"
				FROM ' || :v_tempTableName || ' T0 WHERE T0."LineFlag" <> 999 ORDER BY T0."PaginationId"';
		exec (:v_T_SQL);
			
		v_T_SQL := 'UPDATE SPPLA SET "ChildrenNo" ';
		v_T_SQL := v_T_SQL || '= (SELECT MAX ("PaginationId") FROM ' || :v_tempTableName ||') - "RowID" WHERE "ChildrenNo" IS NULL AND "LineLevel" <> ''3''';
		exec (:v_T_SQL);
	END IF;	
END;


