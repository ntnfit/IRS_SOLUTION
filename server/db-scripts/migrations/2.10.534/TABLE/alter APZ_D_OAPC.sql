
alter table APZ_D_OAPC add("AppFirebaseName" nvarchar(100) NULL);

GO

alter table APZ_D_OAPC add("AppFirebaseKey" nvarchar(254) NULL);

GO

alter table APZ_D_OAPC add("AllowCheckInFarFromStore" nvarchar(5) NULL);

GO

alter table APZ_D_OAPC add("AllowUpdateStoreLocation" nvarchar(5) NULL);

GO

alter table APZ_D_OAPC add("Distance" numeric(19,6) NULL DEFAULT 1000);

GO

alter table APZ_D_OAPC add("UploadSecrecyApp" nvarchar(100) NULL);

GO

alter table APZ_D_OAPC add("AuthorizationMode" char(1) NULL DEFAULT 'R');

GO

alter table APZ_D_OAPC add("AllowUseGoogleApiPaid" nvarchar(5) null);

GO

alter table APZ_D_OAPC add("TrackingRequestInterval" numeric(19, 6) NULL default 5000);

GO

alter table APZ_D_OAPC add("TrackingEmployeeInterval" numeric(19, 6) NULL default 5000);

GO

alter table APZ_D_OAPC add("MarkerSize" numeric(19,6) NULL Default 30);

GO

alter table APZ_D_OAPC add("CurrentMarkerSize" numeric(19,6) NULL Default 38);

GO

alter table APZ_D_OAPC add("AllowUseMultipleGoogleApiKey" nvarchar(5) NULL);

GO

alter table APZ_D_OAPC add("GoogleApiKeyIos" nvarchar(500) NULL);

GO

alter table APZ_D_OAPC add("GoogleApiKeyWeb" nvarchar(500) NULL);

GO

alter table APZ_D_OAPC add("DistanceSaveTracking" numeric(19,6) NULL default 100);

GO

alter table APZ_D_OAPC add("AllowSelectPhotoVisit" nvarchar(5) NULL);

GO

alter table APZ_D_OAPC add("HideVanOrder" nvarchar(5) NULL);

GO

alter table APZ_D_OAPC add("DashboardUrl" nvarchar(500) NULL);

GO

alter table APZ_D_OAPC add("WarehouseInventoryType" nvarchar(5) NULL);

GO

alter table APZ_D_OAPC add("SaveDeliveryDraft" nvarchar(5) NULL);

