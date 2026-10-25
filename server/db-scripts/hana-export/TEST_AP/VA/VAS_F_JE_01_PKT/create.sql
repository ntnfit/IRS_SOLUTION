CREATE procedure VAS_F_JE_01_PKT (
	in DocID	int
)
as
begin
	declare bplID int;
	declare DecQty, DecPrice int;
	declare DateSep, ThousSep nvarchar(10);
	declare num1, num2 numeric(21,6);
	declare tCompName, tCompAddr, tCompTel, tCompFax, tCompWebsite, tCompEmail nvarchar(100);
	declare listAcc varchar(500);
	
	select top 1 ifnull(x."BPLId",0) into bplID from "JDT1" x where x."TransId"=DocID;
	----------------------------------------------------------------------------------------------------------------------------
	create local temporary table #tbl (
		CompName 	nvarchar(300), 
		CompAddr 	nvarchar(300), 
		DateText 	nvarchar(100), 
		DocNum 		nvarchar(20), 
		Acc 		nvarchar(20), 
		BPCode 		nvarchar(20), 
		BPName 		nvarchar(300), 
		DebAmount 	numeric(21,6),
		CredAmount 	numeric(21,6),
		Express 	nvarchar(300),
		D1Code 		nvarchar(10), 
		D2Code 		nvarchar(10), 
		D3Code 		nvarchar(10), 
		D4Code 		nvarchar(10), 
		D5Code 		nvarchar(10), 
		PrjCode 	nvarchar(10),
		D1Name 		nvarchar(99), 
		D2Name 		nvarchar(99), 
		D3Name 		nvarchar(99), 
		D4Name 		nvarchar(99), 
		D5Name 		nvarchar(99), 
		PrjName 	nvarchar(99)
	);	
	-- REPORT ----------------------------------------------------------------------------------------------------------------
	insert into #tbl (CompName,CompAddr,DateText,DocNum,Acc,BPCode,BPName,DebAmount,CredAmount,Express,D1Code,D2Code,D3Code,D4Code,D5Code,PrjCode)
	SELECT 
		b."CompnyName", b."CompnyAddr", 'Ngày ' || to_char(a."RefDate",'DD') || ' tháng ' || to_char(a."RefDate",'MM') || ' năm ' || to_char(a."RefDate",'YYYY'), 
		CAST("TransId" AS nvarchar(20)), a."Account", c."CardCode", c."CardName", "Debit", "Credit", "LineMemo", a."ProfitCode", a."OcrCode2", a."OcrCode3", a."OcrCode4", a."OcrCode5", a."Project" 
	FROM "JDT1" a 
	LEFT OUTER JOIN "OADM" b ON 1 = 1 
	LEFT OUTER JOIN "OCRD" c ON c."CardCode" = a."ShortName" 
	WHERE a."TransId" = DocID;

	update #tbl set D1Name = (select x."OcrName" from "OOCR" x where x."OcrCode" = D1Code);
	update #tbl set D2Name = (select x."OcrName" from "OOCR" x where x."OcrCode" = D2Code);
	update #tbl set D3Name = (select x."OcrName" from "OOCR" x where x."OcrCode" = D3Code);
	update #tbl set D4Name = (select x."OcrName" from "OOCR" x where x."OcrCode" = D4Code);
	update #tbl set D5Name = (select x."OcrName" from "OOCR" x where x."OcrCode" = D5Code);
	update #tbl set PrjName = (select x."PrjName" from "OPRJ" x where x."PrjCode"=#tbl.PrjCode);
	
	select 
		CompName  "CompName",
		CompAddr  "CompAddr",
		DateText  "DateText",
		DocNum  "DocNum",
		Acc  "Acc",
		BPCode  "BPCode",
		BPName  "BPName",
		DebAmount  "DebAmount",
		CredAmount  "CredAmount",
		Express  "Express",
		D1Code  "D1Code",
		D2Code  "D2Code",
		D3Code  "D3Code",
		D4Code  "D4Code",
		D5Code  "D5Code",
		PrjCode  "PrjCode",
		D1Name  "D1Name",
		D2Name  "D2Name",
		D3Name  "D3Name",
		D4Name  "D4Name",
		D5Name  "D5Name",
		PrjName  "PrjName"
	from #tbl;

	drop table #tbl;
end

