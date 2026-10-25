-- B1 DEPENDS: AFTER:SP:_TmSp_BootCreateGlobalTempTables AFTER:SP:TmSp_Sales_An_Crd_Grp_M_Insert AFTER:PT:PROCESS_END

CREATE  PROCEDURE TmSp_Sales_An_Crd_Grp_M (
in ObjectType 		nChar(3),
in ShowType 		nChar(3),
in DocDateFrom 		nvarchar(8),
in DocDateTo		nvarchar(8),
in DocDueDateFrom 	nvarchar(8),
in DocDueDateTo		nvarchar(8),
in CardCodeFrom		nvarchar(30),
in CardCodeTo		nvarchar(30),
in CrdGroupCode 	nvarchar(11),
in ItemCodeFrom		nvarchar(50),
in ItemCodeTo		nvarchar(50),
in ItmGroupCode 	nvarchar(11),
in ItmProps	   		nvarchar(70),
in CrdProps   		nvarchar(70 ),
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
in BPLIdSet 		nvarchar(1000),
in SlpIncludeInactive	nvarchar(1),
in WTaxEngine		nvarchar(1)
)
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS

Header 			nvarchar(4);
Lines  			nvarchar(4);
InstLines  		nvarchar(4);
opObject 		nvarchar(4);
WTLines			nvarchar(4);
 
BEGIN
	opObject := 'NON';

	if (:ObjectType = 'INV') then
		Header := 'OINV';
		Lines :=  'INV1';
		InstLines := 'INV6';
		opObject := 'RIN';
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
		opObject := 'NON';
		WTLines := 'RDR5';
	end if;

	if (:ObjectType = 'DLN') then
		Header := 'ODLN';
		Lines := 'DLN1';
		InstLines := 'DLN6';
		opObject := 'RDN';
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


	delete from "TmpPsar_Sales_An_Crd_Grp_M";
	
	call TmSp_Sales_An_Crd_Grp_M_Insert(:ObjectType, :ShowType, :DocDateFrom ,:DocDateTo, :DocDueDateFrom , :DocDueDateTo,
		:CardCodeFrom, :CardCodeTo, :CrdGroupCode, :ItemCodeFrom, :ItemCodeTo, :ItmGroupCode, :ItmProps, :CrdProps, :SlpCodeFrom, 
		:SlpCodeTo, :TaxDateFrom, :TaxDateTo, :Brand, :PrjCode, :RptType, :cutbyObject, :DspFrznBP, :DspFrznITM, :BPLIdSet, :WTaxEngine );
	
	if(:ObjectType = 'INV' OR :ObjectType = 'DLN') then
	call TmSp_Sales_An_Crd_Grp_M_Insert(:ObjectType, 'CAN', :DocDateFrom ,:DocDateTo, :DocDueDateFrom , :DocDueDateTo,
		:CardCodeFrom, :CardCodeTo, :CrdGroupCode, :ItemCodeFrom, :ItemCodeTo, :ItmGroupCode, :ItmProps, :CrdProps, :SlpCodeFrom, 
		:SlpCodeTo, :TaxDateFrom, :TaxDateTo, :Brand, :PrjCode, :RptType, :cutbyObject, :DspFrznBP, :DspFrznITM, :BPLIdSet, :WTaxEngine );
	end if;
-- print @tmpStr
-- print @WhereStr

	if (:ShowType = 'SNG') then
	
		if (:opObject <> 'NON') then
			call TmSp_Sales_An_Crd_Grp_M_Insert(:opObject, 'NST', :DocDateFrom ,:DocDateTo, :DocDueDateFrom , :DocDueDateTo,
			:CardCodeFrom, :CardCodeTo, :CrdGroupCode, :ItemCodeFrom, :ItemCodeTo, :ItmGroupCode, :ItmProps, :CrdProps, :SlpCodeFrom, :SlpCodeTo, 
			:TaxDateFrom, :TaxDateTo, :Brand, :PrjCode, :RptType, :cutbyObject, :DspFrznBP, :DspFrznITM, :BPLIdSet, :WTaxEngine);
			call TmSp_Sales_An_Crd_Grp_M_Insert(:opObject, 'CAN', :DocDateFrom ,:DocDateTo, :DocDueDateFrom , :DocDueDateTo,
			:CardCodeFrom, :CardCodeTo, :CrdGroupCode, :ItemCodeFrom, :ItemCodeTo, :ItmGroupCode, :ItmProps, :CrdProps, :SlpCodeFrom, :SlpCodeTo, 
			:TaxDateFrom, :TaxDateTo, :Brand, :PrjCode, :RptType, :cutbyObject, :DspFrznBP, :DspFrznITM, :BPLIdSet, :WTaxEngine);
		end if;

		if (:ObjectType = 'INV') then
			call TmSp_Sales_An_Crd_Grp_M_Insert('CSI', 'SN1', :DocDateFrom ,:DocDateTo, :DocDueDateFrom ,:DocDueDateTo,
			:CardCodeFrom, :CardCodeTo, :CrdGroupCode, :ItemCodeFrom, :ItemCodeTo, :ItmGroupCode, :ItmProps, :CrdProps, :SlpCodeFrom, :SlpCodeTo, 
			:TaxDateFrom, :TaxDateTo, :Brand, :PrjCode, :RptType, :cutbyObject, :DspFrznBP, :DspFrznITM, :BPLIdSet, :WTaxEngine);

			call TmSp_Sales_An_Crd_Grp_M_Insert('CSV', 'SN1', :DocDateFrom ,:DocDateTo, :DocDueDateFrom ,:DocDueDateTo,
			:CardCodeFrom, :CardCodeTo, :CrdGroupCode, :ItemCodeFrom, :ItemCodeTo, :ItmGroupCode, :ItmProps, :CrdProps, :SlpCodeFrom, :SlpCodeTo, 
			:TaxDateFrom, :TaxDateTo, :Brand, :PrjCode, :RptType, :cutbyObject, :DspFrznBP, :DspFrznITM, :BPLIdSet, :WTaxEngine);
		end if; 
	end if;

	Select "CardCode", "CardOrSlpName", "Month", "Year", Sum("Sales"), Sum("GrossPrft") , Sum("GrossPrcnt") , Sum("SalesSys") , Sum("GrossPrftSys") , Sum("GrossPrcntSys"), Sum("SalesForPrcnt") , Sum("SalesForPrcntSys")
	From "TmpPsar_Sales_An_Crd_Grp_M" 
	Group By "CardCode", "CardOrSlpName", "Month", "Year"
	Order By "CardCode", "Year", "Month";

	delete from "TmpPsar_Sales_An_Crd_Grp_M";
	
END;


