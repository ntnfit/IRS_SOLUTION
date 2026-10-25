CREATE procedure APZ_CTA_TaoPhieuQAQC_TP_NVL_QC08
as
begin
	declare BaseDocType varchar(3);
	
	Create local temporary table #temp
	(
		DOCENTRY	int,
		BASETYPE 	varchar(3),
		BSDOCENTRY	int, -- Số lệnh sản xuất
		DOCNUMBSENTRY varchar(50),
		QTY decimal(19,6),
		ITEMCODE nvarchar(50),
		ITEMNAME nvarchar(100),
		VENDOR nvarchar(50),
		VENDORNAME nvarchar(100),
		PLANNO nvarchar(20),
		BATCH varchar(50),
		SOXE varchar(20),
		SSCC varchar(5000),
		POSTINGDATE date
	);
	execute immediate 'drop table #temp';
	execute immediate 'create local temporary table #temp  as  (select * from #TEMPUI);';

--
update #temp 
set "BSDOCENTRY" = case when BASETYPE  = '20' then DOCENTRY else   BSDOCENTRY end ;
	
	
Select Nguon1.*, b."U_Ins_Type" as  "InspectionType",
				case when BASETYPE = '20' then 'I' else  'O' end  as "QCTYPE"
From(	
	select  --DOCENTRY,
			--case when BASETYPE  = '20' then DOCENTRY else   BSDOCENTRY end as "BSDOCENTRY",
			BSDOCENTRY,
			DOCNUMBSENTRY,--- DocNum lenh san xuat
			BASETYPE,
			Sum(QTY) as "Qty",
			ITEMCODE,
			ITEMNAME,
			VENDOR,
			VENDORNAME,
			PLANNO,
			BATCH,
			SOXE,
			STRING_AGG(SSCC,',') as SSCC,
			POSTINGDATE

	from #temp a
	
	group by --DOCENTRY,
			 BSDOCENTRY,
			 DOCNUMBSENTRY,
			BASETYPE,ITEMCODE,ITEMNAME,VENDOR,VENDORNAME,PLANNO,BATCH,SOXE,POSTINGDATE) as Nguon1
	
	 left join (select "U_Code","U_Ins_Type" from "@QC_TEMPLATE_H")b 
 			on Nguon1.PLANNO = b."U_Code";
	
	
	drop table #temp;
	exec 'drop table #TEMPUI';
end;

