
Create PROCEDURE APZ_WorkPeriod_Remove(in code nvarchar(50) default '', in branch int default 0)
AS
BEGIN
	if(:code IN (select "Period" from APZ_OTSL))
	THEN
		select 1, 'Cannot remove name because it has been used' from DUMMY;
		return;
	end IF;
	
	Delete from APZ_OHWP where "PeriodCode"=:code  and "Branch"=:branch;
	select 0, '' from DUMMY;
end



