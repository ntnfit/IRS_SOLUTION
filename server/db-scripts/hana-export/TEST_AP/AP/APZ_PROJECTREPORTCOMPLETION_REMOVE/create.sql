
create procedure APZ_ProjectReportCompletion_Remove(in Id int)
as
begin
Delete from APZ_ORTC where "DocEntry"=:Id ;
Delete from APZ_RTC1 where "DocEntry"=:Id;
select 0, '' from dummy;
end;




