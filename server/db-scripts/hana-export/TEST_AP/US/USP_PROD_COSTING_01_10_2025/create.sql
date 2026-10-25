CREATE procedure USP_PROD_COSTING_01_10_2025(
	in period 	nvarchar(20), 
	in PrdLine 	nvarchar(1000),
	in branch 	int
	
)
as
begin
										
	declare strNote nvarchar(100);
	/* Quy trinh xu ly:
		 1. Nếu Bảng đánh giá hàng tồn kho đã tồn tại (với kỳ tính lương) thì báo đã tính giá thành
		 2. Nếu UDO Costing tồn tại dữ liệu (với kỳ tính lương) thì sẽ ưu tiên lấy UDO
		 
		 3. 
			3.1: PrdLine  = 'Step1' -->  Chạy bảng giá thành 1  --> lưu bảng TBL_COSTING_STEP_LOG (Step = 1).Nếu Step = 1 tồn tại rồi thì update Created
								và chưa có PrdLine  = 1 thì load bảng giá thành step1)
			3.2: PrdLine  = 'Step2' -->  Chạy bảng giá thành 2  --> lưu bảng TBL_COSTING_STEP_LOG (Step = 2), Nếu Step = 2 tồn tại rồi thì update Created) --> Lưu vào bảng TBL_COSTING_TOTAL
		---
			Tab phân bổ hạch toán kho: Nếu bảng TBL_COSTING_STEP_LOG CHƯA TỒN TẠI Step  =2 với kỳ tính lương thì sẽ trả ra đoạn text: Cần chạy bảng giá thành (Step1, Step 2)


	*/
	
	select TO_NVARCHAR("T_RefDate",'MM/yyyy') into strNote from "OFPR" where "Code"=period;
	if (select count(1) from "OMRV" where "U_PeriodCost"=period)>0 then
		select 'Đã tính giá thành tháng ' || strNote as "Note" from dummy;
	ElseIf (select count(1) from "@COSTING_H" where "U_Period"=period)>0 then -- lay du lieu UDO
		--================ lay du lieu tu UDO COSTING 
			--trường hợp này là do chạy costing ngoài giờ hỗ trợ, nên tạo udo này để iLD có thể chủ động copy paste dữ liệu vào 
		DELETE FROM TBL_COSTING_TOTAL_STEP1 WHERE PerCode=period ;
	 
		Insert into  "TBL_COSTING_TOTAL_STEP1"  (Type, PERCODE, GRPID, GRPNAME, FGCode, FGName,
											WIP_GB_CODE, FGCmpltQty, FGCmpltAmount, 
											
											RawMaterials_Pr09, ProcessingFee_Pr09, StdUnitCost, 
											RawMaterials_Actual, ProcessingFee_Actual, 
											
											Total_Mnf_Cost, UnitCost, Variance, 
											EBQty, CONSUMED_QTY, ALLOCTOSTOCK, Alloc_Consumption)

		Select "U_TYPE",:period,"U_GRPID","U_GRPNAME","U_FGCODE","U_FGNAME",
	   			"U_WIP_GB_CODE", "U_FGCMPLTQTY", "U_FGCMPLTAMOUNT", 
											
				"U_RAWMATERIALS_PR09", "U_PROCESSINGFEE_PR09", "U_STDUNITCOST", 
				"U_RAWMATERIALS_ACTUAL", "U_PROCESSINGFEE_ACTUAL", 
											
				"U_TOTAL_MNF_COST", "U_UNITCOST", "U_VARIANCE", 
				"U_EBQTY", "U_CONSUMED_QTY", "U_ALLOCTOSTOCK", "U_ALLOC_CONSUMPTION"
		From "@COSTING_H" a
		Inner join "@COSTING_L0" b on a."DocEntry"  = b."DocEntry" 
							and a."U_Period" = :period and a."Canceled" = 'N';
		--===============================================================
		select 1 as ID,
			Type   					"Type",
			GRPNAME  				"Group Name",
			
			FGCode   				"FGCode",
			WIP_GB_CODE   			"WIP-GB Code",
			
			FGName   				"FGName",
			FGCmpltQty   			"CmpltQty",
			
			FGCmpltAmount   		"AmountFG",
			RawMaterials_Pr09   	"Raw materials Pro09",
			ProcessingFee_Pr09  	"Processing fee Pro09", 
			
			StdUnitCost  		 	"Std unit cost",
			RawMaterials_Actual		"Actual Raw materials",
			ProcessingFee_Actual	"Actual Processing fee", 
		
			Total_Mnf_Cost			"Total manufacturing cost",
			UnitCost  				"Unit Cost",
			Variance				"Variance",
			
			EBQty					"End Qty",
			CONSUMED_QTY			"Consumed Qty_Processing",
			ALLOCTOSTOCK 			"Alloc Stock",
			Alloc_Consumption		"Alloc Consumption"
		
		from TBL_COSTING_TOTAL_STEP1 a
		where  PERCODE  = :period
		
		UNION ALL
		select 2 as ID,
			'' 							"Type",
			''  						"Group Name",
			
			''   						"FGCode",
			''   						"WIP-GB Code",
			
			'GRAND TOTAL'   			"FGName",
			sum(FGCmpltQty)  			"CmpltQty",
			
			sum(FGCmpltAmount)   		"AmountFG",
			sum(RawMaterials_Pr09)   	"Raw materials Pro09",
			sum(ProcessingFee_Pr09)  	"Processing fee Pro09", 
			
			sum(StdUnitCost) 		 	"Std unit cost",
			sum(RawMaterials_Actual)		"Actual Raw materials",
			sum(ProcessingFee_Actual)	"Actual Processing fee", 
		
			sum(Total_Mnf_Cost)			"Total manufacturing cost",
			sum(UnitCost)  				"Unit Cost",
			sum(Variance)				"Variance",
			
			sum(EBQty)					"End Qty",
			sum(CONSUMED_QTY)			"Consumed Qty_Processing",
			sum(ALLOCTOSTOCK) 			"Alloc Stock",
			sum(Alloc_Consumption)		"Alloc Consumption"
		from TBL_COSTING_TOTAL_STEP1 a
		where PERCODE  = :period
		order by "ID","FGCode","WIP-GB Code";
		--====================================
		
	elseif ((select count(1) from "OMRV" where "U_PeriodCost"=period)=0)then
--
	 -- 3.1: PrdLine  = 'Step1' -->  Chạy bảng giá thành 1  --> lưu bảng TBL_COSTING_STEP_LOG (Step = 1).Nếu Step = 1 tồn tại rồi thì update Created
								--và chưa có PrdLine  = 1 thì load bảng giá thành step1)
		
		-- Nếu chọn  PrdLine  ='Step1' thì sẽ bắt buộc chạy lại Step 2 --> Xóa log hết 
		Delete from  TBL_COSTING_STEP_LOG where  PERCODE  = :period;
		Insert into TBL_COSTING_STEP_LOG (PERCODE, STEP, CREATED) Select :period, 'Step1',CURRENT_TIMESTAMP from dummy;
		CALL USP_PROD_COSTING_STEP1 (:period);
		
		BEGIN
			select 1 as ID,
			Type   					"Type",
			GRPNAME  				"Group Name",
			
			FGCode   				"FGCode",
			WIP_GB_CODE   			"WIP-GB Code",
			
			FGName   				"FGName",
			FGCmpltQty   			"CmpltQty",
			
			FGCmpltAmount   		"AmountFG",
			RawMaterials_Pr09   	"Raw materials Pro09",
			ProcessingFee_Pr09  	"Processing fee Pro09", 
			
			StdUnitCost  		 	"Std unit cost",
			RawMaterials_Actual		"Actual Raw materials",
			ProcessingFee_Actual	"Actual Processing fee", 
		
			Total_Mnf_Cost			"Total manufacturing cost",
			UnitCost  				"Unit Cost",
			Variance				"Variance",
			
			EBQty					"End Qty",
			CONSUMED_QTY			"Consumed Qty_Processing",
			ALLOCTOSTOCK 			"Alloc Stock",
			Alloc_Consumption		"Alloc Consumption"
		
		from TBL_COSTING_TOTAL_STEP1 a
		
		UNION ALL
		select 2 as ID,
			'' 							"Type",
			''  						"Group Name",
			
			''   						"FGCode",
			''   						"WIP-GB Code",
			
			'GRAND TOTAL'   			"FGName",
			sum(FGCmpltQty)  			"CmpltQty",
			
			sum(FGCmpltAmount)   		"AmountFG",
			sum(RawMaterials_Pr09)   	"Raw materials Pro09",
			sum(ProcessingFee_Pr09)  	"Processing fee Pro09", 
			
			sum(StdUnitCost) 		 	"Std unit cost",
			sum(RawMaterials_Actual)		"Actual Raw materials",
			sum(ProcessingFee_Actual)	"Actual Processing fee", 
		
			sum(Total_Mnf_Cost)			"Total manufacturing cost",
			sum(UnitCost)  				"Unit Cost",
			sum(Variance)				"Variance",
			
			sum(EBQty)					"End Qty",
			sum(CONSUMED_QTY)			"Consumed Qty_Processing",
			sum(ALLOCTOSTOCK) 			"Alloc Stock",
			sum(Alloc_Consumption)		"Alloc Consumption"
		from TBL_COSTING_TOTAL_STEP1 a
		order by "ID","FGCode","WIP-GB Code";
		END;
 
	End IF;
end
