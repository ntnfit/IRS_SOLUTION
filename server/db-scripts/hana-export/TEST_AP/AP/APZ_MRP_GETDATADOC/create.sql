
create PROCEDURE APZ_MRP_GetDataDoc(in id bigint default 0, type int default 0)
AS
BEGIN
	declare minId int;
	declare maxId int;
	
	if :type = 0 then
		select top 1 * from APZ_OMRP order by "MrpCode" ASC;
		return;
	end if;
	
	if :type = 1 then
		minId = ifnull((select min("MrpCode") from APZ_OMRP), 0);
		if :id <= :minId then
			select top 1 * from APZ_OMRP order by "MrpCode" DESC;
		else
			select top 1 * from APZ_OMRP where "MrpCode" < :id order by "MrpCode" DESC;
		end if;
		return;
	end if;
	
	if :type = 2 then
		maxId = ifnull((select max("MrpCode") from APZ_OMRP), 0);
		if :id >= :maxId then
			select top 1 * from APZ_OMRP order by "MrpCode" ASC;
		else
			select top 1 * from APZ_OMRP where "MrpCode" > :id order by "MrpCode" ASC;
		end if;
		return;
	end if;
	
	if :type = 3 then

		select top 1 * from APZ_OMRP order by "MrpCode" DESC;
		return;
	end if;

		Select * from APZ_OMRP where "MrpCode" = :id;

END




