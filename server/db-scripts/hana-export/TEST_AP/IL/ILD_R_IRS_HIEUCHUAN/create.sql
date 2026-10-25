CREATE procedure ILD_R_IRS_HieuChuan
(
	in FromDate date,
	in ToDate date,
	in ItemCode nvarchar(1000),-- ILD_LayItemHieuChuan
	in Manufacturer nvarchar(1000), --Hang San Xuat ILD_LayHangSanXuat
	in Classification nvarchar(500),-- Độ quan trọng của thiết bị ILD_LayDoQuanTrongThietBi
	in ServiceProvider nvarchar(500)--ILD_LayServiceProvider
)
as
begin

create local temporary table #tbl
(
	ItemCode nvarchar(50),
	ItemName nvarchar(100),
	Area nvarchar(50),
	Manufacturer  nvarchar(100),
	Range  nvarchar(50),
	Classification  nvarchar(20),
	ProcessMedium  nvarchar(100),
	Strategy nvarchar(100),
	MPE nvarchar(100),
	NominalInput nvarchar(100),
	CalibrationTime nvarchar(10) ,
	
	StarDate date,
	IntervalDays int,
	EndDate date,
	NextDate date,
	
	ReferenceStandard nvarchar(50),
	ServiceProvider nvarchar(100),
	CalibrationEvaluation nvarchar(10),
--
	DocDate date,
	CreatedByCode nvarchar(10),
	CreatedByName nvarchar(50),
	ChangedByCode nvarchar(10),
	ChangedByName nvarchar(50),
	Remark nvarchar(150),
	CreateDate nvarchar(10),
	UpdateDate nvarchar(10),
	Creator nvarchar(10)
);

	Insert into #tbl(
		ItemCode,
		ItemName,
		Area,
		
		DocDate,
		StarDate,
		IntervalDays,
		EndDate,
		NextDate,
		
		Manufacturer,
		Range ,
		Classification,
		ProcessMedium,
		Strategy,
		MPE,

		NominalInput,
		CalibrationTime,
		ReferenceStandard,
		ServiceProvider,
		CalibrationEvaluation,
		
		CreatedByCode,
		CreatedByName,
		ChangedByCode,
		ChangedByName,
		Remark,
		CreateDate,
		UpdateDate,
		Creator)

	select  
		a."U_ItemCode",
		a."U_ItemName",
		
		a."U_Area",
		a."U_DocDate",
		a."U_StartDate",
		a."U_IntervalC",
		a."U_EndDate",
		a."U_NextDate",
		
		a."U_Manufacturer",
		a."U_Range",
		a."U_Classification",
		a."U_ProcessMedium",
		a."U_Strategy",
		a."U_MPE",
		
		b."U_NominalInput",
		a."U_CalibrationTime",
		a."U_ReferenceStandard",
		a."U_ServiceProvider",
		b."U_CalibrationEvaluation",
		--
		
		a."U_CreatedByCode",
		a."U_CreatedByName",
		a."U_ChangedByCode",
		a."U_ChangedByName",
		a."Remark",
		TO_VARCHAR(a."CreateDate", 'DD/MM/YYYY'),
		TO_VARCHAR(a."UpdateDate", 'DD/MM/YYYY'),
		a."Creator"
 
From  "@CALIBRATIONS_H" a
inner join "@CALIBRATIONS_L" b on a."DocEntry"  = b."DocEntry"
where a."U_StartDate" between :FromDate and :ToDate
	and
		(
				 ifnull(:ItemCode, '') = '' 
				 or cast (a."U_ItemCode" as nvarchar) in (select "DATA" from split(:ItemCode,','))
		)
	and
		(
				 ifnull(:Manufacturer, '') = '' 
				 or cast (a."U_Manufacturer" as nvarchar) in (select "DATA" from split(:Manufacturer,','))
		)
	and
		(
				 ifnull(:Classification, '') = '' 
				 or cast (a."U_Classification" as nvarchar) in (select "DATA" from split(:Classification,','))
		)
    and
		(
				 ifnull(:ServiceProvider, '') = '' 
				 or cast (a."U_ServiceProvider" as nvarchar) in (select "DATA" from split(:ServiceProvider,','))
		);
		
 

select * from #tbl order by DocDate, ItemCode;
drop table #tbl;
end;

