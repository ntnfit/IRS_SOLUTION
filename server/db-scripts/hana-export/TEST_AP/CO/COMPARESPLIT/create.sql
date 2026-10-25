
CREATE FUNCTION CompareSplit(
	compareData nvarchar(5000),
	RowData	nvarchar(5000),
	SplitOn	nvarchar(5)
)
RETURNS result smallint
AS
BEGIN

	declare _index integer;
	declare _tempData nvarchar(5000) = '';

	_index := 1;

	WHILE LOCATE(:RowData,:SplitOn) > 0 DO

		_tempData := SUBSTR_BEFORE(:RowData,:SplitOn);
		if :_tempData = :compareData
		then
		
			result = 1;
			return;
		
		END IF;
		RowData := SUBSTR_AFTER(:RowData,:SplitOn);
		_index := :_index + 1;

	END WHILE;
	
	if :RowData = :compareData
	then
	
		result = 1;
	
	else
		result = 0;
	END if;
END;




