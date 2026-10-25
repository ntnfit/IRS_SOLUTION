

CREATE PROCEDURE APZ_D_SettingReport_FindNextLastFrist(in id bigint, navType int)
AS
BEGIN

	declare docMin int = -1;
	declare docMax int = -1;

	if :navType = 0
	then
		select top 1 * from APZ_D_OSRT  order by "DocEntry" asc;
	end if;
	
	if :navType = 1
	then
		docMin := cast((select min("DocEntry") from APZ_D_OSRT) as int);
		if :id <= :docMin
		then
			select top 1 *  from APZ_D_OSRT order by "DocEntry" desc;
		else	
			select top 1 * from  APZ_D_OSRT where "DocEntry" < :id order by "DocEntry" desc;
		end if;
	end if;
	
	if :navType = 2
	then
		docMax := cast((select ifnull(max("DocEntry"), 0) from APZ_D_OSRT) as int);
		if :id >= :docMax
		then
			select top 1 * from APZ_D_OSRT order by "DocEntry" asc;
		else
			select top 1 * from APZ_D_OSRT where "DocEntry" > :id order by "DocEntry" asc;
		end if;
	end if;
	
	if :navType = 3
	then
		select top 1 * from APZ_D_OSRT order by "DocEntry" desc;
	end if;

END




