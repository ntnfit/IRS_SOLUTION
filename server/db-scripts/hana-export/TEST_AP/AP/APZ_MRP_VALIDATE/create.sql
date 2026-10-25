
CREATE PROCEDURE APZ_MRP_Validate
(
	in fdate date, tdate date,	-- Từ ngày, đến ngày
	MRPTYPE nvarchar(2),				-- MRP Type: 1,2,3,4
	Forecast nvarchar(1000)default'',		-- Forecast ID (1 hoặc nhiều)
	SalesOrderID nvarchar(1000) default '',		-- Sales Order (1 hoặc nhiều)
	PrdSchedule nvarchar(1000) default '',		-- Production Schedule
	Whs nvarchar(4000),
	MrpId int default 0,
	Project nvarchar(10) default '',
	periodType char(1) default ''
	
)
AS
BEGIN

	declare mssCode int;
	
	IF (Select count(1) from apz_WSH1 where "WorkDate" between :fdate and :tdate and Ifnull("Shift",'') <> '') <= 0 then
		
		Select 700012, 'You must be defined work schedule before run mrp' from dummy;
		return;
	
	END IF;

	IF (Select count(1) from Split(:Whs, ',') where trim(Data) not in (Select "WhsCode" from OWHS where Ifnull("Locked",'N') = 'N')) > 0 then
	
		Select 700013, 'You must choose warehouse belong branch' from dummy;
		return;

	END IF;

	create local temporary table #TempMrpType
		(
			MrpType nvarchar(2) null,
			Forecast int null,
			SalesOrder int null,
			Schedule int null,
			MessageCode int null,
			MessageName nvarchar(250) null
		);
	
	Insert INTO #TempMrpType values ('1',2,0,0, 0,			'');
	Insert INTO #TempMrpType values ('1',2,1,2, 700014,		'Input Mrp 1 could not contain salesOrder');
	Insert INTO #TempMrpType values ('1',2,2,1, 700015,		'Input Mrp 1 could not contain Schedule');
	Insert INTO #TempMrpType values ('2',2,2,0, 0,			'');
	Insert INTO #TempMrpType values ('2',2,2,1, 700016,		'Input Mrp 2 could not contain Schedule');
	Insert INTO #TempMrpType values ('3',0,0,2, 0,			'');
	Insert INTO #TempMrpType values ('3',1,2,2, 700017,		'Input Mrp 3 could not contain forecast');
	Insert INTO #TempMrpType values ('3',2,1,2, 700018,		'Input Mrp 3 could not contain sales order');
	Insert INTO #TempMrpType values ('4',0,2,0, 0,			'');
	Insert INTO #TempMrpType values ('4',1,2,2, 700019,		'Input Mrp 4 could not contain forecast');
	Insert INTO #TempMrpType values ('4',2,2,1, 700020,		'Input Mrp 4 could not contain schedule');

	select (select top 1 MessageCode from #TempMrpType where MrpType = :MRPTYPE and 
															((Forecast = 0 and Ifnull(:Forecast,'') = '') or (Forecast = 1 and Ifnull(:Forecast,'') <> '') or Forecast = 2) and 
															((Schedule = 0 and Ifnull(:PrdSchedule,'') = '') or (Schedule = 1 and Ifnull(:PrdSchedule,'') <> '') or Schedule = 2) and
															((SalesOrder = 0 and Ifnull(:SalesOrderID,'') = '') or (SalesOrder = 1 and Ifnull(:SalesOrderID,'') <> '') or SalesOrder = 2)) into mssCode from dummy;
	
	IF :mssCode <> 0 then

		Select :mssCode, (select MessageName from #TempMrpType where MrpType = :MRPTYPE and MessageCode = :mssCode) from dummy;
		return;

	END if;


	if ifnull(:SalesOrderID,'') <> '' and (Select count(1) from Split(:SalesOrderID,',') where trim(Data) in (Select cast("DocNum" as nvarchar) from ORDR)) <= 0 then
	
		Select 700021, 'Could not find sale order' from dummy;
		return;
			
	END IF;

	if ifnull(:Forecast,'') <> '' and (Select count(1) from Split(:Forecast,',') where trim(Data) in (Select cast("Code" as nvarchar) from APZ_OFCT)) <= 0 then

		Select 700022, 'Could not find forecast' from dummy;
		return;

	END IF;

	if ifnull(:PrdSchedule,'') <> '' and (Select count(1) from Split(:PrdSchedule,',') where trim(Data) in (Select cast("PSCode" as nvarchar) from apz_OPRS where "Type" = 'O' or "Type" = 'T')) <= 0 then

		Select 700023, 'Could not find Schedule' from dummy;
		return;

	END IF;

	SELECT 0, '' from dummy;
	
END;




