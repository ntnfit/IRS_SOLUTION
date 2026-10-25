
CALL APZ_EXISTSPROC('APZ_D_FilterEmployees');

GO

CREATE PROCEDURE APZ_D_FilterEmployees (IN userId int DEFAULT -1,IN tempTable nvarchar(50) DEFAULT '')
AS
BEGIN
	declare userName nvarchar(50) = ifnull((select top 1 USER_CODE from OUSR where USERID = :userId), '');
	declare doFilter char(1) = ifnull((select "DoFilter" from CINF), 'N');
	declare doMngMth char(1) = ifnull((select "DoMngMth" from OADM), '');
	declare authMode char(1) = ifnull(nullif((select "AuthorizationMode" from APZ_D_OAPC),''), 'R');
	declare userEmpId int = ifnull((select "empID" from OHEM where "userId" = :userId), -1);
	
	create local temporary table #tbl as (select top 0 * from APZ_D_STR1);
	insert into #tbl select a.* from APZ_D_STR1 a left join APZ_D_OSTR b on a."RouteCode" = b."RouteCode" and b."Status" = 'Y';
	
	create local temporary table #tblVung as (select top 0 * from APZ_D_STR1);
	create local temporary table #temp1 as (select top 0 * from APZ_D_STR1);
	insert into #temp1 select * from #tbl where "SuperVisor" = :userEmpId or COMPARESPLIT(cast(:userEmpId AS nvarchar), "SalesAdmin", ';') = 1;
	create local temporary table #temp2 as (select top 0 * from APZ_D_STR1);
	create local temporary table #tempListChild as (select top 0 * from APZ_D_STR1);
	
	create local temporary table #temp3 ("SaleAdmin" int);
	tbl_temp1 = SELECT * FROM #temp1 where "SuperVisor" = :userEmpId;
	tbl_salesAdmin1 = MAP_MERGE(:tbl_temp1, Split(:tbl_temp1."SalesAdmin", ';'));
	insert into #temp3 select cast(ifnull(t0."DATA",'-1') as int) from :tbl_salesAdmin1 t0 where ifnull(t0."DATA",'') <> '' 
    	and t0."DATA" not in (select "SaleAdmin" from #temp3);
    	
	while exists (select top 1 1 from #temp1) do
		insert into #tblVung select * from #temp1;
		delete from #temp2;
		insert into #temp2 select T0.* from #tbl T0 inner join #temp1 T1 on t1."Id"=T0."FatherId" and t0."RouteCode" = T1."RouteCode";
		
		insert into #tempListChild select * from #temp2;
		delete from #temp1;
		insert into #temp1 select * from #temp2;
	end while;
	
	tbl_tempListChild = SELECT * FROM #tempListChild;
   		tbl_salesAdmin = MAP_MERGE(:tbl_tempListChild, Split(:tbl_tempListChild."SalesAdmin", ';'));
    	insert into #temp3 select cast(ifnull(t0."DATA",'-1') as int) from :tbl_salesAdmin t0 where ifnull(t0."DATA",'') <> '' 
    	and t0."DATA" not in (select "SaleAdmin" from #temp3);
	
	create local temporary table #tblRouteEmployee ("EmpId" int);
	insert into #tblRouteEmployee select * from
	(
		select b."EmpId" from #tblVung a inner join APZ_D_STR4 b on a."Id" = b."Id" and a."RouteCode" = b."RouteCode"
		UNION
		select distinct("SuperVisor") from #tempListChild where "SuperVisor" <> :userEmpId
		UNION
		select distinct("SaleAdmin") from #temp3
		union
		select :userEmpId from dummy
	);
	select * from #tblRouteEmployee order by "EmpId" asc;
	
	if :tempTable <> '' then
		declare query nvarchar(100) = N'insert into ' || :tempTable || ' select * from #tblRouteEmployee;';
		exec(:query);
	end if;
	
	drop table #tbl;
	drop table #tblVung;
	drop table #temp1;
	drop table #temp2;
	drop table #tblRouteEmployee;
	drop table #temp3;
	drop table #tempListChild;
END
