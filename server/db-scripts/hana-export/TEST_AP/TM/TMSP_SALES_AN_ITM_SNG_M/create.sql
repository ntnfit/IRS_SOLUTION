-- B1 DEPENDS: AFTER:SP:TmSp_Sales_An_Itm_Sng_M_Insert AFTER:PT:PROCESS_END

CREATE  PROCEDURE TmSp_Sales_An_Itm_Sng_M (
in ObjectType 		nChar(3),
in ShowType 		nChar(3),
in DocDateFrom 		nvarchar(8),
in DocDateTo		nvarchar(8),
in DocDueDateFrom 	nvarchar(8),
in DocDueDateTo		nvarchar(8),
in CardCodeFrom		nvarchar(30),
in CardCodeTo		nvarchar(30),
in CrdGroupCode 	nvarchar(11),
in ItemCodeFrom		nvarchar(100),
in ItemCodeTo		nvarchar(100),
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
opObject 		nvarchar(4);
WTLines			nvarchar(4);
 
BEGIN
	delete from "SalesAnItmSng_M_TMPPSAR";
	
	opObject := 'NON';

	if (:ObjectType = 'INV') then
		Header := 'OINV';
		Lines :=  'INV1';
		WTLines := :ObjectType || '5';
		opObject := 'RIN';		
	end if;
	
	if (:ObjectType = 'CSI') then		
		Header := 'OCSI';
		Lines :=  ' (SELECT * FROM CSI1) ';
		WTLines := :ObjectType || '5';
		opObject := 'NON';
	end if;
	
	if (:ObjectType = 'CSV') then			
		Header := 'OCSV';
		Lines := ' (SELECT * FROM CSV1) ';
		WTLines := :ObjectType || '5';
		opObject := 'NON';
	end if;
	
	if (:ObjectType = 'RDR') then
		Header := 'ORDR';
		Lines := 'RDR1';
		WTLines := :ObjectType || '5';
		opObject := 'NON';		
	end if;

	if (:ObjectType = 'DLN') then
		Header := 'ODLN';
		Lines := 'DLN1';
		WTLines := :ObjectType || '5';
		opObject := 'RDN';		
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
			
	call TmSp_Sales_An_Itm_Sng_M_Insert(:ObjectType, :ShowType, :DocDateFrom ,:DocDateTo, :DocDueDateFrom , :DocDueDateTo, 	 
		:CardCodeFrom, :CardCodeTo, :CrdGroupCode, :ItemCodeFrom, :ItemCodeTo, :ItmGroupCode, :ItmProps, :CrdProps, :SlpCodeFrom, 
		:SlpCodeTo, :TaxDateFrom, :TaxDateTo, :Brand, :PrjCode, :RptType, :cutbyObject, :DspFrznBP, :DspFrznITM, :BPLIdSet, :WTaxEngine);
	if(:ObjectType = 'INV' OR :ObjectType = 'DLN') then
	call TmSp_Sales_An_Itm_Sng_M_Insert(:ObjectType, 'CAN', :DocDateFrom ,:DocDateTo, :DocDueDateFrom , :DocDueDateTo, 	 
		:CardCodeFrom, :CardCodeTo, :CrdGroupCode, :ItemCodeFrom, :ItemCodeTo, :ItmGroupCode, :ItmProps, :CrdProps, :SlpCodeFrom, 
		:SlpCodeTo, :TaxDateFrom, :TaxDateTo, :Brand, :PrjCode, :RptType, :cutbyObject, :DspFrznBP, :DspFrznITM, :BPLIdSet, :WTaxEngine );
	end if;
-- print @tmpStr
-- print @WhereStr
	
	if (:ShowType = 'SNG') then
		if (:cutbyObject = 'NON') then		
		
			if (:opObject <> 'NON') then
				call TmSp_Sales_An_Itm_Sng_M_Insert (:opObject, 'NST', :DocDateFrom ,:DocDateTo, :DocDueDateFrom , :DocDueDateTo, 	 
				:CardCodeFrom, :CardCodeTo, :CrdGroupCode, :ItemCodeFrom, :ItemCodeTo, :ItmGroupCode, :ItmProps, :CrdProps, :SlpCodeFrom, :SlpCodeTo, 
				:TaxDateFrom, :TaxDateTo, :Brand, :PrjCode, :RptType, :cutbyObject, :DspFrznBP, :DspFrznITM, :BPLIdSet, :WTaxEngine);
				call TmSp_Sales_An_Itm_Sng_M_Insert (:opObject, 'CAN', :DocDateFrom ,:DocDateTo, :DocDueDateFrom , :DocDueDateTo, 	 
				:CardCodeFrom, :CardCodeTo, :CrdGroupCode, :ItemCodeFrom, :ItemCodeTo, :ItmGroupCode, :ItmProps, :CrdProps, :SlpCodeFrom, :SlpCodeTo, 
				:TaxDateFrom, :TaxDateTo, :Brand, :PrjCode, :RptType, :cutbyObject, :DspFrznBP, :DspFrznITM, :BPLIdSet, :WTaxEngine);
			end if;
		
			if (:ObjectType = 'INV') then 		
				call TmSp_Sales_An_Itm_Sng_M_Insert ('CSI' , 'SN1', :DocDateFrom ,:DocDateTo, :DocDueDateFrom , :DocDueDateTo, 	 
				:CardCodeFrom, :CardCodeTo, :CrdGroupCode, :ItemCodeFrom, :ItemCodeTo, :ItmGroupCode, :ItmProps, :CrdProps, :SlpCodeFrom, :SlpCodeTo, 
				:TaxDateFrom, :TaxDateTo, :Brand, :PrjCode, :RptType, :cutbyObject, :DspFrznBP, :DspFrznITM, :BPLIdSet, :WTaxEngine);
				

				call TmSp_Sales_An_Itm_Sng_M_Insert ('CSV' , 'SN1', :DocDateFrom ,:DocDateTo, :DocDueDateFrom , :DocDueDateTo, 	 
				:CardCodeFrom, :CardCodeTo, :CrdGroupCode, :ItemCodeFrom, :ItemCodeTo, :ItmGroupCode, :ItmProps, :CrdProps, :SlpCodeFrom, :SlpCodeTo, 
				:TaxDateFrom, :TaxDateTo, :Brand, :PrjCode, :RptType, :cutbyObject, :DspFrznBP, :DspFrznITM, :BPLIdSet, :WTaxEngine);			
			end if;

			select ItemCode, ItemName, 'DUMMY', 'DUMMY', Month(DocDate), Year(DocDate) ,Sum(Quantity), Sum(Sales), Sum(GrossProfit), Avg(GrossPrcnt), Sum(SalesSys), Sum(GrossProfitSys), Avg(GrossPrcntSys), Sum(SalesForPrcnt) , Sum(SalesForPrcntSys)
			from "SalesAnItmSng_M_TMPPSAR" 
			group by ItemCode, ItemName, Year(DocDate), Month(DocDate)
			Order By ItemCode, Year(DocDate), Month(DocDate);
			
		end if;
	
		if (:cutbyObject = 'SLP') then	
			if (:opObject <> 'NON') then
				call TmSp_Sales_An_Itm_Sng_M_Insert (:opObject, 'NST', :DocDateFrom ,:DocDateTo, :DocDueDateFrom , :DocDueDateTo, 	 
				:CardCodeFrom, :CardCodeTo, :CrdGroupCode, :ItemCodeFrom, :ItemCodeTo, :ItmGroupCode, :ItmProps, :CrdProps, :SlpCodeFrom, :SlpCodeTo, 
				:TaxDateFrom, :TaxDateTo, :Brand, :PrjCode, :RptType, :cutbyObject, :DspFrznBP, :DspFrznITM, :BPLIdSet, :WTaxEngine);
				call TmSp_Sales_An_Itm_Sng_M_Insert (:opObject, 'CAN', :DocDateFrom ,:DocDateTo, :DocDueDateFrom , :DocDueDateTo, 	 
				:CardCodeFrom, :CardCodeTo, :CrdGroupCode, :ItemCodeFrom, :ItemCodeTo, :ItmGroupCode, :ItmProps, :CrdProps, :SlpCodeFrom, :SlpCodeTo, 
				:TaxDateFrom, :TaxDateTo, :Brand, :PrjCode, :RptType, :cutbyObject, :DspFrznBP, :DspFrznITM, :BPLIdSet, :WTaxEngine);
			end if;
		
			if (:ObjectType = 'INV') then 		
				call TmSp_Sales_An_Itm_Sng_M_Insert ('CSI' , 'SN1', :DocDateFrom ,:DocDateTo, :DocDueDateFrom , :DocDueDateTo, 	 
				:CardCodeFrom, :CardCodeTo, :CrdGroupCode, :ItemCodeFrom, :ItemCodeTo, :ItmGroupCode, :ItmProps, :CrdProps, :SlpCodeFrom, :SlpCodeTo, 
				:TaxDateFrom, :TaxDateTo, :Brand, :PrjCode, :RptType, :cutbyObject, :DspFrznBP, :DspFrznITM, :BPLIdSet, :WTaxEngine);

				call TmSp_Sales_An_Itm_Sng_M_Insert ('CSV' , 'SN1', :DocDateFrom ,:DocDateTo, :DocDueDateFrom , :DocDueDateTo, 	 
				:CardCodeFrom, :CardCodeTo, :CrdGroupCode, :ItemCodeFrom, :ItemCodeTo, :ItmGroupCode, :ItmProps, :CrdProps, :SlpCodeFrom, :SlpCodeTo, 
				:TaxDateFrom, :TaxDateTo, :Brand, :PrjCode, :RptType, :cutbyObject, :DspFrznBP, :DspFrznITM, :BPLIdSet, :WTaxEngine);
			end if;

			select ItemCode, ItemName, SlpCode, SlpName, Month(DocDate), Year(DocDate) ,Sum(Quantity), Sum(Sales), Sum(GrossProfit), Avg(GrossPrcnt), Sum(SalesSys), Sum(GrossProfitSys), Avg(GrossPrcntSys), Sum(SalesForPrcnt) , Sum(SalesForPrcntSys)
			from "SalesAnItmSng_M_TMPPSAR" 
			group by ItemCode, ItemName, SlpCode, SlpName, Year(DocDate), Month(DocDate)
			Order By ItemCode, SlpCode, Year(DocDate), Month(DocDate);
			
		end if;
	
		if (:cutbyObject = 'CRD') then	
			if (:opObject <> 'NON') then
				call TmSp_Sales_An_Itm_Sng_M_Insert (:opObject, 'NST', :DocDateFrom ,:DocDateTo, :DocDueDateFrom , :DocDueDateTo, 	 
				:CardCodeFrom, :CardCodeTo, :CrdGroupCode, :ItemCodeFrom, :ItemCodeTo, :ItmGroupCode, :ItmProps, :CrdProps, :SlpCodeFrom, :SlpCodeTo, 
				:TaxDateFrom, :TaxDateTo, :Brand, :PrjCode, :RptType, :cutbyObject, :DspFrznBP, :DspFrznITM, :BPLIdSet, :WTaxEngine); 
				call TmSp_Sales_An_Itm_Sng_M_Insert (:opObject, 'CAN', :DocDateFrom ,:DocDateTo, :DocDueDateFrom , :DocDueDateTo, 	 
				:CardCodeFrom, :CardCodeTo, :CrdGroupCode, :ItemCodeFrom, :ItemCodeTo, :ItmGroupCode, :ItmProps, :CrdProps, :SlpCodeFrom, :SlpCodeTo, 
				:TaxDateFrom, :TaxDateTo, :Brand, :PrjCode, :RptType, :cutbyObject, :DspFrznBP, :DspFrznITM, :BPLIdSet, :WTaxEngine); 
			end if;

			if (:ObjectType = 'INV') then 	
				call TmSp_Sales_An_Itm_Sng_M_Insert ('CSI' , 'SN1', :DocDateFrom ,:DocDateTo, :DocDueDateFrom , :DocDueDateTo, 	 
				:CardCodeFrom, :CardCodeTo, :CrdGroupCode, :ItemCodeFrom, :ItemCodeTo, :ItmGroupCode, :ItmProps, :CrdProps, :SlpCodeFrom, :SlpCodeTo, 
				:TaxDateFrom, :TaxDateTo, :Brand, :PrjCode, :RptType, :cutbyObject, :DspFrznBP, :DspFrznITM, :BPLIdSet, :WTaxEngine);

				call TmSp_Sales_An_Itm_Sng_M_Insert ('CSV' , 'SN1', :DocDateFrom ,:DocDateTo, :DocDueDateFrom , :DocDueDateTo, 	 
				:CardCodeFrom, :CardCodeTo, :CrdGroupCode, :ItemCodeFrom, :ItemCodeTo, :ItmGroupCode, :ItmProps, :CrdProps, :SlpCodeFrom, :SlpCodeTo, 
				:TaxDateFrom, :TaxDateTo, :Brand, :PrjCode, :RptType, :cutbyObject, :DspFrznBP, :DspFrznITM, :BPLIdSet, :WTaxEngine);
			end if;

			select ItemCode, ItemName, CardCode, CardName, Month(DocDate), Year(DocDate) ,Sum(Quantity), Sum(Sales), Sum(GrossProfit), Avg(GrossPrcnt), Sum(SalesSys), Sum(GrossProfitSys), Avg(GrossPrcntSys), Sum(SalesForPrcnt) , Sum(SalesForPrcntSys)
			from "SalesAnItmSng_M_TMPPSAR" 
			group by ItemCode, ItemName, CardCode, CardName, Year(DocDate), Month(DocDate)
			Order By ItemCode, CardCode, Year(DocDate), Month(DocDate);
			
		end if;
	
	else
		select * From "SalesAnItmSng_M_TMPPSAR";
	end if;
	
	delete from "SalesAnItmSng_M_TMPPSAR";
	/* SBO_APA_DEV : TaxOnly */
		
END;


