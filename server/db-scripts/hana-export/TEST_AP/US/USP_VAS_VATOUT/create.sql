CREATE procedure USP_VAS_VATOUT (
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
		InvForm		nvarchar(50),
		InvSerial	nvarchar(50),
		InvNo		nvarchar(50),
		InvDate		timestamp,
		BPCode		nvarchar(50),
		BPName		nvarchar(300),
		BPTaxID		nvarchar(100),
		TotBefVAT	numeric(21,6),
		VATSum		numeric(21,6),
		VATGrp		nvarchar(30),
		Note		nvarchar(300)
	);
	
	insert into #details (JEID, JELine, DocType, DocEntry, DocID, DocDate, DocLine, InvForm, InvSerial, InvNo, InvDate, BPCode, BPName, BPTaxID, TotBefVAT, VATSum, VATGrp, Note)
	select
		b."TransId",
		b."Line_ID",
		a."ObjType",
		a."DocEntry",
		a."DocNum",
		a."DocDate",
		-1,
		a."U_invSym",
		a."U_InvSerial",
		a."U_InvNo",
		a."U_InvDate",
		a."CardCode",
		a."CardName",
		a."LicTradNum",
		b."BaseSum",
		IFNULL(b."Credit",0)-IFNULL(b."Debit",0),
		b."VatGroup",
		a."Comments"
	from
		OINV a
		inner join JDT1 b on a."ObjType" = b."TransType" and a."DocEntry" = b."CreatedBy"
	where
		a."CANCELED" = 'N'
		and b."Account" like '3331%'
		and IFNULL(b."VatGroup",'') <> ''
		and (not (IFNULL(b."Credit",0)-IFNULL(b."Debit",0) = 0 and IFNULL(b."BaseSum",0) = 0))
		and (IFNULL(a."U_InvDate",'1900-01-01') between fdate and tdate or (a."DocDate" between fdate and tdate and IFNULL(a."U_InvNo",'') <> ''))
		and (a."BPLId"=branch or branch=0)
	union all
	select
		b."TransId",
		b."Line_ID",
		a."ObjType",
		a."DocEntry",
		a."DocNum",
		a."DocDate",
		-1,
		a."U_invSym",
		a."U_InvSerial",
		a."U_InvNo",
		a."U_InvDate",
		a."CardCode",
		a."CardName",
		a."LicTradNum",
		-b."BaseSum",
		IFNULL(b."Credit",0)-IFNULL(b."Debit",0),
		b."VatGroup",
		a."Comments"
	from
		ORIN a
		inner join JDT1 b on a."ObjType" = b."TransType" and a."DocEntry" = b."CreatedBy"
	where
		a."CANCELED" = 'N'
		and b."Account" like '3331%'
		and IFNULL(b."VatGroup",'') <> ''
		and (not (IFNULL(b."Credit",0)-IFNULL(b."Debit",0) = 0 and IFNULL(b."BaseSum",0) = 0))
		and (IFNULL(a."U_InvDate",'1900-01-01') between fdate and tdate or (a."DocDate" between fdate and tdate and IFNULL(a."U_InvNo",'') <> ''))
		and (a."BPLId"=branch or branch=0)
	union all
	select
		a."TransId",
		a."Line_ID",
		a."ObjType",
		a."TransId",
		b."Number",
		a."RefDate",
		a."Line_ID",
		a."U_invSym",
		a."U_InvSerial",
		a."U_InvNo",
		a."U_InvDate",
		a."U_BPCode",
		a."U_BPName",
		a."U_BPTaxId",
		a."BaseSum",
		IFNULL(a."Credit",0)-IFNULL(a."Debit",0),
		a."VatGroup",
		a."LineMemo"
	from
		JDT1 a
		left join OJDT b on b."TransId" = a."TransId"
		left join OJDT c on c."StornoToTr" = a."TransId"
	where
		a."TransType" = '30'
		and IFNULL(b."StornoToTr",-1) = -1
		and c."TransId" is null
		and a."Account" like '3331%'
		and IFNULL(a."VatGroup",'') <> ''
		and (not (IFNULL(a."Credit",0)-IFNULL(a."Debit",0) = 0 and IFNULL(a."BaseSum",0) = 0))
		and (IFNULL(a."U_InvDate",'1900-01-01') between fdate and tdate or (a."RefDate" between fdate and tdate and IFNULL(a."U_InvNo",'') <> ''))
		and (a."BPLId"=branch or branch=0);
		
	update #details set
		ID		= CAST(JEID as nvarchar(11)) || '/' || CAST(JELine as nvarchar(11)),
		DocID	= DocType || ' ' || DocID;
	
	tab_Cat =
		select a."Code", a."ReportCode"
		from OVTG a
		where a."Category" = 'O' and IFNULL(a."ReportCode",'') <> '';
		
	create local temporary table #summary (
		ID				nvarchar(30),
		DocType			nvarchar(50),
		DocEntry		int,
		DocDate			timestamp,
		DocLine			int,
		InvType			nvarchar(30),
		InvTypeName		nvarchar(100),
		InvForm			nvarchar(50),
		InvSerial		nvarchar(50),
		InvNo			nvarchar(50),
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
		CompTaxId		nvarchar(100)
	);
	
	insert into #summary (ID, InvType, InvTypeName, InvForm, InvSerial, InvNo, InvCode, InvDate, BPCode, BPName, BPTaxId, TotBefVAT, VATSum)
	select
		MIN(a.ID),
		b."Code",
		b."ReportCode",
		a.InvForm,
		a.InvSerial,
		a.InvNo,
		IFNULL(a.InvNo,a.DocID),
		a.InvDate,
		a.BPCode,
		a.BPName,
		a.BPTaxId,
		SUM(a.TotBefVAT),
		SUM(a.VATSum)
	from
		:tab_Cat b
		left join #details a on a.VATGrp = b."Code"
	group by
		b."Code",
		b."ReportCode",
		a.InvForm,
		a.InvSerial,
		a.InvNo,
		IFNULL(a.InvNo,a.DocID),
		a.InvDate,
		a.BPCode,
		a.BPName,
		a.BPTaxId;
	
	update a set DocType = b.DocType, DocEntry = b.DocEntry, DocDate = b.DocDate, DocLine = b.DocLine, Note = b.Note from #summary a, #details b where a.ID = b.ID;

	update a set DocUrl = 'http://$b1$/link?table=' || b."ObjName" || '&key=' || CAST(a.DocEntry as nvarchar(11)) from #summary a, AZ_OBJTYPE b where a.DocType = b."ObjID";

	update a set CompName = b."PrintHeadr", CompTaxId = b."TaxIdNum" from #summary a, OADM b;
	
	select * from #summary order by InvDate, InvNo;
	
	drop table #details;
	drop table #summary;
end

