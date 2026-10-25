
CREATE PROCEDURE APZ_Operations_SaveData(in RowCount int)
AS
BEGIN

	declare checkCount int = 0;
	declare codeDelete nvarchar(100) = '';
	
	create local temporary table #APZ_OOMDData as (select top 0 * from APZ_OOMD);
	exec 'insert into #APZ_OOMDData select * from "#APZ_OOMD";';
	select ifnull((select top 1 1 from #APZ_OOMDData where ifnull("OpeCode", '') = ''),0) into checkCount from dummy;
	if :checkCount = 1
	THEN
				select 3,'Operation code can not be null' FROM DUMMY;
				return;
	END IF;

	select ifnull((select top 1 x."ResCode" from APZ_RTA1 x where not exists(select 1 from #APZ_OOMDData where "OpeCode" = x."OpCode")), '') into codeDelete from DUMMY;

	if :codeDelete <> ''
	THEN
				select 4,'Operation code "' || :codeDelete || '" had been linked to Routing, Can not delete' FROM DUMMY;
				return;
	END IF;

	select IFNULL((select top 1 1 from #APZ_OOMDData group by "OpeCode" having count(1)>1),0) into checkCount from DUMMY;
	if :checkCount = 0
	THEN
		delete from APZ_OOMD where "OpeCode" in (select "OpeCode" from #APZ_OOMDData);
		INSERT INTO APZ_OOMD SELECT * FROM #APZ_OOMDData WHERE "OpeCode" IS NOT NULL;

		select 1, '' from DUMMY;
	else
		select 2,'Operations Code Duplicate' from DUMMY;
	end IF;
END






