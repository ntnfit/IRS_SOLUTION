
CREATE PROCEDURE APZ_ProjectManager_GetScheduleFromDate (in startDate nvarchar(50))
AS
BEGIN
	select days_between(cast(:startDate as date), t0."WorkDate") "Diff", "WorkDate", "Shift", t2."Start1", t2."End1", t2."Start2", t2."End2", t2."Time"
                    from APZ_WSH1 t0 inner join APZ_OWSH t1 on t0.ID = t1."Id"
                    inner join APZ_OSHP t2 on t0."Shift" = t2."Code"
                    and days_between(cast(:startDate as date), t0."WorkDate") >= 0 order by t0."WorkDate";
END



