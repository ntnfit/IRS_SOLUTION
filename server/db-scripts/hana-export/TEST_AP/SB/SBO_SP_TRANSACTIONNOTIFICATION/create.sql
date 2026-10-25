CREATE PROCEDURE SBO_SP_TransactionNotification
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
--- Kiem tra flow theo AP, AR: Phải đảm bảo phiếu được kế thừa trước khi kiểm tra các thành phần khác.
	if (transaction_type in ('A') ) then
		call USP_CHECKFLOW_APAR(object_type,list_of_cols_val_tab_del,error,error_message);
		if error != 0 then
			select :error, :error_message from dummy;			
		end if;
	end if;
-- Update FromDate/ToDate vào màn hình Lock nghiệp vụ-------------------------------------------
	if :object_type='FunctionLocked' and :transaction_type in ('A','U') then
		update x set "U_FromDate" = y."F_RefDate", "U_ToDate" = y."T_RefDate" from "@FUNCTIONLOCKED" x inner join OFPR y on y."Code"=x."U_Period" and x."DocEntry"=:list_of_cols_val_tab_del;
	end if;
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

--------------------------------------------------------- DOAN CODE BAT BUOC CO----------------------------------------------------------------------------------------------------
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

-- bat buoc nhap so to khai
if (cast(object_type as nvarchar) = '20' and transaction_type in ('A','U')) then
	if (select count(*) from OPDN where "DocEntry" = list_of_cols_val_tab_del and ifnull("U_SoToKhai",'')='') >0 then
		select 20,N'Số tờ khai không được để trống!'  into  error,  error_message  from dummy;
	end if;
end if;
--- Function Locked================================================================================================================================
	if (:object_type in ('13') and :transaction_type in ('A','U')) then
		if (select count(1) from OINV x, "@FUNCTIONLOCKED" y where x."DocEntry"=:list_of_cols_val_tab_del and x."DocDate" between y."U_FromDate" and y."U_ToDate" and y."U_ARInvoice"='Y')>0 then
			select 68,N'Nghiệp vụ đã bị khóa!'  into  error,  error_message  from dummy;
		end if;
	end if;
	if (:object_type in ('14') and :transaction_type in ('A','U')) then
		if (select count(1) from ORIN x, "@FUNCTIONLOCKED" y where x."DocEntry"=:list_of_cols_val_tab_del and x."DocDate" between y."U_FromDate" and y."U_ToDate" and y."U_ARCreditMemo"='Y')>0 then
			select 68,N'Nghiệp vụ đã bị khóa!'  into  error,  error_message  from dummy;
		end if;
	end if;
	if (:object_type in ('15') and :transaction_type in ('A','U')) then
		if (select count(1) from ODLN x, "@FUNCTIONLOCKED" y where x."DocEntry"=:list_of_cols_val_tab_del and x."DocDate" between y."U_FromDate" and y."U_ToDate" and y."U_Delivery"='Y')>0 then
			select 68,N'Nghiệp vụ đã bị khóa!'  into  error,  error_message  from dummy;
		end if;
	end if;
	if (:object_type in ('16') and :transaction_type in ('A','U')) then
		if (select count(1) from ORDN x, "@FUNCTIONLOCKED" y where x."DocEntry"=:list_of_cols_val_tab_del and x."DocDate" between y."U_FromDate" and y."U_ToDate" and y."U_Returns"='Y')>0 then
			select 68,N'Nghiệp vụ đã bị khóa!'  into  error,  error_message  from dummy;
		end if;
	end if;
	if (:object_type in ('18') and :transaction_type in ('A','U')) then
		if (select count(1) from OPCH x, "@FUNCTIONLOCKED" y where x."DocEntry"=:list_of_cols_val_tab_del and x."DocDate" between y."U_FromDate" and y."U_ToDate" and y."U_APInvoice"='Y')>0 then
			select 68,N'Nghiệp vụ đã bị khóa!'  into  error,  error_message  from dummy;
		end if;
	end if;
	if (:object_type in ('19') and :transaction_type in ('A','U')) then
		if (select count(1) from ORPC x, "@FUNCTIONLOCKED" y where x."DocEntry"=:list_of_cols_val_tab_del and x."DocDate" between y."U_FromDate" and y."U_ToDate" and y."U_APCreditMemo"='Y')>0 then
			select 68,N'Nghiệp vụ đã bị khóa!'  into  error,  error_message  from dummy;
		end if;
	end if;
	if (:object_type in ('20') and :transaction_type in ('A','U')) then
		if (select count(1) from OPDN x, "@FUNCTIONLOCKED" y where x."DocEntry"=:list_of_cols_val_tab_del and x."DocDate" between y."U_FromDate" and y."U_ToDate" and y."U_GoodsReceiptPO"='Y')>0 then
			select 68,N'Nghiệp vụ đã bị khóa!'  into  error,  error_message  from dummy;
		end if;
	end if;
	if (:object_type in ('21') and :transaction_type in ('A','U')) then
		if (select count(1) from ORPD x, "@FUNCTIONLOCKED" y where x."DocEntry"=:list_of_cols_val_tab_del and x."DocDate" between y."U_FromDate" and y."U_ToDate" and y."U_GoodsReturn"='Y')>0 then
			select 68,N'Nghiệp vụ đã bị khóa!'  into  error,  error_message  from dummy;
		end if;
	end if;
	if (:object_type in ('59') and :transaction_type in ('A','U')) then
		if (select count(1) from OIGN x, "@FUNCTIONLOCKED" y where x."DocEntry"=:list_of_cols_val_tab_del and x."DocDate" between y."U_FromDate" and y."U_ToDate" and y."U_GoodsReceipt"='Y')>0 then
			select 68,N'Nghiệp vụ đã bị khóa!'  into  error,  error_message  from dummy;
		end if;
	end if;
	if (:object_type in ('60') and :transaction_type in ('A','U')) then
		if (select count(1) from OIGE x, "@FUNCTIONLOCKED" y where x."DocEntry"=:list_of_cols_val_tab_del and x."DocDate" between y."U_FromDate" and y."U_ToDate" and y."U_GoodsIssue"='Y')>0 then
			select 68,N'Nghiệp vụ đã bị khóa!'  into  error,  error_message  from dummy;
		end if;
	end if;
	if (:object_type in ('67') and :transaction_type in ('A','U')) then
		declare U_ContNo nvarchar(100);	--So xe bat buoc nhap khi U_TransferType = 2
		declare CardCode nvarchar(100);	--Mã BP bat buoc nhap khi U_TransferType = 2
		declare U_TransferType nvarchar(100); --Khi U_TransferType = 2 thi neu chua khai bao ty gia VND theo ngay Posting date thi khong cho luu phieu transfer
		declare PostingDate date;
		declare RateExchange decimal(21, 6) = 0;

		select "CardCode", "U_ContNo", "U_TransferType", "DocDate" into CardCode, U_ContNo, U_TransferType, PostingDate default '', '', '', '1900-01-01' from OWTR where "DocEntry" = list_of_cols_val_tab_del;
		select top 1 "Rate" into RateExchange default 0 from ORTT where "Currency" = 'VND' and "RateDate" = :PostingDate;
		
		if (select count(1) from OWTR x, "@FUNCTIONLOCKED" y where x."DocEntry"=:list_of_cols_val_tab_del and x."DocDate" between y."U_FromDate" and y."U_ToDate" and y."U_InventoryTransfer"='Y')>0 then
			select 68,N'Nghiệp vụ đã bị khóa!'  into  error,  error_message  from dummy;
		end if;
	
		if (ifnull(U_TransferType,'') = '2') then
			if (ifnull(CardCode,'') = '') then
				select 67,N'Vui lòng chọn Business Partner !' from dummy;
				return;
			end if;
			if (ifnull(U_ContNo,'') = '') then
				select 67,N'Vui lòng nhập số xe !' from dummy;
				return;
			end if;
			if (ifnull(RateExchange,0) = 0) then
				select 67,N'Tỷ giá quy đổi USD sang VND ngày ' || TO_VARCHAR(:PostingDate, 'DD-MM-YYYY') || ' chưa khai báo !' from dummy;
				return;
			end if;
			
		end if;
		
	end if;
	if (:object_type in ('69') and :transaction_type in ('A','U')) then
		if (select count(1) from OIPF x, "@FUNCTIONLOCKED" y where x."DocEntry"=:list_of_cols_val_tab_del and x."DocDate" between y."U_FromDate" and y."U_ToDate" and y."U_LandedCosts"='Y')>0 then
			select 68,N'Nghiệp vụ đã bị khóa!'  into  error,  error_message  from dummy;
		end if;
	end if;
	-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
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
				update "@QC_RESULT_H" set "U_UserConfirm"  = NULL, "U_ConfirmDate" = NULL
				where "DocEntry"  = list_of_cols_val_tab_del ;
			end if;

			select "U_BaseDocType" into BaseDocType from  "@QC_RESULT_H" where "DocEntry"  = list_of_cols_val_tab_del ;
			select "U_BaseDocEntry" into BaseDocEntry from  "@QC_RESULT_H" where "DocEntry"  = list_of_cols_val_tab_del ;

			 If (BaseDocType= '20') then -- Update vao phieu GRPO
			 	update PDN1 set "U_QCDocEntry" = list_of_cols_val_tab_del where "DocEntry"  = BaseDocEntry;
			 End If;
			If (BaseDocType= '59') then -- Update vao phieu Goods Receipt
			 	update IGN1 set "U_QCDocEntry" = list_of_cols_val_tab_del where "DocEntry"  = BaseDocEntry;
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

			if exists ( select count(*) from  "SPLITSTRING" (SSCC ,',') group by "part" having count("part") >1) then
				select 84,'Chọn trùng SSCC!',  error_message  from dummy;
			end if;
		End;
	end if;
	---- Costing: Chặn giá nhập thành phẩm < min và > max
	IF (cast(object_type as nvarchar) = '59' and transaction_type  in ( 'A','U')) then
		--1 Khi NSD add phiếu Receipt, đơn giá thành phẩm sẽ được so sánh vs bảng giá Standard
		--2 Nếu ĐG thành phẩm <> ĐG Standard => chặn lưu phiếu
		If exists (select 1  from OPLN  where  ifnull("U_IsBasePriceList",'')  = 'Y') then
			declare tblReceipt table(ItemCode nvarchar(50), Price numeric(19,6));
			declare tblBasePrice table(ItemCode nvarchar(50), Rate numeric(19,6), BasePrice numeric(19,6), Min_BasePrice numeric(19,6), Max_BasePrice numeric(19,6));
			declare RS table ("Itemcode" nvarchar(50), "Price" numeric(28,10), "BasePrice" numeric(28,10), "Min_BasePrice" numeric(28,10), "Max_BasePrice" numeric(28,10));
			
			insert into :tblReceipt(ItemCode,Price) select "ItemCode","PriceBefDi"/(case when  ifnull("Rate",0)>0 then "Rate" else 1 end )  as "UnitPrice" from IGN1  where  "DocEntry"  = :list_of_cols_val_tab_del;
			insert into :tblBasePrice (ItemCode, Rate,BasePrice,Min_BasePrice,Max_BasePrice)
			SELECT T1."ItemCode",T2."U_Rate",T1."Price", T1."Price" - (T1."Price" * ifnull(T2."U_Rate",0)/100 ) as "Min",T1."Price" + (T1."Price" * ifnull(T2."U_Rate",0)/100 ) as "Max"
			FROM ITM1  T1 
			INNER JOIN OPLN T2 on ifnull(T2."U_IsBasePriceList",'')  = 'Y' and T1."PriceList" = T2."ListNum"
			where "Price">0;
		
			insert into :RS ("Itemcode","Price","BasePrice","Min_BasePrice","Max_BasePrice")
			select a.ItemCode "Itemcode", a.Price "Price", b.BasePrice "BasePrice",b.Min_BasePrice "Min_BasePrice",b.Max_BasePrice "Max_BasePrice"
			from :tblReceipt a
			left join :tblBasePrice b on a.ItemCode  = b.ItemCode;
		
			IF  Exists(Select 1 From :RS where (ifnull("Price",0) < ifnull("Min_BasePrice",0)) or (ifnull("Price",0) > ifnull("Max_BasePrice",0))) then 
				error:=85;
				error_message:='Giá nhập kho vượt mức giá cơ bản!';
			END IF;
		End If;
	End If;
---------- update trạng thái Batch: Batch Attribute 2 (Từ màn hình nhập QCResult)
IF (cast(object_type as nvarchar) = 'QAQCResult' and transaction_type  in ( 'A','U')) then
	Begin
		
		Create local temporary table #QCResult
		(
			ID_Result int,
			SSCC_Result nclob,
			Status_Result nvarchar(5000)
		);
		insert into  #QCResult (ID_Result,SSCC_Result, Status_Result)
		
		 select ROW_NUMBER() OVER (),  "U_SSCC" , "U_Status"
		 from "@QC_RESULT_INS_L" 
		 where "DocEntry"   = :list_of_cols_val_tab_del and ifnull(cast("U_SSCC" as nvarchar ),'')  <> '';
		 
		
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
						Select  cast (SSCC_Result as nvarchar(5000)) Into SSCC From #QCResult Where ID_Result = :i;
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
	---- Chặn: receipt from product, transfer request, transfer cho PO (KHI U_POStatus = 'Completed')
	IF (:object_type in ('59','1250000001','67','60') and transaction_type  in ( 'A','U')) then
		--- Receipt from production
		if (:object_type = '59') then -- Goods Receipt
			if (select count(1) from IGN1 x, OWOR y where x."DocEntry"=:list_of_cols_val_tab_del and x."BaseType"='202' and x."BaseEntry"=y."DocEntry" and (y."U_POStatus"='Completed' or y."U_POStatus"='Closed'))>0 then
				select 87,N'Lệnh sản xuất đang ở trạng thái: Completed/Closed!'  into  error,  error_message  from dummy;
			end if;
		end if;
		-- Inventory Transfer Request
		if (:object_type = '1250000001') then 
			if (select count(1) from WTQ21 x, OWOR y where x."DocEntry"=:list_of_cols_val_tab_del and x."RefObjType"='202' and x."RefDocEntr"=y."DocEntry" and (y."U_POStatus"='Completed' or y."U_POStatus"='Closed'))>0 then
				select 88,N'Lệnh sản xuất đang ở trạng thái: Completed/Closed!'  into  error,  error_message  from dummy;
			end if; 
		end if;
		-- Inventory Transfer
		if (:object_type = '67') then 
			if (select count(1) from WTR21 x, OWOR y where x."DocEntry"=:list_of_cols_val_tab_del and x."RefObjType"='202' and x."RefDocEntr"=y."DocEntry" and (y."U_POStatus"='Completed' or y."U_POStatus"='Closed'))>0 then
				select 88,N'Lệnh sản xuất đang ở trạng thái: Completed/Closed!'  into  error,  error_message  from dummy;
			end if; 
		end if;
		--- Issue for production
		if (:object_type = '60') then -- Goods Receipt
			if (select count(1) from IGE1 x, OWOR y where x."DocEntry"=:list_of_cols_val_tab_del and x."BaseType"='202' and x."BaseEntry"=y."DocEntry" and (y."U_POStatus"='Closed'))>0 then
				select 87,N'Lệnh sản xuất đang ở trạng thái: Closed!'  into  error,  error_message  from dummy;
			end if;
		end if;
	END IF; 
	---- Chặn nghiệp vụ theo trạng thái SSCC: Batch Attribute 2
	-- Nguyên Vật Liệu: 
	If (cast(object_type as nvarchar) in ( '15','20','60','67') and transaction_type  in ( 'A','U')) then -- 67: Inventory Transfer
		Begin
			declare isNVL char(1) = 'N';
			declare isTP char(1) = 'N';
			declare Batch_Status table(SSCC nvarchar(50), ToWhs nvarchar(50),Status nvarchar(50),BsDocType int);
			
			If exists(select 1 from  OITM a 
								inner join IBT1 b on a."ItemCode"  = b."ItemCode"
								where a."ManBtchNum" = 'Y' 
								--and a."ItmsGrpCod" in (113,114,127) 
								and a."ItmsGrpCod" in (113,114,121,127) 
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
		
			
			
			insert into :Batch_Status (SSCC,ToWhs,Status, BsDocType)
			Select a."BatchNum",a."WhsCode",ifnull(upper(b."LotNumber"),'') as "Status",a."BsDocType"
			from IBT1 a
			inner join  OBTN b on a."BatchNum" =  b."DistNumber"
			where a."Direction" = (case when :object_type  IN ( '15','60') then '1' else '0' end)  and a."BaseType" = :object_type and a."BaseEntry"  = :list_of_cols_val_tab_del;
			--old "Direction" = '0'
				If (object_type  = '15') then -- Delivery
					---
					IF(isTP = 'Y') then
							If exists ((select 1 from  :Batch_Status where Status in ('QI','BLOCK'))) then  
								
								select 90,N'SSCC Status QI, Block chặn xuất bán!'  into  error,  error_message  from dummy;
								
							End If;
					End IF;
					---		
				End if;
				 
				If (object_type  = '20') then -- Goods Receipt PO
					 IF(isNVL = 'Y') then
							If exists ((select 1 from  :Batch_Status where Status <> 'QI'  and ToWhs  =  'K')) then   
							
							select 91,N'Kho K phải ở trạng thái QI'  into  error,  error_message  from dummy;
							
							End If;
							
							If exists ((select 1 from  :Batch_Status where Status not in( 'RELEASE','RELEASED')  and ToWhs  =  'B')) then   
							
							select 92,N'Kho B phải ở trạng thái RELEASE'  into  error,  error_message  from dummy;
							
							End If;
					End if;
					---		
				End if;
				
				If (object_type  = '60') then -- Goods Issue Nhưng lấy phiếu Issue for production
					IF(isNVL = 'Y') then
							If exists ((select 1 from  :Batch_Status where Status in ('QI','BLOCK') and BsDocType = 202)) then  
								
								select 93,N'Tồn tại SSCC với trạng thái: QI hoặc BLOCK!'  into  error,  error_message  from dummy;
								
							End If;
					End IF;


				End if;
				--
				If (object_type  = '67') then -- Inventory Transfer
					
					IF(isNVL = 'Y') then
							If exists ((select 1 from  :Batch_Status where Status in ('QI','BLOCK') and ToWhs in ( 'P-M','P-L-M','P-I-M'))) then -- N: kho chờ xử lý
							
							select 94,N'SSCC Status QI, Block chặn chuyển tới kho: P-M, P-L-M, P-I-M!'  into  error,  error_message  from dummy;
							
							End If;
					End if;
					---
					/*
					IF(isTP = 'Y') then
							If exists ((select 1 from  :Batch_Status where Status in ('QI','BLOCK') and ToWhs <> 'N')) then -- N: kho chờ xử lý
								
								select 78,N'SSCC Status QI, Block chặn chuyển kho!'  into  error,  error_message  from dummy;
								
							End If;
					End IF;
					*/
					---					
				--	
				End if;	

		End;
	End If; 
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
					/* --================= 28.08.2025
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
					*/ --================= 28.08.2025

					IF(cast(object_type as nvarchar) = '59') then
						If(select count(*) from  OIGN where "DocEntry" = :list_of_cols_val_tab_del 
										and "CANCELED" = 'N'    ) > 0 then
						if not exists(
										 select 1
										 from OIGN a 
										 inner join IGN1 b on a."DocEntry"  =  b."DocEntry"
										 inner join OITM c on b."ItemCode"  = c."ItemCode"
										  
										 where b."BaseLine" is not null and b."IsByPrdct" = 'N' AND a."DocEntry"  = :list_of_cols_val_tab_del 
										 			and b."BaseType" = 202  ) THEN
										 
	 
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
			if (select count(*) from OWOR where "Status" = 'L' and "DocEntry" = list_of_cols_val_tab_del)>0 then
				select 97,N'Hệ thốngg không cho phép cập nhật trạng thái Closed!'   into  error,  error_message  from dummy;
			end if;
	End If;
	---- Chặn ko cho lưu UD Code nếu Result ko có kết quả (Tồn bộ result phải có giá trị)
	If (cast(object_type as nvarchar) = 'QAQCResult' and transaction_type  IN ('A','U')) then
		if(select count(*) from "@QC_RESULT_CHAR_L"  a 
							inner join   "@QC_RESULT_H" b on a."DocEntry"  = b."DocEntry"
							where  a."DocEntry" =:list_of_cols_val_tab_del and ifnull(b."U_UDCode",'') <> ''
							        and ifnull("U_QAResult",'') = '')>0 then
							        
			select 102,N'Phải có đầy đủ Result trước khi cập nhật UDCode!'  into  error,  error_message  from dummy;
		end if;
	End If;
	---- Có 1 hoặc nhiều kết quả NOT OK bất kỳ -> Không cho UD 001
	If (cast(object_type as nvarchar) = 'QAQCResult' and transaction_type  IN ('A','U')) then
		if(select count(*) from "@QC_RESULT_CHAR_L"  a 
							inner join   "@QC_RESULT_H" b on a."DocEntry"  = b."DocEntry"
							where  a."DocEntry" =:list_of_cols_val_tab_del and ifnull(b."U_UDCode",'') = '001'
							        and ifnull("U_QAResult",'') = 'NG')>0 then
							        
			select 103,N'The results do not fully in the norm of specifications!'  into  error,  error_message  from dummy;
		end if;
	End If;
	---- Chặn ko cho cập nhật, xóa phiếu Result nếu đã confirm
	If (cast(object_type as nvarchar) = 'QAQCResult' and transaction_type   ='D') then
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
	End If;
	---- Chặn ko cho cập nhật, xóa phiếu Result nếu đã confirm
	If (cast(object_type as nvarchar) = 'QAQCResult' and transaction_type  = 'U') then
			if(select count(*)  from "@QC_RESULT_H" 
					where "DocEntry" =:list_of_cols_val_tab_del 
						  and  ifnull("U_UserConfirm",'') <> '')>0 then
								        
				select 105,N'Phiếu không thể cập nhật do phiếu đã confirm!'  into  error,  error_message  from dummy;
			end if;
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
		declare  GRPO_Qty_Current table
		(
			ItemCode nvarchar(50),
			GRPO_Qty decimal(19,6),
			PO_Entry int,
			PO_Qty decimal(19,6),
			PO_LineNum int,
			WhsCode nvarchar(50) 
			 
		
		);
		declare tblGRPO table 
		(
		 "ItemCode" varchar(50), 
		 "Qty" numeric (21,6),
		 "PO_LineNum" int,
		 "PO_Entry" int,
		 "Qty_PO" numeric (21,6),
		 "WhsCode" nvarchar(10)
		);
---
		declare PO_Entry table
		(
			"ItemCode"  varchar(50),
			"PO_Entry" int,
			"PO_LineNum" int,
			"WhsCode" nvarchar(10)
		);
--
		insert into :tblGRPO ("ItemCode", "Qty","PO_LineNum","PO_Entry", "WhsCode")
		select  a."ItemCode", "Quantity", "BaseLine" as "PO_LineNum", a."BaseEntry" as "PO_Entry"	, a."WhsCode"							        
				from PDN1 a 
				inner join  OPDN b on a."DocEntry"  =  b."DocEntry"
				where  b."CANCELED"  =  'N' and  b."DocEntry" = :list_of_cols_val_tab_del;
				
---
		insert into :PO_Entry ("ItemCode" ,  "PO_Entry",   "PO_LineNum", "WhsCode")
		select distinct "ItemCode" ,  "PO_Entry",   "PO_LineNum", "WhsCode"
		from   :tblGRPO;
				----
		insert into :GRPO_Qty_Current (ItemCode,GRPO_Qty, PO_Entry, PO_LineNum, WhsCode )
		select distinct a."ItemCode",sum(a."Quantity"),
				a."BaseEntry" as "PO_Entry",
				a."BaseLine" as "PO_LineNum",
				b."WhsCode"
							from PDN1 a, :PO_Entry b, OPDN c
							where a."BaseEntry"  = b."PO_Entry"  
									    	  and a."ItemCode"  =  b."ItemCode" 
									    	  and a."BaseLine"  = b."PO_LineNum"
									    	  and a."DocEntry" = c."DocEntry"
									    	  and c."CANCELED"  =  'N'
							group by a."ItemCode",b."WhsCode",a."BaseEntry", a."BaseLine";
				 
---
		
		update a
		set  PO_Qty  =  (select sum (b."Quantity") 
								from  POR1 b  
								inner join OPOR c on b."DocEntry" = c."DocEntry"
										where c."CANCELED"  =  'N' and b."DocEntry"  = a.PO_Entry 
									    	  and  b."ItemCode"  = a.ItemCode 
									    	  and b."LineNum" = a.PO_LineNum   
									    group by b."ItemCode" )
		from  :GRPO_Qty_Current a;
--- số lượng nhập lúc này, bao gồm cả phiếu đang add, vượt hơn số lượng PO) thi thong bao lỗi:
		If exists (select 1 from :GRPO_Qty_Current where    ifnull(PO_QTY,0) - ifnull(GRPO_QTY,0)  < 0 ) then
			select 107,N'Số lượng GRPO không được lớn hơn PO!'  into  error,  error_message  from dummy;
		End If;
		----- Chuyển trạng thái mặc định Batch Attribute 2
		/*
			GRPO to K, K-L, K-I: tức là GRPO mà vào các kho này status mặc định là QI
			 (Batch Attribute 2 - OBTN = 'QI ')
		*/
		
		if exists (select 1 from :tblGRPO where "WhsCode"  in('K','K-L','K-I'))  then
			update a set "LotNumber"  = 'QI' 
			from OBTN a
		   	where a."DistNumber" in (select "BatchNum" from IBT1 where "BaseType"  = 20 and "BaseEntry" = :list_of_cols_val_tab_del and "WhsCode"  in ('K','K-L','K-I'));
		end if;	
		----- 
		----- Chuyển trạng thái mặc định Batch Attribute 2
		/*
			GRPO to A, A-L, A-I or B, B-L, B-I: 
			Tức là GRPO vào các kho này status mặc định là RELEASE (Batch Attribute 2 - OBTN = 'RELEASE ')
		*/
		
		if exists (select 1 from :tblGRPO where "WhsCode"  in('A','A-L','A-I','B','B-L','B-I'))  then
		   	update a
		   	set "LotNumber"  = 'RELEASE'
		   	from OBTN a
		   	where a."DistNumber" in (select "BatchNum" from IBT1 
		   								where "BaseType"  = 20 
		   										and "BaseEntry" = :list_of_cols_val_tab_del
										and "WhsCode"  in ('A','A-L','A-I','B','B-L','B-I'));
		end if;	
	End IF;
	---chan khong cho luu khi ty gia quy doi >= 30,000
	If (cast(object_type as nvarchar) = '13' and  transaction_type  in ( 'A','U')) then -- A/R Invoice
		if (select count(*) from OINV where "U_DocRate" >= 30000 and "DocEntry" = list_of_cols_val_tab_del)>0 then
			select 92,N'Tỷ giá không cho phép vượt quá 30,000'   into  error,  error_message  from dummy;
		end if;
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
---==>Update U_Currency khi làm Reconcile ngoại tệ
if (object_type in ('30') and transaction_type in ('A','U')) then
	update x set "U_Currency" = ifnull(y."FrgnCurr",'USD')
	from JDT1 x 
	inner join ITR1 y on y."ReconNum"=x."CreatedBy" and y."IsCredit"<>x."DebCred" and y."Account" = x."Account" and y."FrgnCurr" is not null
	where x."TransType"='321' and x."TransId"=list_of_cols_val_tab_del and ifnull(x."FCCurrency",'')='' and ifnull(x."U_Currency",'')='';

	update JDT1 set "FCCurrency" = "U_Currency" where "TransType"='321' and "TransId"=list_of_cols_val_tab_del and ifnull("FCCurrency",'')='';
end if;

---====> tam khoa
/*
-- Chặn Trùng Purchase order draft (PO draft)
If (cast(object_type as nvarchar) = '112' and transaction_type  ='A') then

	if exists (select 1 from ODRF where  "ObjType"  ='22' 
				and "CANCELED"  = 'N' and "Series"  <> -1
					 and  "DocEntry"  = :list_of_cols_val_tab_del) then
			select 111,N'Phải chọn manual để sinh số PO'  into  error,  error_message  from dummy;
	end if;
	
	if exists (select 1 from ODRF where  "ObjType"  ='22' 
				and "CANCELED"  = 'N'
					 and  "DocNum"  = (select "DocNum" from  ODRF where "DocEntry"  =:list_of_cols_val_tab_del 
					 													and  "ObjType"  ='22'  and "CANCELED"  = 'N')
														
					and "DocEntry" <> list_of_cols_val_tab_del ) then
														
	
		select 111,N'DocNum PO Draft không được trùng'  into  error,  error_message  from dummy;
	
	
	end if;
End If;
-- Chặn Trùng Purchase request draft (PR draft)
If (cast(object_type as nvarchar) = '1470000113' and transaction_type  ='A') then

	if exists (select 1 from ODRF where  "ObjType"  ='1470000113' 
				and "CANCELED"  = 'N' and "Series"  <> -1
					 and  "DocEntry"  = :list_of_cols_val_tab_del) then
			select 111,N'Phải chọn manual để sinh số PR'  into  error,  error_message  from dummy;
	end if;
	
	if exists (select 1 from ODRF where  "ObjType"  ='1470000113' 
				and "CANCELED"  = 'N'
					 and  "DocNum"  = (select "DocNum" from  ODRF where "DocEntry"  =:list_of_cols_val_tab_del 
					 													and  "ObjType"  ='1470000113'  and "CANCELED"  = 'N')
														
					and "DocEntry" <> list_of_cols_val_tab_del ) then
														
	
		select 111,N'DocNum PR Draft không được trùng'  into  error,  error_message  from dummy;
	
	
	end if;
End If;




*/

--> Bom bị inactive thì không cho tạo bom
If (cast(object_type as nvarchar) in ( '202') and transaction_type   ='A') then
	declare ItemCode nvarchar(20);
	select "ItemCode"
	into ItemCode default ''
	from  OWOR where "DocEntry" = :list_of_cols_val_tab_del;
	
	if exists (select "Code" from OITT 	where "U_Inactive"  = 'Y' and "Code"  =:ItemCode) then
							 
	select 115,N'Không thể tạo PO khi BOM item thành phẩm inactive!'  into  error,  error_message  from dummy;
	end if;
	
	
End if;
--> Chặn trùng item con trên cùng 1 bom (Quantity >0)
If (cast(object_type as nvarchar) in ( '66') and transaction_type    IN('A','U')) then

	declare ItemCode nvarchar(20);
	select "Code"
	into ItemCode default ''
	from  OITT where "Code" = :list_of_cols_val_tab_del;
	--
	if exists (select  1
				from OITT a
				inner join ITT1 b on a."Code"  = b."Father"
				where b."Quantity">0 and b."Father"  = :ItemCode
				
				group by b."Code", b."Father"
				having count(*) >1 ) then
				
	select 116,N'Tồn tại trùng itemcode với (Quantity >0)!'  into  error,  error_message  from dummy;
	
	
	end if;
End if;


-- Chặn không cho lưu nếu trường ContractNo = NULL
If (cast(object_type as nvarchar) = '17' and transaction_type  in ('A', 'U')) then
	 if exists (select 1 from ORDR where "DocEntry"  = :list_of_cols_val_tab_del and ifnull("U_ContractNo",'')  = '' ) then
	 	select 115,N'Contract No# không được để trống!'  into  error,  error_message  from dummy;
	 end if;
End if;

---==>  Remark phiếu JE của phiếu đánh giá lại kho (Khi chạy add ở costing)
IF (cast(object_type as nvarchar) = '162' and transaction_type  in ( 'A')) then
	if exists (select 1 from OMRV where  left("U_PeriodCost",4) = year (CURRENT_DATE) and   right("U_PeriodCost",2)  =  month (CURRENT_DATE) 
								  		 and "Comments"  ='Add-on: Add new Inventory Revaluation' and
													LAST_DAY(CURRENT_DATE)= CURRENT_DATE ) then
		declare Revalu table
		(
			"LineNum"  int,
			"WhsCode" nvarchar(20)
		);
		--
		declare JE_Revalu table
		(
			"TransId" int,
			"DocLine" int,
			"COGS" nvarchar(20)
		);
		--========= Xử lý lưu vào "U_COGS"  = "WhsCode" bảng details Revaluation
		update a
		set "U_COGS"  = "WhsCode"
		from  MRV1 a
		where "DocEntry"  = :list_of_cols_val_tab_del and  ifnull("U_COGS",'')  = '';
  
		
		--========= Xử lý lưu vào JE
		--Revalu
		insert into :Revalu
		select  b."LineNum", b."WhsCode"
		from OMRV a
		inner join  MRV1 b on b."DocEntry" = a."DocEntry"
		where 	 b."ObjType" = 162   and b."DocEntry"  = :list_of_cols_val_tab_del ;
		
		---	JE_Revalu
		insert into :JE_Revalu ("TransId","DocLine","COGS")
		select  a."TransId",a."DocLine" ,b."WhsCode" as "COGS"
			from JDT1 a
			inner join :Revalu b on a."DocLine" =  b."LineNum"
			where  "TransType" = 162 
					and "CreatedBy" = :list_of_cols_val_tab_del;
		--- update  phiếu JE
		update a
		set a."U_COGS" = b."COGS"
		from JDT1 a
		inner join :JE_Revalu b on a."TransId" = b."TransId" and a."DocLine" =  b."DocLine";
		--===========
	end If;	
End IF;
---================= Kết thúc Remark phiếu JE của phiếu đánh giá lại kho (Khi chạy add ở costing)
---==> Cập nhật remark phiếu JE (Costing): hạch tách kết chuyển CPSX trong kỳ

IF (cast(object_type as nvarchar) = '30' and transaction_type  in ( 'A')) then
	If exists (select 1 from OJDT where "TransType"  =30 and "TransId" = :list_of_cols_val_tab_del and
								ifnull("Memo",'') = '' and   left("U_PeriodProd",4) = year (CURRENT_DATE) and   right("U_PeriodProd",2)  =  month (CURRENT_DATE)  
								 and LAST_DAY(CURRENT_DATE)= CURRENT_DATE) then

		declare period nvarchar(20);
		declare number nvarchar(20) ;
		declare TransId nvarchar(20) ;
		
		
		select "U_PeriodProd" into period  from OJDT where "TransType"  =30 and "TransId" = :list_of_cols_val_tab_del;
		select "Number", "TransId" into  number, TransId from  OJDT where "TransType"  =30  and ifnull("U_PeriodProd",'')  =  :period;
		 
		update a
		set "Memo" = 'Transfer manufacturing cost in ' || right (period,2) ||'/'|| left (period,4)  
		from   OJDT a
		where  "Number"  = :number and "TransType"  =30 and "Memo"  ='';
--
		update a
		set "LineMemo" = 'Transfer manufacturing cost in '|| right (period,2) ||'/'|| left (period,4) 
		from  JDT1 a
		where  "TransId"  =:TransId and "LineMemo"  ='' ;
 
	End If;
	
End If;


--========================= Bắt buộc nhập các đơn vị tính
-- Tat chan  = N --> N  : khong co tat chan --> CHAN

IF (cast(object_type as nvarchar) = '4' and transaction_type in ('A','U')) then
	-- Bat dau chan item master data
	IF exists( select 1 from  OITM where "ItemCode"  = :list_of_cols_val_tab_del and "ItemType" <> 'F') Then -- "ItemType" <> 'F': K phai tai san
				IF EXISTS (select 1 from "@OTHERSETINGS" where "Code"  ='ChanItemMasterData' and "U_Value2"  = 'Y' ) THEN
				 	-- 1*
						declare  tblItem table
						(
							BuyUnitMsr  nvarchar(20),
							SalUnitMsr  nvarchar(20), 
							InvntryUom nvarchar(20)
						);
						
						insert into :tblItem (BuyUnitMsr,SalUnitMsr,InvntryUom)
						select "BuyUnitMsr","SalUnitMsr","InvntryUom"
						from OITM 
						where "ItemCode"  = :list_of_cols_val_tab_del;
						
						
				 	If   (left(:list_of_cols_val_tab_del,1)= '1') then -- Itemcode dau 1*
				 			
				 			if exists (select 1 from :tblItem where ifnull(SalUnitMsr,'') = ''   or ifnull(InvntryUom,'') = '') then 
				 				select 1171,N'Đơn vị tính: Sales , Inventory: Không được để trống!'  into  error,  error_message  from dummy;
				 			end if;
				 			
				 			if exists (select 1 from :tblItem where ifnull(BuyUnitMsr,'') <> '') then
				 				select 1171,N'Đơn vị tính: Purchasing không được nhập với item đầu 1*!'  into  error,  error_message  from dummy;
				 			end if;
				 			
				 	
				 	End If;
				 	-- 2*, 3*
					If   (left(:list_of_cols_val_tab_del,1)= '2' or left(:list_of_cols_val_tab_del,1)= '3' ) then -- Itemcode dau 2*, 3*
							if exists (select 1 from :tblItem where ifnull(BuyUnitMsr,'') = ''   or ifnull(InvntryUom,'') = '') then 
				 				select 1172,N'Đơn vị tính: Purchasing, Inventory : Không được để trống!'  into  error,  error_message  from dummy;
				 			end if;
				 			--
							if exists (select 1 from :tblItem where ifnull(SalUnitMsr,'') <> '') then
				 				select 1172,N'Đơn vị tính: Sales không được nhập với item đầu 2*, 3*!'  into  error,  error_message  from dummy;
				 			end if;
					End If;
					
				
				 	 -- 4->8*
					If   (left(:list_of_cols_val_tab_del,1)= '4' or left(:list_of_cols_val_tab_del,1)= '5' or left(:list_of_cols_val_tab_del,1)= '6'
							or left(:list_of_cols_val_tab_del,1)= '7' or left(:list_of_cols_val_tab_del,1)= '8') then -- Itemcode dau 4->8*
						
							if exists (select 1 from :tblItem where ifnull(BuyUnitMsr,'') = '') then
								select 1173,N'Đơn vị tính: Purchasing : Không được để trống!'  into  error,  error_message  from dummy;
							end if;
							--
							if exists (select 1 from :tblItem where ifnull(SalUnitMsr,'') <> '') then
				 				select 1173,N'Đơn vị tính: Sales không được nhập với item đầu 4* ==> 8*'  into  error,  error_message  from dummy;
				 			end if;
				 			--
				 			if exists (select 1 from :tblItem where ifnull(InvntryUom,'') <> '') then
				 				select 1173,N'Đơn vị tính: Inventory không được nhập với item đầu 4* ==> 8*'  into  error,  error_message  from dummy;
				 			end if;
					
					End If;
					-- Khong duoc chon ngoai danh sach
				if exists (select 1 from :tblItem where length(ifnull(BuyUnitMsr,'')) >1 
									 and   BuyUnitMsr not in (select "U_Name" from  "@UOM_LIST_LINE" )) then 
						 
						 select 1182,N'Đơn vị tính nằm ngoài danh sách!'  into  error,  error_message  from dummy;
				elseif exists (select 1 from :tblItem where length(ifnull(SalUnitMsr,'')) >1 
									 and   SalUnitMsr not in (select "U_Name" from  "@UOM_LIST_LINE" )) then
						select 1183,N'Đơn vị tính nằm ngoài danh sách!'  into  error,  error_message  from dummy;
				
				elseif exists (select 1 from :tblItem where length(ifnull(InvntryUom,'')) >1 
									 and   InvntryUom not in (select "U_Name" from  "@UOM_LIST_LINE" )) then
						select 1184,N'Đơn vị tính nằm ngoài danh sách!'  into  error,  error_message  from dummy;
				
				end if;	
				
				 END IF ;
				  
			--
 
	 -- Ket thuc chan item master data
	End IF;
END IF;
--=========================================Không cho chọn lại đối với đầu itemcode (1*,2*,3*) đối với phiếu PR/PO
--=========================================Không cho chọn lại đối với đầu itemcode (1*,2*,3*) đối với phiếu PR/PO
--====== Bổ sung cho phép bật tắt khi cần. 12/09/2025.
--== PO (phieu that va phieu draft)
  -- PO Thật
IF EXISTS (SELECT 1 from "@OTHERSETINGS" WHERE UPPER("Code")  = 'POPRUOMCHECK' AND "U_Value2"  = 'Y' ) THEN
	 IF (cast(object_type as nvarchar)  ='22' and transaction_type in ('A','U')) then
	 	-- item code đầu 1 thì đối chiếu với đvt của "Sales - bán".
	 	 if exists (select 1 from POR1 a where   a."DocEntry"  = :list_of_cols_val_tab_del
	 	 									 	and a."ItemCode"  like '1%'  
	
	 	 									 and ifnull(a."unitMsr",'') <> (select  ifnull(b."SalUnitMsr",'')  from OITM b where a."ItemCode"  = b."ItemCode" )) then
	 	 	 select 1174,N'Đơn vị tính không được phép sửa!'  into  error,  error_message  from dummy;
	
	 	 end if;
	 	 -- item code đầu 2 đến 3 mình kiểm tra với trường đvt của "Purchase - mua" trên item master data,
		 if exists (select 1 from POR1 a where   a."DocEntry"  = :list_of_cols_val_tab_del
	 	 									 	and  (a."ItemCode"  like '2%' or a."ItemCode"  like '3%'  )
	
	 	 									 and ifnull(a."unitMsr",'') <> (select  ifnull(b."BuyUnitMsr",'')  from OITM b where a."ItemCode"  = b."ItemCode" )) then
	 	 	 select 1175,N'Đơn vị tính không được phép sửa!'  into  error,  error_message  from dummy;
	
	 	 end if;
	 	 
	 	 if exists (select 1 from POR1 a where   a."DocEntry"  = :list_of_cols_val_tab_del and  ifnull(a."unitMsr",'') ='') then
	 	 	 select 1175,N'Đơn vị tính không được để trống!'  into  error,  error_message  from dummy;
	 	 end if;
	 	 -- item code dau 4-->8 duoc phep chon lai, nhng phai nam trong list
	 	 if exists (select 1 from POR1 a where   a."DocEntry"  = :list_of_cols_val_tab_del 
	 	 					and  ifnull(a."unitMsr",'')  not in (select "U_Name" from  "@UOM_LIST_LINE" )
	 	 					and  left (a."ItemCode",1) in (4,5,6,7,8)  ) then
	 	 	 select 1175,N'Đơn vị tính nằm ngoài danh sách!'  into  error,  error_message  from dummy;
	 	 end if;
	 End If;
	 
	
	 
	 -- PO draft
	 IF (cast(object_type as nvarchar)  ='112' and transaction_type in ('A','U')) then
	 	-- item code đầu 1 thì đối chiếu với đvt của "Sales - bán".
	 	 if exists (select 1 from ODRF a inner join DRF1 b on a."DocEntry" = b."DocEntry"
	 	 							 where  a."ObjType"  ='22'  and a."DocEntry"  = :list_of_cols_val_tab_del
	
	 	 									 and b."ItemCode"  like '1%'  
	
	 	 									 and ifnull(b."unitMsr",'') <> (select  ifnull(c."SalUnitMsr",'')  from OITM c where "ItemCode"  = b."ItemCode" )) then
	 	 	 select 1176,N'Đơn vị tính không được phép sửa!'  into  error,  error_message  from dummy;
	
	 	 end if;
	 	 -- item code đầu 2 đến 3 mình kiểm tra với trường đvt của "Purchase - mua" trên item master data,
		 	 if exists (select 1 from ODRF a inner join DRF1 b on a."DocEntry" = b."DocEntry"
	 	 							 where  a."ObjType"  ='22'  and a."DocEntry"  = :list_of_cols_val_tab_del
	
	 	 									 and  (b."ItemCode"  like '2%' or b."ItemCode"  like '3%' )
	
	 	 									 and ifnull(b."unitMsr",'') <> (select  ifnull(c."BuyUnitMsr",'')  from OITM c where c."ItemCode"  =  b."ItemCode"  )) then
	
	 	 	 	select 1177,N'Đơn vị tính không được phép sửa!'  into  error,  error_message  from dummy;
	
	 	 end if;
	 	 --
	 	 if exists (select 1 from DRF1 a where   a."DocEntry"  = :list_of_cols_val_tab_del and  ifnull(a."unitMsr",'') ='') then
	 	 	 select 1177,N'Đơn vị tính không được để trống!'  into  error,  error_message  from dummy;
	 	 end if;
	 	-- item code dau 4-->8 duoc phep chon lai, nhng phai nam trong list
	 	 if exists (select 1 from DRF1 a where   a."DocEntry"  = :list_of_cols_val_tab_del 
	 	 					and  ifnull(a."unitMsr",'')  not in (select "U_Name" from  "@UOM_LIST_LINE" )
	 	 					and  left (a."ItemCode",1) in (4,5,6,7,8)  ) then
	 	 	 select 1177,N'Đơn vị tính nằm ngoài danh sách!'  into  error,  error_message  from dummy;
	 	 end if;
	 End If;
--
--========================= PR (phieu that va phieu draft)
	  -- PR Thật
	 IF (cast(object_type as nvarchar)  ='1470000113' and transaction_type in ('A','U')) then
	 	-- item code đầu 1 thì đối chiếu với đvt của "Sales - bán".
	 	 if exists (select 1 from PRQ1 a where   a."DocEntry"  = :list_of_cols_val_tab_del
	 	 									 	and a."ItemCode"  like '1%'  
	
	 	 									 and ifnull(a."unitMsr",'') <> (select  ifnull(b."SalUnitMsr",'')  from OITM b where b."ItemCode"  = a."ItemCode" )) then
	 	 	 select 1178,N'Đơn vị tính không được phép sửa!'  into  error,  error_message  from dummy;
	
	 	 end if;
	 	 -- item code đầu 2 đến 3 mình kiểm tra với trường đvt của "Purchase - mua" trên item master data,
		if exists (select 1 from PRQ1 a where   a."DocEntry"  = :list_of_cols_val_tab_del
	 	 									 	and (a."ItemCode"  like '2%'   or a."ItemCode"  like '3%'  )
	
	 	 									 and ifnull(a."unitMsr",'') <> (select  ifnull(b."BuyUnitMsr",'')  from OITM b where b."ItemCode"  = a."ItemCode" )) then
	 	 	 select 1179,N'Đơn vị tính không được phép sửa!'  into  error,  error_message  from dummy;
	
	 	 end if;
	 	 --
	 	if exists (select 1 from PRQ1 a where   a."DocEntry"  = :list_of_cols_val_tab_del and  ifnull(a."unitMsr",'') ='') then
	 	 	 select 1179,N'Đơn vị tính không được để trống!'  into  error,  error_message  from dummy;
	 	 end if;
	 	-- item code dau 4-->8 duoc phep chon lai, nhng phai nam trong list
	 	if exists (select 1 from PRQ1 a where   a."DocEntry"  = :list_of_cols_val_tab_del 
	 	 					and  ifnull(a."unitMsr",'')  not in (select "U_Name" from  "@UOM_LIST_LINE" )
	 	 					and  left (a."ItemCode",1) in (4,5,6,7,8)  ) then
	 	 	 select 1179,N'Đơn vị tính nằm ngoài danh sách!'  into  error,  error_message  from dummy;
	 	 end if;
	 	 
	 	 
	 	 
	 End If;
	 
	 -- PR draft
	 IF (cast(object_type as nvarchar)  ='112' and transaction_type in ('A','U')) then
	 	-- item code đầu 1 thì đối chiếu với đvt của "Sales - bán".
	 	 if exists (select 1 from ODRF a inner join DRF1 b on a."DocEntry" = b."DocEntry"
	 	 							 where  a."ObjType"  ='1470000113'  and a."DocEntry"  = :list_of_cols_val_tab_del
	
	 	 									 and b."ItemCode"  like '1%'  
	
	 	 									 and ifnull(b."unitMsr",'') <> (select  ifnull(c."SalUnitMsr",'')  from OITM c where c."ItemCode"  = b."ItemCode" )) then
	 	 	 select 1180,N'Đơn vị tính không được phép sửa!'  into  error,  error_message  from dummy;
	
	 	 end if;
	 	 -- item code đầu 2 đến 3 mình kiểm tra với trường đvt của "Purchase - mua" trên item master data,
		 	 
		  if exists (select 1 from ODRF a inner join DRF1 b on a."DocEntry" = b."DocEntry"
	 	 							 where  a."ObjType"  ='1470000113'  and a."DocEntry"  = :list_of_cols_val_tab_del
	
	 	 									 and (b."ItemCode"  like '2%' or   b."ItemCode"  like '3%' )
	
	 	 									 and ifnull(b."unitMsr",'') <> (select  ifnull(c."BuyUnitMsr",'')  from OITM c where c."ItemCode"  = b."ItemCode" )) then
	 	 	 select 1181,N'Đơn vị tính không được phép sửa!'  into  error,  error_message  from dummy;
	
	 	 end if;
	 	 --
	 	if exists (select 1 from DRF1 a where   a."DocEntry"  = :list_of_cols_val_tab_del and  ifnull(a."unitMsr",'') ='') then
	 	 	 select 1181,N'Đơn vị tính không được để trống!'  into  error,  error_message  from dummy;
	 	 end if;
		-- item code dau 4-->8 duoc phep chon lai, nhng phai nam trong list
	 	if exists (select 1 from DRF1 a where   a."DocEntry"  = :list_of_cols_val_tab_del 
	 	 					and  ifnull(a."unitMsr",'')  not in (select "U_Name" from  "@UOM_LIST_LINE" )
	 	 					and  left (a."ItemCode",1) in (4,5,6,7,8)  ) then
	 	 	 select 1181,N'Đơn vị tính nằm ngoài danh sách!'  into  error,  error_message  from dummy;
	 	 end if;
	 End If;
END IF;
--=====================Bắt buộc nhập số nguyên (Chặn nhập số lượng (Quantity) là số lẻ (Số thập phân))
-- PR
IF EXISTS (SELECT 1 from "@OTHERSETINGS" WHERE UPPER("Code")  = 'SOLUONG_PR_LA_SO_NGUYEN' AND "U_Value2"  = 'Y' ) THEN
		If (cast(object_type as nvarchar) in ( '1470000113', '112') and transaction_type  in ('A', 'U')) then
			
			if exists (select   1   from OPRQ a 
								inner join PRQ1 b on a."DocEntry" =  b."DocEntry"
								inner join OITM c on b."ItemCode"  = c."ItemCode"
								where a."DocEntry"  = :list_of_cols_val_tab_del
										 and mod(b."Quantity",1) <> 0
										--and c."InvntItem"  = 'N'
										--and c."ItemCode" like '6%'
						) then
				select 112,N'Quantity item service: Phải là số nguyên!'  into  error,  error_message  from dummy;
			end if;
			-- draft
			if exists (select   1   from ODRF a 
								inner join DRF1 b on a."DocEntry" =  b."DocEntry"
								inner join OITM c on b."ItemCode"  = c."ItemCode"
								where a."DocEntry"  = :list_of_cols_val_tab_del
										 and mod(b."Quantity",1) <> 0
										--and c."InvntItem"  = 'N'
										--and c."ItemCode" like '6%'
										and a."ObjType" = '1470000113'
						) then
				select 112,N'Quantity item service: Phải là số nguyên!'  into  error,  error_message  from dummy;
			end if;
			
			
		End If;
END IF;
-- PO
IF EXISTS (SELECT 1 from "@OTHERSETINGS" WHERE UPPER("Code")  = 'SOLUONG_PO_LA_SO_NGUYEN' AND "U_Value2"  = 'Y' ) THEN
		If (cast(object_type as nvarchar) in('22','112') and transaction_type  in ('A', 'U')) then
			
			if exists (select   1   from OPOR a 
								inner join POR1 b on a."DocEntry" =  b."DocEntry"
								inner join OITM c on b."ItemCode"  = c."ItemCode"
								where a."DocEntry"  = :list_of_cols_val_tab_del
										 and mod(b."Quantity",1) <> 0
										--and c."InvntItem"  = 'N'
										--and c."ItemCode" like '6%'
						) then
				select 113,N'Quantity item service: Phải là số nguyên!'  into  error,  error_message  from dummy;
			end if;
			--
			if exists (select   1   from ODRF a 
								inner join DRF1 b on a."DocEntry" =  b."DocEntry"
								inner join OITM c on b."ItemCode"  = c."ItemCode"
								where a."DocEntry"  = :list_of_cols_val_tab_del
										 and mod(b."Quantity",1) <> 0
										--and c."InvntItem"  = 'N'
										--and c."ItemCode" like '6%'
										and a."ObjType" = '22'
						) then
				select 113,N'Quantity item service: Phải là số nguyên!'  into  error,  error_message  from dummy;
			end if;
		End If;
END IF;
-- GRPO
IF EXISTS (SELECT 1 from "@OTHERSETINGS" WHERE UPPER("Code")  = 'SOLUONG_GRPO_LA_SO_NGUYEN' AND "U_Value2"  = 'Y' ) THEN
		If (cast(object_type as nvarchar) in ( '20','112') and transaction_type  in ('A', 'U')) then
			
			if exists (select   1   from OPDN a 
								inner join PDN1 b on a."DocEntry" =  b."DocEntry"
								inner join OITM c on b."ItemCode"  = c."ItemCode"
								where a."DocEntry"  = :list_of_cols_val_tab_del
										 and mod(b."Quantity",1) <> 0
										--and c."InvntItem"  = 'N'
										--and c."ItemCode" like '6%'
						) then
				select 114,N'Quantity item service: Phải là số nguyên!'  into  error,  error_message  from dummy;
			end if;
			--
			if exists (select   1   from ODRF a 
								inner join DRF1 b on a."DocEntry" =  b."DocEntry"
								inner join OITM c on b."ItemCode"  = c."ItemCode"
								where a."DocEntry"  = :list_of_cols_val_tab_del
										 and mod(b."Quantity",1) <> 0
										--and c."InvntItem"  = 'N'
										--and c."ItemCode" like '6%'
										and a."ObjType" = '20'
						) then
				select 114,N'Quantity item service: Phải là số nguyên!'  into  error,  error_message  from dummy;
			end if;
		End If;
END IF;
--===================================================Kết thúc Bắt buộc nhập số nguyên 
 -- Chặn lưu phiếu PR/PO nếu trường Cost center trống: 23/10/2025
IF (object_type = '112' and transaction_type in ('A','U')) then
 
 	if exists(select 1 from ODRF x,DRF1 y where x."DocEntry" = :list_of_cols_val_tab_del and x."DocEntry" = y."DocEntry" and x."ObjType" in ('22','1470000113') and IFNULL(NULLIF(y."OcrCode",''),'')='') then
 		error := 250924001;
		error_message := N'Cost Center không được để trống';
 	end if;
 
 end if;
 IF (object_type = '1470000113' and transaction_type in ('A','U')) then
 
 	if exists(select 1 from PRQ1 x where x."DocEntry" = :list_of_cols_val_tab_del and IFNULL(NULLIF(x."OcrCode",''),'')='') then
 		error := 250924002;
		error_message := N'Cost Center không được để trống';
 	end if;
 
 end if;
 
  IF (object_type = '22' and transaction_type in ('A','U')) then
 
 	if exists(select 1 from POR1 x where x."DocEntry" = :list_of_cols_val_tab_del and IFNULL(NULLIF(x."OcrCode",''),'')='') then
 		error := 250924003;
		error_message := N'Cost Center không được để trống';
 	end if;
 
 end if;
-- Chặn GRPO date: GRPO date >= PO generated date. 28/10/2025
	if (:object_type in ('20') and :transaction_type in ('A','U')) then
		if exists(
				select
					 1
				from OPDN a 
				inner join PDN1 b on b."DocEntry" = a."DocEntry" 
				inner join OPOR c on c."DocEntry" = b."BaseEntry" 
				WHERE 
					a."DocEntry" = :list_of_cols_val_tab_del 
					and b."BaseType" = 22 
					and a."DocDate" < c."CreateDate" ) then
			error = 12271;
			error_message = N'GRPO date phải >= PO generated date';
			select :error, :error_message from dummy;
		end if;
	end if;
-- Chặn nhập backdating đối với RFP(Receipt From Production) và IFP (Issue For Production)
-- Goods receipt, goods Issuse, Outsourcing 
	if (:object_type in ('59') and :transaction_type in ('A')) then -- RFP
		declare nsongay int;
		select "U_Value" into nsongay from "@OTHERSETINGS" where "Code"  = 'RFPIFPBACKDATE' AND "U_Value" > 0;
		if :nsongay > 0 then
			if exists(
					select 
						1							
					from OIGN a
						inner join IGN1 b  on a."DocEntry" = b."DocEntry"
						inner join OITM c  on b."ItemCode"  = c."ItemCode"
					where 
						c."ItmsGrpCod"  in (120)
						and a."DocEntry" = :list_of_cols_val_tab_del 
						--and b."BaseType"  =  202 
						and DAYS_BETWEEN(a."DocDate",current_date) > :nsongay 
					limit 1
					) then --add_days(a."DocDate",:nsongay) = a."CreateDate"
				error = 2511041;
				error_message = N'Postingdate không được nhập lùi quá '|| :nsongay || ' ngày so với hiện tại';
				select :error, :error_message from dummy;			
			end if;	
		end if;
	end if;
	if (:object_type in ('60') and :transaction_type in ('A')) then -- IFP (Xuat NVL theo BOM), GI (Xuat VNL khac ngoai BOM)
		declare nsongay int;
		select "U_Value" into nsongay from "@OTHERSETINGS" where "Code"  = 'RFPIFPBACKDATE' AND "U_Value" > 0;
		if :nsongay > 0 then
			if exists(
					select 
						1							
					from OIGE a
						inner join IGE1 b  on a."DocEntry" = b."DocEntry"
						inner join OITM c  on b."ItemCode"  = c."ItemCode"
					where 
						c."ItmsGrpCod"  in (113)
						and a."DocEntry" = :list_of_cols_val_tab_del 
						--and b."BaseType"  =  202 
						and DAYS_BETWEEN(a."DocDate",current_date) > :nsongay 
					limit 1
					) then
				error = 2511042;
				error_message = N'Postingdate không được nhập lùi quá '|| :nsongay || ' ngày so với hiện tại';
				select :error, :error_message from dummy;	
			end if;	
		end if;
	end if;	
	
	if error = 0 then
		IF (cast(object_type as nvarchar) = '4' and transaction_type in ('A','U')) then
			IF exists( select 1 from  OITM a inner join "@PRODUCT_TYPE" b on a."U_ProductType" = b."Code" where a."ItemCode"  = :list_of_cols_val_tab_del and b."U_Status" = 'I') Then 
				error = 251201;
				error_message = N'Product Type đã Inactive, không được phép chọn';
				select :error, :error_message from dummy;			
			end if;
		end if;	
	end if;	
--==============================================================================================================
/*
	Yêu cầu: Chặn lưu các phiếu  nếu BP có trường Currency = All Currencies, chỉ được hạch toán loại tiền được ghi nhận trên U_Currency
*/
-- Purchase Request, Purchase Order draft
	if (object_type = '112' and transaction_type = 'A') then
		
		if exists(select 1 
					from 
						ODRF a
						inner join OCRD b on a."CardCode" = b."CardCode" and b."Currency" = '##'
					where
						a."DocEntry" = :list_of_cols_val_tab_del
						and a."DocCur" <> IFNULL(b."U_Currency",'') and a."ObjType" = '22') then
			error = 251020001;
			error_message = 'Business Partner chỉ ghi nhận theo đồng tiền mặc định';
		end if;
		
	end if;
-- Purchase Order
	if (object_type = '22' and transaction_type = 'A') then
		
		if exists(select 1 
					from 
						OPOR a
						inner join OCRD b on a."CardCode" = b."CardCode" and b."Currency" = '##'
					where
						a."DocEntry" = :list_of_cols_val_tab_del
						and a."DocCur" <> IFNULL(b."U_Currency",'')) then
			error = 251020001;
			error_message = 'Business Partner chỉ ghi nhận theo đồng tiền mặc định';
		end if;
		
	end if;
-- Delivery
	if (object_type = '15' and transaction_type = 'A') then
		
		if exists(select 1 
					from 
						ODLN a
						inner join OCRD b on a."CardCode" = b."CardCode" and b."Currency" = '##'
					where
						a."DocEntry" = :list_of_cols_val_tab_del
						and a."DocCur" <> IFNULL(b."U_Currency",'')) then
			error = 251020002;
			error_message = 'Business Partner chỉ ghi nhận theo đồng tiền mặc định';
		end if;
		
	end if;
-- Return
	if (object_type = '16' and transaction_type = 'A') then
		
		if exists(select 1 
					from 
						ORDN a
						inner join OCRD b on a."CardCode" = b."CardCode" and b."Currency" = '##'
					where
						a."DocEntry" = :list_of_cols_val_tab_del
						and a."DocCur" <> IFNULL(b."U_Currency",'')) then
			error = 251020003;
			error_message = 'Business Partner chỉ ghi nhận theo đồng tiền mặc định';
		end if;
		
	end if;
-- AR Invoice
	if (object_type = '13' and transaction_type = 'A') then
		
		if exists(select 1 
					from 
						OINV a
						inner join OCRD b on a."CardCode" = b."CardCode" and b."Currency" = '##'
					where
						a."DocEntry" = :list_of_cols_val_tab_del
						and a."DocCur" <> IFNULL(b."U_Currency",'')) then
			error = 251020004;
			error_message = 'Business Partner chỉ ghi nhận theo đồng tiền mặc định';
		end if;
		
	end if;
-- AR Credit Memo
	if (object_type = '14' and transaction_type = 'A') then
		
		if exists(select 1 
					from 
						ORIN a
						inner join OCRD b on a."CardCode" = b."CardCode" and b."Currency" = '##'
					where
						a."DocEntry" = :list_of_cols_val_tab_del
						and a."DocCur" <> IFNULL(b."U_Currency",'')) then
			error = 251020005;
			error_message = 'Business Partner chỉ ghi nhận theo đồng tiền mặc định';
		end if;
		
	end if;
-- AR Down Payment Invoice
	if (object_type = '203' and transaction_type = 'A') then
		
		if exists(select 1 
					from 
						ODPI a
						inner join OCRD b on a."CardCode" = b."CardCode" and b."Currency" = '##'
					where
						a."DocEntry" = :list_of_cols_val_tab_del
						and a."DocCur" <> IFNULL(b."U_Currency",'')) then
			error = 251020006;
			error_message = 'Business Partner chỉ ghi nhận theo đồng tiền mặc định';
		end if;
		
	end if;
-- Goods Receipt PO
	if (object_type = '20' and transaction_type = 'A') then
		
		if exists(select 1 
					from 
						OPDN a
						inner join OCRD b on a."CardCode" = b."CardCode" and b."Currency" = '##'
					where
						a."DocEntry" = :list_of_cols_val_tab_del
						and a."DocCur" <> IFNULL(b."U_Currency",'')) then
			error = 251020007;
			error_message = 'Business Partner chỉ ghi nhận theo đồng tiền mặc định';
		end if;
		
	end if;
-- Goods Return
	if (object_type = '21' and transaction_type = 'A') then
		
		if exists(select 1 
					from 
						ORPD a
						inner join OCRD b on a."CardCode" = b."CardCode" and b."Currency" = '##'
					where
						a."DocEntry" = :list_of_cols_val_tab_del
						and a."DocCur" <> IFNULL(b."U_Currency",'')) then
			error = 251020008;
			error_message = 'Business Partner chỉ ghi nhận theo đồng tiền mặc định';
		end if;
		
	end if;
-- AP Invoice
	if (object_type = '18' and transaction_type = 'A') then
		
		if exists(select 1 
					from 
						OPCH a
						inner join OCRD b on a."CardCode" = b."CardCode" and b."Currency" = '##'
					where
						a."DocEntry" = :list_of_cols_val_tab_del
						and a."DocCur" <> IFNULL(b."U_Currency",'')) then
			error = 251020009;
			error_message = 'Business Partner chỉ ghi nhận theo đồng tiền mặc định';
		end if;
		
	end if;
-- AP Credit Memo
	if (object_type = '19' and transaction_type = 'A') then
		
		if exists(select 1 
					from 
						ORPC a
						inner join OCRD b on a."CardCode" = b."CardCode" and b."Currency" = '##'
					where
						a."DocEntry" = :list_of_cols_val_tab_del
						and a."DocCur" <> IFNULL(b."U_Currency",'')) then
			error = 251020010;
			error_message = 'Business Partner chỉ ghi nhận theo đồng tiền mặc định';
		end if;
		
	end if;
-- Outgoing Payment
	if (object_type = '46' and transaction_type = 'A') then
		
		if exists(select 1 
					from 
						OVPM a
						inner join OCRD b on a."CardCode" = b."CardCode" and b."Currency" = '##'
					where
						a."DocEntry" = :list_of_cols_val_tab_del
						and a."DocCurr" <> IFNULL(b."U_Currency",'')) then
			error = 251020011;
			error_message = 'Business Partner chỉ ghi nhận theo đồng tiền mặc định';
		end if;
		
	end if;	
	
-- Journal Entry
if (object_type = '30' and transaction_type = 'A') then
	
	if exists(select 1 
				from 
					JDT1 a
					inner join OCRD b on a."ShortName" = b."CardCode" and b."Currency" = '##'
				where
					a."TransId" = :list_of_cols_val_tab_del
					and IFNULL(a."FCCurrency",'') <> IFNULL(b."U_Currency",'') 
					and ((IFNULL(a."FCCurrency",'') = '' and IFNULL(b."U_Currency",'')<>'USD')
								or (IFNULL(b."U_Currency",'') = 'USD' and IFNULL(a."FCCurrency",'') <> '')
								or (IFNULL(b."U_Currency",'') <> 'USD' and IFNULL(a."FCCurrency",'') <> 'USD'))) then
		error = 251020013;
		error_message = 'Business Partner chỉ ghi nhận theo đồng tiền mặc định';
	end if;

end if;
-- Validate dữ liệu
if :error = 0 then
	call SBO_SP_ValidateData(:object_type, :transaction_type, :num_of_cols_in_key, :list_of_key_cols_tab_del, :list_of_cols_val_tab_del,:error,:error_message);
	if :error <> 0 then
		select :error, :error_message FROM dummy;
	end if;
	call SBO_SP_VALIDATEDATA_2026(:object_type,:transaction_type,:num_of_cols_in_key,:list_of_key_cols_tab_del,:list_of_cols_val_tab_del,:error,:error_message);
	if :error <> 0 then
		select :error, :error_message FROM dummy;
	end if;	
end if;
-- Update dữ liệu sau khi đã validate thành công
if :error = 0 then
	call SBO_SP_UpdateData(:object_type, :transaction_type, :num_of_cols_in_key, :list_of_key_cols_tab_del, :list_of_cols_val_tab_del);
end if;	
--=========================
-- Select the return values
select :error, :error_message FROM dummy;

end;
