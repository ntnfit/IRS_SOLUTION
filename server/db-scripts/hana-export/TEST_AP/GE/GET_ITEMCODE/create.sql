CREATE procedure GET_ITEMCODE()
as
begin

	/**
	create local temporary table #tbl(
		Code nvarchar(1),
		Status nvarchar(15)
	);
	insert into #tbl(Code,Status) values ('L','Close');
	insert into #tbl(Code,Status) values ('R','Replease');
	select * from #tbl;
	DROP TABLE #tbl;
	*/
	select "ItemCode","ItemName" from OITM order by "ItemCode";
end;

