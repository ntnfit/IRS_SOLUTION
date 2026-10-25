
call APZ_EXISTSPROC('APZ_Process_AdditionalGeneralSettings');

GO

create procedure APZ_Process_AdditionalGeneralSettings(in tranType varchar(1) default '')
as
begin
	declare rCount Integer=0;
	declare checkDulp varchar(50) = '';
	
	/* Create temp table */
	create local temporary table #APZ_OADMLocal("DisableSplitFolder" varchar(1) null
												, "AutoSaveDraftApproval" varchar(1) null
												, "TaskAsgn" varchar(1) null
												, "DflLng" nvarchar(50) null
												, "CheckPromotion" char(1) null
												, "SearchDate" int null
												, "CheckOpenFormPR" char(1) null
												, "IngoreActiveEmp" char(1) null);
	exec 'drop table #APZ_OADMLocal;';
	exec 'create local temporary table #APZ_OADMLocal as (select * from "#APZ_OADM");';
	
	create local temporary table #APZ_ATC2Local("ObjType" nvarchar(50) null, "Path" nvarchar(255) null);	
	exec 'drop table #APZ_ATC2Local;';
	exec 'create local temporary table #APZ_ATC2Local as (select * from "#APZ_ATC2");';
	
	create local temporary table #APZ_D_OAPCLocal as (select top 0 * from APZ_D_OAPC);	
	exec 'drop table #APZ_D_OAPCLocal;';
	exec 'create local temporary table #APZ_D_OAPCLocal as (select * from "#APZ_D_OAPC");';
	
	UPDATE #APZ_OADMLocal SET "DisableSplitFolder"='N' WHERE IFNULL("DisableSplitFolder",'')='';
	UPDATE #APZ_OADMLocal SET "AutoSaveDraftApproval"='N' WHERE IFNULL("AutoSaveDraftApproval",'')='';
	UPDATE #APZ_OADMLocal SET "TaskAsgn"='U' WHERE IFNULL("TaskAsgn",'U')='';
	UPDATE #APZ_OADMLocal SET "DflLng"='ENG' WHERE IFNULL("DflLng",'')='';
	UPDATE #APZ_OADMLocal SET "SearchDate"=15 WHERE IFNULL("SearchDate",0) < 0;
	
	if :tranType <> 'A' then 
		select '01' as "ErrorCode", 'Invalid action type' as "ErrorMessage" from DUMMY;
		return;
	end if;
	
	select (select 1 from #APZ_ATC2Local where IFNULL("ObjType", '') = '') into rCount from DUMMY;
	if :rCount = 1 then
		select '02' as "ErrorCode", 'Object Type cannot be empty' as "ErrorMessage" from DUMMY;
		return;
	end if;
		
	select (select 1 from #APZ_ATC2Local where IFNULL("Path", '') = '') into rCount from DUMMY;
	if :rCount = 1 then
		select '03' as "ErrorCode", 'Path cannot be empty' as "ErrorMessage" from DUMMY;
		return;
	end if;
		
	select (select "ObjType" from #APZ_ATC2Local group by "ObjType" having count("ObjType") > 1) into checkDulp from DUMMY;
	if :checkDulp <> '' then
		select '04' as "ErrorCode", 'Object Type cannot be duplicated!' as "ErrorMessage" from DUMMY;
		return;
	end if;
	
	/* INSERT - UPDATE */
	if :tranType='A' then
		
		--Insert data to APZ_OADM 
		delete from APZ_OADM;
		insert into APZ_OADM("DisableSplitFolder","AutoSaveDraftApproval","TaskAsgn", "DflLng", "CheckPromotion", "SearchDate", "CheckOpenFormPR","IngoreActiveEmp") 
			select "DisableSplitFolder","AutoSaveDraftApproval","TaskAsgn", "DflLng", "CheckPromotion", "SearchDate","CheckOpenFormPR","IngoreActiveEmp" from #APZ_OADMLocal;
		
		--Insert data to APZ_ATC2
		delete from APZ_ATC2;
		insert into APZ_ATC2("ObjType","Path") select * from #APZ_ATC2Local;
		
		--Insert data to APZ_D_OAPC
		delete from APZ_D_OAPC;
		INSERT INTO APZ_D_OAPC ("AppName", "UploadServer", "DefaultAvatar", "Logo", "GoogleApiKey", "DefaultSeriesStore", "AllowCheckInFarFromStore", "AllowUpdateStoreLocation", "Distance", "UploadSecrecyApp"
								, "AppFirebaseName", "AppFirebaseKey", "AuthorizationMode", "AllowUseGoogleApiPaid", "TrackingRequestInterval", "TrackingEmployeeInterval", "DistanceSaveTracking"
								            ,"AllowUseMultipleGoogleApiKey","GoogleApiKeyIos","GoogleApiKeyWeb", "MarkerSize","CurrentMarkerSize","AllowSelectPhotoVisit","HideVanOrder",
											"DashboardUrl","WarehouseInventoryType","SaveDeliveryDraft","AllowSkipLocationCheckFinish") 
		SELECT "AppName", "UploadServer", "DefaultAvatar", "Logo", "GoogleApiKey", "DefaultSeriesStore", "AllowCheckInFarFromStore", "AllowUpdateStoreLocation", "Distance", "UploadSecrecyApp", 
				"AppFirebaseName", "AppFirebaseKey", "AuthorizationMode","AllowUseGoogleApiPaid", "TrackingRequestInterval", "TrackingEmployeeInterval", "DistanceSaveTracking"
				,"AllowUseMultipleGoogleApiKey","GoogleApiKeyIos","GoogleApiKeyWeb", "MarkerSize","CurrentMarkerSize","AllowSelectPhotoVisit","HideVanOrder",
				"DashboardUrl","WarehouseInventoryType","SaveDeliveryDraft","AllowSkipLocationCheckFinish"
		 FROM #APZ_D_OAPCLocal;
		
	end if;
end;