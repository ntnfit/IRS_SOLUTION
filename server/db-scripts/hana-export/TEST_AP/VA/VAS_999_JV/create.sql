create procedure VAS_999_JV (
	in batchID	int,
	in display	int
)
as
begin
	declare mainCurr nvarchar(5);
	declare idx, countt, varLineID, firstLine, varBatchID, varTransID, countDeb, countCred int;
	declare debit, credit, temp, sumDeb, sumCred numeric(21,6);
	declare debCred char(1);
	
	select "MainCurncy" into mainCurr from OADM;
	
	create local temporary table #TBL_JE_TMP (
		ID			int,
		BPLId		int,
		RefDate		timestamp,
		BatchID		int,
		TransType	nvarchar(30),
		TransId		int,
		LineId		int,
		Account		nvarchar(20),
		ContraAct	nvarchar(20),
		BPCode		nvarchar(20),
		DocID		int,
		DocLine		int,
		Debit		numeric(21,6),
		Credit		numeric(21,6),
		DebitSC		numeric(21,6),
		CreditSC	numeric(21,6),
		FCDebit		numeric(21,6),
		FCCredit	numeric(21,6),
		FCCurrency	nvarchar(10),
		Dim1		nvarchar(20),
		Dim2		nvarchar(20),
		Dim3		nvarchar(20),
		Dim4		nvarchar(20),
		Dim5		nvarchar(20),
		PrjCode		nvarchar(20),
		Idx			nvarchar(10),
		DebCred		char(1)
	);
	
	EXCl =
		select x."BatchNum", x."TransId"
		from BTF1 x
		group by x."BatchNum",x."TransId"
		having SUM(x."Debit" - x."Credit") <> 0;
		
	create local temporary table #JDT1 (
		BPLId		int,
		RefDate		timestamp,
		BatchNum	int,
		TransType	nvarchar(30),
		TransId		int,
		Line_ID		int,
		Account		nvarchar(30),
		ContraAct	nvarchar(30),
		ShortName	nvarchar(30),
		CreatedBy	int,
		DocLine		int,
		Debit		numeric(21,6),
		Credit		numeric(21,6),
		SYSDeb		numeric(21,6),
		SYSCred		numeric(21,6),
		FCDebit		numeric(21,6),
		FCCredit	numeric(21,6),
		FCCurrency	nvarchar(10),
		ProfitCode	nvarchar(30),
		OcrCode2	nvarchar(30),
		OcrCode3	nvarchar(30),
		OcrCode4	nvarchar(30),
		OcrCode5	nvarchar(30),
		Project		nvarchar(30)
	);
	
	insert into #JDT1 (BPLId, RefDate, BatchNum, TransType, TransId, Line_ID, Account, ContraAct, ShortName, CreatedBy, DocLine, Debit, Credit, SYSDeb, SYSCred, FCDebit, FCCredit, FCCurrency, ProfitCode, OcrCode2, OcrCode3, OcrCode4, OcrCode5, Project)
	select
		b."BPLId",
		b."RefDate",
		a."BatchNum",
		b."TransType",
		b."TransId",
		b."Line_ID",
		b."Account",
		b."ContraAct",
		b."ShortName",
		b."CreatedBy",
		b."DocLine",
		b."Debit",
		b."Credit",
		b."SYSDeb",
		b."SYSCred",
		b."FCDebit",
		b."FCCredit",
		CASE when IFNULL(b."FCCurrency",'') = '' then :maincurr else b."FCCurrency" end,
		IFNULL(b."ProfitCode",''),
		IFNULL(b."OcrCode2",''),
		IFNULL(b."OcrCode3",''),
		IFNULL(b."OcrCode4",''),
		IFNULL(b."OcrCode5",''),
		IFNULL(b."Project",'') 
	from OBTF a, BTF1 b 
	where
		a."BtfStatus" = 'O'
		and a."BatchNum" = b."BatchNum"
		and a."TransId" = b."TransId"
		and a."BatchNum" <= :batchID
		and (IFNULL(b."Debit",0) <> 0 or IFNULL(b."Credit",0) <> 0) 
		and a."BatchNum" not in (select x.BatchID from ALL_JV x)
		and a."BatchNum" not in (select y."BatchNum" from :EXCl y);
	
	insert into #TBL_JE_TMP (ID, BPLId, RefDate, BatchID, TransType, TransId, LineId, Account, ContraAct, BPCode, DocID, DocLine, Debit, Credit, DebitSC, CreditSC, FCDebit, FCCredit, FCCurrency, DebCred, Dim1, Dim2, Dim3, Dim4, Dim5, PrjCode)
	select
		ROW_NUMBER () OVER (ORDER BY a.BatchNum, a.TransId, a.Line_ID),
		a.BPLId,
		a.RefDate,
		a.BatchNum,
		a.TransType,
		a.TransId,
		a.Line_ID,
		a.Account,
		a.ContraAct,
		CASE when b."LocManTran" = 'Y' then a.ShortName else '' end,
		a.CreatedBy,
		a.DocLine,
		IFNULL(a.Debit,0),
		IFNULL(a.Credit,0),
		IFNULL(a.SYSDeb,0),
		IFNULL(a.SYSCred,0),
		IFNULL(a.FCDebit,0),
		IFNULL(a.FCCredit,0),
		a.FCCurrency,
		CASE when IFNULL(a.Debit,0) <> 0 then 'D' else 'C' end,
		a.ProfitCode,
		a.OcrCode2,
		a.OcrCode3,
		a.OcrCode4,
		a.OcrCode5,
		a.Project
	from #JDT1 a, OACT b
	where a.Account = b."AcctCode"
	order by a.BatchNum, a.TransId, a.Line_ID;
	
	select MAX(a.ID) into countt from #TBL_JE_TMP a;
	select 1, 0, 0, 0, 0 into idx, sumDeb, sumCred, countDeb, countCred from dummy;
	
	while (:idx <= :countt)
	do
		select a.BatchID, a.TransId, a.LineID, a.Debit, a.Credit, a.DebCred into varBatchID, varTransID, varLineID, debit, credit, debCred from #TBL_JE_TMP a where a.ID = :idx;
		
		if (:countCred = 0 and :countDeb = 0) then
			firstLine = :varLineID;
		end if;
		
		if (:debCred = 'D') then
			sumDeb = :sumDeb + :debit;
			countDeb = :countDeb + 1;
		end if;
		
		if (:debCred = 'C') then
			sumCred = :sumCred + :credit;
			countCred = :countCred + 1;
		end if;
		
		if (:sumDeb = :sumCred) then
			if (:sumDeb = 0) then
				insert into ALL_JV (BPLId, BatchID, TransType, RefDate, TransId, LineId, Account, ContraAct, Debit, Credit, DebitSC, CreditSC, FCDebit, FCCredit, FCCurrency, isNN, BPCode, DocId, DocLineId, Dim1, Dim2, Dim3, Dim4, Dim5, PrjCode)
				select a.BPLId, a.BatchID, a.TransType, a.RefDate, a.TransId, a.LineID, a.Account, a.ContraAct, a.Debit, a.Credit, a.DebitSC, a.CreditSC, a.FCDebit, a.FCCredit, a.FCCurrency, 'N', a.BPCode, a.DocID, a.DocLine, a.Dim1, a.Dim2, a.Dim3, a.Dim4, a.Dim5, a.PrjCode
				from #TBL_JE_TMP a
				where
					a.BatchID = :varBatchID
					and a.TransId = :varTransID
					and a.LineID between :firstLine and :varLineID;
			elseif (:sumDeb <> 0) then
				if (:countDeb = 1 and :countCred = 1) then
					insert into ALL_JV (BPLId, BatchID, TransType, RefDate, TransId, LineId, CorrLineId, Account, ContraAct, BPCode, BPCodeCor, DocId, DocLineId, DocLineIdCor, Debit, Credit, DebitSC, CreditSC, FCDebit, FCCredit, FCCurrency, isNN, Dim1, Dim2, Dim3, Dim4, Dim5, PrjCode, Dim1Cor, Dim2Cor, Dim3Cor, Dim4Cor, Dim5Cor, PrjCodeCor)
					select a.BPLId, a.BatchID, a.TransType, a.RefDate, a.Transid, a.LineId, b.LineId, a.Account, b.Account, a.BPCode, b.BPCode, a.DocID, a.DocLine, b.DocLine, a.Debit, a.Credit, a.DebitSC, a.CreditSC, a.FCDebit, a.FCCredit, a.FCCurrency, 'N', a.Dim1, a.Dim2, a.Dim3, a.Dim4, a.Dim5, a.PrjCode, b.Dim1, b.Dim2, b.Dim3, b.Dim4, b.Dim5, b.PrjCode
					from #TBL_JE_TMP a, #TBL_JE_TMP b 
					where
						a.BatchID = b.BatchID
						and a.BatchID = :varBatchID
						and a.TransId = :varTransID
						and a.DebCred = 'D'
						and b.DebCred = 'C'
						and a.TransId = b.TransId
						and a.LineId between :firstLine and :varLineID
						and b.LineId between :firstLine and :varLineID;
					
					insert into ALL_JV (BPLId, BatchID, TransType, RefDate, TransId, LineId, CorrLineId, Account, ContraAct, BPCode, BPCodeCor, DocId, DocLineId, DocLineIdCor, Debit, Credit, DebitSC, CreditSC, FCDebit, FCCredit, FCCurrency, isNN, Dim1, Dim2, Dim3, Dim4, Dim5, PrjCode, Dim1Cor, Dim2Cor, Dim3Cor, Dim4Cor, Dim5Cor, PrjCodeCor)
					select a.BPLId, a.BatchID, a.TransType, a.RefDate, a.TransId, a.LineId, b.LineId, a.Account, b.Account, a.BPCode, b.BPCode, a.DocID, a.DocLine, b.DocLine, b.Credit, b.Debit, b.CreditSC, b.DebitSC, b.FCCredit, b.FCDebit, a.FCCurrency, 'N', a.Dim1, a.Dim2, a.Dim3, a.Dim4, a.Dim5, a.PrjCode, b.Dim1, b.Dim2, b.Dim3, b.Dim4, b.Dim5, b.PrjCode
					from #TBL_JE_TMP a, #TBL_JE_TMP b 
					where
						a.BatchID = b.BatchID
						and a.BatchID = :varBatchID
						and a.TransId = :varTransID
						and a.DebCred = 'C'
						and b.DebCred = 'D'
						and a.TransId = b.TransId
						and a.LineId between :firstLine and :varLineID
						and b.LineId between :firstLine and :varLineID;
				elseif (:countDeb = 1 and :countCred > 1) then
					insert into ALL_JV (BPLId, BatchID, TransType, RefDate, TransId, LineId, CorrLineId, Account, ContraAct, BPCode, BPCodeCor, DocId, DocLineId, DocLineIdCor, Debit, Credit, DebitSC, CreditSC, FCDebit, FCCredit, FCCurrency, isNN, Dim1, Dim2, Dim3, Dim4, Dim5, PrjCode, Dim1Cor, Dim2Cor, Dim3Cor, Dim4Cor, Dim5Cor, PrjCodeCor)
					select a.BPLId, a.BatchID, a.TransType, a.RefDate, a.TransId, a.LineId, b.LineId, a.Account, b.Account, a.BPCode, b.BPCode, a.DocID, a.DocLine, b.DocLine, b.Credit, b.Debit, b.CreditSC, b.DebitSC, b.FCCredit, b.FCDebit, b.FCCurrency, 'N', a.Dim1, a.Dim2, a.Dim3, a.Dim4, a.Dim5, a.PrjCode, b.Dim1, b.Dim2, b.Dim3, b.Dim4, b.Dim5, b.PrjCode
					from #TBL_JE_TMP a, #TBL_JE_TMP b 
					where
						a.BatchID = b.BatchID
						and a.BatchID = :varBatchID
						and a.TransId = :varTransID
						and a.DebCred = 'D'
						and b.DebCred = 'C'
						and a.TransId = b.TransId
						and a.LineId between :firstLine and :varLineID
						and b.LineId between :firstLine and :varLineID;
					
					insert into ALL_JV (BPLId, BatchID, TransType, RefDate, Transid, LineId, CorrLineId, Account, ContraAct, BPCode, BPCodeCor, DocId, DocLineId, DocLineIdCor, Debit, Credit, DebitSC, CreditSC, FCDebit, FCCredit, FCCurrency, isNN, Dim1, Dim2, Dim3, Dim4, Dim5, PrjCode, Dim1Cor, Dim2Cor, Dim3Cor, Dim4Cor, Dim5Cor, PrjCodeCor)
					select a.BPLId, a.BatchID, a.TransType, a.RefDate, a.TransId, a.LineId, b.LineId, a.Account, b.Account, a.BPCode, b.BPCode, a.DocID, a.DocLine, b.DocLine, a.Debit, a.Credit, a.DebitSC, a.CreditSC, a.FCDebit, a.FCCredit, a.FCCurrency, 'N', a.Dim1, a.Dim2, a.Dim3, a.Dim4, a.Dim5, a.PrjCode, b.Dim1, b.Dim2, b.Dim3, b.Dim4, b.Dim5, b.PrjCode
					from #TBL_JE_TMP a, #TBL_JE_TMP b 
					where
						a.BatchID = b.BatchID
						and a.BatchID = :varBatchID
						and a.TransId = :varTransID
						and a.DebCred = 'C'
						and b.DebCred = 'D'
						and a.TransId = b.TransId
						and a.LineId between :firstLine and :varLineID
						and b.LineId between :firstLine and :varLineID;
				elseif (:countDeb > 1 and :countCred = 1) then
					insert into ALL_JV (BPLId, BatchID, TransType, RefDate, Transid, LineId, CorrLineId, Account, ContraAct, BPCode, BPCodeCor, DocId, DocLineId, DocLineIdCor, Debit, Credit, DebitSC, CreditSC, FCDebit, FCCredit, FCCurrency, isNN, Dim1, Dim2, Dim3, Dim4, Dim5, PrjCode, Dim1Cor, Dim2Cor, Dim3Cor, Dim4Cor, Dim5Cor, PrjCodeCor)
					select a.BPLId, a.BatchID, a.TransType, a.RefDate, a.TransId, a.LineId, b.LineId, a.Account, b.Account, a.BPCode, b.BPCode, a.DocID, a.DocLine, b.DocLine, a.Debit, a.Credit, a.DebitSC, a.CreditSC, a.FCDebit, a.FCCredit, a.FCCurrency, 'N', a.Dim1, a.Dim2, a.Dim3, a.Dim4, a.Dim5, a.PrjCode, b.Dim1, b.Dim2, b.Dim3, b.Dim4, b.Dim5, b.PrjCode
					from #TBL_JE_TMP a, #TBL_JE_TMP b
					where
						a.BatchID = b.BatchID
						and a.BatchID = :varBatchID
						and a.transid = :varTransID
						and a.DebCred = 'D'
						and b.DebCred = 'C'
						and a.TransId = b.TransId
						and a.LineId between :firstLine and :varLineID
						and b.LineId between :firstLine and :varLineID;
					
					insert into ALL_JV (BPLId, BatchID, TransType, RefDate, Transid, LineId, CorrLineId, Account, ContraAct, BPCode, BPCodeCor, DocId, DocLineId, DocLineIdCor, Debit, Credit, DebitSC, CreditSC, FCDebit, FCCredit, FCCurrency, isNN, Dim1, Dim2, Dim3, Dim4, Dim5, PrjCode, Dim1Cor, Dim2Cor, Dim3Cor, Dim4Cor, Dim5Cor, PrjCodeCor)
					select a.BPLId, a.BatchID, a.TransType, a.RefDate, a.Transid, a.LineId, b.LineId, a.Account, b.Account, a.BPCode, b.BPCode, a.DocID, a.DocLine, b.DocLine, b.Credit, b.Debit, b.CreditSC, b.DebitSC, b.FCCredit, b.FCDebit, b.FCCurrency, 'N', a.Dim1, a.Dim2, a.Dim3, a.Dim4, a.Dim5, a.PrjCode, b.Dim1, b.Dim2, b.Dim3, b.Dim4, b.Dim5, b.PrjCode
					from #TBL_JE_TMP a, #TBL_JE_TMP b
					where
						a.BatchID = b.BatchID
						and a.BatchID = :varBatchID
						and a.TransId = :varTransID
						and a.DebCred = 'C'
						and b.DebCred = 'D'
						and a.TransId = b.TransId
						and a.LineId between :firstLine and :varLineID
						and b.LineId between :firstLine and :varLineID;
				elseif (:countDeb > 1 and :countCred > 1) then
					insert into ALL_JE (BPLId, TransType, RefDate, TransId, LineId, Account, ContraAct, Debit, Credit, DebitSC, CreditSC, FCDebit, FCCredit, FCCurrency, isNN)
					select a.BPLId, a.TransType, a.RefDate, a.TransId, a.LineId, a.Account, a.ContraAct, a.Debit, a.Credit, a.DebitSC, a.CreditSC, a.FCDebit, a.FCCredit, a.FCCurrency, 'Y'
					from #TBL_JE_TMP a
					where
						a.TransId = :varTransId
						and a.LineId between :firstLine and :varLineId;
				end if;
			end if;
			
			sumDeb = 0;
			sumCred = 0;
			countDeb = 0;
			countCred = 0;
		end if;
		
		idx = :idx + 1;
	end while;
	
	if (:display = 1) then
		select * from ALL_JV where TransId <= :batchID;
	end if;
	
	drop table #TBL_JE_TMP;
	drop table #JDT1;
end

