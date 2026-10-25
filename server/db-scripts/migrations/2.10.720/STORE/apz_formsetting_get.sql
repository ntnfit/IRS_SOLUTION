
CALL APZ_EXISTSPROC('apz_formsetting_get');

GO

CREATE PROCEDURE apz_formsetting_get
(
	 in FormId NVARCHAR(50),
	 in Itemid NVARCHAR(50),
	 in UserSign NVARCHAR(50),
	 in lngCode nvarchar(50)
)
AS
BEGIN
	declare currentLng nvarchar(50);
	declare tbl table
	(
		"FormId" NVARCHAR(50) ,
		"ItemId" NVARCHAR(50),
		"ColumnId" NVARCHAR(50) ,
		"UserSign" NVARCHAR(20),
		"Width" INT,
		"Visible" tinyint,
		"VisIndex" INT ,
		"Editable" tinyint,
		"IsReadOnly" tinyint,
		"DataType" NVARCHAR(100),
		"ColumnName" NVARCHAR(200),
		"Fixed" CHAR(1),
		"AllowChSet" tinyint,
		"VisSetting" tinyint,
		"GroupIndex" int,
		"CurrField" NVARCHAR(50)
	);
	
	select IFNULL(:UserSign,'manager') into UserSign from DUMMY;
	select IFNULL(:lngCode,'-') into lngCode from DUMMY;
	currentLng = :lngCode;
	if :UserSign <> '-'
	THEN
		 currentLng = '-';
	END IF;
	
	-- Language
	if :formId = 'oMain' and :itemId = 'oMain' then
		select "ColumnId", "ColumnName" from APZ_FORMSETTING where "FormId"='oMain' and "ItemId"='oMain' and "UserSign"='-' and "LngCode" = :lngCode;
		return;
	end if;
		
	if not exists(select 1 from apz_formsetting where "FormId" = :FormId and "ItemId" = :Itemid and "UserSign" = :UserSign)
	then
	 	UserSign = 'manager';
	end if;
	
	INSERT INTO :tbl ("FormId", "ItemId", "ColumnId", "UserSign", "Width", "Visible", "VisIndex", "Editable", "IsReadOnly", "DataType", "ColumnName", "Fixed", "AllowChSet", "VisSetting", "GroupIndex", "CurrField") 
		SELECT "FormId", a."ItemId", a."ColumnId", "UserSign", "Width", "Visible", "VisIndex", "Editable", "IsReadOnly", null, a."ColumnName", "Fixed", a."AllowChSet", a."VisSetting", a."GroupIndex",a."CurrField" 
		FROM apz_formsetting a
		WHERE "FormId"=:FormId AND a."ItemId"=:Itemid and "UserSign"=:UserSign and a."LngCode" = :currentLng;
	
	if not exists(select 1 from :tbl)
	THEN
		INSERT INTO :tbl ("FormId", "ItemId", "ColumnId", "UserSign", "Width", "Visible", "VisIndex", "Editable", "IsReadOnly", "DataType", "ColumnName", "Fixed", "AllowChSet", "VisSetting", "GroupIndex", "CurrField") 
		SELECT "FormId", a."ItemId", a."ColumnId", "UserSign", "Width", "Visible", "VisIndex", "Editable", "IsReadOnly", null, a."ColumnName", "Fixed", a."AllowChSet", a."VisSetting", a."GroupIndex", a."CurrField" 
		FROM apz_formsetting a
		WHERE "FormId" = :FormId AND a."ItemId" = :Itemid and "UserSign" = '-' AND  a."LngCode" = :lngCode;
	END IF;
		
	if not exists(select 1 from :tbl)
	then

		if not exists(select 1 from OUDO WHERE "Code" = :FormId)
		then
							 
			INSERT INTO :tbl ("FormId", "ItemId", "ColumnId", "UserSign", "Width", "Visible", "Editable", "IsReadOnly", "DataType", "ColumnName", "AllowChSet", "VisSetting", "GroupIndex")
				SELECT :FormId, TABLE_NAME, COLUMN_NAME, '',100,0,1,0, a.DATA_TYPE_NAME
					, CASE WHEN EXISTS (SELECT 1 FROM CUFD WHERE a.TABLE_NAME = "TableID" AND a.COLUMN_NAME = CAST('U_' || "AliasID" AS nvarchar)) 
						THEN (SELECT "Descr" FROM CUFD WHERE a.TABLE_NAME = "TableID" AND a.COLUMN_NAME = CAST('U_' || "AliasID" AS nvarchar)) 
					  ELSE (SELECT "Descr" FROM APZ_CUFD WHERE a.TABLE_NAME = "TableID" AND a.COLUMN_NAME = CAST('U_' || "AliasID" AS nvarchar)) END, 1, 1, -1
				FROM TABLE_COLUMNS a WHERE SCHEMA_NAME = current_schema and LEFT(COLUMN_NAME, 2) = 'U_' AND a.TABLE_NAME = :Itemid 
					AND NOT EXISTS (SELECT 1 FROM :tbl WHERE "FormId" = :FormId AND "ItemId" = a.TABLE_NAME AND "ColumnId" = a.COLUMN_NAME) 
						AND ((COLUMN_NAME NOT IN ('U_ItemCode','U_ItemName','U_FrgnName','U_ItemCode','U_Quantity','U_IPercent','U_Price','U_PotentAmount','U_DeliveryDate','U_OpportId','U_line','U_AttFile') 
						AND :Itemid = '@OPR62') OR :Itemid <> '@OPR62');
		
		END if;
	
	END if;
	
	--UPDATE :tbl SET "DataType" = (SELECT "DATA_TYPE_NAME" FROM TABLE_COLUMNS WHERE schema_NAME = current_schema and TABLE_NAME = :Itemid AND COLUMN_NAME = "ColumnId") WHERE "DataType" IS NULL;

	
	UPDATE :tbl SET "ColumnName" = CASE WHEN EXISTS (SELECT 1 FROM CUFD WHERE "ItemId" = "TableID" AND "ColumnId" = CAST('U_' || "AliasID" AS nvarchar)) 
						THEN (SELECT "Descr" FROM CUFD WHERE "ItemId" = "TableID" AND "ColumnId" = CAST('U_' || "AliasID" AS nvarchar)) 
						ELSE (SELECT "Descr" FROM APZ_CUFD WHERE "ItemId" = "TableID" AND "ColumnId" = CAST('U_' || "AliasID" AS nvarchar)) END WHERE "ColumnName" IS NULL AND LEFT("ColumnId", 2) = 'U_';
	
	UPDATE :tbl SET "VisIndex" =IFNULL(( SELECT MAX("VisIndex") FROM :tbl),0) WHERE "VisIndex" IS null;
	
	if :Itemid <> '@OPR62' then
		SELECT * FROM :tbl ORDER BY "Visible" DESC, "VisIndex" ASC;
	else
		SELECT * FROM :tbl
		WHERE "ColumnId" NOT IN ('U_ItemCode','U_ItemName','U_FrgnName','U_ItemCode','U_Quantity','U_IPercent','U_Price','U_PotentAmount','U_DeliveryDate','U_OpportId','U_line','U_AttFile')
		ORDER BY "Visible" DESC, "VisIndex" ASC;
	end if;
			
end