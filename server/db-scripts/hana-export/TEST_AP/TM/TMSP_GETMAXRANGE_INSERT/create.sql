-- B1 DEPENDS: AFTER:PT:PROCESS_END AFTER:SP:_TmSp_BootCreateGlobalTempTables2 AFTER:SP:_TmSp_ValidateSpParam

CREATE PROCEDURE TmSp_GetMaxRange_Insert(
in ObjectType 		nChar(3),
in ShowType 		nChar(3),
in DocDateFrom 		nvarchar(8),
in DocDateTo		nvarchar(8),
in DocDueDateFrom 	nvarchar(8),
in DocDueDateTo		nvarchar(8),
in CardCodeFrom		nvarchar(15),
in CardCodeTo		nvarchar(15),
in CrdGroupCode  	nvarchar(11),
in ItemCodeFrom		nvarchar(50),
in ItemCodeTo		nvarchar(50),
in ItmGroupCode  	nvarchar(11),
in ItmProps	   		nvarchar(70),
in CrdProps   		nvarchar(70),
in SlpCodeFrom		nvarchar(155),
in SlpCodeTo		nvarchar(155),
in TaxDateFrom 		nvarchar(8),
in TaxDateTo		nvarchar(8),
in Brand			nvarchar(11),			
in PrjCode			nvarchar(20),
in RptType			int,
in cutbyObject		nchar(3),
in DspFrznBP		nvarchar(1),
in DspFrznITM		nvarchar(1),
in BPLIdSet 		nvarchar(1000)
)
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
Header nvarchar(4);
Lines  nvarchar(4);
opObject nvarchar(4);
tmpStr nvarchar(4000);
PropsRel nvarchar(10);
ii int;
jj int;
GrossBySal nChar(1);
JoinStr nvarchar(200);
WhereStr nvarchar(4000);
VatSysColName nvarchar(20);
Mult nvarchar(5);

begin

call _TmSp_ValidateSpParam (:ObjectType);
call _TmSp_ValidateSpParam (:ShowType);
call _TmSp_ValidateSpParam (:DocDateFrom);
call _TmSp_ValidateSpParam (:DocDateTo);
call _TmSp_ValidateSpParam (:DocDueDateFrom);
call _TmSp_ValidateSpParam (:DocDueDateTo);
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
call _TmSp_ValidateSpParam (:TaxDateFrom);
call _TmSp_ValidateSpParam (:TaxDateTo);
call _TmSp_ValidateSpParam (:Brand);
call _TmSp_ValidateSpParam (:PrjCode);
call _TmSp_ValidateSpParam (:cutbyObject);
call _TmSp_ValidateSpParam (:DspFrznBP);
call _TmSp_ValidateSpParam (:DspFrznITM);
call _TmSp_ValidateSpParam (:BPLIdSet);

	if (:ShowType = 'SNG') then
		Mult := ' 1 ';
	Else
		Mult :=  ' -1 ';
	End if;
	
	if (:ObjectType = 'INV') then
		Header := 'OINV';
		Lines :=  'INV1';
		opObject := 'RIN';
	End if;
	
	if (:ObjectType = 'RDR') then
		Header := 'ORDR';
		Lines :=  'RDR1';
		opObject := 'NON';
	End if;
	
	if (:ObjectType = 'DLN') then
		Header := 'ODLN';
		Lines :=  'DLN1';
		opObject := 'RDN';
	End if;
	
	if (:ObjectType = 'RIN') then
		Header := 'ORIN';
		Lines :=  'RIN1';
	End if;
	
	if (:ObjectType = 'RDN') then
		Header := 'ORDN';
		Lines :=  'RDN1';
	End if;
	
	if (:ObjectType = 'PCH') then
		Header := 'OPCH';
		Lines := 'PCH1';
		opObject := 'RPC';
	End if;
	
	if (:ObjectType = 'POR') then
		Header := 'OPOR';
		Lines :=  'POR1';
		opObject := 'NON';
	End if;
	
	if (:ObjectType = 'PDN') then
		Header := 'OPDN';
		Lines :=  'PDN1';
		opObject := 'RPD';
	End if;
	
	if (:ObjectType = 'RPC') then
		Header := 'ORPC';
		Lines :=  'RPC1';
	End if;
	
	if (:ObjectType = 'RPD') then
		Header := 'ORPD';
		Lines :=  'RPD1';
	End if;

	WhereStr := '
	Where      Main."DocDate"  Between  TO_DATE(''' ||  :DocDateFrom || ''', ''YYYYMMDD'' )  And    TO_DATE(''' ||  :DocDateTo || ''', ''YYYYMMDD'' ) 
	And    	   Main."DocDueDate"  Between  TO_DATE(''' ||  :DocDueDateFrom || ''', ''YYYYMMDD'' )  And    TO_DATE(''' ||  :DocDueDateTo || ''', ''YYYYMMDD'' ) 
	And 	   Main."Instance" = 0
	And	   Main."CANCELED" = ''N''
	And	   Main."TaxDate" Between TO_DATE(''' ||  :TaxDateFrom || ''', ''YYYYMMDD'' )  And  TO_DATE(''' ||  :TaxDateTo || ''', ''YYYYMMDD'' )';

		--Card Code range
		if LENGTH(:CardCodeFrom) > 0 then
			WhereStr := :WhereStr  || '   And    Main."CardCode" >=  N''' ||  :CardCodeFrom || '''  ';
		END if;
		
		if LENGTH(:CardCodeTo) > 0 then
			WhereStr := :WhereStr   || '   And    Main."CardCode" <=  N''' ||  :CardCodeTo || '''  ';
		END if;
		
		JoinStr := 'Inner Join ' || :Lines || '  Lines On Main."DocEntry" = Lines."DocEntry" ' ||
					   ' Inner  Join OCRD Cards On Cards."CardCode" = Main."CardCode" ' ||
					   ' left Join OITM Items On Items."ItemCode" = Lines."ItemCode" ';
					   
		IF :DspFrznBP = 'N' then
			WhereStr := :WhereStr || ' And ' || '(Cards."validFor" = ''Y'' or (Cards."frozenFor" = ''Y'' and (Cards."frozenFrom" is not null or Cards."frozenTo" is not null)) or (Cards."validFor" = ''N'' and Cards."frozenFor" = ''N''))';
		END if;
		
		IF :DspFrznITM = 'N' then
			WhereStr := :WhereStr || ' And ' ||  '(Items."ItemCode" is null or Items."validFor" = ''Y'' or (Items."frozenFor" = ''Y'' and (Items."frozenFrom" is not null or Items."frozenTo" is not null)) or (Items."validFor" = ''N'' and Items."frozenFor" = ''N''))';
		END if;
		
		if LENGTH(:BPLIdSet) > 0 then
			WhereStr := :WhereStr || ' And Main."BPLId" in (' || :BPLIdSet || ')';
		end if;

		tmpStr := ' Insert Into "TMP_GetMaxRange_Results" Select  Max(Main."DocDate"),  Min(Main."DocDate") From ' || :Header || ' Main  ';

		--select (:tmpStr ||:JoinStr || :WhereStr) from dummy where 1=1;
		exec (:tmpStr ||:JoinStr || :WhereStr);
end;


