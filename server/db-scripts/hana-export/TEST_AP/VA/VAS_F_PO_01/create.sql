CREATE procedure VAS_F_PO_01 (
	in DocID	int
)
as
begin
	declare bplID int;
	declare DecQty, DecPrice int;
	declare DateSep, ThousSep nvarchar(10);
	
	select ifnull(x."BPLId",0) into bplID from "OPOR" x where x."DocEntry"=DocID;

	create local temporary table #tbl (
		RptName		nvarchar(100),
		CompName	nvarchar(300),
		CompAddr	nvarchar(300), 
		CompTel		nvarchar(30),
		CompFax		nvarchar(30),
		DocID		int,
		LinID		int,
		DocNo		nvarchar(100),
		DocDate		timestamp,
		PONo		nvarchar(100),
		DelDate		timestamp,
		BPCode		nvarchar(20),
		BPName		nvarchar(300), 
		BPAddr		nvarchar(300),
		BPTel		nvarchar(30), 
		BPFax		nvarchar(30),
		BPAttn		nvarchar(50),
		CpName		nvarchar(300), 
		CpAddr		nvarchar(300),
		CpTel		nvarchar(30), 
		CpFax		nvarchar(30),
		CpAttn		nvarchar(100),
		Requester	nvarchar(100),
		Deparment	nvarchar(100),
		PayTerm		nvarchar(100),
		OrderMan	nvarchar(100),
		Reqner		nvarchar(100),
		DelVia		nvarchar(100),
		DelTerm		nvarchar(100),
		ItmCode		nvarchar(20),
		ItmName		nvarchar(300),
		Uom			nvarchar(20),
		Qty			numeric(21,6),
		Price		numeric(21,6),
		Curr		nvarchar(5),
		LineSum		numeric(21,6),
		Total		numeric(21,6),
		VatPrcnt	numeric(21,6),
		DocVat		numeric(21,6),
		Others		numeric(21,6),
		DocTotal	numeric(21,6),
		ValInText	nvarchar(1000),
		LineCount	int,
		Remark		nvarchar(300),
		Notes		nvarchar(300)
	);
	insert into #tbl (CompName,CompAddr,CompTel,CompFax,DocID,LinID,DocNo,DocDate,PONo,DelDate
		,BPCode,BPName,BPAddr,BPTel,BPFax,BPAttn
		,CpName,CpAddr,CpTel,CpFax,Reqner
		,ItmCode,ItmName,Uom,Qty,Price,Curr,LineSum
		,Total,VatPrcnt,DocVat,DocTotal
		,Remark,Notes,
		PayTerm
		)
	select d."CompnyName",d."CompnyAddr",d."Phone1",d."Fax",
		a."DocEntry",b."LineNum"*1000,N'' as DocNo, a."DocDate",a."U_JVNumber",a."DocDueDate"
		,c."CardCode",c."CardName",c."Address",c."Phone1",c."Fax",(select "Name" from "OCPR" where "CntctCode"=a."CntctCode") as BPAttn
		,d."CompnyName",replace(d."CompnyAddr",'VIETNAM',''),d."Phone1",d."Fax",(select "SlpName" from "OSLP" where "SlpCode"=a."SlpCode") as CpAttn
		,b."ItemCode",b."Dscription",
		b."unitMsr" Uom,
		b."Quantity" Qty,
		b."Price" Price,
		b."Currency",CASE when b."Currency"='VND' then b."LineTotal" else b."TotalFrgn" end as LineSum
		,CASE when a."DocCur"='VND' then a."DocTotal"-a."VatSum" else a."DocTotalFC"-a."VatSumFC" end as Total
		,b."VatPrcnt"
		,CASE when a."DocCur"='VND' then a."VatSum" else a."VatSumFC" end as DocVat
		,CASE when a."DocCur"='VND' then a."DocTotal" else a."DocTotalFC" end as DocTotal
		,a."Comments",a."Comments",f."PymntGroup"
	from "OPOR" a
	left join "POR1" b on b."DocEntry"=a."DocEntry"
	left join "OCRD" c on c."CardCode"=a."CardCode"
	left join "OADM" d on d."CompnyName" is not null
	left join "OITM" e on e."ItemCode"=b."ItemCode"
	left join "OCTG" f on f."GroupNum"=a."GroupNum"
	where a."DocEntry"=DocID;

	
	update #tbl set RptName = N'PURCHASE ORDER';

	update #tbl set ValInText = Num2Text(to_integer(DocTotal));
	
	-- update Requisitioner, Delivery Area
	update #tbl set
		CpAttn	= (select CONCAT(CONCAT(CONCAT(CONCAT(IFNULL("firstName",''),' '),IFNULL("middleName",'')),' '),IFNULL("lastName",'')) from "OHEM" where "empID"=(select "OwnerCode" from "OPOR" where "DocEntry"=DocID)),
		CpTel	= (select "mobile" from "OHEM" where "empID"=(select "OwnerCode" from "OPOR" where "DocEntry"=DocID)),
		DelVia	= (select "TrnspName" from "OSHP" where "TrnspCode"= (select "TrnspCode" from "OPOR" where "DocEntry"=DocID)),
		DelTerm	= 'ĐK giao hàng',
		VatPrcnt= (select max(VatPrcnt) from #tbl),
		ItmCode = IFNULL(ItmCode,''),
		LinID	= CASE when LinID IS NULL then 100000000 else LinID end;
		-- select * from OPOR

	
	select 
		RptName "RptName",
		CompName "CompName",
		CompAddr "CompAddr",
		CompTel "CompTel",
		CompFax "CompFax",
		DocID "DocID",
		LinID "LinID",
		DocNo "DocNo",
		DocDate "DocDate",
		PONo "PONo",
		DelDate "DelDate",
		BPCode "BPCode",
		BPName "BPName",
		BPAddr "BPAddr",
		BPTel "BPTel",
		BPFax "BPFax",
		BPAttn "BPAttn",
		CpName "CpName",
		CpAddr "CpAddr",
		CpTel "CpTel",
		CpFax "CpFax",
		CpAttn "CpAttn",
		Requester "Requester",
		Deparment "Deparment",
		PayTerm "PayTerm",
		OrderMan "OrderMan",
		Reqner "Reqner",
		DelVia "DelVia",
		DelTerm "DelTerm",
		ItmCode "ItmCode",
		ItmName "ItmName",
		Uom "Uom",
		Qty "Qty",
		Price "Price",
		Curr "Curr",
		LineSum "LineSum",
		Total "Total",
		VatPrcnt "VatPrcnt",
		DocVat "DocVat",
		Others "Others",
		DocTotal "DocTotal",
		ValInText "ValInText",
		LineCount "LineCount",
		Remark "Remark",
		Notes "Notes"
	from #tbl;

	drop table #tbl;
end

