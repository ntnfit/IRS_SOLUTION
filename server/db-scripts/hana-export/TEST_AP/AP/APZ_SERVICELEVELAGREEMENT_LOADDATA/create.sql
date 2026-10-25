
create procedure APZ_ServiceLevelAgreement_LoadData
(
in id int,
in SLANo nvarchar(50),
in proService char(1)
)
as
begin
if :id>0
then
	select * from APZ_OSLA where SLAID=:id;
	return ;
end if;
select * from APZ_OSLA where 
(ifnull(:SLANo,'') ='' or "SLACode"=:SLANo) and
(ifnull(:proService,'') ='' or "ProService"=:proService) ;
end;




