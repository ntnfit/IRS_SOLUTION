CREATE procedure APZ_CTA_TaoRecurringPostings
(
	out ret table ("Code" int, "Msg" Nvarchar(200))
)
as
begin
	declare Code int = 200;
	declare Msg nvarchar(200) ='Success';
	
	Create local temporary table #tempSaLaryIRS( STT smallint,NhomButToan smallint,TaiKhoan  nvarchar(15), 
	 												Debit decimal(21,6),Credit decimal(21,6),
												    BHXH_NLD  decimal(21,6),BHYT_NLD  decimal(21,6),
												    BHTN_NLD  decimal(21,6),
												    
												    KPCD_NLD  decimal(21,6),
												    
												    TNCN_NLD decimal(21,6),
												    THUC_LINH decimal(21,6),
												    
												    BHXH_CTY  decimal(21,6),BHYT_CTY  decimal(21,6),
												    BHTNNLD_CTY  decimal(21,6),BHTN_CTY  decimal(21,6),
												    KPCD_CTY  decimal(21,6),
												    
												    PhongBan nvarchar(20),GhiChu nvarchar(100), 
												    Thang nvarchar(2), Nam nvarchar(4), UserID smallint, UserCode nvarchar(20),
												    CHIPHITHUONG decimal(21,6),
												    CHIPHIOT decimal(21,6),
												    CHIPHIPHUCAP decimal(21,6),
												    COSTCENTER nvarchar(20),
												    CHIPHITHUONGTAM decimal(21,6),
												    DIEUCHINHTANG decimal(21,6),
												    DIEUCHINHGIAM decimal(21,6)
												    );
	
	 exec 'drop table #tempSaLaryIRS';  
	 exec 'create local temporary table #tempSaLaryIRS  as  (select  * from "#tempSaLaryIRS_UI")';
	
----

create local temporary table #tbl  as (select * from #tempSaLaryIRS );
if exists  (select 1 from   #tbl where length(ifnull(Taikhoan,'')) <1) then
			 select 199, 'Tồn tại Tài Khoản Kế Toán <> Cây Tài Khoản!'
			 into Code, Msg
			 from dummy;
			 
 
else 

--
	IF not exists (select "U_KyTinhLuong" 
					from "@SALARYIRSTOSAP" 
					where  "U_Thang" = (select top 1 THANG  from #tempSaLaryIRS) and "U_Nam"  =  (select top 1 NAM  from #tempSaLaryIRS)) then
										
			insert into #tbl (NhomButToan,TaiKhoan,Debit,Credit,GhiChu)
			select NhomButToan,TaiKhoan,sum(Debit),sum(Credit),Max(GhiChu)
			from #tempSaLaryIRS
			group by TaiKhoan,NhomButToan;
		 
		If Exists(Select 1 From #tempSaLaryIRS) Then
			  	declare instan int  = 50;
			  	declare totalDebit decimal(21,6) ;
				declare totalCredit decimal(21,6) ;
	 
				 -----=============== Lưu dữ liệu --> tbl: SALARYIRSTOSAP (dữ liệu lương đã tích hơp sẽ là bảng này) -----===============
			Insert into  "@SALARYIRSTOSAP" ("Code","Name","U_PhongBan","U_CostCenter",
											"U_KyTinhLuong","U_TaiKhoan",
			
											"U_KPCD_NLD","U_TNCN_NLD","U_THUC_LINH",
											"U_BHXH_NLD","U_BHYT_NLD","U_BHTN_NLD",
													
											"U_BHXH_CTY","U_BHYT_CTY","U_BHTNNLD_CTY","U_BHTN_CTY",	"U_KPCD_CTY",
											 "U_CHIPHITHUONG","U_CHIPHIOT","U_CHIPHIPHUCAP","U_DIEUCHINHTANG","U_DIEUCHINHGIAM","U_CHIPHITHUONGTAM",
													
											 "U_NgayTao","U_SAPID","U_Thang","U_Nam","U_STT","U_GHICHU", "U_UserCode","U_NguoiTao","U_TrangThaiTuKeToan")
			
			Select  NhomButToan ||'-'||TAIKHOAN||'-'||PHONGBAN,NhomButToan ||'-'||TAIKHOAN||'-'||PHONGBAN as "Name",PHONGBAN as "PhongBan", PHONGBAN as "CostCenter",
					THANG || '-' || NAM as "KyTinhLuong" ,TAIKHOAN,
					KPCD_NLD,TNCN_NLD,THUC_LINH,
					BHXH_NLD,BHYT_NLD,BHTN_NLD,
					BHXH_CTY,BHYT_CTY,BHTNNLD_CTY,BHTN_CTY,KPCD_CTY,
					CHIPHITHUONG,CHIPHIOT,CHIPHIPHUCAP,DIEUCHINHTANG,DIEUCHINHGIAM,CHIPHITHUONGTAM,
					
					CURRENT_DATE,'Luong Thang: ' ||THANG||'-'||NAM as "SAPID", THANG, NAM, STT -1, GhiChu, UserCode, UserID, N'Kế toán chưa xử lý'
			From #tempSaLaryIRS;
			
			
			----------------- 1: Lưu --> OBTD
			select sum(DEBIT)
			into  totalDebit default 0
			from #tempSaLaryIRS;
		
			---
			select sum(CREDIT)
			into  totalCredit default 0
			from #tempSaLaryIRS;
			 ---- Kiểm tra: nếu Tổng nợ (Debit) và Tổng có (Credit) Không = nhau  --> Không Cân, Không cho Lưu
			if (totalDebit <> totalCredit ) then
					 select 198, 'Tổng Nợ (Debit) và Tổng Có (Credit) Không Bằng Nhau.'
					 into Code, Msg
					 from dummy;
					 
			end if; 
			
			--
				
	
			---
		insert into ORCR("RcurCode","RcurDesc","Frequency","Remind","LastPosted","NextDeu",
						 "EntryCount","Volume","VolCurr","FinancVol","Ref1","Ref2",
						 "TransCode","Memo","LimitRtrns","Returns","LimitDate","Instance",
						 "DataSource","UserSign","StampTax","AutoVat","ManageWTax","Ref3","DeferedTax")
						 
						 
		select THANG||'-'||NAM,'Lương Tháng:' ||THANG||'-'||NAM,'N' as "Frequency",0 ,NULL,CURRENT_DATE as "NextDeu",
				0, totalDebit,'VND',0,NULL,NULL,
				NULL,'Luong Thang: ' ||THANG||'-'||NAM as "Memo",'N',NULL,NULL,1,
				'N',UserID,'N','N','N',NULL,'N'
		From #tempSaLaryIRS a
		where STT  = 1;
-------------------------- RCR1: chi tiết hạch toán
		insert into RCR1 ( "RcurCode","LineId","AcctCode","AcctDesc","Debit","Credit","Currency",
							"Instance","VatGroup","UserSign","VatLine","CtrlAcct","OcrCode","TaxType",
							
							"TaxPostAcc","StaCode","StaType","TaxCode","OcrCode1","OcrCode2","OcrCode3",
							"OcrCode4","OcrCode5","WtLiable","WTaxLine","GrossValue","Project","BPLId","CemCode")
		select THANG||'-'||NAM as "RcurCode", STT -1,TaiKhoan,b."AcctName",Debit, Credit,'VND',
			   1,'',UserID,'N',TaiKhoan,NULL as "OcrCode",0 as "TaxType",
			   'N',NULL,NULL,NULL,NULL,NULL,NULL,
			   PhongBan,NULL,'N','N',0,NULL,NULL,NULL
		From #tempSaLaryIRS a
		left join OACT b on a.TaiKhoan  = b."AcctCode";
		
		
		
			     -----=============== END Save SALARYIRSTOSAP ---- ==============
	
			 
			 
		End If;
		 
	END IF;
END IF;

 
	drop table #tbl;
	 	ret = select Code as "Code",  Msg as "Msg" from dummy ;
	Return;

end;
