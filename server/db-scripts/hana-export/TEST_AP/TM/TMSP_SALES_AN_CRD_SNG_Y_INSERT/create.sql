-- B1 DEPENDS: AFTER:SP:_TmSp_BootCreateGlobalTempTables AFTER:PT:PROCESS_END AFTER:SP:_TmSp_ValidateSpParam

CREATE procedure TmSp_Sales_An_Crd_Sng_Y_Insert (
in ObjectType 		nChar(3),
in ShowType 		nChar(3),
in DocDateFrom_in 		nvarchar(8),   --= '19700101',
in DocDateTo_in		nvarchar(8),   --= '21000101',
in DocDueDateFrom_in 	nvarchar(8),   	--= '19700101',
in DocDueDateTo_in		nvarchar(8),   	--= '21000101',
in CardCodeFrom		nvarchar(30), 	--= '' ,
in CardCodeTo		nvarchar(30), 	--= '',
in CrdGroupCode  	nvarchar(11), 	--= '',
in ItemCodeFrom		nvarchar(50), 	--= '' ,
in ItemCodeTo		nvarchar(50), 	--= '',
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
in DspFrznITM		nvarchar(1), 		--= 'Y',
in BPLIdSet 		nvarchar(1000), --= '',
in WTaxEngine		nvarchar(1),		-- = 'N',
in CrdIndTotalsBy	nvarchar(1) 		--= 'C'
)
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS

DocDateFrom	 		nvarchar(8);
DocDateTo 			nvarchar(8);
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
Mult 			nvarchar(5);
WTLines			nvarchar(4);
FromStr 		nvarchar(4000);
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

	WhereStr :=' Main."DocDate" Between  TO_DATE( ''' ||  :DocDateFrom || ''', ''YYYYMMDD'' ) And  TO_DATE( ''' ||  :DocDateTo || ''', ''YYYYMMDD'' )  And  Inst."DueDate" Between  TO_DATE( ''' ||  :DocDueDateFrom || ''', ''YYYYMMDD'' )   And  TO_DATE( ''' ||  :DocDueDateTo || ''', ''YYYYMMDD'' ) ';
	
--Card Code range
	if LENGTH(:CardCodeFrom) > 0 then 
		WhereStr := :WhereStr  || '   And    Main."CardCode" >=  N''' ||  :CardCodeFrom || '''  ';
	end if;

	if LENGTH(:CardCodeTo) > 0 then 
		WhereStr := :WhereStr   || '   And    Main."CardCode" <=  N''' ||  :CardCodeTo || '''  ';
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
				 	
				WhereStr := :WhereStr || ' Cards."QryGroup' || Cast (:ii As Char(2)) || '" = ''Y''';
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
		DpmSUM_with_Inst := ' + Sum(Main."DpmAmnt" * Inst."InstPrcnt"/100) ';		
		DpmSUM_SC_with_Inst := ' + Sum(Main."DpmAmntSC" * Inst."InstPrcnt"/100)';		
	end if;

	tmpStr := ' Insert Into "TmpPsar_Sales_An_Crd_Sng_Y" ' ||
	'Select Cards."CardCode", Cards."CardName", ' 
	|| '  Count(Main."DocNum") As TotalDocs, ' ||
	'(Sum(Inst."InsTotal") - Sum(Inst."VatSum") + Sum(IfNull(Main."FoCTax",0) * Inst."InstPrcnt" / 100) - Sum(Inst."TotalExpns") + '
			|| (CASE :WTaxEngine WHEN 'N' THEN '(CASE cate."Category" WHEN ''I'' THEN Sum(Inst."WTSum") ELSE 0 END)'
				ELSE '-Sum(Inst."WTSum")' END)
		|| :DpmSUM_with_Inst  || ' - Sum(Main."RoundDif" * Inst."InstPrcnt"/100)' ||
		' ) * ' || :Mult || ' As Sales,' ||
	    '    Sum(Main."GrosProfit" * Inst."InstPrcnt" / 100)  * ' || :Mult || ' As GrossProfit,' ||
	    '   (Sum(Main."GrosProfit" * Inst."InstPrcnt" / 100)  /  nullif((Sum(Inst."InsTotal") - Sum(Inst."VatSum") + Sum(IfNull(Main."FoCTax",0) * Inst."InstPrcnt" / 100) - Sum(Inst."TotalExpns") + '
			|| (CASE :WTaxEngine WHEN 'N' THEN '(CASE cate."Category" WHEN ''I'' THEN Sum(Inst."WTSum") ELSE 0 END)'
				ELSE '-Sum(Inst."WTSum")' END)
		|| '),0) * 100)  * ' || :Mult || ' as GrossPrcnt, ';
			
	if (:ShowType = 'SNG' or :ShowType = 'SN1') then	
		tmpStr := :tmpStr || '(Select Sum(Distinct(aa."DocTotal"))  - Sum(Distinct(aa."VatSum")) + Sum(Distinct(IfNull(aa."FoCTax",0))) - Sum(Distinct(aa."TotalExpns")) + '
							|| (CASE :WTaxEngine WHEN 'N' THEN '(CASE cate."Category" WHEN ''I'' THEN Sum(Inst."WTSum") ELSE 0 END)'
								ELSE '-Sum(Inst."WTSum")' END)
						|| '- (Sum(Distinct(aa."PaidToDate"))-Sum(Distinct(aa."VatPaid")) - Sum(Distinct(aa."ExpAppl")) + '
							|| (CASE :WTaxEngine WHEN 'N' THEN '(CASE cate."Category" WHEN ''I'' THEN Sum(Inst."WTApplied") ELSE 0 END)'
								ELSE '-Sum(Inst."WTApplied")' END)
						|| ' ) From ' || :Header || ' aa   Where  aa."DocEntry" = Main."DocEntry" And Main."PaidToDate" <> Main."DocTotal")  * ' 
						|| :Mult || '  As TotalOpen, ';
	else
		tmpStr := :tmpStr || ' 0, ';
	end if;
	
	tmpStr := :tmpStr || ' ((Sum(Inst."InsTotalSy") - Sum(Inst."' || :VatSysColName || '") + Sum(IfNull(Main."FoCTaxSC",0) * Inst."InstPrcnt" / 100) - Sum(Inst."TotalExpSC") + '
		|| (CASE :WTaxEngine WHEN 'N' THEN '(CASE cate."Category" WHEN ''I'' THEN Sum(Inst."WTSumSC") ELSE 0 END)'
			ELSE '- Sum(Inst."WTSumSC")' END)
	|| :DpmSUM_SC_with_Inst  || ' - Sum(Main."RoundDifSy" * Inst."InstPrcnt"/100)))  * ' || :Mult 
	|| '   As SalesSys, Sum(Main."GrosProfSy" * Inst."InstPrcnt" / 100)  * ' || :Mult 
	|| '  As GrossProfitSys, ((Sum(Main."GrosProfSy" * Inst."InstPrcnt" / 100)  /  nullif((Sum(Inst."InsTotalSy") - Sum(Inst."' 
	|| :VatSysColName || '") + Sum(IfNull(Main."FoCTaxSC",0) * Inst."InstPrcnt" / 100) - Sum(Inst."TotalExpSC") + '
		|| (CASE :WTaxEngine WHEN 'N' THEN '(CASE cate."Category" WHEN ''I'' THEN Sum(Inst."WTSumSC") ELSE 0 END) '
			ELSE '-Sum(Inst."WTSumSC")' END)
	|| '),0)) * 100)  * ' || :Mult 
	|| ' as GrossPrcntSys,';
		
	if (:ShowType = 'SNG' or :ShowType = 'SN1') then
		tmpStr := :tmpStr || ' (Select Sum(Distinct(aa."DocTotalSy")) - Sum(Distinct(aa."' || :VatSysColName 
			|| '")) + Sum(Distinct(IfNull(Main."FoCTaxSC",0))) - Sum(Distinct(aa."TotalExpSC")) + '
				|| (CASE :WTaxEngine WHEN 'N' THEN '(CASE cate."Category" WHEN ''I'' THEN Sum(Inst."WTSumSC") ELSE 0 END)'
					ELSE '-Sum(Inst."WTSumSC")' END)
			|| '- (Sum(Distinct(aa."PaidSys")) - Sum(Distinct(aa."VatPaidSys")) - Sum(Distinct(aa."ExpApplSC")) + '
				|| (CASE :WTaxEngine WHEN 'N' THEN '(CASE cate."Category" WHEN ''I'' THEN Sum(Inst."WTAppliedS") ELSE 0 END)'
					ELSE '-Sum(Inst."WTAppliedS")' END)
			|| ') From ' || :Header || ' aa  Where  aa."DocEntry" = Main."DocEntry"  And Main."PaidToDate" <> Main."DocTotal")* ' 
			|| :Mult || '  As TotalOpenSys ,';
	else 
		tmpStr := :tmpStr || ' 0 , ';
	end if;	

	tmpStr2 := '' || ' (Select  (Sum(aa."DocTotal") - Sum(aa."VatSum") + Sum(IfNull(aa."FoCTax",0))- Sum(aa."TotalExpns") ' || :DpmSUM || ' - Sum(aa."RoundDif") + '
				|| (CASE :WTaxEngine WHEN 'N' THEN '(CASE cate."Category" WHEN ''I'' THEN Sum(Inst."WTSum") ELSE 0 END)'
					ELSE '-Sum(Inst."WTSum")' END)
			|| ') From ' || :Header || ' aa   Where  aa."DocEntry" = Main."DocEntry" )  * ' || :Mult 
			|| '  As  SalesForPrcnt,	(Select  (Sum(aa."DocTotalSy") - Sum(aa."' || :VatSysColName || '") + Sum(IfNull(aa."FoCTaxSC",0))- Sum(aa."TotalExpSC") + '
				|| (CASE :WTaxEngine WHEN 'N' THEN '(CASE cate."Category" WHEN ''I'' THEN Sum(Inst."WTSumSC") ELSE 0 END)'
					ELSE '-Sum(Inst."WTSumSC")' END)
			|| :DpmSUM_SC || ' - Sum(aa."RoundDifSy") ) From ' 
			|| :Header || ' aa   Where  aa."DocEntry" = Main."DocEntry" )  * ' || :Mult || '  As  SalesForPrcntSys';

	if :CrdIndTotalsBy = 'A' then
		tmpStr2 := :tmpStr2 || ',Agreements."Number" As AgrNo, Agreements."AbsID" As AgrAbsId ';
	elseif :CrdIndTotalsBy = 'C' then
		tmpStr2 := :tmpStr2 || ',null AS AgrNo, null As AgrAbsId ';
	end if;
			
	FromStr := ' From  ' || :Header || ' Main Inner  Join OCRD Cards On Cards."CardCode" = Main."CardCode" Inner  Join OCRG Crdg On Crdg."GroupCode" = Cards."GroupCode" '
			|| ' Inner  Join ' || :InstLines || ' inst On inst."DocEntry" = Main."DocEntry" '	;

	if (:WTaxEngine = 'N') then
		FromStr := :FromStr || ' Left Join "TmpPsar_MinCategory" cate on cate."AbsEntry" = Main."DocEntry" ';
	end if;

	FromStr := :FromStr || 'Left Join OOAT Agreements On Main."AgrNo" = Agreements."AbsID" ';

	if :DspFrznBP = 'N' then
		WhereStr := '(' || :WhereStr || ') And ' || '(Cards."validFor" = ''Y'' or (Cards."frozenFor" = ''Y'' and (Cards."frozenFrom" is not null or Cards."frozenTo" is not null)) or (Cards."validFor" = ''N'' and Cards."frozenFor" = ''N''))';
	end if;
	
	if LENGTH(:BPLIdSet) > 0 then
		WhereStr := :WhereStr || ' And Main."BPLId" in (' || :BPLIdSet || ')';
	end if;
	
	WhereStr := ' Where ' || :WhereStr || ' Group By Cards."CardCode", Cards."CardName", Agreements."Number", Agreements."AbsID", Main."DocEntry", Main."PaidToDate", Main."DocTotal", Main."DocTotalSy", Main."PaidSys" ';

	if (:WTaxEngine = 'N') then
		WhereStr := :WhereStr || ', cate."Category"'; 
	end if;
	 
	if LENGTH(:CrdGroupCode) > 0 then
		WhereStr := :WhereStr || ', Cards."GroupCode" ';
	end if;
	
	if ( LENGTH(:CrdProps) > 0 ) then	
		ii := 1;
		while (:ii <= 64) do		
			if (SUBSTRING (:CrdProps, :ii, 1)  = 'Y') then						
				WhereStr := :WhereStr || ' , Cards."QryGroup' || Cast (:ii As nChar(2)) || '"';
			end if;
			
			ii := :ii + 1;
		end while;
	end if;
	
	if (:WTaxEngine = 'N') then
		
		Delete from "TmpPsar_MinCategory";
		SQ := 'Insert into "TmpPsar_MinCategory" Select "AbsEntry", MIN("Category") from '||:WTLines||' Group by "AbsEntry"';
		EXEC (:SQ);
		
	end if;
	
	EXEC (:tmpStr || :tmpStr2 || :FromStr || :WhereStr);

	-- select 1, (:tmpStr || :tmpStr2 ) from dual 	union all	select 2, ( :FromStr || :WhereStr ) from dual;
end;


