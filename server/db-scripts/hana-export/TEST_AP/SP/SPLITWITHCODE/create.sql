
CREATE FUNCTION SplitWithCode (
	Code nvarchar(100),
	RowData	nvarchar(5000),
	SplitOn	nvarchar(10)
)
RETURNS TABLE (
	"id" int,
	"code" nvarchar(100),
	"part"	nvarchar(5000)
)
AS
BEGIN
	declare splitTable 
	table (
		"id" int,
		"code" nvarchar(100),
		"part"	nvarchar(5000)
	);
	declare _index integer;

	_index := 1;

	WHILE LOCATE(:RowData,:SplitOn) > 0 DO

		insert into :splitTable values (:_index,:Code, SUBSTR_BEFORE(:RowData,:SplitOn));

		RowData := SUBSTR_AFTER(:RowData,:SplitOn);

		_index := :_index + 1;

	END WHILE;

	insert into :splitTable values (:_index,:Code, :RowData);
	return :splitTable;
END


