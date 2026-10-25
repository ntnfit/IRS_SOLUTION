-- B1 DEPENDS: AFTER:SP:_TmSp_BootCreateGlobalTempTables AFTER:PT:PROCESS_END AFTER:SP:_TmSp_ValidateSpParam

CREATE procedure TmSp_Sales_An_Crd_Det_Y_Insert (
in ObjectType 		nChar(3),
in ShowType 		nChar(3),
in DocDateFrom_in 	nvarchar(30),   --= '19700101',
in DocDateTo_in		nvarchar(30),   --= '21000101',
in DocDueDateFrom_in 	nvarchar(8),   	--= '19700101',
in DocDueDateTo_in		nvarchar(8),   	--= '21000101',
in CardCodeFrom		nvarchar(30), 	--= '' ,
in CardCodeTo		nvarchar(30), 	--= '',
in CrdGroupCode  	nvarchar(11), 	--= '',
in ItemCodeFrom		nvarchar(100), 	--= '' ,
in ItemCodeTo		nvarchar(100), 	--= '',
in ItmGroupCode  	nvarchar(11), 	--= '',
in ItmProps	   		nvarchar(70), 	--= '',
in CrdProps   		nvarchar(70 ),	--= '',
in SlpCodeFrom		nvarchar(155), 	--= '',
in SlpCodeTo		nvarchar(155), 	--= '',
in TaxDateFrom_in 		nvarchar(8),   	--= '19700101',
in TaxDateTo_in		nvarchar(8),   	--= '21000101',
in Brand			nvarchar(11), 	--='',			
in PrjCode			nvarchar(20),   --='',
in RptType			int, 	     	--= 0,
in cutbyObject		nchar(3), 		--= 'NON',
in DspFrznBP		nvarchar(1), 		--= 'Y',
in DspFrznITM		nvarchar(1), 		--= 'Y'
in BPLIdSet 		nvarchar(1000),	--= '',
in WTaxEngine		nvarchar(1),
in FilterByAgrAbsId	nvarchar(11) 	-- '' means do not filter, '0' means filter by is null, 'number > 0' filter by AgrAbsId
)
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER

AS

DocDateFrom	 		nvarchar(30);
DocDateTo 			nvarchar(30);
DocDueDateFrom 		nvarchar(8); 
DocDueDateTo 		nvarchar(8);
TaxDateFrom 		nvarchar(8); 
TaxDateTo			nvarchar(8);


Header 			nvarchar(4);
Lines  			nvarchar(4);
InstLines  		nvarchar(4);
--opObject 		nvarchar(4);
tmpStr 			nvarchar(4000);
tmpStr2 		nvarchar(4000); --[i037610 add at 26/05/06 10:36] buffer "tmpStr" is not big enough, so add a new buffer
PropsRel 		nvarchar(10);
ii 				int;
jj 				int;
GrossBySal 		nChar(1);
WhereStr 		nvarchar(4000);
VatSysColName 	nvarchar(20);
ObjectColName 	nvarchar(20);
Mult 			nvarchar(5);
WTLines			nvarchar(4);
SQ			nvarchar(1000);
DpmAffectsTotal		int;
DpmSUM_with_Inst		nvarchar(500);
DpmSUM_SC_with_Inst		nvarchar(500);
DpmSUM		nvarchar(500);
DpmSUM_SC		nvarchar(500);


begin

call _TmSp_ValidateSpParam (:ObjectType);
call _TmSp_ValidateSpParam (:ShowType);
call _TmSp_ValidateSpParam (:DocDateFrom_in);
call _TmSp_ValidateSpParam (:DocDateTo_in);
call _TmSp_ValidateSpParam (:DocDueDateFrom_in);
call _TmSp_ValidateSpParam (:DocDueDateTo_in);
call _TmSp_ValidateSpParam (:CardCodeFrom);
call _TmSp_ValidateSpParam (:CardCodeTo);
call _TmSp_ValidateSpParam (:CrdGroupCode);
call _TmSp_ValidateSpParam (:ItemCodeFrom);
call _TmSp_ValidateSpParam (:ItemCodeTo);
call _TmSp_ValidateSpParam (:ItmGroupCode);
call _TmSp_ValidateSpParam (:ItmProps);
call _TmSp_ValidateSpParam (:CrdProps);
call _TmSp_ValidateSpParam (:SlpCodeFrom);
call _TmSp_ValidateSpParam (:SlpCodeTo);
call _TmSp_ValidateSpParam (:TaxDateFrom_in);
call _TmSp_ValidateSpParam (:TaxDateTo_in);
call _TmSp_ValidateSpParam (:Brand);
call _TmSp_ValidateSpParam (:PrjCode);
call _TmSp_ValidateSpParam (:cutbyObject);
call _TmSp_ValidateSpParam (:DspFrznBP);
call _TmSp_ValidateSpParam (:DspFrznITM);
call _TmSp_ValidateSpParam (:BPLIdSet);
call _TmSp_ValidateSpParam (:WTaxEngine);

	DocDateFrom := DocDateFrom_in;
	DocDateTo := DocDateTo_in;
	DocDueDateFrom := DocDueDateFrom_in;
	DocDueDateTo := DocDueDateTo_in;
	TaxDateFrom :=	TaxDateFrom_in;
	TaxDateTo := TaxDateTo_in;
	
	SELECT CASE WHEN "DpmAffTot" = 'Y' THEN 1 ELSE 0 END into DpmAffectsTotal FROM CINF;
	
	if (:ObjectType = 'NON') then  
		--return (0)
		Mult := :Mult;
	end if;
	
	if ((:ShowType = 'SNG' or :ShowType = 'SN1' or :ShowType = 'SN2') OR (:ShowType = 'CAN' AND (:ObjectType = 'RIN' OR :ObjectType = 'RDN'))) then
		Mult := ' 1 '; 
	else
		Mult :=  ' -1 ';
	end if;
	
	if LENGTH(:DocDateFrom) = 0 then 	
		DocDateFrom := '19700101'; 
	end if;	
	if LENGTH(:DocDateTo) = 0 then 	
		DocDateTo := '21000101';
	end if;	
	if LENGTH(:DocDueDateFrom) = 0 then 	
		DocDueDateFrom :=  '19700101'; 
	end if;	
	if LENGTH(:DocDueDateTo) = 0 then 	
		DocDueDateTo := '21000101';
	end if;	
	if LENGTH(:TaxDateFrom) = 0 then 
		TaxDateFrom :=  '19700101'; 
	end if;
	if LENGTH(:TaxDateTo) = 0 then 	
		TaxDateTo := '21000101';
	end if;
	
	if (:ObjectType = 'INV') then
		Header := 'OINV';
		Lines :=  'INV1';
		InstLines := 'INV6';
		--opObject := 'RIN';
		WTLines := 'INV5';
	end if;
	
	if (:ObjectType = 'CSI') then
		Header := 'OCSI';
		Lines := 'CSI1';
		InstLines := 'CSI6';
        WTLines := 'CSI5';
	end if;
	
	if (:ObjectType = 'CSV') then
		Header := 'OCSV';
		Lines := 'CSV1';
		InstLines := 'CSV6';
        WTLines := 'CSV5';
	end if;
	
	if (:ObjectType = 'RDR') then
		Header := 'ORDR';
		Lines := 'RDR1';
		InstLines := 'RDR6';
		--opObject := 'NON';
		WTLines := 'RDR5';
	end if;

	if (:ObjectType = 'DLN') then
		Header := 'ODLN';
		Lines := 'DLN1';
		InstLines := 'DLN6';
		--opObject := 'RDN';
		WTLines := 'DLN5';
	end if;

	if (:ObjectType = 'RIN') then
		Header := 'ORIN';
		Lines := 'RIN1';
		InstLines := 'RIN6';
		WTLines := 'RIN5';
	end if;

	if (:ObjectType = 'RDN') then
		Header := 'ORDN';
		Lines :=  'RDN1';
		InstLines := 'RDN6';
		WTLines := 'RDN5';
	end if;

-- Comment from original SQL SP 
--[RemoveCIN]
--if (@ObjectType = 'CIN')
--Begin
--	Set @Header = 'OCIN'
--	Set @Lines =  'CIN1'
--	set @InstLines = 'CIN6'
--	Set @WTLines = 'CIN5'
--End


	SELECT TOP 1 COLUMN_NAME into VatSysColName FROM "PUBLIC"."TABLE_COLUMNS" as A WHERE TABLE_NAME = :Header and POSITION = 57;
	SELECT TOP 1 COLUMN_NAME into ObjectColName FROM "PUBLIC"."TABLE_COLUMNS" as A WHERE TABLE_NAME = :Header and POSITION = 10;

	WhereStr :=' Main."DocDate" Between  TO_DATE( ''' ||  :DocDateFrom || ''', ''YYYYMMDD'' ) And  TO_DATE( ''' ||  :DocDateTo || ''', ''YYYYMMDD'' )  And  Inst."DueDate" Between  TO_DATE( ''' ||  :DocDueDateFrom || ''', ''YYYYMMDD'' )   And  TO_DATE( ''' ||  :DocDueDateTo || ''', ''YYYYMMDD'' ) ';
	
--Card Code range
	if LENGTH(:CardCodeFrom) > 0 then 
		WhereStr := :WhereStr  || '   And    Main."CardCode" >=  N''' ||  :CardCodeFrom || '''  ';
	end if;

	if LENGTH(:CardCodeTo) > 0 then 
		WhereStr := :WhereStr   || '   And    Main."CardCode" <=  N''' ||  :CardCodeTo || '''  ';
	end if;
	
	--Sales person range
	if LENGTH(:SlpCodeFrom) > 0 then 	
		WhereStr := :WhereStr  || '    And   Slp."SlpName" >=  N''' ||  :SlpCodeFrom || '''  ';
	end if;
	
	if LENGTH(:SlpCodeTo) > 0 then 	
		WhereStr := :WhereStr  || '    And   Slp."SlpName" <=  N''' ||  :SlpCodeTo || '''  ';
	end if;

	WhereStr := :WhereStr  || '  And 	   Main."Instance" = 0 And	   Main."TaxDate"  Between  TO_DATE( ''' || :TaxDateFrom || ''', ''YYYYMMDD'' )  And  TO_DATE( ''' || :TaxDateTo || ''', ''YYYYMMDD'' )';
	
	if(:ObjectType='INV' OR :ObjectType='DLN' OR :ObjectType = 'RIN' OR :ObjectType = 'RDN') then
		if(:ShowType = 'CAN') then
			WhereStr := :WhereStr || 'And	   Main."CANCELED" = ''C'' '	;
		else
			WhereStr := :WhereStr || 'And	   Main."CANCELED" <> ''C'' '	;
		end if;
	else
		if(:ObjectType <> 'CSI') then
			WhereStr := :WhereStr || 'And	   Main."CANCELED" = ''N'' '	;
		end if;
	end if;

	
	
	if LENGTH(:CrdGroupCode) > 0 then
		WhereStr := :WhereStr || ' And  Cards."GroupCode" = ' || :CrdGroupCode; 
	end if;
							
	if LENGTH(:CrdProps) > 0 then 
		if (SUBSTRING (:CrdProps, 65, 1)  = 'N') then
			PropsRel := ' OR ';
		else 
			PropsRel := ' AND ';
		end if;
		
		WhereStr := :WhereStr || '  And ( ';
		ii := 1;
		jj := 1;
		
		while (:ii <= 64) do
			if (SUBSTRING (:CrdProps, :ii, 1)  = 'Y') then
				if (:jj <> 1) then
					WhereStr := :WhereStr || :PropsRel;
				end if;
				 	
				WhereStr := :WhereStr || ' Cards."QryGroup' || Cast (:ii As nChar(2)) || '" = ''Y''';
				jj := :jj + 1;
			end if;
			 
			ii := :ii + 1;
		end while;
		WhereStr := :WhereStr || ') ';
		 
	end if;
	
	if (:ObjectType = 'RIN') then
		if(:ShowType <> 'CAN') then
			WhereStr := :WhereStr || ' AND NOT  EXISTS (SELECT 1 FROM '|| :ObjectType || '1 T1 WHERE T1."DocEntry" = Main."DocEntry"    AND T1."BaseType" = 203) ';
		else
			WhereStr := :WhereStr || ' AND NOT EXISTS (SELECT 1 FROM ' || :ObjectType || '1 T1 ,' || :ObjectType || '1 T2 WHERE T1."DocEntry" = Main."DocEntry"  AND T1."BaseType" = 14 AND T1."BaseEntry" = T2."DocEntry" AND T2."BaseType" = 203)';
		end if;
	end if;
	
	DpmSUM := '';
	DpmSUM_SC := '';
	DpmSUM_with_Inst := '';
	DpmSUM_SC_with_Inst := '';
	if (:DpmAffectsTotal = 1) then 	 	
		DpmSUM := ' + Sum(aa."DpmAmnt") ';		
		DpmSUM_SC := ' + Sum(aa."DpmAmntSC") ';		
		DpmSUM_with_Inst := ' + Main."DpmAmnt" * Inst."InstPrcnt"/100 ';		
		DpmSUM_SC_with_Inst := ' + Main."DpmAmntSC" * Inst."InstPrcnt"/100';
	end if;


	tmpStr := ' Insert Into "TmpPsar_Sales_An_Crd_Det_Y" ' ||
	'Select Main."DocNum", Slp."SlpName", Main."DocDate", Inst."DueDate", Cards."CardCode", Cards."CardName", '
	|| '((Inst."InsTotal") - (Inst."VatSum") + (IfNull(Main."FoCTax",0) * Inst."InstPrcnt" / 100) - (Inst."TotalExpns")  + '
	|| (CASE :WTaxEngine WHEN 'N' THEN '(CASE cate."Category" WHEN ''I'' THEN Inst."WTSum" ELSE 0 END)'
		ELSE '- Inst."WTSum"' END)
	|| :DpmSUM_with_Inst  || ' - Main."RoundDif" * Inst."InstPrcnt"/100' ||
		' ) * ' || :Mult || ' As Sales,';
		
		
	if (:ShowType = 'NST') then
		tmpStr := :tmpStr || ' ((Main."DocTotal") - (Main."VatSum") + IfNull(Main."FoCTax",0) -  (Main."TotalExpns") + '
			|| (CASE :WTaxEngine WHEN 'N' THEN '(CASE cate."Category" WHEN ''I'' THEN Inst."WTSum" ELSE 0 END)'
				ELSE '-Inst."WTSum"' END)
			|| :DpmSUM_with_Inst  || ' ) * -1 , ';
	else 
	 	tmpStr := :tmpStr || '( Inst."PaidToDate" - Inst."VatPaid" - Inst."ExpAppl" + '
	 	|| (CASE :WTaxEngine WHEN 'N' THEN '(CASE cate."Category" WHEN ''I'' THEN Inst."WTApplied" ELSE 0 END)'
			ELSE '-Inst."WTApplied"' END)
		|| :DpmSUM_with_Inst  || ' ) * ' || :Mult || '  As PaidAquit ,';
	end if;		 
		
	tmpStr := :tmpStr || ' (Main."GrosProfit" * Inst."InstPrcnt" / 100 )* ' || :Mult 
		|| ' As GrossProfit ,(((Main."GrosProfit" * Inst."InstPrcnt" / 100 )  / nullif((Inst."InsTotal" - Inst."VatSum" + (IfNull(Main."FoCTax",0) * Inst."InstPrcnt" / 100) '
		|| ' - Inst."TotalExpns" - (Main."RoundDif" * Inst."InstPrcnt" / 100 ) + '
		|| (CASE :WTaxEngine WHEN 'N' THEN '(CASE cate."Category" WHEN ''I'' THEN Inst."WTSum" ELSE 0 END)'
			ELSE '-Inst."WTSum"' END)
		|| ' ),0)) * 100)  * ' || :Mult || ' as GrossPrcnt, ((Inst."InsTotalSy") '
		|| ' - (Inst."' || :VatSysColName || '") + (IfNull(Main."FoCTaxSC",0) * Inst."InstPrcnt" / 100) - (Inst."TotalExpSC") + '
		|| (CASE :WTaxEngine WHEN 'N' THEN '(CASE cate."Category" WHEN ''I'' THEN Inst."WTSumSC" ELSE 0 END)'
			ELSE '-Inst."WTSumSC"' END)
		|| :DpmSUM_SC_with_Inst  || ' - Main."RoundDifSy" * Inst."InstPrcnt"/100 )  * '
		|| :Mult || '  As SalesSys,';

	if (:ShowType = 'NST') then
		tmpStr := :tmpStr || ' ((Main."DocTotalSy") - Main."' || :VatSysColName || ' + IfNull(Main."FoCTaxSC",0) - (Main."TotalExpSC")  + '
			|| (CASE :WTaxEngine WHEN 'N' THEN '(CASE cate."Category" WHEN ''I'' THEN Inst."WTSumSC" ELSE 0 END)'
				ELSE '-Inst."WTSumSC"' END)
			|| :DpmSUM_SC_with_Inst  || ')  * -1 ,'; 
	else 	
	 	tmpStr := :tmpStr || '( Inst."PaidSys" - Inst."VatPaidSys" - Inst."ExpApplSC" + '
	 		|| (CASE :WTaxEngine WHEN 'N' THEN '(CASE cate."Category" WHEN ''I'' THEN Inst."WTAppliedS" ELSE 0 END)'
				ELSE '-Inst."WTAppliedS"' END)
			|| :DpmSUM_SC_with_Inst  || ' ) * ' || :Mult 
	 		|| '  As PaidAquit, ';	 		
	end if;

	tmpStr2 := ' (Main."GrosProfSy" * Inst."InstPrcnt" / 100 ) * ' || :Mult || '   As GrossProfitSys , '
		|| '(((Main."GrosProfSy" * Inst."InstPrcnt" / 100 )  / nullif((Inst."InsTotalSy" - Inst."' || :VatSysColName 
		|| '" + (IfNull(Main."FoCTaxSC",0) * Inst."InstPrcnt" / 100) - Inst."TotalExpSC" ),0)) * 100) * ' || :Mult || ' as GrossPrcntSys, Inst."DocEntry" As ABS, Inst."' || :ObjectColName || '",';
		
	tmpStr2 := :tmpStr2 || ' ((Select  (Sum(aa."DocTotal") - Sum(aa."VatSum") + Sum(IfNull(aa."FoCTax",0)) - Sum(aa."TotalExpns") + '
			|| (CASE :WTaxEngine WHEN 'N' THEN '(CASE cate."Category" WHEN ''I'' THEN Inst."WTSum" ELSE 0 END)'
				ELSE '-Inst."WTSum"' END)
			|| :DpmSUM || ' - Sum(aa."RoundDif")' ||
			') From ' || :Header || ' aa   Where  aa."DocEntry" = Main."DocEntry" )  * Inst."InstPrcnt" / 100 ) * ' || :Mult 
			|| '  As  SalesForPrcnt,	((Select  (Sum(aa."DocTotalSy") - Sum(aa."' || :VatSysColName || '") + Sum(IfNull(aa."FoCTaxSC",0)) - Sum(aa."TotalExpSC") + '
			|| (CASE :WTaxEngine WHEN 'N' THEN '(CASE cate."Category" WHEN ''I'' THEN Inst."WTSumSC" ELSE 0 END)'
				ELSE '-Inst."WTSumSC"' END)
			|| :DpmSUM_SC || ' - Sum(aa."RoundDifSy") ) From ' 
			|| :Header || ' aa   Where  aa."DocEntry" = Main."DocEntry" )  * Inst."InstPrcnt" / 100 )  * ' || :Mult 
			|| '  As  SalesForPrcntSys, inst."InstlmntID" as instId, main."Installmnt" as instNum '
			|| ',Agreements."Number" As AgrNo, Agreements."AbsID" As AgrAbsId ';
			
	tmpStr2 := :tmpStr2 || ' From  ' || :Header || ' Main Inner  Join OCRD Cards On Cards."CardCode" = Main."CardCode" '
		|| ' Inner Join OSLP slp On Slp."SlpCode" = Main."SlpCode" '
		|| ' Inner Join ' || :InstLines || ' inst On inst."DocEntry" = Main."DocEntry"';
		
	if (:WTaxEngine = 'N') then
		tmpStr2 := :tmpStr2 || ' Left Join "TmpPsar_MinCategory" cate on cate."AbsEntry" = Main."DocEntry" ';
	end if;
	
	tmpStr2 := :tmpStr2 || 'Left Join OOAT Agreements On Main."AgrNo" = Agreements."AbsID" ';

	if :DspFrznBP = 'N' then
		WhereStr := '(' || :WhereStr || ') And ' || '(Cards."validFor" = ''Y'' or (Cards."frozenFor" = ''Y'' and '
			|| '(Cards."frozenFrom" is not null or Cards."frozenTo" is not null)) or (Cards."validFor" = ''N'' and Cards."frozenFor" = ''N''))';
	end if;
	
	if LENGTH(:BPLIdSet) > 0 then
		WhereStr := :WhereStr || ' And Main."BPLId" in (' || :BPLIdSet || ')';
	end if;

	if LENGTH(:FilterByAgrAbsId) > 0 then
		if :FilterByAgrAbsId = '0' then
			WhereStr := :WhereStr || ' And Main."AgrNo" is null';
		else
			WhereStr := :WhereStr || ' AND Main."AgrNo" = ' || :FilterByAgrAbsId;
		end if;
	end if;
	
	WhereStr := ' Where ' || :WhereStr;
	
	if (:WTaxEngine = 'N') then
		Delete from "TmpPsar_MinCategory";
		SQ := 'Insert into "TmpPsar_MinCategory" Select "AbsEntry", MIN("Category") from '||:WTLines||' Group by "AbsEntry"';
		EXEC (:SQ);
  	end if;
	
	EXEC (:tmpStr || :tmpStr2 || :WhereStr);
	--select (:tmpStr || :tmpStr2 || :WhereStr) from dual;
		
end;


