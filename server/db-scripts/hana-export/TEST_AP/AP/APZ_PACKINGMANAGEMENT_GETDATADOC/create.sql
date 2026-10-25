
create procedure APZ_PackingManagement_GetDataDoc(in type int, in POCode int default 0)
as
begin
	if :type = 0 then
		select top 1 * from APZ_DMPO order by "POCode" ASC;
		
	else if :type = 3 then
		select top 1 * from APZ_DMPO order by "POCode" DESC;
		
	else if :type = 2 then
		if :POCode >= ifnull((select max("POCode") from APZ_DMPO), 0) then
			select top 1 * from APZ_DMPO order by "POCode" ASC;
		else
			select top 1 * from APZ_DMPO where "POCode" > :POCode order by "POCode" ASC;
		end if;
		
	else if :type = 1 then
		if :POCode <= ifnull((select min("POCode") from APZ_DMPO), 0) then
			select top 1 * from APZ_DMPO order by "POCode" DESC;
		else
			select top 1 * from APZ_DMPO where "POCode" < :POCode order by "POCode" DESC;
		end if;
		
	else
		if (select count(1) from APZ_DMPO where "POCode" = :POCode) > 0 then
			select * from APZ_DMPO where "POCode" = :POCode;
		else 
			create local temporary table #DPOMTL as
				(select top 1 * from APZ_DMPO order by "POCode" DESC);
			update #DPOMTL set "POCode" = "POCode" + 1;
			select * from #DPOMTL;
			
		end if;
		end if;
		end if;
		end if;
		
	end if;
		
end;




