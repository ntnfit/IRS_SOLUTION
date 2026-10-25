
CREATE PROCEDURE APZ_CycleCountDeterminationSubtable_GetUsers
AS 
BEGIN
select USERID,U_NAME from OUSR where "Locked"='N';
END




