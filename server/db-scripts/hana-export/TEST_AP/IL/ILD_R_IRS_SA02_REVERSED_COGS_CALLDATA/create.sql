CREATE procedure ILD_R_IRS_SA02_Reversed_COGS_CallData
(
	in Year_Period int,
	in Period nclob
)
as
begin
	declare WhCode nvarchar(10) = 'FGIT';	
	call ILD_R_IRS_SA02_Reversed_COGS (Year_Period, Period,tab_result);
	-- Ghi nhan du lieu vao bang IRSSA02 de them vao bao cao WH01 (Hang gui di ban, kho FGIT). Theo ky xem bao cao Period.
	--1. Xoa du lieu dang co cua lan xem bao cao truoc nhung chua Confirm.
	DELETE FROM IRSSA02 WHERE Period = :Period and  Confirm_Number = 0 ;
	--2. Ghi nhan du lieu moi cho lan xem bao cao nay.
	insert into IRSSA02(Period,ItemCode,DO_ID,DO_NO,DO_SSCC,DO_SSCC_Qty,DO_Qty_Total,DO_Amount_Total,DO_SSCC_Cost,
		DO_SSCC_Amount,DO_LineNum,WhsCode,Return_ID,ORIBatch_Del,Total_Delivery_Qty,Total_Alloc_COGS,
		Adj_Reversed_COGS,Unit_Var_Cost)
	SELECT 
		:Period AS Period,
		ItemCode,
		DO_ID,
		DO_NO,
		DO_SSCC,
		DO_SSCC_Qty,
		DO_Qty_Total,
		DO_Amount_Total,
		DO_SSCC_Cost,
		DO_SSCC_Amount,
		DO_LineNum,
		:WhCode as WhsCode,
		Return_ID,
		ORIBatch_Del,
		Total_Delivery_Qty,
		Total_Alloc_COGS,
		Adj_Reversed_COGS,
		Unit_Var_Cost
	from $tab_result where U_Term = 'FOB';

	select ItemCode "Item No",
			ItemName "Description",
			
			System_date "System Date",
			Posting_date "Posting Date",
	
			DO_NO "DO no.",
			DO_ID "DO_ID",
			WhsCode "Whse",
			DO_SSCC_Qty "Quantity",
			
			DO_SSCC_Cost  "Unit Cost",
			DO_SSCC_Amount "Amount",
	
			DO_Qty_Total "Total Qty",
			DO_Amount_Total "Total Amount",
			--
			Inv_No "Inv no",
			Inv_TaxNo "Tax Inv",
			Inv_Date "Inv Date",
	
			DO_SSCC "SSCC",
			
			SSCC_Father1 "SSCC_Father1",
			Father1_Date "Father1_Date",
			
			--Return_NO "Return No",
			
			--
			GLAccount "G/L Account",
			Currency "Currency",
			UoM "UoM",
			--
			round(Total_Delivery_Qty,2) "Total Delivery Qty",
			round(Unit_Var_Cost,2) "Unit Variance Cost",
			round(Total_Alloc_COGS,2) "Total Alloc COGS",
			round(Adj_Reversed_COGS,2) "Adj_Reversed_COGS"
			
			
	from $tab_result;
end;
