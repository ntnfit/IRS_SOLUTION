
CREATE FUNCTION GetTimeInteger(in timeValue time)
RETURNS timeInteger INT
AS
BEGIN

	timeInteger := cast((select replace(Data, ':','') from SPLIT(timeValue, ' ')) as int);
	
END




