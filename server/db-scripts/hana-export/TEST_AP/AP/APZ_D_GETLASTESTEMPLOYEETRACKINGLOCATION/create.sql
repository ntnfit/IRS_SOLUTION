

CREATE PROCEDURE APZ_D_GetLastestEmployeeTrackingLocation(in empId int)
AS
BEGIN

	declare empName nvarchar(254) = '';
	empName := CAST((select ifnull("firstName", '') || ' ' || ifnull("middleName", '') || ' ' || ifnull("lastName", '') from OHEM where "empID" = :empId) AS NVARCHAR);
	
	select top 1 *, :empId as "empID", :empName as "empName" from APZ_D_TKLC 
	where "UserSign" = (select top 1 "userId" from OHEM where "empID" = :empId) order by "CreateDate" desc;
	
END



