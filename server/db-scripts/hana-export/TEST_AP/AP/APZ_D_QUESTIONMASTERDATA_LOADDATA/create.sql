

CREATE PROCEDURE APZ_D_QuestionMasterData_LoadData(
	in id int default 0,
	in fromDate timestamp default null,
	in toDate timestamp default null,
	in status char(1) default '')
AS
BEGIN

	if :id > 0
	then
		select * from APZ_D_OCHM where "DocEntry" = :id;
		return; 
	end if;
	
	select * from APZ_D_OCHM 
	where 
	(ifnull(:status, '') = '' or "Status" = :status) 
	and (ifnull(:fromDate, '') = '' or "CreateTime" >= :fromDate) 
	and (ifnull(:toDate, '') = '' or "CreateTime" <= :toDate)
	order by "DocEntry" DESC;

END



