
CREATE PROCEDURE apz_process_EmpRelatives_Remove
(in relaId int,
in empId int)
as
BEGIN
DELETE FROM APZ_OHER WHERE "empID"=:empId AND "relaID" = :relaId;
END




