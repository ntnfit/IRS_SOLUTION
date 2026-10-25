
CREATE PROCEDURE APZ_CycleCountDeterminationSubtable_GetInventoryCycles
AS 
BEGIN
select "Code","Name","NextDate","Hour" from OCYC where "Type"='C';
END




