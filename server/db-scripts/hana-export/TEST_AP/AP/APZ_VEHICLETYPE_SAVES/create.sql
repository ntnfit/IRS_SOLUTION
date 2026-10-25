
Create PROCEDURE APZ_VehicleType_Saves
AS 
BEGIN
Declare isName int=0;
create local temporary table #APZ_VHCTLocal as (select top 0 * from APZ_VHCT);
	execute immediate 'insert into #APZ_VHCTLocal select * from "#APZ_VHCT"';
	select ifnull((select count(1)  from #APZ_VHCTLocal a INNER JOIN #APZ_VHCTLocal b ON   a."Name" = b."Name" and a."Code" != b."Code"),0) into isName from dummy;
	if (ifnull(isName,0)=1)
	then

			select 123 ,'Vehicle Type is overlapping with an existing Vehicle Type' from dummy;
			return;

	end if;
	
--	UPDATE #APZ_VHCTLocal set "Code"=(SELECT  ROW_NUMBER() OVER( ORDER BY "Name") NumId FROM #APZ_VHCTLocal );
	
	delete from APZ_VHCT;
	insert into APZ_VHCT select * from #APZ_VHCTLocal;
	select 0, '' from dummy;
	drop table #APZ_VHCTLocal;
	
END;




