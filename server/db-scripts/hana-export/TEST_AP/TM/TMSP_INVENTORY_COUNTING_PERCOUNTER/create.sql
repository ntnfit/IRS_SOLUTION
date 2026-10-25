CREATE PROCEDURE TMSP_Inventory_Counting_PerCounter (
    DocKey nvarchar(12),
    ObjectId NCHAR(20),
    MultipleCounterType nvarchar (1) default 'I' ,
    MultipleCounterOrder nvarchar (12)
)
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER

AS
CountType nvarchar(1) ;
CountNumber INT;
DocEntry INT := :DocKey ;

BEGIN
	IF (:ObjectId = '1470000065' ) THEN
	               SELECT "OINC" ."CountType" INTO CountType FROM "OINC" WHERE "OINC" ."DocEntry" = :DocEntry ;
	
	               IF (:CountType = '1' ) THEN --single counter
	                  SELECT "T1" ."DocEntry" , "T1" ."ItemCode" , "T1" ."ItemDesc" , "T1" ."WhsCode" , "T1" ."BinEntry" , "T2" ."BinCode" , "T1" ."InWhsQty" , "T1" ."Counted" , "T1" ."CountQty"
	                  FROM "INC1" "T1" LEFT JOIN "OBIN" "T2" ON "T1" ."BinEntry" ="T2" ."AbsEntry" WHERE "T1". "DocEntry" = :DocEntry  ORDER BY "T1" ."VisOrder" ;
	               ELSE   --multiple counters
	                  IF (:MultipleCounterType = 'I' ) THEN --individual counter
	                      SELECT "T1" ."CounterNum" INTO CountNumber FROM "INC8" "T1" WHERE "T1" ."DocEntry" = :DocEntry AND "T1". "VisOrder" = :MultipleCounterOrder;
	                     
	                      SELECT "T1" ."DocEntry" , "T1" ."ItemCode" , "T1" ."ItemDesc" , "T1" ."WhsCode" , "T1" ."BinEntry" , "T2" ."BinCode" , "T1" ."InWhsQty" , "T1" ."Counted" , "T3" ."TotalQty" AS "CountQty"
	                      FROM "INC1" "T1"
	                      LEFT JOIN "OBIN" "T2" ON "T1". "BinEntry" ="T2" ."AbsEntry"
	                      LEFT JOIN "INC9" "T3" ON "T1". "DocEntry" ="T3" ."DocEntry" AND  "T1" ."LineNum" ="T3" ."LineNum" AND "T3". "CounterNum" = :CountNumber
	                      WHERE "T1" ."DocEntry" = :DocEntry ORDER BY "T1" ."VisOrder" ;
	                  ELSE   --Team counters
	                      SELECT "T1" ."CounterNum" INTO CountNumber FROM "INC4" "T1" WHERE "T1" ."DocEntry" = :DocEntry AND "T1". "VisOrder" = :MultipleCounterOrder;
	                     
	                      SELECT "T1" ."DocEntry" , "T1" ."ItemCode" , "T1" ."ItemDesc" , "T1" ."WhsCode" , "T1" ."BinEntry" , "T2" ."BinCode" , "T1" ."InWhsQty" , "T1" ."Counted" , "T3" ."TotalQty" AS "CountQty"
	                      FROM "INC1" "T1"
	                      LEFT JOIN "OBIN" "T2" ON "T1". "BinEntry" ="T2" ."AbsEntry"
	                      LEFT JOIN "INC5" "T3" ON "T1". "DocEntry" ="T3" ."DocEntry" AND  "T1" ."LineNum" ="T3" ."LineNum" AND "T3". "CounterNum" = :CountNumber
	                      WHERE "T1" ."DocEntry" = :DocEntry ORDER BY "T1" ."VisOrder" ;
	                END IF ;
	           END IF ;
	Else
			SELECT  "OICD"."CountType" into CountType  FROM "OICD" WHERE "OICD"."DocEntry" = :DocEntry;
			IF (:CountType = '1') then --single counter
					SELECT "T1"."DocEntry", "T1"."ItemCode", "T1"."ItemDesc", "T1"."WhsCode", "T1"."BinEntry", "T2"."BinCode", "T1"."InWhsQty", "T1"."Counted", "T1"."CountQty" 
					FROM "ICD1" "T1" LEFT JOIN "OBIN" "T2" ON "T1"."BinEntry"="T2"."AbsEntry" WHERE "T1"."DocEntry"=:DocEntry ORDER BY "T1"."VisOrder";
			ELSE  --multiple counters
				IF (:MultipleCounterType = 'I') then --individual counter
					SELECT "T1"."CounterNum" INTO CountNumber FROM "ICD8" "T1" WHERE "T1"."DocEntry" = :DocEntry AND "T1"."VisOrder" = :MultipleCounterOrder;
					SELECT "T1"."DocEntry", "T1"."ItemCode", "T1"."ItemDesc", "T1"."WhsCode", "T1"."BinEntry", "T2"."BinCode", "T1"."InWhsQty", "T1"."Counted", "T3"."TotalQty" AS "CountQty" 
					FROM "ICD1" "T1" 
					LEFT JOIN "OBIN" "T2" ON "T1"."BinEntry"="T2"."AbsEntry"
					LEFT JOIN "ICD9" "T3" ON "T1"."DocEntry"="T3"."DocEntry" AND  "T1"."LineNum"="T3"."LineNum" AND "T3"."CounterNum" = :CountNumber
					WHERE "T1"."DocEntry"=:DocEntry ORDER BY "T1"."VisOrder";
				 
				ELSE  --Team counters
					SELECT "T1"."CounterNum" INTO CountNumber FROM "ICD4" "T1" WHERE "T1"."DocEntry" = :DocEntry AND "T1"."VisOrder" = :MultipleCounterOrder;
					SELECT "T1"."DocEntry", "T1"."ItemCode", "T1"."ItemDesc", "T1"."WhsCode", "T1"."BinEntry", "T2"."BinCode", "T1"."InWhsQty", "T1"."Counted", "T3"."TotalQty" AS "CountQty" 
					FROM "ICD1" "T1" 
					LEFT JOIN "OBIN" "T2" ON "T1"."BinEntry"="T2"."AbsEntry"
					LEFT JOIN "ICD5" "T3" ON "T1"."DocEntry"="T3"."DocEntry" AND  "T1"."LineNum"="T3"."LineNum" AND "T3"."CounterNum" = :CountNumber
					WHERE "T1"."DocEntry"= :DocEntry ORDER BY "T1"."VisOrder";
				End if; 
			End if;
	END IF ;
END


