
CREATE PROCEDURE APZ_PackingManagement_TypeCaseGetDataDoc(in type int, in TypeCode nvarchar(50) default '')
as
begin
	if :type = 0 then
		select top 1 * from APZ_DMTC order by TypeCode ASC;
		
	else if :type = 3 then
		select top 1 * from APZ_DMTC order by TypeCode DESC;
		
	else if :type = 4 then
		if :TypeCode = ifnull((select max("TypeCode") from APZ_DMTC),'') then
			select top 1 * from APZ_DMTC order by "TypeCode" ASC;
		else 
			select top 1 * from  APZ_DMTC where "TypeCode" > :TypeCode order by "TypeCode" ASC;
		end if;
	
	else if :type = 1 then
		if :TypeCode = ifnull((select min("TypeCode") from APZ_DMTC), '') then
			select top 1 * from APZ_DMTC order by "TypeCode" DESC;
		else
			select top 1 * from APZ_DMTC where "TypeCode" < :TypeCode order by "TypeCode" DESC;
		end if;
		
	else
		select * from APZ_DMTC where "TypeCode" = :TypeCode;
		
	end if;
	end if;
	end if;
	end if;
end;




