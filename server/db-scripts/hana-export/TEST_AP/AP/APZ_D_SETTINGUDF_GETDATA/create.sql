
--call APZ_D_SettingUDF_GetData(3,'OCRD','')
CREATE PROCEDURE APZ_D_SettingUDF_GetData
(
	in menuID int,
	IN tableName NVARCHAR(50),
	in type nvarchar(5)
)
AS
BEGIN
	if :type = 'APZ' then
		SELECT t0.*, 
		CASE WHEN t0."TypeID" = 'N' AND t0."EditType" = 'T' THEN 'D' 
		WHEN t0."TypeID" = 'A' AND t0."EditType" = 'I' THEN 'M' 
		WHEN t0."TypeID" = 'M' AND t0."EditType" = '' THEN 'A' 
		ELSE t0."TypeID" END AS "TypeID1", 
		CASE WHEN t0."TypeID" = 'M' AND t0."EditType" = '' THEN 'B' 
		ELSE t0."EditType" END AS "EditType1",
		ifnull(t1."IsActive", 'N') as "IsActive",
		ifnull(t1."IsEditable",'Y') as "IsEditable"
	    FROM  APZ_CUFD t0 left join APZ_D_SUDF t1
	    on t0."TableID" = t1."TableID" and t0."FieldID" = t1."FieldID" and t1."MenuID" = :menuID
	    WHERE t0."TableID" = :tableName ORDER BY t0."FieldID";
	    
	    -- table list cfl
	    SELECT "TableID", "FieldID", "IndexID", "FldValue", "Descr", "FldDate" 
				FROM APZ_UFD1 
				WHERE "TableID" = :tableName 
				ORDER BY "FieldID", "IndexID";
	else
		SELECT t0.*, 
		CASE WHEN t0."TypeID" = 'N' AND t0."EditType" = 'T' THEN 'D' 
		WHEN t0."TypeID" = 'A' AND t0."EditType" = 'I' THEN 'M' 
		WHEN t0."TypeID" = 'M' AND t0."EditType" = '' THEN 'A' 
		ELSE t0."TypeID" END AS "TypeID1", 
		CASE WHEN t0."TypeID" = 'M' AND t0."EditType" = '' THEN 'B' 
		ELSE t0."EditType" END AS "EditType1",
		ifnull(t1."IsActive", 'N') as "IsActive",
		ifnull(t1."IsEditable",'Y') as "IsEditable"
	    FROM  CUFD t0 left join APZ_D_SUDF t1
	    on t0."TableID" = t1."TableID" and t0."FieldID" = t1."FieldID" and t1."MenuID" = :menuID
	    WHERE t0."TableID" = :tableName ORDER BY t0."FieldID";
	    
	    SELECT "TableID", "FieldID", "IndexID", "FldValue", "Descr", "FldDate" 
				FROM UFD1 
				WHERE "TableID" = :tableName 
				ORDER BY "FieldID", "IndexID";	
	end if;
END	



