
CREATE PROCEDURE APZ_D_Promotion_LoadCondition(in Code nvarchar(100))
AS
BEGIN

	create local temporary table #tblCondition as (select top 0 * from APZ_D_PMO1);
	
	insert into #tblCondition
		SELECT * FROM APZ_D_PMO1 WHERE "Code"=:Code;
		
	if not exists (select 1 from #tblCondition)
	THEN
		insert into #tblCondition("Code","Id","FatherId","Type","ConditionType")
			select :Code,1,-1,'G',NULL from DUMMY
				union all 
			select :Code,2,1,'AND',NULL from DUMMY
				union all
			select :Code,3,2,'C','3' FROM DUMMY;
		--union all
		--select @Code,4,3,'C','3'
		--union all
		--select @Code,5,3,'C','3'
		--union all 
		--select @Code,6,2,'C','1'

	end IF;

	select * from #tblCondition;
END



