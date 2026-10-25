
create procedure APZ_PackingManagement_SearchCase(in CaseCode nvarchar(50), in TypeCode nvarchar(50), in PostingDate date)
as
begin
	set :CaseCode = case when :CaseCode ='-1' then '' else :CaseCode end;
	set :TypeCode = case when :TypeCode ='-1' then '' else :TypeCode end;

	if (:PostingDate = '01/01/1990 12:00:00 AM') then
			SELECT *
			From APZ_DMC
			where 
				(LOWER("CaseCode") like LOWER(N'%'|| :CaseCode || '%') )
				and (LOWER("TypeCode") like LOWER(N'%'||:TypeCode||'%') )
			order by "POCode" ASC;

	else

			SELECT *
			From APZ_DMC
			where 
				(LOWER("CaseCode") like LOWER(N'%'|| :CaseCode || '%') )
				and (LOWER("TypeCode") like LOWER(N'%'||:TypeCode||'%') )
				and ("PostingDate" = cast(:PostingDate as date))
			order by "POCode" ASC;

	end if;

end;




