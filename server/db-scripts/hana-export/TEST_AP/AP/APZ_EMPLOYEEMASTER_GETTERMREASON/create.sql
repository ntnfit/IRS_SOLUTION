
CREATE  PROCEDURE APZ_EmployeeMaster_GetTermReason
AS
BEGIN
	select "reasonID" as "Code", "name" as "Name" From OHTR;
END




