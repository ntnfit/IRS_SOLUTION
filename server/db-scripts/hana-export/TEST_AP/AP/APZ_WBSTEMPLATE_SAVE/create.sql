
CREATE PROCEDURE APZ_WBSTemplate_Save(in tranType char(1) default '')
AS
BEGIN
	declare Id int = 0;
	declare code nvarchar(50) = '';
	declare countCode int = 0;
	declare checkEmp int = 0;
	declare isExstcode smallint;
	--Create temp table
	create local temporary table #APZ_TASKTmp as (select top 0 * from APZ_TASK);
	exec 'DROP TABLE #APZ_TASKTmp;';
	exec 'CREATE LOCAL TEMPORARY TABLE #APZ_TASKTmp AS (SELECT * FROM #APZ_TASK);';
	
	create local temporary table #APZ_WBS1Tmp as (select top 0 * from APZ_WBS1);
	exec 'DROP TABLE #APZ_WBS1Tmp;';
	exec 'CREATE LOCAL TEMPORARY TABLE #APZ_WBS1Tmp AS (SELECT * FROM #APZ_WBS1);';
	
	create local temporary table #APZ_OWBSTmp as (select top 0 * from APZ_OWBS);
	exec 'DROP TABLE #APZ_OWBSTmp;';
	exec 'CREATE LOCAL TEMPORARY TABLE #APZ_OWBSTmp AS (SELECT * FROM #APZ_OWBS);';
	
	--Check row have empty value
	select (select 1 from #APZ_WBS1Tmp where "WBSName" is null) into checkEmp from DUMMY;
	if :checkEmp = 1 then
		delete from #APZ_WBS1Tmp where "WBSName" is null;
	end if;
	
	select (select 1 from #APZ_TASKTmp where ifnull("WBSCode", '') = '') into checkEmp from DUMMY;
	if :checkEmp = 1 then
		delete from #APZ_TASKTmp where ifnull("WBSCode", '') = '';
	end if;
	
	--Check transaction type same 'U' then delete current record else update DocEntry
	if :tranType = 'U' then
		select (select top 1 "Code" from #APZ_OWBSTmp) into code from DUMMY;
		select (select top 1 "DocEntry" from #APZ_OWBSTmp) into Id from DUMMY;
		delete from APZ_TASK where "Code" = :code;
		delete from APZ_WBS1 where "Code" = :code;
		delete from APZ_OWBS where "Code" = :code;
	else
		select (select ifnull(max("DocEntry"), 0) + 1 from APZ_OWBS) into Id from DUMMY;
		update #APZ_OWBSTmp set "DocEntry" = :Id;
	end if;
	
	--Check Code have valid
	select IFNULL((select top 1 1 from #APZ_OWBSTmp a INNER JOIN APZ_OWBS b ON a."Code"=b."Code"),0) into isExstcode from DUMMY;
	if :isExstcode=1
	then
		select 122 ,'Code  is overlapping with an existing Wbs Template' from dummy;
		return;
	END if;
	
	select IFNULL((select top 1 1 from #APZ_OWBSTmp  where ifnull("Code",'')=''),0) into code from DUMMY;
	if :code=1
	then
		select 123 ,'Code  is missing' from dummy;
		return;
	end if;
	
	--Insert data
	insert into APZ_TASK select * from #APZ_TASKTmp;
	insert into APZ_WBS1 select * from #APZ_WBS1Tmp;
	insert into APZ_OWBS select * from #APZ_OWBSTmp;
	
	--Return Id inserted
	select "Code", '' from #APZ_OWBSTmp;
END;



