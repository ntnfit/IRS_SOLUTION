CREATE procedure USP_PROD_COSTING_2(
	in period 	nvarchar(20), 
	in PrdLine 	nvarchar(1000),
	in branch 	int
	
)
as
begin
										
	declare strNote nvarchar(100);
		select TO_NVARCHAR("T_RefDate",'MM/yyyy') into strNote from "OFPR" where "Code"=period;
		
	IF (select count(1) from "OMRV" where "U_PeriodCost"=period)>0 then
		select 'Đã tính giá thành tháng ' || strNote as "Note" from dummy;
		return;
	ElseIf (select count(1) from "@COSTING_H" a inner join "@COSTING_L1" b on a."DocEntry" = b."DocEntry"
							   where a."U_Period"= :period)>0 then -- lay du lieu UDO

		 Begin
		 				--trường hợp này là do chạy costing ngoài giờ hỗ trợ, nên tạo udo này để iLD có thể chủ động copy paste dữ liệu vào 
				DELETE FROM TBL_COSTING_TOTAL WHERE PerCode= :period and ifnull(BrchID,0)=branch;
			 
				insert into TBL_COSTING_TOTAL (PerCode,BrchID,Type, GrpName, 
				
												Orginal_FGCode,Sub_FGCode,
												
												FGCode, FGName, FGCmpltQty, FGCmpltAmount, 
												
												RawMaterials_Pr09,Sub_Materials_Pr09,Additive_materials_PR09,
												 WIP_GreenBean_Pr09, WIP_Rework_Pr09,
												FGConsumption_Pr09,WIP_OilCoffee_Pr09,Labour_Pr09,ProcessingFee_Pr09,
												Overhead_Pr09,StdUnitCost,RawMaterials_Actual,Sub_Materials_Actual,Actual_Additive_Materials,
		
												
												WIP_GreenBean_Actual,WIP_Rework_Actual,FGConsumption_Actual,WIP_OilCoffee_Actual,
												LBAct,ProcessingFee_Actual,MCAct,Total_Mnf_Cost,
												UnitCost,Variance,
												Proccessing_Conspt_Var, Alloc_Proccessing_Conspt_Var, Final_Variance,
												Total_MNF_Cost_InclPro,Final_Unit_Cost,
												
												EBQty,SalesQty,
												CONSUMED_QTY,Sale_Sample_Qty,Test_Sample_Qty,ALLOCTOSTOCK,
												ALLOCTOCOGS,ALLOCTOCOGS_6329,Allocate_Sale_Sample,Allocate_Test_Sample,LBRatio,OHRatio)				
				Select :period,:branch,"U_Type", "U_GrpName",
				
								"U_Orginal_FGCode","U_Sub_FGCode",
						 		"U_FGCode", "U_FGName", "U_FGCmpltQty", "U_FGCmpltAmount", 
						 		
								"U_RawMaterials_Pr09","U_Sub_Materials_Pr09","U_Additive_materials_PR09",
								"U_WIP_GreenBean_Pr09","U_WIP_Rework_Pr09",
								"U_FGConsumption_Pr09","U_WIP_OilCoffee_Pr09","U_Labour_Pr09","U_ProcessingFee_Pr09",
								"U_Overhead_Pr09","U_StdUnitCost","U_RawMaterials_Actual","U_Sub_Materials_Actual","U_Actual_Additive_Materials",
								"U_WIP_GreenBean_Actual","U_WIP_Rework_Actual","U_FGConsumption_Actual","U_WIP_OilCoffee_Actual",
								"U_LBAct","U_ProcessingFee_Actual","U_MCAct","U_Total_Mnf_Cost",
								"U_UnitCost","U_Variance",
								
								"U_Proccessing_Conspt_Var", "U_Alloc_Proccessing_Conspt_Var", "U_Final_Variance",
								"U_Total_MNF_Cost_InclPro","U_Final_Unit_Cost",
								
								"U_EBQty","U_SalesQty",
								"U_CONSUMED_QTY","U_Sale_Sample_Qty","U_Test_Sample_Qty","U_ALLOCTOSTOCK",
								"U_ALLOCTOCOGS","U_ALLOCTOCOGS_6329","U_Allocate_Sale_Sample","U_Allocate_Test_Sample"
								,c."LabourRatio",c."OverheadRatio"
				from "@COSTING_H" a
				inner join "@COSTING_L1" b on a."DocEntry"  = b."DocEntry" and a."U_Period" = :period and a."Canceled" = 'N'
				left join (select "ItemCode" ,"LabourRatio","OverheadRatio" from GetProductTypeRatio(:period)) c on c."ItemCode" = b."U_FGCode";
				--
			   	update TBL_COSTING_TOTAL set PrdType = N'FG' where PERCODE = :period;
				update TBL_COSTING_TOTAL set PrdType='SG' where FGCode in (select x."Code" from "ITT1" x) and PERCODE = :period;
						
		 End;
		
	ElseIf ((select count(1) from "OMRV" where "U_PeriodCost"=period)=0  )then

		BEGIN
 		
	 		IF (select count(*) from TBL_COSTING_STEP_LOG where  PERCODE  = :period and STEP = 'Step1' )<=0 Then
	 			select 'Phải chạy bước 1 trước!' from dummy;
	 			return;
	 			
	 		ELSE  --=================Chạy bảng giá thành 2
	 				--Lưu vào bảng TBL_COSTING_TOTAL
					Delete from  TBL_COSTING_STEP_LOG where  PERCODE  = :period and  STEP = 'Step2' ;
					Insert into TBL_COSTING_STEP_LOG (PERCODE, STEP, CREATED) Select :period, 'Step2',CURRENT_TIMESTAMP from dummy;
	 				
	 				call USP_PROD_COSTING_STEP2 (:period,'Step2',0);
	 				--===============================================================================================================================	 				
	 		End If;
	 	END;

	End IF;
	select 1 as ID,
		case when PrdType='FG' then 'Finish Goods' else 'Semi Goods' end  "Type",
		GrpName													"Group Name",
		c."Code"												"Pro Type Code",
		c."Name"												"Pro Type Name",						
		Orginal_FGCode											"Orginal FG",
		Sub_FGCode												"Sub-FGCode",
		FGCode													"FGCode", 
		FGName													"FGName",
		
		FGCmpltQty												"CmpltQty", 
		FGCmpltAmount											"AmountFG", 
					
		RawMaterials_Pr09										"Raw materials Pro09", 
		Sub_Materials_Pr09										"Sub-materials Pro09", 
		ADDITIVE_MATERIALS_PR09									"Additive materials Pro09", 
		
		WIP_GreenBean_Pr09										"WIP-Green Bean Pro09", 
		WIP_Rework_Pr09											"WIP-Rework Pro09", 
		FGConsumption_Pr09										"FG consumption Pro09", 
		WIP_OilCoffee_Pr09										"WIP-Oil Coffee Pro09", 
		Labour_Pr09										 		"Labour Pro09", 
		Overhead_Pr09											"Overhead Pro09",
		StdUnitCost												"Std unit cost",	
		
		RawMaterials_Actual										"Actual Raw materials",
		Sub_Materials_Actual									"Sub-materials",
		ACTUAL_ADDITIVE_MATERIALS								"Actual Additive materials",

		WIP_GreenBean_Actual									"WIP-Green Bean",
		WIP_Rework_Actual										"WIP-Rework",
		FGConsumption_Actual									"FG consumption",
		WIP_OilCoffee_Actual									"WIP-Oil Coffee",
		LBRATIO													"Labour Ratio",
		LBAct													"Actual Labour", 
		OHRATIO													"Overhead Ratio",
		MCAct													"Actual Overhead",
		Total_Mnf_Cost											"Total manufacturing cost",
		cast (UnitCost  as  decimal(28,2)) 						"Unit Cost",
		Variance												"Variance",
		
		PROCCESSING_CONSPT_VAR									"Proccessing Conspt Var.",
		ALLOC_PROCCESSING_CONSPT_VAR							"Alloc Proccessing Conspt Var.",
		FINAL_VARIANCE											"Final Variance",
		TOTAL_MNF_COST_INCLPRO									"Total manufacturing cost incl. processing",
		FINAL_UNIT_COST											"Final Unit cost",	
		
		EBQty													"End Qty",
		SalesQty												"Delivery Qty",
		cast (CONSUMED_QTY as  decimal(28,2))					"Consumed Qty",
		
		cast (Sale_Sample_Qty as  decimal(28,2))				"Sale Sample Qty",
		cast (Test_Sample_Qty as  decimal(28,2)) 				"Test Sample Qty",

		cast (ALLOCTOSTOCK as  decimal(28,2))	 				"Alloc Stock",
		cast (ALLOCTOCOGS as  decimal(28,2))					"Alloc COGS",
		cast (ALLOCTOCOGS_6329 as  decimal(28,2))				"Alloc COGS_6329",
		
		cast (Allocate_Sale_Sample as  decimal(28,2))			"Allocate Sale Sample",				
		cast (Allocate_Test_Sample as  decimal(28,2))			"Allocate Test Sample",
		case when GRPID=120 and SUB_FGCODE != FGCODE 
			then '' else ORGINAL_FGCODE end 					"Orginal FG-FG consumption Qty",
		case when GRPID=120 and SUB_FGCODE != FGCODE 
			then 0 else FGCmpltQty end							"CmpltQty-FG consumption Qty"
		
	from TBL_COSTING_TOTAL a
	left join "OPRC" b on b."PrcCode"=a.ProductLine		
	left join (select "ItemCode" ,"Code" ,"Name" 
				from GetProductTypeRatio(:period)) c on c."ItemCode" = a.FGCode				
	where  "PERCODE"  = :period
	UNION ALL
	select 2 as ID,
		''  "Type",
		''														"Group Name",
		''														"Pro Type Code",
		''														"Pro Type Name",							
		''														"Orginal FG",
		''														"Sub-FGCode",
		''														"FGCode", 
		'GRAND TOTAL'											"FGName",
		
		sum(FGCmpltQty)											"CmpltQty", 
		sum(FGCmpltAmount)										"AmountFG", 
		
		sum(RawMaterials_Pr09)									"Raw materials Pro09", 
		sum(Sub_Materials_Pr09)									"Sub-materials Pro09", 
		sum(ADDITIVE_MATERIALS_PR09)							"Additive materials Pro09", 
		
		
		sum(WIP_GreenBean_Pr09)									"WIP-Green Bean Pro09", 
		sum(WIP_Rework_Pr09)									"WIP-Rework Pro09", 
		sum(FGConsumption_Pr09)									"FG consumption Pro09", 
		sum(WIP_OilCoffee_Pr09)									"WIP-Oil Coffee Pro09", 
		sum(Labour_Pr09)										"Labour Pro09", 
		sum(Overhead_Pr09)										"Overhead Pro09", 
		
		sum(StdUnitCost)										"Std unit cost",
		
		sum(RawMaterials_Actual)								"Actual Raw materials",
		sum(Sub_Materials_Actual)								"Sub-materials",
		sum(ACTUAL_ADDITIVE_MATERIALS)							"Actual Additive materials",
		
		sum(WIP_GreenBean_Actual)								"WIP-Green Bean",
		sum(WIP_Rework_Actual)									"WIP-Rework",
		sum(FGConsumption_Actual)								"FG consumption",
		sum(WIP_OilCoffee_Actual)								"WIP-Oil Coffee",
		null													"Labour Ratio",
		sum(LBAct)												"Actual Labour", 
		null													"Overhead Ratio",
		sum(MCAct)												"Actual Overhead",
		sum(Total_Mnf_Cost)										"Total manufacturing cost",
		cast (sum(UnitCost)  as  decimal(28,2)) 				"Unit Cost",
		sum(Variance) 											"Variance",
		
		sum(PROCCESSING_CONSPT_VAR)								"Proccessing Conspt Var.",
		sum(ALLOC_PROCCESSING_CONSPT_VAR)						"Alloc Proccessing Conspt Var.",
		sum(FINAL_VARIANCE)										"Final Variance",
		sum(TOTAL_MNF_COST_INCLPRO)								"Total manufacturing cost incl. processing",
		sum(FINAL_UNIT_COST)									"Final Unit cost",					
		
		sum(EBQty)												"End Qty",
		sum(SalesQty)											"Delivery Qty",
		cast (sum(CONSUMED_QTY)	 as  decimal(28,2))				"Consumed Qty",

		
					
		cast (sum(Sale_Sample_Qty) as  decimal(28,2))			"Sale Sample Qty",
		cast (sum(Test_Sample_Qty) as  decimal(28,2))			"Test Sample Qty",


		cast (sum(ALLOCTOSTOCK)	as  decimal(28,2))				"Alloc Stock",
		cast (sum(ALLOCTOCOGS) as  decimal(28,2))		 		"Alloc COGS",
		cast (sum(ALLOCTOCOGS_6329) as  decimal(28,2))			"Alloc COGS_6329",
		cast (sum(Allocate_Sale_Sample)	as  decimal(28,2))  	"Allocate Sale Sample",
		cast (sum(Allocate_Test_Sample)	as  decimal(28,2))		"Allocate Test Sample",
		''														"Orginal FG-FG consumption Qty",
		0														"CmpltQty-FG consumption Qty"					

	from TBL_COSTING_TOTAL a
	where "PERCODE"  = :period
	order by ID,"FGCode","Sub-FGCode";		
end
