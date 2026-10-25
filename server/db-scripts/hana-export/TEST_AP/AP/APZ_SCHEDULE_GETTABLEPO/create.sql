
CREATE PROCEDURE APZ_SCHEDULE_GETTABLEPO
AS
BEGIN
	create local temporary table #tmpDoc as (select top 0 * from OWOR);
	insert into #tmpDoc ("DocEntry", "DocNum","PIndicator") values (0, 0, '');
	
	create local temporary table #tmpLine as (select top 0 * from WOR1);
	insert into #tmpLine("DocEntry", "LineNum", "VisOrder") values (0, 0, 0);

	select * from #tmpDoc;
	select * from #tmpLine;

	drop table #tmpDoc;
	drop table #tmpLine;
	
END;




