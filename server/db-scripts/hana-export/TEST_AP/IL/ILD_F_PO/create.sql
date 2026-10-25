CREATE procedure ILD_F_PO (
	in DocID	int
)
as
begin
	declare bplID int;
	declare DecQty, DecPrice int;
	declare DateSep, ThousSep nvarchar(10);
	
	--select ifnull(x."BPLId",0) into bplID from "OPOR" x where x."DocEntry"=DocID;

declare tbl table (
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
		ItmCode		nvarchar(50),
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
		Prepared	nvarchar(100),
		Checked		nvarchar(100),
		Approved	nvarchar(100),
		Notes		nvarchar(300),
		OrderDate	date,
		BuyerName	nvarchar(150),
		ReqDate		date,
		GTotal 		numeric(21,6),
		NguoiDuyet1 nvarchar(50),
		NgayNguoi1Duyet nvarchar(50),
		
		NguoiDuyet2 nvarchar(50),
		NgayNguoi2Duyet nvarchar(50),
		
		NguoiDuyet3 nvarchar(50),
		NgayNguoi3Duyet nvarchar(50),
		
		NguoiDuyet4 nvarchar(50),
		NgayNguoi4Duyet nvarchar(50),
		
		DocCur char(4),
		RateLine decimal(21,6),
		
		--format number
		AmountDec		smallint,
		QtyDec			smallint,
		PriceDec		smallint,
		RateDec			smallint,
		PercentDec		smallint,
		DecSep			nvarchar(1),
		ThousSep		nvarchar(1)
	);

	insert into :tbl (CompName,CompAddr,CompTel,CompFax,DocID,LinID,DocNo,DocDate,PONo,DelDate
		,BPCode,BPName,BPAddr,BPTel,BPFax,BPAttn
		,CpName,CpAddr,CpTel,CpFax,Reqner
		,ItmCode
		,ItmName
		,Uom,Qty,Price,Curr,LineSum
		,Total,VatPrcnt,DocVat,DocTotal
		,Remark,Notes,PayTerm
		,DelTerm,DelVia
		,Requester,Deparment
		,Prepared,Checked,Approved,
		ReqDate, GTotal,
		DocCur,
		RateLine
		)
	select d."PrintHdrF",d."CmpnyAddrF",d."Phone1",d."Fax",
		a."DocEntry",b."LineNum"*1000,a."DocNum" as DocNo, a."DocDate",
		--a."U_JVNumber",
		a."DocNum" as  "PONo",
		a."DocDueDate"
		,c."CardCode",c."CardName",c."Address",c."Phone1",c."Fax",(select "Name" from "OCPR" where "CntctCode"=a."CntctCode") as BPAttn
		,d."PrintHdrF",replace(d."CmpnyAddrF",'VIETNAM',''),d."Phone1",d."Fax",(select "SlpName" from "OSLP" where "SlpCode"=a."SlpCode") as CpAttn
		,b."ItemCode"
		,b."Dscription"
		,b."unitMsr",
		b."Quantity",
		b."Price",
		b."Currency",
		CASE when a."DocCur"<>'USD' then b."TotalFrgn" else b."LineTotal"   end as LineSum 
		,CASE when a."DocCur"<>'USD' then a."DocTotalFC"-a."VatSumFC" else  a."DocTotal"-a."VatSum" end as Total
		,b."VatPrcnt"
		,CASE when a."DocCur"<>'USD' then a."VatSumFC" else a."VatSum" end as DocVat 
		,CASE when a."DocCur"<>'USD' then a."DocTotalFC"  else  a."DocTotal" end as DocTotal
		,a."Comments",a."Comments",f."PymntGroup"
		  
		,a."U_Incoterm" as "DelTerm"
		
		--,a."U_PortNo"
		,''
		
		,(select  x."ReqName"  from "OPRQ" x where x."DocEntry"=b."BaseEntry")
		,(select  y."Name"  from "OPRQ" x, "OUDP" y where x."DocEntry"=b."BaseEntry" and x."Department"=y."Code")
		--,a."U_Prepared",
		,''
		--,a."U_Checked"
		,''
		--,a."U_Approved"
		,''
		--, (select distinct max(x."PQTReqDate") from "PRQ1" x where x."DocEntry"=b."BaseEntry" and x."ItemCode"  =  b."ItemCode"  )
		,a."DocDueDate"
		,CASE when a."DocCur"<>'USD' then B."GTotalFC" else b."GTotal" end as GTotal,
		a."DocCur",
		case when b."Rate" <=0 then 1 else b."Rate" end
	from "OPOR" a
	left join "POR1" b on b."DocEntry"=a."DocEntry"
	left join "OCRD" c on c."CardCode"=a."CardCode"
	left join "OADM" d on d."CompnyName" is not null
	left join "OITM" e on e."ItemCode"=b."ItemCode"
	left join "OCTG" f on f."GroupNum"=a."GroupNum"
	where a."DocEntry"=DocID;
	-- and   e."InvntItem"   =  'Y';
	--- Update Gia Tri VND khi DocCur  ='USD'
	Update a
	set   GTotal  = GTotal * RateLine , 
					LineSum  = LineSum*   RateLine, 
					DocVat  = DocVat * RateLine,
					DocTotal = DocTotal * RateLine
				
	from :tbl a
	where a.DocCur ='USD';
	----
	
	update :tbl set RptName = N'PURCHASE ORDER';
	-- Update format number
	update x set 
			AmountDec	= case when x.DocCur ='VND' then 0 else 2 end,
			QtyDec		= y."QtyDec",
			PriceDec	= y."PriceDec",
			RateDec		=  case when x.DocCur ='VND' then 0 else 2 end,
			PercentDec	= y."PercentDec",
			DecSep		= y."DecSep",
			ThousSep	= y."ThousSep"
	from :tbl x inner join OADM y on 1=1;
	--update :tbl set ValInText = Num2Text(to_integer(DocTotal));
    --update :tbl set ValInText = Num2Text(DocTotal);
	
	-- update Requisitioner, Delivery Area
	update :tbl set
		CpAttn	= (select CONCAT(CONCAT(CONCAT(CONCAT(IFNULL("lastName",''),' '),IFNULL("middleName",'')),' '),IFNULL("firstName",'')) from "OHEM" where "empID"=(select "OwnerCode" from "OPOR" where "DocEntry"=DocID)),
		CpTel	= (select "mobile" from "OHEM" where "empID"=(select "OwnerCode" from "OPOR" where "DocEntry"=DocID)),
--		DelVia	= (select "TrnspName" from "OSHP" where "TrnspCode"= (select "TrnspCode" from "OPOR" where "DocEntry"=DocID)),
--		DelTerm	= 'ĐK giao hàng',
		VatPrcnt= (select max(VatPrcnt) from :tbl),
		ItmCode = IFNULL(ItmCode,''),
		LinID	= CASE when LinID IS NULL then 100000000 else LinID end;
----  Update thông tin người duyệt 

update a
set NguoiDuyet1 = (select  top 1 c."lastName" || case when ifnull( c."middleName",'') = '' then '' else ' '||  c."middleName"  ||' '||  c."firstName"  end 
					from  OHEM c
					where "userId"  = (select top 1  "UserID" 
										from WDD1 a 
										inner join OWDD b on a."WddCode"  = b."WddCode"
										where b."DocEntry"  =  :DocID and "SortId"  = 1 and b."ObjType" = 22))
from :tbl a ;


update a
set NgayNguoi1Duyet = (select top 1  case when  ifnull("UpdateDate",'') = '' then '' else ' - dated '|| TO_VARCHAR("UpdateDate",'DD/MM/YYYY') end
										from WDD1 a 
										inner join OWDD b on a."WddCode"  = b."WddCode"
										where b."DocEntry"  =  :DocID and "SortId"  = 1 and b."ObjType" = 22)
from :tbl a ;

--- Người duyệt 2


update a
set NguoiDuyet2 = (select  top 1 c."lastName" || case when ifnull( c."middleName",'') = '' then '' else ' '||  c."middleName"  ||' '||  c."firstName"  end 
					from  OHEM c
					where "userId"  = (select top 1  "UserID" 
										from WDD1 a 
										inner join OWDD b on a."WddCode"  = b."WddCode"
										where b."DocEntry"  =  :DocID and "SortId"  = 2 and b."ObjType" = 22))
							
from :tbl a ;

 
update a
set NgayNguoi2Duyet = (select top 1 case when  ifnull("UpdateDate",'') = '' then '' else ' - dated '|| TO_VARCHAR("UpdateDate",'DD/MM/YYYY') end
										from WDD1 a 
										inner join OWDD b on a."WddCode"  = b."WddCode"
										where b."DocEntry"  =  :DocID and "SortId"  = 2 and b."ObjType" = 22)
from :tbl a ;


---- Nguoi duyệt 3
update a
set NguoiDuyet3 = (select  top 1 c."lastName" || case when ifnull( c."middleName",'') = '' then '' else ' '||  c."middleName"  ||' '||  c."firstName"  end 
					from  OHEM c
					where "userId"  = (select top 1  "UserID" 
										from WDD1 a 
										inner join OWDD b on a."WddCode"  = b."WddCode"
										where b."DocEntry"  =  :DocID and "SortId"  = 3 and b."ObjType" = 22))
							
from :tbl a ;

update a
set NgayNguoi3Duyet = (select top 1   case when  ifnull("UpdateDate",'') = '' then '' else ' - dated '|| TO_VARCHAR("UpdateDate",'DD/MM/YYYY') end
										from WDD1 a 
										inner join OWDD b on a."WddCode"  = b."WddCode"
										where b."DocEntry"  =  :DocID and "SortId"  = 3 and b."ObjType" = 22)
from :tbl a ;



------- Người duyệt 4
update a
set NguoiDuyet4 = (select  top 1 c."lastName" || case when ifnull( c."middleName",'') = '' then '' else ' '||  c."middleName"  ||' '||  c."firstName"  end 
					from  OHEM c
					where "userId"  = (select top 1  "UserID" 
										from WDD1 a 
										inner join OWDD b on a."WddCode"  = b."WddCode"
										where b."DocEntry"  =  :DocID and "SortId"  = 4 and b."ObjType" = 22))
							
from :tbl a ;

update a
set NgayNguoi4Duyet = (select top 1   case when  ifnull("UpdateDate",'') = '' then '' else ' - dated '|| TO_VARCHAR("UpdateDate",'DD/MM/YYYY') end
										from WDD1 a 
										inner join OWDD b on a."WddCode"  = b."WddCode"
										where b."DocEntry"  =  :DocID and "SortId"  = 4 and b."ObjType" = 22)
from :tbl a ;


----------------

	
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
		DocCur "DocCur",
		DocVat "DocVat",
		Others "Others",
		DocTotal "DocTotal",
		ValInText "ValInText",
		LineCount "LineCount",
		Remark "Remark",
		Prepared "Prepared",
		Checked "Checked",
		Approved "Approved",
		Notes "Notes",
		OrderDate	"OrderDate",
		BuyerName	"BuyerName",
		ReqDate "ReqDate",
		GTotal 	"GTotal",
		
		NguoiDuyet1 "NguoiDuyet1",
		NgayNguoi1Duyet "NgayNguoi1Duyet",
		
		NguoiDuyet2 "NguoiDuyet2",
		NgayNguoi2Duyet "NgayNguoi2Duyet",
		
		NguoiDuyet3 "NguoiDuyet3",
		NgayNguoi3Duyet "NgayNguoi3Duyet",
		
		NguoiDuyet4 "NguoiDuyet4",
		NgayNguoi4Duyet "NgayNguoi4Duyet",
		-- Format Number
		AmountDec		"AmountDec",
		QtyDec			"QtyDec",
		PriceDec		"PriceDec",
		RateDec			"RateDec",
		PercentDec		"PercentDec",
		DecSep			"DecSep",
		ThousSep		 "ThousSep"
		
	from :tbl;

end
