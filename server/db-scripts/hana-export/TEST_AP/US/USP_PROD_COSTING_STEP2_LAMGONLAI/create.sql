CREATE procedure USP_PROD_COSTING_STEP2_LAMGONLAI(
	in period 	nvarchar(20), 
	in PrdLine 	nvarchar(1000),
	in branch 	int
	
)
as
begin
		declare tblITEM table
		(
			"ItemCode" nvarchar(50),
			"ItemName" nvarchar(150),
			"ItmsGrpCod" int
		);
		--
		declare ItemTPham table
		(
			ItemCode nvarchar(50)
		);
		--
		declare tblEstimateCost table
		(
			Ref3Line		nvarchar(20),
			Account 		nvarchar(20),
			TransType		nvarchar(20),
			Debit			numeric(28,10),
			Credit			numeric(28,10),
			Amount			numeric(28,10),
			Amount_Debit_Credit	numeric(28,10)
			
		);
		declare pro04 table
		(
			AMOUNT				numeric(28,10),
			QTY					numeric(28,10),
			MATERIALS			numeric(28,10),
			MACHINE				numeric(28,10),
			LABOUR				numeric(28,10),
			ItemCode			nvarchar(50),
			WOID				nvarchar(20),
			WONo				nvarchar(20),
			OUTSOURCING_TYPE	nvarchar(20)
		);
 		--
		declare pro09 table
		(
			"CmpltQty"		numeric(28,10),
			"AmountFG"		numeric(28,10),
			"Amount"		numeric(28,10),
			"SdtAmount"		numeric(28,10),
			"IssuedQty"		numeric(28,10),
			"FGCode"		nvarchar(50),
			"ItmsGrpCod"	nvarchar(10),
			"ItemCode"		nvarchar(50),
			"DocNum"		nvarchar(20)
		
		);
		--
		declare pro05 table
		(
			ItemCode nvarchar(50),
			Qty	numeric(28,10),
			FATHER1_Date date
		);
		declare Material_Amount_Temp table
		(
			ItemCode varchar(100),-- Thanh pham
			ItmsGrpCod int,
			QtyFG	numeric(28,10),
			Material_Amt_pro09_FG numeric(28,10),
						--
			ItemRework varchar(100), -- item rework trong bom (Line Bom (ITT1)
			QtyRework  numeric(28,10),
			IsHaveBom char(1),-- Item code này có thiết lập bom hay không: Y: Có, N: Không 
			Material_Amt_pro09	numeric(28,10),
			Material_Amt_pro09_Rework numeric(28,10),
						
			Material_IssuedQty_pro09 numeric(28,10),
						--
			ProductType varchar(10),
			BomRate_Oil	numeric(28,10),
			IsMaxMaterialStd char(1)
		);
--
		declare Upgrade table
		(
			ItemCode nvarchar(50),
			ItemRework nvarchar(50),
			IsRework char(1),
			-- quantity
			Stock_Qty_RW numeric(28,10),
			Stock_Qty numeric(28,10),
			Upgrade_Qty numeric(28,10),
			Total_RW_Qty numeric(28,10),
			--
			QTY_RW_Pro04 numeric(28,10),
			--
			Stock_Value numeric(28,10),
			Upgrade_Value numeric(28,10),
			--
			TotalDiff_Qty numeric(28,10),
			TotalDiff_Value numeric(28,10),
			--
			SalesQty numeric(28,10), --Delivery Qty
			CONSUMED_QTY numeric(28,10), --Consumed Qty
			Sale_Sample_Qty numeric(28,10), --Sale Sample Qty
			Test_Sample_Qty numeric(28,10),
			ALLOCTOSTOCK numeric(28,10),
			ALLOCTOCOGS numeric(28,10),
			Allocate_Sale_Sample numeric(28,10),
			Allocate_Test_Sample numeric(28,10),
			--
			RW_Stock_Amount	numeric(28,10),
			RW_Upgraded_Amount	numeric(28,10)
		);
		--
		declare  SSCC_FGCode_OutSourcing table
		(
				SSCC_OngNoi varchar(100),
				Admission_Date_OngNoi date,
				--
				ItemCode varchar(100),
				SSCC varchar(100),
				BaseType int,
				Qty numeric(28,10)
			
		);
			--
		declare  XUATDINHMUC table  
		(
				TransType	nvarchar(20), 
				CreatedBy	int, 
				DocEntry 	int, 
				FGCode 		nvarchar(50), 
				PlannedQty 	numeric(28,10), 
				ItemCode 	nvarchar(50), 
				InQty 		numeric(28,10), 
				OutQty 		numeric(28,10), 
				TransValue 	numeric(28,10), 
				WipAct 		nvarchar(20), 
				CardCode 	nvarchar(20)
		);
			--
		declare COMPLETED table 
		(
				DocEntry 	int,
				ItemCode 	nvarchar(50),
				InQty 		numeric(28,10),
				OutQty		numeric(28,10),
				TransValue	numeric(28,10),
				POType		varchar(1)
		);
		--	
		declare XUATCL table 
		(
			TransType	nvarchar(20),
			DocID		int,
			ItemCode 	nvarchar(50),
			InQty 		numeric(28,10),
			OutQty		numeric(28,10),
			CardCode	nvarchar(20),
			TransValue	numeric(28,10),
			PriceDifAc	nvarchar(20),
			PriceDiff	numeric(28,10),
			IncAct		nvarchar(20),
			IncVal		numeric(28,10),
			DecAcc		nvarchar(20),
			DecVal		numeric(28,10),
			OtherRM		nvarchar(1),--default 'N',
			OcrCode		nvarchar(10),
			PrcCode		nvarchar(10),
			InQty_PrdLine 	numeric(28,10),
			OutQty_PrdLine	numeric(28,10),
			TransValue_PrdLine	numeric(28,10),
			PriceDiff_PrdLine	numeric(28,10),
			IncVal_PrdLine	numeric(28,10),
			DecVal_PrdLine	numeric(28,10)
		);
		--
		declare COSTING_DETAIL table  (
			BrchID 			int, 
			PrdID 			int, 
			PrjCode 		nvarchar(20), 
			FGCode 			nvarchar(50), 
			FGName 			nvarchar(100), 
			FGPlnQty 		numeric(28,10), 
			FGCmpltQty 		numeric(28,10),
			FGCmpltAmount	numeric(28,10),
			FGOpnQty 		numeric(28,10), 
			PrdStatus 		char(1),
			ComponentNoGroup	smallint,
			ComponentNo 	nvarchar(50), 
			ComponentName 	nvarchar(100), 
			BaseQty 		numeric(28,10), 
			TotBaseQty 		numeric(28,10),
			PlnQty 			numeric(28,10), 
			StdQty 			numeric(28,10), 
			StdVal 			numeric(28,10), 
			TotStdQty 		numeric(28,10), 
			TotStdVal 		numeric(28,10), 
			ActQty 			numeric(28,10), 
			ActVal 			numeric(28,10), 
			DifQty 			numeric(28,10), 
			DifVal 			numeric(28,10), 
			TotDifQty 		numeric(28,10), 
			TotDifVal 		numeric(28,10),
			LaborHourStd	numeric(28,10),
			LBStd 			numeric(28,10), 
			MachineHourIssue numeric(28,10),
			FreezedriedHourIssue numeric(28,10),
			FreezedriedHourProJournal numeric(28,10),
			MachineHourStd	numeric(28,10),
			TotalMachineExpense numeric(28,10), 
			MCStd 			numeric(28,10), 
			OHStd 			numeric(28,10),
			WIPQty 			numeric(28,10), 
			WIPVal 			numeric(28,10), 
			isSemi			char(1),-- default 'N',
			Note 			nvarchar(100)
		);
		declare PhanBo_FGQty_Temp table 
		(
			GrpID 	 int, 
			FGCode 	 nvarchar(50),
			FGCmpltQty numeric(28,10),
			LBAct  numeric(28,10),
			Material_Act numeric(28,10),
			GeneralProCosts  numeric(28,10)
			
		);
		--
		declare fdate, tdate timestamp;
		declare totRMOther, totRMAct, totalRMAct, totLBAct, totOHAct, totMCAct, totSubCogs, totalMaterial_Act numeric(28,10);
		declare total627,totalDebit627, totalDebit6279, totalCredit627,totalCredit6279,total6279,total_154503, total622, total6229 decimal(28,10);
		declare totalLaborHourStd, TotalLaborExpense,TotalMachineHourStd,TotalMachineExpense,TotalOverheadExpense,Total627_Not627402_6279,
					Total_Material_Amt,Total_Material_Coffee_Oil_Amt,TotalMaterialExpense, TotalFGCmpltAmount,TotalFGCmpltQty   decimal(28,10);
		declare idx int;
		declare fgcode nvarchar(50);
		declare TotAloc, DifAloc, TotalVal, DifTotal decimal(28,10);
		declare Total_Amount_NVL_Oil, M_Coffee_Oil_Amt,Total_FGQty_TypeF, FGQty_ThanhPham, Total_Amount_NVL_ThanhPham,totalFGConsumption decimal(28,10);
						
		
		
		declare pro11 table
		(
			STDQTY DECIMAL(21,6),
			FGCODE VARCHAR(50),
			ITMSGRPCOD int,
			ITEMCODE VARCHAR(50),
			WOID int,
			U_Group nvarchar(20)
		);
		declare pro09_NVLPhu_WipOIL table
		(
			FGCODE nvarchar(50),
			FGCmpltQty  decimal(21,6),
			TotalCmpltQty decimal(21,6),
			Total_CmpltQty_FG_RW decimal(21,6),
			ITMSGRPCOD int,
			NL_VLPhu_Amount decimal(21,6), -- Nguyen lieu, Vat lieu phu (Sub_Materials_pro09 - tổng Amount của PRO09 của item group 114)
			Wip_OiL_Amount	decimal(21,6), --tổng Amount của PRO09 của item group 127(WIP_Oil_Coffee_pro09)
			ItemRwork_Bom nvarchar(50),
			
			Sub_Materials_6329  decimal(21,6),
			WIP_Oil_Coffee_6329  decimal(21,6),
			
			Total_Sub_Materials_Rework decimal(21,6),
			Total_Wip_OiL_Rework decimal(21,6),
			
			---Labor (Act)_Draft
			TotalCmpltQty_NVLGiaCong decimal(21,6),	
			Labor_Act_Draft decimal(21,6),
			Overhead_Act_Draft decimal(21,6),
			Processingfee_Act_Draft decimal(21,6),
			General_Production_Costs_Act_Draft decimal(21,6),
			 
			ALLOCTOCOGS_6329_NL_VLPhu decimal(21,6),
			ALLOCTOCOGS_6329_Wip_OiL decimal(21,6)
			
		);
		declare tblFGConsumption table
		(
			FGCode nvarchar(50),
			Sub_FGCode nvarchar(50),
			FGCmpltQty  decimal(21,6),
			FGCmpltAmount	numeric(28,10)
		);
		declare pro09Sum table
		(
			FGCode nvarchar(50),
			ItemCode nvarchar(50),
			ItmsGrpCod int ,
			Amount numeric(28,10)
			
		);
		select "F_RefDate", "T_RefDate" into fdate,tdate from "OFPR" where "Code"=period;
				--============================ Dữ liệu pro09
			 -- fromdate,todate,resource,outsourcing, po no, itemcode
			CALL ILD_R_IRS_LenhSanXuatDaNhapThanhPham(fdate,tdate,'Y','N','','', datapro09);
			Insert into  :pro09 ("CmpltQty","AmountFG","Amount", "SdtAmount",
									"IssuedQty", "FGCode","ItmsGrpCod","ItemCode", "DocNum")
									
			select sum("CmpltQty"),sum("AmountFG"),sum("Amount"), sum("PlanCompltAmount"),
					sum("IssuedQty"), "FGCode", "ItmsGrpCod", "ItemCode","DocNum"
			from :datapro09 
			where ("ItmsGrpCod"   IN (113,114,121,127,233,253,254,256,126,120) or "LineNum"  =  911)
															--"ItmsGrpCod" NOT IN (125,126,233) and ifnull("U_Group",'') not in ('MC_TTMC','LB_TTLB')
			group by "FGCode","ItemCode", "ItmsGrpCod" , "DocNum";
														
			DELETE FROM  :pro09 WHERE "FGCode" in (select  WIP_GB_CODE from TBL_COSTING_TOTAL_STEP1);
		
		--============================ Dữ liệu pro04
			--fdate, tdate, itemcode, warehouse, batch, DocNumPO, OutSoucing (Truoc la  OutSoucing = N. bo sung them OUTSOURCING_TYPE
			CALL USP_RPT_PROD_04(fdate,tdate,'','','','','Y', datapro04);
			Insert into  :pro04 (AMOUNT,QTY,MATERIALS,MACHINE,LABOUR,
									ItemCode,WOID,WONo, OUTSOURCING_TYPE)
			
			select Sum(AMOUNT),Sum(QTY),sum(MATERIALS),sum(MACHINE) ,sum(LABOUR),
					ItemCode,WOID,WONo, OUTSOURCING_TYPE
			from :datapro04 
			WHERE ifnull(Doc_Exception,'')  <> 'Y'
			group by ItemCode,WOID,WONo, OUTSOURCING_TYPE; 
		--============================ Dữ liệu pro05 (fdate, tdate, warehouse,  batch, DocNumPO, itemcode, Resource)
			call USP_RPT_PROD_05 (fdate,tdate,'','','','','Y',datapro05);
		 	insert into :pro05(ItemCode, Qty, FATHER1_Date)
		 	select ItemCode, Qty, FATHER1_Date
		 	from  :datapro05 
		 	where GrpCode  in ( 121,127,253);

		-- Lay Du lieu SSCC Cha --> End Qty (bo SSCC co SSCC Cha khong tao trong ky costing)
		call ILD_TruyXuatSSCCNhieuCap (tab_result); 
		create local temporary table #TruyXuatSSCC2 as (select * from :tab_result);
		--============================ Danh sách item thành phẩm
		insert into :ItemTPham (ItemCode)
		SELECT distinct a."ItemCode" 
		FROM IGN1 a 
		left JOIN OIGN b  ON a."DocEntry" = b."DocEntry"
		left join OITM c on c."ItemCode" =a."ItemCode"
		left join OITB d on d."ItmsGrpCod" = c."ItmsGrpCod"
		left join "@TBL_OUTS_DOCNUM" e on e."U_receiptID" = a."DocEntry"
		WHERE d."ItmsGrpCod" in (120,121,127) and a."DocDate"  between  fdate and tdate; 

	--============================ Danh sach itemCode
	insert into  :tblITEM ("ItemCode","ItemName","ItmsGrpCod") select "ItemCode","ItemName","ItmsGrpCod" from "OITM" ;
		-- Xuất theo định mức
		insert into :XUATDINHMUC  (TransType,  CreatedBy, DocEntry, FGCode, PlannedQty,  ItemCode,
										InQty, OutQty, TransValue, WipAct, CardCode)
		select a."TransType",a."CreatedBy",b."DocEntry",b."ItemCode" as FGCode, b."PlannedQty", a."ItemCode", a."InQty", a."OutQty", a."TransValue", a."WipAct",a."CardCode"
		from "OINM" a, "OWOR" b, "OWHS" c  
		where a."ApplObj"='202'  
		 and a."AppObjAbs"=b."DocEntry"
		 and a."DocDate" between fdate and tdate and a."Warehouse"=c."WhsCode"  
		 --and   a."ItemCode" in (select ItemCode from :ItemTPham) 
		 and ifnull(c."BPLid",0)=branch;
		 
		-- SỐ LƯỢNG HOÀN THÀNH
		insert into :COMPLETED (DocEntry, ItemCode, InQty, OutQty, TransValue, POType)
		select a."DocEntry",a."ItemCode",b."InQty",b."OutQty" , b."TransValue", a."Type"
		from "OWOR" a,"OINM" b  
		where  
			a."DocEntry"=b."AppObjAbs" and b."ApplObj"='202' and a."ItemCode"=b."ItemCode" 
		and b."DocDate" between fdate and tdate;		
		-- Xuất chênh lệch
		insert into :XUATCL (TransType, DocID,  ItemCode, InQty, OutQty,
							 CardCode, TransValue, PriceDifAc, PriceDiff,
							 IncAct, IncVal,  DecAcc, DecVal, OtherRM,
							 OcrCode,PrcCode, InQty_PrdLine,  OutQty_PrdLine,TransValue_PrdLine,
							  PriceDiff_PrdLine ,IncVal_PrdLine, DecVal_PrdLine)
			select a."TransType",a."CreatedBy",a."ItemCode",a."InQty",a."OutQty",
					a."CardCode",a."TransValue",a."PriceDifAc",a."PriceDiff",
					a."IncAct",a."IncVal",a."DecAcc",a."DecVal", 'Y' OtherRM,
					 a."OcrCode", c."PrcCode",
			case when ifnull(a."OcrCode",'')='' then a."InQty" else a."InQty"*c."PrcAmount"/c."OcrTotal" end InQty_PrdLine, 
			case when ifnull(a."OcrCode",'')='' then a."OutQty" else a."OutQty"*c."PrcAmount"/c."OcrTotal" end OutQty_PrdLine, 
			case when ifnull(a."OcrCode",'')='' then a."TransValue" else a."TransValue"*c."PrcAmount"/c."OcrTotal" end TransValue_PrdLine, 
			case when ifnull(a."OcrCode",'')='' then a."PriceDiff" else a."PriceDiff"*c."PrcAmount"/c."OcrTotal" end PriceDiff_PrdLine,
			case when ifnull(a."OcrCode",'')='' then a."IncVal" else a."IncVal"*c."PrcAmount"/c."OcrTotal" end IncVal_PrdLine, 
			case when ifnull(a."OcrCode",'')='' then a."DecVal" else a."DecVal"*c."PrcAmount"/c."OcrTotal" end DecVal_PrdLine
		from "OINM" a
			inner join "OWHS" b on (a."Warehouse"=b."WhsCode" and ifnull(b."BPLid",0)=branch)
			left join "OCR1" c on (c."OcrCode"=a."OcrCode" and a."DocDate" between c."ValidFrom" and ifnull(c."ValidTo",'2099-12-31'))
		where a."DocDate" between fdate and tdate and a."ApplObj"<>'202' 
			and ((a."CardCode" like '154%' or a."PriceDifAc" like '154%' or a."IncAct" like '154%' or a."DecAcc" like '154%')
				or (a."CardCode" like '621%' or a."PriceDifAc" like '621%' or a."IncAct" like '621%' or a."DecAcc" like '621%'));
				
		
		update :XUATCL set PrcCode = ifnull(PrcCode,'');
		--------------------------------------=================================
		insert into :COSTING_DETAIL(BrchID,PrdID,PrjCode,FGCode,FGPlnQty,ComponentNo,ComponentNoGroup,BaseQty,PlnQty,PrdStatus)
		select branch,a."DocEntry",a."Project",a."ItemCode",a."PlannedQty",b."ItemCode",c."ItmsGrpCod",sum(b."BaseQty"),sum(b."PlannedQty"),a."Status"
		from "OWOR" a, "WOR1" b, OITM c
		where  ifnull(b."PlannedQty",0) > 0 and a."DocEntry"=b."DocEntry" and b."ItemCode" = c."ItemCode"
		and a."DocEntry" in (select x.DocEntry from :XUATDINHMUC x)
		group by a."DocEntry",a."Project",a."ItemCode",a."PlannedQty",b."ItemCode",c."ItmsGrpCod",a."Status";
		----
		insert into :COSTING_DETAIL (FGCode, FGCmpltQty)
		select ItemCode,0 from :ItemTPham where ItemCode not in (select ItemCode from :COMPLETED);
		-- Số lượng hoàn thành theo lệnh sx
		update :COSTING_DETAIL 	set FGCmpltQty = (select sum(InQty-OutQty) from :COMPLETED x where  x.DocEntry=PrdID and x.ItemCode=FGCode );
		--				
		update :COSTING_DETAIL 
		set FGCmpltAmount = (select sum(TransValue) 
									from :COMPLETED x 
									where  x.DocEntry=PrdID and x.ItemCode=FGCode );
									--where  x.DocEntry=PrdID and x.ItemCode=FGCode and POType <>  'D');
		-----
		insert into :pro11 (STDQTY,FGCODE, ITMSGRPCOD, ITEMCODE,WOID )
		select STDQTY,FGCODE, ITMSGRPCOD, ITEMCODE,WOID
		from  PRO11_DATA_TEMP
		where POSTINGDATE between fdate and tdate;
		
--- insert thêm dư liệu từ bảng vât lý COSTING_STANDARDCOST
		insert into :pro11 (STDQTY,FGCODE, ITMSGRPCOD, ITEMCODE,WOID, U_Group)
		select STDQTY,FGCODE, ITMSGRPCOD, ITEMCODE,WOID, U_Group
		from  COSTING_STANDARDCOST
		where POSTINGDATE between fdate and tdate;

        
        
        Update a 
        set a.LaborHourStd =  (select sum(b.STDQTY) 
        							from  :pro11 b
        						 where  a.FGCode  = b.FGCODE 
        						 			and  (b.ITMSGRPCOD = 126  or ifnull(b.U_Group,'')  ='LB_TTLB')
        						 				and  a.ComponentNo  = b.ITEMCODE and a.PrdID = b.WOID) 
        from :COSTING_DETAIL a;	
        
 
		--
		select sum(LaborHourStd) into totalLaborHourStd from :COSTING_DETAIL;

--  Giờ máy theo Plan		
 		Update a 
        set a.MachineHourIssue =  (select sum(b.STDQTY) 
        							from  :pro11 b
        							where  a.FGCode  = b.FGCODE 
        						 			and (b.ITMSGRPCOD = 125  or ifnull(b.U_Group,'')  ='MC_TTMC')
        						 			and  a.ComponentNo  = b.ITEMCODE and a.PrdID = b.WOID) 
        						 
        from :COSTING_DETAIL a;	
		
/* -- Tam thoi khong xai 
	--  Freeze dried Hour (Issue)
	Update x 
		set FreezedriedHourIssue  = (select ifnull(sum("Quantity"),0)
										from IGE1 a
										inner join OWOR c on c."DocEntry" = a."BaseEntry"
										inner join "@PRO_JOURNAL_H" d on d."DocEntry" = c."U_Pro_Journal"
										inner join "@PRO_JOURNAL_L" e on e."DocEntry" = d."DocEntry" and e."U_ItemCode" = a."ItemCode"
										inner join OITM f on f."ItemCode" = a."ItemCode"
										where a."BaseType"  ='202' and a."DocDate"  between fdate and tdate
										and  f."ItmsGrpCod"  = '124' and a."BaseEntry" = x.PrdID  and x.FGCode =c."ItemCode"
										and	x.ComponentNo = e."U_ItemCode"
								 )
		from :COSTING_DETAIL x;	
	-- Giờ máy chạy theo thực tế 

   
		Update x 
		set FreezedriedHourProJournal  = (select ifnull(sum(b."U_Machine") ,0)
										
										from  "@PRO_JOURNAL_H" a
										inner join "@PRO_JOURNAL_L" b on a."DocEntry" = b."DocEntry" 
										inner join OITM c on c."ItemCode" = b."U_ItemCode"
										where  b."U_WorkDate"  between fdate and tdate
										and  c."ItmsGrpCod"  = '124' and a."U_ProID" = x.PrdID  and x.FGCode =a."U_ItemCode"
										and	x.ComponentNo = b."U_ItemCode"
								 )
		from :COSTING_DETAIL x;	
    */
-- MachineHourStd
	update  :COSTING_DETAIL 
	set MachineHourStd = ifnull(MachineHourIssue,0) - ifnull(FreezedriedHourIssue,0) + ifnull(FreezedriedHourProJournal,0); 
	
-- TotalMachineHourStd
		select sum(MachineHourStd) into TotalMachineHourStd
		from :COSTING_DETAIL;
		


		update :COSTING_DETAIL set PrdStatus='L' where ifnull(FGCmpltQty,0)>=ifnull(FGPlnQty,0);
		-- Nguyên vật liệu xuất theo định mức
			update a
			set StdQty = (select sum(OutQty-InQty) from :XUATDINHMUC x where x.DocEntry=a.PrdID and x.ItemCode= a.ComponentNo)
			from  :COSTING_DETAIL a;
			---
			update a
			set  StdVal = (select sum(-TransValue) from :XUATDINHMUC x where x.DocEntry=a.PrdID and x.ItemCode= a.ComponentNo) 
			from  :COSTING_DETAIL a;
			
			
-- Nhân công, Khấu hao, Chi phí chung định mức
		
		-- Số lượng xuất thêm theo mã NVL
		update :COSTING_DETAIL set 
			TotDifQty	= (select sum(x.OutQty_PrdLine-x.InQty_PrdLine) from :XUATCL x where x.ItemCode=ComponentNo and (CardCode like '621%')),
			TotDifVal	= ifnull((select sum(-x.TransValue_PrdLine) from :XUATCL x where x.ItemCode=ComponentNo and (CardCode like '621%')),0)
						+ ifnull((select sum(x.IncVal_PrdLine) from :XUATCL x where x.ItemCode=ComponentNo and (IncAct like '621%')),0)
						+ ifnull((select sum(x.DecVal_PrdLine) from :XUATCL x where x.ItemCode=ComponentNo and (DecAcc like '621%')),0)
						+ ifnull((select sum(x.PriceDiff_PrdLine) from :XUATCL x where x.ItemCode=ComponentNo and PriceDifAc like '621%'),0);
			
		-- Phân bổ tổng chênh lệch cho từng lệnh sx theo tỷ lệ tổng xuất định mức
		create local temporary table #tblTMP1 (ComponentNo nvarchar(50), TotBaseQty numeric(28,10), TotStdQty numeric(28,10), TotStdVal numeric(28,10));
		insert into #tblTMP1 (ComponentNo,TotBaseQty,TotStdQty,TotStdVal)
		select ComponentNo, sum(x.BaseQty*x.FGCmpltQty),sum(x.StdQty),sum(x.StdVal) from :COSTING_DETAIL x group by ComponentNo;

		
		update x set TotBaseQty = y.TotBaseQty, TotStdQty = y.TotStdQty, TotStdVal = y.TotStdVal 
		from :COSTING_DETAIL x inner join #tblTMP1 y on y.ComponentNo=x.ComponentNo;

		update :COSTING_DETAIL set
			DifQty	= TotDifQty*StdQty/TotStdQty,
			DifVal	= TotDifVal*StdQty/TotStdQty
		where ifnull(TotStdQty,0)<>0;
		
		update :COSTING_DETAIL set
			DifQty	= TotDifQty*FGCmpltQty*BaseQty/TotBaseQty,
			DifVal	= TotDifVal*FGCmpltQty*BaseQty/TotBaseQty
		where ifnull(TotStdQty,0)=0 and ifnull(TotBaseQty,0)<>0;
				
		-- Thực tế = Định mức + Chênh lệch

		update :COSTING_DETAIL set
			ActQty	= ifnull(StdQty,0) + ifnull(DifQty,0),
			ActVal	= ifnull(StdVal,0) + ifnull(DifVal,0);

		-- Đánh dấu những NVL phát sinh chi phí 621, 154, mà những mã NVL này không nằm trong các lệnh sản xuất
		update :XUATCL set OtherRM = 'Y' where ItemCode not in (select ComponentNo from :COSTING_DETAIL);
		

		--
		update :COSTING_DETAIL set FGOpnQty = ifnull(FGPlnQty,0) - ifnull(FGCmpltQty,0) where PrdStatus<>'L';
		update :COSTING_DETAIL set FGOpnQty = 0 where PrdStatus='L';
		update :COSTING_DETAIL set FGName = (select "ItemName" from :tblITEM where "ItemCode"=FGCode);
		update :COSTING_DETAIL set ComponentName = (select "ItemName" from :tblITEM where "ItemCode"=ComponentNo);
		
		-- Đánh dấu là Bán Thành phẩm (isSemi =  Y)/ Thành phẩm
		update x set isSemi = 'Y' 
		from :COSTING_DETAIL x, "OITT" y 
		where x.ComponentNo=y."Code" or x.PlnQty <0;
		
		delete from TBL_COSTING_DETAIL where PerCode=period and ifnull(BrchID,0)=branch;
		insert into TBL_COSTING_DETAIL(PerCode, BrchID, PrdID,PrjCode, FGCode, FGName, FGPlnQty, FGCmpltQty,FGCmpltAmount,FGOpnQty,
		PrdStatus, ComponentNo,LaborHourStd,MachineHourIssue,FreezedriedHourIssue,FreezedriedHourProJournal,MachineHourStd,
		 ComponentName,BaseQty,PlnQty,StdQty,StdVal,TotStdQty,TotStdVal,ActQty,ActVal,DifQty,DifVal,WIPQty,WIPVal,TotDifQty,TotDifVal)
		 
		select period, BrchID, PrdID,PrjCode, FGCode, FGName, FGPlnQty, FGCmpltQty,FGCmpltAmount,FGOpnQty,
		PrdStatus, ComponentNo, LaborHourStd,MachineHourIssue,FreezedriedHourIssue,FreezedriedHourProJournal,MachineHourStd,
		ComponentName,BaseQty,PlnQty,StdQty,StdVal,TotStdQty,TotStdVal,ActQty,ActVal,DifQty,DifVal,WIPQty,WIPVal,TotDifQty,TotDifVal
		 from :COSTING_DETAIL;
		 
		 
		-- TỔNG HỢP CÁC LOẠI CHI PHÍ: -----------------------------------------------------------------------------------------------------------------------------
		create local temporary table  #COSTING_SUM (
			BrchID 				int, 
			PrdID 				int, 
			PrdNo 				nvarchar(20),
			PrjCode 			nvarchar(20),
			GrpID 				int,
			GrpName 			nvarchar(50),
			ProductLine 		nvarchar(20), 
			FGCode 				nvarchar(50), 
			FGName 				nvarchar(100), 
			FGPlnQty 			numeric(28,10), 
			FGCmpltQty 			numeric(28,10), 
			FGCmpltAmount		numeric(28,10), 
			PrdStatus 			char(1),
			SGStd 				numeric(28,10),
			SGAct 				numeric(28,10), 
			RMStd 				numeric(28,10), 
			RMAct 				numeric(28,10),
			Material_Act 		numeric(28,10),
			RMDiff 				numeric(28,10), 
			 
			RMOther 			numeric(28,10), 
			LaborHourStd		numeric(28,10), 
			MachineHourIssue 	numeric(28,10),
			FreezedriedHourIssue numeric(28,10),
			FreezedriedHourProJournal numeric(28,10), 
			MachineHourStd		numeric(28,10),
			TotalMachineExpense numeric(28,10),
			LBPrice				numeric(28,10), 
			LBStd 				numeric(28,10), 
			MCStd 				numeric(28,10), 
			OHStd 				numeric(28,10),
	
			LBAct 				numeric(28,10), -- Nhân công trực tiếp
			LBDiff 				numeric(28,10), 

			OHAct 				numeric(28,10), -- Chi phí chung
			OSAct 				numeric(28,10), -- Chi phí gia công
			MCAct 				numeric(28,10), -- Chi phí khấu hao
			MCDiff 				numeric(28,10), -- Machine Diff
			LBIndAct 			numeric(28,10), -- Chi phí nhân công gián tiếp
			ToolAct 			numeric(28,10), -- Chi phí NVL phụ, CCDC
			RepairAct 			numeric(28,10), -- Chi phí sửa chữa
			ElecAct 			numeric(28,10), -- Chi phí điện
			
			RateLB				numeric(28,15),	-- Tỷ lệ giờ công
			RateMC				numeric(28,15), -- Tỷ lệ giờ máy
			RateLB_MC			numeric(28,15), -- Tỷ lệ giờ máy + giờ máy
			
			ItemPrice			numeric(28,10),	-- Đơn giá sản phẩm: Có thể là giá bán, giá ... dùng để phân bổ chi phí theo giá trị sản phẩm
			ItemAmount			numeric(28,10),	-- = SLSX * ItemPrice: Tổng giá trị sản phẩm ... dùng để phân bổ chi phí theo giá trị sản phẩm
			ItemRate			numeric(28,15), -- Tỷ lệ về giá trị sản phẩm: = ItemAmount/Tổng ItemAmount... dùng để phân bổ chi phí theo giá trị sản phẩm
			
			WIPOB 				numeric(28,10), 
			WODiff				numeric(28,10),
			TotStd 				numeric(28,10), --Tổng chi phí định mức
			TotAct 				numeric(28,10), --Tổng chi phí thực tế
			TotDif 				numeric(28,10), 
			AllocAmt 			numeric(28,10), 
			WIPAmt 				numeric(28,10)
		);
		
		
		insert into #COSTING_SUM(BrchID,PrdID,PrjCode,FGCode,FGPlnQty,FGCmpltQty,FGCmpltAmount,PrdStatus,SGStd,
						--RMStd,
						LaborHourStd,
						--LBStd,
						MachineHourIssue,FreezedriedHourIssue,FreezedriedHourProJournal,MachineHourStd,
						OHStd,SGAct
						,RMAct
						,WIPAmt)
		select BrchID,PrdID,PrjCode,FGCode,FGPlnQty,FGCmpltQty,FGCmpltAmount,PrdStatus,
			sum(case when ifnull(isSemi,'')='Y' then StdVal else 0 end) as "SGStd", -- dinh muc ban thanh pham
			--sum(case when isSemi='N' then StdVal else 0 end) as "RMStd", -- dinh muc thanh pham
			sum(LaborHourStd),
			--sum(LBStd),
			sum(MachineHourIssue),
			sum(FreezedriedHourIssue),
			sum(FreezedriedHourProJournal),
			sum(MachineHourStd),
			sum(OHStd),
			sum(case when ifnull(isSemi,'')='Y' then ActVal else 0 end) as  "SGAct",
			sum(case when ifnull(isSemi,'') <> 'Y' then ActVal else 0 end) as "RMAct",
			sum(WIPVal)
		from :COSTING_DETAIL
		group by BrchID,PrdID,PrjCode,FGCode,FGPlnQty,FGCmpltQty,FGCmpltAmount,PrdStatus;
		update #COSTING_SUM set PrdNo = (select x."DocNum" from "OWOR" x where x."DocEntry"=#COSTING_SUM.PrdID);
		
--Materials (Std) = Tổng cột Materials của PRO04
 /*
	update a
	set  RMStd =   round((select  sum(b.Materials) from  :pro04 b 
					where   a.FGCODE  = b.ItemCode and a.PrdID  = b.WOID) ,2)
	from #COSTING_SUM a;
*/

 /*
(select sum (round(STDAMOUNT,2)) from :pro11 b
			where  a.WONO = b.WONO and a.DOCID = b.DOCENTRY and a.DOCTYPE =  59 
				and a.ItemCode  = b.FGCODE 
				and b.ITMSGRPCOD not in (125,126,233)  and ifnull(b.U_Group,'') not in ('MC_TTMC','LB_TTLB'))
*/
	
	
--Machine (Std) = tổng cột Machine trên PRO04
	update a
	set  MCStd =   (select  sum(b.Machine) from  :pro04 b 
					where   a.FGCODE  = b.ItemCode and a.PrdID  = b.WOID and ifnull(b.OUTSOURCING_TYPE,'')  = '') 
	from #COSTING_SUM a;
	 
-- Labor (Std) = tổng cột Labor trên PRO04
	update a
	set  LBStd =   (select   sum(b.LABOUR)  from  :pro04 b 
					where   a.FGCODE  = b.ItemCode and a.PrdID  = b.WOID and ifnull(b.OUTSOURCING_TYPE,'')  = '') 
	from #COSTING_SUM a;
 		--============================ Số liệu hạch toán theo tài khoản
		insert into :tblEstimateCost (Ref3Line,Account,TransType, Debit, Credit, Amount, Amount_Debit_Credit)
		select "Ref3Line","Account","TransType",Sum("Debit"), Sum("Credit"), sum("Credit"-"Debit") as Amount ,  sum("Debit"-"Credit") as Amount_Debit_Credit 
		from "JDT1"
		where ("Account" like '621%' or "Account" like '622%' or "Account" like '627%' or  "Account" like '154%') 
				and "RefDate" between fdate and tdate group by "Ref3Line","Account","TransType";
	
--- TotalMachineExpense: Total Machine Expense
/*
     select ifnull(sum(Amount_Debit_Credit),0) into TotalMachineExpense  default 0
     from :tblEstimateCost 
     where Account = '627402' ; 
*/
     
--- TotalMaterialExpense: Total Material Expense (NVL)
  select ifnull(sum(Amount_Debit_Credit),0) into TotalMaterialExpense 
  from :tblEstimateCost 
  where Account like '621%' ;
--- TotalLaborExpense: Total Labor Expense
  select ifnull(sum(Amount_Debit_Credit),0) into total622  default 0
  from :tblEstimateCost 
  where Account like '622%' ;
 ---
  select ifnull(sum(Amount_Debit_Credit),0) into total6229  default 0
  from :tblEstimateCost 
  where Account like '6229%' ;
 ---
   select ifnull(sum(Amount_Debit_Credit),0) into Total627_Not627402_6279  default 0
  from :tblEstimateCost 
  where Account like '627%' and ( Account not like '627402%' and Account not like '6279%') ;

-- Chi phí Nhân công, Khấu hao, chung thực tế -----------------------------------------------------------------------------------------------------------
		update #COSTING_SUM set 
			ProductLine	= ifnull(ProductLine,''),
			RMAct		= ifnull(RMAct,0),
			RMOther		= iFnull(RMOther,0);

			

	
		

		
		-- Phân bổ các loại chi phí-------------------------------------------------------------------------------------------------------------------------
		--update x set LBPrice = z."U_LBPrice" from #COSTING_SUM x inner join "@LB_PRICE" y on y."U_FromDate"<=tdate and y."U_ToDate">=tdate inner join "@LB_PRICE_1" z on z."DocEntry"=y."DocEntry" and z."U_ItemCode"=x.FGCode;
		
		--select sum(RMAct) into totalRMAct from #COSTING_SUM ;
		--select sum(FGCmpltQty*LBPrice) into totalRMAct from #COSTING_SUM;
		select sum(FGCmpltQty) 	into totalRMAct from #COSTING_SUM ;
	 	
		--Total Qty
		totalRMAct = ifnull(totalRMAct,0)+				
		
									  (select ifnull(sum("InQty"-"OutQty"),0)
														from  OINM a
														where    a."ApplObj" <> '202'  
																and a."DocDate" between fdate and tdate 
																and a."ItemCode" in (select ItemCode from :COMPLETED)
																);
		-- totalDebit627
			select sum(Debit)	into totalDebit627 from  :tblEstimateCost where Account like '627%';
		-- totalCredit627
			select sum(Credit)	into totalCredit627 from  :tblEstimateCost where Account like '627%';
		-- 
			select sum(Amount_Debit_Credit)	into total627 from  :tblEstimateCost where Account like '627%';
			
		-- totalDebit627
			select sum(Debit)	into totalDebit6279 from  :tblEstimateCost where Account like '6279%';
		-- totalCredit627
			select sum(Credit)	into totalCredit6279 from  :tblEstimateCost where Account like '6279%';
		-- total6279
			select sum(Amount_Debit_Credit)	into total6279 from  :tblEstimateCost where Account like '6279';
	    -- total6279
			select sum(Amount_Debit_Credit)	into total_154503 from  :tblEstimateCost where Account like '154503';
			
			
--
--- TotalMachineExpense: Total Machine Expense
			TotalMachineExpense  = (total627)  -  (:total6279)  + (total_154503);
			TotalOverheadExpense  = (total627)  -  (:total6279);
			TotalLaborExpense = total622 -  total6229; 
--
		update #COSTING_SUM set 
			TotStd	= ifnull(SGStd,0) + ifnull(RMStd,0) + ifnull(LBStd,0) + ifnull(MCStd,0) + ifnull(OHStd,0);

		--- Machine (Act)
		
		update #COSTING_SUM a set MCAct	= ifnull((select sum(Amount_Debit_Credit) from :tblEstimateCost where Account = '627402'),0);
		
		-- Machine Diff: Machine (Act) - Machine (Std)
		update #COSTING_SUM a set MCDiff	=  MCAct  - ifnull(MCStd,0);
		-- Chi phí sản xuất chung: Phân bổ theo tỷ lệ tổng chi phí định mức
		update #COSTING_SUM 
		set OHAct	= round((TotStd /:totalRMAct) * totOHAct, 2)
		where totalRMAct * totOHAct <>0;
		-- Chi phí Khau hao: Phân bổ theo tỷ lệ tổng chi phí định mức
		--update #COSTING_SUM set MCAct	= round((TotStd / :totalRMAct) * totMCAct, 2);

		-- WIP Openning Balance
		update #COSTING_SUM set WIPOB =0;
		-- Làm tròn
		update #COSTING_SUM set 
			SGAct		 = ROUND(SGAct,2),
			RMAct		 = ROUND(RMAct,2),
			RMOther		 = ROUND(RMOther,2),
			OHAct		 = ROUND(OHAct,2),
			OSAct		 = ROUND(OSAct,2),
			MCAct		 = ROUND(MCAct,2),
			LBIndAct	 = ROUND(LBIndAct,2),
			ToolAct		 = ROUND(ToolAct,2),
			RepairAct	 = ROUND(RepairAct,2),
			ElecAct		 = ROUND(ElecAct,2);
		---
		delete from TBL_COSTING_SUM where PerCode=period and ifnull(BrchID,0)=branch;
		insert into TBL_COSTING_SUM (PerCode,BrchID,PrdID,GrpID,GrpName,ProductLine,FGCode,FGName,FGPlnQty,FGCmpltQty,PrdStatus,SGStd,SGAct,RMStd,RMAct,RMOther,LBAct,OHAct,OSAct,MCAct,MCDiff,LBIndAct,ToolAct,RepairAct,ElecAct,RateLB,RateMC,ItemPrice,ItemAmount,ItemRate,WIPOB,WODiff,TotStd,TotAct,TotDif,AllocAmt,WIPAmt, RMDiff,Material_Act)
		select period,BrchID,PrdID,GrpID,GrpName,ProductLine,FGCode,FGName,FGPlnQty,FGCmpltQty,PrdStatus,SGStd,SGAct,RMStd,RMAct,RMOther,LBAct,OHAct,OSAct,MCAct,MCDiff,LBIndAct,ToolAct,RepairAct,ElecAct,RateLB,RateMC,ItemPrice,ItemAmount,ItemRate,WIPOB,WODiff,TotStd,TotAct,TotDif,AllocAmt,WIPAmt, RMDiff,Material_Act from #COSTING_SUM;
			
--===============================================================================================================================
		-- TỔNG HỢP THEO MÃ THÀNH PHẨM-----------------------------------------------------------------------------------------------------------------------------------------------
		create local temporary table #COSTING_TOTAL (
			ID 							int not null primary key generated by default as IDENTITY,
			BrchID 						int, 
			PrjCode 					nvarchar(20), 
			PrjName 					nvarchar(100), 
			PrdType 					nvarchar(20), 
			GrpID 						int, 
			GrpName 					nvarchar(50), 
			ProductLine 				nvarchar(50), 
			FGCode 						nvarchar(50), 
			FGName 						nvarchar(100), 
			Sub_FGCode 					nvarchar(50),
			Orginal_FGCode				nvarchar(50),
			
			FGPlnQty 					numeric(28,10), 
			FGCmpltQty 					numeric(28,10), 
			FGCmpltAmount 				numeric(21,6),
			PrdStatus 					char(1), 
			SGStd 						numeric(28,10), 
			SGAct 						numeric(28,10), 
			RMStd 						numeric(28,10), 
			RMAct 						numeric(28,10), 
			--
			RawMaterials_Pr09			numeric(28,10),
			RawMaterials_Actual			numeric(28,10),
			
			Sub_Materials_Pr09			numeric(28,10),
			Sub_Materials_Actual		numeric(28,10),	
			ADDITIVE_MATERIALS_PR09		numeric(28,10),
			ACTUAL_ADDITIVE_MATERIALS	numeric(28,10),
				
			Labour_Pr09					numeric(28,10),	
			Overhead_Pr09				numeric(28,10),	
			ProcessingFee_Pr09			numeric(28,10),	
			FGConsumption_Pr09			numeric(28,10),
			FGConsumption_Actual		numeric(28,10),
				
			WIP_GreenBean_Pr09			numeric(28,10),	
			WIP_GreenBean_Actual		numeric(28,10),	
			
			WIP_OilCoffee_Pr09			numeric(28,10),	
			WIP_OilCoffee_Actual		numeric(28,10),	
			ProcessingFee_Actual		numeric(28,10),	
			Total_Mnf_Cost				numeric(28,10),	
			Variance					numeric(28,10),	
			PROCCESSING_CONSPT_VAR		numeric(28,10),	
			ALLOC_PROCCESSING_CONSPT_VAR	numeric(28,10),	
			FINAL_VARIANCE				numeric(28,10),	
			TOTAL_MNF_COST_INCLPRO		numeric(28,10),	
			FINAL_UNIT_COST				numeric(28,10),	
			
			WIP_Rework_Pr09				numeric(28,10),
			WIP_Rework_Actual			numeric(28,10),
			StdUnitCost					numeric(28,10),
			
			Upgraded_Qty 				numeric(28,10),
			Upgrade_Adj 				numeric(28,10),
			Upgrade_Variance_After_Adj 	numeric(28,10),
			--

			Material_Act 				numeric(28,10),
			RMDiff						numeric(28,10),
			RMOther 					numeric(28,10), 
			LaborHourStd				numeric(28,10),
			MachineHourIssue			numeric(28,10),
			FreezedriedHourIssue 		numeric(28,10),
			FreezedriedHourProJournal 	numeric(28,10),
			MachineHourStd				numeric(28,10),
			TotalMachineExpense			numeric(28,10),
			LBStd 						numeric(28,10), 
			MCStd 						numeric(28,10), 
			OHStd 						numeric(28,10), 
			
			LBAct 						numeric(28,10), -- Nhân công trực tiếp
			LBDiff 						numeric(28,10),
			OHAct 						numeric(28,10), -- Chi phí chung
			OSAct 						numeric(28,10), -- Chi phí gia công
			Overhead_Act				numeric(28,10),
			Overhead_Diff				numeric(28,10),
			MCAct 						numeric(28,10), -- Chi phí khấu hao
			MCDiff 						numeric(28,10), -- Machine Diff
			LBIndAct 					numeric(28,10), -- Chi phí nhân công gián tiếp
			ToolAct 					numeric(28,10), -- Chi phí NVL phụ, CCDC
			RepairAct 					numeric(28,10), -- Chi phí sửa chữa
			ElecAct 					numeric(28,10), -- Chi phí điện 
			
			
			WIPOB 						numeric(28,10), 
			WODiff						numeric(28,10), 
			TotStd 						numeric(28,10), 
			TotAct 						numeric(28,10), 
			TotDif 						numeric(28,10), 
			AllocAmt 					numeric(28,10), 
			WIPAmt 						numeric(28,10),
			 
			EBQty 						numeric(28,10),
			EBAmt 						numeric(28,10),
			
			SalesQty					numeric(28,10),
			AllocToItem 				numeric(28,10), 
			AllocToCOGS 				numeric(28,10), 
			AllocFromSGToItem 			numeric(28,10), 
			AllocFromSGToCOGS 			numeric(28,10),
			Tot_Alloc_To_Item 			numeric(28,10), 
			Tot_Alloc_To_COGS 			numeric(28,10),
			Debit627					numeric(28,10),
			Credit627					numeric(28,10),
			Debit6279					numeric(28,10),
			Credit6279					numeric(28,10),
			TotalGeneralCost			numeric(28,10),
			GeneralProCosts				numeric(28,10), -- General production costs
			Gendiff						numeric(28,10),
			TotalCost					numeric(28,10),
			TotalDiff					numeric(28,10),
			UnitCost					numeric(28,10),
			
			ALLOCTOSTOCK				numeric(28,10),
			CONSUMED_QTY				numeric(28,10),
			
			Sale_Sample_Qty				numeric(28,10),
			Test_Sample_Qty				numeric(28,10),
			
			Allocate_Sale_Sample		numeric(28,10),
			Allocate_Test_Sample		numeric(28,10),
			
			ALLOCTOCOGS_6329 			numeric(28,10),
			ISFGConsumption				char(1)	
		);

	
		
		insert into #COSTING_TOTAL(BrchID,PrjCode,GrpID,GrpName,FGCode,FGName,FGPlnQty,FGCmpltQty,FGCmpltAmount,
		SGStd,SGAct,RMStd,RMAct,RMOther,LBStd,LaborHourStd,
		MachineHourIssue,FreezedriedHourIssue,FreezedriedHourProJournal,MachineHourStd,
		MCStd,OHStd,LBAct,OHAct,
		OSAct,MCAct,MCDiff,LBIndAct,ToolAct,RepairAct,ElecAct,WIPOB,WODiff,TotStd,
		TotAct,TotDif,AllocAmt,WIPAmt,
		LBDiff,
		RMDiff,
		Material_Act)
		
		select BrchID,ifnull(PrjCode,''),GrpID,GrpName,FGCode,FGName,sum(FGPlnQty),sum(FGCmpltQty),sum(FGCmpltAmount),
		sum(SGStd),sum(SGAct),sum(RMStd),sum(RMAct),sum(RMOther),sum(LBStd),sum(LaborHourStd),
		sum(MachineHourIssue),sum(FreezedriedHourIssue),sum(FreezedriedHourProJournal),sum(MachineHourStd),sum(MCStd),sum(OHStd),sum(LBAct),sum(OHAct),
		sum(OSAct),sum(MCAct),sum(MCDiff),sum(LBIndAct),sum(ToolAct),sum(RepairAct),sum(ElecAct),sum(WIPOB),sum(WODiff),sum(TotStd),
		sum(TotAct),sum(TotDif),sum(AllocAmt),sum(WIPAmt),
		sum(LBDiff),
		sum(RMDiff), -- Material Diff
		sum(Material_Act)
		from #COSTING_SUM group by BrchID,ifnull(PrjCode,''),GrpID,GrpName,FGCode,FGName;
		
	
		--
		--
		update x 
		set FGName = y."ItemName", 
		GrpName = z."ItmsGrpNam",
		GrpID  =z."ItmsGrpCod"
		from #COSTING_TOTAL x, "OITM" y, "OITB" z where x.FGCode=y."ItemCode" and y."ItmsGrpCod"=z."ItmsGrpCod";
		--
		update a
		set  RMStd =   (select   round(sum ("SdtAmount"),2) from :pro09 b
								where  a.FGCODE  = b."FGCode"   and b."ItmsGrpCod" in (113,114,121,127,253))  
		from #COSTING_TOTAL a;
		
		
		--
		update a
		set  LBStd =    (select   round(sum ("SdtAmount"),2) from :pro09 b 
								where  a.FGCODE  = b."FGCode"  and b."ItmsGrpCod"  =  126)  
								
		from #COSTING_TOTAL a;
		-- Lam tron
		update a
		set  
			 MCStd  = round(MCStd,2),
			 MachineHourIssue  = round (MachineHourIssue,2),
			 LaborHourStd  = round (LaborHourStd,2)
		from  #COSTING_TOTAL a;
	
	
	/* LAM GON LAI
		--- Total General Cost
		update #COSTING_TOTAL
		set Debit627 =  :totalDebit627, Credit627 =  :totalCredit627 ,
			Debit6279 = :totalDebit6279, Credit6279 = :totalCredit6279,
			TotalGeneralCost=  ifnull((select sum(Amount_Debit_Credit) from :tblEstimateCost where Account like '627%'),0)
							- ifnull((select  sum(Amount_Debit_Credit) from :tblEstimateCost where Account = '6279'),0)
		   					- ifnull((select   sum(Amount_Debit_Credit) from :tblEstimateCost where Account = '627402'),0) ;
							
		-- update Allocate to Inventory
update #COSTING_TOTAL set AllocToItem = ifnull(AllocAmt,0) - ifnull(AllocToCOGS,0);
*/




--=================  FGCmpltAmount
---
Update a
set FGCmpltAmount  =  (select  round(sum(ifnull("AmountFG",0)),2)  from  :pro09 b   where   a.FGCode  = b."FGCode"  )
from  #COSTING_TOTAL a;

--- Xu ly chenh lech FGCmpltAmount
select round(sum("AmountFG"),2) into TotalFGCmpltAmount from  :pro09;
		update a
		set a.FGCmpltAmount  = a.FGCmpltAmount +  (:TotalFGCmpltAmount - (select sum(b.FGCmpltAmount) from  #COSTING_TOTAL b) )
		from #COSTING_TOTAL a
		where a.FGCmpltAmount  = (select max(c.FGCmpltAmount) from #COSTING_TOTAL c);
--=================  FGCmpltQty
Update a
set FGCmpltQty  =  (select  round(sum(ifnull("CmpltQty",0)),2)  from  :pro09 b  where   a.FGCode  = b."FGCode" )
from #COSTING_TOTAL a;
--
delete from #COSTING_TOTAL where  ifnull(FGCmpltQty,0)  = 0;
---
select round(sum("CmpltQty"),2) into TotalFGCmpltQty from  :pro09 ;
--
update a
set a.FGCmpltQty  = a.FGCmpltQty +  (:TotalFGCmpltQty - (select sum(b.FGCmpltQty) from  #COSTING_TOTAL b) )
from #COSTING_TOTAL a
where a.FGCmpltQty  = (select max(c.FGCmpltQty) from #COSTING_TOTAL c);
--
insert into :SSCC_FGCode_OutSourcing(ItemCode, SSCC, BaseType, Qty)
select   "ItemCode",  "BatchNum" as "SSCC",   "BaseType",   "Quantity"
from IBT1  
where    "BsDocType"  <> 202  
		and "BaseType" in ( 59, 15) 
		 and "DocDate"  between fdate and tdate
		and "ItemCode"  in (select FGCode from  #COSTING_TOTAL);
		
		


if exists (select 1 from :SSCC_FGCode_OutSourcing ) then
	begin

				 -- LV0
				 update a
				 set a.SSCC_OngNoi  = b.SSCC_OngNoi,
				 	 a.Admission_Date_OngNoi = b.Admission_Date_OngNoi
				 from :SSCC_FGCode_OutSourcing a
				 inner join #TruyXuatSSCC2 b on   a.SSCC = b.SSCC_LV0
				 where ifnull(b.SSCC_LV6,'') = '' and ifnull(b.SSCC_LV5,'') = '' 
				 and ifnull(b.SSCC_LV4,'') = '' and ifnull(b.SSCC_LV3,'') = '' 
				 and  ifnull(b.SSCC_LV2,'') =''  and ifnull(b.SSCC_LV1,'') = ''and  ifnull(b.SSCC_LV0,'') <> '';
				 		 -- LV1
				 update a
				 set a.SSCC_OngNoi  = b.SSCC_OngNoi,
				 	 a.Admission_Date_OngNoi = b.Admission_Date_OngNoi
				 from :SSCC_FGCode_OutSourcing a
				 inner join #TruyXuatSSCC2 b on   a.SSCC = b.SSCC_LV1
				 where ifnull(b.SSCC_LV6,'') = '' and ifnull(b.SSCC_LV5,'') = ''
				  and ifnull(b.SSCC_LV4,'') = '' and ifnull(b.SSCC_LV3,'') = '' and  ifnull(b.SSCC_LV2,'') =''  and  ifnull(b.SSCC_LV1,'') <> '';
				  		 -- LV2
				 update a
				 set a.SSCC_OngNoi  = b.SSCC_OngNoi,
				 	 a.Admission_Date_OngNoi = b.Admission_Date_OngNoi
				 from :SSCC_FGCode_OutSourcing a
				 inner join #TruyXuatSSCC2 b on   a.SSCC = b.SSCC_LV2
				 where ifnull(b.SSCC_LV6,'') = '' and ifnull(b.SSCC_LV5,'') = '' 
				 and ifnull(b.SSCC_LV4,'') = '' and ifnull(b.SSCC_LV3,'') = ''   and  ifnull(b.SSCC_LV2,'') <> '';
				 		 -- LV3
				 update a
				 set a.SSCC_OngNoi  = b.SSCC_OngNoi,
				 	 a.Admission_Date_OngNoi = b.Admission_Date_OngNoi
				 from :SSCC_FGCode_OutSourcing a
				 inner join #TruyXuatSSCC2 b on   a.SSCC = b.SSCC_LV3
				 where ifnull(b.SSCC_LV6,'') = '' and ifnull(b.SSCC_LV5,'') = '' and ifnull(b.SSCC_LV4,'') = '' and  ifnull(b.SSCC_LV3,'') <> '';
				 		 -- LV4
				 update a
				 set a.SSCC_OngNoi  = b.SSCC_OngNoi,
				 	 a.Admission_Date_OngNoi = b.Admission_Date_OngNoi
				 from :SSCC_FGCode_OutSourcing a
				 inner join #TruyXuatSSCC2 b on   a.SSCC = b.SSCC_LV4
				 where ifnull(b.SSCC_LV6,'') = '' and ifnull(b.SSCC_LV5,'') = '' and  ifnull(b.SSCC_LV4,'') <> '';
				 		 -- LV5
				 update a
				 set a.SSCC_OngNoi  = b.SSCC_OngNoi,
				 	 a.Admission_Date_OngNoi = b.Admission_Date_OngNoi
				 from :SSCC_FGCode_OutSourcing a
				 inner join #TruyXuatSSCC2 b on   a.SSCC = b.SSCC_LV5
				 where ifnull(b.SSCC_LV6,'') = '' and  ifnull(b.SSCC_LV5,'') <> '';
				 		 -- LV6
				 		 update a
				 set a.SSCC_OngNoi  = b.SSCC_OngNoi,
				 	 a.Admission_Date_OngNoi = b.Admission_Date_OngNoi
				 from :SSCC_FGCode_OutSourcing a
				 inner join #TruyXuatSSCC2 b on   a.SSCC = b.SSCC_LV6
				 where ifnull(b.SSCC_LV6,'') <> '';
		
		
			
			end;
-- Update  SSCC Ông Nội, và ngày tạo  = chính nó nếu nó là kẻ FA
		update a 
		set a.SSCC_OngNoi  = a.SSCC,
			a.Admission_Date_OngNoi =  b."InDate"
		from  :SSCC_FGCode_OutSourcing a
		inner join OBTN b  on a.SSCC  = b."DistNumber"
		where ifnull(a.SSCC_OngNoi,'') = '';
--==== Xoa di SSCC Ông nội, tạo ra không trong kỳ  đang tính costing===--
		  
		 delete from :SSCC_FGCode_OutSourcing where Admission_Date_OngNoi<fdate or  Admission_Date_OngNoi>tdate;



end if;

List_SSCC_FGCode  =  select distinct a."ItemCode", a."BatchNum" as "SSCC"
       				from IBT1 a
       				inner join  OBTN b on a."BatchNum" = b."DistNumber"
					where 
							a."BsDocType"  = 202  and
							   a."BaseType" = 59 
							   and b."InDate" between fdate and tdate
							--and a."DocDate"  between fdate and tdate
							and a."ItemCode"  in (select FGCode from  #COSTING_TOTAL);
							
 SSCC_FGCode_OutSourcing_EndQty = select  * from :SSCC_FGCode_OutSourcing 
								 where  SSCC not in (select SSCC from  :List_SSCC_FGCode);
 
 --delete from :SSCC_FGCode_OutSourcing where SSCC in (select SSCC from  :List_SSCC_FGCode);

End_Qty_SSCC_FGCode   = select sum("QtyIn")  - sum("QtyOut") as "End_Qty",  "ItemCode"
						from (	select case when "Direction" = 0 then "Quantity" else 0 end "QtyIn",
							  	 case when "Direction" = 1 then "Quantity" else 0 end "QtyOut",
							  	 "ItemCode"
								from 
									(select  "Direction",a."ItemCode", sum("Quantity" ) as "Quantity"
									 
									from IBT1  a
									inner join OITM b on a."ItemCode" = b."ItemCode"
									inner join :List_SSCC_FGCode c on c."ItemCode" = a."ItemCode" and c."SSCC"  = a."BatchNum"
									where   b."ItmsGrpCod" in (120,121,127)
											and a."Direction" in (0,1)
									and a."DocDate"      <= tdate
									group by  a."Direction", a."ItemCode"
									) as Nguon1
							
						) as Nguon2
						  group by "ItemCode";
		update a
		set a.EBQty = ifnull(b."End_Qty",0)
		from #COSTING_TOTAL a
		inner join  :End_Qty_SSCC_FGCode b on a.FGCode  = b."ItemCode";
---
End_Qty_SSCC_FGCode_OutSourcing  = select sum("QtyIn")  - sum("QtyOut") as "End_Qty",  "ItemCode"
						from (	select case when  "Direction" = 0 then  "Quantity" else 0 end "QtyIn",
							  	 case when  "Direction" = 1 then  "Quantity" else 0 end "QtyOut","ItemCode"
							  	  
								from 
									(select  a."Direction",a."ItemCode", sum(a."Quantity" ) as "Quantity"
									 
									from IBT1 a
									inner join OITM b on a."ItemCode" = b."ItemCode"
									inner join :SSCC_FGCode_OutSourcing_EndQty  c on c.ItemCode = a."ItemCode" and c.SSCC  = a."BatchNum"
									--inner join :SSCC_FGCode_OutSourcing  c on c.ItemCode = a."ItemCode" and c.SSCC  = a."BatchNum"
								
									
									where   b."ItmsGrpCod" in (120,121,127)
											and a."Direction" in (0,1)
									and a."DocDate" between fdate and tdate

									group by  a."Direction", a."ItemCode"
									) as Nguon1
							
						) as Nguon2
						  group by "ItemCode";
						  
					  
Delivery_Qty_SSCC_FGCode_OutSourcing  = select ItemCode,sum(Qty) as "Delivery_Qty"
										from :SSCC_FGCode_OutSourcing
										where BaseType = 15
										group by ItemCode; 

-- Cộng thêm số lượng tồn mà SSCC Ông Nội có trong kỳ kế toán
		update a
		set a.EBQty = ifnull(a.EBQty,0) +  ifnull(b."End_Qty",0)
		from #COSTING_TOTAL a
		inner join  :End_Qty_SSCC_FGCode_OutSourcing b on a.FGCode  = b."ItemCode";
		
-- Update số lượng Delivery =  SL đã Delivery (theo SSCC đã sx trong kỳ)
		update a
		set a.SalesQty = ifnull(b."Delivery_Qty",0)
		from #COSTING_TOTAL a
		inner join  :Delivery_Qty_SSCC_FGCode_OutSourcing b on a.FGCode  = b.ItemCode;	
		--- 
---=======   RETURN 
		Delivery_Qty_Return  = select -sum("Quantity") as "Quantity", "ItemCode"   
								from IBT1  
								where "BaseType" = 16
								 and "BatchNum"   in (select SSCC from :SSCC_FGCode_OutSourcing)
								  and "DocDate" between fdate and tdate
									 group by  "ItemCode";


		--- Trừ di so luong return
		update a
		set a.SalesQty = a.SalesQty + ifnull(b."Quantity",0)
		from #COSTING_TOTAL a
		inner join :Delivery_Qty_Return b on a.FGCode  = b."ItemCode";
 
		---
		update #COSTING_TOTAL set SalesQty = 0 where SalesQty<0;



		-- Xác định Thành phẩm hay bán thành phẩm, để phân bổ chênh lệch của bán thành phẩm cho thành phẩm sử dụng nó
		update #COSTING_TOTAL set PrdType = N'FG';
		update #COSTING_TOTAL set PrdType='SG' where FGCode in (select x."Code" from "ITT1" x);
			
--- Material_Amt

Insert into  :Material_Amount_Temp(ItemCode,ItmsGrpCod, 
			IsHaveBom,
			ItemRework,ProductType)
Select a."ItemCode",a."ItmsGrpCod",
		case when ifnull(b."Code",'') ='' then 'N' else 'Y' end as "IsHaveBom",
		c."Code" as "ItemRwork_Bom",
		
		a."U_ProductType"
from OITM a
left join OITT b on a."ItemCode" = b."Code"
left join (select * from  ITT1 where "Quantity" <0) c on c."Father" = b."Code"
where a."ItemCode" in (select distinct FGCode from #COSTING_TOTAL);
--  update thong tin quantity Thanh pham va rework lien quan
update a
set  QtyFG =  ifnull(b.FGCmpltQty,0)
from :Material_Amount_Temp a
inner join  #COSTING_TOTAL b on b.FGCode = a.ItemCode and  a.ItmsGrpCod <> 121; -- 121 : Thành phẩm-rework
--- QtyFG cho Rework
update a
set  QtyFG =  (select max(b.QtyFG) from :Material_Amount_Temp b where a.ItemCode =  b.ItemRework )
from :Material_Amount_Temp a
where   a.ItmsGrpCod = 121; -- 121 : Thành phẩm-rework

--- update QtyRework cho thanh pham: vd: Tp: 10100004 --> rework: 10200002 --> update thong tin qty: 10200002
update a
set  QtyRework =  ifnull(b.FGCmpltQty,0)
from :Material_Amount_Temp a
inner join  #COSTING_TOTAL b on b.FGCode = a.ItemRework;


update a
set  QtyRework =   (select max(b.QtyRework) from :Material_Amount_Temp b where a.ItemCode  = b.ItemRework)
from :Material_Amount_Temp a
where ifnull(a.QtyRework,0)  = 0;
 
--Material_Amt_pro09: thanh pham
update a
set Material_Amt_pro09  =  (select sum(b."Amount") from :pro09 b where a.ItemCode = b."FGCode" and  b."ItmsGrpCod" in(113,114,121,127,253))
from  :Material_Amount_Temp a;


update a
set a.Material_Amt_pro09_FG  =   ifnull(a.QtyFG,0)/ (a.QtyFG+ ifnull(a.QtyRework,0))  *  (a.Material_Amt_pro09)
from  :Material_Amount_Temp a
where ifnull(a.QtyFG,0)  <>  0 and  ifnull(a.Material_Amt_pro09, 0)  <> 0;

--- update Material_Amt_pro09_FG cho rework
update a
set a.Material_Amt_pro09_FG  =   (select max (b.Material_Amt_pro09_FG) from :Material_Amount_Temp b where a.ItemCode  = b.ItemRework   and b. ItmsGrpCod  in(113,114,121,127,253))
from  :Material_Amount_Temp a
where ItmsGrpCod  = 121;
 
--Material_Amt_pro09: Rework
update a
set  a.Material_Amt_pro09_Rework  = ifnull(a.QtyRework,0)/  NULLIF((IFNULL(a.QtyFG,0) + ifnull(a.QtyRework,0)) 
							*  (select  max(b.Material_Amt_pro09)   from  :Material_Amount_Temp b 
										where a.ItemCode =  b.ITEMREWORK ), NULL)
from :Material_Amount_Temp a 
where   ifnull(a.QtyFG,0) <> 0 and  a.ItmsGrpCod  = 121;

--======= Material_Act
-- thanh pham
update a
		set a.Material_Act  =  round(ifnull(b.MATERIAL_AMT_PRO09_FG,0),2)
		from  #COSTING_TOTAL a
		inner join :Material_Amount_Temp b on a.FGCode  =  b.ItemCode 
		where b.ItmsGrpCod  <> 121;
-- Rework
update a
		set a.Material_Act  =  round(ifnull(b.MATERIAL_AMT_PRO09_REWORK,0),2)
		from  #COSTING_TOTAL a
		inner join :Material_Amount_Temp b on a.FGCode  =  b.ItemCode 
		where b.ItmsGrpCod  =  121;
		------ XỬ LÝ TRƯỜNG HỢP CHẠY COSTING SỐ THỦ CÔNG (Fix số) :Material_Act
	/*
		update a
		set Material_Act = case     WHEN FGCode = '11000039' THEN 1297958.45
							        WHEN FGCode = '11000040' THEN 679923.77
							        WHEN FGCode = '12000030' THEN 857767.86
							        WHEN FGCode = '12000031' THEN 161830.06
							        WHEN FGCode = '12000032' THEN 164832.71
							        WHEN FGCode = '12000033' THEN 141633.42
							        WHEN FGCode = '10400002' THEN 16979.57
							        WHEN FGCode = '11100040' THEN 3225.25
							        WHEN FGCode = '12100030' THEN 5870.58
							        WHEN FGCode = '12100031' THEN 2331.99 end
		from  #COSTING_TOTAL a;
	*/

		-- Xử lý chênh lệch Material_Act
		select sum(Material_Act)  into totalMaterial_Act  from  #COSTING_TOTAL;
		
		update a
		--set LBAct  =  round(:TotalLaborExpense * ifnull(Material_Act,1)/ :totalMaterial_Act,2) 
		set LBAct  =  round(:TotalLaborExpense *  Material_Act/ :totalMaterial_Act,2)
		from   #COSTING_TOTAL  a
		where ifnull(totalMaterial_Act,0) <> 0;
		-- Xử lý chênh lệchLBAct
		update a
		set a.LBAct  = a.LBAct +  (:TotalLaborExpense - (select sum(b.LBAct) from  #COSTING_TOTAL b) )
		from #COSTING_TOTAL a
		where a.LBAct  = (select max(c.LBAct) from #COSTING_TOTAL c);
		
		--================== MCAct
		update a
		set MCAct  =    round(ifnull(:TotalOverheadExpense,0) * ifnull(Material_Act,0)/  :totalMaterial_Act ,2)
		from   #COSTING_TOTAL  a
		where ifnull(totalMaterial_Act,0) <> 0;
		
		-- Xử lý chênh lệch MCAct
		update a
		set a.MCAct  = ifnull(a.MCAct,0) +   (ifnull(:TotalOverheadExpense,0) - (select sum(b.MCAct) from  #COSTING_TOTAL b) )						 
		from #COSTING_TOTAL a
		where a.MCAct  = (select max(c.MCAct) from #COSTING_TOTAL c);
		--
		
		--================ GeneralProCosts
/*
		update a
		--set  GeneralProCosts  =  round(TotalGeneralCost * ifnull(Material_Act,1)/ :totalMaterial_Act,2) 
		set  GeneralProCosts  =  round(TotalGeneralCost *  Material_Act / :totalMaterial_Act,2) 
		from   #COSTING_TOTAL  a
		where ifnull(totalMaterial_Act,0) <> 0;
		
 		-- Xử lý chênh lệch GeneralProCosts
		update a
		set a.GeneralProCosts  = a.GeneralProCosts +  (TotalGeneralCost - (select sum(b.GeneralProCosts) from  #COSTING_TOTAL b) )
		from #COSTING_TOTAL a
		where a.GeneralProCosts  = (select max(c.GeneralProCosts) from #COSTING_TOTAL c);
*/

----======================= ket thuc xu ly chenh lech 
--	 Material Diff
		update #COSTING_TOTAL
		set RMDiff  = ifnull(Material_Act,0) - ifnull(RMStd,0);
-- 	 Labor Diff
		update #COSTING_TOTAL
		set LBDiff  =  ifnull(LBAct,0) -  ifnull(LBStd,0);
--   Machine Diff
		update #COSTING_TOTAL
		set MCDiff  =  ifnull(MCAct,0) -  ifnull(MCStd,0);
				
		--update #COSTING_TOTAL set TotalCost = ifnull(LBAct,0) + ifnull(MCAct,0) + ifnull(GeneralProCosts,0) ;
		--update #COSTING_TOTAL set TotalCost = ifnull(Material_Act,0)+ ifnull(LBAct,0) + ifnull(MCAct,0) + ifnull(GeneralProCosts,0) ;	
		
		-- Unit Cost
		Update #COSTING_TOTAL
		set  UnitCost  =   ifnull(TotalCost,0)/(case when ifnull(FGCmpltQty,0)  = 0 then 1 else FGCmpltQty end);
		
		---  Total Diff
		update #COSTING_TOTAL set  TotalDiff =  ifnull(TotalCost,0)  -  ifnull(FGCmpltAmount,0),
									UnitCost  = round(UnitCost,2);

--======================================== Tach Wip OIL 
insert into :pro09_NVLPhu_WipOIL (FGCODE,FGCmpltQty, ITMSGRPCOD )
select  FGCode,FGCmpltQty, GrpID
from #COSTING_TOTAL;
--
update a
set NL_VLPhu_Amount   =    ( select  sum (b."Amount") from  :pro09 b  where a.FGCODE  = b."FGCode" and b."ItmsGrpCod" = 114) 				
from  :pro09_NVLPhu_WipOIL a;
--
update a
set Wip_OiL_Amount   =  ( select  sum (b."Amount") from  :pro09 b where a.FGCODE  = b."FGCode" and b."ItmsGrpCod" = 127)					
from  :pro09_NVLPhu_WipOIL a;


-- update thong tin item rework
update  a
set a.ItemRwork_Bom = b.ItemCode
from  :pro09_NVLPhu_WipOIL a
inner join :Material_Amount_Temp b on a.FGCODE  = b.ItemRework
where ifnull(b.IsHaveBom,'')  = 'Y';
-- Total FG qty (TP + Rwork: theo Bom)
update a
set a.Total_CmpltQty_FG_RW = a.FGCmpltQty +  (select sum(b.FGCmpltQty) 
												from  :pro09_NVLPhu_WipOIL  b where a.ItemRwork_Bom  = b.FGCode)
from :pro09_NVLPhu_WipOIL a 
where  ifnull(a.ItemRwork_Bom,'')  <> '';-- old: a.ITMSGRPCOD  = 121; -- WIP-Rework

-- Tổng Sub-Materials-PRO09 Thành phẩm + Rework
update a
set a.Total_Sub_Materials_Rework  =  (select sum(b.NL_VLPhu_Amount) 
												from  :pro09_NVLPhu_WipOIL  b where a.ItemRwork_Bom  = b.FGCode)
from :pro09_NVLPhu_WipOIL a 
where  ifnull(a.ItemRwork_Bom,'')  <> '';
--
update a
set a.Total_Wip_OiL_Rework  =  (select sum(b.Wip_OiL_Amount) 
												from  :pro09_NVLPhu_WipOIL  b where a.ItemRwork_Bom  = b.FGCode)
from :pro09_NVLPhu_WipOIL a 
where  ifnull(a.ItemRwork_Bom,'')  <> '';

-- Sub-Materials_6329
update a
set Sub_Materials_6329    =   round(case when ITMSGRPCOD  in (120,127) then 0 else ifnull(FGCmpltQty,0)/Total_CmpltQty_FG_RW* ifnull(Total_Sub_Materials_Rework,0) end,2)
from  :pro09_NVLPhu_WipOIL a
where ifnull(Total_CmpltQty_FG_RW,0)  <> 0 ;
-- WIP-Oil Coffee_6329
update a
set WIP_Oil_Coffee_6329  =   round(case when ITMSGRPCOD  in (120,127) then 0 else ifnull(FGCmpltQty,0)/Total_CmpltQty_FG_RW* ifnull(Total_Wip_OiL_Rework,0) end,2)
from  :pro09_NVLPhu_WipOIL a
where ifnull(Total_CmpltQty_FG_RW,0)  <> 0 ;

----------------  Labor (Act)_Draft
-- Tong so luong thanh pham
update  a
set a.TotalCmpltQty = (select sum(b.FGCmpltQty) from  :pro09_NVLPhu_WipOIL b)
from  :pro09_NVLPhu_WipOIL a;
-- Tong so luong nhom NL_Vat lieu gia cong (253)
update a
set a.TotalCmpltQty_NVLGiaCong = (select sum(b.FGCmpltQty) from  :pro09_NVLPhu_WipOIL b where b.ITMSGRPCOD =  253)
from :pro09_NVLPhu_WipOIL a;
---
update a 
set a.Labor_Act_Draft = case when a.ITMSGRPCOD =  253 then 0 else round(a.FGCmpltQty/(a.TotalCmpltQty - ifnull(TotalCmpltQty_NVLGiaCong,0)) * :TotalLaborExpense,2) end,
	a.Overhead_Act_Draft = case when a.ITMSGRPCOD =  253 then 0 else round(a.FGCmpltQty/(a.TotalCmpltQty - ifnull(TotalCmpltQty_NVLGiaCong,0)) * :TotalOverheadExpense,2) end,
	a.Processingfee_Act_Draft = case when a.ITMSGRPCOD <>  253 then 0 else round(a.FGCmpltQty/ ifnull(TotalCmpltQty_NVLGiaCong,0)  * :total_154503,2) end				
from   :pro09_NVLPhu_WipOIL a
where ifnull(TotalCmpltQty,0) <> 0;


--================================Xử lý chênh lệch 
--Labor_Act_Draft
/*
update a
set a.Labor_Act_Draft  = a.Labor_Act_Draft +  (:TotalLaborExpense - (select sum(b.Labor_Act_Draft) from  :pro09_NVLPhu_WipOIL b) )
from :pro09_NVLPhu_WipOIL a
where a.Labor_Act_Draft  = (select max(c.Labor_Act_Draft) from :pro09_NVLPhu_WipOIL c);
*/		
	-- 
	 
---=======================
-- ALLOCTOCOGS_6329_NL_VLPhu
update a
set ALLOCTOCOGS_6329_NL_VLPhu = 0, ALLOCTOCOGS_6329_Wip_OiL  =0
from :pro09_NVLPhu_WipOIL a
where a.ITMSGRPCOD  in (120,127);
--
update a
set ALLOCTOCOGS_6329_NL_VLPhu =  round(a.FGCmpltQty/ifnull(a.Total_CmpltQty_FG_RW,0) * ifnull(a.NL_VLPhu_Amount,0),2)				
from :pro09_NVLPhu_WipOIL a
where a.ITMSGRPCOD   = 121 and ifnull(a.Total_CmpltQty_FG_RW,0) <> 0; --WIP-Rework

--
update a
set ALLOCTOCOGS_6329_Wip_OiL =  round(a.FGCmpltQty/ifnull(a.Total_CmpltQty_FG_RW,0) * ifnull(a.Wip_OiL_Amount,0),2)				
from :pro09_NVLPhu_WipOIL a
where a.ITMSGRPCOD   = 121 and ifnull(a.Total_CmpltQty_FG_RW,0) <> 0; --WIP-Rework
--====================================================================
		Pro05_SumQty  = select ItemCode, sum(Qty) as Qty   
						 from :pro05 where   Father1_Date between fdate and tdate
						 group by ItemCode;
		
		update a
		set CONSUMED_QTY = abs(b.Qty)
		from #COSTING_TOTAL a
		inner join :Pro05_SumQty b on a.FGCode = b.ItemCode;
---------------- PHAN BO


	    -- 
		GI_QTY_TransType =	select sum(b."Quantity") as "Quantity",b."ItemCode", "U_TransType" as "TransType"
							from OIGE a
							inner join  IGE1 b on a."DocEntry" = b."DocEntry"
							where  a."U_TransType"  in ('256','257') 
									 and a."DocDate" between fdate and tdate
							group by b."ItemCode", "U_TransType";
		
		--- Sale_Sample_Qty
		update a
		set a.Sale_Sample_Qty  = b."Quantity"
		from #COSTING_TOTAL a 
		inner join :GI_QTY_TransType b on a.FGCode  = b."ItemCode"
		where b."TransType"= '256';
		
		--- Test_Sample_Qty
		update a
		set a.Test_Sample_Qty  = b."Quantity"
		from #COSTING_TOTAL a 
		inner join :GI_QTY_TransType b on a.FGCode  = b."ItemCode"
		where b."TransType"= '257';
-----============
					update a
					set  a.Material_Act =  a.Material_Act  - ifnull(b.Sub_Materials_6329,0)  - ifnull(WIP_Oil_Coffee_6329,0),
						 a.LBAct   = round(case when  a.GrpID  =  120 then  b.Labor_Act_Draft else 0 end,2),
						 a.MCAct =   round(case when  a.GrpID  =  120 then  b.Overhead_Act_Draft else 0 end + ifnull(b.Processingfee_Act_Draft,0) ,2)
					from  #COSTING_TOTAL a
					inner join  :pro09_NVLPhu_WipOIL b on a.FGCode = b.FGCode;
--

		


--===================================================FG Consumption
-- ItemThanh pham dc tao tu nvl la Thanh pham 120, vaf trong trong bom khong co nvl gia cong, 113 (NVL Chinh)

ThanhPham_NgoaiLe = SELECT "DocNum"
					FROM :pro09
					GROUP BY "DocNum"
					HAVING SUM(CASE WHEN "ItmsGrpCod" IN (113, 253) THEN 1 ELSE 0 END) = 0;
					 	 
					

insert into :tblFGConsumption (FGCode,Sub_FGCode,FGCmpltQty,FGCmpltAmount)
select "FGCode", "ItemCode","IssuedQty","Amount"	
from :pro09
where ifnull("IssuedQty",0)>0 and "ItmsGrpCod"   = 120 and "DocNum"  not in (select "DocNum" from :ThanhPham_NgoaiLe);

select sum(FGCmpltQty) into totalFGConsumption default 0 from  :tblFGConsumption;
--
insert into #COSTING_TOTAL (FGCode,Sub_FGCode,FGCmpltQty,FGCmpltAmount,ISFGConsumption)
select FGCode,Sub_FGCode,FGCmpltQty,FGCmpltAmount, 'Y'
from :tblFGConsumption a;
--
update a
set a.Orginal_FGCode = b."Father"
from  #COSTING_TOTAL a
inner join  ITT1 b on a.FGCode  =  b."Code"
where  b."Quantity"  <0;
--
update #COSTING_TOTAL
set Orginal_FGCode = FGCode
where  ifnull(Orginal_FGCode,'') ='';
--

insert into :tblFGConsumption (FGCode, FGCmpltQty,FGCmpltAmount)
select FGCode, FGCmpltQty,FGCmpltAmount
from  #COSTING_TOTAL a
where a.FGCode in (select FGCode from :tblFGConsumption ) and  ifnull(FGCmpltQty,0)>0;

--
update a
set  a.FGCmpltQty  = a.FGCmpltQty -  
						(select sum(b.FGCmpltQty) from :tblFGConsumption b where a.FGCode =  b.FGCode 
									and ifnull(b.Sub_FGCode,'') <>'')
from  :tblFGConsumption a
where ifnull(a.Sub_FGCode,'') ='' ;
--
update a
set  a.FGCmpltAmount  = a.FGCmpltAmount -  
						(select sum(b.FGCmpltAmount) from :tblFGConsumption b where a.FGCode =  b.FGCode 
											and ifnull(b.Sub_FGCode,'') <>'')
from  :tblFGConsumption a
where ifnull(a.Sub_FGCode,'') ='';
--
 
update a
set a.FGCmpltAmount  = b.FGCmpltAmount,
	a.FGCmpltQty = b.FGCmpltQty
from #COSTING_TOTAL a
inner join :tblFGConsumption b on a.FGCode  =  b.FGCode and ifnull(b.Sub_FGCode,'') =''
where ifnull(GrpName,'')  <> '';
 
--
update  #COSTING_TOTAL set FGCmpltAmount = round(FGCmpltAmount,2);

--Raw materials:Nguyên liệu, vật liệu chính, Item group 113 ()

insert into :pro09Sum (FGCode,ItemCode,ItmsGrpCod,Amount)
select "FGCode","ItemCode","ItmsGrpCod",sum("Amount")
from :pro09 
group by  "FGCode","ItemCode","ItmsGrpCod";
 
--
update #COSTING_TOTAL a
set a.Labour_Pr09 = round((select sum(b.Amount) from :pro09Sum b where a.FGCode  = b.FGCode   and b.ItmsGrpCod  = 126),2)	 
where ifnull(a.Sub_FGCode,'') = '';
--
update #COSTING_TOTAL a
set a.Overhead_Pr09 = round((select sum(b.Amount) from :pro09Sum b where a.FGCode  = b.FGCode   and b.ItmsGrpCod  = 233)	,2) 
where ifnull(a.Sub_FGCode,'') = '';
--
update #COSTING_TOTAL a
set a.ProcessingFee_Pr09 = round((select sum(b.Amount) from :pro09Sum b where a.FGCode  = b.FGCode   and b.ItmsGrpCod  = 254),2)	 
where ifnull(a.Sub_FGCode,'') = '';
--
update #COSTING_TOTAL a
set a.FGConsumption_Pr09 = round((select sum(b.Amount) from :pro09Sum b where a.FGCode  = b.FGCode  and a.Sub_FGCode  = b.ItemCode 
																		and b.ItmsGrpCod  = 120),2)	 
where ifnull(a.Sub_FGCode,'') <> '';
--
update #COSTING_TOTAL a
set a.RawMaterials_Pr09 = round((select sum(b.Amount) from :pro09Sum b where a.FGCode  = b.FGCode   and b.ItmsGrpCod  = 113)	,2) 
where ifnull(a.Sub_FGCode,'') = '';
--
update #COSTING_TOTAL a
set a.WIP_GreenBean_Pr09 = round((select sum(b.Amount) from :pro09Sum b where a.FGCode  = b.FGCode   and b.ItmsGrpCod  = 253),2)	 
where ifnull(a.Sub_FGCode,'') = '';
--
update #COSTING_TOTAL a
set a.Sub_Materials_Pr09 = round((select sum(b.Amount) from :pro09Sum b where a.FGCode  = b.FGCode   and b.ItmsGrpCod  = 114),2)	 
where ifnull(a.Sub_FGCode,'') = '';

-- Nguyên liệu phụ gia
update #COSTING_TOTAL a
set a.ADDITIVE_MATERIALS_PR09 = round((select sum(b.Amount) from :pro09Sum b where a.FGCode  = b.FGCode   and b.ItmsGrpCod  = 256),2)	 
where ifnull(a.Sub_FGCode,'') = '';
--
update #COSTING_TOTAL a
set a.WIP_OilCoffee_Pr09 = round((select sum(b.Amount) from :pro09Sum b where a.FGCode  = b.FGCode   and b.ItmsGrpCod  = 127),2)	 
where ifnull(a.Sub_FGCode,'') = '';
--
update #COSTING_TOTAL a
set a.WIP_Rework_Pr09 = round((select sum(b.Amount) from :pro09Sum b where a.FGCode  = b.FGCode   and b.ItmsGrpCod  = 121),2)
where ifnull(a.Sub_FGCode,'') = '';
--
update #COSTING_TOTAL a
set StdUnitCost = round((ifnull(RawMaterials_Pr09,0)  + ifnull(Sub_Materials_Pr09,0)  + ifnull(ADDITIVE_MATERIALS_PR09,0)
							+ ifnull(WIP_GreenBean_Pr09,0)  +
				  				ifnull(WIP_Rework_Pr09,0)  + ifnull(FGConsumption_Pr09,0)  + ifnull(WIP_OilCoffee_Pr09,0)  +
				 				 ifnull(Labour_Pr09,0)  + ifnull(ProcessingFee_Pr09,0)  + ifnull(Overhead_Pr09,0) )/ nullif(FGCmpltQty,0),2);	
--
update a 
set  Sub_FGCode = b."Father"
from #COSTING_TOTAL a
left join ITT1 b on a.FGCode = b."Code" and  b."Quantity"  <0
where ifnull(Sub_FGCode,'') = '' ; 
-- or GrpID  in  (121,127) 
update #COSTING_TOTAL  set  Sub_FGCode = FGCode where ifnull(Sub_FGCode,'') = ''  ;
--
update a
set FGName = b."ItemName",
	a.GrpName  = c."ItmsGrpNam",
	a.GrpID =  c."ItmsGrpCod"
from  #COSTING_TOTAL a
inner join OITM b on a.Sub_FGCode = b."ItemCode"
inner join OITB c on c."ItmsGrpCod" = b."ItmsGrpCod"
where ifnull(FGName,'') = '';

--
update a
set a.RawMaterials_Actual  = ROUND(a.FGCmpltQty/
									NULLIF((select sum(b.FGCmpltQty) from #COSTING_TOTAL b where  a.Sub_FGCode = b.Sub_FGCode ),0)
									 *
									NULLIF((select sum(c.RawMaterials_Pr09) from #COSTING_TOTAL c where  a.Sub_FGCode = c.Sub_FGCode ),0)
							,2)
from #COSTING_TOTAL a;
--
update a
set a.Sub_Materials_Actual  = ROUND(a.FGCmpltQty/
									NULLIF((select sum(b.FGCmpltQty) from #COSTING_TOTAL b where  a.Orginal_FGCode = b.Orginal_FGCode ),0)
									 *
									NULLIF((select sum(c.Sub_Materials_Pr09) from #COSTING_TOTAL c where  a.Orginal_FGCode = c.Orginal_FGCode ),0)
							,2)
from #COSTING_TOTAL a;
-- 

update a
set ACTUAL_ADDITIVE_MATERIALS  = ROUND(a.FGCmpltQty/
									NULLIF((select sum(b.FGCmpltQty) from #COSTING_TOTAL b where  a.Sub_FGCode = b.Sub_FGCode ),0)
									*
									NULLIF((select sum(c.ADDITIVE_MATERIALS_PR09) from #COSTING_TOTAL c where  a.Sub_FGCode = c.Sub_FGCode ),0)
								,2)	
from #COSTING_TOTAL a;

--
update a
set a.WIP_GreenBean_Actual  = ROUND(a.FGCmpltQty/
									NULLIF((select sum(b.FGCmpltQty) from #COSTING_TOTAL b where  a.Sub_FGCode = b.Sub_FGCode ),0)
									 *
									NULLIF((select sum(c.WIP_GreenBean_Pr09) from #COSTING_TOTAL c where  a.Sub_FGCode = c.Sub_FGCode ),0)
							,2)
from #COSTING_TOTAL a;
--
update a
set a.WIP_Rework_Actual  = ROUND(a.FGCmpltQty/
									NULLIF((select sum(b.FGCmpltQty) from #COSTING_TOTAL b where  a.Sub_FGCode = b.Sub_FGCode ),0)
									 *
									NULLIF((select sum(c.WIP_Rework_Pr09) from #COSTING_TOTAL c where  a.Sub_FGCode = c.Sub_FGCode ),0)
							,2)
from #COSTING_TOTAL a;
--
update a
set a.FGConsumption_Actual  = ROUND(a.FGCmpltQty/
									NULLIF((select sum(b.FGCmpltQty) from #COSTING_TOTAL b where  a.Sub_FGCode = b.Sub_FGCode ),0)
									 *
									NULLIF((select sum(c.FGConsumption_Pr09) from #COSTING_TOTAL c where  a.Sub_FGCode = c.Sub_FGCode ),0)
							,2)
from #COSTING_TOTAL a;
--
update a
set a.WIP_OilCoffee_Actual  = ROUND(a.FGCmpltQty/
									NULLIF((select sum(b.FGCmpltQty) from #COSTING_TOTAL b where  a.Sub_FGCode = b.Sub_FGCode ),0)
									 *
									NULLIF((select sum(c.WIP_OilCoffee_Pr09) from #COSTING_TOTAL c where  a.Sub_FGCode = c.Sub_FGCode ),0)
							,2)
from #COSTING_TOTAL a;
--
select sum(FGCmpltQty) into totalRMAct from  #COSTING_TOTAL;
-- Actual Labour
update #COSTING_TOTAL a
set a.LBAct  =  round(a.FGCmpltQty/nullif((ifnull(:totalRMAct,0) - ifnull(:totalFGConsumption,0)),NULL) * :TotalLaborExpense,2)
where a.GrpID  in (121,127) or a.Sub_FGCode = a.FGCode;
-- 
update #COSTING_TOTAL a
set a.ProcessingFee_Actual  =  round(a.FGCmpltQty/nullif((ifnull(:totalRMAct,0) - ifnull(:totalFGConsumption,0)),NULL) * :total_154503,2)
where a.GrpID = 253 and a.Sub_FGCode = a.FGCode;
-- Actual Overhead
update #COSTING_TOTAL a
set a.MCAct  =  round(a.FGCmpltQty/NULLif((ifnull(:totalRMAct,0) -  ifnull(:totalFGConsumption,0)), NULL) * ifnull(:TotalOverheadExpense,0),2)
where a.GrpID  in (121,127) or a.Sub_FGCode = a.FGCode;

--======================= xu ly chenh lenh :  Actual Overhead
update a
set a.MCAct  =  a.MCAct  + ( :TotalOverheadExpense -  (select sum(b.MCAct) from  #COSTING_TOTAL b))
from #COSTING_TOTAL a  
where  ifnull(a.MCAct,0)  >0 and a.MCAct  = (select max(MCAct) from  #COSTING_TOTAL c);

--======================= xu ly chenh lenh :  Actual Labour
update a
set a.LBAct  =  a.LBAct  + ( :TotalLaborExpense -  (select sum(b.LBAct) from  #COSTING_TOTAL b))
from #COSTING_TOTAL a  
where  ifnull(a.LBAct,0)  >0 and a.LBAct  = (select max(LBAct) from  #COSTING_TOTAL c);
 
--======================= xu ly chenh lenh :  Actual Processing fee
update a
set a.ProcessingFee_Actual  =  a.ProcessingFee_Actual  + ( :total_154503 -  (select sum(b.ProcessingFee_Actual) from  #COSTING_TOTAL b))
from #COSTING_TOTAL a  
where  ifnull(a.ProcessingFee_Actual,0)  >0 and a.ProcessingFee_Actual  = (select max(ProcessingFee_Actual) from  #COSTING_TOTAL c);
 
-- Total manufacturing cost
update a
set Total_Mnf_Cost = round( case 
						  WHEN  GrpID = 120 and Sub_FGCode = FGCode  THEN
					 					
							 					 ifnull(RawMaterials_Actual,0)  +
												 ifnull(Sub_Materials_Actual,0)  +
												 ifnull(ACTUAL_ADDITIVE_MATERIALS,0)  +
												 ifnull(WIP_GreenBean_Actual,0)   +
												 ifnull(WIP_Rework_Actual,0)  +
												 ifnull(FGConsumption_Actual,0)   +
												 ifnull(WIP_OilCoffee_Actual,0)  +
												 ifnull(LBAct,0)  +
												 ifnull(ProcessingFee_Actual,0)  +
												 ifnull(MCAct,0) 
												
						WHEN  GrpID in( 121,127) THEN
										
												 ifnull(RawMaterials_Actual,0)  +
												 ifnull(WIP_GreenBean_Actual,0)  +
												 ifnull(WIP_Rework_Actual,0)  
										
						ELSE 
								ifnull(FGConsumption_Actual,0)
						END,2)
from   #COSTING_TOTAL a;
--
update #COSTING_TOTAL set UnitCost = ifnull(Total_Mnf_Cost,0) / NULLIF(FGCmpltQty, 0);
--
update #COSTING_TOTAL set  Variance =  round(ifnull(Total_Mnf_Cost,0) - ifnull(FGCmpltAmount,0),2);
--
update  a
set  PROCCESSING_CONSPT_VAR = round((select ifnull(sum(b.ALLOC_CONSUMPTION),0) from  TBL_COSTING_TOTAL_STEP1 b where a.FGCode  =  b.FGCode),2)
from #COSTING_TOTAL  a;
--
update a
set a.ALLOC_PROCCESSING_CONSPT_VAR  = ROUND(a.FGCmpltQty/
									NULLIF((select sum(b.FGCmpltQty) from #COSTING_TOTAL b where  a.Orginal_FGCode = b.Orginal_FGCode ),0)
									 *
									NULLIF((select sum(c.PROCCESSING_CONSPT_VAR) from #COSTING_TOTAL c where  a.Orginal_FGCode = c.Orginal_FGCode ),0)
							,2)
from #COSTING_TOTAL a;
--
update #COSTING_TOTAL set  FINAL_VARIANCE =  round(ifnull(Variance,0) + ifnull(ALLOC_PROCCESSING_CONSPT_VAR,0),2);
--
update #COSTING_TOTAL 
set  TOTAL_MNF_COST_INCLPRO =  round(ifnull(ALLOC_PROCCESSING_CONSPT_VAR,0) 
								+ ifnull(TOTAL_MNF_COST,0),2);
--
update #COSTING_TOTAL 
set  FINAL_UNIT_COST	 =  round(ifnull(TOTAL_MNF_COST_INCLPRO,0) 
								/ NULLIF(FGCmpltQty,0),2);
/* tam khoa upgrade: 23.05.2025
--======================================== xu ly truong hop Upgrade
Item_Upgrade_Outsourcing = SELECT  c."ItemCode" as "RWItemCode" 
							from  "@TBL_OUTS_DOCNUM" a
							inner join   OIGE b  on a."U_issueID"  = b."DocEntry"
							inner join IGE1 c  on b."DocEntry" = c."DocEntry"
							where ifnull(a."U_Type",'')  = 1 
							and a."U_docDate"  between  fdate and tdate; 

insert into :Upgrade (ItemCode,Stock_Qty,Stock_Qty_RW, SalesQty,CONSUMED_QTY ,
					   Sale_Sample_Qty, Test_Sample_Qty,
					   ALLOCTOSTOCK,ALLOCTOCOGS,
					   Allocate_Sale_Sample,Allocate_Test_Sample,TotalDiff_Value,
					 	IsRework  )
					    
select  FGCode,
		EBQty,-- end qty
		case when  ifnull("isRework",'') =  'Y' 
						then ifnull(EBQty,0) + ifnull(SalesQty,0) +ifnull(CONSUMED_QTY,0) 
								+ifnull(Sale_Sample_Qty,0) +ifnull(Test_Sample_Qty,0) else 0 end as "Stock_Qty_RW",
		SalesQty, -- Delivery Qty 
		CONSUMED_QTY, -- 
		Sale_Sample_Qty ,Test_Sample_Qty,
		ALLOCTOSTOCK, ALLOCTOCOGS,
		Allocate_Sale_Sample, Allocate_Test_Sample,
		TotalDiff, 
		b."isRework"

from #COSTING_TOTAL a
inner join  (select "RWItemCode" as "ItemCode", 'Y' as "isRework" from :Item_Upgrade_Outsourcing
						 	union
			select ItemCode as "ItemCode", 'N' as "isRework"  from :Material_Amount_Temp 
							where  IsHaveBom  = 'Y' and   ItemCode <>  ItemRework) b
on a.FGCode = b."ItemCode";
--
IF exists (select 1 from :Upgrade) then 
	update a
	set a.ItemRework  = b.ItemRework
	from  :Upgrade a
	inner join  :Material_Amount_Temp b on a.ItemCode  = b.ItemCode and b.IsHaveBom = 'Y' 
	     and  b.ItemCode <>  b.ItemRework and ifnull(b.ItemRework,'')<>'';
--
 
 	update a
 	set a.QTY_RW_Pro04  = (select abs(sum(QTY)) from :pro04 b  where OUTSOURCING_TYPE = 'Upgrade' and a.ItemCode  =  b.ItemCode)
 	from :Upgrade a
 	where ifnull(ISREWORK,'')  ='Y';
 	--
	update a
	set  Total_RW_Qty = ifnull(Stock_Qty_RW,0)  + ifnull(QTY_RW_Pro04,0) 
	from  :Upgrade a
	where ifnull(ISREWORK,'')  ='Y';
	-- update  RW_Stock_Amount cho item Con  
	update a
	set a.RW_Stock_Amount  = round(TOTALDIFF_VALUE/TOTAL_RW_QTY * STOCK_QTY_RW,2)
	from  :Upgrade a
	where ifnull(ISREWORK,'')  ='Y';
	
	-- update RW_Upgraded_Amount cho item Con  
	update a
	set a.RW_Upgraded_Amount  = round(b.TOTALDIFF_VALUE/b.TOTAL_RW_QTY * b.QTY_RW_PRO04,2)
	from  :Upgrade a
	inner join :Upgrade b on a.ITEMREWORK= b.ItemCode 
	where ifnull(a.ISREWORK,'')  ='N';
	--
	Delete from  :Upgrade where  ifnull(RW_STOCK_AMOUNT,0) =  0 and  ifnull(RW_Upgraded_Amount,0) =  0;
	--
	update a
	set a.ALLOCTOSTOCK  = case when b.ISREWORK  ='N' then ifnull(b.ALLOCTOSTOCK, 0)  +  ifnull(b.RW_Upgraded_Amount, 0)
								  else    ifnull(b.RW_Stock_Amount, 0)   end
	from  #COSTING_TOTAL a
	inner join :Upgrade  b on a.FGCode  = b.ItemCode;
	--

	
END IF;
---ket thuc tam khoa upgrade

update a
set a.Upgraded_Qty =  (
						ifnull(a.EBQty,0) + ifnull(a.SalesQty,0) + ifnull(a.CONSUMED_QTY,0)+ ifnull(a.Sale_Sample_Qty,0)+ ifnull(a.Test_Sample_Qty,0)
					   )
					  -
					  (
					  	select sum(b.FGCmpltQty) from #COSTING_TOTAL  b where a.FGCode  = b.FGCode 
					  )
from #COSTING_TOTAL  a;
*/
--- ALLOCTOSTOCK
Update a
set ALLOCTOSTOCK = case 
    when a.GrpID in (121, 127) then FINAL_VARIANCE
    else 
        round(EBQty / NULLIF(
		            (select sum(b.FGCmpltQty) 
		             from #COSTING_TOTAL b 
		             where a.Orginal_FGCode = b.FGCode), 0) * NULLIF(a.FINAL_VARIANCE, 0),2)
END
from #COSTING_TOTAL a;
--- ALLOCTOCOGS
Update a
set ALLOCTOCOGS = case 
    when a.GrpID in (121, 127) then 0
    else 
        round(SalesQty / NULLIF(
            (select sum(b.FGCmpltQty) 
             from #COSTING_TOTAL b 
             where a.Orginal_FGCode = b.FGCode), 
            0) * NULLIF(a.FINAL_VARIANCE, NULL),2)
END
from #COSTING_TOTAL a;

--
Update a
set ALLOCTOCOGS_6329 = case 
					    when a.GrpID in (121, 127) then ifnull(Sub_Materials_Actual,0) +  ifnull(WIP_OilCoffee_Actual,0) 
					    								+ ifnull(LBAct,0) + ifnull(MCAct,0) 
					    when Sub_FGCode  =  FGCode and GrpID =  120 then 0  else Sub_Materials_Actual 
        
END
from #COSTING_TOTAL a;

update #COSTING_TOTAL  set ALLOCTOSTOCK= 0 where  ifnull(EBQty,0) = 0;



-- Allocate_Sale_Sample
		update a
		set Allocate_Sale_Sample  =  Variance  *  Sale_Sample_Qty 
								/ (NULLIF(EBQty,NULL) + ifnull(SalesQty,0) + ifnull(Sale_Sample_Qty,0) 
							+ ifnull(Test_Sample_Qty,0) + ifnull(CONSUMED_QTY,0))
		from #COSTING_TOTAL a
		where (ifnull(EBQty,0)+ ifnull(SalesQty,0) 
										+ ifnull(Sale_Sample_Qty,0)
										 + ifnull(Test_Sample_Qty,0)
										  + ifnull(CONSUMED_QTY,0))  <> 0;
		
		--- Allocate_Test_Sample
		update a
		set Allocate_Test_Sample  =  Variance  *  Test_Sample_Qty 
									/ (NULLIF(EBQty,NULL) + ifnull(SalesQty,0) + ifnull(Sale_Sample_Qty,0) 
										+ ifnull(Test_Sample_Qty,0) + ifnull(CONSUMED_QTY,0))
		from #COSTING_TOTAL a
		where (ifnull(EBQty,0)+ ifnull(SalesQty,0) 
										+ ifnull(Sale_Sample_Qty,0)
										 + ifnull(Test_Sample_Qty,0)
										  + ifnull(CONSUMED_QTY,0))  <> 0;

----update #COSTING_TOTAL  set ALLOCTOCOGS= ifnull(TotalDiff,0) where GRPID  = 253 and  ifnull(EBQty,0) = 0;

--
----------------
 
		DELETE FROM TBL_COSTING_TOTAL WHERE PerCode=period and ifnull(BrchID,0)=branch;
		insert into TBL_COSTING_TOTAL (PerCode,BrchID,PrjCode,PrdType,GrpID,GrpName,ProductLine,FGCode,FGName,FGPlnQty,FGCmpltQty,
		PrdStatus,SGStd,SGAct,RMStd,RMAct,RMDiff,RMOther,LBStd,OHStd,LBAct,OHAct,OSAct,MCAct,MCDiff,LBIndAct,ToolAct,RepairAct,ElecAct,WIPOB,WODiff,
		TotStd,TotAct,TotDif,AllocAmt,WIPAmt,EBQty,
		AllocToItem,AllocToCOGS,AllocFromSGToItem,AllocFromSGToCOGS,Tot_Alloc_To_Item,Tot_Alloc_To_COGS, GeneralProCosts,
		Gendiff,
		TotalCost,
		TotalDiff,
		Material_Act,
		SalesQty,
		ALLOCTOSTOCK,
		FGCmpltAmount,
		LaborHourStd,
		LBDiff,
		MachineHourIssue,
		MCStd,
		Sale_Sample_Qty,
		Test_Sample_Qty,
		Allocate_Sale_Sample,
		Allocate_Test_Sample,
		ALLOCTOCOGS_6329,
		Overhead_Act,
		Overhead_Diff,
		---
		Orginal_FGCode,
		Sub_FGCode,
		
		Labour_Pr09,
		Overhead_Pr09,
		ProcessingFee_Pr09,
		FGConsumption_Pr09,
		RawMaterials_Pr09,
		WIP_GreenBean_Pr09,
		Sub_Materials_Pr09,
		ADDITIVE_MATERIALS_PR09,
		ACTUAL_ADDITIVE_MATERIALS,
		
		WIP_OilCoffee_Pr09,
		WIP_Rework_Pr09,
		StdUnitCost,
		
		RawMaterials_Actual,
		Sub_Materials_Actual,
		WIP_GreenBean_Actual,
		WIP_Rework_Actual,
		FGConsumption_Actual,
		WIP_OilCoffee_Actual,
		ProcessingFee_Actual,
		Total_Mnf_Cost,
		UnitCost,
		Variance,
		PROCCESSING_CONSPT_VAR,
		ALLOC_PROCCESSING_CONSPT_VAR,
		FINAL_VARIANCE,
		TOTAL_MNF_COST_INCLPRO,
		FINAL_UNIT_COST,
		
		CONSUMED_QTY
 
		)
 	
		select period,BrchID,PrjCode,PrdType,GrpID,GrpName,ProductLine,FGCode,FGName,FGPlnQty,FGCmpltQty,
		PrdStatus,SGStd,SGAct,RMStd,RMAct,RMDiff, RMOther,LBStd,OHStd,LBAct,OHAct,OSAct,MCAct,MCDiff,LBIndAct,ToolAct,RepairAct,ElecAct,WIPOB,WODiff,
		TotStd,TotAct,TotDif,AllocAmt,WIPAmt,EBQty,
		AllocToItem,AllocToCOGS,AllocFromSGToItem,AllocFromSGToCOGS,Tot_Alloc_To_Item,Tot_Alloc_To_COGS, GeneralProCosts,
		Gendiff,
		TotalCost, 
		TotalDiff,
		Material_Act,
		SalesQty,
		ALLOCTOSTOCK,
		FGCmpltAmount,
		LaborHourStd,
		LBDiff,
		MachineHourIssue,
		MCStd,
		Sale_Sample_Qty,
		Test_Sample_Qty,
		Allocate_Sale_Sample,
		Allocate_Test_Sample,
		
		ALLOCTOCOGS_6329,
		Overhead_Act,
		Overhead_Diff,
		---
		Orginal_FGCode,
		Sub_FGCode,
		Labour_Pr09,
		Overhead_Pr09,
		ProcessingFee_Pr09,
		FGConsumption_Pr09,
		RawMaterials_Pr09,
		WIP_GreenBean_Pr09,
		Sub_Materials_Pr09,
		ADDITIVE_MATERIALS_PR09,
		ACTUAL_ADDITIVE_MATERIALS,
		
		WIP_OilCoffee_Pr09,
		WIP_Rework_Pr09,
		StdUnitCost,
		
		RawMaterials_Actual,
		Sub_Materials_Actual,
		
		WIP_GreenBean_Actual,
		WIP_Rework_Actual,
		FGConsumption_Actual,
		WIP_OilCoffee_Actual,
		ProcessingFee_Actual,
		Total_Mnf_Cost,
		UnitCost,
		Variance,
		PROCCESSING_CONSPT_VAR,
		ALLOC_PROCCESSING_CONSPT_VAR,
		FINAL_VARIANCE,
		TOTAL_MNF_COST_INCLPRO,
		FINAL_UNIT_COST,
		CONSUMED_QTY
 
		from #COSTING_TOTAL;
		

 

		drop table #COSTING_SUM;
		drop table #COSTING_TOTAL;
		drop table #TBLTMP1;
		drop table #TruyXuatSSCC2;
end
