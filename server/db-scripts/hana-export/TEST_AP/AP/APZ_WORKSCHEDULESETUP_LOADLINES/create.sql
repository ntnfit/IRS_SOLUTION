CREATE PROCEDURE APZ_WorkScheduleSetup_LoadLines(in code varchar(20))
AS
BEGIN
	
	select a."DayID", "ShiftID" as "WorkShift" 
	from APZ_WSC1 a left join APZ_OWSC b on a."SchID" = b."SchID" where a."SchID"=:code;
	
	select a."DayID", "ShiftID" as "WorkShift" 
	from APZ_WSC2 a left join APZ_OWSC b on a."SchID" = b."SchID" where a."SchID"=:code;
	
	select a."DayID", "ShiftID" as "WorkShift" 
	from APZ_WSC3 a left join APZ_OWSC b on a."SchID" = b."SchID" where a."SchID"=:code;
	
	--select a."DayID", ifnull((select string_agg(d."ShiftCode", ',') from APZ_WSC1 c left join APZ_OEWS d on c."ShiftID" = d."ShiftID" where a."DayID" = c."DayID" and a."SchID" = c."SchID"),'') as "WorkShift" 
	--from APZ_WSC1 a left join APZ_OWSC b on a."SchID" = b."SchID" where a."SchID"=:code;
	
	--select a."DayID", ifnull((select string_agg(d."ShiftCode", ',') from APZ_WSC2 c left join APZ_OEWS d on c."ShiftID" = d."ShiftID" where a."DayID" = c."DayID" and a."SchID" = c."SchID"),'') as "WorkShift" 
	--from APZ_WSC2 a left join APZ_OWSC b on a."SchID" = b."SchID" where a."SchID"=:code;
	
	--select a."DayID", ifnull((select string_agg(d."ShiftCode", ',') from APZ_WSC3 c left join APZ_OEWS d on c."ShiftID" = d."ShiftID" where a."DayID" = c."DayID" and a."SchID" = c."SchID"),'') as "WorkShift" 
	--from APZ_WSC3 a left join APZ_OWSC b on a."SchID" = b."SchID" where a."SchID"=:code;
END;

