
create procedure APZ_MaterialShortageReport_Save(in type char(1))

as
begin

	DECLARE code int = IFNULL((SELECT MAX("Code") from APZ_OMSR), 0) + 1;
	DECLARE maxLine int;
	
	create local temporary table #APZ_OMSR as (select top 0 * from APZ_OMSR);
	execute immediate 'insert into #APZ_OMSR select * from "#TempDoc"';
	
	create local temporary table #APZ_MSR1 as (select top 0 * from APZ_MSR1);
	execute immediate 'insert into #APZ_MSR1 select * from "#TempLine"';
	
	select IfNULL((SELECT MAX("LineId") from #APZ_MSR1),0) into maxLine from dummy;
	
	if (select count(1) from #APZ_OMSR) <= 0 then
		SELECT 7000174, 'Data doc could not null' from dummy;
		return;
	end if;
	
	if (select count(1) from #APZ_MSR1) <= 0 then
		SELECT 7000175, 'Data line could not null' from dummy;
		return;
	end if;
	
	if (select count(1) from #APZ_OMSR where "FromDate" is null) > 0 then
		select 7000172, 'From date could not null' from dummy;
		return;
	end if;
	
	if (select count(1) from #APZ_OMSR where "ToDate" is null) > 0 then
		select 7000173, 'To date could not null' from dummy;
		return; 
	end if;
		
	IF :type = 'U' then
		
		if (select count(1) from APZ_OMSR a INNER JOIN #APZ_OMSR b ON a."Code" = b."Code") <= 0 then
		
			SELECT 7000171, 'Material Shortage did not exists' from dummy;
			return;

		end if;
		
		select (select "Code" from #APZ_OMSR) into code from dummy;

		DELETE FROM APZ_MSR1 WHERE "Code" = :code;
		DELETE FROM APZ_OMSR WHERE "Code" = :code;
		
		create local temporary table #temp as
		(
			select 
			"Code",
			ROW_NUMBER() OVER(ORDER BY "PrdNo" ASC) as "LineId",
			"WOID",
			"PrdNo",
			"PrdName",
			"SubCode",
			"SubName",
			"Note",
			"Qty",
			"PrdOpnQty",
			"PrdPlnQty",
			"JobNo"
 			from #APZ_MSR1 where IFNULL("LineId",0) = 0
		);
		
		update #temp set "LineId" = "LineId" + :maxLine;
		delete from #APZ_MSR1 where IFNULL("LineId",0) = 0;
		insert into #APZ_MSR1 select * from #temp;
		
	ELSE
	
		create local temporary table #temp as
		(
			select 
			"Code",
			ROW_NUMBER() OVER(ORDER BY "PrdNo" ASC) as "LineId",
			"WOID",
			"PrdNo",
			"PrdName",
			"SubCode",
			"SubName",
			"Note",
			"Qty",
			"PrdOpnQty",
			"PrdPlnQty",
			"JobNo"
 			from #APZ_MSR1
		);
		
		update #temp set "Code" = :code;
		delete from #APZ_MSR1;
		insert into #APZ_MSR1 select * from #temp;
		
		UPDATE #APZ_OMSR SET "Code" = :code;

	END IF;

	INSERT INTO APZ_OMSR SELECT * FROM #APZ_OMSR;
	INSERT INTO APZ_MSR1 SELECT * FROM #APZ_MSR1;
	
	UPDATE ONNM SET "AutoKey" = IFNULL((SELECT MAX("Code") from APZ_OMSR), 0) + 1 where "ObjectCode" = '700016';
	SELECT :code, '' FROM DUMMY;
	
end;




