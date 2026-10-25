
CREATE PROCEDURE APZ_Operations_GetDataDoc(OpeCode nvarchar(50) default '', in typeGet int)
AS
BEGIN
	declare checkCode nvarchar(50) = '';
	--first
	if :typeGet=0
	THEN
		select top 1 * from APZ_OOMD as newTable order by newTable."OpeCode" ASC;
	--last
	elseif :typeGet=3
	THEN
		select top 1 * from APZ_OOMD as newTable order by newTable."OpeCode" DESC;
	--next
	elseif :typeGet=2
	THEN
		select ifnull((select max("OpeCode") from APZ_OOMD),'') into checkCode from DUMMY;
		if :OpeCode = :checkCode
		THEN
			--set @resCode = ifnull((select min(ResCode) from APZ_ORSC),'')
			select top 1 * from APZ_OOMD as newTable order by newTable."OpeCode" ASC;
		end IF;

		select top 1 * from APZ_OOMD as newTable where newTable."OpeCode" > :OpeCode order by newTable."OpeCode" ASC;
	--previous
	elseif :typeGet=1
	THEN
		select ifnull((select min("OpeCode") from APZ_OOMD),'') into checkCode from DUMMY;
		if :OpeCode = :checkCode
		THEN
			--set @resCode = ifnull((select max(ResCode) from APZ_ORSC), '')
			select top 1 * from APZ_OOMD as newTable order by newTable."OpeCode" DESC;
		else
		
			select top 1 * from APZ_OOMD as newTable where newTable."OpeCode" < :OpeCode order by newTable."OpeCode" DESC;
		end IF;
	end IF;

END



