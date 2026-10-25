
CREATE  PROCEDURE APZ_D_Promotion_Register
(
	UserSign int DEFAULT -1
)
AS
BEGIN

	declare currentDate timestamp;
	declare currentTime int;
	declare maxLogInstance int;
	
	currentDate = current_date;
	currenttime = CAST( REPLACE (CAST( cast(current_time as VARCHAR (8)) AS VARCHAR (5)), ':', '') AS INT);
	
	create local temporary table #tempData ("CardCode" nvarchar(50), "Code" nvarchar(50), "PakageNo" nvarchar(50), "StatusCode" nvarchar(1), "Quantity" numeric(19,6), "RegisterRemarks" nvarchar(254));
	exec 'drop table #tempData';
	exec 'create local temporary table #tempData as (select * from "#temp")';
	
	create local temporary table #recent as(
		select t0.*  from APZ_D_PMO3 t0 inner join #tempdata t1 on t0."CardCode"=t1."CardCode" and t0."Code"=t1."Code" 
			and (exists(select 1 from APZ_OPPR where "Code" =t0."Code" and ifnull("PakageSplit", 'N') = 'Y') and t0."GroupCode" = t1."PakageNo" or 
			(exists(select 1 from APZ_OPPR where "Code" = t0."Code" and "PakageSplit" = 'N') and t0."GroupCode" is not null))
	);
	-- lưu lại các lần update data
	select ifnull((select max("LogIntance") from APZ_D_PMO3_Log),0) into maxLogInstance from DUMMY;
	
	create local temporary table #logData as(select t0.*,
			case when IFNULL(t0."Quantity",0) <> IFNULL(t1."Quantity",0) then 'quantity' 
			when IFNULL(t0."StatusCode",'') = 'Y' and IFNULL(t1."StatusCode",'') = 'N' then 'unregister'
			when IFNULL(t0."StatusCode",'') <> IFNULL(t1."StatusCode",'') then 'status' else null end as "UpdateType"
		from APZ_D_PMO3 t0 inner join #tempData t1 on t0."CardCode"=t1."CardCode" and t0."Code"=t1."Code"
			and (exists(select 1 from APZ_OPPR where "Code" =t0."Code" and "PakageSplit" = 'Y') and t0."GroupCode" = t1."PakageNo" or 
			(exists(select 1 from APZ_OPPR where "Code" = t0."Code" and "PakageSplit" = 'N') and t0."GroupCode" is not null))
			where IFNULL(t0."StatusCode",'') <> IFNULL(t1."StatusCode",'') 
			or IFNULL(t0."Quantity",0) <> IFNULL(t1."Quantity",0)
			or IFNULL(t0."RegisterRemarks",'') <> IFNULL(t1."RegisterRemarks",''));
	
	update #logData set "LogIntance" = :maxLogInstance+1;
	
	--lưu thêm các lần uncheck
	insert into APZ_D_PMO3_Log("Code","CardCode","GroupCode","Quantity","RegisterRemarks","StatusCode","CreateDate","CreateTime","UpdateDate","UpdateTime","UserSign", "UserSign2","LogIntance","UpdateType")
		select t0."Code",T0."CardCode",ifnull(T0."PakageNo",''),t0."Quantity",t0."RegisterRemarks",t0."StatusCode",ifnull(t1."CreateDate",:currentDate),ifnull(t1."CreateTime",:currentTime),
			case when t1."Code" is not null then :currentDate else null end,case when t1."Code" is not null then :currentTime else null end ,
			ifnull(t1."UserSign",:UserSign),case when t1."Code" is not null then :UserSign else null end, "LogIntance","UpdateType"
			from #tempdata t0 inner join #logData t1 on t0."CardCode"=t1."CardCode" and t0."Code"=t1."Code" 
			and (exists(select 1 from APZ_OPPR where "Code" =t0."Code" and "PakageSplit" = 'Y') and t0."PakageNo" = t1."GroupCode" or 
			(exists(select 1 from APZ_OPPR where "Code" = t0."Code" and "PakageSplit" = 'N') and t0."PakageNo" is not null));

	-- insert bảng log dữ liệu khởi tạo (dùng left join A và B vế B null để tìm ra các bản ghi chưa có trong bảng APZ_D_PMO3)
	insert into APZ_D_PMO3_log("Code","CardCode","GroupCode","Quantity","RegisterRemarks","StatusCode","CreateDate","CreateTime","UpdateDate","UpdateTime","UserSign", "UserSign2","UpdateType","LogIntance")
		select t0."Code",T0."CardCode",ifnull(T0."PakageNo",''),t0."Quantity",t0."RegisterRemarks",t0."StatusCode",ifnull(t1."CreateDate",:currentDate),ifnull(t1."CreateTime",:currentTime),
			case when t1."Code" is not null then :currentDate else null end,case when t1."Code" is not null then :currentTime else null end,
			IFNULL(t1."UserSign",:UserSign),case when t1."Code" is not null then :UserSign else null end, 'init',(:maxLogInstance+1)
		from #tempData t0 left join #recent t1 on t0."CardCode"=t1."CardCode" and t0."Code"=t1."Code" 
				and (exists(select 1 from APZ_OPPR where "Code" =t0."Code" and "PakageSplit" = 'Y') and t0."PakageNo" = t1."GroupCode" or 
				(exists(select 1 from APZ_OPPR where "Code" = t0."Code" and "PakageSplit" = 'N') and t0."PakageNo" is not null))
			left join APZ_D_PMO3 t2 on t0."CardCode" = t2."CardCode" and t0."Code"=t2."Code" 
			and (exists(select 1 from APZ_OPPR where "Code" =t0."Code" and "PakageSplit" = 'Y') and t0."PakageNo" = t2."GroupCode" or 
			(exists(select 1 from APZ_OPPR where "Code" = t0."Code" and "PakageSplit" = 'N') and t0."PakageNo" is not null))
		where t0."StatusCode" = 'Y' and t2."Code" is null;
	
	-- update dữ liệu chính
	
	delete from APZ_D_PMO3 t0 where exists(select 1 from #tempdata t1 where t0."CardCode"=t1."CardCode" and t0."Code"=t1."Code" 
		and (exists(select 1 from APZ_OPPR where "Code" =t0."Code" and "PakageSplit" = 'Y') and t0."GroupCode" = t1."PakageNo" or 
			(exists(select 1 from APZ_OPPR where "Code" = t0."Code" and "PakageSplit" = 'N') and t0."GroupCode" is not null)));

	insert into APZ_D_PMO3("Code","CardCode","GroupCode","Quantity","RegisterRemarks","StatusCode","CreateDate","CreateTime","UpdateDate","UpdateTime","UserSign", "UserSign2")
		select t0."Code",T0."CardCode",ifnull(T0."PakageNo",''),t0."Quantity",t0."RegisterRemarks",t0."StatusCode",
			ifnull(t1."CreateDate",:currentDate),ifnull(t1."CreateTime",:currentTime),
			case when t1."Code" is not null then :currentDate else null end,case when t1."Code" is not null then :currentTime else null end ,
			ifnull(t1."UserSign",:UserSign),case when t1."Code" is not null then :UserSign else null end
		from #tempdata t0 left join #recent t1 on t0."CardCode"=t1."CardCode" and t0."Code"=t1."Code" 
			and (exists(select 1 from APZ_OPPR where "Code" =t0."Code" and "PakageSplit" = 'Y') and t0."PakageNo" = t1."GroupCode" or 
			(exists(select 1 from APZ_OPPR where "Code" = t0."Code" and "PakageSplit" = 'N') and t0."PakageNo" is not null)) 
			where t0."StatusCode" = 'Y';

END




