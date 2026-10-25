
CALL APZ_EXISTSPROC('apz_formsetting_save');

GO

Create PROCEDURE apz_formsetting_save(
	in userSign nvarchar(50),
	in formId nvarchar(50),
	in itemId nvarchar(50)
)
AS
BEGIN
	
	declare tempSettingLocal table
	(
		"FormId" nvarchar(100),
		 "ItemId" nvarchar(50),
		 "ColumnId" nvarchar(50),
		 "UserSign" nvarchar(20),
		 "LngCode" nvarchar(50),
		 "Width" integer,
		 "Visible" tinyint,
		 "VisIndex" integer,
		 "Editable" tinyint,
		 "IsReadOnly" tinyint,
		 "ColumnName" nvarchar(254),
		 "Fixed" char(1),
		 "AllowChSet" tinyint,
		 "VisSetting" tinyint,
		 "LogInstanc" int,
		 "UserUpdate" int,
		 "UpdateDate" timestamp,
		 "GroupIndex" int,
		 "CurrField" nvarchar(50)
	);
	
	--Check exists setting for user. if not copy setting from manager to user
	if not exists(select 1 from APZ_FORMSETTING where "UserSign" = :userSign and "ItemId" = :itemId and "FormId" = :formId)
	then

		INSERT INTO APZ_FORMSETTING ("FormId", "ItemId", "ColumnId", "UserSign", "Width", "Visible", "VisIndex", "Editable", "IsReadOnly", "ColumnName", "Fixed", "AllowChSet", "VisSetting", "GroupIndex","CurrField") 
			SELECT "FormId", "ItemId", "ColumnId", :userSign, "Width", "Visible", "VisIndex", "Editable", "IsReadOnly", "ColumnName", "Fixed", "AllowChSet", "VisSetting", "GroupIndex","CurrField"
						FROM APZ_FORMSETTING WHERE "UserSign" = 'manager' AND "FormId" = :formId AND "ItemId" = :itemId;

	END IF;
	
	temp = select top 0 * from APZ_FORMSETTING;
	
	exec 'select * from "#tempSetting"' into temp;
	exec 'drop table "#tempSetting"';
	
	insert into :tempSettingLocal select * from :temp;
	
	-- xóa udf không tồn tại
	delete from APZ_FORMSETTING a where exists(select 1 from :tempSettingLocal b where b."UserSign"='-' and a."FormId"=b."FormId" and a."ItemId"=b."ItemId" and a."ColumnId"=b."ColumnId");
	delete from :tempSettingLocal where "UserSign"='-';
	
	--update setting visible, editable, readonly from database to temptable
	UPDATE a SET a."AllowChSet" = b."AllowChSet", a."VisSetting" = b."VisSetting" FROM :tempSettingLocal a 
									INNER JOIN APZ_FORMSETTING b ON a."FormId" = b."FormId" AND a."ItemId" = b."ItemId" AND a."ColumnId" = b."ColumnId" AND a."UserSign" = b."UserSign" AND a."VisSetting" = 1;
	
	--delete setting from database exists in temptable with vissetting = 1
	DELETE FROM APZ_FORMSETTING WHERE "UserSign" = :userSign AND "FormId" = :formId AND "ItemId" = :itemId AND "VisSetting" = 1;

	--delete setting from database exists in temptable
	DELETE FROM :tempSettingLocal where ("FormId","ItemId","ColumnId","UserSign") in (select "FormId","ItemId","ColumnId","UserSign" from APZ_FORMSETTING);

	INSERT INTO APZ_FORMSETTING select * from :tempSettingLocal;
	
END;