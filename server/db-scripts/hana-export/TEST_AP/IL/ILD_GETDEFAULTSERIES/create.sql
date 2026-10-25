create procedure ILD_GetDefaultSeries(in userId int, in objCode nvarchar(50))
as
begin
	declare series int;
	
	select "Series" into series default -1 from NNM2 where "ObjectCode"=:objCode and "UserSign"=:userid;
	if :series <> -1 then
		select :series from dummy;
		return;
	end if;
	
	select "DfltSeries" into series default -1 from ONNM where "ObjectCode"=:objCode;
	select :series from dummy; 
	
end

