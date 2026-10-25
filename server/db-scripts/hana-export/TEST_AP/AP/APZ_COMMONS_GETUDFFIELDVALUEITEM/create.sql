
CREATE  PROCEDURE APZ_Commons_GetUDFFieldValueItem(in Father nvarchar(50) default '', in ChildNum int default 0)
as
BEGIN
	select :Father as "U_BOMTemp" FROM DUMMY;
END



