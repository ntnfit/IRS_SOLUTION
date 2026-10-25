

CREATE PROCEDURE APZ_D_QuestionMasterData_FindNextLastFrist(
	in id int default 0,
	in navType int)
AS
BEGIN

	declare minDocEntry int = -1;
	declare maxDocEntry int = -1;
	
	--First
	if :navType = 0
	then
		select top 1 * from APZ_D_OCHM  order by "DocEntry" ASC;
	--Previous
	elseif :navType = 1
	then
		
		select (select IFNULL(MIN("DocEntry"), 0) from APZ_D_OCHM) into minDocEntry from dummy;
		if :id <= :minDocEntry
		then
			select top 1 *  from APZ_D_OCHM order by "DocEntry" DESC;
		else
			select top 1 * from  APZ_D_OCHM where "DocEntry" < :id order by "DocEntry" DESC;
		end if;
	--Next	
	elseif :navType = 2
	then
		select (select IFNULL(MAX("DocEntry"), 0) from APZ_D_OCHM) into maxDocEntry from dummy;
		if :id >= :maxDocEntry
		then
			select top 1 * from APZ_D_OCHM  order by "DocEntry" ASC;
		else
			select top 1 * from APZ_D_OCHM  where "DocEntry" > :id order by "DocEntry" ASC;
		end if;
	--Last
	else
		select top 1 * from APZ_D_OCHM order by "DocEntry" DESC;
	end if;
END




