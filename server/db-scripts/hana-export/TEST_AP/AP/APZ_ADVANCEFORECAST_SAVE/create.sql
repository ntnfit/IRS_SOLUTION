
CREATE PROCEDURE APZ_AdvanceForecast_Save(in TypeAdd char(1))
AS
BEGIN

	declare id int = 0;
	declare checkCount int = 0;
	
	create local temporary table #APZDocData as (select top 0 * from APZ_OFCT);
	exec 'drop table #APZDocData;';
	exec 'create local temporary table #APZDocData as (select * from "#APZDoc");';
	
	create local temporary table #APZLineData as (select top 0 * from APZ_FCT1);
	exec 'drop table #APZLineData;';
	exec 'create local temporary table #APZLineData as (select * from "#APZLine");';
	
	select ifnull((select top 1 1 from #APZLineData where ifnull("ItemCode",'') <> ''),0) into checkCount from DUMMY;
	IF :checkCount = 0
	THEN
		select 13023, 'Could not save forecast. Item data can not be null' from DUMMY;
		return;
	end IF;

	select ifnull((select top 1 1 from #APZLineData a LEFT JOIN OITM b ON TRIM(' ' From a."ItemCode") = TRIM(' ' From b."ItemCode") where TRIM(' ' From ifnull(a."ItemCode",'')) <> '' and b."ItemCode" is null), 0) into checkCount from DUMMY;
	IF :checkCount = 1
	THEN
		select 13024, 'Could not save forecast. Item does not exist' from DUMMY;
		return;
	end IF;

	select ifnull((select top 1 1 from #APZLineData a LEFT JOIN OWHS b ON a."WhsCode" = b."WhsCode" where ifnull(a."WhsCode",'') <> '' and b."WhsCode" is null), 0) into checkCount from DUMMY;
	IF :checkCount = 1
	THEN
		select 13025, 'Could not save forecast. Warehouse does not exist' from DUMMY;
		return;
	end IF;

	select ifnull((select 1 from #APZLineData where ifnull("WhsCode", '') = ''), 0) into checkCount from DUMMY;
	if :checkCount = 1
	Then
		select 13011, 'Could not save forecast. WhsCode could not be null' from DUMMY;
		return;
	END IF;

	IF :typeAdd = 'A'
	THEN
		Select ifnull((select max("Code") from APZ_OFCT), 0) + 1 into id from DUMMY;
		Update #APZDocData set "Code" = :id;
		Update #APZLineData set "Code"  = :id;
	Else
		Select (select top 1 "Code" from #APZDocData) into id from DUMMY;
		Select ifnull((select top 1 1 from APZ_OMRP where CompareSplit(:id, "FCCode", ',') = 1 and "Status" <> 'D'),0) into checkCount from DUMMY;
		
		IF :checkCount = 1
		THEN
			select 13002, 'Could not update forecast. Forecast was generated mrp' from DUMMY;
			return;
		END IF;
		
		update #APZLineData set "Code"  = :id;
		Delete from APZ_FCT1 where "Code" = :id;
		Delete From APZ_OFCT where "Code" = :id;
	End IF;

	Update #APZDocData set "CreateDate" = current_date;
	Update #APZDocData set "OriginType" = 'E' where ifnull("OriginType",'') = '';
	
	Update #APZLineData Set "ItemCode" = TRIM(' ' From "ItemCode");

	Insert Into APZ_OFCT select * from #APZDocData;
	Insert Into APZ_FCT1 select * from #APZLineData;

	Update ONNM SET "AutoKey" = Ifnull((select max("Code") from APZ_OFCT),0) + 1 Where "ObjectCode" = '8000005';

	select 0, '' from DUMMY;
	--select @sql,@columsJoin,@columsGroupSumQty,@columsPivotQty,@columsInsert
END


