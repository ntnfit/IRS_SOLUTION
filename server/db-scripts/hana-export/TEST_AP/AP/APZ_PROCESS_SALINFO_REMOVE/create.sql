
CREATE PROCEDURE apz_process_SalInfo_Remove
(in lineNum int,
in empId int)
as
BEGIN
DELETE FROM APZ_OHSI WHERE "EmpId"=:empId AND "LineNum" = :lineNum;
END




