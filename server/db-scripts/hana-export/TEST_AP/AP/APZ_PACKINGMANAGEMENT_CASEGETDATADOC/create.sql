
create procedure APZ_PackingManagement_CaseGetDataDoc(in type int, in caseCode nvarchar(50) default '')
as
begin
	--first
	if :type=0 then
		select top 1 * from APZ_DMC order by "CaseCode" ASC;
		
	--last
	else if :type=3 then
		select top 1 * from APZ_DMC order by "CaseCode" DESC;
	--next
	else if :type=2 then
	
		if :CaseCode = ifnull((select max("CaseCode") from APZ_DMC),'') then

			select top 1 * from APZ_DMC order by "CaseCode" ASC;

		else
		
			select top 1 * from APZ_DMC where "CaseCode" > :CaseCode order by "CaseCode" ASC;
		end if;

		
	--previous
	else if :type=1 then
		if :CaseCode = ifnull((select min("CaseCode") from APZ_DMC),'') then
			
			select top 1 * from APZ_DMC order by "CaseCode" DESC;
		else

			select top 1 * from APZ_DMC where "CaseCode" < :CaseCode order by "CaseCode" DESC;
			
		end if;

	--search
	else 
		select * from APZ_DMC where "CaseCode" = :CaseCode;
	
	
	end if;
	end if;
	end if;
	end if;
end;




