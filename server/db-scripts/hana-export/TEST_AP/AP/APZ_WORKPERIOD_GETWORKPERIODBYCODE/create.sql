
Create PROCEDURE APZ_WorkPeriod_GetWorkPeriodByCode(in code nvarchar(50) default '', in branch int default 0)
AS
BEGIN
	select * from APZ_OHWP where "PeriodCode"=:code  and "Branch"=:branch;
end



