
Create PROCEDURE APZ_D_SettingApprovalDetail_LoadData(in id int default 0, in fromDate timestamp default '', in toDate timestamp default '', in objectType nvarchar(250) default '')
AS
BEGIN
	if :id>0
	THEN
		select * from APZ_D_OSAD where "DocEntry"=:id;
		return;
	end IF;
	
	select * from APZ_D_OSAD where (ifnull(:objectType,'') ='' or "ApprovalObjectType"=:objectType) and (ifnull(:fromDate, '') = '' or "CreateDate" >= :fromDate) and
									(ifnull(:toDate,'') = '' or "CreateDate" <= :toDate)
	 	order by "DocEntry" DESC;
end



