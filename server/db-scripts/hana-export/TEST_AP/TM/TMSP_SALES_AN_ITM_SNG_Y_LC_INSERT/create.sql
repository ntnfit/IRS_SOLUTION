-- B1 DEPENDS: AFTER:PT:PROCESS_END AFTER:SP:_TmSp_BootCreateGlobalTempTables2 AFTER:SP:_TmSp_ValidateSpParam

CREATE procedure TmSp_Sales_An_Itm_Sng_Y_LC_Insert (
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
in DspFrznITM		nvarchar(1) ,		--= 'Y',
in BPLIdSet 		nvarchar(1000),	--= ''
in WTaxEngine		nvarchar(1)
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

MultOrDivSysRate nvarchar(4);
Header 			nvarchar(4);
Lines  			nvarchar(255);
WTLines  		nvarchar(4);
--opObject 		nvarchar(4);
divideStr nvarchar(1000);
divideStrSys nvarchar(1000);
insertStr nvarchar(4000);

joinStr nvarchar(4000);
salesStr nvarchar(2000);
grossProfitStr nvarchar(2000);
grossPrcntStr nvarchar(2000);
priceSysStr nvarchar(2000);
salesSysStr nvarchar(2000);
grossProfitSysStr  nvarchar(2000);
grossPrcntSysStr	nvarchar(2000);
grossCalcStr		nvarchar(2000);
grossCalcSysStr	nvarchar(2000);
PropsRel 		nvarchar(10);
ii 				int;
jj 				int;
GrossBySal 		nChar(1);
WhereStr 		nvarchar(4000);
DiscSumSysColName nvarchar(20);
VatSysColName 	nvarchar(20);
QtyFldName 		nvarchar(100);
Mult 			nvarchar(5);
CutObjects nvarchar(50);
DpmAffectsTotal		int;
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
		
	Select MIN("DirectRate") into MultOrDivSysRate from OADM;
	
	if (:MultOrDivSysRate = 'Y') then
		MultOrDivSysRate := '/';
	else
		MultOrDivSysRate := '*';
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
		WTLines := :ObjectType || '5';
		--opObject := 'RIN';		
	end if;
	
	if (:ObjectType = 'CSI') then		
		Header := 'OCSI';
		Lines :=  ' (SELECT * FROM CSI1) ';
		WTLines := :ObjectType || '5';
		--opObject := 'NON';
	end if;
	
	if (:ObjectType = 'CSV') then
		Header := 'OCSV';
		Lines := ' (SELECT * FROM CSV1) ';
		WTLines := :ObjectType || '5';
		--opObject := 'NON';
	end if;
	
	if (:ObjectType = 'RDR') then
		Header := 'ORDR';
		Lines := 'RDR1';
		WTLines := :ObjectType || '5';
		--opObject := 'NON';		
	end if;

	if (:ObjectType = 'DLN') then
		Header := 'ODLN';
		Lines := 'DLN1';
		WTLines := :ObjectType || '5';
		--opObject := 'RDN';		
	end if;

	if (:ObjectType = 'RIN') then
		Header := 'ORIN';
		Lines := 'RIN1';
		WTLines := :ObjectType || '5';
	end if;

	if (:ObjectType = 'RDN') then
		Header := 'ORDN';
		Lines :=  'RDN1';
		WTLines := :ObjectType || '5';
	end if;

-- Comment from original SQL SP 
--[RemoveCIN]
--if (:ObjectType = 'CIN')
--Begin
--	Set :Header = 'OCIN'
--	Set :Lines =  'CIN1'
--	WTLines := :ObjectType || '5';
--End

	SELECT TOP 1 COLUMN_NAME into DiscSumSysColName FROM "PUBLIC"."TABLE_COLUMNS" as A WHERE TABLE_NAME = :Header and POSITION = 58;	
	SELECT TOP 1 COLUMN_NAME into VatSysColName FROM "PUBLIC"."TABLE_COLUMNS" as A WHERE TABLE_NAME = :Header and POSITION = 57;	

	WhereStr :=' Main."DocDate" Between  TO_DATE( ''' ||  :DocDateFrom || ''', ''YYYYMMDD'' ) And  TO_DATE( ''' ||  :DocDateTo 
		|| ''', ''YYYYMMDD'' )  And  Main."DocDueDate" Between  TO_DATE( ''' ||  :DocDueDateFrom || ''', ''YYYYMMDD'' )   And  TO_DATE( ''' 
		||  :DocDueDateTo || ''', ''YYYYMMDD'' ) ';
	
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

	--Item Code range
	if LENGTH(:ItemCodeFrom) > 0 then 	
		WhereStr := :WhereStr  || '    And    Items."ItemCode" >=  N''' ||  :ItemCodeFrom || '''  ';
	end if;
	
	if LENGTH(:ItemCodeTo) > 0 then 	
		WhereStr := :WhereStr  || '    And    Items."ItemCode" <=  N''' ||  :ItemCodeTo || '''  ';
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
	
	if (:ObjectType = 'INV' or :ObjectType = 'DLN') then
		WhereStr := WhereStr || ' And Lines."Quantity" <> 0';
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
	
	if LENGTH(:ItmProps) > 0 then 
		if (SUBSTRING (:ItmProps, 65, 1)  = 'N') then		
			PropsRel := ' OR ';		
		else
			PropsRel := ' AND ';
		end if;
		 
		WhereStr := :WhereStr || '  And ( '; 
		ii := 1;
		jj := 1;
		while (:ii <= 64) do		
			if (SUBSTRING (:ItmProps, :ii, 1)  = 'Y') then
				if (:jj <> 1) then
					WhereStr := :WhereStr || :PropsRel;
				end if;
				 	
				WhereStr := :WhereStr || ' Items."QryGroup' || Cast (:ii As Char(2)) || '" = ''Y''';
				jj := :jj + 1;
			end if;
			 
			ii := :ii + 1;
		end while;
		
		if(:jj = 1) then  
				WhereStr := WhereStr || ' 1 = 1 ';	/* fill empty brackets */ 
			end if;
		WhereStr := WhereStr || ') '; 
	end if;
	
	if LENGTH(:ItmGroupCode) > 0 then 
		WhereStr := WhereStr || ' And  ItmGrp."ItmsGrpCod" = ' || :ItmGroupCode; 
	end if;
	
	
	if (:ObjectType = 'RIN') then
		if(:ShowType <> 'CAN') then
			WhereStr := :WhereStr || ' AND NOT  EXISTS (SELECT 1 FROM '|| :ObjectType || '1 T1 WHERE T1."DocEntry" = Main."DocEntry"    AND T1."BaseType" = 203) ';
		else
			WhereStr := :WhereStr || ' AND NOT EXISTS (SELECT 1 FROM ' || :ObjectType || '1 T1 ,' || :ObjectType || '1 T2 WHERE T1."DocEntry" = Main."DocEntry"  AND T1."BaseType" = 14 AND T1."BaseEntry" = T2."DocEntry" AND T2."BaseType" = 203)';
		end if;
	end if;

	if (:ObjectType = 'INV' or :ObjectType = 'CSI' or :ObjectType = 'CSV' or :ObjectType = 'RIN') then	
		QtyFldName := ' Lines."Quantity"  ';	
	else	
		if (:ObjectType = 'DLN') then		
			QtyFldName := ' ( case when Lines."BaseType" in (13,165) then 0 else ( Lines."OpenCreQty" ) end )';		
		else		
			QtyFldName := ' Lines."OpenCreQty" ';
		end if;
	end if;
	
	DpmSUM := '';
	DpmSUM_SC := '';
	if (:DpmAffectsTotal = 1) then 	 	
		DpmSUM := ' + Sum(Distinct(Main."DpmAmnt")) ';		
		DpmSUM_SC := ' + Sum(Distinct(Main."DpmAmntSC")) ';		
	end if;


	divideStr := '( ( case when ( Sum(Distinct(Main."DocTotal")) - Sum(Distinct(Main."VatSum")) + Sum(Distinct(IfNull(Main."FoCTax",0))) - Sum(Distinct(Main."TotalExpns")) '
		|| ' + Sum(Distinct(Main."DiscSum")) ) = 0 then 0 else ( case (Sum(Distinct(Main."DocTotal")) ' || :DpmSUM 
		|| ' + IFNULL(WT."WTSum",0.0) - Sum(Distinct(Main."VatSum")) + Sum(Distinct(IfNull(Main."FoCTax",0))) - Sum(Distinct(Main."TotalExpns")) + Sum(Distinct(Main."DiscSum"))) '
		|| ' when 0 then 0 else (Sum(Distinct(Main."DiscSum")) / nullif((Sum(Distinct(Main."DocTotal"))' || :DpmSUM 
		|| ' + IFNULL(WT."WTSum",0.0)-Sum(Distinct(Main."VatSum")) + Sum(Distinct(IfNull(Main."FoCTax",0))) -Sum(Distinct(Main."TotalExpns"))+Sum(Distinct(Main."DiscSum"))),0)) '
		|| '  end) end ) * 100 ) / 100 ';

	divideStrSys := '( ( case when ((Sum(Distinct(Main."DocTotalSy"))  -  Sum(Distinct(Main."' || :VatSysColName || '"))  '
		|| ' + Sum(Distinct(IfNull(Main."FoCTaxSC",0))) -  Sum(Distinct(Main."TotalExpSC"))  + Sum(Distinct(Main."' || :DiscSumSysColName || '")))) = 0 ' 
		|| ' then 0 else (case (Sum(Distinct(Main."DocTotalSy")) ' || :DpmSUM_SC || ' + IFNULL(WT."WTSumSC",0.0) '
		|| ' -  Sum(Distinct(Main."' || :VatSysColName || '")) + Sum(Distinct(IfNull(Main."FoCTaxSC",0))) -  Sum(Distinct(Main."TotalExpSC"))  + Sum(Distinct(Main."' 
		|| :DiscSumSysColName || '"))) when 0 then 0 else (Sum(Distinct(Main."' || :DiscSumSysColName 
		|| '")) / nullif(Sum(Distinct(Main."DocTotalSy")) ' || :DpmSUM_SC || ' + IFNULL(WT."WTSumSC",0.0)-Sum(Distinct(Main."' 
		|| :VatSysColName || '")) + Sum(Distinct(IfNull(Main."FoCTaxSC",0))) -Sum(Distinct(Main."TotalExpSC")) + Sum(Distinct(Main."' || :DiscSumSysColName 
		|| '"))),0)) end) end ) * 100 ) / 100 ';

	insertStr := ' Insert Into "TmpPsar_Sales_An_Itm_Sng_Y_LC" ' 
		|| ' Select Main."DocEntry", Main."DocNum", Slp."SlpCode", Slp."SlpName", 	Cards."CardCode", Cards."CardName", '
		|| ' Main."DocDate", Main."DocDueDate",   Items."ItemCode", Items."ItemName", '
		|| ' 	case Lines."NoInvtryMv" when ''Y'' then 0 else '
		|| ' case Lines."UseBaseUn" when ''Y'' then  ' || :QtyFldName || ' * ' || :Mult 
		|| ' else (' || :QtyFldName || ' * Lines."NumPerMsr")  * ' || :Mult 
		|| ' end end as Quantity, '		
		|| ' 0 as Price, '; 		 

	if (:ObjectType = 'CSI' or :ObjectType = 'CSV') then	
		salesStr := '((((cast (Sum( Lines."LineTotal") as DECIMAL(21,6)) / nullif(Lines."Quantity",0))  / nullif(Lines."NumPerMsr",0))  * (' 
			|| :QtyFldName || ' * Lines."NumPerMsr")) )   * ' || :Mult || ' As Sales , ';
			
		grossProfitStr := '((((cast (Sum( Lines."LineTotal") as DECIMAL(21,6)) / nullif(Lines."Quantity",0))  / nullif(Lines."NumPerMsr",0))  * (' 
			|| :QtyFldName || ' * Lines."NumPerMsr")) - (Lines."GrossBuyPr" *' || :QtyFldName || ' )) * ' || :Mult || ' As GrossProfit,';
			
		grossPrcntStr := '-1 As GrossPrcnt,';
		
		priceSysStr := '0 As PriceSys ,';
		
		salesSysStr := '0 As SalesSys ,';
			
		grossProfitSysStr := ' 0 As GrossProfitSys,';
			
		grossPrcntSysStr := '-1 As GrossPrcntSys,Lines."LineNum", ';
		
		grossCalcStr := '0 AS GrossCalc,';
			
		grossCalcSysStr := '0 AS GrossCalcSys	';
	
	else
		salesStr := ' case when Lines."TaxOnly" = ''Y'' OR Lines."FreeChrgBP" = ''Y'' then  0 else ( (((cast (Sum( Lines."LineTotal") as DECIMAL(21,6)) / nullif(Lines."Quantity",0)) '
			|| '  / nullif(Lines."NumPerMsr",0))  * (' || :QtyFldName || ' * Lines."NumPerMsr"))  - (   (((cast (Sum( Lines."LineTotal") '
			|| ' as DECIMAL(21,6)) / nullif(Lines."Quantity",0))  / nullif(Lines."NumPerMsr",0))  * (' || :QtyFldName || ' * Lines."NumPerMsr")) * '
			|| :divideStr || ') )   * ' || :Mult || ' end as Sales, ';
			
		grossProfitStr := ' case when Lines."TaxOnly" = ''Y'' OR Lines."FreeChrgBP" = ''Y'' then (0 - (Lines."GrossBuyPr" * ' || :QtyFldName || ' )) * ' 
			|| :Mult || ' else ( (((cast (Sum( Lines."LineTotal") as DECIMAL(21,6)) / nullif(Lines."Quantity",0))  / nullif(Lines."NumPerMsr",0))  * (' 
			|| :QtyFldName || ' * Lines."NumPerMsr"))  - (   (((cast (Sum( Lines."LineTotal") as DECIMAL(21,6)) / nullif(Lines."Quantity",0)) '
			|| '  / nullif(Lines."NumPerMsr",0))  * (' || :QtyFldName || ' * Lines."NumPerMsr")) *  ' 
			|| :divideStr || ' )   - (Lines."GrossBuyPr" * ' || :QtyFldName || ' )) * ' || :Mult || ' end as GrossProfit, ';
			
		grossPrcntStr := ' -1  As GrossPrcnt,'; 
			
		priceSysStr := '  0 as PriceSys, ';
			
		salesSysStr := ' 0 as SalesSys, ';
			
		grossProfitSysStr := ' 0 as GrossProfitSys, ';
			
			
		grossPrcntSysStr :=	' -1  As GrossPrcntSYS, Lines."LineNum",';
	
	
		grossCalcStr := ' 0 as GrossCalc, ';
			
			
		grossCalcSysStr := ' 0 as GrossCalcSys';
	end if;

	joinStr := '  From  ' || :Header || ' Main 	'
		|| ' Inner Join ' || :Lines || ' Lines On Main."DocEntry" = Lines."DocEntry" '
		|| ' Inner Join OITM Items On Items."ItemCode" = Lines."ItemCode" '
		|| ' Inner Join OITB ItmGrp On ItmGrp."ItmsGrpCod" = Items."ItmsGrpCod" '
		|| ' Inner  Join OCRD Cards On Cards."CardCode" = Main."CardCode" ';
		
	if (:RptType = 0) then	
		joinStr := :joinStr || ' Inner Join OSLP slp On Slp."SlpCode" = Main."SlpCode" ';
	end if;
	
	if (:RptType = 1) then
		joinStr := :joinStr || ' Inner Join OSLP slp On Slp."SlpCode" = Lines."SlpCode" ';
	end if;
	
	if (:WTaxEngine = 'N') then
		joinStr := :joinStr || ' LEFT OUTER JOIN (select WT0."DocEntry", WT0."WTSum", WT0."WTSumSC" from ' || :Header 
		|| ' WT0 INNER JOIN ' || :WTLines || ' WT1 on WT1."AbsEntry" = WT0."DocEntry" where WT1."Category" = ''I'' '
		|| ' GROUP BY WT0."DocEntry", WT0."WTSum", WT0."WTSumSC") WT on WT."DocEntry" = Main."DocEntry" ';
	else
		joinStr := :joinStr || ' LEFT OUTER JOIN (select WT0."DocEntry", WT0."WTSum", WT0."WTSumSC" from ' || :Header 
		|| ' WT0 GROUP BY WT0."DocEntry", WT0."WTSum", WT0."WTSumSC") WT on WT."DocEntry" = Main."DocEntry" ';
	end if;
	
	if :DspFrznBP = 'N' then
		WhereStr := '(' || :WhereStr || ') And ' || '(Cards."validFor" = ''Y'' or (Cards."frozenFor" = ''Y'' and '
			|| '(Cards."frozenFrom" is not null or Cards."frozenTo" is not null)) or (Cards."validFor" = ''N'' and Cards."frozenFor" = ''N''))';
	end if;

	if :DspFrznITM = 'N' then
		WhereStr := '(' || :WhereStr || ') And ' || '(Items."validFor" = ''Y'' or (Items."frozenFor" = ''Y'' and '
			|| ' (Items."frozenFrom" is not null or Items."frozenTo" is not null)) or (Items."validFor" = ''N'' and Items."frozenFor" = ''N''))';
	end if;			
	
	if LENGTH(:BPLIdSet) > 0 then
		WhereStr := :WhereStr || ' And Main."BPLId" in (' || :BPLIdSet || ')';
	end if;

	WhereStr := ' Where ' || :WhereStr || '  Group BY Main."DocEntry", Slp."SlpCode", Slp."SlpName", '
		|| ' Main."DocEntry", Main."DocNum", Main."DocDate", '
		|| '  Items."ItemCode", Items."ItemName", Lines."TaxOnly", Lines."FreeChrgBP", Lines."Price", Main."DocDueDate", Cards."CardCode", Cards."CardName", '
		|| ' Lines."GrossBuyPr", Lines."Quantity", Main."SysRate", Lines."LineNum", Lines."NumPerMsr", '
		|| ' Lines."OpenCreQty",	Main."DocTotal", Main."VatSum", lines."UseBaseUn", WT."WTSum", WT."WTSumSC", Lines."BaseType", Lines."NoInvtryMv" ';
		
	if LENGTH(:CrdGroupCode) > 0 then 
		WhereStr := :WhereStr || ', Cards."GroupCode" '; 
	end if;
	
	if LENGTH(:ItmGroupCode) > 0 then 
		WhereStr := :WhereStr || ', ItmGrp."ItmsGrpCod" ';
	end if;

	WhereStr := :WhereStr || ' Order By Items."ItemCode", Main."DocEntry", Lines."LineNum" ';
  
	exec (:insertStr || :salesStr || :grossProfitStr || :grossPrcntStr || :priceSysStr || :salesSysStr || :grossProfitSysStr || :grossPrcntSysStr || :grossCalcStr || :grossCalcSysStr || :joinStr || :WhereStr);
	
	--select 1, (:insertStr || :salesStr || :grossProfitStr || :grossPrcntStr || :priceSysStr || :salesSysStr   ) from dummy
	--union all
	--select 2, (:grossProfitSysStr || :grossPrcntSysStr || :grossCalcStr || :grossCalcSysStr   ) from dummy
	--union all
	--select 3, ( :joinStr || :WhereStr  ) from dummy;	
		
end;


