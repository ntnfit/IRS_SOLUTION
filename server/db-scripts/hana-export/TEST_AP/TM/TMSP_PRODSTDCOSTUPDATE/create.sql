CREATE PROCEDURE TmSp_ProdStdCostUpdate
(
	IN itemCodeFrom nvarchar(255),
	IN itemCodeTo nvarchar(255),
	IN itmGroupCode nvarchar(11),
	IN itmProps nvarchar(70)
)
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
	isPerHW nvarchar (1);
	dfltWH nvarchar (255);
	queryStr nclob;
	whereCommonStr nclob;
	whereItemPropStr nvarchar(4000);
	propsRel nvarchar(10);
	ii integer;
	jj integer;
	kk integer;
	oneProp nvarchar(1);
	exactMatch nvarchar(1);
	andRel nvarchar(10);
	whereExactMatchN nvarchar(4000);
BEGIN

	SELECT TOP 1 "PriceSys" into isPerHW FROM OADM;
	SELECT TOP 1 "DfltWhs" into dfltWH FROM OADM;
	
	--Create WHERE condition to filter:
	-- items span
	whereCommonStr := '';
	andRel := '';
	IF :itemCodeFrom <> ''
	THEN
		whereCommonStr := '"T0"."ItemCode" >= ''' || :itemCodeFrom || '''';
		andRel := ' AND ';
	END IF;
	IF :itemCodeTo <> ''
	THEN
		whereCommonStr := :whereCommonStr || :andRel || '"T0"."ItemCode" <= ''' || :itemCodeTo || '''';
		andRel := ' AND ';
	END IF;	
	--group codes
	IF :itmGroupCode <> '0' AND :itmGroupCode <> ''
	THEN
		whereCommonStr := :whereCommonStr || :andRel || '"T0"."ItmsGrpCod" = ''' || :itmGroupCode || '''';
		andRel := ' AND ';
	END IF;
	--item properties
	IF LENGTH(:itmProps) > 0 
	THEN
		IF (SUBSTRING (:itmProps, 65, 1)  = 'N')
		THEN		
			propsRel := ' OR ';
		ELSE
			propsRel := ' AND ';
		END IF;
		
		whereItemPropStr := '';
		whereExactMatchN := '';
		ii := 1;
		jj := 1;
		kk := 1;
		exactMatch := SUBSTRING (:itmProps, 67, 1);
		WHILE (:ii <= 64)
		DO
			oneProp := SUBSTRING (:itmProps, :ii, 1);
			
			IF (:exactMatch = 'Y' AND :oneProp = 'N')
			THEN
				IF (:kk <> 1)
				THEN
					whereExactMatchN := :whereExactMatchN || ' AND ';
				END IF;
				whereExactMatchN := :whereExactMatchN || ' "T0"."QryGroup' || CAST (ii AS CHAR(2)) || '" = ''N''';
				kk := :kk + 1;
			END IF;
						
			IF (:oneProp = 'Y')
			THEN			
				IF (:jj <> 1)
				THEN
					whereItemPropStr := :whereItemPropStr || :propsRel;
				END IF; 	
				whereItemPropStr := :whereItemPropStr || ' "T0"."QryGroup' || CAST (ii AS CHAR(2)) || '" = ''Y''';
				jj := :jj + 1;
			END IF;
			ii := :ii + 1;
		END WHILE;
		
		
		IF (LENGTH(:whereItemPropStr) = 0)
		THEN
			whereItemPropStr := :whereExactMatchN;
			whereExactMatchN := '';
		END IF;
		
		IF (LENGTH(:whereItemPropStr) > 0)
		THEN
			whereItemPropStr := '(' || :whereItemPropStr || ')';
			IF (LENGTH(:whereExactMatchN) > 0)
			THEN
				whereItemPropStr := '(' || :whereItemPropStr || ' AND ' || :whereExactMatchN || ')';
			END IF;
			whereCommonStr := :whereCommonStr || :andRel || :whereItemPropStr;
			andRel := ' AND ';
		END IF;
	END IF;
	
	IF :isPerHW = 'Y'
	THEN
		queryStr := 
		'SELECT "T0"."ItemCode", "T1"."StockValue", "T1"."OnHand", "T1"."AvgPrice", "T0"."PrdStdCst", "T0"."EvalSystem" 
		FROM OITM T0
		INNER JOIN OITW T1 ON "T0"."ItemCode" = "T1"."ItemCode" AND (("T0"."DfltWH" Is Not Null AND "T0"."DfltWH" = "T1"."WhsCode") OR ("T0"."DfltWH" Is Null AND "T1"."WhsCode" = ''' || :dfltWH || '''))
		WHERE ' || :whereCommonStr || :andRel || '("T0"."EvalSystem" = (''F'') OR "T0"."EvalSystem" = (''A'') OR "T0"."EvalSystem" = (''S'')) AND ("T1"."OnHand" <> 0 OR "T1"."StockValue" = 0)';
	ELSE
		queryStr :=
		'SELECT "T0"."ItemCode", "T0"."StockValue", "T0"."OnHand", "T0"."AvgPrice", "T0"."PrdStdCst", "T0"."EvalSystem" 
		FROM OITM T0
		WHERE ' || :whereCommonStr || :andRel || '("T0"."EvalSystem" = (''F'') OR "T0"."EvalSystem" = (''A'') OR "T0"."EvalSystem" = (''S'')) AND ("T0"."OnHand" <> 0 OR "T0"."StockValue" = 0)';
	END IF;	

	queryStr := :queryStr || 
	'
	
	UNION ALL 
	
	SELECT "T0"."ItemCode", SUM("T1"."CostTotal"), SUM("T1"."Quantity"), 1, "T0"."PrdStdCst", "T0"."EvalSystem" 
	FROM OITM T0
	INNER JOIN OSRN T1 ON "T0"."ItemCode" = "T1"."ItemCode" 
	WHERE ' || :whereCommonStr || :andRel || '"T0"."EvalSystem" = (''B'') AND "T0"."ManSerNum" = (''Y'')
	GROUP BY "T0"."ItemCode", "T0"."PrdStdCst", "T0"."EvalSystem" 

	UNION ALL 
	
	SELECT "T0"."ItemCode", SUM("T1"."CostTotal"), SUM("T1"."Quantity"), 1, "T0"."PrdStdCst", "T0"."EvalSystem" 
	FROM OITM T0
	INNER JOIN OBTN T1 ON "T0"."ItemCode" = "T1"."ItemCode" 
	WHERE ' || :whereCommonStr || :andRel || '"T0"."EvalSystem" = (''B'') AND "T0"."ManBtchNum" = (''Y'') 
	GROUP BY "T0"."ItemCode", "T0"."PrdStdCst", "T0"."EvalSystem"';
	

	--select :queryStr from DUMMY;
	EXECUTE IMMEDIATE :queryStr;
END


