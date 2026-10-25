
CREATE PROCEDURE APZ_BomVersion_Save(in tranType char(1) default '')
AS
BEGIN
	DECLARE id int;
	declare checkCount int;
	
	create local temporary table #tempdocData as (select top 0 * from APZ_OITT);
	exec 'drop table #tempdocData;';
	exec 'create local temporary table #tempdocData as (select * from "#tempdoc");';
	
	create local temporary table #tempLineData as (select top 0 * from APZ_ITT1);
	exec 'drop table #tempLineData;';
	exec 'create local temporary table #tempLineData as (select * from "#templine");';
	
	select ifnull((select top 1 "BomVerID" from #tempdocData),0) into id from DUMMY;
	IF :tranType = 'A'
	THEN

		select ifnull((Select top 1 1 from APZ_OITT a INNER JOIN #tempdocData b ON a."Code" = b."Code" and a."Version" = b."Version"),0) into checkCount from DUMMY;
		IF :checkCount = 1
		THEN

			Select 120001, 'Bom Version already in table' from DUMMY;
			return;

		END IF;

		select ifnull((select MAX("BomVerID") from APZ_OITT), 0) + 1 into id from DUMMY;
		update #tempdocData set "BomVerID" = :id;

	else
		
		select IFNULL((Select top 1 1 from APZ_OITT a INNER JOIN #tempdocData b ON a."Code" = b."Code" and a."Version" = b."Version"), 0) into checkCount from dummy;
		IF :checkCount  = 0
		THEN

			Select 120001, 'Bom Version did not already in table' from dummy;
			return;

		END IF;
		
		delete from APZ_OITT where "BomVerID" = :id;
		delete from APZ_ITT1 where "Father" = :id;

	end IF;

	update #templineData set "Father" = :id;

	select ifnull((select top 1 1 from #tempdocData where ifnull("Code",'') = '' or ifnull("Version",'') = ''), 0) into checkCount from DUMMY;
	IF :checkCount = 1
	THEN

		Select 120004, 'Code and version could not null' from DUMMY;
		return;

	END IF;

	select IFNULL((select top 1 1 from #templineData where ifnull("Quantity",0) = 0),0) into checkCount from DUMMY;
	IF :checkCount = 1
	THEN

		Select 120005, 'Father quatity must greater than 0' from DUMMY;
		return;

	END IF;

	select ifnull((select top 1 1 from #templineData where ifnull("Code",'') = ''),0) into checkCount from DUMMY;
	IF :checkCount = 1
	THEN

		Select 120006, 'ItemCode could not null' from DUMMY;
		return;

	END IF;

	select IFNULL((select top 1 1 from #templineData where ifnull("Quantity",0) = 0),0) into checkCount from DUMMY;
	IF :checkCount = 1
	Then

		Select 120007, 'tem quatity must greater than 0' from DUMMY;
		return;

	END IF;

	select ifnull((select top 1 1 from APZ_OITT a inner join #tempdocData b on a."Code" = b."Code" and a."Type" = b."Type" and 
					((a."ActFrom" between b."ActFrom" and b."ActTo") or (b."ActFrom" between a."ActFrom" and a."ActTo") or a."ActTo" = null) where a."Status" = 'Y' and b."Status" = 'Y'),0) into checkCount from DUMMY;
	IF :checkCount = 1
	THEN

			select 120003,'Date Ranges Overlap!' from DUMMY;
			return;

	END IF;

	insert into APZ_OITT select * from #tempdocData;
	insert into APZ_ITT1 select * from #templineData;

	select 0, '' from DUMMY;
END;



