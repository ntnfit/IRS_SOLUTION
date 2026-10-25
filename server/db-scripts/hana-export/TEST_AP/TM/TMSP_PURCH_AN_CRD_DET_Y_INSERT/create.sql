-- B1 DEPENDS: AFTER:SP:_TmSp_BootCreateGlobalTempTables AFTER:PT:PROCESS_END AFTER:SP:_TmSp_ValidateSpParam


CREATE PROCEDURE TmSp_Purch_An_Crd_Det_Y_Insert
(in ObjectType		nChar(3),
in ShowType 		nChar(3),
in DocDateFrom_in 		nvarchar(8),  -- = '19700101',
in DocDateTo_in		nvarchar(8),  -- = '21000101',
in DocDueDateFrom_in 	nvarchar(8),  -- = '19700101',
in DocDueDateTo_in		nvarchar(8),  -- = '21000101',
in CardCodeFrom		nvarchar(30), -- = '' ,
in CardCodeTo		nvarchar(30), -- = '',
in CrdGroupCode  		nvarchar(11), -- = '',
in ItemCodeFrom		nvarchar(100), -- = '' ,
in ItemCodeTo		nvarchar(100), -- = '',
in ItmGroupCode  		nvarchar(11), -- = '',
in ItmProps	   	nvarchar(70), -- = '',
in CrdProps   		nvarchar(70), -- = '',
in SlpCodeFrom		nvarchar(155), -- = '',
in SlpCodeTo		nvarchar(155), -- = '',
in TaxDateFrom_in 		nvarchar(8),   -- = '19700101',
in TaxDateTo_in		nvarchar(8),   -- = '21000101',
in Brand			nvarchar(11), -- ='',			
in PrjCode		nvarchar(20),   -- ='',
in RptType		int, 	     -- = 0,
in cutbyObject		nchar(3), -- = 'NON',
in DspFrznBP		nvarchar(1), -- = 'Y',
in DspFrznITM		nvarchar(1), -- = 'Y',
in BPLIdSet 		nvarchar(1000), 	--= '',
in WTaxEngine	nvarchar(1), -- = 'N',
in FilterByAgrAbsId	nvarchar(11) 	-- '' means do not filter, '0' means filter by is null, 'number > 0' filter by AgrAbsId
)
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
	
	--*With Encryption 

AS

DocDateFrom 		nvarchar(30);
DocDateTo			nvarchar(30);   -- = '21000101',
DocDueDateFrom 		nvarchar(8);   -- = '19700101',
DocDueDateTo		nvarchar(8);   -- = '21000101',
TaxDateFrom 		nvarchar(8);   -- = '19700101',
TaxDateTo			nvarchar(8);   -- = '21000101',


Header 		nvarchar(4);
Lines  		nvarchar(4);
--opObject 		nvarchar(4);
tmpStr 		nvarchar(4000);
PropsRel 		nvarchar(10);
ii 			int;
jj 			int;
GrossBySal 		nChar(1);
WhereStr 		nvarchar(4000);
VatSysColName 		nvarchar(20);
VatSysColName2 	nvarchar(20);
Mult 			nvarchar(5);
InstLines  		nvarchar(4);
WTLines		nvarchar(4);
SQ		nvarchar(1000);
DpmAffectsTotal int;
DpmSUM_with_Inst nvarchar(500);
DpmSUM_SC_with_Inst nvarchar(500);

BEGIN

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

if ((:ShowType = 'SNG' or :ShowType = 'SN1' or :ShowType = 'SN2') OR (:ShowType = 'CAN' AND (:ObjectType = 'RPC' OR :ObjectType = 'RPD'))) then
		Mult := ' 1 ';
	else	
		Mult :=  ' -1 ';
end if;

IF LENGTH(:DocDateFrom) = 0 then
	DocDateFrom :=  '19700101';
end if;

IF LENGTH(:DocDateTo) = 0 then
	DocDateTo := '21000101';
end if;

IF LENGTH(:DocDueDateFrom) = 0 then
	DocDueDateFrom :=  '19700101';
end if;

IF LENGTH(:DocDueDateTo) = 0 then
	DocDueDateTo := '21000101';
end if;

IF LENGTH(:TaxDateFrom) = 0 then
	TaxDateFrom :=  '19700101';
end if;
IF LENGTH(:TaxDateTo) = 0 then
	TaxDateTo := '21000101';
end if;

if (:ObjectType = 'PCH') then
	Header := 'OPCH';
	Lines := 'PCH1';
	InstLines := 'PCH6';
	--opObject := 'RPC';
    WTLines := 'PCH5';
end if;
if (:ObjectType = 'CPI') then
	Header := 'OCPI';
	Lines := 'CPI1';
	InstLines := 'CPI6';
    WTLines := 'CPI5';
end if;
if (:ObjectType = 'CPV') then
	Header := 'OCPV';
	Lines := 'CPV1';
	InstLines := 'CPV6';
	WTLines := 'CPV5';
end if;
if (:ObjectType = 'POR') then
	Header := 'OPOR';
	Lines :=  'POR1';
	InstLines := 'POR6';
	--opObject := 'NON';
    WTLines := 'POR5';
end if;
if (:ObjectType = 'PDN') then
	Header := 'OPDN';
	Lines :=  'PDN1';
	InstLines := 'PDN6';
	--opObject := 'RPD';
    WTLines := 'PDN5';
end if;
if (:ObjectType = 'RPC') then
	Header := 'ORPC';
	Lines :=  'RPC1';
	InstLines := 'RPC6';
    WTLines := 'RPC5';
end if;
if (:ObjectType = 'RPD') then
	Header := 'ORPD';
	Lines :=  'RPD1';
	InstLines := 'RPD6';
    WTLines := 'RPD5';
end if;

SELECT TOP 1 COLUMN_NAME into VatSysColName FROM "PUBLIC"."TABLE_COLUMNS" as A WHERE TABLE_NAME = :Header and POSITION = 57;

WhereStr := ' Main."DocDate" Between   TO_DATE( ''' ||  :DocDateFrom 
		|| ''', ''YYYYMMDD'' )	And  TO_DATE( ''' ||  :DocDateTo 
		|| ''', ''YYYYMMDD'' ) And  Inst."DueDate" Between TO_DATE( ''' 
		||  :DocDueDateFrom || ''', ''YYYYMMDD'' )	And  TO_DATE( ''' 
		||  :DocDueDateTo || ''', ''YYYYMMDD'' ) ';			
		
--Card Code range
if LENGTH (:CardCodeFrom) > 0 then
	WhereStr := :WhereStr  || '   And    Main."CardCode" >=  N''' ||  :CardCodeFrom || '''  ';
end if;
if LENGTH (:CardCodeTo) > 0 then
	WhereStr := :WhereStr   || '   And    Main."CardCode" <=  N''' ||  :CardCodeTo || '''  ';
end if;

--Sales person range
if LENGTH (:SlpCodeFrom) > 0 then
	WhereStr := :WhereStr  || '    And   Slp."SlpName" >=  N''' ||  :SlpCodeFrom || '''  ';
end if;
if LENGTH (:SlpCodeTo) > 0 then
	WhereStr := :WhereStr  || '    And   Slp."SlpName" <=  N''' ||  :SlpCodeTo || '''  ';
end if;

WhereStr := :WhereStr  || '	And	 Main."Instance" = 0	'
		|| ' And Main."TaxDate"  Between  TO_DATE( ''' ||  :TaxDateFrom 
		|| ''', ''YYYYMMDD'' )  And  TO_DATE( ''' ||  :TaxDateTo || ''', ''YYYYMMDD'' )	';

if(:ObjectType='PCH' OR :ObjectType='PDN' OR :ObjectType = 'RPC' OR :ObjectType = 'RPD') then
	if(:ShowType = 'CAN') then
		WhereStr := :WhereStr || 'And	   Main."CANCELED" = ''C'' ';
	else
		WhereStr := :WhereStr || 'And	   Main."CANCELED" <> ''C'' ';
	end if;
else
	if (:ObjectType <> 'CPI') then
		WhereStr := :WhereStr || 'And	   Main."CANCELED" = ''N'' ';
	end if;
end if;

IF LENGTH(:CrdGroupCode) > 0 then
	WhereStr := :WhereStr || ' And  Cards."GroupCode" = ' || :CrdGroupCode;
end if;

if LENGTH(:CrdProps) > 0 then
	if (SUBSTRING (:CrdProps, 65, 1)  = 'N') then
		PropsRel := ' OR ';
	elseif (SUBSTRING (:CrdProps, 65, 1)  <> 'N') then
		PropsRel := ' AND ';
	end if;
	WhereStr := :WhereStr || '  And ( '; 
	ii := 1;
	jj := 1;
	While (:ii <= 64) do		
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


if (:ObjectType = 'RPC') then
	if(:ShowType <> 'CAN') then 
		WhereStr := :WhereStr || ' AND NOT  EXISTS (SELECT 1 FROM ' || :ObjectType || '1 T1 WHERE T1."DocEntry" = Main."DocEntry"   AND T1."BaseType" = 204)';
	else
		WhereStr := :WhereStr || ' AND NOT  EXISTS (SELECT 1 FROM '|| :ObjectType || '1 T1,' || :ObjectType || '1 T2 WHERE T1."DocEntry" = Main."DocEntry"  AND T1."BaseType" = 19 AND T1."BaseEntry" = T2."DocEntry" AND T2."BaseType" = 204) ';
	end if;
end if;

DpmSUM_with_Inst := '';
DpmSUM_SC_with_Inst := '';

if (:DpmAffectsTotal = 1) then 	 	
		DpmSUM_with_Inst := ' + Sum(Main."DpmAmnt" * Inst."InstPrcnt"/100) ';		
		DpmSUM_SC_with_Inst := ' + Sum(Main."DpmAmntSC" * Inst."InstPrcnt"/100) ';		
	end if;

tmpStr := ' Insert Into "TmpPsar_Purch_An_Crd_Det_Y" '||
		'Select Main."DocEntry", Main."DocNum", Main."ObjType", Slp."SlpName", Main."DocDate", Inst."DueDate", Cards."CardCode", Cards."CardName",(Sum(Inst."InsTotal") - Sum(Inst."VatSum") + Sum(IfNull(Main."FoCTax",0) * Inst."InstPrcnt" / 100) - (Inst."TotalExpns") + '
		|| (CASE :WTaxEngine WHEN 'N' THEN '(CASE cate."Category" WHEN ''I'' THEN Sum(Inst."WTSum") ELSE 0 END)'
			ELSE '-Sum(Inst."WTSum")' END) 
		|| :DpmSUM_with_Inst || ' - Sum(Main."RoundDif" * Inst."InstPrcnt"/100)) * ' || :Mult || ' As Sales, ';

if (:ShowType = 'NST') then 
	tmpStr := :tmpStr ||' ((Inst."InsTotal") - (Inst."VatSum") + (IfNull(Main."FoCTax",0) * Inst."InstPrcnt" / 100) - (Inst."TotalExpns") + '
		|| (CASE :WTaxEngine WHEN 'N' THEN '(CASE cate."Category" WHEN ''I'' THEN Sum(Inst."WTSum") ELSE 0 END)'
			ELSE '-Sum(Inst."WTSum")' END)
		|| :DpmSUM_with_Inst || ' - Sum(Main."RoundDif" * Inst."InstPrcnt"/100))  * -1 , ';
ELSE
	 tmpStr := :tmpStr || '( Inst."PaidToDate" - Inst."VatPaid" - Inst."ExpAppl" + '
		|| (CASE :WTaxEngine WHEN 'N' THEN '(CASE cate."Category" WHEN ''I'' THEN Sum(Inst."WTApplied") ELSE 0 END)'
			ELSE '-Sum(Inst."WTApplied")' END)
		|| :DpmSUM_with_Inst || ' - Sum(Main."RoundDif" * Inst."InstPrcnt"/100)) * ' || :Mult || '  As PaidAquit , ';
end	if;

tmpStr := :tmpStr ||'  (Sum(Inst."InsTotalSy") - Sum(Inst."' || :VatSysColName || '") + Sum(IfNull(Main."FoCTaxSC",0) * Inst."InstPrcnt" / 100) - Sum(Inst."TotalExpSC") + '
		|| (CASE :WTaxEngine WHEN 'N' THEN '(CASE cate."Category" WHEN ''I'' THEN Sum(Inst."WTSumSC") ELSE 0 END)'
			ELSE '-Sum(Inst."WTSumSC")' END)
		|| :DpmSUM_SC_with_Inst || ' - Sum(Main."RoundDifSy" * Inst."InstPrcnt"/100)) * ' || :Mult || '  As SalesSys, ';
		
if (ShowType = 'NST') then
	tmpStr := :tmpStr || ' ((Inst."InsTotalSy") - Inst.' || :VatSysColName || ' + (IfNull(Main."FoCTaxSC",0) * Inst."InstPrcnt" / 100) - (Inst."TotalExpSC") + '
	|| (CASE :WTaxEngine WHEN 'N' THEN '(CASE cate."Category" WHEN ''I'' THEN Sum(Inst."WTSumSC") ELSE 0 END)'
		ELSE '-Sum(Inst."WTSumSC")' END)
	|| :DpmSUM_SC_with_Inst || ' - Sum(Main."RoundDifSy" * Inst."InstPrcnt"/100))  * -1 ';
 
ELSE
	 tmpStr := :tmpStr || '( Inst."PaidSys" - Inst."VatPaidSys" - Inst."ExpApplSC" + '
								|| (CASE :WTaxEngine WHEN 'N' THEN '(CASE cate."Category" WHEN ''I'' THEN Sum(Inst."WTAppliedS") ELSE 0 END)'
									ELSE '-Sum(Inst."WTAppliedS") ' END)
								|| :DpmSUM_SC_with_Inst || ' - Sum(Main."RoundDifSy" * Inst."InstPrcnt"/100)) * ' || :Mult || '  As PaidAquit ';
end	if;

tmpStr := :tmpStr || ', Inst."InstlmntID" as instId, Main."Installmnt" as instNum '
	|| ',Agreements."Number" As AgrNo, Agreements."AbsID" As AgrAbsId ';

tmpStr := :tmpStr || ' From  ' || :Header || ' Main	Inner  Join OCRD Cards On Cards."CardCode" = Main."CardCode" '||
	'Inner  Join OSLP slp On Slp."SlpCode" = Main."SlpCode" '||
	'Inner  Join ' || :InstLines || ' Inst On Inst."DocEntry" = Main."DocEntry" ';

	if (:WTaxEngine = 'N') then
		tmpStr := :tmpStr || ' Left Join "TmpPsar_MinCategory" cate on cate."AbsEntry" = Main."DocEntry" ';
	end if;

tmpStr := :tmpStr || 'Left Join OOAT Agreements On Main."AgrNo" = Agreements."AbsID" ';

IF :DspFrznBP = 'N' then
	WhereStr := '(' || :WhereStr || ') And ' || 
					'(Cards."validFor" = ''Y'' or (Cards."frozenFor" = ''Y'' and (Cards."frozenFrom" is not null or Cards."frozenTo" is not null)) or (Cards."validFor" = ''N'' and Cards."frozenFor" = ''N''))';
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

WhereStr := 'Where ' || :WhereStr || '  Group By  Main."DocEntry",'||
				'Main."DocNum", '  ||
				'Main."ObjType", ' ||
				'Cards."CardCode", ' ||
				'Slp."SlpName", '||
				'Agreements."Number", ' ||
				'Agreements."AbsID", ' ||
				'Main."DocDate", '||
				'Inst."DueDate", '||
				'Cards."CardName", '||
				'Inst."VatSum", '||
				'Inst."VatSumSy", '||
				'Inst."PaidSys", '||
				'Inst."PaidToDate", '||
				'Main."VatPercent", '||
				'Inst."InsTotal", '||
				'Main."VatSum", '||
				'Main."PaidSys", '||
				'Main."' || :VatSysColName || '" , '||
				'Inst."InsTotalSy", '||
				'Inst."InstlmntID", '||
				'Main."Installmnt", '||
				'Inst."TotalExpns", '||
				'Inst."TotalExpSC", '||
				'Inst."VatPaid", '||
				'Inst."ExpAppl", '||
				'Inst."WTApplied", '||
				'Inst."VatPaidSys", '||
				'Inst."ExpApplSC", '||
				'Inst."WTAppliedS" ';
				
	if (:WTaxEngine = 'N') then
		WhereStr := :WhereStr || ', cate."Category"'; 
	end if;

IF LENGTH(:CrdGroupCode) > 0 then
	WhereStr := :WhereStr || ', Cards."GroupCode" ';
end if;
IF LENGTH(:CrdProps) > 0 then
	ii := 1;
	While (:ii <= 64) do
		IF (SUBSTRING (:CrdProps, :ii, 1)  = 'Y') then
			WhereStr := :WhereStr || ' , Cards."QryGroup' || Cast (:ii As Char(2))||'"'; 
		end if;
		ii := :ii + 1;
	end while;
end if;

if (:WTaxEngine = 'N') then

	Delete from "TmpPsar_MinCategory";
	SQ := 'Insert into "TmpPsar_MinCategory" Select "AbsEntry", MIN("Category") from '||:WTLines||' Group by "AbsEntry"';
	EXEC (:SQ);
	
end if;

exec (:tmpStr || :WhereStr);
-- select TOP 1 (:tmpStr || :WhereStr) from  dual;

END;


