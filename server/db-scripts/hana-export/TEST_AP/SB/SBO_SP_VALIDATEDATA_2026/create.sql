CREATE procedure SBO_SP_VALIDATEDATA_2026
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

	error := 0;
	error_message := N'Ok';
-- == ID: 14866: Chặn remove BOM khi không được phép
	If (cast(object_type as nvarchar) in ('66') and transaction_type IN('D','C','L')) then
		declare pCode nvarchar(20);
		declare pCheck varchar(1);
		declare pAutho int = 0; -- = 0. Khong co quyen Remove, 1 Co quyen Remove.
		select TO_VARCHAR("U_Value",'999999999'),"U_Value2" into pCode, pCheck default '','N' from "@OTHERSETINGS" where "Code" = 'XoaBOM' ;
		if (pCheck = 'Y') then
			if (pCode = :list_of_cols_val_tab_del) then
				pAutho = 1;		
			end if;
		else
			pAutho = 1;
		end if;	
		if (pAutho = 0) then
			error = 14866;
			error_message = 'Không được phép Remove BOM !';
			return; -- return error not remove	
		end if;		
		
	End if;	
-- == END 14866	
-- ==Misa ID: 13227. PO creation for Green Bean Decaf Tolling.Golive:11-02-2026
	IF (:object_type in ('202','59','60') ) THEN
		--Trong danh sách chỉ cho làm hàng 253,khác sẽ không được
		--Ngoài danh sách chỉ làm khác 253, = 253 sẽ không cho làm
		declare pUserId int ;
		declare pBaseType int = 202;
		declare pCheck int = 0;
		declare pAutho int =0 ;
		declare pCondition varchar(100) = ' = 253';
		declare pConditionIFP varchar(100) = '(substring(b."BaseRef",0,1) = ''9'' )';
		declare pQuery nvarchar(1000);
		declare pmessage nvarchar(50) = N' không' ;
	
		if (:object_type in ('202')) then -- PO: Production Order
			select case when :transaction_type = 'A' then "UserSign" else "UserSign2" end into pUserId from OWOR where "DocEntry" = :list_of_cols_val_tab_del;			
		end if;
		if (:object_type in ('59') ) then -- RFP: Receipt From Production
			select case when :transaction_type = 'A' then "UserSign" else "UserSign2" end into pUserId from OIGN where "DocEntry" = :list_of_cols_val_tab_del;			
		end if;
		if (:object_type in ('60')) then -- RFP: Issue For Production
			select case when :transaction_type = 'A' then "UserSign" else "UserSign2" end into pUserId from OIGE where "DocEntry" = :list_of_cols_val_tab_del;			
		end if;
		IF EXISTS(select 1 from USR7 where "GroupId" = 1 and "UserId" = :pUserId) then
			pCondition := '!= 253';
			pConditionIFP = '(substring(b."BaseRef",0,1) != ''9'' )';
			pmessage := '';
			--ELSE
			-- Tạm ngắt với User không thuộc nhóm trên
			--return;
		END IF;
		-- Có tên trong danh sách nhân viên kho.
		-- Mã sản phẩm thuộc nhóm hàng 253 - Nguyên liệu, vật liệu chính đã qua gia công.-> add/ update.
		-- PO: Production Order
		if (:object_type in ('202')  and :transaction_type in ('A','U')) then
			pQuery = '
			select count(a."ItemCode") 							
			from OWOR a
				inner join OITM c  on a."ItemCode"  = c."ItemCode"
			where 
				ifnull(c."ItmsGrpCod",0) ' || pCondition || ' and a."DocEntry" =  ' ||  :list_of_cols_val_tab_del ||	' limit 1 ';
			execute immediate pQuery into pAutho;
			if (:pAutho > 0) then
				error = 26011;
				error_message = 'Bạn chỉ được phép thực hiện mã sản phẩm' ||pmessage||' thuộc nhóm hàng 253 !';
				return; -- return error not add/update									
			end if;
		end if;	
		-- RFP: Receipt From Production
		if (:object_type in ('59')and :transaction_type in ('A')) then 
			pQuery = '
			select count(b."ItemCode")			
			from OIGN a
				inner join IGN1 b  on a."DocEntry" = b."DocEntry"
				inner join OITM c  on b."ItemCode"  = c."ItemCode"
			where 
				ifnull(c."ItmsGrpCod",0) ' || pCondition ||
				' and a."DocEntry" = ' ||  :list_of_cols_val_tab_del || ' and b."BaseType" = ' ||   :pBaseType||' limit 1 ';
			execute immediate pQuery into pAutho;
			if (:pAutho > 0) then
				error = 26012;
				error_message = 'Bạn chỉ được phép thực hiện mã sản phẩm' ||pmessage||' thuộc nhóm hàng 253 !';
				return; -- return error not add/update				
			end if;	
		end if;		
		-- IFP: Issue For Production	
		if (:object_type in ('60')and :transaction_type in ('A')) then 
			pQuery = '
			select count(*)			
			from OIGE a
				inner join IGE1 b  on a."DocEntry" = b."DocEntry"
			where 
				a."DocEntry" = ' ||  :list_of_cols_val_tab_del || 
				' and b."BaseType" = ' || :pBaseType ||						
				' and '|| pConditionIFP ;
			execute immediate pQuery into pAutho;
			if (:pAutho > 0) then
				error = 26013;
				error_message = 'Bạn chỉ được phép thực hiện với PO' ||pmessage||' bắt đầu bằng 9';
				return; -- return error not add/update	
			end if;										
		end if;						
		
	END IF;
-- ==== END Green Bean Decaf Tolling
-- Delivery chặn không cho lưu nếu trường Term = 'FOB' NHUNG KHONG TICK USE SHIPPED GOODS ACCOUNT Hoac Term = ''

	If (cast(object_type as nvarchar) in ('15','17') and transaction_type  in ('A')) then
		declare message1 nvarchar(200) = N'Bạn chưa tick vào Use Shipped Goods Account';
		declare message2 nvarchar(200) = N'Vui lòng chọn Delivery Term';
		declare pTerm nvarchar(10) = 'A';
		declare pUseShpdGd VarChar(1)= 'N';
		-- Delivery
		If (cast(object_type as nvarchar) = '15') then
			select ifnull("U_Term",'') as "U_Term","UseShpdGd" into pTerm,pUseShpdGd default '','N'
			from ODLN where "DocEntry"  = :list_of_cols_val_tab_del ;
		---- Return ket qua
			 if ifnull(:pTerm,'') = '' then
			 	error = 119;
			 	error_message = message2;
			 	return;
			 end if;	
			 if (ifnull(:pTerm,'') = 'FOB' and :pUseShpdGd = 'N') then
			 	error = 119;
			 	error_message = message1;
			 	return;
			 end if;		
		End if;
		--- Seles Order
		If (cast(object_type as nvarchar) = '17') then
			select ifnull("U_Term",'') as "U_Term","UseShpdGd" into pTerm,pUseShpdGd default '','N'
			from ORDR where "DocEntry"  = :list_of_cols_val_tab_del ;
		---- Return ket qua
			 if ifnull(:pTerm,'') = '' then
			 	error = 120;
			 	error_message = message2;
			 	return;
			 end if;	
			 if (ifnull(:pTerm,'') = 'FOB' and :pUseShpdGd = 'N') then
			 	error = 120;
			 	error_message = message1;
			 	return;
			 end if;	
		end if;
		/*
		-- Draft
		If (cast(object_type as nvarchar) = '112' and transaction_type = 'A') then
			select ifnull("U_Term",'') as "U_Term","UseShpdGd" into pTerm,pUseShpdGd default '','N'
			from ODRF where "ObjType" in ('15','17') and "DocEntry"  = :list_of_cols_val_tab_del ;
		---- Return ket qua
			 if ifnull(:pTerm,'') = '' then
			 	error = 121;
			 	error_message = message2;
			 	return;
			 end if;	
			 if (ifnull(:pTerm,'') = 'FOB' and :pUseShpdGd = 'N') then
			 	error = 121;
			 	error_message = message1;
			 	return;
			 end if;			
		end if;
		*/
	 		
	End if;	
----- END Term = 'FOB' =======	

-- == Chan newformat cho Journal Remark khong duoc vuot qua 241 ky tu:
	If (object_type = '18' and transaction_type = 'U') then
		declare pRemark nvarchar(254);
		SELECT "JrnlMemo" into pRemark default '' from OPCH where "DocEntry" = :list_of_cols_val_tab_del;
	 	if (length(pRemark) > 241) then
		 	error = 241;
		 	error_message = 'Journal Remark không được vượt quá 241 ký tự.(Đã vượt ' || length(pRemark) - 241 ||' ký tự)';
		 	return;	
	 	end if;
	end if;
--== END chan ky tu

end
