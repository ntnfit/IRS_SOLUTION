--- [APZ_D_Document_GetDocLine] '2758','OPOR','POR1'

CREATE PROCEDURE APZ_D_Document_GetDocLine(
in DocEntry NVARCHAR(254),
in tbl VARCHAR(30),
in tblLine NVARCHAR(30))
AS
BEGIN
	DECLARE SQLQuery VARCHAR(50000);
	DECLARE EnblExpns CHAR(1) ='N';
	DECLARE tbl10 NVARCHAR(10)='';
	DECLARE sql_expns NVARCHAR(50000)='';
	DECLARE col_expns VARCHAR(50000)='';
	DECLARE tbl_expns CHAR(4);
	DECLARE i TINYINT=1;
	declare SqlCreateTable nvarchar(50000);
	tbl10=SUBSTRING(:tblLine,0,LENGTH(:tblLine) - 1) || '10';
	EnblExpns='Y';--ISNULL((SELECT EnblExpns FROM cinf),'N')

	IF	:EnblExpns='Y'
	then
	
		tbl_expns=SUBSTRING(:tblLine,0,LENGTH(:tblLine) - 1) || '2';

		WHILE i<=3
		DO
			select (SELECT string_agg(case when IS_NULLABLE = 'TRUE' then '"' || COLUMN_NAME || '"' else 'cast(null as '|| DATA_TYPE_NAME || case when DATA_TYPE_NAME = 'NVARCHAR' then '('|| "LENGTH" ||')' else '' end  ||')' end || ' "'|| COLUMN_NAME || '_F' || cast(:i as nvarchar) ||'",') FROM TABLE_COLUMNS WHERE SCHEMA_NAME = current_schema and TABLE_NAME='POR2') into col_expns from DUMMY;
			col_expns=SUBSTRING(col_expns,0,LENGTH(col_expns) - 1);
			sql_expns= :sql_expns || ' 
			LEFT JOIN                       
					(SELECT '|| :col_expns || ' FROM ' || :tbl_expns || ' WHERE "GroupNum"=' || cast((:i-1) as nvarchar) || ') FG' || cast(:i as nvarchar) || ' ON A."DocEntry"=FG' || cast(:i as nvarchar) || '."DocEntry_F' || cast(:i as nvarchar) || '" AND FG' || cast(:i as nvarchar) || '."LineNum_F' || cast(:i as nvarchar) ||
					 '" = A."LineNum"';		
			i=:i+1;
		END while;
	END if;
	
	
	SqlCreateTable = 'create local temporary table #tbl as (SELECT top 0 A.*
											,(select cast('''' as ntext) from dummy) "LineText"
											,C."Rate" "RateVat",null "BOMTreeType", b."SalUnitMsr" as "SalUnitMsr", b."NumInSale" "NumInSale", b."BuyUnitMsr" as "BuyUnitMsr", "NumInBuy" as "NumInBuy", 
											b."UgpEntry" as "UgpCode", b."InvntryUom" as "InvUnitMsr", d."AltQty" as "NumInInven",
											''B'' "ManBatchSerial",0 "LineSeq", b."OnHand" "QuantityOnStock"
											, t1."OnHand" "QuantityOnWareHouse",b."IsCommited" "IsCommited", t1."IsCommited" "IsCommitedInWareHouse",
											 b."OnOrder" "OnOrder", t1."OnOrder" "OnOrderInWareHouse", b."PurPackUn", b."SalPackUn", t1."AvgPrice", e."Number" "AgrNumber" ';
	
	SQLQuery='insert into #tbl
		SELECT A.*,'''',C."Rate" "RateVat",'''' "BOMTreeType", b."SalUnitMsr" as "SalUnitMsr", ifnull(b."NumInSale",0) as "NumInSale", b."BuyUnitMsr" as "BuyUnitMsr", ifnull(b."NumInBuy",0) as "NumInBuy", 
											ifnull(b."UgpEntry",-1) as "UgpCode", b."InvntryUom" as "InvUnitMsr", ifnull(d."AltQty", 1) / IfNULL(d."BaseQty", 1) as "NumInInven",
											case when b."ManBtchNum"=''Y'' then ''B'' when b."ManSerNum"=''Y'' then ''S'' else ''N'' END as "ManBatchSerial",0 "LineSeq", Ifnull(b."OnHand",0) "QuantityOnStock"
											, ifnull(t1."OnHand",0) "QuantityOnWareHouse",Ifnull(b."IsCommited",0) "IsCommited", ifnull(t1."IsCommited",0) "IsCommitedInWareHouse",
											 Ifnull(b."OnOrder",0) "OnOrder", ifnull(t1."OnOrder",0) "OnOrderInWareHouse", b."PurPackUn", b."SalPackUn", t1."AvgPrice", ifnull(e."Number",0) ';
	IF :EnblExpns='Y'
	then
		SqlCreateTable=:SqlCreateTable || ',FG1.*,FG2.*,FG3.* ';
		SQLQuery=:SQLQuery || ',FG1.*,FG2.*,FG3.* ';
	end if;
	
	SQLQuery=:SQLQuery || '
	FROM ' || :tblLine || ' A 
											LEFT JOIN OVTG C ON C."Code"=A."VatGroup" 
											INNER JOIN OITM b on a."ItemCode" = b."ItemCode"
											LEFT JOIN OOAT e on a."AgrNo" = e."AbsID"
											LEFT JOIN OITW t1 on a."ItemCode" = t1."ItemCode" and a."WhsCode" = t1."WhsCode"
											LEFT JOIN UGP1 d on b."UgpEntry" = d."UgpEntry" and d."UomEntry" in (select "UomEntry" from OUOM where "UomName" = b."InvntryUom")';
											
	SqlCreateTable=:SqlCreateTable || '
	FROM ' || :tblLine || ' A 
											LEFT JOIN OVTG C ON C."Code"=A."VatGroup" 
											INNER JOIN OITM b on a."ItemCode" = b."ItemCode"
											LEFT JOIN OOAT e on a."AgrNo" = e."AbsID"
											LEFT JOIN OITW t1 on a."ItemCode" = t1."ItemCode" and a."WhsCode" = t1."WhsCode"
											LEFT JOIN UGP1 d on b."UgpEntry" = d."UgpEntry" and d."UomEntry" in (select "UomEntry" from OUOM where "UomName" = b."InvntryUom")';
							
	IF :EnblExpns='Y'
	then
		SQLQuery=:SQLQuery || :sql_expns;
		SqlCreateTable=:SqlCreateTable || :sql_expns || ' where 0 = 1';
	end if;
	
	exec :SqlCreateTable || ')'; 
	
	SQLQuery= :SQLQuery || '
				WHERE  A."DocEntry" in ('|| :DocEntry || ') ORDER BY A."LineNum";';
				exec :SQLQuery;
				--alter table #tbl
				--alter column "ItemCode" nvarchar(5000);
				SQLQuery = 'INSERT INTO #tbl ("BOMTreeType","DocEntry","LineNum","ItemCode","LineText","VisOrder","Dscription","SalUnitMsr","NumInSale","BuyUnitMsr","NumInBuy","UgpCode","InvUnitMsr","NumInInven","ManBatchSerial"
				,"LineType","LineSeq","QuantityOnStock","QuantityOnWareHouse","IsCommited","IsCommitedInWareHouse","OnOrder","OnOrderInWareHouse","ItemType","AgrNumber")
				SELECT '''',"DocEntry","AftLineNum",'''',"LineText","AftLineNum",CASE WHEN "LineType"=''S'' THEN ''SubTotal'' ELSE '''' END,'''',0,'''',0,-1,'''',0,''N'',"LineType","LineSeq",0,0,0,0,0,0,-18,0
				FROM ' || :tbl10 || ' WHERE "DocEntry" in (' || :DocEntry || ');';
				exec :SQLQuery;
				SQLQuery = 'update #tbl set "Rate"=1 where "Rate"=0;';
				exec :SQLQuery;
				SQLQuery = 'SELECT *,"ManBatchSerial" "ManBtchSerial","IsCommited" "IsCommittedItem", "OnOrder" "OnOrderedItem","QuantityOnStock" "OnHandItem","QuantityOnWareHouse" "OnHandWhs","IsCommitedInWareHouse" "IsCommittedWhs"
						,"OnOrderInWareHouse"  "OnOrderedWhs" FROM #tbl order by "VisOrder","LineSeq";';
							
execute immediate :SQLQuery;
exec 'DROP TABLE #tbl;';
						--select :SqlCreateTable || '); 
						--' || :SQLQuery from dummy;
END



