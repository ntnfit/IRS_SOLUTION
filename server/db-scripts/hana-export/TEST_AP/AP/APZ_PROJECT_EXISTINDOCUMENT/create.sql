
CREATE PROCEDURE APZ_Project_ExistInDocument(in prjCode nvarchar(20) default '')
as
begin	
	declare rowCount int=0;	
	-- OCPI
	select(select count(1) from OCPI where "Project"=:prjCode) into rowCount from DUMMY;
	if rowCount=1 then
		select 'Project  exists in A/P Correction Invoice' as ErrorMessage from DUMMY;
		return;
	end if;
	
	-- CPI3
	select(select count(1) from CPI3 where "Project"=:prjCode) into rowCount from DUMMY;
	if rowCount=1 then
		select 'Project  exists in A/P Correction Invoice - Freight' as ErrorMessage from DUMMY;
		return;
	end if;
	
	-- CPI2
	select(select count(1) from CPI2 where "Project"=:prjCode) into rowCount from DUMMY;
	if rowCount=1 then
		select 'Project  exists in A/P Correction Invoice - Freight - Rows' as ErrorMessage from DUMMY;
		return;
	end if;
	
	-- CPI1
	select(select count(1) from CPI1 where "Project"=:prjCode) into rowCount from DUMMY;
	if rowCount=1 then
		select 'Project  exists in A/P Correction Invoice - Rows' as ErrorMessage from DUMMY;
		return;
	end if;
	
	-- OCPV
	select(select count(1) from OCPV where "Project"=:prjCode) into rowCount from DUMMY;
	if rowCount=1 then
		select 'Project  exists in A/P Correction Invoice Reversal' as ErrorMessage from DUMMY;
		return;
	end if;
	
	-- CPV3
	select(select count(1) from CPV3 where "Project"=:prjCode) into rowCount from DUMMY;
	if rowCount=1 then
		select 'Project  exists in A/P Correction Invoice Reversal - Freight' as ErrorMessage from DUMMY;
		return;
	end if;
	
	-- CPV2
	select(select count(1) from CPV2 where "Project"=:prjCode) into rowCount from DUMMY;
	if rowCount=1 then
		select 'Project  exists in A/P Correction Invoice Reversal - Freight - Rows' as ErrorMessage from DUMMY;
		return;
	end if;
	
	-- CPV1
	select(select count(1) from CPV1 where "Project"=:prjCode) into rowCount from DUMMY;
	if rowCount=1 then
		select 'Project  exists in A/P Correction Invoice Reversal - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- CPV13
	select(select count(1) from CPV13 where "Project"=:prjCode) into rowCount from DUMMY;
	if rowCount=1 then
		select 'Project  exists in A/P Correction Invoice Reversal Rows - Distributed Expenses' as ErrorMessage from DUMMY;
		return;
	end if;

	-- CPI13
	select(select count(1) from CPI13 where "Project"=:prjCode) into rowCount from DUMMY;
	if rowCount=1 then
		select 'Project exists in A/P Correction Invoice Rows - Distributed Expenses' as ErrorMessage from DUMMY;
		return;
	end if;

	-- ORPC
	select(select count(1) from ORPC where "Project"=:prjCode) into rowCount from DUMMY;
	if rowCount=1 then
		select 'Project  exists in A/P Credit Memo' as ErrorMessage from DUMMY;
		return;
	end if;

	-- RPC3
	select(select count(1) from RPC3 where "Project"=:prjCode) into rowCount from DUMMY;
	if rowCount=1 then
		select 'Project  exists in A/P Credit Memo - Freight' as ErrorMessage from DUMMY;
		return;
	end if;

	-- RPC1
	select(select count(1) from RPC1 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in A/P Credit Memo - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- RPC13
	select(select count(1) from RPC13 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in A/P Credit Memo Rows - Distributed Expenses' as ErrorMessage from DUMMY;
		return;
	end if;

	-- RPC2
	select(select count(1) from RPC2 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in A/P Credit Memo Rows - Expenses' as ErrorMessage from DUMMY;
		return;
	end if;

	-- ODPO
	select(select count(1) from ODPO where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in A/P Down Payment' as ErrorMessage from DUMMY;
		return;
	end if;

	-- DPO3
	select(select count(1) from DPO3 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in A/P Down Payment - Freight' as ErrorMessage from DUMMY;
		return;
	end if;

	-- DPO2
	select(select count(1) from DPO2 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in A/P Down Payment - Freight - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- DPO1
	select(select count(1) from DPO1 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in A/P Down Payment - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- DPO13
		select(select count(1) from DPO13 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in A/P Down Payment Rows - Distributed Expenses' as ErrorMessage from DUMMY;
		return;
	end if;

	-- OPCH
	select(select count(1) from OPCH where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in A/P Invoice' as ErrorMessage from DUMMY;
		return;
	end if;

	-- PCH3
	select(select count(1) from PCH3 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in A/P Invoice - Freight' as ErrorMessage from DUMMY;
		return;
	end if;

	-- PCH2
	select(select count(1) from PCH2 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in A/P Invoice - Freight - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- PCH1
	select(select count(1) from PCH1 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in A/P Invoice - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- PCH13
	select(select count(1) from PCH13 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in A/P Invoice Rows - Distributed Expenses' as ErrorMessage from DUMMY;
		return;
	end if;

	-- CSI2
	select(select count(1) from CSI2 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in A/R Corr Inv - Freight - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- OCSI
	select(select count(1) from OCSI where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in A/R Correction Invoice' as ErrorMessage from DUMMY;
		return;
	end if;

	-- OCIN
	select(select count(1) from OCIN where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in A/R Correction Invoice' as ErrorMessage from DUMMY;
		return;
	end if;

	-- OCPI
	select(select count(1) from OCPI where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in A/P Correction Invoice' as ErrorMessage from DUMMY;
		return;
	end if;

	-- CIN3
	select(select count(1) from CIN3 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in A/R Correction Invoice - Freight' as ErrorMessage from DUMMY;
		return;
	end if;

	-- CSI3
	select(select count(1) from CSI3 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in A/R Correction Invoice - Freight' as ErrorMessage from DUMMY;
		return;
	end if;

	-- CIN2
	select(select count(1) from CIN2 where "Project"=:prjCode) into rowCount from DUMMY;
	if rowCount=1 then
		select 'Project  exists in A/R Correction Invoice - Freight - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- CSI1
	select(select count(1) from CSI1 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in A/R Correction Invoice - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- OCSV
	select(select count(1) from OCSV where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in A/R Correction Invoice Reversal' as ErrorMessage from DUMMY;
		return;
	end if;

	-- CSV3
	select(select count(1) from CSV3 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in A/R Correction Invoice Reversal - Freight' as ErrorMessage from DUMMY;
		return;
	end if;

	-- CSV2
	select(select count(1) from CSV2 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in A/R Correction Invoice Reversal - Freight - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- CSV13
	select(select count(1) from CSV13 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in A/R Correction Invoice Reversal - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- CSI13
	select(select count(1) from CSI13 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in A/R Correction Invoice Rows - Distributed Expenses' as ErrorMessage from DUMMY;
		return;
	end if;

	-- ORIN
	select(select count(1) from ORIN where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in A/R Credit Memo' as ErrorMessage from DUMMY;
		return;
	end if;

	-- RIN3
	select(select count(1) from RIN3 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in A/R Credit Memo - Freight' as ErrorMessage from DUMMY;
		return;
	end if;

	-- RIN2
	select(select count(1) from ORPC where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in A/R Credit Memo - Freight - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- RIN1
	select(select count(1) from RIN1 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in A/R Credit Memo - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- RIN13
	select(select count(1) from RIN13 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in A/R Credit Memo Rows - Distributed Expenses' as ErrorMessage from DUMMY;
		return;
	end if;

	-- ODPI
	select(select count(1) from ODPI where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in A/R Down Payment' as ErrorMessage from DUMMY;
		return;
	end if;

	-- DPI3
	select(select count(1) from DPI3 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in A/R Down Payment - Freight' as ErrorMessage from DUMMY;
		return;
	end if;

	-- DPI2
	select(select count(1) from DPI2 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in A/R Down Payment - Freight - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- DPI1
	select(select count(1) from DPI1 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in A/R Down Payment - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- DPI13
	select(select count(1) from DPI13 where "Project"=:prjCode) into rowCount from DUMMY;
	if rowCount=1 then
		select 'Project  exists in A/R Down Payment Rows - Distributed Expenses' as ErrorMessage from DUMMY;
		return;
	end if;

	-- OINV
	select(select count(1) from OINV where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in A/R Invoice' as ErrorMessage from DUMMY;
		return;
	end if;

	-- INV3
	select(select count(1) from INV3 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in A/R Invoice - Freight' as ErrorMessage from DUMMY;
		return;
	end if;

	-- ADO3
	select(select count(1) from ADO3 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in A/R Invoice - Freight - History' as ErrorMessage from DUMMY;
		return;
	end if;

	-- ADO2
	select(select count(1) from ADO2 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in A/R Invoice - Freight - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- INV2
	select(select count(1) from INV2 where "Project"=:prjCode) into rowCount from DUMMY;
	if rowCount=1 then
		select 'Project  exists in A/R Invoice - Freight - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- INV1
	select(select count(1) from INV1 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in A/R Invoice - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- ADO1
	select(select count(1) from ADO1 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in A/R Invoice (Rows) - History' as ErrorMessage from DUMMY;
		return;
	end if;

	-- ADO13
	select(select count(1) from ADO13 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in A/R Invoice Rows - Distributed Expenses' as ErrorMessage from DUMMY;
		return;
	end if;

	-- INV13
	select(select count(1) from INV13 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in A/R Invoice Rows - Distributed Freights' as ErrorMessage from DUMMY;
		return;
	end if;

	-- OMRL
	--	select(select count(1) from ORPC where "Project"=:prjCode) into rowCount from DUMMY; 
	--if rowCount=1 then
	--	select 'Project  exists in Advanced Inventory Revaluation'
	--	return;
	--end if;

	-- OAIM
	-- select(select * from OAIM where "Project"=:prjCode) into rowCount from DUMMY; 
	--if rowCount=1 then
		--select 'Project  exists in Archive Inventory Message' as ErrorMessage from DUMMY;
		--return;
	--end if;

	-- AFA1
	select(select count(1) from AFA1 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Asset Document - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- ITM5
	select(select count(1) from ITM5 where "Project"=:prjCode) into rowCount from DUMMY;
	if rowCount=1 then
		select 'Project  exists in Asset Item Projects' as ErrorMessage from DUMMY;
		return;
	end if;

	-- AIT5
	select(select count(1) from AIT5 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Asset Item Projects - History' as ErrorMessage from DUMMY;
		return;
	end if;

	-- BNK2
	select(select count(1) from BNK2 where "Project"=:prjCode) into rowCount from DUMMY;
	if rowCount=1 then
		select 'Project  exists in Bank Statement - Recommend if;ation List' as ErrorMessage from DUMMY;
		return;
	end if;

	-- ITT1
	select(select count(1) from ITT1 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Bill of Materials - Components' as ErrorMessage from DUMMY;
		return;
	end if;

	-- ATT1
	select(select count(1) from ATT1 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Bill of Materials - Components - History' as ErrorMessage from DUMMY;
		return;
	end if;

	-- OBGS
	--select(select count(1) from OBGS where "Project"=:prjCode) into rowCount from DUMMY; 
	--if rowCount=1 then
		--select 'Project  exists in Budget Scenario' as ErrorMessage from DUMMY;
		--return;
	--end if;

	-- OCRD
	--select(select count(1) from OCRD where "Project"=:prjCode) into rowCount from DUMMY;
	--if rowCount=1 then
		--select 'Project  exists in Business Partner' as ErrorMessage from DUMMY;
		--return;
	--end if;

	-- ACRD
	--select(select count(1) from ACRD where "Project"=:prjCode) into rowCount from DUMMY; 
	--if rowCount=1 then
		--select 'Project  exists in Business Partners - History' as ErrorMessage from DUMMY;
		--return;
	--end if;

	-- ACQ1
	select(select count(1) from ACQ1 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Capitalization - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- OCHH
	select(select count(1) from OCHH where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Check Register' as ErrorMessage from DUMMY;
		return;
	end if;

	-- CIN1
	select(select count(1) from CIN1 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Correction Invoice - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- CIN13
	select(select count(1) from CIN13 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Correction Invoice Rows - Distributed Expenses' as ErrorMessage from DUMMY;
		return;
	end if;

	-- OCRH
	select(select count(1) from OCRH where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Credit Card Management' as ErrorMessage from DUMMY;
		return;
	end if;

	-- ACD1
	select(select count(1) from ACD1 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Credit Memo - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- ODLN
	select(select count(1) from ODLN where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Delivery' as ErrorMessage from DUMMY;
		return;
	end if;

	-- DLN1
	select(select count(1) from DLN1 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Delivery - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- DLN3
	select(select count(1) from DLN3 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Delivery Notes - Freight' as ErrorMessage from DUMMY;
		return;
	end if;

	-- DLN2
	select(select count(1) from DLN2 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Delivery Notes - Freight - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- DLN13
	select(select count(1) from DLN13 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Delivery Rows - Distributed Expenses' as ErrorMessage from DUMMY;
		return;
	end if;

	-- ODPS
	select(select count(1) from ODPS where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Deposit' as ErrorMessage from DUMMY;
		return;
	end if;

	-- DRF3
	select(select count(1) from DRF3 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Draft - Freight' as ErrorMessage from DUMMY;
		return;
	end if;

	-- DRF2
	select(select count(1) from DRF2 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Draft - Freight - Row' as ErrorMessage from DUMMY;
		return;
	end if;

	-- DRF1
	select(select count(1) from DRF1 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Draft - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- DRF13
	select(select count(1) from DRF13 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Draft Rows - Distributed Expenses' as ErrorMessage from DUMMY;
		return;
	end if;

	-- ODRF
	select(select count(1) from ODRF where "Project"=:prjCode) into rowCount from DUMMY;
	if rowCount=1 then
		select 'Project  exists in Drafts' as ErrorMessage from DUMMY;
		return;
	end if;

	-- OBNK
	--select(select count(1) from OBNK where "Project"=:prjCode) into rowCount from DUMMY;
	--if rowCount=1 then
		--select 'Project  exists in External Bank Statement Received' as ErrorMessage from DUMMY;
		--return;
	--end if;

	-- AEXD
	select(select count(1) from AEXD where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Freight Setup' as ErrorMessage from DUMMY;
		return;
	end if;

	-- OEXD
	select(select count(1) from OEXD where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in A/P Correction Invoice' as ErrorMessage from DUMMY;
		return;
	end if;

	-- AACT
	select(select count(1) from AACT where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in G/L Account - History' as ErrorMessage from DUMMY;
		return; 
	end if;

	-- OACT
		select(select count(1) from OACT where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in G/L Accounts' as ErrorMessage from DUMMY;
		return;
	end if;

	-- GBI5
	--select(select count(1) from GBI5 where "Project"=:prjCode) into rowCount from DUMMY; 
	--if rowCount=1 then
		--select 'Project  exists in GBI Row 5 - Projects' as ErrorMessage from DUMMY;
		--return;
	--end if;

	-- OIGE
	select(select count(1) from OIGE where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Goods Issue' as ErrorMessage from DUMMY;
		return;
	end if;

	-- IGE3
	select(select count(1) from IGE3 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Goods Issue - Freight' as ErrorMessage from DUMMY;
		return;
	end if;

	-- IGE2
	select(select count(1) from IGE2 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Goods Issue - Freight - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- IGE1
	select(select count(1) from IGE1 where "Project"=:prjCode) into rowCount from DUMMY;
	if rowCount=1 then
		select 'Project  exists in Goods Issue - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- IGE13
	select(select count(1) from IGE13 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Goods Issue Rows - Distributed Expenses' as ErrorMessage from DUMMY;
		return;
	end if;

	-- OIGN
	select(select count(1) from OIGN where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Goods Receipt' as ErrorMessage from DUMMY;
		return;
	end if;

	-- IGN3
	select(select count(1) from IGN3 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Goods Receipt - Freight' as ErrorMessage from DUMMY;
		return;
	end if;

	-- IGN2
	select(select count(1) from IGN2 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Goods Receipt - Freight - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- IGN1
	select(select count(1) from IGN1 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Goods Receipt - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- OPDN
	select(select count(1) from OPDN where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Goods Receipt PO' as ErrorMessage from DUMMY;
		return;
	end if;

	-- PDN3
	select(select count(1) from PDN3 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Goods Receipt PO - Freight' as ErrorMessage from DUMMY;
		return;
	end if;

	-- PDN2
	select(select count(1) from PDN2 where "Project"=:prjCode) into rowCount from DUMMY;
	if rowCount=1 then
		select 'Project  exists in Goods Receipt PO - Freight - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- PDN1
	select(select count(1) from PDN1 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Goods Receipt PO - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- PDN13
	select(select count(1) from PDN13 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Goods Receipt PO Rows - Distributed Expenses'  as ErrorMessage from DUMMY;
		return;
	end if;

	-- IGN13
	select(select count(1) from IGN13 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Goods Receipt Rows - Distributed Expenses' as ErrorMessage from DUMMY;
		return;
	end if;

	-- ORPD
	select(select count(1) from ORPD where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Goods return' as ErrorMessage from DUMMY;
		return;
	end if;

	-- RPD3
	select(select count(1) from RPD3 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Goods return; - Freight' as ErrorMessage from DUMMY;
		return;
	end if;

	-- RPD2
	select(select count(1) from RPD2 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Goods return; - Freight - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- RPD1
	select(select count(1) from RPD1 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Goods return; - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- RPD13
	select(select count(1) from RPD13 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Goods return;s Rows - Distributed Expenses' as ErrorMessage from DUMMY;
		return;
	end if;

	-- IEI3
	select(select count(1) from IEI3 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in IEI - Freight' as ErrorMessage from DUMMY;
		return;
	end if;

	-- OIEI
	select(select count(1) from OIEI where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Incoming Excise Invoice' as ErrorMessage from DUMMY;
		return;
	end if;

	-- IEI2
	select(select count(1) from IEI2 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Incoming Excise Invoice - Freight - History - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- IEI1
	select(select count(1) from IEI1 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Incoming Excise Invoice - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- IEI13
		select(select count(1) from IEI13 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Incoming Excise Invoice Rows - Distributed Expenses' as ErrorMessage from DUMMY;
		return;
	end if;

	-- ORCT
	--select(select count(1) from ORCT where "Project"=:prjCode) into rowCount from DUMMY;
	--if rowCount=1 then
		--select 'Project  exists in Incoming Payment' as ErrorMessage from DUMMY;
		--return;
	--end if;

	-- RCT4
	select(select count(1) from RCT4 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Incoming Payment - Account List' as ErrorMessage from DUMMY;
		return;
	end if;

	-- ARC4
	select(select count(1) from ARC4 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Incoming Payment - Account List - History' as ErrorMessage from DUMMY;
		return;
	end if;

	-- RCC4
	select(select count(1) from RCC4 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Incoming Payment - Credit Vouchers' as ErrorMessage from DUMMY;
		return;
	end if;

	-- ARCT
	--select(select count(1) from ARCT where "Project"=:prjCode) into rowCount from DUMMY; 
	--if rowCount=1 then
		--select 'Project  exists in Incoming Payment - History' as ErrorMessage from DUMMY;
		--return;
	--end if;

	-- OBTC
	--select(select count(1) from OBTC where "Project"=:prjCode) into rowCount from DUMMY; 
	--if rowCount=1 then
		--select 'Project  exists in Internal Bank Operation Codes' as ErrorMessage from DUMMY;
		--return;
	--end if;

	-- BTC1
	select(select count(1) from BTC1 where "Project"=:prjCode) into rowCount from DUMMY;
	if rowCount=1 then
		select 'Project  exists in Internal Bank Operation Codes - Accounts' as ErrorMessage from DUMMY;
		return;
	end if;

	-- ABT1
	select(select count(1) from ABT1 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Internal Bank Operation Codes - Accounts - Log' as ErrorMessage from DUMMY;
		return;
	end if;

	-- ABTC
	--select(select count(1) from ABTC where "Project"=:prjCode) into rowCount from DUMMY; 
	--if rowCount=1 then
		--select 'Project  exists in Internal Bank Operation Codes - Log' as ErrorMessage from DUMMY;
		--return;
	--end if;

	-- AIN1
	--select(select count(1) from AIN1 where "Project"=:prjCode) into rowCount from DUMMY; 
	--if rowCount=1 then
		--select 'Project  exists in Inventory Counting - Rows' as ErrorMessage from DUMMY;
		--return;
	--end if;

	-- INC1
	--select(select count(1) from INC1 where "Project"=:prjCode) into rowCount from DUMMY; 
	--if rowCount=1 then
		--select 'Project  exists in Inventory Counting - Rows' as ErrorMessage from DUMMY;
		--return;
	--end if;

	-- ICD1
	--select(select count(1) from ICD1 where "Project"=:prjCode) into rowCount from DUMMY; 
	--if rowCount=1 then
		--select 'Project  exists in Inventory Counting Draft - Rows' as ErrorMessage from DUMMY;
		--return;
	--end if;

	-- IOD1
	select(select count(1) from IOD1 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Inventory Initial Qty Draft Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- AQI1
	select(select count(1) from AQI1 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Inventory Initial Qty Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- IQI1
	select(select count(1) from IQI1 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Inventory Initial Qty Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- OILM
	--select(select count(1) from OILM where "Project"=:prjCode) into rowCount from DUMMY; 
	--if rowCount=1 then
		--select 'Project  exists in Inventory Log Message' as ErrorMessage from DUMMY;
		--return;
	--end if;

	-- AMR1
	select(select count(1) from AMR1 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Inventory Revaluation - History - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- MRV1
	select(select count(1) from MRV1 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Inventory Revaluation Information Array' as ErrorMessage from DUMMY;
		return;
	end if;

	-- IQR1
	select(select count(1) from IQR1 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Inventory Stock Posting (Reconcile) Lines' as ErrorMessage from DUMMY;
		return;
	end if;

	-- AQR1
	select(select count(1) from AQR1 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Inventory Stock Posting (Reconcile) Lines' as ErrorMessage from DUMMY;
		return;
	end if;

	-- IPD1
	select(select count(1) from IPD1 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Inventory Stock Posting Draft (Reconcile) Lines' as ErrorMessage from DUMMY;
		return;
	end if;

	-- OWTR
	select(select count(1) from OWTR where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Inventory Transfer' as ErrorMessage from DUMMY;
		return;
	end if;

	-- WTR3
	select(select count(1) from WTR3 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Inventory Transfer - Freight' as ErrorMessage from DUMMY;
		return;
	end if;

	-- WTR2
	select(select count(1) from WTR2 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Inventory Transfer - Freight - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- WTR1
	select(select count(1) from WTR1 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Inventory Transfer - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- OWTQ
	select(select count(1) from OWTQ where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Inventory Transfer Request' as ErrorMessage from DUMMY;
		return;
	end if;

	-- WTQ1
	select(select count(1) from WTQ1 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Inventory Transfer Request - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- WTQ13
	select(select count(1) from WTQ13 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Inventory Transfer Request Rows - Distributed Freights' as ErrorMessage from DUMMY;
		return;
	end if;

	-- WTR13
	select(select count(1) from WTR13 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Inventory Transfer Rows - Distributed Freights' as ErrorMessage from DUMMY;
		return;
	end if;

	-- ADOC
	select(select count(1) from ADOC where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Invoice - History' as ErrorMessage from DUMMY;
		return;
	end if;

	-- UILM
	--select(select count(1) from UILM where "Project"=:prjCode) into rowCount from DUMMY; 
	--if rowCount=1 then
		--select 'Project  exists in IVI Inventory Log Message' as ErrorMessage from DUMMY;
		--return;
	--end if;

	-- OJDT
	select(select count(1) from OJDT where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Journal Entry' as ErrorMessage from DUMMY;
		return;
	end if;

	-- AJDT
	select(select count(1) from AJDT where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Journal Entry - History' as ErrorMessage from DUMMY;
		return;
	end if;

	-- AJD1
	select(select count(1) from AJD1 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Journal Entry - History - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- JDT1
	select(select count(1) from JDT1 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Journal Entry - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- BTF1
	select(select count(1) from BTF1 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Journal Voucher - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- OBTF
	select(select count(1) from OBTF where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Journal Voucher Entry' as ErrorMessage from DUMMY;
		return;
	end if;

	-- IPF1
	select(select count(1) from IPF1 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Landed Costs - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- MDP1
	select(select count(1) from MDP1 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Manual Depreciation - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- OEI3
	select(select count(1) from OEI3 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in OEI - Freight' as ErrorMessage from DUMMY;
		return;
	end if;

	-- OEI2
	select(select count(1) from OEI2 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in OEI - Freight Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- OOEI
	select(select count(1) from OOEI where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Outgoing Excise Invoice' as ErrorMessage from DUMMY;
		return;
	end if;

	-- OEI1
	select(select count(1) from OEI1 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Outgoing Excise Invoice - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- OEI13
	select(select count(1) from OEI13 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Outgoing Excise Invoice Rows - Distributed Expenses' as ErrorMessage from DUMMY;
		return;
	end if;

	-- OVPM
	--select(select count(1) from OVPM where "Project"=:prjCode) into rowCount from DUMMY;
	--if rowCount=1 then
		--select 'Project  exists in Outgoing Payments' as ErrorMessage from DUMMY;
		--return;
	--end if;

	-- VPM4
	select(select count(1) from VPM4 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Outgoing Payments - Accounts' as ErrorMessage from DUMMY;
		return;
	end if;

	-- OPDF
	--select(select count(1) from OPDF where "Project"=:prjCode) into rowCount from DUMMY; 
	--if rowCount=1 then
		--select 'Project  exists in Payment Draft' as ErrorMessage from DUMMY;
		--return;
	--end if;

	-- PDF4
	select(select count(1) from PDF4 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Payment Draft - Account List' as ErrorMessage from DUMMY;
		return;
	end if;

	-- PWZ3
	--select(select count(1) from PWZ3 where "Project"=:prjCode) into rowCount from DUMMY;
	--if rowCount=1 then
		--select 'Project  exists in Payment Wizard - Rows 3' as ErrorMessage from DUMMY;
		--return;
	--end if;

	-- ODPT
	select(select count(1) from ODPT where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Postdated Deposit' as ErrorMessage from DUMMY;
		return;
	end if;

	-- TRT1
	select(select count(1) from TRT1 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Posting Templates - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- OITT
	select(select count(1) from OITT where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Product Tree' as ErrorMessage from DUMMY;
		return;
	end if;

	-- AITT
	select(select count(1) from AITT where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then 
		select 'Project  exists in Product Tree - History' as ErrorMessage from DUMMY;
		return;
	end if;

	-- OWOR
	select(select count(1) from OWOR where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Production Order' as ErrorMessage from DUMMY;
		return;
	end if;

	-- UWOR
	select(select count(1) from UWOR where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Production Order' as ErrorMessage from DUMMY;
		return;
	end if;

	-- AWOR
	select(select count(1) from AWOR where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Production Order - History' as ErrorMessage from DUMMY;
		return;
	end if;

	-- UWOR1
	select(select count(1) from UWOR1 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Production Order - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- WOR1
	select(select count(1) from WOR1 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Production Order - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- AWO1
	select(select count(1) from AWO1 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Production Order (Rows) - History' as ErrorMessage from DUMMY;
		return;
	end if;

	-- OPOR
	select(select count(1) from OPOR where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Purchase Order' as ErrorMessage from DUMMY;
		return;
	end if;

	-- POR3
	select(select count(1) from POR3 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Purchase Order - Freight' as ErrorMessage from DUMMY;
		return;
	end if;

	-- POR2
	select(select count(1) from POR2 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Purchase Order - Freight - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- POR1
	select(select count(1) from POR1 where "Project"=:prjCode) into rowCount from DUMMY;
	if rowCount=1 then
		select 'Project  exists in Purchase Order - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- POR13
	select(select count(1) from POR13 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then 
		select 'Project  exists in Purchase Order Rows - Distributed Expenses' as ErrorMessage from DUMMY;
		return;
	end if;

	-- OPQT
	select(select count(1) from OPQT where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Purchase Quotation' as ErrorMessage from DUMMY;
		return;
	end if;

	-- PQT3
	select(select count(1) from PQT3 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Purchase Quotation - Freight' as ErrorMessage from DUMMY;
		return;
	end if;

	-- PQT2
	select(select count(1) from PQT2 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Purchase Quotation - Freight - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- PQT1
	select(select count(1) from PQT1 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Purchase Quotation - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- PQT13
	select(select count(1) from PQT13 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Purchase Quotation Rows - Distributed Expenses' as ErrorMessage from DUMMY;
		return;
	end if;

	-- OPRQ
	select(select count(1) from OPRQ where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Purchase Request' as ErrorMessage from DUMMY;
		return;
	end if;

	-- PRQ3
	select(select count(1) from PRQ3 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Purchase Request - Freight' as ErrorMessage from DUMMY;
		return;
	end if;

	-- PRQ2
	select(select count(1) from PRQ2 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Purchase Request - Freight - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- PRQ1
	select(select count(1) from PRQ1 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Purchase Request - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- PRQ13
	select(select count(1) from PRQ13 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Purchase Request Rows - Distributed Expenses'  as ErrorMessage from DUMMY;
		return;
	end if;

	-- RCR1
	select(select count(1) from RCR1 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Recurring Postings - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- OFLT
	--select(select count(1) from OFLT where "Project"=:prjCode) into rowCount from DUMMY; 
	--if rowCount=1 then
		--select 'Project  exists in Report - Selection Criteria' as ErrorMessage from DUMMY;
		--return;
	--end if;

	-- RTI1
	select(select count(1) from RTI1 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Retirement - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- RDN3
	select(select count(1) from RDN3 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in return; - Freight' as ErrorMessage from DUMMY;
		return;
	end if;

	-- RDN2
	select(select count(1) from RDN2 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in return; - Freight - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- ORDN
	select(select count(1) from ORDN where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in return;s' as ErrorMessage from DUMMY;
		return;
	end if;

	-- RDN1
	select(select count(1) from RDN1 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in return;s - Rows'  as ErrorMessage from DUMMY;
		return;
	end if;

	-- RDN13
	select(select count(1) from RDN13 where "Project"=:prjCode) into rowCount from DUMMY;
	if rowCount=1 then
		select 'Project  exists in return;s Rows - Distributed Expenses' as ErrorMessage from DUMMY;
		return;
	end if;

	-- AOPR
	--select(select count(1) from AOPR where "Project"=:prjCode) into rowCount from DUMMY; 
	--if rowCount=1 then
		--select 'Project  exists in Sales Opportunity'  as ErrorMessage from DUMMY;
		--return;
	--end if;

	-- OOPR
	--select(select count(1) from OOPR where "Project"=:prjCode) into rowCount from DUMMY; 
	--if rowCount=1 then
		--select 'Project  exists in Sales Opportunity' as ErrorMessage from DUMMY;
		--return;
	--end if;

	-- ORDR
	select(select count(1) from ORDR where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Sales Order' as ErrorMessage from DUMMY;
		return;
	end if;

	-- RDR3
	select(select count(1) from RDR3 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Sales Order - Freight' as ErrorMessage from DUMMY;
		return; 
	end if;

	-- RDR2
	select(select count(1) from RDR2 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Sales Order - Freight - Rows' as ErrorMessage from DUMMY;	 
		return;
	end if;

	-- RDR1
	select(select count(1) from RDR1 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Sales Order - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- RDR13
	select(select count(1) from RDR13 where "Project"=:prjCode) into rowCount from DUMMY;
	if rowCount=1 then
		select 'Project  exists in Sales Order Rows - Distributed Expenses' as ErrorMessage from DUMMY;
		return;
	end if;

	-- OQUT
	select(select count(1) from OQUT where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Sales Quotation' as ErrorMessage from DUMMY;
		return;
	end if;

	-- QUT3
	select(select count(1) from QUT3 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Sales Quotation - Freight' as ErrorMessage from DUMMY;
		return;
	end if;

	-- QUT2
	select(select count(1) from QUT2 where "Project"=:prjCode) into rowCount from DUMMY;
	if rowCount=1 then
		select 'Project  exists in Sales Quotation - Freight - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- QUT1
	select(select count(1) from QUT1 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Sales Quotation - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- QUT13
	select(select count(1) from QUT13 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Sales Quotation Rows - Distributed Expenses' as ErrorMessage from DUMMY;
		return;
	end if;

	-- FTR1
	select(select count(1) from FTR1 where "Project"=:prjCode) into rowCount from DUMMY; 
	if rowCount=1 then
		select 'Project  exists in Transfer - Rows' as ErrorMessage from DUMMY;
		return;
	end if;

	-- OINM
	--select(select count(1) from OINM where "Project"=:prjCode) into rowCount from DUMMY; 
	--if rowCount=1 then
		--select 'Project  exists in Whse Journal' as ErrorMessage from DUMMY;
		--return;
	--end if;
end;




