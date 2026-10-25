
create procedure APZ_ProjectManager_Save(in typeSave char(1))
as
begin
	Declare Id int =0;
	Declare MasterEntry int =0;
	Declare baseProject int =0;
	declare taskCode nvarchar(50)='';
	declare isExstM smallint;
	declare isExstta smallint;
	declare isExstcode smallint;
	
	declare isExstco smallint;
	declare isExsttype smallint;
		
	declare isExststatus smallint;
	declare isExsttbTask smallint;
	declare isExsttCode smallint;
	
	declare isExstst smallint;
	declare isExstAt smallint;
	declare isBaseLine smallint;
	declare isExstStDate smallint;
	declare ocrCode nvarchar(50)='';
	
	declare isExstStDate2 smallint;
	declare isExstStDate3 smallint;
	declare isExstStDate4 smallint;
	declare isExstStDate5 smallint;
	
	create local temporary table #APZ_OPRGDATA as (select top 0 * from APZ_OPRG);
	exec 'drop table #APZ_OPRGDATA;';
	exec 'create local temporary table #APZ_OPRGDATA as (select * from "#APZ_OPRG");';

	select MIN("MasterEntry") into MasterEntry from #APZ_OPRGDATA;
	select MIN("BaseProject") into baseProject from #APZ_OPRGDATA;
	if :typeSave='U' then
			
			declare userCode nvarchar(25) = ifnull((select "UserCodeSign2" from #APZ_OPRGDATA), '');
			select min("AbsEntry") into Id from #APZ_OPRGDATA;
			call apz_func_OwnerOverShip(:userCode, '1470000132', OwnerView);
			if not exists(select 1 from APZ_OPRG where "AbsEntry"=:id and (ifnull("Owner",0)=0 or "UserCodeSign"=:userCode or "Owner" in (select distinct "OwnerId" from :OwnerView where "Authorize"='F'))) then
				select -1, 'User does not have permission to update project' from dummy;
				return;
			end if;
			
			delete from  APZ_PRG1 where "AbsEntry" = :Id;
			delete from APZ_PRG2 where "AbsEntry" = :Id;
			delete from  APZ_PRG3 where "AbsEntry" = :Id;
			delete from APZ_PRG4 where "AbsEntry" = :Id;
			delete from APZ_PRG6 where "AbsEntry" = :Id;
			delete from APZ_OPRG where "AbsEntry"= :Id;
			
	else 
			select IFNULL((select max("AbsEntry") from APZ_OPRG),0) + 1 into Id FROM DUMMY;
			update #APZ_OPRGDATA set "AbsEntry" = :Id;
	end if;

	create local temporary table #APZ_PRG1DATA as (select top 0 * from APZ_PRG1);
	exec 'drop table #APZ_PRG1DATA;';
	exec 'create local temporary table #APZ_PRG1DATA as (select * from "#APZ_PRG1");';

	create local temporary table #APZ_PRG2DATA as (select top 0 * from APZ_PRG2);
	exec 'drop table #APZ_PRG2DATA;';
	exec 'create local temporary table #APZ_PRG2DATA as (select * from "#APZ_PRG2");';

	create local temporary table #APZ_PRG3DATA as (select top 0 * from APZ_PRG3);
	exec 'drop table #APZ_PRG3DATA;';
	exec 'create local temporary table #APZ_PRG3DATA as (select * from "#APZ_PRG3");';

	create local temporary table #APZ_PRG4DATA as (select top 0 y.* from (select 1 DocEntry from DUMMY) x left join APZ_PRG4 y ON 1=1);
	exec 'drop table #APZ_PRG4DATA;';
	exec 'create local temporary table #APZ_PRG4DATA as (select * from "#APZ_PRG4");';

	create local temporary table #APZ_PRG6DATA as (select top 0 * from APZ_PRG6);
	exec 'drop table #APZ_PRG6DATA;';
	exec 'create local temporary table #APZ_PRG6DATA as (select * from "#APZ_PRG6");';

	create local temporary table #APZ_OPRG1DATA as (select top 0 * from APZ_OPRG);
	exec 'drop table #APZ_OPRG1DATA;';
	exec 'create local temporary table #APZ_OPRG1DATA as (select * from "#APZ_OPRG1");';
	
	if ifnull(:MasterEntry, 0) <> 0 then
		select IFNULL((select top 1 1 from APZ_OPRG  where "AbsEntry"= :MasterEntry),0) into isExstM from DUMMY;
		if :isExstM = 0 then
			select 120 ,'Master Entry =' || :MasterEntry || ' not exist in table Project Manager' from dummy;
			return;
		end if;
	end if;
	
	--select IFNULL((select top 1 1 from #APZ_PRG4DATA a left join #APZ_PRG1DATA b ON  a."TaskCode" = b."TaskCode" where ifnull(a."TaskCode",'') != '' and b."LineNum" is null),0) into isExstta from DUMMY;
	--if :isExstta = 1
	--then
					
	--	select top 1 "TaskCode" into taskCode from #APZ_PRG4DATA;
	--	select 121,' Wbs code ='|| ifnull(:taskCode,'') || ' not exist in Work Breakdown Structure' from dummy;
	--end if;
	
	select IFNULL((select top 1 1 from #APZ_OPRGDATA a INNER JOIN APZ_OPRG b ON a."Code"=b."Code"),0) into isExstcode from DUMMY;
	if :isExstcode=1
	then
	
		select 122 ,'Project Code  is overlapping with an existing Project' from dummy;
		return;
	
	END if;
	
	select IFNULL((select top 1 1 from #APZ_OPRGDATA  where ifnull("Code",'')=''),0) into isExstco from DUMMY;
	if :isExstco=1
	then
	
		select 123 ,'Project Code  is missing' from dummy;
		return;
	
	end if;
	
	select IFNULL((select top 1 1 from #APZ_OPRGDATA  where ifnull("ProjectType",'')=''),0) into isExsttype from DUMMY;
	if :isExsttype=1
	then
	
		select 124 ,'Project Type  is missing' from dummy;
		return;
	
	end if;
	
	select IFNULL((select top 1 1 from #APZ_OPRGDATA  where ifnull("PrgStatus",'')=''), 0) into isExststatus from DUMMY;
	if :isExststatus=1
	then
	
		select 125 ,'Project Status  is missing' from dummy;
		return;
	
	end if;
	
	select IFNULL((select top 1 1  from #APZ_PRG1DATA),0) into isExsttbTask from DUMMY;
	if :isExsttbTask=0
	then
			select 127,'Task Not enough data' from dummy;
			return;
	end if;
	
	select IFNULL((select top 1 1 from #APZ_PRG1DATA  where ifnull("TaskCode",'')=''),0) into isExsttCode from DUMMY;
	if :isExsttCode=1
	then
			select 128,'Task Code  is missing' from dummy;
			return;
	end if;
	
	select IFNULL((select top 1 1 from #APZ_OPRGDATA a where a."StartDate" > a."DueDate"),0) into isExstst FROM DUMMY;
	if :isExstst=1
	then
	
			select 129 ,'Start Date cannot be later than  Due Date' from dummy;
			return;
	
	END if;
	
	select IFNULL((select top 1 1 from #APZ_OPRGDATA a where a."StartDate" > a."ActualDate"),0) into isExstAt FROM DUMMY;
	if :isExstAt=1
	then
	
			select 130 ,'Start Date cannot be later than Actual Date'from dummy;
			return;
	
	END if;
	
	select IFNULL((select top 1 1 from #APZ_OPRGDATA a where a."BaseLine"='Y'),0) into isBaseLine from DUMMY;
	if :isBaseLine=1
	then
			Update APZ_OPRG set "BaseLineActive"='N' where "BaseProject"= :baseProject;
	end if;
	--kiểm tra startdate trên header có nằm ngoài khoảng from và to trong costcenter không(kiểm tra ở tab wbs và item) 
	select IFNULL(( select top 1 1 from #APZ_OPRGDATA  a left join #APZ_PRG1DATA b on b."AbsEntry"=a."AbsEntry"
	left join OPRC c on c."PrcCode"=b."OcrCode" where a."StartDate" not BETWEEN c."ValidFrom" AND c."ValidTo" ),0) into isExstStDate from  DUMMY;
	if :isExstStDate=1
		then
		select IFNULL ((select top 1 b."OcrCode" from #APZ_OPRGDATA  a left join #APZ_PRG1DATA b on b."AbsEntry"=a."AbsEntry"
		left join OPRC c on c."PrcCode"=b."OcrCode" where a."StartDate" not BETWEEN c."ValidFrom" AND c."ValidTo" ),'') into ocrCode from dummy;
		select 135,'Start Date nằm ngoài khoảng Valid của Cost Center '|| :ocrCode from dummy;
		return;
		END if;
	
	select IFNULL(( select top 1 1 from #APZ_OPRGDATA  a left join #APZ_PRG1DATA b on b."AbsEntry"=a."AbsEntry"
	left join OPRC c on c."PrcCode"=b."OcrCode2" where a."StartDate" not BETWEEN c."ValidFrom" AND c."ValidTo" ),0) into isExstStDate from  DUMMY;
	if :isExstStDate=1
		then
		select IFNULL ((select top 1 b."OcrCode2" from #APZ_OPRGDATA  a left join #APZ_PRG1DATA b on b."AbsEntry"=a."AbsEntry"
		left join OPRC c on c."PrcCode"=b."OcrCode2" where a."StartDate" not BETWEEN c."ValidFrom" AND c."ValidTo" ),'') into ocrCode from dummy;
		select 135,'Start Date nằm ngoài khoảng Valid của Cost Center '|| :ocrCode from dummy;
		return;
		END if;	
	select IFNULL(( select top 1 1 from #APZ_OPRGDATA  a left join #APZ_PRG1DATA b on b."AbsEntry"=a."AbsEntry"
	left join OPRC c on c."PrcCode"=b."OcrCode3" where a."StartDate" not BETWEEN c."ValidFrom" AND c."ValidTo" ),0) into isExstStDate from  DUMMY;
	if :isExstStDate=1
		then
		select IFNULL ((select top 1 b."OcrCode3" from #APZ_OPRGDATA  a left join #APZ_PRG1DATA b on b."AbsEntry"=a."AbsEntry"
		left join OPRC c on c."PrcCode"=b."OcrCode3" where a."StartDate" not BETWEEN c."ValidFrom" AND c."ValidTo" ),'') into ocrCode from dummy;
		select 135,'Start Date nằm ngoài khoảng Valid của Cost Center '|| :ocrCode from dummy;
		return;
		END if;	
		
	select IFNULL(( select top 1 1 from #APZ_OPRGDATA  a left join #APZ_PRG1DATA b on b."AbsEntry"=a."AbsEntry"
	left join OPRC c on c."PrcCode"=b."OcrCode4" where a."StartDate" not BETWEEN c."ValidFrom" AND c."ValidTo" ),0) into isExstStDate from  DUMMY;
	if :isExstStDate=1
		then
		select IFNULL ((select top 1 b."OcrCode4" from #APZ_OPRGDATA  a left join #APZ_PRG1DATA b on b."AbsEntry"=a."AbsEntry"
		left join OPRC c on c."PrcCode"=b."OcrCode4" where a."StartDate" not BETWEEN c."ValidFrom" AND c."ValidTo" ),'') into ocrCode from dummy;
		select 135,'Start Date nằm ngoài khoảng Valid của Cost Center '|| :ocrCode from dummy;
		return;
		END if;	
		
	select IFNULL(( select top 1 1 from #APZ_OPRGDATA  a left join #APZ_PRG1DATA b on b."AbsEntry"=a."AbsEntry"
	left join OPRC c on c."PrcCode"=b."OcrCode5" where a."StartDate" not BETWEEN c."ValidFrom" AND c."ValidTo" ),0) into isExstStDate from  DUMMY;
	if :isExstStDate=1
		then
		select IFNULL ((select top 1 b."OcrCode5" from #APZ_OPRGDATA  a left join #APZ_PRG1DATA b on b."AbsEntry"=a."AbsEntry"
		left join OPRC c on c."PrcCode"=b."OcrCode5" where a."StartDate" not BETWEEN c."ValidFrom" AND c."ValidTo" ),'') into ocrCode from dummy;
		select 135,'Start Date nằm ngoài khoảng Valid của Cost Center '|| :ocrCode from dummy;
		return;
		END if;	
	--check cho tab item
	select IFNULL(( select top 1 1 from #APZ_OPRGDATA  a left join #APZ_PRG4DATA b on b."AbsEntry"=a."AbsEntry"
	left join OPRC c on c."PrcCode"=b."OcrCode" where a."StartDate" not BETWEEN c."ValidFrom" AND c."ValidTo" ),0) into isExstStDate from  DUMMY;
	if :isExstStDate=1
		then
		select IFNULL ((select top 1 b."OcrCode" from #APZ_OPRGDATA  a left join #APZ_PRG4DATA b on b."AbsEntry"=a."AbsEntry"
		left join OPRC c on c."PrcCode"=b."OcrCode" where a."StartDate" not BETWEEN c."ValidFrom" AND c."ValidTo" ),'') into ocrCode from dummy;
		select 135,'Start Date nằm ngoài khoảng Valid của Cost Center '|| :ocrCode from dummy;
		return;
		END if;	
	
	select IFNULL(( select top 1 1 from #APZ_OPRGDATA  a left join #APZ_PRG4DATA b on b."AbsEntry"=a."AbsEntry"
	left join OPRC c on c."PrcCode"=b."OcrCode2" where a."StartDate" not BETWEEN c."ValidFrom" AND c."ValidTo" ),0) into isExstStDate2 from  DUMMY;
	if :isExstStDate2=1
		then
		select IFNULL ((select top 1 b."OcrCode2" from #APZ_OPRGDATA  a left join #APZ_PRG4DATA b on b."AbsEntry"=a."AbsEntry"
		left join OPRC c on c."PrcCode"=b."OcrCode2" where a."StartDate" not BETWEEN c."ValidFrom" AND c."ValidTo" ),'') into ocrCode from dummy;
		select 135,'Start Date nằm ngoài khoảng Valid của Cost Center '|| :ocrCode from dummy;
		return;
		END if;		
	select IFNULL(( select top 1 1 from #APZ_OPRGDATA  a left join #APZ_PRG4DATA b on b."AbsEntry"=a."AbsEntry"
	left join OPRC c on c."PrcCode"=b."OcrCode3" where a."StartDate" not BETWEEN c."ValidFrom" AND c."ValidTo" ),0) into isExstStDate3 from  DUMMY;
	if :isExstStDate3=1
		then
		select IFNULL ((select top 1 b."OcrCode3" from #APZ_OPRGDATA  a left join #APZ_PRG4DATA b on b."AbsEntry"=a."AbsEntry"
		left join OPRC c on c."PrcCode"=b."OcrCode3" where a."StartDate" not BETWEEN c."ValidFrom" AND c."ValidTo" ),'') into ocrCode from dummy;
		select 135,'Start Date nằm ngoài khoảng Valid của Cost Center '|| :ocrCode from dummy;
		return;
		END if;		
	select IFNULL(( select top 1 1 from #APZ_OPRGDATA  a left join #APZ_PRG4DATA b on b."AbsEntry"=a."AbsEntry"
	left join OPRC c on c."PrcCode"=b."OcrCode4" where a."StartDate" not BETWEEN c."ValidFrom" AND c."ValidTo" ),0) into isExstStDate4 from  DUMMY;
	if :isExstStDate4=1
		then
		select IFNULL ((select top 1 b."OcrCode4" from #APZ_OPRGDATA  a left join #APZ_PRG4DATA b on b."AbsEntry"=a."AbsEntry"
		left join OPRC c on c."PrcCode"=b."OcrCode4" where a."StartDate" not BETWEEN c."ValidFrom" AND c."ValidTo" ),'') into ocrCode from dummy;
		select 135,'Start Date nằm ngoài khoảng Valid của Cost Center '|| :ocrCode from dummy;
		return;
		END if;		
			select IFNULL(( select top 1 1 from #APZ_OPRGDATA  a left join #APZ_PRG4DATA b on b."AbsEntry"=a."AbsEntry"
	left join OPRC c on c."PrcCode"=b."OcrCode5" where a."StartDate" not BETWEEN c."ValidFrom" AND c."ValidTo" ),0) into isExstStDate5 from  DUMMY;
	if :isExstStDate5=1
		then
		select IFNULL ((select top 1 b."OcrCode5" from #APZ_OPRGDATA  a left join #APZ_PRG4DATA b on b."AbsEntry"=a."AbsEntry"
		left join OPRC c on c."PrcCode"=b."OcrCode5" where a."StartDate" not BETWEEN c."ValidFrom" AND c."ValidTo" ),'') into ocrCode from dummy;
		select 135,'Start Date nằm ngoài khoảng Valid của Cost Center '|| :ocrCode from dummy;
		return;
		END if;			
	 UPDATE #APZ_PRG1DATA set "AbsEntry" = :Id;
	 UPDATE #APZ_PRG2DATA set "AbsEntry" = :Id ;
	 UPDATE #APZ_PRG3DATA set "AbsEntry" = :Id;
	 UPDATE #APZ_PRG4DATA set "AbsEntry" = :Id;
	 UPDATE #APZ_PRG6DATA set "AbsEntry" = :Id;
	 UPDATE #APZ_OPRG1DATA set "MasterEntry" = :Id;
	
	--update create date, update date
	update #APZ_OPRGDATA 
	set "UpdateDate" = current_timestamp,
	"UpdateTime" = cast(to_varchar(current_time,'HH24MI') as int);
	if :typeSave='A' then
		update #APZ_OPRGDATA 
		set "CreateDate" = current_timestamp,
		"CreateTime" = cast(to_varchar(current_time,'HH24MI') as int);
	end if;
	insert into APZ_OPRG select * from #APZ_OPRGDATA;
	insert into APZ_PRG1 select * from #APZ_PRG1DATA;
	insert into APZ_PRG2 select * from #APZ_PRG2DATA;
	insert into APZ_PRG3 select * from #APZ_PRG3DATA;
	insert into APZ_PRG4 select * from #APZ_PRG4DATA;
	insert into APZ_PRG6 select * from #APZ_PRG6DATA;
	
	select :Id,'' from Dummy;
	
end;

