-- B1 DEPENDS: AFTER:SP:_TmSp_BootCreateGlobalTempTables AFTER:SP:TmSp_Purch_An_Itm_Sng_M_Insert AFTER:PT:PROCESS_END

CREATE PROCEDURE TmSp_Purch_An_Itm_Sng_M
(
in ObjectType 		nChar(3),
in ShowType 		nChar(3),
in DocDateFrom 		nvarchar(8),   --= '19700101',
in DocDateTo		nvarchar(8),   --= '21000101',
in DocDueDateFrom 	nvarchar(8),   --= '19700101',
in DocDueDateTo		nvarchar(8),   --= '21000101',
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
in TaxDateFrom 		nvarchar(8),   --= '19700101',
in TaxDateTo		nvarchar(8),   --= '21000101',
in Brand			nvarchar(11), --='',			
in PrjCode		nvarchar(20),   --='',
in RptType		int, 	     --= 0,
in cutbyObject		nchar(3), --= 'NON',
in DspFrznBP		nvarchar(1), --= 'Y',
in DspFrznITM		nvarchar(1), --= 'Y'
in BPLIdSet 		nvarchar(1000), 	--= ''
in SlpIncludeInactive	nvarchar(1),
in WTaxEngine	nvarchar(1) -- = 'N'
)
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
	
	--*With Encryption 


AS
 
Header 		nvarchar(4);
Lines  		nvarchar(4);
WTLines		nvarchar(4);
opObject 		nvarchar(4);
tmpStr 		nvarchar(4000);
PropsRel 		nvarchar(10);
TotalDiscPerc	nvarchar(512);
TotalDiscPercSC nvarchar(512);
ii 			int;
jj 			int;
GrossBySal 		nChar(1);
WhereStr 		nvarchar(4000);
DiscSumSysColName 	nvarchar(20);
SlpCodeColName 	nvarchar(20);
VatSysColName 		nvarchar(20);
QtyFldName 		nvarchar(100);

BEGIN


if (:ObjectType = 'PCH') then
	Header := 'OPCH';
	Lines := 'PCH1';
	WTLines := :ObjectType || '5';
	opObject := 'RPC';
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
	opObject := 'NON';
end if;

if (:ObjectType = 'PDN') then
	Header := 'OPDN';
	Lines :=  'PDN1';
	WTLines := :ObjectType || '5';
	opObject := 'RPD';
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

delete from "TmpPsar_Purch_An_Itm_Sng_M";

call TmSp_Purch_An_Itm_Sng_M_Insert(:ObjectType, :ShowType, :DocDateFrom, :DocDateTo, :DocDueDateFrom, :DocDueDateTo,
	:CardCodeFrom, :CardCodeTo, :CrdGroupCode, :ItemCodeFrom, :ItemCodeTo, :ItmGroupCode, :ItmProps, :CrdProps, :SlpCodeFrom, :SlpCodeTo,
	:TaxDateFrom, :TaxDateTo, :Brand, :PrjCode, :RptType, :cutbyObject, :DspFrznBP, :DspFrznITM, :BPLIdSet, :WTaxEngine);
	
if(:ObjectType = 'PCH' OR :ObjectType = 'PDN') then
call TmSp_Purch_An_Itm_Sng_M_Insert(:ObjectType, 'CAN', :DocDateFrom, :DocDateTo, :DocDueDateFrom, :DocDueDateTo,
	:CardCodeFrom, :CardCodeTo, :CrdGroupCode, :ItemCodeFrom, :ItemCodeTo, :ItmGroupCode, :ItmProps, :CrdProps, :SlpCodeFrom, :SlpCodeTo,
	:TaxDateFrom, :TaxDateTo, :Brand, :PrjCode, :RptType, :cutbyObject, :DspFrznBP, :DspFrznITM, :BPLIdSet, :WTaxEngine);
end if;

If (:ShowType = 'SNG') then
	if (:cutbyObject = 'NON') then
		if (:opObject <> 'NON') then
			call TmSp_Purch_An_Itm_Sng_M_Insert (:opObject , 'NST', :DocDateFrom ,:DocDateTo, :DocDueDateFrom ,:DocDueDateTo, 	 
			:CardCodeFrom, :CardCodeTo, :CrdGroupCode, :ItemCodeFrom, :ItemCodeTo, :ItmGroupCode, :ItmProps, :CrdProps, :SlpCodeFrom, :SlpCodeTo, 
			:TaxDateFrom, :TaxDateTo, :Brand, :PrjCode, :RptType, :cutbyObject, :DspFrznBP, :DspFrznITM, :BPLIdSet, :WTaxEngine);
			call TmSp_Purch_An_Itm_Sng_M_Insert (:opObject , 'CAN', :DocDateFrom ,:DocDateTo, :DocDueDateFrom ,:DocDueDateTo, 	 
			:CardCodeFrom, :CardCodeTo, :CrdGroupCode, :ItemCodeFrom, :ItemCodeTo, :ItmGroupCode, :ItmProps, :CrdProps, :SlpCodeFrom, :SlpCodeTo, 
			:TaxDateFrom, :TaxDateTo, :Brand, :PrjCode, :RptType, :cutbyObject, :DspFrznBP, :DspFrznITM, :BPLIdSet, :WTaxEngine);
		end if;
			
		If (:ObjectType = 'PCH') then
			call TmSp_Purch_An_Itm_Sng_M_Insert ('CPI' , 'SN1', :DocDateFrom ,:DocDateTo, :DocDueDateFrom ,:DocDueDateTo, 	 
			:CardCodeFrom, :CardCodeTo, :CrdGroupCode, :ItemCodeFrom, :ItemCodeTo, :ItmGroupCode, :ItmProps, :CrdProps, :SlpCodeFrom, :SlpCodeTo, 
			:TaxDateFrom, :TaxDateTo, :Brand, :PrjCode, :RptType, :cutbyObject, :DspFrznBP, :DspFrznITM, :BPLIdSet, :WTaxEngine);		 

			call TmSp_Purch_An_Itm_Sng_M_Insert ('CPV' , 'SN1', :DocDateFrom ,:DocDateTo, :DocDueDateFrom ,:DocDueDateTo, 	 
			:CardCodeFrom, :CardCodeTo, :CrdGroupCode, :ItemCodeFrom, :ItemCodeTo, :ItmGroupCode, :ItmProps, :CrdProps, :SlpCodeFrom, :SlpCodeTo, 
			:TaxDateFrom, :TaxDateTo, :Brand, :PrjCode, :RptType, :cutbyObject, :DspFrznBP, :DspFrznITM, :BPLIdSet, :WTaxEngine);		 
		end if;

		select "ItemCode", "ItemName", 'DUMMY', 'DUMMY', Month("DocDate"), Year("DocDate") ,Sum("Quantity"), Sum("Sales"), Sum("SalesSys")
		from "TmpPsar_Purch_An_Itm_Sng_M" 
		group by "ItemCode", "ItemName", Year("DocDate"), Month("DocDate")
		Order By "ItemCode", Year("DocDate"), Month("DocDate");
	end if;
	
	if (:cutbyObject = 'SLP') then
		if (:opObject <> 'NON') then
			call TmSp_Purch_An_Itm_Sng_M_Insert (:opObject , 'NST', :DocDateFrom ,:DocDateTo, :DocDueDateFrom ,:DocDueDateTo, 	 
			:CardCodeFrom, :CardCodeTo, :CrdGroupCode, :ItemCodeFrom, :ItemCodeTo, :ItmGroupCode, :ItmProps, :CrdProps, :SlpCodeFrom, :SlpCodeTo, 
			:TaxDateFrom, :TaxDateTo, :Brand, :PrjCode, :RptType, :cutbyObject, :DspFrznBP, :DspFrznITM, :BPLIdSet, :WTaxEngine);
			call TmSp_Purch_An_Itm_Sng_M_Insert (:opObject , 'CAN', :DocDateFrom ,:DocDateTo, :DocDueDateFrom ,:DocDueDateTo, 	 
			:CardCodeFrom, :CardCodeTo, :CrdGroupCode, :ItemCodeFrom, :ItemCodeTo, :ItmGroupCode, :ItmProps, :CrdProps, :SlpCodeFrom, :SlpCodeTo, 
			:TaxDateFrom, :TaxDateTo, :Brand, :PrjCode, :RptType, :cutbyObject, :DspFrznBP, :DspFrznITM, :BPLIdSet, :WTaxEngine);
		end if;
			
		If (:ObjectType = 'PCH') then
			call TmSp_Purch_An_Itm_Sng_M_Insert ('CPI' , 'SN1', :DocDateFrom ,:DocDateTo, :DocDueDateFrom ,:DocDueDateTo, 	 
			:CardCodeFrom, :CardCodeTo, :CrdGroupCode, :ItemCodeFrom, :ItemCodeTo, :ItmGroupCode, :ItmProps, :CrdProps, :SlpCodeFrom, :SlpCodeTo, 
			:TaxDateFrom, :TaxDateTo, :Brand, :PrjCode, :RptType, :cutbyObject, :DspFrznBP, :DspFrznITM, :BPLIdSet, :WTaxEngine);		 

			call TmSp_Purch_An_Itm_Sng_M_Insert ('CPV' , 'SN1', :DocDateFrom ,:DocDateTo, :DocDueDateFrom ,:DocDueDateTo, 	 
			:CardCodeFrom, :CardCodeTo, :CrdGroupCode, :ItemCodeFrom, :ItemCodeTo, :ItmGroupCode, :ItmProps, :CrdProps, :SlpCodeFrom, :SlpCodeTo, 
			:TaxDateFrom, :TaxDateTo, :Brand, :PrjCode, :RptType, :cutbyObject, :DspFrznBP, :DspFrznITM, :BPLIdSet, :WTaxEngine);		 
		end if;

		select "ItemCode", "ItemName", "SlpCode", "SlpName", Month("DocDate"), Year("DocDate") ,Sum("Quantity"), Sum("Sales"), Sum("SalesSys")
		from "TmpPsar_Purch_An_Itm_Sng_M" 
		group by "ItemCode", "ItemName", "SlpCode", "SlpName", Year("DocDate"), Month("DocDate")
		Order By "ItemCode", "SlpCode", Year("DocDate"), Month("DocDate");
		
	end if;
	if (:cutbyObject = 'CRD') then
		if (:opObject <> 'NON') then
			call TmSp_Purch_An_Itm_Sng_M_Insert (:opObject , 'NST', :DocDateFrom ,:DocDateTo, :DocDueDateFrom ,:DocDueDateTo, 	 
			:CardCodeFrom, :CardCodeTo, :CrdGroupCode, :ItemCodeFrom, :ItemCodeTo, :ItmGroupCode, :ItmProps, :CrdProps, :SlpCodeFrom, :SlpCodeTo, 
			:TaxDateFrom, :TaxDateTo, :Brand, :PrjCode, :RptType, :cutbyObject, :DspFrznBP, :DspFrznITM, :BPLIdSet, :WTaxEngine);
			call TmSp_Purch_An_Itm_Sng_M_Insert (:opObject , 'CAN', :DocDateFrom ,:DocDateTo, :DocDueDateFrom ,:DocDueDateTo, 	 
			:CardCodeFrom, :CardCodeTo, :CrdGroupCode, :ItemCodeFrom, :ItemCodeTo, :ItmGroupCode, :ItmProps, :CrdProps, :SlpCodeFrom, :SlpCodeTo, 
			:TaxDateFrom, :TaxDateTo, :Brand, :PrjCode, :RptType, :cutbyObject, :DspFrznBP, :DspFrznITM, :BPLIdSet, :WTaxEngine);
		end if;
			
		if (:ObjectType = 'PCH') then
			call TmSp_Purch_An_Itm_Sng_M_Insert ('CPI' , 'SN1', :DocDateFrom ,:DocDateTo, :DocDueDateFrom ,:DocDueDateTo, 	 
			:CardCodeFrom, :CardCodeTo, :CrdGroupCode, :ItemCodeFrom, :ItemCodeTo, :ItmGroupCode, :ItmProps, :CrdProps, :SlpCodeFrom, :SlpCodeTo, 
			:TaxDateFrom, :TaxDateTo, :Brand, :PrjCode, :RptType, :cutbyObject, :DspFrznBP, :DspFrznITM, :BPLIdSet, :WTaxEngine);		 

			call TmSp_Purch_An_Itm_Sng_M_Insert ('CPV' , 'SN1', :DocDateFrom ,:DocDateTo, :DocDueDateFrom ,:DocDueDateTo, 	 
			:CardCodeFrom, :CardCodeTo, :CrdGroupCode, :ItemCodeFrom, :ItemCodeTo, :ItmGroupCode, :ItmProps, :CrdProps, :SlpCodeFrom, :SlpCodeTo, 
			:TaxDateFrom, :TaxDateTo, :Brand, :PrjCode, :RptType, :cutbyObject, :DspFrznBP, :DspFrznITM, :BPLIdSet, :WTaxEngine);		 
		end if;

		select "ItemCode", "ItemName", "CardCode", "CardName",  Month("DocDate"), Year("DocDate") ,Sum("Quantity"), Sum("Sales"), Sum("SalesSys")
		from "TmpPsar_Purch_An_Itm_Sng_M" 
		group by "ItemCode", "ItemName", "CardCode", "CardName", Year("DocDate"), Month("DocDate")
		Order By "ItemCode", "CardCode", Year("DocDate"), Month("DocDate");
	end if;
Else
	select * From "TmpPsar_Purch_An_Itm_Sng_M";
end If;

delete from "TmpPsar_Purch_An_Itm_Sng_M";

END;


