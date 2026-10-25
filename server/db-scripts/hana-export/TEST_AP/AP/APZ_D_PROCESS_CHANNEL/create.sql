

CREATE PROCEDURE APZ_D_Process_Channel(in actionType char(1))
AS
BEGIN

	declare allItem char(1) = 'N';

	create local temporary table #APZ_D_OCHNLocal as (select top 0 * from APZ_D_OCHN);
	exec 'drop table #APZ_D_OCHNLocal';
	exec 'create local temporary table #APZ_D_OCHNLocal as (select * from "#APZ_D_OCHN")';
	
	create local temporary table #APZ_D_CHN1Local as (select top 0 * from APZ_D_CHN1);
	exec 'drop table #APZ_D_CHN1Local';
	exec 'create local temporary table #APZ_D_CHN1Local as (select * from "#APZ_D_CHN1");';

	--Update default value
	if :actionType = 'A'
	then
		update #APZ_D_OCHNLocal set "UserSign" = 1, "Loginstanc" = NULL, "CreateDate" = current_date, "CreateTime" = cast((select GetTimeInteger(current_time) from dummy) as int), "UpdateDate" = null, "UpdateTime" = null;
		update #APZ_D_CHN1Local set "CreateDate" = current_date, "CreateTime" = cast((select GetTimeInteger(current_time) from dummy) as int), "UpdateDate" = null, "UpdateTime" = null;
	else
		update #APZ_D_OCHNLocal set "UserSign" = 1, "Loginstanc" = NULL, "UpdateDate" = current_date, "UpdateTime" = cast((select GetTimeInteger(current_time) from dummy) as int);
		update #APZ_D_CHN1Local set "UpdateDate" = current_date, "UpdateTime" = cast((select GetTimeInteger(current_time) from dummy) as int);
	end if;
	
	update #APZ_D_CHN1Local set "ChannelCode" = (select top 1 "ChannelCode" from #APZ_D_OCHNLocal);
	
	--Kiểm tra channel code rỗng
	if exists(select 1 from #APZ_D_OCHNLocal where ifnull("ChannelCode", '') = '')
	then
		select '01' as "ErrorCode", 'Channel code is empty value' as "ErrorMessage" from dummy;
		return;
	end if;

	--Kiểm tra channel name rỗng
	if exists(select 1 from #APZ_D_OCHNLocal where "ChannelName" = '')
	then
		select '02' as "ErrorCode", 'Channel Name is empty value' as "ErrorMessage" from dummy;
		return;
	end if;

	--Kiểm tra các channel có check All Item
	if exists(select 1 from #APZ_D_OCHNLocal where "AllItem" = 'Y') or exists(select 1 from APZ_D_OCHN where "AllItem" = 'Y')
	then
		allItem := 'Y';
		--Cập nhật channel check All Item 
		update APZ_D_OCHN set "AllItem" = 'N' where "ChannelCode" not in(select top 1 "ChannelCode" from #APZ_D_OCHNLocal);
		--Khi check all item thì xóa các item đã có trong danh sách
		delete from APZ_D_CHN1 where ifnull("ItemCode", '') <> ''; 
	else
		--Kiểm tra các item khi chưa thêm vào danh sách
		if exists(select 1 from #APZ_D_OCHNLocal where "AllItem" = 'N') and (select count(1) from #APZ_D_CHN1Local) = 0
		then
			select '03' as "ErrorCode", 'Item is empty' as "ErrorMessage" from dummy;
			return;
		end if;
		--Kiểm tra item code rỗng
		if exists (select 1 from #APZ_D_CHN1Local where ifnull("ItemCode", '') = '') and exists (select 1 from APZ_D_OCHN where "AllItem" = 'N')
		then
			select '04' as "ErrorCode", 'Item Code is empty value' as "ErrorMessage" from dummy;
			return;
		end if;

		--Kiểm tra trùng Item Code
		if exists (select 1 from #APZ_D_CHN1Local group by "ItemCode" having count(1) > 1)
		then
			select '06' as "ErrorCode", 'This item already exists' as "ErrorMessage" from dummy;
			return;
		end if;
	end if;

	--Kiểm tra trùng Channel
	if exists(select 1 from #APZ_D_OCHNLocal a inner join APZ_D_OCHN b on a."ChannelCode" = b."ChannelCode") and :actionType = 'A'
	then
		select '05' as "ErrorCode", 'This channel already exists' as "ErrorMessage" from dummy;
		return;
	end if;

	--Kiểm tra channel này đã tồn tại
	if  exists(select 1 from #APZ_D_CHN1Local a inner join APZ_D_CHN1 b on a."ChannelCode" != b."ChannelCode" and a."ItemCode" = b."ItemCode") 
	then
		select '07' as "ErrorCode", 'This item already exist in another channel' as "ErrorMessage" from dummy;
		return;
	end if;
	
	if :actionType = 'U'
	then
		if exists(select 1 from #APZ_D_OCHNLocal where "AllItem" = 'Y')
		then
			delete from APZ_D_CHN1 where "ChannelCode" in (select top 1 "ChannelCode" from #APZ_D_OCHNLocal);
		else
			delete from APZ_D_CHN1 where "ChannelCode" in (select a."ChannelCode" from APZ_D_CHN1 a inner join #APZ_D_CHN1Local b on a."ChannelCode" = b."ChannelCode" and a."ItemCode" = b."ItemCode");
		end if;
		delete from APZ_D_OCHN where "ChannelCode" in (select a."ChannelCode" from APZ_D_OCHN a inner join #APZ_D_OCHNLocal b on a."ChannelCode" = b."ChannelCode");
	end if;

	insert into APZ_D_OCHN("ChannelCode", "ChannelName", "Status", "AllItem", "Remarks", "UserSign", "Loginstanc", "CreateDate", "CreateTime", "UpdateDate", "UpdateTime") 
	select "ChannelCode", "ChannelName", "Status", "AllItem", "Remarks", "UserSign", "Loginstanc", "CreateDate", "CreateTime", "UpdateDate", "UpdateTime" from #APZ_D_OCHNLocal;
	
	if :allItem = 'N'
	then
		insert into APZ_D_CHN1("ItemCode", "ChannelCode", "Status", "Active", "Remarks", "CreateDate", "CreateTime", "UpdateDate", "UpdateTime") 
		select "ItemCode", "ChannelCode", "Status", "Active", "Remarks", "CreateDate", "CreateTime", "UpdateDate", "UpdateTime" from #APZ_D_CHN1Local;
	end if;
	
END



