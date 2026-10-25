CREATE procedure APZ_CTA_TaoPhieuQAQC_NVL_QC12
as
begin
	declare BaseDocType varchar(3);
	
	Create local temporary table #temp
	(
		DOCENTRY	int,
		BSDOCENTRY	int, -- Số lệnh sản xuất
		QTY decimal(19,6),
		ITEMCODE nvarchar(50),
		ITEMNAME nvarchar(100),
		VENDOR nvarchar(50),
		VENDORNAME nvarchar(100),
		PLANNO nvarchar(20),
		BATCH varchar(50),
		SOXE varchar(20),
		SSCC varchar(5000),
		DOCNUM  varchar(50) -- DocNum lenh san xuat
	);
	execute immediate 'drop table #temp';
	execute immediate 'create local temporary table #temp  as  (select * from #TEMPUI);';
	
Select Nguon1.*, b."U_Ins_Type" as  "InspectionType",
				 'I'    as "QCTYPE"
From(	
	select  DOCENTRY,
			DOCNUM,
			DOCENTRY  as "BSDOCENTRY",
			Sum(QTY) as "Qty",
			ITEMCODE,
			ITEMNAME,
			VENDOR,
			VENDORNAME,
			PLANNO,
			BATCH,
			SOXE,
			STRING_AGG(SSCC,',') as SSCC

	from #temp a
	
	group by DOCENTRY,DOCNUM,BSDOCENTRY,ITEMCODE,ITEMNAME,VENDOR,VENDORNAME,PLANNO,BATCH,SOXE) as Nguon1
	
	 left join (select "U_Code","U_Ins_Type" from "@QC_TEMPLATE_H")b 
 			on Nguon1.PLANNO = b."U_Code";
	
	
	drop table #temp;
	exec 'drop table #TEMPUI';
end;

