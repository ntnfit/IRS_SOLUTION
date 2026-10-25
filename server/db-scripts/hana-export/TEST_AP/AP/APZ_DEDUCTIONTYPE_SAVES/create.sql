

CREATE PROCEDURE APZ_DeductionType_Saves(tranType char(1))
AS 
BEGIN
	Declare docId bigint = 0;
	declare isValidFrom bigint=0;
	declare isValidTo bigint=0;
	declare validFromNull bigint=0;
	declare lineNum int=0;
	
	/* Create new temporary table */
	create local temporary table #APZ_OHDTLocal as (select top 0 * from APZ_OHDT);
	exec 'DROP TABLE #APZ_OHDTLocal;';
	exec 'CREATE LOCAL TEMPORARY TABLE #APZ_OHDTLocal as (SELECT * FROM #APZ_OHDT)';
	
	create local temporary table #APZ_HDT1Local as (select top 0 * from APZ_HDT1);
	exec 'DROP TABLE #APZ_HDT1Local;';
	exec 'CREATE LOCAL TEMPORARY TABLE #APZ_HDT1Local as (SELECT * FROM #APZ_HDT1)';
	
	/* Check transaction type is U then delete record else update code to temporary table */
	if :tranType = 'U' then 
		select top 1 "Code" into docId from #APZ_OHDTLocal;
		delete from APZ_HDT1 where "Code" = :docId;
		delete from APZ_OHDT where "Code" = :docId;
	else
		select ifnull((select MAX("Code") from APZ_OHDT),0) + 1 into docId from DUMMY;
		update #APZ_OHDTLocal set "Code" = :docId;
		update #APZ_HDT1Local set "Code" = :docId;
	end if;
	
	update #APZ_HDT1Local set "Code" = :docId, "LineNum" = null;
	while exists(select 1 from #APZ_HDT1Local where "LineNum" is null)
	DO
		update top 1 #APZ_HDT1Local set "LineNum" = :lineNum where "LineNum" is null;
		lineNum = :lineNum + 1;
	END WHILE;
		
	
	/* Check data is valid in temporary table */
	select count(1) into  isValidFrom 
	from #APZ_HDT1Local a 
	INNER JOIN #APZ_HDT1Local b ON ((b."ValidFrom" between a."ValidFrom" and a."ValidTo") or (b."ValidTo" between a."ValidFrom" and a."ValidTo") 
	or (a."ValidFrom" between b."ValidFrom" and b."ValidTo") or (a."ValidTo" between b."ValidFrom" and b."ValidTo"))  and a."LineNum" != b."LineNum";
	
	if(ifnull(:isValidFrom,0)=1)
	then
			select 123 ,'Deduction Type is overlapping with an existing Deduction Type' from dummy;
			return;
	end if;
	
	/* Check ValidFrom, ValidTo is valid */
	select count(1) into isValidTo  from #APZ_HDT1Local a where a."ValidFrom" > a."ValidTo";
	if(ifnull(isValidTo,0)=1 ) 
	then

			select 121 ,'Valid From cannot be later than Valid To' from dummy;
			return;

	end if;
	
	/* Check ValidFrom null */
	select count(1) into validFromNull from #APZ_HDT1Local where ifnull("ValidFrom",'') = '';
	IF(ifnull(validFromNull,0)=1)
	then

		Select 124, 'ValidFrom could not null' from dummy;
		return;

	end if;
	
	--Insert record
	insert into APZ_OHDT select * from #APZ_OHDTLocal;
	insert into APZ_HDT1 select * from #APZ_HDT1Local;
	select 0, '' from dummy;
END;




