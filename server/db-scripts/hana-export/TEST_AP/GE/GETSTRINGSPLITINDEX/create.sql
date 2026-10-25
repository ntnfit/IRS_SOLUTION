
CREATE FUNCTION GetStringSplitIndex (
	RowData	nvarchar(5000),
	SplitOn	nvarchar(5),
	stringIndex int
)
RETURNS result nvarchar(5000)
AS
BEGIN
	declare _index integer;
	_index := 1;
	WHILE LOCATE(:RowData,:SplitOn) > 0 DO

		if :_index = :stringIndex
		THEN
			result = trim(SUBSTR_BEFORE(:RowData,:SplitOn));
			return;
		END IF;

		RowData := SUBSTR_AFTER(:RowData,:SplitOn);
		_index := :_index + 1;

	END WHILE;
	if _index = :stringIndex then
		result := :RowData;
	else
	 result = '';
	 end if;
END




