CREATE PROCEDURE TmSp_B1GetSumDifferences(
	docViewName NVARCHAR(32),
	LCCredFieldName NVARCHAR(20),
	FCCredFieldName NVARCHAR(20),
	LCDebFieldName NVARCHAR(20),
	FCDebFieldName NVARCHAR(20),
	excludeGainsAfterDate NVARCHAR(8),
	rDate NVARCHAR(8),
	isDirectRate SMALLINT)
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
	v_sql nvarchar(1000);
	SumDec NVARCHAR(2);
BEGIN	
	v_sql := 'SELECT T0."ShortName", T0."FCCurrency", T0."BPLId", T0."Account", SUM(T0."' || :LCCredFieldName || '"), SUM(T0."' || :FCCredFieldName ||
								'"), SUM(T0."' || :LCDebFieldName || '"), SUM(T0."' || :FCDebFieldName || '") FROM ' || :docViewName || ' AS T0';
	IF LENGTH(:excludeGainsAfterDate) > 0 THEN
		SELECT TOP 1 "SumDec" INTO SumDec FROM OADM;
		v_sql := :v_sql || ' LEFT OUTER JOIN ORTT AS T1 ON T0."FCCurrency" = T1."Currency" AND T1."RateDate" = TO_DATE(''' || :rDate || ''', ''YYYYMMDD'')';
		v_sql := :v_sql || ' WHERE T0."DueDate" <= TO_DATE(''' || :excludeGainsAfterDate || ''', ''YYYYMMDD'') OR ROUND((T0."'||:FCCredFieldName||'"-T0."'||:FCDebFieldName||'")';
		IF :isDirectRate = 1 THEN
			v_sql := :v_sql || '*';
		ELSE	
			v_sql := :v_sql || '/';
		END IF;
		v_sql := :v_sql || 'T1."Rate", ' || :SumDec || ') >= (T0."'||:LCCredFieldName||'"-T0."'||:LCDebFieldName||'")';
	END	IF;
	v_sql := :v_sql	|| ' GROUP BY T0."ShortName", T0."FCCurrency", T0."BPLId", T0."Account" ORDER BY T0."ShortName"';
	
	EXECUTE IMMEDIATE (:v_sql);
	--SELECT :v_sql from dummy;
END


