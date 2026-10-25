CREATE PROCEDURE SBO_SP_TransactionNotification_Before240827 -- GOLIVE
(
	in object_type nvarchar(30), 				-- SBO Object Type
	in transaction_type nchar(1),			-- [A]dd, [U]pdate, [D]elete, [C]ancel, C[L]ose
	in num_of_cols_in_key int,
	in list_of_key_cols_tab_del nvarchar(255),
	in list_of_cols_val_tab_del nvarchar(255)
)
LANGUAGE SQLSCRIPT
AS
-- Return values
error  int;				-- Result (0 for no error)
error_message nvarchar (200); 		-- Error string to be displayed
begin

error := 0;
error_message := N'Ok';


-----------------------------------================= CHẶN ================--------------------------

--Chặn Purchase Request Draft --> Phiếu draft :ODRF:112
/* --- UNLOCK

if(cast(object_type  as nvarchar) = '112'   and  transaction_type in ('A','U')) 
  and (select count(*) from ODRF where cast("DocEntry" as nvarchar) =  cast( :list_of_cols_val_tab_del as nvarchar) and cast("ObjType" as nvarchar)  = '1470000113'  )>0 then -- phiếu draft PR


	
--select "OcrCode4" as "PhongBan", "AbsId" as "MasterId",* from  OBGS;-- thiết lập master budget theo phòng ban

--select "Instance" as "MasterId", * from OBGT; -- Bảng detail, có tài khoản
 
	declare phongBan nvarchar(20);
	declare phongBanYeuCau nvarchar(20);
	declare phongBanNganSach_Instance nvarchar(20);


	
	if exists (select  "AcctCode" from DRF1 where ifnull("AcctCode",'') = ''
		 AND "DocEntry"  =  list_of_cols_val_tab_del and "ObjType"  = '1470000113'  ) then
	
			select 1470000113,N'G/L Account không được trống.'  into  error,  error_message  from dummy;
	end if;
	


	
	select "U_CostCenter" -- CostCenter: HR, F&C,...
	into phongBan
	from  OUSR a 
	where "USER_CODE" = (select   ifnull("Requester",'')  
															from ODRF 
															where "DocEntry"  =  list_of_cols_val_tab_del and "ObjType"  = '1470000113' );
	
	
   
   	select "AbsId"  --- chính là ID table master của Ngân Sách theo Cost Center (Phòng ban)
	into phongBanNganSach_Instance
	from OBGS 
	where "OcrCode4"  =   :phongBan and year("FinancYear")  = year(CURRENT_DATE);


 
	--- Thong tin ngân sách

create local temporary table #NganSach -- Chi tiết từng tài khoản
	(
		AcctCode nvarchar(15),
		PhongBan nvarchar(20),
		NganSach decimal(19,6),
		DangChoDuyet decimal(19,6),  -- Lây những phiếu có trạng thái đang chờ duyệt luôn
		DaSuDung decimal(19,6),
		SoTienYeuCau decimal(19,6),
		DaSuDung_YeuCau decimal(19,6), -- so tien da su dung + so tien yêu cầu PR hiện tại
		SoDuKhaDung decimal(19,6) -- So Dư khả dụng
		
	); 

	insert into #NganSach (AcctCode,PhongBan,NganSach) -- Table Detail Ngân sách chi tiết theo tài khoản
	
	select "AcctCode" as "TaiKhoan",
	"Instance" as  "PhongBan", --> ID master Ngan Sách: OBGS
    
	"DebLTotal" as "NganSach"

	from OBGT
	where cast("Instance" as int)  = :phongBanNganSach_Instance
	and  year("FinancYear" )  = year(CURRENT_DATE);
	
	--------- Update Tiền Đã Dùng (Lấy từ phiếu PR)
update ns
	set DangChoDuyet = pr."LineTotal" 
	from #NganSach ns
	left join  (select   a."AcctCode", sum(ifnull(a."LineTotal",0)) as  "LineTotal"

				from DRF1 a
				inner join ODRF b on a."DocEntry" =  b."DocEntry" 
				inner join OUSR c on b."Requester" = c."USER_CODE"
				where   year(b."DocDate") = year(CURRENT_DATE) and b."ObjType"  = '1470000113'
						and ifnull("WddStatus",'') not in('C', 'N')
						and b."DocEntry" <>   :list_of_cols_val_tab_del
						and  c."U_CostCenter"  = :phongBan
				
				group by  a."AcctCode", c."U_CostCenter") pr on ns.AcctCode = pr."AcctCode";
				
	--------- Số tiền từ PR Request đang chờ Duyệt (PR request với trạng thái duyệt <> C, N: từ chối
update ns
	set DaSuDung = pr."LineTotal" 
	from #NganSach ns
	inner join  (select   a."AcctCode", sum(ifnull(a."LineTotal",0)) as  "LineTotal"
						  	--,case when ifnull(phongBanYeuCau,0) =0 then 0 else  phongBanYeuCau end as  "PhongBan"

				from PRQ1 a
				inner join OPRQ b on a."DocEntry" =  b."DocEntry" 
				inner join OUSR c on b."Requester" = c."USER_CODE"
				where   year(b."DocDate") = year(CURRENT_DATE) and    c."U_CostCenter"  = :phongBan
				
				group by  a."AcctCode", c."U_CostCenter") pr on ns.AcctCode = pr."AcctCode";
	---
	update ns
	set DaSuDung =  ifnull(DaSuDung ,0)   , DangChoDuyet  = ifnull(DangChoDuyet ,0) 
	from #NganSach ns;
	--- Kiểm tra : ITem (Tài khoản ), chưa đc thiết lập ngân sách mà User lại input trên Purchase Request
 
	 if exists (select "AcctCode" from DRF1 a where   "DocEntry" = :list_of_cols_val_tab_del 
											       and "AcctCode" not in (select AcctCode  from #NganSach ns)) then
	 
	 		select 1470000113,N'Có tài khoản chưa được thiết lập ngân sách!'  into  error,  error_message  from dummy;
	 end if;
	
	--- Xóa đi những tài khoản không có trong Purchase request
	delete 
	from #NganSach ns
	where AcctCode not in (select "AcctCode" from DRF1 a where   "DocEntry" = :list_of_cols_val_tab_del);

	---------
	update ns
	set SoTienYeuCau = pr."LineTotal"
	from #NganSach ns
	inner join  (select   a."AcctCode", ifnull(a."LineTotal",0) as  "LineTotal"
						 	--,case when ifnull(phongBanYeuCau,0) =0 then 0 else  phongBanYeuCau end as  "PhongBan" 	

				from DRF1 a
				inner join ODRF b on a."DocEntry" =  b."DocEntry"
				
				where b."DocEntry" =   :list_of_cols_val_tab_del and a."ObjType"  = '1470000113' ) pr
				 on ns.AcctCode = pr."AcctCode";
---
	update ns 
	set SoDuKhaDung =  NganSach  -  (cast(DaSuDung as decimal)  + cast(DangChoDuyet as decimal)   +  cast(SoTienYeuCau as decimal)),
		 DaSuDung_YeuCau =   cast(DaSuDung as decimal)   +  cast(SoTienYeuCau as decimal)
	from #NganSach ns ;
	 
	---
	If ( select count (*) from  #NganSach ns where SoDuKhaDung <0 ) >0   -- Có 1 số khả dụng - âm (vượt ngân sách của line tài khoản)

	or (select sum(NganSach) - sum(DaSuDung_YeuCau) from #NganSach    ) <0 then 

	select 3,N'PR vượt Budget của Phòng Ban.'  into  error,  error_message  from dummy;
	End if;
	-- select list_of_cols_val_tab_del,N'Li 1.'  into  error,  error_message  from dummy;

drop table #NganSach;


end if;
 
----------------------------------============== End Chặn PR
if  (cast(object_type as nvarchar) =  '1470000113' and transaction_type in ('A','U')) then -- PR
 

--select "OcrCode4" as "PhongBan", "AbsId" as "MasterId",* from  OBGS;-- thiết lập master budget theo phòng ban

--select "Instance" as "MasterId", * from OBGT; -- Bảng detail, có tài khoản
 
 

declare phongBan nvarchar(20);
	declare phongBanYeuCau nvarchar(20);
	declare phongBanNganSach_Instance nvarchar(20);
 
	if exists (select  "AcctCode" from PRQ1 where ifnull("AcctCode",'') = '' and "DocEntry"  =  list_of_cols_val_tab_del  ) then
	
			select 1470000113,N'G/L Account không được trống.'  into  error,  error_message  from dummy;
	end if;
	

	select "U_CostCenter" 
	into phongBan
	from  OUSR a 
	where "USER_CODE" = (select   ifnull("Requester",'')  
															from OPRQ 
															where "DocEntry"  =  list_of_cols_val_tab_del and "ObjType"  = '1470000113' );
	

								
								
	select "AbsId" 
	into phongBanNganSach_Instance
	from OBGS 
	where "OcrCode4"  =   :phongBan and year("FinancYear")  = year(CURRENT_DATE);
	
	
	--- Thong tin ngân sách

create local temporary table #NganSach
	(
		AcctCode nvarchar(15),
		PhongBan smallint,
		NganSach decimal(19,6),
		DaSuDung decimal(19,6),
		SoTienYeuCau decimal(19,6),
		DaSuDung_YeuCau decimal(19,6), -- so tien da su dung + so tien yêu cầu PR hiện tại
		SoDuKhaDung decimal(19,6) -- So Dư khả dụng
		
	); 

	insert into #NganSach (AcctCode,PhongBan,NganSach)
	
	select "AcctCode" as "TaiKhoan",
	 "Instance" as  "PhongBan",
    
	"DebLTotal" as "NganSach"

	from OBGT
	where cast("Instance" as int)  = :phongBanNganSach_Instance
	 
	and  year("FinancYear" )  = year(CURRENT_DATE);
	
	--------- Update Tiền Đã Dùng (PR request với trạng thái duyệt <> C, N: từ chối,,'W': chờ duyệt
update ns
	set DaSuDung = pr."LineTotal" 
	from #NganSach ns
	inner join  (select   a."AcctCode", sum(ifnull(a."LineTotal",0)) as  "LineTotal"
						  	--,case when ifnull(phongBanYeuCau,0) =0 then 0 else  phongBanYeuCau end as  "PhongBan"

				from PRQ1 a
				inner join OPRQ b on a."DocEntry" =  b."DocEntry" 
				inner join OUSR c on b."Requester" = c."USER_CODE"
				where b."DocEntry" <>   :list_of_cols_val_tab_del and year(b."DocDate") = year(CURRENT_DATE)
						and c."U_CostCenter"  = :phongBan
						
						and ifnull("WddStatus",'') not in('C', 'N')
				
				group by  a."AcctCode", c."U_CostCenter") pr on ns.AcctCode = pr."AcctCode";
				-----
	-----------------			
	update ns
	set DaSuDung =  ifnull(DaSuDung ,0)   
	from #NganSach ns;
	------
	
	 if exists (select "AcctCode" from DRF1 a where   "DocEntry" = :list_of_cols_val_tab_del 
											       and "AcctCode" not in (select AcctCode  from #NganSach ns)) then
	 
	 		select 1470000113,N'Có tài khoản chưa được thiết lập ngân sách!'  into  error,  error_message  from dummy;
	 end if;
	 


	--- Xóa đi những tài khoản không có trong Purchase request
	delete 
	from #NganSach ns
	where AcctCode not in (select "AcctCode" from PRQ1 a where   "DocEntry" = :list_of_cols_val_tab_del);

	---------
	update ns
	set SoTienYeuCau = pr."LineTotal"
	from #NganSach ns
	inner join  (select   a."AcctCode", ifnull(a."LineTotal",0) as  "LineTotal"
						  	--,case when ifnull(phongBanYeuCau,0) =0 then 0 else  phongBanYeuCau end as  "PhongBan"

				from PRQ1 a
				inner join OPRQ b on a."DocEntry" =  b."DocEntry"
				where b."DocEntry" =   :list_of_cols_val_tab_del) pr on ns.AcctCode = pr."AcctCode";
	  
	 
	update ns 
	set SoDuKhaDung =  NganSach  -  (cast(DaSuDung as decimal)   +  cast(SoTienYeuCau as decimal)),
		 DaSuDung_YeuCau =   cast(DaSuDung as decimal)   +  cast(SoTienYeuCau as decimal)
	from #NganSach ns ;
	 
	---
	If ( select count (*) from  #NganSach ns where SoDuKhaDung <0 ) >0   -- Có 1 số khả dụng - âm (vượt ngân sách của line tài khoản)

	or (select sum(NganSach) - sum(DaSuDung_YeuCau) from #NganSach    ) <0 then 

	select 2,N'PR vượt Budget của Phòng Ban.'  into  error,  error_message  from dummy;
	End if;
	 

drop table #NganSach;


end if;

*/----- End Unlock
----------------------------------============== End Chặn PR (DRAFT)


------------------============= Chặn PO: Nếu giá trị PO cao hơn PR đã được duyệt -> Chặn
/* -- Tam Khoa Lenh: 1/3/2023
if (((cast(object_type as nvarchar) = '112' 
and (select count(*) from OPOR where cast("DocEntry"  as nvarchar) =  cast( :list_of_cols_val_tab_del as nvarchar) and "ObjType"  = '22'  )>0 and transaction_type in ('A','U'))
    
	OR ((object_type =  '22' and transaction_type in ('A','U'))))) then -- PO

	declare TotalPR decimal (19,6);
	declare TotalPO decimal (19,6);
	--- TotalPR
	select ifnull(sum("GTotalSC"),0)
	into TotalPR
	from  PRQ1
	where "DocEntry" =(
						select "DocEntry"
						from OPRQ
						where "DocNum"  = (select top 1 ifnull("BaseRef",0)
											from POR1
											where "DocEntry"= list_of_cols_val_tab_del 
											and  "BaseType"  =1470000113 ));
																		
    ---

	select ifnull(sum("GTotalSC"),0)
	into TotalPO
	from  POR1
	where "DocEntry"= list_of_cols_val_tab_del and ifnull("BaseRef",'0')<>'0' and  ifnull("BaseType",0)  <> 0;
 if(ifnull(TotalPR,0) > 0) then -- Nếu <=
 
	if(ifnull(TotalPO,0)  > ifnull(TotalPR,0) ) then 
	 
		select 22,N'Giá trị PO cao hơn PR đã được duyệt.'  into  error,  error_message  from dummy;
	end if;
  end if;
end if;
*/
----------------------------------============== End Chặn PO
-------********** Khi lưu Purchase Quotation: ---> update phiếu gốc (PR Request) và Purchase Quotation

----------------------============= HỢP ĐỒNG ============----------------------------
 /*
Mục đích: khi thêm mới 1 hơp đồng chính thức đầu tiên ( từ hơp đồng thử việc --> hdong chính thức
thì sẽ kiểm tra nhân viên này có người giới thiệu không? nếu có thì thêm tiền thưởng --> cho người giới thiệu)
 
 /*
-----------===================== Tiền thương giới thiệu=====================--------------------
If (object_type = '171' and transaction_type in ('A','U')) then
  If exists (select  "U_PersonWhoIntroduced"  from  OHEM where ifnull("U_PersonWhoIntroduced",0) <> 0 and "empID"  = :list_of_cols_val_tab_del) then
 
	
	declare isHDChinhThuc smallint = 0;
	declare soTienThuong float = 0;
	declare nguoiGioiThieu smallint = 0;
	
	declare nhomNhanVienDaGioiThieu smallint; -- empIDGioiThieu giới thiệu nhân viên gì (0: --> office, 1: --> shopfloor)
	declare soGioLamNguoiGioiThieu nvarchar(10);

	If exists(select  "U_Type"  from OHEM where ifnull("U_Type",2) <>  2 and "empID"  = :list_of_cols_val_tab_del ) then
	
		 select  "U_Type"
		 into nhomNhanVienDaGioiThieu
		 from OHEM
		 where  "empID"  = :list_of_cols_val_tab_del;
		 
		 ---- Giơi thieu Office : 0
		IF (NhomNhanVienDaGioiThieu  = 0) then
		 	 If exists (select "U_Value" from "@OTHERSETINGS" where "Code" = 'TTGTOF') then   
				select  "U_Value"
				into soTienThuong
				from  "@OTHERSETINGS"
				where "Code" = 'TTGTOF';
				
			End If;  
	    End If;
		 ---- Giơi thieu Shopfloor : 1
		IF (NhomNhanVienDaGioiThieu  = 1) then
		 	 If exists (select "U_Value" from "@OTHERSETINGS" where "Code" = 'TTGTSF') then   
				select  "U_Value"
				into soTienThuong
				from  "@OTHERSETINGS"
				where "Code" = 'TTGTSF';
				
			End If;  
	    End If;
		----

	End If;
	------- isHDChinhThuc
	select count(*) 
	into isHDChinhThuc
	
	from APZ_OHEC a
	inner join APZ_OCTY b on a."type"  =  b."typeID"
	where a."empID"  = :list_of_cols_val_tab_del and b."code" not in  ('HDTV','PLHD');
	
	-------- nguoiGioiThieu
    
    select  ifnull("U_PersonWhoIntroduced",0)
    into nguoiGioiThieu
    from OHEM
    where "empID"= list_of_cols_val_tab_del;
    
 -- Chỉ có 1 hơp đồng chính thức đầu tiên và có thông tin người giới thiệu
	if (:isHDChinhThuc  = 1 ) and (:nguoiGioiThieu  >0) then
		
		declare ngayBatDauHDCThuc timestamp;
		declare periodCode nvarchar(50);
		declare docNumThuongMaster smallint;
		declare soGioLamNguoiGioiThieu smallint;
		  
		----- So gio lam viec cua nguoi gioi thieu  : 40,44, 48h/ tuan
		select ifnull("U_WorkingHours",40)
		
		into soGioLamNguoiGioiThieu
		from OHEM
		where "empID"  = :nguoiGioiThieu;
		 
		
		------ngayBatDauHDCThuc --> kỳ làm việc
		select max(a."startDate")
			into  ngayBatDauHDCThuc
			
			from APZ_OHEC a
			inner join APZ_OCTY b on a."type"  =  b."typeID"
			where a."empID"  = :list_of_cols_val_tab_del and b."code" not in  ('HDTV','PLHD');
			
		------ Lấy kỳ làm việc: periodCode để insert vào kỳ thưởng
			select Max("PeriodCode")
			into periodCode
			
			from APZ_OHWP
			where "PeriodCode" like '%'||(case when month(ngayBatDauHDCThuc) = 12 then year(CURRENT_DATE)+1 else year(CURRENT_DATE) end)||'%' and "PeriodCode" like '%'||:soGioLamNguoiGioiThieu||'%'
				and 
				cast(:ngayBatDauHDCThuc as date) between cast("FromDate" as date) and  cast("ToDate" as  date);
				
	   ---------------------	docNumThuongMaster --> Lấy Docnum
If (length(periodCode)>1) then -- Tồn tại kỳ mới cho insert


		if(select count(*) from APZ_HPT2 master  where "Period"  = :periodCode  ) >0 then
			select ifnull("DocNum",0)
			into docNumThuongMaster
			
			from  APZ_HPT2 master
			where "Period"  = :periodCode;
		else
			 docNumThuongMaster  = 0;
		
		end if;
			
 
		if(docNumThuongMaster ) >0  then
		-- đã tồn tại recode ở master thưởng phạt --> chỉ insert vào table detail thôi
		 if not exists(select "Note"  from  APZ_HPT1 where "Note" like '%' || :list_of_cols_val_tab_del || '%') then 
		 

				insert into  APZ_HPT1("DocNum","EmpId","PenalizationType","LineNum","Date","TotalAmount","Note")

				values(:docNumThuongMaster,
					   :nguoiGioiThieu,
					   5,
					   (select ifnull(max("LineNum"),0) + 1 from APZ_HPT1 where "DocNum"  = :docNumThuongMaster ),
					   ifnull(:ngayBatDauHDCThuc,CURRENT_DATE),
					   :soTienThuong, 'Đã giới thiệu nhân viên '|| list_of_cols_val_tab_del);
		 end if;
		 		
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
					   :nguoiGioiThieu,
					   5,
					   1,-- Line Num (vì chưa có detail nào nên line Num này bắt đầu  = 1
					   ifnull(:ngayBatDauHDCThuc,CURRENT_DATE),
					   :soTienThuong,'Đã giới thiệu nhân viên '|| list_of_cols_val_tab_del);
		 
		 
		end if;	
end if; -- ket thuc if kiem tra ton tai period
	 
	 end if;
  End If;
End If;
*/
------================KET THUC HRM=
-----------===================== End tiền thương giới thiệu=====================--------------------
----------------------============= END HỢP ĐỒNG ============----------------------------
---------======= Chặn Active Master data: Insert
if (cast(object_type as nvarchar) = '4' and transaction_type in ('A')) then
	declare userlist nvarchar(100);
	declare dem smallint = 0;
	declare userUpdated nvarchar(10);
	
 if exists (select "U_Value2" from "@OTHERSETINGS" where   "Code"  = 'USATITEM') then
 	select "U_Value2"
 	into userlist
 	from  "@OTHERSETINGS"
 	where   "Code"  = 'USATITEM';
 	
 	---
	userlist = replace(replace (userlist,'[',''''),']','''');
	select "USER_CODE"
	into  userUpdated
	from  OITM a
	inner join OUSR b on a."UserSign"  =  b."USERID"
	where "ItemCode" = list_of_cols_val_tab_del ;
	
	EXECUTE IMMEDIATE 'select  count(*) from  OUSR where "USER_CODE"  =' ||''''|| :userUpdated ||''''|| ' and  "USER_CODE" not in (' || :userlist || ')'
	into dem ;
	if(dem )>0 then
		 update a 
		 set "frozenFor"  = 'Y',"validFor" = 'N'
		 from OITM a
		 where  "ItemCode" =  :list_of_cols_val_tab_del and a."ItemType" <> 'F';
	end if;
 end if;
end if;
---------======= End Chặn Active Master data: Insert
---------========================== Chặn Active Master data: UPDATE
if (cast(object_type as nvarchar) = '4' and transaction_type in ('U')) then
	declare userlist nvarchar(100);
	declare dem smallint = 0;
	declare userUpdated nvarchar(10);
	
 if exists (select "U_Value2" from "@OTHERSETINGS" where   "Code"  = 'USATITEM') then
 	select "U_Value2"
 	into userlist
 	from  "@OTHERSETINGS"
 	where   "Code"  = 'USATITEM';
 	
 	---
	userlist = replace(replace (userlist,'[',''''),']','''');
	select "USER_CODE"
	into  userUpdated
	from  OITM a
	inner join OUSR b on a."UserSign2"  =  b."USERID"
	where "ItemCode" = list_of_cols_val_tab_del ;
	
	EXECUTE IMMEDIATE 'select  count(*) from  OUSR where "USER_CODE"  =' ||''''|| :userUpdated ||''''|| ' and  "USER_CODE" not in (' || :userlist || ')'
	into dem ;
	if(dem )>0 then
		 update a 
		 set "frozenFor"  = 'Y',"validFor" = 'N'
		 from OITM a
		 where  a."ItemCode" =  :list_of_cols_val_tab_del and a."ItemType" <> 'F';
	end if;
 end if;
end if;
---------========================== END Chặn Active Master data: UPDATE
--------------------------------------------------------------------------------------------------------------------------------

-----  Sinh ma Pr
/*
if (cast(object_type as nvarchar) = '1470000113' and transaction_type in ('A','U')) then

		declare phongban nvarchar(20);
		declare subfuntion  nvarchar(20);
		declare isphongbanOP nvarchar(5)='N';
		declare prefix nvarchar(20) = 'PR';
		declare JVNo int = 0;
		--- Insert mã phòng ban 
		select a."U_CostCenter",  case when a."U_CostCenter" in ( 'PRO','IT','WH' ) then 'Y' else 'N' end 
		into phongban, isphongbanOP
		from OUSR a
		---inner join OUDP b  on a."Department" = b."Code"
		where a."USER_CODE"  = (select distinct "Requester" from OPRQ where "DocEntry"  =  list_of_cols_val_tab_del);
		--- PRGM20220001: PR + Mã phòng ban + Năm + 4 số
		    ----
			if (isphongbanOP = 'Y') then -- phòng ban LÀ  OP 
				prefix  = prefix || 'OP' ||phongban || to_varchar(year(CURRENT_DATE));
			else 
				prefix  = prefix || phongban || to_varchar(year(CURRENT_DATE));
			end if;
			
			select count(*)   
			into JVNo  
			from OPRQ
			where  "U_JVNumber" 
			like  '%'||prefix || '%' and "DocEntry" <> list_of_cols_val_tab_del ;
			
			---- Update giá trị vao U_JVNumber

			update "OPRQ" set "U_JVNumber" =  prefix ||REPLICATE(0, 4 - (LENGTH(JVNo+1))) ||JVNo+1
			where "DocEntry" = list_of_cols_val_tab_del;
			
		 
end if;
*/
--- Sinh Mã PQ : Purchase Quotation
/*
if (cast(object_type as nvarchar) = '540000006' and transaction_type in ('A','U')) then
	declare prefix nvarchar(20) = 'PQ'||to_varchar(year(CURRENT_DATE));
	declare JVNo int;
	--select to_varchar("DocDate",'yy/MM') into prefix from "ODLN" where "DocEntry"=list_of_cols_val_tab_del;
	
	select count(*) into JVNo from "OPQT" 
							  where  "U_JVNumber" like 'PQ'|| to_varchar(year(CURRENT_DATE))||'-%'
	
									  and "DocEntry" <> list_of_cols_val_tab_del ;
	
		 
	
	update "OPQT" set "U_JVNumber" =  prefix  ||  '-'||REPLICATE(0, 6 - (LENGTH(JVNo+1))) ||JVNo+1
	where "DocEntry" = list_of_cols_val_tab_del;
	
end if;
*/
--- Sinh Mã GRPO--> OPDN : Goods Receipt PO
/*
if (cast(object_type as nvarchar) = '20' and transaction_type in ('A','U')) then
	declare prefix nvarchar(20) = 'GRPO'||to_varchar(year(CURRENT_DATE));
	declare JVNo int;
	--select to_varchar("DocDate",'yy/MM') into prefix from "ODLN" where "DocEntry"=list_of_cols_val_tab_del;
	
	select count(*) into JVNo from "OPDN" 
							  where  "U_JVNumber" like 'GRPO'|| to_varchar(year(CURRENT_DATE))||'-%'
	
									  and "DocEntry" <> list_of_cols_val_tab_del ;
	
	
	update "OPDN" set "U_JVNumber" =  prefix  ||  '-'||REPLICATE(0, 6 - (LENGTH(JVNo+1))) ||JVNo+1
	where "DocEntry" = list_of_cols_val_tab_del;
	
end if;
*/
--- Sinh Mã PO
/*
if (cast(object_type as nvarchar) = '22' and transaction_type in ('A','U')) then
	declare prefix nvarchar(20) = 'PO'||to_varchar(year(CURRENT_DATE));
	declare JVNo int;
	--select to_varchar("DocDate",'yy/MM') into prefix from "ODLN" where "DocEntry"=list_of_cols_val_tab_del;
	
	select count(*) into JVNo from "OPOR" 
							  where  "U_JVNumber" like 'PO'|| to_varchar(year(CURRENT_DATE))||'-%'
	
									  and "DocEntry" <> list_of_cols_val_tab_del ;
	
		 
	
	update "OPOR" set "U_JVNumber" =  prefix  ||  '-'||REPLICATE(0, 6 - (LENGTH(JVNo+1))) ||JVNo+1
	where "DocEntry" = list_of_cols_val_tab_del;
	
end if;
*/
--- Sinh Mã Purchase Blanket: CT2022   - 0001
/*
if (cast(object_type as nvarchar) = '1250000025'  and transaction_type in ('A','U')) then
	declare prefix nvarchar(20) = 'CT'||to_varchar(year(CURRENT_DATE));
	declare JVNo int;
	--select to_varchar("DocDate",'yy/MM') into prefix from "ODLN" where "DocEntry"=list_of_cols_val_tab_del;
	
	select count(*) into JVNo from "OOAT" 
	where  "U_JVNumber" like 'CT'|| to_varchar(year(CURRENT_DATE))||'-%' 
	and "AbsID" <> list_of_cols_val_tab_del ; 
	
	update "OOAT" set "U_JVNumber" =  prefix  ||  '-'||REPLICATE(0, 4 - (LENGTH(JVNo+1))) ||JVNo+1
	where "AbsID" = list_of_cols_val_tab_del;
	
end if;
*/
-----------------==================== KẾT THÚC SINH MÃ
--------------------------------------------------------- DOAN CODE BAT BUOC CO----------------------------------------------------------------------------------------------------
-- Tự động tách đối ứng khi add JE----------------------------------------------------------------------------------------------------
if (object_type = '30' and transaction_type in ('A','U')) then
 if (transaction_type = 'U') then
  delete from ALL_JE where TransId = list_of_cols_val_tab_del;
 end if;

 call VAS_999 (tNo=>list_of_cols_val_tab_del,display=>0);
end if;
-- End-Closing Period: Tự động lùi Posting Date về ngày cuối tháng trước
if (object_type = '30' And transaction_type in ('A')) then
	declare transType int;
	declare transDate timestamp;	
	select "TransType","RefDate" into transType,transDate from OJDT a where a."TransId" = list_of_cols_val_tab_del;
	
	if (transType = '-3' and DAYOFMONTH(transDate)=1) then
		declare FinPeriodID int;
		select "AbsEntry" into FinPeriodID from OFPR where "T_RefDate"=ADD_DAYS(transDate,-1);
		
		update OJDT set 
			"RefDate" = ADD_DAYS("RefDate",-1),
			"FinncPriod" = FinPeriodID
		where "TransId" in (select "TransId" from JDT1 where "TransId" = list_of_cols_val_tab_del and LEFT("Account",3) = '421');
		
		update JDT1 set 
			"RefDate" = ADD_DAYS("RefDate",-1),
			"FinncPriod" = FinPeriodID
		where "TransId" in (select "TransId" from JDT1 where "TransId" = list_of_cols_val_tab_del and LEFT("Account",3) = '421');
	end if;
end if;

--------------------------------------------------------------------------------------------------------------------------------
----- Update so phieu va table detail
/* tam khoa
--1.OPRQ --  Purchase Request
if (cast(object_type as nvarchar) = '1470000113'  and transaction_type in ('A','U')) then

	update  a
	set "U_CodeParent"  = b."U_JVNumber"
	from PRQ1 a
	inner join  OPRQ b on a."DocEntry" = b."DocEntry"
	where b."DocEntry"  = list_of_cols_val_tab_del;
end if;
*/
/* tam khoa
--2.OPQT --  Purchase Quotation
if (cast(object_type as nvarchar) = '540000006'  and transaction_type in ('A','U')) then

	update  a
	set "U_CodeParent"  = b."U_JVNumber"
	from PQT1 a
	inner join  OPQT b on a."DocEntry" = b."DocEntry"
	where b."DocEntry"  = list_of_cols_val_tab_del;
end if;
*/
/* tam khoa
--3.OPOR --  Purchase Order
if (cast(object_type as nvarchar) = '22'  and transaction_type in ('A','U')) then

	update  a
	set "U_CodeParent"  = b."U_JVNumber"
	from POR1 a
	inner join  OPOR b on a."DocEntry" = b."DocEntry"
	where b."DocEntry"  = list_of_cols_val_tab_del;
end if;
*/
--4.OPDN --  Goods Receipt PO
/* tam khoa
if (cast(object_type as nvarchar) = '20'  and transaction_type in ('A','U')) then

	update  a
	set "U_CodeParent"  = b."U_JVNumber"
	from PDN1 a
	inner join  OPDN b on a."DocEntry" = b."DocEntry"
	where b."DocEntry"  = list_of_cols_val_tab_del;
end if;
*/
/*
--5.ODRF : phieu Draft
if (cast(object_type as nvarchar) = '112'  and transaction_type in ('A','U')) then

	update  a
	set "U_CodeParent"  = b."U_JVNumber"
	from DRF1 a
	inner join  ODRF b on a."DocEntry" = b."DocEntry"
	where b."DocEntry"  = list_of_cols_val_tab_del;
	--- Update vào số tham chiếu 2 - line: Khi copy from từ PO --> BaseCode =  PO; Khi đó ThamChieu2= PR và ngược lại
	update  a
	set "U_CodeParent"  = b."U_JVNumber"
	from DRF1 a
	inner join  ODRF b on a."DocEntry" = b."DocEntry"
	where b."DocEntry"  = list_of_cols_val_tab_del;
end if;
*/

--------------- Update thong tin Remark IRS  --> JE 
/* tam khoa
if (cast(object_type as nvarchar) = '30' and transaction_type in ('A','U')) then

declare Remarks nvarchar(500) ;

 
if exists (select "Memo"  from OJDT where "TransId" = :list_of_cols_val_tab_del) then  
 
 
 select "Memo" 
 into Remarks  
 from OJDT where "TransId" = list_of_cols_val_tab_del;
   
   
 
 	if exists(select "U_SAPID" from "@SALARYIRSTOSAP" where "U_SAPID" =  Remarks ) then
	   update a
	   set  "LineMemo"  = (select  "U_GHICHU" from "@SALARYIRSTOSAP" b where  "U_SAPID" =  Remarks and a."Line_ID"  = b."U_STT" )
	   from JDT1 a   
	    where "TransId" = list_of_cols_val_tab_del ;
	end if;
 
 end if;


End If;
*/
-------------------
/* tam khoa
if (cast(object_type as nvarchar) = '30' and transaction_type in ('A')) then
	 Update a
	 set "U_TrangThaiTuKeToan" = N'Kế toán đã tạo phiếu JE'
	 from  "@SALARYIRSTOSAP" a
	 where "U_SAPID" = (select "Memo" from OJDT where "TransId" = list_of_cols_val_tab_del );
 

 
end if;
*/
----- Luong Thang: 12-2022: 20 ký tự, nếu bút toán cancel (đảo) --> >20 ký tự
/* tam khoa
if (cast(object_type as nvarchar) = '30' and transaction_type in ('A')) then
	 Update a
	 set "U_TrangThaiTuKeToan" = N'Kế toán đã hủy phiếu JE'
	 from  "@SALARYIRSTOSAP" a
  	  where "U_SAPID" like  '%'||(select left("Memo",20) from OJDT where LENGTH ("Memo")>20 and "TransId" = list_of_cols_val_tab_del )||'%' ;

 
end if;
*/
 
-- bat buoc nhap so to khai
if (cast(object_type as nvarchar) = '20' and transaction_type in ('A','U')) then
	if (select count(*) from OPDN where "DocEntry" = list_of_cols_val_tab_del and ifnull("U_SoToKhai",'')='') >0 then
		select 20,N'Số tờ khai không được để trống!'  into  error,  error_message  from dummy;
	end if;
end if;
 
--- Function Locked
IF (cast(object_type as nvarchar) in ('13','14','15','16','18','19','20','21','59','60','67','69') and transaction_type in ('A','U')) then
	-- 13: U_ARInvoice
	if (cast(object_type as nvarchar)  = '13') then
		declare postingDate date;
		
		select "DocDate"
		into postingDate
		from OINV
		where "DocEntry" = list_of_cols_val_tab_del ;
		
		
		
		 if ( select count (*) from   "@FUNCTIONLOCKED" where "U_ARInvoice" =  'Y'  
		 and left("U_Period",4)  =  year(postingDate) and right("U_Period",2)  =  month(postingDate)) > 0 then 
		

			select 68,N'Nghiệp vụ đã bị khóa!'  into  error,  error_message  from dummy;
		 end if;
	end if;
	---
-- 14: U_ARCreditMemo
	if (cast(object_type as nvarchar)  = '14') then
		declare postingDate date;
		
		select "DocDate"
		into postingDate
		from ORIN
		where "DocEntry" = list_of_cols_val_tab_del ;
		
		
		
		 if ( select count (*) from   "@FUNCTIONLOCKED" where "U_ARCreditMemo" =  'Y'  
		 and left("U_Period",4)  =  year(postingDate) and right("U_Period",2)  =  month(postingDate)) > 0 then 
		

			select 69,N'Nghiệp vụ đã bị khóa!'  into  error,  error_message  from dummy;
		 end if;
	end if;
	---
-- 15: U_Delivery
	if (cast(object_type as nvarchar)  = '15') then
		declare postingDate date;
		
		select "DocDate"
		into postingDate
		from ODLN
		where "DocEntry" = list_of_cols_val_tab_del ;
		
		
		
		 if ( select count (*) from   "@FUNCTIONLOCKED" where "U_Delivery" =  'Y'  
		 and left("U_Period",4)  =  year(postingDate) and right("U_Period",2)  =  month(postingDate)) > 0 then 
		

			select 70,N'Nghiệp vụ đã bị khóa!'  into  error,  error_message  from dummy;
		 end if;
	end if;
	---
-- 16: U_Returns
	if (cast(object_type as nvarchar)  = '16') then
		declare postingDate date;
		
		select "DocDate"
		into postingDate
		from ORDN
		where "DocEntry" = list_of_cols_val_tab_del ;
		
		
		
		 if ( select count (*) from   "@FUNCTIONLOCKED" where "U_Returns" =  'Y'  
		 and left("U_Period",4)  =  year(postingDate) and right("U_Period",2)  =  month(postingDate)) > 0 then 
		

			select 71,N'Nghiệp vụ đã bị khóa!'  into  error,  error_message  from dummy;
		 end if;
	end if;
	---
-- 18: U_APInvoice
	if (cast(object_type as nvarchar)  = '18') then
		declare postingDate date;
		
		select "DocDate"
		into postingDate
		from OPCH
		where "DocEntry" = list_of_cols_val_tab_del ;
		
		
		
		 if ( select count (*) from   "@FUNCTIONLOCKED" where "U_APInvoice" =  'Y'  
		 and left("U_Period",4)  =  year(postingDate) and right("U_Period",2)  =  month(postingDate)) > 0 then 
		

			select 72,N'Nghiệp vụ đã bị khóa!'  into  error,  error_message  from dummy;
		 end if;
	end if;
	---
-- 19: U_APCreditMemo
	if (cast(object_type as nvarchar)  = '19') then
		declare postingDate date;
		
		select "DocDate"
		into postingDate
		from ORPC
		where "DocEntry" = list_of_cols_val_tab_del ;
		
		
		
		 if ( select count (*) from   "@FUNCTIONLOCKED" where "U_APCreditMemo" =  'Y'  
		 and left("U_Period",4)  =  year(postingDate) and right("U_Period",2)  =  month(postingDate)) > 0 then 
		

			select 73,N'Nghiệp vụ đã bị khóa!'  into  error,  error_message  from dummy;
		 end if;
	end if;
	---
	-- 20: U_GoodsReceiptPO
	if (cast(object_type as nvarchar)  = '20') then
		declare postingDate date;
		
		select "DocDate"
		into postingDate
		from OPDN
		where "DocEntry" = list_of_cols_val_tab_del ;
		
		
		
		 if ( select count (*) from   "@FUNCTIONLOCKED" where "U_GoodsReceiptPO" =  'Y'  
		 and left("U_Period",4)  =  year(postingDate) and right("U_Period",2)  =  month(postingDate)) > 0 then 
		

			select 74,N'Nghiệp vụ đã bị khóa!'  into  error,  error_message  from dummy;
		 end if;
	end if;
	---
-- 21: U_GoodsReturn
	if (cast(object_type as nvarchar)  = '21') then
		declare postingDate date;
		
		select "DocDate"
		into postingDate
		from ORPD
		where "DocEntry" = list_of_cols_val_tab_del ;
		
		
		
		 if ( select count (*) from   "@FUNCTIONLOCKED" where "U_GoodsReturn" =  'Y'  
		 and left("U_Period",4)  =  year(postingDate) and right("U_Period",2)  =  month(postingDate)) > 0 then 
		

			select 75,N'Nghiệp vụ đã bị khóa!'  into  error,  error_message  from dummy;
		 end if;
	end if;
	---
-- 59: U_GoodsReceipt
	if (cast(object_type as nvarchar)  = '59') then
		declare postingDate date;
		
		select "DocDate"
		into postingDate
		from OIGN
		where "DocEntry" = list_of_cols_val_tab_del ;
		
		
		
		 if ( select count (*) from   "@FUNCTIONLOCKED" where "U_GoodsReceipt" =  'Y'  
		 and left("U_Period",4)  =  year(postingDate) and right("U_Period",2)  =  month(postingDate)) > 0 then 
		

			select 76,N'Nghiệp vụ đã bị khóa!'  into  error,  error_message  from dummy;
		 end if;
	end if;
	---
-- 60: U_GoodsIssue
	if (cast(object_type as nvarchar)  = '60') then
		declare postingDate date;
		
		select "DocDate"
		into postingDate
		from OIGE
		where "DocEntry" = list_of_cols_val_tab_del ;
		
		
		
		 if ( select count (*) from   "@FUNCTIONLOCKED" where "U_GoodsIssue" =  'Y'  
		 and left("U_Period",4)  =  year(postingDate) and right("U_Period",2)  =  month(postingDate)) > 0 then 
		

			select 77,N'Nghiệp vụ đã bị khóa!'  into  error,  error_message  from dummy;
		 end if;
	end if;
	---
-- 67: U_InventoryTransfer
	if (cast(object_type as nvarchar)  = '67') then
		declare postingDate date;
		
		select "DocDate"
		into postingDate
		from OWTR
		where "DocEntry" = list_of_cols_val_tab_del ;
		
		
		
		 if ( select count (*) from   "@FUNCTIONLOCKED" where "U_InventoryTransfer" =  'Y'  
		 and left("U_Period",4)  =  year(postingDate) and right("U_Period",2)  =  month(postingDate)) > 0 then 
		

			select 78,N'Nghiệp vụ đã bị khóa!'  into  error,  error_message  from dummy;
		 end if;
	end if;
	---
-- 69: U_LandedCosts
	if (cast(object_type as nvarchar)  = '69') then
		declare postingDate date;
		
		select "DocDate"
		into postingDate
		from OIPF
		where "DocEntry" = list_of_cols_val_tab_del ;
		
		
		
		 if ( select count (*) from   "@FUNCTIONLOCKED" where "U_LandedCosts" =  'Y'  
		 and left("U_Period",4)  =  year(postingDate) and right("U_Period",2)  =  month(postingDate)) > 0 then 
		

			select 79,N'Nghiệp vụ đã bị khóa!'  into  error,  error_message  from dummy;
		 end if;
	end if;
	---
END if;
--- 
--- UPDATE QA Template
	if (cast(object_type as nvarchar)  = '20' and transaction_type in ('A','U')) then
		 update a
		 set "U_QCTemplate"  = b."U_QCTemplate"
		 from PDN1 a
		 inner join OITM b on a."ItemCode"  =  b."ItemCode"
		 where a."DocEntry"  = list_of_cols_val_tab_del;
		   
	end if;
----- Chặn xóa danh mục: QC_METHOD
 
if (cast(object_type as nvarchar) = 'QC_METHOD' and transaction_type  ='D') then
		
		if(((select count(*) from   "@QC_TEMPLATE_L" where "U_Method" = list_of_cols_val_tab_del ) > 0)
			or 
			((select count(*) from   "@QC_CHAR" where "U_Method" = list_of_cols_val_tab_del ) > 0)
			
			)
		 then
		  select 80,'Không thể xóa. Danh mục này đã được sử dụng!'  into  error,  error_message  from dummy;
		end if;
			
end if;
----- Chặn xóa danh mục: QC_INS_STAGE
if (cast(object_type as nvarchar) = 'QC_INS_STAGE' and transaction_type  ='D') then
		
		if(((select count(*) from   "@QC_RESULT_H" where "U_Ins_StageCode" = list_of_cols_val_tab_del ) > 0)
			or 
			((select count(*) from   "@QC_CHAR" where "U_Ins_StageCode" = list_of_cols_val_tab_del ) > 0)
			or
			((select count(*) from   "@QC_TEMPLATE_H" where "U_Ins_StageCode" = list_of_cols_val_tab_del ) > 0)
			)
		 then
		  select 81,'Không thể xóa. Danh mục này đã được sử dụng!'  into  error,  error_message  from dummy;
		end if;
			
end if;
----- Chặn xóa danh mục: QC_CHAR_GRP
 
 
if (cast(object_type as nvarchar) = 'QC_INS_STAGE' and transaction_type  ='D') then
		
		if(((select count(*) from   "@QC_TEMPLATE_L" where "U_GroupCode" = list_of_cols_val_tab_del ) > 0)
			or 
			((select count(*) from   "@QC_CHAR" where "U_GroupCode" = list_of_cols_val_tab_del ) > 0)
			or
			((select count(*) from   "@QC_RESULT_CHAR_L" where "U_GroupCode" = list_of_cols_val_tab_del ) > 0)
			)
		 then
		  select 82,'Không thể xóa. Danh mục này đã được sử dụng!'  into  error,  error_message  from dummy;
		end if;
			
end if;

 
----- Chặn lưu trùng danh mục: QC_CHAR
 
 
if (cast(object_type as nvarchar) = 'QAQCCharacteristics' and transaction_type  IN ('A','U')) then
		begin 
		declare Code  nvarchar(20);
		if((select count(*) from   "@QC_CHAR" where "U_Code" = (select "U_Code" from  "@QC_CHAR" where "DocEntry"  = list_of_cols_val_tab_del  )) > 1) then
			 
			 select "U_Code"
			 into Code
			 from  "@QC_CHAR" where "DocEntry"  = list_of_cols_val_tab_del ;
		 
		  select 83,'Code không thể trùng!' || Code  into  error,  error_message  from dummy;
		end if;
		end;	
end if;
---- Update thông tin QCResultEntry tơi phiếu gốc: GRPO or Goods Receipt (Receipt from production)
if (cast(object_type as nvarchar) = 'QAQCResult' and transaction_type  IN ('A','U')) then
		Begin 
			declare BaseDocType  nvarchar(10);
			declare BaseDocEntry  nvarchar(10);
			
			if(transaction_type = 'A') then
				update "@QC_RESULT_H"
				set "U_UserConfirm"  = NULL, "U_ConfirmDate" = NULL
				where   "DocEntry"  = list_of_cols_val_tab_del ;
			end if;
			
			select "U_BaseDocType"
				 into BaseDocType
				 from  "@QC_RESULT_H" where "DocEntry"  = list_of_cols_val_tab_del ;
				 
			select "U_BaseDocEntry"
				 into BaseDocEntry
				 from  "@QC_RESULT_H" where "DocEntry"  = list_of_cols_val_tab_del ;
			
			
			 If (BaseDocType= '20') then -- Update vao phieu GRPO
			 
			 	update PDN1
			 	set "U_QCDocEntry" = list_of_cols_val_tab_del
			 	where "DocEntry"  = BaseDocEntry;
			 End If;
			 
			If (BaseDocType= '59') then -- Update vao phieu Goods Receipt
			 
			 	update IGN1
			 	set "U_QCDocEntry" = list_of_cols_val_tab_del
			 	where "DocEntry"  = BaseDocEntry;
			 End If;
			 
		End;	
end if;

if (cast(object_type as nvarchar) = 'QAQCResult' and transaction_type  in ( 'A','U')) then
		Begin 
		
			Declare SSCC nvarchar(5000);
			
			select STRING_AGG(cast("U_SSCC" as nvarchar(5000)),',')
			into SSCC
			from "@QC_RESULT_INS_L" where "DocEntry" = list_of_cols_val_tab_del
			group by "DocEntry";
			
			 
			
			if exists ( select count(*)
					from  "SPLITSTRING" (SSCC ,',')
					group by "part"
					having count("part") >1) then
					
			 select 84,'Chọn trùng SSCC!',  error_message  from dummy;
			
			
			end if;


		End;
end if;
---
/*
if (cast(object_type as nvarchar) = 'MaintenManageme' and transaction_type  in ( 'U','D')) then
	Begin
			
			If exists (select 1 from   "@MAINTENANCE_H" 
								where "DocEntry"  = list_of_cols_val_tab_del 
									and "U_DocStatus" = 'C' ) then
			select 72,'Không thể thực hiện do phiếu đã closed! ',  error_message  from dummy;
			
			End if;
			
			
	End;
		  
		  
		  
end if;
*/

/* tam khoa -- 26:08:2023
---- Update thoong tin lenh san xuat vao udo production journal
if (cast(object_type as nvarchar) = '202' and transaction_type  in ( 'A','U')) then
		Begin 
			
			
			update a
			set "U_ItemCode" = (select "ItemCode" from OWOR where "DocEntry" = list_of_cols_val_tab_del),
				"U_ItemName" = (select "ProdName" from OWOR where "DocEntry" = list_of_cols_val_tab_del),
				"U_ProID" = list_of_cols_val_tab_del,
				"U_ProDocNum"  = (select "DocNum" from OWOR where "DocEntry" = list_of_cols_val_tab_del)
			from  "@PRO_JOURNAL_H" a
			where  a."DocEntry" =  (select "U_Pro_Journal" from OWOR where "DocEntry" = list_of_cols_val_tab_del );
		end;
end if;
*/
---- Costing: Chặn giá nhập thành phẩm < min và > max
 
IF (cast(object_type as nvarchar) = '59' and transaction_type  in ( 'A','U')) then

	--1 Khi NSD add phiếu Receipt, đơn giá thành phẩm sẽ được so sánh vs bảng giá Standard
	--2 Nếu ĐG thành phẩm <> ĐG Standard => chặn lưu phiếu

---------
 If exists (select 1  from OPLN  where  ifnull("U_IsBasePriceList",'')  = 'Y') then
	create local temporary table #tblReceipt
	(
		ItemCode nvarchar(50),
		Price numeric(19,6)
	);
	
	
	insert into #tblReceipt
	select "ItemCode","PriceBefDi"/(case when  ifnull("Rate",0)>0 then "Rate" else 1 end )  as "UnitPrice"
	from IGN1 
	where  "DocEntry"  = list_of_cols_val_tab_del;
--- Bảng giá cơ sở
	create local temporary table #tblBasePrice
	(
		ItemCode nvarchar(50),
		Rate numeric(19,6),
		BasePrice numeric(19,6),
		Min_BasePrice numeric(19,6),
		Max_BasePrice numeric(19,6)
	);
	
	insert into #tblBasePrice (ItemCode, Rate,BasePrice,Min_BasePrice,Max_BasePrice)
	SELECT   T1."ItemCode",T2."U_Rate",T1."Price", 
				T1."Price" - (T1."Price" * ifnull(T2."U_Rate",0)/100 ) as "Min",
				T1."Price" + (T1."Price" * ifnull(T2."U_Rate",0)/100 ) as "Max"
	 FROM ITM1  T1 
	 INNER JOIN (select top 1 * from OPLN  where  ifnull("U_IsBasePriceList",'')  = 'Y') T2 ON T1."PriceList" = T2."ListNum"
	 where  "Price">0;
	
	RS= 
		select a.ItemCode "Itemcode", a.Price "Price", b.BasePrice "BasePrice",b.Min_BasePrice "Min_BasePrice",b.Max_BasePrice "Max_BasePrice"
		from #tblReceipt a
		left join #tblBasePrice b on a.ItemCode  = b.ItemCode;
	
	
	IF  Exists(Select 1 From :RS where (ifnull("Price",0) < ifnull("Min_BasePrice",0)) or (ifnull("Price",0) > ifnull("Max_BasePrice",0))) then 
		select 85,N'Giá nhập kho vượt mức giá cơ bản!'  into  error,  error_message  from dummy;
	
	END IF;
	

	drop table #tblReceipt;
	drop table #tblBasePrice;
   End If;
End If;
 
---------- update trạng thái Batch: Batch Attribute 2 (Từ màn hình nhập QCResult)
IF (cast(object_type as nvarchar) = 'QAQCResult' and transaction_type  in ( 'A','U')) then
	Begin
		
		Create local temporary table #QCResult
		(
			ID_Result int,
			SSCC_Result nvarchar(5000),
			Status_Result nvarchar(5000)
		);
		insert into  #QCResult (ID_Result,SSCC_Result, Status_Result)
		
		 select ROW_NUMBER() OVER (),"U_SSCC", "U_Status"
		 from "@QC_RESULT_INS_L" 
		 where "DocEntry"   = :list_of_cols_val_tab_del and ifnull(cast("U_SSCC" as nvarchar(5000)),'')  <> '';
		 
		
		 If exists (select 1 from #QCResult where ifnull(Status_Result,'') = '' ) then
		 	select 86,N'Cần chọn trạng thái SSCC!'  into  error,  error_message  from dummy;
		 Else 
		 	 Begin
		 	 		Declare i int = 1;
					Declare loopCount int = 0;
					Declare SSCC nvarchar(5000);
					Declare Status nvarchar(5000);
		 	 		Create local temporary table #SSCC
					(
						ID int,
						SSCC nvarchar(50),
						Status nvarchar(50)
					);
		
					Select Count(*) Into loopCount From #QCResult;
					While :i <= :loopCount
				  	DO
						Select SSCC_Result Into SSCC From #QCResult Where ID_Result = :i;
						Select Status_Result Into Status From #QCResult Where ID_Result = :i;
						
							insert into  #SSCC (ID,SSCC,Status )
							select :i,"part" ,:Status
							from  "SPLITSTRING" (SSCC ,',');
					i := i + 1;
					END WHILE;
					
					update a
					set "LotNumber" = b.Status
					from OBTN a 
					inner join #SSCC b on a."DistNumber" = b.SSCC  ;
					
				 drop table #SSCC;
					
		 	 End;
		 End IF;
		
		
		 drop table #QCResult;
		
		end;
End IF;
---- Update thoong tin lenh san xuat vao phieu Transfer tu Transfer Request
/*
if (cast(object_type as nvarchar) = '67' and transaction_type  in ( 'A','U')) then -- 67:Inventory Transfer
		Begin 
			declare TransferRequestEntry nvarchar(15);
			
		if exists (select top 1 "BaseRef" from WTR1
					where "BaseType" = '1250000001' and "DocEntry" = :list_of_cols_val_tab_del) then
			
			
			select top 1 "BaseRef"
			into TransferRequestEntry
			from WTR1
			where "BaseType" = '1250000001' and "DocEntry" = :list_of_cols_val_tab_del;
			
			update a
			set "U_ProductionEntry" = (select ifnull("U_ProductionEntry",'') 
										from OWTQ where "DocEntry" = :TransferRequestEntry)
			from  OWTR a			
			where  a."DocEntry" =  :list_of_cols_val_tab_del;
		end if;	

			 
		end;
end if;
*/
---- Chặn: receipt from product, transfer request, transfer cho PO (KHI U_POStatus = 'Completed')
IF (cast(object_type as nvarchar) in ('59','1250000001','67') and transaction_type  in ( 'A','U')) then -- 67:Inventory Transfer
		Begin 
			 --- Receipt from production
			 If (cast(object_type as nvarchar) = '59') then -- Goods Receipt
				begin
				
					
					if exists(select  "U_POStatus"
										from OWOR 
										where "DocEntry"  = (select top 1 "BaseEntry" from IGN1 where "BaseType"  = '202' 
															 and "DocEntry" = :list_of_cols_val_tab_del) and ifnull("U_POStatus",'') ='Completed') then
															 
							select 87,N'Lệnh sản xuất đang ở trạng thái: Completed!'  into  error,  error_message  from dummy;
					end if; 
					
				end;
			 End If;
			 ---
			 --- Receipt from production
			 If (cast(object_type as nvarchar) = '1250000001') then -- Inventory Transfer Request	
				begin

					if exists(select  "U_POStatus"
										from OWOR 
										where "DocEntry"  = (select "RefDocEntr" 
										from  WTQ21 
										where "ObjectType"  ='1250000001' and "RefObjType"  = '202' 
										and "DocEntry" =:list_of_cols_val_tab_del ) and ifnull("U_POStatus",'') ='Completed') then
															 
							select 88,N'Lệnh sản xuất đang ở trạng thái: Completed!'  into  error,  error_message  from dummy;
					end if; 
					
				end;
			 End If;
			 ---
						 --- Receipt from production
			 If (cast(object_type as nvarchar) = '67') then -- Inventory Transfer
				begin

					if exists(select  "U_POStatus"
										from OWOR 
										where "DocEntry"  = (select "RefDocEntr" 
										from  WTR21 
										where "ObjectType"  ='67' and "RefObjType"  = '202' 
										and "DocEntry" =:list_of_cols_val_tab_del ) and ifnull("U_POStatus",'') ='Completed') then
															 
							select 89,N'Lệnh sản xuất đang ở trạng thái: Completed!'  into  error,  error_message  from dummy;
					end if; 
					
				end;
			 End If;
			 ---
		end;
END IF; 
---
---- Chặn nghiệp vụ theo trạng thái SSCC: Batch Attribute 2
-- Nguyên Vật Liệu: 
If (cast(object_type as nvarchar) in ( '15','20','60','67') and transaction_type  in ( 'A','U')) then -- 67: Inventory Transfer
		Begin
			declare isNVL char(1) = 'N';
			declare isTP char(1) = 'N';
			If exists(select 1 from  OITM a 
								inner join IBT1 b on a."ItemCode"  = b."ItemCode"
								where a."ManBtchNum" = 'Y' and a."ItmsGrpCod" in (113,114,127) 
								and b."BaseEntry"  = list_of_cols_val_tab_del and b."BaseType" = object_type ) then -- NVL chinh, NVL Phu

				select 'Y' into isNVL  from dummy;
				
				
			ElseIf exists(select 1 from  OITM a 
								inner join WTR1 b on a."ItemCode"  = b."ItemCode"
								where a."ManBtchNum" = 'Y' and a."ItmsGrpCod" in (120,121,127)
								and b."BaseEntry"  = list_of_cols_val_tab_del and b."BaseType" = object_type ) then -- Thanh pham, Rework

				select 'Y' into isTP  from dummy;
				
							--- Delivery
			ElseIf exists(select 1 from  OITM a 
								inner join IBT1 b on a."ItemCode"  = b."ItemCode"
								where a."ManBtchNum" = 'Y' and a."ItmsGrpCod" in (120,121)  
								and b."BaseEntry"  = list_of_cols_val_tab_del and b."BaseType" = object_type ) then -- Thanh pham, Rework

				select 'Y' into isTP  from dummy;
				
				
			End IF;
		
			
			Create local temporary table #Batch_Status
			(
				SSCC nvarchar(50),
				ToWhs nvarchar(50),
				Status nvarchar(50),
				BsDocType int
			);
			insert into #Batch_Status (SSCC,ToWhs,Status, BsDocType)
			Select a."BatchNum",a."WhsCode",ifnull(upper(b."LotNumber"),'') as "Status",a."BsDocType"
			from IBT1 a
			inner join  OBTN b on a."BatchNum" =  b."DistNumber"
			where a."Direction" = (case when :object_type  IN ( '15','60') then '1' else '0' end)  and a."BaseType" = :object_type and a."BaseEntry"  = :list_of_cols_val_tab_del;
			--old "Direction" = '0'
				If (object_type  = '15') then -- Delivery
					---
					IF(isTP = 'Y') then
							If exists ((select 1 from  #Batch_Status where Status in ('QI','BLOCK'))) then  
								
								select 90,N'SSCC Status QI, Block chặn xuất bán!'  into  error,  error_message  from dummy;
								
							End If;
					End IF;
					---		
				End if;
				 
				If (object_type  = '20') then -- Goods Receipt PO
					 IF(isNVL = 'Y') then
							If exists ((select 1 from  #Batch_Status where Status <> 'QI'  and ToWhs  =  'K')) then   
							
							select 91,N'Kho K phải ở trạng thái QI'  into  error,  error_message  from dummy;
							
							End If;
							
							If exists ((select 1 from  #Batch_Status where Status not in( 'RELEASE','RELEASED')  and ToWhs  =  'B')) then   
							
							select 92,N'Kho B phải ở trạng thái RELEASE'  into  error,  error_message  from dummy;
							
							End If;
					End if;
					---		
				End if;
				
				If (object_type  = '60') then -- Goods Issue Nhưng lấy phiếu Issue for production
					IF(isNVL = 'Y') then
							If exists ((select 1 from  #Batch_Status where Status in ('QI','BLOCK') and BsDocType = 202)) then  
								
								select 93,N'Tồn tại SSCC với trạng thái: QI hoặc BLOCK!'  into  error,  error_message  from dummy;
								
							End If;
					End IF;


				End if;
				--
				If (object_type  = '67') then -- Inventory Transfer
					
					IF(isNVL = 'Y') then
							If exists ((select 1 from  #Batch_Status where Status in ('QI','BLOCK') and ToWhs in ( 'P-M','P-L-M','P-I-M'))) then -- N: kho chờ xử lý
							
							select 94,N'SSCC Status QI, Block chặn chuyển tới kho: P-M, P-L-M, P-I-M!'  into  error,  error_message  from dummy;
							
							End If;
					End if;
					---
					/*
					IF(isTP = 'Y') then
							If exists ((select 1 from  #Batch_Status where Status in ('QI','BLOCK') and ToWhs <> 'N')) then -- N: kho chờ xử lý
								
								select 78,N'SSCC Status QI, Block chặn chuyển kho!'  into  error,  error_message  from dummy;
								
							End If;
					End IF;
					*/
					---					
				--	
				End if;	

		End;
		drop table #Batch_Status;
		
End If; 
--- Chặn trùng SSCC (Số Batch) - Khi Gen
/*
If (cast(object_type as nvarchar) in ( '20','59')) then -- GRPO, Goods Receipt
				begin
					if (select count(*)   from  IBT1 where "BaseType" = :object_type and "BatchNum"  
										= (select top 1 "BatchNum" from IBT1 
											where "BaseEntry"  = :list_of_cols_val_tab_del and "BaseType" = :object_type))>1 then
						select 82,N'SSCC không được trùng!'   into  error,  error_message  from dummy;
					end if;
				end;
				
End If;
*/
--- Chặn trùng SSCC (Số Batch) - Khi Gen

 If (cast(object_type as nvarchar) in ( '20','59') and 
 	(select count(*) from "@OTHERSETINGS" where upper("Code")  ='CHANTRUNGSSCC' and upper("U_Value2")  = 'Y')=1) then -- GRPO, Goods Receipt
				begin
					IF(cast(object_type as nvarchar) = '20') then
						If(select count(*) from  OPDN where "DocEntry" = :list_of_cols_val_tab_del 
								and "CANCELED" = 'N' ) > 0 then
								
								
						--
							if (select count(*)   from  IBT1 where "BaseType" = :object_type and "BatchNum"  
											= (select top 1 "BatchNum" from IBT1 
												where "BaseEntry"  = :list_of_cols_val_tab_del and "BaseType" = :object_type))>1 then
									select 95,N'SSCC không được trùng!'   into  error,  error_message  from dummy;
							end if;
						End If;
					END IF;
					------ 59
					IF(cast(object_type as nvarchar) = '59') then
						If(select count(*) from  OIGN where "DocEntry" = :list_of_cols_val_tab_del 
										and "CANCELED" = 'N'    ) > 0 then
						if not exists(
										 select 1
										 from OIGN a 
										 inner join IGN1 b on a."DocEntry"  =  b."DocEntry"
										 inner join OITM c on b."ItemCode"  = c."ItemCode"
										 inner join OITB d on c."ItmsGrpCod"  = d."ItmsGrpCod"
										 inner join OWOR e on e."DocEntry" = b."BaseEntry"
										 inner join WOR1 f on e."DocEntry"  = f."DocEntry" and b."ItemCode" =  f."ItemCode" and b."BaseLine" = f."LineNum"
										 where a."DocEntry"  = :list_of_cols_val_tab_del and b."BaseType" = 202 
										 AND 
										 	 ((f."PlannedQty"  > 0 --PlannedQty = - (âm) là rework 
												and d."ItmsGrpCod" not in (120,121,127))
												------
												or --- Thành phẩm: rework - âm ở line 
												(f."PlannedQty" > 0 and d."ItmsGrpCod"  in (121,127)))) then
								   --- Không là phiểu return component thì kiểm tra trùng sscc		
												
									if (select count(*)   from  IBT1 where "BaseType" = :object_type and "BatchNum"  
													= (select top 1 "BatchNum" from IBT1 
														where "BaseEntry"  = :list_of_cols_val_tab_del and "BaseType" = :object_type))>1 then
											select 96,N'SSCC không được trùng!'   into  error,  error_message  from dummy;
									end if;
						
						end if;
									

						End If;
					---
						
						
					END IF;
					
					
				end;
				
End If;

--- Chặn không cho chuyển trạng thái closed trên lệnh sản xuất (Trường chuẩn sap)
If (cast(object_type as nvarchar) = '202' and  transaction_type  in ( 'A','U')) then -- lênh sản xuất
				begin
					if (select count(*) from OWOR where "Status" = 'L' and "DocEntry" = list_of_cols_val_tab_del)>0 then
						select 97,N'Hệ thốngg không cho phép cập nhật trạng thái Closed!'   into  error,  error_message  from dummy;
					end if;
				end;
				
End If;

----------
---- Chặn: receipt from product, transfer request, transfer cho PO (KHI U_POStatus = 'Close')
IF (cast(object_type as nvarchar) in ('59','60','1250000001','67') and transaction_type  in ( 'A','U')) then -- 67:Inventory Transfer
		Begin 
			 --- Receipt from production
			 If (cast(object_type as nvarchar) = '59') then -- Goods Receipt
				begin
				
					
					if exists(select  "U_POStatus"
										from OWOR 
										where "DocEntry"  = (select top 1 "BaseEntry" from IGN1 where "BaseType"  = '202' 
															 and "DocEntry" = :list_of_cols_val_tab_del) and ifnull("U_POStatus",'') ='Closed') then
															 
							select 98,N'Lệnh sản xuất đang ở trạng thái: Closed!'  into  error,  error_message  from dummy;
					end if; 
					
				end;
			 End If;
			 ---
			If (cast(object_type as nvarchar) = '60') then -- Goods Issued
				begin
				
					
					if exists(select  "U_POStatus"
										from OWOR 
										where "DocEntry"  = (select top 1 "BaseEntry" from IGE1 where "BaseType"  = '202' 
															 and "DocEntry" = :list_of_cols_val_tab_del) and ifnull("U_POStatus",'') ='Closed') then
															 
							select 99,N'Lệnh sản xuất đang ở trạng thái: Closed!'  into  error,  error_message  from dummy;
					end if; 
					
				end;
			 End If;
			 --- Receipt from production
			 If (cast(object_type as nvarchar) = '1250000001') then -- Inventory Transfer Request	
				begin

					if exists(select  "U_POStatus"
										from OWOR 
										where "DocEntry"  = (select "RefDocEntr" 
										from  WTQ21 
										where "ObjectType"  ='1250000001' and "RefObjType"  = '202' 
										and "DocEntry" =:list_of_cols_val_tab_del ) and ifnull("U_POStatus",'') ='Closed') then
															 
							select 100,N'Lệnh sản xuất đang ở trạng thái: Closed!'  into  error,  error_message  from dummy;
					end if; 
					
				end;
			 End If;
			 ---
			-----
			 If (cast(object_type as nvarchar) = '67') then -- Inventory Transfer
				begin

					if exists(select  "U_POStatus"
										from OWOR 
										where "DocEntry"  = (select "RefDocEntr" 
										from  WTR21 
										where "ObjectType"  ='67' and "RefObjType"  = '202' 
										and "DocEntry" =:list_of_cols_val_tab_del ) and ifnull("U_POStatus",'') ='Closed') then
															 
							select 101,N'Lệnh sản xuất đang ở trạng thái: Closed!'  into  error,  error_message  from dummy;
					end if; 
					
				end;
			 End If;
			 ---
		end;
END IF;
---- Chặn ko cho lưu UD Code nếu Result ko có kết quả (Tồn bộ result phải có giá trị)
If (cast(object_type as nvarchar) = 'QAQCResult' and transaction_type  IN ('A','U')) then
		Begin 
			if(select count(*) from "@QC_RESULT_CHAR_L"  a 
								inner join   "@QC_RESULT_H" b on a."DocEntry"  = b."DocEntry"
								where  a."DocEntry" =:list_of_cols_val_tab_del and ifnull(b."U_UDCode",'') <> ''
								        and ifnull("U_QAResult",'') = '')>0 then
								        
				select 102,N'Phải có đầy đủ Result trước khi cập nhật UDCode!'  into  error,  error_message  from dummy;
			end if;
		End;
End If;
---- Có 1 hoặc nhiều kết quả NOT OK bất kỳ -> Không cho UD 001
If (cast(object_type as nvarchar) = 'QAQCResult' and transaction_type  IN ('A','U')) then
		Begin 
			if(select count(*) from "@QC_RESULT_CHAR_L"  a 
								inner join   "@QC_RESULT_H" b on a."DocEntry"  = b."DocEntry"
								where  a."DocEntry" =:list_of_cols_val_tab_del and ifnull(b."U_UDCode",'') = '001'
								        and ifnull("U_QAResult",'') = 'NG')>0 then
								        
				select 103,N'The results do not fully in the norm of specifications!'  into  error,  error_message  from dummy;
			end if;
		End;
End If;
---- Chặn ko cho cập nhật, xóa phiếu Result nếu đã confirm
If (cast(object_type as nvarchar) = 'QAQCResult' and transaction_type   ='D') then
		Begin 
			if(select count(*)  from "@QC_RESULT_LOG" 
					where "Code" =:list_of_cols_val_tab_del 
						  and  ifnull("U_IsConfirm",'') = 'Y')>0 then
								        
				select 104,N'Phiếu không thể delete do phiếu đã confirm!'  into  error,  error_message  from dummy;
			else
			
			
				if exists(select 1 from  "@QC_RESULT_H" where ifnull("U_UserConfirm",'') = '' and "DocEntry" = :list_of_cols_val_tab_del) then
			      Begin
						declare BaseDocType  nvarchar(10);
						declare BaseDocEntry  nvarchar(10);
						
						select "U_BaseDocType"
							 into BaseDocType
							 from  "@QC_RESULT_H" where "DocEntry"  = list_of_cols_val_tab_del ;
							 
						select "U_BaseDocEntry"
							 into BaseDocEntry
							 from  "@QC_RESULT_H" where "DocEntry"  = list_of_cols_val_tab_del ;
						
						
						 If (BaseDocType= '20') then -- Update vao phieu GRPO
						 
						 	update PDN1
						 	set "U_QCDocEntry" = ''
						 	where "DocEntry"  = BaseDocEntry;
						 End If;
						 
						If (BaseDocType= '59') then -- Update vao phieu Goods Receipt
						 
						 	update IGN1
						 	set "U_QCDocEntry" = ''
						 	where "DocEntry"  = BaseDocEntry;
						 End If;
			 
			     End;	
			end if;
			--- Delete "@QC_RESULT_LOG" 
				delete from "@QC_RESULT_LOG"  where "Code" =:list_of_cols_val_tab_del;
			end if;
			
		end;
		
End If;
---- Chặn ko cho cập nhật, xóa phiếu Result nếu đã confirm
If (cast(object_type as nvarchar) = 'QAQCResult' and transaction_type  = 'U') then
		Begin 
			if(select count(*)  from "@QC_RESULT_H" 
					where "DocEntry" =:list_of_cols_val_tab_del 
						  and  ifnull("U_UserConfirm",'') <> '')>0 then
								        
				select 105,N'Phiếu không thể cập nhật do phiếu đã confirm!'  into  error,  error_message  from dummy;
			end if;
		End;
End If;
--- upate code cho form 05.Plan
if(object_type in ('QAQCTemplate') and transaction_type  = 'A') then
	update x set "U_Code" = (
	select 
	case when (select count(1) from "@QC_TEMPLATE_H") + 1 > 99 then 'Q' || (select count(1) + 1 from "@QC_TEMPLATE_H")
	     when (select count(1) from "@QC_TEMPLATE_H") + 1 <= 99 and (select count(1) from "@QC_TEMPLATE_H") + 1 > 9 then 'Q0' ||  (select count(1) + 1 from "@QC_TEMPLATE_H")
	     when (select count(1) from "@QC_TEMPLATE_H") + 1 <= 9 then 'Q00' ||  (select count(1) + 1 from "@QC_TEMPLATE_H")
	end as "Code" 
	from dummy
	)
	from "@QC_TEMPLATE_H" x where "DocEntry" =:list_of_cols_val_tab_del  ;
end if;
---- Update thong tin QC Confirm

IF (cast(object_type as nvarchar)  = '67' and transaction_type  in ( 'A')) then -- 67:Inventory Transfer
		Begin 
			if exists(select 1 from OWTR 
								where ifnull("U_QCEntry_Confirmed",'') <>'' 
									and "DocEntry" = :list_of_cols_val_tab_del) then
				Update  A
				set "U_UserConfirm" = B."USER_CODE" , "U_ConfirmDate" = CURRENT_TIMESTAMP
				from  "@QC_RESULT_H" A
				inner join (select "U_QCEntry_Confirmed", "USER_CODE"
								from OWTR b
								inner join OUSR c on b."UserSign"  =  c."USERID" 
								where  b."DocEntry" = :list_of_cols_val_tab_del) B on B."U_QCEntry_Confirmed" = A."DocEntry"
								
								
				where  "DocEntry" = (select  "U_QCEntry_Confirmed" from OWTR where "DocEntry" = :list_of_cols_val_tab_del  );
			
			
			end if;
		End;
END IF;

---
-- Update thông tin Plan No (phiếu bảo trì bảo dưỡng  - kiểm định)

If (cast(object_type as nvarchar) = 'MaintenManageme' and transaction_type  = 'A') then
		Begin 
			 --
				declare Activity_Type nvarchar(30);
				declare DocType nvarchar(30);
				declare PlanNo_New varchar(11);
				
				
				select  "U_ActivityType","U_DocType"   into Activity_Type, DocType 
				from "@MAINTENANCE_H" where  "DocEntry"  = :list_of_cols_val_tab_del;
				
				Tbl_temp = select   "U_PlanNo"  as "PlanNo_Current"
				from "@MAINTENANCE_H" a
				where "U_StartDate" = (select max("U_StartDate") from "@MAINTENANCE_H" b
										 where a."U_ItemCodeF" = b."U_ItemCodeF"
										 		 and  b."U_Floc" = a."U_Floc" 
										 		 and  b."U_ActivityType" = a."U_ActivityType"
										 		 and  b."U_DocType" = :DocType
										 		 and "DocEntry"  <> :list_of_cols_val_tab_del
									   ) 
					
						and a."U_DocType" = :DocType
						and a."U_ActivityType" = :Activity_Type
						and "DocEntry"  <> :list_of_cols_val_tab_del
						and ifnull("U_Floc",'')  <> '';
---
				IF Exists (select 1 from :Tbl_temp where  ifnull("PlanNo_Current",'') <> '' and length("PlanNo_Current") =11) then --  
					
					 
					select right(:Activity_Type,3)  || REPLICATE(0, 8 - (LENGTH(MAX(Right ("PlanNo_Current",8))+1)))  || cast(max(Right ("PlanNo_Current",8))+1  as varchar)
					into  PlanNo_New
					from :Tbl_temp;

					
				End IF;
			--
					--- Update thong tin
					Update "@MAINTENANCE_H"
					set "U_PlanNo"  = case when ifnull(PlanNo_New,'') = '' then right(Activity_Type,3)||'00000001' else  PlanNo_New end
					where "DocEntry" = :list_of_cols_val_tab_del;
		End;
End If;
-- Chặn trùng mã số thuế
if(object_type = '2'  and transaction_type  in ('A')) then
	 if exists(select 1 from OCRD where ifnull("LicTradNum",'') <> '' and "CardCode" <> :list_of_cols_val_tab_del
	 				and "LicTradNum" = (select "LicTradNum" from OCRD where "CardCode" = :list_of_cols_val_tab_del)
	 				and "CardType" = (select "CardType" from OCRD where "CardCode" = :list_of_cols_val_tab_del)
	 				
	 				) then
			select 106,N'Federal Tax ID đã tồn tại!'  into  error,  error_message  from dummy;
	end if;

end if;
---
--- Update ngay san xuat, ngay het han (tu batch - Outsourcing)
If (cast(object_type as nvarchar) = '59' and transaction_type in ('A')) then -- Goods receipt
	 if exists(select 1 from OIGN where "DocEntry"  = :list_of_cols_val_tab_del and "JrnlMemo"  = 'Apz Outsourcing') then 
	 Begin 
	 		 Update  a
			 set a."ExpDate" = b."ExpDate",
			 	 a."MnfDate"= b."MnfDate",
			 	 a."U_CreatedTime" = b."U_CreatedTime"
			 	 -- TO_VARCHAR(TO_TIME(CURRENT_TIMESTAMP), 'HH24MI')
			 from  OBTN a
			 inner join OBTN b on a."U_ORIBatch" = b."DistNumber"
			 where ifnull (a."U_ORIBatch",'') <> '' and  ifnull(a."ExpDate",'') ='' and  ifnull(a."MnfDate",'') ='';
	 End;
	 
	End if;

end if;
-- Số lượng GRPO <= Số lượng Purchase Order
If (cast(object_type as nvarchar) = '20' and transaction_type in ('A')) then -- Goods Receipt PO
	Begin
		create local temporary table #GRPO
		(
			ItemCode varchar(50),
			Qty numeric (21,6),
			
			PO_Line int,
			DocEntry_PO int,
			Qty_PO numeric (21,6),
			WhsCode nvarchar(10)
		);
---
		insert into #GRPO (ItemCode, Qty,PO_Line,DocEntry_PO, WhsCode)
		select  a."ItemCode", "Quantity", "BaseLine", a."BaseEntry"	, a."WhsCode"							        
		from PDN1 a 
		inner join  OPDN b on a."DocEntry"  =  b."DocEntry"
		where  b."CANCELED"  =  'N' and  b."DocEntry" = :list_of_cols_val_tab_del;
		 
---
		update  a
		set  Qty_PO  = (select sum (b."Quantity") from  POR1 b  
								where b."DocEntry"  = a.DocEntry_PO 
							    	  and a.ItemCode  =  b."ItemCode" 
							    	  and a.PO_Line  = b."LineNum"
							    group by b."ItemCode" )
		from #GRPO a;
---
		if exists (select 1 from #GRPO where Qty > Qty_PO)  then
		  select 107,N'Số lượng GRPO không được lớn hơn PO!'  into  error,  error_message  from dummy;
		end if;	
		----- Chuyển trạng thái mặc định Batch Attribute 2
		/*
			GRPO to K, K-L, K-I: tức là GRPO mà vào các kho này status mặc định là QI
			 (Batch Attribute 2 - OBTN = 'QI ')
		*/
		
		if exists (select 1 from #GRPO where WhsCode  in('K','K-L','K-I'))  then
		   	update a
		   	set "LotNumber"  = 'QI'
		   	from OBTN a
		   	where a."DistNumber" in (select "BatchNum" from IBT1 
		   								where "BaseType"  = 20 
		   										and "BaseEntry" = :list_of_cols_val_tab_del
										and "WhsCode"  in ('K','K-L','K-I'));
		end if;	
		----- 
		----- Chuyển trạng thái mặc định Batch Attribute 2
		/*
			GRPO to A, A-L, A-I or B, B-L, B-I: 
			Tức là GRPO vào các kho này status mặc định là RELEASE (Batch Attribute 2 - OBTN = 'RELEASE ')
		*/
		
		if exists (select 1 from #GRPO where WhsCode  in('A','A-L','A-I','B','B-L','B-I'))  then
		   	update a
		   	set "LotNumber"  = 'RELEASE'
		   	from OBTN a
		   	where a."DistNumber" in (select "BatchNum" from IBT1 
		   								where "BaseType"  = 20 
		   										and "BaseEntry" = :list_of_cols_val_tab_del
										and "WhsCode"  in ('A','A-L','A-I','B','B-L','B-I'));
		end if;	
		----- 
		


		drop table  #GRPO;
		end; 
End IF;
--
---chan khong cho luu khi ty gia quy doi >= 30,000
If (cast(object_type as nvarchar) = '13' and  transaction_type  in ( 'A','U')) then -- A/R Invoice
				begin
					if (select count(*) from OINV where "U_DocRate" >= 30000 and "DocEntry" = list_of_cols_val_tab_del)>0 then
						select 92,N'Tỷ giá không cho phép vượt quá 30,000'   into  error,  error_message  from dummy;
					end if;
				end;
				
End If;
---==> Update trạng thái lệnh rã cho SSCC: Khi SSCC làm phiếu nhập thành phẩm (của lệnh rã)
IF ((cast(object_type as nvarchar)  = '60')  and transaction_type  in ( 'A') )then
	If exists (select 1 from IBT1 where "BsDocType"  = 202 and  "BaseEntry" = :list_of_cols_val_tab_del) then
	
		

	
		-- Lay DocEntry cua lenh san xuat: lenh Ra
		if exists (select 1 from OWOR where "DocEntry"  = (select top 1 "BsDocEntry"
													from  IBT1 
													where "BaseEntry" = :list_of_cols_val_tab_del 
														and "BaseType" = 60
														and  "BsDocType" = 202  )  and "Type" = 'D') then 
									
									 
		update a
		set "U_Dis_Status"  = 'Y'
		from OBTN a
		inner join IBT1 b on a."DistNumber"  = b."BatchNum" 
						and b."BaseEntry" = :list_of_cols_val_tab_del -- DocEntry phieu receip from production
						and b."BaseType" = 60
						and b."BsDocType"  =  202;
		
		end if;


	End If;
	
End IF;
--
---==> Update số lần vào UDO Counting (đây chính là số lần hiện tại của Counting Locked)
IF ((cast(object_type as nvarchar)  = 'INVENTORYCOUNTING')  and transaction_type   = 'A' )then
	update  "@INVENTORYCOUNTING_H"
	set "U_Slan" = (select max("U_SoLan") 
						from "@COUNTINGLOCKED" 
							where "U_Period"  = (select "U_PeriodCode" from  "@INVENTORYCOUNTING_H" 
													where "DocEntry"  =:list_of_cols_val_tab_del)
													
								   and ifnull("Canceled",'')  <> 'Y') 
	where "DocEntry"  = list_of_cols_val_tab_del;
END IF;
---==> Counting Locked: Canceled
IF ((cast(object_type as nvarchar)  = 'CountingLocked')  and transaction_type  = 'C' )then
 update a
 set  Canceled = 'Y'
 from  TBL_COUNTING_SYSQTY a
 where CountingLockedEntry = :list_of_cols_val_tab_del;
End IF;
---==> Counting Locked
IF ((cast(object_type as nvarchar)  = 'CountingLocked')  and transaction_type  in ( 'A','U') )then
		if exists (select 1  from "@COUNTINGLOCKED" where "U_Period"  = (select "U_Period" from  "@COUNTINGLOCKED" 
																			where "DocEntry" =:list_of_cols_val_tab_del)
															and   "U_SoLan" = (select "U_SoLan" from  "@COUNTINGLOCKED" 
																			where "DocEntry" =:list_of_cols_val_tab_del)
															and "DocEntry"  <> :list_of_cols_val_tab_del
															and ifnull("Canceled",'') <> 'Y' )  then
				select 108,N'Số lần mở kỳ đã tồn tại!'  into  error,  error_message  from dummy;
			 
			end if;
	If(transaction_type  = 'A') then
		--- Lưu dữ liệu Qty in system tại thời điểm add phiếu
			----	if not exists(select 1 from TBL_Counting_SysQty 
								----	where CreatedDate || CreatedTime  =
										   
											---TO_VARCHAR(CURRENT_TIMESTAMP,'DDMMYYYY')||TO_VARCHAR(CURRENT_TIMESTAMP,'HH24MI')) then 
											
				insert into TBL_Counting_SysQty (ItemCode,ItemName,UOM,SSCC, Bin, WhsCode,Qty,CreatedDate,CreatedTime, USERADD, 
												 Period, SoLan, CountingLockedEntry )
				select  
				a."ItemCode",
				d."ItemName",
				d."InvntryUom",
				b."DistNumber" as "SSCC",
				c."BinCode",
				a."WhsCode",
				a."OnHandQty" as "Qty",
				TO_VARCHAR(CURRENT_TIMESTAMP,'DDMMYYYY') as "DocDate",
				TO_VARCHAR(CURRENT_TIMESTAMP,'HH24MI') as "DocTime",
				(select "Creator" from "@COUNTINGLOCKED" where "DocEntry" = :list_of_cols_val_tab_del),
				--TO_VARCHAR(CURRENT_TIMESTAMP,'YYYY-MM') as  "Period",
				(select "U_Period" from "@COUNTINGLOCKED" where "DocEntry" = :list_of_cols_val_tab_del),
				(select "U_SoLan" from "@COUNTINGLOCKED" where "DocEntry" = :list_of_cols_val_tab_del),
				:list_of_cols_val_tab_del as "CountingLockedEntry"
				from OBBQ a
				inner join OBTN b on a."SnBMDAbs"  = b."AbsEntry"
				inner join OBIN c on a."BinAbs"  = c."AbsEntry"
				inner join OITM d on a."ItemCode"  = d."ItemCode"
				where  a."OnHandQty"  >0;
			
			-----end if;
			---
		 
			--- Khi trạng thái là yes. thì update các trạng thái bên functionlock thành Y
			If exists (select 1 from  "@COUNTINGLOCKED" where "DocEntry" = :list_of_cols_val_tab_del 
						and "U_StatusLock"  =  'Y' and  transaction_type = 'A' ) then 
				update "@FUNCTIONLOCKED"
				set --"U_ARInvoice" = 'Y',
					--"U_ARCreditMemo"= 'Y',
					"U_Delivery"= 'Y',
					"U_Returns"= 'Y',
					--"U_APInvoice"= 'Y',
					--"U_APCreditMemo"= 'Y',
					"U_GoodsReceiptPO"= 'Y',
					"U_GoodsReturn"= 'Y',
					"U_GoodsReceipt"= 'Y',
					"U_GoodsIssue"= 'Y',
					"U_InventoryTransfer"= 'Y'
					--"U_LandedCosts"= 'Y'
				where "U_Period"  =  (select "U_Period" from  "@COUNTINGLOCKED" where "DocEntry" = :list_of_cols_val_tab_del );
 
			End If;
			---
	End If;
	
	If(transaction_type  = 'U' ) then
		--- Khi trạng thái là yes. thì update các trạng thái bên functionlock thành Y
			If exists (select 1 from  "@COUNTINGLOCKED" where "DocEntry" = :list_of_cols_val_tab_del and "U_StatusLock"  =  'Y') then 
				update "@FUNCTIONLOCKED"
				set --"U_ARInvoice" = 'Y',
					--"U_ARCreditMemo"= 'Y',
					"U_Delivery"= 'Y',
					"U_Returns"= 'Y',
					--"U_APInvoice"= 'Y',
					--"U_APCreditMemo"= 'Y',
					"U_GoodsReceiptPO"= 'Y',
					"U_GoodsReturn"= 'Y',
					"U_GoodsReceipt"= 'Y',
					"U_GoodsIssue"= 'Y',
					"U_InventoryTransfer"= 'Y'
					--"U_LandedCosts"= 'Y'
				where "U_Period"  =  (select "U_Period" from  "@COUNTINGLOCKED" where "DocEntry" = :list_of_cols_val_tab_del );
 			-- update Counting Locked voi Status  = N, "U_StatusLock"  =  'N' --> Mo khoa cac nghiep vu
 			ElseIf exists (select 1 from  "@FUNCTIONLOCKED" 
 									where "U_ARInvoice" = 'Y' 
 											and "U_Period"  
 											= (select "U_Period" from  "@COUNTINGLOCKED" 
 																	where "DocEntry" = :list_of_cols_val_tab_del )) then 
				update "@FUNCTIONLOCKED"
				set --"U_ARInvoice" = 'N',
					--"U_ARCreditMemo"= 'N',
					"U_Delivery"= 'N',
					"U_Returns"= 'N',
					--"U_APInvoice"= 'N',
					--"U_APCreditMemo"= 'N',
					"U_GoodsReceiptPO"= 'N',
					"U_GoodsReturn"= 'N',
					"U_GoodsReceipt"= 'N',
					"U_GoodsIssue"= 'N',
					"U_InventoryTransfer"= 'N'
					--"U_LandedCosts"= 'N'
				where "U_Period"  =  (select "U_Period" from  "@COUNTINGLOCKED" 
										where "DocEntry" = :list_of_cols_val_tab_del );
 			
 			
 			
			End If;
			
			
			
			
	End If;
 

----
End IF;
--------------------------
 
 
 -- Update Production Order: ktra quyen luu PO Stauts 
IF ((cast(object_type as nvarchar)  = '202')  and transaction_type  in ( 'U') )then
	declare  Postatus_Current varchar(15); -- Status PO đang lưu dữ liệu
	declare  Postatus_Before varchar(15); -- Status PO  trước khi thay đổi
	declare  userlist nvarchar(100);
	declare  dem smallint = 0;
	declare  userUpdated nvarchar(10);

	select "U_POStatus" into Postatus_Current from OWOR where "DocEntry"  = :list_of_cols_val_tab_del;

	
	select top 1 POStatus into Postatus_Before from ProductionOrder_ChangedStatus 
					where POEntry = :list_of_cols_val_tab_del
						 order by ActionDate   desc;
	 
	If (  ifnull(Postatus_Current,'') <> '' and  ifnull(Postatus_Before,'') <>  ifnull(Postatus_Current,'')) then
		--========== Postatus_Current:Completed
		if(upper(Postatus_Current) = 'COMPLETED' ) then
			if (  upper(Postatus_Before) = 'CLOSED' ) then
					select 107,N'Không thể cập nhật status Completed khi PO đã closed!'  into  error,  error_message  from dummy;
			 
				elseif exists (select "U_Value2" from "@OTHERSETINGS" where   "Code"  = 'ProStatus_Completed') then
				
				 	select "U_Value2"
				 	into userlist
				 	from  "@OTHERSETINGS"
				 	where   "Code"  = 'ProStatus_Completed';
				 	
				 	userlist = replace(replace (userlist,'[',''''),']','''');
					select "USER_CODE"
					into  userUpdated
					from  OWOR a
					inner join OUSR b on a."UserSign2"  =  b."USERID"
					where a."DocEntry" = list_of_cols_val_tab_del ;
					
					EXECUTE IMMEDIATE 'select  count(*) from  OUSR where "USER_CODE"  =' ||''''|| :userUpdated ||''''|| ' and  "USER_CODE" not in (' || :userlist || ')'
					into dem ;
					if(dem )>0 then
							select 107,N'User không có quyền cập nhật status: Completed'  into  error,  error_message  from dummy;
					else -- User nằm trong danh sách thiết lập
						Insert into  ProductionOrder_ChangedStatus  (POEntry, PONum,Action,ActionDate,UserAction,POStatus,Sapstatus,SapType )
						select  a."DocEntry", a."DocNum",'U',CURRENT_TIMESTAMP,b."USER_CODE", a."U_POStatus",a."Status", a."Type"
						from OWOR a
						inner join OUSR b on a."UserSign2"  =  b."USERID"
						where "DocEntry"  = :list_of_cols_val_tab_del;
					end if;
				 	
				end if;
		end if;

	--========== Postatus_Current:Released
	if(upper(Postatus_Current) = 'RELEASED') then
				if exists (select "U_Value2" from "@OTHERSETINGS" where   "Code"  = 'ProStatus_Released') then
				
				 	select "U_Value2"
				 	into userlist
				 	from  "@OTHERSETINGS"
				 	where   "Code"  = 'ProStatus_Released';
				 	
				 	userlist = replace(replace (userlist,'[',''''),']','''');
					select "USER_CODE"
					into  userUpdated
					from  OWOR a
					inner join OUSR b on a."UserSign2"  =  b."USERID"
					where a."DocEntry" = list_of_cols_val_tab_del ;
					
					EXECUTE IMMEDIATE 'select  count(*) from  OUSR where "USER_CODE"  =' ||''''|| :userUpdated ||''''|| ' and  "USER_CODE" not in (' || :userlist || ')'
					into dem ;
					if(dem )>0 then
							select 107,N'User không có quyền cập nhật status: Released'  into  error,  error_message  from dummy;
					else -- User nằm trong danh sách thiết lập
						Insert into  ProductionOrder_ChangedStatus  (POEntry, PONum,Action,ActionDate,UserAction,POStatus,Sapstatus,SapType )
						select  a."DocEntry", a."DocNum",'U',CURRENT_TIMESTAMP,b."USER_CODE", a."U_POStatus",a."Status", a."Type"
						from OWOR a
						inner join OUSR b on a."UserSign2"  =  b."USERID"
						where "DocEntry"  = :list_of_cols_val_tab_del;
					end if;
				 	
				end if;
		end if;
	--========== Postatus_Current:Closed
	if(upper(Postatus_Current) = 'CLOSED') then
				if exists (select "U_Value2" from "@OTHERSETINGS" where   "Code"  = 'ProStatus_Closed') then
				
				 	select "U_Value2"
				 	into userlist
				 	from  "@OTHERSETINGS"
				 	where   "Code"  = 'ProStatus_Closed';
				 	
				 	userlist = replace(replace (userlist,'[',''''),']','''');
					select "USER_CODE"
					into  userUpdated
					from  OWOR a
					inner join OUSR b on a."UserSign2"  =  b."USERID"
					where a."DocEntry" = list_of_cols_val_tab_del ;
					
					EXECUTE IMMEDIATE 'select  count(*) from  OUSR where "USER_CODE"  =' ||''''|| :userUpdated ||''''|| ' and  "USER_CODE" not in (' || :userlist || ')'
					into dem ;
					if(dem )>0 then
							select 107,N'User không có quyền cập nhật status: Closed'  into  error,  error_message  from dummy;
					else -- User nằm trong danh sách thiết lập
						Insert into  ProductionOrder_ChangedStatus  (POEntry, PONum,Action,ActionDate,UserAction,POStatus,Sapstatus,SapType )
						select  a."DocEntry", a."DocNum",'U',CURRENT_TIMESTAMP,b."USER_CODE", a."U_POStatus",a."Status", a."Type"
						from OWOR a
						inner join OUSR b on a."UserSign2"  =  b."USERID"
						where "DocEntry"  = :list_of_cols_val_tab_del;
					end if;
				 	
				end if;
		end if;
	--========== Ket thuc Postatus_Current:Closed

	End If;

	 
End If;
---------------------------======= Ket Thuc Update Production Order: ktra quyen luu PO Stauts 
-- Insert Production Order: ktra quyen luu PO Status 
IF ((cast(object_type as nvarchar)  = '202')  and transaction_type  in ( 'A') )then
	declare  Postatus_Current varchar(15); -- Status PO đang lưu dữ liệu
	declare  Postatus_Before varchar(15); -- Status PO  trước khi thay đổi
	declare  userlist nvarchar(100);
	declare  dem smallint = 0;
	declare  userUpdated nvarchar(10);

	select "U_POStatus" into Postatus_Current from OWOR where "DocEntry"  = :list_of_cols_val_tab_del;
	
	Insert into  ProductionOrder_ChangedStatus  (POEntry, PONum,Sapstatus,SapType,Action,ActionDate )
	select  "DocEntry", "DocNum","Status", "Type",'A', CURRENT_TIMESTAMP   from OWOR where "DocEntry"  = :list_of_cols_val_tab_del;

	If ( ifnull(Postatus_Current,'') <> '') then
		--========== Postatus_Current:Completed
		if(upper(Postatus_Current) = 'COMPLETED') then
				if exists (select "U_Value2" from "@OTHERSETINGS" where   "Code"  = 'ProStatus_Completed') then
				
				 	select "U_Value2"
				 	into userlist
				 	from  "@OTHERSETINGS"
				 	where   "Code"  = 'ProStatus_Completed';
				 	
				 	userlist = replace(replace (userlist,'[',''''),']','''');
					select "USER_CODE"
					into  userUpdated
					from  OWOR a
					inner join OUSR b on a."UserSign"  =  b."USERID"
					where a."DocEntry" = list_of_cols_val_tab_del ;
					
					EXECUTE IMMEDIATE 'select  count(*) from  OUSR where "USER_CODE"  =' ||''''|| :userUpdated ||''''|| ' and  "USER_CODE" not in (' || :userlist || ')'
					into dem ;
					if(dem )>0 then
							select 108,N'User không có quyền lưu status: Completed'  into  error,  error_message  from dummy;
					else -- User nằm trong danh sách thiết lập
						Insert into  ProductionOrder_ChangedStatus  (POEntry, PONum,Action,ActionDate,UserAction,POStatus,Sapstatus,SapType )
						select  a."DocEntry", a."DocNum",'A',CURRENT_TIMESTAMP,b."USER_CODE", a."U_POStatus",a."Status", a."Type"
						from OWOR a
						inner join OUSR b on a."UserSign2"  =  b."USERID"
						where "DocEntry"  = :list_of_cols_val_tab_del;
					end if;
				 	
				end if;
		end if;

	--========== Postatus_Current:Released
	if(upper(Postatus_Current) = 'RELEASED') then
				if exists (select "U_Value2" from "@OTHERSETINGS" where   "Code"  = 'ProStatus_Released') then
				
				 	select "U_Value2"
				 	into userlist
				 	from  "@OTHERSETINGS"
				 	where   "Code"  = 'ProStatus_Released';
				 	
				 	userlist = replace(replace (userlist,'[',''''),']','''');
					select "USER_CODE"
					into  userUpdated
					from  OWOR a
					inner join OUSR b on a."UserSign"  =  b."USERID"
					where a."DocEntry" = list_of_cols_val_tab_del ;
					
					EXECUTE IMMEDIATE 'select  count(*) from  OUSR where "USER_CODE"  =' ||''''|| :userUpdated ||''''|| ' and  "USER_CODE" not in (' || :userlist || ')'
					into dem ;
					if(dem )>0 then
							select 108,N'User không có quyền lưu status: Released'  into  error,  error_message  from dummy;
					else -- User nằm trong danh sách thiết lập
						Insert into  ProductionOrder_ChangedStatus  (POEntry, PONum,Action,ActionDate,UserAction,POStatus,Sapstatus,SapType )
						select  a."DocEntry", a."DocNum",'A',CURRENT_TIMESTAMP,b."USER_CODE", a."U_POStatus",a."Status", a."Type"
						from OWOR a
						inner join OUSR b on a."UserSign2"  =  b."USERID"
						where "DocEntry"  = :list_of_cols_val_tab_del;
					end if;
				 	
				end if;
		end if;
	--========== Postatus_Current:Closed
	if(upper(Postatus_Current) = 'CLOSED') then
				if exists (select "U_Value2" from "@OTHERSETINGS" where   "Code"  = 'ProStatus_Closed') then
				
				 	select "U_Value2"
				 	into userlist
				 	from  "@OTHERSETINGS"
				 	where   "Code"  = 'ProStatus_Closed';
				 	
				 	userlist = replace(replace (userlist,'[',''''),']','''');
					select "USER_CODE"
					into  userUpdated
					from  OWOR a
					inner join OUSR b on a."UserSign"  =  b."USERID"
					where a."DocEntry" = list_of_cols_val_tab_del ;
					
					EXECUTE IMMEDIATE 'select  count(*) from  OUSR where "USER_CODE"  =' ||''''|| :userUpdated ||''''|| ' and  "USER_CODE" not in (' || :userlist || ')'
					into dem ;
					if(dem )>0 then
							select 107,N'User không có quyền cập nhật status: Closed'  into  error,  error_message  from dummy;
					else -- User nằm trong danh sách thiết lập
						Insert into  ProductionOrder_ChangedStatus  (POEntry, PONum,Action,ActionDate,UserAction,POStatus,Sapstatus,SapType )
						select  a."DocEntry", a."DocNum",'A',CURRENT_TIMESTAMP,b."USER_CODE", a."U_POStatus",a."Status", a."Type"
						from OWOR a
						inner join OUSR b on a."UserSign2"  =  b."USERID"
						where "DocEntry"  = :list_of_cols_val_tab_del;
					end if;
				 	
				end if;
		end if;
	--========== Ket thuc Postatus_Current:Closed

	End If;

	 
End If;
---------------------------======= Ket Thuc Insert Production Order: ktra quyen luu PO Stauts 
 


 
 	---==>Thêm dữ liệu vào bảng vật lý Costing_StandardCost: để truy xuất costing sau này
IF ((cast(object_type as nvarchar)  = '59')  and transaction_type  in ( 'A') )then
	if exists (select 1 from IBT1 where "BsDocType"  = 202 and  "BaseEntry" = :list_of_cols_val_tab_del) then
	
		call ILD_PRODUCTION_COSTING (DocEntry_Receipt=>list_of_cols_val_tab_del);
		

	end if;
	
End IF;
---==>Thêm dữ liệu vào bảng vật lý Costing_StandardCost: để truy xuất costing sau này
IF ((cast(object_type as nvarchar)  = '60')  and transaction_type  in ( 'A') )then
	if exists (select 1 from IBT1 where "BsDocType"  = 202 and  "BaseEntry" = :list_of_cols_val_tab_del) then
	
		if exists (select top 1 *
					from IGE1 b
					inner join OWOR d on d."DocNum"  = b."BaseRef"
					where 	  d."Type" = 'D' --Disassembly
					and b."BaseType"  =  202  and b."DocEntry"  = :list_of_cols_val_tab_del ) then
					
					
			call ILD_PRODUCTION_COSTING_Disassembly (DocEntry=>list_of_cols_val_tab_del);
			
		end if;	
		
		
		

	end if;
	
End IF;
---====================== Truong hop can khoa lai chuc nang luu phieu NHAP, XUAT
/*
IF ((cast(object_type as nvarchar)  = '59')  and transaction_type  in ( 'A') )then
	select 110,N'Hệ thống đang cập nhật!'  into  error,  error_message  from dummy;
End IF;

IF ((cast(object_type as nvarchar)  = '60')  and transaction_type  in ( 'A') )then
	select 110,N'Hệ thống đang cập nhật!'  into  error,  error_message  from dummy;
End IF;
*/
--===========
---==>Chặn update transtype ở Goods issued Khi phiếu đã lưu rồi
IF ((cast(object_type as nvarchar)  = '60')  and transaction_type  in ( 'U') )then
	if exists (select 1 from IGE1 where "BaseType"  <> 202 and  "DocEntry" = :list_of_cols_val_tab_del) then
		 begin
		 		declare TransType_Current nvarchar(20);
		 		Change_Log  = select ifnull("U_TransType",'') as "TransType_Before"
		 		 				from   ADOC
								where "ObjType"  = 60 and "DocEntry" = :list_of_cols_val_tab_del;

				select ifnull("U_TransType",'') into TransType_Current
				from  OIGE where "DocEntry"  = :list_of_cols_val_tab_del;
				if exists ( select 1 from  :Change_Log where "TransType_Before" <> :TransType_Current) then
					select 109,N'Phiếu GI không thể thay đổi transtype'  into  error,  error_message  from dummy;
				end if;
		 end;

 
	end if;
End IF;
-- Select the return values
select :error, :error_message FROM dummy;

end;

