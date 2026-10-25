

CREATE PROCEDURE APZ_D_QuestionMasterData_GetLineById(in id int default 0)
AS
BEGIN

	select * from APZ_D_CHM1 where "DocEntry" = :id;

END



