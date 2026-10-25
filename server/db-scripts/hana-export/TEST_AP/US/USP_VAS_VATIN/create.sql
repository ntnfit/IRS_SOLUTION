CREATE procedure USP_VAS_VATIN (
	in fdate timestamp,
	in tdate timestamp,
	in branch int
)
as
begin
	create local temporary table #details (
		ID			nvarchar(30),
		JEID		int,
		JELine		int,
		DocType		nvarchar(30),
		DocEntry	int,
		DocID		nvarchar(100),
		DocDate		timestamp,
		DocLine		int,
		InvType		int,
		InvForm		nvarchar(50),
		InvSerial	nvarchar(50),
		InvNo		nvarchar(254),
		InvDate		timestamp,
		PeriodDate	timestamp,
		BPCode		nvarchar(50),
		BPName		nvarchar(300),
		BPTaxID		nvarchar(100),
		TotBefVAT	numeric(21,6),
		VATSum		numeric(21,6),
		
		--Doc Line
		Line_InvType	int,
		Line_InvForm	nvarchar(50),
		Line_InvSerial	nvarchar(50),
		Line_InvNo		nvarchar(254),
		Line_InvDate	timestamp,
		Line_PeriodDate	timestamp,
		Line_BPCode		nvarchar(50),
		Line_BPName		nvarchar(300),
		Line_BPTaxID	nvarchar(100),
		
		Note		nvarchar(300),
		
		Currency	nvarchar(10)
	);
	-- AP Invoice
	insert into #details (JEID, JELine, DocType, DocEntry, DocID, DocDate, DocLine, InvType, InvForm, InvSerial, InvNo, InvDate, PeriodDate, BPCode, BPName, BPTaxID, TotBefVAT, VATSum, Note,
							Line_InvType, Line_InvForm, Line_InvSerial, Line_InvNo, Line_InvDate, Line_PeriodDate, Line_BPCode, Line_BPName, Line_BPTaxID,Currency)
	select
		a."TransId",
		0,
		a."ObjType",
		a."DocEntry",
		a."DocNum",
		a."DocDate",
		-1,
		a."U_InvType",
		a."U_invSym",
		a."U_InvSerial",
		a."U_APInv",
		a."U_InvDate",
		a."U_InvPeriod",
		a."CardCode",
		a."CardName",
		a."LicTradNum",
		case when b."Currency" = 'USD' then b."LineTotal" else b."TotalFrgn" end,-- ,
		b."VatSum",
		a."JrnlMemo",
		a."U_InvType",
		a."U_invSym",
		a."U_InvSerial",
		a."U_InvNo",
		a."U_InvDate",
		a."U_InvPeriod",
		a."CardCode",
		a."CardName",
		a."LicTradNum",
		b."Currency"
	from
		OPCH a, PCH1 b
	where
		a."CANCELED" = 'N' and a."DocEntry"=b."DocEntry"
		and (a."U_isVat"='Y'or a."U_isVat"='Y')
		and ifnull(ifnull(a."U_InvPeriod",a."U_InvPeriod"),a."DocDate") between fdate and tdate
--		and a."DocDate" between fdate and tdate
		and (a."BPLId"=branch or branch=0);
	-- AP Memo
	insert into #details (JEID, JELine, DocType, DocEntry, DocID, DocDate, DocLine, InvType, InvForm, InvSerial, InvNo, InvDate, PeriodDate, BPCode, BPName, BPTaxID, TotBefVAT, VATSum, Note,
							Line_InvType, Line_InvForm, Line_InvSerial, Line_InvNo, Line_InvDate, Line_PeriodDate, Line_BPCode, Line_BPName, Line_BPTaxID,Currency)
	select
		a."TransId",
		0,
		a."ObjType",
		a."DocEntry",
		a."DocNum",
		a."DocDate",
		-1,
		a."U_InvType",
		a."U_invSym",
		a."U_InvSerial",
		a."U_InvNo",
		a."U_InvDate",
		a."U_InvPeriod",
		a."CardCode",
		a."CardName",
		a."LicTradNum",
		-case when b."Currency" = 'USD' then b."LineTotal" else b."TotalFrgn" end,
		-b."VatSum",
		a."JrnlMemo",
		a."U_InvType",
		a."U_invSym",
		a."U_InvSerial",
		a."U_InvNo",
		a."U_InvDate",
		a."U_InvPeriod",
		a."CardCode",
		a."CardName",
		a."LicTradNum",
		b."Currency"
	from
		ORPC a, RPC1 b
	where
		a."CANCELED" = 'N' and a."DocEntry"=b."DocEntry"
		and (a."U_isVat"='Y'or a."U_isVat"='Y')
		and ifnull(ifnull(a."U_InvPeriod",a."U_InvPeriod"),a."DocDate") between fdate and tdate
		and (a."BPLId"=branch or branch=0);
	-- Outgoing Payment
	insert into #details (JEID, JELine, DocType, DocEntry, DocID, DocDate, DocLine, InvType, InvForm, InvSerial, InvNo, InvDate, PeriodDate, BPCode, BPName, BPTaxID, TotBefVAT, VATSum, Note,Currency)
	select
		a."TransId",
		0,
		a."ObjType",
		a."DocEntry",
		a."DocNum",
		a."DocDate",
		-1,
		b."U_InvType",
		b."U_invSym",
		b."U_InvSerial",
		b."U_InvNo",
		b."U_InvDate",
		b."U_InvPeriod",
		b."U_BPCode",
		b."U_BPName",
		b."U_BPTaxId",
		b."SumApplied",
		b."VatAmnt",
		a."JrnlMemo",
		a."DocCurr"
	from
		OVPM a, VPM4 b
	where
		a."Canceled" = 'N' and a."DocEntry"=b."DocNum"
		and b."U_isVat"='Y'
		and ifnull(b."U_InvPeriod",a."DocDate") between fdate and tdate
		and (a."BPLId"=branch or branch=0);
	-- Manual JE
	insert into #details (JEID, JELine, DocType, DocEntry, DocID, DocDate, DocLine, InvType, InvForm, InvSerial, InvNo, InvDate, PeriodDate, BPCode, BPName, BPTaxID, TotBefVAT, VATSum, Note,Currency)
	select
		a."TransId",
		a."Line_ID",
		a."ObjType",
		a."TransId",
		b."Number",
		a."RefDate",
		a."Line_ID",
		a."U_InvType",
		a."U_invSym",
		a."U_InvSerial",
		a."U_InvNo",
		a."U_InvDate",
		a."U_InvPeriod",
		a."U_BPCode",
		a."U_BPName",
		a."U_BPTaxId",
		a."BaseSum",
		--IFNULL(a."Debit",0)-IFNULL(a."Credit",0),
		case when a."FCCurrency" is not null then IFNULL(a."FCDebit",0)-IFNULL(a."FCCredit",0) else IFNULL(a."Debit",0)-IFNULL(a."Credit",0) end,
		a."LineMemo",
		ifnull(a."FCCurrency",'USD')
	from
		JDT1 a
		left join OJDT b on b."TransId" = a."TransId"
		left join OJDT c on c."StornoToTr" = a."TransId"
	where
		a."TransType" = '30'
		and IFNULL(b."StornoToTr",-1) = -1
		and c."TransId" is null
		and a."Account" like '133%'
		and IFNULL(a."VatGroup",'') <> ''
		and (not (IFNULL(a."Debit",0)-IFNULL(a."Credit",0) = 0 and IFNULL(a."BaseSum",0) = 0))
		and IFNULL(a."U_InvPeriod",a."RefDate") between fdate and tdate
		and (a."BPLId"=branch or branch=0);
	-----------------------------------------------------------------------------

	update #details set InvType 	= Line_InvType where ifnull(Line_InvType,0)<>0;
	update #details set InvForm 	= Line_InvForm where ifnull(Line_InvForm,'')<>'';
	update #details set InvSerial 	= Line_InvSerial where ifnull(Line_InvSerial,'')<>'';
	update #details set InvNo 		= Line_InvNo where ifnull(Line_InvNo,'')<>'';
	update #details set InvDate 	= Line_InvDate where Line_InvDate is not null;
	update #details set PeriodDate 	= Line_PeriodDate where Line_PeriodDate is not null;
	update #details set BPCode 		= Line_BPCode where ifnull(Line_BPCode,'')<>'';
	update #details set BPName 		= Line_BPName where ifnull(Line_BPName,'')<>'';
	update #details set BPTaxID 	= Line_BPTaxID where ifnull(Line_BPTaxID,'')<>'';
	
	update #details set InvForm = 'PU ' || DocID where DocType='18';
	update #details set InvForm = 'PC ' || DocID where DocType='19';
	update #details set InvForm = 'PS ' || DocID where DocType='46';
	update #details set InvForm = 'JE ' || DocID where DocType='30';
	
	update #details set
		ID		= CAST(JEID as nvarchar(11)) || '/' || CAST(JELine as nvarchar(11)),
		DocID	= DocType || ' ' || DocID;
	
	tab_Cat =
		select a."FldValue", a."Descr"
		from UFD1 a,CUFD b
		where a."TableID" = 'OPCH' and a."TableID" = b."TableID" and a."FieldID" = b."FieldID" and b."AliasID" = 'InvType';
	
	create local temporary table #summary (
		ID				nvarchar(30),
		DocType			nvarchar(50),
		DocEntry		int,
		DocDate			timestamp,
		DocLine			int,
		InvType			int,
		InvTypeName		nvarchar(500),
		InvTypeDescr	nvarchar(500),
		InvForm			nvarchar(50),
		InvSerial		nvarchar(50),
		InvNo			nvarchar(254),
		InvCode			nvarchar(254),
		InvDate			timestamp,
		BPCode			nvarchar(50),
		BPName			nvarchar(254),
		BPTaxId			nvarchar(100),
		TotBefVAT		numeric(19,6),
		VATSum			numeric(19,6),
		Note			nvarchar(4000),
		DocUrl			nvarchar(100),
		CompName		nvarchar(254),
		CompAdd			nvarchar(300),
		CompTaxId		nvarchar(100),
		Currency	nvarchar(10)
	);
	
	insert into #summary (ID, InvType, InvTypeName, InvTypeDescr, InvForm, InvSerial, InvNo, InvCode, InvDate, BPCode, BPName, BPTaxId, TotBefVAT, VATSum, Currency)
	select
		MIN(a.ID),
		b."FldValue",
		b."Descr",
		'   ' || b."Descr",
		a.InvForm,
		a.InvSerial,
		a.InvNo,
		IFNULL(a.InvNo,a.DocID),
		a.InvDate,
		a.BPCode,
		a.BPName,
		a.BPTaxId,
		SUM(a.TotBefVAT),
		SUM(a.VATSum),
		a.Currency
	from
		:tab_Cat b
		inner join #DETAILS a on a.InvType = b."FldValue"
	group by
		b."FldValue",
		b."Descr",
		a.InvForm,
		a.InvSerial,
		a.InvNo,
		IFNULL(a.InvNo,a.DocID),
		a.InvDate,
		a.BPCode,
		a.BPName,
		a.BPTaxId,
		a.Currency
		;
	
	update a set DocType = b.DocType, DocEntry = b.DocEntry, DocDate = b.DocDate, DocLine = b.DocLine, Note = b.Note from #summary a, #details b where a.ID = b.ID;

	update a set DocUrl = 'http://$b1$/link?table=' || b."ObjName" || '&key=' || CAST(a.DocEntry as nvarchar(11)) from #summary a, AZ_OBJTYPE b where a.DocType = b."ObjID";

	update a set CompName = b."PrintHeadr", CompTaxId = b."TaxIdNum" from #summary a, OADM b;
	
	select * from #summary order by InvDate, InvNo;
	
	drop table #details;
	drop table #summary;
end
