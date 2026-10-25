CREATE PROCEDURE "TmSp_GetDefaultElementsforCR" (
in DocKey int,
in ObjectId int
)
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS 

object NVARCHAR(4000);
CODE INTEGER;
NAME NVARCHAR(100);
TRANSL NCLOB;
LANG INTEGER;
STR NCLOB;
CODE_OLD INTEGER;
LanguageID INTEGER;
LanguageCnt INTEGER;
COMMA_FLAG INTEGER;

CURSOR curTemp FOR SELECT T0."Code", T0."Name", T2."LangCode", IFNULL(T2."Trans", T0."Name") AS "TRANSLATION" 
    FROM "OMLP" T0 
        LEFT OUTER JOIN OMLT T1 ON T1.PK = T0."Code" AND T1."TableName" = 'OMLP' AND T1."FieldAlias" = 'Name' 
        LEFT JOIN MLT1 T2 ON T1."TranEntry" = T2."TranEntry" AND T2."LangCode" = LanguageID
        ORDER BY T0."Code"; 
BEGIN 

create local temporary row table "#Language" ("LangCode" Integer);

IF :ObjectId = 13 THEN
	object :=  'INV';
	INSERT INTO  "#Language" (SELECT "LangCode" FROM OINV WHERE "DocEntry" = :DocKey);
ELSEIF :ObjectId=14 THEN 
	object :=  'RIN';
	INSERT INTO  "#Language" (SELECT "LangCode" FROM ORIN WHERE "DocEntry" = :DocKey);
ELSEIF :ObjectId=15 THEN 
	object :=  'DLN';
	INSERT INTO  "#Language" (SELECT "LangCode" FROM ODLN WHERE "DocEntry" = :DocKey);
ELSEIF :ObjectId=16 THEN
	object :=  'RDN';
	INSERT INTO  "#Language" (SELECT "LangCode" FROM ORDN WHERE "DocEntry" = :DocKey);
ELSEIF :ObjectId=17 THEN
	object :=  'RDR';
	INSERT INTO  "#Language" (SELECT "LangCode" FROM ORDR WHERE "DocEntry" = :DocKey);
ELSEIF :ObjectId=18 THEN
	object :=  'PCH'; 
	INSERT INTO  "#Language" (SELECT "LangCode" FROM OPCH WHERE "DocEntry" = :DocKey);
ELSEIF :ObjectId=19 THEN
	object :=  'RPC';
	INSERT INTO  "#Language" (SELECT "LangCode" FROM ORPC WHERE "DocEntry" = :DocKey);
ELSEIF :ObjectId=20 THEN 
	object :=  'PDN'; 
	INSERT INTO  "#Language" (SELECT "LangCode" FROM OPDN WHERE "DocEntry" = :DocKey);
ELSEIF :ObjectId=21 THEN
	object :=  'RPD';
	INSERT INTO  "#Language" (SELECT "LangCode" FROM ORPD WHERE "DocEntry" = :DocKey);
ELSEIF :ObjectId=22 THEN
	object :=  'POR';
	INSERT INTO  "#Language" (SELECT "LangCode" FROM OPOR WHERE "DocEntry" = :DocKey);
ELSEIF :ObjectId=23 THEN
	object :=  'QUT';
	INSERT INTO  "#Language" (SELECT "LangCode" FROM OQUT WHERE "DocEntry" = :DocKey);
ELSEIF :ObjectId=59 THEN
	object :=  'IGN';
	INSERT INTO  "#Language" (SELECT "LangCode" FROM OIGN WHERE "DocEntry" = :DocKey);
ELSEIF :ObjectId=60 THEN 
	object :=  'IGE';
	INSERT INTO  "#Language" (SELECT "LangCode" FROM OIGE WHERE "DocEntry" = :DocKey);
ELSEIF :ObjectId=67 THEN
	object :=  'WTR';
	INSERT INTO  "#Language" (SELECT "LangCode" FROM OWTR WHERE "DocEntry" = :DocKey);
ELSEIF :ObjectId=112 THEN
	object :=  'DRF';
	INSERT INTO  "#Language" (SELECT "LangCode" FROM ODRF WHERE "DocEntry" = :DocKey);
ELSEIF :ObjectId=132 THEN 
	object :=  'CIN';
	INSERT INTO  "#Language" (SELECT "LangCode" FROM OCIN WHERE "DocEntry" = :DocKey);
ELSEIF :ObjectId=163 THEN 
	object := 'CPI';
	INSERT INTO  "#Language" (SELECT "LangCode" FROM OCPI WHERE "DocEntry" = :DocKey);
ELSEIF :ObjectId=164 THEN 
	object := 'CPV';
	INSERT INTO  "#Language" (SELECT "LangCode" FROM OCPV WHERE "DocEntry" = :DocKey);
ELSEIF :ObjectId=165 THEN 
	object := 'CSI';
	INSERT INTO  "#Language" (SELECT "LangCode" FROM OCSI WHERE "DocEntry" = :DocKey);
ELSEIF :ObjectId=166 THEN 
	object := 'CSV';
	INSERT INTO  "#Language" (SELECT "LangCode" FROM OCSV WHERE "DocEntry" = :DocKey);
ELSEIF :ObjectId=203 THEN 
	object := 'DPI';
	INSERT INTO  "#Language" (SELECT "LangCode" FROM ODPI WHERE "DocEntry" = :DocKey);
ELSEIF :ObjectId=204 THEN 
	object := 'DPO';
	INSERT INTO  "#Language" (SELECT "LangCode" FROM ODPO WHERE "DocEntry" = :DocKey);
ELSEIF :ObjectId=140000009 THEN
	 object :=  'OEI';
	 INSERT INTO  "#Language" (SELECT "LangCode" FROM OOEI WHERE "DocEntry" = :DocKey);
ELSEIF :ObjectId=140000010 THEN
	object :=  'IEI';
	INSERT INTO  "#Language" (SELECT "LangCode" FROM OIEI WHERE "DocEntry" = :DocKey);
ELSEIF :ObjectId=540000006 THEN
	object :=  'PQT';
	INSERT INTO  "#Language" (SELECT "LangCode" FROM OPQT WHERE "DocEntry" = :DocKey);
ELSEIF :ObjectId=1470000113 THEN
	object :=  'PRQ';
	INSERT INTO  "#Language" (SELECT "LangCode" FROM OPRQ WHERE "DocEntry" = :DocKey);
ELSEIF :ObjectId=234000031 THEN
	object :=  'RRR';
	INSERT INTO  "#Language" (SELECT "LangCode" FROM ORRR WHERE "DocEntry" = :DocKey);
ELSEIF :ObjectId=234000032 THEN
	object :=  'PRR';
	INSERT INTO  "#Language" (SELECT "LangCode" FROM OPRR WHERE "DocEntry" = :DocKey);
ELSEIF :ObjectId=254000065 THEN
	object :=  'SFI';
	INSERT INTO  "#Language" (SELECT "LangCode" FROM OSFI WHERE "DocEntry" = :DocKey);
ELSEIF :ObjectId=254000066 THEN
	object :=  'SFC';
	INSERT INTO  "#Language" (SELECT "LangCode" FROM OSFC WHERE "DocEntry" = :DocKey);
ELSE 
	INSERT INTO  "#Language" (SELECT "Language" FROM CINF);
END IF;

SELECT COUNT(*) INTO LanguageCnt FROM "#Language";
IF :LanguageCnt = 0 THEN
	SELECT "Language" INTO LanguageID FROM CINF;
ELSE
	SELECT "LangCode" INTO LanguageID FROM "#Language";
END IF;
DROP TABLE "#Language";

create local temporary row table #t1 (CODE1 INTEGER, NAME1 NVARCHAR(100), TRANSLATION1 NCLOB);
STR := 'SELECT ';
        
OPEN curTemp ;
FETCH curTemp INTO CODE, NAME, LANG, TRANSL;
CODE_OLD := 0;
COMMA_FLAG := 0;
WHILE NOT curTemp::NOTFOUND DO
IF :CODE_OLD <> :CODE THEN
	IF :COMMA_FLAG <> 0 THEN
		STR := :STR || ', ';
	ELSE
		COMMA_FLAG := 1;
	END IF;
	STR := :STR || ' MAX(CASE WHEN "CODE1" = ' || :CODE || ' THEN SUBSTRING("TRANSLATION1",1,8000) ELSE '''' END) AS "' || :NAME || '"';
	CODE_OLD := :CODE;
END IF;
	INSERT INTO #t1 VALUES (:CODE, :NAME, :TRANSL);
FETCH curTemp INTO CODE, NAME, LANG, TRANSL;
END WHILE;
CLOSE curTemp;
STR := :STR || ' FROM #t1 ';

EXECUTE IMMEDIATE :STR; 
DROP TABLE #t1;

END;


