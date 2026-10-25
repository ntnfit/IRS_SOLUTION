
Create Procedure APZ_Solution_FindNextLastFrist(in id bigint, in typeGet int)
as
begin
	
	declare minEntry int = 0;
	declare maxEntry int = 0;
	if :typeGet = 0
	THEN
	
			select top 1 * from OSLT order by "SltCode" ASC;
	
	elseif :typeGet = 1
	THEN
	
		select ifnull((select min("SltCode") from OSLT), 0) into minEntry from DUMMY;
		if :id <= :minEntry
		THEN
	
			select top 1 *  from OSLT   order by "SltCode" DESC;
	
		else
	
			select top 1 * from  OSLT  where "SltCode" < :id order by "SltCode" DESC;
	
		end IF;
	
	elseif :typeGet = 2
	THEN
	
		select ifnull((select max("SltCode") from OSLT), 0) into maxEntry from DUMMY;
		if :id >= :maxEntry
		THEN
	
			select top 1 * from OSLT  order by "SltCode" ASC;
	
		else
		
			select top 1 * from OSLT  where "SltCode" > :id order by "SltCode" ASC;
	
		end IF;
	else
	
			select top 1 * from OSLT order by "SltCode" DESC;
	
	end IF;
END




