
CALL APZ_EXISTSPROC('APZ_LandedCosts_GetNameLandedCosts');

GO

create PROCEDURE APZ_LandedCosts_GetNameLandedCosts()
AS 
BEGIN
	create local temporary table #temp as (select top 0 *, cast(null as nvarchar(30)) "AlcName", cast(null as nvarchar(3)) "DocCur", cast(null as decimal(21,6)) "DocRate" from IPF2);
	insert into #temp("DocEntry","CostType","AlcCode","AlcName","OhType","InCustCalc")
	select 0,'F',"AlcCode","AlcName","OhType",'N' from OALC;
	
	select * from #temp;
	drop table #temp;
END