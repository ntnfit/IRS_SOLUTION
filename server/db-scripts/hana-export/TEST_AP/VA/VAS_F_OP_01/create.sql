CREATE PROCEDURE VAS_F_OP_01 (
	in objtype	nvarchar(20),
	in docKey	int
)
AS
BEGIN
	declare branch int;
--	select IFNULL(a."BPLId",0) into branch from OVPM a where a."DocEntry" = :docKey;
	
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
	if :objtype='46' then
		insert into #tbl (DocNum,DocDate,BPCode,BPName,BPAddr,JVNum,DocTotal,DocRate,DocCurr,Comments,CounterRef,Ref
						  ,CompName,CompAddr,DeciAmt,DeciRate,DecSep,ThousSep,RptName,RptNameEN)
		select
			a."DocNum"
			,a."DocDate"
			,a."CardCode"
			,CASE when a."DocType" <> 'A' and LTRIM(IFNULL(a."U_Payer",''))='' then a."CardName" else a."U_Payer" end
			,CASE when IFNULL(a."U_PayAdd",'')='' then REPLACE(a."Address", 'VIETNAM', '') else a."U_PayAdd" end
			,'PS '|| cast(a."DocNum" as nvarchar(20))
			,case when a."DocCurr"=b."MainCurncy" then a."DocTotal" else a."DocTotalFC" end
			,a."DocRate"
			,a."DocCurr"
			,a."Comments"
			,a."U_JVNumber"
			,''
			,b."CompnyName"
			,replace(b."CompnyAddr",'VIETNAM','')
			,b."SumDec"
			,2
			,b."DecSep"
			,b."ThousSep"
			,case when a."TrsfrAcct" like '112%' and IFNULL(a."TrsfrSum",0) !=0 then N'ỦY NHIỆM CHI' else N'PHIẾU CHI' end
			,case when a."TrsfrAcct" like '112%' and IFNULL(a."TrsfrSum",0) !=0 then 'PAYMENT VOUCHER' else 'OUTGOING PAYMENT' end
		from 
			OVPM a,OADM b
		where
			a."DocEntry" =:docKey;
		
		update #tbl set Ref = case when IFNULL(Ref,'')='' then cast((select sum(x.Ref) from (select count(a."DocLine") Ref from VPM2 a where a."DocNum"=:docKey union all select count(a."LineId") from VPM4 a where a."DocNum"=:docKey) x) as nvarchar) else Ref end; 

	else if :objtype='30' then
		create local temporary table #JE (JEID int, Account nvarchar(10), DocDate timestamp, BPCode nvarchar(20), BPName nvarchar(100), BPAddr nvarchar(100), JVNum nvarchar(50), DocTotal numeric(28,10), DocTotalFC numeric(28,10), DocRate numeric(28,10), DocCurr nvarchar(10), Comments nvarchar(300));
		insert into #JE (JEID,JVNum,DocDate,Comments,Account,DocTotal,DocTotalFC,DocCurr)
		select a."TransId",a."U_JVNumber",a."RefDate",a."Memo",max(left(b."Account",3)),sum(b."Credit"),sum(b."FCCredit"),max(b."FCCurrency") from OJDT a, JDT1 b where a."TransId"=b."TransId" and a."TransId"=:docKey and (b."Account" like '112%' or b."Account" like '111%') and b."Credit">0 group by a."TransId",a."U_JVNumber",a."RefDate",a."Memo";
		
		update x set BPCode = z."CardCode", BPName = z."CardName", BPAddr = z."Address" from #JE x inner join JDT1 y on y."TransId"=x.JEID and y."ShortName"<>y."Account" inner join OCRD z on z."CardCode"=y."ShortName";
		update #JE set DocRate = DocTotal/DocTotalFC where ifnull(DocTotalFC,0)<>0;
		
		insert into #tbl (DocNum,DocDate,BPCode,BPName,BPAddr,JVNum,DocTotal,DocRate,DocCurr,Comments,CounterRef,Ref
						  ,CompName,CompAddr,DeciAmt,DeciRate,DecSep,ThousSep,RptName,RptNameEN)
		select 
			a.JEID
			,a.DocDate
			,a.BPCode
			,a.BPName
			,a.BPAddr
			,'JE '|| cast(a.JEID as nvarchar(20))
			,case when a.DocTotalFC=0 then a.DocTotal else a.DocTotalFC end
			,a.DocRate
			,a.DocCurr
			,a.Comments
			,a.JVNum
			,''
			,b."CompnyName"
			,replace(b."CompnyAddr",'VIETNAM','')
			,b."SumDec"
			,2
			,b."DecSep"
			,b."ThousSep"
			,case when a.Account like '112%' then N'ỦY NHIỆM CHI' else N'PHIẾU CHI' end
			,case when a.Account like '112%' then 'PAYMENT VOUCHER' else 'OUTGOING PAYMENT' end
		from #JE a, OADM b;
		
		drop table #JE;
	end if;	
	end if;
	
	update #tbl set ValInText =  Num2Text(cast(DocTotal as bigint)) || ' ' || IFNULL((select x."ChkName" from OCRN x where x."CurrCode"=#tbl.DocCurr),'') 
								 || case when DocTotal = Round(DocTotal,0) then '' else N' và ' || lower(Num2Text(cast((DocTotal-ROUND(DocTotal-0.5,0))*100 as int))) || ' ' || IFNULL((select x."Chk100Name" from OCRN x where x."CurrCode"=#tbl.DocCurr),'') end;
	update b set Logo = a."LogoImage" from OADP a,#tbl b;
	update b set DeciAmt = case when a."Decimals"=-1 then b.DeciAmt else a."Decimals" end from OCRN a,#tbl b where b.DocCurr=a."CurrCode";
	update b set CompName = IFNULL(a."BPLName",CompName), CompAddr = IFNULL(a."Address",CompAddr) from OBPL a,#tbl b where a."BPLId"=:branch;
	
--	update b set GiamDoc = a."U_GiamDoc", KeToanTruong = a."U_KeToanTruong", ThuQuy = a."U_ThuQuy", NguoiLap = a."U_NguoiLap" from "@DOCSIGN" a, #tbl b where a."Code" = 'GEN';
	
	select * from #tbl;
	drop table #tbl;
END;

