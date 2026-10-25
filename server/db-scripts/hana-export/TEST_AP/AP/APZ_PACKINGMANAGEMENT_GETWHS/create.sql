
CREATE PROCEDURE APZ_PackingManagement_GetWhs
as
begin

	SELECT "WhsCode", "WhsName" 
	FROM OWHS;
end;




