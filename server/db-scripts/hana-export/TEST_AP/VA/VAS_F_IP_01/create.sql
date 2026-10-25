CREATE PROCEDURE VAS_F_IP_01 (
	in docKey	int
)
AS
BEGIN
	declare branch int;
	select IFNULL(a."BPLId",0) into branch from ORCT a where a."DocEntry" = :docKey;
	
	create local temporary table #tbl (
		CompName			nvarchar(200)
		,CompAddr			nvarchar(254)
		,RptName			nvarchar(100)
		,RptNameEN			nvarchar(100)
		,DocNum				int
		,DocDate			timestamp
		,BPCode				nvarchar(50)
		,BPName				nvarchar(254)
		,BPAddr				nvarchar(254)
		,JVNum				nvarchar(50)
		,DocTotal			numeric(19,6)
		,DocRate			numeric(19,6)
		,DocCurr			nvarchar(10)
		,Comments			nvarchar(254)
		,ValInText			nvarchar(500)
		,CounterRef			nvarchar(40)
		,Ref				nvarchar(100)
		,GiamDoc			nvarchar(200)
		,KeToanTruong		nvarchar(200)
		,ThuQuy				nvarchar(200)
		,NguoiLap			nvarchar(200)
		,DeciAmt			int
		,DeciRate			int
		,DecSep				varchar(1)
		,ThousSep			varchar(1)
		,Logo				blob
	);
	insert into #tbl (DocNum,DocDate,BPCode,BPName,BPAddr,JVNum,DocTotal,DocRate,DocCurr,Comments,CounterRef,Ref
					  ,CompName,CompAddr,DeciAmt,DeciRate,DecSep,ThousSep,RptName,RptNameEN)
	select
		a."DocNum"
		,a."DocDate"
		,a."CardCode"
		,CASE when a."DocType" <> 'A' and LTRIM(IFNULL(a."U_Payer",''))='' then a."CardName" else a."U_Payer" end
		,CASE when IFNULL(a."U_PayAdd",'')='' then REPLACE(a."Address", 'VIETNAM', '') else a."U_PayAdd" end
		,'RC '|| cast(a."DocNum" as nvarchar(20))
		,case when a."DocCurr"=b."MainCurncy" then a."DocTotal" else a."DocTotalFC" end
		,a."DocRate"
		,a."DocCurr"
		,a."Comments"
		,IFNULL(a."U_JVNumber",cast(a."DocNum" as nvarchar))
		,''
		,b."CompnyName"
		,replace(b."CompnyAddr",'VIETNAM','')
		,b."SumDec"
		,2
		,b."DecSep"
		,b."ThousSep"
		,case when a."TrsfrAcct" like '112%' and IFNULL(a."TrsfrSum",0) !=0 then N'ỦY NHIỆM THU' else N'PHIẾU THU' end
		,case when a."TrsfrAcct" like '112%' and IFNULL(a."TrsfrSum",0) !=0 then 'RECEIPT VOUCHER' else 'INCOMING PAYMENT' end
	from 
		ORCT a,OADM b
	where
		a."DocEntry" =:docKey;
	
	update #tbl set Ref = case when IFNULL(Ref,'')='' then cast((select sum(x.Ref) from (select count(a."DocLine") Ref from RCT2 a where a."DocNum"=:docKey union all select count(a."LineId") from RCT4 a where a."DocNum"=:docKey) x) as nvarchar) else Ref end; 
	update #tbl set ValInText =  Num2Text(cast(DocTotal as bigint)) || ' ' || IFNULL((select x."ChkName" from OCRN x where x."CurrCode"=#tbl.DocCurr),'') 
								 || case when DocTotal = Round(DocTotal,0) then '' else N' và ' || lower(Num2Text(cast((DocTotal-ROUND(DocTotal-0.5,0))*100 as int))) || ' ' || IFNULL((select x."Chk100Name" from OCRN x where x."CurrCode"=#tbl.DocCurr),'') end;
	update b set Logo = a."LogoImage" from OADP a,#tbl b;
	update b set DeciAmt = case when a."Decimals"=-1 then b.DeciAmt else a."Decimals" end from OCRN a,#tbl b where b.DocCurr=a."CurrCode";
	update b set CompName = IFNULL(a."BPLName",CompName), CompAddr = IFNULL(a."Address",CompAddr) from OBPL a,#tbl b where a."BPLId"=:branch;
	
--	update b set GiamDoc = a."U_GiamDoc", KeToanTruong = a."U_KeToanTruong", ThuQuy = a."U_ThuQuy", NguoiLap = a."U_NguoiLap" from "@DOCSIGN" a, #tbl b where a."Code" = 'GEN';
	
	select * from #tbl;
	drop table #tbl;
END;

