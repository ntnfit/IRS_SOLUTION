-- B1 DEPENDS: AFTER:SP:_TmSp_BootCreateGlobalTempTables AFTER:PT:PROCESS_END

CREATE PROCEDURE TmSp_DocLineTypeLayout(
in DocKey int,
in ObjectId int)
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
object NVARCHAR(4000);
LineSeq INT;
LineNum2 INT;
LineSeq2 INT;
LineType NCHAR;
TotalLevel INT;
d1 DECIMAL(21,6);
d2 DECIMAL(21,6);
d3 DECIMAL(21,6);
d4 DECIMAL(21,6);
d5 DECIMAL(21,6);
d6 DECIMAL(21,6);
d7 DECIMAL(21,6);
d8 DECIMAL(21,6);
d9 DECIMAL(21,6);
fromLineNum int;
level INT;
CURSOR curTemp FOR SELECT "LineType", "LineNum2", "LineSeq2" FROM "DocLineTypeLayoutTempTab" ORDER BY "DocEntry", "LineNum2", "LineSeq2";
CURSOR cur (level int) FOR SELECT "LineType", "LineNum2", "LineSeq2" FROM "DocLineTypeLayoutTempTab" WHERE "TotalLevel" = :level ORDER BY "DocEntry", "LineNum2", "LineSeq2";
begin
	DELETE FROM "DocLineTypeLayoutTempTab";
	DELETE FROM "DocLineTypeLayoutDOC1";
	DELETE FROM "DocLineTypeLayoutDOC10";

	IF :ObjectId = 13 THEN
		INSERT INTO  "DocLineTypeLayoutDOC1" (SELECT "DocEntry", "VisOrder", "LineType", "LineTotal", "TotalFrgn", "TotalSumSy", "GTotal", "GTotalFC", "GTotalSC", "VatSum", "VatSumFrgn", "VatSumSy"  FROM INV1 WHERE "DocEntry" = :DocKey);
		INSERT INTO  "DocLineTypeLayoutDOC10" (SELECT "DocEntry", "AftLineNum", "LineSeq", "LineType", "LineText" FROM INV10 WHERE "DocEntry" = :DocKey);
	ELSEIF :ObjectId=14 THEN 
		object :=  'RIN';
		INSERT INTO  "DocLineTypeLayoutDOC1" (SELECT "DocEntry", "VisOrder", "LineType", "LineTotal", "TotalFrgn", "TotalSumSy", "GTotal", "GTotalFC", "GTotalSC", "VatSum", "VatSumFrgn", "VatSumSy" FROM RIN1 WHERE "DocEntry" = :DocKey);
		INSERT INTO  "DocLineTypeLayoutDOC10" (SELECT "DocEntry", "AftLineNum", "LineSeq", "LineType", "LineText" FROM RIN10 WHERE "DocEntry" = :DocKey);
	ELSEIF :ObjectId=15 THEN 
		object :=  'DLN';
		INSERT INTO  "DocLineTypeLayoutDOC1" (SELECT "DocEntry", "VisOrder", "LineType", "LineTotal", "TotalFrgn", "TotalSumSy", "GTotal", "GTotalFC", "GTotalSC", "VatSum", "VatSumFrgn", "VatSumSy" FROM DLN1 WHERE "DocEntry" = :DocKey);
		INSERT INTO  "DocLineTypeLayoutDOC10" (SELECT "DocEntry", "AftLineNum", "LineSeq", "LineType", "LineText" FROM DLN10 WHERE "DocEntry" = :DocKey);
	ELSEIF :ObjectId=16 THEN
		object :=  'RDN';
		INSERT INTO  "DocLineTypeLayoutDOC1" (SELECT "DocEntry", "VisOrder", "LineType", "LineTotal", "TotalFrgn", "TotalSumSy", "GTotal", "GTotalFC", "GTotalSC", "VatSum", "VatSumFrgn", "VatSumSy" FROM RDN1 WHERE "DocEntry" = :DocKey);
		INSERT INTO  "DocLineTypeLayoutDOC10" (SELECT "DocEntry", "AftLineNum", "LineSeq", "LineType", "LineText" FROM RDN10 WHERE "DocEntry" = :DocKey);
	ELSEIF :ObjectId=17 THEN
		object :=  'RDR';
		INSERT INTO  "DocLineTypeLayoutDOC1" (SELECT "DocEntry", "VisOrder", "LineType", "LineTotal", "TotalFrgn", "TotalSumSy", "GTotal", "GTotalFC", "GTotalSC", "VatSum", "VatSumFrgn", "VatSumSy" FROM RDR1 WHERE "DocEntry" = :DocKey);
		INSERT INTO  "DocLineTypeLayoutDOC10" (SELECT "DocEntry", "AftLineNum", "LineSeq", "LineType", "LineText" FROM RDR10 WHERE "DocEntry" = :DocKey);
	ELSEIF :ObjectId=18 THEN
		object :=  'PCH';
		INSERT INTO  "DocLineTypeLayoutDOC1" (SELECT "DocEntry", "VisOrder", "LineType", "LineTotal", "TotalFrgn", "TotalSumSy", "GTotal", "GTotalFC", "GTotalSC", "VatSum", "VatSumFrgn", "VatSumSy" FROM PCH1 WHERE "DocEntry" = :DocKey);
		INSERT INTO  "DocLineTypeLayoutDOC10" (SELECT "DocEntry", "AftLineNum", "LineSeq", "LineType", "LineText" FROM PCH10 WHERE "DocEntry" = :DocKey);
	ELSEIF :ObjectId=19 THEN
		object :=  'RPC';
		INSERT INTO  "DocLineTypeLayoutDOC1" (SELECT "DocEntry", "VisOrder", "LineType", "LineTotal", "TotalFrgn", "TotalSumSy", "GTotal", "GTotalFC", "GTotalSC", "VatSum", "VatSumFrgn", "VatSumSy" FROM RPC1 WHERE "DocEntry" = :DocKey);
		INSERT INTO  "DocLineTypeLayoutDOC10" (SELECT "DocEntry", "AftLineNum", "LineSeq", "LineType", "LineText" FROM RPC10 WHERE "DocEntry" = :DocKey);
	ELSEIF :ObjectId=20 THEN 
		object :=  'PDN';
		INSERT INTO  "DocLineTypeLayoutDOC1" (SELECT "DocEntry", "VisOrder", "LineType", "LineTotal", "TotalFrgn", "TotalSumSy", "GTotal", "GTotalFC", "GTotalSC", "VatSum", "VatSumFrgn", "VatSumSy" FROM PDN1 WHERE "DocEntry" = :DocKey);
		INSERT INTO  "DocLineTypeLayoutDOC10" (SELECT "DocEntry", "AftLineNum", "LineSeq", "LineType", "LineText" FROM PDN10 WHERE "DocEntry" = :DocKey);
	ELSEIF :ObjectId=21 THEN
		object :=  'RPD';
		INSERT INTO  "DocLineTypeLayoutDOC1" (SELECT "DocEntry", "VisOrder", "LineType", "LineTotal", "TotalFrgn", "TotalSumSy", "GTotal", "GTotalFC", "GTotalSC", "VatSum", "VatSumFrgn", "VatSumSy" FROM RPD1 WHERE "DocEntry" = :DocKey);
		INSERT INTO  "DocLineTypeLayoutDOC10" (SELECT "DocEntry", "AftLineNum", "LineSeq", "LineType", "LineText" FROM RPD10 WHERE "DocEntry" = :DocKey);
	ELSEIF :ObjectId=22 THEN
		object :=  'POR';
		INSERT INTO  "DocLineTypeLayoutDOC1" (SELECT "DocEntry", "VisOrder", "LineType", "LineTotal", "TotalFrgn", "TotalSumSy", "GTotal", "GTotalFC", "GTotalSC", "VatSum", "VatSumFrgn", "VatSumSy" FROM POR1 WHERE "DocEntry" = :DocKey);
		INSERT INTO  "DocLineTypeLayoutDOC10" (SELECT "DocEntry", "AftLineNum", "LineSeq", "LineType", "LineText" FROM POR10 WHERE "DocEntry" = :DocKey);
	ELSEIF :ObjectId=23 THEN
		object :=  'QUT';
		INSERT INTO  "DocLineTypeLayoutDOC1" (SELECT "DocEntry", "VisOrder", "LineType", "LineTotal", "TotalFrgn", "TotalSumSy", "GTotal", "GTotalFC", "GTotalSC", "VatSum", "VatSumFrgn", "VatSumSy" FROM QUT1 WHERE "DocEntry" = :DocKey);
		INSERT INTO  "DocLineTypeLayoutDOC10" (SELECT "DocEntry", "AftLineNum", "LineSeq", "LineType", "LineText" FROM QUT10 WHERE "DocEntry" = :DocKey);
	ELSEIF :ObjectId=59 THEN
		object :=  'IGN';
		INSERT INTO  "DocLineTypeLayoutDOC1" (SELECT "DocEntry", "VisOrder", "LineType", "LineTotal", "TotalFrgn", "TotalSumSy", "GTotal", "GTotalFC", "GTotalSC", "VatSum", "VatSumFrgn", "VatSumSy" FROM IGN1 WHERE "DocEntry" = :DocKey);
		INSERT INTO  "DocLineTypeLayoutDOC10" (SELECT "DocEntry", "AftLineNum", "LineSeq", "LineType", "LineText" FROM IGN10 WHERE "DocEntry" = :DocKey);
	ELSEIF :ObjectId=60 THEN 
		object :=  'IGE';
		INSERT INTO  "DocLineTypeLayoutDOC1" (SELECT "DocEntry", "VisOrder", "LineType", "LineTotal", "TotalFrgn", "TotalSumSy", "GTotal", "GTotalFC", "GTotalSC", "VatSum", "VatSumFrgn", "VatSumSy" FROM IGE1 WHERE "DocEntry" = :DocKey);
		INSERT INTO  "DocLineTypeLayoutDOC10" (SELECT "DocEntry", "AftLineNum", "LineSeq", "LineType", "LineText" FROM IGE10 WHERE "DocEntry" = :DocKey);
	ELSEIF :ObjectId=67 THEN
		object :=  'WTR';
		INSERT INTO  "DocLineTypeLayoutDOC1" (SELECT "DocEntry", "VisOrder", "LineType", "LineTotal", "TotalFrgn", "TotalSumSy", "GTotal", "GTotalFC", "GTotalSC", "VatSum", "VatSumFrgn", "VatSumSy" FROM WTR1 WHERE "DocEntry" = :DocKey);
		INSERT INTO  "DocLineTypeLayoutDOC10" (SELECT "DocEntry", "AftLineNum", "LineSeq", "LineType", "LineText" FROM WTR10 WHERE "DocEntry" = :DocKey);
	ELSEIF :ObjectId=112 THEN
		object :=  'DRF';
		INSERT INTO  "DocLineTypeLayoutDOC1" (SELECT "DocEntry", "VisOrder", "LineType", "LineTotal", "TotalFrgn", "TotalSumSy", "GTotal", "GTotalFC", "GTotalSC", "VatSum", "VatSumFrgn", "VatSumSy" FROM DRF1 WHERE "DocEntry" = :DocKey);
		INSERT INTO  "DocLineTypeLayoutDOC10" (SELECT "DocEntry", "AftLineNum", "LineSeq", "LineType", "LineText" FROM DRF10 WHERE "DocEntry" = :DocKey);
	ELSEIF :ObjectId=132 THEN 
		object :=  'CIN';
		INSERT INTO  "DocLineTypeLayoutDOC1" (SELECT "DocEntry", "VisOrder", "LineType", "LineTotal", "TotalFrgn", "TotalSumSy", "GTotal", "GTotalFC", "GTotalSC", "VatSum", "VatSumFrgn", "VatSumSy" FROM CIN1 WHERE "DocEntry" = :DocKey);
		INSERT INTO  "DocLineTypeLayoutDOC10" (SELECT "DocEntry", "AftLineNum", "LineSeq", "LineType", "LineText" FROM CIN10 WHERE "DocEntry" = :DocKey);
	ELSEIF :ObjectId=163 THEN
		object :=  'CPI';
		INSERT INTO  "DocLineTypeLayoutDOC1" (SELECT "DocEntry", "VisOrder", "LineType", "LineTotal", "TotalFrgn", "TotalSumSy", "GTotal", "GTotalFC", "GTotalSC", "VatSum", "VatSumFrgn", "VatSumSy" FROM CPI1 WHERE "DocEntry" = :DocKey);
		INSERT INTO  "DocLineTypeLayoutDOC10" (SELECT "DocEntry", "AftLineNum", "LineSeq", "LineType", "LineText" FROM CPI10 WHERE "DocEntry" = :DocKey);
	ELSEIF :ObjectId=164 THEN
		object :=  'CPV';
		INSERT INTO  "DocLineTypeLayoutDOC1" (SELECT "DocEntry", "VisOrder", "LineType", "LineTotal", "TotalFrgn", "TotalSumSy", "GTotal", "GTotalFC", "GTotalSC", "VatSum", "VatSumFrgn", "VatSumSy" FROM CPV1 WHERE "DocEntry" = :DocKey);
		INSERT INTO  "DocLineTypeLayoutDOC10" (SELECT "DocEntry", "AftLineNum", "LineSeq", "LineType", "LineText" FROM CPV10 WHERE "DocEntry" = :DocKey);
	ELSEIF :ObjectId=165 THEN
		object :=  'CSI';
		INSERT INTO  "DocLineTypeLayoutDOC1" (SELECT "DocEntry", "VisOrder", "LineType", "LineTotal", "TotalFrgn", "TotalSumSy", "GTotal", "GTotalFC", "GTotalSC", "VatSum", "VatSumFrgn", "VatSumSy" FROM CSI1 WHERE "DocEntry" = :DocKey);
		INSERT INTO  "DocLineTypeLayoutDOC10" (SELECT "DocEntry", "AftLineNum", "LineSeq", "LineType", "LineText" FROM CSI10 WHERE "DocEntry" = :DocKey);
	ELSEIF :ObjectId=166 THEN 
		object :=  'CSV';
		INSERT INTO  "DocLineTypeLayoutDOC1" (SELECT "DocEntry", "VisOrder", "LineType", "LineTotal", "TotalFrgn", "TotalSumSy", "GTotal", "GTotalFC", "GTotalSC", "VatSum", "VatSumFrgn", "VatSumSy" FROM CSV1 WHERE "DocEntry" = :DocKey);
		INSERT INTO  "DocLineTypeLayoutDOC10" (SELECT "DocEntry", "AftLineNum", "LineSeq", "LineType", "LineText" FROM CSV10 WHERE "DocEntry" = :DocKey);
	ELSEIF :ObjectId=203 THEN
		object :=  'DPI';
		INSERT INTO  "DocLineTypeLayoutDOC1" (SELECT "DocEntry", "VisOrder", "LineType", "LineTotal", "TotalFrgn", "TotalSumSy", "GTotal", "GTotalFC", "GTotalSC", "VatSum", "VatSumFrgn", "VatSumSy" FROM DPI1 WHERE "DocEntry" = :DocKey);
		INSERT INTO  "DocLineTypeLayoutDOC10" (SELECT "DocEntry", "AftLineNum", "LineSeq", "LineType", "LineText" FROM DPI10 WHERE "DocEntry" = :DocKey);
	ELSEIF :ObjectId=204 THEN
		object :=  'DPO';
		INSERT INTO  "DocLineTypeLayoutDOC1" (SELECT "DocEntry", "VisOrder", "LineType", "LineTotal", "TotalFrgn", "TotalSumSy", "GTotal", "GTotalFC", "GTotalSC", "VatSum", "VatSumFrgn", "VatSumSy" FROM DPO1 WHERE "DocEntry" = :DocKey);
		INSERT INTO  "DocLineTypeLayoutDOC10" (SELECT "DocEntry", "AftLineNum", "LineSeq", "LineType", "LineText" FROM DPO10 WHERE "DocEntry" = :DocKey);
	ELSEIF :ObjectId=140000009 THEN
		 object :=  'OEI';		
		 INSERT INTO  "DocLineTypeLayoutDOC1" (SELECT "DocEntry", "VisOrder", "LineType", "LineTotal", "TotalFrgn", "TotalSumSy", "GTotal", "GTotalFC", "GTotalSC", "VatSum", "VatSumFrgn", "VatSumSy" FROM OEI1 WHERE "DocEntry" = :DocKey);
		 INSERT INTO  "DocLineTypeLayoutDOC10" (SELECT "DocEntry", "AftLineNum", "LineSeq", "LineType", "LineText" FROM OEI10 WHERE "DocEntry" = :DocKey);
	ELSEIF :ObjectId=140000010 THEN
		object :=  'IEI';
		INSERT INTO  "DocLineTypeLayoutDOC1" (SELECT "DocEntry", "VisOrder", "LineType", "LineTotal", "TotalFrgn", "TotalSumSy", "GTotal", "GTotalFC", "GTotalSC", "VatSum", "VatSumFrgn", "VatSumSy" FROM IEI1 WHERE "DocEntry" = :DocKey);
		INSERT INTO  "DocLineTypeLayoutDOC10" (SELECT "DocEntry", "AftLineNum", "LineSeq", "LineType", "LineText" FROM IEI10 WHERE "DocEntry" = :DocKey);
	ELSEIF :ObjectId=540000006 THEN
		object :=  'PQT';
		INSERT INTO  "DocLineTypeLayoutDOC1" (SELECT "DocEntry", "VisOrder", "LineType", "LineTotal", "TotalFrgn", "TotalSumSy", "GTotal", "GTotalFC", "GTotalSC", "VatSum", "VatSumFrgn", "VatSumSy" FROM PQT1 WHERE "DocEntry" = :DocKey);
		INSERT INTO  "DocLineTypeLayoutDOC10" (SELECT "DocEntry", "AftLineNum", "LineSeq", "LineType", "LineText" FROM PQT10 WHERE "DocEntry" = :DocKey);
	ELSEIF :ObjectId=1470000113 THEN
		object :=  'PRQ';
		INSERT INTO  "DocLineTypeLayoutDOC1" (SELECT "DocEntry", "VisOrder", "LineType", "LineTotal", "TotalFrgn", "TotalSumSy", "GTotal", "GTotalFC", "GTotalSC", "VatSum", "VatSumFrgn", "VatSumSy" FROM PRQ1 WHERE "DocEntry" = :DocKey);
		INSERT INTO  "DocLineTypeLayoutDOC10" (SELECT "DocEntry", "AftLineNum", "LineSeq", "LineType", "LineText" FROM PRQ10 WHERE "DocEntry" = :DocKey);
	ELSEIF :ObjectId=234000031 THEN
		object :=  'RRR';
		INSERT INTO  "DocLineTypeLayoutDOC1" (SELECT "DocEntry", "VisOrder", "LineType", "LineTotal", "TotalFrgn", "TotalSumSy", "GTotal", "GTotalFC", "GTotalSC", "VatSum", "VatSumFrgn", "VatSumSy" FROM RRR1 WHERE "DocEntry" = :DocKey);
		INSERT INTO  "DocLineTypeLayoutDOC10" (SELECT "DocEntry", "AftLineNum", "LineSeq", "LineType", "LineText" FROM RRR10 WHERE "DocEntry" = :DocKey);
	ELSEIF :ObjectId=234000032 THEN
		object :=  'PRR';
		INSERT INTO  "DocLineTypeLayoutDOC1" (SELECT "DocEntry", "VisOrder", "LineType", "LineTotal", "TotalFrgn", "TotalSumSy", "GTotal", "GTotalFC", "GTotalSC", "VatSum", "VatSumFrgn", "VatSumSy" FROM PRR1 WHERE "DocEntry" = :DocKey);
		INSERT INTO  "DocLineTypeLayoutDOC10" (SELECT "DocEntry", "AftLineNum", "LineSeq", "LineType", "LineText" FROM PRR10 WHERE "DocEntry" = :DocKey);			
	ELSEIF :ObjectId=254000065 THEN
		object :=  'SFI';
		INSERT INTO  "DocLineTypeLayoutDOC1" (SELECT "DocEntry", "VisOrder", "LineType", "LineTotal", "TotalFrgn", "TotalSumSy", "GTotal", "GTotalFC", "GTotalSC", "VatSum", "VatSumFrgn", "VatSumSy" FROM SFI1 WHERE "DocEntry" = :DocKey);
		INSERT INTO  "DocLineTypeLayoutDOC10" (SELECT "DocEntry", "AftLineNum", "LineSeq", "LineType", "LineText" FROM SFI10 WHERE "DocEntry" = :DocKey);	
	ELSEIF :ObjectId=254000066 THEN
		object :=  'SFC';
		INSERT INTO  "DocLineTypeLayoutDOC1" (SELECT "DocEntry", "VisOrder", "LineType", "LineTotal", "TotalFrgn", "TotalSumSy", "GTotal", "GTotalFC", "GTotalSC", "VatSum", "VatSumFrgn", "VatSumSy" FROM SFC1 WHERE "DocEntry" = :DocKey);
		INSERT INTO  "DocLineTypeLayoutDOC10" (SELECT "DocEntry", "AftLineNum", "LineSeq", "LineType", "LineText" FROM SFC10 WHERE "DocEntry" = :DocKey);	
	END IF;


INSERT INTO "DocLineTypeLayoutTempTab" SELECT "DocEntry", "VisOrder", "VisOrder", -1, "LineType", '', -1, 0, 0, 0, 0, 0, 0 , 0, 0, 0 FROM "DocLineTypeLayoutDOC1";
INSERT INTO "DocLineTypeLayoutTempTab" SELECT "DocEntry", -1, "AftLineNum", "LineSeq", "LineType", "LineText", -1, 0, 0, 0, 0, 0, 0, 0, 0, 0 FROM "DocLineTypeLayoutDOC10";

TotalLevel := 0;
OPEN curTemp;
FETCH curTemp INTO LineType, LineNum2, LineSeq2;
WHILE NOT curTemp::NOTFOUND DO
	IF :LineType = 'S' THEN
		UPDATE "DocLineTypeLayoutTempTab" SET "TotalLevel" = :TotalLevel WHERE "LineNum2" = :LineNum2 AND "LineSeq2" = :LineSeq2;
		TotalLevel := :TotalLevel + 1;
	END IF;
	
	IF :LineType = 'R' THEN
		TotalLevel := 0;
	END IF;
	
	FETCH curTemp INTO LineType, LineNum2, LineSeq2;
END WHILE;

-- Update Total
level := 0;
WHILE :level < 10 DO
	
	fromLineNum := 0;
	
	OPEN cur (:level);
	FETCH cur INTO LineType, LineNum2, LineSeq2;
	
	WHILE NOT cur::NOTFOUND DO
		IF :LineType = 'S' THEN
			SELECT 
				 SUM("LineTotal"), SUM("TotalFrgn"), SUM("TotalSumSy"), SUM("GTotal"), SUM("GTotalFC"),
				 SUM("GTotalSC"), SUM("VatSum"), SUM("VatSumFrgn"), SUM("VatSumSy") 
				 into d1, d2, d3, d4, d5, d6, d7, d8, d9
			FROM "DocLineTypeLayoutDOC1"
			WHERE (:fromLineNum<= "VisOrder") AND ("VisOrder"<=:LineNum2) AND ("LineType"='R');

			UPDATE "DocLineTypeLayoutTempTab" 
			SET "SubTotal" =	:d1,
				"TotalFrgn" = :d2,
				"TotalSumSy" =:d3,
				"GTotal" =	:d4,
				"GTotalFC" =	:d5,
				"GTotalSC" =	:d6,
				"VatSum" =	:d7,
				"VatSumFrgn" =:d8,
				"VatSumSy" =	:d9
			WHERE "LineNum2" = :LineNum2 AND "LineSeq2" = :LineSeq2;
			fromLineNum := :LineNum2 + 1;
		END IF;
		FETCH cur INTO LineType, LineNum2, LineSeq2;
	END WHILE;
	CLOSE cur;
	level := :level + 1;
END WHILE;

	SELECT * FROM "DocLineTypeLayoutTempTab" ORDER BY "DocEntry", "LineNum2", "LineSeq2";
	
	DELETE FROM "DocLineTypeLayoutTempTab";
	DELETE FROM "DocLineTypeLayoutDOC1";
	DELETE FROM "DocLineTypeLayoutDOC10";
END;


