create procedure ILD_LayPlanNo
as
begin
	Select distinct "U_PlanNo","CreateDate" from  "@MAINTENANCE_H" Order by "CreateDate";
	 
end;

