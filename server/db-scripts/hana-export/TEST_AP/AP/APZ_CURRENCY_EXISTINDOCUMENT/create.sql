
CREATE PROCEDURE APZ_Currency_ExistInDocument (
	IN 	CurrCode nvarchar(3) DEFAULT ''
)
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER

AS
BEGIN
	DECLARE tableDocCurrCheck nvarchar(5000);
	DECLARE tableDocCheck nvarchar(5000);
	DECLARE tableLogCheck nvarchar(5000);
	DECLARE tableLineCheck nvarchar(5000);
	DECLARE tableCreditCurCheck nvarchar(5000);
	DECLARE tableActCurrCheck nvarchar(5000);
	DECLARE tableItemLogCheck nvarchar(5000);
	
	DECLARE tableHistoryCheck nvarchar(5000);
	DECLARE tableWTCurrencyCheck nvarchar(5000);
	
	DECLARE sqlStr NCLOB:=''; 
	
	IF CurrCode <> '' THEN
		tableDocCurrCheck := 'ORCT,OVPM,OPDF';
		--DocCur
		tableDocCheck := 'OCPI,ORPC,OCPV,ODPO,OPCH,OCIN,OCSI,OCSV,ORIN,ODPI,OINV,ODLN,ODRF,OIGE,OIGN,OPDN,ORPD,OIEI,OWTR,OWTQ,OOEI,OPOR,OPQT,OPRQ,ORDN,ORDR,OQUT';
		--FixCurr
		tableLogCheck := 'CPI3,CPI2,CPV3,CPV2,CPV13,CPI13,RPC3,RPC13,RPC2,DPO3,DPO2,DPO13,PCH3,PCH2,PCH13,CSI2,CIN3,CSI3,CIN2,CSV3,CSV2,CSV13,CSI13,RIN3,RIN2,RIN13,DPI3,DPI2,DPI13,INV3,INV2,INV13,CIN13,DLN3,DLN2,DLN13,DRF3,DRF2,DRF13,IGE3,IGE2,IGE13,IGN3,IGN2,PDN3,PDN2,PDN13,IGN13,RPD3,RPD2,RPD13,IEI3,IEI2,IEI13,IGN20,POR20,CIN20,RPD20,DPI20,DRF20,INV20,WTQ20,PCH20,QUT20,RIN20,CPV20,IGE20,RDN20,PQT20,OEI20,CSV20,IEI20,DOC20,PDN20,CPI20,RPC20,PRQ20,DPO20,RDR20,WTR20,CSI20,DLN20,WTR3,WTR2,WTQ3,WTQ2,WTQ13,WTR13,OEI3,OEI2,OEI13,POR3,POR2,POR13,PQT3,PQT2,PQT13,PRQ3,PRQ2,PRQ13,RDN3,RDN2,RDN13,RDR3,RDR2,RDR13,QUT3,QUT2,QUT13';
		--Currency
		tableLineCheck := 'CPI1,CPV1,RPC1,DPO1,PCH1,CSI1,CSV1,RIN1,DPI1,INV1,OAIM,OBNH,OBOE,ITT1,OCRD,CIN1,OACD,DRF1,IEI1,RCT1,OEI1,VPM1,PDF1,PQT1,QUT1';
		--CreditCur
		tableCreditCurCheck := 'RCT3,VPM3,PDF3';
		--ActCurr
		tableActCurrCheck := 'OACT';
		--Currency=@CurrCode or PriceCurr=@CurrCode or DocPrcCurr=@CurrCode
		tableItemLogCheck := 'OILM';
		
		tableHistoryCheck := 'ITM9,ITM1,RSC2';
		--WTCurrency
		tableWTCurrencyCheck := 'OWHT';
		CREATE LOCAL TEMPORARY TABLE #TEMP_GET_TABLE ( "TableName" NVARCHAR(10));
		
		sqlStr := :sqlStr || 'INSERT INTO #TEMP_GET_TABLE SELECT TOP 1 ''OAMD'' FROM OAMD WHERE "RpCurrency"='''|| :CurrCode || '''
			';
		sqlStr := :sqlStr || 'UNION SELECT TOP 1 ''AMD1'' FROM AMD1 WHERE "FCCurrency"='''|| :CurrCode || '''
			';
		sqlStr := :sqlStr || 'UNION SELECT TOP 1 ''BOE1'' FROM BOE1 WHERE "LineCurr"='''|| :CurrCode || '''
			';
		sqlStr := :sqlStr || 'UNION SELECT TOP 1 ''OBNK'' FROM OBNK WHERE "DebAmntCur"='''|| :CurrCode || '''
			';		
		
		SELECT :sqlStr || STRING_AGG('UNION SELECT TOP 1 ''' || DATA || ''' FROM ' || DATA || ' WHERE "DocCur"='''|| :CurrCode || '''
			' ,' ') INTO sqlStr FROM Split(:tableDocCheck,',');	
			
		SELECT  :sqlStr || STRING_AGG('UNION SELECT TOP 1 ''' || DATA || ''' FROM ' || DATA || ' WHERE "DocCurr"='''|| :CurrCode || '''
			' ,' ') INTO sqlStr FROM Split(:tableDocCurrCheck,',');
		--FixCurr
		SELECT  :sqlStr || STRING_AGG('UNION SELECT TOP 1 ''' || DATA || ''' FROM ' || DATA || ' WHERE "FixCurr"='''|| :CurrCode || '''
			' ,' ') INTO sqlStr FROM Split(:tableLogCheck,',');
		
		--Currency
		SELECT  :sqlStr || STRING_AGG('UNION SELECT TOP 1 ''' || DATA || ''' FROM ' || DATA || ' WHERE "Currency"='''|| :CurrCode || '''
			' ,' ') INTO sqlStr FROM Split(:tableLineCheck,',');
		
		--CreditCur
		SELECT  :sqlStr || STRING_AGG('UNION SELECT TOP 1 ''' || DATA || ''' FROM ' || DATA || ' WHERE "CreditCur"='''|| :CurrCode || '''
			' ,' ') INTO sqlStr FROM Split(:tableCreditCurCheck,',');
		
		--ActCurr
		SELECT  :sqlStr || STRING_AGG('UNION SELECT TOP 1 ''' || DATA || ''' FROM ' || DATA || ' WHERE "ActCurr"='''|| :CurrCode || '''
			' ,' ') INTO sqlStr FROM Split(:tableActCurrCheck,',');
		
		--DocPrcCurr
		SELECT  :sqlStr || STRING_AGG('UNION SELECT TOP 1 ''' || DATA || ''' FROM ' || DATA || ' WHERE "DocPrcCurr"='''|| :CurrCode || '''
			' ,' ') INTO sqlStr FROM Split(:tableItemLogCheck,',');
		
		--Currency2
		SELECT  :sqlStr || STRING_AGG('UNION SELECT TOP 1 ''' || DATA || ''' FROM ' || DATA || ' WHERE "Currency2"='''|| :CurrCode || '''
			' ,' ') INTO sqlStr FROM Split(:tableHistoryCheck,',');
		
		--WTCurrency
		SELECT  :sqlStr || STRING_AGG('UNION SELECT TOP 1 ''' || DATA || ''' FROM ' || DATA || ' WHERE "WTCurrency"='''|| :CurrCode || '''
			' ,' ') INTO sqlStr FROM Split(:tableWTCurrencyCheck,',');
		
		sqlStr := :sqlStr || ';';
		--SELECT sqlStr FROM DUMMY;	
		EXEC sqlStr;	
		SELECT STRING_AGG("TableName",',') "TableName" FROM #TEMP_GET_TABLE;
		DROP TABLE #TEMP_GET_TABLE;
		
	END IF;
	
END;




