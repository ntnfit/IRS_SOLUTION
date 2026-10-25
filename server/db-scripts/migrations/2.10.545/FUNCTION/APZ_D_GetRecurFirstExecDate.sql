
Drop Function APZ_D_GetRecurFirstExecDate;

CREATE FUNCTION APZ_D_GetRecurFirstExecDate
(
	IN startDate date,
	IN dayOfDate nvarchar(10),
	IN type nvarchar(3),
	IN format nvarchar(10) default 'yyyy-MM-dd'
)
returns result date
AS

BEGIN
	declare nextDayID int;
	select case
		when(upper(:dayOfDate) = 'MONDAY') then 0
		when(upper(:dayOfDate) = 'TUESDAY') then 1
		when(upper(:dayOfDate) = 'WEDNESDAY') then 2
		when(upper(:dayOfDate) = 'THURSDAY') then 3
		when(upper(:dayOfDate) = 'FRIDAY') then 4
		when(upper(:dayOfDate) = 'SATURDAY') then 5
		else 6 end
		into nextDayID from dummy;
	if :type = 'W' then
		if upper(dayname(:startDate)) = upper(:dayOfDate) then
			result = :startDate;
		else 
			if :nextDayID > WEEKDAY(:startDate) then
				result = ADD_DAYS(:startDate,:nextDayID - WEEKDAY(:startDate));
			else
				result = ADD_DAYS(:startDate,:nextDayID - WEEKDAY(:startDate) + 7);
			end if;
		end if;
	else
		if cast(:dayOfDate as int) < extract(day from :startDate) then
			result = ADD_MONTHS(to_date(to_varchar(:startDate,'yyyy-MM') || '-' || :dayOfDate,:format),1);
		else
			result = to_date(to_varchar(:startDate,'yyyy-MM') || '-' || :dayOfDate,:format);
		end if;
	end if;
END;