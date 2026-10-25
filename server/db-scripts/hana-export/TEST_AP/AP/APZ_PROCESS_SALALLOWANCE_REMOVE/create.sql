
CREATE PROCEDURE apz_process_SalAllowance_Remove
(in lineNum int,
in empId int)
as
BEGIN
DELETE FROM APZ_HSI1 WHERE "EmpId"=:empId AND "LineNum" = :lineNum;
END




