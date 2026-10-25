
CREATE PROCEDURE Apz_KPIGroup_CFLEmployee
AS
BEGIN
	select "empID" "EmpID", "empID" "EmpNo", ifnull("firstName", '') || ' ' || ifnull("middleName",'') || ' ' || ifnull("lastName", '') "EmpName", * from OHEM WHERE "Active" = 'Y';
END




