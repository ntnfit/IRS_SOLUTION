CREATE procedure ILD_F_PhieuKeToan (
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
	
	--select top 1 ifnull(x."BPLId",0) into bplID from "JDT1" x where x."TransId"=DocID;
	----------------------------------------------------------------------------------------------------------------------------
	create local temporary table #tbl (
		CompName 		nvarchar(300), 
		CompAddr 		nvarchar(300), 
		
		CompTel 		nvarchar(30), 
		CompFax 		nvarchar(30), 
		CompTaxNum		nvarchar(30),
		
		RptNameVN 		nvarchar(300) default N'PHIẾU KẾ TOÁN/GENERAL VOUCHER', 

		
		DateText 		nvarchar(100), 
		DocNum 			nvarchar(20),
		TransType 		int,
		Line_ID			int,
		Acc 			nvarchar(20),
		Contraact		nvarchar(20), 
		BPCode 			nvarchar(20), 
		BPName 			nvarchar(300), 
		DebAmount 		numeric(21,6),
		CredAmount 		numeric(21,6),
		ExRate			decimal(21,6),
		-- FC 
		FCcredit		decimal(21,6),
		FCdebit			decimal(21,6),
		FCcurrency		nvarchar(10),

	
		Express 		nvarchar(300),
		LineMemo		nvarchar(100),
		PrjCode 		nvarchar(10),
		PrjName 		nvarchar(99),
		TotalAmount		numeric(21,6),
		TotalInWords	nvarchar(300),
		--format number
		AmountDec		smallint,
		QtyDec			smallint,
		PriceDec		smallint,
		RateDec			smallint,
		PercentDec		smallint,
		DecSep			nvarchar(1),
		ThousSep		nvarchar(1),
		DocCurr			nvarchar(10)
	);	
	-- REPORT ----------------------------------------------------------------------------------------------------------------
	/*insert into #tbl (CompName,CompAddr,DateText,DocNum,TransType,Line_ID,BPCode,BPName,DebAmount,CredAmount,Express,PrjCode)
	SELECT 
		b."CompnyName", b."CompnyAddr", 'Ngày ' || to_char(a."RefDate",'DD') || ' tháng ' || to_char(a."RefDate",'MM') || ' năm ' || to_char(a."RefDate",'YYYY'), 
		CAST("TransId" AS nvarchar(20)),
		a."TransType",
		a."Line_ID",
		
		 c."CardCode", c."CardName", "Debit", "Credit", "LineMemo", a."Project" 
	FROM "JDT1" a 
	LEFT OUTER JOIN "OADM" b ON 1 = 1 
	LEFT OUTER JOIN "OCRD" c ON c."CardCode" = a."ShortName" 
	WHERE a."TransId" = DocID;*/
	-- 		Acc 			nvarchar(20),
		
	
	insert into #tbl (CompName,CompAddr,DateText,DocNum,TransType,Line_ID,
					  	Acc,
					  	Contraact,	
						BPCode,BPName,DebAmount,CredAmount,FCdebit,FCcredit,FCcurrency,ExRate)
	SELECT 
		DISTINCT  b."CompnyName", b."CompnyAddr", 'Ngày ' || to_char(a."REFDATE",'DD') || ' tháng ' || to_char(a."REFDATE",'MM') || ' năm ' || to_char(a."REFDATE",'YYYY'), 
		CAST("TRANSID" AS nvarchar(20)),
		a."TRANSTYPE",
		a."LINEID",
		a."ACCOUNT",
		a."CONTRAACT",
		 c."CardCode", c."CardName", "DEBIT", "CREDIT",
		 -- FC
		a."FCDEBIT",
		a."FCCREDIT",
		a."FCCURRENCY",
		0 as "ExRate"
		
	FROM ALL_JE a 
	LEFT  JOIN "OADM" b ON 1 = 1 
	LEFT JOIN "OCRD" c ON c."CardCode" = a."BPCODE" 
	WHERE a."TRANSID" = :DocID;
	
			/*




				decimal(21,6),
					decimal(21,6),
				nvarchar(10),
*/
	
	--- Update diễn giải master của phiếu
	update  a
	set   Express = (select  "Memo" from OJDT b	where b."TransId" = :DocID  )
	from #tbl a;
	
	
	---- Update diễn giải details
	update  a
	set   LineMemo = (select distinct "LineMemo" from JDT1 b 
						where b."TransId" = :DocID and  a.Line_ID = b."Line_ID" )
	from #tbl a;
	
	
	   				
	update #tbl set PrjName = (select x."PrjName" from "OPRJ" x where x."PrjCode"=#tbl.PrjCode);
	
	--update #tbl set Contraact = (select x.ContraAct from ALL_JE x where x."TransId" = DocID);
	--update #tbl set Acc = (select x.Account from ALL_JE x where x."TransId" = DocID);
	
	update #tbl set TotalAmount = (select sum(a."Debit") from JDT1 a where a."TransId" = DocID);
	update #tbl set TotalInWords = Num2Text(cast(TotalAmount as bigint)) || ' ' 
								 || case when TotalAmount = Round(TotalAmount,0) then '' else N' và ' || lower(Num2Text(cast((TotalAmount-ROUND(TotalAmount-0.5,0))*100 as int))) end;
	
	update x set 
			AmountDec	= y."SumDec",
			QtyDec		= y."QtyDec",
			PriceDec	= y."PriceDec",
			RateDec		= y."RateDec",
			PercentDec	= y."PercentDec",
			DecSep		= y."DecSep",
			ThousSep	= y."ThousSep"
	from #tbl x inner join OADM y on 1=1;							 
	
	select 
		*
	from #tbl;

	drop table #tbl;
end


