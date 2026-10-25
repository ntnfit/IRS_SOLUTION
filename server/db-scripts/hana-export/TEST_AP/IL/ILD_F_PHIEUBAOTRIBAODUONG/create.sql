CREATE procedure ILD_F_PhieuBaoTriBaoDuong
(
	in DocID	int
	
)
as
begin
-- Task List
/*GroupReport: Lý do IRS không thể preview rpt với sub report nên phải dồn dữ liệu vào  1 store và ở rpt sẽ xử lý group
						1: Dữ liệu bảo trì - bảo dưỡng
						2: Dữ liệu Tasklist
*/
--------------------------------------- Kết thúc insert TaskList
 create local temporary table #tbl
 (
 		RptName		nvarchar(100) default 'Material Issue', 
 		TaskListHyperlinkText	nvarchar(150) default 'Xem và in task list',
 		AttachFiles nvarchar(150),
		CompName	nvarchar(300),
		CompAddr	nvarchar(300), 
		CompTel		nvarchar(30),
		CompFax		nvarchar(30),
		DocID		int,
		DocNo		nvarchar(100),
		DocDate		timestamp,
 
		-- Header
		Room nvarchar(50),
		WeekNo nvarchar(2), -- Tuần thứ mấy trong năm
		WorkCenter nvarchar(150),
		WorkOrder nvarchar(50),
		PlannerGroup nvarchar(100),
		PlanFreq nvarchar(100),
		Priority nvarchar(50),
		StartDate nvarchar(10),
		EndDate nvarchar(10),
		Activity nvarchar(100),
		Notification	nvarchar(100),
		NoOfPeople   nvarchar(2),
		
		FixAssetCode nvarchar(50),
		FixAssetName nvarchar(100),
		Floc nvarchar(150),
		TasklistNo nvarchar(150),
		Notes nvarchar(250),
		Remarks nvarchar(250),
		
		CreatedBy nvarchar(50),
		CreatedOn nvarchar(10),
		-- Detail
		ItemCode nvarchar(50),
		ItemName nvarchar(100),
		Quantity decimal(21,6),
		UOM nvarchar(20),
		Price decimal(21,6),
		Amount decimal(21,6),
		BinNo nvarchar(100),
		
		
		
		-- Format number
		AmountDec		smallint,
		QtyDec			smallint,
		PriceDec		smallint,
		RateDec			smallint,
		PercentDec		smallint,
		DecSep			nvarchar(1),
		ThousSep		nvarchar(1)
		----- 

 );
 ---- Lay du lieu  MAINTENANCE

DuLieuBaoTri = select 
		c."U_AttachFiles" as "AttachFiles",
 		a."U_RoomName" as "Room", -- Department: Phòng có thiết bị cần bảo trì
		'' as"WeekNo",-- Tuần thứ mấy trong năm
		'' as "WorkCenter",-- Chia theo Group: Boiler, Grasso, Water treatment, M&I. E&A
		'' as "WorkOrder",
		'' as "PlannerGroup",-- Hiên tại chưa cần
		a."U_PlanNo" as "PlanFreq",
		Cast( a."U_Interval" || ' (days)' as nvarchar(10)) as "Priority",--Bảo trì định kì: 7 ngày, 1 tuần, 1 tháng, 3 tháng,6 tháng hay 1 năm ....
		TO_VARCHAR("U_StartDate",'DD/MM/YYYY') as "StartDate",
		TO_VARCHAR("U_EndDate",'DD/MM/YYYY') as "EndDate", -- Dưa vào Priority sẽ xác định end date theo start date (End date = Start date + Priority)
		a."U_ActivityType" as "Activity", --Link voi UDT: PM01-101, PM01_102,...
		'' as "Notification", -- Hiên tại chưa cần
		a."U_NoOfPeople" as "NoOfPeople",-- Số người thực hiện công việc bảo trì
		
		a."U_ItemCodeF" as "FixAssetCode",
		a."U_ItemNameF" as "FixAssetName",
		a."U_Floc" as "Floc",
		a."U_TasklistNo" as "TasklistNo",
		a."U_Notes" as "Notes",
		a."Remark" as "Remarks",
		a."U_CreatedByName" as "CreatedBy",
		TO_VARCHAR(a."CreateDate",'DD/MM/YYYY') as "CreateOn",
		
-- Detail		
		
		b."U_ItemCode" as "ItemCode",
		b."U_ItemName" as "ItemName",
		b."U_Quantity" as "Quantity",
		b."U_UOM" as "UOM",
		b."U_Price" as "Price",
		b."U_Amount" as "Amount",
		'' as  "BinNo"
		
from "@MAINTENANCE_H" a
inner join "@MAINTENANCE_L" b on a."DocEntry" = b."DocEntry"
left join "@TASKLISTTEMPLATE" c on a."U_TasklistNo"  = c."Code"
where a."DocEntry" = :DocID;
---------------------
	
 
 insert into #tbl(
 					AttachFiles,
					Room,
					WeekNo,-- Tuần thứ mấy trong năm
					WorkCenter,
					WorkOrder,
					PlannerGroup,
					PlanFreq,
					Priority,
					StartDate,
					EndDate,
					Activity,
					Notification,
					NoOfPeople,
					
					FixAssetCode,
					FixAssetName,
					Floc,
					TasklistNo,
					Notes,
					Remarks,
					CreatedBy,
					CreatedOn,
					-- Detail
					ItemCode,
					ItemName,
					Quantity,
					UOM,
					Price,
					Amount,
					BinNo)
 select 
 		"AttachFiles",
 		"Room" as "Room",
		"WeekNo" as"WeekNo",-- Tuần thứ mấy trong năm
		"WorkCenter" as "WorkCenter",
		"WorkOrder" as "WorkOrder",
		'' as "PlannerGroup",
		"PlanFreq" as "PlanFreq",
		"Priority" as "Priority",
		"StartDate" as "StartDate",
		"EndDate" as "EndDate",
		"Activity" as "Activity",
		'' as "Notification",
		"NoOfPeople" as "NoOfPeople",
		
		a."FixAssetCode",
		a."FixAssetName",
		a."Floc",
		a."TasklistNo",
		a."Notes",
		a."Remarks",
		"CreatedBy" as "CreatedBy",
		a."CreateOn",
		
-- Detail		
		
		a."ItemCode",
		a."ItemName",
		a."Quantity",
		a."UOM",
		a."Price",
		a."Amount",
		'' as  "BinNo"
from :DuLieuBaoTri a;

 -- Update thong tin cong ty
	update x set CompName = y."CompnyName", CompAddr = SUBSTR_BEFORE(y."CompnyAddr",char(13)) from #tbl x, OADM y;
 
 --Update thong tin dinh dang number
 	update x set 
			AmountDec	= y."SumDec",
			QtyDec		= y."QtyDec",
			PriceDec	= y."PriceDec",
			RateDec		= y."RateDec",
			PercentDec	= y."PercentDec",
			DecSep		= y."DecSep",
			ThousSep	= y."ThousSep"
	from #tbl x inner join OADM y on 1=1;

select *  from #tbl; 
drop table #tbl;

end;

