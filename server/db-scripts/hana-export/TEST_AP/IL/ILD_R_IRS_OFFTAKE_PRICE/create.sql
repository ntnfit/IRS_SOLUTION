CREATE procedure ILD_R_IRS_OFFTAKE_PRICE
(
	in FromDate date,
	in ToDate date
)
as
BEGIN
 
		--declare FromDate date = '2024-10-01';
		--declare ToDate date = '2025-02-28';
		declare mcode nvarchar(50);
		declare mtotalf nvarchar(200);
		declare v_thangnam nvarchar(10);
		declare v_tothangnam nvarchar(10);
		
		declare v_sql NVARCHAR(10000);
	    declare v_thang INT ;
	    declare v_nam INT;
	    declare v_index INT DEFAULT 1;
	    declare v_total INT;
	    declare v_avg_count INT;
	    declare v_to_thang INT;
	    declare v_to_nam INT;	    
		DECLARE nround int = 6;
		declare Amount_NVL table 
		(
			ItemCode		nvarchar(50),
			WOID			int,
			QtyTotal		numeric(21,6),
			Qty				numeric(21,6),	-- -- số lượng tôchi tiết của từng Batch/SSCC
			Amount			numeric(21,6),
			DocType			nvarchar(20),
			ItmsGrpCod		int,
			DocDate			date
		);
		declare total6229,total6279 decimal(28,10);
		declare pro11 table
		(
			STDQTY  DECIMAL(21,6),
			STDAMOUNT  DECIMAL(21,6),
			WONO VARCHAR(50), 
			ITEMCODE VARCHAR(50), 
			DISNO  VARCHAR(50),
			U_Group nvarchar(20),
			DocDate date
		);
		declare tbl table
		 (
		  	"ObjType"			nvarchar(30) ,
			"DocEntry"			int,
			"LineNum"			int,
			"DocNum"			int,
			"StartDate"			timestamp,
			"FGCode"			nvarchar(50),
			"FGName"			nvarchar(100),
			"PlanQty"			numeric(21,6),
			"CmpltQty"			numeric(21,6),
			"ItemCode"			nvarchar(50),
			"ItemName"			nvarchar(100),
			"IssueQty"			numeric(21,6),
			"IssuedQty"			numeric(21,6),
			"OnHand"			numeric(21,6),
			"OpnQty"			numeric(21,6),
			"NeedQty"			numeric(21,6),
			"WhsCode"			nvarchar(30),
			"WhsName"			nvarchar(100),
			"ItmsGrpCod"		int,
			"ItmsGrpNam"		nvarchar(100),
			"IsOpn"				char(1),
			"IsStock"			char(1),
			"TotQty"			numeric(21,6),
			"Status"			nvarchar(30),
			"AmountFG"			numeric(21,6),
			"UnitPriceFG"		numeric(21,6),
			"Amount"			numeric(21,6),
			"UnitPrice"			numeric(21,6),
			"PlanCompltQty"		numeric(21,6),
			"PlanCompltAmount"	numeric(21,6),
			"BaseQty"			numeric(21,6),
			"BomRate"			numeric(21,6),
			"DueDate"			date,
			"DueDate_Late"		int,
			"PO_Status" 		nvarchar(30),
			"DiffQty"  			numeric(21,6),
			"DiffAmount"  		numeric(21,6),
			"Actual_BOM_Rate1"	numeric(21,6),
			"Actual_BOM_Rate2"	numeric(21,6),
			"Actual_BOM_Rate"   numeric(21,6),
			--
			"U_Group" nvarchar(20),
			"DocDate" 		date
			);
-- JE
		declare JE table
		(
			
			Account nvarchar(50),
			Amount numeric(21,6),
			Amount_Debit_Credit numeric(21,6)
			
		);
		declare pro04 table
		(
			Qty numeric(21,6),
			Amount numeric(21,6),
			ItemCode nvarchar(50),
			ItemName nvarchar(150),
			GrpCode nvarchar(50),
			GrpName nvarchar(100),
			WOID nvarchar(50),
			WONo nvarchar(50),
			DocDate date
		);
		declare tblSum table
		(
			"CmpltQty" numeric(21,6),
			"PlanCompltQty" numeric(21,6),
			"PlanCompltAmount" numeric(21,6),
			"Amount" numeric(21,6),  
			"IssuedQty" numeric(21,6),  
			"Month" int,  
			"Year" int, 
			"FG_Group" nvarchar(100),
			"FG_Group_Code" nvarchar(20),
			"ItemCode_Group" nvarchar(100),
			"FG_ProType" nvarchar(50)
		);

		declare tblEstimateCost table 
		(
			Account 		nvarchar(20),
			OffsetAcct		nvarchar(20),
			CostCenter		nvarchar(10),
			Debit			numeric(28,10),
			Credit			numeric(28,10),
			Amount_Debit_Credit			numeric(28,10),
			Thang			int,
			Nam				int
	
		);
		declare tblOffTake_Price_List table
		(
			STT	int,
			GroupName  nvarchar(100),
			Code nvarchar(50)
		);
		declare tblOffTake_Price_Setting table
		(
			ID int,
			STT	int,
			Code nvarchar(50),
			Name nvarchar(100),
			GroupName  nvarchar(100),
			Account nvarchar (254),
			Exclude_Account nvarchar (254),
			CostCenter nvarchar (254),
			Exclude_OffsetAcct  nvarchar (254),
			Balance decimal(19,6)
		);
		declare tblBalance_Account_Dpt table
		(
			Balance	decimal(19,6),
			Account nvarchar(50),
			GroupName nvarchar(100),
			Thang int,
			Nam int,
			CostCenter nvarchar(50),
			Offtake_CostCenter nvarchar(254)
		);
		declare tblBalance_Account_Dpt_Sum table
		(
			STT int,
			Balance	decimal(19,6),
			GroupName nvarchar(100),
			Thang int,
			Nam int,
			Code nvarchar(50)
		);
		
		
		
		
		
--- Fromdate va Todate
	Fromdate   = cast( year(Fromdate)||'-'|| month(Fromdate)||'-01' as date);
	Todate   = cast( LAST_DAY(Todate) as date);
------ lay du lieu tu store pro 04
		CALL USP_RPT_PROD_04(FromDate,ToDate,'','','','','N', datapro04);
		insert into :pro04 (Qty,Amount,ItemCode,ItemName,GrpCode,GrpName,WOID,WONo, DocDate)
		select sum(Qty) as Qty,sum(Amount) as Amount,ItemCode,ItemName,GrpCode,GrpName,WOID,WONo, DocDate
												from :datapro04 
												where StartDate between FromDate and ToDate
												group by ItemCode,ItemName,GrpCode,GrpName,WOID,WONo, docdate;
---
	
	
		insert into :tbl (
			"ObjType", "DocEntry", "LineNum", "DocNum", "StartDate","DocDate",
			"FGCode", "FGName", "PlanQty", "CmpltQty",
			
			"ItemCode", "ItemName", "IssueQty", "IssuedQty",  "OnHand",
			 
			"WhsCode", "ItmsGrpCod", "ItmsGrpNam", "Status" ,"BaseQty", "U_Group")
		select
			a."ObjType",
			a."DocEntry",
			b."LineNum",
			a."DocNum",
			a."StartDate",
			a."StartDate" as "DocDate", 
			
			c."ItemCode",
			c."ItemName",
			a."PlannedQty",
			a."CmpltQty",
			
			d."ItemCode",
			d."ItemName",
			b."PlannedQty",
			b."IssuedQty",
			IFNULL(f."OnHand",0),
			
			b."wareHouse",
			e."ItmsGrpCod",
			e."ItmsGrpNam",
			case when  a."Status" = 'R' then 'Released' else 'Closed' end as "Status",-- r, l
			--ROW_NUMBER() OVER (PARTITION BY a."DocEntry",  c."ItemCode") as "STT"
			b."BaseQty",
			--
			e."U_Group"
	
			 
		from
			OWOR a
			inner join WOR1 b on a."DocEntry" = b."DocEntry"
			inner join OITM c on a."ItemCode" = c."ItemCode"
			inner join OITM d on b."ItemCode" = d."ItemCode"
			inner join OITB e on c."ItmsGrpCod" = e."ItmsGrpCod"
			left join OITW f on b."ItemCode" = f."ItemCode" and b."wareHouse" = f."WhsCode"
		where
			a."StartDate" between FromDate and ToDate
			and a."Status" in ('R','L')
			and a."Type" <>  'D'
			and (a."CmpltQty" > 0)
			
			and (b."IssueType" = 'M')
			
			and not (a."Status" = 'L' and b."IssuedQty" = 0);
	
--
		update a
		set a."CmpltQty" = (select sum(b.Qty)  from   :pro04 b where  a."FGCode"  = b.ItemCode and a."DocEntry" = b.WOID)
		from :tbl a 
		where a."DocNum" not in ('612400042','612400045','612400046');
		
		--
		update a
		set a."AmountFG" = (select sum(b.Amount)  from   :pro04 b where  a."FGCode"  = b.ItemCode and a."DocEntry" = b.WOID)
		from :tbl a 
		where a."DocNum" not in ('612400042','612400045','612400046');
		
		---
	 	update a
		set a."CmpltQty" = b.Qty, a."AmountFG"  = b.Amount
		from :tbl a 
		inner join (select ItemCode, Wono ,WoID, sum(Qty) Qty, sum (Amount)  Amount  from  :pro04 group by  ItemCode, Wono, WOID ) b on a."FGCode"  = b.ItemCode and a."DocEntry" = b.WOID
		 where a."DocNum"   in ('612400042','612400045','612400046');
--
--
	insert into :tbl ( "DocEntry","DocNum",
						"FGCode","FGName",
						"ItmsGrpCod", "ItmsGrpNam", 
						"CmpltQty","AmountFG", "LineNum",  "DocDate")
	select WOID,WONo,
		ItemCode,ItemName,
		GrpCode,GrpName,
		
		Qty,Amount, 911, DocDate
		
	from :pro04 a 
	where ItemCode not in (select distinct "FGCode" from :tbl b where a.WOID = b."DocEntry" );
--- update line num
	update a
	set a."StartDate" =  b."StartDate", a."Status" = b."Status"
	from  :tbl  a
--inner join (select distinct "StartDate","Status", "DocEntry" from :tbl  where  "LineNum"  <> 911  ) b on   a."DocEntry" = b."DocEntry"
	inner join OWOR b on a."DocEntry" = b."DocEntry" 
	where "LineNum"  = 911 and ifnull(a."StartDate",'') ='';
---- update line num va planqty
	update a
	set "LineNum" = b."LineNum",
		"PlanQty" =  b."PlannedQty" * -1,
		"BaseQty"  = b."BaseQty"
	from :tbl a
	inner join (select  b."LineNum", b."PlannedQty", b."DocEntry", b."BaseQty"
				from OWOR a inner join WOR1 b on a."DocEntry" = b."DocEntry" 
				where a."StartDate" between FromDate and ToDate
					and a."Status" in ('R','L') and a."Type" <>  'D'
					and (b."PlannedQty" < 0)) b on  a."DocEntry" = b."DocEntry"  -- PlannedQty<0: rework
	 
	where  a."LineNum"  = 911;
	
		update :tbl set "OpnQty" = CASE when IFNULL("IssuedQty",0) >= "IssueQty" then 0 else "IssueQty"-IFNULL("IssuedQty",0) end where "Status" = 'R';
			
		update :tbl set "NeedQty" = CASE when "OpnQty" > "OnHand" then "OpnQty" - "OnHand" else 0 end where "Status" = 'R';
			
		update a set "IsStock" = 'N' from :tbl a, :tbl b where  b."NeedQty" > 0 and a."Status" = 'R';
		update a set "IsOpn" = 'Y' from :tbl a, :tbl b where   b."OpnQty" > 0 and a."Status" = 'R';
			
		update a set "TotQty" = b."Quantity" from :tbl a, (select x."DocEntry", SUM(x."IssueQty") "Quantity" from :tbl x where x."IssueQty" > 0 group by x."DocEntry") b where a."DocEntry" = b."DocEntry";
			
		update a set "NeedQty"= ifnull("IssueQty",0) - ifnull("IssuedQty",0)  from :tbl a;
		
		delete from :tbl where ifnull("IssueQty",0) < 0;
		
		--- update gia tri nguyen vat lieu
		insert into :Amount_NVL ( ItemCode,WOID, QtyTotal,Qty, Amount,DocType, ItmsGrpCod, DocDate )
			
		select   a."ItemCode", d."DocEntry" , 
						a."InQty"-a."OutQty",e."Quantity",
						a."TransValue" ,
						a."TransType",
						g."ItmsGrpCod",
						a."DocDate"
						
				 			
				from "OINM" a
					left join "OWOR" d on a."AppObjAbs"=d."DocEntry"
					left join IBT1 e on e."BaseEntry" = a."CreatedBy" and a."ItemCode" = e."ItemCode" and e."BaseType" = a."TransType" 
								and   a."DocLineNum"  =  e."BaseLinNum"
					left join OBTN f on e."BatchNum"=f."DistNumber" and e."ItemCode"=f."ItemCode"
					inner join OITM g on g."ItemCode"  = a."ItemCode"
					left join (select  "DocNum"
								from OIGN a
								inner join IGN1 b on a."DocEntry"  =  b."DocEntry"
								where b."BaseType" = 202 and b."IsByPrdct" ='Y' and b."BaseLine" is not null
								) h on h."DocNum" = a."BASE_REF"
				where a."TransType"  in  (60,59) --and  a."AppObjType"='C' 
				   and (a."ApplObj"='202'   OR a."BASE_REF" in ('322418143', '322418144','332416371','332416372'))
						and a."DocDate" between FromDate and ToDate
						and  h."DocNum" is null ;
		-- 
		update x set Qty = (QtyTotal) from :Amount_NVL x where Qty is null or Qty = 0;
		update x set Qty = (- Qty) from :Amount_NVL x where DocType = 59 and Qty < 0;
		update x set Qty = (- Qty)from :Amount_NVL x where DocType = 60 and Qty > 0;
		
		update a set Amount =   (a.Qty * a.Amount)/ (case when QtyTotal = 0 then 1 else QtyTotal end)  from :Amount_NVL a ;
		----- update amount NVL la rework
		update a
		set "Amount" =   case when  b."Amount" <0 then b."Amount" * -1 else b."Amount" end
		from :tbl a
		inner join (select sum(Amount) as "Amount",WOID,ItemCode from :Amount_NVL b 
		
		where  ItmsGrpCod = 121  and DocType in (59,  60) group by  WOID,  ItemCode)b on b.WOID = a."DocEntry" --  tam bo dkien: and   
							and b.ItemCode = a."ItemCode";
							
		----- update amount NVL <> rework
		update a
		set "Amount" =   case when  b."Amount" <0 then b."Amount" * -1 else b."Amount" end
		from :tbl a
		inner join (select sum(Amount) as "Amount",WOID,ItemCode from :Amount_NVL b 
		where DocType  in ( 59,60) and ItmsGrpCod <> 121  group by  WOID,  ItemCode)b on b.WOID = a."DocEntry"
							and b.ItemCode = a."ItemCode"; 
--
---- Disassebly
	
	insert into :pro11 (STDQTY,STDAMOUNT, WONO, ITEMCODE, DISNO, DocDate )
	select STDQTY,STDAMOUNT, WONO, ITEMCODE, DISNO, POSTINGDATE
	from  PRO11_DATA_TEMP
	where POSTINGDATE between Fromdate and todate;
--- insert thêm dư liệu từ bảng vât lý COSTING_STANDARDCOST
	insert into :pro11 (STDQTY,STDAMOUNT, WONO, ITEMCODE, DISNO , U_Group, DocDate)
	select STDQTY,STDAMOUNT, WONO, ITEMCODE, DISNO, U_Group,  POSTINGDATE
	from  COSTING_STANDARDCOST
	where POSTINGDATE between Fromdate and todate;
	
	 
	Update a
	set  a."IssuedQty" = ifnull( a."IssuedQty",0) +  ifnull((select sum(STDQTY) from  :pro11 b
														 where ifnull(DisNo,'') <> '' and a."DocNum" = b.WONO
														 		and a."ItemCode"  = b.ItemCode ),0)
	from :tbl a
	where a."ItmsGrpCod" in (125,126, 233) or ifnull(a."U_Group",'')   in ('MC_TTMC','LB_TTLB') ;
	
	
--
	Update a
	set "Amount" = ifnull("Amount",0) +    ifnull((select sum(STDAMOUNT) from  :pro11 b
														 where ifnull(DisNo,'') <> '' and a."DocNum" = b.WONO
														 		and a."ItemCode"  = b.ItemCode ),0)
	from :tbl a
	where  a."ItmsGrpCod" in (125,126, 233) or ifnull(a."U_Group",'')  in  ('MC_TTMC','LB_TTLB');
	 
	
---
		update :tbl
		set "UnitPriceFG"  =   "AmountFG"/(case when "CmpltQty"  =0 then 1 else "CmpltQty" end),
			"UnitPrice"  =   "Amount"/(case when "IssuedQty"  =0 then 1 else "IssuedQty" end),
			"PlanCompltQty" = "CmpltQty" * "BaseQty";
--	
	update a
	set  "DueDate" = b."DueDate",
		"DueDate_Late" = DAYS_BETWEEN ( b."DueDate",CURRENT_DATE),
		"PO_Status"  =  b."U_POStatus"
	from :tbl a
	inner join OWOR b on a."DocEntry" =  b."DocEntry";	
	
	update :tbl set "LineNum" = 911 where "ItmsGrpCod"  = 121; 
---
			update  a
			set "PlanQty"  = NULL,  "CmpltQty"  = NULL,  "AmountFG"  =NULL, "UnitPriceFG" = NULL
			from :tbl a
			where  "LineNum" >0  and "ItmsGrpCod"= '120' and "LineNum" <> 911;
			 
			
-----
	update a
	set  "FGCode"  = b."FGCode", "FGName" = b."FGName", "DueDate"  = b."DueDate",
		 "DueDate_Late"  = b."DueDate_Late",
		 "PO_Status" = b."PO_Status"
	from :tbl  a 
	inner join  (select * from :tbl ) b on a."DocEntry"  = b."DocEntry"
	where a."FGCode" is null;
--
	Update a
	set "BomRate"  = c."PlannedQty" / b."PlannedQty"
	from :tbl a 
	inner join OWOR b on a."DocEntry"  =b."DocEntry"
	inner join WOR1 c on c."DocEntry"  =b."DocEntry" and a."ItemCode" = c."ItemCode"
	where ifnull(c."PlannedQty",0)>0;
	
	
---
	
	update a
	set "ItmsGrpCod"  =  c."ItmsGrpCod",
		"ItmsGrpNam" =   c."ItmsGrpNam",
		"U_Group"  = 	c."U_Group"
	from :tbl a
	left join OITM b on b."ItemCode"  =  a."ItemCode"
	left join OITB c on b."ItmsGrpCod"  = c."ItmsGrpCod";
----
	update a
	set  a."FGCode" = b."ItemCode",
	     a."FGName" = c."ItemName",
	     "U_Group"  = 	d."U_Group"
	
	from :tbl a 
	inner join OWOR b on a."DocEntry" = b."DocEntry"
	inner join OITM c on b."ItemCode" = c."ItemCode"
	left join OITB d on c."ItmsGrpCod" = d."ItmsGrpCod"
	where "LineNum" <> 911;
---and ifnull("FGName",'')  = '';
---
	update a
	set "PlanCompltQty" =  (select sum (b.STDQTY) from :pro11 b where  a."DocNum" = b.WONO 
						and  a."ItemCode" =  b.ITEMCODE ) ,
		"PlanCompltAmount" = (select sum (b.STDAMOUNT) from :pro11 b where  a."DocNum" = b.WONO and  a."ItemCode" =  b.ITEMCODE ) 
	from :tbl a;
	 
--
	update a
	set  "DiffQty" = ifnull("PlanCompltQty",0) - ifnull("IssuedQty",0),
		 "DiffAmount" =  ifnull("PlanCompltAmount",0) - ifnull("Amount",0)
	from :tbl a;
--
	DELETE FROM :tbl where "StartDate" is null;
	update x set "Actual_BOM_Rate1" = 
	(
		-- lấy CmpltQty của dòng đầu tiên Của từng LSX
		ifnull((select max("CmpltQty") from :tbl where "DocNum" = x."DocNum" and  "LineNum" = (select min("LineNum") from :tbl where "DocNum" = x."DocNum")),0)
		-- cộng với tổng CmpltQty của itemgroup 121 (hàng reowork)
		+ ifnull((select sum(a."CmpltQty") from :tbl a left join OITM b on b."ItemCode" = a."FGCode"  where a."DocNum" = x."DocNum" and b."ItmsGrpCod" =121 /*"LineNum" = (select max("LineNum") from :tbl where "DocNum" = x."DocNum"*/ and 1 != (select count(1) from :tbl where "DocNum" = x."DocNum" )),0)
		-- trừ đi tổng IssuedQty của itemgroup 121 (hàng reowork)
		- ifnull((select sum("IssuedQty") from :tbl where "DocNum" = x."DocNum" /*and "ItemName" like 'Rework%'*/ and "ItmsGrpCod" =121),0)
	)
	from :tbl x ;
	
	update x set "Actual_BOM_Rate2" = 
	(
		-- lấy CmpltQty của dòng đầu tiên Của từng LSX
		ifnull((select max("CmpltQty") from :tbl where "DocNum" = x."DocNum" and  "LineNum" = (select min("LineNum") from :tbl where "DocNum" = x."DocNum")),0)
		-- cộng với tổng CmpltQty của itemgroup 121 (hàng reowork)
		+ ifnull((select sum(a."CmpltQty") from :tbl a left join OITM b on b."ItemCode" = a."FGCode"  where a."DocNum" = x."DocNum" and b."ItmsGrpCod" =121 /*"LineNum" = (select max("LineNum") from :tbl where "DocNum" = x."DocNum"*/ and 1 != (select count(1) from :tbl where "DocNum" = x."DocNum" )),0)
	)
	from :tbl x ;
	
--- Actual_BOM_Rate
	update a
	set  "Actual_BOM_Rate" = case 
			when "ItmsGrpCod" = 113 and "Actual_BOM_Rate1" <> 0 then round("IssuedQty" / "Actual_BOM_Rate1",6)
			when "ItmsGrpCod" != 113 and "Actual_BOM_Rate2" <> 0 then round("IssuedQty" / "Actual_BOM_Rate2",6)
			else 0 end
	from :tbl a;
---
	 
	update :tbl   
	set "UnitPrice" =  0
	where "IssuedQty"  =  0 and  ("ItmsGrpCod" in (125,126, 233)  or   ifnull("U_Group",'')  in ('MC_TTMC','LB_TTLB') );
	
	
--- Xử lý chênh lệch so với số liệu hạch toán
	insert into :JE  (Account,AMOUNT,Amount_Debit_Credit)
	 select "Account", sum("Credit"-"Debit") as Amount ,  
	sum("Debit"-"Credit") as Amount_Debit_Credit 
			from "JDT1"
			where ("Account" like '621%' or "Account" like '622%' or "Account" like '627%') 
					and "RefDate" between fromdate and Todate group by "Account";
---======================== LBOR
	select ifnull(sum(Amount_Debit_Credit),0) into total6229   from :JE  where Account like '6229%' ;	
	
			update a
			set a."PlanCompltAmount"  =  ifnull(a."PlanCompltAmount",0) +  ((case when :total6229 <0 then :total6229 * -1 else :total6229 end)  
										- (select  abs(sum(ifnull(b."PlanCompltAmount",0))) from  :tbl b where b."ItmsGrpCod" = 126 or ifnull(b."U_Group",'')  = 'LB_TTLB')  ) 
			from :tbl a
			where  a."DocNum" || a."LineNum"      = (select max(c."DocNum")||max(c."LineNum")  from :tbl c where  c."ItmsGrpCod" = 126 or ifnull(c."U_Group",'')  = 'LB_TTLB')
					and ("ItmsGrpCod" = 126 or ifnull("U_Group",'')  = 'LB_TTLB') ;
	
	
---======================== Machine 
-- :total6279
	  select ifnull(sum(Amount_Debit_Credit),0) into total6279   from :JE  where Account like '6279%' ;
			update a
			set a."PlanCompltAmount"  =  ifnull(a."PlanCompltAmount",0) +  ((case when :total6279 <0 then :total6279 * -1 else :total6279 end)  
										- (select   abs(sum(ifnull(b."PlanCompltAmount",0))) 
										from  :tbl b where b."ItmsGrpCod" in (125,233) or  ifnull(b."U_Group",'')  = 'MC_TTMC') ) 
			from :tbl a
			where  a."DocNum" || a."LineNum"      = (select max(c."DocNum")||max(c."LineNum")  from :tbl c where  c."ItmsGrpCod" in (125,233) or ifnull(c."U_Group",'')  = 'MC_TTMC')
					and ("ItmsGrpCod"  in (125,233) or   ifnull("U_Group",'')  = 'MC_TTMC');
	
---
		insert into :tblEstimateCost (Account, OffsetAcct,CostCenter, Debit, Credit, Amount_Debit_Credit, Thang, Nam )									 
		select "Account", "ContraAct" as "OffsetAcct","ProfitCode", ("Debit") "Debit",  ("Credit") "Credit"
		, ("Debit") - ("Credit"),month( "RefDate"),   year( "RefDate")  
		from JDT1 
		WHERE   substring("Account",0,3) in ('621','622','627','642','641','632','154')
		 and  "RefDate" between fromdate and todate ;
		--("Account" like '621%' or "Account" like '622%' or "Account" like '627%' or  "Account" like '642%' or  "Account" like '641%' or "Account" like '632%' or "Account" like '154%')
---

	insert into :tblSum ("CmpltQty","PlanCompltQty", "PlanCompltAmount","Amount", 
							"IssuedQty","Month","Year",
								"FG_Group","FG_Group_Code","ItemCode_Group","FG_ProType"  )

	select  sum(a."CmpltQty"),sum(a."PlanCompltQty"),sum(a."PlanCompltAmount"),	sum(a."Amount")  "Amount",  
			sum(a."IssuedQty")  "IssuedQty",  	month(a."DocDate") as "Month",  year(a."DocDate")   , 
			d."ItmsGrpNam" as "FG_Group",d."ItmsGrpCod" as "FG_Group_Code",	a."ItmsGrpCod",c."U_ProductType" as "FG_ProType"
		
	from :tbl  a  
	inner join OWOR b on a."DocEntry"  =  b."DocEntry"
	inner join  OITM c  on a."FGCode"  = c."ItemCode"
	inner join OITB d on c."ItmsGrpCod"  =  d."ItmsGrpCod"
	left join "@PRODUCT_TYPE" e on e."Code" = c."U_ProductType"
	group by a."DocDate", d."ItmsGrpNam",d."ItmsGrpCod", a."ItmsGrpCod",c."U_ProductType";
---
	-- test test
	
	--select * from :tblSum ;
	--return ;
	
	---- end
	create local temporary  table #OfftakePrice
	(
			Code nvarchar(50),
			Thang int,
			GiaTri	numeric(21,6),
			Nam int,
			Thang_Nam nvarchar(7)			
	 );

-- Insert du lieu mau vao truoc, khong can biet co du lieu phat sinh trng khoan thoi gian xem hay khong.

-- end
 	 
----03-09-2025: Them cot Code de insert Code cua Items (NoiDung), Cot STT truoc do su dung cho viec sap xep khi hien thi len bao cao.
--  0004 = 0008 + 0005 : Insert tam code = 120 de co so lieu. Sau do se cap nhat theo cong thuc.
	select "Code" into mcode from "@OFFTAKE_GRN" where "Code" = '0004';
	INSERT INTO #OfftakePrice (Code, GiaTri,Thang,Nam) 
	select mcode,sum(ifnull("CmpltQty",0)),"Month","Year"  
	from  :tblSum where "FG_Group_Code"  in ('120') group by  "Month","Year";

--
	select "Code" into mcode from "@OFFTAKE_GRN" where "Code" = '0006';
	INSERT INTO #OfftakePrice (Code, GiaTri,Thang,Nam)  
	select mcode,sum("CmpltQty"),"Month","Year"  from  :tblSum where "FG_ProType" = 'L' group by  "Month","Year";
	--select mcode,5, mname, sum(ifnull(QTY,0)) AS "QTy", MONTH(DOCDATE) AS "Month",year(DOCDATE) as "Year" 
	--from   :pro04  where  "GRPCODE" = 120 and "ITEMCODE"  in ('11000111','11000123') group by GRPCODE,MONTH(DOCDATE),year(DOCDATE) ;
	--select mcode,5, mname,sum("CmpltQty"),"Month","Year"  from  :tblSum where "FG_Group_Code" = '120' group by  "Month","Year";	
--
	select "Code" into mcode from "@OFFTAKE_GRN" where "Code" = '0007';
	INSERT INTO #OfftakePrice (Code, GiaTri,Thang,Nam) 
	select mcode,sum(ifnull("IssuedQty",0)),"Month","Year"  
	from  :tblSum 
	where "ItemCode_Group" = '120' group by  "Month","Year";
	
--	Tinh lai so 5 sau khi them dong Liquid.Code: 0006
	select "Code" into mcode from "@OFFTAKE_GRN" where "Code" = '0005';
	INSERT INTO #OfftakePrice (Code,GiaTri,Thang,Nam) 
	select mcode,sum(ifnull("CmpltQty",0)),"Month","Year"  from  :tblSum where "FG_ProType" = 'F' group by  "Month","Year";
--- ifnull((select sum(b.GiaTri) from  #OfftakePrice b where  a.thang = b.thang and a.Nam  = b.Nam and b.Code = '0006' ),0)	
	update   a
	set  a.GiaTri = a.GiaTri 
							- ifnull((select sum(c.GiaTri) from  #OfftakePrice c where  a.thang = c.thang 
								and a.Nam  = c.Nam and c.Code = '0007' ),0)	
							
	from  #OfftakePrice a
	where  Code  = mcode;

--
	select "Code" into mcode from "@OFFTAKE_GRN" where "Code" = '0008';
	INSERT INTO #OfftakePrice (Code, GiaTri,Thang,Nam) 
	select mcode,sum(ifnull("CmpltQty",0)),"Month","Year"  from  :tblSum where "FG_Group_Code" = '121' group by  "Month","Year";
--  0004 =0008 + 0005
	update   a
	set  a.GiaTri = ifnull((select sum(b.GiaTri) from  #OfftakePrice b where  a.thang = b.thang 
									and a.Nam  = b.Nam and b.Code = '0005'),0)
					+ ifnull((select sum(b.GiaTri) from  #OfftakePrice b where  a.thang = b.thang 
									and a.Nam  = b.Nam and b.Code = '0008'),0)					
	from  #OfftakePrice a
	where  Code  = '0004';
--
	select "Code" into mcode from "@OFFTAKE_GRN" where "Code" = '0009';
	INSERT INTO #OfftakePrice (Code, GiaTri,Thang,Nam) 
	select mcode,sum(ifnull("IssuedQty",0)),"Month","Year"  from  :tblSum where "ItemCode_Group" = '121' group by  "Month","Year";

-------------------- Tinh cho Code = '0011'	
	select "Code" into mcode from "@OFFTAKE_GRN" where "Code" = '0011';
	INSERT INTO #OfftakePrice (Code, GiaTri,Thang,Nam) 
	select mcode,sum(ifnull("IssuedQty",0)),"Month","Year" 
	from  :tblSum where "ItemCode_Group" in ('113','253') group by  "Month","Year";
	
	update a
	set GiaTri = GiaTri - ifnull(b."CmpltQty",0)
	from #OfftakePrice a
	inner join (select  sum(a."CmpltQty") as "CmpltQty",mcode as "Code", month(a."DocDate") as "Thang",  year(a."DocDate") as "Nam"		
		from :tbl  a  
		inner join  OITM c  on a."FGCode"  = c."ItemCode"
		left join "@PRODUCT_TYPE" e on e."Code" = c."U_ProductType"
		where substring(a."DocNum",0,1) = '9' -- and c."U_ProductType" = 'OTH'
		group by month(a."DocDate"),year(a."DocDate") 
		) b on a.Code = b."Code" 
	where a.Thang = b."Thang" and a.Nam = b."Nam" ;
	
--   12
	select "Code" into mcode from "@OFFTAKE_GRN" where "Code" = '0012';
	INSERT INTO #OfftakePrice (Code, GiaTri,Thang,Nam) 
	select mcode,sum(ifnull("Amount",0)),"Month","Year"  
	from  :tblSum where "ItemCode_Group" in ('113','253') group by  "Month","Year";

	update a
	set GiaTri = ifnull(GiaTri,0) - ifnull(b."AmountFG",0)
	from #OfftakePrice a
	inner join (select  sum(ifnull(a."AmountFG",0)) as "AmountFG",mcode as "Code", month(a."DocDate") as "Thang",  year(a."DocDate") as "Nam"		
		from :tbl  a  
		inner join  OITM c  on a."FGCode"  = c."ItemCode"
		left join "@PRODUCT_TYPE" e on e."Code" = c."U_ProductType"
		where substring(a."DocNum",0,1) = '9' -- and c."U_ProductType" = 'OTH'
		group by month(a."DocDate"),year(a."DocDate") 
		) b on a.Code = b."Code" 
	where a.Thang = b."Thang" and a.Nam = b."Nam" ;
	
--
	select "Code" into mcode from "@OFFTAKE_GRN" where "Code" = '0013';
	INSERT INTO #OfftakePrice (Code, GiaTri,Thang,Nam)
	SELECT mcode,t1.GiaTri / NULLIF(t2.GiaTri, 0) as GiaTri, t1.Thang,t1.Nam
	from #OfftakePrice t1
	inner join #OfftakePrice t2 ON t1.Thang = t2.Thang  AND t1.Nam = t2.Nam AND t1.Code = '0012'  AND t2.Code = '0011';
--
	select "Code" into mcode from "@OFFTAKE_GRN" where "Code" = '0010';
	INSERT INTO #OfftakePrice (Code, GiaTri,Thang,Nam)
	SELECT mcode,round(t1.GiaTri / (NULLIF(t2.GiaTri, 0) - NULLIF(t3.GiaTri, 0)),nround)as GiaTri, t1.Thang,t1.Nam
	from #OfftakePrice t1
	inner join #OfftakePrice t2 ON t1.Thang = t2.Thang  AND t1.Nam = t2.Nam AND t1.Code = '0011'  AND t2.Code = '0004'   
	inner join  #OfftakePrice t3 ON t1.Thang = t3.Thang  AND t1.Nam = t3.Nam AND t1.Code = '0011'  AND t3.Code = '0009';  
--
	select "Code" into mcode from "@OFFTAKE_GRN" where "Code" = '0014';
	INSERT INTO #OfftakePrice (Code, GiaTri,Thang,Nam) 
	select mcode,sum(ifnull("Amount",0)),"Month","Year"  from  :tblSum where "ItemCode_Group" = '127' group by  "Month","Year";
--
	select "Code" into mcode from "@OFFTAKE_GRN" where "Code" = '0015';
	INSERT INTO #OfftakePrice (Code,GiaTri,Thang,Nam) 
	select mcode,sum(ifnull("Amount",0)),"Month","Year"  from  :tblSum where "ItemCode_Group" = '114' group by  "Month","Year";

-- 
	select "Code" into mcode from "@OFFTAKE_GRN" where "Code" = '0048';
	INSERT INTO #OfftakePrice (Code,GiaTri,Thang,Nam) 
	select mcode,sum(ifnull("CmpltQty",0)) as "CmpltQty","Month","Year"
	from :tblSum where "FG_ProType" = 'OF' group by  "Month","Year" ;
--
	select "Code" into mcode from "@OFFTAKE_GRN" where "Code" = '0016';
	INSERT INTO #OfftakePrice (Code, GiaTri,Thang,Nam) 
	select mcode,sum(ifnull("Amount",0)),"Month","Year"  from  :tblSum where "ItemCode_Group" = '256' group by  "Month","Year";
--
	select "Code" into mcode from "@OFFTAKE_GRN" where "Code" = '0017';
	INSERT INTO #OfftakePrice (Code, GiaTri,Thang,Nam) 
	select mcode,sum(ifnull("Amount",0)),"Month","Year"  from  :tblSum where "ItemCode_Group" = '121' group by  "Month","Year";
--
	select "Code" into mcode from "@OFFTAKE_GRN" where "Code" = '0018';
	INSERT INTO #OfftakePrice (Code, GiaTri,Thang,Nam) 
	select mcode,sum(ifnull("Amount",0)),"Month","Year"  from  :tblSum where "ItemCode_Group" = '120' group by  "Month","Year";
	
	
----------------------------- xu ly du lieu tu JE (JDT1). HieuChinh-03-09-2025.
		/*
		insert into  :tblOffTake_Price_List (Code, STT,GroupName)
		select 
			distinct PR."U_GroupName" "Code",  GRN."U_STT", GRN."Name"   
		from "@OFFTAKE_PRICE_L" PR 
		LEFT JOIN "@OFFTAKE_GRN" GRN ON GRN."Code" = PR."U_GroupName";
		*/
--

		insert into  :tblOffTake_Price_Setting (ID,STT,Code, Name,GroupName,Account,Exclude_Account,CostCenter,Exclude_OffsetAcct)
		select   ROW_NUMBER() OVER (),GRN."U_STT", GRN."Code",  GRN."Name", GRN."Name", PR."U_Account",PR."U_Exclude_Account", PR."U_Department",  PR."U_Exclude_OffsetAcct"
		from "@OFFTAKE_PRICE_L" PR
		INNER JOIN "@OFFTAKE_GRN" GRN ON GRN."Code" = PR."U_GroupName";
		--
		insert into :tblBalance_Account_Dpt (Balance,Account,GroupName,Thang,Nam,CostCenter,Offtake_CostCenter)
		SELECT case when (A.AMOUNT_DEBIT_CREDIT)  = 0 
			then (DEBIT) else   (A.AMOUNT_DEBIT_CREDIT)  end as "Total_Balance"
	 		,A.ACCOUNT,B.Code, a.thang, a.nam,a.CostCenter,b.CostCenter
		FROM :tblEstimateCost A
		JOIN :tblOffTake_Price_Setting B
		    ON A.ACCOUNT = B.ACCOUNT and  A.CostCenter  = B.CostCenter
		WHERE 
		NOT (   ',' || B.EXCLUDE_OFFSETACCT || ',' LIKE '%,' || A.OFFSETACCT || ',%' )
		
		 order by B.Code,a.thang, a.nam	 , a.CostCenter ;
	 	------------------ SUM 
	-- Thay doi :tblOffTake_Price_List = "@OFFTAKE_GRN" : 12/03/2026
		insert into :tblBalance_Account_Dpt_Sum (Code, STT,Balance,GroupName,Thang,Nam)
		select b."Code",max(b."U_STT"),sum(Balance),a.GroupName,a.Thang,a.Nam
		from :tblBalance_Account_Dpt a
		inner join "@OFFTAKE_GRN" b on a.GroupName = b."Code"
		group by a.GroupName,a.Thang,a.Nam, b."Code"
		order by  b."Code";
		---- Insert them du lieu tblBalance_Account_Dpt_Sum --> Bang Chinh: OfftakePrice
	
		insert into  #OfftakePrice (Code, GiaTri,Thang,Nam)
		select Code,Balance,Thang,Nam
		from  :tblBalance_Account_Dpt_Sum;
		
		---
		 --'Total variable cost $/MT Instant Coffee'
		select "U_TotalFormular", "Code" into mtotalf,mcode from "@OFFTAKE_GRN" where "Code" = '0028';
		insert into  #OfftakePrice (Code, GiaTri,Thang,Nam)
		select mcode,round(ifnull(t2.GiaTri,0),nround),t1.Thang,t1.Nam
		from #OfftakePrice t1
		inner join  (select sum(t22.GiaTri)  GiaTri, t22.Thang,t22.Nam, t22.Code from  #OfftakePrice t22 
						where t22.Code  in (select "part" from SplitString(mtotalf,',') ) group by Thang,Nam, Code) t2 on  t1.Thang = t2.Thang  
						AND t1.Nam = t2.Nam and t1.Code = t2.Code;
		
		--
		select "U_TotalFormular", "Code" into mtotalf,mcode from "@OFFTAKE_GRN" where "Code" = '0029';
		insert into  #OfftakePrice (Code,GiaTri,Thang,Nam)
		select mcode,ifnull(t2.GiaTri,0),t1.Thang,t1.Nam
		from #OfftakePrice t1
		inner join  (select sum(t22.GiaTri)  GiaTri, t22.Thang,t22.Nam, t22.Code from  #OfftakePrice t22 
						where Code in (select "part" from SplitString(mtotalf,',') ) group by Thang,Nam, Code) t2 on  t1.Thang = t2.Thang
						  AND t1.Nam = t2.Nam and t1.Code = t2.Code;
		
		
		--
		select "U_TotalFormular", "Code" into mtotalf,mcode from "@OFFTAKE_GRN" where "Code" = '0047';
		insert into  #OfftakePrice (Code,GiaTri,Thang,Nam)
		select mcode,ifnull(t2.GiaTri,0),t1.Thang,t1.Nam
		from #OfftakePrice t1
		inner join  (select sum(t22.GiaTri)  GiaTri, t22.Thang,t22.Nam, t22.Code from  #OfftakePrice t22 
						where Code in (select "part" from SplitString(mtotalf,',') ) group by Thang,Nam, Code) t2 on  t1.Thang = t2.Thang
						  AND t1.Nam = t2.Nam and t1.Code = t2.Code;
		
		--============================Cost per KG
		select "Code" into mcode from "@OFFTAKE_GRN" where "Code" = '0003';
		insert into  #OfftakePrice (Code, GiaTri,Thang,Nam)
		select mcode,sum(ifnull(t1.giatri,0)),t1.Thang,t1.Nam
		from  #OfftakePrice t1
		where t1.Code  = '0028'
		group by t1.Thang,t1.Nam;
		
		update   a
		set  a.GiaTri = a.GiaTri + (select ifnull(sum(b.GiaTri),0) from  #OfftakePrice b where  a.thang = b.thang 
										and a.Nam  = b.Nam and b.Code = '0047' )
		from  #OfftakePrice a
		where  Code  = mcode;
		
		
		update a
		set a.GiaTri = a.GiaTri/ (select ifnull(sum(b.GiaTri),0) from  #OfftakePrice b where  a.thang = b.thang 
										and a.Nam  = b.Nam and b.Code = '0004') 
		from  #OfftakePrice a
		where  Code  = mcode;
		--============================ Mark up - 20% net
		select "Code" into mcode  from "@OFFTAKE_GRN" where "Code" = '0002';
		insert into  #OfftakePrice (Code, GiaTri,Thang,Nam)
		select mcode,sum(ifnull(t1.giatri,0)),t1.Thang,t1.Nam
		from  #OfftakePrice t1
		where t1.Code  = '0028'
		group by t1.Thang,t1.Nam;
		--
		update   a
		set  a.GiaTri = a.GiaTri + (select ifnull(sum(b.GiaTri),0) from  #OfftakePrice b where  a.thang = b.thang 
										and a.Nam  = b.Nam and b.Code = '0047' )
		from  #OfftakePrice a
		where  Code  = mcode;
		
		--
		update   a
		set  a.GiaTri = a.GiaTri -(select ifnull(sum(b.GiaTri),0) from  #OfftakePrice b where  a.thang = b.thang 
										and a.Nam  = b.Nam and b.Code in ( '0043','0044') )
		from  #OfftakePrice a
		where  Code  = mcode;
		
		--
		update   a
		set  a.GiaTri = a.GiaTri  * 1.2 
		from  #OfftakePrice a
		where  Code  = mcode;
		--
		
		update   a
		set  a.GiaTri = a.GiaTri + (select ifnull(sum(b.GiaTri),0) from  #OfftakePrice b where  a.thang = b.thang 
										and a.Nam  = b.Nam and b.Code  in ( '0043','0044') )
		from  #OfftakePrice a
		where  Code  = mcode;		
		--
		update   a
		set  a.GiaTri = round( a.GiaTri  / NULLIF((select sum(b.GiaTri) from  #OfftakePrice b where  a.thang = b.thang 
										and a.Nam  = b.Nam and b.Code = '0004' ), NULL),nround)
		from  #OfftakePrice a
		where  Code  = mcode;
				
		--Offtake price (USD/KG)
		select "Code" into mcode from "@OFFTAKE_GRN" where "Code" = '0001';
		INSERT INTO #OfftakePrice (Code, GiaTri,Thang,Nam)
		SELECT mcode,
				round(ifnull(t1.GiaTri / NULLIF(t2.GiaTri, 0),0)  * ifnull(t3.GiaTri, 0) + ifnull(t4.GiaTri, 0),nround) as GiaTri,
					 t1.Thang,t1.Nam
		from #OfftakePrice t1
		inner join #OfftakePrice t2 ON t1.Thang = t2.Thang  AND t1.Nam = t2.Nam AND t1.Code = '0012'  AND t2.Code = '0011'
		inner join  #OfftakePrice t3 ON t1.Thang = t3.Thang  AND t1.Nam = t3.Nam AND t1.Code = '0012'  AND t3.Code = '0010' 
		inner join  #OfftakePrice t4 ON t1.Thang = t4.Thang  AND t1.Nam = t4.Nam AND t1.Code = '0012'  AND t4.Code = '0002';  
		 
	 
	 
		
---- PIVOT
	
	CREATE LOCAL TEMPORARY TABLE #TempColumns AS 
	        (SELECT ROW_NUMBER() OVER (ORDER BY Nam, Thang) AS ID, Thang, Nam 
	         FROM (SELECT DISTINCT Thang, Nam FROM #OfftakePrice ORDER BY Nam, Thang) AS SubQuery);
	    
	    SELECT COUNT(*) INTO v_total FROM #TempColumns;
	    
	    -- Số lượng cột trung bình có thể tạo được
	    SELECT (v_total - 3) INTO v_avg_count from dummy;
	    
	    v_sql = 'CREATE LOCAL TEMPORARY TABLE #tblPivot (SourceCode NVARCHAR(10),SourceName NVARCHAR(200),STT int,Items NVARCHAR(255), ';
	    
	    WHILE v_index <= v_total DO
	        SELECT INITCAP(TO_VARCHAR(TO_DATE(TO_VARCHAR(Nam)||'-'|| TO_VARCHAR(Thang)), 'MON yyyy')) as vtn
	        , Nam INTO v_thangnam, v_nam FROM #TempColumns WHERE ID = v_index;
	        v_sql = v_sql || '"' || v_thangnam  || '" NUMERIC(21,6), ';
	        v_index = v_index + 1;
	    END WHILE;
	    
	    -- Thêm cột trung bình động
	    v_index = 1;
	    WHILE v_index <= v_avg_count DO
	        SELECT INITCAP(TO_VARCHAR(TO_DATE(TO_VARCHAR(Nam)||'-'|| TO_VARCHAR(Thang)), 'MON yyyy')) as vtn
	        , Nam INTO v_thangnam, v_nam 
	        FROM #TempColumns WHERE ID = v_index;
	        SELECT INITCAP(TO_VARCHAR(TO_DATE(TO_VARCHAR(Nam)||'-'|| TO_VARCHAR(Thang)), 'MON yyyy')) as vtn
	        , Nam INTO v_tothangnam, v_to_nam FROM #TempColumns WHERE ID = v_index + 3;
	        v_sql = v_sql || '"Avg.Act_' || v_index || '_from_' || v_thangnam || '_to_' || v_tothangnam || '" NUMERIC(21,6), ';
	        v_index = v_index + 1;
	    END WHILE;
	    
	    v_sql = LEFT(v_sql, LENGTH(v_sql) - 2) || ')';
	    EXECUTE IMMEDIATE v_sql;
	    
	    v_index = 0;
	    v_sql = 'INSERT INTO #tblPivot (SourceCode';
	    
	    WHILE v_index < v_total DO
	        SELECT INITCAP(TO_VARCHAR(TO_DATE(TO_VARCHAR(Nam)||'-'|| TO_VARCHAR(Thang)), 'MON yyyy')) as vtn
	        , Nam INTO v_thangnam, v_nam FROM #TempColumns WHERE ID = v_index + 1;
	        v_sql = v_sql || ', "' || v_thangnam || '"';
	        v_index = v_index + 1;
	    END WHILE;
	    
	    v_index = 1;
	    WHILE v_index <= v_avg_count DO
	        SELECT INITCAP(TO_VARCHAR(TO_DATE(TO_VARCHAR(Nam)||'-'|| TO_VARCHAR(Thang)), 'MON yyyy')) as vtn
	        , Nam INTO v_thangnam, v_nam FROM #TempColumns WHERE ID = v_index;
	        SELECT INITCAP(TO_VARCHAR(TO_DATE(TO_VARCHAR(Nam)||'-'|| TO_VARCHAR(Thang)), 'MON yyyy')) as vtn
	        , Nam INTO v_tothangnam, v_to_nam FROM #TempColumns WHERE ID = v_index + 3;
	        v_sql = v_sql || ', "Avg.Act_' || v_index || '_from_' || v_thangnam || '_to_' || v_tothangnam || '"';
	        v_index = v_index + 1;
	    END WHILE;
	    
	    v_sql = v_sql || ') SELECT Code ';
	    
	    v_index = 0;
	    WHILE v_index < v_total DO
	        SELECT Thang, Nam, INITCAP(TO_VARCHAR(TO_DATE(TO_VARCHAR(Nam)||'-'|| TO_VARCHAR(Thang)), 'MON yyyy')) as vtn
	         INTO v_thang, v_nam, v_thangnam FROM #TempColumns WHERE ID = v_index + 1;
	        v_sql = v_sql || ', ROUND(SUM(CASE WHEN Thang = ' || v_thang || ' AND Nam = ' || v_nam || ' THEN ifnull(GiaTri,0) ELSE 0 END),'|| nround ||') AS "' || v_thangnam || '"';
	        v_index = v_index + 1;
	    END WHILE;
	    
	    v_index = 1;
			   
	    WHILE v_index <= v_avg_count DO
	        SELECT Thang, Nam, INITCAP(TO_VARCHAR(TO_DATE(TO_VARCHAR(Nam)||'-'|| TO_VARCHAR(Thang)), 'MON yyyy'))  as vtn
	         INTO v_thang, v_nam, v_thangnam FROM #TempColumns WHERE ID = v_index;
	        SELECT Thang, Nam
	        ,INITCAP(TO_VARCHAR(TO_DATE(TO_VARCHAR(Nam)||'-'|| TO_VARCHAR(Thang)), 'MON yyyy'))  as vtn
	         INTO v_to_thang, v_to_nam, v_tothangnam FROM #TempColumns WHERE ID = v_index + 3;
	         
	        v_sql = v_sql || ', ROUND((SUM(CASE WHEN (Thang IN (' || v_thang || ', ' || (CASE WHEN v_thang+1 > 12 THEN 1 ELSE v_thang+1 END) || ', ' || (CASE WHEN v_thang+2 > 12 THEN case when v_thang = 12 then 	2 else 1 end ELSE v_thang+2 END) || ', ' || v_to_thang || ') 
	        AND Nam IN (' || v_nam || ', ' || (CASE WHEN v_thang+1 > 12 THEN v_nam+1 ELSE v_nam END) || ', ' || (CASE WHEN v_thang+2 > 12 THEN v_nam+1 ELSE v_nam END) || ', ' || (CASE WHEN v_to_thang > 12 THEN v_to_nam+1 ELSE v_to_nam END) || ')) 
	        THEN GiaTri ELSE 0 END) / 4),'|| nround ||') AS "Avg.Act_' || v_index || '_from_' || v_thangnam || '_to_' || v_tothangnam || '"';
	        v_index = v_index + 1;
	    END WHILE;
	    v_sql = v_sql || ' FROM #OfftakePrice GROUP BY Code ';
	    
	    
	    
	    EXECUTE IMMEDIATE v_sql;	    
	    
		execute immediate 'update a set a.SourceName = d."SourceName", a.STT = d."STT", a.items = d."Items"
		from #tblPivot a
		left join (select b."Code" as "SourceCode",max(c."U_SourceName") as "SourceName", max(b."U_STT") as "STT",max(b."Name") as "Items"
				 from "@OFFTAKE_GRN" b
				left join "@OFFTAKE_PRICE_H" c on c."DocEntry" = b."U_CodeSource" GROUP BY b."Code"
				) d on d."SourceCode" = a.SourceCode' ;
				
---====Cap nhat du lieu cot AVG.tinh cho cac cot trung binh theo cong thuc rieng cua cac chi tieu.
		v_index = 1;
	    v_sql = '';
	    CREATE LOCAL TEMPORARY TABLE #sum04 (giatri numeric(21,6) );
	    
	    WHILE v_index <= v_avg_count DO
	        SELECT Thang, Nam, INITCAP(TO_VARCHAR(TO_DATE(TO_VARCHAR(Nam)||'-'|| TO_VARCHAR(Thang)), 'MON yyyy'))  as vtn
	         INTO v_thang, v_nam, v_thangnam FROM #TempColumns WHERE ID = v_index;
	        SELECT Thang, Nam
	        ,INITCAP(TO_VARCHAR(TO_DATE(TO_VARCHAR(Nam)||'-'|| TO_VARCHAR(Thang)), 'MON yyyy'))  as vtn
	         INTO v_to_thang, v_to_nam, v_tothangnam FROM #TempColumns WHERE ID = v_index + 3;
	       
	       v_sql = ' 	        
	       	insert into #sum04 (giatri) 
	        select sum(GiaTri) as GiaTri
			from #OfftakePrice 
			where Code = ''0004'' and'  || ' Thang IN (' || v_thang || ', ' || (CASE WHEN v_thang+1 > 12 THEN 1 ELSE v_thang+1 END) || ', ' || (CASE WHEN v_thang+2 > 12 THEN case when v_thang = 12 then 2 else 1 end ELSE v_thang+2 END) || ', ' || v_to_thang || ') 
	        AND Nam IN (' || v_nam || ', ' || (CASE WHEN v_thang+1 > 12 THEN v_nam+1 ELSE v_nam END)|| ', ' || (CASE WHEN v_thang+2 > 12 THEN v_nam+1 ELSE v_nam END)|| ', ' || (CASE WHEN v_to_thang > 12 THEN v_to_nam+1 ELSE v_to_nam END) || ');'  ;
       
       		execute immediate v_sql;
       		
			v_sql = '
			update a
			set ' || '"Avg.Act_' || v_index || '_from_' || v_thangnam || '_to_' || v_tothangnam || '"' || ' = round((ifnull(b.giatri,0)/ifnull(c.giatri,1))*1000,'|| nround ||')
			from #tblPivot a
			left join (
				select Code, ifnull(sum(GiaTri),0) as GiaTri
				from #OfftakePrice 
				where '  || ' Thang IN (' || v_thang || ', ' || (CASE WHEN v_thang+1 > 12 THEN 1 ELSE v_thang+1 END) || ', ' || (CASE WHEN v_thang+2 > 12 THEN case when v_thang = 12 then 	2 else 1 end ELSE v_thang+2 END) || ', ' || v_to_thang || ') 
	        	AND Nam IN (' || v_nam || ', ' || (CASE WHEN v_thang+1 > 12 THEN v_nam+1 ELSE v_nam END)|| ', ' || (CASE WHEN v_thang+2 > 12 THEN v_nam+1 ELSE v_nam END)|| ', ' || (CASE WHEN v_to_thang > 12 THEN v_to_nam+1 ELSE v_to_nam END) || ')' ||' group by Code
				) b on b.Code = a.SourceCode
			left join #sum04 c on 1 = 1
			where 
				a.SourceCode in (select "Code" from "@OFFTAKE_GRN" 
				where ("U_TotalFormular" is null or length("U_TotalFormular") = 0)
				and "U_CodeSource" in (3,4) or "Code" in(''0014'',''0015'',''0016'',''0017'',''0018'')) ;';
			
			execute immediate v_sql;
			
			-- tinh lai tong cho cac dong co code = 0028,0029, 0047
			select "U_TotalFormular", "Code" into mtotalf,mcode from "@OFFTAKE_GRN" where "Code" = '0028';
			
			v_sql =  'update a set a.' || '"Avg.Act_' || v_index || '_from_' || v_thangnam || '_to_' || v_tothangnam || '" = round(ifnull(b.giatri,0),'|| nround ||')' 
			|| ' from #tblPivot a left join (select ''' || mcode || ''' as SourceCode,sum(' || '"Avg.Act_' || v_index || '_from_' || v_thangnam || '_to_' || v_tothangnam || '"' ||') as giatri from #tblPivot where SourceCode in (select "part" from SplitString(''' || mtotalf || ''','','')) ) b on b.SourceCode = a.SourceCode where a.SourceCode = ''' || mcode || ''' ;'
			;
			execute immediate v_sql;
			
			select "U_TotalFormular", "Code" into mtotalf,mcode from "@OFFTAKE_GRN" where "Code" = '0029';
			
			v_sql = 'update a set a.' || '"Avg.Act_' || v_index || '_from_' || v_thangnam || '_to_' || v_tothangnam || '" = round(ifnull(b.giatri,0),'|| nround ||')' 
			|| ' from #tblPivot a left join (select ''' || mcode || ''' as SourceCode,sum(' || '"Avg.Act_' || v_index || '_from_' || v_thangnam || '_to_' || v_tothangnam || '"' ||') as giatri from #tblPivot where SourceCode in (select "part" from SplitString(''' || mtotalf || ''','','')) ) b on b.SourceCode = a.SourceCode where a.SourceCode = ''' || mcode || ''' ;'
			;
			execute immediate v_sql;
			
			select "U_TotalFormular", "Code" into mtotalf,mcode from "@OFFTAKE_GRN" where "Code" = '0047';
			
			v_sql =  'update a set a.' || '"Avg.Act_' || v_index || '_from_' || v_thangnam || '_to_' || v_tothangnam || '" = round(ifnull(b.giatri,0),'|| nround ||')' 
			|| ' from #tblPivot a left join (select ''' || mcode || ''' as SourceCode,sum(' || '"Avg.Act_' || v_index || '_from_' || v_thangnam || '_to_' || v_tothangnam || '"' ||') as giatri from #tblPivot where SourceCode in (select "part" from SplitString(''' || mtotalf || ''','','')) ) b on b.SourceCode = a.SourceCode where a.SourceCode = ''' || mcode || ''' ;'
			;
			execute immediate v_sql;
-- tinh lai avg cho cac dong co code = 0002,0003, 0001
			mcode = '0003';
			v_sql = 'update a set a.' || '"Avg.Act_' || v_index || '_from_' || v_thangnam || '_to_' || v_tothangnam || '" = round(ifnull(b.giatri,0)/1000,'|| nround ||')' 
			|| ' from #tblPivot a 
			left join (select ''' || mcode || ''' as SourceCode,sum(' || '"Avg.Act_' || v_index || '_from_' || v_thangnam || '_to_' || v_tothangnam || '"' ||') as giatri 
				from #tblPivot where SourceCode in (''0028'',''0047'') ) b on b.SourceCode = a.SourceCode 
			where a.SourceCode = ''' || mcode || ''' ;'
			;
			execute immediate v_sql;
			
			mcode = '0002';
			v_sql = 'update a set a.' || '"Avg.Act_' || v_index || '_from_' || v_thangnam || '_to_' || v_tothangnam || '" = round(((b.giatri-ifnull(c.giatri,0))*1.2 + ifnull(c.giatri,0))/1000,'|| nround ||')' 
			|| ' from #tblPivot a 
			left join (select ''' || mcode || ''' as SourceCode,sum(' || '"Avg.Act_' || v_index || '_from_' || v_thangnam || '_to_' || v_tothangnam || '"' ||') as giatri 
				from #tblPivot where SourceCode in (''0028'',''0047'') ) b on b.SourceCode = a.SourceCode 
			left join (select ''' || mcode || ''' as SourceCode,sum(' || '"Avg.Act_' || v_index || '_from_' || v_thangnam || '_to_' || v_tothangnam || '"' ||') as giatri 
				from #tblPivot where SourceCode in (''0043'',''0044'') ) c on c.SourceCode = a.SourceCode 
			where a.SourceCode = ''' || mcode || ''';'
			;
			execute immediate v_sql;			
			mcode = '0001';
			v_sql = 'update a set a.' || '"Avg.Act_' || v_index || '_from_' || v_thangnam || '_to_' || v_tothangnam || '" = round(b.giatri/ifnull(c.giatri,1)*d.giatri+e.giatri,'|| nround ||')' 
			|| ' from #tblPivot a 
			left join (select ''' || mcode || ''' as SourceCode,(' || '"Avg.Act_' || v_index || '_from_' || v_thangnam || '_to_' || v_tothangnam || '"' ||') as giatri 
				from #tblPivot where SourceCode in (''0012'') ) b on b.SourceCode = a.SourceCode 
			left join (select ''' || mcode || ''' as SourceCode,(' || '"Avg.Act_' || v_index || '_from_' || v_thangnam || '_to_' || v_tothangnam || '"' ||') as giatri 
				from #tblPivot where SourceCode in (''0011'') ) c on c.SourceCode = a.SourceCode 
			left join (select ''' || mcode || ''' as SourceCode,(' || '"Avg.Act_' || v_index || '_from_' || v_thangnam || '_to_' || v_tothangnam || '"' ||') as giatri 
				from #tblPivot where SourceCode in (''0010'') ) d on d.SourceCode = a.SourceCode 
			left join (select ''' || mcode || ''' as SourceCode,(' || '"Avg.Act_' || v_index || '_from_' || v_thangnam || '_to_' || v_tothangnam || '"' ||') as giatri 
				from #tblPivot where SourceCode in (''0002'') ) e on d.SourceCode = a.SourceCode 
			where a.SourceCode = ''' || mcode || ''';'
			;
			execute immediate v_sql;
			
			delete from #sum04;
			v_sql = '';
			v_index = v_index + 1;
			
	    END WHILE;
	   
	    v_sql = '';
--- Lam tron 2 so le cho cac cot phat sinh theo tung thang va avg.
		nround = 2; 
		v_index = 1 ;  
	    WHILE v_index <= v_total DO
	        SELECT INITCAP(TO_VARCHAR(TO_DATE(TO_VARCHAR(Nam)||'-'|| TO_VARCHAR(Thang)), 'MON yyyy')) as vtn
	        , Nam INTO v_thangnam, v_nam FROM #TempColumns WHERE ID = v_index;
	        v_sql = 'update #tblPivot set "' || v_thangnam  || '" = round("'|| v_thangnam ||'", '|| nround ||') ;';
	        execute immediate v_sql;
	        v_index = v_index + 1;
	    END WHILE;
		
		v_index = 1 ;
		v_sql = '';
		WHILE v_index <= v_avg_count DO
			SELECT Thang, Nam, INITCAP(TO_VARCHAR(TO_DATE(TO_VARCHAR(Nam)||'-'|| TO_VARCHAR(Thang)), 'MON yyyy'))  as vtn
	         INTO v_thang, v_nam, v_thangnam FROM #TempColumns WHERE ID = v_index;
	        SELECT Thang, Nam
	        ,INITCAP(TO_VARCHAR(TO_DATE(TO_VARCHAR(Nam)||'-'|| TO_VARCHAR(Thang)), 'MON yyyy'))  as vtn
	         INTO v_to_thang, v_to_nam, v_tothangnam FROM #TempColumns WHERE ID = v_index + 3;
	         
			v_sql = 'update #tblPivot set ' || '"Avg.Act_' || v_index || '_from_' || v_thangnam || '_to_' || v_tothangnam || '" = round('||'"Avg.Act_' || v_index || '_from_' || v_thangnam || '_to_' || v_tothangnam || '",'|| nround ||');' ;
			
			execute immediate v_sql;
	        v_index = v_index + 1;
		END WHILE;	
			 
-- insert luon nhung dong khong co phat sinh
		execute immediate 'insert into #tblPivot(SourceCode,SourceName,STT,Items)
		select d."SourceCode",d."SourceName", d."STT",d."Items" 
		from (select b."Code" as "SourceCode",max(c."U_SourceName") as "SourceName", max(b."U_STT") as "STT",max(b."Name") as "Items"
				 from "@OFFTAKE_GRN" b
				left join "@OFFTAKE_PRICE_H" c on c."DocEntry" = b."U_CodeSource" GROUP BY b."Code"
				) d				
		left join #tblPivot a on d."SourceCode" = a.SourceCode
		where a.SourceCode is null';
		
		execute immediate 'SELECT * FROM  #tblPivot order by STT'; 
------------------------
	
-- drop table
	drop table #OfftakePrice;
	exec 'drop table #tblPivot';
	drop table #TempColumns;
	exec 'drop table #sum04';
End
