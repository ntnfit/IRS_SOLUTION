CREATE procedure ILD_R_DSTaoPhieuHangLoat_DenHanBaoTri_Bkup
(
	/*
		Báo cáo này nhằm list ra danh sách bảo trì sắp đến hạn bảo trì - bảo dưỡng,
	Và dữ liệu gốc: TEMPLATE_AUTO_GENMA (template mẫu: tài sản + floc --> chi tiết item)
	*/
	in FromDate date,
	in ToDate date,
   	in UserCode nvarchar(250),
	in TaiSanCode nvarchar(250),-- store ILD_LayItemTaiSan
	in Floc nvarchar(250),-- store ILD_LayFlocCumChucNang
	in OrderType nvarchar(250), -- store ILD_LayOrderType
	in DocType nvarchar(20), -- Loại phiếu có 2 lựa chọn: 1.Bao tri bao duong. 2: Kiểm Định (bắt buộc chọn để gen loại phiếu)
	in GenOnlyFloc	char(1) -- N: Gen sẽ LẤY từ Bom và lấy cả item code,Floc(header) phiếu sẽ null
							-- Y: Gen sẽ KHÔNG lấy từ Bom, Floc (Header) có giá trị. Dữ liệu line = null
	

)
as
begin
	declare template_genma table
	(
		PlanNo 	varchar(50),
		TasklistNo nvarchar(50),
		TasklistName nvarchar(250),
		ItemCodeF	varchar(50), -- Ma Tai san
		ItemNameF   nvarchar(150), -- Ten Tai san	
		Floc	nvarchar(250),
		ActivityType nvarchar(50),
		CreatedByCode nvarchar(10),
		CreatedByName nvarchar(50),
		PWO nvarchar(50),
		StartDate date,
		EndDate date,
		NextDate date,
		Room nvarchar(50),
		WorkCenter nvarchar(50),
		Interval smallint,
		NoOfPeople smallint,
		RoomName nvarchar(100),
		RoomID	int,
		
		Doctype nvarchar(20),
		TotalNumberOfItems smallint, -- Tổng số items theo từng loại doctype chọn: bao trì, kiểm định.
		
		Template_Gen_Code varchar(150),-- MA plan code
		Template_Gen_Name nvarchar(250),
		-- Chi tiet line 
		ItemCode	varchar(50), 
		ItemName   nvarchar(150), 
		Quantity Decimal(19,6),
		Uom Nvarchar(20) ,
		-- Thông tin ngày hoàn thành phiếu và trạng thái approval 
		ApprovedStatus nvarchar(50),
		CompletionDate date
	);
	declare taisan_floc_dataophieu table 
	(
		DocEntry int,
		ItemCodeF nvarchar(50),
		Floc nvarchar(50),
		StartDate date,
		Interval	smallint,
		EndDate	date,
		NextDate date,
		Gen_Only_Floc char(1),
		ActivityType nvarchar(50),
		
		ApprovedStatus nvarchar(50),
		CompletionDate date,
		
		Template_Gen_Code varchar(150)
	
	);

--================================= #template_genma
 
				Insert into  :template_genma (TasklistNo,
											TasklistName,
											ItemCodeF,
											ItemNameF,
											Floc,
											ActivityType,-- OrderType
											CreatedByCode,
											CreatedByName,
											PWO,
											StartDate,
											EndDate,
											NextDate,
											Room,
											WorkCenter,
											Interval,
											NoOfPeople,
											RoomName,
											Doctype,
											-- Lấy chi tiết đến itemcode
											ItemCode,
											ItemName,
											
											Template_Gen_Code,-- MA plan code
											Template_Gen_Name 
											
											)
											
	Select 
					"U_TasklistNo",
					"U_TasklistName",
					"U_ItemCodeF",
					"U_ItemNameF",
					"U_Floc",
					"U_ActivityType",
					"U_CreatedByCode",
					"U_CreatedByName",
					"U_PWO",
					"StartDate",
					ADD_DAYS("StartDate",ifnull("Interval",0)) as  "EndDate",
					ADD_DAYS(ADD_DAYS("StartDate",ifnull("Interval",0)),ifnull("Interval",0))as  "NextDate",
					"U_Room",
					"U_WorkCenter",
					"Interval",
					"U_NoOfPeople",
					"U_RoomName",
					:DocType,
					-- Lấy chi tiết đến itemcode
					"U_ItemCode",
					"U_ItemName",
					
					"U_MAPlanCode",
					"U_MAPlanName"
					
					
					
			From 
			(
					select distinct "U_TasklistNo",
					b."Name" as "U_TasklistName",
					"U_ItemCodeF",
					"U_ItemNameF",
					"U_Floc",
					"U_ActivityType",
					"U_CreatedByCode",
					"U_CreatedByName",
					"U_PWO",
					case when :DocType = 'Maintainance' then "U_StartDateMA" else  "U_StartDateHRE" end as "StartDate",
					case when :DocType = 'Maintainance' then ifnull("U_IntervalMA",0)  else ifnull("U_IntervalHRE",0)  end 
					as 	"Interval",
					"U_Room",
					"U_NoOfPeople",
					"U_RoomName",
					"U_WorkCenter",
					--Lấy chi tiết đến itemcode
					"U_ItemCode",
					"U_ItemName",
					
					a."U_MAPlanCode",
					a."U_MAPlanName"
			
					from "@TEMP_AUTO_GEN"  a
					left join "@TASKLISTTEMPLATE" b on a."U_TasklistNo"  = b."Code"
					where ifnull("U_Active",'') = 'Y' and ifnull("U_ItemCode",'')   <> '') as Nguon1;
 
--================================= End :template_genma


-------------------------------------------
--update End Date va  NextDate
update a
set EndDate = ADD_DAYS(StartDate,ifnull(Interval,0)) ,
	NextDate = ADD_DAYS(ADD_DAYS(StartDate,ifnull(Interval,0)),ifnull(Interval,0))
from :template_genma a;
---------------------------------------------
/*
	Phiếu CHỈ CÓ HEADER, KHÔNG có ITEM CODE LINE
	(do bảo trì bảo dưỡng theo floc - không theo item con) --> line Không có, floc header <> ''
*/
--================================= #taisan_floc_dataophieu

insert into :taisan_floc_dataophieu (DocEntry,Gen_Only_Floc,ItemCodeF, Floc, StartDate,Interval,EndDate,NextDate, ActivityType,
											  ApprovedStatus,CompletionDate,
											  
											  Template_Gen_Code
											  	)
		select "DocEntry","U_Gen_Only_Floc","U_ItemCodeF","U_Floc", "U_StartDate" ,"U_Interval","U_EndDate","U_NextDate", "U_ActivityType",
			   "U_ApprovedStatus",  "U_CompletionDate",
			   
			   "U_Template_Gen_Code" 
		from "@MAINTENANCE_H" a
		where "U_StartDate" = (select max("U_StartDate") from "@MAINTENANCE_H" b
								 where a."U_ItemCodeF" = b."U_ItemCodeF"
								 		 and  b."U_Floc" = a."U_Floc" 
								 		 and  b."U_ActivityType" = a."U_ActivityType"
								 		 and  b."U_DocType" = :DocType
								 		 and  b."U_Gen_Only_Floc" =  :GenOnlyFloc
								 		 and  b."U_Template_Gen_Code"  = a."U_Template_Gen_Code"
								 		 and  b."Canceled"  = 'N'
								 		 --and   ifnull(b."U_CompletionDate",'')  <> ''
							   ) 
			
				and
				(
						 ifnull(:DocType, '') = '' 
						 or cast (a."U_DocType" as nvarchar) in (select "DATA" from split(:DocType,','))
				)
				and ifnull("U_Floc",'')  <> ''  
				 
				and  "U_Gen_Only_Floc" =  :GenOnlyFloc;
--================================= End :taisan_floc_dataophieu
/*
	Vì tài sản + floc trong table (taisan_floc_dataophieu) đã tạo phiếu rồi,
	nên logic sẽ update thông tin gần nhất vào table nguồn :template_genma,
	Còn nếu chưa tồn tại thì sẽ lấy dữ liệu gốc :template_genma mà insert phiếu.
*/
--- update interval:
/*
	Phiếu tạo lần đầu tiên: lấy interval từ :@TEMPLATE_AUTO_GENMA
		phiếu thứ 2 trở đi sẽ lấy interval trên phiếu (vì thực tế người dùng
			có nhu cầu sửa ngày bảo trì, bảo dưỡng tiếp theo nếu phiếu hiện tại đang làm chưa kịp)
*/

update a
set a.Interval = b.Interval,
	a.ApprovedStatus  = b.ApprovedStatus,
	a.CompletionDate  =b.CompletionDate
from  :template_genma a
inner join  :taisan_floc_dataophieu b on a.ItemCodeF = b.ItemCodeF
where  a.Floc = b.Floc 
and a.ActivityType  =  b.ActivityType
and b.Gen_Only_Floc  = :GenOnlyFloc;
---
/* old
Update a
set a.StartDate  = ifnull(b.NextDate,ADD_DAYS (b.EndDate,ifnull(a.Interval,0))),
	a.EndDate  =   ADD_DAYS( ifnull(b.NextDate,ADD_DAYS (b.EndDate,ifnull(a.Interval,0))),ifnull(a.Interval,0)),  -- StartDate+ Interval
	a.NextDate  =  ADD_DAYS (ADD_DAYS( ifnull(b.NextDate,ADD_DAYS (b.EndDate,ifnull(a.Interval,0))),ifnull(a.Interval,0))
							,ifnull(a.Interval,0))
from :template_genma a
inner join  :taisan_floc_dataophieu b on a.ItemCodeF = b.ItemCodeF
where  a.Floc = b.Floc 
  	and a.ActivityType  =  b.ActivityType
  	and b.Gen_Only_Floc  = :GenOnlyFloc
  	and a.Template_Gen_Code  =  b.Template_Gen_Code;
*/

Update a
set a.StartDate  = ADD_DAYS (b.CompletionDate,ifnull(a.Interval,0)),
	a.EndDate  =  ADD_DAYS (ADD_DAYS (b.CompletionDate,ifnull(a.Interval,0)),ifnull(a.Interval,0)) ,  -- StartDate+ Interval
	a.NextDate  =  ADD_DAYS (ADD_DAYS (ADD_DAYS (b.CompletionDate,ifnull(a.Interval,0)),ifnull(a.Interval,0)),ifnull(a.Interval,0))
from :template_genma a
inner join  :taisan_floc_dataophieu b on a.ItemCodeF = b.ItemCodeF
where  a.Floc = b.Floc 
  	and a.ActivityType  =  b.ActivityType
  	and b.Gen_Only_Floc  = :GenOnlyFloc
  	and a.Template_Gen_Code  =  b.Template_Gen_Code;
  --	and b.CompletionDate <> '';
--================================= TotalNumberOfItems
	-- Update TotalNumberOfItems  -- Tổng số items theo từng loại doctype chọn: bao trì, kiểm định.
IF (:GenOnlyFloc = 'N') Then
		update tbl
		set TotalNumberOfItems  = (select count(*) 
									from OITM  c
									where  c."ItemType"  <>  'F'  
									
									and ((c."U_IsNeedCheckM" = case  ifnull(:DocType,'')    when 'Maintainance' then 'Y' end)
										 or  (c."U_IsNeedCheckHRE" = case  ifnull(:DocType,'')    when 'HighRiskEquipment' then 'Y' end))
									
									and c."ItemCode" in (select a."U_ItemCode" 
														 from "@BOMAINTENANCE_L"  a  
														 inner join "@BOMAINTENANCE_H" b on a."DocEntry"  = b."DocEntry" 
														 where b."U_ItemCodeF" = tbl.ItemCodeF and a."U_Floc" =  tbl.Floc)  )
		from :template_genma tbl;
		
		---- 


END IF;
--================================= End TotalNumberOfItems
-- update room id  = department id

update a
set RoomID  = b."Code"
from  :template_genma a
inner join OUDP b on  a.Room  = b."Name";
---
 
		select  distinct 
				PlanNo,
				TasklistNo,
			    TasklistName,
				ItemCodeF as FixAssetCode,
				ItemNameF as FixAssetName,
				a.ItemCode,
				a.ItemName,
				bom."U_Quantity" as Quantity,
				bom."U_UOM" as Uom,
				Floc,
				TotalNumberOfItems,
				ActivityType,
				CreatedByCode,
				CreatedByName,
				PWO,
				StartDate,
				EndDate,
				NextDate,
				RoomID,
				Room,
				WorkCenter,
				Interval,
				NoOfPeople,
				RoomName,
				DocType,
				:GenOnlyFloc as GenOnlyFloc,
				-- Thông tin ngày hoàn thành phiếu và trạng thái approval
				ApprovedStatus,
				CompletionDate,
				
				Template_Gen_Code as "MA_Plan_Code",-- MA plan code
				Template_Gen_Name as "Ma_Plan_Name"
		FROM :template_genma a
		INNER join (select a."U_ItemCodeF",b.*
			from "@BOMAINTENANCE_H" a
			inner join  "@BOMAINTENANCE_L" b on a."DocEntry" = b."DocEntry") bom 
				   on a.ItemCodeF = bom."U_ItemCodeF" and  a.FLOC = bom."U_Floc"
				   
		LEFT join  OITM b on bom."U_ItemCode" =  b."ItemCode"  
		 	 and a.ItemCode  =  b."ItemCode"
		WHERE  a.TOTALNUMBEROFITEMS  >0 and   a.INTERVAL  >0 and
		((b."U_IsNeedCheckM" = case  ifnull(a.DOCTYPE,'')    when 'Maintainance' then 'Y' end)
			 or  (b."U_IsNeedCheckHRE" = case  ifnull(a.DOCTYPE,'')    when 'HighRiskEquipment' then 'Y' end))
	 
		and   cast(a.StartDate as date) between  FromDate and  ToDate
				and
				(
						 ifnull(:TaiSanCode, '') = '' 
						 or cast (a.ItemCodeF as nvarchar) in (select "DATA" from split(:TaiSanCode,','))
				)
				and
				(
						 ifnull(:Floc, '') = '' 
						 or cast (a.Floc as nvarchar) in (select "DATA" from split(:Floc,','))
				)
				and
				(
						 ifnull(:OrderType, '') = '' 
						 or cast (a.ActivityType as nvarchar) in (select "DATA" from split(:OrderType,','))
				)
			 --and  ifnull(a.CompletionDate,'')  <> ''
		order by ItemCodeF,NextDate;
 
end;
