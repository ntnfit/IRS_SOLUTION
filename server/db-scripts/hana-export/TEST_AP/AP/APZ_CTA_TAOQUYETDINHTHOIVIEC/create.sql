CREATE procedure APZ_CTA_TaoQuyetDinhThoiViec(in clgCode nvarchar(10), in empId nvarchar(10))
as
BEGIN
	declare soQuyetDinh nvarchar(50);
	declare ngayQuyetDinh nvarchar(50);
	
	declare soHDong nvarchar(100);
	declare ngayKyHDong nvarchar(100);
	
	declare maNhanVien nvarchar(50);
	declare hoVaTen nvarchar(50);

	
	declare chucDanh nvarchar(50);
	declare phongBan nvarchar(50);
	declare ngayNghiViec nvarchar(100);
	declare LyDoNghiViec nvarchar(100);
	declare NgayLamViecCuoi nvarchar(100); -- ngày làm việc cuối cùng
	
	
	
	

Select ep."U_ResignationCode" as "soQuyetDinh",
	   ep."U_ResignationDate" as "ngayQuyetDinh",
	   
	   ifnull((select  top 1 "code" from APZ_OHEC a 
	   				where  cast(a."empID" as nvarchar) = :empId  order by "startDate" desc),'') as "soHDong",
	   ifnull((select  top 1 "signDate" from APZ_OHEC a 
	   				where  cast(a."empID" as nvarchar) = :empId  order by "startDate" desc),'') as "ngayKyHDong",
	  
	   ep."Code" as "maNhanVien",
	   ep."lastName" || case when ifnull(ep."middleName",'') = '' then '' 
	   else ' '|| ep."middleName"  ||' '|| ep."firstName"  end as "hoVaTen",
	   
	   ep."jobTitle" as  "chucDanh",
	   dp."Name" as "phongBan",
	   ep."termDate" as "NgayNghiViec",
	   ep."termReason" as "LyDoNghiViec",
	   ep."U_LastWorkingDay" as  "NgayLamViecCuoi"
	  			 
Into soQuyetDinh, ngayQuyetDinh,soHDong, ngayKyHDong,maNhanVien,
			 	  hoVaTen, chucDanh, phongBan, NgayNghiViec, LyDoNghiViec, NgayLamViecCuoi
			 	  
From  OHEM ep
left join OUDP dp on dp."Code"  = ep."dept"
 where cast(ep."empID" as nvarchar) = :empId;
	
	
	select "Details", "CardCode", "Action", "Location", "status", "personal", "parentType", "parentId", "SeriesNum", "parentTask", "PrjCode", "OcrCode", "OcrCode2", "OcrCode3", "OcrCode4", "OcrCode5", "RecurrenceInfo"
			,"ResourceIdsInfo", "ProjectEntry", "ProjectWBS", "BaseType", "BaseEntry", "BaseLine", "BaseCode" 
			 
			
			, replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(replace("HtmlNotes", 
					  '[%0]', ifnull(:soQuyetDinh, ''))
					, '[%1]', ifnull(TO_VARCHAR(:ngayQuyetDinh,'DD/MM/YYYY'), ''))
					, '[%2]', ifnull(:soHDong, ''))
					, '[%3]', ifnull(TO_VARCHAR(:ngayKyHDong,'DD/MM/YYYY'), ''))
					, '[%4]', ifnull(:maNhanVien, ''))
					, '[%5]', ifnull(:hoVaTen, ''))
					, '[%6]', ifnull(:chucDanh, ''))
					, '[%7]', ifnull(:phongBan, ''))
					, '[%8]', ifnull(TO_VARCHAR(:NgayNghiViec,'DD/MM/YYYY'), ''))
					, '[%9]', ifnull(:LyDoNghiViec, ''))
					, '[%10]', ifnull(TO_VARCHAR(:NgayLamViecCuoi,'DD/MM/YYYY'), ''))
					, '&',  '&amp;')
					"HtmlNotes" 
	from APZ_OCLG where cast("ClgCode" as nvarchar) = :clgCode;

END


