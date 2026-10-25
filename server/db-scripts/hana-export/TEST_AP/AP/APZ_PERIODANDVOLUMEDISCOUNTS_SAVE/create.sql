
Create procedure APZ_PeriodAndVolumeDiscounts_Save()
As 
Begin
	declare cardCode nvarchar(50);

	create local temporary table #osppdata as (select top 0 b.* from (select 1 A from DUMMY) a left join OSPP b ON 1=2);
	exec 'insert into #osppdata select * from #OSPP';
	
	create local temporary table #spp1data as (select top 0 b.* from (select 1 A from DUMMY) a left join SPP1 b ON 1=2);
	exec 'insert into #spp1data select * from #SPP1';
	
	create local temporary table #spp2data as (select top 0 b.* from (select 1 A from DUMMY) a left join SPP2 b ON 1=2);
	exec 'insert into #spp2data select * from #SPP2';
	
	delete from #osppdata where ifnull("ItemCode", '') = '' or ifnull("CardCode", '') = '' or ifnull("ListNum", 0) = 0;
	delete from #spp1data where ifnull("ItemCode", '') = '' or ifnull("CardCode", '') = '' or ifnull("ListNum", 0) = 0 or (ifnull("FromDate", '') = '' and ifnull("ToDate", '') = '') or ifnull("Discount", 0) = 0;
	delete from #spp2data where ifnull("ItemCode", '') = '' or ifnull("CardCode", '') = '' or ifnull("Amount",0) <= 0 or ifnull("Discount", 0) = 0;
	
	delete from #spp1data a where not exists(select 1 from #osppdata b where a."ItemCode" = b."ItemCode" and a."CardCode" = b."CardCode") ;
	delete from #spp2data a where not exists(select 1 from #spp1data b where a."ItemCode" = b."ItemCode" and a."CardCode" = b."CardCode" and a."SPP1LNum" = b."LINENUM");
	
	select top 1 "CardCode" into cardCode from #osppdata;
	
	delete from OSPP where "CardCode" = :cardCode;
	delete from SPP1 where "CardCode" = :cardCode;
	delete from SPP2 where "CardCode" = :cardCode;
	
	insert into OSPP select * from #osppdata;
	insert into SPP1 select * from #spp1data;
	insert into SPP2 select * from #spp2data;

End;




