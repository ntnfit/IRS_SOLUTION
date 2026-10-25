CREATE procedure ILD_R_TrackingPR_SCFC
(		
	in fdate	timestamp,
	in tdate	timestamp,
	in dept 	table (Code Nvarchar(50)),
	in creator	table(Code Nvarchar(50)),
	in prNo		table (Code int),
	in Item		table (Code Nvarchar(50)),


	out tab_result table
	(
		 TenPhieu nvarchar(200),
		 SoPhieuPR int,
		 ItemCode nvarchar(100),
		 ItemName nvarchar(100),
		 ItemDesc	nvarchar(500), -- ItemDetails
		 
		 CostCenter nvarchar(100), -- Ten
		 CostCTCode nvarchar(50), -- Code
		 Quantity decimal (21,6),
		 UOM_PR	nvarchar(50),
		 Currency_PR nvarchar(20),
		 GenerateDate_PR date, -- Ngày tạo phiếu PR chính thức (pr draft add)
		 OpenQty decimal (21,6),
		 
		 Price decimal (21,6),
		 Amount decimal (21,6),
		 Rate	decimal (21,6),
		 Requester nvarchar(50),
		 DocDate date, -- ngay tao phieu PR draft
		 DepartmentCode  nvarchar(50),
		 Department  nvarchar(50),
		 DraftKeyPO	 nvarchar(50),
		 Status		 nvarchar(50),
		 --
		 GLAccount nvarchar(20),
		 
		 -- PO
		SoPhieu_PO  int,
		Quantity_PO decimal (21,6),
		OpenQty_PO decimal (21,6), 
		Price_PO decimal (21,6), 
		Amount_PO decimal (21,6),
		Rate_PO	decimal (21,6), -- Ty gia PO line
		DocDate_PO  date,
		Creator_PO nvarchar(50),
		Status_PO nvarchar(50),
		Remarks_PO nvarchar(254),
		GenerateDate_PO date, -- Ngày tạo phiếu PO chính thức (Po draft add)
		--
		DeliveryDate date,  -- Delivery date (lấy từ PO)
		CustomsClearanceDate date, -- lấy từ Udo
		
		-- GRPO
		SoPhieu_GRPO  int,
		Quantity_GRPO decimal (21,6),
		OpenQty_GRPO decimal (21,6), 
		Price_GRPO decimal (21,6), 
		Amount_GRPO decimal (21,6),
		Rate_GRPO	decimal (21,6), -- Ty gia PO line
		DocDate_GRPO  date,
		Creator_GRPO nvarchar(50),
		Status_GRPO nvarchar(50),
		
		VendorCode nvarchar(50),
		VendorName	nvarchar(250),
		UOM_PO nvarchar(50),
		Currency_PO  nvarchar(20),
		-- A/P Invoice
		SoPhieu_AP  int,
		Quantity_AP decimal (21,6),
		OpenQty_AP decimal (21,6), 
		Price_AP decimal (21,6), 
		Amount_AP decimal (21,6),
		Rate_AP	decimal (21,6), -- Ty gia PO line
		DocDate_AP  date,
		Creator_AP nvarchar(50),
		Status_AP nvarchar(50),
		--
		
		
		-- Thoong tin cong ty 
		CompnyName		nvarchar(300),
		CompnyAddr		nvarchar(500),
		CompTel			nvarchar(20),
		CompWeb         nvarchar(150),
		CompFax 		nvarchar(20),
		CompEmail		nvarchar(150),
		CompTaxNum		nvarchar(50),
		--format number
		AmountDec       smallint,
        QtyDec          smallint,
        PriceDec        smallint,
        RateDec         smallint,
        PercentDec      smallint,
        DecSep          nvarchar(1),
        ThousSep        nvarchar(1)
	
	) 
	
)
as
begin
--create local temporary table #tbl as (select * from :tab_result);
declare tbl table
(
		 TenPhieu nvarchar(200),
		 SoPhieuPR int,
		 ItemCode nvarchar(100),
		 ItemName nvarchar(100),
		 ItemDesc	nvarchar(500), -- ItemDetails
		 
		 CostCenter nvarchar(100), -- Ten
		 CostCTCode nvarchar(50), -- Code
		 Quantity decimal (21,6),
		 UOM_PR	nvarchar(50),
		 Currency_PR nvarchar(20),
		 GenerateDate_PR date, -- Ngày tạo phiếu PR chính thức (pr draft add)
		 OpenQty decimal (21,6),
		 
		 Price decimal (21,6),
		 Amount decimal (21,6),
		 Rate	decimal (21,6),
		 Requester nvarchar(50),
		 DocDate date, -- ngay tao phieu PR draft
		 DepartmentCode  nvarchar(50),
		 Department  nvarchar(50),
		 DraftKeyPO	 nvarchar(50),
		 Status		 nvarchar(50),
		 --
		 GLAccount nvarchar(20),
		 
		 -- PO
		SoPhieu_PO  int,
		Quantity_PO decimal (21,6),
		OpenQty_PO decimal (21,6), 
		Price_PO decimal (21,6), 
		Amount_PO decimal (21,6),
		Rate_PO	decimal (21,6), -- Ty gia PO line
		DocDate_PO  date,
		Creator_PO nvarchar(50),
		Status_PO nvarchar(50),
		Remarks_PO nvarchar(254),
		GenerateDate_PO date, -- Ngày tạo phiếu PO chính thức (Po draft add)
		--
		DeliveryDate date,  -- Delivery date (lấy từ PO)
		CustomsClearanceDate date, -- lấy từ Udo
		
		-- GRPO
		SoPhieu_GRPO  int,
		Quantity_GRPO decimal (21,6),
		OpenQty_GRPO decimal (21,6), 
		Price_GRPO decimal (21,6), 
		Amount_GRPO decimal (21,6),
		Rate_GRPO	decimal (21,6), -- Ty gia PO line
		DocDate_GRPO  date,
		Creator_GRPO nvarchar(50),
		Status_GRPO nvarchar(50),
		
		VendorCode nvarchar(50),
		VendorName	nvarchar(250),
		UOM_PO nvarchar(50),
		Currency_PO  nvarchar(20),
		-- A/P Invoice
		SoPhieu_AP  int,
		Quantity_AP decimal (21,6),
		OpenQty_AP decimal (21,6), 
		Price_AP decimal (21,6), 
		Amount_AP decimal (21,6),
		Rate_AP	decimal (21,6), -- Ty gia PO line
		DocDate_AP  date,
		Creator_AP nvarchar(50),
		Status_AP nvarchar(50),
		-- Thoong tin cong ty 
		CompnyName		nvarchar(300),
		CompnyAddr		nvarchar(500),
		CompTel			nvarchar(20),
		CompWeb         nvarchar(150),
		CompFax 		nvarchar(20),
		CompEmail		nvarchar(150),
		CompTaxNum		nvarchar(50),
		--format number
		AmountDec       smallint,
        QtyDec          smallint,
        PriceDec        smallint,
        RateDec         smallint,
        PercentDec      smallint,
        DecSep          nvarchar(1),
        ThousSep        nvarchar(1)
);
insert into :tbl (TenPhieu,
					SoPhieuPR,ItemCode,ItemName,ItemDesc,UOM_PR,GLACCOUNT,Currency_PR,GenerateDate_PR,
					CostCenter,CostCTCode,
		  			Quantity,OpenQty,Price,Amount,
		  			Rate,Requester,DocDate,Department, DepartmentCode ,Status,
		  			DraftKeyPO,
					-- PO
					SoPhieu_PO,Quantity_PO,OpenQty_PO,Price_PO,Amount_PO,Rate_PO,DocDate_PO, GenerateDate_PO,Status_PO,
					VendorCode,VendorName,UOM_PO,Currency_PO,
					Creator_PO,Remarks_PO,DeliveryDate,
					-- GRPO
					SoPhieu_GRPO,Quantity_GRPO,OpenQty_GRPO,Price_GRPO,Amount_GRPO,Rate_GRPO,DocDate_GRPO,Status_GRPO,
					-- A/P Invoice 
					   SoPhieu_AP, Quantity_AP,OpenQty_AP,
					   Price_AP,Amount_AP,Rate_AP,DocDate_AP,Status_AP)
Select 		N'BÁO CÁO TRACKING PURCHASE REQUEST'  as "TenPhieu",
  			-- PR, Hien thi len Report cot CostCenter lay theo ma OcrCode, khong lay OcrName.
  			Nguon4_PR_PO_GRPO."SoPhieuPR",Nguon4_PR_PO_GRPO."ItemCode",Nguon4_PR_PO_GRPO."ItemName",
  			Nguon4_PR_PO_GRPO."ItemDetails",
  			Nguon4_PR_PO_GRPO."UOM_PR",
  			Nguon4_PR_PO_GRPO."GLAccount",  			
  			Nguon4_PR_PO_GRPO."Currency_PR",
  			Nguon4_PR_PO_GRPO."GenerateDate_PR",
  			Nguon4_PR_PO_GRPO."OcrCode", 
  			Nguon4_PR_PO_GRPO."OcrCode",
  			Nguon4_PR_PO_GRPO."Quantity",Nguon4_PR_PO_GRPO."OpenQty",Nguon4_PR_PO_GRPO."Price",Nguon4_PR_PO_GRPO."Amount",
  			Nguon4_PR_PO_GRPO."Rate",Nguon4_PR_PO_GRPO."Requester",Nguon4_PR_PO_GRPO."DocDate",Nguon4_PR_PO_GRPO."Department", Nguon4_PR_PO_GRPO."DepartmentCode", 
  			Nguon4_PR_PO_GRPO."Status",
  			Nguon4_PR_PO_GRPO."PO_Draft_Key",
			-- PO
			"SoPhieu_PO","Quantity_PO","OpenQty_PO","Price_PO","Amount_PO","Rate_PO","DocDate_PO", "GenerateDate_PO","Status_PO",
			"VendorCode","VendorName","UOM_PO","Currency_PO",
			"Creator_PO","Remarks_PO","DeliveryDate",
			-- GRPO
			"SoPhieu_GRPO","Quantity_GRPO","OpenQty_GRPO","Price_GRPO","Amount_GRPO","Rate_GRPO","DocDate_GRPO","Status_GRPO",
			

			-- A/P Invoice 
			    pch1."SoPhieu_AP",pch1."Quantity" as "Quantity_AP",pch1."OpenQty" as  "OpenQty_AP",
				pch1."Price" as   "Price_AP",pch1."Amount" as "Amount_AP",
				--pch1."Rate" as  "Rate_AP",
				case when pch1."CANCELED" = 'C' then -1 else 1 end as  "Rate_AP",
				pch1."DocDate" as "DocDate_AP",pch1."Status_AP"
From(
			Select Nguon3_PR_PO_2.*,
				   			    -- Phiếu nhập kho: GRPO
						    pdn1."SoPhieu_GRPO",pdn1."Quantity" as "Quantity_GRPO",pdn1."OpenQty" as  "OpenQty_GRPO",
							pdn1."Price" as   "Price_GRPO",pdn1."Amount" as "Amount_GRPO",
							--pdn1."Rate" as "Rate_GRPO" ,
							case when pdn1."CANCELED" = 'C' then -1 else 1 end as "Rate_GRPO",
							pdn1."DocDate" as "DocDate_GRPO",
							pdn1."LineNumGRPO",
							pdn1."Status_GRPO",
							pdn1."DocEntry" as "KeyInvoiceCancel",
							case when pdn1."TargetType" =  18  then 1 else 0 end as "iSTransTypeAPInvoice",
							case when pdn1."TargetType" =  18 and ifnull(pdn1."TrgetEntry",0) > 0  then pdn1."TrgetEntry" else 0 end as "DocEntryAPInvoice"
			From (
					Select Nguon2_PR_PO.*,
						   -- PO
						   por1."SoPhieu_PO",por1."Quantity" as "Quantity_PO",por1."OpenQty" as  "OpenQty_PO",
													   por1."Price" as   "Price_PO",por1."Amount" as "Amount_PO",
													   --por1."Rate" as "Rate_PO",
													   1 as "Rate_PO",
													   por1."DocDate" as "DocDate_PO",
													   por1."CreateDate" as "GenerateDate_PO",
													   por1."TargetType" as "PO_TargetType",
													   por1."TrgetEntry" as "PO_TrgetEntry",
													   -- Base của phiếu PO --> PR hoặc (Purchase Quotation)
													   por1."BaseType" as "PO_BaseType",
													   por1."BaseEntry" as "PO_BaseEntry",
													   por1."LineNumPO",
													   por1."Status_PO",
													   por1."VendorCode",
													   por1."VendorName",
													   por1."UOM_PO",
													   por1."Currency_PO",
													   por1."Creator_PO",
													   por1."Remarks_PO",
													   por1."DeliveryDate"
					From (
								Select  Nguon1_PR.*,
									    case when ifnull("PR_TargetType",0) = 540000006 and  ifnull("PQ_TargetType",0) =  22 then  "PQ_TrgetEntry"
									         when ifnull("PR_TargetType",0) = 22 and ifnull("PR_TrgetEntry",0)  <> 0 then  "PR_TrgetEntry"
									     else 0  end as "PO_DocEntry"
								From  
								(
								--- Purchase Request
								select pr."DocNum" as "SoPhieuPR",
																   pr1."ItemCode",
																   pr1."Dscription" as  "ItemName",
																   pr1."U_ItemDetail" as "ItemDetails",
																   pr1."unitMsr" as  "UOM_PR",
																   pr1."AcctCode" as  "GLAccount",
																   pr1."Currency" as  "Currency_PR",
																   pr."CreateDate" as "GenerateDate_PR",
																   pr1."Quantity",  
																   pr1."OpenQty",
																   pr1."Price",
																   pr1."LineTotal" as "Amount",
																   --pr1."Rate",
																   1 as "Rate",
																   pr."Requester",
																   --TO_VARCHAR(TO_DATE(pr."DocDate"),'DD/MM/YYYY') as "DocDate",
																	pr."DocDate",
																   dp."Name" as "Department",
																   dp."Code" as  "DepartmentCode",
																   dp2."OcrName", dp2."OcrCode",
																   pr1."LineNum",
																   case when pr1."TargetType"  =  540000006 or  pr1."TargetType" = 22  then pr1."TargetType" else 0 end as  "PR_TargetType",
																   case when Po_draft."PO_DocEntry" is null then pr1."TrgetEntry" else Po_draft."PO_DocEntry" end as "PR_TrgetEntry",
																   pr1."DocEntry" as "PR_DocEntry",
																   PQ."TrgetEntry" as "PQ_TrgetEntry",
																   PQ."TargetType" as  "PQ_TargetType",
																   
																   case when pr."CANCELED"  ='Y' then 'Canceled'
																   else (case when pr1."LineStatus" = 'C' then 'Closed' when pr1."LineStatus" = 'O' then 'Open' end) 
																   end as "Status",
																   Po_draft."PO_Draft_Key",Po_draft."PO_DocNum"
															--- Purchase Request.
															From PRQ1 pr1
															inner join OPRQ pr on pr."DocEntry"  = pr1."DocEntry"
															inner join OUDP dp on dp."Code"  =   pr."Department" 
															left join OOCR dp2 on dp2."OcrCode" = pr1."OcrCode"
															left join (
																select distinct 
																	b."DocEntry" as "PO_Draft_Key"
																	,c."BaseRef"
																	,c."BaseEntry"
																	,c."BaseLine" 
																	,PO."DocNum" as "PO_DocNum"  ,PO."DocEntry" as "PO_DocEntry" 
																from ODRF b
																left join OPOR PO on b."DocNum"= PO."DocNum" 
																left join DRF1 c on c."DocEntry" = b."DocEntry"
																WHERE PO."DocNum" is null  and b."ObjType" = '22' and b."WddStatus" != 'C'			
																union all
																select distinct 
																	NULL as "PO_Draft_Key"
																	,R1."BaseRef"
																	,R1."BaseEntry"
																	,R1."BaseLine"
																	,PO."DocNum" as "PO_DocNum"  ,PO."DocEntry" as "PO_DocEntry" 
																from OPOR PO
																left join POR1 R1 ON R1."DocEntry" = PO."DocEntry"								
																) Po_draft on  Po_draft."BaseRef" = cast(pr."DocNum"   as nvarchar) and Po_draft."BaseLine" = pr1."LineNum"
															-- PQ (purchase Quotation)
															left  join  (select a."ItemCode",a."DocEntry","LineNum","TargetType","TrgetEntry", a."ObjType",  a."BaseEntry", a."BaseType",
																				"CANCELED"
																		from PQT1 a
																		inner join  OPQT b  on a."DocEntry" = b."DocEntry"
																		 
																		--where "CANCELED"  =  'N'
																			) PQ  on   PQ."ItemCode"  =  pr1."ItemCode" and  PQ."LineNum"  =  pr1."LineNum"
																					   and ( (pr1."TrgetEntry"  = PQ."DocEntry" and   pr1."TargetType" = 540000006)
																							  		or (PQ."ObjType" = 540000006   --and PQ."CANCELED"  =  'N'
																							  		       and PQ."BaseEntry"  = pr1."DocEntry" 
																							  			   and  PQ."DocEntry"  = pr1."DocEntry"  )) 
																							  			   
																							  			   
																 		WHERE 	pr."DocDate" between fdate and tdate
																			 	and 
																				(
																					ifnull((select STRING_AGG(Code) from :creator), '') = '' 
																					or   pr."Requester"  in (select Code from :creator)
																			 	)
																				and 
																				(
																					ifnull((select STRING_AGG(Code) from :dept), '') = '' 
																					or   pr."Department"  in (select Code from :dept)
																			 	) 
																			 	and 
																				(
																					ifnull((select STRING_AGG(cast(Code as nvarchar(50))) from :prNo), '') = '' 
																					or   cast(pr."DocNum" as nvarchar(50))  in (select Code from :prNo)
																			 	) 
																			 	and 
																				(
																					ifnull((select STRING_AGG(cast(Code as nvarchar(50))) from :Item), '') = '' 
																					or   cast(pr1."ItemCode" as nvarchar(50))  in (select Code from :Item)
																			 	) 
																		 
																			 				  			   
																				 ) as Nguon1_PR 
														 ---- End Purchase Request   
																							  			   
																		
																									  			   
																							  			   
																) as  Nguon2_PR_PO
								
											-- hung.tran. 23/09/2025. them dieu kien and Nguon2_PR_PO."PO_DocNum" = por1."SoPhieu_PO". Loc trung					
										  left join (select b."DocNum" as "SoPhieu_PO","ItemCode", a."DocEntry","BaseLine",
										  							"TargetType","TrgetEntry" ,
										  							a."ObjType", a."BaseEntry", a."BaseType",
															        "Quantity", "OpenQty", "Price", "LineTotal" as "Amount", 
															        "Rate",
															        --TO_VARCHAR(TO_DATE(b."DocDate"),'DD/MM/YYYY') as "DocDate", 
															        b."DocDate" as "DocDate",
															        b."CreateDate"  as "CreateDate",
															        "LineNum" as "LineNumPO" ,
															        CASE when b."CANCELED"  ='Y' then 'Canceled'
															          else (case when a."LineStatus" = 'C' then 'Closed' when a."LineStatus" = 'O' then 'Open' end) 
															        END as "Status_PO",
															        b."CardCode" as  "VendorCode",
															        b."CardName" as  "VendorName",
															        a."unitMsr" as  "UOM_PO",
															        a."Currency" as  "Currency_PO",
															        b."Comments" as "Remarks_PO",
															        c."USER_CODE" as "Creator_PO",
															        b."DocDueDate" as  "DeliveryDate"
															        
															from POR1 a 
															inner join  OPOR b on a."DocEntry"  =  b."DocEntry"
															left join OUSR c on b."UserSign"  = c."USERID"
															--where  "CANCELED"  =  'N'
															 ) por1  on   por1."ItemCode"  =  Nguon2_PR_PO."ItemCode" 
															 and  por1."BaseLine"  =  Nguon2_PR_PO."LineNum"
															 and Nguon2_PR_PO."PO_DocEntry"  = por1."DocEntry"
															 and Nguon2_PR_PO."PO_DocNum" = por1."SoPhieu_PO"
						
															 ) as  Nguon3_PR_PO_2
			
						                 ---------- Join Dữ Liệu Nguôn 3 vơi Goods Receipt PO (GRPO). 
										------------Lay luon c phieu GRPO Cancellation.23/09/2025
										left join (
												select b."DocNum" as "SoPhieu_GRPO",a."ItemCode", a."DocEntry",a."BaseLine",
													a."BaseEntry", a."BaseType",a."TargetType",a."TrgetEntry" , 
											        a."Quantity", a."OpenQty", a."Price", a."LineTotal" as "Amount", 
											        a."Rate",
											       -- TO_VARCHAR(TO_DATE(b."DocDate"),'DD/MM/YYYY') as "DocDate",
													b."DocDate" as "DocDate",
											        a."LineNum" as "LineNumGRPO",b."CANCELED",
											       CASE when b."CANCELED"  ='Y' then 'Canceled'
											        	ELSE (case when a."LineStatus" = 'C' then 'Closed' when "LineStatus" = 'O' then 'Open' end) 
											        END as "Status_GRPO"
										        
													from PDN1 a 
													inner join  OPDN b on a."DocEntry"  =  b."DocEntry"
													where b."CANCELED"  !='C'
													union all
													select  b."DocNum" as "SoPhieu_GRPO"
													 		,a."ItemCode", a."DocEntry"
													 		,(select "BaseLine" from PDN1 where "DocEntry" = a."BaseEntry" and "ItemCode" = a."ItemCode" and "LineNum" = a."BaseLine") "BaseLine",
															(select max("BaseEntry") from PDN1 where "DocEntry" = a."BaseEntry" group by "DocEntry") as "BaseEntry" 
															,a."BaseType"
															,a."TargetType",a."TrgetEntry" , 
													        a."Quantity"*-1 as "Quantity", a."OpenQty", a."Price"*-1 as "Price", a."LineTotal"*-1 as "Amount", 
													        a."Rate"*-1 as "Rate",b."DocDate" as "DocDate",
													        a."LineNum" as "LineNumGRPO",b."CANCELED",
													        'Cancellation' as "Status_GRPO"			        
													from PDN1 a 
													left join  OPDN b on a."DocEntry"  =  b."DocEntry"
													where b."CANCELED"  ='C' and a."BaseType" = 20										
											 ) pdn1  on   pdn1."ItemCode"  =  Nguon3_PR_PO_2."ItemCode" 
											 and  pdn1."BaseLine"  =  Nguon3_PR_PO_2."LineNumPO"
																  and Nguon3_PR_PO_2."PO_DocEntry"  = pdn1."BaseEntry" and   Nguon3_PR_PO_2."PO_TargetType" = 20 
	
													) as  Nguon4_PR_PO_GRPO
										-----------
									    left join (select b."DocNum" as "SoPhieu_AP","ItemCode", a."DocEntry","BaseLine",
											a."BaseEntry", a."BaseType",a."TargetType","TrgetEntry" , 
							       			 "Quantity", "OpenQty", "Price", "LineTotal" as "Amount", 
							       			 "Rate",b."DocDate" as "DocDate","LineNum",b."CANCELED"	
							       			 ,CASE when b."CANCELED"  ='Y' then 'Canceled'
											        	ELSE (case when b."DocStatus" = 'C' then 'Closed' when b."DocStatus" = 'O' then 'Open' end)  
											        END as "Status_AP"						        
											from PCH1 a 
											inner join  OPCH b on a."DocEntry"  =  b."DocEntry"
											where b."CANCELED"  !='C'
											union all -- lay AP cancellation
											select b."DocNum" as "SoPhieu_AP","ItemCode", a."DocEntry"
											,(select "BaseLine" from PCH1 where "DocEntry" = a."BaseEntry" and "ItemCode" = a."ItemCode" and "LineNum" = a."BaseLine") "BaseLine",
											(select max("BaseEntry") from PCH1 where "DocEntry" = a."BaseEntry" group by "DocEntry") as "BaseEntry" 
											, a."BaseType",a."TargetType","TrgetEntry" , 
							       			 "Quantity"*-1 as "Quantity", "OpenQty"*-1 as "OpenQty", "Price"*-1 as "Price", "LineTotal"*-1 as "Amount", 
							       			 "Rate"*-1 as "Rate",b."DocDate" as "DocDate","LineNum"	,b."CANCELED"	
							       			 ,'Cancellation' as "Status_AP"					        
											from PCH1 a 
											inner join  OPCH b on a."DocEntry"  =  b."DocEntry"
											where b."CANCELED"  ='C' and a."BaseType" = 18 
										 ) pch1  on   pch1."ItemCode"  =  Nguon4_PR_PO_GRPO."ItemCode" 
										   and  pch1."BaseLine"  =  Nguon4_PR_PO_GRPO."LineNumGRPO"
												  and( Nguon4_PR_PO_GRPO."DocEntryAPInvoice"  = pch1."DocEntry" 
													  and   Nguon4_PR_PO_GRPO."iSTransTypeAPInvoice" = 1
													  or pch1."BaseEntry" = Nguon4_PR_PO_GRPO."KeyInvoiceCancel"
												  	)

			order by "DocDate" desc;
--==== Cap nhat lai Quantity, Price, Total = null cho cac truonghop sau:
--TH1: Nếu phiếu phía sau (A/P Invoice hoặc GRPO) có Doc Status là Cancellation thì sẽ không thể hiện thông tin của 3 cột 
--		Quantity, Unit Price, Total trong toàn bộ các phiếu trước đó (GRPO, PO, PR)
update :tbl
set Quantity_GRPO = 0,OpenQty_GRPO = 0,Price_GRPO = 0,Amount_GRPO = 0
,Quantity_PO = 0,OpenQty_PO = 0,Price_PO = 0,Amount_PO = 0
,Quantity = 0,OpenQty = 0,Price = 0,Amount = 0
where Status_AP = 'Cancellation';
-- Nếu phiếu GRPO có Doc Status là Canceled thì sẽ không thể hiện thông tin của 3 cột Quantity, Unit Price, Total trong toàn bộ các phiếu trước đó (PO, PR)
UPDATE :tbl
set Quantity_PO = 0,OpenQty_PO = 0,Price_PO = 0,Amount_PO = 0
,Quantity = 0,OpenQty = 0,Price = 0,Amount = 0
where Status_GRPO IN ('Canceled','Cancellation');

-- Neu NUL se gan = 0
update :tbl set Quantity = 0 		where Quantity is null;
update :tbl set OpenQty = 0 		where OpenQty is null;
update :tbl set Price = 0 			where Price is null;
update :tbl set Amount = 0 			where Amount is null;
update :tbl set Quantity_PO = 0 	where Quantity_PO is null;
update :tbl set OpenQty_PO = 0 		where OpenQty_PO is null;
update :tbl set Price_PO = 0 		where Price_PO is null;
update :tbl set Amount_PO = 0 		where Amount_PO is null;
update :tbl set Quantity_GRPO = 0 	where Quantity_GRPO is null;
update :tbl set OpenQty_GRPO = 0  	where OpenQty_GRPO is null;
update :tbl set Price_GRPO = 0 		where Price_GRPO is null;
update :tbl set Amount_GRPO = 0 	where Amount_GRPO is null;
update :tbl set QUANTITY_AP = 0 	where QUANTITY_AP is null;
update :tbl set PRICE_AP = 0 		where PRICE_AP is null;

-- Customs clearance date
update a
set CustomsClearanceDate  =   (select  max("U_ClearanceDate") from "@UCD_H"  b where a.SoPhieu_PO  = b."U_PO")
from :tbl a;
----------------------- Update

    update a set
        AmountDec    = b."SumDec",
        QtyDec        = b."QtyDec",
        PriceDec    = b."PriceDec",
        RateDec        = b."RateDec",
        PercentDec    = b."PercentDec",
        DecSep        = b."DecSep",
        ThousSep    = b."ThousSep"
    from :tbl a, OADM b;


--- Return table 
tab_result = select * from :tbl order by DocDate desc;

end
