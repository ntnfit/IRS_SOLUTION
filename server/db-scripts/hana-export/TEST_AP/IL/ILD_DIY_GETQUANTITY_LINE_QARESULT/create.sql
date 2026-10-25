CREATE procedure ILD_DIY_GetQuantity_Line_QAResult
(
	in SSCC ntext
)
as
begin

	declare str ntext;
	select 'select sum( qty."Quantity") as "Quantity"
		 from OBTN master
		 left join OBTQ qty on master."AbsEntry" =   qty."MdAbsEntry"
		  where  "DistNumber" in  (' || '''' || replace(SSCC, ',' ,''',''')  || ''')'  
	into str
	from dummy;
	execute immediate  str;
end;

