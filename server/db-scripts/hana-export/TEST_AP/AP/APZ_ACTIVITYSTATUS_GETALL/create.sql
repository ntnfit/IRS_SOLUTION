
CREATE PROCEDURE APZ_ActivityStatus_GetAll
	
AS
BEGIN
	select IFNULL("statusID",0) as "statusID", ifnull ("name",'') as "name", IfNULL ("descriptio",'') as "descriptio" ,IfNULL ("Locked",'N') as "Locked"  from OCLA	Order by "name" asc;
END






