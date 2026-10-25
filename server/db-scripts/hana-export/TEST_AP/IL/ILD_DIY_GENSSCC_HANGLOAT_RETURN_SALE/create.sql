CREATE procedure ILD_DIY_GenSSCC_HangLoat_Return_Sale
(
	in sscc_List varchar(5000),
	out tab_result table
	(
		Row_Number int,
		SSCC_Old varchar(20),
		ProductionNum	varchar(9),
		Row_Num_Old  varchar(5),
		Row_Num_New_Temp  int,
		Row_Num_New varchar(20)
		
	)
)
as
Begin
	--declare sscc_List varchar (5000) ='30230000800011;30230000800013;30230000800014;30250000800014;';

	create local temporary table #temp
	(
		Row_Number int,
		SSCC_Old varchar(20),
		ProductionNum	varchar(9),
		Row_Num_Old  varchar(5),
		Row_Num_New_Temp  int,
		Row_Num_New varchar(20)
		
	);
	insert into #temp (Row_Number,SSCC_Old,ProductionNum ,Row_Num_Old)
	select  ROW_NUMBER() OVER(PARTITION BY  left("part",9) order by  left("part",9) asc ),
			"part",
			 left("part",9)as "ProductionNum",
			 right("part",5) 
			 
	from  "SPLITSTRING" (:sscc_List,';');
---
	delete from  #temp where ifnull(SSCC_Old,'')  = '';
--
-- 
	create local temporary table #Production_MaxNumer
	(
		ProductionNum varchar(9),
		SSCC_Max_Number int,
		Count_Row int
	);
	insert into #Production_MaxNumer(ProductionNum)
	select distinct  ProductionNum
	from #temp;
	
---
	update  a
	set a.SSCC_Max_Number = (select  ifnull( max(right("BatchNum",5)),0)  
								from   OIBT b
								where left(b."BatchNum",9) = a.ProductionNum)
	from #Production_MaxNumer a;
---
	update a
	set Count_row  =  (select count(*) from #temp b 
						
						where a.ProductionNum  =  b.ProductionNum
						group by b.ProductionNum
						)
	from #Production_MaxNumer a;
--- 
	 
	update a
	set Row_Num_New_Temp  =  (select   b.SSCC_Max_Number
						from #Production_MaxNumer b 
						  
						where  a.ProductionNum = b.ProductionNum
						) 
	from #temp a;
--
	update a
	set Row_Num_New_Temp  =  case when  ifnull(Row_Num_New_Temp,0) = 0 then 0 else  Row_Num_New_Temp + Row_Number end
	from #temp a;
---
	delete from #temp where  Row_Num_New_Temp = 0; 
--
	update a
	set Row_Num_New  = ProductionNum || REPLICATE(0, 5 - (  LENGTH(cast(Row_Num_New_Temp as nvarchar) )  )) || cast(Row_Num_New_Temp as nvarchar(5))
	from #temp a;
--
	--out tab_result table as (select * from  #temp);
	 
	--execute immediate 'create local  out  table #tab_result  as  (select  * from #temp);';
	tab_result = select  * from #temp;
	 
---
	drop table #temp;
	drop table #Production_MaxNumer;
	

end;

