
Drop Function APZ_D_ConvertDateFormat;

CREATE FUNCTION APZ_D_ConvertDateFormat
(
	IN dateValue date,
	IN format nvarchar(10) default 'yyyy-MM-dd'
)
returns result date
AS
BEGIN
	result = to_date(to_varchar(:dateValue,:format),:format);
END;