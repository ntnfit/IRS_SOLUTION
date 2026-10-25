
CREATE PROCEDURE APZ_BarcodeInventoryCounting_Save(in transType nvarchar(1) default 'A')
AS
BEGIN

	declare checkCount int = 0;
	declare newId int;
	
	------------------------------- Get data Header from temp table -------------------------------
	create local temporary table #tempDocData as (select top 0 * from APZ_OBIC);
	exec 'drop table #tempDocData;';
	exec 'create local temporary table #tempDocData as (select * from "#tempDoc")';
	-------------------------------- END GET DATA -------------------------------------------------

	------------------------------- Get data Line from temp table -------------------------------
	create local temporary table #tempLineData as (select top 0 * from APZ_BIC1);
	exec 'drop table #tempLineData;';
	exec 'create local temporary table #tempLineData as (select * from "#tempLine")';
	-------------------------------- END GET DATA -------------------------------------------------
	
	-------------------------------- Validate data -----------------------------------------------
	select ifnull((select top 1 1 from #tempDocData),0) into checkCount from DUMMY;
	if :checkCount = 0
	then

		select '100000', 'Data header can not be null' from DUMMY;
		return;

	end IF;
	
	select ifnull((select top 1 1 from #tempLineData),0) into checkCount from DUMMY;
	if :checkCount = 0
	then

		select '100001', 'Data Lines can not be null' from DUMMY;
		return;

	end IF;

	select IFNULL((select top 1 1 from APZ_OBIC a inner join #tempDocData b On a."DocEntry" = b."DocEntry"),0) into checkCount from DUMMY;
	if :transType = 'U' and :checkCount = 0
	then

		select '100002', 'Entry does not exists' from DUMMY;
		return;

	end if;
	---------------------------------- END VALIDATE ---------------------------------------------

	select IFNULL((select "DocEntry" from #tempDocData), 0) into newId from DUMMY;

	if :transType = 'A'
	THEN

		select IFnull((select max("DocEntry") from APZ_OBIC), 0) + 1 into newId from DUMMY;
		update #tempDocData set "DocEntry" = :newId;

	END IF;
	
	if :newId = 0
	THEN
	
		select '100003', 'Can not get entry to add new' from DUMMY;
		return;
	
	END IF;

	update #tempLineData set "DocEntry" = :newId;

	delete from APZ_BIC1 a where exists(select 1 from #tempDocData where "DocEntry" = a."DocEntry");
	delete from APZ_OBIC a where exists(select 1 from #tempDocData where "DocEntry" = a."DocEntry");

	update #tempLineData set "VisOrder" = "LineNum";

	insert into APZ_OBIC select * from #tempDocData;
	insert into APZ_BIC1 select * from #tempLineData;

	select "DocEntry", '' from #tempDocData;
	
END;



