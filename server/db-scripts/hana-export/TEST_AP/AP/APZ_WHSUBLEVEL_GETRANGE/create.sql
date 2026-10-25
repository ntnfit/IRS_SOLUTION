
CREATE procedure APZ_WhSublevel_GetRange
(in fldAbs int, in slFrom nvarchar(50), in slTo nvarchar(50), in descr nvarchar(50) default null)
as
begin
	declare query nvarchar(500) = 'select "AbsEntry", "FldAbs", "SLCode", "Descr" from OBSL where "FldAbs" = ' || to_nvarchar(:fldAbs);
	
	if ifnull(:slFrom,'') <> '' then
		query = :query || ' and "SLCode" >= ''' || :slFrom || '''';
	end if;
	
	if ifnull(:slTo,'') <> '' then
		query = :query || ' and "SLCode" <= ''' || :slTo || '''';
	end if;
	
	if :descr is not null then
		query = :query || ' and ifnull("Descr",'') = ''' || :descr || '''';
	end if;
	
	execute immediate (:query); 
end




