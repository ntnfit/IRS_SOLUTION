
CREATE PROCEDURE APZ_ActivitiesMeetingLocation_ShowData
AS
BEGIN
	select "Code", "Name", IFNULL ( "DataSource",'N') as "DataSource" ,IFNULL ("UserSign",0) as "UserSign", IFNULL("Locked",'N')as "Locked"  from OCLO;	
END




