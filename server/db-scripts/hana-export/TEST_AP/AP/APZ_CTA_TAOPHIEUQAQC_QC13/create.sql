CREATE procedure APZ_CTA_TaoPhieuQAQC_QC13
as
begin
	
	Create local temporary table #temp
	(
		WOID	int, -- Số lệnh sản xuất
		WONO	varchar(50), -- Docnum lệnh sản xuất
		QTY decimal(19,6),
		ITEMCODE nvarchar(50),
		ITEMNAME nvarchar(100),
		PLANNO nvarchar(20),
		BATCH varchar(50),
		SOXE varchar(20),
		SSCC varchar(5000) 
	);
	execute immediate 'drop table #temp';
	execute immediate 'create local temporary table #temp  as  (select * from #TEMPUI where QTY >0);';
	
Select Nguon1.*, b."U_Ins_Type" as  "InspectionType",
				 'O'  as "QCTYPE"
				
From(	
	select  
			WOID   as "BSDOCENTRY",
			WONO,
			Sum(QTY) as "Qty",
			ITEMCODE,
			ITEMNAME,
			PLANNO,
			BATCH,
			STRING_AGG(SSCC,',') as SSCC

	from #temp a
	
	group by  WOID,WONO,ITEMCODE,ITEMNAME,PLANNO,BATCH) as Nguon1
	
	 left join (select "U_Code","U_Ins_Type" from "@QC_TEMPLATE_H")b 
 			on Nguon1.PLANNO = b."U_Code";
	
	
	drop table #temp;
	exec 'drop table #TEMPUI';
end;

