CREATE procedure ILD_DIY_GetSSCC_Line_QAResult
(
	in SSCC_header nvarchar(5000)  default 'Toan',
	in SSCC_1 nvarchar(5000),
	in SSCC_2 nvarchar(5000),
	in SSCC_3 nvarchar(5000)
)
as
begin
	declare SSCC_123_first ntext ;
	select  SSCC_1 ||',' || SSCC_2 ||',' ||SSCC_3
	into SSCC_123_first
	from dummy;
	-- SSCC_header
	Create local temporary table #tblSSCC_header
	(
		part nvarchar(50)
	);
	
	insert into #tblSSCC_header
	select "part"
	from "SPLITSTRING" (SSCC_header,',') order by "part";
	
	
	-- SSCC_ 1
	Create local temporary table #tblSSCC_123
	(
		part nvarchar(50)
	);
	
	insert into #tblSSCC_123
	select "part"
	from "SPLITSTRING" (SSCC_123_first,',') order by "part";	
	
	
    select STRING_AGG(a.part,',') as  "SSCC", SSCC_123_first as "SSCC_DaChon"
    from #tblSSCC_header a
    where a.part not in (select part from #tblSSCC_123 );

 
 
 	drop table #tblSSCC_123;
	drop table #tblSSCC_header;
	 

end;

