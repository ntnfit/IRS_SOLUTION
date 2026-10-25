

CREATE PROCEDURE APZ_D_SettingReport_LoadData(
	in id int default 0,
	in fromDate timestamp default null,
	in toDate timestamp default null,
	in report nvarchar(250) default ''
)
AS
BEGIN

	if :id > 0
	then
		select * from APZ_D_OSRT where "DocEntry" = :id;
		return; 
	end if;
	
	select * from APZ_D_OSRT 
	where (ifnull(:report, '') = '' or "ReportId" = :report) 
	and (ifnull(:fromDate, '') = '' or "CreateDate" >= :fromDate) 
	and (ifnull(:toDate, '') = '' or "CreateDate" <= :toDate)
	order by "DocEntry" desc;

END



