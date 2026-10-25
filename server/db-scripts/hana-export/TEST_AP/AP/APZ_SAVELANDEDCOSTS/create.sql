

CREATE PROCEDURE APZ_SaveLandedCosts 
AS
BEGIN
	declare countRow int=0;
 	
 	--Create temp table
 	create local temporary table #OALCLocal
 	(
 		"AlcCode" nvarchar(2) null, 
 		"AlcName" nvarchar(30) null,
 		"OhType" nvarchar(1) null, 
 		"AcctCode" nvarchar(15) null, 
 		"DataSource" nvarchar(1) null, 
 		"UserSign" nvarchar(5) null, 
 		"LaCAllcAcc" nvarchar(15) null,
 		"CostCateg" nvarchar(15) null
 	);	
	exec 'drop table #OALCLocal;';
	exec 'create local temporary table #OALCLocal as (select * from "#OALC");';
	
	--Assign value to countRow
	select (select 1 from OALC a inner join #OALCLocal b ON a."AlcCode" = b."AlcCode" where a."AlcName" <> b."AlcName" and a."AlcCode" in (select  "AlcCode" from IPF2))
	into countRow from DUMMY;

	--Check value has used
	if countRow > 0 then
  		select 1 as "ErrorCode" ,'Cannot update because being used' as "ErrorMessage" from DUMMY;
  		return;
  	end if;
	
	--Delete data OALC table
 	delete from  OALC;
 	
 	--Insert data to OALC
  	insert into  OALC("AlcCode","AlcName","OhType","AcctCode","DataSource","UserSign","LaCAllcAcc","CostCateg")
  	select "AlcCode","AlcName","OhType","AcctCode","DataSource","UserSign","LaCAllcAcc","CostCateg" from #OALCLocal;
END;




