

CREATE PROCEDURE APZ_WorkingProcess_CheckUserPermission(in wddCode int)
AS
BEGIN

	declare curStepCode int = 0;
	declare curStatus varchar(1) = '';
	declare wtmCode int = 0;
	declare lastStage int = 0;
	
	curStepCode := cast((select "CurrStep" from OWDD where "WddCode" = :wddCode) as int);
	curStatus := cast((select "Status" from OWDD where "WddCode" = :wddCode) as varchar);
	wtmCode := cast((select "WtmCode" from OWDD where "WddCode" = :wddCode) as int);
	lastStage := cast((select "WstCode" from WTM2 where "WtmCode" = :wtmCode and "SortId" = (select MAX("SortId") from WTM2 where "WtmCode" = :wtmCode)) as int);
	
	
	if :curStepCode = :lastStage and :curStatus = 'Y'
	then 
		select 'Y' as "Status" from DUMMY;
	else
		select 'N' as "Status" from DUMMY;
	end if;

END




