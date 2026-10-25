CREATE procedure ILD_R_IRS_BaoTriBaoDuong_KiemDinh
(
	FromDate date,
	ToDate date,
	in PlanNo nvarchar(250),-- store ILD_LayPlanNo
	in TasklistNo nvarchar(250),-- store ILD_LayTaskList
	in TaiSanCode nvarchar(250),-- store ILD_LayItemTaiSan
	in ActivityType nvarchar(250), -- store LayActivityType
	in DocType nvarchar(50),
	in Completed	char(1) -- N: Chưa hoàn thành --> Chưa có ngày CompletionDate
							-- Y: Đã hoàn thành   --> Có ngày CompletionDate
)
as
begin
create local temporary table #tbl
(
		 DocEntry int,
		 PlanNo nvarchar(50),
	     TasklistNo nvarchar(50),
	     FixAssetCode nvarchar(50),
	     FixAssetName nvarchar(100),
	     Floc nvarchar(50),
	     OrderType nvarchar(50),
	     ActivityType nvarchar(50),
	     TasklistName nvarchar(50),
	     CreateDate nvarchar(50),
	     CreatedByName nvarchar(50),
	     UpdateDate nvarchar(50),
	     ChangedByName nvarchar(50),
	     PWO nvarchar(50),-- Previous Work Order
	     Notes nvarchar(50),
	     Remark nvarchar(50),
	     CompletedByName nvarchar(50),
	     CompletionDate nvarchar(50),
	     Creator nvarchar(50),
	     StartDate date,
		 Interval smallint,
		 EndDate date,
		 NextDate date
);
	insert into #tbl (	 DocEntry,
						 PlanNo,
					     TasklistNo,
					     FixAssetCode,
					     FixAssetName,
					     Floc,
					     ActivityType,
					     TasklistName,
					     CreateDate,
					     CreatedByName,
					     UpdateDate,
					     ChangedByName,
					     PWO,-- Previous Work Order
					     Notes,
					     Remark,
					     CompletedByName,
					     CompletionDate,
					     Creator,
					     
					     StartDate,
					     Interval,
					     EndDate,
					     NextDate)
					     
	select
		 a."DocEntry",
	     "U_PlanNo",
	     "U_TasklistNo",
	     "U_ItemCodeF",
	     "U_ItemNameF",
	     "U_Floc",
	     "U_ActivityType",
	     "U_TasklistName",
	     TO_VARCHAR("CreateDate",'DD/MM/YYYY') as "CreateDate",
	     "U_CreatedByName",
	     TO_VARCHAR("UpdateDate",'DD/MM/YYYY') as "UpdateDate",
	     "U_ChangedByName",
	     "U_PWO",-- Previous Work Order
	     "U_Notes",
	     "Remark",
	     "U_CompletedByName",
	     TO_VARCHAR("U_CompletionDate",'DD/MM/YYYY') as "U_CompletionDate",
	     "Creator",
	     a."U_StartDate",
		 a."U_Interval",
		 a."U_EndDate",
		 a."U_NextDate"
from "@MAINTENANCE_H" a
where a."U_StartDate" between :FromDate and :ToDate
	  and
		(				 
				 ifnull(:DocType, '') = '' or :DocType  = 'ALL' 
				 or  cast (a."U_DocType" as nvarchar) in (select "DATA" from split(:DocType,','))
				 
		)
	  and
		(
				 ifnull(:PlanNo, '') = '' 
				 or cast (a."U_PlanNo" as nvarchar) in (select "DATA" from split(:PlanNo,','))
		)
		and
		(
				 ifnull(:TasklistNo, '') = '' 
				 or cast (a."U_TasklistNo" as nvarchar) in (select "DATA" from split(:TasklistNo,','))
		)
			  and
		(
				 ifnull(:TaiSanCode, '') = '' 
				 or cast (a."U_ItemCodeF" as nvarchar) in (select "DATA" from split(:TaiSanCode,','))
		)
		and
		(
				 ifnull(:ActivityType, '') = '' 
				 or cast (a."U_ActivityType" as nvarchar) in (select "DATA" from split(:ActivityType,','))
		);


select * 
from #tbl
where  ifnull(CompletionDate,'N') =  (case  when :Completed = 'N' then 'N' else CompletionDate end) 
order by  CreateDate , FixAssetCode ;
drop table #tbl;
end;

