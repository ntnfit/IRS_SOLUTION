
drop function Split;

GO


CREATE FUNCTION Split (
	RowData	nvarchar(5000),
	SplitOn	nvarchar(5)
)
RETURNS TABLE (
	"DATA"	nvarchar(5000)
)
AS
BEGIN
	declare _items nvarchar(1000) ARRAY;

	declare _index integer;

	_index := 1;

	WHILE LOCATE(:RowData,:SplitOn) > 0 DO

		_items[:_index] := trim(SUBSTR_BEFORE(:RowData,:SplitOn));

		RowData := SUBSTR_AFTER(:RowData,:SplitOn);

		_index := :_index + 1;

	END WHILE;

	_items[:_index] := trim(:RowData);

	rst = UNNEST(:_items) AS ("DATA");

	return :rst;
END