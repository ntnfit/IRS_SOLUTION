CREATE procedure ILD_DIY_GetStartDate_UDO_QLyBaoTri
(
	in DocType nvarchar(50),
	in ITemCode nvarchar(50),
	in Floc nvarchar(50),
	in ActivityType nvarchar(50),
	in Template_Gen_Code nvarchar(50)
	 

)
as
Begin 
	/*declare DocType nvarchar(50) = 'Maintainance';
	declare ITemCode nvarchar(50) = '2110200028';
	declare Floc nvarchar(50) = 'Cum chuc nang 4';*/
---
	create local temporary table #temp
	(
		    StartDate date,
		    Interval smallint,
			EndDate date,
			NextDate date
			
	);
	insert into #temp 
	select "StartDate",
			"Interval",	
		    ADD_DAYS("StartDate",ifnull("Interval",0)) as  "EndDate",
			ADD_DAYS(ADD_DAYS("StartDate",ifnull("Interval",0)),ifnull("Interval",0))as  "NextDate"
	from 
	(
	 	select
			case when :DocType = 'Maintainance' then "U_StartDateMA" else  "U_StartDateHRE" end as "StartDate",
			case when :DocType = 'Maintainance' then ifnull("U_IntervalMA",0)  else ifnull("U_IntervalHRE",0)  end 
			as 	"Interval" 
	
			from "@TEMPLATE_AUTO_GENMA"  a
			where "Code"  = :Template_Gen_Code
			--where "U_ItemCodeF"  = :ITemCode and "U_Floc"  =  :Floc and "U_ActivityType"  = :ActivityType
	) as Nguon1;
---
	create local temporary table #taisan_floc_dataophieu
	(
		StartDate date,
		Interval	smallint,
		EndDate	date,
		NextDate	date
			
	);
	
	insert into #taisan_floc_dataophieu (StartDate,Interval,EndDate,NextDate)
	select "U_StartDate" ,"U_Interval","U_EndDate","U_NextDate"
	from "@MAINTENANCE_H" a
	where "U_StartDate" = (select max("U_StartDate") from "@MAINTENANCE_H" b
							 where a."U_ItemCodeF" = b."U_ItemCodeF"
							 		 and  b."U_Floc" = a."U_Floc" 
							 		 and  b."U_Template_Gen_Code" = a."U_Template_Gen_Code"
							 		 and  b."U_ActivityType" = a."U_ActivityType"
							 		 and  b."U_DocType" = :DocType
						   ) 
		
			and	cast (a."U_DocType" as nvarchar) = :DocType
			and  a."U_Floc" = :Floc
			and  a."U_Template_Gen_Code" = :Template_Gen_Code
			and  a."U_ActivityType" =:ActivityType ;
			
	
----- Update 
	Update a
	set a.StartDate  = ifnull(b.NextDate,ADD_DAYS (b.EndDate,ifnull(a.Interval,0))),
		a.EndDate  =   ADD_DAYS( ifnull(b.NextDate,ADD_DAYS (b.EndDate,ifnull(a.Interval,0))),ifnull(a.Interval,0)),  -- StartDate+ Interval
		a.NextDate  =  ADD_DAYS (ADD_DAYS( ifnull(b.NextDate,ADD_DAYS (b.EndDate,ifnull(a.Interval,0))),ifnull(a.Interval,0))
								,ifnull(a.Interval,0))
	from #temp a, #taisan_floc_dataophieu b;
----
	select * from #temp;
	drop table #temp;
	drop table #taisan_floc_dataophieu;
end;

