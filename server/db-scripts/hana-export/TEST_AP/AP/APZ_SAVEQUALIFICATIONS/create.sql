

CREATE PROCEDURE APZ_SaveQualifications
AS 
BEGIN
	declare exist int=0;
	declare quaCode int=0;
	create local temporary table #APZ_QualificationsLocal as(select top 0 * from APZ_OHQU);
	exec 'drop table #APZ_QualificationsLocal;';
	exec 'create local temporary table #APZ_QualificationsLocal as (select * from "#APZ_Qualifications");';
	
	select (select 1 from APZ_OHQU a 
	inner join #APZ_QualificationsLocal b on a."QuaCode"=b."QuaCode"
	where a."Name"<>b."Name" and a."QuaCode" in (select "QuaCode" from APZ_OHAQ)) into exist from DUMMY;
	if :exist=1 then 
		select 1 as "ErrorCode",'You cannot update because being used' as "ErrorMessage" from DUMMY;
		return;
	end if;
	
	delete from APZ_OHQU;
	select (select 1 from #APZ_QualificationsLocal where "Name" in(select "Name" from APZ_OHQU)) into exist from DUMMY;
	if :exist=1 then
		select 2 as "ErrorCode", 'Qualification is overlapping with an existing Qualification' as "ErrorMessage" from DUMMY;
		return;
	end if;
	
	--Insert record with QuaCode <> 0
	insert into APZ_OHQU("QuaCode","QuaGroup","Name","Description") 
	select "QuaCode","QuaGroup","Name","Description" from #APZ_QualificationsLocal where ifnull("QuaCode",0)<>0;	
	
	--Insert record with QuaCode = 0
	select ifnull((select max("QuaCode") from APZ_OHQU),0) into quaCode from DUMMY;
	insert into APZ_OHQU("QuaCode","QuaGroup","Name","Description") 
	select quaCode + row_number() over (partition by "QuaCode" order by "QuaCode" asc),"QuaGroup","Name","Description" from #APZ_QualificationsLocal where ifnull("QuaCode",0)=0;	 
	
  	select 0, '' from DUMMY;
END;




