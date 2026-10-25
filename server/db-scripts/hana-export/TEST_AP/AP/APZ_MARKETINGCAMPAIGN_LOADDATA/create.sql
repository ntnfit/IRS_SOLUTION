
create procedure APZ_MarketingCampaign_LoadData
(
in id int,
in campaignNo nvarchar(50),
in startFrom timestamp,
in startTo timestamp,
in status char(1)
)
as
begin
	if :id>0
	then
		select * from APZ_OSMC where "DocEntry"=:id;
		return;
	end if;
	select * from APZ_OSMC where (ifnull(:campaignNo,'') ='' or "CampaignNo"=:campaignNo) 
				and (ifnull(:status,'') ='' or "Status"=:status) 
				and (ifnull(:startFrom, '') = '' or "StartDate" >= :startFrom) 
				and (ifnull(:startTo,'') = '' or "StartDate" <= :startTo) 
	order by "DocEntry" Desc;
end;




