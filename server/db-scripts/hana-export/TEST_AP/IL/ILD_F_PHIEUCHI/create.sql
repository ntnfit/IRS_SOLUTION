CREATE procedure ILD_F_PhieuChi (
	in DocID	int
)
as
begin
	declare Accts nvarchar(100);
	declare bplID int;
	bplID:=0;
	
	if (select count(x."BPLId") from "OVPM" x where x."DocEntry"=DocID) >= 1 then 
		select ifnull(x."BPLId",0) into bplID from "OVPM" x where x."DocEntry"=DocID;
	end if;	
	----------------------------------------------------------------------------------------------------------------------------
	create local temporary table #tbl (
		BPLId			int,
		UserSign		int,
		CompName 		nvarchar(300), 
		CompAddr 		nvarchar(300),
		CompTel 		nvarchar(30), 
		CompFax 		nvarchar(30), 
		CompTaxNum		nvarchar(30),
		
		RptNameVN 		nvarchar(300) default N'PHIẾU CHI / PAYMENT VOUCHER', 
		
		DocID			int,
		DocNo 			nvarchar(50), 
		PONo			nvarchar(50),
		TransID			int,
		DebitNote		nvarchar(100),
		CreditNote		nvarchar(100),
		DocDate 		timestamp, 
		DocCurr			nvarchar(10),
		
		FullName		nvarchar(100),
		BPName			nvarchar(100),
		LicTradNum      nvarchar(20),
		BPAddr			nvarchar(300),
		EmpCode			nvarchar(50),
		Reason			nvarchar(300),
		Amount			numeric(28,10),
		TotalInWords	nvarchar(300), 

		Reference		nvarchar(300),
		OriginDoc		nvarchar(300),
		
		Font			nvarchar(50) default 'Arial',
		Note 			nvarchar(300),
		-- Bank info
		BankAccount nvarchar(30),
		SWIFT nvarchar(30),
		BankName nvarchar(50),
		BankAdd nvarchar(50),
		--DflBranch
		BankIBan nvarchar(30),
		
		--format number
		AmountDec		smallint,
		QtyDec			smallint,
		PriceDec		smallint,
		RateDec			smallint,
		PercentDec		smallint,
		DecSep			nvarchar(1),
		ThousSep		nvarchar(1)
	);
	-- REPORT ----------------------------------------------------------------------------------------------------------------
	insert into #tbl (BPLId,UserSign,DocID,DocNo,PONo,TransID,DocDate,DocCurr,FullName,BPName,LicTradNum,BPAddr,Reason,Amount,
					 BankAccount,SWIFT,BankName,BankAdd,BankIBan)
	select 
		"BPLId",
		a."UserSign",
		a."DocEntry",
		"U_JVNumber",
		
		'' as "PONo",
		
		"TransId", 
		"DocDate", 
		"DocCurr",
		"U_Payer", 
		a."CardName",
		b."LicTradNum" as  "TaxCode",
		"U_PayAdd", 
		"Comments", 
		case when "DocTotalFC"=0 then "DocTotal" else "DocTotalFC" end,
		a."BankCode" as "BankAccount",
		b."DflSwift" as "SWIFT",
		
		'' as  "BankName",
		
		'DflBranch' as "BankAdd",
		b."DflIBAN" as "BankIBan"
	from OVPM a
	left join OCRD b on a."CardCode"  =b."CardCode"  
	where a."DocEntry"=:DocID;
	
	create local temporary table #tblACC (AcctCode nvarchar(20));
	insert into #tblACC (AcctCode) select a."Account" from JDT1 a, #tbl b where a."TransId"=b.TransID and a."DebCred"='D' and a."Debit"<>0 group by a."Account";

	update #tbl set DebitNote = (select STRING_AGG(AcctCode, ',') from #tblACC);
	
	delete from #tblACC;
	insert into #tblACC (AcctCode) select a."Account" from JDT1 a, #tbl b where a."TransId"=b.TransID and a."DebCred"='C' and a."Credit"<>0 group by a."Account";
	
	update #tbl set CreditNote = (select STRING_AGG(AcctCode, ',') from #tblACC);
		
	update #tbl set TotalInWords = Num2Text(cast(Amount as bigint)) || ' ' || IFNULL((select x."CurrName" from OCRN x where x."CurrCode"=#tbl.DocCurr),'') 
								 || case when Amount = Round(Amount,0) then '' else N' và ' || lower(Num2Text(cast((Amount-ROUND(Amount-0.5,0))*100 as int))) || ' ' || IFNULL((select x."Chk100Name" from OCRN x where x."CurrCode"=#tbl.DocCurr),'') end;
	
	
	if bplID=0 then 
		update x set CompName = y."CompnyName", CompAddr = SUBSTR_BEFORE(y."CompnyAddr",char(13)) from #tbl x, OADM y;
	else
		update x set CompName = y."BPLName", CompAddr = SUBSTR_BEFORE(y."Address",char(13)) from #tbl x, OBPL y where y."BPLId"=:bplID;
	end if;
	
	update x set
		EmpCode = b."USER_CODE"
	from #tbl x
	inner join OUSR b on x.UserSign = b."USERID";	
	
	update x set 
			AmountDec	= y."SumDec",
			QtyDec		= y."QtyDec",
			PriceDec	= y."PriceDec",
			RateDec		= y."RateDec",
			PercentDec	= y."PercentDec",
			DecSep		= y."DecSep",
			ThousSep	= y."ThousSep"
	from #tbl x inner join OADM y on 1=1;	
	
	select * from #tbl;

	drop table #tbl;
	drop table #tblACC;
end


