

CREATE PROCEDURE APZ_D_Authorization_Copy(in source nvarchar(100), in target nvarchar(4000))
AS
BEGIN

	delete from APZ_D_OPRM where USER_CODE in (select * from  SPLIT(:target, ','));
	
	insert into APZ_D_OPRM(USER_CODE, "menucode", "authoriza")
	select a.DATA, "menucode", "authoriza" 
	from APZ_D_OPRM, (select DATA from SPLIT(:target, ',')) a 
	where USER_CODE = :source;

END




