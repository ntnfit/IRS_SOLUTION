
CREATE PROCEDURE APZ_PackingManagement_GetType
as
begin

	SELECT "TypeCode", "TypeName", "Amount"
	FROM APZ_DMTC
	Where "Status" = 'a';
end;




