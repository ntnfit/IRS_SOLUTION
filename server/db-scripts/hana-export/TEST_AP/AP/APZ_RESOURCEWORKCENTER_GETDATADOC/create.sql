
CREATE PROCEDURE APZ_ResourceWorkCenter_GetDataDoc(resCode nvarchar(50) default '', in typeGet int default 0)
AS
BEGIN
	declare checkCode nvarchar(50) = '';
	--first
	if :typeGet = 0
	THEN
		select top 1 * from ORSC order by "ResCode" ASC;

	--last
	elseif :typeGet=3
	THEN
		select top 1 * from ORSC order by "ResCode" DESC;
	
	--next
	elseif :typeGet=2
	THEN
		select ifnull((select max("ResCode") from ORSC),'') into checkCode from DUMMY;
		if :resCode = :checkCode
		THEN
			--set @resCode = isnull((select min(ResCode) from APZ_ORSC),'')
			select top 1 * from ORSC order by "ResCode" ASC;
		end IF;

		select top 1 * from ORSC where "ResCode" > :resCode order by "ResCode" ASC;
	
	--previous
	elseif :typeGet=1
	THEN
		select ifnull((select min("ResCode") from ORSC),'') into checkCode from DUMMY;
		if :resCode = :checkCode
		THEN
			select top 1 * from ORSC order by "ResCode" DESC;
		
		else
			select top 1 * from ORSC where "ResCode" < :resCode order by "ResCode" DESC;
		end IF;
	end IF;

END



