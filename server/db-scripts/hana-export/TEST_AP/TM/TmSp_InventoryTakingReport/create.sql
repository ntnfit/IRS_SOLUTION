-- B1 DEPENDS: AFTER:PT:PROCESS_END

CREATE PROCEDURE "TmSp_InventoryTakingReport"
(
IN itemCodes		nvarchar(100),-- := '' ,
IN itmGroupCode	nvarchar(11),-- := 'A',
IN itmProps	   	nvarchar(70),-- := '',
IN slpCodes    	nvarchar(40),-- := '',
IN whsCodes		nvarchar(4000),-- := '' ,
IN whsCodeRanges  nvarchar(40),-- := '' ,
IN binCodes    	nvarchar(500),-- := '',
IN alterSortRange nvarchar(200),-- := '' ,
IN binSublevels   nvarchar(500),-- := '' ,
IN binAttributes	nvarchar(500),-- := '' ,
IN countDateFrom 	nvarchar(20),-- := '19700101',
IN countDateTo	nvarchar(20),-- := '21000101',
IN postDateFrom 	nvarchar(20),-- := '19700101',
IN postDateTo		nvarchar(20),-- := '21000101',
IN userCodes      nvarchar(4000),-- := '' ,
IN employeeCodes	nvarchar(4000),-- := '' ,
IN dspPosting		nvarchar(1),-- := 'Y',
IN dspCounting	nvarchar(1),-- := 'Y',
IN countingStatus nvarchar(1),-- := 'O',
IN notCountSince 	nvarchar(20),--   := '',
IN postDiffSelected 	nvarchar(1),-- := 'N',
IN postDiffPrcnt 	nvarchar(32)-- := ''
) 
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
	V_INT integer := 0;
	BIN_SUBLEVEL_LENGTH integer := 50;
	BIN_ATTR_LENGTH integer := 20;
	BIN_LOC_CODE_LENGTH integer := 228;
	ALT_SORT_CODE_LENGTH integer := 50;
	ITEM_CODE_LENGTH integer := 50;
	SNB_NUMBER_LENGTH integer := 36;
	SNB_ATTR_LENGTH integer := 32;
	WHS_CODE_LENGTH integer := 8;
	CARD_CODE_LENGTH integer := 15;
	
	BIN_SUBLEVEL_COUNT integer := 4;
	BIN_ATTR_COUNT integer := 10;
	
	itemCodeFrom nvarchar(50);
	itemCodeTo nvarchar(50);
	splCodeFrom nvarchar(64);
	splCodeTo nvarchar(64);
		
	whsIncludingFrom nvarchar(8);
	whsIncludingTo nvarchar(8);	
	whsExcludingFrom nvarchar(8);
	whsExcludingTo nvarchar(8);

	binLocCodeFrom nvarchar(228);
	binLocCodeTo nvarchar(228);
	alterSortCodeFrom nvarchar(228);
	alterSortCodeTo nvarchar(228);

	binSbl1From nvarchar(50);
	binSbl1To nvarchar(50);
	binSbl2From nvarchar(50);
	binSbl2To nvarchar(50);
	binSbl3From nvarchar(50);
	binSbl3To nvarchar(50);		
	binSbl4From nvarchar(50);
	binSbl4To nvarchar(50);
		
	binAttr1From nvarchar(20);
	binAttr1To nvarchar(20);
	binAttr2From nvarchar(20);
	binAttr2To nvarchar(20);
	binAttr3From nvarchar(20);
	binAttr3To nvarchar(20);
	binAttr4From nvarchar(20);
	binAttr4To nvarchar(20);
	binAttr5From nvarchar(20);
	binAttr5To nvarchar(20);
	binAttr6From nvarchar(20);
	binAttr6To nvarchar(20);
	binAttr7From nvarchar(20);
	binAttr7To nvarchar(20);
	binAttr8From nvarchar(20);
	binAttr8To nvarchar(20);
	binAttr9From nvarchar(20);
	binAttr9To nvarchar(20);
	binAttr10From nvarchar(20);
	binAttr10To nvarchar(20);
	
	transType nvarchar(1);
	selectStr nvarchar(4000);
	fromStr nvarchar(4000);
	whereCommonStr nvarchar(4000);
	whereBinCommonStr nvarchar(4000);
	whereItemPropStr nvarchar(4000);
	whereCountingStr nvarchar(4000);
	wherePostingStr nvarchar(4000);
	countingSQL nclob;
	postingSQL nclob;
	execSQL nclob;
	nullSQL nvarchar(4000);
	subSQL1 nvarchar(4000);
	subSQL2 nvarchar(4000);
	itwSQL nclob;
	ibqSQL nclob;
	ii integer;
	jj integer;
	typeUSR nvarchar(10) := '12';
	typeHEM nvarchar(10) := '171';
	PropsRel nvarchar(10);
	IsDisplayInactiveItem nvarchar(10);
BEGIN

	IF :slpCodes<> ''''
		THEN
			V_INT := 1;
			splCodeFrom := RTRIM(SUBSTRING(slpCodes, V_INT, CARD_CODE_LENGTH));
			V_INT := :V_INT + CARD_CODE_LENGTH;
			splCodeTo := RTRIM(SUBSTRING(slpCodes, V_INT, CARD_CODE_LENGTH));
		END IF;

	IF :itemCodes<> ''''
		THEN
			V_INT := 1;
			itemCodeFrom := RTRIM(SUBSTRING(itemCodes, V_INT, ITEM_CODE_LENGTH));
			V_INT := V_INT + ITEM_CODE_LENGTH;
			itemCodeTo := RTRIM(SUBSTRING(itemCodes, V_INT, ITEM_CODE_LENGTH));
		END IF;
		
	IF :whsCodeRanges <> ''''
		THEN
			V_INT := 1;
			whsIncludingFrom := RTRIM(SUBSTRING(whsCodeRanges, V_INT, WHS_CODE_LENGTH));
			V_INT := V_INT + WHS_CODE_LENGTH;
			whsIncludingTo := RTRIM(SUBSTRING(whsCodeRanges, V_INT, WHS_CODE_LENGTH));
			V_INT := V_INT + WHS_CODE_LENGTH;
			whsExcludingFrom := RTRIM(SUBSTRING(whsCodeRanges, V_INT, WHS_CODE_LENGTH));
			V_INT := V_INT + WHS_CODE_LENGTH;
			whsExcludingTo := RTRIM(SUBSTRING(whsCodeRanges, V_INT, WHS_CODE_LENGTH));
		END IF;
	
	IF :binCodes <> ''''
		THEN
			V_INT := 1;
			binLocCodeFrom := RTRIM(SUBSTRING(binCodes, V_INT, bin_LOC_CODE_LENGTH));
			V_INT := V_INT + bin_LOC_CODE_LENGTH;
			binLocCodeTo := RTRIM(SUBSTRING(binCodes, V_INT, bin_LOC_CODE_LENGTH));
		END IF;	
	IF :alterSortRange <> ''''
		THEN
			V_INT := 1;
			alterSortCodeFrom := RTRIM(SUBSTRING(alterSortRange, V_INT, ALT_SORT_CODE_LENGTH));
			V_INT := V_INT + ALT_SORT_CODE_LENGTH;
			alterSortCodeTo := RTRIM(SUBSTRING(alterSortRange, V_INT, ALT_SORT_CODE_LENGTH));
		END IF;	
		
	IF :binSublevels <> ''''
		THEN
			V_INT := 1;
			binSbl1From := RTRIM(SUBSTRING(binSublevels, V_INT, bin_SUBLEVEL_LENGTH));
			V_INT := V_INT + bin_SUBLEVEL_LENGTH;
			binSbl1To := RTRIM(SUBSTRING(binSublevels, V_INT, bin_SUBLEVEL_LENGTH));
			V_INT := V_INT + bin_SUBLEVEL_LENGTH;
			binSbl2From := RTRIM(SUBSTRING(binSublevels, V_INT, bin_SUBLEVEL_LENGTH));
			V_INT := V_INT + bin_SUBLEVEL_LENGTH;
			binSbl2To := RTRIM(SUBSTRING(binSublevels, V_INT, bin_SUBLEVEL_LENGTH));
			V_INT := V_INT + bin_SUBLEVEL_LENGTH;
			binSbl3From := RTRIM(SUBSTRING(binSublevels, V_INT, bin_SUBLEVEL_LENGTH));
			V_INT := V_INT + bin_SUBLEVEL_LENGTH;
			binSbl3To := RTRIM(SUBSTRING(binSublevels, V_INT, bin_SUBLEVEL_LENGTH));
			V_INT := V_INT + bin_SUBLEVEL_LENGTH;
			binSbl4From := RTRIM(SUBSTRING(binSublevels, V_INT, bin_SUBLEVEL_LENGTH));
			V_INT := V_INT + bin_SUBLEVEL_LENGTH;
			binSbl4To := RTRIM(SUBSTRING(binSublevels, V_INT, bin_SUBLEVEL_LENGTH));
		END IF;
		
	IF :binAttributes <> ''''
		THEN	
			V_INT := 1;
			binAttr1From := RTRIM(SUBSTRING(binAttributes, V_INT, bin_ATTR_LENGTH));
			V_INT := V_INT + bin_ATTR_LENGTH;
			binAttr1To := RTRIM(SUBSTRING(binAttributes, V_INT, bin_ATTR_LENGTH));
			V_INT := V_INT + bin_ATTR_LENGTH;
			binAttr2From := RTRIM(SUBSTRING(binAttributes, V_INT, bin_ATTR_LENGTH));
			V_INT := V_INT + bin_ATTR_LENGTH;
			binAttr2To := RTRIM(SUBSTRING(binAttributes, V_INT, bin_ATTR_LENGTH));
			V_INT := V_INT + bin_ATTR_LENGTH;
			binAttr3From := RTRIM(SUBSTRING(binAttributes, V_INT, bin_ATTR_LENGTH));
			V_INT := V_INT + bin_ATTR_LENGTH;
			binAttr3To := RTRIM(SUBSTRING(binAttributes, V_INT, bin_ATTR_LENGTH));
			V_INT := V_INT + bin_ATTR_LENGTH;
			binAttr4From := RTRIM(SUBSTRING(binAttributes, V_INT, bin_ATTR_LENGTH));
			V_INT := V_INT + bin_ATTR_LENGTH;
			binAttr4To := RTRIM(SUBSTRING(binAttributes, V_INT, bin_ATTR_LENGTH));
			V_INT := V_INT + bin_ATTR_LENGTH;
			binAttr5From := RTRIM(SUBSTRING(binAttributes, V_INT, bin_ATTR_LENGTH));
			V_INT := V_INT + bin_ATTR_LENGTH;
			binAttr5To := RTRIM(SUBSTRING(binAttributes, V_INT, bin_ATTR_LENGTH));
			V_INT := V_INT + bin_ATTR_LENGTH;
			binAttr6From := RTRIM(SUBSTRING(binAttributes, V_INT, bin_ATTR_LENGTH));
			V_INT := V_INT + bin_ATTR_LENGTH;
			binAttr6To := RTRIM(SUBSTRING(binAttributes, V_INT, bin_ATTR_LENGTH));
			V_INT := V_INT + bin_ATTR_LENGTH;
			binAttr7From := RTRIM(SUBSTRING(binAttributes, V_INT, bin_ATTR_LENGTH));
			V_INT := V_INT + bin_ATTR_LENGTH;
			binAttr7To := RTRIM(SUBSTRING(binAttributes, V_INT, bin_ATTR_LENGTH));
			V_INT := V_INT + bin_ATTR_LENGTH;
			binAttr8From := RTRIM(SUBSTRING(binAttributes, V_INT, bin_ATTR_LENGTH));
			V_INT := V_INT + bin_ATTR_LENGTH;
			binAttr8To := RTRIM(SUBSTRING(binAttributes, V_INT, bin_ATTR_LENGTH));
			V_INT := V_INT + bin_ATTR_LENGTH;
			binAttr9From := RTRIM(SUBSTRING(binAttributes, V_INT, bin_ATTR_LENGTH));
			V_INT := V_INT + bin_ATTR_LENGTH;
			binAttr9To := RTRIM(SUBSTRING(binAttributes, V_INT, bin_ATTR_LENGTH));
			V_INT := V_INT + bin_ATTR_LENGTH;
			binAttr10From := RTRIM(SUBSTRING(binAttributes, V_INT, bin_ATTR_LENGTH));
			V_INT := V_INT + bin_ATTR_LENGTH;
			binAttr10To := RTRIM(SUBSTRING(binAttributes, V_INT, bin_ATTR_LENGTH));
		END IF;
	whereCommonStr := '';
	
	IF :itemCodeFrom <> ''
	THEN
		whereCommonStr := :whereCommonStr || N' and "T0"."ItemCode" >= ''' || :itemCodeFrom || ''' ';
	END IF;
	IF :itemCodeTo <> ''
	THEN
		whereCommonStr := :whereCommonStr || N' and "T0"."ItemCode" <= ''' || :itemCodeTo || ''' ';
	END IF;	
			
	IF :itmGroupCode <> 'A'
	THEN
		whereCommonStr := :whereCommonStr || N' and "TITM"."ItmsGrpCod" >= ''' || :itmGroupCode || ''' ';
	END IF;
	
	IF LENGTH(:itmProps) > 0 
	THEN
		IF (SUBSTRING (:itmProps, 65, 1) = 'N')
		THEN
			PropsRel := ' OR ';
		Else
			PropsRel := ' AND ';
		END IF; 
		whereItemPropStr := '  And ( ';
		ii := 1;
		jj := 1;
		While (:ii <= 64)
		DO
			IF (SUBSTRING (:itmProps, :ii, 1) = 'Y')
			THEN			
				IF (:jj <> 1)
				THEN
					whereItemPropStr := :whereItemPropStr || :PropsRel;
				END IF; 	
				whereItemPropStr := :whereItemPropStr || ' "TITM"."QryGroup' || Cast (ii As Char(2)) || '" = ''Y''';
				jj := :jj + 1;	
			END IF; 
			ii := :ii + 1;
		END WHILE;
		whereItemPropStr := :whereItemPropStr || ') ';
		IF (:whereItemPropStr <> '  And ( ) ') 
		THEN
			whereCommonStr := :whereCommonStr || :whereItemPropStr;
		END IF;
	END IF;

	IF (:itmGroupCode <> 'A')
	THEN
		whereCommonStr := :whereCommonStr || ' And "TITM"."ItmsGrpCod" = ''' || :itmGroupCode || '''';
	END IF;
	
	SELECT "DspFrznITM" into IsDisplayInactiveItem FROM OADM;
	IF (:IsDisplayInactiveItem = 'N') THEN
		whereCommonStr := :whereCommonStr || ' AND (TITM."ItemCode" IS NULL OR (TITM."validFor" = (N''Y'') OR (' 
						|| 'TITM."frozenFor" = (N''Y'') AND (TITM."frozenFrom" IS NOT NULL OR '
						|| 'TITM."frozenTo" IS NOT NULL)) OR (TITM."validFor" = (N''N'') AND '
						|| 'TITM."frozenFor" = (N''N'') ))) ';
	END IF;
	
	IF (:whsCodes <> '')
	THEN
		whereCommonStr := :whereCommonStr || ' AND "T0"."WhsCode" in (' || :whsCodes || ')';
	END IF;

	IF :whsIncludingFrom <> '' 
	THEN
		whereCommonStr := :whereCommonStr || ' AND "T0"."WhsCode" >= ''' || :whsIncludingFrom || '''';
	END IF;
		
	IF :whsIncludingTo <> '' 
	THEN
		whereCommonStr := :whereCommonStr || ' AND "T0"."WhsCode" <= ''' || :whsIncludingTo || '''';
	END IF;
	
	IF :whsExcludingFrom <> '' OR  :whsExcludingTo <> '' 
	THEN
		whereCommonStr := :whereCommonStr || ' AND ( ';
		IF :whsExcludingFrom <> '' 
		THEN
			whereCommonStr := :whereCommonStr || ' "T0"."WhsCode" < ''' || :whsExcludingFrom || '''';
		END IF;
	
		IF :whsExcludingTo <> '' 
		THEN
			IF :whsExcludingFrom <> ''
			THEN 
				whereCommonStr := :whereCommonStr || ' OR ';
			END IF;
			whereCommonStr := :whereCommonStr || ' "T0"."WhsCode" > ''' || :whsExcludingTo || '''';		
		END IF;
		whereCommonStr := :whereCommonStr || ' ) ';
	END IF;
	
	whereBinCommonStr := '';	
	IF :binLocCodeFrom <> '' 
	THEN
		whereBinCommonStr := :whereBinCommonStr || ' AND "TBIN"."BinCode" >= ''' || :binLocCodeFrom || '''';
	END IF;
	IF :binLocCodeTo <> '' 
	THEN
		whereBinCommonStr := :whereBinCommonStr || ' AND "TBIN"."BinCode" <= ''' || :binLocCodeTo || '''';
	END IF;
			
	IF :alterSortCodeFrom <> '' THEN
		whereBinCommonStr := :whereBinCommonStr || ' AND "TBIN"."AltSortCod" >= ''' || :alterSortCodeFrom || '''';
	END IF;
	IF :alterSortCodeTo <> '' THEN
		whereBinCommonStr := :whereBinCommonStr || ' AND "TBIN"."AltSortCod" <= ''' || :alterSortCodeTo || '''';
	END IF;
	
	IF :binSbl1From <> '' THEN
		whereBinCommonStr := :whereBinCommonStr || ' AND "TBIN"."SL1Code" >= ''' || :binSbl1From || '''';
	END IF;
	IF :binSbl1To <> '' THEN
		whereBinCommonStr := :whereBinCommonStr || ' AND "TBIN"."SL1Code" <= ''' || :binSbl1To || '''';
	END IF;
	
	IF :binSbl2From <> '' THEN
		whereBinCommonStr := :whereBinCommonStr || ' AND "TBIN"."SL2Code" >= ''' || :binSbl2From || '''';
	END IF;
	IF :binSbl2To <> '' THEN
		whereBinCommonStr := :whereBinCommonStr || ' AND "TBIN"."SL2Code" <= ''' || :binSbl2To || '''';
	END IF;
	
	IF :binSbl3From <> '' THEN
		whereBinCommonStr := :whereBinCommonStr || ' AND "TBIN"."SL3Code" >= ''' || :binSbl3From || '''';
	END IF;
	IF :binSbl3To <> '' THEN
		whereBinCommonStr := :whereBinCommonStr || ' AND "TBIN"."SL3Code" <= ''' || :binSbl3To || '''';
	END IF;
	
	IF :binSbl4From <> '' THEN
		whereBinCommonStr := :whereBinCommonStr || ' AND "TBIN"."SL4Code" >= ''' || :binSbl4From || '''';
	END IF;
	IF :binSbl4To <> '' THEN
		whereBinCommonStr := :whereBinCommonStr || ' AND "TBIN"."SL4Code" <= ''' || :binSbl4To || '''';
	END IF;
	
	IF :binAttr1From <> '' THEN
		whereBinCommonStr := :whereBinCommonStr || ' AND "TBIN"."Attr1Val" >= ''' || :binAttr1From || '''';
	END IF;
	IF :binAttr1To <> '' THEN
		whereBinCommonStr := :whereBinCommonStr || ' AND "TBIN"."Attr1Val" <= ''' || :binAttr1To || '''';
	END IF;

	IF :binAttr2From <> '' THEN
		whereBinCommonStr := :whereBinCommonStr || ' AND "TBIN"."Attr2Val" >= ''' || :binAttr2From || '''';
	END IF;
	IF :binAttr2To <> '' THEN
		whereBinCommonStr := :whereBinCommonStr || ' AND "TBIN"."Attr2Val" <= ''' || :binAttr2To || '''';
	END IF;
	
	IF :binAttr3From <> '' THEN
		whereBinCommonStr := :whereBinCommonStr || ' AND "TBIN"."Attr3Val" >= ''' || :binAttr3From || '''';
	END IF;
	IF :binAttr3To <> '' THEN
		whereBinCommonStr := :whereBinCommonStr || ' AND "TBIN"."Attr3Val" <= ''' || :binAttr3To || '''';
	END IF;
	
	IF :binAttr4From <> '' THEN
		whereBinCommonStr := :whereBinCommonStr || ' AND "TBIN"."Attr4Val" >= ''' || :binAttr4From || '''';
	END IF;
	IF :binAttr4To <> '' THEN
		whereBinCommonStr := :whereBinCommonStr || ' AND "TBIN"."Attr4Val" <= ''' || :binAttr4To || '''';
	END IF;

	IF :binAttr5From <> '' THEN
		whereBinCommonStr := :whereBinCommonStr || ' AND "TBIN"."Attr5Val" >= ''' || :binAttr5From || '''';
	END IF;
	IF :binAttr5To <> '' THEN
		whereBinCommonStr := :whereBinCommonStr || ' AND "TBIN"."Attr5Val" <= ''' || :binAttr5To || '''';
	END IF;
	
	IF :binAttr6From <> '' THEN
		whereBinCommonStr := :whereBinCommonStr || ' AND "TBIN"."Attr6Val" >= ''' || :binAttr6From || '''';
	END IF;
	IF :binAttr6To <> '' THEN
		whereBinCommonStr := :whereBinCommonStr || ' AND "TBIN"."Attr6Val" <= ''' || :binAttr6To || '''';
	END IF;
	
	IF :binAttr7From <> '' THEN
		whereBinCommonStr := :whereBinCommonStr || ' AND "TBIN"."Attr7Val" >= ''' || :binAttr7From || '''';
	END IF;
	IF :binAttr7To <> '' THEN
		whereBinCommonStr := :whereBinCommonStr || ' AND "TBIN"."Attr7Val" <= ''' || :binAttr7To || '''';
	END IF;
	
	IF :binAttr8From <> '' THEN
		whereBinCommonStr := :whereBinCommonStr || ' AND "TBIN"."Attr8Val" >= ''' || :binAttr8From || '''';
	END IF;
	IF :binAttr8To <> '' THEN
		whereBinCommonStr := :whereBinCommonStr || ' AND "TBIN"."Attr8Val" <= ''' || :binAttr8To || '''';
	END IF;
	
	IF :binAttr9From <> '' THEN
		whereBinCommonStr := :whereBinCommonStr || ' AND "TBIN"."Attr9Val" >= ''' || :binAttr9From || '''';
	END IF;
	IF :binAttr9To <> '' THEN
		whereBinCommonStr := :whereBinCommonStr || ' AND "TBIN"."Attr9Val" <= ''' || :binAttr9To || '''';
	END IF;
	
	IF :binAttr10From <> '' THEN
		whereBinCommonStr := :whereBinCommonStr || ' AND "TBIN"."Attr10Val" >= ''' || :binAttr10From || '''';
	END IF;
	IF :binAttr10To <> '' THEN
		whereBinCommonStr := :whereBinCommonStr || ' AND "TBIN"."Attr10Val" <= ''' || :binAttr10To || '''';
	END IF;

If(:dspCounting = 'Y')
THEN
    transType := 'C';
    selectStr := N'Select ''' || :transType || ''' as TransType, ';
    selectStr := :selectStr ||  N'"T0"."DocEntry", "T1"."DocNum", "T0"."VisOrder" as LineNum, "T0"."ItemCode", "T0"."ItemDesc", "T0"."WhsCode","T0"."BinEntry", "TBIN"."BinCode", "T0"."InWhsQty", "T0"."CountQty","T12"."TotalQty" As "CountQty1", "T14"."TotalQty" As "CountQty2", "T0"."Difference", "T0"."DiffPercen" as "DiffPercnt", NULL as "Price", NULL as "Total", NULL as "Currency", "T0"."CountDate", NULL as "PostDate", "T0"."LineStatus"';
    fromStr := N' from INC1 "T0" join OINC "T1" on "T0"."DocEntry" = "T1"."DocEntry" left outer join OBIN TBIN on "T0"."BinEntry" = "TBIN"."AbsEntry"';
    fromStr := :fromStr || N' left outer join OITM TITM on "T0"."ItemCode"="TITM"."ItemCode"';
    fromStr := :fromStr || N' LEFT OUTER JOIN "INC8" T11 ON T11."DocEntry" = T0."DocEntry" AND T11."VisOrder" = 1';
    fromStr := :fromStr || N' LEFT OUTER JOIN "INC9" T12 ON T12."DocEntry" = T0."DocEntry" AND T12."LineNum" = T0."LineNum" AND T11."CounterNum" = T12."CounterNum"';
    fromStr := :fromStr || N' LEFT OUTER JOIN "INC8" T13 ON T13."DocEntry" = T0."DocEntry" AND T13."VisOrder" = 2';
    fromStr := :fromStr || N' LEFT OUTER JOIN "INC9" T14 ON T14."DocEntry" = T0."DocEntry" AND T14."LineNum" = T0."LineNum" AND T13."CounterNum" = T14."CounterNum"';

    whereCountingStr := N' where "T0"."DocEntry" = "T1"."DocEntry"' || :whereCommonStr || :whereBinCommonStr;

	If (LENGTH(:countDateFrom) > 0)
	THEN
	    whereCountingStr := :whereCountingStr || N' and "T0"."CountDate" >= ''' || :countDateFrom || ''' ';
	END IF;
	If (LENGTH(:countDateTo) > 0)
	THEN
	    whereCountingStr := :whereCountingStr || N' and "T0"."CountDate" <= ''' || :countDateTo || ''' ';
	END IF;

	IF (TRIM(:splCodeFrom) <> '' or TRIM(:splCodeTo) <> '') THEN
        whereCountingStr := :whereCountingStr ||  ' and (';
        
		IF TRIM(:splCodeFrom) <> ''
		THEN
			whereCountingStr := :whereCountingStr || N' "T0"."PrefVendor" >= ''' || :splCodeFrom || ''' ';
		ELSE
			whereCountingStr := :whereCountingStr || N' "T0"."PrefVendor" IS NULL';	
		END IF;
		
		IF (TRIM(:splCodeFrom) <> '' and TRIM(:splCodeTo) <> '') THEN
			whereCountingStr := :whereCountingStr || N' and ';
		ELSE
			whereCountingStr := :whereCountingStr || N' or ';
		END IF;
		
		IF TRIM(:splCodeTo) <> ''
		THEN
			whereCountingStr := :whereCountingStr || N' "T0"."PrefVendor" <= ''' || :splCodeTo || ''' ';
		ELSE
			whereCountingStr := :whereCountingStr || N' "T0"."PrefVendor" IS NULL';	
		END IF;

		whereCountingStr := :whereCountingStr ||  ')';
    END IF;
    
    If ((:userCodes <> '') or (:employeeCodes <> ''))
    THEN
		whereCountingStr := :whereCountingStr || ' and (';
		IF (:userCodes <> '')
		THEN
		    whereCountingStr := :whereCountingStr || N' (';
			whereCountingStr := whereCountingStr || N'   ( "T1"."CountType" =(N''1'') and';	
			whereCountingStr := whereCountingStr || N'     (' || N' ("T1"."Taker1Type"=' || :typeUSR || ' and "T1"."Taker1Id" in (' || :userCodes || ') )';
			IF (:employeeCodes = '') THEN
				whereCountingStr := whereCountingStr || N' OR "T1"."Taker1Type"='  || :typeHEM;
			END IF;
			whereCountingStr := whereCountingStr || N'      )';
			whereCountingStr := whereCountingStr || N'   ) or ';
			
			whereCountingStr := whereCountingStr || N'   (';
			whereCountingStr := whereCountingStr || N'    "T1"."CountType" =(N''2'') and ';
			whereCountingStr := whereCountingStr || N'     (';
			whereCountingStr := whereCountingStr || N'       "T1"."DocEntry" IN ';
			whereCountingStr := whereCountingStr || N'       ( SELECT DISTINCT U0."DocEntry" FROM "INC4" U0 WHERE ';
			whereCountingStr := whereCountingStr || N'         (U0."CounteType" = ' || :typeUSR|| ' AND U0."CounterId" IN (' || :userCodes || '))';
			IF (:employeeCodes = '') THEN
				whereCountingStr := whereCountingStr || N' OR U0."CounteType" = '  || :typeHEM ;
			END IF;
			whereCountingStr := whereCountingStr || N'       )';
			whereCountingStr := whereCountingStr || N'      OR ';
			whereCountingStr := whereCountingStr || N'      "T1"."DocEntry" IN ';
			whereCountingStr := whereCountingStr || N'        ( SELECT DISTINCT U0."DocEntry" FROM "INC8" U0 WHERE ';
			whereCountingStr := whereCountingStr || N'          (U0."CounteType" = ' || :typeUSR|| ' AND U0."CounterId" IN (' || :userCodes || '))';
			IF (:employeeCodes = '') THEN
				whereCountingStr := whereCountingStr || N' OR U0."CounteType" = '  || :typeHEM ;
			END IF;
			whereCountingStr := whereCountingStr || N'        )';
			whereCountingStr := whereCountingStr || N'      )';
			whereCountingStr := whereCountingStr || N'    )';
			whereCountingStr := whereCountingStr || N'  )';
		END IF;
		If ((:userCodes <> '') and (:employeeCodes <> ''))
		THEN
			whereCountingStr := whereCountingStr || ' or ';
		END IF;
		
		If (:employeeCodes <> '')
		THEN
		    whereCountingStr := whereCountingStr || N' (';
			
			whereCountingStr := whereCountingStr || N'   ( "T1"."CountType" =(N''1'')  and';
			whereCountingStr := whereCountingStr || N'     (';
			whereCountingStr := whereCountingStr || N'       ("T1"."Taker1Type"=' || :typeHEM || ' and "T1"."Taker1Id" in (' || :employeeCodes || '))';
			IF (:userCodes = '') THEN
				whereCountingStr := whereCountingStr || N' OR "T1"."Taker1Type"='  || :typeUSR;
			END IF;
			whereCountingStr := whereCountingStr || N'      )';
			whereCountingStr := whereCountingStr || N'   ) or ';

			whereCountingStr := whereCountingStr || N'   (';
			whereCountingStr := whereCountingStr || N'    "T1"."CountType" =(N''2'') and ';
			whereCountingStr := whereCountingStr || N'     (';
			whereCountingStr := whereCountingStr || N'       "T1"."DocEntry" IN ';
			whereCountingStr := whereCountingStr || N'       ( SELECT DISTINCT U0."DocEntry"FROM INC4 U0 WHERE ';
			whereCountingStr := whereCountingStr || N'         (U0."CounteType" = ' || :typeHEM || ' AND U0."CounterId" IN (' || :employeeCodes || '))';
			IF (:userCodes = '') THEN
				whereCountingStr := whereCountingStr || N' OR U0."CounteType" = '  || :typeUSR;
			END IF;
			whereCountingStr := whereCountingStr || N'       )';
			whereCountingStr := whereCountingStr || N'      OR ';
			whereCountingStr := whereCountingStr || N'      "T1"."DocEntry" IN ';
			whereCountingStr := whereCountingStr || N'        ( SELECT DISTINCT U0."DocEntry"FROM INC8 U0 WHERE ';
			whereCountingStr := whereCountingStr || N'          (U0."CounteType" = ' || :typeHEM || ' AND U0."CounterId" IN (' || :employeeCodes || '))';
			IF (:userCodes = '') THEN
				whereCountingStr := whereCountingStr || N' OR U0."CounteType" = '  || :typeUSR;
			END IF;
			whereCountingStr := whereCountingStr || N'        )';
			whereCountingStr := whereCountingStr || N'      )';
			whereCountingStr := whereCountingStr || N'    )';
			whereCountingStr := whereCountingStr || N'  )';


		END IF;
		whereCountingStr := :whereCountingStr || ')';
	END IF;

    If (:countingStatus = 'O' or :countingStatus = 'C')
    THEN
        whereCountingStr := :whereCountingStr || N' and "T0"."LineStatus" = ''' || :countingStatus || ''' ';
    END IF;

    If (LENGTH(:notCountSince) > 0)
    THEN
        whereCountingStr := :whereCountingStr || N' and (';
        whereCountingStr := :whereCountingStr || N' "T0"."Counted" = ''' || 'N' || ''' ';
        whereCountingStr := :whereCountingStr || N' and ';
        whereCountingStr := :whereCountingStr || N' "T0"."CountDate" >= ''' || :notCountSince || ''' ';
        whereCountingStr := :whereCountingStr || N')';
    END IF;

    countingSQL:= :selectStr || :fromStr || :whereCountingStr;
END IF; 

If(:dspPosting = 'Y' or :postDiffSelected = 'Y' )
THEN
    transType := 'P';
    selectStr := N'Select ''' || :transType || ''' as TransType, ';
    selectStr := :selectStr ||  N'"T0"."DocEntry", "T1"."DocNum", "T0"."DocLineNum" as "LineNum", "T0"."ItemCode", "T0"."ItemName" as "ItemDesc", "T0"."WhsCode", "T0"."BinEntry", "TBIN"."BinCode", "T0"."OnHandBef" as "InWhsQty", "T0"."CountQty",NULL AS "CountQty1",NULL AS "CountQty2", "T0"."Quantity" as "Difference", "T0"."DiffPercnt", "T0"."Price", "T0"."DocTotal" as "Total", "T0"."Currency", "T0"."CountDate", "T1"."DocDate" as "PostDate", NULL as "LineStatus"';
    fromStr := N' from IQR1 "T0" join OIQR "T1" on "T0"."DocEntry" = "T1"."DocEntry" left outer join OBIN TBIN on "T0"."BinEntry" = "TBIN"."AbsEntry"';
    fromStr := :fromStr || N' left outer join OITM TITM on "T0"."ItemCode" = "TITM"."ItemCode"';

    wherePostingStr := N' where "T0"."DocEntry" = "T1"."DocEntry"' || :whereCommonStr || :whereBinCommonStr;

	If (LENGTH(:countDateFrom) > 0)
	THEN
	    wherePostingStr := :wherePostingStr || N' and "T0"."CountDate" >= ''' || :countDateFrom || ''' ';
	END IF;
	If (LENGTH(:countDateTo) > 0)
	THEN
	    wherePostingStr := :wherePostingStr || N' and "T0"."CountDate" <= ''' || :countDateTo || ''' ';
	END IF;

	If (LENGTH(:postDateFrom) > 0)
	THEN
	    wherePostingStr := :wherePostingStr || N' and "T1"."DocDate" >= ''' || :postDateFrom || ''' ';
	END IF;
	If (LENGTH(:postDateTo) > 0)
	THEN
	    wherePostingStr := :wherePostingStr || N' and "T1"."DocDate" <= ''' || :postDateTo || ''' ';
	END IF;

	IF (TRIM(:splCodeFrom) <> '' or TRIM(:splCodeTo) <> '') THEN
        wherePostingStr := :wherePostingStr ||  ' and (';
        
		IF TRIM(:splCodeFrom) <> ''
		THEN
			wherePostingStr := :wherePostingStr || N' "T0"."CardCode" >= ''' || :splCodeFrom || ''' ';
		ELSE
			wherePostingStr := :wherePostingStr || N' "T0"."CardCode" IS NULL';	
		END IF;
		
		IF (TRIM(:splCodeFrom) <> '' and TRIM(:splCodeTo) <> '') THEN
			wherePostingStr := :wherePostingStr || N' and ';
		ELSE
			wherePostingStr := :wherePostingStr || N' or ';
		END IF;
		
		IF TRIM(:splCodeTo) <> ''
		THEN
			wherePostingStr := :wherePostingStr || N' "T0"."CardCode" <= ''' || :splCodeTo || ''' ';
		ELSE
			wherePostingStr := :wherePostingStr || N' "T0"."CardCode" IS NULL';	
		END IF;

		wherePostingStr := :wherePostingStr ||  ')';
    END IF;
       
    If (LENGTH(:postDateFrom) > 0)
    THEN
        wherePostingStr := :wherePostingStr || N' and "T1"."DocDate" >= ''' || :postDateFrom || ''' ';
    END IF;
    If (LENGTH(:postDateTo) > 0)
    THEN
        wherePostingStr := :wherePostingStr || N' and "T1"."DocDate" <= ''' || :postDateTo || ''' ';
    END IF;
    If (:postDiffSelected = 'Y' and TRIM(:postDiffPrcnt) <> '')
    THEN
        wherePostingStr := :wherePostingStr || N' and "T0"."DiffPercnt" > cast(''' || :postDiffPrcnt || '''as DECIMAL(21,6))';
    END IF;
    postingSQL:=  :selectStr || :fromStr || :wherePostingStr;
END IF; 

nullSQL := N'Select Cast(NULL as nvarchar(1)) as "TransType", NULL as "DocEntry", NULL as "DocNum", NULL as "LineNum", Cast(NULL as char(50)) as "ItemCode", Cast(NULL as char(100)) as "ItemDesc", Cast(NULL as char(8)) as "WhsCode", NULL as "BinEntry", Cast(NULL as char(228)) as "BinCode", NULL as "InWhsQty", NULL as "CountQty", NULL As "CountQty1", NULL As "CountQty2",NULL as "Difference", NULL as "DiffPercnt", NULL as "Price", NULL as "Total", Cast(NULL as char(20)) as "Currency", Cast(NULL as DateTime) as "CountDate", Cast(NULL as DateTime) as "PostDate",Cast(NULL as nvarchar(1)) as "LineStatus" from dummy';

execSQL := nullSQL;

If(:dspCounting = 'Y' and :dspPosting = 'Y' and :postDiffSelected = 'N' )
THEN
    execSQL := :execSQL || ' union all ' || :countingSQL || ' union all ' || :postingSQL;
    execSQL := :execSQL || 'order by "TransType", "DocNum", "ItemCode", "LineNum"';
ELSEIF (:dspCounting = 'Y' and :postDiffSelected = 'N' )
THEN
    execSQL := :execSQL || ' union all ' || :countingSQL;
    execSQL := :execSQL || 'order by "TransType", "DocNum", "ItemCode", "LineNum"';
ELSEIF(:dspPosting = 'Y' or :postDiffSelected = 'Y' )
THEN
    execSQL := :execSQL || ' union all ' || :postingSQL;
    execSQL := :execSQL || 'order by "TransType", "DocNum", "ItemCode", "LineNum"';
END IF;

If (LENGTH(:notCountSince) > 0)
THEN
    IF (TRIM(:splCodeFrom) <> '' or TRIM(:splCodeTo) <> '') THEN
        whereCommonStr := :whereCommonStr ||  ' and (';
        
		IF (TRIM(:splCodeFrom) <> '')
		THEN
			whereCommonStr := :whereCommonStr || N' "TITM"."CardCode" >= ''' || :splCodeFrom || ''' ';
		ELSE
			whereCommonStr := :whereCommonStr || N' "TITM"."CardCode" IS NULL';	
		END IF;
		
		IF (TRIM(:splCodeFrom) <> '' and TRIM(:splCodeTo) <> '') THEN
			whereCommonStr := :whereCommonStr || N' and ';
		ELSE
			whereCommonStr := :whereCommonStr || N' or ';
		END IF;
		
		IF (TRIM(:splCodeTo) <> '')
		THEN
			whereCommonStr := :whereCommonStr || N' "TITM"."CardCode" <= ''' || :splCodeTo || ''' ';
		ELSE
			whereCommonStr := :whereCommonStr || N' "TITM"."CardCode" IS NULL';	
		END IF;

		whereCommonStr := :whereCommonStr ||  ')';
    END IF;
	
    ------------------------------Load not counted items from oitw that the record is not in INC1 table---------------------------------------------------------------------------------
    selectStr := N'Select NULL as "TransType", NULL as "DocEntry", NULL as "DocNum", NULL as "LineNum", "T0"."ItemCode", "TITM"."ItemName" as ItemDesc, "T0"."WhsCode", NULL as "BinEntry", Cast(NULL as char(228)) as "BinCode", "T0"."OnHand" as "InWhsQty", NULL as "CountQty", NULL as "CountQty1", NULL as "CountQty2", NULL as "Difference", NULL as "DiffPercnt", NULL as "Price", NULL as "Total", Cast(NULL as char(20)) as "Currency", Cast(NULL as DateTime) as "CountDate", Cast(NULL as DateTime) as "PostDate",Cast(NULL as nvarchar(1)) as "LineStatus"';
    fromStr := N' from OITW "T0" join OITM "TITM" on "T0"."ItemCode" = "TITM"."ItemCode" where "T0"."ItemCode" = "TITM"."ItemCode" ';
    subSQL1 := N' and not exists (' ||  N'select "T2"."ItemCode", "T2"."WhsCode" from INC1 "T2" where "T0"."ItemCode" = "T2"."ItemCode" and "T0"."WhsCode" = "T2"."WhsCode" and "T2"."BinEntry" is NULL  GROUP BY "T2"."ItemCode", "T2"."WhsCode"  HAVING Max("T2"."CountDate")>=''' || :notCountSince ||N''')';
    subSQL2 := N' and not exists (' ||  N'select "T4"."ItemCode", "T4"."WhsCode" from IQR1 "T4" where "T0"."ItemCode" = "T4"."ItemCode" and "T0"."WhsCode" = "T4"."WhsCode" and "T4"."BinEntry" is NULL  GROUP BY "T4"."ItemCode", "T4"."WhsCode"  HAVING Max("T4"."CountDate")>=''' || :notCountSince ||N''')';
   
    itwSQL := :selectStr || :fromStr || :whereCommonStr || :subSQL1 || :subSQL2;

    ------------------------------Load not counted items from OIBQ that the warehouse is bin active and the record is not in INC1 table------------------------------------------    
    selectStr := N'Select NULL as "TransType", NULL as "DocEntry", NULL as "DocNum", NULL as "LineNum", "T0"."ItemCode", "TITM"."ItemName" as ItemDesc, "T0"."WhsCode", "TBIN"."AbsEntry" as "BinEntry", "TBIN"."BinCode", "T0"."OnHandQty" as "InWhsQty", NULL as "CountQty", NULL As "CountQty1", NULL As "CountQty2", NULL as "Difference", NULL as "DiffPercnt", NULL as "Price", NULL as "Total", Cast(NULL as char(20)) as "Currency", Cast(NULL as DateTime) as "CountDate", Cast(NULL as DateTime) as "PostDate", Cast(NULL as nvarchar(1)) as "LineStatus"';
    fromStr := N' from OIBQ "T0" join OITM "TITM" on "T0"."ItemCode" = "TITM"."ItemCode" join OBIN "TBIN" on "T0"."BinAbs" = TBIN."AbsEntry"';
    fromStr := :fromStr || N' where "T0"."ItemCode" = "TITM"."ItemCode"';
    subSQL1 := N' and not exists (' ||  N'select "T3"."ItemCode", "T3"."WhsCode" from INC1 "T3" where "T0"."ItemCode" = "T3"."ItemCode" and "T0"."WhsCode" = "T3"."WhsCode" and "T0"."BinAbs" = "T3"."BinEntry"  GROUP BY "T3"."ItemCode", "T3"."WhsCode"  HAVING Max("T3"."CountDate")>=''' || :notCountSince ||N''')';
    subSQL2 := N' and not exists (' ||  N'select "T5"."ItemCode", "T5"."WhsCode" from IQR1 "T5" where "T0"."ItemCode" = "T5"."ItemCode" and "T0"."WhsCode" = "T5"."WhsCode" and "T0"."BinAbs" = "T5"."BinEntry"  GROUP BY "T5"."ItemCode", "T5"."WhsCode"  HAVING Max("T5"."CountDate")>=''' || :notCountSince ||N''')';
    ibqSQL := :selectStr || :fromStr || :whereCommonStr || :whereBinCommonStr || :subSQL1 || :subSQL2;
    execSQL := :nullSQL || ' union all ' || :itwSQL || ' union all ' || :ibqSQL;
    execSQL := :execSQL || ' order by "ItemCode", "WhsCode", "BinCode"';
END IF;
--SELECT :execSQL FROM DUMMY;
EXECUTE IMMEDIATE :execSQL;
END;


