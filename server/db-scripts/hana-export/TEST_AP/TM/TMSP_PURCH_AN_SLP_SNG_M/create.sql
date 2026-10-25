-- B1 DEPENDS: AFTER:SP:_TmSp_BootCreateGlobalTempTables AFTER:SP:TmSp_Purch_An_Slp_Sng_M_Insert AFTER:PT:PROCESS_END

CREATE  PROCEDURE TmSp_Purch_An_Slp_Sng_M (
in ObjectType 		nChar(3),
in ShowType 		nChar(3),
in DocDateFrom 		nvarchar(8),  -- = '19700101',
in DocDateTo		nvarchar(8),  -- = '21000101',
in DocDueDateFrom 	nvarchar(8),  -- = '19700101',
in DocDueDateTo		nvarchar(8),  -- = '21000101',
in CardCodeFrom		nvarchar(15), --= '' ,
in CardCodeTo		nvarchar(15), --= '',
in CrdGroupCode  	nvarchar(11), --= '',
in ItemCodeFrom		nvarchar(50), --= '' ,
in ItemCodeTo		nvarchar(50), --= '',
in ItmGroupCode  	nvarchar(11), --= '',
in ItmProps	   		nvarchar(70), --= '',
in CrdProps   		nvarchar(70 ),--= '',
in SlpCodeFrom		nvarchar(155),-- = '',
in SlpCodeTo		nvarchar(155),-- = '',
in TaxDateFrom	 	nvarchar(8),  -- = '19700101',
in TaxDateTo		nvarchar(8),  -- = '21000101',
in Brand			nvarchar(11), --='',			
in PrjCode			nvarchar(20), --  ='',
in RptType			int, 	     --= 0,
in cutbyObject		nchar(3), --= 'NON',
in DspFrznBP		nvarchar(1), --= 'Y',
in DspFrznITM		nvarchar(1), --= 'Y',
in BPLIdSet 		nvarchar(1000), 	--= ''
in SlpIncludeInactive	nvarchar(1),
in WTaxEngine	nvarchar(1) -- = 'N'
)
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER

  --With Encryption 

AS

Header 			nvarchar(4);
Lines  			nvarchar(4);
InstLines  		nvarchar(4);
opObject 		nvarchar(4);
tmpStr 			nvarchar(4000);
PropsRel 		nvarchar(10);
ii 				int;
jj 				int;
GrossBySal 		nChar(1);
WhereStr 		nvarchar(4000);
VatSysColName 	nvarchar(20);
WTLines			nvarchar(4);

Begin


--SET ANSI_WARNINGS OFF
--SET ARITHIGNORE ON
--SET ARITHABORT OFF

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
		InstLines := 'CPI6';
		Lines := 'CPI1';
        WTLines := 'CPI5';
	end if;
	
	if (:ObjectType = 'CPV') then
		Header := 'OCPV';
		InstLines := 'CPV6';
		Lines := 'CPV1';
        WTLines := 'CPV5';
	end if;

	if (:ObjectType = 'POR') then
		Header := 'OPOR';
		Lines := 'POR1';
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

	delete from "TmpPsar_Purch_An_Slp_Sng_M";
		
	call TmSp_Purch_An_Slp_Sng_M_Insert(:ObjectType, :ShowType, :DocDateFrom ,:DocDateTo, :DocDueDateFrom ,:DocDueDateTo, 	 
		:CardCodeFrom, :CardCodeTo, :CrdGroupCode, :ItemCodeFrom, :ItemCodeTo, :ItmGroupCode, :ItmProps, :CrdProps, :SlpCodeFrom, :SlpCodeTo, 
		:TaxDateFrom, :TaxDateTo, :Brand, :PrjCode, :RptType, :cutbyObject, :DspFrznBP, :DspFrznITM, :BPLIdSet, :SlpIncludeInactive, :WTaxEngine);
	if(:ObjectType = 'PCH' OR :ObjectType = 'PDN') then
	call TmSp_Purch_An_Slp_Sng_M_Insert(:ObjectType, 'CAN', :DocDateFrom ,:DocDateTo, :DocDueDateFrom ,:DocDueDateTo, 	 
		:CardCodeFrom, :CardCodeTo, :CrdGroupCode, :ItemCodeFrom, :ItemCodeTo, :ItmGroupCode, :ItmProps, :CrdProps, :SlpCodeFrom, :SlpCodeTo, 
		:TaxDateFrom, :TaxDateTo, :Brand, :PrjCode, :RptType, :cutbyObject, :DspFrznBP, :DspFrznITM, :BPLIdSet, :SlpIncludeInactive, :WTaxEngine);
	end if;
	
	if (:ShowType = 'SNG') then
		if (:opObject <> 'NON') then
			call TmSp_Purch_An_Slp_Sng_M_Insert(:opObject , 'NST', :DocDateFrom ,:DocDateTo, :DocDueDateFrom ,:DocDueDateTo, 	 
			:CardCodeFrom, :CardCodeTo, :CrdGroupCode, :ItemCodeFrom, :ItemCodeTo, :ItmGroupCode, :ItmProps, :CrdProps, :SlpCodeFrom, :SlpCodeTo, 
			:TaxDateFrom, :TaxDateTo, :Brand, :PrjCode, :RptType, :cutbyObject, :DspFrznBP, :DspFrznITM, :BPLIdSet, :SlpIncludeInactive, :WTaxEngine);
			
			call TmSp_Purch_An_Slp_Sng_M_Insert(:opObject , 'CAN', :DocDateFrom ,:DocDateTo, :DocDueDateFrom ,:DocDueDateTo, 	 
			:CardCodeFrom, :CardCodeTo, :CrdGroupCode, :ItemCodeFrom, :ItemCodeTo, :ItmGroupCode, :ItmProps, :CrdProps, :SlpCodeFrom, :SlpCodeTo, 
			:TaxDateFrom, :TaxDateTo, :Brand, :PrjCode, :RptType, :cutbyObject, :DspFrznBP, :DspFrznITM, :BPLIdSet, :SlpIncludeInactive, :WTaxEngine);
		end if;
	
		if (:ObjectType = 'PCH') then
			call TmSp_Purch_An_Slp_Sng_M_Insert( 'CPI' , 'SN1', :DocDateFrom ,:DocDateTo, :DocDueDateFrom ,:DocDueDateTo, 	 
			:CardCodeFrom, :CardCodeTo, :CrdGroupCode, :ItemCodeFrom, :ItemCodeTo, :ItmGroupCode, :ItmProps, :CrdProps, :SlpCodeFrom, :SlpCodeTo, 
			:TaxDateFrom, :TaxDateTo, :Brand, :PrjCode, :RptType, :cutbyObject, :DspFrznBP, :DspFrznITM, :BPLIdSet, :SlpIncludeInactive, :WTaxEngine);
	
			call TmSp_Purch_An_Slp_Sng_M_Insert( 'CPV' , 'SN1', :DocDateFrom ,:DocDateTo, :DocDueDateFrom ,:DocDueDateTo, 	 
			:CardCodeFrom, :CardCodeTo, :CrdGroupCode, :ItemCodeFrom, :ItemCodeTo, :ItmGroupCode, :ItmProps, :CrdProps, :SlpCodeFrom, :SlpCodeTo, 
			:TaxDateFrom, :TaxDateTo, :Brand, :PrjCode, :RptType, :cutbyObject, :DspFrznBP, :DspFrznITM, :BPLIdSet, :SlpIncludeInactive, :WTaxEngine);
		end if;
	end if;

	Select "CardCode", "CardOrSlpName", "Month", "Year", Sum("Sales"),  Sum("SalesSys") 
	From "TmpPsar_Purch_An_Slp_Sng_M" 
	Group By "CardCode", "CardOrSlpName", "Month", "Year"
	Order By "CardCode", "Year", "Month";

	delete from "TmpPsar_Purch_An_Slp_Sng_M";

End;


