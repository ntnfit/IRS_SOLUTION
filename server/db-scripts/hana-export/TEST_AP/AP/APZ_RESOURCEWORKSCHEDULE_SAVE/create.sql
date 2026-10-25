
CREATE PROCEDURE APZ_ResourceWorkSchedule_Save(in tranType char(1))
AS
BEGIN

	declare checkCount int = 0;
	----------------------- Lấy Code của Lịch làm việc truyền lên ----------------------------------------------
	declare code nvarchar(50) = '';
	declare shift nvarchar(50) = '';
	
	CREATE LOCAL TEMPORARY TABLE #APZ_ORWS_TEMPDATA as (select top 0 * from APZ_ORWS);
	exec 'insert into #APZ_ORWS_TEMPDATA select * from "#APZ_ORWS_TEMP";';
	
	CREATE LOCAL TEMPORARY TABLE #APZ_RWS1_TEMPDATA as (select top 0 * from APZ_RWS1);
	exec 'insert into #APZ_RWS1_TEMPDATA select * from "#APZ_RWS1_TEMP";';
	
	select ifnull((select top 1 "Code" from #APZ_ORWS_TEMPDATA),'') into code FROM DUMMY;

	----------------------------------- Nếu Code Null báo lỗi --------------------------------------------------
	if :code = ''
	THEN

		select 800013, 'Schedule code is empty' FROM DUMMY;
		return;

	END IF;

	--------------------------------------------- Nếu là tạo mới và tồn tại code trong hệ thống thì báo lỗi -----------------------
	select IFNULL((select top 1 1 from APZ_ORWS where "Code" = :code),0) into checkCount FROM DUMMY;
	if :tranType = 'A' and :checkCount = 1
	THEN

		select 800014, 'Schedule "' || :code || ' exists. Can not save duplicate key' FROM DUMMY;
		return;

	END IF;

	------------------------------------ Nếu là update mà không tồn tại dữ liệu trên hệ thống thì báo lỗi --------------------------
	select IFNULL((select top 1 1 from APZ_ORWS where "Code" = :code),0) into checkCount FROM DUMMY;
	if :tranType = 'U' and :checkCount = 0
	THEN

		select 800015, 'Schedule "' || :code || ' does not exists' FROM DUMMY;
		return;

	END IF;

	------------------------------------- Kiểm tra dữ liệu Shift -------------------------------------------------------------------
	select IFNULL((select top 1 "Shift" from #APZ_RWS1_TEMPDATA x where ifnull(x."Shift", '') <> '' and not exists(select 1 from apz_OSHP where "Code" = x."Shift")),'') into shift FROM DUMMY;
	if IFNULL(:shift,'') <> ''
	THEN

		select 800016, 'Shift code "' || :shift || '" does not exists' FROM DUMMY;
		return;

	END IF;

	update #APZ_RWS1_TEMPDATA set "Code" = :code;

	delete from APZ_ORWS where "Code" = :code;
	delete from APZ_RWS1 a where exists(select 1 from #APZ_RWS1_TEMPData b where a."Code" = b."Code" and a."WorkDate" = b."WorkDate");

	insert into APZ_ORWS select * from #APZ_ORWS_TEMPData;
	insert into APZ_RWS1 select * from #APZ_RWS1_TEMPData;

	select :code, '' FROM DUMMY;
		
END



