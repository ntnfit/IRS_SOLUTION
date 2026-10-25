CREATE procedure ILD_LayTaskList
as
begin
	 select "Code","Name"  from "@TASKLISTTEMPLATE" order by "Code";
end;

