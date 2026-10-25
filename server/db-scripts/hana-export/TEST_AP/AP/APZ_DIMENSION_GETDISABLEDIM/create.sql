
Create procedure APZ_Dimension_GetDisableDim
As
Begin

declare dim1 char(1) = 'N';
declare dim2 char(1) = 'N';
declare dim3 char(1) = 'N';
declare dim4 char(1) = 'N';
declare dim5 char(1) = 'N';
declare temp_var_0 integer;
declare temp_var_1 integer;
declare temp_var_2 integer;
declare temp_var_3 integer;
declare temp_var_4 integer;
declare temp_var_02 integer;
declare temp_var_12 integer;
declare temp_var_22 integer;
declare temp_var_32 integer;
declare temp_var_42 integer;
declare temp_var_03 integer;
declare temp_var_13 integer;
declare temp_var_23 integer;
declare temp_var_33 integer;
declare temp_var_43 integer;
declare temp_var_04 integer;
declare temp_var_14 integer;
declare temp_var_24 integer;
declare temp_var_34 integer;
declare temp_var_44 integer;
declare temp_var_05 integer;
declare temp_var_15 integer;
declare temp_var_25 integer;
declare temp_var_35 integer;
declare temp_var_45 integer;

SELECT (SELECT TOP 1 1 FROM APRC WHERE "DimCode" = 1) INTO temp_var_0 FROM DUMMY;
IF :temp_var_0 > 0 THEN dim1 := 'Y';

SELECT (SELECT COUNT(1) FROM OPRC WHERE "DimCode" = 1) INTO temp_var_1 FROM DUMMY;
IF :temp_var_1 > 1 THEN dim1 := 'Y';

SELECT (SELECT TOP 1 1 FROM OOCR WHERE "DimCode" = 1) INTO temp_var_2 FROM DUMMY;
IF :temp_var_2 > 0 THEN dim1 := 'Y';
 
SELECT (SELECT TOP 1 1 FROM AOCR WHERE "DimCode" = 1) INTO temp_var_3 FROM DUMMY;
IF :temp_var_3 > 0 THEN dim1 := 'Y';

SELECT (SELECT TOP 1 1 FROM AMDR WHERE "DimCode" = 1) INTO temp_var_4 FROM DUMMY;
IF :temp_var_4 > 0 THEN dim1 := 'Y';
END IF;
END IF;
END IF;
END IF;
END IF;



SELECT (SELECT TOP 1 1 FROM APRC WHERE "DimCode" = 2) INTO temp_var_02 FROM DUMMY;
IF :temp_var_02 > 0 THEN dim2 := 'Y';

SELECT (SELECT COUNT(1) FROM OPRC WHERE "DimCode" = 2) INTO temp_var_12 FROM DUMMY;
IF :temp_var_12 > 1 THEN dim2 := 'Y';

SELECT (SELECT TOP 1 1 FROM OOCR WHERE "DimCode" = 2) INTO temp_var_22 FROM DUMMY;
IF :temp_var_22 > 0 THEN dim2 := 'Y';

SELECT (SELECT TOP 1 1 FROM AOCR WHERE "DimCode" = 2) INTO temp_var_32 FROM DUMMY;
IF :temp_var_32 > 0 THEN dim2 := 'Y';


SELECT (SELECT TOP 1 1 FROM AMDR WHERE "DimCode" = 2) INTO temp_var_42 FROM DUMMY;
IF :temp_var_42 > 0 THEN dim2 := 'Y';
END IF;
END IF;
END IF;
END IF;
END IF;



SELECT (SELECT TOP 1 1 FROM APRC WHERE "DimCode" = 3) INTO temp_var_03 FROM DUMMY;
IF :temp_var_03 > 0 THEN dim3 := 'Y';

SELECT (SELECT COUNT(1) FROM OPRC WHERE "DimCode" = 3) INTO temp_var_13 FROM DUMMY;
IF :temp_var_13 > 1 THEN dim3 := 'Y';

SELECT (SELECT TOP 1 1 FROM OOCR WHERE "DimCode" = 3) INTO temp_var_23 FROM DUMMY;
IF :temp_var_23 > 0 THEN dim3 := 'Y';

SELECT (SELECT TOP 1 1 FROM AOCR WHERE "DimCode" = 3) INTO temp_var_33 FROM DUMMY;
IF :temp_var_33 > 0 THEN dim3 := 'Y';


SELECT (SELECT TOP 1 1 FROM AMDR WHERE "DimCode" = 3) INTO temp_var_43 FROM DUMMY;
IF :temp_var_43 > 0 THEN dim3 := 'Y';
END IF;
END IF;
END IF;
END IF;
END IF;


SELECT (SELECT TOP 1 1 FROM APRC WHERE "DimCode" = 4) INTO temp_var_04 FROM DUMMY;
IF :temp_var_04 > 0 THEN dim4 := 'Y';

SELECT (SELECT COUNT(1) FROM OPRC WHERE "DimCode" = 4) INTO temp_var_14 FROM DUMMY;
IF :temp_var_1 > 1 THEN dim4 := 'Y';

SELECT (SELECT TOP 1 1 FROM OOCR WHERE "DimCode" = 4) INTO temp_var_24 FROM DUMMY;
IF :temp_var_24 > 0 THEN dim4 := 'Y';

SELECT (SELECT TOP 1 1 FROM AOCR WHERE "DimCode" = 4) INTO temp_var_34 FROM DUMMY;
IF :temp_var_34 > 0 THEN dim4 := 'Y';


SELECT (SELECT TOP 1 1 FROM AMDR WHERE "DimCode" = 4) INTO temp_var_44 FROM DUMMY;
IF :temp_var_44 > 0 THEN dim4 := 'Y';
END IF;
END IF;
END IF;
END IF;
END IF;



SELECT (SELECT TOP 1 1 FROM APRC WHERE "DimCode" = 5) INTO temp_var_05 FROM DUMMY;
IF :temp_var_05 > 0 THEN dim5 := 'Y';

SELECT (SELECT COUNT(1) FROM OPRC WHERE "DimCode" = 5) INTO temp_var_15 FROM DUMMY;
IF :temp_var_15 > 1 THEN dim5 := 'Y';

SELECT (SELECT TOP 1 1 FROM OOCR WHERE "DimCode" = 5) INTO temp_var_25 FROM DUMMY;
IF :temp_var_25 > 0 THEN dim5 := 'Y';

SELECT (SELECT TOP 1 1 FROM AOCR WHERE "DimCode" = 5) INTO temp_var_35 FROM DUMMY;
IF :temp_var_35 > 0 THEN dim5 := 'Y';


SELECT (SELECT TOP 1 1 FROM AMDR WHERE "DimCode" = 5) INTO temp_var_45 FROM DUMMY;
IF :temp_var_45 > 0 THEN dim5 := 'Y';
END IF;
END IF;
END IF;
END IF;
END IF;

SELECT :dim1 || ',' || :dim2 || ',' || :dim3 || ',' || :dim4 || ',' || :dim5 as result FROM DUMMY;
End;




