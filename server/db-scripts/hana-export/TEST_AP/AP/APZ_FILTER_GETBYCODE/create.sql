
CREATE procedure apz_filter_getbycode(in Code nvarchar(100))
as
BEGIN
	select * from APZ_FILTER where "Code"=:Code;
END



