Create Procedure ILD_R_DSTaiSanFlocDenHanBaoTri
(
	in TaiSanCode nvarchar(250),-- store ILD_LayItemTaiSan
	in Floc nvarchar(250),-- store ILD_LayFlocCumChucNang
	in PriorityDays	smallint, -- Tần suất (ngày) bảo trì - bảo dưỡng
 	in UserCode nvarchar(50) -- Hệ thống mặc định sẽ lấy usercode nhân viên login
	
)
as
begin
create local temporary table #template_Auto_GenMA
(
	TasklistNo nvarchar(50),
	TasklistName nvarchar(250),
	ItemCodeF nvarchar(50),
	ItemNameF nvarchar(150),
	Floc nvarchar(50),
	ActivityType nvarchar(50),
	CreatedByCode nvarchar(10),
	CreatedByName nvarchar(50),
	PWO nvarchar(50),
	StartDate date,
	Room nvarchar(50),
	WorkCenter nvarchar(50),
	Priority smallint,
	NoOfPeople smallint,
	RoomName nvarchar(100)
);

drop table #template_Auto_GenMA;
end;

