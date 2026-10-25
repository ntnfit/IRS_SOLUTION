CREATE procedure APZ_CTA_TaoHopDongChinhThuc1Nam
(
	out ret table ("Code" int, "Msg" Nvarchar(200))
)
LANGUAGE SQLSCRIPT
as
begin
	declare Code int = 200;
	declare Msg nvarchar(200) ='Success';
	declare loaiHDCT1Nam smallint ;
  
	
	Create local temporary table #tempHopDong(HopDongID int,
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
												NgayKetThucHD date ,
												
												NVGioiThieuID int,
												NguoiGioiThieu nvarchar(100),
												NhomGioLamNguoiGioiThieu nvarchar(10),
												
												
												SoNgaySeHetHanHD nvarchar(20) );
	
	 exec 'drop table #tempHopDong';  
	 exec 'create local temporary table #tempHopDong  as  (select  * from "#tempHopDong_UI")';
	 -- Lấy loại hợp đồng 1 năm
	 select top 1 "typeID"
	 	into loaiHDCT1Nam
	 	from APZ_OCTY
	 	where "code" like '%1%';
	 	
	 
		IF  Exists(Select 1 From #tempHopDong) then 
		
		
		
		
		BEGIN /* UPDATE */
			Declare i int = 1;
			Declare loopCount int = 0;
			Declare nhanvienID int;
			Declare ngayKetThucHD date;
			
			Declare ngayBatDauHDCThuc date;
			Declare nhomNhanVien nvarchar(20);
			Declare nhomGioLamNguoiGioiThieu nvarchar(10);
			Declare nvGioiThieuID int;
       		create local temporary table #tbl as (Select *, ROW_NUMBER() OVER () AS row_num From #tempHopDong);
       		Select Count(*) Into loopCount From #tbl;
       		While :i <= :loopCount
		  	DO
		  	    Select NhanVienID Into nhanvienID From #tbl Where row_num = :i;

		  	
		  		 IF (   select count(*) from  APZ_OHEC 
	 									 			where "empID" = nhanvienID and   "type" = loaiHDCT1Nam    )<=0 Then
	 

				 	
				Select NgayKetThucHD Into ngayKetThucHD From #tbl Where row_num = :i;
		  	    Select ADD_DAYS (ngayKetThucHD, 1)  Into ngayBatDauHDCThuc From dummy;
	 	
	 			 insert into APZ_OHEC ("empID","contractID","code","type","signDate","startDate","endDate","PymMeth")
				 select nhanvienID, 
				  (select max("contractID") + 1 from APZ_OHEC a where  a."empID"  = b.NhanVienID  ) as "contractID",
				  (SELECT ILD_FT_SinhMaHopDong(:i)   FROM dummy),				 
				 
				 loaiHDCT1Nam , 
			      ngayBatDauHDCThuc as "signDate",
			     ngayBatDauHDCThuc as "startDate",
				  
			      ADD_MONTHS(ADD_DAYS (  ngayBatDauHDCThuc, 1),12)as "endDate",
				  5 AS  "PymMeth"
				 		
				 from #tbl b
				 where row_num = :i;
				 
				 
		-- Nếu nhân viên thuộc Office và Shopfloor + Có người giới thiệu + Số giờ làm người giới thiêu <> '' -- Thực hiện ghi nhận tiền thưởng giới thiệu>
		 If exists (select NhomNhanVien from  #tbl where row_num = :i and NhomNhanVien in  ('Office','Shopfloor') 
		 and ifnull(NVGioiThieuID,0) <>0 and  ifnull(NhomGioLamNguoiGioiThieu,0) <>0  AND LoaiHopDong  = 'Hợp đồng thử việc') then
		    
		    
		    Select ADD_DAYS(NgayKetThucHD, 1) Into ngayBatDauHDCThuc From #tbl Where row_num = :i;
		    
		    Select  NhomNhanVien Into nhomNhanVien From #tbl Where row_num = :i;
		    
		    Select  NhomGioLamNguoiGioiThieu Into nhomGioLamNguoiGioiThieu From #tbl Where row_num = :i;
		    
		    Select  NVGioiThieuID Into nvGioiThieuID From #tbl Where row_num = :i;
	 
	       if not exists(select "Note"  from  APZ_HPT1 where "Note" like '%' || nhanvienID || '%') then 
	        
	       
			 	 call ILD_TienThuongNguoiGioiThieu ( ngayBDHDCThuc=> ngayBatDauHDCThuc ,
			 	 									nhomNhanVienDaGioiThieu=>nhomNhanVien,
			 	 									
			 	 									soGioLamNguoiGioiThieu=>nhomGioLamNguoiGioiThieu,
			 	 									empIDGioiThieu=>nvGioiThieuID,
			 	 									  empID=>nhanvienID);
			    
	 		    end if;
		 
		 	end if;
		 End If;
	 	
		  	
		  	
		  	i := i + 1;
			END WHILE;
			
			
	   END;
		
 
		    
	 END IF;
	 
---
	drop table #tempHopDong;
	 	ret = select Code as "Code",  Msg as "Msg" from dummy ;
	Return;

end;


