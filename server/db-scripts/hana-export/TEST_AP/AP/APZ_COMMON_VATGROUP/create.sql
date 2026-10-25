
CREATE PROCEDURE apz_common_VatGroup(in category NVARCHAR(1) default '')
AS
BEGIN
	IF	:category <>''
	then
		SELECT "Code","Name","Rate","Category" FROM OVTG WHERE "Category"=:category  AND "Inactive"='N';
	ELSE
	SELECT "Code","Name","Rate","Category" FROM OVTG WHERE "Inactive"='N';
	end if;
END




