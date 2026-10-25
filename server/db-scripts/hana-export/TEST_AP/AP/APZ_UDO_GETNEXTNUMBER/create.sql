
CREATE PROCEDURE APZ_UDO_GetNextNumber(in udoCode nvarchar(50), in series int)
AS
BEGIN
	if exists(select 1 from OUDO where "Code"=:udoCode and "TYPE" = 3) then
		select "NextNumber" from NNM1 where "ObjectCode"=:udoCode and "Series"=:series;
	else
		select ifnull("BeginStr", '') || cast("NextNumber" as nvarchar) || ifnull("EndStr", '') from NNM1 where "ObjectCode"=:udoCode and "Series"=:series;
	end if;
END



