
CREATE PROCEDURE SP_APZ_GETEXCHANGERATES(
 in Month Nvarchar(50),
 in Year Nvarchar(50))
As
BEGIN
	DECLARE mainCurr  Nvarchar(10);
	DECLARE cols  NVARCHAR(5000) = '';
	DECLARE query  NVARCHAR(5000);
	
	select (select "MainCurncy" from oadm) into mainCurr from DUMMY;
	
	select (SELECT distinct string_agg(',' || "CurrCode") FROM ocrn where "CurrCode" != :mainCurr) into cols from DUMMY;
	if length(:cols) > 1
	then
		cols = substring(:cols, 2, length(:cols));
	end if;
	
	create local temporary table #result (docentry int);
	
	query =	'DO BEGIN
	DECLARE startDate DATE=CAST(''' || :Year || '-' || :Month || '-01'' as DATE);
	DECLARE endDate DATE= ADD_DAYS(ADD_MONTHS(:startDate,1), -1);
	create local temporary table #tbl (
	"ratedate" timestamp
	);
	insert into #tbl("ratedate")
	SELECT ADD_DAYS(:startDate,cast(data as int)) 
	FROM split(''0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30'', '','') where cast(data as int) < cast(TO_VARCHAR(:endDate, ''dd'') as int);
	
	create local temporary table #ExRateOut ("RateDate" timestamp);
	create local temporary table #inTableRate as (SELECT * FROM (SELECT ortt."Currency",ifnull(ortt."Rate",0) "rate","RateDate" FROM ortt where cast(TO_VARCHAR(ortt."RateDate", ''yyyy'') as int)=' || :Year 
		|| ' and cast(TO_VARCHAR(ortt."RateDate", ''MM'') as int)=' || :Month || '));
	
	call PivotTable(''#ExRateOut'', ''#inTableRate'', ''max'', ''rate'', ''numeric(19,6)'', ''RateDate'', ''' ||:cols|| ''', ''Currency'');
	exec ''drop table #result;'';
	create local temporary table #result as (select B."ratedate" as "Date",A.*  from #tbl B LEFT JOIN #ExRateOut A on to_varchar(B."ratedate", ''dd'')=to_varchar(A."RateDate",''dd'') order by "Date");
	drop table #tbl;
	drop table #ExRateOut;
	drop table #inTableRate;
	END;';
	--select :query from dummy;
	 execute immediate :query;
	 select * from #result;
	 drop table #result;
END




