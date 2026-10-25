-- B1 DEPENDS: AFTER:SP:TmSp_Sales_An_Crd_Det_Y_LC AFTER:PT:PROCESS_END AFTER:SP:_TmSp_BootCreateGlobalTempTables AFTER:SP:_TmSp_ValidateSpParam

CREATE PROCEDURE TmSp_DAB003_CustomerRankingFiscalY 
(
in ObjectType 		nChar(3),
in ShowType 		nChar(3),
inout DocDateFrom 		nvarchar(30),
inout DocDateTo			nvarchar(30),
inout DocDueDateFrom 	nvarchar(8),
inout DocDueDateTo		nvarchar(8),
inout CardCodeFrom		nvarchar(30),
inout CardCodeTo		nvarchar(30),
inout CrdGroupCode  	nvarchar(11),
inout ItemCodeFrom		nvarchar(100),
inout ItemCodeTo		nvarchar(100),
inout ItmGroupCode  	nvarchar(11),
inout ItmProps	   		nvarchar(70),
inout CrdProps   		nvarchar(70),
inout SlpCodeFrom		nvarchar(64),
inout SlpCodeTo			nvarchar(64),
inout TaxDateFrom 		nvarchar(8),
inout TaxDateTo			nvarchar(8),
inout Brand				nvarchar(11),
inout PrjCode			nvarchar(20),
inout RptType			int,
inout cutbyObject		nchar(3),
inout DspFrznBP			nvarchar(1),
inout DspFrznITM		nvarchar(1),
inout ExcludeCardCode1	nvarchar(30),
inout ExcludeCardCode2	nvarchar(30),
inout ExcludeCardCode3	nvarchar(30),
inout ExcludeCardCode4	nvarchar(30),
inout ExcludeCardCode5	nvarchar(30)
)

LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS

Header 	nvarchar(4);
Lines  	nvarchar(4);
InstLines 	nvarchar(4);
opObject 	nvarchar(4);
tmpStr 	nvarchar(4000);
tmpStr2 	nvarchar(4000);
PropsRel 	nvarchar(10);
ii 		int;
jj 		int;
GrossBySal 	nChar(1);
WhereStr 	nvarchar(4000);
VatSysColName 	nvarchar(20);
ObjectColName 	nvarchar(20);
Mult 		nvarchar(5);
WTLines	nvarchar(4);
CurrentDate		nvarchar(30);
WTaxEngine		nvarchar(1);
LawsSet			nvarchar(3);

begin
	
	call _TmSp_ValidateSpParam(ObjectType);
	call _TmSp_ValidateSpParam(ShowType);
	call _TmSp_ValidateSpParam(DocDateFrom);
	call _TmSp_ValidateSpParam(DocDateTo);
	call _TmSp_ValidateSpParam(DocDueDateFrom);
	call _TmSp_ValidateSpParam(DocDateTo);
	call _TmSp_ValidateSpParam(CardCodeFrom);
	call _TmSp_ValidateSpParam(CardCodeTo);
	call _TmSp_ValidateSpParam(CrdGroupCode);
	call _TmSp_ValidateSpParam(ItemCodeFrom);
	call _TmSp_ValidateSpParam(ItemCodeTo);
	call _TmSp_ValidateSpParam(ItmGroupCode);
	call _TmSp_ValidateSpParam(ItmProps);
	call _TmSp_ValidateSpParam(CrdProps);
	call _TmSp_ValidateSpParam(SlpCodeFrom);
	call _TmSp_ValidateSpParam(SlpCodeTo);
	call _TmSp_ValidateSpParam(TaxDateFrom);
	call _TmSp_ValidateSpParam(TaxDateTo);
	call _TmSp_ValidateSpParam(Brand);
	call _TmSp_ValidateSpParam(PrjCode);
	call _TmSp_ValidateSpParam(cutbyObject);
	call _TmSp_ValidateSpParam(ExcludeCardCode1);
	call _TmSp_ValidateSpParam(ExcludeCardCode2);
	call _TmSp_ValidateSpParam(ExcludeCardCode3);
	call _TmSp_ValidateSpParam(ExcludeCardCode4);
	call _TmSp_ValidateSpParam(ExcludeCardCode5);
	
	
	SELECT "LawsSet" into LawsSet FROM CINF;
	IF :LawsSet = 'AR' then
		WTaxEngine := 'Y';
	else
		WTaxEngine := 'N';
	end if;
	
	--TODO
	if :ObjectType = 'NON' then
		return;
	end if;

	if :ShowType = 'SNG' or :ShowType = 'SN1' or :ShowType = 'SN2' then
		Mult := ' 1 ';
	else
		Mult := ' -1 ';
	end if;

	DocDateFrom := TO_DATE(CURDATE(), 'YYMMDD');
	
	CurrentDate	:= :DocDateFrom;
	DocDateTo := :CurrentDate;
	select min("F_RefDate") into DocDateFrom from ofpr group by "Category" having min("F_RefDate") <= :CurrentDate and max ("T_RefDate") >= :CurrentDate;

	IF LENGTH(:DocDueDateFrom) = 0 then
		DocDueDateFrom := '19700101'; 
	END if;
	
	IF LENGTH(:DocDueDateTo) = 0 then
		DocDueDateTo := '21000101';
	END if;

	IF LENGTH(:TaxDateFrom) = 0 then
		TaxDateFrom := '19700101'; 
	END if;
	
	IF LENGTH(:TaxDateTo) = 0 then
		TaxDateTo := '21000101';
	END if;

	if (:ObjectType = 'INV') then
		Header := 'OINV';
		Lines :=  'INV1';
		InstLines := 'INV6';
		opObject := 'RIN';
		WTLines := 'INV5';
	End if;
	
	if (:ObjectType = 'CSI') then
		Header := 'OCSI';
		Lines :=  'CSI1';
		InstLines := 'CSI6';
		WTLines := 'CSI5';
	end if;
	
	if (:ObjectType = 'CSV') then
		Header := 'OCSV';
		Lines :=  'CSV1';
		InstLines := 'CSV6';
		WTLines := 'CSV5';
	End if;
	
	if (:ObjectType = 'RDR') then
		Header := 'ORDR';
		Lines :=  'RDR1';
		InstLines := 'RDR6';
		opObject := 'NON';
		WTLines := 'RDR5';
	End if;
	
	if (:ObjectType = 'DLN') then
		Header := 'ODLN';
		Lines :=  'DLN1';
		InstLines := 'DLN6';
		opObject := 'RDN';
		WTLines := 'DLN5';
	End if;
	
	if (:ObjectType = 'RIN') then
		Header := 'ORIN';
		Lines :=  'RIN1';
		InstLines := 'RIN6';
		WTLines := 'RIN5';
	End if;
	
	if (:ObjectType = 'RDN') then
		Header := 'ORDN';
		Lines :=  'RDN1';
		InstLines := 'RDN6';
		WTLines := 'RDN5';
	End if;

	select COLUMN_NAME into VatSysColName from "PUBLIC"."TABLE_COLUMNS" where TABLE_NAME = :Header and POSITION = 57;
	select COLUMN_NAME into ObjectColName from "PUBLIC"."TABLE_COLUMNS" where TABLE_NAME = :Header and POSITION = 10;

	WhereStr := 'Main."DocDate" Between  TO_DATE(''' ||  :DocDateFrom || ''', ''YYMMDD'' )  And  TO_DATE(DATETIME, ''' ||  :DocDateTo || ''', ''YYMMDD'' ) 
	And    	  Inst."DueDate" Between  TO_DATE(DATETIME, ''' ||  :DocDueDateFrom || ''', ''YYMMDD'')  And  TO_DATE(DATETIME, ''' ||  :DocDueDateTo || ''', ''YYMMDD'')';
	--Card Code range
	if length (:CardCodeFrom) > 0 then
		WhereStr := :WhereStr  || '   And    Main."CardCode" >=  N''' ||  :CardCodeFrom || '''  ';
	END if;
	
	if length (:CardCodeTo) > 0 then
		WhereStr := :WhereStr || '   And    Main."CardCode" <=  N''' ||  :CardCodeTo || '''  ';
	END if;

	--Sales person range
	if length (:SlpCodeFrom) > 0 then
		WhereStr := :WhereStr  || '    And   Slp."SlpName" >=  N''' ||  :SlpCodeFrom || '''  ';
	END if;
	
	if length (:SlpCodeTo) > 0 then
		WhereStr := :WhereStr  || '    And   Slp."SlpName" <=  N''' ||  :SlpCodeTo || '''  ';
	END if;

	WhereStr := :WhereStr  || 'And 	   Main."Instance" = 0
	And	   Main."TaxDate"  Between  TO_DATE(''' ||  :TaxDateFrom || ''', ''YYMMDD'' )  And  TO_DATE(''' ||  :TaxDateTo || ''', ''YYMMDD'')';

	If :ObjectType <> 'CSI' then
		WhereStr := :WhereStr || 'And   Main."Canceled" = ''N'' ';
	End if;

	IF LENGTH(:CrdGroupCode) > 0 then
		WhereStr := :WhereStr || ' And  Cards."GroupCode" = ' || :CrdGroupCode;
	END if;
								
	IF LENGTH(:CrdProps) > 0 then
		IF (SUBSTR (:CrdProps, 65, 1)  = 'N') then
			PropsRel := ' OR ';
		Else
			PropsRel := ' AND ';
		End if;
		
		WhereStr := :WhereStr || '  And ( '; 
		ii := 1;
		jj := 1;
		
		While :ii <= 64 DO
			IF (SUBSTR (:CrdProps, :ii, 1)  = 'Y') then
				if (:jj <> 1) then
					WhereStr := :WhereStr || :PropsRel;
				End if;
				
				WhereStr := :WhereStr || ' Cards."QryGroup"' || Cast (:ii As nChar(2)) || ' = ''Y''';
				jj := :jj + 1;
			End if;
			
			ii := :ii + 1;
		End while;
		WhereStr := :WhereStr || ') ';
	END if;
	
	if (:ObjectType = 'RIN') then
		WhereStr := :WhereStr || 'AND NOT  EXISTS (SELECT 1 FROM ' || :ObjectType || '1 T1 WHERE T1."DocEntry" = Main."DocEntry" AND T1."BaseType" = 203)'; 
	END if;

	tmpStr := ' Insert Into CustomerRankingFiscalY_CustomerRanking_1
		Select 	Main."DocNum",
			Slp."SlpName",		
			Main."DocDate", 
			Inst."DueDate",
			Cards."CardCode",
			Cards."CardName",
			((Inst."InsTotal") - (Inst."VatSum") + (IfNull(Main."FoCTax",0) * Inst."InstPrcnt" / 100) - (Inst."TotalExpns") + 
					  (CASE (SELECT TOP 1 (Wt."Category")
							   FROM ' || :WTLines || ' Wt where Wt."AbsEntry" = Main."DocEntry"
							  ORDER BY Wt."Category" ASC)
						WHEN ''I'' THEN Inst."WTSum" 
						ELSE 0
					   END) + Main."DpmAmnt" * Inst."InstPrcnt"/100 - Main."RoundDif" * Inst."InstPrcnt"/100
					)* ' || :Mult || ' As Sales,';

	if (:ShowType = 'NST') then
		tmpStr := :tmpStr || ' ((Main."DocTotal") - (Main."VatSum") + IfNull(Main."FoCTax",0) -  (Main."TotalExpns") +
									  (CASE (SELECT TOP 1 (Wt."Category")
											   FROM ' || :WTLines || ' Wt where Wt."AbsEntry" = Main."DocEntry"
											  ORDER BY Wt."Category" ASC)
										WHEN ''I'' THEN Main."WTSum"
										ELSE 0
									  END) + Main."DpmAmnt" * Inst."InstPrcnt"/100 
									  ) * -1 ,';
	ELSE
		 tmpStr := :tmpStr || '( Inst."PaidToDate" - Inst."vatpaid" - Inst."expappl" +
									  (CASE (SELECT TOP 1 (Wt."Category")
											   FROM ' || :WTLines || ' Wt where Wt."AbsEntry" = Main."DocEntry"
											  ORDER BY Wt."Category" ASC)
										WHEN ''I'' THEN Inst."WTApplied" 
										ELSE 0
									  END) + Main."DpmAmnt" * Inst."InstPrcnt"/100 
									  ) * ' || :Mult || '  As PaidAquit ,';
	End	if;	 
				
	tmpStr := :tmpStr || ' (Main."GrosProfit" * Inst."InstPrcnt" / 100 )* ' || :Mult || ' As GrossProfit , 0 as GrossPrcnt, 0  As SalesSys,';

	if (:ShowType = 'NST') then
		tmpStr := :tmpStr || ' 0 ,';
	ELSE
		tmpStr := :tmpStr || '0  As PaidAquit, ';
	End	IF;

	tmpStr2 := '' ||' 0 as GrossPrcntSys,0 as GrossPrcntSys,
				Inst.DocEntry As ABS,
				Inst.' || :ObjectColName || ',	';
				
	tmpStr2 := :tmpStr2 || ' 0  As  SalesForPrcnt, 0  As  SalesForPrcntSys ' || ', inst."instlmntid" as instId, main."installmnt" as instNum';

	tmpStr2 := :tmpStr2 || 'From  ' || :Header || ' Main 
		Inner  Join OCRD Cards On Cards."CardCode" = Main."CardCode"
		Inner  Join OSLP slp On Slp."SlpCode" = Main."SlpCode" Inner  
		Join ' || :InstLines || ' inst On inst."docentry" = Main."docentry"';

	IF :DspFrznBP = 'N' then
		WhereStr := '(' || :WhereStr || ') And ' || 
						'(Cards."validFor" = ''Y'' or (Cards."frozenFor" = ''Y'' and (Cards."frozenFrom" is not null or Cards."frozenTo" is not null)) or (Cards."validFor" = ''N'' and Cards."frozenFor" = ''N''))';
	END if;

	wherestr := ' Where ' || :WhereStr;

	exec (:tmpStr || :tmpStr2 || :WhereStr);

	If (:ShowType = 'SNG') then
		call TmSp_Sales_An_Crd_Det_Y_LC (:opObject , 'XX', :DocDateFrom ,:DocDateTo, :DocDueDateFrom ,:DocDueDateTo, 	 
		:CardCodeFrom, :CardCodeTo, :CrdGroupCode, :ItemCodeFrom, :ItemCodeTo, :ItmGroupCode, :ItmProps, :CrdProps, :SlpCodeFrom, :SlpCodeTo, 
		:TaxDateFrom, :TaxDateTo, :Brand, :PrjCode, :RptType, :cutbyObject, :DspFrznBP, :DspFrznITM, '', '', :WTaxEngine);

		if (:ObjectType = 'INV') then
			call TmSp_Sales_An_Crd_Det_Y_LC ('CSI' , 'XX1', :DocDateFrom ,:DocDateTo, :DocDueDateFrom ,:DocDueDateTo, 	 
			:CardCodeFrom, :CardCodeTo, :CrdGroupCode, :ItemCodeFrom, :ItemCodeTo, :ItmGroupCode, :ItmProps, :CrdProps, :SlpCodeFrom, :SlpCodeTo, 
			:TaxDateFrom, :TaxDateTo, :Brand, :PrjCode, :RptType, :cutbyObject, :DspFrznBP, :DspFrznITM, '', '', :WTaxEngine);

			call TmSp_Sales_An_Crd_Det_Y_LC ('CSV' , 'XX1', :DocDateFrom ,:DocDateTo, :DocDueDateFrom ,:DocDueDateTo, 	 
			:CardCodeFrom, :CardCodeTo, :CrdGroupCode, :ItemCodeFrom, :ItemCodeTo, :ItmGroupCode, :ItmProps, :CrdProps, :SlpCodeFrom, :SlpCodeTo, 
			:TaxDateFrom, :TaxDateTo, :Brand, :PrjCode, :RptType, :cutbyObject, :DspFrznBP, :DspFrznITM, '', '', :WTaxEngine);
		END IF;
	End if;

	IF LENGTH(:ExcludeCardCode1) > 0 then
		select 'Others', 'Others', SUM("Sales"), SUM("GrossProfit"), SUM("Paid") 
		From "CustomerRankingFiscalY_CustomerRanking_1"
		WHERE "CardCode"<>:ExcludeCardCode1 AND "CardCode"<>:ExcludeCardCode2 AND "CardCode"<>:ExcludeCardCode3 AND "CardCode"<>:ExcludeCardCode4 AND "CardCode"<>:ExcludeCardCode5;
	ELSE
		select TOP 5 "CardCode","CardName",SUM("Sales"),SUM("GrossProfit"),SUM("Paid") 
		From "CustomerRankingFiscalY_CustomerRanking_1"
		Group By "CardCode", "CardName" 
		Order By Sum("Sales") DESC;
	END if;
end;


