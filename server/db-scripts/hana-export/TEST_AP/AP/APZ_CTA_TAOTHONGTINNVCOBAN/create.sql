CREATE procedure APZ_CTA_TaoThongTinNVCoBan(in clgCode nvarchar(10), in empId nvarchar(10))
as
BEGIN

	declare soHDong nvarchar(100);
	declare ngayKyHDong nvarchar(100);
	
	declare maNhanVien nvarchar(50);
	declare hoVaTen nvarchar(50);
	
	declare chucDanh nvarchar(50);
	declare phongBan nvarchar(50);
	declare thoiGianThuViec nvarchar(100);
	declare quanLy1 nvarchar(100);
	declare quanLy2 nvarchar(100);
	
	declare ngayBatDauHdong nvarchar(100); 
	declare ngayHetHanHdong nvarchar(100); 
	
	declare ngayVaoLam nvarchar(100);
	declare quocTich nvarchar(100);
	
	declare ngaySinh nvarchar(100);
	declare noiSinh nvarchar(254);
	declare diaChiThuongTru nvarchar(100);
	declare diaChiTamTru nvarchar(100); -- workStreet
	
	-- CMND/ CCCD
	declare soCMND_CCCD nvarchar(50);
	declare ngayCapCMND_CCCD nvarchar(70);
	declare noiCapCMND_CCCD nvarchar(100);
	declare ngayXuat nvarchar(70);
	

Select  
	   
		ifnull((select  top 1 "code" from APZ_OHEC a 
	   				where  cast(a."empID" as nvarchar) = :empId  order by "startDate" desc),'') as "soHDong",
	    ifnull((select  top 1 "signDate" from APZ_OHEC a 
	   				where  cast(a."empID" as nvarchar) = :empId  order by "startDate" desc),'') as "ngayKyHDong",
	   				
	   ifnull((select  top 1 "startDate" from APZ_OHEC a 
	   				where  cast(a."empID" as nvarchar) = :empId  order by "startDate" desc),'') as "ngayBatDauHdong",
	   				
	   	ifnull((select  top 1 "endDate" from APZ_OHEC a 
	   				where  cast(a."empID" as nvarchar) = :empId  order by "startDate" desc),'') as "ngayHetHanHdong",
	  
	    ep."Code" as "maNhanVien",
	    ep."lastName" || case when ifnull(ep."middleName",'') = '' then '' else ' '|| ep."middleName"  ||' '|| ep."firstName"  end as "hoVaTen",
	   
	   
	   	ep."jobTitle" as  "chucDanh",
	   	dp."Name" as "phongBan",
		manager1."lastName" || case when ifnull(manager1."middleName",'') = '' then '' else ' '|| manager1."middleName"  ||' '|| manager1."firstName"  end 
		as "QuanLy1",
	
	   	manager2."lastName" || case when ifnull(manager2."middleName",'') = '' then '' else ' '|| manager2."middleName"  ||' '|| manager2."firstName"  end 
		as "QuanLy2",
		'' as "ThoiGianThuViec",
		ep."startDate" as  "NgayVaoLam",
		ep."brthCountr" as  "quocTich",
		ep."birthDate" as  "ngaySinh",
		ep."U_BirthAddr" as "noiSinh",
		ep."homeStreet" as  "diaChiThuongTru",
		ep."workStreet" as  "diaChiTamTru",
		ep."govID" as "soCMND_CCCD",
		ep."U_DateOfIDIssue" as "ngayCapCMND_CCCD",
		ep."U_PlaceOfIDIssue" as "noiCapCMND_CCCD",
		Current_date as "ngayXuat"
		
		 			 
Into soHDong, ngayKyHDong,ngayBatDauHdong,ngayHetHanHdong, maNhanVien,
			 	  hoVaTen, chucDanh, phongBan,  quanLy1,quanLy2, thoiGianThuViec, ngayVaoLam ,quocTich, ngaySinh, noiSinh, diaChiThuongTru, diaChiTamTru,
			 	  soCMND_CCCD,ngayCapCMND_CCCD,noiCapCMND_CCCD, ngayXuat
			 	  
From  OHEM ep
--Manager 1: Line 1
left join  OHEM manager1 on ep."manager"  = manager1."empID"
--Manager 1: Line 2
left join  OHEM manager2 on ep."U_LineManager2Code"  = manager2."empID"
left join OUDP dp on dp."Code"  = ep."dept"
 where cast(ep."empID" as nvarchar) = :empId;
	
	
	select "Details", "CardCode", "Action", "Location", "status", "personal", "parentType", "parentId", "SeriesNum", "parentTask", "PrjCode", "OcrCode", "OcrCode2", "OcrCode3", "OcrCode4", "OcrCode5", "RecurrenceInfo"
			,"ResourceIdsInfo", "ProjectEntry", "ProjectWBS", "BaseType", "BaseEntry", "BaseLine", "BaseCode" 
			 
			
			,  replace(replace(replace(replace(replace(replace(replace( replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(replace("HtmlNotes"
					, '[%0]', ifnull(:soHDong, ''))
					, '[%1]', ifnull(TO_VARCHAR(:ngayKyHDong,'DD/MM/YYYY'), ''))
					, '[%2]', ifnull(:maNhanVien, ''))
					, '[%3]', ifnull(:hoVaTen, ''))
					, '[%4]', ifnull(:chucDanh, ''))
					, '[%5]', ifnull(:phongBan, ''))
					
					, '[%6]', ifnull(:quanLy1, ''))
					, '[%7]', ifnull(:quanLy2, ''))
					, '[%8]', ifnull(:thoiGianThuViec, ''))
					, '[%9]', ifnull(TO_VARCHAR(:ngayHetHanHdong,'DD/MM/YYYY'), ''))
					, '[%10]', ifnull(TO_VARCHAR(:ngayVaoLam,'DD/MM/YYYY'), ''))
					, '[%11]', ifnull(TO_VARCHAR(:ngayBatDauHdong,'DD/MM/YYYY'), ''))
					, '[%12]', ifnull(:quocTich, ''))
					, '[%13]', ifnull(TO_VARCHAR(:ngaySinh,'DD/MM/YYYY'), ''))
					, '[%14]', ifnull(:noiSinh, ''))
					, '[%15]', ifnull(:diaChiThuongTru, ''))
					, '[%16]', ifnull(:diaChiTamTru, ''))
					, '[%17]', ifnull(:soCMND_CCCD, ''))
					, '[%18]', ifnull(TO_VARCHAR(:ngayCapCMND_CCCD,'DD/MM/YYYY'), ''))
					, '[%19]', ifnull(:noiCapCMND_CCCD, ''))
					, '[%20]', ifnull(TO_VARCHAR(:ngayXuat,'DD/MM/YYYY'), ''))
					, '&',  '&amp;')
					"HtmlNotes" 
	from APZ_OCLG where cast("ClgCode" as nvarchar) = :clgCode;

END

