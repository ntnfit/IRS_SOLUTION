
CREATE PROCEDURE apz_Document_duplicate(in DocEntry INT, in tbl NVARCHAR(30), in tblLine NVARCHAR(30))
AS
BEGIN

	DECLARE EnblExpns CHAR(1) ='N';
	DECLARE sql_expns NVARCHAR(400000)='';
	DECLARE col_expns VARCHAR(400000)='';
	DECLARE tbl_expns CHAR(4);
	DECLARE i TINYINT=1;
	DECLARE tbl10 NVARCHAR(10)='';
	DECLARE sqlString VARCHAR(400000)='';
	
	EnblExpns='Y';--ifnull((SELECT EnblExpns FROM cinf),'N')
	IF :EnblExpns='Y'
	THEN
		
		tbl_expns = SUBSTRING(:tblLine,0,LENGTH(:tblLine) - 1) || '2';

		WHILE :i<=3
		DO
			select IFNULL((SELECT string_agg('"' || COLUMN_NAME || '" "' || COLUMN_NAME || '_F' || cast(:i as nvarchar) || '"', ',') FROM TABLE_COLUMNS WHERE SCHEMA_NAME = CURRENT_SCHEMA and TABLE_NAME='POR2'), '') into col_expns FROM DUMMY;
			sql_expns = :sql_expns || ' 
			LEFT JOIN                       
					(SELECT ' || :col_expns || ' FROM ' || :tbl_expns || ' WHERE "GroupNum"=' || cast((:i-1) as nvarchar) || ')
					 FG' || cast(:i as NVARCHAR) || ' ON A."DocEntry"=FG' || cast(:i as nvarchar) || '."DocEntry_F' || cast(:i as nvarchar) || '" AND FG' || cast(:i as nvarchar) || '."LineNum_F' || cast(:i as nvarchar) || '" = A."LineNum"';
			i = :i + 1;
		END WHILE;
	END IF; 
    
	tbl10=SUBSTRING(:tblLine,0,LENGTH(:tblLine) - 1) || '10';
	
	sqlString = 'create local temporary table #tbl as (SELECT	 A.*
						,(select cast('''' as ntext) from dummy) "LineText"
						,case when b."ManBtchNum"=''Y'' then ''B'' when b."ManSerNum"=''Y'' then ''S'' else ''N'' END as "ManBatchSerial"
						,case when b."ManBtchNum"=''Y'' then ''B'' when b."ManSerNum"=''Y'' then ''S'' else ''N'' END as "ManBtchSerial"
						,ifnull(C."Rate",0) "TaxPercentagePerRow"
						,ifnull(C."Rate",0) "RateVat"
						,ifnull(A."TreeType",'''') "BOMTreeType"
						,A."LineNum" "TempBaseLine"
						,A."DocEntry" "TempBaseEntry"
						,A."ObjType" "TempBaseType"
						,ROW_NUMBER() OVER(ORDER BY A."VisOrder" ASC) AS "TempLineNum"
						, b."SalUnitMsr" as "SalUnitMsr", ifnull(b."NumInSale",0) as "NumInSale", b."BuyUnitMsr" as "BuyUnitMsr", ifnull(b."NumInBuy",0) as "NumInBuy"
						, ifnull(b."UgpEntry",-1) as "UgpCode", b."InvntryUom" as "InvUnitMsr", ifnull(T4."AltQty", 1) / ifnull(T4."BaseQty", 1) as "NumInInven",0 "LineSeq"
						, ifnull(b."OnHand",0) "QuantityOnStock", ifnull(b1."OnHand",0) "QuantityOnWareHouse"
						,ifnull(b."IsCommited",0) "IsCommited", ifnull(b1."IsCommited",0) "IsCommitedInWareHouse"
						,ifnull(b."OnOrder",0) "OnOrder", ifnull(b1."OnOrder",0) "OnOrderInWareHouse", b."PurPackUn", b."SalPackUn", b1."AvgPrice" ';
	sqlString = :sqlString || ',FG1.*,FG2.*,FG3.*
			 FROM ' || :tblline || ' A 
				LEFT JOIN OVTG C on C."Code"=A."VatGroup"  
				LEFT JOIN OITW b1 on a."ItemCode" = b1."ItemCode" and a."WhsCode" = b1."WhsCode"
				INNER JOIN OITM b on a."ItemCode" = b."ItemCode"
				LEFT JOIN UGP1 T4 on b."UgpEntry" = T4."UgpEntry" and T4."UomEntry" in (select "UomEntry" from OUOM where "UomName" = b."InvntryUom")';
	sqlString = :sqlString || :sql_expns;
	sqlString = :sqlString || '
				WHERE  A."DocEntry" =' || casT(:DocEntry as VARCHAR(20)) || '
		
				ORDER BY  ROW_NUMBER() OVER(ORDER BY A."LineNum"));';
	
	exec :sqlString;
	
	sqlString = 'Update #tbl Set  "LineStatus"=''O'' 
							,"BaseType"=-1
							,"BaseEntry"=0
							,"BaseLine"=0
							,"TargetType"=-1
							,"TrgetEntry"=0
							,"LineNum"="TempLineNum"-1
							,"OpenQty"="Quantity"
							,"OpenInvQty"="InvQty"
							,"DelivrdQty"=0;';

	exec :sqlString;
				    	
	sqlString = 'Update #tbl Set "Rate"=1 where "Rate"=0;';
	exec :sqlString;
	
	
	sqlString =  'INSERT INTO #tbl ("BOMTreeType", "DocEntry", "LineNum", "ItemCode","LineText", "VisOrder", "Dscription", "SalUnitMsr", "NumInSale", "BuyUnitMsr", "NumInBuy", "UgpCode", "InvUnitMsr", "NumInInven", "ManBatchSerial", "ManBtchSerial", "LineType", "TaxPercentagePerRow", "RateVat", "TempBaseLine", "TempBaseEntry", "TempBaseType", "TempLineNum", "LineSeq", "QuantityOnStock", "QuantityOnWareHouse", "IsCommited", "IsCommitedInWareHouse", "OnOrder", "OnOrderInWareHouse")
					SELECT '''',"DocEntry","AftLineNum",'''',"LineText","AftLineNum",CASE WHEN "LineType"=''S'' THEN ''SubTotal'' ELSE '''' END,'''',0,'''',0,-1,'''',0,''N'', ''N'',"LineType",0,0,0,0,-1,0,"LineSeq",0,0,0,0,0,0
					FROM ' || :tbl10 || ' WHERE "DocEntry" =' || cast(:DocEntry as NVARCHAR) || ';';

	exec :sqlString;
	
	EXECUTE IMMEDIATE 'SELECT  * FROM #tbl  order by "VisOrder","LineSeq";';
	exec 'DROP TABLE #tbl;';
END




