CREATE procedure APZ_CTA_TaoJournalVouchers
(
	out ret table ("Code" int, "Msg" Nvarchar(200))
)
as
begin
	declare autokey int;
	declare Code int = 200;
	declare Msg nvarchar(200) ='Success';
	
	Create local temporary table #tempSaLaryIRS( STT smallint,LINE_ID smallint,NhomButToan smallint,TaiKhoan  nvarchar(15), 
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
	
---- : Journal Vouchers List, Lấy key (hiện tại) để +1 --> Key cho phiếu đang tạo
select max("BatchNum") into autokey from OBTD;


create local temporary table #tbl  as (select * from #tempSaLaryIRS );

 
	
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
		into  totalDebit
		from #tempSaLaryIRS;
	
		---
		select sum(CREDIT)
		into  totalCredit
		from #tempSaLaryIRS;
	--========================================Journal Vouchers List (header)
	insert into OBTD ("BatchNum", "Status", "NumOfTrans", "DateID", "LocTotal", "FcTotal", "SysTotal", "UserSign","Remarks","MemoID")
	select autokey+1,'O',1,Current_date as "DateID",0,:totalDebit as  "FcTotal",0,UserID ,'Lương Tháng:' ||THANG||'-'||NAM,  THANG||'-'||NAM
	from #tempSaLaryIRS
	where STT  = 1;
	--========================================Journal Voucher Entry (header)
	insert into OBTF ("BatchNum", "TransId", "BtfStatus", "TransType", "RefDate", "Memo", "Ref1", "Ref2", "LocTotal", "FcTotal", "SysTotal", "TransCode", "OrignCurr", "TransRate", "Project", "DueDate", "TaxDate", "PCAddition", "FinncPriod", "DataSource", "UpdateDate", "CreateDate"
						, "UserSign", "UserSign2", "RefndRprt", "LogInstanc", "ObjType", "AdjTran", "RevSource", "AutoStorno", "Corisptivi", "StampTax", "Series", "AutoVAT", "BlockDunn", "ReportEU", "Report347", "Printed", "AttNum", "GenRegNo", "AutoWT", "WTSum", "WTSumSC", "WTSumFC"
						, "WTApplied", "WTAppliedS", "WTAppliedF", "BaseAmnt", "BaseAmntSC", "BaseAmntFC", "BaseVtAt", "BaseVtAtSC", "BaseVtAtFC", "VersionNum", "ResidenNum", "Ref3", "DeferedTax", "ECDPosTyp", "PrlLinked", "ExclTaxRep", "IsCoEntry", "U_JVNumber")
	select autokey+1,1,'O',-1,current_date,'Lương Tháng:' ||THANG||'-'||NAM,NULL,NULL,0 as "LocTotal",:totalDebit,0 as "SysTotal",NULL,NULL,0,NULL,current_date,current_date,'N',b2."AbsEntry" FinncPriod,'I' "DataSource",NULL UpdateDate,current_date CreateDate
			,a.UserID, -1, 'N', -1, 30,'N' AdjTran,'N' RevSource,'N' AutoStorno,'N' Corisptivi,'N' StampTax, b1."Series",'N' AutoVAT,'N' BlockDunn,'N' ReportEU,'N' Report347,'N' Printed,0 AttNum,'N' GenRegNo,'N' AutoWT,0 WTSum,0 WTSumSC,0 WTSumFC
			,0 WTApplied,0 WTAppliedS,0 WTAppliedF,0 BaseAmnt,0 BaseAmntSC,0 BaseAmntFC,0 BaseVtAt,0 BaseVtAtSC,0 BaseVtAtFC,'10.00.120.02' VersionNum,1 ResidenNum,'' Ref3,'N' DeferedTax,'N' ECDPosTyp,'N' PrlLinked,'N' ExclTaxRep,'N' IsCoEntry
			,NAM||'-'|| THANG
	from 
		#tempSaLaryIRS a
		inner join (select max("Series") as  "Series" from  NNM1 where "ObjectCode" = '30') b1 on  1= 1
		inner join OFPR b2 on "Code"  = NAM||'-'|| THANG;
		
	--========================================Journal Voucher Entry (Details)
/*
   insert into BTF1 ("TransId", "Line_ID", "Account", "BPLId", "Debit", "Credit", "SYSCred", "SYSDeb", "FCDebit", "FCCredit", "FCCurrency", "DueDate", "ShortName", "IntrnMatch", "ExtrMatch", "LineMemo", "TransType", "RefDate", "Ref1", "Ref2", "Project", "ProfitCode", "TaxDate", "SystemRate", "ToMthSum", "UserSign", "BatchNum", "FinncPriod", "RelTransId", "RelLineID", "RelType", "LogInstanc"
					, "BaseSum", "VatRate", "Indicator", "AdjTran", "RevSource", "ObjType", "SYSBaseSum", "MultMatch", "VatLine", "VatAmount", "SYSVatSum", "Closed", "GrossValue", "LineType", "DebCred", "SequenceNr", "BalDueDeb", "BalDueCred", "BalFcDeb", "BalFcCred", "BalScDeb", "BalScCred", "IsNet", "DunWizBlck", "DunnLevel", "TaxType", "TaxPostAcc", "ValidFrom", "GrossValFc"
				    , "OcrCode2", "OcrCode3", "OcrCode4", "OcrCode5", "MIEntry", "MIVEntry", "ClsInTP", "CenVatCom", "MatType", "PstngType", "ValidFrom2", "ValidFrom3", "ValidFrom4", "ValidFrom5", "EquVatRate", "EquVatSum", "SYSEquSum", "TotalVat", "SYSTVat", "WTLiable", "WTLine", "WTApplied", "WTAppliedS", "WTAppliedF", "WTSum", "WTSumFC", "WTSumSC", "PayBlock", "InterimTyp", "Ordered","ContraAct")
	select
			1 TransId, row_number() over (order by a."DocEntry",b."LineId") "Line_ID",b."U_Account" Account,a."U_Branch",IFNULL(NULLIF(b."U_Debit",0),0) Debit,IFNULL(NULLIF(b."U_Credit",0),0) Credit,IFNULL(NULLIF(b."U_Debit",0),0) SYSDeb,IFNULL(NULLIF(b."U_Credit",0),0) SYSCred,0 FCDebit,0 FCCredit,'' FCCurrency,a."U_Date" DueDate
			,b."U_Account" ShortName,0 IntrnMatch,0 ExtrMatch,b."U_Note" LineMemo,-1 TransType,a."U_Date" RefDate,'' Ref1,'' Ref2
			,NULL Project,b."U_OcrCode" ProfitCode,a."U_Date" TaxDate,0 SystemRate,0 ToMthSum,a."UserSign" UserSign,autokey+1 BatchNum,c."AbsEntry" FinncPriod,-1 RelTransId,-1 RelLineID,'N' RelType,0 LogInstanc,0 BaseSum,0 VatRate,'' Indicator,'N' AdjTran
			,'N' RevSource,'30' ObjType,0 SYSBaseSum,0 MultMatch,'N' VatLine,0 VatAmount,0 SYSVatSum,'N' Closed,0 GrossValue,0 LineType,case when IFNULL(NULLIF(b."U_Debit",0),0) <>0 then 'D' when IFNULL(NULLIF(b."U_Credit",0),0)<>0 then 'C' end DebCred,0 SequenceNr,0 BalDueDeb,0 BalDueCred,0 BalFcDeb,0 BalFcCred
			,0 BalScDeb,0 BalScCred,'Y' IsNet,'N' DunWizBlck,0 DunnLevel,0 TaxType,'N' TaxPostAcc,'1900-01-01' ValidFrom,0 GrossValFc,b."U_OcrCode2" OcrCode2,b."U_OcrCode3" OcrCode3,b."U_OcrCode4" OcrCode4,b."U_OcrCode5" OcrCode5,0 MIEntry,0 MIVEntry,0 ClsInTP,-1 CenVatCom
			,-1 MatType,0 PstngType,'1900-01-01' ValidFrom2,'1900-01-01' ValidFrom3,'1900-01-01' ValidFrom4,'1900-01-01' ValidFrom5,0 EquVatRate,0 EquVatSum,0 SYSEquSum,0 TotalVat,0 SYSTVat,'N' WTLiable,'N' WTLine,0 WTApplied,0 WTAppliedS,0 WTAppliedF
			,0 WTSum,0 WTSumFC,0 WTSumSC,'N' PayBlock,0 InterimTyp,'N' Ordered
			,(select max(x."U_Account") from "@UJV1" x where x."DocEntry"=b."DocEntry" and x."U_LineId"=b."U_LineId" and x."U_OcrCode4"=b."U_OcrCode4" and x."U_Account"<>b."U_Account") ContraAct
	from 
		"@OUJV" a
		inner join "@UJV1" b on a."DocEntry" = b."DocEntry"
		inner join OFPR c on on "Code"  = NAM||'-'|| THANG;
	where
		a."DocEntry" = :docentry
	order by a."DocEntry",b."LineId";
*/
		
   insert into BTF1 ("TransId", "Line_ID", "Account", "BPLId", "Debit", "Credit", "SYSCred", "SYSDeb", "FCDebit", "FCCredit", "FCCurrency", "DueDate", 
   				"ShortName", "IntrnMatch", "ExtrMatch", "LineMemo", "TransType", "RefDate", "Ref1", "Ref2", 
   				"Project", "ProfitCode", "TaxDate", "SystemRate", "ToMthSum", "UserSign", "BatchNum", "FinncPriod", "RelTransId", "RelLineID", "RelType", "LogInstanc", "BaseSum", "VatRate", "Indicator", "AdjTran"
					, "RevSource", "ObjType", "SYSBaseSum", "MultMatch", "VatLine", "VatAmount", "SYSVatSum", "Closed", "GrossValue", "LineType", "DebCred", "SequenceNr", "BalDueDeb", "BalDueCred", "BalFcDeb", "BalFcCred", "BalScDeb", "BalScCred", 
					"IsNet", "DunWizBlck", "DunnLevel", "TaxType", "TaxPostAcc", "ValidFrom", "GrossValFc" , "OcrCode2", "OcrCode3", "OcrCode4", "OcrCode5", "MIEntry", "MIVEntry", "ClsInTP", "CenVatCom",
				     "MatType", "PstngType", "ValidFrom2", "ValidFrom3", "ValidFrom4", "ValidFrom5", "EquVatRate", "EquVatSum", "SYSEquSum", "TotalVat", "SYSTVat", "WTLiable", "WTLine",
				      "WTApplied", "WTAppliedS", "WTAppliedF", 
				      "WTSum", "WTSumFC", "WTSumSC", "PayBlock", "InterimTyp", "Ordered","ContraAct")
	select
			1 TransId, STT -1 as "Line_ID",a.taikhoan as Account,NULL as BPLId,IFNULL(NULLIF(a.Debit,0),0) Debit,IFNULL(NULLIF(a.Credit,0),0) Credit,0 SYSDeb,0 SYSCred,IFNULL(NULLIF(a.Debit,0),0) FCDebit,IFNULL(NULLIF(a.Credit,0),0) FCCredit,'VND' FCCurrency,Current_date DueDate
			,a.TaiKhoan as ShortName,0 IntrnMatch,0 ExtrMatch,a.GHICHU LineMemo,-1 TransType,current_date RefDate,'' Ref1,'' Ref2
			,NULL Project,a.PhongBan as ProfitCode,current_date TaxDate,0 SystemRate,0 ToMthSum,a.userID  as UserSign,autokey+1 BatchNum,c."AbsEntry" FinncPriod,-1 RelTransId,-1 RelLineID,'N' RelType,0 LogInstanc,0 BaseSum,0 VatRate,'' Indicator,'N' AdjTran
			,'N' RevSource,'30' ObjType,0 SYSBaseSum,0 MultMatch,'N' VatLine,0 VatAmount,0 SYSVatSum,'N' Closed,0 GrossValue,0 LineType,case when IFNULL(NULLIF(a.debit,0),0) <>0 then 'D' when IFNULL(NULLIF(a.credit,0),0)<>0 then 'C' end DebCred,0 SequenceNr,0 BalDueDeb,0 BalDueCred,0 BalFcDeb,0 BalFcCred,0 BalScDeb,0 BalScCred,
			
			'Y' IsNet,'N' DunWizBlck,0 DunnLevel,0 TaxType,'N' TaxPostAcc,'1900-01-01' ValidFrom,0 GrossValFc, NULL OcrCode2,NULL OcrCode3,NULL OcrCode4,NULL  OcrCode5,0 MIEntry,0 MIVEntry,0 ClsInTP,-1 CenVatCom
			
			,-1 MatType,0 PstngType,'1900-01-01' ValidFrom2,'1900-01-01' ValidFrom3,'1900-01-01' ValidFrom4,'1900-01-01' ValidFrom5,0 EquVatRate,0 EquVatSum,0 SYSEquSum,0 TotalVat,0 SYSTVat,'N' WTLiable,'N' WTLine,
			0 WTApplied,0 WTAppliedS,0 WTAppliedF
			,0 WTSum,0 WTSumFC,0 WTSumSC,'N' PayBlock,0 InterimTyp,'N' Ordered
			,(select max(x.TaiKhoan) from #tempSaLaryIRS x where x.NAM||THANG= a.NAM||THANG 
						and x.NHOMBUTTOAN=a.NHOMBUTTOAN and x.phongban= a.phongban) ContraAct
	from #tempSaLaryIRS a
	inner join OFPR c on  "Code"  = NAM||'-'|| THANG
	order by a.STT;
	
	
	-----
	update ONNM set "AutoKey" = autokey + 2 where "ObjectCode" = '29';
		
 
	/*
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
	
	*/
	
		     -----=============== END Save SALARYIRSTOSAP ---- ==============
		 ---- Kiểm tra: nếu Tổng nợ (Debit) và Tổng có (Credit) Không = nhau  --> Không Cân, Không cho Lưu
			if (totalDebit <> totalCredit ) then
				 select 199, 'Tổng Nợ (Debit) và Tổng Có (Credit) Không Bằng Nhau.'
				 into Code, Msg
				 from dummy;
				 
			end if; 
		 -----------------------------------------------------------------------------------
		 
	End If;
	 
END IF;
 

 
	drop table #tbl;
	 	ret = select Code as "Code",  Msg as "Msg" from dummy ;
	Return;

end;
