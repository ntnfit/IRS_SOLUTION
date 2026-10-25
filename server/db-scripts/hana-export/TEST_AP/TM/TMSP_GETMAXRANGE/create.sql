-- B1 DEPENDS: AFTER:SP:TmSp_GetMaxRange_Insert AFTER:PT:PROCESS_END AFTER:SP:_TmSp_BootCreateGlobalTempTables2

CREATE PROCEDURE TmSp_GetMaxRange(
in ObjectType 		nChar(3),
in ShowType 		nChar(3),
in DocDateFrom_in 		nvarchar(8),
in DocDateTo_in			nvarchar(8),
in DocDueDateFrom_in 	nvarchar(8),
in DocDueDateTo_in		nvarchar(8),
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
in TaxDateFrom_in 	nvarchar(8),
in TaxDateTo_in		nvarchar(8),
in Brand			nvarchar(11),			
in PrjCode			nvarchar(20),
in RptType			int,
in cutbyObject		nchar(3),
in DspFrznBP		nvarchar(1),
in DspFrznITM		nvarchar(1),
in BPLIdSet 		nvarchar(1000), 	--= ''
in SlpIncludeInactive	nvarchar(1),
in WTaxEngine		nvarchar(1)
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
DocDateFrom 	nvarchar(8);
DocDateTo		nvarchar(8);
DocDueDateFrom 	nvarchar(8);
DocDueDateTo	nvarchar(8);
TaxDateFrom 	nvarchar(8);
TaxDateTo		nvarchar(8);

begin
	DocDateFrom := DocDateFrom_in;
	DocDateTo := DocDateTo_in;
	DocDueDateFrom := DocDueDateFrom_in;
	DocDueDateTo := DocDueDateTo_in;
	TaxDateFrom :=	TaxDateFrom_in;
	TaxDateTo := TaxDateTo_in;

	if (:ObjectType = 'NON') then
		return;
	end if;

	if (:ShowType = 'SNG') then
		Mult := ' 1 ';
	Else
		Mult :=  ' -1 ';
	End if;
	
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
	
	delete from "TMP_GetMaxRange_Results";
			
	call TmSp_GetMaxRange_Insert (:ObjectType, 'NST', :DocDateFrom ,:DocDateTo, :DocDueDateFrom ,:DocDueDateTo, 	 
		:CardCodeFrom, :CardCodeTo, :CrdGroupCode, :ItemCodeFrom, :ItemCodeTo, :ItmGroupCode, :ItmProps, :CrdProps, :SlpCodeFrom, :SlpCodeTo, 
		:TaxDateFrom, :TaxDateTo, :Brand, :PrjCode, :RptType, :cutbyObject,:DspFrznBP, :DspFrznITM, :BPLIdSet);
			
	if (:ShowType = 'SNG' and :opObject <> 'NON') then
		call TmSp_GetMaxRange_Insert (:opObject , 'NST', :DocDateFrom ,:DocDateTo, :DocDueDateFrom ,:DocDueDateTo, 	 
		:CardCodeFrom, :CardCodeTo, :CrdGroupCode, :ItemCodeFrom, :ItemCodeTo, :ItmGroupCode, :ItmProps, :CrdProps, :SlpCodeFrom, :SlpCodeTo, 
		:TaxDateFrom, :TaxDateTo, :Brand, :PrjCode, :RptType, :cutbyObject,:DspFrznBP, :DspFrznITM, :BPLIdSet);
	end if;
		
	select Min(DocDateMain) As "Min", Max(DocDateMax) As "Max" from "TMP_GetMaxRange_Results";
	
	delete from "TMP_GetMaxRange_Results";
end;


