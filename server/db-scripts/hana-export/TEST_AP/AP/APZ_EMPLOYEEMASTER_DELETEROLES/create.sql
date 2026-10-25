
CREATE PROCEDURE APZ_EmployeeMaster_DeleteRoles
(in	typeID int)
AS
BEGIN
	DELETE  FROM OHTY WHERE "typeID" = :typeID and :typeID NOT IN( SELECT "roleID" FROM HEM6);
END




