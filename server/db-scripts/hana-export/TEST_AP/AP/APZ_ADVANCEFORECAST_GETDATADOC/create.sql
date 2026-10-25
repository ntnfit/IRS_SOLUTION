
CREATE PROCEDURE APZ_AdvanceForecast_GetDataDoc(in forecastCode nvarchar(100) default '', in viewType char(1) default '', in typeGet int default 0)
AS
BEGIN

	declare tempCode int = 0;
	IF :typeGet = 4
	THEN

		select * from APZ_OFCT where (ifnull(:forecastCode,'') = '' or  "Code" in (select Data from Split(:forecastCode,',')))  and (ifnull(:viewType, '') = '' or :viewType = "FormView") order by "Code" DESC;

	elseif :typeGet = 0
	THEN

		select top 1 * from APZ_OFCT order by "Code" ASC;

	elseif :typeGet = 2
	THEN

		select ifnull((select max("Code") from APZ_OFCT),0) into tempCode from DUMMY;
		if :forecastCode = :tempCode
		THEN
			select ifnull((select min("Code") from APZ_OFCT),0) - 1 into forecastCode from DUMMY;
		end IF;

		select top 1 * from APZ_OFCT where "Code" > :forecastCode order by "Code" ASC;

	elseif :typeGet = 3
	THEN

		select top 1 * from APZ_OFCT order by "Code" DESC;

	elseIF :typeGet = 1
	THEN
	
		select ifnull((select min("Code") from APZ_OFCT),0) into tempCode from DUMMY;
		if :forecastCode = :tempCode
		THEN
			select ifnull((select max("Code") from APZ_OFCT), 0) + 1 into forecastCode from DUMMY;
		end IF;

		select top 1 * from APZ_OFCT where "Code" < :forecastCode order by "Code" DESC;

	end IF;
END



