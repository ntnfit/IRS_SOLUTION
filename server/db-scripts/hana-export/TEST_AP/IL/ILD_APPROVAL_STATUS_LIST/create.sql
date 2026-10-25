CREATE procedure ILD_Approval_Status_List
as begin
create local temporary table #tbl 
(
	DraftEntry 		int,
	DraftNum 		int,
	Status			nvarchar(50),
	ObjType			nVarChar(50),
	CREATOR			nVarChar(50),-- Nguoi tao
	CreateDate		date,
	Remarks 		nVarChar(250),
	--
	StepCode		int,
	APPROVER		nVarChar(50),
	Status_L		nvarchar(50),-- status line
	NgayDuyet		date,
	ThoiGianDuyet	varchar(20),
	NguoiDuyetDau	char(1), -- Người duyệt dau tien ở thời điểm hiện tại
	NguoiDuyetCuoi	char(1), -- Người duyệt cuối ở thời điểm hiện tại, vdu: có 3 bước duyệt: A: duyệt, B: Duyệt, C: chưa duyệt--> B: Duyệt cuối
	Pre_Approval_Date date, -- Ngay duyet truoc do
	Val_date int,
	Remarks_Line varchar(1000),
	Template_Approval  nvarchar(50)
);
insert into #tbl(DraftEntry,Status,ObjType,CREATOR,CreateDate,Remarks,
					StepCode,APPROVER,Status_L,NgayDuyet,ThoiGianDuyet, NguoiDuyetCuoi,Remarks_Line, Template_Approval)

select a."DraftEntry",a."Status",a."ObjType", c."U_NAME" as "CREATOR",a."CreateDate",a."Remarks",
		--
		b."StepCode", d."U_NAME" as "APPROVER", b."Status",b."UpdateDate" as "NgayDuyet",  
		case when LENGTH( b."UpdateTime")  = 1 then  '00:0'||  b."UpdateTime"
							 when LENGTH( b."UpdateTime")  = 2 then  '00:'||  b."UpdateTime"				
							 when LENGTH( b."UpdateTime")  = 3 then  '0'||left( b."UpdateTime",1) || ':' || right( b."UpdateTime",2) 
		 					 when LENGTH( b."UpdateTime")  = 4 then   left( b."UpdateTime",2) || ':' || right( b."UpdateTime",2)  
	 			else  '' end  as "ThoiGianDuyet",
	 			'N',
	 	e."Comments" as "Remarks_Line",
	 	--
		tplate."Name" as "Template Approval"
from OWDD a
inner join WDD1 b on a."WddCode"  = b."WddCode"
inner join  OWTM tplate on a."WtmCode"  = tplate."WtmCode"
inner join OUSR c on a."OwnerID"  =  c."USERID"
left join OUSR d on d."USERID"  =  b."UserID"
inner join ODRF e on e."DocEntry"  =  a."DraftEntry" and  e."ObjType"  =  a."ObjType"
where  e."ObjType" in (22,1470000113) and e."WddStatus" = 'W'
order  by a."WddCode", b."StepCode";
---
NguoiDuyetCuoi_Temp =  select   *  
					from  #tbl a 
					where  ifnull(NGAYDUYET,'')  <>'' and StepCode = (select max(StepCode) from  #tbl b  
					where a.DraftEntry = b.DraftEntry and  ifnull(b.NGAYDUYET,'')  <>'');


---
update a
set NguoiDuyetCuoi  = (select 'Y' from :NguoiDuyetCuoi_Temp b
				where a.DraftEntry||a.OBJTYPE||a.StepCode = b.DraftEntry||b.OBJTYPE||b.StepCode)
from #tbl a;
--
update #tbl
set NguoiDuyetCuoi = 'N'
where  ifnull(NgayDuyet,'') = '';

---------
NguoiDuyetDau_Temp =  select   *  
					from  #tbl a 
					where  ifnull(NGAYDUYET,'')  <>'' and StepCode = (select Min(StepCode) from  #tbl b  
					where a.DraftEntry = b.DraftEntry and  ifnull(b.NGAYDUYET,'')  <>'');


---
update a
set NguoiDuyetDau  = (select 'Y' from :NguoiDuyetDau_Temp b
				where a.DraftEntry||a.OBJTYPE||a.StepCode = b.DraftEntry||b.OBJTYPE||b.StepCode)
from #tbl a;
-------------============== Update ngay duyet truoc do
--- Trường hợp có hơn 2 người duyệt: người duyệt đầu tiên <> Người duyệt cuối cùng hiện tại
update a
set Pre_Approval_Date =  NGAYDUYET
from #tbl a
where  ifnull(NguoiDuyetDau,'') ='Y' and (ifnull(NguoiDuyetCuoi,'') ='N' or  ifnull(NguoiDuyetCuoi,'') = '');
--- Trường hợp có 2 người duyệt: người duyệt đầu tiên cũng là người duyệt cuối cùng hiện tại
update a
set Pre_Approval_Date =  NGAYDUYET
from #tbl a
where ifnull(NguoiDuyetCuoi,'') ='Y' and ifnull(NguoiDuyetDau,'') ='Y';

--- Trường hợp có 2 người duyệt: người duyệt đầu tiên cũng là người duyệt cuối cùng hiện tại
update a
set Pre_Approval_Date =  NGAYDUYET
from #tbl a
where ifnull(NguoiDuyetCuoi,'') ='Y' and ifnull(NguoiDuyetDau,'') ='Y';

--- Lấy ngày duyệt trước đó
update a
set Pre_Approval_Date =  (select NGAYDUYET from #tbl b where a.DraftEntry = b.DraftEntry 
														and ifnull(b.NGAYDUYET,'') <>'' and b.StepCode = a.StepCode-1)
from #tbl a
where ifnull(a.NguoiDuyetDau,'')='';

---======== update valid date duyet
--1. Duyet dau  = duyet cuoi (hien tai)
update #tbl a
set Val_date  =   DAYS_BETWEEN(CreateDate,NgayDuyet)  
where ifnull(NguoiDuyetDau,'') = 'Y';

--1. Là duyệt cuối hiện tại nhưng còn những người chưa duyệt phía dưới
update #tbl a
set Val_date  =   DAYS_BETWEEN(Pre_Approval_Date,NgayDuyet)  
where ifnull(NguoiDuyetDau,'') = '' AND ifnull(NguoiDuyetCuoi,'') = 'Y';
--2. Duyet dau  <> duyet cuoi (hien tai)
update #tbl a
set Val_date  =   DAYS_BETWEEN(CreateDate,NgayDuyet)  
where (ifnull(NguoiDuyetDau,'') = 'N' or ifnull(NguoiDuyetDau,'') = '') 
		 and  (ifnull(NguoiDuyetCuoi,'') = 'N' OR ifnull(NguoiDuyetCuoi,'') = '')
			and ifnull(NgayDuyet,'') <> '';
--3. Khong phai duyet dau va Cung Khong Phai Duyet Cuoi
update #tbl a
set Val_date  =   DAYS_BETWEEN(NgayDuyet,Pre_Approval_Date)  
where (ifnull(NguoiDuyetDau,'') = 'N' or ifnull(NguoiDuyetDau,'') = '') 
and  (ifnull(NguoiDuyetCuoi,'') = 'N' OR ifnull(NguoiDuyetCuoi,'') = '') and ifnull(NgayDuyet,'') <> '';

--4.Chua co duyet
update #tbl a
set Val_date  =   DAYS_BETWEEN(Pre_Approval_Date,CURRENT_DATE)  
where ifnull(NgayDuyet,'') = '';

 
-- update 

update  a
set  DraftNum = b."DocNum"
from #tbl a
inner join  ODRF b   on a.ObjType = b."ObjType" and a.DraftEntry = b."DocEntry";
--
update a
set Status_L = case when Status_L = 'W' then  'Pending' 
					when Status_L = 'Y' then  'Approved' 
					when Status_L = 'N' then  'Not Approved' end	
from #tbl a;
---
update a
set ObjType = case when ObjType = '22' then  'Purchase Order' 
					when ObjType = '1470000113' then  'Purchase Request' end	
from #tbl a;



select 
	DraftEntry,
	DraftNum,
	--Status,
	ObjType,
	CREATOR,
	CreateDate,
	Remarks,
	--
	StepCode,
	APPROVER,
	Status_L as "STATUS",
	NgayDuyet,
	ThoiGianDuyet,
	NguoiDuyetDau,
	NguoiDuyetCuoi,
	Pre_Approval_Date,
	Val_date,
	Remarks_Line,
	Template_Approval
 --DraftEntry,DraftNum,OBJTYPE,CREATOR,CREATEDATE,REMARKS,STATUS_L as "STATUS",PRE_APPROVAL_DATE,VAL_DATE, Stepcode
from #tbl
order by CREATOR;

--
drop table #tbl;
end;
