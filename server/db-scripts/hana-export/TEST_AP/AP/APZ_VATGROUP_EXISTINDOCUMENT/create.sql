
CREATE PROCEDURE APZ_VatGroup_ExistInDocument
(
Code nvarchar(8) default ''
)
as
begin
create local temporary table #tbl as (Select Top 1  CAST('' as nvarchar(254)) ExistInObj from DUMMY);
if :Code<> ''
then
begin
	-- CPI15
	insert into #tbl  Select Top 1  'Tax group exist A/P Corr Inv - Drawn Dpm Appld' from CPI15 where  "VatGroup"=:Code

	-- CPI11
		union Select Top 1  'Tax group exist A/P Corr Inv - Drawn Dpm Det' from CPI11 where  "VatGroup"=:Code

	-- CPI3
	union Select Top 1  'Tax group exist A/P Correction Invoice - Freight' from CPI3 where  "VatGroup"=:Code
		
	-- CPI2
	union Select Top 1  'Tax group exist A/P Correction Invoice - Freight - Rows' from CPI2 where  "VatGroup"=:Code
	
	-- CPI1
	union Select Top 1  'Tax group exist A/P Correction Invoice - Rows'  from CPI1 where  "VatGroup"=:Code
	
	-- CPV3
	union Select Top 1  'Tax group exist A/P Correction Invoice Reversal - Freight' from CPV3 where  "VatGroup"=:Code

	-- CPV2
	union Select Top 1  'Tax group exist A/P Correction Invoice Reversal - Freight - Rows' from CPV2 where  "VatGroup"=:Code

	-- CPV1
	union Select Top 1  'Tax group exist A/P Correction Invoice Reversal - Rows' from CPV1 where  "VatGroup"=:Code
		
	-- CPV13
	union Select Top 1  'Tax group exist A/P Correction Invoice Reversal Rows - Distributed Expenses' from CPV13 where  "VatGroup"=:Code
	
	-- CPI13
	union Select Top 1  'Tax group exist A/P Correction Invoice Rows - Distributed Expenses' from CPI13 where  "VatGroup"=:Code
	
	-- RPC15
	union Select Top 1  'Tax group exist A/P Cr. Memo - Drawn Dpm Appld'  from RPC15 where  "VatGroup"=:Code

	-- RPC11
	union Select Top 1  'Tax group exist A/P Cr. Memo - Drawn Dpm Det.' from RPC11 where  "VatGroup"=:Code
	
	-- RPC3
	union Select Top 1  'Tax group exist A/P Credit Memo - Freight' from RPC3 where  "VatGroup"=:Code

	-- RPC1
	union Select Top 1  'Tax group exist A/P Credit Memo - Rows' from RPC1 where  "VatGroup"=:Code

	-- RPC13
	union Select Top 1  'Tax group exist A/P Credit Memo Rows - Distributed Expenses' from RPC13 where  "VatGroup"=:Code

	-- RPC2
	union Select Top 1  'Tax group exist A/P Credit Memo Rows - Expenses' from RPC2 where  "VatGroup"=:Code

	-- CPV15
	union Select Top 1  'Tax group exist A/P CrIn Rev - Drawn Dpm Appld' from CPV15 where  "VatGroup"=:Code
		
	-- CPV11
	union Select Top 1  'Tax group exist A/P CrIn Rev - Drawn Dpm Det.' from CPV11 where  "VatGroup"=:Code
		
	-- DPO3
	union Select Top 1  'Tax group exist A/P Down Payment - Freight' from DPO3 where  "VatGroup"=:Code
		
	-- DPO2
	union Select Top 1  'Tax group exist A/P Down Payment - Freight - Rows' from DPO2 where  "VatGroup"=:Code
	
	-- DPO1
	union Select Top 1  'Tax group exist A/P Down Payment - Rows' from DPO1 where  "VatGroup"=:Code

	-- DPO13
	union Select Top 1  'Tax group exist A/P Down Payment Rows - Distributed Expenses' from DPO13 where  "VatGroup"=:Code

	-- DPO15
	union Select Top 1  'Tax group exist A/P DP - Drawn Dpm Applied' from DPO15 where  "VatGroup"=:Code

	-- DPO11
	union Select Top 1  'Tax group exist AA/P DP - Drawn Dpm Detail' from DPO11 where  "VatGroup"=:Code

	-- PCH15
	union Select Top 1  'Tax group exist A/P Invoice - Drawn Dpm Appld' from PCH15 where  "VatGroup"=:Code

	-- PCH11
	union Select Top 1  'Tax group exist A/P Invoice - Drawn Dpm Det.' from PCH11 where  "VatGroup"=:Code

	-- PCH3
	union Select Top 1  'Tax group exist A/P Invoice - Freights' from PCH3 where  "VatGroup"=:Code

	-- PCH2
	union Select Top 1  'Tax group exist A/P Invoice - Freight - Rows' from PCH2 where  "VatGroup"=:Code

	-- PCH1
	union Select Top 1  'Tax group exist A/P Invoice - Rows' from PCH1 where  "VatGroup"=:Code

	-- PCH13
	union Select Top 1  'Tax group exist A/P Invoice Rows - Distributed Expenses' from PCH13 where  "VatGroup"=:Code
	
	-- CSI15
	union Select Top 1  'Tax group exist A/R Corr Inv - Drawn Dpm Appld' from CSI15 where  "VatGroup"=:Code

	-- CSI2
	union Select Top 1  'Tax group exist A/R Corr Inv - Freight - Rows' from CSI2 where  "VatGroup"=:Code

	-- CSI11
	union Select Top 1  'Tax group exist A/R Corr. Inv. - Drawn Dpm Det' from CSI11 where  "VatGroup"=:Code
	
	-- CIN3
	union Select Top 1  'Tax group exist A/R Correction Invoice - Freight' from CIN3 where  "VatGroup"=:Code

	-- CSI3
	union Select Top 1  'Tax group exist A/R Correction Invoice - Freight' from CSI3 where  "VatGroup"=:Code

	-- CIN2
	union Select Top 1  'Tax group exist A/R Correction Invoice - Freight - Rows' from CIN2 where  "VatGroup"=:Code
		
	-- CSI1
	union Select Top 1  'Tax group exist A/R Correction Invoice - Rows' from CSI1 where  "VatGroup"=:Code

	-- CSV3
	union Select Top 1  'Tax group exist A/R Correction Invoice Reversal - Freight' from CSV3 where  "VatGroup"=:Code

	-- CSV2
	union Select Top 1  'Tax group exist A/R Correction Invoice Reversal - Freight - Rows' from CSV2 where  "VatGroup"=:Code

	-- CSV1
	union Select Top 1  'Tax group exist A/R Correction Invoice Reversal - Rows' from CSV1 where  "VatGroup"=:Code

	-- CSV13
	union Select Top 1  'Tax group exist A/R Correction Invoice Reversal Rows - Distributed Expenses' from CSV13 where  "VatGroup"=:Code

	-- CSI13
	union Select Top 1  'Tax group exist A/R Correction Invoice Rows - Distributed Expenses' from CSI13 where  "VatGroup"=:Code

	-- RIN15
	union Select Top 1  'Tax group exist A/R Cr. Memo - Drawn Dpm Appld - Rows' from RIN15 where  "VatGroup"=:Code

	-- RIN11
	union Select Top 1  'Tax group exist A/R Cr. Memo - Drawn Dpm Det.' from RIN11 where  "VatGroup"=:Code

	-- RIN3
	union Select Top 1  'Tax group exist A/R Credit Memo - Freight' from RIN3 where  "VatGroup"=:Code

	-- RIN2
	union Select Top 1  'Tax group exist A/R Credit Memo - Freight - Rows' from RIN2 where  "VatGroup"=:Code	

	-- RIN1
	union Select Top 1  'Tax group exist A/R Credit Memo - Rows' from RIN1 where  "VatGroup"=:Code

	-- RIN13
	union Select Top 1  'Tax group exist A/R Credit Memo Rows - Distributed Expenses' from RIN13 where  "VatGroup"=:Code
	
	-- CSV15
	union Select Top 1  'Tax group exist A/R CrIn Rev - Drawn Dpm Appld' from CSV15 where  "VatGroup"=:Code

	-- CSV11
	union Select Top 1  'Tax group exist A/R CrIn Rev - Drawn Dpm Det.' from CSV11 where  "VatGroup"=:Code

	-- DPI2
	union Select Top 1  'Tax group exist A/R Down Payment - Freight - Rows' from DPI2 where  "VatGroup"=:Code

	-- DPI1
	union Select Top 1  'Tax group exist A/R Down Payment - Rows' from DPI1 where  "VatGroup"=:Code

	-- DPI13
	union Select Top 1  'Tax group exist A/R Down Payment Rows - Distributed Expenses' from DPI13 where  "VatGroup"=:Code

	-- DPI15
	union Select Top 1  'Tax group exist A/R DP - Drawn Dpm Applied' from DPI15 where  "VatGroup"=:Code

	-- DPI11
	union Select Top 1  'Tax group exist A/R DP - Drawn Dpm Detail' from DPI11 where  "VatGroup"=:Code

	-- ADO15
	union Select Top 1  'Tax group exist A/R Inv (Drawn Dpm Applied - Hist)' from ADO15 where  "VatGroup"=:Code

	-- ADO11
	union Select Top 1  'Tax group exist A/R Inv (Drawn Dpm Det - Hist)' from ADO11 where  "VatGroup"=:Code

	-- INV15
	union Select Top 1  'Tax group exist A/R Inv. - Drawn Dpm Applied' from INV15 where  "VatGroup"=:Code
	
	-- INV11
	union Select Top 1  'Tax group exist A/R Invoice - Drawn Dpm Detail' from INV11 where  "VatGroup"=:Code

	-- INV3
	union Select Top 1  'Tax group exist A/R Invoice - Freight' from INV3 where  "VatGroup"=:Code

	-- ADO3
	union Select Top 1  'Tax group exist A/R Invoice - Freight - History' from ADO3 where  "VatGroup"=:Code

	-- ADO2
	union Select Top 1  'Tax group exist A/R Invoice - Freight - Rows' from ADO2 where  "VatGroup"=:Code
	
	-- INV2
	union Select Top 1  'Tax group exist A/R Invoice - Freight - Rows' from INV2 where  "VatGroup"=:Code

	-- INV1
	union Select Top 1  'Tax group exist A/R Invoice - Rows' from INV1 where  "VatGroup"=:Code
	
	-- ADO1
	union Select Top 1  'Tax group exist A/R Invoice (Rows - History' from ADO1 where  "VatGroup"=:Code

	-- ADO13
	union Select Top 1  'Tax group exist A/R Invoice Rows - Distributed Expenses' from ADO13 where  "VatGroup"=:Code

	-- INV13
	union Select Top 1  'Tax group exist A/R Invoice Rows - Distributed Freights' from INV13 where  "VatGroup"=:Code

	-- BNK2
	union Select Top 1  'Tax group exist Bank Statement - Recommendation List' from BNK2 where  "VatCode"=:Code

	-- OBOE
	union Select Top 1  'Tax group exist Bill of Exchange for Payment' from OBOE where  "StampTax"=:Code

	-- ABOE
	union Select Top 1  'Tax group exist Bill of Exchange for Payment - History' from ABOE where  "StampTax"=:Code

	-- BOE1
	union Select Top 1  'Tax group exist Bill of Exchange for Payment - Rows' from BOE1 where  "Code"=:Code

	-- OCRD
	union Select Top 1  'Tax group exist Business Partner' from OCRD where  "ECVatGroup"=:Code

	-- ACRD
	union Select Top 1  'Tax group exist Business Partners - History' from ACRD where  "ECVatGroup"=:Code

	-- CHO1
	union Select Top 1  'Tax group exist Checks for Payment - Rows' from CHO1 where  "Code"=:Code

	-- CHD1
	union Select Top 1  'Tax group exist Checks for Payment Drafts - Rows' from CHD1 where  "Code"=:Code

	-- AINF
	union Select Top 1  'Tax group exist Company Info' from AINF where  "stampTax"=:Code

	-- CINF
	union Select Top 1  'Tax group existCompany Info' from CINF where  "stampTax"=:Code

	-- CIN11
	union Select Top 1  'Tax group exist Correction Invoice - Drawn Dpm Detail' from CIN11 where  "VatGroup"=:Code

	-- CIN1
	union Select Top 1  'Tax group exist Correction Invoice - Rows' from CIN1 where  "VatGroup"=:Code

	-- CIN13
	union Select Top 1  'Tax group exist Correction Invoice Rows - Distributed Expenses' from CIN13 where  "VatGroup"=:Code
	
	-- DLN15
	union Select Top 1  'Tax group exist Delivery - Drawn Dpm Applied' from DLN15 where  "VatGroup"=:Code

	-- DLN11
	union Select Top 1  'Tax group exist Delivery - Drawn Dpm Detail' from DLN11 where  "VatGroup"=:Code

	-- DLN1
	union Select Top 1  'Tax group exist Delivery - Rows' from DLN1 where  "VatGroup"=:Code

	-- DLN3
	union Select Top 1  'Tax group exist Delivery Notes - Freight' from DLN3 where  "VatGroup"=:Code
	
	-- DLN2
	union Select Top 1  'Tax group exist Delivery Notes - Freight - Rows' from DLN2 where  "VatGroup"=:Code

	-- DLN13
	union Select Top 1  'Tax group exist Delivery Rows - Distributed Expenses' from DLN13 where  "VatGroup"=:Code

	-- ODPS
	union Select Top 1  'Tax group exist Deposit' from ODPS where  "CommisVat"=:Code
	
	-- DRF15
	union Select Top 1  'Tax group exist Draft - Drawn Dpm Applied' from DRF15 where  "VatGroup"=:Code

	-- DRF11
	union Select Top 1  'Tax group exist Draft - Drawn Dpm Detail' from DRF11 where  "VatGroup"=:Code

	-- DRF3
	union Select Top 1  'Tax group exist Draft - Freight' from DRF3 where  "VatGroup"=:Code

	-- DRF2
	union Select Top 1  'Tax group exist Draft - Freight - Rows' from DRF2 where  "VatGroup"=:Code

	-- DRF1
	union Select Top 1  'Tax group exist Draft - Rows' from DRF1 where  "VatGroup"=:Code
	
	-- DRF13
	union Select Top 1  'Tax group exist Draft Rows - Distributed Expenses' from DRF13 where  "VatGroup"=:Code

	-- OEXD
	union Select Top 1  'Tax group exist Freight Setup' from OEXD where  "VatGroupI"=:Code or "VatGroupO"=:Code
	
	-- AEXD
	union Select Top 1  'Tax group exist Freight Setup' from AEXD where  "VatGroupI"=:Code or "VatGroupO"=:Code
	
	-- AACT
	union Select Top 1  'Tax group exist G/L Account - History' from AACT where  "DfltVat"=:Code

	-- OACT
	union Select Top 1  'Tax group exist G/L Accounts' from OACT where  "DfltVat"=:Code

	-- IGE15
	union Select Top 1  'Tax group exist Gds Issue - Drawn Dpm Applied' from IGE15 where  "VatGroup"=:Code
	
	-- IGN15
	union Select Top 1  'Tax group exist Gds Rcpt - Drawn Dpm Applied' from IGN15 where  "VatGroup"=:Code

	-- RPD15
	union Select Top 1  'Tax group exist Gds Return - Drawn Dpm Applied' from RPD15 where  "VatGroup"=:Code

	-- RPD11
	union Select Top 1  'Tax group exist Gds Return - Drawn Dpm Detail' from RPD11 where  "VatGroup"=:Code

	-- IGE11
	union Select Top 1  'Tax group exist Goods Issue - Drawn Dpm Detail' from IGE11 where  "VatGroup"=:Code

	-- IGE3
	union Select Top 1  'Tax group exist Goods Issue - Freight' from IGE3 where  "VatGroup"=:Code

	-- IGE2
	union Select Top 1  'Tax group exist Goods Issue - Freight - Rows' from IGE2 where  "VatGroup"=:Code

	-- IGE1
	union Select Top 1  'Tax group exist Goods Issue - Rows' from IGE1 where  "VatGroup"=:Code

	-- IGE13
	union Select Top 1  'Tax group exist Goods Issue Rows - Distributed Expenses' from IGE13 where  "VatGroup"=:Code

	-- IGN11
	union Select Top 1  'Tax group exist Goods Receipt - Drawn Dpm Det' from IGN11 where  "VatGroup"=:Code

	-- IGN3
	union Select Top 1  'Tax group exist Goods Receipt - Freight' from IGN3 where  "VatGroup"=:Code

	-- IGN2
	union Select Top 1  'Tax group exist Goods Receipt - Freight - Rows' from IGN2 where  "VatGroup"=:Code

	-- IGN1
	union Select Top 1  'Tax group exist Goods Receipt - Rows' from IGN1 where  "VatGroup"=:Code

	-- PDN3
	union Select Top 1  'Tax group exist Goods Receipt PO - Freight' from PDN3 where  "VatGroup"=:Code

	-- PDN2
	union Select Top 1  'Tax group exist Goods Receipt PO - Freight - Rows' from PDN2 where  "VatGroup"=:Code

	-- PDN1
	union Select Top 1  'Tax group exist Goods Receipt PO - Rows' from PDN1 where  "VatGroup"=:Code

	-- PDN13
	union Select Top 1  'Tax group exist Goods Receipt PO Rows - Distributed Expenses' from PDN13 where  "VatGroup"=:Code

	-- IGN13
	union Select Top 1  'Tax group exist Goods Receipt Rows - Distributed Expenses' from IGN13 where  "VatGroup"=:Code

	-- RPD3
	union Select Top 1  'Tax group exist Goods Return - Freight' from RPD3 where  "VatGroup"=:Code

	-- RPD2
	union Select Top 1  'Tax group exist Goods Return - Freight - Rows' from RPD2 where  "VatGroup"=:Code

	-- RPD1
	union Select Top 1  'Tax group exist Goods Return - Rows' from RPD1 where  "VatGroup"=:Code

	-- RPD13
	union Select Top 1  'Tax group exist Goods Returns Rows - Distributed Expenses' from RPD13 where  "VatGroup"=:Code

	-- PDN15
	union Select Top 1  'Tax group exist GRPO - Drawn Dpm Applied' from PDN15 where  "VatGroup"=:Code

	-- PDN11
	union Select Top 1  'Tax group exist GRPO - Drawn Dpm Detail' from PDN11 where  "VatGroup"=:Code

	-- IEI15
	union Select Top 1  'Tax group exist IEI - Drawn Dpm Applied' from IEI15 where  "VatGroup"=:Code

	-- IEI11
	union Select Top 1  'Tax group exist IEI - Drawn Dpm Detail' from IEI11 where  "VatGroup"=:Code

	-- IEI3
	union Select Top 1  'Tax group exist IEI - Freight' from IEI3 where  "VatGroup"=:Code
	
	-- IEI2
	union Select Top 1  'Tax group exist Incoming Excise Invoice - Freight - History - Rows' from IEI2 where  "VatGroup"=:Code

	-- IEI1
	union Select Top 1  'Tax group exist Incoming Excise Invoice - Rows' from IEI1 where  "VatGroup"=:Code

	-- IEI13
	union Select Top 1  'Tax group exist Incoming Excise Invoice Rows - Distributed Expenses' from IEI13 where  "VatGroup"=:Code

	-- ORCT
	union Select Top 1  'Tax group exist Incoming Payment' from ORCT where  "VatGroup"=:Code

	-- RCT4
	union Select Top 1  'Tax group exist Incoming Payment - Account List' from RCT4 where  "VatGroup"=:Code

	-- ARC4
	union Select Top 1  'Tax group exist Incoming Payment - Account List - History' from ARC4 where  "VatGroup"=:Code

	-- RCC4
	union Select Top 1  'Tax group exist Incoming Payment - Credit Vouchers' from RCC4 where  "VatGroup"=:Code
	
	-- ARCT
	union Select Top 1  'Tax group exist Incoming Payment - History' from ARCT where  "VatGroup"=:Code

	-- BTC1
	union Select Top 1  'Tax group exist Internal Bank Operation Codes - Accounts' from BTC1 where  "VatCode"=:Code

	-- WTR15
	union Select Top 1  'Tax group exist Inv. Transfer - Drawn Dpm Appl' from WTR15 where  "VatGroup"=:Code

	-- WTR11
	union Select Top 1  'Tax group exist Inv. Transfer - Drawn Dpm Det.' from WTR11 where  "VatGroup"=:Code

	-- WTQ15
	union Select Top 1  'Tax group exist Inv. Transfer Request - Drawn Dpm Appl' from WTQ15 where  "VatGroup"=:Code

	-- WTQ11
	union Select Top 1  'Tax group exist Inv. Transfer Request - Drawn Dpm Det.' from WTQ11 where  "VatGroup"=:Code

	-- WTR3
	union Select Top 1  'Tax group exist Inventory Transfer - Freight' from WTR3 where  "VatGroup"=:Code

	-- WTR2
	union Select Top 1  'Tax group exist Inventory Transfer - Freight - Rows' from WTR2 where  "VatGroup"=:Code

	-- WTR1
	union Select Top 1  'Tax group exist Inventory Transfer - Rows' from WTR1 where  "VatGroup"=:Code

	-- WTQ3
	union Select Top 1  'Tax group exist Inventory Transfer Request - Freight' from WTQ3 where  "VatGroup"=:Code

	-- WTQ2
	union Select Top 1  'Tax group exist Inventory Transfer Request - Freight - Rows' from WTQ2 where  "VatGroup"=:Code

	-- WTQ1
	union Select Top 1  'Tax group exist Inventory Transfer Request - Rows' from WTQ1 where  "VatGroup"=:Code
	
	-- WTQ13
	union Select Top 1  'Tax group exist Inventory Transfer Request Rows - Distributed Freights' from WTQ13 where  "VatGroup"=:Code

	-- WTR13
	union Select Top 1  'Tax group exist Inventory Transfer Rows - Distributed Freights' from WTR13 where  "VatGroup"=:Code

	-- OITM
	union Select Top 1  'Tax group exist Items' from OITM where  "VatGourpSa"=:Code or "VatGroupPu"=:Code

	-- AITM
	union Select Top 1  'Tax group exist Items - History' from AITM where  "VatGourpSa"=:Code or "VatGroupPu"=:Code

	-- IPF3
	union Select Top 1  'Tax group exist Landed Costs - Customs Summary' from IPF3 where  "VatGroup"=:Code

	-- OEI15
	union Select Top 1  'Tax group exist OEI - Drawn Dpm Applied' from OEI15 where  "VatGroup"=:Code

	-- OEI11
	union Select Top 1  'Tax group exist OEI - Drawn Dpm Detail' from OEI11 where  "VatGroup"=:Code

	-- OEI3
	union Select Top 1  'Tax group exist OEI - Freight' from OEI3 where  "VatGroup"=:Code

	-- OEI2
	union Select Top 1  'Tax group exist OEI - Freight Rows' from OEI2 where  "VatGroup"=:Code

	-- OEI1
	union Select Top 1  'Tax group exist Outgoing Excise Invoice - Rows' from OEI1 where  "VatGroup"=:Code

	-- OEI13
	union Select Top 1  'Tax group exist Outgoing Excise Invoice Rows - Distributed Expenses' from OEI13 where  "VatGroup"=:Code

	-- OVPM
	union Select Top 1  'Tax group exist Outgoing Payments' from OVPM where  "VatGroup"=:Code

	-- VPM4
	union Select Top 1  'Tax group exist Outgoing Payments - Accounts' from VPM4 where  "VatGroup"=:Code

	-- VPM5
	union Select Top 1  'Tax group exist Outgoing Payments - Vat Adjustment' from VPM5 where  "VatGroup"=:Code

	-- PMN5
	union Select Top 1  'Tax group exist Payment - VAT Adjustment' from PMN5 where  "VatGroup"=:Code

	-- OPDF
	union Select Top 1  'Tax group exist Payment Draft' from OPDF where  "VatGroup"=:Code

	-- PDF4
	union Select Top 1  'Tax group exist Payment Draft - Account List' from PDF4 where  "VatGroup"=:Code

	-- PDF5
	union Select Top 1  'Tax group exist PaymentDraft - vat adjustment' from PDF5 where  "VatGroup"=:Code

	-- POR15
	union Select Top 1  'Tax group exist PO - Drawn Dpm Applied' from POR15 where  "VatGroup"=:Code

	-- POR11
	union Select Top 1  'Tax group exist PO - Drawn Dpm Detail' from POR11 where  "VatGroup"=:Code

	-- ODPT
	union Select Top 1  'Tax group exist Postdated Deposit' from ODPT where  "CommisVat"=:Code

	-- PQT15
	union Select Top 1  'Tax group exist PQ - Drawn DPM Applied' from PQT15 where  "VatGroup"=:Code

	-- PQT11
	union Select Top 1  'Tax group exist PQ - Drawn DPM Detail' from PQT11 where  "VatGroup"=:Code

	-- PRQ15
	union Select Top 1  'Tax group exist PR - Drawn DPM Applied' from PRQ15 where  "VatGroup"=:Code

	-- PRQ11
	union Select Top 1  'Tax group exist PR - Drawn DPM Detail' from PRQ11 where  "VatGroup"=:Code

	-- POR3
	union Select Top 1  'Tax group exist Purchase Order - Freight' from POR3 where  "VatGroup"=:Code

	-- POR2
	union Select Top 1  'Tax group exist Purchase Order - Freight - Rows' from POR2 where  "VatGroup"=:Code

	-- POR1
	union Select Top 1  'Tax group exist Purchase Order - Rows' from POR1 where  "VatGroup"=:Code

	-- POR13
	union Select Top 1  'Tax group exist Purchase Order Rows - Distributed Expenses' from POR13 where  "VatGroup"=:Code

	-- PQT3
	union Select Top 1  'Tax group exist Purchase Quotation - Freight' from PQT3 where  "VatGroup"=:Code

	-- PQT2
	union Select Top 1  'Tax group exist Purchase Quotation - Freight - Rows' from PQT2 where  "VatGroup"=:Code

	-- PQT1
	union Select Top 1  'Tax group exist Purchase Quotation - Rows' from PQT1 where  "VatGroup"=:Code

	-- PQT13
	union Select Top 1  'Tax group exist Purchase Quotation Rows - Distributed Expenses' from PQT13 where  "VatGroup"=:Code

	-- PRQ3
	union Select Top 1  'Tax group exist Purchase Request - Freight' from PRQ3 where  "VatGroup"=:Code

	-- PRQ2
	union Select Top 1  'Tax group exist Purchase Request - Freight - Rows' from PRQ2 where  "VatGroup"=:Code

	-- PRQ1
	union Select Top 1  'Tax group exist Purchase Request - Rows' from PRQ1 where  "VatGroup"=:Code

	-- PRQ13
	union Select Top 1  'Tax group exist Purchase Request Rows - Distributed Expenses' from PRQ13 where  "VatGroup"=:Code
	
	-- ARC5
	union Select Top 1  'Tax group exist Reciept log vat adjustment-History' from ARC5 where  "VatGroup"=:Code

	-- RCT5
	union Select Top 1  'Tax group exist Reciept vat adjustment' from RCT5 where  "VatGroup"=:Code

	-- RDN3
	union Select Top 1  'Tax group exist Return - Freight' from RDN3 where  "VatGroup"=:Code

	-- RDN2
	union Select Top 1  'Tax group exist Return - Freight - Rows' from RDN2 where  "VatGroup"=:Code

	-- RDN15
	union Select Top 1  'Tax group exist Returns - Drawn Dpm Applied' from RDN15 where  "VatGroup"=:Code

	-- RDN11
	union Select Top 1  'Tax group exist Returns - Drawn Dpm Detail' from RDN11 where  "VatGroup"=:Code
	
	-- RDN1
	union Select Top 1  'Tax group exist Returns - Rows' from RDN1 where  "VatGroup"=:Code

	-- RDN13
	union Select Top 1  'Tax group exist Returns Rows - Distributed Expenses' from RDN13 where  "VatGroup"=:Code

	-- RDR3
	union Select Top 1  'Tax group exist Sales Order - Freight' from RDR3 where  "VatGroup"=:Code

	-- RDR2
	union Select Top 1  'Tax group exist Sales Order - Freight - Rows' from RDR2 where  "VatGroup"=:Code
	
	-- RDR1
	union Select Top 1  'Tax group exist Sales Order - Rows' from RDR1 where  "VatGroup"=:Code
	
	-- RDR13
	union Select Top 1  'Tax group exist Sales Order Rows - Distributed Expenses' from RDR13 where  "VatGroup"=:Code

	-- QUT3
	union Select Top 1  'Tax group exist Sales Quotation - Freight' from QUT3 where  "VatGroup"=:Code

	-- QUT2
	union Select Top 1  'Tax group exist Sales Quotation - Freight - Rows' from QUT2 where  "VatGroup"=:Code
	
	-- QUT1
	union Select Top 1  'Tax group exist Sales Quotation - Rows' from QUT1 where  "VatGroup"=:Code

	-- QUT13
	union Select Top 1  'Tax group exist Sales Quotation Rows - Distributed Expenses' from QUT13 where  "VatGroup"=:Code
	
	-- RDR15
	union Select Top 1  'Tax group exist Sls Ord. - Drawn Dpm Applied' from RDR15 where  "VatGroup"=:Code

	-- RDR11
	union Select Top 1  'Tax group exist Sls Ord. - Drawn Dpm Detail' from RDR11 where  "VatGroup"=:Code

	-- QUT15
	union Select Top 1  'Tax group exist Sls Quote - Drawn Dpm Applied' from QUT15 where  "VatGroup"=:Code

	-- QUT11
	union Select Top 1  'Tax group exist Sls Quote - Drawn Dpm Detail' from QUT11 where  "VatGroup"=:Code;
		
end;
end if;

select Top 1 ExistInObj  from #tbl;
	
END




