
CREATE PROCEDURE apz_process_SalAdvance_Remove
(in lineNum int,
in empId int)
as
BEGIN
DELETE FROM APZ_HSI2 WHERE "EmpId"=:empId AND "LineNum" = :lineNum;
END




