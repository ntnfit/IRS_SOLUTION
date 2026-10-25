-- B1 DEPENDS: AFTER:PT:PROCESS_END AFTER:SP:_TmSp_BootCreateGlobalTempTables2

CREATE PROCEDURE TmSp_GetDocVatTotals(in array nvarchar(4),
	in absEntry int, 
	in totalField int, 
	object nvarchar(4)) 
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
	--i int;
BEGIN
	IF (:totalField = 0) THEN
		EXECUTE IMMEDIATE ('SELECT
			T1."Code", 
			T1."Name", 
			T0."VatPrcnt", 
			T0."EquVatPer",
			0 As "Col1", 0 As "Col2", 0 As "Col3", 0 As "Col4", 0 As "Col5", 0 As "Col6", 0 As "Col7", 0 As "Col8", 0 As "Col9", 0 As "Col10", 0 As "Col11", 0 As "Col12", 0 As "Col13", 0 As "Col14", 0 As "Col15", 0 As "Col16"
			FROM "' || :array || '" T0 INNER  JOIN "OVTG" T1 ON  T0."VatGroup" = T1."Code"   
			INNER JOIN "' || :object || '" T2 ON T0."DocEntry" = T2."DocEntry"
		WHERE T0."DocEntry" = ' || :absEntry || '
		GROUP BY T1."Code", T1."Name", T0."VatPrcnt",  T0."EquVatPer"
		ORDER BY T1."Code"');
   	ELSE IF (:totalField = 1) THEN
		EXECUTE IMMEDIATE ('SELECT 
			T1."Code", 
			T1."Name", 
			T0."VatPrcnt", 
			T0."EquVatPer",
			0 As "Col1", 0 As "Col2", 0 As "Col3", 0 As "Col4", 0 As "Col5", 0 As "Col6", 0 As "Col7", 0 As "Col8", 0 As "Col9", 0 As "Col10", 0 As "Col11", 0 As "Col12", 0 As "Col13", 0 As "Col14", 0 As "Col15", 0 As "Col16"
			FROM "' || :array || '" T0 INNER  JOIN "OVTG" T1 ON  T0."VatGroup" = T1."Code"   
				INNER JOIN "' || :object || '" T2 ON T0."DocEntry" = T2."DocEntry"
			WHERE T0."DocEntry" = ' || :absEntry || '   
			GROUP BY T1."Code", T1."Name", T0."VatPrcnt",  T0."EquVatPer"
			ORDER BY T1."Code"');	
		ELSE
			EXECUTE IMMEDIATE ('SELECT
				T1."Code", 
				T1."Name", 
				T0."VatPrcnt", 
				T0."EquVatPer",
				0 As "Col1", 0 As "Col2", 0 As "Col3", 0 As "Col4", 0 As "Col5", 0 As "Col6", 0 As "Col7", 0 As "Col8", 0 As "Col9", 0 As "Col10", 0 As "Col11", 0 As "Col12", 0 As "Col13", 0 As "Col14", 0 As "Col15", 0 As "Col16"
				FROM "' || :array || '" T0 INNER  JOIN "OVTG" T1 ON  T0."VatGroup" = T1."Code"   
				INNER JOIN "' || :object || '" T2 ON T0."DocEntry" = T2."DocEntry"
				WHERE T0."DocEntry" = ' || :absEntry || '
				GROUP BY T1."Code", T1."Name", T0."VatPrcnt",  T0."EquVatPer"
				ORDER BY T1."Code"');
		END IF;
	END IF;
END;


