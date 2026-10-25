CREATE procedure ILD_R_TrackingPR
(		
	in fdate	timestamp,
	in tdate	timestamp,
	in dept 	table (Code Nvarchar(50)),
	in creator	table(Code Nvarchar(50)),
	in prNo		table (Code Nvarchar(50)),
	in Item		table (Code Nvarchar(50)),


	out tab_result table
	(
		 TenPhieu nvarchar(200),
		 SoPhieuPR nvarchar(50),
		 ItemCode nvarchar(100),
		 ItemName nvarchar(100),
		 Quantity decimal (21,6),
		 OpenQty decimal (21,6),
		 
		 Price decimal (21,6),
		 Amount decimal (21,6),
		 Rate	decimal (21,6),
		 Requester nvarchar(50),
		 DocDate date,
		 DepartmentCode  nvarchar(50),
		 Department  nvarchar(50),
		 Status		 nvarchar(50),
		 
		 -- PO
		SoPhieu_PO  nvarchar(50),
		Quantity_PO decimal (21,6),
		OpenQty_PO decimal (21,6), 
		Price_PO decimal (21,6), 
		Amount_PO decimal (21,6),
		Rate_PO	decimal (21,6), -- Ty gia PO line
		DocDate_PO  date,
		Creator_PO nvarchar(50),
		Status_PO nvarchar(50),
		-- GRPO
		SoPhieu_GRPO  nvarchar(50),
		Quantity_GRPO decimal (21,6),
		OpenQty_GRPO decimal (21,6), 
		Price_GRPO decimal (21,6), 
		Amount_GRPO decimal (21,6),
		Rate_GRPO	decimal (21,6), -- Ty gia PO line
		DocDate_GRPO  date,
		Creator_GRPO nvarchar(50),
		Status_GRPO nvarchar(50),
		-- A/P Invoice
		SoPhieu_AP  nvarchar(50),
		Quantity_AP decimal (21,6),
		OpenQty_AP decimal (21,6), 
		Price_AP decimal (21,6), 
		Amount_AP decimal (21,6),
		Rate_AP	decimal (21,6), -- Ty gia PO line
		DocDate_AP  date,
		Creator_AP nvarchar(50),
		
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
		SoPhieuPR nvarchar(50),
		ItemCode nvarchar(100),
		ItemName nvarchar(100),
		Quantity decimal (21,6),
		OpenQty decimal (21,6),
		 
		Price decimal (21,6),
		Amount decimal (21,6),
		Rate	decimal (21,6),
		Requester nvarchar(50),
		DocDate date,
		DepartmentCode  nvarchar(50),
		Department  nvarchar(50),
		Status		 nvarchar(50),
		 
		 -- PO
		SoPhieu_PO  nvarchar(50),
		Quantity_PO decimal (21,6),
		OpenQty_PO decimal (21,6), 
		Price_PO decimal (21,6), 
		Amount_PO decimal (21,6),
		Rate_PO	decimal (21,6), -- Ty gia PO line
		DocDate_PO  date,
		Creator_PO nvarchar(50),
		Status_PO nvarchar(50),
		-- GRPO
		SoPhieu_GRPO  nvarchar(50),
		Quantity_GRPO decimal (21,6),
		OpenQty_GRPO decimal (21,6), 
		Price_GRPO decimal (21,6), 
		Amount_GRPO decimal (21,6),
		Rate_GRPO	decimal (21,6), -- Ty gia PO line
		DocDate_GRPO  date,
		Creator_GRPO nvarchar(50),
		Status_GRPO nvarchar(50),
		-- A/P Invoice
		SoPhieu_AP  nvarchar(50),
		Quantity_AP decimal (21,6),
		OpenQty_AP decimal (21,6), 
		Price_AP decimal (21,6), 
		Amount_AP decimal (21,6),
		Rate_AP	decimal (21,6), -- Ty gia PO line
		DocDate_AP  date,
		Creator_AP nvarchar(50),
		
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
list_JE  = 
			Select N'BÁO CÁO TRACKING PURCHASE REQUEST'  as "TenPhieu",
  			-- PR
  			Nguon4_PR_PO_GRPO."SoPhieuPR",Nguon4_PR_PO_GRPO."ItemCode",Nguon4_PR_PO_GRPO."ItemName",
  			Nguon4_PR_PO_GRPO."Quantity",Nguon4_PR_PO_GRPO."OpenQty",Nguon4_PR_PO_GRPO."Price",Nguon4_PR_PO_GRPO."Amount",
  			Nguon4_PR_PO_GRPO."Rate",Nguon4_PR_PO_GRPO."Requester",Nguon4_PR_PO_GRPO."DocDate",Nguon4_PR_PO_GRPO."Department", Nguon4_PR_PO_GRPO."DepartmentCode", 
  			Nguon4_PR_PO_GRPO."Status",
			-- PO
			"SoPhieu_PO","Quantity_PO","OpenQty_PO","Price_PO","Amount_PO","Rate_PO","DocDate_PO", "Status_PO",
			-- GRPO
			"SoPhieu_GRPO","Quantity_GRPO","OpenQty_GRPO","Price_GRPO","Amount_GRPO","Rate_GRPO","DocDate_GRPO","Status_GRPO",

			-- A/P Invoice 
			    pch1."SoPhieu_AP",pch1."Quantity" as "Quantity_AP",pch1."OpenQty" as  "OpenQty_AP",
				pch1."Price" as   "Price_AP",pch1."Amount" as "Amount_AP",
				--pch1."Rate" as  "Rate_AP",
				1 as  "Rate_AP",
				pch1."DocDate" as "DocDate_AP"
From(
			Select Nguon3_PR_PO_2.*,
				   			    -- Phiếu nhập kho: GRPO
						    pdn1."SoPhieu_GRPO",pdn1."Quantity" as "Quantity_GRPO",pdn1."OpenQty" as  "OpenQty_GRPO",
							pdn1."Price" as   "Price_GRPO",pdn1."Amount" as "Amount_GRPO",
							--pdn1."Rate" as "Rate_GRPO" ,
							1 as "Rate_GRPO",
							pdn1."DocDate" as "DocDate_GRPO",
							pdn1."LineNumGRPO",
							pdn1."Status_GRPO",
							
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
													   por1."TargetType" as "PO_TargetType",
													   por1."TrgetEntry" as "PO_TrgetEntry",
													   -- Base của phiếu PO --> PR hoặc (Purchase Quotation)
													   por1."BaseType" as "PO_BaseType",
													   por1."BaseEntry" as "PO_BaseEntry",
													   por1."LineNumPO",
													   por1."Status_PO"
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
																   pr1."LineNum",
																   case when pr1."TargetType"  =  540000006 or  pr1."TargetType" = 22  then pr1."TargetType" else 0 end as  "PR_TargetType",
																   pr1."TrgetEntry" as "PR_TrgetEntry",
																   pr1."DocEntry" as "PR_DocEntry",
																   PQ."TrgetEntry" as "PQ_TrgetEntry",
																   PQ."TargetType" as  "PQ_TargetType",
																   (case when pr1."LineStatus" = 'C' then 'Closed' when pr1."LineStatus" = 'O' then 'Open' end) as "Status"
																   
															--- Purchase Request
															From PRQ1 pr1
															inner join OPRQ pr on pr."DocEntry"  = pr1."DocEntry"
															inner join OUDP dp on dp."Code"  =   pr."Department" 
															-- PQ (purchase Quotation)
															left  join  (select a."ItemCode",a."DocEntry","LineNum","TargetType","TrgetEntry", a."ObjType",  a."BaseEntry", a."BaseType",
																				"CANCELED"
																		from PQT1 a
																		inner join  OPQT b  on a."DocEntry" = b."DocEntry"
																		 
																		where "CANCELED"  =  'N'
																			) PQ  on   PQ."ItemCode"  =  pr1."ItemCode" and  PQ."LineNum"  =  pr1."LineNum"
																					   and ( (pr1."TrgetEntry"  = PQ."DocEntry" and   pr1."TargetType" = 540000006)
																							  		or (PQ."ObjType" = 540000006   and PQ."CANCELED"  =  'N'
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
								
							
										  left join (select b."DocNum" as "SoPhieu_PO","ItemCode", a."DocEntry","BaseLine",
										  							"TargetType","TrgetEntry" ,
										  							a."ObjType", a."BaseEntry", a."BaseType",
															        "Quantity", "OpenQty", "Price", "LineTotal" as "Amount", 
															        "Rate",
															        --TO_VARCHAR(TO_DATE(b."DocDate"),'DD/MM/YYYY') as "DocDate", 
															        b."DocDate" as "DocDate",
															        "LineNum" as "LineNumPO" ,
															         (case when a."LineStatus" = 'C' then 'Closed' when a."LineStatus" = 'O' then 'Open' end) as "Status_PO"
															        
															from POR1 a 
															inner join  OPOR b on a."DocEntry"  =  b."DocEntry"
															where  "CANCELED"  =  'N'
															 ) por1  on   por1."ItemCode"  =  Nguon2_PR_PO."ItemCode" 
															 and  por1."BaseLine"  =  Nguon2_PR_PO."LineNum"
															 			   and Nguon2_PR_PO."PO_DocEntry"  = por1."DocEntry"
						
															 ) as  Nguon3_PR_PO_2
			
						                 ---------- Join Dữ Liệu Nguôn 3 vơi Goods Receipt PO (GRPO)
										left join (select b."DocNum" as "SoPhieu_GRPO","ItemCode", a."DocEntry","BaseLine",
												a."BaseEntry", a."BaseType","TargetType","TrgetEntry" , 
										        "Quantity", "OpenQty", "Price", "LineTotal" as "Amount", 
										        "Rate",
										       -- TO_VARCHAR(TO_DATE(b."DocDate"),'DD/MM/YYYY') as "DocDate",
												b."DocDate" as "DocDate",
										        "LineNum" as "LineNumGRPO",
										        (case when "LineStatus" = 'C' then 'Closed' when "LineStatus" = 'O' then 'Open' end) as "Status_GRPO"
										        
										from PDN1 a 
										inner join  OPDN b on a."DocEntry"  =  b."DocEntry"
										where  "CANCELED"  =  'N'
										 ) pdn1  on   pdn1."ItemCode"  =  Nguon3_PR_PO_2."ItemCode" 
										 and  pdn1."BaseLine"  =  Nguon3_PR_PO_2."LineNumPO"
															  and Nguon3_PR_PO_2."PO_DocEntry"  = pdn1."BaseEntry" and   Nguon3_PR_PO_2."PO_TargetType" = 20 
			
			       
			
														 	 ) as  Nguon4_PR_PO_GRPO
										-----------
									    left join (select b."DocNum" as "SoPhieu_AP","ItemCode", a."DocEntry","BaseLine",
											a."BaseEntry", a."BaseType",a."TargetType","TrgetEntry" , 
							       			 "Quantity", "OpenQty", "Price", "LineTotal" as "Amount", 
							       			 "Rate",
							        		-- TO_VARCHAR(TO_DATE(b."DocDate"),'DD/MM/YYYY')b."DocDate" as "DocDate",  
											b."DocDate" as "DocDate",
							        		"LineNum"
							        
											from PCH1 a 
											inner join  OPCH b on a."DocEntry"  =  b."DocEntry"
											where  "CANCELED"  =  'N'
										 ) pch1  on   pch1."ItemCode"  =  Nguon4_PR_PO_GRPO."ItemCode" 
										   and  pch1."BaseLine"  =  Nguon4_PR_PO_GRPO."LineNumGRPO"
												  and Nguon4_PR_PO_GRPO."DocEntryAPInvoice"  = pch1."DocEntry" and   Nguon4_PR_PO_GRPO."iSTransTypeAPInvoice" = 1

			order by "DocDate" desc;
--- insert value

insert into :tbl (TenPhieu,SoPhieuPR,ItemCode,ItemName,
		  			Quantity,OpenQty,Price,Amount,
		  			Rate,Requester,DocDate,Department, DepartmentCode ,Status,
					-- PO
					SoPhieu_PO,Quantity_PO,OpenQty_PO,Price_PO,Amount_PO,Rate_PO,DocDate_PO, Status_PO,
					-- GRPO
					SoPhieu_GRPO,Quantity_GRPO,OpenQty_GRPO,Price_GRPO,Amount_GRPO,Rate_GRPO,DocDate_GRPO,Status_GRPO,
					-- A/P Invoice 
					   SoPhieu_AP, Quantity_AP,OpenQty_AP,Price_AP,Amount_AP,Rate_AP,DocDate_AP)
select "TenPhieu","SoPhieuPR","ItemCode","ItemName",
		"Quantity","OpenQty","Price","Amount",
		"Rate","Requester","DocDate","Department","DepartmentCode","Status",
		-- PO
		"SoPhieu_PO","Quantity_PO","OpenQty_PO","Price_PO","Amount_PO","Rate_PO","DocDate_PO", "Status_PO",
		-- GRPO
		"SoPhieu_GRPO","Quantity_GRPO","OpenQty_GRPO","Price_GRPO","Amount_GRPO","Rate_GRPO","DocDate_GRPO","Status_GRPO",
		-- A/P Invoice 
		"SoPhieu_AP", "Quantity_AP","OpenQty_AP","Price_AP","Amount_AP","Rate_AP","DocDate_AP"
from :list_JE a;

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
