
Create PROCEDURE APZ_WarrantyCard_Save(in type char(1))
AS 
BEGIN
	Declare Id int =0;
	Declare isWrntyNum int =0;
	Declare isType int =0;
	
	create local temporary table #APZ_OINSLocal as (select top 0 * from APZ_OINS);
	exec 'drop table #APZ_OINSLocal';
	exec 'create local temporary table #APZ_OINSLocal as (select * from #APZ_OINS)';
	
	create local temporary table #APZ_INS1Local as (select top 0 * from APZ_INS1);
	exec 'drop table #APZ_INS1Local';
	exec 'create local temporary table #APZ_INS1Local as (select * from #APZ_INS1)';
	
	if :type='U'
	then
		select ifnull((select top 1 "WrntyID" from #APZ_OINSLocal),0) into Id from dummy;
		delete from APZ_INS1 where "WrntyID" =:Id;
		delete from APZ_OINS where "WrntyID"=:Id;
	
	else
		
		select ifnull((select max("WrntyID") from APZ_OINS), 0) + 1 into Id from dummy;
		update #APZ_OINSLocal set "WrntyID" = :Id;
	end if;
	select count(1) into isWrntyNum from #APZ_OINSLocal a INNER JOIN APZ_OINS b ON a."WrntyNum"=b."WrntyNum";
	if (ifnull(isWrntyNum,0)=1)
	then

			select 122 ,'Warranty card code  is overlapping with an existing Warranty card' from dummy;
			return;
	end if;
	select count(1) into isType  from #APZ_OINSLocal  where ifnull("Type",'')='';
		if (ifnull(isType,0)=1)
		then

			select 124 ,'Type  is missing' from dummy;
			return;

		end if;
	UPDATE #APZ_INS1Local set "WrntyID" = :Id; --"LineNum" = NumId FROM (SELECT *,  ROW_NUMBER() OVER( ORDER BY "LineNum") NumId FROM #APZ_INS1Local);
	
	insert into APZ_OINS select * from #APZ_OINSLocal;
	insert into APZ_INS1 select :Id as "WrntyID", ROW_NUMBER() OVER(ORDER BY "WrntyID") as "LineNum", ROW_NUMBER() OVER(ORDER BY "ContractID") as "ContractID", "ContractTemplateNum", "ContractNum", "ContractDate", "EndDate", "TerminatedDate", "Status", "Remark" from #APZ_INS1Local;
	select :Id,'' from dummy;
END;




