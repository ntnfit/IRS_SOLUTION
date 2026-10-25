CREATE procedure VAS_999 (
	in tNo int,
	in display int
)
as
begin
	declare mainCurr nvarchar(5);
	declare idx, countt, varLineId, firstLine, varTransId, countDeb, countCred int;
	declare debit, credit, temp, sumDeb, sumCred numeric(21,6);
	declare debCred char(1);
	declare stt, sttmax, JEID int;
	declare DebSum, CredSum numeric(28,6);
	--==========================================================================================
	declare tblJE table (BPLId int, RefDate timestamp, TransType nvarchar(30), TransId int, Line_ID int, Account nvarchar(30), ContraAct nvarchar(30), ShortName nvarchar(30), CreatedBy int, DocLine int, Debit numeric(21,6), Credit numeric(21,6), SYSDeb numeric(21,6), SYSCred numeric(21,6), FCDebit numeric(21,6), FCCredit numeric(21,6), FCCurrency nvarchar(30), ProfitCode nvarchar(30), OcrCode2 nvarchar(30), OcrCode3 nvarchar(30), OcrCode4 nvarchar(30), OcrCode5 nvarchar(30), Project nvarchar(30));
	declare tblTBL_JE_TMP table (ID bigint primary key, BPLId int, RefDate timestamp, TransType nvarchar(30), TransId int, LineId int, Account nvarchar(20), ContraAct nvarchar(20), BPCode nvarchar(20), DocID int, DocLine int, Debit numeric(21,6), Credit numeric(21,6), DebitSC numeric(21,6), CreditSC numeric(21,6), FCDebit numeric(21,6), FCCredit numeric(21,6), FCCurrency nvarchar(10), DebCred char(1), SumDebit numeric(28,6), SumCredit numeric(28,6), Dim1 nvarchar(20), Dim2 nvarchar(20), Dim3 nvarchar(20), Dim4 nvarchar(20), Dim5 nvarchar(20), PrjCode nvarchar(20), Idx nvarchar(10), JE_ID int, D_Count bigint, C_Count bigint);
	declare tblTMP table (ID bigint primary key, STT int, ID0 bigint);
	declare EXISTED table (JEID int);
	
	select "MainCurncy" into mainCurr from OADM;

	insert into :EXISTED (JEID) select x.TransId as JEID from ALL_JE x group by x.TransId;

	insert into :tblJE (BPLId, RefDate, TransType, TransId, Line_ID, Account, ContraAct, ShortName, CreatedBy, DocLine, Debit, Credit, SYSDeb, SYSCred, FCDebit, FCCredit, FCCurrency, ProfitCode, OcrCode2, OcrCode3, OcrCode4, OcrCode5, Project)
	select a."BPLId", a."RefDate", a."TransType", a."TransId", a."Line_ID", a."Account", a."ContraAct", a."ShortName", a."CreatedBy", a."DocLine", a."Debit", a."Credit", a."SYSDeb", a."SYSCred", a."FCDebit", a."FCCredit", a."FCCurrency", a."ProfitCode", a."OcrCode2", a."OcrCode3", a."OcrCode4", a."OcrCode5", a."Project"
	from JDT1 a
	left join :EXISTED b on a."TransId" = b.JEID
	where a."TransId" <= :tNo and b.JEID is null and (a."Debit"<>0 or a."Credit"<>0);
	---
	insert into :tblTBL_JE_TMP (ID, BPLId, RefDate, TransType, TransId, LineId, Account, ContraAct, BPCode, DocID, DocLine, Debit, Credit, DebitSC, CreditSC, FCDebit, FCCredit, FCCurrency, DebCred, Dim1, Dim2, Dim3, Dim4, Dim5, PrjCode,SumDebit,SumCredit)
	select ROW_NUMBER () OVER (ORDER BY a.TransId, a.Line_ID), a.BPLId, a.RefDate, a.TransType, a.TransId, a.Line_ID, a.Account, a.ContraAct, case when a.Account<>a.ShortName then a.ShortName else '' end, a.CreatedBy, a.DocLine, IFNULL(a.Debit,0), IFNULL(a.Credit,0), IFNULL(a.SYSDeb,0), IFNULL(a.SYSCred,0), IFNULL(a.FCDebit,0), IFNULL(a.FCCredit,0), a.FCCurrency, CASE when IFNULL(a.Debit,0) <> 0 then 'D' else 'C' end, a.ProfitCode, a.OcrCode2, a.OcrCode3, a.OcrCode4, a.OcrCode5, a.Project, sum(a.Debit) over (order by a.TransId, a.Line_ID),sum(a.Credit) over (order by a.TransId, a.Line_ID)
	from :tblJE a order by a.TransId, a.Line_ID;

	update x set ContraAct = y.Account from :tblTBL_JE_TMP x inner join :tblJE y on y.TransId=x.TransId and y.ShortName=x.ContraAct and y.Account<>y.ShortName;
	
	insert into :tblTMP (ID,STT) select ID, ROW_NUMBER() OVER (Order by ID) from :tblTBL_JE_TMP where SumDebit=SumCredit;
	
	update x set ID0 = y.ID+1 from :tblTMP x inner join :tblTMP y on y.STT=x.STT-1;
	update :tblTMP set ID0=1 where ID0 is null;
	
	update  x set JE_ID = y.STT from :tblTBL_JE_TMP x inner join :tblTMP y on y.ID >= x.ID and y.ID0<=x.ID;

	tbl_Count =  select JE_ID,count(case when DebCred='D' then 1 end) as D_Count,count(case when DebCred='C' then 1 end) as C_Count from :tblTBL_JE_TMP group by JE_ID;
	update :tblTBL_JE_TMP y set D_Count = (select x.D_Count from :tbl_Count x where x.JE_ID=y.JE_ID);
	update :tblTBL_JE_TMP y set C_Count = (select x.C_Count from :tbl_Count x where x.JE_ID=y.JE_ID);
	update :tblTBL_JE_TMP set D_Count = 0 where D_Count is null;
	update :tblTBL_JE_TMP set C_Count = 0 where C_Count is null;
	-- TÁCH ĐỐI ỨNG=====================================================================================================
	-------Các trường hợp (1 Nợ - 1 Có)---------------------------------------
	----- Insert các dòng Nợ
	insert into ALL_JE (BPLId, TransType, RefDate, TransId, LineId, CorrLineId, Account, ContraAct, BPCode, BPCodeCor, DocId, DocLineId, DocLineIdCor, Debit, Credit, DebitSC, CreditSC, FCDebit, FCCredit, FCCurrency, isNN, Dim1, Dim2, Dim3, Dim4, Dim5, PrjCode, Dim1Cor, Dim2Cor, Dim3Cor, Dim4Cor, Dim5Cor, PrjCodeCor)
	select a.BPLId, a.TransType, a.RefDate, a.TransId, a.LineId, b.LineId, a.Account, b.Account, a.BPCode, b.BPCode, a.DocId, a.DocLine, b.DocLine, a.Debit, a.Credit, a.DebitSC, a.CreditSC, a.FCDebit, a.FCCredit, a.FCCurrency, 'N', a.Dim1, a.Dim2, a.Dim3, a.Dim4, a.Dim5, a.PrjCode, b.Dim1, b.Dim2, b.Dim3, b.Dim4, b.Dim5, b.PrjCode
	from :tblTBL_JE_TMP a, :tblTBL_JE_TMP b
	where a.D_Count = 1 and a.C_Count = 1 and a.DebCred = 'D' and b.DebCred = 'C' and a.JE_ID = b.JE_ID;
	----- Insert các dòng Có
	insert into ALL_JE (BPLId, TransType, RefDate, TransId, LineId, CorrLineId, Account, ContraAct, BPCode, BPCodeCor, DocId, DocLineId, DocLineIdCor, Debit, Credit, DebitSC, CreditSC, FCDebit, FCCredit, FCCurrency, isNN, Dim1, Dim2, Dim3, Dim4, Dim5, PrjCode, Dim1Cor, Dim2Cor, Dim3Cor, Dim4Cor, Dim5Cor, PrjCodeCor)
	select a.BPLId, a.TransType, a.RefDate, a.TransId, a.LineId, b.LineId, a.Account, b.Account, a.BPCode, b.BPCode, a.DocId, a.DocLine, b.DocLine, b.Credit, b.Debit, b.CreditSC, b.DebitSC, b.FCCredit, b.FCDebit, a.FCCurrency, 'N', a.Dim1, a.Dim2, a.Dim3, a.Dim4, a.Dim5, a.PrjCode, b.Dim1, b.Dim2, b.Dim3, b.Dim4, b.Dim5, b.PrjCode
	from :tblTBL_JE_TMP a, :tblTBL_JE_TMP b
	where a.D_Count = 1 and a.C_Count = 1 and a.DebCred = 'C' and b.DebCred = 'D' and a.JE_ID = b.JE_ID;
	------Các trường hợp (1 Nợ - nhiều Có) ---------------------------------------
	------ Insert các dòng Nợ
	insert into ALL_JE (BPLId, TransType, RefDate, TransId, LineId, CorrLineId, Account, ContraAct, BPCode, BPCodeCor, DocId, DocLineId, DocLineIdCor, Debit, Credit, DebitSC, CreditSC, FCDebit, FCCredit, FCCurrency, isNN, Dim1, Dim2, Dim3, Dim4, Dim5, PrjCode, Dim1Cor, Dim2Cor, Dim3Cor, Dim4Cor, Dim5Cor, PrjCodeCor)
	select a.BPLId, a.TransType, a.RefDate, a.TransId, a.LineId, b.LineId, a.Account, b.Account, a.BPCode, b.BPCode, a.DocId, a.DocLine, b.DocLine, b.Credit, b.Debit, b.CreditSC, b.DebitSC, b.FCCredit, b.FCDebit, a.FCCurrency, 'N', a.Dim1, a.Dim2, a.Dim3, a.Dim4, a.Dim5, a.PrjCode, b.Dim1, b.Dim2, b.Dim3, b.Dim4, b.Dim5, b.PrjCode
	from :tblTBL_JE_TMP a, :tblTBL_JE_TMP b
	where a.D_Count = 1 and a.C_Count > 1 and a.DebCred = 'D' and b.DebCred = 'C' and a.JE_ID = b.JE_ID;
	------ Insert các dòng Có						
	insert into ALL_JE (BPLId, TransType, RefDate, TransId, LineId, CorrLineId, Account, ContraAct, BPCode, BPCodeCor, DocId, DocLineId, DocLineIdCor, Debit, Credit, DebitSC, CreditSC, FCDebit, FCCredit, FCCurrency, isNN, Dim1, Dim2, Dim3, Dim4, Dim5, PrjCode, Dim1Cor, Dim2Cor, Dim3Cor, Dim4Cor, Dim5Cor, PrjCodeCor)
	select a.BPLId, a.TransType, a.RefDate, a.TransId, a.LineId, b.LineId, a.Account, b.Account, a.BPCode, b.BPCode, a.DocId, a.DocLine, b.DocLine, a.Debit, a.Credit, a.DebitSC, a.CreditSC, a.FCDebit, a.FCCredit, a.FCCurrency, 'N', a.Dim1, a.Dim2, a.Dim3, a.Dim4, a.Dim5, a.PrjCode, b.Dim1, b.Dim2, b.Dim3, b.Dim4, b.Dim5, b.PrjCode
	from :tblTBL_JE_TMP a, :tblTBL_JE_TMP b
	where a.D_Count = 1 and a.C_Count > 1 and a.DebCred = 'C' and b.DebCred = 'D' and a.JE_ID = b.JE_ID;
	------Các trường hợp (nhiều Nợ - 1 Có) ---------------------------------------
	------ Insert các dòng Nợ
	insert into ALL_JE (BPLId, TransType, RefDate, TransId, LineId, CorrLineId, Account, ContraAct, BPCode, BPCodeCor, DocId, DocLineId, DocLineIdCor, Debit, Credit, DebitSC, CreditSC, FCDebit, FCCredit, FCCurrency, isNN, Dim1, Dim2, Dim3, Dim4, Dim5, PrjCode, Dim1Cor, Dim2Cor, Dim3Cor, Dim4Cor, Dim5Cor, PrjCodeCor)
	select a.BPLId, a.TransType, a.RefDate, a.TransId, a.LineId, b.LineId, a.Account, b.Account, a.BPCode, b.BPCode, a.DocId, a.DocLine, b.DocLine, a.Debit, a.Credit, a.DebitSC, a.CreditSC, a.FCDebit, a.FCCredit, a.FCCurrency, 'N', a.Dim1, a.Dim2, a.Dim3, a.Dim4, a.Dim5, a.PrjCode, b.Dim1, b.Dim2, b.Dim3, b.Dim4, b.Dim5, b.PrjCode
	from :tblTBL_JE_TMP a, :tblTBL_JE_TMP b
	where a.D_Count > 1 and a.C_Count = 1 and a.DebCred = 'D' and b.DebCred = 'C' and a.JE_ID = b.JE_ID;
	------ Insert các dòng Có
	insert into ALL_JE (BPLId, TransType, RefDate, TransId, LineId, CorrLineId, Account, ContraAct, BPCode, BPCodeCor, DocId, DocLineId, DocLineIdCor, Debit, Credit, DebitSC, CreditSC, FCDebit, FCCredit, FCCurrency, isNN, Dim1, Dim2, Dim3, Dim4, Dim5, PrjCode, Dim1Cor, Dim2Cor, Dim3Cor, Dim4Cor, Dim5Cor, PrjCodeCor)
	select a.BPLId, a.TransType, a.RefDate, a.TransId, a.LineId, b.LineId, a.Account, b.Account, a.BPCode, b.BPCode, a.DocId, a.DocLine, b.DocLine, b.Credit, b.Debit, b.CreditSC, b.DebitSC, b.FCCredit, b.FCDebit, b.FCCurrency, 'N', a.Dim1, a.Dim2, a.Dim3, a.Dim4, a.Dim5, a.PrjCode, b.Dim1, b.Dim2, b.Dim3, b.Dim4, b.Dim5, b.PrjCode
	from :tblTBL_JE_TMP a, :tblTBL_JE_TMP b
	where a.D_Count > 1 and a.C_Count = 1 and a.DebCred = 'C' and b.DebCred = 'D' and a.JE_ID = b.JE_ID;
	------Các trường hợp khác (Chỉ phát sinh Nợ, hoặc chỉ phát sinh Có, hoặc nhiều Nợ/nhiều Có) ---------------------------------------
	insert into ALL_JE (BPLId, TransType, RefDate, TransId, LineId, Account, ContraAct, BPCode, DocId, DocLineId, Debit, Credit, DebitSC, CreditSC, FCDebit, FCCredit, FCCurrency, isNN, Dim1, Dim2, Dim3, Dim4, Dim5, PrjCode)
	select a.BPLId, a.TransType, a.RefDate, a.TransId, a.LineId, a.Account, a.ContraAct, a.BPCode, a.DocId, a.DocLine, a.Debit, a.Credit, a.DebitSC, a.CreditSC, a.FCDebit, a.FCCredit, a.FCCurrency, 'Y', a.Dim1, a.Dim2, a.Dim3, a.Dim4, a.Dim5, a.PrjCode
	from :tblTBL_JE_TMP a
	where (a.D_Count = 0 or a.C_Count = 0 or (a.D_Count > 1 and a.C_Count > 1));
	--===============================================================================================================================================

	if (:display = 1) then
		select * from ALL_JE where TransId = :tNo;
	end if;
end

