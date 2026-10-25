
CREATE PROCEDURE APZ_Shift_Save(in typeGet char(1))
AS
BEGIN

	declare checkCount int = 0;
	create local temporary table #APZTemData as (select top 0 * from APZ_OSHP);
	exec 'drop table #APZTemData;';
	exec 'create local temporary table #APZTemData as (select * from "#APZTem");';
	
	if :typeGet = 'A'
	THEN

		update #APZTemData set "SpID" = ifnull((select max("SpID") from APZ_OSHP),0) + 1;

	else

		delete from APZ_OSHP where "SpID" = Ifnull((select top 1 "SpID" from #APZTemData), 0);

	end IF;

	select ifnull((Select top 1 1 from #APZTemData a INNER JOIN apz_OSHP b ON a."Code" = b."Code" and IFNULL(a."BplId",-1) = IFNULL(b."BplId",-1)),0) into checkCount from DUMMY;
	IF :checkCount = 1
	THEN

		select 11001, 'Shift code already in table' from DUMMY;

	END IF;

	insert into APZ_OSHP select * from #APZTemData;
	select 0, '' from DUMMY;

END;






