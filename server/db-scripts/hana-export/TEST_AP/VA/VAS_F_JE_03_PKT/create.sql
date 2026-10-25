CREATE procedure VAS_F_JE_03_PKT (
	in ObjType	nvarchar(20),
	in DocID	int
)
as
begin
	declare JEID int;
	declare TongCong numeric(21,0);
	
	if ObjType='30' then
		JEID :=DocID;
	else 
		select "TransId" into JEID from OJDT where "ObjType"=:ObjType and "CreatedBy"=:DocID;
	end if;
	----------------------------------------------------------------------------------------------------------------------------
	create local temporary table #tbl (
		CompName 	nvarchar(300), 
		CompAddr 	nvarchar(300),
		RptName		nvarchar(100) default 'PHIẾU HẠCH TOÁN', 
		DateText 	nvarchar(100), 
		DocNum 		nvarchar(20), 
		ObjType		nvarchar(20),
		TransId		int,
		BPCodeDeb	nvarchar(20), 
		BPNameDeb 	nvarchar(300), 
		BPCodeCred	nvarchar(20), 
		BPNameCred 	nvarchar(300), 
		
		Dept		nvarchar(100),
		Comments	nvarchar(300),
		
		InvNo		nvarchar(50),
		Detail		nvarchar(300),
		CostCode	nvarchar(100),
		Project		nvarchar(50),
		DebitAcct	nvarchar(20),
		CreditAcct	nvarchar(20),
		
		Amount 		numeric(21,6),
		AmountFC 	numeric(21,6),
		ExRate		numeric(21,6),
		
		TotalInWords 	nvarchar(300),
		Note 		nvarchar(100)
	);	
	-- REPORT ----------------------------------------------------------------------------------------------------------------
	insert into #tbl (CompName,CompAddr,DateText,DocNum,ObjType,TransId,BPCodeDeb,BPNameDeb,BPCodeCred,BPNameCred,Comments,Detail,Project,DebitAcct,CreditAcct,Amount,AmountFC,CostCode)
	SELECT 
		b."CompnyName", b."CompnyAddr", 'Ngày ' || to_char(a."REFDATE",'DD') || ' tháng ' || to_char(a."REFDATE",'MM') || ' năm ' || to_char(a."REFDATE",'YYYY'), d."U_JVNumber",
		a."TRANSTYPE",a."TRANSID", c1."CardCode",c1."CardName",c2."CardCode",c2."CardName",d."Memo",e."LineMemo",a."PRJCODE",a."ACCOUNT",a."CONTRAACT",a."DEBIT",a."FCDEBIT",a."DIM2"
	FROM "ALL_JE" a 
	LEFT OUTER JOIN "OADM" b ON 1 = 1 
	LEFT OUTER JOIN "OCRD" c1 ON c1."CardCode" = a."BPCODE" 
	LEFT JOIN OCRD c2 on c2."CardCode"=a."BPCODECOR"
	LEFT JOIN OJDT d on d."TransId"=a."TRANSID"
	LEFT JOIN JDT1 e on e."TransId"=a."TRANSID" and e."Line_ID"=a."LINEID"
	WHERE a."TRANSID" = :JEID;
	
	delete from #tbl where ifnull(Amount,0)=0;
	
	update #tbl set ExRate = Amount/AmountFC where ifnull(AmountFC,0)<>0;
	
	update #tbl set InvNo = :JEID;
	
	select sum(Amount) into TongCong from #tbl;
	
	update #tbl set TotalInWords = Num2Text (TongCong);
	
	select 
		*
	from #tbl;

	drop table #tbl;
end

