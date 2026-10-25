
create procedure APZ_ServiceLevelAggreement_Remove
(
in Id int
)
as
begin
Declare isExist int=0;
select count(1) into isExist from APZ_OSLA a  where a.SLAID  in(SELECT DISTINCT SLAID from APZ_OCTT);
if( ifnull(isExist,0)=1)
	then
			select 122 ,'Service Level Aggreement cant not delete,this record is being used!' from dummy;
			return;
	end if;
Delete from APZ_OSLA where SLAID=:Id ;
Delete from APZ_SLA1 where SLAID=:Id;
Delete from APZ_SLA2 where SLAID=:Id;
select 0, '' from dummy;
end;




