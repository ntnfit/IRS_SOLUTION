
CREATE PROCEDURE APZ_SavePenalization
AS
BEGIN

	declare error int =0;
	declare error_prefix int =800011;
	
	declare DocNum int;
	declare NextDocNum int;
	declare LineNum int;
	
	create local temporary table #temp1 as (select top 0 * from APZ_HPT1);
	create local temporary table #temp2 as (select top 0 * from APZ_HPT2);
	
	exec 'drop table #temp1';
	exec 'create local temporary table #temp1 as (select * from #APZ_HPT1)';
	
	exec 'drop table #temp2';
	exec 'create local temporary table #temp2 as (select * from #APZ_HPT2)';
	
	select ifnull((select top 1 "DocNum" from #temp2),0) into DocNum from DUMMY;
	select ifnull(Max("DocNum"),0) + 1 into NextDocNum from APZ_HPT2;
	select ifnull(Max("LineNum"), -1) into LineNum from #temp1;
	
	while exists(select 1 from #temp1 where ifnull("LineNum",-1) = -1)
	DO
		LineNum = :LineNum + 1;
		update top 1 #temp1 set "LineNum" = :LineNum where ifnull("LineNum",-1) = -1;
	END WHILE;
	
	if(:DocNum <= 0)
	THEN
		update #temp1 set  "DocNum" = :NextDocNum;
		update #temp2 set "DocNum" = :NextDocNum;
		delete from APZ_HPT1 where "DocNum" = :NextDocNum;
	else
	 	update #temp1 set  "DocNum" = :DocNum;
		delete from APZ_HPT1 where "DocNum" = :DocNum;
	end	IF;
	
	create local temporary table #tbl as (SELECT a."EmpId",a."Date" from #temp1 a INNER JOIN #temp1 b 
		ON a."LineNum" <> b."LineNum" AND a."DocNum" = b."DocNum" and a."EmpId" = b."EmpId" AND a."Date" = b."Date" AND a."PenalizationType" = b."PenalizationType");

	create local temporary table #tbl2 as (select a."Period" from #temp2 a inner join APz_HPT2 b on a."DocNum" <> b."DocNum" and a."Period" = b."Period" and a."Branch" = b."Branch");
	
	IF EXISTS(Select 1 from #tbl2)
	THEN
		SELECT  :error_prefix+1 "ErrorNumber"
							,N'Period duplicated [Period = ' || cast((SELECT top 1 "Period" From #tbl2) as nvarchar) || ']' "ErrorMessage" from dummy;
		return;
	END IF;
	
	update T set T."Remarks" = S."Remarks",
				T."IncludeBonus" = S."IncludeBonus",
				T."IncludePenalty" = S."IncludePenalty",
				T."UserSign2" = S."UserSign2",
				T."UpdateDate" = S."UpdateDate"
		 from APZ_HPT2 T inner join #temp2 s ON T."DocNum" = S."DocNum" and T."Period" = S."Period";
		 
	insert into APZ_HPT2("DocNum", "Period", "Remarks", "Branch", "IncludeBonus", "IncludePenalty", "UserSign2", "UpdateDate")
		select "DocNum", "Period", "Remarks", "Branch", "IncludeBonus", "IncludePenalty", "UserSign2", "UpdateDate" from #temp2 t 
				where not exists(select 1 from APZ_HPT2 s where T."DocNum" = S."DocNum" and T."Period" = S."Period");
	
	--LINE
	insert into APZ_HPT1 select * from #temp1 where "EmpId" is not null;
	
	select :DocNum,'' from DUMMY;
	
END




