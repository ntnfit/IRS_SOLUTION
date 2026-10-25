create function Function_GetApprName(
	in docID int,
	in ObjType nVarChar(20)
)
	returns table (
		NguoiDuyet1 	nvarchar(50),
		NgayNguoi1Duyet nvarchar(50),
		NguoiDuyet2 	nvarchar(50),
		NgayNguoi2Duyet nvarchar(50),	
		NguoiDuyet3 	nvarchar(50),
		NgayNguoi3Duyet nvarchar(50),	
		NguoiDuyet4 	nvarchar(50),
		NgayNguoi4Duyet nvarchar(50),	
		NguoiDuyet5 	nvarchar(50),
		NgayNguoi5Duyet nvarchar(50)	
	)
AS
BEGIN
	DECLARE nguoiduyet table (
		NguoiDuyet1 	nvarchar(50),
		NgayNguoi1Duyet nvarchar(50),
		NguoiDuyet2 	nvarchar(50),
		NgayNguoi2Duyet nvarchar(50),	
		NguoiDuyet3 	nvarchar(50),
		NgayNguoi3Duyet nvarchar(50),	
		NguoiDuyet4 	nvarchar(50),
		NgayNguoi4Duyet nvarchar(50),	
		NguoiDuyet5 	nvarchar(50),
		NgayNguoi5Duyet nvarchar(50)	
	);
	-- Update thông tin người duyệt
	insert into :nguoiduyet(NguoiDuyet1) select 1 from dummy;
	
	update :nguoiduyet set NguoiDuyet1 = (
	select  top 1 c."lastName" || case when ifnull( c."middleName",'') = '' then '' else ' '||  c."middleName"  ||' '||  c."firstName"  end 
						from  OHEM c
						where "userId"  = (select top 1  "UserID" 
											from WDD1 a 
											inner join OWDD b on a."WddCode"  = b."WddCode"
											where b."DocEntry"  =  :DocID and "SortId"  = 1 and b."ObjType" = :ObjType));
	
	
	
	update :nguoiduyet set NgayNguoi1Duyet = (
	select top 1  case when  ifnull("UpdateDate",'') = '' then '' else ' - dated '|| TO_VARCHAR("UpdateDate",'DD/MM/YYYY') end
											from WDD1 a 
											inner join OWDD b on a."WddCode"  = b."WddCode"
											where b."DocEntry"  =  :DocID and "SortId"  = 1 and b."ObjType" = :ObjType) ;
	
--- Người duyệt 2
	update :nguoiduyet set NguoiDuyet2 = (
	select  top 1 c."lastName" || case when ifnull( c."middleName",'') = '' then '' else ' '||  c."middleName"  ||' '||  c."firstName"  end 
						from  OHEM c
						where "userId"  = (select top 1  "UserID" 
											from WDD1 a 
											inner join OWDD b on a."WddCode"  = b."WddCode"
											where b."DocEntry"  =  :DocID and "SortId"  = 2 and b."ObjType" = :ObjType));
								
	 
	update :nguoiduyet set NgayNguoi2Duyet = (
	select top 1 case when  ifnull("UpdateDate",'') = '' then '' else ' - dated '|| TO_VARCHAR("UpdateDate",'DD/MM/YYYY') end
											from WDD1 a 
											inner join OWDD b on a."WddCode"  = b."WddCode"
											where b."DocEntry"  =  :DocID and "SortId"  = 2 and b."ObjType" = :ObjType);
	
	
---- Nguoi duyệt 3
	update :nguoiduyet set NguoiDuyet3 = (
	select  top 1 c."lastName" || case when ifnull( c."middleName",'') = '' then '' else ' '||  c."middleName"  ||' '||  c."firstName"  end 
						from  OHEM c
						where "userId"  = (select top 1  "UserID" 
											from WDD1 a 
											inner join OWDD b on a."WddCode"  = b."WddCode"
											where b."DocEntry"  =  :DocID and "SortId"  = 3 and b."ObjType" = :ObjType)) ;
	
	update :nguoiduyet set NgayNguoi3Duyet = (
	select top 1   case when  ifnull("UpdateDate",'') = '' then '' else ' - dated '|| TO_VARCHAR("UpdateDate",'DD/MM/YYYY') end
											from WDD1 a 
											inner join OWDD b on a."WddCode"  = b."WddCode"
											where b."DocEntry"  =  :DocID and "SortId"  = 3 and b."ObjType" = :ObjType);
	

------- Người duyệt 4
	update :nguoiduyet set NguoiDuyet4 = (
	select  top 1 c."lastName" || case when ifnull( c."middleName",'') = '' then '' else ' '||  c."middleName"  ||' '||  c."firstName"  end 
						from  OHEM c
						where "userId"  = (select top 1  "UserID" 
											from WDD1 a 
											inner join OWDD b on a."WddCode"  = b."WddCode"
											where b."DocEntry"  =  :DocID and "SortId"  = 4 and b."ObjType" = :ObjType)) ;
	
	update :nguoiduyet set NgayNguoi4Duyet = (
	select top 1   case when  ifnull("UpdateDate",'') = '' then '' else ' - dated '|| TO_VARCHAR("UpdateDate",'DD/MM/YYYY') end
											from WDD1 a 
											inner join OWDD b on a."WddCode"  = b."WddCode"
											where b."DocEntry"  =  :DocID and "SortId"  = 4 and b."ObjType" = :ObjType) ;
	
	return select * from :nguoiduyet ;
											
END
