CREATE procedure VAS_F_JE_04_PKT (
	in ObjType	nvarchar(20),
	in DocID	int
)
as
begin
	declare JEID, BranchID int;
	declare TongCong numeric(21,0);
	declare bpcode nvarchar(20);
	declare bpname nvarchar(100);
	declare transtype	nvarchar(50);
	
	if ObjType='30' then
		JEID :=DocID;
	else 
		select "TransId" into JEID from OJDT where "ObjType"=:ObjType and "CreatedBy"=:DocID;
	end if;
	
	select max("BPLId"),max("TransType") into BranchID,transtype from JDT1 where "TransId"=:JEID;
	----------------------------------------------------------------------------------------------------------------------------
	create local temporary table #tbl (
		CompName 	nvarchar(300), 
		CompAddr 	nvarchar(300),
		RptName		nvarchar(100) default 'PHIẾU HẠCH TOÁN/JOURNAL VOUCHER', 
		DateText 	nvarchar(100), 
		DocNum 		nvarchar(20),
		DocDate		timestamp, 
		ObjType		nvarchar(20),
		TransId		int,
		BPCode		nvarchar(20), 
		BPName		nvarchar(300), 
		
		Dept		nvarchar(100),
		Comments	nvarchar(300),
		
		
		AcctCode	nvarchar(20),
		AcctName	nvarchar(100),
		Detail		nvarchar(300),
		InvNo		nvarchar(50),
		ComInvNo	nvarchar(50),
		
		CostCode	nvarchar(100),
		Project		nvarchar(50),
		Dim1		nvarchar(150),
		Dim2		nvarchar(150),
		Dim3		nvarchar(150),
		Dim4		nvarchar(150),
		Dim5		nvarchar(150),
		
		Debit 		numeric(21,6),
		Credit 		numeric(21,6),
		DebitFC 	numeric(21,6),
		CreditFC 	numeric(21,6),
		Curr		nvarchar(5),
		ExRate		numeric(21,6),
		
		TotalInWords 	nvarchar(300),
		Note 		nvarchar(100)
	);	
	-- REPORT ----------------------------------------------------------------------------------------------------------------
	insert into #tbl (CompName,CompAddr,DateText,DocNum,DocDate,ObjType,TransId,Comments,Detail,AcctCode,Project,Dim1,Dim2,Dim3,Dim4,Dim5,Debit,Credit,DebitFC,CreditFC,Curr,InvNo)
	SELECT 
		b."CompnyName", b."CompnyAddr", 'Ngày ' || to_char(a."RefDate",'DD') || ' tháng ' || to_char(a."RefDate",'MM') || ' năm ' || to_char(a."RefDate",'YYYY'), d."U_JVNumber",a."RefDate",
		a."TransType",a."TransId",d."Memo",a."LineMemo",a."Account",a."Project",a."ProfitCode",a."OcrCode2",a."OcrCode3",a."OcrCode4",a."OcrCode5",a."Debit",a."Credit",a."FCDebit",a."FCCredit",a."FCCurrency",a."U_InvNo"
	FROM "JDT1" a 
	LEFT OUTER JOIN "OADM" b ON 1 = 1 
	LEFT JOIN OJDT d on d."TransId"=a."TransId"
	WHERE a."TransId" = :JEID;
	
	
	select max(z."CardCode"), max(z."CardName") into bpcode,bpname from #tbl x inner join JDT1 y on y."TransId"=x.TransId inner join OCRD z on z."CardCode"=y."ShortName";
	
	update #tbl set BPCode = :bpcode, BPName = :bpname;
	
	update #tbl set ExRate = ROUND((Debit+Credit)/(DebitFC+CreditFC),2) where (DebitFC+CreditFC)<>0;
	
--	update #tbl set InvNo = :JEID;
	if transtype='13' then
		update x set InvNo = y."U_InvNo", DocNum = y."U_JVNumber", Comments = y."Comments", ComInvNo = '' from #tbl x inner join OINV y on y."TransId"=:JEID;
	else if transtype='14' then
		update x set InvNo = y."U_InvNo", DocNum = y."U_JVNumber", Comments = y."Comments", ComInvNo = '' from #tbl x inner join ORIN y on y."TransId"=:JEID;
	else if transtype='15' then
		update x set InvNo = y."U_InvNo", DocNum = y."U_JVNumber", Comments = y."Comments" from #tbl x inner join ODLN y on y."TransId"=:JEID;
	else if transtype='16' then
		update x set InvNo = y."U_InvNo", DocNum = y."U_JVNumber", Comments = y."Comments" from #tbl x inner join ORDN y on y."TransId"=:JEID;
	else if transtype='18' then
		update x set InvNo = y."U_InvNo", DocNum = y."U_JVNumber", Comments = y."Comments" from #tbl x inner join OPCH y on y."TransId"=:JEID;
	else if transtype='19' then
		update x set InvNo = y."U_InvNo", DocNum = y."U_JVNumber", Comments = y."Comments" from #tbl x inner join ORPC y on y."TransId"=:JEID;
	else if transtype='20' then
		update x set InvNo = y."U_InvNo", DocNum = y."U_JVNumber", Comments = y."Comments" from #tbl x inner join OPDN y on y."TransId"=:JEID;
	else if transtype='21' then
		update x set InvNo = y."U_InvNo", DocNum = y."U_JVNumber", Comments = y."Comments" from #tbl x inner join ORPD y on y."TransId"=:JEID;
	else if transtype='24' then
		update x set DocNum = y."U_JVNumber", Comments = y."Comments" from #tbl x inner join ORCT y on y."TransId"=:JEID;
	else if transtype='46' then
		update x set DocNum = y."U_JVNumber", Comments = y."Comments" from #tbl x inner join OVPM y on y."TransId"=:JEID;
	else if transtype='59' then
		update x set DocNum = y."U_JVNumber", Comments = y."Comments" from #tbl x inner join OIGN y on y."TransId"=:JEID;
	else if transtype='60' then
		update x set DocNum = y."U_JVNumber", Comments = y."Comments" from #tbl x inner join OIGE y on y."TransId"=:JEID;
	else if transtype='67' then
		update x set DocNum = y."U_JVNumber", Comments = y."Comments" from #tbl x inner join OWTR y on y."TransId"=:JEID;
	end if;
	end if;
	end if;
	end if;
	end if;
	end if;
	end if;
	end if;
	end if;
	end if;
	end if;
	end if;
	end if;
	
	update #tbl set Detail = Comments;
	
	select abs(sum(Debit+Credit)) into TongCong from #tbl;
	
	update #tbl set TotalInWords = Num2Text (TongCong);
	
	
	update x set AcctName = y."AcctName" from #tbl x inner join OACT y on y."AcctCode"=x.AcctCode;
	update x set Project = y."PrjCode" || ' - ' || y."PrjName" from #tbl x inner join OPRJ y on y."PrjCode"=x.Project;
	update x set Dim1 = y."PrcCode" || ' - ' || y."PrcName" from #tbl x inner join OPRC y on y."PrcCode"=x.Dim1;
	update x set Dim2 = y."PrcCode" || ' - ' || y."PrcName" from #tbl x inner join OPRC y on y."PrcCode"=x.Dim2;
	update x set Dim3 = y."PrcCode" || ' - ' || y."PrcName" from #tbl x inner join OPRC y on y."PrcCode"=x.Dim3;
	update x set Dim4 = y."PrcCode" || ' - ' || y."PrcName" from #tbl x inner join OPRC y on y."PrcCode"=x.Dim4;
	update x set Dim5 = y."PrcCode" || ' - ' || y."PrcName" from #tbl x inner join OPRC y on y."PrcCode"=x.Dim5;
	
	if BranchID<>0 then
		update x set CompName = y."BPLFrName", CompAddr = y."Street" from #tbl x inner join OBPL y on y."BPLId"=:BranchID;
	end if;
	
	select 
		*
	from #tbl;

	drop table #tbl;
end

