-- B1 DEPENDS: AFTER:SP:_TmSp_BootCreateGlobalTempTables AFTER:PT:PROCESS_END AFTER:SP:_TmSp_ValidateSpParam

CREATE PROCEDURE TmSp_Purch_An_Itm_Sng_M_Insert
(
in ObjectType 		nChar(3),
in ShowType 		nChar(3),
in DocDateFrom_in 		nvarchar(8),   --= '19700101',
in DocDateTo_in		nvarchar(8),   --= '21000101',
in DocDueDateFrom_in 	nvarchar(8),   --= '19700101',
in DocDueDateTo_in		nvarchar(8),   --= '21000101',
in CardCodeFrom		nvarchar(30), --= '' ,
in CardCodeTo		nvarchar(30), --= '',
in CrdGroupCode  		nvarchar(11), --= '',
in ItemCodeFrom		nvarchar(100), --= '' ,
in ItemCodeTo		nvarchar(100), --= '',
in ItmGroupCode  		nvarchar(11), --= '',
in ItmProps	   	nvarchar(70), --= '',
in CrdProps   		nvarchar(70 ), --= '',
in SlpCodeFrom		nvarchar(155), --= '',
in SlpCodeTo		nvarchar(155), --= '',
in TaxDateFrom_in 		nvarchar(8),   --= '19700101',
in TaxDateTo_in		nvarchar(8),   --= '21000101',
in Brand			nvarchar(11), --='',			
in PrjCode		nvarchar(20),   --='',
in RptType		int, 	     --= 0,
in cutbyObject		nchar(3), --= 'NON',
in DspFrznBP		nvarchar(1), --= 'Y',
in DspFrznITM		nvarchar(1), --= 'Y'
in BPLIdSet 		nvarchar(1000), 	--= ''
in WTaxEngine	nvarchar(1) -- = 'N'
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
WTLines		nvarchar(4);
--opObject 		nvarchar(4);
tmpStr 		nvarchar(4000);
PropsRel 		nvarchar(10);
TotalDiscPerc	nvarchar(1000);
TotalDiscPercSC nvarchar(1000);
ii 			int;
jj 			int;
GrossBySal 		nChar(1);
WhereStr 		nvarchar(4000);
DiscSumSysColName 	nvarchar(20);
SlpCodeColName 	nvarchar(20);
VatSysColName 		nvarchar(20);
QtyFldName 		nvarchar(100);
Mult 			nvarchar(5);
DpmAffectsTotal		int;
DpmSUM		nvarchar(500);
DpmSUM_SC		nvarchar(500);


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
if :ObjectType = 'NON' then	
	--* return (0)		
	Mult := Mult; --* empty statement 
end if;

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
	WTLines := :ObjectType || '5';
	--opObject := 'RPC';
end if;

if (:ObjectType = 'CPI') then
	Header := 'OCPI';
	Lines := 'CPI1';
	WTLines := :ObjectType || '5';
end if;

if (:ObjectType = 'CPV') then
	Header := 'OCPV';
	Lines := 'CPV1';
	WTLines := :ObjectType || '5';
end if;

if (:ObjectType = 'POR') then
	Header := 'OPOR';
	Lines :=  'POR1';
	WTLines := :ObjectType || '5';
	--opObject := 'NON';
end if;

if (:ObjectType = 'PDN') then
	Header := 'OPDN';
	Lines :=  'PDN1';
	WTLines := :ObjectType || '5';
	--opObject := 'RPD';
end if;

if (:ObjectType = 'RPC') then
	Header := 'ORPC';
	Lines :=  'RPC1';
	WTLines := :ObjectType || '5';
end if;

if (:ObjectType = 'RPD') then
	Header := 'ORPD';
	Lines :=  'RPD1';
	WTLines := :ObjectType || '5';
end if;

SELECT TOP 1 COLUMN_NAME into DiscSumSysColName FROM "PUBLIC"."TABLE_COLUMNS" as A WHERE TABLE_NAME = :Header and POSITION = 58;
SELECT TOP 1 COLUMN_NAME into VatSysColName FROM "PUBLIC"."TABLE_COLUMNS" as A WHERE TABLE_NAME = :Header and POSITION = 57;
SELECT TOP 1 COLUMN_NAME into SlpCodeColName FROM "PUBLIC"."TABLE_COLUMNS" as A WHERE TABLE_NAME = :Lines and POSITION = 26;

WhereStr := ' Main."DocDate" Between   TO_DATE( ''' ||  :DocDateFrom 
		|| ''', ''YYYYMMDD'' )	And  TO_DATE( ''' ||  :DocDateTo 
		|| ''', ''YYYYMMDD'' ) And  Main."DocDueDate" Between TO_DATE( ''' 
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
if LENGTH(:SlpCodeFrom) > 0 then
		WhereStr := :WhereStr  || '    And   Slp."SlpName" >=  N''' ||  :SlpCodeFrom || '''  ';
end if;
if LENGTH(:SlpCodeTo) > 0 then	
		WhereStr := :WhereStr  || '    And   Slp."SlpName" <=  N''' ||  :SlpCodeTo || '''  ';
end if;
--Item Code range
if LENGTH (:ItemCodeFrom) > 0 then 
WhereStr := :WhereStr  || '    And    Items."ItemCode" >=  N''' ||  :ItemCodeFrom || '''  ';
end if;

if LENGTH (:ItemCodeTo) > 0 then 
WhereStr := :WhereStr  || '    And    Items."ItemCode" <=  N''' ||  :ItemCodeTo || '''  ';
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


if LENGTH(:CrdGroupCode) > 0 then 	
		WhereStr := :WhereStr || ' And  Cards."GroupCode" = ' || :CrdGroupCode; 
end if;
							
if LENGTH(:CrdProps) > 0 then
	if (SUBSTRING (:CrdProps, 65, 1)  = 'N') then		
		PropsRel := ' OR ';		
	Else		
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
	if(:jj = 1) then  
		WhereStr := WhereStr || ' 1 = 1 ';	/* fill empty brackets */ 
	end if;
	WhereStr := :WhereStr || ') '; 
end if;

IF LENGTH(:ItmProps) > 0 then 
	IF (SUBSTRING (:ItmProps, 65, 1)  = 'N') then
		PropsRel := ' OR ';
	Else
		PropsRel := ' AND ';
	End if;
	WhereStr := :WhereStr || '  And ( ';
	ii := 1;
	jj := 1;
	While (:ii <= 64) do
		IF (SUBSTRING (:ItmProps, :ii, 1)  = 'Y') then
			if (:jj <> 1) then
				WhereStr := :WhereStr || :PropsRel;
			end if;
			WhereStr := :WhereStr || ' Items."QryGroup' || Cast (:ii As nChar(2)) || '" = ''Y''';
			jj := :jj + 1;
		end if; 
			ii := :ii + 1;
	end while;
	if(:jj = 1) then  
		WhereStr := WhereStr || ' 1 = 1 ';	/* fill empty brackets */ 
	end if;
	WhereStr := :WhereStr || ') '; 
end if;

IF LENGTH(:ItmGroupCode) > 0 then 
WhereStr := :WhereStr || ' And  ItmGrp."ItmsGrpCod" = ' || :ItmGroupCode; 
end if;

if (:ObjectType = 'RPC') then
	if(:ShowType <> 'CAN') then
		WhereStr := :WhereStr ||
	 '
	  AND NOT  EXISTS (SELECT 1 FROM '|| :ObjectType || '1 T1 WHERE T1."DocEntry" = Main."DocEntry" 
	  AND T1."BaseType" = 204)
	 ';
	else
		WhereStr := :WhereStr ||
	 '
	  AND NOT EXISTS (SELECT 1 FROM '|| :ObjectType || '1 T1 ,' || :ObjectType || '1 T2 WHERE T1."DocEntry" = Main."DocEntry"  AND T1."BaseType" = 19 AND T1."BaseEntry" = T2."DocEntry" AND T2."BaseType" = 204)
	 ';
	end if;
end if;

DpmSUM := '';
DpmSUM_SC := '';
if (:DpmAffectsTotal = 1) then 	 	
		DpmSUM := ' + Sum(Distinct(Main."DpmAmnt")) ';		
		DpmSUM_SC := ' + Sum(Distinct(Main."DpmAmntSC")) ';		
	end if;

	
TotalDiscPerc := '(case (Sum(Distinct(Main."DocTotal")) ' || :DpmSUM || ' + IFNULL(WT."WTSum",0.0) -  Sum(Distinct(Main."VatSum")) + Sum(Distinct(IfNull(Main."FoCTax",0))) + Sum(Distinct(Main."DiscSum"))) '||
						   'when 0 '||
						   'then 0 '||
						   'else (Sum(Distinct(Main."DiscSum"))  /  nullif((Sum(Distinct(Main."DocTotal")) ' || :DpmSUM || ' + IFNULL(WT."WTSum",0.0) -  Sum(Distinct(Main."VatSum")) + Sum(Distinct(IfNull(Main."FoCTax",0))) + Sum(Distinct(Main."DiscSum"))),0)) end) ';
TotalDiscPercSC := '(case (Sum(Distinct(Main."DocTotalSy")) ' || :DpmSUM_SC || ' + IFNULL(WT."WTSumSC",0.0) -  Sum(Distinct(Main."' || :VatSysColName || '")) + Sum(Distinct(IfNull(Main."FoCTaxSC",0))) + Sum(Distinct(Main."' || :DiscSumSysColName || '"))) '||
							 'when 0 '||
							 'then 0 '||
							 'else (Sum(Distinct(Main."' || :DiscSumSysColName || '"))  /  nullif((Sum(Distinct(Main."DocTotalSy")) ' || :DpmSUM_SC || ' + IFNULL(WT."WTSumSC",0.0) - Sum(Distinct(Main."' || :VatSysColName || '")) + Sum(Distinct(IfNull(Main."FoCTaxSC",0))) + Sum(Distinct(Main."' || :DiscSumSysColName || '"))),0)) end) ';
	
if ((:ObjectType = 'PCH') OR (:ObjectType = 'RPC') or (:ObjectType = 'CPI') or (:ObjectType = 'CPV')) then
	QtyFldName := ' Lines."Quantity"  ';
else
	if (:ObjectType = 'PDN') then
		QtyFldName := ' ( case when Lines."BaseType" in (18,163) then 0 else ( Lines."OpenCreQty" ) end )';
	else
		QtyFldName := ' Lines."OpenCreQty" ';
	end if;
end if;
	
tmpStr := 'Insert into "TmpPsar_Purch_An_Itm_Sng_M" Select '||
			'Main."DocEntry", '||
			'Main."DocNum", '||
			'Slp."SlpCode", '||
			'Slp."SlpName", '||
			'Cards."CardCode", '||
			'Cards."CardName", '||
			'Main."DocDate", '||
			'Main."DocDueDate", '||
		    'Items."ItemCode", ' || 
			'Items."ItemName", '||
			
			'case Lines."NoInvtryMv" '|| 
				'when ''Y'' '||
				'then 0 '|| 
				'else case Lines."UseBaseUn" '||
						'when ''Y'' '||
						'then  ' || :QtyFldName || ' * ' || :Mult ||
						' else (' || :QtyFldName || ' * Lines."NumPerMsr")  * ' || :Mult || '	end end as Quantity, '|| 
	
			'case when Lines."TaxOnly" = ''Y'' OR Lines."FreeChrgBP" = ''Y'' '||
				'then 0 '||
				'else ( (((cast (Sum( Lines."LineTotal") as DECIMAL(21,6)) / nullif(Lines."Quantity",0))  / nullif(Items."NumInSale",0))  * (' || :QtyFldName || ' * Items."NumInSale"))  - '||
			 '(   (((cast (Sum( Lines."LineTotal") as DECIMAL(21,6)) / nullif(Lines."Quantity",0))  / nullif(Items."NumInSale",0))  * (' || :QtyFldName || ' * Items."NumInSale")) * '|| 
 	         '(' || :TotalDiscPerc || ' * 100)  / 100))  * ' || :Mult || '	end as Sales, '||

		
		/*System Curr*/ 
		
			'case when Lines."TaxOnly" = ''Y'' OR Lines."FreeChrgBP" = ''Y'' '||
				'then 0 '||
				'else ( (((cast (Sum( Lines."TotalSumSy") as DECIMAL(21,6)) / nullif(Lines."Quantity",0))  / nullif(Items."NumInSale",0))  * (' || :QtyFldName || ' * Items."NumInSale"))  - '||
			 '(   (((cast (Sum( Lines."TotalSumSy") as DECIMAL(21,6)) / nullif(Lines."Quantity",0))  / nullif(Items."NumInSale",0))  * (' || :QtyFldName || ' * Items."NumInSale")) * '|| 
 		     '(' || :TotalDiscPercSC || ' * 100)  / 100))  * ' || :Mult || ' end as SalesSys, '||
		
			'Lines."LineNum"';
tmpStr := :tmpStr || 
	' From  ' || :Header || ' Main '|| 
    	'Inner Join ' || :Lines || ' Lines On Main."DocEntry" = Lines."DocEntry" '||
    	'Inner Join OITM Items On Items."ItemCode" = Lines."ItemCode" '||
    	'Inner Join OITB ItmGrp On ItmGrp."ItmsGrpCod" = Items."ItmsGrpCod" '||
    	'Inner Join OCRD Cards On Cards."CardCode" = Main."CardCode"';
if (:RptType = 0) then
	tmpStr := :tmpStr || ' Inner Join OSLP slp On Slp."SlpCode" = Main."SlpCode" ';
end if;

if (:RptType = 1) then
	tmpStr := :tmpStr || ' Inner Join OSLP slp On Slp."SlpCode" = Lines."' || :SlpCodeColName ||'"'; 
end if;

if (:WTaxEngine = 'N') then
	tmpStr := :tmpStr || ' LEFT OUTER JOIN (select WT0."DocEntry", WT0."WTSum", WT0."WTSumSC" from ' || :Header 
	|| ' WT0 INNER JOIN ' || :WTLines || ' WT1 on WT1."AbsEntry" = WT0."DocEntry" where WT1."Category" = ''I'' '
	|| ' GROUP BY WT0."DocEntry", WT0."WTSum", WT0."WTSumSC") WT on WT."DocEntry" = Main."DocEntry" ';
else
	tmpStr := :tmpStr || ' LEFT OUTER JOIN (select WT0."DocEntry", WT0."WTSum", WT0."WTSumSC" from ' || :Header 
	|| ' WT0 GROUP BY WT0."DocEntry", WT0."WTSum", WT0."WTSumSC") WT on WT."DocEntry" = Main."DocEntry" ';
end if;

if :DspFrznBP = 'N' then
WhereStr := '(' || :WhereStr || ') And ' || 
					'(Cards."validFor" = ''Y'' or (Cards."frozenFor" = ''Y'' and (Cards."frozenFrom" is not null or Cards."frozenTo" is not null)) or (Cards."validFor" = ''N'' and Cards."frozenFor" = ''N''))';
end if;

if :DspFrznITM = 'N' then	
		WhereStr := ' (' || :WhereStr || ') And ' || 
					'(Items."validFor" = ''Y'' or (Items."frozenFor" = ''Y'' and ' ||
					'(Items."frozenFrom" is not null or Items."frozenTo" is not null))' ||
					' or (Items."validFor" = ''N'' and Items."frozenFor" = ''N''))';
end if;

	if LENGTH(:BPLIdSet) > 0 then
		WhereStr := :WhereStr || ' And Main."BPLId" in (' || :BPLIdSet || ')';
	end if;

WhereStr :=  
'Where ' || :WhereStr || '  Group BY '||
	 'Main."DocEntry", '|| 
	 'Slp."SlpCode", '||
	 'Slp."SlpName", '||
	 'Cards."CardCode", '||
	 'Cards."CardName", '||
	 'Main."DocEntry", '||
	 'Main."DocNum", '||
	 'Main."DocDate", '||
	 'Items."ItemCode",  ' ||
	 'Items."ItemName", ' ||
	 'Lines."TaxOnly", '||
	 'Lines."FreeChrgBP", '||
	 'Lines."Price", '||
	 'Main."DocDueDate", '||
	 'Lines."GrossBuyPr", '||
	 'Lines."Quantity", '||
	 'Main."SysRate", '||
	 'Lines."LineNum", '||
	 'Items."NumInSale", '||
	 'Lines."NumPerMsr", '||
	 'Lines."OpenCreQty", '||
	 'Lines."UseBaseUn", '||
	 'WT."WTSum", '||
	 'WT."WTSumSC", '||
	 'Lines."BaseType", '||
	 'Lines."NoInvtryMv"' ;
	 
IF LENGTH(:CrdGroupCode) > 0 then 
	WhereStr := :WhereStr|| ', Cards."GroupCode" ';
END if;

if LENGTH(:ItmGroupCode) > 0 then 
	WhereStr := :WhereStr || ', ItmGrp."ItmsGrpCod" ';
end if;

WhereStr := :WhereStr || ' Order By Items."ItemCode", Main."DocEntry", Lines."LineNum" ';

 exec (:tmpStr || :WhereStr);
-- select TOP 1 (:tmpStr || :WhereStr) from  AACP;
		 /* End Here */

--print @tmpStr
--print @WhereStr



/* SBO_APA_DEV : TaxOnly */
END;


