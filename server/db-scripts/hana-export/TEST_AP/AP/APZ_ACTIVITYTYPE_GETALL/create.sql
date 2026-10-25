
CREATE PROCEDURE APZ_ActivityType_GetAll
AS
BEGIN
	select "Code", ifnull ("Name",'') as "Name", ifnull("DataSource",'N') "DataSource" ,ifnull("UserSign",1) "UserSign" from OCLT order by "Name" asc;
END




