CREATE procedure ILD_R_IRS_HopDong
(
	in TuNgay	date,
	in DenNgay	date,
	in PhongBan 	nvarchar(5000),
	in NhanVien 	nvarchar(5000),
	in LoaiHopDong 	nvarchar(5000),
	in SoNgaySeHetHanHD 	nvarchar(100)
)
as
Begin

create local temporary table #tbl (
		HopDongID int,
		NhanVienID int,
		MaNhanVien nvarchar(50),
		HovaTen nvarchar(100),
		NhomNhanVien nvarchar(20),
		ChucVu nvarchar(100),
		PhongBan nvarchar(100),
		
		QuanLy1 nvarchar(100),
		QuanLy2 nvarchar(100),
		SoHopDong nvarchar(100),
		LoaiHopDong nvarchar(100),
		
		NgayBatDauLam date,
		NgayBatDauHD date,
		NgayKetThucHD date,
		
		SoNgaySeHetHanHD nvarchar(20),
		
		NVGioiThieuID int,
		NguoiGioiThieu nvarchar(100),
		NhomGioLamNguoiGioiThieu nvarchar(10)
		);


insert into #tbl(HopDongID,NhanVienID,MaNhanVien,HovaTen,ChucVu,PhongBan,QuanLy1,QuanLy2,
				  SoHopDong,LoaiHopDong,NgayBatDauLam,NgayBatDauHD,NgayKetThucHD,
				  SoNgaySeHetHanHD,NVGioiThieuID,NguoiGioiThieu,NhomNhanVien,NhomGioLamNguoiGioiThieu)
				
select 

	Contract."contractID" as  "HopDongID",
	c."empID" as "NhanVienID",
	
	c."Code" as  "MaNhanVien",
	
	c."lastName" || case when ifnull( c."middleName",'') = '' then '' else ' '||  c."middleName"  ||' '||  c."firstName"  end 
	as "HovaTen",
	
	c."jobTitle" as "ChucVu",
	d."Name" as  "PhongBan",
	
	manager1."lastName" || case when ifnull(manager1."middleName",'') = '' then '' else ' '|| manager1."middleName"  ||' '|| manager1."firstName"  end 
	as "QuanLy1",
	
	manager2."lastName" || case when ifnull(manager2."middleName",'') = '' then '' else ' '|| manager2."middleName"  ||' '|| manager2."firstName"  end 
	as "QuanLy2",
	
	 Contract."code" as "SoHopDong",
	 f."name" as "LoaiHopDong",

	--ifnull(TO_VARCHAR(Contract."startDate",'DD/MM/YYYY'),'') 
	--ifnull(TO_VARCHAR(Contract."startDate",'DD/MM/YYYY'),'')  as "NgayBatDauHD",
	--ifnull(TO_VARCHAR(Contract."endDate",'DD/MM/YYYY'),'')  as "NgayKetThucHD",
	Contract."startDate" as "NgayBatDauLam",
    Contract."startDate" as  "NgayBatDauHD",
    Contract."endDate" as "NgayKetThucHD",
	DAYS_BETWEEN (CURRENT_DATE,Contract."endDate") as  "NgayHetHanHD",
	-- Người giới thiệu
	nguoigioithieu."empID" as "NVGioiThieuID",
	nguoigioithieu."lastName" || case when ifnull( nguoigioithieu."middleName",'') = '' then '' else ' '||  nguoigioithieu."middleName"  ||' '||  nguoigioithieu."firstName"  end 
	as "NguoiGioiThieu",
	
	case 
		  when ifnull(c."U_Type",'') =''     then 'Chưa nhập thông tin' 
		  
		  when ifnull(c."U_Type",'') ='0'     then 'Office' 
		  when ifnull(c."U_Type",'') ='1'     then 'Shopfloor' 
		     end as "NhomNhanVien",
		     
     nguoigioithieu."U_WorkingHours" as "NhomGioLamNguoiGioiThieu"
	
	

	
from  OHEM c
inner join  (select    "empID" , "startDate","contractID", "signDate","code","endDate" ,  "type"
				from    APZ_OHEC a
				
				where "startDate"  in  (select    max("startDate") 
										from   APZ_OHEC b
										 left join APZ_OCTY loaihd on loaihd."typeID"  =  b."type"
										
										
										 where a."empID" =  b."empID" and loaihd."code" not like '%PL%'  ) order by  "empID"
				
				) as Contract on Contract."empID"  = c."empID"
				
left join OUDP d on d."Code"  = c."dept"
--Manager 1: Line 1
left join  OHEM manager1 on c."manager"  = manager1."empID"
left join  OHEM manager2 on c."U_LineManager2Code"  = manager2."empID"
-- Người giới thiệu
left join  OHEM nguoigioithieu on c."U_PersonWhoIntroduced"  = nguoigioithieu."empID"
 
 
left join APZ_OCTY f on Contract."type"  =  f."typeID"

where  
-- Thơi gian
Contract."startDate"  between :TuNgay and :DenNgay

and
(
					ifnull(:PhongBan, '') = '' 
					or cast (d."Code" as nvarchar) in (select "DATA" from split(:PhongBan,','))
)

and
(
					ifnull(:NhanVien, '') = '' 
					or cast(c."empID" as nvarchar) in (select "DATA" from split(:NhanVien,','))
)
and
(
					ifnull(:LoaiHopDong, '') = '' 
					or cast(f."typeID" as nvarchar)  in (select "DATA" from split(:LoaiHopDong,','))
)

and
(
					ifnull(:SoNgaySeHetHanHD, '') = ''
					--or cast(DAYS_BETWEEN (Contract."startDate",Contract."endDate") as nvarchar)  <= (select "DATA" from split(:SoNgaySeHetHanHD,','))
					or cast(DAYS_BETWEEN (CURRENT_DATE,Contract."endDate") as int)  <= (select case when ifnull("DATA",'') ='' then 100000 else "DATA"  end from split(:SoNgaySeHetHanHD,','))
)
 
order by c."empID",Contract."empID";

 
-------------
		
		
select * from #tbl;
drop table #tbl;
		
end;


