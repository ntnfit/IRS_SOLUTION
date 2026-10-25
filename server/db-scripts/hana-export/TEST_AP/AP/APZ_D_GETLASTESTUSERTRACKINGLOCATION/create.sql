

CREATE PROCEDURE APZ_D_GetLastestUserTrackingLocation(in userId int)
AS
BEGIN

	declare empId int = -1;
	declare empName nvarchar(254) = '';
	
	select "firstName" || ' ' || "middleName" || ' ' || "lastName" from OHEM
	where userId = :userId;
	
	select (select top 1 "empID" from OHEM where "userId" = :userId) into empId from dummy;
	select top 1 *, :empID as "empID", :empName as "empName" from APZ_D_TKLC where "UserSign" = :userId order by "CreateDate" desc;
	
END;




