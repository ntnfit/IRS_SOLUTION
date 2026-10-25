CREATE procedure APZ_CTA_TaoPhieuQAQC_ThanhPham_Pro04
as
begin

	Create local temporary table #temp
	(
		WOID	int,
		ITEMCODE nvarchar(50),
		ITEMNAME nvarchar(100),
		QTY decimal(19,6),
		SSCC varchar(50),
		BATCH varchar(50) 
		
	);
	execute immediate 'drop table #temp';
	execute immediate 'create local temporary table #temp  as  (select * from #TEMPUI);';
	
	
Select Nguon1.*, b."U_Ins_Type" as  "InspectionType"
From(	
	select  WOID,
			ITEMCODE,
			ITEMNAME,
			Sum(Qty) as "Qty",
			STRING_AGG(SSCC,',' order by  SSCC) as "SSCC",
			BATCH,
			"U_QCTemplate" as "TemplateCode"

	from #temp a
	inner join OITM b on  b."ItemCode"  = a.ITEMCODE
	group by WOID,ITEMCODE,ITEMNAME,BATCH, "U_QCTemplate") as Nguon1
	
	 left join (select "U_Code","U_Ins_Type" from "@QC_TEMPLATE_H")b 
 			on Nguon1."TemplateCode"= b."U_Code";
	
	
	drop table #temp;
	exec 'drop table #TEMPUI';
end;

