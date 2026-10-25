
CREATE PROCEDURE APZ_PackingManagement_SearchTypeCase(in TypeCode nvarchar(50), in TypeName nvarchar(50), in CreateDate date)
as
begin

	create local temporary table #APZ_DMTC as (select top 0 * from APZ_DMTC);
	exec 'insert into #APZ_DMTC select * from #DMTCT;';
	
	if :TypeCode = 'U' then
		delete from APZ_DMTC where "TypeCode" = (select top 1 "TypeCode" from #APZ_DMTC);
		UPDATE #APZ_DMTC SET  "CreateDate" = current_date;	
		INSERT INTO APZ_DMTC select * from #APZ_DMTC;
	else
		UPDATE #APZ_DMTC SET "CreateDate" = current_date;
		INSERT INTO APZ_DMTC select * from #APZ_DMTC;
	end if;
end;




