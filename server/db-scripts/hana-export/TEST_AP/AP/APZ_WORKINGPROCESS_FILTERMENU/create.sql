

CREATE PROCEDURE APZ_WorkingProcess_FilterMenu()
AS
BEGIN
	
	declare status char(1) = '';
	declare requestType char(1) = '';
	declare userId int = -1;
	declare maxId int = -1;
	declare queryCond varchar(4000) = '';
	declare queryList varchar(4000) = '';
	declare docExists int = -1;
	declare rowCount int = -1;

	create local temporary table #dtDataLocal
	(
		"WddCode" int, 
		"WtmCode" int,
		"WtmName" nvarchar(100),
		"CardCode" nvarchar(15),
		"CardName" nvarchar(100),
		"DocNo" varchar(50),
		"ObjType" int,
		"DocType" nvarchar(100),
		"DocDate" timestamp,
		"DocDueDate" varchar(80),
		"DocTotal" decimal(21,6),
		"DocCur" varchar(10),
		"IsDraft" char(1),
		"UserId" int,
		"UserCode" varchar(10),
		"UserName" nvarchar(50),
		"UpdateDate" varchar(80),
		"UpdateTime" int,
		"Status" char(1),
		"Remarks" nvarchar(100),
		"RequestType" char(1),
		"ItemCheck" char(1)
	);
	
	exec 'drop table #dtDataLocal;';
	exec 'create local temporary table #dtDataLocal as (select * from "#dtSource");';
	
	create local temporary table #dtCondsLocal("UserId" int null, "Status" char(1) null, "RequestType" char(1) null);
	exec 'drop table #dtCondsLocal;';
	exec 'create local temporary table #dtCondsLocal as (select * from "#dtConds");';
	
	create local temporary table #dtListLocal("Name" nvarchar(4000) null, "Type" char(1) null);
	exec 'drop table #dtListLocal;';
	exec 'create local temporary table #dtListLocal as (select * from "#dtList");';

	select (select "Status" from #dtCondsLocal) into status from dummy;
	select (select "RequestType" from #dtCondsLocal) into requestType from dummy;
	select (select "UserId" from #dtCondsLocal) into userId from dummy;
	
	if :requestType = 'M'
	then
		queryCond := ' and "RequestType" = ''M''';
	elseif :requestType = 'P'
	then
		queryCond := ' and "RequestType" = ''P''';
	end if;

	select (select count(1) from #dtListLocal where "Type" = 'P') into rowCount from dummy;
	if :rowCount > 0
	then
		queryList := ' and "WtmName" in (select "Name" from #dtListLocal where "Type" = ''P'')';
	end if;
	
	select (select count(1) from #dtListLocal where "Type" = 'D') into rowCount from dummy;
	if :rowCount > 0
	then
		queryList := :queryList || ' and "DocType" in (select "Name" from #dtListLocal where "Type" = ''D'')';
	end if;
	
	if :status = 'A'
	then
		execute immediate 'select * from #dtDataLocal where "Status" <> ''''' || :queryCond || :queryList || ';';
	elseif :status = 'Y'
	then
		execute immediate 'select * from #dtDataLocal where "Status" = ''Y'' and "IsDraft" <> ''N''' || :queryCond || :queryList || ' order by "UpdateDate" desc;';
	elseif :status = 'N'
	then
		execute immediate 'select * from #dtDataLocal where "Status" = ''N'' and "IsDraft" <> ''N''' || :queryCond || :queryList || 'order by "UpdateDate" desc;';
	elseif :status = 'W'
	then
		execute immediate 'select * from #dtDataLocal where "Status" = ''W'' and "IsDraft" <> ''N''' || :queryCond || :queryList || ' order by "UpdateDate" desc;';
	elseif :status = 'C'
	then
		execute immediate 'select * from #dtDataLocal where "IsDraft" = ''N''' || :queryCond || :queryList || ' order by "DocNo";';
	end if;
			
END




