CREATE procedure APZ_CTA_TaoHopDong(in clgCode nvarchar(10), in empId nvarchar(10), in contractId nvarchar(10))
as
BEGIN
	declare loaiHD nvarchar(50);
	declare loaiHDTA nvarchar(100);
	declare loaiHDPL nvarchar(100);

	declare fromToDate nvarchar(50);
	declare fromToDateTA nvarchar(50);
	
	declare contactNumber nvarchar(50);
	declare dateString nvarchar(100);
	declare fullName nvarchar(100);
	declare nationality nvarchar(100);
	declare birthDate nvarchar(100);
	declare birthAddress nvarchar(254);
	declare homeAddress nvarchar(254);
	
	declare startDate nvarchar(70); 
	-- CMND/ CCCD
	declare idCard nvarchar(50);
	declare dateOfIDIssue nvarchar(70);
	declare placeOfIDIssue nvarchar(100);
	
	declare passIssue nvarchar(50);
	declare passDate nvarchar(50);
	declare signDate nvarchar(100);
	
	declare parentContract nvarchar(50);
	declare parentContractDate nvarchar(100);
	declare parentId int;
	
	declare sex nvarchar(20);
	declare soCMND_CCCD nvarchar(20);
	declare diaChiTamTru nvarchar(100); -- workStreet

	declare chucDanh nvarchar(100); -- Tên chức danh jobTitle
	declare chucDanhTA nvarchar(100); -- Tên chức danh tiếng anh: U_JobTitleEN
	declare tGianLamViec nvarchar(100);
	declare tGianLamViecTA nvarchar(100);
	
	declare luong  float;
	declare luongText nvarchar(100);
	declare luongTextTA nvarchar(100);
	
	declare pTienDiLam nvarchar(150);
	declare pTienDiLamTA nvarchar(150);
	
	declare pcXang float;
	declare pcXangText nvarchar(100);
	declare pcXangTextTA nvarchar(100);
	
	declare pcDThoai float;
	declare pcDThoaiText nvarchar(100);
	declare pcDThoaiTextTA nvarchar(100);
	
	declare pcAn float;
	declare pcAnText nvarchar(50);
	declare pcAnTextTA nvarchar(50);

	
	
	if exists(select 1 from APZ_OHEC where cast("empID" as nvarchar) = :empId and cast("contractID" as nvarchar) = :contractId)
	THEN
		 	--- Insert giá tri tiền phụ cấp
			create local temporary table #tblPhuCap
			(
				Amount		float,
				Name		nvarchar(150)
			);
			insert into #tblPhuCap(Amount, Name)
			select   ifnull(a."Amount",0)  as "Amount" , b."Name" 
			from APZ_HSI1 a
				
			inner join  APZ_OHAT  b on a."AllowanceType"  =  b."Code"
			where CAST(a."ValidFrom" as Date) >=  (select  cast("startDate" as Date)
 											from APZ_OHEC a 
 												where cast(a."empID" as nvarchar) = :empId and cast(a."contractID" as nvarchar) = :contractId) -- FromDate của hợp đồng đang chọn
	
	
			and cast(a."EmpId" as nvarchar) = :empId;
	
	
	if (select count(*) from #tblPhuCap) > 0 then
	 select  Amount,Num2Text(Amount), Num2TextEN(Amount)  into pcXang,pcXangText,pcXangTextTA from #tblPhuCap where Name like '%Petro%';
			
	 select   Amount,Num2Text(Amount), Num2TextEN(Amount)  into pcDThoai,pcDThoaiText,pcDThoaiTextTA from #tblPhuCap where Name like '%Phone%';
	
	 select  Amount,Num2Text(Amount), Num2TextEN(Amount) into pcAn,pcAnText,pcAnTextTA from #tblPhuCap where Name = 'Meal';
	end if;
	drop table #tblPhuCap;
	 
	
	------------------------end
		select a."code",b."startDate",
		
		 'ngày ' || substring('0' || extract(DAY from a."signDate"), 1) || ' tháng ' || substring('0' || extract(month from a."signDate"), 1) || ' năm ' || extract(year from a."signDate"),
			
			CASE  
			when  ifnull(a."startDate",'') != '' and  ifnull(a."endDate",'') = ''   then N'Từ ngày ' || TO_VARCHAR(a."startDate",'DD/MM/YYYY') 
  	  			  when  ifnull(a."startDate",'') != '' and  ifnull(a."endDate",'') != ''   
  	  																					then N'Từ ngày ' || TO_VARCHAR(a."startDate",'DD/MM/YYYY') || ' đến ngày ' || TO_VARCHAR(a."endDate",'DD/MM/YYYY') 
  				 else '..............' END,
  				 
			-- From to date tieng anh
			CASE  
			when  ifnull(a."startDate",'') != '' and  ifnull(a."endDate",'') = ''   then N'from ' || TO_VARCHAR(a."startDate",'DD/MM/YYYY') 
  	  			  when  ifnull(a."startDate",'') != '' and  ifnull(a."endDate",'') != ''   
  	  																					then N'from ' || TO_VARCHAR(a."startDate",'DD/MM/YYYY') || ' to date ' || TO_VARCHAR(a."endDate",'DD/MM/YYYY') 
  				 else '..............' END
			---
			, case 
				when ifnull(b."firstName", '') != '' and ifnull(b."middleName", '') != '' and ifnull(b."lastName", '') != '' then b."lastName" || ' ' || b."middleName" || ' ' || b."firstName"
				when ifnull(b."firstName", '') != '' and ifnull(b."middleName", '') != '' and ifnull(b."lastName", '') = '' then b."middleName" || ' ' || b."firstName"
				when ifnull(b."firstName", '') != '' and ifnull(b."middleName", '') = '' and ifnull(b."lastName", '') != '' then b."lastName" || ' ' || b."firstName"
				when ifnull(b."firstName", '') = '' and ifnull(b."middleName", '') != '' and ifnull(b."lastName", '') != '' then b."lastName" || ' ' || b."middleName"
				when ifnull(b."firstName", '') = '' and ifnull(b."middleName", '') = '' and ifnull(b."lastName", '') != '' then b."lastName"
				when ifnull(b."firstName", '') = '' and ifnull(b."middleName", '') != '' and ifnull(b."lastName", '') = '' then b."middleName"
				when ifnull(b."firstName", '') != '' and ifnull(b."middleName", '') = '' and ifnull(b."lastName", '') = '' then b."firstName"
				else '' end, b."brthCountr", TO_VARCHAR(b."birthDate", 'dd/MM/yyyy'), 
				b."U_BirthAddr" as "NoiSinh",
				 b."homeStreet" ||' '||b."HomeBuild" ||' ' || b."homeCity"  || ' '||b."homeCounty",
				 
				 b."passportNo", b."PassIssue", b."PassIssuer", a."signDate", 
				a."ParentContract",
				
			case 
				when ifnull(b."sex", '') != '' and b."sex" = 'F' Then 'Bà' Else 'Ông' End,
				--- CMND/ CCCD
				b."govID",
				b."U_PlaceOfIDIssue",
				b."U_DateOfIDIssue",			
				
				b."U_Address_Tempt" as "DiaChiTamTru",
				
				b."jobTitle",
				b."U_JobTitleEN",
								
				
				---
		case when b."U_WorkingHours" =  40 then N'40 giờ/tuần (08 giờ/ngày), từ Thứ Hai đến Thứ Sáu'
			 when b."U_WorkingHours" =  44 then N'44 giờ/tuần (08 giờ/ngày), từ Thứ Hai đến hết Sáng Thứ Bảy'
			 when b."U_WorkingHours" =  48 then N'48 giờ/tuần (08 giờ/ngày), từ Thứ Hai đến Thứ Bảy'
				  else '' end as "ThoiGianLamViec",
				  
				  
		case when b."U_WorkingHours" =  40 then N'40 hours/week (08 hours/day), from Monday to Friday'
			 when b."U_WorkingHours" =  44 then N'44 hours/week (08 hours/day), from Monday to Saturday Morning'
			 when b."U_WorkingHours" =  48 then N'48 hours/week (08 hours/day), from Monday to Saturday'
				  else '' end as "ThoiGianLamViecTA",	


				
				
				a."salary",-- Lương : number

				Num2Text (cast(a."salary" as float )),
				Num2TextEN (cast(a."salary" as float )),
				-- Phuong tien di lam
				case when ifnull(b."U_TransTowork",'') =  '' then '' 
					 when b."U_TransTowork" = '0' then N'Công ty sẽ cung cấp phương tiện đi lại cho nhân viên khi làm việc tại nhà máy KCN Protrade'
					 when b."U_TransTowork" = '1' then N'Nhân viên tự túc phương tiện đi lại khi làm việc tại KCN Protrade'	 end,
				-- Phuong tien di lam tieng Anh
				case when ifnull(b."U_TransTowork",'') =  '' then '' 
					 when b."U_TransTowork" = '0' then N'The Company will provide means of transportation for employee when working Ho Chi Minh office or other locations for business purposes'
					 when b."U_TransTowork" = '1' then N'Employee self travel when working at working at Factory in Protrade Industrial Zone'	 end,
					 
				c."name" ,
				c."descriptio" 
					 
					 
			 into contactNumber, startDate,dateString, fromToDate,fromToDateTA,
			 	  fullName, nationality, birthDate, birthAddress, homeAddress, idCard, passIssue, passDate, signDate, parentId, sex,
			 	  
			 	  soCMND_CCCD,
			 	  placeOfIDIssue,
				  dateOfIDIssue,
				  
			 	  diaChiTamTru,
			 	  
			 	  chucDanh,
			 	  chucDanhTA,
			 	  
			 	  tGianLamViec,
			 	  tGianLamViecTA,
			 	  luong,
			 	  luongText,
			 	  luongTextTA,
			 	  pTienDiLam,
			 	  pTienDiLamTA,
			 	  loaiHD,
			 	  loaiHDTA
			 	  
			 	   -- Loai hop dong
			 	  
		from APZ_OHEC a 
		inner join OHEM b on a."empID" = b."empID" 
			 left join APZ_OCTY c on c."typeID"  =  a."type" 
		
		where cast(a."empID" as nvarchar) = :empId 
				and cast(a."contractID" as nvarchar) = :contractId;
		
		
	END IF;
	
	loaiHDPL  =  ifnull((select a."code" from APZ_OHEC a inner join APZ_OCTY  b on a."type" = b."typeID" where b."code" like '%PL%' and "empID" = :empId and  "contractID" = :contractId ),'');
	
	-- Đối với Phu Luc Hợp Đồng thì lấy hợp đồng đầu tiên (vì bên ILD chỉ làm phụ lục cho hop đồng đầu tiên)
	parentContract = ifnull((select top 1 case when ifnull(:loaiHDPL,'') !='' then   a."code" else '' end from APZ_OHEC a where cast(a."empID" as nvarchar) = :empId  order by "startDate" ), '');
	

	parentContractDate = ifnull((select top 1 case when ifnull(:loaiHDPL,'') !='' then   a."signDate" else '' end from APZ_OHEC a where cast(a."empID" as nvarchar)  = :empId  order by "startDate" ), '');
	select "Details", "CardCode", "Action", "Location", "status", "personal", "parentType", "parentId", "SeriesNum", "parentTask", "PrjCode", "OcrCode", "OcrCode2", "OcrCode3", "OcrCode4", "OcrCode5", "RecurrenceInfo"
			,"ResourceIdsInfo", "ProjectEntry", "ProjectWBS", "BaseType", "BaseEntry", "BaseLine", "BaseCode" 
			 
			
			, replace(replace(replace(replace(replace( replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(replace( replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(replace("HtmlNotes", 
					  '[%0]', ifnull(:contactNumber, ''))
					, '[%1]', ifnull(:dateString, ''))
					, '[%2]', ifnull(:fullName, ''))
					, '[%3]', ifnull(:nationality, ''))
					, '[%4]', ifnull(:birthDate, ''))
					, '[%5]', ifnull(:birthAddress, ''))
					, '[%6]', ifnull(:homeAddress, ''))
					, '[%7]', ifnull(:idCard, ''))
					, '[%8]', ifnull(:passIssue, ''))
					, '[%9]', ifnull(:passDate, ''))
					--, '[%10]', ifnull(:signDate, ''))
					, '[%10]', ifnull(TO_VARCHAR(:signDate,'DD/MM/YYYY'), ''))
					, '[%11]', ifnull(:parentContract, ''))
					, '[%12]', ifnull(:sex, '')) 
					, '[%13]', ifnull(:soCMND_CCCD, ''))
					, '[%14]', ifnull(:diaChiTamTru, '')) 
					
					, '[%15]', ifnull(:chucDanhTA, '')) 
					, '[%16]', ifnull(:tGianLamViec, '')) 
					, '[%17]', ifnull(:tGianLamViecTA, '')) 
					
					, '[%18]', ifnull(to_varchar(:luong,'9,999'), '')) 

					, '[%19]', case when LENGTH(:luongText) <=0 THEN 'Không' else :luongText end )
					, '[%20]', case when LENGTH(:luongTextTA) <=0 THEN 'Zero' else :luongTextTA end )
					
					, '[%21]', ifnull(:pTienDiLam, '')) 
					, '[%22]', ifnull(:pTienDiLamTA, '')) 
					
					, '[%23]', ifnull(:pcXang, 0)) 
					, '[%24]', ifnull(:pcXangText, '')) 
					, '[%25]', ifnull(:pcXangTextTA, '')) 
					
					, '[%26]', ifnull(:pcDThoai, 0)) 
					, '[%27]', ifnull(:pcDThoaiText, '')) 
					, '[%28]', ifnull(:pcDThoaiTextTA, '')) 
					
					, '[%29]', ifnull(:pcAn, 0)) 
					, '[%30]', ifnull(:pcAnText, '')) 
					, '[%31]', ifnull(:pcAnTextTA, '')) 
					
					, '[%32]', ifnull(:fromToDate, ''))
					
					, '[%33]', ifnull(TO_VARCHAR(:dateOfIDIssue,'DD/MM/YYYY'), ''))
					--, '[%33]', ifnull(:dateOfIDIssue, ''))
					
					, '[%34]', ifnull(:placeOfIDIssue, ''))
					
					
					, '[%35]', ifnull(TO_VARCHAR(:parentContractDate,'DD/MM/YYYY'), ''))
					--, '[%35]', ifnull(:parentContract, ''))
					
					, '[%36]', ifnull(:loaiHD, ''))
					, '[%37]', ifnull(:loaiHDTA, ''))
					
					, '[%38]', ifnull(:fromToDateTA, ''))
					
					, '[%39]', ifnull(:chucDanh, ''))
					
					, '[%40]', ifnull(TO_VARCHAR(:startDate,'DD/MM/YYYY'), ''))
					, '[%41]', upper(ifnull(:fullName, '')))
					
					, '&',  '&amp;')

					"HtmlNotes" 
	from APZ_OCLG where cast("ClgCode" as nvarchar) = :clgCode;

END

