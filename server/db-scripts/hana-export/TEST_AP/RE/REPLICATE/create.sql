

CREATE FUNCTION REPLICATE (IN string nvarchar(100), IN repeat_count int)
RETURNS REPLICATED NVARCHAR(5000)
AS
BEGIN

	DECLARE i int;
	DECLARE tempString nvarchar(5000) = '';
	
	FOR i IN 1..repeat_count DO
		tempString = :tempString || :string;
	END FOR;
	
	REPLICATED = :tempString;
	
END



