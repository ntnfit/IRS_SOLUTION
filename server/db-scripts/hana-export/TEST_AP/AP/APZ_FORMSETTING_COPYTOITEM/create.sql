
CREATE PROCEDURE apz_formsetting_copyToItem(in settingId int)
AS
BEGIN
	declare settingName nvarchar(254);
	declare formId nvarchar(254);
	declare userName nvarchar(254);
	declare toSettingId int;
		
	if not exists(select 1 from APZ_OFST where ID = :settingId)
	then
		return;
	END if;
		
	select "Name", "FormId", "UserSign" into settingName,formId,userName from APZ_OFST where ID = :settingId;
	if :formId = 'oSalaryResult'
	THEN
		formId = 'oSalaryResultView';
	else
		formId = 'oSalaryResult';
	END IF;

	if exists(select 1 from APZ_OFST where "UserSign" = :userName and "FormId" = :formId and "Name" = :settingName)
	THEN

		select top 1 ID into toSettingId from APZ_OFST where "UserSign" = :userName and "FormId" = :formId and "Name" = :settingName;
		delete from APZ_FST1 where ID = :toSettingId;

	else

		select ifnull((select max(ID) from APZ_OFST), 0) + 1 into toSettingId from DUMMY;
		
		insert into APZ_OFST(ID, "Name", "FormId", "ItemId", "SetDefault", "UserSign", "CreateDate")
			values (:toSettingId, :settingName, :formId, 'APZ_OSAL', 0, :userName, current_date);

	END IF;

	create local temporary table #tempSetting as (select top 0 * from APZ_FST1);
	insert into #tempSetting select * from APZ_FST1 where ID = :settingId;
	update #tempSetting set ID = :toSettingId;
	insert into APZ_FST1 select * from #tempSetting;
    
END




