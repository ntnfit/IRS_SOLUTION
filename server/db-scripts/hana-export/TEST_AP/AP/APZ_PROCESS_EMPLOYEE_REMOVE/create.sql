
CREATE PROCEDURE  apz_process_Employee_Remove
(in empID int default 0)
as
BEGIN
	declare checkExist int;
	declare tbl table (
			"ObjType" VARCHAR(20),
			"DocEntry" int
		);
		
	CALL APZ_EmployeeMasterData_exists_linked (:empID,:tbl);
	
	select count(*) into checkExist from :tbl;
	IF (checkExist >0)
	THEN
	BEGIN
		SELECT 'You cannot remove this Employee; linked transactions exist' from Dummy;
		return;
	END;
	END IF;
	DELETE FROM OHEM WHERE "empID"=:empID;
	DELETE FROM HEM1 WHERE "empID"=:empID;
	DELETE FROM HEM2 WHERE "empID"=:empID;
	DELETE FROM HEM3 WHERE "empID"=:empID;
	DELETE FROM HEM4 WHERE "empID"=:empID;
	DELETE FROM HEM5 WHERE "empID"=:empID;
	DELETE FROM HEM6 WHERE "empID"=:empID;
	DELETE FROM HEM7 WHERE "empID"=:empID;
	DELETE FROM APZ_OHER WHERE "empID"=:empID;
	DELETE FROM APZ_OHEH WHERE "empID"=:empID;
	DELETE FROM HTM1 WHERE "empID"=:empID;
	DELETE FROM APZ_OHEC WHERE "empID"=:empID;
END




