
DROP FUNCTION DATEDIFF;

GO

CREATE FUNCTION DATEDIFF (
	interval nvarchar(11), 
	date1 DATETIME,
	date2 DATETIME
)
RETURNS result DECIMAL
AS
BEGIN
	--lower case interval
	interval = LOWER(:interval);
	result := 0;
	
	IF :interval IN('y','yy','yyyy','year') THEN
		result = YEARS_BETWEEN (date1, date2);
	ELSEIF :interval IN('q','qq','quarter') THEN
		result = FLOOR(MONTHS_BETWEEN (date1, date2)/3) + MOD(MONTHS_BETWEEN (date1, date2),3);
	ELSEIF :interval IN('m','mm','month') THEN
		result = MONTHS_BETWEEN (date1, date2);
	ELSEIF :interval IN('dy','dayofyear') THEN
		result = DAYS_BETWEEN (date1, date2);
	ELSEIF :interval IN('d','dd','day') THEN
		result = DAYS_BETWEEN (date1, date2);
	ELSEIF :interval IN('wk','ww','week') THEN
		result = FLOOR(DAYS_BETWEEN (date1, date2)/7) + (CASE WHEN MOD(DAYS_BETWEEN (date1, date2),7) > 0 THEN 1 ELSE 0 END);
	ELSEIF :interval IN('hh','hour') THEN
		result = FLOOR(SECONDS_BETWEEN (date1, date2)/60/60) + (CASE WHEN MOD(SECONDS_BETWEEN (date1, date2)/60,60) > 0 THEN 1 ELSE 0 END);
	ELSEIF :interval IN('n','mi','minute') THEN
		result = FLOOR(SECONDS_BETWEEN (date1, date2)/60) + (CASE WHEN MOD(SECONDS_BETWEEN (date1, date2),60) > 0 THEN 1 ELSE 0 END);
	ELSEIF :interval IN('s','ss','second') THEN
		result = SECONDS_BETWEEN (date1, date2);
	ELSEIF :interval IN('ms','millisecond') THEN
		result = NANO100_BETWEEN (date1, date2);
	ELSEIF :interval IN('mcs','microsecond') THEN
		result = FLOOR(NANO100_BETWEEN (date1, date2)/10) + (CASE WHEN MOD(NANO100_BETWEEN (date1, date2),10) > 0 THEN 1 ELSE 0 END);
	ELSEIF :interval IN('ns','nanosecond') THEN
		result = NANO100_BETWEEN (date1, date2)*100;
	END IF;

END;