CREATE procedure SBO_SP_UpdateData
(
	in object_type nvarchar(30), 				-- SBO Object Type
	in transaction_type nchar(1),			-- [A]dd, [U]pdate, [D]elete, [C]ancel, C[L]ose
	in num_of_cols_in_key int,
	in list_of_key_cols_tab_del nvarchar(255),
	in list_of_cols_val_tab_del nvarchar(255)
)
as
begin
------------------------------------------ update description, U_ItemDetail, ItemCode Phieu Goc  --> JE
----========================Item description về UDF ItemCode cho phiếu A/R Invoice
	If (cast(object_type as nvarchar) in ( '13') and transaction_type    = 'A') then
		if exists ( select  1 from  INV1 where "DocEntry"  = :list_of_cols_val_tab_del) then  
	
		AR_Invoice  =  select  "LineNum",  "ItemCode", "Dscription","U_ItemDetail"
				  from  INV1 where "DocEntry"  = :list_of_cols_val_tab_del ;
				  
		if exists (select 1 from :AR_Invoice) then
				--- update vao JE
				update a
				set a."U_ItemCode"  = b."ItemCode",
					a."U_ItemName"  = b."Dscription",
					a."U_ItemDetail" =  b."U_ItemDetail"
				from JDT1 a
				inner join  :AR_Invoice b on   a."DocLine"  = b."LineNum"
				where  a."TransType"  = 13 and    "CreatedBy"  = :list_of_cols_val_tab_del;
			 
			 		--and ifnull("InitRef3Ln",'')  <> '';
		end if;
	 end if;
	End If;
----========================Item description về UDF ItemCode cho phiếu A/R Credit Memo
	If (cast(object_type as nvarchar) in ( '14') and transaction_type    = 'A') then
		if exists ( select  1 from  RIN1 where "DocEntry"  = :list_of_cols_val_tab_del) then  
	
		AR_Credit_Memo  =  select  "LineNum",  "ItemCode", "Dscription","U_ItemDetail"
				  from  RIN1 where "DocEntry"  = :list_of_cols_val_tab_del ;
				  
		if exists (select 1 from :AR_Credit_Memo) then
				--- update vao JE
				update a
				set a."U_ItemCode"  = b."ItemCode",
					a."U_ItemName"  = b."Dscription",
					a."U_ItemDetail" =  b."U_ItemDetail"
				from JDT1 a
				inner join  :AR_Credit_Memo b on   a."DocLine"  = b."LineNum"
				where  a."TransType"  = 14 and    "CreatedBy"  = :list_of_cols_val_tab_del;
		end if;
	 end if;
	End If;
----========================Item description về UDF ItemCode cho phiếu Delivery
	If (cast(object_type as nvarchar) in ( '15') and transaction_type    = 'A') then
		if exists ( select  1 from  DLN1 where "DocEntry"  = :list_of_cols_val_tab_del) then  
	
		Delivery  =  select  "LineNum",  "ItemCode", "Dscription","U_ItemDetail"
				  from  DLN1 where "DocEntry"  = :list_of_cols_val_tab_del ;
				  
		if exists (select 1 from :Delivery) then
				--- update vao JE
				update a
				set a."U_ItemCode"  = b."ItemCode",
					a."U_ItemName"  = b."Dscription",
					a."U_ItemDetail" =  b."U_ItemDetail"
				from JDT1 a
				inner join  :Delivery b on   a."DocLine"  = b."LineNum"
				where  a."TransType"  = 15 and    "CreatedBy"  = :list_of_cols_val_tab_del;
		end if;
	 end if;
	End If;
----========================Item description về UDF ItemCode cho phiếu Return
	If (cast(object_type as nvarchar) in ( '16') and transaction_type    = 'A') then
		if exists ( select  1 from  RDN1 where "DocEntry"  = :list_of_cols_val_tab_del) then  
	
		tblReturns  =  select  "LineNum",  "ItemCode", "Dscription","U_ItemDetail"
				  from  RDN1 where "DocEntry"  = :list_of_cols_val_tab_del ;
				  
		if exists (select 1 from :tblReturns) then
				--- update vao JE
				update a
				set a."U_ItemCode"  = b."ItemCode",
					a."U_ItemName"  = b."Dscription",
					a."U_ItemDetail" =  b."U_ItemDetail"
				from JDT1 a
				inner join  :tblReturns b on   a."DocLine"  = b."LineNum"
				where  a."TransType"  = 16 and    "CreatedBy"  = :list_of_cols_val_tab_del;
		end if;
	 end if;
	End If;
	
----========================Item description về UDF ItemCode cho phiếu A/P Invoice
	If (cast(object_type as nvarchar) in ( '18') and transaction_type  in ('A','U')) then
		if exists ( select  1 from  PCH1 where "DocEntry"  = :list_of_cols_val_tab_del) then  
	
		AP_Invoice  =  select  a."LineNum",  a."ItemCode", a."Dscription",a."U_ItemDetail",substring(b."U_APInv",0,254) as "U_APInv"
				  from  PCH1 a inner join OPCH b on b."DocEntry" = a."DocEntry" where a."DocEntry"  = :list_of_cols_val_tab_del ;
				  
		if exists (select 1 from :AP_Invoice) then
				--- update vao JE
				if(transaction_type  in ('A')) then
					update a
					set a."U_ItemCode"  = b."ItemCode",
						a."U_ItemName"  = b."Dscription",
						a."U_ItemDetail" =  b."U_ItemDetail",
						a."U_InvNo" = b."U_APInv"
					from JDT1 a
					inner join  :AP_Invoice b on   a."DocLine"  = b."LineNum"
					where  a."TransType"  = 18 and    "CreatedBy"  = :list_of_cols_val_tab_del;		
				end if;				
		end if;
		-- Update Invno JE = APInvNo A/P Invoice.
		if exists (select top 1 "DocEntry" from  opch where "DocEntry"  = :list_of_cols_val_tab_del and length(ifnull("U_APInv",'')) > 0) then
			update a
			set a."U_InvNo" = substring(b."U_APInv",0,254)
			from JDT1 a
			inner join OPCH b on b."TransId" = a."TransId" 
		  	where a."TransType"  = 18 and a."CreatedBy" = :list_of_cols_val_tab_del
		  	and ifnull(a."U_InvNo",'') != cast(ifnull(b."U_APInv",'xxx') as nvarchar(254));	
	  	end if;
	 end if;
	End If;
----========================Item description về UDF ItemCode cho phiếu A/P Credit Invoice
	If (cast(object_type as nvarchar) in ( '19') and transaction_type    = 'A') then
		if exists ( select  1 from  RPC1 where "DocEntry"  = :list_of_cols_val_tab_del) then  
	
		AP_Credit_Invoice  =  select  "LineNum",  "ItemCode", "Dscription","U_ItemDetail"
				  from  RPC1 where "DocEntry"  = :list_of_cols_val_tab_del ;
				  
		if exists (select 1 from :AP_Credit_Invoice) then
				--- update vao JE
				update a
				set a."U_ItemCode"  = b."ItemCode",
					a."U_ItemName"  = b."Dscription",
					a."U_ItemDetail" =  b."U_ItemDetail"
				from JDT1 a
				inner join  :AP_Credit_Invoice b on   a."DocLine"  = b."LineNum"
				where  a."TransType"  = 19 and    "CreatedBy"  = :list_of_cols_val_tab_del;
		end if;
	 end if;
	End IF;
----========================Item description về UDF ItemCode cho phiếu Goods Receipt PO 
	If (cast(object_type as nvarchar) in ( '20') and transaction_type    = 'A') then
		if exists ( select  1 from  PDN1 where "DocEntry"  = :list_of_cols_val_tab_del) then  
	
		GRPO  =  select  "LineNum",  "ItemCode", "Dscription","U_ItemDetail"
				  from  PDN1 where "DocEntry"  = :list_of_cols_val_tab_del ;
				  
		if exists (select 1 from :GRPO) then
				--- update vao JE
				update a
				set a."U_ItemCode"  = b."ItemCode",
					a."U_ItemName"  = b."Dscription",
					a."U_ItemDetail" =  b."U_ItemDetail"
				from JDT1 a
				inner join  :GRPO b on   a."DocLine"  = b."LineNum"
				where  a."TransType"  = 20 and    "CreatedBy"  = :list_of_cols_val_tab_del;
		end if;
	 end if;
	End IF;
----========================Item description về UDF ItemCode cho phiếu Goods Return
	If (cast(object_type as nvarchar) in ( '21') and transaction_type    = 'A') then
		if exists ( select  1 from  RPD1 where "DocEntry"  = :list_of_cols_val_tab_del) then  
	
		GoodReturn  =  select  "LineNum",  "ItemCode", "Dscription","U_ItemDetail"
				  from  RPD1 where "DocEntry"  = :list_of_cols_val_tab_del ;
				  
		if exists (select 1 from :GoodReturn) then
				--- update vao JE
				update a
				set a."U_ItemCode"  = b."ItemCode",
					a."U_ItemName"  = b."Dscription",
					a."U_ItemDetail" =  b."U_ItemDetail"
				from JDT1 a
				inner join  :GoodReturn b on   a."DocLine"  = b."LineNum"
				where  a."TransType"  = 21 and    "CreatedBy"  = :list_of_cols_val_tab_del;
		end if;
	 end if;
	End IF;
----========================Item description về UDF ItemCode cho phiếu Inventory Transfer
	If (cast(object_type as nvarchar) in ( '67') and transaction_type    = 'A') then
		if exists ( select  1 from  WTR1 where "DocEntry"  = :list_of_cols_val_tab_del) then  
	
		Inventory_Transfer  =  select  "LineNum",  "ItemCode", "Dscription","U_ItemDetail"
				  from  WTR1 where "DocEntry"  = :list_of_cols_val_tab_del ;
				  
		if exists (select 1 from :Inventory_Transfer) then
				--- update vao JE
				update a
				set a."U_ItemCode"  = b."ItemCode",
					a."U_ItemName"  = b."Dscription",
					a."U_ItemDetail" =  b."U_ItemDetail"
				from JDT1 a
				inner join  :Inventory_Transfer b on   a."DocLine"  = b."LineNum"
				where  a."TransType"  = 67 and    "CreatedBy"  = :list_of_cols_val_tab_del;
		end if;
	 end if;
	End IF;
----========================Item description về UDF ItemCode cho phiếu IFP, GI
	If (cast(object_type as nvarchar) in ( '60') and transaction_type    = 'A') then
		if exists ( select  1 from  IGE1 where "DocEntry"  = :list_of_cols_val_tab_del ) then  
	
		Issued  =  select  "LineNum",  "ItemCode", "Dscription","U_ItemDetail"
				  from  IGE1 where "DocEntry"  = :list_of_cols_val_tab_del ;
				  
		if exists (select 1 from :Issued) then
				--- update vao JE
				update a
				set a."U_ItemCode"  = b."ItemCode",
					a."U_ItemName"  = b."Dscription",
					a."U_ItemDetail" =  b."U_ItemDetail"
				from JDT1 a
				inner join  :Issued b on   a."DocLine"  = b."LineNum"
				where  a."TransType"  = 60 and    "CreatedBy"  = :list_of_cols_val_tab_del;
		end if;
	 end if;
	End If;
----========================Item description về UDF ItemCode cho phiếu RFP, GR 
	If (cast(object_type as nvarchar) in ( '59') and transaction_type    = 'A') then
			if exists ( select  1 from  IGN1 where "DocEntry"  = :list_of_cols_val_tab_del ) then  
	
				receipt  =  select  "LineNum",  "ItemCode","Dscription","U_ItemDetail" 
				 			 from  IGN1 where "DocEntry"  = :list_of_cols_val_tab_del  
				  				and "PriceBefDi" <>0;
				  
				if exists (select 1 from :receipt) then
							--- update vao JE
							update a
							set a."U_ItemCode"  = b."ItemCode",
								a."U_ItemName"  = b."Dscription",
								a."U_ItemDetail"  = b."U_ItemDetail"
							from JDT1 a
							inner join  :receipt b on   a."DocLine"  = b."LineNum"
							where  a."TransType"  = 59 and    "CreatedBy"  = :list_of_cols_val_tab_del;
					
				end if;
	
	 
	 	end if;
	End If;
	/* Chua su dung
----========================Item description về UDF ItemCode cho phiếu Landed Costs. 67 
	If (cast(object_type as nvarchar) in ( '67') and transaction_type    = 'A') then
			if exists ( select  1 from  IPF1 where "DocEntry"  = :list_of_cols_val_tab_del ) then  
	
				landed  =  select  "LineNum",  "ItemCode","Dscription"
				 			 from  IPF1 where "DocEntry"  = :list_of_cols_val_tab_del ;
				  
				if exists (select 1 from :landed) then
							--- update vao JE
							update a
							set a."U_ItemCode"  = b."ItemCode",
								a."U_ItemName"  = b."Dscription"							
							from JDT1 a
							inner join  :landed b on   a."DocLine"  = b."LineNum"
							where  a."TransType"  = 67 and    "CreatedBy"  = :list_of_cols_val_tab_del;				
				end if; 
	 	end if;
	End If;
----========================Item description về UDF ItemCode cho phiếu Inventory Revaluation. 162 
	If (cast(object_type as nvarchar) in ( '162') and transaction_type    = 'A') then
			if exists ( select  1 from  MRV1 where "DocEntry"  = :list_of_cols_val_tab_del ) then  
	
				revaluation  =  select  "LineNum",  "ItemCode","Dscription"
				 			 from  MRV1 where "DocEntry"  = :list_of_cols_val_tab_del ;
				  
				if exists (select 1 from :revaluation) then
							--- update vao JE
							update a
							set a."U_ItemCode"  = b."ItemCode",
								a."U_ItemName"  = b."Dscription"							
							from JDT1 a
							inner join  :revaluation b on   a."DocLine"  = b."LineNum"
							where  a."TransType"  = 162 and    "CreatedBy"  = :list_of_cols_val_tab_del;				
				end if; 
	 	end if;
	End If;
*/

--======================Update nam 2026
--===== Auto Journal Remark new format: Theo Misa 13416,13437,13438
	IF(:transaction_type IN ('A','C')) THEN --A
		IF( object_type in ('13','15','18','204','46','24','20','16','21','67','30')) THEN
		
			declare pRemark nvarchar(254) ;
			declare pTransId int ;
			declare pTransId_Old int ;
			declare pDocNum int = 0;
			declare pNumber int = 0;
			declare pCancel char(1)= '' ;
			
			if(:object_type  = '30') then
				select "TransType", "BaseRef" into  object_type, pDocNum default '30',0 from OJDT where "TransId" = list_of_cols_val_tab_del;		
			end if;
/* Tam khoa
			-- Lay tinh trang Cancel phieu
			SELECT top 1 tt.CANCELED,tt."TransId" into pCancel,pTransId_Old default '' ,0
			FROM (
				SELECT CANCELED,"TransId" FROM OINV WHERE "ObjType" = object_type and CANCELED = 'C' and ("DocEntry" = list_of_cols_val_tab_del or "DocNum" = pDocNum)
				union all
				SELECT CANCELED,"TransId" FROM OPCH WHERE "ObjType" = object_type and CANCELED = 'C' and ("DocEntry" = list_of_cols_val_tab_del or "DocNum" = pDocNum)
				union all
				SELECT "Canceled","TransId" FROM OVPM WHERE "ObjType" = object_type and "Canceled" = 'Y' and ("DocEntry" = list_of_cols_val_tab_del or "DocNum" = pDocNum)
				union all
				SELECT "Canceled","TransId" FROM ORCT WHERE "ObjType" = object_type and "Canceled" = 'Y' and ("DocEntry" = list_of_cols_val_tab_del or "DocNum" = pDocNum)
				union all
				SELECT CANCELED,"TransId" FROM OPDN WHERE "ObjType" = object_type and CANCELED = 'C' and ("DocEntry" = list_of_cols_val_tab_del or "DocNum" = pDocNum)
				union all
				SELECT CANCELED,"TransId" FROM ORDN WHERE "ObjType" = object_type and CANCELED = 'C' and ("DocEntry" = list_of_cols_val_tab_del or "DocNum" = pDocNum)
				union all
				SELECT CANCELED,"TransId" FROM ORPD WHERE "ObjType" = object_type and CANCELED = 'C' and ("DocEntry" = list_of_cols_val_tab_del or "DocNum" = pDocNum)
				union all
				SELECT CANCELED,"TransId" FROM OWTR WHERE "ObjType" = object_type and CANCELED = 'Y' and ("DocEntry" = list_of_cols_val_tab_del or "DocNum" = pDocNum)
			) tt ;
																		
if(:pCancel != '') then 
*/
				IF(:object_type = '13') THEN -- A/R Invoice
					SELECT CANCELED,"TransId"  into pCancel,pTransId_Old default '' ,0 
					FROM OINV WHERE "ObjType" = object_type and CANCELED = 'C' and ("DocEntry" = list_of_cols_val_tab_del or "DocNum" = pDocNum);
					if (:pCancel != '') then
						select 'Cancellation-'||u."TransId"||'-'||ifnull(u."JrnlMemo",'') as "JnRemark",s."TransId" into pRemark,pTransId
						from (
							select b."DocEntry",b."TransId",a."BaseEntry"
							from OINV b
							left join INV1 a on b."DocEntry" = a."DocEntry"
							where b.CANCELED='C'and b."DocEntry" = :list_of_cols_val_tab_del
							group by b."DocEntry",b."TransId", a."BaseEntry"
							) s
						inner join OINV u on s."BaseEntry" = u."DocEntry";
							
						update OINV set "JrnlMemo" = :pRemark where "DocEntry" = :list_of_cols_val_tab_del ;
					end if;
				ELSEIF(:object_type = '15') THEN -- Delivery Note
					SELECT CANCELED,"TransId"  into pCancel,pTransId_Old default '' ,0 
					FROM ODLN WHERE "ObjType" = object_type and CANCELED = 'C' and ("DocEntry" = list_of_cols_val_tab_del or "DocNum" = pDocNum);
					if (:pCancel != '') then
						select 'Cancellation-'||u."TransId"||'-'||ifnull(u."JrnlMemo",'') as "JnRemark",s."TransId" into pRemark,pTransId
						from (
							select b."DocEntry",b."TransId",a."BaseEntry"
							from ODLN b
							left join DLN1 a on b."DocEntry" = a."DocEntry"
							where b.CANCELED='C'and b."DocEntry" = :list_of_cols_val_tab_del
							group by b."DocEntry",b."TransId", a."BaseEntry"
							) s
						inner join ODLN u on s."BaseEntry" = u."DocEntry";
							
						update ODLN set "JrnlMemo" = :pRemark where "DocEntry" = :list_of_cols_val_tab_del ;
					end if;
				ELSEIF (:object_type = '18')  THEN -- A/P
					SELECT CANCELED,"TransId"  into pCancel,pTransId_Old default '' ,0 
					FROM OPCH WHERE "ObjType" = object_type and CANCELED = 'C' and ("DocEntry" = list_of_cols_val_tab_del or "DocNum" = pDocNum);
					if (:pCancel != '') then
						select 'Cancellation-'||u."TransId"||'-'||ifnull(u."JrnlMemo",'') as "JnRemark",s."TransId" into pRemark,pTransId
						from (
							select b."DocEntry",b."TransId",a."BaseEntry"
							from OPCH b
							left join PCH1 a on b."DocEntry" = a."DocEntry"
							where b.CANCELED='C'and b."DocEntry" = :list_of_cols_val_tab_del
							group by b."DocEntry",b."TransId", a."BaseEntry"
							) s
						inner join OPCH u on s."BaseEntry" = u."DocEntry";		
									
						update OPCH set "JrnlMemo" = :pRemark where "DocEntry" = :list_of_cols_val_tab_del ;	
					end if;									
				ELSEIF (:object_type = '20')  THEN -- GRPO
					SELECT CANCELED,"TransId"  into pCancel,pTransId_Old default '' ,0 
					FROM OPDN WHERE "ObjType" = object_type and CANCELED = 'C' and ("DocEntry" = list_of_cols_val_tab_del or "DocNum" = pDocNum);
					if (:pCancel != '') then
						select 'Cancellation-'|| CASE WHEN u."TransId" IS NULL THEN ''  ELSE u."TransId" || '-' END || ifnull(u."JrnlMemo",'') as "JnRemark",s."TransId" into pRemark,pTransId
						from (
							select b."DocEntry",b."TransId",a."BaseEntry"
							from OPDN b
							left join PDN1 a on b."DocEntry" = a."DocEntry"
							where  b."DocEntry" = :list_of_cols_val_tab_del
							group by b."DocEntry",b."TransId", a."BaseEntry"
							) s
						inner join OPDN u on s."BaseEntry" = u."DocEntry";
						update OPDN set "JrnlMemo" = :pRemark where "DocEntry" = :list_of_cols_val_tab_del ;
					END IF;					
				ELSEIF (:object_type = '16')  THEN --Returns
					SELECT CANCELED,"TransId"  into pCancel,pTransId_Old default '' ,0 
					FROM ORDN WHERE "ObjType" = object_type and CANCELED = 'C' and ("DocEntry" = list_of_cols_val_tab_del or "DocNum" = pDocNum);
					if (:pCancel != '') then
						select 'Cancellation-'||u."TransId"||'-'||ifnull(u."JrnlMemo",'') as "JnRemark",s."TransId" into pRemark,pTransId
						from (
							select b."DocEntry",b."TransId",a."BaseEntry"
							from ORDN b
							left join RDN1 a on b."DocEntry" = a."DocEntry"
							where b.CANCELED='C'and b."DocEntry" = :list_of_cols_val_tab_del
							group by b."DocEntry",b."TransId", a."BaseEntry"
							) s
						inner join ORDN u on s."BaseEntry" = u."DocEntry";					
						update ORDN set "JrnlMemo" = :pRemark where "DocEntry" = :list_of_cols_val_tab_del ;
					end if;					
				ELSEIF (:object_type = '21')  THEN --Goods Return
					SELECT CANCELED,"TransId"  into pCancel,pTransId_Old default '' ,0 
					FROM ORPD WHERE "ObjType" = object_type and CANCELED = 'C' and ("DocEntry" = list_of_cols_val_tab_del or "DocNum" = pDocNum);
					if (:pCancel != '') then
						select 'Cancellation-'||u."TransId"||'-'||ifnull(u."JrnlMemo",'') as "JnRemark",s."TransId" into pRemark,pTransId
						from (
							select b."DocEntry",b."TransId",a."BaseEntry"
							from ORPD b
							left join RPD1 a on b."DocEntry" = a."DocEntry"
							where b.CANCELED='C'and b."DocEntry" = :list_of_cols_val_tab_del
							group by b."DocEntry",b."TransId", a."BaseEntry"
							) s
						inner join ORPD u on s."BaseEntry" = u."DocEntry";					
						update ORPD set "JrnlMemo" = :pRemark where "DocEntry" = :list_of_cols_val_tab_del ;
					end if;				
				ELSEIF (:object_type = '46' and transaction_type = 'A')  THEN -- Chay su kien Cancel- C, cancel chinh no, k phat sinh phieu moi.
					--select 'Cancellation-'|| list_of_cols_val_tab_del ||'-'||"Memo" into pRemark  default ''  from OJDT where "TransId" = pTransId_Old;					
					SELECT "Canceled","TransId" into pCancel,pTransId_Old default '' ,0 
					FROM OVPM WHERE "ObjType" = object_type and "Canceled" = 'Y' and ("DocEntry" = list_of_cols_val_tab_del or "DocNum" = pDocNum);
					if (:pCancel != '') then
						select 'Cancellation-'|| pTransId_Old ||'-'||"Memo" into pRemark  default ''  from OJDT where "TransId" = pTransId_Old;
						--in (select "TransId" from OVPM  where "DocNum" = pDocNum);
						pTransId = list_of_cols_val_tab_del;
						update OVPM set "JrnlMemo" = :pRemark where "TransId" = pTransId_Old ; --"DocNum" = pDocNum;
					end if;
					
				ELSEIF (:object_type = '24'  and transaction_type = 'A')  THEN
					SELECT "Canceled","TransId" into pCancel,pTransId_Old default '' ,0 
					FROM ORCT WHERE "ObjType" = object_type and "Canceled" = 'Y' and ("DocEntry" = list_of_cols_val_tab_del or "DocNum" = pDocNum);
					if (:pCancel != '') then
						select 'Cancellation-'|| pTransId_Old ||'-'||ifnull("Memo",'') into pRemark  default ''  from OJDT where "TransId" = pTransId_Old;
	
						update ORCT set "JrnlMemo" = :pRemark where "TransId" = pTransId_Old ; --"DocNum" = pDocNum;
						pTransId = list_of_cols_val_tab_del;
					end if;
				ELSEIF (:object_type = '67'  and :transaction_type IN ('C'))  THEN	
					SELECT "DocNum", "TransId" into pDocNum,pTransId_Old FROM OWTR WHERE ("DocEntry" = list_of_cols_val_tab_del);
					select "TransId" into pTransId default 0 from owtr where "Comments" like '%'|| :pDocNum ||'%' limit 1;		
					select  'Cancellation-'|| pTransId_Old ||'-'|| "Memo" into pRemark default '' from ojdt where "TransId" = pTransId_Old;
					UPDATE OWTR SET "JrnlMemo" = :pRemark WHERE "TransId" = :pTransId;
					
				END IF ;
				-- Cap nhat Remark vao JE or :object_type = '30'
				IF (pRemark != '' and ifnull(pTransId,0) > 0) THEN
					-- update phieu cancellation
					--UPDATE OJDT SET "Memo" = pRemark WHERE "TransId" = pTransId_Old ;
					--UPDATE JDT1 SET "LineMemo" = pRemark WHERE "TransId" = pTransId_Old ;
					-- update phieu Revert (Cancellation)
					UPDATE OJDT SET "Memo" = pRemark WHERE "TransId" = pTransId ;
					UPDATE JDT1 SET "LineMemo" = pRemark WHERE "TransId" = pTransId ;
					
				END IF;					
				-- OPDN: khong co/khong add JE.	
			IF (:pCancel = '') THEN
				-- Update new format cho AP Invoice va Outgoing Payments
				IF(:transaction_type = 'A') THEN
					IF (:object_type = '18')  THEN
						SELECT "JrnlMemo" INTO pRemark DEFAULT '' FROM UFC_GetNewFormat(:object_type,list_of_cols_val_tab_del);
						IF length(pRemark) > 0 THEN
							update OPCH set "JrnlMemo" = :pRemark where "DocEntry" = :list_of_cols_val_tab_del ;
							UPDATE OJDT SET "Memo" = :pRemark WHERE "CreatedBy" = :list_of_cols_val_tab_del;
							UPDATE JDT1 SET "LineMemo" = :pRemark WHERE "CreatedBy" = :list_of_cols_val_tab_del;
						END IF;
					end if;
					IF (:object_type = '204')  THEN -- A/P Down Payment
						SELECT "JrnlMemo" INTO pRemark DEFAULT '' FROM UFC_GetNewFormat(:object_type,list_of_cols_val_tab_del);
						IF length(pRemark) > 0 THEN
							update ODPO set "JrnlMemo" = :pRemark where "DocEntry" = :list_of_cols_val_tab_del ;
							UPDATE OJDT SET "Memo" = :pRemark WHERE "CreatedBy" = :list_of_cols_val_tab_del;
							UPDATE JDT1 SET "LineMemo" = :pRemark WHERE "CreatedBy" = :list_of_cols_val_tab_del;
						END IF;
					end if;					
					IF (:object_type = '46')  THEN
						SELECT "JrnlMemo" INTO pRemark DEFAULT '' FROM UFC_GetNewFormat(:object_type,list_of_cols_val_tab_del);
						IF length(pRemark) > 0 THEN
							update OVPM set "JrnlMemo" = :pRemark where "DocEntry" = :list_of_cols_val_tab_del ;
							UPDATE OJDT SET "Memo" = :pRemark WHERE "CreatedBy" = :list_of_cols_val_tab_del;
							UPDATE JDT1 SET "LineMemo" = :pRemark WHERE "CreatedBy" = :list_of_cols_val_tab_del;							
						END IF	;					
					end if;	
				END IF;			 		
			END IF;
		
		end if;
	end if;
---======== end new format

-- Misa 17365: Clear thong tin lien quan eInvoice khi lam AR tu phieu DN.	
	If (cast(object_type as nvarchar) in ( '13') and transaction_type = 'A') then
		if exists ( select  1 from  INV1 a left join OINV b on b."DocEntry" = a."DocEntry" where a."DocEntry"  = :list_of_cols_val_tab_del and a."BaseType" = 15
					AND length(ifnull(b."U_InvNo",'')||ifnull(b."U_eInvTransID",'')||ifnull(b."U_eInvMTC",'')) > 0) then
			UPDATE OINV
			SET "U_invSym" = '', "U_InvSerial" = '',"U_InvNo"= '',"U_InvDate"= NULL,"U_eInvTransID" = NULL
			,"U_eInvMTC" = NULL,"U_ARReason" = NULL
			WHERE "DocEntry"  = :list_of_cols_val_tab_del;
		end if;	
	end if;	
	
end
