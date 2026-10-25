
CREATE PROCEDURE apz_filtersetting_insertquery(in UserName nvarchar(20) default '')
as
BEGIN

	delete from APZ_FilterObject where Left("ObjType",2)='Q-';
	
	insert into APZ_FilterObject("ObjType","ObjDesc","DataFilter")
	SELECT 'Q-'|| cast(a."IntrnalKey" as nvarchar),a."QName",a."QString"
				FROM OUQR a
				RIGHT JOIN OQCN b ON a."QCategory" = b."CategoryId"
				WHERE /*CHARINDEX('Y', PermMask) != 0 AND*/ cast(a."IntrnalKey" as nvarchar) IS NOT null
				ORDER BY b."CategoryId", a."QName";

end;





