
create procedure APZ_ProjectReportCompletion_GetLineById(in Id int)
as
begin
select * from APZ_RTC1 where "DocEntry"=:Id;
end;




