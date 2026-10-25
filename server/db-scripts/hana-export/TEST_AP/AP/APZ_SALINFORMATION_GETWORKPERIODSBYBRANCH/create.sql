
CREATE PROCEDURE APZ_SalInformation_GetWorkPeriodsByBranch(in branch int)
as
BEGIN
	select "PeriodCode" as "Code","PeriodName" as "Name","PeriodStatus" as "status" from APZ_OHWP where "Branch" = :branch order by "FromDate" DESC;	
END

