-- B1 DEPENDS: AFTER:SP:_TmSp_BootCreateGlobalTempTables AFTER:SP:TmSp_Purch_An_Crd_Det_Y_Insert AFTER:PT:PROCESS_END AFTER:SP:_TmSp_ValidateSpParam

CREATE  PROCEDURE TmSp_Purch_An_Crd_Det_Y
(in ObjectType		nChar(3),
in ShowType 		nChar(3),
in DocDateFrom 		nvarchar(8),  -- = '19700101',
in DocDateTo		nvarchar(8),  -- = '21000101',
in DocDueDateFrom 	nvarchar(8),  -- = '19700101',
in DocDueDateTo		nvarchar(8),  -- = '21000101',
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
in TaxDateFrom 		nvarchar(8),   -- = '19700101',
in TaxDateTo		nvarchar(8),   -- = '21000101',
in Brand			nvarchar(11), -- ='',			
in PrjCode		nvarchar(20),   -- ='',
in RptType		int, 	     -- = 0,
in cutbyObject		nchar(3), -- = 'NON',
in DspFrznBP		nvarchar(1), -- = 'Y',
in DspFrznITM		nvarchar(1), -- = 'Y',
in BPLIdSet 		nvarchar(1000), 	--= ''
in SlpIncludeInactive	nvarchar(1),
in WTaxEngine	nvarchar(1), -- = 'N',
in FilterByAgrAbsId	nvarchar(11) -- '' means do not filter, '0' means filter by is null, 'number > 0' filter by AgrAbsId
)
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
	
	--*With Encryption 

AS

Header 		nvarchar(4);
Lines  		nvarchar(4);
opObject 		nvarchar(4);
tmpStr 		nvarchar(4000);
PropsRel 		nvarchar(10);
ii 			int;
jj 			int;
GrossBySal 		nChar(1);
WhereStr 		nvarchar(4000);
VatSysColName 		nvarchar(20);
VatSysColName2 	nvarchar(20);
InstLines  		nvarchar(4);
WTLines		nvarchar(4);

BEGIN

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
call _TmSp_ValidateSpParam (:SlpIncludeInactive);
call _TmSp_ValidateSpParam (:WTaxEngine);

opObject := 'NON';

if (:ObjectType = 'PCH') then
	Header := 'OPCH';
	Lines := 'PCH1';
	InstLines := 'PCH6';
	opObject := 'RPC';
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
	opObject := 'NON';
    WTLines := 'POR5';
end if;
if (:ObjectType = 'PDN') then
	Header := 'OPDN';
	Lines :=  'PDN1';
	InstLines := 'PDN6';
	opObject := 'RPD';
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

delete from "TmpPsar_Purch_An_Crd_Det_Y";

call TmSp_Purch_An_Crd_Det_Y_Insert(:ObjectType, :ShowType, :DocDateFrom, :DocDateTo, :DocDueDateFrom, :DocDueDateTo,
	:CardCodeFrom, :CardCodeTo, :CrdGroupCode, :ItemCodeFrom, :ItemCodeTo, :ItmGroupCode, :ItmProps, :CrdProps, :SlpCodeFrom, :SlpCodeTo,
	:TaxDateFrom, :TaxDateTo, :Brand, :PrjCode, :RptType, :cutbyObject, :DspFrznBP, :DspFrznITM, :BPLIdSet, :WTaxEngine, :FilterByAgrAbsId);
	
if(:ObjectType = 'PCH' OR :ObjectType = 'PDN') then
	call TmSp_Purch_An_Crd_Det_Y_Insert(:ObjectType, 'CAN', :DocDateFrom, :DocDateTo, :DocDueDateFrom, :DocDueDateTo,
	:CardCodeFrom, :CardCodeTo, :CrdGroupCode, :ItemCodeFrom, :ItemCodeTo, :ItmGroupCode, :ItmProps, :CrdProps, :SlpCodeFrom, :SlpCodeTo,
	:TaxDateFrom, :TaxDateTo, :Brand, :PrjCode, :RptType, :cutbyObject, :DspFrznBP, :DspFrznITM, :BPLIdSet, :WTaxEngine, :FilterByAgrAbsId);
end if;
	
If (:ShowType = 'SNG') then
	If (:opObject <> 'NON') then
		call TmSp_Purch_An_Crd_Det_Y_Insert (:opObject, 'XXX', :DocDateFrom, :DocDateTo, :DocDueDateFrom, :DocDueDateTo,
		:CardCodeFrom, :CardCodeTo, :CrdGroupCode, :ItemCodeFrom, :ItemCodeTo, :ItmGroupCode, :ItmProps, :CrdProps, :SlpCodeFrom, :SlpCodeTo,
		:TaxDateFrom, :TaxDateTo, :Brand, :PrjCode, :RptType, :cutbyObject, :DspFrznBP, :DspFrznITM, :BPLIdSet, :WTaxEngine, :FilterByAgrAbsId);
		
		call TmSp_Purch_An_Crd_Det_Y_Insert (:opObject, 'CAN', :DocDateFrom, :DocDateTo, :DocDueDateFrom, :DocDueDateTo,
		:CardCodeFrom, :CardCodeTo, :CrdGroupCode, :ItemCodeFrom, :ItemCodeTo, :ItmGroupCode, :ItmProps, :CrdProps, :SlpCodeFrom, :SlpCodeTo,
		:TaxDateFrom, :TaxDateTo, :Brand, :PrjCode, :RptType, :cutbyObject, :DspFrznBP, :DspFrznITM, :BPLIdSet, :WTaxEngine, :FilterByAgrAbsId);		
	end if;

	if (:ObjectType = 'PCH') then
		call TmSp_Purch_An_Crd_Det_Y_Insert ('CPI', 'SN1', :DocDateFrom, :DocDateTo, :DocDueDateFrom, :DocDueDateTo,
		:CardCodeFrom, :CardCodeTo, :CrdGroupCode, :ItemCodeFrom, :ItemCodeTo, :ItmGroupCode, :ItmProps, :CrdProps, :SlpCodeFrom, :SlpCodeTo,
		:TaxDateFrom, :TaxDateTo, :Brand, :PrjCode, :RptType, :cutbyObject, :DspFrznBP, :DspFrznITM, :BPLIdSet, :WTaxEngine, :FilterByAgrAbsId);

		call TmSp_Purch_An_Crd_Det_Y_Insert('CPV', 'SN2', :DocDateFrom, :DocDateTo, :DocDueDateFrom, :DocDueDateTo,
		:CardCodeFrom, :CardCodeTo, :CrdGroupCode, :ItemCodeFrom, :ItemCodeTo, :ItmGroupCode, :ItmProps, :CrdProps, :SlpCodeFrom, :SlpCodeTo,
		:TaxDateFrom, :TaxDateTo, :Brand, :PrjCode, :RptType, :cutbyObject, :DspFrznBP, :DspFrznITM, :BPLIdSet, :WTaxEngine, :FilterByAgrAbsId);
	end if;
end if;

--exec (:tmpStr || :WhereStr)
		 /* end Here */

--print :tmpStr;
--print :WhereStr;

select * from "TmpPsar_Purch_An_Crd_Det_Y" Order By "ObjCode", "DocEntry";

delete from "TmpPsar_Purch_An_Crd_Det_Y";

END;


