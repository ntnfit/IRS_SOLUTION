
create PROCEDURE APZ_CycleCountDeterminationSubtable_GetByCycleCode(in cycleCode int)
AS 
BEGIN
select "NextDate","Hour" from OCYC where "Code"=:cycleCode;
END;




