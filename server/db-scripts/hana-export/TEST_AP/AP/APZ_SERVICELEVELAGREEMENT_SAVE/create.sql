
create procedure APZ_ServiceLevelAgreement_Save
(
	in type char(1)
)
as
begin
Declare Id int =0;
Declare isExist int=0;
create local temporary table #APZ_OSLATmp as (select  top 0 * from APZ_OSLA);
execute immediate 'insert into #APZ_OSLATmp select * from "#APZ_OSLA"';

create local temporary table #APZ_SLA1Tmp as (select  top 0 * from APZ_SLA1);
execute immediate 'insert into #APZ_SLA1Tmp select * from "#APZ_SLA1"';

create local temporary table #APZ_SLA2Tmp as (select  top 0 * from APZ_SLA2);
execute immediate 'insert into #APZ_SLA2Tmp select * from "#APZ_SLA2"';

	if :type='U'
	then
	select top 1 "SLAID" into Id from #APZ_OSLATmp;
	delete from  APZ_SLA1 where "SLAID"=:Id;
	delete from  APZ_SLA2 where "SLAID"=:Id;
	delete from  APZ_OSLA where "SLAID"=:Id;
	
	else
		
	select ifnull(max("SLAID"), 0) + 1 into Id from APZ_OSLA;
	update #APZ_OSLATmp set "SLAID" = :Id;
	end if;
	select count(1) into isExist from #APZ_OSLATmp a INNER JOIN APZ_OSLA b ON a."SLACode"=b."SLACode";
	if(ifnull(isExist,0)=1)
	then

			select 122 ,'Service level agreement code  is overlapping with an existing Service level agreement' from dummy;
			return;
			
	end if;	
				
--UPDATE #APZ_SLA1Tmp set SLAID = :Id,"LineNum"= NumId FROM (SELECT *, ROW_NUMBER() OVER( ORDER BY SLAID) NumId FROM #APZ_SLA1Tmp );
--UPDATE #APZ_SLA2Tmp set SLAID = :Id,"LineNum"= NumId FROM (SELECT *, ROW_NUMBER() OVER( ORDER BY SLAID) NumId FROM #APZ_SLA2Tmp );
--insert into APZ_OSLA select * from #APZ_OSLA;
--insert into APZ_SLA1 select * from #APZ_SLA1;
--insert into APZ_SLA2 select * from #APZ_SLA2;

INSERT INTO APZ_OSLA SELECT * FROM #APZ_OSLATmp;
INSERT INTO APZ_SLA1 SELECT :Id AS SLAID, ROW_NUMBER() OVER(ORDER BY SLAID) AS "LineNum", "Day", "StartTime", "EndTime" FROM #APZ_SLA1Tmp;
INSERT INTO APZ_SLA2 SELECT :Id AS SLAID, ROW_NUMBER() OVER(ORDER BY SLAID) AS "LineNum", "IssueLevel", "ResponseTime", "ResolutionTime" FROM #APZ_SLA2Tmp;
	
select :Id,'' from dummy;

end;





