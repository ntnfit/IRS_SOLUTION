
CREATE PROCEDURE APZ_ResourceWorkSchedule_GetDataLine(in code nvarchar(50) default '', in GetYear int default null)
AS
BEGIN

	if :getYear  is null THEN
		select YEAR(CURRENT_DATE) into getYear from DUMMY;
	END IF;

	select "Code", "WorkDate", (select string_agg("Shift", ',') from (SELECT DISTINCT "Shift" FROM APZ_RWS1 WHERE "WorkDate" = a."WorkDate" and "Code" = :code) x)  AS "Shift", "Remark", 0 "Line" from APZ_RWS1 a
		  where "Code" = :code and Year("WorkDate") = :getYear Group by "WorkDate", "Code", "Remark" order by "WorkDate", "Shift";
		
END



