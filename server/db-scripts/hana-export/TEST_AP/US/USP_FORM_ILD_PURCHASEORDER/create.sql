CREATE PROCEDURE USP_FORM_ILD_PurchaseOrder(
	in docID				int
)
AS
BEGIN

	declare tbl table(
		-- Buyer
		CompName			nvarchar(254)
		,CompAdd			nvarchar(254)
		,CompTax			nvarchar(50)
		,CompBank			nvarchar(254)
		,CompTel			nvarchar(50)
		,ComContact			nvarchar(254)
		-- Original
		,DocEntry			int
		,DocDate			timestamp
		,PONo				nvarchar(50)
		,PODate				timestamp
		,POExpDate			timestamp
		-- Supplier
		,VendorCode			nvarchar(50)
		,VendorName			nvarchar(254)
		,VendorAdd			nvarchar(254)
		,VendorTax			nvarchar(50)
		,VendorBank			nvarchar(254)
		,VendorTel			nvarchar(50)
		,VendorContact		nvarchar(254)
		-- Delivery To
		,DelName			nvarchar(254)
		,DelAdd				nvarchar(254)
		,DelContact			nvarchar(254)
		,DelTel				nvarchar(50)
		-- Detail
		,IDType				int
		,LineNum			int
		,ItemCode			nvarchar(50)
		,ItemDetail			nvarchar(4000)
		,Quantity			numeric(19,6)
		,UOM				nvarchar(50)
		,Price				numeric(19,6)
		,LineTotal			numeric(19,6)
		,DelDate			timestamp
		,RemarkTax			numeric(19,6)
		,TotalPO			numeric(19,6)
		,Currency			nvarchar(10)
		,AmountInWords		nvarchar(254)
		-- 
		,Note1				nvarchar(254)
		,Note2				nvarchar(254)
		,PayTerm			nvarchar(254)
		,PaymentDetail		nvarchar(4000)
		--
		,DelTerm			nvarchar(254)
		,ImportNote			nvarchar(254)
		
		,NguoiDuyet1 		nvarchar(50)
		,NgayNguoi1Duyet 	nvarchar(50)
		
		,NguoiDuyet2 		nvarchar(50)
		,NgayNguoi2Duyet 	nvarchar(50)
		
		,NguoiDuyet3 		nvarchar(50)
		,NgayNguoi3Duyet 	nvarchar(50)
		
		,NguoiDuyet4 		nvarchar(50)
		,NgayNguoi4Duyet 	nvarchar(50)
		,CompRefRegular		nvarchar(4000)
		
		,QtyDec				int
		,PriceDec			int
		,AmountDec			int
	);
	--e."SlpName"
	insert into :tbl(DocEntry,DocDate,PONo,PODate,POExpDate,VendorCode,VendorName,VendorAdd,VendorTax,VendorBank,VendorTel,DelTel
					 ,IDType,LineNum,ItemCode,ItemDetail,Quantity,UOM,Price,LineTotal,DelDate,RemarkTax,TotalPO,Currency
					 ,Note1,Note2,PayTerm,PaymentDetail,DelTerm,ImportNote,ComContact,VendorContact,DelContact)
	select a."DocEntry",a."DocDate",cast(a."DocNum" as nvarchar(50)),a."CreateDate",add_days(a."CreateDate",IFNULL(a."U_Validity_Days",0))
			,a."CardCode",a."CardName",a."Address",a."LicTradNum",d."BankName",c."Phone1",e."Telephone"
			,1,row_number() over (order by b."LineNum"),b."ItemCode",b."U_ItemDetail",b."Quantity",b."unitMsr",b."Price",case a."DocCur" when 'USD' then b."LineTotal" else b."TotalFrgn" end
			,a."DocDueDate",b."U_Remark_Tax",case a."DocCur" when 'USD' then a."DocTotal" else a."DocTotalFC" end,a."DocCur"
			,'Based On Purchase Request ' || cast(case when b."BaseType"='1470000113' then "BaseRef" else '' end as nvarchar(50))
			,'The quotation' || IFNULL(' ' || a."U_PQ_No",'') || ' is an integral part of the purchase order.'
			,k."PymntGroup",cast(a."U_Payment_Detail" as nvarchar(4000)),a."U_Incoterm",a."Comments"
			,'Contact name: ' ||ifnull(l."lastName",'') || case when ifnull( l."middleName",'') = '' then '' else ' '||  ifnull(l."middleName",'') end ||' '||  ifnull(l."firstName",'') || ' - Tel: ' || ifnull(l."mobile",'') as  "ComContact"
			,'Contact name: ' ||ifnull(n."Name",'')|| ' - Tel: ' || ifnull(n."Cellolar",'') as "VendorContact"
			,'Contact name: ' ||ifnull(m."Name",'')|| ' - Tel: ' || ifnull(m."U_Tel",'') as "DelContact"
	from 
		OPOR a
		inner join POR1 b on a."DocEntry" = b."DocEntry"
		inner join OCRD c on c."CardCode" = a."CardCode"
		left join ODSC d on d."BankCode" = c."BankCode" and d."CountryCod" = c."BankCountr"
		left join OSLP e on e."SlpCode" = a."SlpCode"
		left join OCTG k on k."GroupNum" = a."GroupNum"
		left join OHEM l on l."empID" = a."OwnerCode"	
		left join "@PO_RECIEVER" m on m."Code" = a."U_Receiver"
		left join OCPR n on n."CntctCode" = a."CntctCode"
	where
		a."DocEntry" = :docID;
	
	update :tbl set AmountInWords = N'Số tiền bằng chữ: ' || Num2Text(cast(TotalPO as bigint)) || ' đồng' where Currency = 'VND';
	update :tbl a set AmountInWords = 'Amount in Words: '  
			|| (select Num2TextEN(TO_INT(a.TotalPO)) from dummy) 
			|| (select ' ' || x."ChkName" from OCRN x where x."CurrCode"=a.Currency) 
										|| case when round(a.TotalPO,0)<>a.TotalPO then ' and ' 
										|| (select Num2TextEN(TO_INT((a.TotalPO-TO_INT(a.TotalPO))*100)) from dummy) 
										|| (select ' ' || x."Chk100Name" from OCRN x where x."CurrCode"=a.Currency) else '' end 
						where a.Currency<>'VND';
	update :tbl a set Note1 = (select max(x.Note1) from :tbl x);

	-- Update VendorAdd
	update c
	set VendorAdd = (select a."Street"
				from crd1 a
				inner join ocrd b on a."CardCode" = b."CardCode" and b."BillToDef" = a."Address"
				where b."CardCode" = c.VendorCode)
	from :tbl c 
	where ifnull(VendorAdd,'') = '' ;

/*
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
*/

	update a set CompName = x."PrintHdrF"
				,CompAdd = x1."StreetF"
				,CompTax = x."TaxIdNum"
				,CompTel = x."Phone1"
				,DelName = x."PrintHdrF"
				,DelAdd = x1."StreetF", PriceDec = x."PriceDec", AmountDec = x."SumDec" from :tbl a,OADM x,ADM1 x1;
				
	update a set PriceDec = case when x."Decimals" <> -1 then x."Decimals" else a.PriceDec end
				 ,AmountDec = case when x."Decimals" <> -1 then x."Decimals" else a.AmountDec end from :tbl a,OCRN x where a.Currency = x."CurrCode";
	
	update :tbl set QtyDec = case when round(Quantity,0) = Quantity then 0
								  when round(Quantity,1) = Quantity then 1
								  when round(Quantity,2) = Quantity then 2 else 3 end;
	
	select * from :tbl;
	
END
