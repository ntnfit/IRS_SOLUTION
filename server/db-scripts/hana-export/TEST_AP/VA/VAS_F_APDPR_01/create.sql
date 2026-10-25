CREATE procedure VAS_F_APDPR_01 (
	in ObjType	nvarchar(20),
	in DocID	int
)
as
begin
	declare bplID int;
	declare DecQty, DecPrice int;
	declare DateSep, ThousSep nvarchar(10);
	
	select ifnull(x."BPLId",0) into bplID from "ODPO" x where x."DocEntry"=DocID;

	create local temporary table #tbl (
		RptName		nvarchar(100),
		CompName	nvarchar(300),
		CompAddr	nvarchar(300), 
		CompTel		nvarchar(30),
		CompFax		nvarchar(30),
		
		BaseType	nvarchar(20),
		BaseEntry	int,
		BaseLine	int,
		
		LoginID		int,
		PRID		int,
		ReqType		nvarchar(20),
		Requester	nvarchar(100),
		EmployeeNo	nvarchar(50),
		Department	nvarchar(100),
		JobTitle	nvarchar(100),
		DueDate		timestamp,
		Currency	nvarchar(10),
		DocDate		timestamp,
		DiaDiemPSCP nvarchar(100),
		
		DocID		int,
		DocNo		nvarchar(100),
		BPCode		nvarchar(20),
		BPName		nvarchar(300), 
		BPBankName	nvarchar(300),
		BPBankAcct	nvarchar(50),
		BPBankBranch nvarchar(100),
		PayMethod	nvarchar(50),
		DelDocDate	timestamp,
		
		DienGiai	nvarchar(300),
		BoPhan		nvarchar(100),
		IPO			nvarchar(100),
		KMP			nvarchar(100),
		SoHoaDon	nvarchar(50),
		Commercial	nvarchar(50),
		AmountFC	numeric(28,10),
		ExRate		numeric(28,10),
		AmountLC	numeric(28,10),

		Notes		nvarchar(300)
	);
	insert into #tbl (LoginID,BaseType,BaseEntry,BaseLine,Requester,Department,DueDate,Currency,DocDate,DocID,DocNo,BPCode,BPName,BPBankName,BPBankAcct,DienGiai,AmountFC,ExRate,AmountLC,IPO,PayMethod)
	select a."UserSign",a1."BaseType",a1."BaseEntry",a1."BaseLine",a."ReqName",c."Remarks",a."DocDueDate",a."DocCur",a."DocDate",a."DocEntry",a."U_JVNumber",a."CardCode",a."CardName",e."BankName",d."DflAccount",a1."Dscription",
		case when a1."TotalFrgn"<>0 then a1."TotalFrgn" else a1."LineTotal" end,a1."Rate",a1."LineTotal",f."PrjName",''
	from ODPO a
	left join DPO1 a1 on a1."DocEntry"=a."DocEntry" 
	left join OHEM b on b."Code"=a."OwnerCode"
	left join OUDP c on c."Code"=a."Department"
	left join OCRD d on d."CardCode"=a."CardCode"
	left join ODSC e on e."BankCode"=d."BankCode"
	left join OPRJ f on f."PrjCode"=a."Project"
	where a."DocEntry"=:DocID;
	
	
	
	update #tbl set PRID = BaseEntry where BaseType='1470000113';
	update x set PRID = z."DocEntry" from #tbl x inner join POR1 y on y."ObjType"=x.BaseType and y."DocEntry"=x.BaseEntry and y."LineNum"=x.BaseLine inner join PRQ1 z on z."ObjType"=y."BaseType" and z."DocEntry"=y."BaseEntry" and z."LineNum"=y."BaseLine" where x.BaseType='22';
	
	update x set EmployeeNo = z."ExtEmpNo", JobTitle = z."jobTitle" from #tbl x inner join OPRQ y on y."DocEntry"=x.PRID and y."ReqType"='171' inner join OHEM z on z."empID"=y."Requester";
	
	
	update x set Requester = y."U_NAME" from #tbl x inner join OUSR y on y."USERID"=x.LoginID and ifnull(x.Requester,'')='';
	
	if bplID<>0 then
		update x set CompName = y."BPLFrName", CompAddr = y."Street" from #tbl x inner join OBPL y on 1=1;
	else
		update x set CompName = y."CompnyName", CompAddr = y."CompnyAddr" from #tbl x inner join OADM y on 1=1;
	end if;
	
	select 
		*
	from #tbl;

	drop table #tbl;
end

