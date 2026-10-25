
CREATE PROCEDURE APZ_D_NotificationEvent_Update(in typeName nvarchar(100) default 'update')
AS
BEGIN
	create local temporary table #tempTabledata as (select top 0 *, '' "EventName", '' "AppUse" from APZ_D_EVENTAPPCONFIG);
	exec 'drop table #tempTabledata';
	exec 'create local temporary table #tempTabledata as (select * from "#tempTable");';

	IF :typeName = 'add'
	THEN
		--#region add
		IF exists (select 1 from APZ_D_EVENTCONFIG where "EventCode" in (select "EventCode" from #tempTableData))
		THEN
			select 'Duplicate EventCode' as Message from DUMMY;
			return;
		END IF;
		
		insert into APZ_D_EVENTCONFIG("EventCode","EventName","AppUse") select top 1 "EventCode","EventName","AppUse" from #tempTabledata;
		insert into APZ_D_EVENTAPPCONFIG("EventCode","AppName","Message","Title","Active") select top 1 "EventCode","AppName","Message","Title","Active" from #tempTabledata;
	
		select '' as "Message" from DUMMY;
		--#endregion
	else
		BEGIN
			declare listAppName table("AppName" nvarchar(100));
			declare oldEventCode nvarchar(100);
			declare tempAppuse nvarchar(5000);
			declare updateID int;	
			
			select ifnull((select top 1 "AppUse" from APZ_D_EVENTCONFIG where ID = (select top 1 ID from #tempTableData)), '') into tempAppuse from dummy;
			
			insert into :listAppName SELECT RTRIM(LTRIM(Lower("part"))) as "AppName" FROM SplitString(:tempAppuse,',') x;
			
			select top 1 "ID" into updateID from #tempTabledata
			where Lower("AppName") in (select Lower("AppName") from :listAppName);
			
			select top 1 "EventCode" into oldEventCode from APZ_D_EVENTCONFIG where ID = :updateID;
			
			--#region update
			-- update bảng cha
			
			update a set a."EventCode" = b."EventCode",
						a."EventName" = b."EventName",
						a."AppUse" = case when Lower(b."AppName") in (select Lower("AppName") from :listAppName) then a."AppUse" else
						a."AppUse" || ',' || b."AppName" end
			from APZ_D_EVENTCONFIG a 
					inner join #tempTableData b on a.ID = b.ID;
			
			-- update bảng con
			IF ifnull(:oldEventCode,'') = ''
			THEN
				insert into APZ_D_EVENTAPPCONFIG("EventCode","AppName","Message","Title","Active") select top 1 "EventCode","AppName","Message","Title","Active" from #tempTabledata;
			ELSE
				update a set a."EventCode" = (select top 1 "EventCode" from #tempTabledata),
								a."Active" = (select top 1 "Active" from #tempTabledata),
								a."AppName" = (select top 1 "AppName" from #tempTabledata),
								a."Message" = (select top 1 "Message" from #tempTabledata),
								a."Title" = (select top 1 "Title" from #tempTabledata)
				from APZ_D_EVENTAPPCONFIG a 
					where a."EventCode" = :oldEventCode and Lower(a."AppName") = Lower((select top 1 "AppName" from #tempTabledata));
				
			END IF;
			
			select '' as "Message" from dummy;
		END;
		--#endregion
	END IF;
END



