CREATE procedure ILD_TienThuongNguoiGioiThieu
(
	in ngayBDHDCThuc date,
	in nhomNhanVienDaGioiThieu nvarchar(20), -- empIDGioiThieu giới thiệu nhân viên gì (shopfloor hay office)
	in soGioLamNguoiGioiThieu nvarchar(10),
	in empIDGioiThieu int,
	in empID int-- ID nhan vien

)
as
begin
	declare soTienThuong int =200000;
	declare periodCode nvarchar(20);
	declare docNumThuongMaster smallint;
	declare ngayBatDauHDCThuc timestamp;
	
 IF (NhomNhanVienDaGioiThieu  ='Shopfloor') then
 	 If exists (select "U_Value" from "@OTHERSETINGS" where "Code" = 'TTGTSF') then   
		select  "U_Value"
		into soTienThuong
		from  "@OTHERSETINGS"
		where "Code" = 'TTGTSF';
		
	End If;  
 End If;
 
  IF (NhomNhanVienDaGioiThieu  ='Office') then
 	If exists (select "U_Value" from "@OTHERSETINGS" where "Code" = 'TTGTOF') then   
		select  "U_Value"
		into soTienThuong
		from  "@OTHERSETINGS"
		where "Code" = 'TTGTOF';
		
	End If;   
 End If;

		 	 	
	  ------ Lấy kỳ làm việc: periodCode để insert vào kỳ thưởng
				
 		select Max("PeriodCode")
			into periodCode
			
			from APZ_OHWP
			where "PeriodCode" like '%'||(case when month(ngayBDHDCThuc) = 12 then year(CURRENT_DATE)+1 else year(CURRENT_DATE) end)||'%' and "PeriodCode" like '%'||:soGioLamNguoiGioiThieu||'%'
				and 
				cast(:ngayBDHDCThuc as date) between cast("FromDate" as date) and  cast("ToDate" as  date);

	 ---------------------	docNumThuongMaster --> Lấy Docnum 	
		if(select count(*) from APZ_HPT2 master  where "Period"  = :periodCode  ) >0 then
			select ifnull("DocNum",0)
			into docNumThuongMaster
			
			from  APZ_HPT2 master
			where "Period"  = :periodCode;
		else
			 docNumThuongMaster  = 0;
		
		end if;
	------ Insert

		if(docNumThuongMaster ) >0  then
		-- đã tồn tại recode ở master thưởng phạt --> chỉ insert vào table detail thôi

				insert into  APZ_HPT1("DocNum","EmpId","PenalizationType","LineNum","Date","TotalAmount","Note")

				values(:docNumThuongMaster,
					   :empIDGioiThieu,
					   5,
					   (select ifnull(max("LineNum"),0) + 1 from APZ_HPT1 where "DocNum"  = :docNumThuongMaster ),
					   ifnull(:ngayBDHDCThuc,CURRENT_DATE),
					   :soTienThuong, 'Đã giới thiệu nhân viên '|| empID);
		 		
	   else --> Chưa tồn tại khen thưởng master --> insert 2 bảng: master và detai của khen thưởng
			
		---Master 
			select ifnull(max("DocNum"),0)
			into docNumThuongMaster
			from APZ_HPT2;

			insert into APZ_HPT2 ("DocNum","Period","Remarks","Branch","IncludeBonus","IncludePenalty","UpdateDate")
			values(docNumThuongMaster+1,
					periodCode,
					'',
					-2,
					'Y',
					'N',
					CURRENT_DATE);
	   	 
		--- Detail 
	 		insert into  APZ_HPT1("DocNum","EmpId","PenalizationType","LineNum","Date","TotalAmount","Note")

			values(:docNumThuongMaster+1,
					   :empIDGioiThieu,
					   5,
					   1,-- Line Num (vì chưa có detail nào nên line Num này bắt đầu  = 1
					   ifnull(:ngayBDHDCThuc,CURRENT_DATE),
					   :soTienThuong, 'Đã giới thiệu nhân viên '|| empID);
		 
		 
		end if;	
	
	
	



end;


