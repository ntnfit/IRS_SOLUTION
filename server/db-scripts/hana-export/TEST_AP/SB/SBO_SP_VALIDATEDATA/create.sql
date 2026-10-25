CREATE procedure SBO_SP_ValidateData
(
	in object_type nvarchar(30), 				-- SBO Object Type
	in transaction_type nchar(1),			-- [A]dd, [U]pdate, [D]elete, [C]ancel, C[L]ose
	in num_of_cols_in_key int,
	in list_of_key_cols_tab_del nvarchar(255),
	in list_of_cols_val_tab_del nvarchar(255),
	out error  int,
	out error_message nvarchar (200)
)
as
begin

	DECLARE v_EInvoiceStatus  NVARCHAR(20) = '';
	DECLARE v_EInvoiceMessage NVARCHAR(200) = '';
	error := 0;
	error_message := N'Ok';
/*
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
			return;
		end if;
	end if;
*/
-- Kiểm tra trùng dữ liệu Offatke Price setting
	if (cast(object_type as nvarchar) = 'OFTPRICESTFORM' and transaction_type in ('A','U')) then
		declare macc nvarchar(10);
		declare mdept nvarchar(10);
		select 
			"U_Account","U_Department" 
		into macc,mdept default null,null
		from "@OFFTAKE_PRICE_L" 
		group by 
			"U_Account","U_Department" 
		having(count("U_Account")>1);
		if macc is not null then
			error = 250830;
			error_message = N'Đã tồn tại Account '|| macc || N' và department ' || mdept;
			return  ;
		end if;		
	end if;
--Kiểm tra nhập Itemdetail, Item description. 25/12/2025
 If (cast(object_type as nvarchar) in ('1470000113') and transaction_type  in ('A', 'U')) then
 	if exists (select 1 from PRQ1 a where  a."DocEntry"  = :list_of_cols_val_tab_del and  ifnull(a."U_ItemDetail",'') ='') then
		error = 113251;
		error_message = N'Item detail không được trống';
		return;
	end if;
		
 	if exists (select 1 from PRQ1 a 
 			 LEFT JOIN OITM B ON b."ItemCode" = a."ItemCode" 
 			 where  a."DocEntry"  = :list_of_cols_val_tab_del and  a."Dscription" != b."ItemName") then
		error = 22255;
		error_message = N'Item description phải giống trong Item master data';
		return;
		--select :error, :error_message FROM dummy;
	end if;	
		
 end if;
 
 If (cast(object_type as nvarchar) in ('112') and transaction_type  in ('A', 'U')) then
 	if exists (select 1 from DRF1 a
 		LEFT JOIN OITM b ON b."ItemCode" = a."ItemCode"  
 		where  a."DocEntry" = :list_of_cols_val_tab_del and a."Dscription" != b."ItemName" AND A."ObjType" IN(1470000113)) then
		error = 112251;
		error_message = N'Item description phải giống trong Item master data';
		return;
		--select :error, :error_message FROM dummy;
	end if;	
	
 	if exists (select 1 from DRF1 a 
 		where  a."DocEntry" = :list_of_cols_val_tab_del and ifnull(a."U_ItemDetail",'') ='' AND A."ObjType" IN(1470000113,22)) then
		error = 112252;
		error_message = N'Item detail không được trống';
		return;
	end if;		
 end if;
 
 If (cast(object_type as nvarchar) in ('22') and transaction_type  in ('A', 'U')) then
 	if exists (select 1 from POR1 a where  a."DocEntry"  = :list_of_cols_val_tab_del and  ifnull(a."U_ItemDetail",'') ='') then
		error = 22253;
		error_message = N'Item detail không được trống';
		return;
		--select :error, :error_message FROM dummy;
	end if;
 	/*if exists (select 1 from POR1 a 
 			 LEFT JOIN OITM B ON b."ItemCode" = a."ItemCode" 
 			 where  a."DocEntry"  = :list_of_cols_val_tab_del and  a."Dscription" != b."ItemName") then
		error = 22254;
		error_message = N'Item description phải giống trong Item master data';
		select :error, :error_message FROM dummy;
	end if;*/	
 end if;
--=========================================================
/*
Yêu cầu: chặn từng hóa đơn theo [Vendor code][Year][Inv. No. 1], [Vendor code][Year][Inv. No. 2], ...[Vendor code][Year][Inv. No. n]
21/10/2025. đảm bảo dấu cách giữa các hóa đơn là dấu phẩy (,) khi nhập liệu trường "Số hóa đơn" [Inv. No. 1],[Inv. No. 2],...,[Inv. No. n].
*/
-- A/P Invoice
	if (:object_type in ('18') and :transaction_type in ('A','U')) then
		
		if exists(select 1 from OPCH where (ifnull(to_nvarchar("U_APInv"),'') = '' or "U_InvDate" is null) and "DocEntry" =:list_of_cols_val_tab_del) then
			error = 181;
	        error_message = N'Bạn phải nhập số và ngày hóa đơn';
	        return;
		else	
			declare pinput_invno NVARCHAR(1000);
			declare pvendercode nvarchar(15);
			declare pYearInv int ;
			declare exit_inv nvarchar(100);
			
			select 
				"U_APInv","CardCode",year("U_InvDate") into pinput_invno,pvendercode,pYearInv 
			from OPCH 
			where 
				"DocEntry" = :list_of_cols_val_tab_del;
				
		   -- Không kiểm tra phiếu đang thao tác, tránh trường hợp sửa phiếu sẽ kiểm tra chính nó.
			select top 1 N'Trùng InvNo: ' || sv."InvNo" || ', DocNum: ' || t."DocNum" || ', InvDate: ' ||TO_DATE(t."U_InvDate") as "exitinv"
			into exit_inv default '' 
			from (select
				 	"part" as "InvNo" ,'%,' || "part" || ',%' AS "InputInvNo"
				  from 
					SplitString(pinput_invno,',')) sv
			inner join (select
							"DocNum",
							"DocDate",
							"CardCode",
							"U_APInv",
							"U_InvDate",
							(',' || replace("U_APInv",	' ','') || ',') as "KeySearch" 
						from OPCH 
						where 
							length("U_APInv") > 0 
							and year("U_InvDate") = :pYearInv 
							and "CardCode" = :pvendercode
							and "DocEntry" != :list_of_cols_val_tab_del
				) t on t."KeySearch" like sv."InputInvNo"
			;
		    if length(:exit_inv) > 0 then -- Có giá trị trùng
		        error = 25102101;
		        error_message = :exit_inv;
		        return;
		    end if ;
	    end if;
	end if;	
	
	/* ===================================================================
   	   CHẶN ADD / UPDATE @EINVOICE Bảng thiết lập thông số hóa đơn điện tử 
	   không đúng cấu hình của site BAKV chính thức, demo
   	   =================================================================== */

	IF :object_type = '-3	@EINVOICE' AND :transaction_type IN ('A', 'U') THEN
	    CALL "SP_CHECK_EINVOICE_CONFIG"
	    (
	        v_EInvoiceStatus,
	        v_EInvoiceMessage
	    );
	    IF :v_EInvoiceStatus = 'ERROR' THEN
	        error := -10001;
	        error_message := :v_EInvoiceMessage;
	        RETURN;
	    END IF;
	    
		UPDATE "APZ_OCTA"
		SET "HttpUri" = (SELECT TOP 1 "U_HttpUri" FROM "@EINVOICE" WHERE "Name" = 'BKAV')
		WHERE "ItemId" LIKE 'APZ_EI%'
			AND "ActionTypeEx" = 9;
	END IF;
	
end