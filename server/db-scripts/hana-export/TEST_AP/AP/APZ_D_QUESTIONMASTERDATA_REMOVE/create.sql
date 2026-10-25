

CREATE PROCEDURE APZ_D_QuestionMasterData_Remove(in id int default 0)
AS
BEGIN

	--Check if this question is used
	if exists(select 1 from APZ_D_OSUV where "SurveyQuessionId" = :id)
	then
		select 1 as "ErrorCode", 'Cannot remove Question because it has been used' as "ErrorMessage" from dummy;
 		return;
	end if;
	
	--Delete this question
	delete from APZ_D_OCHM where "DocEntry" = :id;
	delete from APZ_D_CHM1 where "DocEntry" = :id;
	
	select 0, '' from dummy;

END




