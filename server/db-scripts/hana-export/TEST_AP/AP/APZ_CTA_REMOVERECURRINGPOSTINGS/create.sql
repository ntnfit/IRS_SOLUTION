CREATE procedure APZ_CTA_RemoveRecurringPostings
(
	out ret table ("Code" int, "Msg" Nvarchar(200))
)
as
begin
	declare Code int = 200;
	declare Msg nvarchar(200) ='Success';
	

	Create local temporary table #tempSaLaryIRS(	PhongBan nvarchar(20),
	
													BHXH_NLD DECIMAL(21,6),
													BHYT_NLD DECIMAL(21,6),
													BHTN_NLD DECIMAL(21,6),
													KPCD_NLD DECIMAL(21,6),
													TNCN_NLD DECIMAL(21,6),
													THUC_LINH DECIMAL(21,6),
													KhenThuongKhac DECIMAL(21,6),
													
													BHXH_CTY DECIMAL(21,6),
													BHYT_CTY DECIMAL(21,6),
													BHTNNLD_CTY DECIMAL(21,6),
													BHTN_CTY DECIMAL(21,6),
													KPCD_CTY DECIMAL(21,6) ,
													
													NgayTao nvarchar(20),
													UserTao nvarchar(15),
													
													KyTinhLuong nvarchar(20),
													TrangThai  nvarchar(50) );												       
	
	 exec 'drop table #tempSaLaryIRS';  
	 exec 'create local temporary table #tempSaLaryIRS  as  (select  * from "#tempSaLaryIRS_UI")';
	
----

	--create local temporary table #tbl  as (select * from #tempSaLaryIRS );

 

	
IF   exists (select "U_KyTinhLuong" 
				from "@SALARYIRSTOSAP" 
				where   "U_KyTinhLuong" =  (select top 1 KyTinhLuong from  #tempSaLaryIRS) 
				and ("U_TrangThaiTuKeToan" in (N'Kế toán đã hủy phiếu JE','Kế toán chưa xử lý'))) then
									
 declare KyLuong nvarchar(50) ;
 select top 1 KyTinhLuong into KyLuong  from  #tempSaLaryIRS ;
   

 
 delete from RCR1 where "RcurCode" = KyLuong;		 
 delete from ORCR where "RcurCode" = KyLuong;
 delete from "@SALARYIRSTOSAP" where "U_KyTinhLuong"  = KyLuong;
 
		 
END IF;

 
	--drop table #tbl;
	 	ret = select Code as "Code",  Msg as "Msg" from dummy ;
	Return;

end;
