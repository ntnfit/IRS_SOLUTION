
CREATE PROCEDURE APZ_ActivitiesMeetingLocation_GetAll
AS
BEGIN
	select "Code", "Name", IFNULL ( "DataSource",'N') as "DataSource" ,IFNULL ("UserSign",0) as "UserSign" , IFNULL ("Locked",'N') as "Locked" from OCLO
	Order by "Name" asc;
END








