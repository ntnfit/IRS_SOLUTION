CREATE procedure APZ_CTA_TaoPhieuQAQC_ThanhPham

as
begin

	Create local temporary table #temp
	(
		PROID	int,
		PRODUCTCODE nvarchar(50),
		PRODUCTNAME nvarchar(100),
		QTY decimal(19,6),
		SSCC nvarchar(50),
		BATCH nvarchar(50) 
		
	);
	execute immediate 'drop table #temp';
	execute immediate 'create local temporary table #temp  as  (select * from #TEMPUI);';
	
	
Select Nguon1.*, b."U_Ins_Type" as  "InspectionType"
From(	
	select  ProID,
			ProductCode,
			ProductName,
			Sum(Qty) as "Qty",
			STRING_AGG(SSCC,',' order by  SSCC) as "SSCC",
			Batch,
			"U_QCTemplate" as "TemplateCode"

	from #temp a
	inner join OITM b on  b."ItemCode"  = a.ProductCode
	group by PROID,ProductCode,ProductName,Batch, "U_QCTemplate") as Nguon1
	
	 left join (select "U_Code","U_Ins_Type" from "@QC_TEMPLATE_H")b 
 			on Nguon1."TemplateCode"= b."U_Code";
	--where Batch = :Batch;
	
	drop table #temp;
	exec 'drop table #TEMPUI';
end;

