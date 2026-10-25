
CREATE PROCEDURE APZ_EmployeeMaster_RemoveTeamsSetup
	(in teamID int)
AS
BEGIN
	 DELETE  FROM OHTM WHERE "teamID" = :teamID and :teamID NOT IN( SELECT "teamID" FROM HTM1);
END




