CREATE PROCEDURE USP_FORM_ILD_PurchaseOrder_SubDocument(
	in docID				int
)
AS
BEGIN

	declare tbl table(
		-- Original
		DocEntry			int
		-- Payment Doc	
		,IDNum				int
		,PayName			nvarchar(254)
		,PayNameVN			nvarchar(254)
		,PayQty				int
		,PayCopy			nvarchar(50)
		--
		,Note2				nvarchar(254)
		,PayTerm			nvarchar(254)
		,PaymentDetail		nvarchar(4000)
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
	);
	
	insert into :tbl(DocEntry,IDNum,PayName,PayNameVN,PayQty,PayCopy,Note2,PayTerm,PaymentDetail,DelTerm,ImportNote)
	select a."DocEntry",row_number() over (order by b."LineId"),b."U_Name",b."U_VN_Name",b."U_Qty",case b."U_DocType" when 'O' then 'Origin' when 'C' then 'Copy' end
	,'The quotation' || IFNULL(' ' || a."U_PQ_No",'') || ' is an integral part of the purchase order.'
	,k."PymntGroup",cast(a."U_Payment_Detail" as nvarchar(4000)),a."U_Incoterm",a."Comments"
	from 
		OPOR a
		left join OCTG k on k."GroupNum" = a."GroupNum"
		left join "@PO_DOC_GR1" b on a."U_Payment_Doc" = b."Code"
	where
		a."DocEntry" = :docID;

	----  Update thông tin người duyệt 
/*	
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
*/	
	update a
	set NgayNguoi4Duyet = (select top 1   case when  ifnull("UpdateDate",'') = '' then '' else ' - dated '|| TO_VARCHAR("UpdateDate",'DD/MM/YYYY') end
											from WDD1 a 
											inner join OWDD b on a."WddCode"  = b."WddCode"
											where b."DocEntry"  =  :DocID and "SortId"  = 4 and b."ObjType" = 22)
	from :tbl a ;

	select * from :tbl;
	
END
