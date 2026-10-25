
CREATE PROCEDURE APZ_ActivityStatus_ShowData
	
AS
BEGIN
	select IFNULL("statusID",0) as "statusID", IFNULL ("name",'') as "name", IFNULL ( "descriptio",'') as "descriptio" ,IFNULL ("Locked",'N') as "Locked"  from OCLA Order by "statusID" asc;
END



